using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsAppraiseeType
    {
        public SettingsAppraiseeType()
        {
            AppraisalFeedback = new HashSet<AppraisalFeedback>();
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
        [Column("appraiseeType")]
        [StringLength(100)]
        public string AppraiseeType { get; set; }
        [Required]
        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsAppraiseeTypeAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsAppraiseeType")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsAppraiseeTypeUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("AppraiseeTypeNavigation")]
        public virtual ICollection<AppraisalFeedback> AppraisalFeedback { get; set; }
    }
}
