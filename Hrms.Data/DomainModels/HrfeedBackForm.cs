using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    [Table("HRFeedBackForm")]
    public partial class HrfeedBackForm
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
        [InverseProperty("HrfeedBackFormAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("HrfeedBackFormEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("ExitId")]
        [InverseProperty("HrfeedBackForm")]
        public virtual EmployeeExit Exit { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("HrfeedBackFormUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
