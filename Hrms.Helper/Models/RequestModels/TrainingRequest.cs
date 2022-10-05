using System;
using System.Collections.Generic;
using System.Text;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.RequestModels
{
    public class TrainingFilterRequest : BaseRequest
    {
        public string TrainingName { get; set; }
        public List<string> EmployeeIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateTrainingRequest : BaseRequest
    {
        public string TrainingId { get; set; }
        public string TrainingTitle { get; set; }
        public string TrainingCode { get; set; }
        public List<DateTime> Dates { get; set; }
        public string TrainerName { get; set; }
        public string TrainingType { get; set; }
        public string TrainingCategory { get; set; }
        public string OtherLocation { get; set; }
        public bool IsOfficeLocation { get; set; }
        public string OfficeLocationId { get; set; }
        public List<string> Organizers { get; set; }
        public int MaxNominees { get; set; }
        public string Description { get; set; }
        public bool IsConfirmed { get; set; }
        public string TimeOfDay { get; set; }
        public List<string> Grades { get; set; }
        public List<string> Locations { get; set; }
        public List<string> Departments { get; set; }
        public List<string> Designations { get; set; }
        public List<string> Nominees { get; set; }
    }

    public class TrainingActionRequest : BaseRequest
    {
        public string TrainingId { get; set; }
        public bool GetAllFeedbacks { get; set; }
    }

    public class TrainingCancellationActionRequest : BaseRequest
    {
        public string TrainingId { get; set; }
        public string Reason { get; set; }
    }

    public class TrainingNomineeRequest : BaseRequest
    {
        public string TrainingId { get; set; }
        public List<TrainingNomineeDto> Nominees { get; set; }
    }

    public class TrainingAttendanceRequest : BaseRequest
    {
        public string TrainingId { get; set; }
        public List<TrainingAttendanceDto> Attendance { get; set; }
    }

    public class AddNomineesRequest : BaseRequest
    {
        public string TrainingId { get; set; }
        public List<string> Nominees { get; set; }
    }

    public class SubmitFeedbackRequest : BaseRequest
    {
        public string TrainingId { get; set; }
        public List<TrainingFeedbackAnswerDto> Answers { get; set; }
        public string FeedbackContent { get; set; }
    }

}
