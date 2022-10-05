using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsCategory
    {
        public SettingsCategory()
        {
            EmployeeCompany = new HashSet<EmployeeCompany>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("category")]
        [StringLength(100)]
        public string Category { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsCategoryAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsCategory")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsCategoryUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Category")]
        public virtual ICollection<EmployeeCompany> EmployeeCompany { get; set; }
    }
}
