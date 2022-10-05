using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeExitForm
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("exitId")]
        public long ExitId { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [StringLength(100)]
        public string TenureInKai { get; set; }
        [StringLength(100)]
        public string TotalExperience { get; set; }
        [StringLength(1000)]
        public string LikeAboutKai { get; set; }
        [StringLength(1000)]
        public string DislikeAboutKai { get; set; }
        [StringLength(1000)]
        public string ThingsKaiMustChange { get; set; }
        [StringLength(1000)]
        public string ThingsKaiMustContinue { get; set; }
        [StringLength(1000)]
        public string WhatPromptedToChange { get; set; }
        [StringLength(1000)]
        public string ReasonForLeavingKai { get; set; }
        public bool? RejoinKaiLater { get; set; }
        [StringLength(1000)]
        public string AssociateWhom { get; set; }
        [StringLength(1000)]
        public string WhichOrganization { get; set; }
        [StringLength(100)]
        public string Designation { get; set; }
        [StringLength(100)]
        public string Ctc { get; set; }
        [StringLength(100)]
        public string EmailId { get; set; }
        [StringLength(15)]
        public string MobileNumber { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [StringLength(100)]
        public string BankName { get; set; }
        [StringLength(100)]
        public string AccountNo { get; set; }
        [Column("IFSCCode")]
        [StringLength(20)]
        public string Ifsccode { get; set; }
        [StringLength(1000)]
        public string Address { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeExitFormAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeExitFormEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("ExitId")]
        [InverseProperty("EmployeeExitForm")]
        public virtual EmployeeExit Exit { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeExitFormUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
