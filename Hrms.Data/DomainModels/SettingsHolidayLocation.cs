using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsHolidayLocation
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("holidayId")]
        public long HolidayId { get; set; }
        [Column("locationId")]
        public long LocationId { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsHolidayLocation")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("HolidayId")]
        [InverseProperty("SettingsHolidayLocation")]
        public virtual SettingsHoliday Holiday { get; set; }
        [ForeignKey("LocationId")]
        [InverseProperty("SettingsHolidayLocation")]
        public virtual SettingsLocation Location { get; set; }
    }
}
