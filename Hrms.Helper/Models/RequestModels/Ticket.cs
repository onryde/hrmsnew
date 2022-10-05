using System;
using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Microsoft.AspNetCore.Http;

namespace Hrms.Helper.Models.RequestModels
{
    public class UpdateTicketRequest : BaseRequest
    {
        public string TicketId { get; set; }
        public string CategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public string Title { get; set; }
        public string Explanation { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
        public List<IFormFile> NewAttachments { get; set; }
        public string FileBasePath { get; set; }
    }

    public class TicketActionRequest : BaseRequest
    {
        public string TicketId { get; set; }
    }
    
    public class AddTicketCommentRequest : BaseRequest
    {
        public string TicketId { get; set; }
        public string CommentId { get; set; }
        public string Comment { get; set; }
    }

    public class TicketCommentActionRequest : BaseRequest
    {
        public string TicketId { get; set; }
        public string CommentId { get; set; }
    }

    public class TicketFilterRequest : BaseRequest
    {
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Employee { get; set; }
        public List<string> AddedBy { get; set; }
        public List<string> Status { get; set; }
        
    }
}