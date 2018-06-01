using Forum020.Server.Services.Interfaces;
using System.Threading.Tasks;

namespace Forum020.Server.Services.Local
{
    internal class LocalNotificationsService : NotificationsServiceBase, INotificationsService
    {
        public LocalNotificationsService(INotificationsServerSentEventsService notificationsServerSentEventsService)
            : base(notificationsServerSentEventsService)
        { }

        public Task SendNotificationAsync(string notification)
        {
            return SendSseEventAsync(notification );
        }
    }
}
