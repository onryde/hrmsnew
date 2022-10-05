using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsAssetTypes
    {
        public SettingsAssetTypes()
        {
            EmployeeAsset = new HashSet<EmployeeAsset>();
            SettingsAssetTypeOwner = new HashSet<SettingsAssetTypeOwner>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("assetType")]
        [StringLength(1000)]
        public string AssetType { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsAssetTypesAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsAssetTypes")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsAssetTypesUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Asset")]
        public virtual ICollection<EmployeeAsset> EmployeeAsset { get; set; }
        [InverseProperty("AssetType")]
        public virtual ICollection<SettingsAssetTypeOwner> SettingsAssetTypeOwner { get; set; }
    }
}
