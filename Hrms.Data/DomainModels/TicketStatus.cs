using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TicketStatus
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("ticketId")]
        public long TicketId { get; set; }
        [Column("statusId")]
        public long StatusId { get; set; }
        [Column("changedOn", TypeName = "datetime")]
        public DateTime ChangedOn { get; set; }
        [Column("changedBy")]
        public long ChangedBy { get; set; }

        [ForeignKey("ChangedBy")]
        [InverseProperty("TicketStatus")]
        public virtual Employee ChangedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("TicketStatus")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("StatusId")]
        [InverseProperty("TicketStatus")]
        public virtual SettingsTicketStatus Status { get; set; }
        [ForeignKey("TicketId")]
        [InverseProperty("TicketStatus")]
        public virtual Ticket Ticket { get; set; }
    }
}
