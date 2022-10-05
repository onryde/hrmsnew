using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsTicketFaq
    {
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("title")]
        [StringLength(200)]
        public string Title { get; set; }
        [Required]
        [Column("explanation")]
        [StringLength(1000)]
        public string Explanation { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("ticketCategoryId")]
        public long? TicketCategoryId { get; set; }
        [Column("ticketSubCategoryId")]
        public long? TicketSubCategoryId { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsTicketFaqAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsTicketFaq")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsTicketFaqUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
