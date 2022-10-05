using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TicketAttachment
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
        [Column("ticketId")]
        public long TicketId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("TicketAttachmentAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("AttachmentId")]
        [InverseProperty("TicketAttachment")]
        public virtual Attachment Attachment { get; set; }
        [ForeignKey("TicketId")]
        [InverseProperty("TicketAttachment")]
        public virtual Ticket Ticket { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("TicketAttachmentUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
