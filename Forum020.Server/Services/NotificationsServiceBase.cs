using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forum020.Server.Services.Interfaces;
using Lib.AspNetCore.ServerSentEvents;

namespace Forum020.Server.Services
{
    internal abstract class NotificationsServiceBase
    {
        private INotificationsServerSentEventsService _notificationsServerSentEventsService;

        protected NotificationsServiceBase(INotificationsServerSentEventsService notificationsServerSentEventsService)
        {
            _notificationsServerSentEventsService = notificationsServerSentEventsService;
        }

        protected Task SendSseEventAsync(string notification)
        {
            return _notificationsServerSentEventsService.SendEventAsync(new ServerSentEvent
            {
                Data = new List<string>(notification.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
            });
        }
    }
}
