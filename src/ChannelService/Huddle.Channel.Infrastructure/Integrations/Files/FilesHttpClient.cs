using System.Net.Http.Json;
using Huddle.Channel.Application.Exceptions;
using Huddle.Channel.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Huddle.Channel.Infrastructure.Integrations.Files
{
    public class FilesHttpClient : IFilesClient
    {
        public const string InternalKeyHeaderName = "X-Internal-Key";

        private readonly HttpClient _httpClient;
        private readonly string? _internalApiKey;

        public FilesHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _internalApiKey = configuration["Files:InternalApiKey"]
                ?? Environment.GetEnvironmentVariable("FILES_INTERNAL_API_KEY");
        }

        public Task<IReadOnlyList<ClaimedFile>> ClaimAttachmentsAsync(Guid ownerId, IReadOnlyList<Guid> fileIds, CancellationToken cancellationToken = default) =>
            ClaimAsync(ownerId, "attachment", fileIds, cancellationToken);

        public async Task<ClaimedFile> ClaimAvatarAsync(Guid ownerId, Guid fileId, CancellationToken cancellationToken = default)
        {
            var files = await ClaimAsync(ownerId, "avatar", [fileId], cancellationToken);

            return files[0];
        }

        private async Task<IReadOnlyList<ClaimedFile>> ClaimAsync(Guid ownerId, string kind, IReadOnlyList<Guid> fileIds, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/files/claim")
            {
                Content = JsonContent.Create(new ClaimFilesRequest(ownerId, kind, fileIds))
            };

            if (!string.IsNullOrEmpty(_internalApiKey))
                request.Headers.Add(InternalKeyHeaderName, _internalApiKey);

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new FileReferenceException($"Files service rejected the request with status {(int)response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<ClaimFilesResponse>(cancellationToken: cancellationToken)
                ?? throw new FileReferenceException("Files service returned an empty response");

            if (result.MissingIds.Count > 0)
                throw new FileReferenceException($"Files [{string.Join(", ", result.MissingIds)}] were not found");

            return result.Files
                .Select(file => new ClaimedFile(file.Id, file.FileName, file.ContentType, file.Size, file.IsImage))
                .ToList();
        }

        private record ClaimFilesRequest(Guid OwnerId, string Kind, IReadOnlyList<Guid> Ids);

        private record ClaimedFileResponse(Guid Id, string FileName, string ContentType, long Size, bool IsImage);

        private record ClaimFilesResponse(IReadOnlyList<ClaimedFileResponse> Files, IReadOnlyList<Guid> MissingIds);
    }
}
