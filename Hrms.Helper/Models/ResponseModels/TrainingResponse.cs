using System;
using System.Collections.Generic;
using System.Text;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class TrainingListResponse : BaseResponse
    {
        public List<TrainingDto> Trainings { get; set; }
        public List<TrainingDto> UpcomingTrainings { get; set; }
        public List<TrainingCalDto> TrainingsCalendar { get; set; }
        //public string TrainingTitle { get; set; }
        //public string TrainingCode { get; set; }
        //public string TrainingName { get; set; }
        //public List<string> Organizers { get; set; }
        //public string TrainerName { get; set; }
        //public string Location { get; set; }
        //public List<DateTime> Days { get; set; }
        //public string AttendancePercentage { get; set; }
        //public int MaxNominees { get; set; }

    }

    public class TrainingDetailsResponse : BaseResponse
    {
        public string TrainingId { get; set; }
        public string TrainingTitle { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingCategory { get; set; }
        public string TrainingTypeId { get; set; }
        public List<string> Organizers { get; set; }
        public string TrainerName { get; set; }
        public List<TrainingNomineeDto> Nominees { get; set; }
        public List<string> Grades { get; set; }
        public List<string> Locations { get; set; }
        public List<string> Departments { get; set; }
        public List<string> Designations { get; set; }
        public string TrainingTiming { get; set; }
        public List<TrainingAttendanceDto> Attendance { get; set; }
        public string Description { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFeedbackClosed { get; set; }
        public string OtherLocation { get; set; }
        public bool IsOfficeLocation { get; set; }
        public string OfficeLocationId { get; set; }
        public List<TrainingEffectivenessDto> Effectiveness { get; set; }
        public List<DateTime?> Dates { get; set; }
        public int MaxNominees { get; set; }
        public List<TrainingQuestionDto> SelfFeedback { get; set; }
        public List<TrainingQuestionDto> Feedbacks { get; set; }
        public string FeedbackContent { get; set; }
    }
}
