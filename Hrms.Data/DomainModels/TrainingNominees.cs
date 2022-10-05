using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TrainingNominees
    {
        public TrainingNominees()
        {
            TrainingAttendance = new HashSet<TrainingAttendance>();
            TrainingFeedback = new HashSet<TrainingFeedback>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("selfAccepted")]
        public bool? SelfAccepted { get; set; }
        [Column("managerAccepted")]
        public bool? ManagerAccepted { get; set; }
        [Column("hrAccepted")]
        public bool? HrAccepted { get; set; }
        [Column("rejectedReason")]
        [StringLength(500)]
        public string RejectedReason { get; set; }
        [Column("isRejected")]
        public bool IsRejected { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }
        [Column("managerId")]
        public long? ManagerId { get; set; }
        [Column("hrId")]
        public long? HrId { get; set; }
        [Column("managerUpdatedOn", TypeName = "datetime")]
        public DateTime? ManagerUpdatedOn { get; set; }
        [Column("hrUpdatedOn", TypeName = "datetime")]
        public DateTime? HrUpdatedOn { get; set; }
        [Column("selfUpdatedOn", TypeName = "datetime")]
        public DateTime? SelfUpdatedOn { get; set; }
        [Column("feedbackContent")]
        [StringLength(1000)]
        public string FeedbackContent { get; set; }

        [ForeignKey("EmployeeId")]
        [InverseProperty("TrainingNomineesEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("HrId")]
        [InverseProperty("TrainingNomineesHr")]
        public virtual Employee Hr { get; set; }
        [ForeignKey("ManagerId")]
        [InverseProperty("TrainingNomineesManager")]
        public virtual Employee Manager { get; set; }
        [ForeignKey("TrainingId")]
        [InverseProperty("TrainingNominees")]
        public virtual Training Training { get; set; }
        [InverseProperty("TrainingNomineeNavigation")]
        public virtual ICollection<TrainingAttendance> TrainingAttendance { get; set; }
        [InverseProperty("Nominee")]
        public virtual ICollection<TrainingFeedback> TrainingFeedback { get; set; }
    }
}
