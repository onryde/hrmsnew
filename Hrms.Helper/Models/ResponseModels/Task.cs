using System;
using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class GetTaskCommentsResponse : BaseResponse
    {
        public List<CommentDto> Comments { get; set; }
    }

    public class GetTasksResponse : BaseResponse
    {
        public List<TaskDto> Tasks { get; set; }
    }

    public class GetTaskFilterResponse : BaseResponse
    {
        public List<EmployeeBaseInfoDto> CreatedBy { get; set; }
        public List<EmployeeBaseInfoDto> AssignedTo { get; set; }
    }

    public class TaskDetailsResponse : BaseResponse
    {
        public string TaskId { get; set; }
        public DateTime? DueOn { get; set; }
        public string Content { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToId { get; set; }
        public List<CommentDto> Comments { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsVerified { get; set; }
        public bool IsIrrelevant { get; set; }
        public bool IsSelf { get; set; }
        public bool IsCreator { get; set; }
        public int HourTime { get; set; }
        public int MinuteTime { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public string TaskPriority { get; set; }
    }
}