using System;
using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.RequestModels
{
    public class UpdateAppraisalRequest : BaseRequest
    {
        public string AppraisalId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> GradeIds { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool ShowCalculation { get; set; }
        public bool IsLive { get; set; }
        public string Category { get; set; }
        public string Mode { get; set; }
        public string CalculationMethod { get; set; }
        public DateTime EligibleFrom { get; set; }
        public DateTime EligibleTo { get; set; }
        public long Year { get; set; }
        public List<UpdateAppraisalQuestionDto> Questions { get; set; }
    }

    public class AppraisalActionRequest : BaseRequest
    {
        public string AppraisalId { get; set; }
    }

    public class AppraisalFilterRequest: BaseRequest
    {
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> GradeIds { get; set; }
    }
}