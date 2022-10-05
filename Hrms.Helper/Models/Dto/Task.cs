using System;
using System.Collections.Generic;

namespace Hrms.Helper.Models.Dto
{
    public class TaskDto
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
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string Priority { get; set; }
    }
}