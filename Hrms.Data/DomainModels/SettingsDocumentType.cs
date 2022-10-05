using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsDocumentType
    {
        public SettingsDocumentType()
        {
            EmployeeDocument = new HashSet<EmployeeDocument>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("documentType")]
        [StringLength(100)]
        public string DocumentType { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Column("isRestricted")]
        public bool IsRestricted { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsDocumentTypeAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsDocumentType")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsDocumentTypeUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("DocumentType")]
        public virtual ICollection<EmployeeDocument> EmployeeDocument { get; set; }
    }
}
