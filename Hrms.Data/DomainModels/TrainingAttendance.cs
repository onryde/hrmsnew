using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TrainingAttendance
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }
        [Column("trainingDate")]
        public long TrainingDate { get; set; }
        [Column("trainingNominee")]
        public long TrainingNominee { get; set; }
        [Column("hasAttended")]
        public bool? HasAttended { get; set; }
        [Column("remark")]
        [StringLength(500)]
        public string Remark { get; set; }

        [ForeignKey("TrainingId")]
        [InverseProperty("TrainingAttendance")]
        public virtual Training Training { get; set; }
        [ForeignKey("TrainingDate")]
        [InverseProperty("TrainingAttendance")]
        public virtual TrainingDate TrainingDateNavigation { get; set; }
        [ForeignKey("TrainingNominee")]
        [InverseProperty("TrainingAttendance")]
        public virtual TrainingNominees TrainingNomineeNavigation { get; set; }
    }
}
