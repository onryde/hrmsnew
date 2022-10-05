using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsModuleAccess
    {
        //public SettingsModuleAccess()
        //{
        //    Employee = new HashSet<Employee>();
        //    //SettingsModuleAccess = new HashSet<SettingsModuleAccess>();
        //}


        [Column("id")]
        public int Id { get; set; }
        [Column("moduleid")]
        public int ModuleID { get; set; }
        [Column("roleid")]
        public int RoleId { get; set; }
        [Column("ismanager")]
        public int Ismanager { get; set; }
        [Column("employeeid")]
        public long EmployeeId { get; set; }
        [Column("canAccess")]
        public int CanAccess { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("companyId")]
        public long? CompanyId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsModuleAccessAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsModuleAccess")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsModuleAccessUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }


    }
}
