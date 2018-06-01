using System;
using Forum020.Server.Services.Interfaces;
using Forum020.Server.Services.Local;
using Forum020.Server.Services.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Forum020.Server.Services
{
    internal static class ServiceCollectionExtensions
    {
        private const string NOTIFICATIONS_SERVICE_TYPE_CONFIGURATION_KEY = "NotificationsService";
        private const string NOTIFICATIONS_SERVICE_TYPE_LOCAL = "Local";
        private const string NOTIFICATIONS_SERVICE_TYPE_REDIS = "Redis";

        public static IServiceCollection AddNotificationsService(this IServiceCollection services, IConfiguration configuration)
        {
            string notificationsServiceType = configuration.GetValue(NOTIFICATIONS_SERVICE_TYPE_CONFIGURATION_KEY, NOTIFICATIONS_SERVICE_TYPE_LOCAL);

            if (notificationsServiceType.Equals(NOTIFICATIONS_SERVICE_TYPE_LOCAL, StringComparison.InvariantCultureIgnoreCase))
            {
                services.AddTransient<INotificationsService, LocalNotificationsService>();
            }
            else if (notificationsServiceType.Equals(NOTIFICATIONS_SERVICE_TYPE_REDIS, StringComparison.InvariantCultureIgnoreCase))
            {
                services.AddSingleton<INotificationsService, RedisNotificationsService>();
            }
            else
            {
                throw new NotSupportedException($"Not supported {nameof(INotificationsService)} type.");
            }

            return services;
        }
    }
}
