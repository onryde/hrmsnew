using System;
using System.Linq;
using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Data.Interfaces;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Hrms.Helper.StaticClasses;
using Newtonsoft.Json;

namespace Hrms.Data.Repositories
{
    public class EventLogRepo : BaseRepository<AuditLog>, IEventLogRepo
    {
        public EventLogRepo(HrmsContext context) : base(context)
        {
        }
        public void AddAuditLog(AuditInfo auditInfo)
        {
            AddItem(new AuditLog
            {
                Guid = CustomGuid.NewGuid(),
                ActionId = (int)auditInfo.ActionId,
                Datetime = DateTime.UtcNow,
                PerformedBy = auditInfo.PerformedBy,
                Text = auditInfo.AuditText,
                Data = JsonConvert.SerializeObject(auditInfo.Data)
            });

            //Save();
        }

        public EventLogResponse GetAllAuditLogs(EventLogRequest request)
        {
            if (request.UserRole.Equals("Admin"))
            {
                var logs = GetAll()
                    .Select(var => new EventLogDto
                    {
                        ActionId = var.ActionId,
                        Data = var.Data,
                        Datetime = var.Datetime,
                        PerformedBy = var.PerformedByNavigation.Name
                    })
                    .ToList();

                return new EventLogResponse
                {
                    IsSuccess = true,
                    EventLogs = logs
                };
            }

            return new EventLogResponse
            {
                IsSuccess = true
            };
        }
    }
}
