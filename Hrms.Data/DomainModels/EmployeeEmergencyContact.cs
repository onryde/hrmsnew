using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeEmergencyContact
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
        public long UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime UpdatedOn { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [Column("relationship")]
        [StringLength(100)]
        public string Relationship { get; set; }
        [Required]
        [Column("contactNumber")]
        [StringLength(100)]
        public string ContactNumber { get; set; }
        [Column("address")]
        [StringLength(100)]
        public string Address { get; set; }
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeEmergencyContactAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeEmergencyContact")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeEmergencyContactEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeEmergencyContactUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
