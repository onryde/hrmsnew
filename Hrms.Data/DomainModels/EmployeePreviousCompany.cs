using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeePreviousCompany
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
        public long EmployeeId { get; set; }
        [Column("employer")]
        [StringLength(100)]
        public string Employer { get; set; }
        [Column("designation")]
        [StringLength(100)]
        public string Designation { get; set; }
        [Column("department")]
        [StringLength(100)]
        public string Department { get; set; }
        [Column("doj", TypeName = "date")]
        public DateTime? Doj { get; set; }
        [Column("doe", TypeName = "date")]
        public DateTime? Doe { get; set; }
        [Column("duration")]
        public int? Duration { get; set; }
        [Column("reasonForChange")]
        [StringLength(1000)]
        public string ReasonForChange { get; set; }
        [Column("lastCtc")]
        public double? LastCtc { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeePreviousCompanyAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeePreviousCompany")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeePreviousCompanyEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeePreviousCompanyUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
