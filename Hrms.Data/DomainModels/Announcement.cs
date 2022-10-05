using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class Announcement
    {
        public Announcement()
        {
            AnnouncementAttachment = new HashSet<AnnouncementAttachment>();
            AnnouncementDepartment = new HashSet<AnnouncementDepartment>();
            AnnouncementLocation = new HashSet<AnnouncementLocation>();
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
        [Column("typeId")]
        public long TypeId { get; set; }
        [Column("date", TypeName = "date")]
        public DateTime Date { get; set; }
        [Required]
        [Column("title")]
        [StringLength(500)]
        public string Title { get; set; }
        [Required]
        [Column("content")]
        [StringLength(4000)]
        public string Content { get; set; }
        [Column("isPublished")]
        public bool IsPublished { get; set; }
        [Column("endDate", TypeName = "date")]
        public DateTime? EndDate { get; set; }
        [Column("isHidden")]
        public bool? IsHidden { get; set; }
        [Column("startDate", TypeName = "date")]
        public DateTime? StartDate { get; set; }
        [Column("subTitle")]
        [StringLength(500)]
        public string SubTitle { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("AnnouncementAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("Announcement")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("TypeId")]
        [InverseProperty("Announcement")]
        public virtual SettingsAnnouncementType Type { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("AnnouncementUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Announcement")]
        public virtual ICollection<AnnouncementAttachment> AnnouncementAttachment { get; set; }
        [InverseProperty("Announcement")]
        public virtual ICollection<AnnouncementDepartment> AnnouncementDepartment { get; set; }
        [InverseProperty("Announcement")]
        public virtual ICollection<AnnouncementLocation> AnnouncementLocation { get; set; }
    }
}
