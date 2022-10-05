using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsAnnouncementType
    {
        public SettingsAnnouncementType()
        {
            Announcement = new HashSet<Announcement>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("type")]
        [StringLength(100)]
        public string Type { get; set; }
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
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsAnnouncementTypeAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsAnnouncementType")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsAnnouncementTypeUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Type")]
        public virtual ICollection<Announcement> Announcement { get; set; }
    }
}
