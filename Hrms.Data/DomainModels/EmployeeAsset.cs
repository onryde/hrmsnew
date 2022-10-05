using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeAsset
    {
        public EmployeeAsset()
        {
            EmployeeExitAsset = new HashSet<EmployeeExitAsset>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("assetId")]
        public long AssetId { get; set; }
        [Column("givenOn", TypeName = "datetime")]
        public DateTime? GivenOn { get; set; }
        [Column("description")]
        [StringLength(200)]
        public string Description { get; set; }
        [Column("assetUniqueId")]
        [StringLength(100)]
        public string AssetUniqueId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }
        [Column("isSignedOff")]
        public bool? IsSignedOff { get; set; }
        [Column("signedOffOn", TypeName = "datetime")]
        public DateTime? SignedOffOn { get; set; }
        [Column("signedOffBy")]
        public long? SignedOffBy { get; set; }
        [Column("signOffComment")]
        [StringLength(1000)]
        public string SignOffComment { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeAssetAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("AssetId")]
        [InverseProperty("EmployeeAsset")]
        public virtual SettingsAssetTypes Asset { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeAsset")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeAssetEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeAssetUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("EmployeeAsset")]
        public virtual ICollection<EmployeeExitAsset> EmployeeExitAsset { get; set; }
    }
}
