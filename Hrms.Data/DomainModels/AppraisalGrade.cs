using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AppraisalGrade
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("appraisalId")]
        public long AppraisalId { get; set; }
        [Column("gradeId")]
        public long GradeId { get; set; }

        [ForeignKey("AppraisalId")]
        [InverseProperty("AppraisalGrade")]
        public virtual Appraisal Appraisal { get; set; }
        [ForeignKey("GradeId")]
        [InverseProperty("AppraisalGrade")]
        public virtual SettingsGrade Grade { get; set; }
    }
}
