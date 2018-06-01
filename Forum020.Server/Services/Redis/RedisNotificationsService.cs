using System;
using System.Threading.Tasks;
using Forum020.Server.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Forum020.Server.Services.Redis
{
    internal class RedisNotificationsService : NotificationsServiceBase, INotificationsService
    {
        private const string CONNECTION_MULTIPLEXER_CONFIGURATION_KEY = "Redis";

        private const string NOTIFICATIONS_CHANNEL = "NOTIFICATIONS";

        private readonly ConnectionMultiplexer _redis;

        public RedisNotificationsService(INotificationsServerSentEventsService notificationsServerSentEventsService, IConfiguration configuration)
            : base(notificationsServerSentEventsService)
        {
            _redis = ConnectionMultiplexer.Connect(configuration.GetValue<String>(CONNECTION_MULTIPLEXER_CONFIGURATION_KEY));

            ISubscriber subscriber = _redis.GetSubscriber();
            subscriber.Subscribe(NOTIFICATIONS_CHANNEL, async (channel, message) => { await SendSseEventAsync(message); });
        }

        public Task SendNotificationAsync(string notification)
        {
            ISubscriber subscriber = _redis.GetSubscriber();

            return subscriber.PublishAsync(NOTIFICATIONS_CHANNEL, notification);
        }
    }
}
