using System.Threading.Tasks;

namespace Forum020.Server.Services.Interfaces
{
    public interface INotificationsService
    {
        Task SendNotificationAsync(string notification);
    }
}
