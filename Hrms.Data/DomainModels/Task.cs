using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class Task
    {
        public Task()
        {
            TaskComment = new HashSet<TaskComment>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("isStarted")]
        public bool IsStarted { get; set; }
        [Column("isCompleted")]
        public bool IsCompleted { get; set; }
        [Column("dueDate", TypeName = "date")]
        public DateTime? DueDate { get; set; }
        [Required]
        [Column("taskContent")]
        [StringLength(1000)]
        public string TaskContent { get; set; }
        [Column("assignedTo")]
        public long AssignedTo { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("isVerified")]
        public bool IsVerified { get; set; }
        [Column("startedOn", TypeName = "datetime")]
        public DateTime? StartedOn { get; set; }
        [Column("completedOn", TypeName = "datetime")]
        public DateTime? CompletedOn { get; set; }
        [Column("verifiedOn", TypeName = "datetime")]
        public DateTime? VerifiedOn { get; set; }
        [Column("isIrrelevant")]
        public bool? IsIrrelevant { get; set; }
        [Column("hourTime")]
        public int? HourTime { get; set; }
        [Column("minuteTime")]
        public int? MinuteTime { get; set; }
        [Column("priority")]
        [StringLength(100)]
        public string Priority { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("TaskAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("AssignedTo")]
        [InverseProperty("TaskAssignedToNavigation")]
        public virtual Employee AssignedToNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("Task")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("TaskUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Task")]
        public virtual ICollection<TaskComment> TaskComment { get; set; }
    }
}
