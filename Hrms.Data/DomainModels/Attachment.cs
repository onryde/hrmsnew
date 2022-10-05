using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class Attachment
    {
        public Attachment()
        {
            AnnouncementAttachment = new HashSet<AnnouncementAttachment>();
            TicketAttachment = new HashSet<TicketAttachment>();
        }

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
        [Required]
        [Column("fileName")]
        [StringLength(200)]
        public string FileName { get; set; }
        [Required]
        [Column("contentType")]
        [StringLength(200)]
        public string ContentType { get; set; }
        [Required]
        [Column("attachmentUrl")]
        [StringLength(1000)]
        public string AttachmentUrl { get; set; }
        [Column("attachmentSize")]
        public double AttachmentSize { get; set; }
        [Required]
        [Column("attachmentType")]
        [StringLength(20)]
        public string AttachmentType { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("AttachmentAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("AttachmentUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Attachment")]
        public virtual ICollection<AnnouncementAttachment> AnnouncementAttachment { get; set; }
        [InverseProperty("Attachment")]
        public virtual ICollection<TicketAttachment> TicketAttachment { get; set; }
    }
}
