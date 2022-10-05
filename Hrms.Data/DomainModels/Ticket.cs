using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class Ticket
    {
        public Ticket()
        {
            TicketAttachment = new HashSet<TicketAttachment>();
            TicketComment = new HashSet<TicketComment>();
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
        [Column("categoryId")]
        public long? CategoryId { get; set; }
        [Column("subCategoryId")]
        public long? SubCategoryId { get; set; }
        [Required]
        [Column("title")]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        [Column("explanation")]
        [StringLength(1000)]
        public string Explanation { get; set; }
        [Column("statusId")]
        public long StatusId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("TicketAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CategoryId")]
        [InverseProperty("Ticket")]
        public virtual SettingsTicketCategory Category { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("Ticket")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("StatusId")]
        [InverseProperty("Ticket")]
        public virtual SettingsTicketStatus Status { get; set; }
        [ForeignKey("SubCategoryId")]
        [InverseProperty("Ticket")]
        public virtual SettingsTicketSubCategory SubCategory { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("TicketUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<TicketAttachment> TicketAttachment { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<TicketComment> TicketComment { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<TicketStatus> TicketStatus { get; set; }
    }
}
