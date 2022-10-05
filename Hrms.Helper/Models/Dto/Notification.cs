using System;

namespace Hrms.Helper.Models.Dto
{
    public class NotificationDto
    {
        public string NotificationId { get; set; }
        public string Text { get; set; }
        public DateTime ReceivedOn { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadOn { get; set; }
        public string Data { get; set; }
        public int ActionId { get; set; }
    }
}