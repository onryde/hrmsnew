using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeDocument
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
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
        [Column("documentTypeId")]
        public long DocumentTypeId { get; set; }
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("type")]
        [StringLength(100)]
        public string Type { get; set; }
        [Column("size")]
        public double? Size { get; set; }
        [Column("fileLocation")]
        [StringLength(1000)]
        public string FileLocation { get; set; }
        [Column("fileUrl")]
        [StringLength(1000)]
        public string FileUrl { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeDocumentAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeDocument")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("DocumentTypeId")]
        [InverseProperty("EmployeeDocument")]
        public virtual SettingsDocumentType DocumentType { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeDocumentEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeDocumentUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
