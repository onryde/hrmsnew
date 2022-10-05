using System;
using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Microsoft.AspNetCore.Http;

namespace Hrms.Helper.Models.RequestModels
{
    public class AnnouncementActionRequest : BaseRequest
    {
        public string AnnouncementId { get; set; }
    }

    public class UpdateAnnouncementRequest : BaseRequest
    {
        public string AnnouncementId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Content { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsHidden { get; set; }
        public bool IsPublished { get; set; }
        public DateTime Date { get; set; }
        public string AnnouncementType { get; set; }
        public List<string> AttachmentType { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
        public List<IFormFile> NewAttachments { get; set; }
        public List<string> Locations { get; set; }
        public string FileBasePath { get; set; }
    }

    public class AnnouncementFilterRequest : BaseRequest
    {
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public List<bool> Publish { get; set; }
        public List<string> Types { get; set; }
        public List<string> Locations { get; set; }
    }
}