using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TrainingDesignation
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("designationId")]
        public long DesignationId { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }

        [ForeignKey("DesignationId")]
        [InverseProperty("TrainingDesignation")]
        public virtual SettingsDepartmentDesignation Designation { get; set; }
        [ForeignKey("TrainingId")]
        [InverseProperty("TrainingDesignation")]
        public virtual Training Training { get; set; }
    }
}
