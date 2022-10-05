using System;
using System.Collections.Generic;
using System.Text;
using Hrms.Helper.Settings;

namespace Hrms.Helper.Models.Dto
{
    public class AuditInfo
    {
        public EventLogs ActionId { get; set; }
        public long PerformedBy { get; set; }
        public string AuditText { get; set; }
        public string Data { get; set; }
    }
}
