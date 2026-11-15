using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace Huddle.AppHost;

/// <summary>
/// Secrets that the AppHost shares between the containers and the services during development.
/// </summary>
/// <remarks>
/// Values are generated on every run, so no credentials are stored in the repository or on the
/// developer's machine. Set the <c>LiveKit:ApiKey</c>, <c>LiveKit:ApiSecret</c> or
/// <c>Files:InternalApiKey</c> settings when a value has to outlive an AppHost restart, for example
/// when a service is started outside of Aspire or when the LiveKit connection test page is used.
/// </remarks>
internal static class DevSecret
{
    /// <summary>Secret with 256 bits of entropy, limited to hexadecimal characters.</summary>
    public static string New() => Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

    /// <summary>API key in the shape LiveKit itself generates: a prefix followed by 12 characters.</summary>
    public static string NewLiveKitApiKey() => $"API{Convert.ToHexString(RandomNumberGenerator.GetBytes(6))}";

    /// <summary>Gets the configured value or falls back to a generated one. Empty values count as not configured.</summary>
    public static string Resolve(IConfiguration configuration, string key, string generated) =>
        configuration[key] is { Length: > 0 } configured ? configured : generated;
}
