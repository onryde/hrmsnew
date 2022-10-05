using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TrainingDepartment
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("departmentId")]
        public long DepartmentId { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }

        [ForeignKey("DepartmentId")]
        [InverseProperty("TrainingDepartment")]
        public virtual SettingsDepartment Department { get; set; }
        [ForeignKey("TrainingId")]
        [InverseProperty("TrainingDepartment")]
        public virtual Training Training { get; set; }
    }
}
