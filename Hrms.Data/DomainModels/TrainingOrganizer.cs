using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TrainingOrganizer
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }

        [ForeignKey("EmployeeId")]
        [InverseProperty("TrainingOrganizer")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("TrainingId")]
        [InverseProperty("TrainingOrganizer")]
        public virtual Training Training { get; set; }
    }
}
