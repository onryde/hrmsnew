using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class Appraisal
    {
        public Appraisal()
        {
            AppraisalEmployee = new HashSet<AppraisalEmployee>();
            AppraisalGrade = new HashSet<AppraisalGrade>();
            AppraisalQuestion = new HashSet<AppraisalQuestion>();
            AppraisalTraining = new HashSet<AppraisalTraining>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Required]
        [Column("title")]
        [StringLength(200)]
        public string Title { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Column("startDate", TypeName = "date")]
        public DateTime StartDate { get; set; }
        [Column("endDate", TypeName = "date")]
        public DateTime EndDate { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("isOpen")]
        public bool IsOpen { get; set; }
        public bool? IsLive { get; set; }
        [Column("showCalculation")]
        public bool? ShowCalculation { get; set; }
        [Column("category")]
        [StringLength(50)]
        public string Category { get; set; }
        [Column("mode")]
        public long? Mode { get; set; }
        [Column("calculationMethod")]
        [StringLength(50)]
        public string CalculationMethod { get; set; }
        [Column("eligibleFrom", TypeName = "date")]
        public DateTime? EligibleFrom { get; set; }
        [Column("eligibleTo", TypeName = "date")]
        public DateTime? EligibleTo { get; set; }
        [Column("year")]
        public long? Year { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("AppraisalAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("Appraisal")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("Mode")]
        [InverseProperty("Appraisal")]
        public virtual SettingsAppraisalMode ModeNavigation { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("AppraisalUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Appraisal")]
        public virtual ICollection<AppraisalEmployee> AppraisalEmployee { get; set; }
        [InverseProperty("Appraisal")]
        public virtual ICollection<AppraisalGrade> AppraisalGrade { get; set; }
        [InverseProperty("Appraisal")]
        public virtual ICollection<AppraisalQuestion> AppraisalQuestion { get; set; }
        [InverseProperty("Appraisal")]
        public virtual ICollection<AppraisalTraining> AppraisalTraining { get; set; }
    }
}
