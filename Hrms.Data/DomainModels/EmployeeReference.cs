using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeReference
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
        [Column("employeeId")]
        public long? EmployeeId { get; set; }
        [Column("referenceName")]
        [StringLength(100)]
        public string ReferenceName { get; set; }
        [Column("designation")]
        [StringLength(100)]
        public string Designation { get; set; }
        [Column("address")]
        [StringLength(1000)]
        public string Address { get; set; }
        [Column("phone")]
        [StringLength(100)]
        public string Phone { get; set; }
        [Column("remarks")]
        [StringLength(1000)]
        public string Remarks { get; set; }
        [Column("company")]
        [StringLength(100)]
        public string Company { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("emailId")]
        [StringLength(100)]
        public string EmailId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeReferenceAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeReference")]
        public virtual AppCompany CompanyNavigation { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeReferenceEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeReferenceUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
