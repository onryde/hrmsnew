using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeDataVerification
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("updatedBy")]
        public long UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime UpdatedOn { get; set; }
        [Column("verifiedBy")]
        public long? VerifiedBy { get; set; }
        [Column("verifiedOn", TypeName = "datetime")]
        public DateTime? VerifiedOn { get; set; }
        [Required]
        [Column("employeeSection")]
        [StringLength(100)]
        public string EmployeeSection { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeDataVerification")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeDataVerificationEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeDataVerificationUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [ForeignKey("VerifiedBy")]
        [InverseProperty("EmployeeDataVerificationVerifiedByNavigation")]
        public virtual Employee VerifiedByNavigation { get; set; }
    }
}
