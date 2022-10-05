using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TrainingGrade
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("gradeId")]
        public long GradeId { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }

        [ForeignKey("GradeId")]
        [InverseProperty("TrainingGrade")]
        public virtual SettingsGrade Grade { get; set; }
        [ForeignKey("TrainingId")]
        [InverseProperty("TrainingGrade")]
        public virtual Training Training { get; set; }
    }
}
