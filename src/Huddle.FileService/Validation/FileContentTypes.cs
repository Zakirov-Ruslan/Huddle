using Huddle.FileService.Data;

namespace Huddle.FileService.Validation;

public static class FileContentTypes
{
    private static readonly Dictionary<string, string> ExtensionByContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/png"] = ".png",
        ["image/jpeg"] = ".jpg",
        ["image/gif"] = ".gif",
        ["image/webp"] = ".webp",
        ["application/pdf"] = ".pdf",
        ["text/plain"] = ".txt",
        ["text/csv"] = ".csv",
        ["application/zip"] = ".zip",
        ["application/json"] = ".json",
        ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"] = ".docx",
        ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"] = ".xlsx",
    };

    public static IReadOnlyCollection<string> AllowedContentTypes { get; } = ExtensionByContentType.Keys.ToArray();

    public static bool IsImage(string contentType) =>
        contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

    public static bool TryGetExtension(string contentType, out string extension) =>
        ExtensionByContentType.TryGetValue(contentType, out extension!);

    public static bool IsAllowed(FileKind kind, string contentType)
    {
        if (!TryGetExtension(contentType, out _))
            return false;

        return kind != FileKind.Avatar || IsImage(contentType);
    }
}
