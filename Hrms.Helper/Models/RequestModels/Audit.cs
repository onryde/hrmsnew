using System;
using System.Collections.Generic;
using System.Text;
using Hrms.Helper.Models.Contracts;

namespace Hrms.Helper.Models.RequestModels
{
    public class AuditFilterRequest: BaseRequest
    {
        public List<string> EmployeeIds { get; set; }
        public List<string> Modules { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? VerifiedStartDate { get; set; }
        public DateTime? VerifiedEndDate { get; set; }
        public List<string> VerifiedEmpIds { get; set; }
        public List<string> UpdatedEmpIds { get; set; }
    }
}
