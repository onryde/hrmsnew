using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Hrms.Helper.Models.Dto
{
    public class TrainingDto
    {
        public string TrainingId { get; set; }
        public string TrainingName { get; set; }
        public string Organizers { get; set; }
        public int TotalDays { get; set; }
        public string TrainerName { get; set; }
        public string Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AttendancePercentage { get; set; }
        public int MaxNominees { get; set; }
        public string Effectiveness { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsStarted { get; set; }
        public bool IsFeedbackClosed { get; set; }
        public string FeedbackPercentage { get; set; }
    }

    public class TrainingCalDto
    {
        public string TrainingId { get; set; }
        public string TrainingName { get; set; }
        public string TrainerName { get; set; }
        public string Organizers { get; set; }
        public string Location { get; set; }
        public DateTime? Date { get; set; }
        public int MaxNominees { get; set; }
        public bool? isConfirmed { get; set; }
        public bool? isClosed { get; set; }
        public bool? isStartd { get; set; }
    }


    public class TrainingAttendanceDto
    {
        public DateTime? Date { get; set; }
        public string NomineeId { get; set; }
        public string EmployeeName { get; set; }
        public bool? IsAttended { get; set; }
        public string Remark { get; set; }
    }

    public class TrainingFeedbackAnswerDto
    {
        public long QuestionId { get; set; }
        public string Answer { get; set; }
    }

    public class TrainingNomineeDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string NomineeId { get; set; }
        public string Name { get; set; }
        public bool? IsSelfAccepted { get; set; }
        public bool? IsMangerAccepted { get; set; }
        public bool? IsHrAccepted { get; set; }
        public DateTime? SelfUpdatedOn { get; set; }
        public DateTime? ManagerUpdatedOn { get; set; }
        public DateTime? HrUpdatedOn { get; set; }
        public string ManagerName { get; set; }
        public string HrName { get; set; }
        public bool IsSelf { get; set; }
        public bool IsManager { get; set; }
        public bool IsHr { get; set; }
        public bool IsAccepted { get; set; }
        public string RejectionReason { get; set; }
        public bool IsRejected { get; set; }
        public List<TrainingQuestionDto> Feedback { get; set; }
        public bool IsFeedbackDone { get; set; }
        public string FeedbackContent { get; set; }
        public double FeedbackRating { get; set; }
    }

    public class TrainingQuestionDto
    {
        public string EmployeeId { get; set; }
        public string NomineeId { get; set; }
        public string Name { get; set; }
        public long? QuestionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class TrainingEffectivenessDto
    {
        public long QuestionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class TrainingEmailDto
    {
        public string TrainingName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Mode { get; set; }
        public string Grades { get; set; }
        public string Designations { get; set; }
        public string Departments { get; set; }
        public string Locations { get; set; }
        public string AcceptanceLastDate { get; set; }
        public string TrainingDescription { get; set; }
    }
}
