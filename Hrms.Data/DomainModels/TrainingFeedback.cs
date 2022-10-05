using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TrainingFeedback
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }
        [Column("nomineeId")]
        public long NomineeId { get; set; }
        [Column("questionId")]
        public long QuestionId { get; set; }
        [Column("answer")]
        [StringLength(500)]
        public string Answer { get; set; }

        [ForeignKey("NomineeId")]
        [InverseProperty("TrainingFeedback")]
        public virtual TrainingNominees Nominee { get; set; }
        [ForeignKey("QuestionId")]
        [InverseProperty("TrainingFeedback")]
        public virtual SettingsTrainingFeedbackQuestion Question { get; set; }
        [ForeignKey("TrainingId")]
        [InverseProperty("TrainingFeedback")]
        public virtual Training Training { get; set; }
    }
}
