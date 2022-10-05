using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsAppraisalQuestion
    {
        public SettingsAppraisalQuestion()
        {
            AppraisalQuestion = new HashSet<AppraisalQuestion>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("question")]
        [StringLength(200)]
        public string Question { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsAppraisalQuestionAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsAppraisalQuestion")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsAppraisalQuestionUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Question")]
        public virtual ICollection<AppraisalQuestion> AppraisalQuestion { get; set; }
    }
}
