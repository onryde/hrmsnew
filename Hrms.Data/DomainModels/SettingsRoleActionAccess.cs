using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsRoleActionAccess
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("roleid")]
        public long? Roleid { get; set; }
        [Column("actionId")]
        public long? ActionId { get; set; }
        [Column("canAccess")]
        public bool? CanAccess { get; set; }
        [Column("companyId")]
        public long? CompanyId { get; set; }

        [ForeignKey("ActionId")]
        [InverseProperty("SettingsRoleActionAccess")]
        public virtual AppAction Action { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsRoleActionAccess")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("Roleid")]
        [InverseProperty("SettingsRoleActionAccess")]
        public virtual SettingsRole Role { get; set; }
    }
}
