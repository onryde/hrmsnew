using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AnnouncementDepartment
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("announcementId")]
        public long? AnnouncementId { get; set; }
        [Column("departmentId")]
        public long? DepartmentId { get; set; }

        [ForeignKey("AnnouncementId")]
        [InverseProperty("AnnouncementDepartment")]
        public virtual Announcement Announcement { get; set; }
        [ForeignKey("DepartmentId")]
        [InverseProperty("AnnouncementDepartment")]
        public virtual SettingsDepartment Department { get; set; }
    }
}
