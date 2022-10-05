using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeFamily
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
        [Column("relationship")]
        [StringLength(100)]
        public string Relationship { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("gender")]
        [StringLength(100)]
        public string Gender { get; set; }
        [Column("dob")]
        [StringLength(100)]
        public string Dob { get; set; }
        [Column("occupation")]
        [StringLength(100)]
        public string Occupation { get; set; }
        [Column("isDependant")]
        public bool? IsDependant { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("isEmergencyContact")]
        public bool? IsEmergencyContact { get; set; }
        [Column("isAlive")]
        public bool IsAlive { get; set; }
        [Column("emailId")]
        [StringLength(100)]
        public string EmailId { get; set; }
        [Column("address")]
        [StringLength(1000)]
        public string Address { get; set; }
        [Column("phone")]
        [StringLength(100)]
        public string Phone { get; set; }
        [StringLength(100)]
        public string Column1 { get; set; }
        [Column("isOptedForMediclaim")]
        public bool? IsOptedForMediclaim { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeFamilyAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeFamily")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeFamilyEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeFamilyUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
