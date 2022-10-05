using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    [Table("HODFeedBackForm")]
    public partial class HodfeedBackForm
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("exitId")]
        public long ExitId { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        public bool IsDesiredAttrition { get; set; }
        [StringLength(1000)]
        public string IntentionToLeaveKai { get; set; }
        [StringLength(1000)]
        public string AttemptsToRetainEmployee { get; set; }
        [Column("ELigibleToRehire")]
        [StringLength(1000)]
        public string EligibleToRehire { get; set; }
        [StringLength(1000)]
        public string Comments { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("HodfeedBackFormAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("HodfeedBackFormEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("ExitId")]
        [InverseProperty("HodfeedBackForm")]
        public virtual EmployeeExit Exit { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("HodfeedBackFormUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
