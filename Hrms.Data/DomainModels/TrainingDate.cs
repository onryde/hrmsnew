using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TrainingDate
    {
        public TrainingDate()
        {
            TrainingAttendance = new HashSet<TrainingAttendance>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }
        [Column("date", TypeName = "date")]
        public DateTime? Date { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }

        [ForeignKey("TrainingId")]
        [InverseProperty("TrainingDate")]
        public virtual Training Training { get; set; }
        [InverseProperty("TrainingDateNavigation")]
        public virtual ICollection<TrainingAttendance> TrainingAttendance { get; set; }
    }
}
