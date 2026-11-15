using Huddle.FileService.Contracts;
using Huddle.FileService.Data;
using Huddle.FileService.Extensions;
using Huddle.FileService.Options;
using Huddle.FileService.Storage;
using Huddle.FileService.Validation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Huddle.FileService.Endpoints;

public static class FileEndpoints
{
    public const string InternalKeyHeaderName = "X-Internal-Key";

    public static IEndpointRouteBuilder MapFileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/files");

        group.MapPost("", UploadAsync)
            .RequireAuthorization()
            .DisableAntiforgery();

        group.MapGet("{id:guid}/content", GetContentAsync);

        group.MapGet("{id:guid}/url", GetPresignedUrlAsync)
            .RequireAuthorization();

        group.MapDelete("{id:guid}", DeleteAsync)
            .RequireAuthorization();

        group.MapPost("claim", ClaimAsync);

        return app;
    }

    private static async Task<IResult> UploadAsync(
        HttpContext httpContext,
        FileDbContext db,
        IFileStorage storage,
        FileUploadValidator validator,
        IOptions<StorageOptions> options,
        CancellationToken cancellationToken)
    {
        var ownerId = httpContext.User.GetCurrentUserIdentityId();
        if (ownerId is null)
            return Results.Unauthorized();

        if (!TryParseKind(httpContext.Request.Query["kind"], out var kind))
            return Results.BadRequest("Unknown file kind, expected 'attachment' or 'avatar'");

        if (!httpContext.Request.HasFormContentType)
            return Results.BadRequest("Multipart form data is expected");

        var form = await httpContext.Request.ReadFormAsync(cancellationToken);
        var file = form.Files.GetFile("file") ?? form.Files.FirstOrDefault();
        if (file is null)
            return Results.BadRequest("No file was provided");

        var validation = validator.Validate(kind, file.FileName, file.ContentType, file.Length);
        if (!validation.IsValid)
            return Results.Problem(validation.Error, statusCode: validation.StatusCode);

        FileContentTypes.TryGetExtension(file.ContentType, out var extension);

        var settings = options.Value;
        var fileId = Guid.NewGuid();
        var fileName = FileUploadValidator.SanitizeFileName(file.FileName);
        var bucket = settings.GetBucket(kind);
        var objectKey = $"{ownerId:N}/{DateTime.UtcNow:yyyy}/{DateTime.UtcNow:MM}/{fileId:N}{extension}";

        await using (var content = file.OpenReadStream())
        {
            await storage.UploadAsync(bucket, objectKey, content, file.Length, file.ContentType, cancellationToken);
        }

        var storedFile = new StoredFile(fileId, ownerId.Value, kind, bucket, objectKey, fileName, file.ContentType, file.Length);
        db.Files.Add(storedFile);
        await db.SaveChangesAsync(cancellationToken);

        var dto = MapToDto(storedFile, settings);

        return Results.Created(dto.Url, dto);
    }

    private static async Task<IResult> GetContentAsync(
        Guid id,
        HttpContext httpContext,
        FileDbContext db,
        IFileStorage storage,
        CancellationToken cancellationToken)
    {
        var file = await db.Files.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        if (file is null)
            return Results.NotFound();

        var asDownload = httpContext.Request.Query.ContainsKey("download");

        // The MinIO SDK copies the object in a synchronous callback, which Kestrel blocks by default.
        httpContext.Features.Get<IHttpBodyControlFeature>()!.AllowSynchronousIO = true;

        httpContext.Response.ContentType = file.ContentType;
        httpContext.Response.ContentLength = file.Size;
        httpContext.Response.Headers.CacheControl = "public,max-age=86400";
        httpContext.Response.Headers.XContentTypeOptions = "nosniff";

        if (asDownload)
        {
            httpContext.Response.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileNameStar = file.FileName
                }.ToString();
        }

        await storage.WriteToAsync(file.Bucket, file.ObjectKey, httpContext.Response.Body, cancellationToken);

        return Results.Empty;
    }

    private static async Task<IResult> GetPresignedUrlAsync(
        Guid id,
        HttpContext httpContext,
        FileDbContext db,
        IFileStorage storage,
        IOptions<StorageOptions> options,
        CancellationToken cancellationToken)
    {
        var ownerId = httpContext.User.GetCurrentUserIdentityId();
        if (ownerId is null)
            return Results.Unauthorized();

        var file = await db.Files.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        if (file is null)
            return Results.NotFound();

        if (file.OwnerId != ownerId.Value)
            return Results.Forbid();

        var lifetime = TimeSpan.FromMinutes(options.Value.PresignedUrlLifetimeMinutes);
        var url = await storage.CreatePresignedDownloadUrlAsync(file.Bucket, file.ObjectKey, lifetime, file.FileName);

        return Results.Ok(new PresignedUrlDto(url, DateTime.UtcNow.Add(lifetime)));
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        HttpContext httpContext,
        FileDbContext db,
        IFileStorage storage,
        CancellationToken cancellationToken)
    {
        var ownerId = httpContext.User.GetCurrentUserIdentityId();
        if (ownerId is null)
            return Results.Unauthorized();

        var file = await db.Files.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        if (file is null)
            return Results.NotFound();

        if (file.OwnerId != ownerId.Value)
            return Results.Forbid();

        await storage.DeleteAsync(file.Bucket, file.ObjectKey, cancellationToken);

        db.Files.Remove(file);
        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> ClaimAsync(
        ClaimFilesRequest request,
        HttpContext httpContext,
        FileDbContext db,
        IOptions<StorageOptions> options,
        CancellationToken cancellationToken)
    {
        if (!IsInternalCaller(httpContext, options.Value))
            return Results.Unauthorized();

        if (request.Ids.Count == 0 || request.Ids.Count > options.Value.MaxAttachmentsPerMessage)
            return Results.BadRequest($"Between 1 and {options.Value.MaxAttachmentsPerMessage} files can be claimed at once");

        var uniqueIds = request.Ids.Distinct().ToList();

        var files = await db.Files
            .Where(f => uniqueIds.Contains(f.Id) && f.OwnerId == request.OwnerId && f.Kind == request.Kind)
            .ToListAsync(cancellationToken);

        foreach (var file in files)
            file.MarkAttached();

        await db.SaveChangesAsync(cancellationToken);

        var claimed = files
            .Select(f => new ClaimedFileDto(f.Id, f.FileName, f.ContentType, f.Size, FileContentTypes.IsImage(f.ContentType)))
            .ToList();

        var missing = uniqueIds.Except(claimed.Select(c => c.Id)).ToList();

        return Results.Ok(new ClaimFilesResponse(claimed, missing));
    }

    private static FileDto MapToDto(StoredFile file, StorageOptions options) =>
        new(
            file.Id,
            options.BuildPublicUrl(file.Id),
            file.FileName,
            file.ContentType,
            file.Size,
            FileContentTypes.IsImage(file.ContentType));

    private static bool TryParseKind(string? value, out FileKind kind)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            kind = FileKind.Attachment;
            return true;
        }

        return Enum.TryParse(value, ignoreCase: true, out kind) && Enum.IsDefined(kind);
    }

    private static bool IsInternalCaller(HttpContext httpContext, StorageOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.InternalApiKey))
            return false;

        var provided = httpContext.Request.Headers[InternalKeyHeaderName].ToString();

        return !string.IsNullOrEmpty(provided)
            && string.Equals(provided, options.InternalApiKey, StringComparison.Ordinal);
    }
}
