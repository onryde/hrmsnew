using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsDepartmentDesignation
    {
        public SettingsDepartmentDesignation()
        {
            EmployeeCompany = new HashSet<EmployeeCompany>();
            EmployeeCompensation = new HashSet<EmployeeCompensation>();
            InverseReportingTo = new HashSet<SettingsDepartmentDesignation>();
            TrainingDesignation = new HashSet<TrainingDesignation>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("departmentId")]
        public long? DepartmentId { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Column("order")]
        public int? Order { get; set; }
        [Column("reportingToId")]
        public long? ReportingToId { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsDepartmentDesignationAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsDepartmentDesignation")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("DepartmentId")]
        [InverseProperty("SettingsDepartmentDesignation")]
        public virtual SettingsDepartment Department { get; set; }
        [ForeignKey("ReportingToId")]
        [InverseProperty("InverseReportingTo")]
        public virtual SettingsDepartmentDesignation ReportingTo { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsDepartmentDesignationUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Designation")]
        public virtual ICollection<EmployeeCompany> EmployeeCompany { get; set; }
        [InverseProperty("Designation")]
        public virtual ICollection<EmployeeCompensation> EmployeeCompensation { get; set; }
        [InverseProperty("ReportingTo")]
        public virtual ICollection<SettingsDepartmentDesignation> InverseReportingTo { get; set; }
        [InverseProperty("Designation")]
        public virtual ICollection<TrainingDesignation> TrainingDesignation { get; set; }
    }
}
