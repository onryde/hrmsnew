using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsDepartment
    {
        public SettingsDepartment()
        {
            AnnouncementDepartment = new HashSet<AnnouncementDepartment>();
            EmployeeCompany = new HashSet<EmployeeCompany>();
            EmployeeCompensation = new HashSet<EmployeeCompensation>();
            SettingsDepartmentDesignation = new HashSet<SettingsDepartmentDesignation>();
            TrainingDepartment = new HashSet<TrainingDepartment>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(63)]
        public string Guid { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Column("frontColor")]
        [StringLength(6)]
        public string FrontColor { get; set; }
        [Column("backColor")]
        [StringLength(6)]
        public string BackColor { get; set; }
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
        [Column("companyId")]
        public long CompanyId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsDepartmentAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsDepartment")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsDepartmentUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<AnnouncementDepartment> AnnouncementDepartment { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<EmployeeCompany> EmployeeCompany { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<EmployeeCompensation> EmployeeCompensation { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<SettingsDepartmentDesignation> SettingsDepartmentDesignation { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<TrainingDepartment> TrainingDepartment { get; set; }
    }
}
