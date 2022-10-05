using System;
namespace Hrms.Helper.Models.Dto
{
    public class EventLogDto
    {
        public long ActionId { get; set; }
        public DateTime Datetime { get; set; }
        public string PerformedBy { get; set; }
        public string Text { get; set; }
        public string Data { get; set; }
    }
}
