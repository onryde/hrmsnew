using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeBank
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Required]
        [Column("bankName")]
        [StringLength(100)]
        public string BankName { get; set; }
        [Required]
        [Column("bankAccountNumber")]
        [StringLength(100)]
        public string BankAccountNumber { get; set; }
        [Required]
        [Column("ifscCode")]
        [StringLength(100)]
        public string IfscCode { get; set; }
        [Required]
        [Column("bankBranch")]
        [StringLength(100)]
        public string BankBranch { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Required]
        [Column("accountType")]
        [StringLength(100)]
        public string AccountType { get; set; }
        [Column("effectiveDate", TypeName = "date")]
        public DateTime? EffectiveDate { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeBankAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeBank")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeBankEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeBankUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
