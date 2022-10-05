using System;
using Hrms.Helper.Models.Contracts;

namespace Hrms.Helper.Models.RequestModels
{
    public class UpdateTaskRequest : BaseRequest
    {
        public string TaskId { get; set; }
        public string TaskContent { get; set; }
        public DateTime DueDate { get; set; }
        public string AssignedTo { get; set; }
        public int HourTime { get; set; }
        public int MinuteTime { get; set; }
        public string TaskPriority { get; set; }
    }

    public class AddTaskCommentRequest : BaseRequest
    {
        public string TaskId { get; set; }
        public string CommentId { get; set; }
        public string Comment { get; set; }
    }

    public class TaskCommentActionRequest : BaseRequest
    {
        public string TaskId { get; set; }
        public string CommentId { get; set; }
    }

    public class TaskActionRequest : BaseRequest
    {
        public string TaskId { get; set; }
    }
}