using Huddle.Channel.Application.Commands;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Huddle.Channel.Infrastructure.Idempotency;

public class RequestManager : IRequestManager
{
    private readonly ChannelContext _context;

    public RequestManager(ChannelContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }


    public async Task<bool> IsExistsAsync(Guid id)
    {
        var request = _context.ClientRequests.Local.FirstOrDefault(cr => cr.Id == id)
            ?? await _context.ClientRequests.FirstOrDefaultAsync(cr => cr.Id == id);

        return request != null;
    }

    public async Task CreateRequestForCommandAsync<T>(Guid id)
    {
        var exists = await IsExistsAsync(id);
        if (exists)
        {
            return;
        }

        var request = new ClientRequest()
        {
            Id = id,
            Name = typeof(T).Name,
            Time = DateTime.UtcNow
        };

        try
        {
            _context.ClientRequests.Add(request);
            await _context.SaveChangesAsync();
        }
        // Catching unique keys exception
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException exception && exception.SqlState == "23505")
        {
            
        }
    }
}
