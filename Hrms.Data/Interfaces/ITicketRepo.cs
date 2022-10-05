using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Hrms.Helper.Settings;

namespace Hrms.Data.Interfaces
{
    public interface ITicketRepo
    {
        GetTicketsResponse GetAllTickets(TicketFilterRequest request);
        TicketDetailsResponse GetTicketDetails(TicketActionRequest request);
        BaseResponse UpdateTicket(UpdateTicketRequest request, ApplicationSettings settings);
        BaseResponse StartTicket(TicketActionRequest request);
        BaseResponse CloseTicket(TicketActionRequest request);
        BaseResponse ReopenTicket(TicketActionRequest request);
        BaseResponse UndoStartTicket(TicketActionRequest request);
        BaseResponse UndoCloseTicket(TicketActionRequest request);
        BaseResponse UndoReopenTicket(TicketActionRequest request);
        GetTicketCommentsResponse AddComment(AddTicketCommentRequest request);
        GetTicketCommentsResponse DeleteComment(TicketCommentActionRequest request);
    }
}