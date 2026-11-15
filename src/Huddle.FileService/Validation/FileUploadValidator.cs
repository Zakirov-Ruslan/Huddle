using Huddle.FileService.Data;
using Huddle.FileService.Options;
using Microsoft.Extensions.Options;

namespace Huddle.FileService.Validation;

public record FileValidationResult(bool IsValid, int StatusCode, string? Error)
{
    public static FileValidationResult Ok { get; } = new(true, StatusCodes.Status200OK, null);

    public static FileValidationResult BadRequest(string error) =>
        new(false, StatusCodes.Status400BadRequest, error);

    public static FileValidationResult PayloadTooLarge(string error) =>
        new(false, StatusCodes.Status413PayloadTooLarge, error);

    public static FileValidationResult UnsupportedMediaType(string error) =>
        new(false, StatusCodes.Status415UnsupportedMediaType, error);
}

public class FileUploadValidator
{
    private readonly StorageOptions _options;

    public FileUploadValidator(IOptions<StorageOptions> options)
    {
        _options = options.Value;
    }

    public FileValidationResult Validate(FileKind kind, string? fileName, string? contentType, long size)
    {
        if (size <= 0)
            return FileValidationResult.BadRequest("File is empty");

        if (size > _options.GetMaxSize(kind))
        {
            var limitMb = _options.GetMaxSize(kind) / (1024 * 1024);
            return FileValidationResult.PayloadTooLarge($"File exceeds the {limitMb} MB limit");
        }

        if (string.IsNullOrWhiteSpace(fileName))
            return FileValidationResult.BadRequest("File name is required");

        if (string.IsNullOrWhiteSpace(contentType) || !FileContentTypes.IsAllowed(kind, contentType))
            return FileValidationResult.UnsupportedMediaType($"Content type '{contentType}' is not allowed");

        return FileValidationResult.Ok;
    }

    public static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName).Trim();
        if (name.Length == 0)
            return "file";

        return name.Length > 200 ? name[^200..] : name;
    }
}
