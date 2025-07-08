using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using Huddle.Channel.Domain.SeedWork;
using Huddle.Channel.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;
using System.Data;
using Huddle.IntegrationEventLogEF;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.InviteAggregate;

namespace Huddle.Channel.Infrastructure
{
    /// <summary>
    /// Add migrations using the following command inside the solution directory:
    /// 
    /// dotnet ef migrations add [migration-name] --project src\ChannelService\Huddle.Channel.Infrastructure --startup-project src\ChannelService\Huddle.Channel.WebApi
    /// </summary>
    public class ChannelContext : DbContext, IUnitOfWork
    {
        public DbSet<Server> Servers { get; set; }
        public DbSet<Domain.Aggregates.ServerAggregate.Channel> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Invite> Invites { get; set; }

        private readonly IMediator _mediator;

        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;

        public ChannelContext(DbContextOptions<ChannelContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("Channels");

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.UseIntegrationEventLogs();
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this);

            _ = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async System.Threading.Tasks.Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
