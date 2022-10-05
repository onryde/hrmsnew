using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;

namespace Hrms.Data.Interfaces
{
    public interface IAnnouncementRepo : IBaseRepository<Announcement>
    {
        GetAnnouncementsResponse GetAllAnnouncements(AnnouncementFilterRequest request);
        GetAnnouncementsResponse GetAnnouncementToShow(BaseRequest request);
        AnnouncementDetailsResponse GetAnnouncementDetails(AnnouncementActionRequest request);
        BaseResponse UpdateAnnouncement(UpdateAnnouncementRequest request);
        BaseResponse DeleteAnnouncement(AnnouncementActionRequest request);
    }
}