using System;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;

namespace Hrms.Data.Interfaces
{
    public interface IEventLogRepo
    {
        void AddAuditLog(AuditInfo auditInfo);
        EventLogResponse GetAllAuditLogs(EventLogRequest request);
    }
}
