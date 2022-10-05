using System;

namespace Hrms.Helper.Models.Dto
{
    public class AnnouncementDto
    {
        public string AnnouncementId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Content { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsHidden { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? Date { get; set; }
        public string AnnouncementType { get; set; }
        public int AttachmentCount { get; set; }
        public string Location { get; set; }
    }
    
}