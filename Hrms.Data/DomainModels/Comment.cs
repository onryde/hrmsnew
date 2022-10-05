using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class Comment
    {
        public Comment()
        {
            TaskComment = new HashSet<TaskComment>();
            TicketComment = new HashSet<TicketComment>();
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
        [Required]
        [Column("content")]
        [StringLength(500)]
        public string Content { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("CommentAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("CommentUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Comment")]
        public virtual ICollection<TaskComment> TaskComment { get; set; }
        [InverseProperty("Comment")]
        public virtual ICollection<TicketComment> TicketComment { get; set; }
    }
}
