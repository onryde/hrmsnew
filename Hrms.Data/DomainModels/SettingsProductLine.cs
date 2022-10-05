using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsProductLine
    {
        public SettingsProductLine()
        {
            EmployeeCompany = new HashSet<EmployeeCompany>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("productLine")]
        [StringLength(100)]
        public string ProductLine { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("companyId")]
        public long? CompanyId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsProductLineAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsProductLine")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsProductLineUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("ProductLine")]
        public virtual ICollection<EmployeeCompany> EmployeeCompany { get; set; }
    }
}
