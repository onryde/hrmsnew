using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Data.Interfaces;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Data.Repositories
{
    public class ReportRepo : BaseRepository<SettingsReport>, IReportRepo
    {
        public ReportRepo(HrmsContext context) : base(context)
        {
        }

        public ReportListResponse GetAllAvailableReports(BaseRequest request)
        {
            var reports = GetAll()
                .Include(var => var.SettingsReportInputs)
                .Where(var => var.IsActive)
                .Select(var => new ReportDto
                {
                    Name = var.ReportName,
                    Description = var.Description,
                    ReportId = var.Guid,
                    ReportInputs = var.SettingsReportInputs
                         .Select(var1 => new ReportInputDto()
                         {
                             Name = var1.FieldName,
                             Description = var1.Description,
                             ReportInputId = var1.Guid
                         })
                         .ToList()
                })
                .ToList();

            return new ReportListResponse
            {
                IsSuccess = true,
                Reports = reports
            };
        }

        public void DownloadReport(DownloadReportRequest request)
        {
            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT * From Announcement";
                using (var result = command.ExecuteReader())
                {

                }
            }

            if (!string.IsNullOrWhiteSpace(request.ReportId))
            {
                var report = GetAll()
                    .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.ReportId));
                if (report != null)
                {

                }
                else
                {
                    throw new Exception("Report not found");
                }
            }
            else
            {
                throw new Exception("Report not found");
            }
        }
    }
}
