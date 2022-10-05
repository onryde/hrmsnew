using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsRole
    {
        public SettingsRole()
        {
            Employee = new HashSet<Employee>();
            SettingsRoleActionAccess = new HashSet<SettingsRoleActionAccess>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("roleName")]
        [StringLength(100)]
        public string RoleName { get; set; }
        [Column("isAdmin")]
        public bool IsAdmin { get; set; }
        [Required]
        [Column("roleSlug")]
        [StringLength(100)]
        public string RoleSlug { get; set; }
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
        [InverseProperty("SettingsRoleAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsRole")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsRoleUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<Employee> Employee { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<SettingsRoleActionAccess> SettingsRoleActionAccess { get; set; }
    }
}
