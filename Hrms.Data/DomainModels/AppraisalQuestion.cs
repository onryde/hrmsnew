using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AppraisalQuestion
    {
        public AppraisalQuestion()
        {
            AppraisalAnswer = new HashSet<AppraisalAnswer>();
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
        [Column("appraisalId")]
        public long AppraisalId { get; set; }
        [Column("questionId")]
        public long QuestionId { get; set; }
        [Column("percentage")]
        public double Percentage { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("AppraisalQuestionAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("AppraisalId")]
        [InverseProperty("AppraisalQuestion")]
        public virtual Appraisal Appraisal { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("AppraisalQuestion")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("QuestionId")]
        [InverseProperty("AppraisalQuestion")]
        public virtual SettingsAppraisalQuestion Question { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("AppraisalQuestionUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("AppraisalQuestion")]
        public virtual ICollection<AppraisalAnswer> AppraisalAnswer { get; set; }
    }
}
