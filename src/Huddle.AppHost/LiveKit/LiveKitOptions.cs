namespace Huddle.AppHost.LiveKit;

/// <summary>
/// Development settings for the LiveKit container, bound from the <c>LiveKit</c> configuration
/// section (appsettings.Development.json, user secrets, <c>LiveKit__*</c> environment variables
/// or launchSettings.json).
/// </summary>
internal sealed class LiveKitOptions
{
    public const string SectionName = "LiveKit";

    /// <summary>
    /// Public address of the developer machine that WebRTC clients should connect to, usually the
    /// router's external IP with port forwarding configured for the RTC ports.
    /// Detected automatically when it is not set.
    /// </summary>
    public string? NodeIp { get; set; }

    /// <summary>Resolve <see cref="NodeIp"/> from <see cref="NodeIpEchoUrl"/> when it is not configured.</summary>
    public bool AutoDetectNodeIp { get; set; } = true;

    /// <summary>Endpoint that echoes back the caller's public IP address. Detection is skipped when the URL is empty.</summary>
    public string? NodeIpEchoUrl { get; set; } = "https://api.ipify.org";
}
