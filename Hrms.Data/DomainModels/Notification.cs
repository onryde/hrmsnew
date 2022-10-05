using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class Notification
    {
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("notificationTime", TypeName = "datetime")]
        public DateTime NotificationTime { get; set; }
        [Column("notificationTo")]
        public long NotificationTo { get; set; }
        [Column("content")]
        [StringLength(1000)]
        public string Content { get; set; }
        [Column("actionId")]
        public int ActionId { get; set; }
        [Column("notificationData")]
        [StringLength(1000)]
        public string NotificationData { get; set; }
        [Column("isRead")]
        public bool IsRead { get; set; }
        [Column("readOn", TypeName = "datetime")]
        public DateTime? ReadOn { get; set; }

        [ForeignKey("NotificationTo")]
        [InverseProperty("Notification")]
        public virtual Employee NotificationToNavigation { get; set; }
    }
}
