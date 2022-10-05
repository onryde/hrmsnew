using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.RequestModels
{
    public class ReadNotificationRequest : BaseRequest
    {
        public List<NotificationDto> Notifications { get; set; }   
    }
}