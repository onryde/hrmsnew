using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Data.Interfaces;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Hrms.Helper.StaticClasses;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Data.Repositories
{
    public class AuditRepo : BaseRepository<EmployeeAudit>, IAuditRepo
    {
        public AuditRepo(HrmsContext context) : base(context)
        {
        }

        public SelectOptionResponse GetAllAuditModules(BaseRequest request)
        {
            var datatable = new DataTable();
            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT * From Announcement";
                dbContext.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    datatable.Load(result);

                    result.Close();
                }
            }
            //ExcelHelper.GenerateExcel("Report", datatable, @"C:\temp\temp.xls");
            var modules = GetAll()
                .Select(var => var.Module).Distinct()
                .Select(var => new SelectOptionDto
                {
                    Value = var,
                    Label = var
                }).ToList();

            return new SelectOptionResponse
            {
                IsSuccess = true,
                Options = modules
            };
        }

        public AuditListResponse GetAllAudit(AuditFilterRequest request)
        {
            var isFilterUnSet = (request.EmployeeIds == null || request.EmployeeIds.Any())
                              && (request.StartDate == null)
                              && (request.EndDate == null)
                              && (request.VerifiedStartDate == null)
                              && (request.VerifiedEndDate == null)
                              && (request.Modules == null || request.Modules.Any())
                              && (request.VerifiedEmpIds == null || request.VerifiedEmpIds.Any())
                              && (request.VerifiedEmpIds == null || request.VerifiedEmpIds.Any())
                              && (request.UpdatedEmpIds == null || request.UpdatedEmpIds.Any());

            var audits = GetAll()
                .Where(var =>
                    (request.EmployeeIds == null || !request.EmployeeIds.Any() ||
                     (var.Emp != null && request.EmployeeIds.Contains(var.Emp.Guid)))
                    && (request.StartDate == null || !var.UpdatedDate.HasValue ||
                        var.UpdatedDate.Value.Date >= request.StartDate)
                    && (request.EndDate == null || !var.UpdatedDate.HasValue ||
                        var.UpdatedDate.Value.Date <= request.EndDate)
                    && (request.VerifiedStartDate == null || !var.VerifiedDate.HasValue ||
                        var.VerifiedDate.Value.Date >= request.VerifiedStartDate)
                    && (request.VerifiedEndDate == null || !var.VerifiedDate.HasValue ||
                        var.VerifiedDate.Value.Date <= request.VerifiedEndDate)
                    && (request.Modules == null || !request.Modules.Any() ||
                        request.Modules.Contains(var.Module))
                    && (request.VerifiedEmpIds == null || !request.VerifiedEmpIds.Any() ||
                         (var.VerifiedByNavigation != null && request.VerifiedEmpIds.Contains(var.VerifiedByNavigation.Guid)))
                    && (request.UpdatedEmpIds == null || !request.UpdatedEmpIds.Any() ||
                        (var.UpdatedByNavigation != null && request.UpdatedEmpIds.Contains(var.UpdatedByNavigation.Guid)))
                )
                .Select(var => new AuditDto
                {
                    EmployeeId = var.Emp != null ? var.Emp.Guid : string.Empty,
                    EmployeeCode = var.EmployeeCode,
                    EmployeeName = var.Emp != null ? var.Emp.Name : string.Empty,
                    Module = var.Module,
                    FieldName = var.FieldName,
                    NewValue = var.NewValue != null ? var.NewValue.ToString() : null,
                    OldValue = var.OldValue != null ? var.OldValue.ToString() : null,
                    UpdatedBy = var.UpdatedByNavigation != null ? var.UpdatedByNavigation.Name : string.Empty,
                    UpdatedDate = var.UpdatedDate,
                    VerifiedBy = var.VerifiedByNavigation != null ? var.VerifiedByNavigation.Name : string.Empty,
                    VerifiedDate = var.VerifiedDate
                })
                .OrderByDescending(var => var.UpdatedDate)
                .Take(200)
                .ToList();

            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(22) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(22) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            return new AuditListResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                AuditList = audits
            };
        }
    }
}
