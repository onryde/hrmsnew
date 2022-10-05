using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsAttachmentType
    {
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("attachmentType")]
        [StringLength(100)]
        public string AttachmentType { get; set; }
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

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsAttachmentTypeAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsAttachmentTypeUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
