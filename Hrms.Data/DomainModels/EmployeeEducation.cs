using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeEducation
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("institute")]
        [StringLength(100)]
        public string Institute { get; set; }
        [Column("city")]
        [StringLength(100)]
        public string City { get; set; }
        [Column("state")]
        [StringLength(100)]
        public string State { get; set; }
        [Column("courseName")]
        [StringLength(100)]
        public string CourseName { get; set; }
        [Column("startedYear")]
        public int? StartedYear { get; set; }
        [Column("completedYear")]
        public int? CompletedYear { get; set; }
        [Column("majorSubject")]
        [StringLength(100)]
        public string MajorSubject { get; set; }
        [Column("marksPercentage")]
        public double? MarksPercentage { get; set; }
        [Column("grade")]
        [StringLength(100)]
        public string Grade { get; set; }
        [Column("courseType")]
        [StringLength(100)]
        public string CourseType { get; set; }
        [Column("courseDuration")]
        public int? CourseDuration { get; set; }
        [Column("employeeId")]
        public long? EmployeeId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeEducationAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeEducation")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeEducationEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeEducationUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
