using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AuditLog
    {
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("actionId")]
        public long ActionId { get; set; }
        [Column("moduleId")]
        public long? ModuleId { get; set; }
        [Column("datetime", TypeName = "datetime")]
        public DateTime Datetime { get; set; }
        [Column("performedBy")]
        public long PerformedBy { get; set; }
        [Column("text")]
        [StringLength(1000)]
        public string Text { get; set; }
        [Column("data")]
        [StringLength(1000)]
        public string Data { get; set; }

        [ForeignKey("PerformedBy")]
        [InverseProperty("AuditLog")]
        public virtual Employee PerformedByNavigation { get; set; }
    }
}
