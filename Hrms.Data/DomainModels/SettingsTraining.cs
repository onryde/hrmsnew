using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsTraining
    {
        public SettingsTraining()
        {
            AppraisalTraining = new HashSet<AppraisalTraining>();
            SettingsTrainingGrade = new HashSet<SettingsTrainingGrade>();
            Training = new HashSet<Training>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Required]
        [Column("name")]
        [StringLength(200)]
        public string Name { get; set; }
        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("trainingCode")]
        [StringLength(100)]
        public string TrainingCode { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsTrainingAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsTraining")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsTrainingUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<AppraisalTraining> AppraisalTraining { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<SettingsTrainingGrade> SettingsTrainingGrade { get; set; }
        [InverseProperty("TrainingNavigation")]
        public virtual ICollection<Training> Training { get; set; }
    }
}
