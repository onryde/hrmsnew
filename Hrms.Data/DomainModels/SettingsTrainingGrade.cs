using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsTrainingGrade
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }
        [Column("gradeId")]
        public long GradeId { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsTrainingGrade")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("GradeId")]
        [InverseProperty("SettingsTrainingGrade")]
        public virtual SettingsGrade Grade { get; set; }
        [ForeignKey("TrainingId")]
        [InverseProperty("SettingsTrainingGrade")]
        public virtual SettingsTraining Training { get; set; }
    }
}
