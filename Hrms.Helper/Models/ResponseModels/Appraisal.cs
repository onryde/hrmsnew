using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class GetAppraisalsResponse : BaseResponse
    {
        public bool IsGradeMismatch { get; set; }
        public List<AppraisalDto> Appraisals { get; set; }
    }

    public class AppraisalDetailsResponse : BaseResponse
    {
        public AppraisalDto AppraisalInfo { get; set; }
        public List<AppraisalEmployeeDto> AppraisalEmployees { get; set; }
    }
}