using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeExitAnswers
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("employeeExitId")]
        public long EmployeeExitId { get; set; }
        [Column("exitQuestionId")]
        public long ExitQuestionId { get; set; }
        [Required]
        [Column("answer")]
        [StringLength(1000)]
        public string Answer { get; set; }

        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeExitAnswersEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("EmployeeExitId")]
        [InverseProperty("EmployeeExitAnswers")]
        public virtual EmployeeExit EmployeeExit { get; set; }
        [ForeignKey("ExitQuestionId")]
        [InverseProperty("EmployeeExitAnswers")]
        public virtual SettingsExitQuestion ExitQuestion { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeExitAnswersUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
