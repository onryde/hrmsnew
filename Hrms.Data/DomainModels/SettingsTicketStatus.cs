using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsTicketStatus
    {
        public SettingsTicketStatus()
        {
            Ticket = new HashSet<Ticket>();
            TicketStatus = new HashSet<TicketStatus>();
        }

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
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsTicketStatusAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsTicketStatus")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsTicketStatusUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Status")]
        public virtual ICollection<Ticket> Ticket { get; set; }
        [InverseProperty("Status")]
        public virtual ICollection<TicketStatus> TicketStatus { get; set; }
    }
}
