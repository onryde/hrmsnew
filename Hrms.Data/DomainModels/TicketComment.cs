using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TicketComment
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("ticketId")]
        public long TicketId { get; set; }
        [Column("commentId")]
        public long CommentId { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }

        [ForeignKey("CommentId")]
        [InverseProperty("TicketComment")]
        public virtual Comment Comment { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("TicketComment")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("TicketId")]
        [InverseProperty("TicketComment")]
        public virtual Ticket Ticket { get; set; }
    }
}
