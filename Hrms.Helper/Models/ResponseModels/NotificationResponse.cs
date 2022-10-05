using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class NotificationResponse : BaseResponse
    {
        public List<NotificationDto> Notifications { get; set; }
        public int UnReadNotifications { get; set; }
    }
}