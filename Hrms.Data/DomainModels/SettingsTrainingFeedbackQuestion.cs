using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsTrainingFeedbackQuestion
    {
        public SettingsTrainingFeedbackQuestion()
        {
            TrainingFeedback = new HashSet<TrainingFeedback>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("question")]
        [StringLength(500)]
        public string Question { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [InverseProperty("Question")]
        public virtual ICollection<TrainingFeedback> TrainingFeedback { get; set; }
    }
}
