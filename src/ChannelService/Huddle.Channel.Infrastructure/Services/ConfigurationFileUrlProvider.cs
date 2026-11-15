using Huddle.Channel.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Huddle.Channel.Infrastructure.Services;

public class ConfigurationFileUrlProvider : IFileUrlProvider
{
    private const string DefaultBasePath = "/files/api/files";

    private readonly string _basePath;

    public ConfigurationFileUrlProvider(IConfiguration configuration)
    {
        _basePath = (configuration["Files:PublicBasePath"] ?? DefaultBasePath).TrimEnd('/');
    }

    public string BuildUrl(Guid fileId) => $"{_basePath}/{fileId}/content";
}
