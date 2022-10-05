using System;
using System.Collections.Generic;
using System.Text;

namespace Hrms.Helper.Models.Dto
{
    public class AuditDto
    {
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Module { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
