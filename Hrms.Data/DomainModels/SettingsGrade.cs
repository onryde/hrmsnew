using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsGrade
    {
        public SettingsGrade()
        {
            AppraisalGrade = new HashSet<AppraisalGrade>();
            EmployeeCompany = new HashSet<EmployeeCompany>();
            EmployeeCompensation = new HashSet<EmployeeCompensation>();
            SettingsTrainingGrade = new HashSet<SettingsTrainingGrade>();
            TrainingGrade = new HashSet<TrainingGrade>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("grade")]
        [StringLength(20)]
        public string Grade { get; set; }
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
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("noticePeriod")]
        public int? NoticePeriod { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsGradeAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsGrade")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsGradeUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Grade")]
        public virtual ICollection<AppraisalGrade> AppraisalGrade { get; set; }
        [InverseProperty("Grade")]
        public virtual ICollection<EmployeeCompany> EmployeeCompany { get; set; }
        [InverseProperty("Grade")]
        public virtual ICollection<EmployeeCompensation> EmployeeCompensation { get; set; }
        [InverseProperty("Grade")]
        public virtual ICollection<SettingsTrainingGrade> SettingsTrainingGrade { get; set; }
        [InverseProperty("Grade")]
        public virtual ICollection<TrainingGrade> TrainingGrade { get; set; }
    }
}
