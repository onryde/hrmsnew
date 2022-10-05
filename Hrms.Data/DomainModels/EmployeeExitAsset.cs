using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeExitAsset
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("employeeExitId")]
        public long EmployeeExitId { get; set; }
        [Column("employeeAssetId")]
        public long? EmployeeAssetId { get; set; }
        [Column("assetBreakageFee", TypeName = "decimal(18, 2)")]
        public decimal? AssetBreakageFee { get; set; }
        [Column("status")]
        [StringLength(100)]
        public string Status { get; set; }
        [Column("assetOwner")]
        public long? AssetOwner { get; set; }
        [Column("manager")]
        public long? Manager { get; set; }
        [Column("seniorManager")]
        public long? SeniorManager { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        public long AssetTypeId { get; set; }
        [Column("comments")]
        [StringLength(200)]
        public string Comments { get; set; }
        [Column("HODComments")]
        [StringLength(500)]
        public string Hodcomments { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeExitAssetAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("AssetOwner")]
        [InverseProperty("EmployeeExitAssetAssetOwnerNavigation")]
        public virtual Employee AssetOwnerNavigation { get; set; }
        [ForeignKey("EmployeeAssetId")]
        [InverseProperty("EmployeeExitAsset")]
        public virtual EmployeeAsset EmployeeAsset { get; set; }
        [ForeignKey("EmployeeExitId")]
        [InverseProperty("EmployeeExitAsset")]
        public virtual EmployeeExit EmployeeExit { get; set; }
        [ForeignKey("Manager")]
        [InverseProperty("EmployeeExitAssetManagerNavigation")]
        public virtual Employee ManagerNavigation { get; set; }
        [ForeignKey("SeniorManager")]
        [InverseProperty("EmployeeExitAssetSeniorManagerNavigation")]
        public virtual Employee SeniorManagerNavigation { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeExitAssetUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
