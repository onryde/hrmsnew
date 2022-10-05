using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeCareer
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
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("employeeCode")]
        public string EmployeeCode { get; set; }
        [Column("addressingName")]
        public string AddressingName { get; set; }
        [Column("appraisalYear")]
        public long AppraisalYear { get; set; }
        [Column("appraisalType")]
        public string AppraisalType { get; set; }
        [Column("rating")]
        public long Rating { get; set; }
        [Column("Description")]
        public string Description { get; set; }
        [Column("gradeId")]
        public long GradeId { get; set; }
        [Column("designationId")]
        public long DesignationId { get; set; }
        [Column("departmentId")]
        public long DepartmentId { get; set; }
        [Column("locationId")]
        public long LocationId { get; set; }
        [Column("dateOfChange", TypeName = "date")]
        public DateTime? DateofChange { get; set; }
        [Column("effectiveFrom", TypeName = "date")]
        public DateTime? EffectiveFrom { get; set; }
        [Column("reasonForChange")]
        public string ReasonForChange { get; set; }
        [Column("RnR")]
        public string RnR { get; set; }
        [Column("remarks")]
        public string Remarks { get; set; }
        [Column("movementStatus")]
        public string MovementStatus { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("prelocation")]
        public string Prelocation { get; set; }
        [Column("predepartment")]
        public string Predepartment { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeCareerAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeCareer")]
        public virtual AppCompany Company { get; set; }

        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeCareerEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeCareerUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
