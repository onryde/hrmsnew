using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsAssetTypeOwner
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("assetTypeId")]
        public long AssetTypeId { get; set; }
        [Column("ownerId")]
        public long OwnerId { get; set; }

        [ForeignKey("AssetTypeId")]
        [InverseProperty("SettingsAssetTypeOwner")]
        public virtual SettingsAssetTypes AssetType { get; set; }
        [ForeignKey("OwnerId")]
        [InverseProperty("SettingsAssetTypeOwner")]
        public virtual Employee Owner { get; set; }
    }
}
