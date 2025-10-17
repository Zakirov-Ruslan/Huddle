namespace Huddle.Channel.Application.Commands;

public interface IRequestManager
{
    Task<bool> IsExistsAsync(Guid id);

    Task CreateRequestForCommandAsync<T>(Guid id);
}
