using System;
using System.Collections.Generic;

namespace Hrms.Helper.Models.Dto
{
    public class AppraisalDto
    {
        public string AppraisalId { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCurrent { get; set; }
        public bool ShowCalculation { get; set; }
        public bool IsLive { get; set; }
        public List<string> Grades { get; set; }
        public List<UpdateAppraisalQuestionDto> Questions { get; set; }
        public string Category { get; set; }
        public string Mode { get; set; }
        public string CalculationMethod { get; set; }
        public DateTime? EligibleFrom { get; set; }
        public DateTime? EligibleTo { get; set; }
        public long? Year { get; set; }
    }

    public class AppraisalEmployeeDto
    {
        public string AppraisalTitle { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string EmailId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Location { get; set; }
        public string Grade { get; set; }
        public string Status { get; set; }
        public string Rating { get; set; }
        public DateTime? SelfFilledOn { get; set; }
        public DateTime? RmFilledOn { get; set; }
        public DateTime? HrFilledOn { get; set; }
        public DateTime? L2FilledOn { get; set; }
        public DateTime? AppraisalClosedOn { get; set; }
        public string ManagerName { get; set; }
        public string L2ManagerName { get; set; }

        public string VariableRating { get; set; }
        public DateTime? SelfObjectiveFilledOn { get; set; }
        public DateTime? RmObjectiveFilledOn { get; set; }
        public DateTime? HrObjectiveFilledOn { get; set; }
        public DateTime? L2ObjectiveFilledOn { get; set; }
        public DateTime? SelfVariableFilledOn { get; set; }
        public DateTime? RmVariableFilledOn { get; set; }
        public DateTime? HrVariableFilledOn { get; set; }
        public DateTime? L2VariableFilledOn { get; set; }
        public long? AppraisalMode { get; set; }
    }

    public class UpdateAppraisalQuestionDto
    {
        public string QuestionId { get; set; }
        public double Percentage { get; set; }
    }
}