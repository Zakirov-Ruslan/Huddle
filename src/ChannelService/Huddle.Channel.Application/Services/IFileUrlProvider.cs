namespace Huddle.Channel.Application.Services
{
    public interface IFileUrlProvider
    {
        string BuildUrl(Guid fileId);
    }
}
