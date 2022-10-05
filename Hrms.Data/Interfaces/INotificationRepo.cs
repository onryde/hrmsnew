using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;

namespace Hrms.Data.Interfaces
{
    public interface INotificationRepo
    {
        NotificationResponse GetRecentNotifications(BaseRequest request);
        NotificationResponse GetAllNotifications(BaseRequest request);
        NotificationResponse MarkNotificationsAsRead(ReadNotificationRequest request);
    }
}