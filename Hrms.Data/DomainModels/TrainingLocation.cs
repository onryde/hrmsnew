using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TrainingLocation
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("locationId")]
        public long LocationId { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }

        [ForeignKey("LocationId")]
        [InverseProperty("TrainingLocation")]
        public virtual SettingsLocation Location { get; set; }
        [ForeignKey("TrainingId")]
        [InverseProperty("TrainingLocation1")]
        public virtual Training Training { get; set; }
    }
}
