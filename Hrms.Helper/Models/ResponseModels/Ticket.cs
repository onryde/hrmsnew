using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class GetTicketsResponse : BaseResponse
    {
        public List<TicketDto> Tickets { get; set; }
    }

    public class TicketDetailsResponse : BaseResponse
    {
        public TicketDto TicketInfo { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
        public List<CommentDto> Comments { get; set; }
    }

    public class GetTicketCommentsResponse : BaseResponse
    {
        public List<CommentDto> Comments { get; set; }
    }
}