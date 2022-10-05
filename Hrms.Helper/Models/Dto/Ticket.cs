using System;

namespace Hrms.Helper.Models.Dto
{
    public class TicketDto
    {
        public string TicketId { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Title { get; set; }
        public string Explanation { get; set; }
        public string Status { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? ClosedOn { get; set; }
        public string AddedBy { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsReopened { get; set; }
        public bool IsCreator { get; set; }
    }
}