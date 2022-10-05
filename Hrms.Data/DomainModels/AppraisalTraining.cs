using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AppraisalTraining
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("appraisalId")]
        public long AppraisalId { get; set; }
        [Column("appraisalEmployeeId")]
        public long AppraisalEmployeeId { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("AppraisalTraining")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("AppraisalId")]
        [InverseProperty("AppraisalTraining")]
        public virtual Appraisal Appraisal { get; set; }
        [ForeignKey("AppraisalEmployeeId")]
        [InverseProperty("AppraisalTraining")]
        public virtual AppraisalEmployee AppraisalEmployee { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("AppraisalTraining")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("TrainingId")]
        [InverseProperty("AppraisalTraining")]
        public virtual SettingsTraining Training { get; set; }
    }
}
