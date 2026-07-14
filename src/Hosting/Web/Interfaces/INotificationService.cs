using CelebrationPassports.Web.Models.Notifications;

namespace CelebrationPassports.Web.Interfaces;

public interface INotificationService
{
    Task<List<NotificationViewModel>> GetMineAsync(int take = 20);

    Task<int> GetUnreadCountAsync();

    Task MarkAsReadAsync(Guid id);
}
