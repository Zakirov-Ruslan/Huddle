using Huddle.Channel.Application.Commands.Server;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Application.IntegrationEvents;
using Huddle.Channel.Application.Queries.Invites;
using Huddle.Channel.Application.Queries.Members;
using Huddle.Channel.Application.Queries.Messages;
using Huddle.Channel.Application.Queries.Servers;
using Huddle.Channel.Application.Services;
using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;
using Huddle.Channel.Domain.SeedWork;
using Huddle.Channel.Infrastructure.Behaviors;
using Huddle.Channel.Infrastructure.Repositories;
using Huddle.Channel.Infrastructure.Services;
using Huddle.IntegrationEventLogEF.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Huddle.Channel.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("channeldb")
                ?? throw new ArgumentNullException("Connection string 'channeldb' not found");
            services.AddDbContext<ChannelContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ChannelContext>());

            services.AddMigration<ChannelContext>();

            services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<ChannelContext>>();
            services.AddTransient<IChannelsIntegrationEventService, ChannelsIntegrationEventService>();

            services.AddAutoMapper(s => s.AddProfile<MappingProfile>());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(CreateServerCommand)));
                cfg.RegisterServicesFromAssembly(typeof(LoggingBehavior<,>).Assembly);

                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            services.AddScoped<IServerRepository, ServerRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IInviteRepository, InviteRepository>();
            services.AddScoped<IAccessService, AccessService>();

            services.AddScoped<IMessagesQueries, MessagesQueries>();
            services.AddScoped<IServersQueries, ServersQueries>();
            services.AddScoped<IInvitesQueries, InvitesQueries>();
            services.AddScoped<IMembersQueries, MembersQueries>();

            return services;
        }

        public static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
        {
            //eventBus.AddSubscription<GracePeriodConfirmedIntegrationEvent, GracePeriodConfirmedIntegrationEventHandler>();
        }
    }
}
