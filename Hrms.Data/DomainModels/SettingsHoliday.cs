using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsHoliday
    {
        public SettingsHoliday()
        {
            SettingsHolidayLocation = new HashSet<SettingsHolidayLocation>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("date", TypeName = "date")]
        public DateTime Date { get; set; }
        [Column("reason")]
        [StringLength(200)]
        public string Reason { get; set; }
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
        [Column("year")]
        public int? Year { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("holidayType")]
        [StringLength(100)]
        public string HolidayType { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsHolidayAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsHoliday")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsHolidayUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Holiday")]
        public virtual ICollection<SettingsHolidayLocation> SettingsHolidayLocation { get; set; }
    }
}
