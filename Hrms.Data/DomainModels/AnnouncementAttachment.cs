using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AnnouncementAttachment
    {
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
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
        [Column("attachmentId")]
        public long AttachmentId { get; set; }
        [Column("announcementId")]
        public long AnnouncementId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("AnnouncementAttachmentAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("AnnouncementId")]
        [InverseProperty("AnnouncementAttachment")]
        public virtual Announcement Announcement { get; set; }
        [ForeignKey("AttachmentId")]
        [InverseProperty("AnnouncementAttachment")]
        public virtual Attachment Attachment { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("AnnouncementAttachmentUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
