using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeExit
    {
        public EmployeeExit()
        {
            EmployeeExitAnswers = new HashSet<EmployeeExitAnswers>();
            EmployeeExitAsset = new HashSet<EmployeeExitAsset>();
            EmployeeExitForm = new HashSet<EmployeeExitForm>();
            EmployeeExitHodfeedBackForm = new HashSet<EmployeeExitHodfeedBackForm>();
            EmployeeExitHrfeedBackForm = new HashSet<EmployeeExitHrfeedBackForm>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("resignationReason")]
        [StringLength(1000)]
        public string ResignationReason { get; set; }
        [Column("resignedOn", TypeName = "datetime")]
        public DateTime ResignedOn { get; set; }
        [Column("preferredRelievingDate", TypeName = "date")]
        public DateTime? PreferredRelievingDate { get; set; }
        [Column("actualRelievingDate", TypeName = "date")]
        public DateTime? ActualRelievingDate { get; set; }
        [Column("feedback")]
        [StringLength(1000)]
        public string Feedback { get; set; }
        [Column("managerEmployee")]
        public long? ManagerEmployee { get; set; }
        [Column("seniorManagerEmployee")]
        public long? SeniorManagerEmployee { get; set; }
        [Column("hrEmployee")]
        public long? HrEmployee { get; set; }
        [Required]
        [Column("status")]
        [StringLength(200)]
        public string Status { get; set; }
        [Column("exitInterviewDoneOn", TypeName = "datetime")]
        public DateTime? ExitInterviewDoneOn { get; set; }
        [Column("isRevoked")]
        public bool IsRevoked { get; set; }
        [Column("revokedOn", TypeName = "datetime")]
        public DateTime? RevokedOn { get; set; }
        [Column("confirmedBy")]
        public long? ConfirmedBy { get; set; }
        [Column("confirmedOn", TypeName = "datetime")]
        public DateTime? ConfirmedOn { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedOnRM", TypeName = "datetime")]
        public DateTime? UpdatedOnRM { get; set; }
        [Column("updatedOnSM", TypeName = "datetime")]
        public DateTime? UpdatedOnSM { get; set; }
        [Column("updatedOnHR", TypeName = "datetime")]
        public DateTime? UpdatedOnHR { get; set; }
        [Column("L1ApprovalFeedback")]
        [StringLength(1000)]
        public string L1approvalFeedback { get; set; }
        [Column("L2ApprovalFeedback")]
        [StringLength(1000)]
        public string L2approvalFeedback { get; set; }
        [Column("HRApprovalFeedback")]
        [StringLength(1000)]
        public string HrapprovalFeedback { get; set; }
        [Column("relievingDateAsPerPolicy", TypeName = "date")]
        public DateTime? RelievingDateAsPerPolicy { get; set; }
        [Column("clearanceComments")]
        [StringLength(500)]
        public string ClearanceComments { get; set; }
        [Column("eligibleForRehire")]
        public bool? EligibleForRehire { get; set; }
        [Column("L1ApprovalFeedbackForOthers")]
        [StringLength(1000)]
        public string L1approvalFeedbackForOthers { get; set; }
        [Column("L2ApprovalFeedbackForOthers")]
        [StringLength(1000)]
        public string L2approvalFeedbackForOthers { get; set; }
        [Column("HRApprovalFeedbackForOthers")]
        [StringLength(1000)]
        public string HrapprovalFeedbackForOthers { get; set; }
        [Column("feedbackForOthers")]
        [StringLength(1000)]
        public string FeedbackForOthers { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeExitAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeExit")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("ConfirmedBy")]
        [InverseProperty("EmployeeExitConfirmedByNavigation")]
        public virtual Employee ConfirmedByNavigation { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeExitEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("HrEmployee")]
        [InverseProperty("EmployeeExitHrEmployeeNavigation")]
        public virtual Employee HrEmployeeNavigation { get; set; }
        [ForeignKey("ManagerEmployee")]
        [InverseProperty("EmployeeExitManagerEmployeeNavigation")]
        public virtual Employee ManagerEmployeeNavigation { get; set; }
        [ForeignKey("SeniorManagerEmployee")]
        [InverseProperty("EmployeeExitSeniorManagerEmployeeNavigation")]
        public virtual Employee SeniorManagerEmployeeNavigation { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeExitUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("EmployeeExit")]
        public virtual ICollection<EmployeeExitAnswers> EmployeeExitAnswers { get; set; }
        [InverseProperty("EmployeeExit")]
        public virtual ICollection<EmployeeExitAsset> EmployeeExitAsset { get; set; }
        [InverseProperty("Exit")]
        public virtual ICollection<EmployeeExitForm> EmployeeExitForm { get; set; }
        [InverseProperty("Exit")]
        public virtual ICollection<EmployeeExitHodfeedBackForm> EmployeeExitHodfeedBackForm { get; set; }
        [InverseProperty("Exit")]
        public virtual ICollection<EmployeeExitHrfeedBackForm> EmployeeExitHrfeedBackForm { get; set; }
    }
}
