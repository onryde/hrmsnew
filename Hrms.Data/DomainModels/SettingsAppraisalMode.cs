using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsAppraisalMode
    {
        public SettingsAppraisalMode()
        {
            Appraisal = new HashSet<Appraisal>();
            AppraisalBusinessNeed = new HashSet<AppraisalBusinessNeed>();
            AppraisalFeedback = new HashSet<AppraisalFeedback>();
            AppraisalSelfAnswer = new HashSet<AppraisalSelfAnswer>();
        }

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
        [Required]
        [Column("appraisalMode")]
        [StringLength(100)]
        public string AppraisalMode { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsAppraisalModeAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsAppraisalMode")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsAppraisalModeUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("ModeNavigation")]
        public virtual ICollection<Appraisal> Appraisal { get; set; }
        [InverseProperty("AppraisalModeNavigation")]
        public virtual ICollection<AppraisalBusinessNeed> AppraisalBusinessNeed { get; set; }
        [InverseProperty("AppraisalModeNavigation")]
        public virtual ICollection<AppraisalFeedback> AppraisalFeedback { get; set; }
        [InverseProperty("AppraisalModeNavigation")]
        public virtual ICollection<AppraisalSelfAnswer> AppraisalSelfAnswer { get; set; }
    }
}
