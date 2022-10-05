using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsTeam
    {
        public SettingsTeam()
        {
            EmployeeCompany = new HashSet<EmployeeCompany>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Required]
        [Column("name")]
        [StringLength(200)]
        public string Name { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsTeamAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsTeam")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsTeamUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Team")]
        public virtual ICollection<EmployeeCompany> EmployeeCompany { get; set; }
    }
}
