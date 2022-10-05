using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeAudit
    {
        [Column("auditId")]
        public int AuditId { get; set; }
        public long? EmpId { get; set; }
        [StringLength(100)]
        public string EmployeeCode { get; set; }
        [StringLength(150)]
        public string Module { get; set; }
        [Column(TypeName = "sql_variant")]
        public object OldValue { get; set; }
        [Column(TypeName = "sql_variant")]
        public object NewValue { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? VerifiedDate { get; set; }
        [StringLength(150)]
        public string FieldName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public long? VerifiedBy { get; set; }

        [ForeignKey("EmpId")]
        [InverseProperty("EmployeeAuditEmp")]
        public virtual Employee Emp { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeAuditUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [ForeignKey("VerifiedBy")]
        [InverseProperty("EmployeeAuditVerifiedByNavigation")]
        public virtual Employee VerifiedByNavigation { get; set; }
    }
}
