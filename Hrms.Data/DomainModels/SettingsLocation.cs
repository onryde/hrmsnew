using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsLocation
    {
        public SettingsLocation()
        {
            AnnouncementLocation = new HashSet<AnnouncementLocation>();
            EmployeeCompany = new HashSet<EmployeeCompany>();
            SettingsHolidayLocation = new HashSet<SettingsHolidayLocation>();
            Training = new HashSet<Training>();
            TrainingLocation = new HashSet<TrainingLocation>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Required]
        [Column("name")]
        [StringLength(200)]
        public string Name { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
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
        [Column("gstNumber")]
        [StringLength(100)]
        public string GstNumber { get; set; }
        [Column("phone")]
        [StringLength(100)]
        public string Phone { get; set; }
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }
        [Column("address")]
        [StringLength(1000)]
        public string Address { get; set; }
        [Column("state")]
        [StringLength(100)]
        public string State { get; set; }
        [Column("country")]
        [StringLength(100)]
        public string Country { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsLocationAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsLocation")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsLocationUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Location")]
        public virtual ICollection<AnnouncementLocation> AnnouncementLocation { get; set; }
        [InverseProperty("Location")]
        public virtual ICollection<EmployeeCompany> EmployeeCompany { get; set; }
        [InverseProperty("Location")]
        public virtual ICollection<SettingsHolidayLocation> SettingsHolidayLocation { get; set; }
        [InverseProperty("TrainingLocationNavigation")]
        public virtual ICollection<Training> Training { get; set; }
        [InverseProperty("Location")]
        public virtual ICollection<TrainingLocation> TrainingLocation { get; set; }
    }
}
