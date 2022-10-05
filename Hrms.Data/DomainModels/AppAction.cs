using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AppAction
    {
        public AppAction()
        {
            SettingsRoleActionAccess = new HashSet<SettingsRoleActionAccess>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("action")]
        [StringLength(100)]
        public string Action { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Column("actionSlug")]
        [StringLength(100)]
        public string ActionSlug { get; set; }
        [Column("moduleId")]
        public long ModuleId { get; set; }

        [InverseProperty("Action")]
        public virtual ICollection<SettingsRoleActionAccess> SettingsRoleActionAccess { get; set; }
    }
}
