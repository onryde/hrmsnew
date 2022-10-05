using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AnnouncementLocation
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("announcementId")]
        public long? AnnouncementId { get; set; }
        [Column("locationId")]
        public long? LocationId { get; set; }

        [ForeignKey("AnnouncementId")]
        [InverseProperty("AnnouncementLocation")]
        public virtual Announcement Announcement { get; set; }
        [ForeignKey("LocationId")]
        [InverseProperty("AnnouncementLocation")]
        public virtual SettingsLocation Location { get; set; }
    }
}
