using System.Data.Common;

namespace Huddle.FileService.Options;

/// <summary>
/// The Aspire MinIO integration exposes `Endpoint=...;AccessKey=...;SecretKey=...`,
/// while a plain endpoint URL is also accepted for standalone runs.
/// </summary>
public record MinioConnection(string? Endpoint, string? AccessKey, string? SecretKey)
{
    public static MinioConnection Empty { get; } = new(null, null, null);

    public static MinioConnection Parse(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return Empty;

        connectionString = connectionString.Trim();

        if (!connectionString.Contains('='))
            return new MinioConnection(connectionString, null, null);

        var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

        string? Get(string key) =>
            builder.TryGetValue(key, out var value) ? value?.ToString() : null;

        return new MinioConnection(Get("Endpoint"), Get("AccessKey"), Get("SecretKey"));
    }
}
