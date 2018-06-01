using Forum020.Server.Services.Interfaces;
using Lib.AspNetCore.ServerSentEvents;

namespace Forum020.Server.Services
{
    internal class NotificationsServerSentEventsService : ServerSentEventsService, INotificationsServerSentEventsService
    {
        public NotificationsServerSentEventsService()
        {
            ChangeReconnectIntervalAsync(5000);
        }
    }
}
