using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class TaskComment
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("taskId")]
        public long TaskId { get; set; }
        [Column("commentId")]
        public long CommentId { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }

        [ForeignKey("CommentId")]
        [InverseProperty("TaskComment")]
        public virtual Comment Comment { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("TaskComment")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("TaskId")]
        [InverseProperty("TaskComment")]
        public virtual Task Task { get; set; }
    }
}
