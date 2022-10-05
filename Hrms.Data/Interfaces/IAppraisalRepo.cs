using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;

namespace Hrms.Data.Interfaces
{
    public interface IAppraisalRepo : IBaseRepository<Appraisal>
    {
        GetAppraisalsResponse GetAllAppraisals(AppraisalFilterRequest request);
        BaseResponse UpdateAppraisal(UpdateAppraisalRequest request);
        BaseResponse DeleteAppraisal(AppraisalActionRequest request);
        AppraisalDetailsResponse GetAppraisalDetails(AppraisalActionRequest request);
    }
}