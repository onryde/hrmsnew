using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AppraisalFeedback
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("companyId")]
        public long? CompanyId { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("appraisalEmployeeId")]
        public long AppraisalEmployeeId { get; set; }
        [Column("appraiseeType")]
        public long AppraiseeType { get; set; }
        [Column("givenBy")]
        public long GivenBy { get; set; }
        [Required]
        [Column("feedback")]
        [StringLength(1000)]
        public string Feedback { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }
        [Column("appraisalMode")]
        public long? AppraisalMode { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("AppraisalFeedbackAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("AppraisalEmployeeId")]
        [InverseProperty("AppraisalFeedback")]
        public virtual AppraisalEmployee AppraisalEmployee { get; set; }
        [ForeignKey("AppraisalMode")]
        [InverseProperty("AppraisalFeedback")]
        public virtual SettingsAppraisalMode AppraisalModeNavigation { get; set; }
        [ForeignKey("AppraiseeType")]
        [InverseProperty("AppraisalFeedback")]
        public virtual SettingsAppraiseeType AppraiseeTypeNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("AppraisalFeedback")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("GivenBy")]
        [InverseProperty("AppraisalFeedbackGivenByNavigation")]
        public virtual Employee GivenByNavigation { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("AppraisalFeedbackUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
