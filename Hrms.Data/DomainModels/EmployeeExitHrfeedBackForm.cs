using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    [Table("EmployeeExitHRFeedBackForm")]
    public partial class EmployeeExitHrfeedBackForm
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("exitId")]
        public long ExitId { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [StringLength(1000)]
        public string EmployeeThoughtOnKai { get; set; }
        [StringLength(1000)]
        public string EmployeeLikeToChange { get; set; }
        [StringLength(1000)]
        public string EmployeeRejoinLater { get; set; }
        [StringLength(1000)]
        public string SalaryAndDesignationOffered { get; set; }
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
        [InverseProperty("EmployeeExitHrfeedBackFormAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeExitHrfeedBackFormEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("ExitId")]
        [InverseProperty("EmployeeExitHrfeedBackForm")]
        public virtual EmployeeExit Exit { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeExitHrfeedBackFormUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
