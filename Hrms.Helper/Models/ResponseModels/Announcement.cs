using System;
using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class GetAnnouncementsResponse : BaseResponse
    {
        public List<AnnouncementDto> Announcements { get; set; }
    }

    public class AnnouncementDetailsResponse : BaseResponse
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
        public List<AttachmentDto> Attachments { get; set; }
        public List<string> Locations { get; set; }
    }
}