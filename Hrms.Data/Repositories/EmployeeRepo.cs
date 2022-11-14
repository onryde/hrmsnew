using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Data.Interfaces;
using Hrms.Helper.EmailHelper;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Hrms.Helper.Settings;
using Hrms.Helper.StaticClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Hrms.Data.Repositories
{
    public class EmployeeRepo : BaseRepository<Employee>, IEmployeeRepo
    {
        private IEventLogRepo _eventLogRepo;
        public EmployeeRepo(HrmsContext context, IEventLogRepo eventLogRepo) : base(context)
        {
            _eventLogRepo = eventLogRepo;
        }

        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            var employee = GetAll()
                .FirstOrDefault(var => var.Id == request.UserIdNum);

            if (employee == null)
                throw new Exception("Employee not found. Id - " + request.UserIdNum);

            var encCurPassword = Cryptography.Encrypt(request.CurrentPassword, employee.PasswordSalt);
            if (encCurPassword.Equals(employee.Password) ||
                ((employee.IsTemporaryPasswordSet ?? false) && encCurPassword.Equals(employee.TemporaryPassword)))
            {
                var encNewPassword = Cryptography.Encrypt(request.NewPassword, employee.PasswordSalt);
                employee.Password = encNewPassword;
                employee.IsTemporaryPasswordSet = false;

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.ChangedPassword.Template, request.UserName, request.UserId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.ChangedPassword.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                return new ChangePasswordResponse
                {
                    IsSuccess = true,
                    IsPasswordChanged = true
                };
            }

            return new ChangePasswordResponse
            {
                IsSuccess = true,
                IsPasswordChanged = false
            };
        }

        public EmployeeListResponse GetAllEmployees(EmployeeListFilterRequest request)
        {
            var employees =
                GetAll()
                    .Include(var => var.Role)
                    .Include(var => var.EmployeeCompanyEmployee)
                    .Include(var => var.EmployeeDataVerificationEmployee)
                    .Include(var => var.EmployeeContactEmployee)
                    .Where(var => var.IsActive
                                  && var.EmployeeCompanyEmployee != null
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                                  && (var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned == null || var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned == false)
                                  && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                                  && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                                  && (string.IsNullOrWhiteSpace(request.Code) ||
                                      var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .EmployeeCode.Contains(request.Code) ||
                                      var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .OffRoleCode.Contains(request.Code))
                                  && (request.Status == null || !request.Status.Any() ||
                                      request.Status.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Status))
                                  && (string.IsNullOrWhiteSpace(request.Phone) ||
                                      var.EmployeeContactEmployee.Any(var1 =>
                                          var1.OfficialNumber.Contains(request.Phone)))
                                  && (request.Locations == null || !request.Locations.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Location != null &&
                                      request.Locations.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Location.Guid)))
                                  && (request.Grades == null || !request.Grades.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Grade != null &&
                                      request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                                                     .Grade.Guid)))
                                  && (request.Departments == null || !request.Departments.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Department != null &&
                                      request.Departments.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Department.Guid)))
                                  && (request.Designations == null || !request.Designations.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Designation != null &&
                                      request.Designations.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Designation.Guid)))
                                  && (request.Roles == null || !request.Roles.Any() ||
                                      request.Roles.Any(var1 =>
                                          var1 == var.Role.Guid))
                    )
                    .Select(var => new EmployeeDto
                    {
                        Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                        Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                        Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                        Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                        Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                        Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                        EmployeeId = var.Guid,
                        EmailId = var.EmailId,
                        Name = var.Name,
                        CanLogin = var.CanLogin ?? false,
                        Status = var.EmployeeCompanyEmployee.FirstOrDefault().Status,
                        Role = var.Role.RoleName,
                        Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                            ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                            : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                        IsVerificationPending = var.EmployeeDataVerificationEmployee.Any(var1 => var1.IsActive && var1.VerifiedByNavigation == null)
                    })
                    .ToList();

            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(23) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(23) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;


            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();



            return new EmployeeListResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                Employees = employees
            };
        }

        public EmployeeRptBdayResponse GetEmployeeRptBday(EmployeeReportFilterRequest request)
        {
            var employees =
                GetAll()
                    .Include(var => var.Role)
                    .Include(var => var.EmployeeCompanyEmployee)
                    .Include(var => var.EmployeeDataVerificationEmployee)
                    .Include(var => var.EmployeeContactEmployee)
                    .Include(var => var.EmployeePersonalEmployee)
                    .Where(var => var.IsActive
                                  && var.EmployeeCompanyEmployee != null
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                                  && var.EmployeePersonalEmployee.FirstOrDefault().DobActual.HasValue
                                  && var.EmployeePersonalEmployee.FirstOrDefault().DobActual.Value.Birthdayday(request.FromDate)
                                  && (var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned == null || var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned == false)
                                  && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                                  && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                                  && (string.IsNullOrWhiteSpace(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .EmployeeCode.Contains(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .OffRoleCode.Contains(request.Code))
                                          && (request.Status == null || !request.Status.Any() ||
                                      request.Status.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault().Status))
                                  && (string.IsNullOrWhiteSpace(request.Phone) || var.EmployeeContactEmployee.Any(var1 =>
                                          var1.OfficialNumber.Contains(request.Phone)))
                                  && (request.Locations == null || !request.Locations.Any() || (var.EmployeeCompanyEmployee.FirstOrDefault().Location != null &&
                                      request.Locations.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Location.Guid))) && (request.Grades == null || !request.Grades.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault().Grade != null &&
                                      request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                                                     .Grade.Guid)))
                                  && (request.Departments == null || !request.Departments.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Department != null &&
                                      request.Departments.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Department.Guid)))
                                  && (request.Designations == null || !request.Designations.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Designation != null &&
                                      request.Designations.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Designation.Guid)))
                                  && (request.Roles == null || !request.Roles.Any() ||
                                      request.Roles.Any(var1 =>
                                          var1 == var.Role.Guid))
                    )
                    .Select(var => new EmployeeRptBdayDto
                    {
                        Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                        Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                        Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                        Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                        Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                        Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                        EmployeeId = var.Guid,
                        EmailId = var.EmailId,
                        Name = var.Name,
                        Status = var.EmployeeCompanyEmployee.FirstOrDefault().Status,
                        Role = var.Role.RoleName,
                        Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                            ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                            : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                        BdayDate = var.EmployeePersonalEmployee.FirstOrDefault().DobActual.Value
                    })
                    .OrderBy(var => var.BdayDate.Value.Day)
                    .ToList();

            var empid = request.UserIdNum;
            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(23) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(23) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new EmployeeRptBdayResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                EmployeeRptBday = employees
            };
        }

        public EmployeeRptWdayResponse GetEmployeeRptWday(EmployeeReportFilterRequest request)
        {
            var employees =
                GetAll()
                    .Include(var => var.Role)
                    .Include(var => var.EmployeeCompanyEmployee)
                    .Include(var => var.EmployeeDataVerificationEmployee)
                    .Include(var => var.EmployeeContactEmployee)
                    .Include(var => var.EmployeePersonalEmployee)
                    .Where(var => var.IsActive
                                  && var.EmployeeCompanyEmployee != null
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                                  && var.EmployeePersonalEmployee.FirstOrDefault().MaritalStatus == "Married"
                                  && var.EmployeePersonalEmployee.FirstOrDefault().MarriageDate.HasValue
                                  && var.EmployeePersonalEmployee.FirstOrDefault().MarriageDate.Value.Birthdayday(request.FromDate)
                                  && (var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned == null || var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned == false)
                                  && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                                  && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                                  && (string.IsNullOrWhiteSpace(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .EmployeeCode.Contains(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .OffRoleCode.Contains(request.Code))
                                          && (request.Status == null || !request.Status.Any() ||
                                      request.Status.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault().Status))
                                  && (string.IsNullOrWhiteSpace(request.Phone) || var.EmployeeContactEmployee.Any(var1 =>
                                          var1.OfficialNumber.Contains(request.Phone)))
                                  && (request.Locations == null || !request.Locations.Any() || (var.EmployeeCompanyEmployee.FirstOrDefault().Location != null &&
                                      request.Locations.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Location.Guid))) && (request.Grades == null || !request.Grades.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault().Grade != null &&
                                      request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                                                     .Grade.Guid)))
                                  && (request.Departments == null || !request.Departments.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Department != null &&
                                      request.Departments.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Department.Guid)))
                                  && (request.Designations == null || !request.Designations.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Designation != null &&
                                      request.Designations.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Designation.Guid)))
                                  && (request.Roles == null || !request.Roles.Any() ||
                                      request.Roles.Any(var1 =>
                                          var1 == var.Role.Guid))
                    //&& (var.EmployeePersonalEmployee.FirstOrDefault().DobActual.Value.Birthdayday(request.FromDate.Date))
                    )
                    .Select(var => new EmployeeRptWdayDto
                    {
                        Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                        Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                        Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                        Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                        Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                        Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                        EmployeeId = var.Guid,
                        EmailId = var.EmailId,
                        Name = var.Name,
                        Status = var.EmployeeCompanyEmployee.FirstOrDefault().Status,
                        Role = var.Role.RoleName,
                        Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                            ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                            : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                        WdayDate = var.EmployeePersonalEmployee.FirstOrDefault().MarriageDate.Value
                    })
                    .OrderBy(var => var.WdayDate.Value.Day)
                    .ToList();

            var empid = request.UserIdNum;
            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(23) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(23) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new EmployeeRptWdayResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                EmployeeRptWday = employees
            };
        }

        public EmployeeRptHeadCountResponse GetEmployeeRptHeadCount(EmployeeReportFilterRequest request)
        {

            var allEmployees = GetAll()
                    .Include(var => var.EmployeePersonalEmployee)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                    .Where(var => var.IsActive && var.EmployeeCompanyEmployee != null && var.EmployeeCompanyEmployee.Any()
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Doj.HasValue
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Doj.Value <= request.FromDate
                    ).ToList();

            var distinctLocations = allEmployees
            .Where(var => var.EmployeeCompanyEmployee != null && var.EmployeeCompanyEmployee.FirstOrDefault()?.Location != null)
            .Select(var => new { LocationId = var.EmployeeCompanyEmployee.FirstOrDefault().LocationId, LocationName = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name })
            .Distinct();

            var locationWiseList = new List<HeadCountrptDto>();

            foreach (var location in distinctLocations)
            {
                var onRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "on-roll"
                && var.EmployeeCompanyEmployee.FirstOrDefault().LocationId == location.LocationId);

                var offRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "off-roll"
                && var.EmployeeCompanyEmployee.FirstOrDefault().LocationId == location.LocationId);

                var expatriateCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "expatriate"
                && var.EmployeeCompanyEmployee.FirstOrDefault().LocationId == location.LocationId);

                var traineeCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "trainee"
                && var.EmployeeCompanyEmployee.FirstOrDefault().LocationId == location.LocationId);

                var CasualCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "casual"
                && var.EmployeeCompanyEmployee.FirstOrDefault().LocationId == location.LocationId);

                locationWiseList.Add(new HeadCountrptDto
                {
                    Type = location.LocationName,
                    OffRollCount = offRollCount,
                    OnRollCount = onRollCount,
                    Expatriate = expatriateCount,
                    Trainee = traineeCount,
                    CasualorTemp = CasualCount
                });
            }

            var distinctDepartments = allEmployees
                    .Where(var => var.EmployeeCompanyEmployee != null && var.EmployeeCompanyEmployee.FirstOrDefault()?.Department != null)
                    .Select(var => new { DepartmentId = var.EmployeeCompanyEmployee.FirstOrDefault().DepartmentId, DepartmentName = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name })
                    .Distinct();

            var departmentWiseList = new List<HeadCountrptDto>();

            foreach (var department in distinctDepartments)
            {
                var onRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "on-roll"
                && var.EmployeeCompanyEmployee.FirstOrDefault().DepartmentId == department.DepartmentId);

                var offRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "off-roll"
                && var.EmployeeCompanyEmployee.FirstOrDefault().DepartmentId == department.DepartmentId);

                var expatriateCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "expatriate"
                && var.EmployeeCompanyEmployee.FirstOrDefault().DepartmentId == department.DepartmentId);

                var traineeCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "trainee"
                && var.EmployeeCompanyEmployee.FirstOrDefault().DepartmentId == department.DepartmentId);

                var CasualCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "casual"
                && var.EmployeeCompanyEmployee.FirstOrDefault().DepartmentId == department.DepartmentId);

                departmentWiseList.Add(new HeadCountrptDto
                {
                    Type = department.DepartmentName,
                    OffRollCount = offRollCount,
                    OnRollCount = onRollCount,
                    Expatriate = expatriateCount,
                    Trainee = traineeCount,
                    CasualorTemp = CasualCount
                });
            }

            var distinctGrades = allEmployees
                    .Where(var => var.EmployeeCompanyEmployee != null && var.EmployeeCompanyEmployee.FirstOrDefault()?.Grade != null)
                    .Select(var => new { GradeId = var.EmployeeCompanyEmployee.FirstOrDefault().GradeId, Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade })
                    .Distinct();

            var gradeWiseList = new List<HeadCountrptDto>();

            foreach (var grade in distinctGrades)
            {
                var onRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "on-roll"
                && var.EmployeeCompanyEmployee.FirstOrDefault().GradeId == grade.GradeId);

                var offRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "off-roll"
                && var.EmployeeCompanyEmployee.FirstOrDefault().GradeId == grade.GradeId);

                var expatriateCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "expatriate"
                && var.EmployeeCompanyEmployee.FirstOrDefault().GradeId == grade.GradeId);

                var traineeCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "trainee"
                && var.EmployeeCompanyEmployee.FirstOrDefault().GradeId == grade.GradeId);

                var CasualCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "casual"
                && var.EmployeeCompanyEmployee.FirstOrDefault().GradeId == grade.GradeId);

                gradeWiseList.Add(new HeadCountrptDto
                {
                    Type = grade.Grade,
                    OffRollCount = offRollCount,
                    OnRollCount = onRollCount,
                    Expatriate = expatriateCount,
                    Trainee = traineeCount,
                    CasualorTemp = CasualCount
                });
            }

            var distinctDivision = allEmployees
                 .Where(var => var.EmployeeCompanyEmployee != null && var.EmployeeCompanyEmployee.FirstOrDefault()?.Division != null)
                 .Select(var => new { Division = var.EmployeeCompanyEmployee.FirstOrDefault().Division })
                 .Distinct();

            var divisionWiseList = new List<HeadCountrptDto>();

            foreach (var division in distinctDivision)
            {
                var onRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "on-roll"
                && var.EmployeeCompanyEmployee.FirstOrDefault().Division == division.Division);

                var offRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "off-roll"
                && var.EmployeeCompanyEmployee.FirstOrDefault().Division == division.Division);

                var expatriateCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "expatriate"
                && var.EmployeeCompanyEmployee.FirstOrDefault().Division == division.Division);

                var traineeCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "trainee"
                && var.EmployeeCompanyEmployee.FirstOrDefault().Division == division.Division);

                var CasualCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "casual"
                && var.EmployeeCompanyEmployee.FirstOrDefault().Division == division.Division);

                divisionWiseList.Add(new HeadCountrptDto
                {
                    Type = division.Division,
                    OffRollCount = offRollCount,
                    OnRollCount = onRollCount,
                    Expatriate = expatriateCount,
                    Trainee = traineeCount,
                    CasualorTemp = CasualCount
                });
            }

            var empid = request.UserIdNum;
            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(2) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(2) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new EmployeeRptHeadCountResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                LocationWiseRptHeadCount = locationWiseList,
                DepartmentWiseRptHeadCount = departmentWiseList,
                GradeWiseRptHeadCount = gradeWiseList,
                DivisionWiseRptHeadCount = divisionWiseList
            };
        }

        public EmployeeRptAddandExitResponse GetEmployeeRptAddandExit(EmployeeReportFilterRequest request)
        {

            var adddet =
                GetAll()
                    .Include(var => var.Role)
                    .Include(var => var.EmployeeCompanyEmployee)
                    .Include(var => var.EmployeeDataVerificationEmployee)
                    .Include(var => var.EmployeeContactEmployee)
                    .Include(var => var.EmployeePersonalEmployee)
                    .Where(var => var.IsActive
                                  && var.EmployeeCompanyEmployee != null
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().Doj.HasValue
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().Doj.Value >= request.FromDate
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().Doj.Value <= request.ToDate
                                  && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                                  && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                                  && (string.IsNullOrWhiteSpace(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .EmployeeCode.Contains(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .OffRoleCode.Contains(request.Code))
                                          && (request.Status == null || !request.Status.Any() ||
                                      request.Status.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault().Status))
                                  && (string.IsNullOrWhiteSpace(request.Phone) || var.EmployeeContactEmployee.Any(var1 =>
                                          var1.OfficialNumber.Contains(request.Phone)))
                                  && (request.Locations == null || !request.Locations.Any() || (var.EmployeeCompanyEmployee.FirstOrDefault().Location != null &&
                                      request.Locations.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Location.Guid))) && (request.Grades == null || !request.Grades.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault().Grade != null &&
                                      request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                                                     .Grade.Guid)))
                                  && (request.Departments == null || !request.Departments.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Department != null &&
                                      request.Departments.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Department.Guid)))
                                  && (request.Designations == null || !request.Designations.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Designation != null &&
                                      request.Designations.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Designation.Guid)))
                                  && (request.Roles == null || !request.Roles.Any() ||
                                      request.Roles.Any(var1 =>
                                          var1 == var.Role.Guid))
                    )
                    .Select(var => new AddandExitrptDto
                    {
                        Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                        Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                        Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                        Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                        Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                        Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                        EmployeeId = var.Guid,
                        EmailId = var.EmailId,
                        Name = var.Name,
                        Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                            ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                            : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                        addExitDate = var.EmployeeCompanyEmployee.FirstOrDefault().Doj.Value
                    })
                    .OrderBy(var => var.addExitDate.Value)
                    .ToList();


            var exitdet =
            GetAll()
                .Include(var => var.Role)
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.EmployeeExitEmployee)
                .Where(var => var.IsActive
                              && var.EmployeeCompanyEmployee != null
                              && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                              && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                              && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                              && var.EmployeeExitEmployee.FirstOrDefault().ActualRelievingDate.HasValue
                              && var.EmployeeExitEmployee.FirstOrDefault().ActualRelievingDate.Value >= request.FromDate
                              && var.EmployeeExitEmployee.FirstOrDefault().ActualRelievingDate.Value <= request.ToDate
                              && (string.IsNullOrWhiteSpace(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .EmployeeCode.Contains(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .OffRoleCode.Contains(request.Code))
                                      && (request.Status == null || !request.Status.Any() ||
                                  request.Status.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault().Status))
                              && (string.IsNullOrWhiteSpace(request.Phone) || var.EmployeeContactEmployee.Any(var1 =>
                                      var1.OfficialNumber.Contains(request.Phone)))
                              && (request.Locations == null || !request.Locations.Any() || (var.EmployeeCompanyEmployee.FirstOrDefault().Location != null &&
                                  request.Locations.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Location.Guid))) && (request.Grades == null || !request.Grades.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault().Grade != null &&
                                  request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                                                 .Grade.Guid)))
                              && (request.Departments == null || !request.Departments.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .Department != null &&
                                  request.Departments.Any(var1 =>
                                      var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Department.Guid)))
                              && (request.Designations == null || !request.Designations.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .Designation != null &&
                                  request.Designations.Any(var1 =>
                                      var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Designation.Guid)))
                              && (request.Roles == null || !request.Roles.Any() ||
                                  request.Roles.Any(var1 =>
                                      var1 == var.Role.Guid))
                )
                .Select(var => new AddandExitrptDto
                {
                    Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                    Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                    Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                    Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                    Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                    Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                    EmployeeId = var.Guid,
                    EmailId = var.EmailId,
                    Name = var.Name,
                    Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                        ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                        : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                    addExitDate = var.EmployeeExitEmployee.FirstOrDefault().ActualRelievingDate.Value
                })
                .OrderBy(var => var.addExitDate.Value)
                .ToList();

            var empid = request.UserIdNum;
            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;
            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new EmployeeRptAddandExitResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                AdditionDetails = adddet,
                ExitDetails = exitdet
            };
        }

        public EmployeeRptProbationResponse GetEmployeeRptProbation(EmployeeReportFilterRequest request)
        {
            var employees =
                GetAll()
                    //.Include(var => var.Role)
                    .Include(var => var.EmployeeCompanyEmployee)
                    //.Include(var => var.EmployeeDataVerificationEmployee)
                    .Include(var => var.EmployeeContactEmployee)
                    .Where(var => var.IsActive
                                  && var.EmployeeCompanyEmployee != null
                                  //&& var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().Doj.HasValue
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().Doj.Value.Date >= request.FromDate
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().ProbationEndDate.HasValue
                                  && var.EmployeeCompanyEmployee.FirstOrDefault().ProbationEndDate.Value.Date <= request.ToDate
                                  && (var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned == null || var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned == false)
                                  && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                                  && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                                  && (string.IsNullOrWhiteSpace(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .EmployeeCode.Contains(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .OffRoleCode.Contains(request.Code))
                                          && (request.Status == null || !request.Status.Any() ||
                                      request.Status.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault().Status))
                                  && (string.IsNullOrWhiteSpace(request.Phone) || var.EmployeeContactEmployee.Any(var1 =>
                                          var1.OfficialNumber.Contains(request.Phone)))
                                  && (request.Locations == null || !request.Locations.Any() || (var.EmployeeCompanyEmployee.FirstOrDefault().Location != null &&
                                      request.Locations.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Location.Guid))) && (request.Grades == null || !request.Grades.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault().Grade != null &&
                                      request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                                                     .Grade.Guid)))
                                  && (request.Departments == null || !request.Departments.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Department != null &&
                                      request.Departments.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Department.Guid)))
                                  && (request.Designations == null || !request.Designations.Any() ||
                                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Designation != null &&
                                      request.Designations.Any(var1 =>
                                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                              .Designation.Guid)))
                                  && (request.Roles == null || !request.Roles.Any() ||
                                      request.Roles.Any(var1 =>
                                          var1 == var.Role.Guid))
                    )
                    .Select(var => new ProbationrptDto
                    {
                        Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                        Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                        Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                        Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                        Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                        Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                        EmployeeId = var.Guid,
                        EmailId = var.EmailId,
                        Name = var.Name,
                        Status = var.EmployeeCompanyEmployee.FirstOrDefault().Status,
                        Role = var.Role.RoleName,
                        Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                            ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                            : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                        DateofJoing = var.EmployeeCompanyEmployee.FirstOrDefault().Doj.Value,
                        ConfirmationDueDate = var.EmployeeCompanyEmployee.FirstOrDefault().Doj.Value.Date.AddDays(184),
                        ExtenstionPeriod = var.EmployeeCompanyEmployee.FirstOrDefault().ProbationExtension,
                        ConfirmationDueDateExtended = var.EmployeeCompanyEmployee.FirstOrDefault().ProbationEndDate.Value,
                        ConfirmationRemarks = var.EmployeeCompanyEmployee.FirstOrDefault().ConfirmationRemarks
                    })
                    .OrderBy(var => var.DateofJoing.Value)
                    .ToList();

            var empid = request.UserIdNum;
            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(2) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(2) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new EmployeeRptProbationResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                ProbationDetails = employees
            };
        }

        public EmployeeRptBasicResponse GetEmployeeRptBasic(EmployeeReportFilterRequest request)
        {
            var employees = (from employee in GetAll()
                             join empCompany in dbContext.EmployeeCompany on employee.Id equals empCompany.EmployeeId
                             join empContact in dbContext.EmployeeContact on employee.Id equals empContact.EmployeeId
                             join empPersonal in dbContext.EmployeePersonal on employee.Id equals empPersonal.EmployeeId
                             join empReport in dbContext.EmployeeCompany on empCompany.ReportingToId equals empReport.EmployeeId into rptdet
                             from empRpt in rptdet.DefaultIfEmpty()
                             where employee.IsActive && empCompany != null
                                  && empCompany.EmployeeCode != null
                                  && (empCompany.IsResigned == null || empCompany.IsResigned == false)
                                  && (string.IsNullOrWhiteSpace(request.Name) || empCompany.AddressingName.Contains(request.Name))
                                  && (string.IsNullOrWhiteSpace(request.Email) || employee.EmailId.Contains(request.Email))
                                  && (string.IsNullOrWhiteSpace(request.Code) ||
                                      empCompany.EmployeeCode.Contains(request.Code) ||
                                      empCompany.OffRoleCode.Contains(request.Code))
                                  && (request.Status == null || !request.Status.Any() ||
                                      request.Status.Any(var1 =>
                                          var1 == empCompany.Status))
                                  && (string.IsNullOrWhiteSpace(request.Phone) ||
                                      empContact.OfficialNumber.Contains(request.Phone))
                                  && (request.Locations == null || !request.Locations.Any() ||
                                      (empCompany.Location != null &&
                                      request.Locations.Any(var1 =>
                                          var1 == empCompany.Location.Guid)))
                                  && (request.Grades == null || !request.Grades.Any() ||
                                      (empCompany.Grade != null &&
                                      request.Grades.Any(var1 => var1 == empCompany.Grade.Guid)))
                                  && (request.Departments == null || !request.Departments.Any() ||
                                      (empCompany.Department != null &&
                                      request.Departments.Any(var1 =>
                                          var1 == empCompany.Department.Guid)))
                                  && (request.Designations == null || !request.Designations.Any() ||
                                      (empCompany.Designation != null &&
                                      request.Designations.Any(var1 =>
                                          var1 == empCompany.Designation.Guid)))
                                  && (request.Roles == null || !request.Roles.Any() ||
                                      request.Roles.Any(var1 =>
                                          var1 == employee.Role.Guid))
                             select new BasicrptDto
                             {
                                 EmployeeId = employee.Guid,
                                 Code = empCompany.Status.Equals("on-roll")
                                        ? empCompany.EmployeeCode
                                        : empCompany.OffRoleCode,
                                 Name = empCompany.AddressingName,
                                 EmailId = employee.EmailId,
                                 Location = empCompany.Location.Name,
                                 Department = empCompany.Department.Name,
                                 Designation = empCompany.Designation.Name,
                                 Grade = empCompany.Grade.Grade,
                                 Region = empCompany.Region.Name,
                                 Team = empCompany.Team.Name,
                                 Status = empCompany.Status,
                                 Role = employee.Role.RoleName,
                                 DateofJoing = empCompany.Doj.Value,
                                 PerEmailId = empContact.PersonalEmailId,
                                 PerPhone = empContact.ContactNumber,
                                 OffPhone = empContact.OfficialNumber,
                                 PresentAddr = empContact.PresentAddress.DoorNo + ", " + empContact.PresentAddress.StreetName + ", " + empContact.PresentAddress.City + ", " + empContact.PresentAddress.District + ", " + empContact.PresentAddress.State + "- " + empContact.PresentAddress.Pincode,
                                 PermenantAddr = empContact.PermanentAddress.DoorNo + ", " + empContact.PermanentAddress.StreetName + ", " + empContact.PermanentAddress.City + ", " + empContact.PermanentAddress.District + ", " + empContact.PermanentAddress.State + "- " + empContact.PermanentAddress.Pincode,
                                 RMCode = empCompany.ReportingToId != null ? empCompany.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode : null,
                                 SMCode = empRpt.ReportingToId != null ? empRpt.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode : null,
                                 //SMCode = empReport.ReportingToId != null && empReport.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != null
                                 //                  ? empCompany.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo.FirstOrDefault().EmployeeCode : null,
                                 RMEmailId = empCompany.ReportingToId != null ? empCompany.ReportingTo.EmailId : null,
                                 SMEmailId = empCompany.ReportingToId != null
                                                   && empCompany.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != null
                                                   ? empCompany.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo.EmailId : null,
                                 RMName = empCompany.ReportingToId != null ? empCompany.ReportingTo.Name : null,
                                 SMName = empCompany.ReportingToId != null
                                                   && empCompany.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != null
                                                   ? empCompany.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo.Name : null,
                             }
                            ).OrderBy(var => var.Code).ToList();

            var empid = request.UserIdNum;
            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(23) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(23) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new EmployeeRptBasicResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                EmployeebasicDetails = employees
            };
        }

        public EmployeeRptCTCResponse GetEmployeeRptCTC(EmployeeReportFilterRequest request)
        {
            var employees = (from employee in GetAll()
                             join empCompany in dbContext.EmployeeCompany on employee.Id equals empCompany.EmployeeId
                             join empContact in dbContext.EmployeeContact on employee.Id equals empContact.EmployeeId
                             join empPersonal in dbContext.EmployeePersonal on employee.Id equals empPersonal.EmployeeId
                             where employee.IsActive && empCompany != null
                                  && empCompany.EmployeeCode != null
                                  && (empCompany.IsResigned == null || empCompany.IsResigned == false)
                                  && (string.IsNullOrWhiteSpace(request.Name) || empCompany.AddressingName.Contains(request.Name))
                                  && (string.IsNullOrWhiteSpace(request.Email) || employee.EmailId.Contains(request.Email))
                                  && (string.IsNullOrWhiteSpace(request.Code) ||
                                      empCompany.EmployeeCode.Contains(request.Code) ||
                                      empCompany.OffRoleCode.Contains(request.Code))
                                  && (request.Status == null || !request.Status.Any() ||
                                      request.Status.Any(var1 =>
                                          var1 == empCompany.Status))
                                  && (string.IsNullOrWhiteSpace(request.Phone) ||
                                      empContact.OfficialNumber.Contains(request.Phone))
                                  && (request.Locations == null || !request.Locations.Any() ||
                                      (empCompany.Location != null &&
                                      request.Locations.Any(var1 =>
                                          var1 == empCompany.Location.Guid)))
                                  && (request.Grades == null || !request.Grades.Any() ||
                                      (empCompany.Grade != null &&
                                      request.Grades.Any(var1 => var1 == empCompany.Grade.Guid)))
                                  && (request.Departments == null || !request.Departments.Any() ||
                                      (empCompany.Department != null &&
                                      request.Departments.Any(var1 =>
                                          var1 == empCompany.Department.Guid)))
                                  && (request.Designations == null || !request.Designations.Any() ||
                                      (empCompany.Designation != null &&
                                      request.Designations.Any(var1 =>
                                          var1 == empCompany.Designation.Guid)))
                                  && (request.Roles == null || !request.Roles.Any() ||
                                      request.Roles.Any(var1 =>
                                          var1 == employee.Role.Guid))
                             select new CTCrptDto
                             {
                                 EmployeeId = employee.Id.ToString(),
                                 Code = empCompany.Status.Equals("on-roll")
                                        ? empCompany.EmployeeCode
                                        : empCompany.OffRoleCode,
                                 Name = empCompany.AddressingName,
                                 EmailId = employee.EmailId,
                                 Location = empCompany.Location.Name,
                                 LocationBifurcation = empCompany.LocationBifurcation,
                                 Department = empCompany.Department.Name,
                                 Designation = empCompany.Designation.Name,
                                 Grade = empCompany.Grade.Grade,
                                 Region = empCompany.Region.Name,
                                 Team = empCompany.Team.Name,
                                 Status = empCompany.Status,
                                 Role = employee.Role.RoleName,
                                 DateofJoing = empCompany.Doj.Value
                             }
                            ).OrderBy(var => var.Code).ToList();

            var maxEdu = (from edu in dbContext.EmployeeEducation
                          where edu.CourseType == "Regular" && edu.CompletedYear != null
                          group edu by edu.EmployeeId into empedugrp
                          let completedyr = empedugrp.Max(x => x.CompletedYear)
                          select new CTCrptDto
                          {
                              EmployeeId = empedugrp.Key.ToString(),
                              EducationDetails = empedugrp.First(s => s.CompletedYear == completedyr).CourseName,
                              //CompletedYear = completedyr
                          }).ToList();

            var maxCTC = (from ctc in dbContext.EmployeeCompensation 
                          group ctc by ctc.EmployeeId into empctcgrp
                          let ctcyr = empctcgrp.Max(x => x.Year)
                          select new CTCrptDto
                          {
                              EmployeeId = empctcgrp.Key.ToString(),
                              CTC = empctcgrp.First(s => s.Year == ctcyr).AnnualCtc
                          }).ToList();

            var minExp = (from exp in dbContext.EmployeePreviousCompany 
                          group exp by exp.EmployeeId into empexpgrp
                          let dojval = empexpgrp.Min(x => x.Doj)
                          select new CTCrptDto
                          {
                              EmployeeId = empexpgrp.Key.ToString(),
                              DateofJoing = empexpgrp.First(s => s.Doj == dojval).Doj.Value
                          }).ToList();
            var combine = (from emp in employees
                           join empedu in maxEdu on emp.EmployeeId equals empedu.EmployeeId
                           join empCtc in maxCTC on emp.EmployeeId equals empCtc.EmployeeId
                           join empexp in minExp on emp.EmployeeId equals empexp.EmployeeId 
                           //into expdet from empexpdet in expdet.DefaultIfEmpty()
                           select new CTCrptDto
                           {
                               EmployeeId = emp.EmployeeId,
                               Code = emp.Code,
                               Name = emp.Name,
                               EmailId = emp.EmailId,
                               Location = emp.Location,
                               LocationBifurcation = emp.LocationBifurcation,
                               Department = emp.Department,
                               Designation = emp.Designation,
                               Grade = emp.Grade,
                               Region = emp.Region,
                               Team = emp.Team,
                               Status = emp.Status,
                               Role = emp.Role,
                               DateofJoing = emp.DateofJoing,
                               EducationDetails = empedu.EducationDetails,
                               CTC = empCtc.CTC,
                               DateofJoingExp = empexp.DateofJoing
                               //TotalyrExp = empexpdet.DateofJoing != null && empexpdet.DateofJoing != DateTime.MinValue ? (DateTime.Today.Year - empexpdet.DateofJoing.Value.Year) : 0
                           }).ToList();

            var empid = request.UserIdNum;
            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(14) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(14) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new EmployeeRptCTCResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                EmployeeCTCDetails = combine
            };
        }

        public EmployeeRptResignedResponse GetEmployeeRptResigned(EmployeeReportFilterRequest request)
        {
            var employees = (from employee in GetAll()
                             join empCompany in dbContext.EmployeeCompany on employee.Id equals empCompany.EmployeeId
                             join empExit in dbContext.EmployeeExit on empCompany.EmployeeId equals empExit.EmployeeId
                             join empExitHODFeedback in dbContext.EmployeeExitHodfeedBackForm on empExit.Id equals empExitHODFeedback.ExitId
                             where empCompany.EmployeeCode != null
                             && empExit.Status == "Completed"
                             && (string.IsNullOrWhiteSpace(request.Name) || empCompany.AddressingName.Contains(request.Name))
                             && (string.IsNullOrWhiteSpace(request.Email) || empCompany.AddressingName.Contains(request.Email))
                             && empExit.ActualRelievingDate.HasValue
                             && empExit.ActualRelievingDate.Value >= request.FromDate
                             && empExit.ActualRelievingDate.Value <= request.ToDate
                             select new ResignedrptDto
                             {
                                 Name = employee.Name,
                                 Code = empCompany.Status.Equals("on-roll") ? empCompany.EmployeeCode : empCompany.OffRoleCode,
                                 Department = empCompany.Department.Name,
                                 Grade = empCompany.Grade.Grade,
                                 Location = empCompany.Location.Name,
                                 Designation = empCompany.Designation.Name,
                                 Region = empCompany.Region.Name,
                                 Team = empCompany.Team.Name,
                                 EmployeeId = employee.Guid,
                                 EmailId = employee.EmailId,
                                 DateofConfirmation = empExit.ActualRelievingDate.Value,
                                 DateofResignation = empExit.ResignedOn,
                                 Desired = empExitHODFeedback != null ? empExitHODFeedback.IsDesiredAttrition.Equals(0) ? "Undesired" : "Desired" : null,
                                 Status = empCompany.Status,
                                 ExitStatus = empCompany.IsResigned.Equals(0) ? "Live" : "Resigned",
                                 ExitClearanceStatus = empExit.Status
                             }).ToList();

                             //var employees =
                             //        GetAll()
                             //    .Include(var => var.EmployeeCompanyEmployee)
                             //    .Include(var => var.EmployeeExitEmployee).ThenInclude(var => var.EmployeeExitHodfeedBackForm)
                             //    .Where(var => var.EmployeeCompanyEmployee != null
                             //                  && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                             //                  && var.EmployeeExitEmployee.FirstOrDefault().Status == "Completed"
                             //                  && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                             //                  && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                             //                  && var.EmployeeExitEmployee.FirstOrDefault().ActualRelievingDate.HasValue
                             //                  && var.EmployeeExitEmployee.FirstOrDefault().ActualRelievingDate.Value >= request.FromDate
                             //                  && var.EmployeeExitEmployee.FirstOrDefault().ActualRelievingDate.Value <= request.ToDate
                             //                  && (string.IsNullOrWhiteSpace(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                             //                          .EmployeeCode.Contains(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                             //                          .OffRoleCode.Contains(request.Code))
                             //                          && (request.Status == null || !request.Status.Any() ||
                             //                      request.Status.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault().Status))
                             //                  && (string.IsNullOrWhiteSpace(request.Phone) || var.EmployeeContactEmployee.Any(var1 =>
                             //                          var1.OfficialNumber.Contains(request.Phone)))
                             //                  && (request.Locations == null || !request.Locations.Any() || (var.EmployeeCompanyEmployee.FirstOrDefault().Location != null &&
                             //                      request.Locations.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                             //                              .Location.Guid))) && (request.Grades == null || !request.Grades.Any() ||
                             //                      (var.EmployeeCompanyEmployee.FirstOrDefault().Grade != null &&
                             //                      request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                             //                                                     .Grade.Guid)))
                             //                  && (request.Departments == null || !request.Departments.Any() ||
                             //                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                             //                          .Department != null &&
                             //                      request.Departments.Any(var1 =>
                             //                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                             //                              .Department.Guid)))
                             //                  && (request.Designations == null || !request.Designations.Any() ||
                             //                      (var.EmployeeCompanyEmployee.FirstOrDefault()
                             //                          .Designation != null &&
                             //                      request.Designations.Any(var1 =>
                             //                          var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                             //                              .Designation.Guid)))
                             //                  && (request.Roles == null || !request.Roles.Any() ||
                             //                      request.Roles.Any(var1 =>
                             //                          var1 == var.Role.Guid))
                             //    )
                             //    .Select(var => new ResignedrptDto
                             //    {
                             //        Name = var.Name,
                             //        Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                             //            ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                             //            : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                             //        Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                             //        Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                             //        Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                             //        Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                             //        Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                             //        Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                             //        EmployeeId = var.Guid,
                             //        EmailId = var.EmailId,
                             //        DateofConfirmation = var.EmployeeExitEmployee.FirstOrDefault().ActualRelievingDate.Value,
                             //        DateofResignation = var.EmployeeExitEmployee.FirstOrDefault().ResignedOn,
                             //        Desired = var.EmployeeExitHodfeedBackFormEmployee != null ? var.EmployeeExitHodfeedBackFormEmployee.FirstOrDefault().IsDesiredAttrition.Equals(0) ? "Undesired" : "Desired" : null,
                             //        Status = var.EmployeeCompanyEmployee.FirstOrDefault().Status,
                             //        ExitStatus = var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned.Equals(0) ? "Live" : "Resigned",
                             //        ExitClearanceStatus = var.EmployeeExitEmployee.FirstOrDefault().Status
                             //    })
                             //    .OrderBy(var => var.DateofResignation.Value)
                             //    .ToList();

            var empid = request.UserIdNum;
            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new EmployeeRptResignedResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                EmployeeresignedDetails = employees
            };
        }
        public EmployeeRptObjectiveResponse GetEmployeeRptObjective(EmployeeReportFilterRequest request)
        {

            var objemployees =
                    GetAll()
                .Include(var => var.Role)
                .Include(var => var.EmployeeCompanyEmployee)
                //.Include(var => var.EmployeeDataVerificationEmployee)
                //.Include(var => var.EmployeeContactEmployee)
                //.Include(var => var.EmployeePersonalEmployee)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .Where(var => var.IsActive
                              && var.EmployeeCompanyEmployee != null
                              && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                              && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                              && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                              && (string.IsNullOrWhiteSpace(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .EmployeeCode.Contains(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .OffRoleCode.Contains(request.Code))
                                      && (request.Status == null || !request.Status.Any() ||
                                  request.Status.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault().Status))
                              && (string.IsNullOrWhiteSpace(request.Phone) || var.EmployeeContactEmployee.Any(var1 =>
                                      var1.OfficialNumber.Contains(request.Phone)))
                              && (request.Locations == null || !request.Locations.Any() || (var.EmployeeCompanyEmployee.FirstOrDefault().Location != null &&
                                  request.Locations.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Location.Guid))) && (request.Grades == null || !request.Grades.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault().Grade != null &&
                                  request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                                                 .Grade.Guid)))
                              && (request.Departments == null || !request.Departments.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .Department != null &&
                                  request.Departments.Any(var1 =>
                                      var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Department.Guid)))
                              && (request.Designations == null || !request.Designations.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .Designation != null &&
                                  request.Designations.Any(var1 =>
                                      var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Designation.Guid)))
                              && (request.Roles == null || !request.Roles.Any() ||
                                  request.Roles.Any(var1 =>
                                      var1 == var.Role.Guid))
                                      && var.AppraisalEmployeeEmployee.FirstOrDefault().Appraisal.Year == request.FromDate.Year
                && var.AppraisalEmployeeEmployee.FirstOrDefault().Appraisal.Mode == 1
                )
                .Select(var => new ObjectiverptDto
                {
                    Name = var.Name,
                    Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                        ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                        : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                    Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                    Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                    Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                    Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                    Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                    Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                    EmployeeId = var.Guid,
                    EmailId = var.EmailId,
                    Role = var.EmployeeCompanyEmployee.FirstOrDefault().Status,
                    Status = var.IsActive == true ? "Live" : "Resigned",
                    SelfSubmitted = var.AppraisalEmployeeEmployee.FirstOrDefault().SelfObjectiveSubmittedOn,
                    RMSubmitted = var.AppraisalEmployeeEmployee.FirstOrDefault().RmObjectiveSubmittedOn,
                    HODSubmitted = var.AppraisalEmployeeEmployee.FirstOrDefault().L2ObjectiveSubmittedOn
                })
                .OrderBy(var => var.Code)
                .ToList();

            var vbemployees =
                    GetAll()
                .Include(var => var.Role)
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .Where(var => var.IsActive
                              && var.EmployeeCompanyEmployee != null
                              && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                              && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                              && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                              && (string.IsNullOrWhiteSpace(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .EmployeeCode.Contains(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .OffRoleCode.Contains(request.Code))
                                      && (request.Status == null || !request.Status.Any() ||
                                  request.Status.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault().Status))
                              && (string.IsNullOrWhiteSpace(request.Phone) || var.EmployeeContactEmployee.Any(var1 =>
                                      var1.OfficialNumber.Contains(request.Phone)))
                              && (request.Locations == null || !request.Locations.Any() || (var.EmployeeCompanyEmployee.FirstOrDefault().Location != null &&
                                  request.Locations.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Location.Guid))) && (request.Grades == null || !request.Grades.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault().Grade != null &&
                                  request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                                                 .Grade.Guid)))
                              && (request.Departments == null || !request.Departments.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .Department != null &&
                                  request.Departments.Any(var1 =>
                                      var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Department.Guid)))
                              && (request.Designations == null || !request.Designations.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .Designation != null &&
                                  request.Designations.Any(var1 =>
                                      var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Designation.Guid)))
                              && (request.Roles == null || !request.Roles.Any() ||
                                  request.Roles.Any(var1 =>
                                      var1 == var.Role.Guid))
                && var.AppraisalEmployeeEmployee.FirstOrDefault().Appraisal.Year == request.FromDate.Year
                && var.AppraisalEmployeeEmployee.FirstOrDefault().Appraisal.Mode == 2
                )
                .Select(var => new ObjectiverptDto
                {
                    Name = var.Name,
                    Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                        ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                        : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                    Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                    Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                    Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                    Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                    Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                    Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                    Role = var.EmployeeCompanyEmployee.FirstOrDefault().Status,
                    Status = var.IsActive == true ? "Live" : "Resigned",
                    EmployeeId = var.Guid,
                    EmailId = var.EmailId,
                    SelfSubmitted = var.AppraisalEmployeeEmployee.FirstOrDefault().SelfVariableSubmittedOn,
                    RMSubmitted = var.AppraisalEmployeeEmployee.FirstOrDefault().RmVariableSubmittedOn,
                    HODSubmitted = var.AppraisalEmployeeEmployee.FirstOrDefault().L2VariableSubmittedOn
                })
                .OrderBy(var => var.Code)
                .ToList();


            var Appraisalemployees =
                    GetAll()
                .Include(var => var.Role)
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .Where(var => var.IsActive
                              && var.EmployeeCompanyEmployee != null
                              && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                              && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                              && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                              && (string.IsNullOrWhiteSpace(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .EmployeeCode.Contains(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .OffRoleCode.Contains(request.Code))
                                      && (request.Status == null || !request.Status.Any() ||
                                  request.Status.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault().Status))
                              && (string.IsNullOrWhiteSpace(request.Phone) || var.EmployeeContactEmployee.Any(var1 =>
                                      var1.OfficialNumber.Contains(request.Phone)))
                              && (request.Locations == null || !request.Locations.Any() || (var.EmployeeCompanyEmployee.FirstOrDefault().Location != null &&
                                  request.Locations.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Location.Guid))) && (request.Grades == null || !request.Grades.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault().Grade != null &&
                                  request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                                                 .Grade.Guid)))
                              && (request.Departments == null || !request.Departments.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .Department != null &&
                                  request.Departments.Any(var1 =>
                                      var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Department.Guid)))
                              && (request.Designations == null || !request.Designations.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .Designation != null &&
                                  request.Designations.Any(var1 =>
                                      var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Designation.Guid)))
                              && (request.Roles == null || !request.Roles.Any() ||
                                  request.Roles.Any(var1 =>
                                      var1 == var.Role.Guid))
                && var.AppraisalEmployeeEmployee.FirstOrDefault().Appraisal.Year == request.FromDate.Year
                && var.AppraisalEmployeeEmployee.FirstOrDefault().Appraisal.Mode == 3
                )
                .Select(var => new ObjectiverptDto
                {
                    Name = var.Name,
                    Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                        ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                        : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                    Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                    Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                    Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                    Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                    Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                    Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                    Role = var.EmployeeCompanyEmployee.FirstOrDefault().Status,
                    Status = var.IsActive == true ? "Live" : "Resigned",
                    EmployeeId = var.Guid,
                    EmailId = var.EmailId,
                    SelfSubmitted = var.AppraisalEmployeeEmployee.FirstOrDefault().SelfSubmittedOn,
                    RMSubmitted = var.AppraisalEmployeeEmployee.FirstOrDefault().RmSubmittedOn,
                    HODSubmitted = var.AppraisalEmployeeEmployee.FirstOrDefault().L2SubmittedOn
                })
                .OrderBy(var => var.Code)
                .ToList();

            var empid = request.UserIdNum;
            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new EmployeeRptObjectiveResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                EmployeeObjectiveDetails = objemployees,
                EmployeeVariableBonusDetails = vbemployees,
                EmployeeAppraisalDetails = Appraisalemployees
            };
        }

        public EmployeeRehireResponse GetEmployeeRehire(EmployeeListFilterRequest request)
        {

            var employees =
                    GetAll()
                .Include(var => var.Role)
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.EmployeeExitEmployee).ThenInclude(var => var.EmployeeExitHodfeedBackForm)
                .Where(var => !var.IsActive
                              && var.EmployeeCompanyEmployee != null
                              && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                              && var.EmployeeExitEmployee.FirstOrDefault().Status == "Completed"
                              && (string.IsNullOrWhiteSpace(request.Name) || var.Name.Contains(request.Name))
                              && (string.IsNullOrWhiteSpace(request.Email) || var.EmailId.Contains(request.Email))
                              && (string.IsNullOrWhiteSpace(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .EmployeeCode.Contains(request.Code) || var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .OffRoleCode.Contains(request.Code))
                                      && (request.Status == null || !request.Status.Any() ||
                                  request.Status.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault().Status))
                              && (string.IsNullOrWhiteSpace(request.Phone) || var.EmployeeContactEmployee.Any(var1 =>
                                      var1.OfficialNumber.Contains(request.Phone)))
                              && (request.Locations == null || !request.Locations.Any() || (var.EmployeeCompanyEmployee.FirstOrDefault().Location != null &&
                                  request.Locations.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Location.Guid))) && (request.Grades == null || !request.Grades.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault().Grade != null &&
                                  request.Grades.Any(var1 => var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                                                 .Grade.Guid)))
                              && (request.Departments == null || !request.Departments.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .Department != null &&
                                  request.Departments.Any(var1 =>
                                      var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Department.Guid)))
                              && (request.Designations == null || !request.Designations.Any() ||
                                  (var.EmployeeCompanyEmployee.FirstOrDefault()
                                      .Designation != null &&
                                  request.Designations.Any(var1 =>
                                      var1 == var.EmployeeCompanyEmployee.FirstOrDefault()
                                          .Designation.Guid)))
                              && (request.Roles == null || !request.Roles.Any() ||
                                  request.Roles.Any(var1 =>
                                      var1 == var.Role.Guid))
                )
                .Select(var => new EmployeeRehireDto
                {
                    Name = var.Name,
                    Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                        ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                        : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                    Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                    Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                    Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                    Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                    Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                    Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                    EmployeeId = var.Guid,
                    EmailId = var.EmailId,
                    DateofJoing = var.EmployeeCompanyEmployee.FirstOrDefault().Doj.Value,
                    DateofRelieving = var.EmployeeExitEmployee.FirstOrDefault().ActualRelievingDate.Value,
                    Desired = var.EmployeeExitHodfeedBackFormEmployee != null ? var.EmployeeExitHodfeedBackFormEmployee.FirstOrDefault().IsDesiredAttrition.Equals(0) ? "Undesired" : "Desired" : null,
                    Status = var.EmployeeCompanyEmployee.FirstOrDefault().Status,
                    Rehired = var.EmployeeCompanyEmployee.FirstOrDefault().IsRehired != null ? var.EmployeeCompanyEmployee.FirstOrDefault().IsRehired.Equals(true) ? "Yes" : "No" : ""
                })
                .OrderBy(var => var.Code)
                .ToList();

            var empid = request.UserIdNum;
            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(21) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(21) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;


            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllEmployees.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllEmployees.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new EmployeeRehireResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                EmployeeRehireDetails = employees
            };
        }

        public CreateEmployeeResponse CopyEmployee(CopyEmployeeRequest request, ApplicationSettings appSettings, bool sendEmail = true)
        {
            var employee = GetAll().FirstOrDefault(var => var.EmailId.Equals(request.OfficialEmail) && var.IsActive);
            if (employee == null)
            {
                var role = dbContext.SettingsRole.FirstOrDefault(var => var.Guid.Equals(request.RoleId));
                if (role != null)
                {
                    var uniqueNum = GetAll()
                        .Include(var => var.EmployeeCompanyEmployee)
                        .Max(var => Convert.ToInt32(var.EmployeeCompanyEmployee.FirstOrDefault().UniqueCode ?? "0")) + 1;

                    var tempPassword = RandomString.GetRandomString(10);
                    var salt = RandomString.GetRandomString(10);
                    var employeeCopy = GetAll()
                         .Include(var => var.EmployeePersonalEmployee)
                         .Include(var => var.EmployeeCompanyEmployee)
                         .Include(var => var.EmployeeContactEmployee)
                         .Include(var => var.EmployeeStatutoryEmployee)
                         .FirstOrDefault(var => var.IsActive && var.EmailId.Equals(request.OldEmail));
                    employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().IsRehired = true;
                    var newEmployee = new Employee
                    {
                        Guid = CustomGuid.NewGuid(),
                        Name = request.Name,
                        Password = Cryptography.Encrypt(tempPassword, salt),
                        PasswordSalt = salt,
                        Role = role,
                        IsActive = true,
                        CanLogin = request.CanLogin,
                        CompanyId = request.CompanyIdNum,
                        EmailId = request.OfficialEmail,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        EmployeeCompanyEmployee = new List<EmployeeCompany>
                        {
                            new EmployeeCompany
                            {
                                CompanyId = request.CompanyIdNum,
                                Status = request.Status,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.UtcNow,
                                UniqueCode = uniqueNum.ToString(),
                                AddressingName = request.Name,
                                EmployeeCode = request.EmployeeCode,
                                CategoryId = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().CategoryId,
                                DepartmentId = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().DepartmentId,
                                DesignationId = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().DesignationId,
                                GradeId = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().GradeId,
                                LocationId = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().LocationId,
                                RegionId = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().RegionId,
                                TeamId = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().TeamId,
                                OffRoleCode = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode,
                                LocationBifurcation = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().LocationBifurcation,
                                Vendor = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().Vendor,
                                ReportingToId = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId,
                                StatusCategory = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().StatusCategory,
                                LocationForField = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().LocationForField,
                                Division = employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().Division
                            }
                        },
                        EmployeePersonalEmployee = new List<EmployeePersonal>
                        {
                            new EmployeePersonal
                            {
                                CompanyId = request.CompanyIdNum,
                                Gender = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().Gender,
                                Height = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().Height,
                                Nationality = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().Nationality,
                                Sports = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().Sports,
                                Weight = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().Weight,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.UtcNow,
                                BloodGroup = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().BloodGroup,
                                DobActual = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().DobActual,
                                DobRecord = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().DobRecord,
                                MaritalStatus = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().MaritalStatus,
                                MarriageDate = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().MarriageDate,
                                SpecializedTraining = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().SpecializedTraining,
                                Age = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().Age,
                                PhotoUrl = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().PhotoUrl,
                                HideBirthday = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().HideBirthday,
                                PhotoLinkUrl = employeeCopy.EmployeePersonalEmployee.FirstOrDefault().PhotoLinkUrl
                            }
                        },
                        EmployeeStatutoryEmployee = new List<EmployeeStatutory>
                        {
                            new EmployeeStatutory
                            {
                                CompanyId = request.CompanyIdNum,
                                AadharName = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().AadharName,
                                AadharNumber = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().AadharNumber,
                                EsiNumber = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().EsiNumber,
                                PanNumber = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().PanNumber,
                                PassportNumber = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().PassportNumber,
                                PassportValidity = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().PassportValidity,
                                PfNumber = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().PfNumber,
                                UanNumber = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().UanNumber,
                                DrivingLicenseNumber = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().DrivingLicenseNumber,
                                DrivingLicenseValidity = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().DrivingLicenseValidity,
                                LicIdNumber = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().LicIdNumber,
                                PreviousEmpPension = employeeCopy.EmployeeStatutoryEmployee.FirstOrDefault().PreviousEmpPension,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.UtcNow,
                            }
                        },
                        EmployeeContactEmployee = new List<EmployeeContact>
                        {
                            new EmployeeContact
                            {
                                CompanyId = request.CompanyIdNum,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.UtcNow,
                                OfficialEmailId = request.OfficialEmail,
                                Guid = CustomGuid.NewGuid(),
                                AlternateNumber = employeeCopy.EmployeeContactEmployee.FirstOrDefault().AlternateNumber,
                                ContactNumber = employeeCopy.EmployeeContactEmployee.FirstOrDefault().ContactNumber,
                                OfficialNumber = employeeCopy.EmployeeContactEmployee.FirstOrDefault().OfficialNumber,
                                PermanentAddress = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress != null ?
                                    new EmployeeAddress
                                    {
                                        CompanyId = request.CompanyIdNum,
                                        City = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PermanentAddress.City,
                                        Country = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PermanentAddress.Country,
                                        District = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PermanentAddress.District,
                                        Landmark = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PermanentAddress.Landmark,
                                        Pincode = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PermanentAddress.Pincode,
                                        State = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PermanentAddress.State,
                                        StreetName = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PermanentAddress.StreetName,
                                        Village = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PermanentAddress.Village,
                                        DoorNo = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PermanentAddress.DoorNo
                                    } : null,
                                PresentAddress = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress != null ?
                                    new EmployeeAddress
                                    {
                                        CompanyId = request.CompanyIdNum,
                                        City = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress.City,
                                        Country = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress.Country,
                                        District = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress.District,
                                        Landmark = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress.Landmark,
                                        Pincode = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress.Pincode,
                                        State = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress.State,
                                        StreetName = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress.StreetName,
                                        Village = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress.Village,
                                        DoorNo = employeeCopy.EmployeeContactEmployee.FirstOrDefault().PresentAddress.DoorNo
                                    } : null,
                            }
                        }
                    };
                    //Bank Update
                    var employeeBank = GetAll()
                        .Include(var => var.EmployeeBankEmployee)
                        .FirstOrDefault(var => var.IsActive && var.Id.Equals(employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().EmployeeId));
                    if (employeeBank != null)
                    {
                        var banks = employeeBank.EmployeeBankEmployee
                        .Where(var => var.IsActive)
                        .Select(var => new EmployeeBankDto()
                        {
                            Branch = var.BankBranch,
                            AccountNumber = var.BankAccountNumber,
                            AccountType = var.AccountType,
                            BankName = var.BankName,
                            IfscCode = var.IfscCode,
                            IsActive = true,
                            EmployeeBankId = var.Guid,
                            EffectiveDate = var.EffectiveDate
                        }).ToList();

                        foreach (var bank in banks)
                        {
                            var newBank = new EmployeeBank
                            {
                                CompanyId = request.CompanyIdNum,
                                Guid = CustomGuid.NewGuid(),
                                AddedOn = DateTime.UtcNow,
                                AddedBy = request.UserIdNum,
                                BankBranch = bank.Branch,
                                BankName = bank.BankName,
                                IfscCode = bank.IfscCode,
                                IsActive = true,
                                EffectiveDate = bank.EffectiveDate,
                                BankAccountNumber = bank.AccountNumber,
                                AccountType = bank.AccountType
                            };
                            newEmployee.EmployeeBankEmployee.Add(newBank);
                        }
                    }

                    //Career
                    var employeeCareer = GetAll()
                    .Include(var => var.EmployeeCareerEmployee)
                    .FirstOrDefault(var => var.IsActive && var.Id.Equals(employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().EmployeeId));
                    if (employeeCareer != null)
                    {
                        var careers = employeeCareer.EmployeeCareerEmployee
                        .Where(var => var.IsActive)
                        .Select(var => new EmployeeCareerDto()
                        {
                            AppraisalYear = var.AppraisalYear,
                            AppraisalType = var.AppraisalType,
                            Rating = var.Rating,
                            Description = var.Description,
                            Department = var.DepartmentId != 0 ? dbContext.SettingsDepartment.Where(a => a.Id.Equals(var.DepartmentId)).FirstOrDefault().Name : null,
                            Grade = var.GradeId != 0 ? dbContext.SettingsGrade.Where(a => a.Id.Equals(var.GradeId)).FirstOrDefault().Grade : null,
                            Location = var.LocationId != 0 ? dbContext.SettingsLocation.Where(a => a.Id.Equals(var.LocationId)).FirstOrDefault().Name : null,
                            Designation = var.DesignationId != 0 ? dbContext.SettingsDepartmentDesignation.Where(a => a.Id.Equals(var.DesignationId)).FirstOrDefault().Name : null,
                            RnR = var.RnR,
                            Remarks = var.Remarks,
                            ReasonForChange = var.ReasonForChange,
                            IsActive = var.IsActive,
                            EmployeeCareerId = var.Guid,
                            EffectiveFrom = var.EffectiveFrom,
                            DateofChange = var.DateofChange,
                            MovementStatus = var.MovementStatus
                        }).ToList();

                        foreach (var career in careers)
                        {

                            var location = !string.IsNullOrWhiteSpace(career.Location) ?
                            dbContext.SettingsLocation.FirstOrDefault(
                                var => var.IsActive && var.Guid.Equals(career.Location))
                            : null;
                            var department = !string.IsNullOrWhiteSpace(career.Department) ?
                            dbContext.SettingsDepartment.FirstOrDefault(
                                var => var.IsActive && var.Guid.Equals(career.Department))
                            : null;
                            var newCareer = new EmployeeCareer
                            {
                                AppraisalType = career.AppraisalType,
                                AppraisalYear = career.AppraisalYear,
                                CompanyId = request.CompanyIdNum,
                                Guid = CustomGuid.NewGuid(),
                                AddedOn = DateTime.UtcNow,
                                AddedBy = request.UserIdNum,
                                DateofChange = DateTime.UtcNow,
                                LocationId = career.Location != null ? location.Id : 0,
                                DepartmentId = career.Department != null ? department.Id : 0,
                                ReasonForChange = career.ReasonForChange,
                                Remarks = career.Remarks,
                                IsActive = true,
                                EffectiveFrom = career.EffectiveFrom,
                                MovementStatus = career.MovementStatus
                            };
                            newEmployee.EmployeeCareerEmployee.Add(newCareer);
                        }
                    }
                    //Family
                    var employeeFamily = GetAll()
                        .Include(var => var.EmployeeFamilyEmployee)
                        .FirstOrDefault(var => var.IsActive && var.Id.Equals(employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().EmployeeId));
                    if (employeeFamily != null)
                    {
                        var familys = employeeFamily.EmployeeFamilyEmployee
                                    .Where(var => var.IsActive)
                            .Select(var => new EmployeeFamilyDto()
                            {
                                Dob = var.Dob,
                                Address = var.Address,
                                Email = var.EmailId,
                                Gender = var.Gender,
                                Name = var.Name,
                                Occupation = var.Occupation,
                                Phone = var.Phone,
                                Relation = var.Relationship,
                                IsActive = true,
                                IsAlive = var.IsAlive,
                                IsDependant = var.IsDependant ?? false,
                                EmployeeFamilyId = var.Guid,
                                IsEmergencyContact = var.IsEmergencyContact ?? false,
                                IsOptedForMediclaim = var.IsOptedForMediclaim ?? false
                            }).ToList();

                        foreach (var family in familys)
                        {
                            var newfamily = new EmployeeFamily()
                            {
                                CompanyId = request.CompanyIdNum,
                                Guid = CustomGuid.NewGuid(),
                                AddedOn = DateTime.UtcNow,
                                AddedBy = request.UserIdNum,
                                IsActive = true,
                                Dob = family.Dob,
                                Occupation = family.Occupation,
                                Name = family.Name,
                                Relationship = family.Relation,
                                EmailId = family.Email,
                                IsAlive = family.IsAlive,
                                IsDependant = family.IsDependant,
                                IsEmergencyContact = family.IsEmergencyContact,
                                Phone = family.Phone,
                                Address = family.Address,
                                IsOptedForMediclaim = family.IsOptedForMediclaim
                            };
                            newEmployee.EmployeeFamilyEmployee.Add(newfamily);
                        }
                    }
                    //Education 
                    var employeeEdu = GetAll()
                    .Include(var => var.EmployeeEducationEmployee)
                    .FirstOrDefault(var => var.IsActive && var.Id.Equals(employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().EmployeeId));
                    if (employeeEdu != null)
                    {
                        var educations = employeeEdu.EmployeeEducationEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeEducationDto()
                    {
                        City = var.City,
                        Grade = var.Grade,
                        Institute = var.Institute,
                        Percentage = var.MarksPercentage ?? 0,
                        State = var.State,
                        CompletedYear = var.CompletedYear ?? 0,
                        CourseDuration = var.CourseDuration ?? 0,
                        CourseName = var.CourseName,
                        CourseType = var.CourseType,
                        IsActive = true,
                        MajorSubject = var.MajorSubject,
                        StartedYear = var.StartedYear ?? 0,
                        EmployeeEducationId = var.Guid
                    }).ToList();

                        foreach (var education in educations)
                        {
                            var newEducation = new EmployeeEducation()
                            {
                                Guid = CustomGuid.NewGuid(),
                                AddedOn = DateTime.UtcNow,
                                AddedBy = request.UserIdNum,
                                IsActive = true,
                                City = education.City,
                                Grade = education.Grade,
                                Institute = education.Institute,
                                State = education.State,
                                CompletedYear = education.CompletedYear,
                                CourseDuration = education.CourseDuration,
                                CourseName = education.CourseName,
                                CourseType = education.CourseType,
                                MajorSubject = education.MajorSubject,
                                MarksPercentage = education.Percentage,
                                StartedYear = education.StartedYear,
                                CompanyId = request.CompanyIdNum
                            };
                            newEmployee.EmployeeEducationEmployee.Add(newEducation);
                        }
                    }
                    //Language
                    var employeeLang = GetAll()
                .Include(var => var.EmployeeLanguageEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().EmployeeId));
                    if (employeeLang != null)
                    {
                        var languages = employeeLang.EmployeeLanguageEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeLanguageDto()
                    {
                        IsActive = true,
                        Language = var.LanguageName,
                        CanRead = var.CanRead ?? false,
                        CanSpeak = var.CanSpeak ?? false,
                        CanWrite = var.CanWrite ?? false,
                        EmployeeLanguageId = var.Guid,
                        Level = var.Level
                    }).ToList();

                        foreach (var langugage in languages)
                        {

                            var newLanguage = new EmployeeLanguage()
                            {
                                Guid = CustomGuid.NewGuid(),
                                AddedOn = DateTime.UtcNow,
                                AddedBy = request.UserIdNum,
                                IsActive = true,
                                CompanyId = request.CompanyIdNum,
                                LanguageName = langugage.Language,
                                CanRead = langugage.CanRead,
                                CanSpeak = langugage.CanSpeak,
                                CanWrite = langugage.CanWrite,
                                Level = langugage.Level
                            };
                            newEmployee.EmployeeLanguageEmployee.Add(newLanguage);
                        }
                    }
                    //Pervious Company

                    var employeePrev = GetAll()
                .Include(var => var.EmployeePreviousCompanyEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(employeeCopy.EmployeeCompanyEmployee.FirstOrDefault().EmployeeId));
                    if (employeePrev != null)
                    {
                        var previousCompanies = employeePrev.EmployeePreviousCompanyEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeePreviousCompanyDto
                    {
                        PreviousCompanyId = var.Guid,
                        Department = var.Department,
                        Designation = var.Designation,
                        Employer = var.Employer,
                        DateOfExit = var.Doe,
                        DateOfJoin = var.Doj,
                        ReasonForChange = var.ReasonForChange,
                        Duration = var.Duration ?? 0,
                        Ctc = var.LastCtc ?? 0,
                        IsActive = true
                    }).ToList();

                        foreach (var previousCompany in previousCompanies)
                        {
                            var newPreviousCompany = new EmployeePreviousCompany
                            {
                                CompanyId = request.CompanyIdNum,
                                Guid = CustomGuid.NewGuid(),
                                AddedOn = DateTime.UtcNow,
                                AddedBy = request.UserIdNum,
                                IsActive = true,
                                Department = previousCompany.Department,
                                Designation = previousCompany.Designation,
                                Doe = previousCompany.DateOfExit,
                                Doj = previousCompany.DateOfJoin,
                                Duration = previousCompany.Duration,
                                Employer = previousCompany.Employer,
                                ReasonForChange = previousCompany.ReasonForChange,
                                LastCtc = previousCompany.Ctc
                            };
                            newEmployee.EmployeePreviousCompanyEmployee.Add(newPreviousCompany);
                        }
                    }


                    AddItem(newEmployee);

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.CreateNewEmployee.Template, request.UserName, request.UserId, newEmployee.Name, newEmployee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.CreateNewEmployee.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    if (sendEmail)
                    {
                        EmailSender.SendRegisteredEmail(newEmployee.Name, newEmployee.EmailId, tempPassword, appSettings);
                    }

                    return new CreateEmployeeResponse
                    {
                        EmployeeId = newEmployee.Guid,
                        IsCreated = true,
                        IsSuccess = true
                    };
                }

                throw new Exception("Role not found.");
            }
            else
            {
                var uniqueNum = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .Max(var => Convert.ToInt32(var.EmployeeCompanyEmployee.FirstOrDefault().UniqueCode ?? "0")) + 1;
                employee.EmployeeCompanyEmployee = new List<EmployeeCompany>
                {
                    new EmployeeCompany
                    {
                        CompanyId = request.CompanyIdNum,
                        Status = request.Status,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        UniqueCode = uniqueNum.ToString(),
                        AddressingName = request.Name,
                        EmployeeCode = request.EmployeeCode
}
                };
                Save();
            }

            return new CreateEmployeeResponse
            {
                IsSuccess = true,
                IsCreated = false
            };
        }

        public CreateEmployeeResponse CreateEmployee(CreateEmployeeRequest request, ApplicationSettings appSettings, bool sendEmail = true)
        {
            var employee = GetAll().FirstOrDefault(var => var.EmailId.Equals(request.OfficialEmail) && var.IsActive);
            if (employee == null)
            {
                var role = dbContext.SettingsRole.FirstOrDefault(var => var.Guid.Equals(request.RoleId));
                if (role != null)
                {
                    var uniqueNum = GetAll()
                        .Include(var => var.EmployeeCompanyEmployee)
                        .Max(var => Convert.ToInt32(var.EmployeeCompanyEmployee.FirstOrDefault().UniqueCode ?? "0")) + 1;

                    var tempPassword = RandomString.GetRandomString(10);
                    var salt = RandomString.GetRandomString(10);

                    var newEmployee = new Employee
                    {
                        Guid = CustomGuid.NewGuid(),
                        Name = request.Name,
                        Password = Cryptography.Encrypt(tempPassword, salt),
                        PasswordSalt = salt,
                        Role = role,
                        IsActive = true,
                        CanLogin = request.CanLogin,
                        CompanyId = request.CompanyIdNum,
                        EmailId = request.OfficialEmail,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        EmployeeCompanyEmployee = new List<EmployeeCompany>
                        {
                            new EmployeeCompany
                            {
                                CompanyId = request.CompanyIdNum,
                                Status = request.Status,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.UtcNow,
                                UniqueCode = uniqueNum.ToString(),
                                AddressingName = request.Name,
                                EmployeeCode = request.EmployeCode
                            }
                        },
                        EmployeeContactEmployee = new List<EmployeeContact>
                        {
                            new EmployeeContact
                            {
                                CompanyId = request.CompanyIdNum,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.UtcNow,
                                OfficialEmailId = request.OfficialEmail,
                                Guid = CustomGuid.NewGuid()
                            }
                        }
                    };

                    AddItem(newEmployee);

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.CreateNewEmployee.Template, request.UserName, request.UserId, newEmployee.Name, newEmployee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.CreateNewEmployee.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    if (sendEmail)
                    {
                        EmailSender.SendRegisteredEmail(newEmployee.Name, newEmployee.EmailId, tempPassword, appSettings);
                    }

                    return new CreateEmployeeResponse
                    {
                        EmployeeId = newEmployee.Guid,
                        IsCreated = true,
                        IsSuccess = true
                    };
                }

                throw new Exception("Role not found.");
            }
            else
            {
                var uniqueNum = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .Max(var => Convert.ToInt32(var.EmployeeCompanyEmployee.FirstOrDefault().UniqueCode ?? "0")) + 1;
                employee.EmployeeCompanyEmployee = new List<EmployeeCompany>
                {
                    new EmployeeCompany
                    {
                        CompanyId = request.CompanyIdNum,
                        Status = request.Status,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        UniqueCode = uniqueNum.ToString(),
                        AddressingName = request.Name,
                        EmployeeCode = request.EmployeCode
                    }
                };
                Save();
            }

            return new CreateEmployeeResponse
            {
                IsSuccess = true,
                IsCreated = false
            };
        }

        public GetEmployeeAccountResponse UpdateEmployeeAccount(UpdateEmployeeAccountRequest request, bool removeExisting = false)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeDataVerificationEmployee)
                .Include(var => var.EmployeeCompanyEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var company = employee.EmployeeCompanyEmployee.FirstOrDefault();
                var isCodeAlreadyAdded = GetAll()
                    .Any(var => var.IsActive
                                && !var.Guid.Equals(request.EmployeeId)
                                && (
                                    (request.Status.Equals("on-roll") && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                                                 .Equals(request.EmployeeCode))
                                    || (!request.Status.Equals("on-roll") && var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode
                                            .Equals(request.OffRoleCode))
                                    ));
                if (isCodeAlreadyAdded)
                {
                    return new GetEmployeeAccountResponse
                    {
                        IsSuccess = true,
                        EmployeeId = null
                    };
                }

                var role = dbContext.SettingsRole.FirstOrDefault(var => var.Guid.Equals(request.RoleId));
                if (role != null)
                {
                    employee.CanLogin = request.CanLogin;
                    employee.Role = role;
                    company.Status = request.Status;
                    company.AddressingName = request.AddressingName;
                    company.EmployeeCode = request.EmployeeCode;
                    company.OffRoleCode = request.OffRoleCode;

                    employee.Name = request.AddressingName;

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.UpdateEmployeeAccount.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.UpdateEmployeeAccount.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                    {
                        Section = "account",
                        EmployeeId = request.EmployeeId,
                        CompanyIdNum = request.CompanyIdNum,
                        UserIdNum = request.UserIdNum
                    });


                    if (removeExisting)
                    {
                        return new GetEmployeeAccountResponse()
                        {
                            IsSuccess = true
                        };
                    }
                    else
                    {
                        return GetEmployeeAccount(new EmployeeActionRequest
                        {
                            EmployeeId = request.EmployeeId,
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId
                        });
                    }
                }

                throw new Exception("Role not found");
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeePersonalResponse UpdateEmployeePersonal(UpdateEmployeePersonalRequest request, bool removeExisting = false)
        {
            var employee = GetAll()
                .Include(var => var.EmployeePersonalEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var photoPath = "";
                if (request.Photo != null)
                {
                    var path = Path.Combine(new string[]
                        {request.FileBasePath, "employees", employee.Guid, "photo"});
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        request.Photo.CopyTo(memoryStream);
                        File.WriteAllBytes(Path.Combine(path, request.Photo.FileName), memoryStream.ToArray());
                    }

                    photoPath = string.Concat(path, "\\", request.Photo.FileName);
                }

                var addedPersonalInfo = employee.EmployeePersonalEmployee.FirstOrDefault();
                if (addedPersonalInfo != null)
                {
                    addedPersonalInfo.Gender = request.Gender;
                    addedPersonalInfo.Height = request.Height;
                    addedPersonalInfo.Nationality = request.Nationality;
                    addedPersonalInfo.Sports = request.Sports;
                    addedPersonalInfo.Weight = request.Weight;
                    addedPersonalInfo.UpdatedBy = request.UserIdNum;
                    addedPersonalInfo.UpdatedOn = DateTime.UtcNow;
                    addedPersonalInfo.BloodGroup = request.BloodGroup;
                    addedPersonalInfo.DobActual = request.ActualDob.Date;
                    addedPersonalInfo.DobRecord = request.RecordDob.Date;
                    addedPersonalInfo.MaritalStatus = request.MaritalStatus;
                    addedPersonalInfo.HideBirthday = request.HideBirthday;
                    addedPersonalInfo.MarriageDate = request.MarriageDate;
                    addedPersonalInfo.SpecializedTraining = request.SpecializedTraining;
                    addedPersonalInfo.Age = request.Age;
                    addedPersonalInfo.PhotoUrl = request.Photo != null ? photoPath : addedPersonalInfo.PhotoUrl;
                    addedPersonalInfo.PhotoLinkUrl = request.Photo != null ? photoPath.Replace("C:\\\\", "").Replace("\\", "/") : addedPersonalInfo.PhotoLinkUrl;
                }
                else
                {
                    employee.EmployeePersonalEmployee = new List<EmployeePersonal>
                    {
                        new EmployeePersonal
                        {
                            CompanyId = request.CompanyIdNum,
                            Gender = request.Gender,
                            Height = request.Height,
                            Nationality = request.Nationality,
                            Sports = request.Sports,
                            Weight = request.Weight,
                            AddedBy = request.UserIdNum,
                            AddedOn = DateTime.UtcNow,
                            BloodGroup = request.BloodGroup,
                            DobActual = request.ActualDob.Date,
                            DobRecord = request.RecordDob.Date,
                            MaritalStatus = request.MaritalStatus,
                            MarriageDate = request.MarriageDate,
                            SpecializedTraining = request.SpecializedTraining,
                            Age = request.Age,
                            PhotoUrl = photoPath,
                            HideBirthday = request.HideBirthday,
                            PhotoLinkUrl = photoPath.Replace("C:\\\\", "").Replace("\\", "/")
                        }
                    };
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeePersonal.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeePersonal.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "personal",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                if (removeExisting)
                {
                    return new GetEmployeePersonalResponse()
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return GetEmployeePersonal(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeeCompanyResponse UpdateEmployeeCompany(UpdateEmployeeCompanyRequest request, bool removeExisting = false)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var company = employee.EmployeeCompanyEmployee.FirstOrDefault();

                if (company != null)
                {
                    var managerHierarchy = new List<EmployeeCardDto>();
                    managerHierarchy.Add(new EmployeeCardDto
                    {
                        EmployeeId = employee.Guid,
                        Name = employee.Name,
                    });
                    var reportingTo =
                        GetAll()
                            .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.ReportingToId));
                    var initialManager = reportingTo;

                    var isCircularManager = false;
                    while (reportingTo != null)
                    {
                        var managerEmployee = GetAll()
                                    .Include(var => var.EmployeePersonalEmployee)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Region)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Team)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                            .FirstOrDefault(var => var.Id == reportingTo.Id && var.IsActive);
                        if (managerEmployee != null)
                        {
                            reportingTo = managerEmployee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingTo;
                        }
                        else
                        {
                            reportingTo = null;
                        }

                        if (managerHierarchy.Any(var => var.EmployeeId.Equals(managerEmployee.Guid)))
                        {
                            isCircularManager = true;
                            managerHierarchy.Add(new EmployeeCardDto
                            {
                                EmployeeId = managerEmployee.Guid,
                                Name = managerEmployee.Name,
                            });

                            break;
                        }

                        managerHierarchy.Add(new EmployeeCardDto
                        {
                            EmployeeId = managerEmployee.Guid,
                            Name = managerEmployee.Name,
                        });
                    }

                    if (isCircularManager)
                    {
                        return new GetEmployeeCompanyResponse
                        {
                            IsSuccess = true,
                            IsCircularManager = true,
                            ManagerList = managerHierarchy
                        };
                    }

                    var category = !string.IsNullOrWhiteSpace(request.CategoryId) ?
                        dbContext.SettingsCategory.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(request.CategoryId))
                        : null;

                    var grade = !string.IsNullOrWhiteSpace(request.GradeId) ?
                        dbContext.SettingsGrade.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(request.GradeId))
                        : null;

                    var location = !string.IsNullOrWhiteSpace(request.LocationId) ?
                        dbContext.SettingsLocation.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(request.LocationId))
                        : null;

                    var team = !string.IsNullOrWhiteSpace(request.TeamId) ?
                        dbContext.SettingsTeam.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(request.TeamId))
                        : null;

                    var department = !string.IsNullOrWhiteSpace(request.DepartmentId) ?
                        dbContext.SettingsDepartment.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(request.DepartmentId))
                        : null;

                    var designation = !string.IsNullOrWhiteSpace(request.DesignationId) ?
                        dbContext.SettingsDepartmentDesignation.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(request.DesignationId))
                        : null;

                    var region = !string.IsNullOrWhiteSpace(request.RegionId) ?
                        dbContext.SettingsRegion.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(request.RegionId))
                        : null;

                    company.CategoryId = !string.IsNullOrWhiteSpace(request.CategoryId) ? category.Id : (long?)null;
                    company.CompanyId = request.CompanyIdNum;
                    company.DepartmentId = !string.IsNullOrWhiteSpace(request.DepartmentId) ? department.Id : (long?)null;
                    company.DesignationId = !string.IsNullOrWhiteSpace(request.DesignationId) ? designation.Id : (long?)null;
                    company.Doj = request.Doj;
                    company.GradeId = !string.IsNullOrWhiteSpace(request.GradeId) ? grade.Id : (long?)null;
                    company.LocationId = !string.IsNullOrWhiteSpace(request.LocationId) ? location.Id : (long?)null;
                    company.RegionId = !string.IsNullOrWhiteSpace(request.RegionId) ? region.Id : (long?)null;
                    //company.Status = request.Status;
                    company.TeamId = !string.IsNullOrWhiteSpace(request.TeamId) ? team.Id : (long?)null;
                    company.ProbationExtension = request.ProbationExtraDays;
                    company.AddressingName = request.AddressingName;
                    company.ConfirmationRemarks = request.ConfirmationRemarks;
                    //company.EmployeeCode = request.EmployeeCode;
                    company.OffRoleCode = request.OffRoleCode;
                    company.OnRollDate = request.OnRollDate;
                    company.ProbationStartDate = request.ProbationStartDate;
                    company.ProbationEndDate = request.ProbationEndDate;
                    company.LocationBifurcation = request.LocationBifurcation;
                    company.Vendor = request.Vendor;
                    company.ReportingToId = !string.IsNullOrWhiteSpace(request.ReportingToId) ? initialManager.Id : (long?)null;
                    company.StatusCategory = request.StatusCategory;
                    company.UpdatedBy = request.UserIdNum;
                    company.UpdatedOn = DateTime.UtcNow;
                    company.LocationForField = request.LocationForField;
                    company.Division = request.Division;

                    employee.Name = request.AddressingName;

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.UpdateEmployeeCompany.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.UpdateEmployeeCompany.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                    {
                        Section = "company",
                        EmployeeId = request.EmployeeId,
                        CompanyIdNum = request.CompanyIdNum,
                        UserIdNum = request.UserIdNum
                    });
                }
                else
                {
                    throw new Exception("Employee company info not found - " + request.EmployeeId);
                }

                if (removeExisting)
                {
                    return new GetEmployeeCompanyResponse()
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return GetEmployeeCompany(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeeStatutoryResponse UpdateEmployeeStatutory(UpdateEmployeeStatutoryRequest request, bool removeExisting = false)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeStatutoryEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var isFirstTime = employee.EmployeeStatutoryEmployee == null ||
                                  !employee.EmployeeStatutoryEmployee.Any();

                var newEmployeeStatutory = new EmployeeStatutory
                {
                    CompanyId = request.CompanyIdNum,
                    AadharName = request.AadharName,
                    AadharNumber = request.AadharNumber,
                    EsiNumber = request.EsiNumber,
                    PanNumber = request.PanNumber,
                    PassportNumber = request.PassportNumber,
                    PassportValidity = request.PassportValidity,
                    PfNumber = request.PfNumber,
                    UanNumber = request.UanNumber,
                    DrivingLicenseNumber = request.DrivingLicenseNumber,
                    DrivingLicenseValidity = request.DrivingLicenseValidity,
                    LicIdNumber = request.LicIdNumber,
                    PreviousEmpPension = request.PreviousEmployeePensionNumber,
                    AddedBy = request.UserIdNum,
                    AddedOn = DateTime.UtcNow,
                };
                if (isFirstTime)
                {
                }

                dbContext.EmployeeStatutory.RemoveRange(employee.EmployeeStatutoryEmployee);
                employee.EmployeeStatutoryEmployee = new List<EmployeeStatutory>
                {
                    newEmployeeStatutory
                };

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeeStatutory.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeeStatutory.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "statutory",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                if (removeExisting)
                {
                    return new GetEmployeeStatutoryResponse()
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return GetEmployeeStatutory(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeePreviousCompanyResponse UpdateEmployeePreviousCompany(
            UpdateEmployeePreviousCompanyRequest request, bool removeExisting = false)
        {

            var employee = GetAll()
                .Include(var => var.EmployeePreviousCompanyEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                if (removeExisting)
                {
                    dbContext.EmployeePreviousCompany.RemoveRange(employee.EmployeePreviousCompanyEmployee);
                }

                foreach (var previousCompany in request.PreviousCompanies)
                {
                    if (string.IsNullOrWhiteSpace(previousCompany.PreviousCompanyId))
                    {
                        var newPreviousCompany = new EmployeePreviousCompany
                        {
                            CompanyId = request.CompanyIdNum,
                            Guid = CustomGuid.NewGuid(),
                            AddedOn = DateTime.UtcNow,
                            AddedBy = request.UserIdNum,
                            IsActive = true,
                            Department = previousCompany.Department,
                            Designation = previousCompany.Designation,
                            Doe = previousCompany.DateOfExit,
                            Doj = previousCompany.DateOfJoin,
                            Duration = previousCompany.Duration,
                            Employer = previousCompany.Employer,
                            ReasonForChange = previousCompany.ReasonForChange,
                            LastCtc = previousCompany.Ctc
                        };
                        employee.EmployeePreviousCompanyEmployee.Add(newPreviousCompany);
                    }
                    else
                    {
                        var addedPreviousCompany = employee.EmployeePreviousCompanyEmployee.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(previousCompany.PreviousCompanyId)
                            );
                        if (addedPreviousCompany != null)
                        {
                            if (previousCompany.IsActive)
                            {
                                addedPreviousCompany.Department = previousCompany.Department;
                                addedPreviousCompany.Designation = previousCompany.Designation;
                                addedPreviousCompany.Doe = previousCompany.DateOfExit;
                                addedPreviousCompany.Doj = previousCompany.DateOfJoin;
                                addedPreviousCompany.Duration = previousCompany.Duration;
                                addedPreviousCompany.Employer = previousCompany.Employer;
                                addedPreviousCompany.ReasonForChange = previousCompany.ReasonForChange;
                                addedPreviousCompany.LastCtc = previousCompany.Ctc;
                                addedPreviousCompany.UpdatedBy = request.UserIdNum;
                                addedPreviousCompany.UpdatedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                addedPreviousCompany.IsActive = false;
                                addedPreviousCompany.UpdatedBy = request.UserIdNum;
                                addedPreviousCompany.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                    }
                }
                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeePreviousCompany.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeePreviousCompany.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "previous",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                if (removeExisting)
                {
                    return new GetEmployeePreviousCompanyResponse()
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return GetPreviousCompany(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeeDocumentsResponse UpdateEmployeeDocument(UpdateEmployeeDocumentRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeDocumentEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var documentType =
                    dbContext.SettingsDocumentType.FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(request.DocumentTypeId));
                if (documentType != null)
                {
                    if (string.IsNullOrWhiteSpace(request.DocumentId))
                    {
                        var guid = CustomGuid.NewGuid();
                        var path = Path.Combine(new string[]
                            {request.FileBasePath, "employees", employee.Guid, "documents"});
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            request.File.CopyTo(memoryStream);
                            File.WriteAllBytes(string.Concat(path, "\\", string.Concat(guid, request.File.FileName)),
                                memoryStream.ToArray());
                        }

                        var filePath = Path.Combine(path, string.Concat(guid, request.File.FileName));
                        var fileUrl = Path.Combine(new string[]
                            {"employees", employee.Guid, "documents", string.Concat(guid, request.File.FileName)});

                        var newDocument = new EmployeeDocument
                        {
                            IsActive = true,
                            Guid = guid,
                            Name = request.DocumentName,
                            Size = request.File.Length,
                            CompanyId = request.CompanyIdNum,
                            FileLocation = filePath,
                            FileUrl = fileUrl,
                            DocumentType = documentType,
                            AddedBy = request.UserIdNum,
                            AddedOn = DateTime.UtcNow
                        };
                        employee.EmployeeDocumentEmployee.Add(newDocument);
                    }
                    else
                    {
                        var addedDocument =
                            employee.EmployeeDocumentEmployee.FirstOrDefault(var =>
                                var.Guid.Equals(request.DocumentId));
                        if (addedDocument != null)
                        {
                            addedDocument.DocumentType = documentType;
                            addedDocument.Name = request.DocumentName;
                            addedDocument.UpdatedBy = request.UserIdNum;
                            addedDocument.UpdatedOn = DateTime.UtcNow;
                        }
                    }

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.UpdateEmployeeDocument.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.UpdateEmployeeDocument.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                    {
                        Section = "documents",
                        EmployeeId = request.EmployeeId,
                        CompanyIdNum = request.CompanyIdNum,
                        UserIdNum = request.UserIdNum
                    });

                    return GetEmployeeDocuments(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }

                throw new Exception("Document Type not found - " + request.DocumentTypeId);
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeeDocumentsResponse DeleteEmployeeDocument(DeleteEmployeeDocumentRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeDocumentEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var addedDocument =
                    employee.EmployeeDocumentEmployee.FirstOrDefault(var => var.Guid.Equals(request.DocumentId));
                if (addedDocument != null)
                {
                    addedDocument.IsActive = false;
                    addedDocument.UpdatedBy = request.UserIdNum;
                    addedDocument.UpdatedOn = DateTime.UtcNow;

                    var path = Path.Combine(new string[]
                            { request.FileBasePath, "employees", employee.Guid, "documents", addedDocument.FileLocation });

                    File.Delete(path);

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.DeleteEmployeeDocument.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.DeleteEmployeeDocument.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    return GetEmployeeDocuments(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }


                throw new Exception("Document not found - " + request.DocumentId);
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeeBankResponse UpdateEmployeeBank(UpdateEmployeeBankRequest request, bool removeExisting = false)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeBankEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                if (removeExisting)
                {
                    dbContext.EmployeeBank.RemoveRange(employee.EmployeeBankEmployee);
                }

                foreach (var bank in request.EmployeeBanks)
                {
                    if (string.IsNullOrWhiteSpace(bank.EmployeeBankId))
                    {
                        var newBank = new EmployeeBank
                        {
                            CompanyId = request.CompanyIdNum,
                            Guid = CustomGuid.NewGuid(),
                            AddedOn = DateTime.UtcNow,
                            AddedBy = request.UserIdNum,
                            BankBranch = bank.Branch,
                            BankName = bank.BankName,
                            IfscCode = bank.IfscCode,
                            IsActive = true,
                            EffectiveDate = bank.EffectiveDate,
                            BankAccountNumber = bank.AccountNumber,
                            AccountType = bank.AccountType
                        };
                        employee.EmployeeBankEmployee.Add(newBank);
                    }
                    else
                    {
                        var addedBank = employee.EmployeeBankEmployee.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(bank.EmployeeBankId));
                        if (addedBank != null)
                        {
                            if (bank.IsActive)
                            {
                                addedBank.BankBranch = bank.Branch;
                                addedBank.BankName = bank.BankName;
                                addedBank.IfscCode = bank.IfscCode;
                                addedBank.EffectiveDate = bank.EffectiveDate;
                                addedBank.AccountType = bank.AccountType;
                                addedBank.BankAccountNumber = bank.AccountNumber;
                                addedBank.UpdatedBy = request.UserIdNum;
                                addedBank.UpdatedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                addedBank.IsActive = false;
                                addedBank.UpdatedBy = request.UserIdNum;
                                addedBank.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                    }
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeeBank.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeeBank.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "bank",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                if (removeExisting)
                {
                    return new GetEmployeeBankResponse
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return GetEmployeeBanks(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }
        public GetEmployeeCareerResponse UpdateEmployeeCareer(UpdateEmployeeCareerRequest request, bool removeExisting = false)
        {

            var employee = GetAll()
                .Include(var => var.EmployeeCareerEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));


            if (employee != null)
            {

                if (removeExisting)
                {
                    dbContext.EmployeeCareer.RemoveRange(employee.EmployeeCareerEmployee);
                }

                foreach (var career in request.EmployeeCareers)
                {
                    if (string.IsNullOrWhiteSpace(career.EmployeeCareerId))
                    {
                        var location = !string.IsNullOrWhiteSpace(career.Location) ?
                        dbContext.SettingsCategory.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(career.Location))
                        : null;
                        var department = !string.IsNullOrWhiteSpace(career.Department) ?
                        dbContext.SettingsDepartment.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(career.Department))
                        : null;
                        var newCareer = new EmployeeCareer
                        {
                            AppraisalType = career.AppraisalType,
                            AppraisalYear = career.AppraisalYear,
                            CompanyId = request.CompanyIdNum,
                            Guid = CustomGuid.NewGuid(),
                            AddedOn = DateTime.UtcNow,
                            AddedBy = request.UserIdNum,
                            DateofChange = DateTime.UtcNow,
                            LocationId = career.Location != null ? location.Id : 0,
                            DepartmentId = career.Department != null ? department.Id : 0,
                            ReasonForChange = career.ReasonForChange,
                            Remarks = career.Remarks,
                            IsActive = true,
                            EffectiveFrom = career.EffectiveFrom,
                            MovementStatus = career.MovementStatus

                        };
                        employee.EmployeeCareerEmployee.Add(newCareer);
                    }
                    else
                    {
                        var addedCareer = employee.EmployeeCareerEmployee.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(career.EmployeeCareerId));
                        if (addedCareer != null)
                        {
                            if (career.IsActive)
                            {
                                //addedCareer.AppraisalType = career.AppraisalType;
                                //addedCareer.LocationId = location.Id;
                                //addedCareer.DepartmentId = dept.Id;
                                //addedCareer.DesignationId = designation.Id;
                                //addedCareer.GradeId = grade.Id;
                                addedCareer.RnR = career.RnR;
                                addedCareer.Remarks = career.Remarks;
                                addedCareer.EffectiveFrom = career.EffectiveFrom;
                                addedCareer.DateofChange = DateTime.UtcNow;
                                addedCareer.ReasonForChange = career.ReasonForChange;
                                addedCareer.MovementStatus = career.MovementStatus;
                                addedCareer.UpdatedBy = request.UserIdNum;
                                addedCareer.UpdatedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                addedCareer.IsActive = false;
                                addedCareer.UpdatedBy = request.UserIdNum;
                                addedCareer.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                    }
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeeCareer.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeeCareer.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "career",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                if (removeExisting)
                {
                    return new GetEmployeeCareerResponse
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return GetEmployeeCareers(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }
        public GetEmployeeContactResponse UpdateEmployeeContact(UpdateEmployeeContactRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeContactEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var isFirstTime = employee.EmployeeContactEmployee == null ||
                                  !employee.EmployeeContactEmployee.Any();

                var contact = new EmployeeContact
                {
                    CompanyId = request.CompanyIdNum,
                    Guid = CustomGuid.NewGuid(),
                    AlternateNumber = request.AlternateContactNumber,
                    ContactNumber = request.ContactNumber,
                    OfficialNumber = request.OfficialContactNumber,
                    PermanentAddress = request.PermanentAddress != null ?
                        new EmployeeAddress
                        {
                            CompanyId = request.CompanyIdNum,
                            City = request.PermanentAddress.City,
                            Country = request.PermanentAddress.Country,
                            District = request.PermanentAddress.District,
                            Landmark = request.PermanentAddress.Landmark,
                            Pincode = request.PermanentAddress.Pincode,
                            State = request.PermanentAddress.State,
                            StreetName = request.PermanentAddress.Street,
                            Village = request.PermanentAddress.Village,
                            DoorNo = request.PermanentAddress.DoorNo
                        }
                    : null,
                    PresentAddress = request.PresentAddress != null
                        ? new EmployeeAddress
                        {
                            CompanyId = request.CompanyIdNum,
                            City = request.PresentAddress.City,
                            Country = request.PresentAddress.Country,
                            District = request.PresentAddress.District,
                            Landmark = request.PresentAddress.Landmark,
                            Pincode = request.PresentAddress.Pincode,
                            State = request.PresentAddress.State,
                            StreetName = request.PresentAddress.Street,
                            Village = request.PresentAddress.Village,
                            DoorNo = request.PresentAddress.DoorNo
                        }
                    : null,
                    OfficialEmailId = request.OfficialEmail,
                    PersonalEmailId = request.PersonalEmail,
                    PermanentAddressSame = request.PermanentAddressSame
                };

                if (isFirstTime)
                {
                    contact.AddedBy = request.UserIdNum;
                    contact.AddedOn = DateTime.UtcNow;
                }
                else
                {
                    var alreadyAdded = employee.EmployeeContactEmployee.FirstOrDefault();
                    contact.UpdatedBy = request.UserIdNum;
                    contact.UpdatedOn = DateTime.UtcNow;
                    contact.AddedBy = alreadyAdded.AddedBy;
                    contact.AddedOn = alreadyAdded.AddedOn;
                }

                employee.EmailId = request.OfficialEmail;
                dbContext.EmployeeContact.RemoveRange(employee.EmployeeContactEmployee);
                employee.EmployeeContactEmployee.Add(contact);

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeeContact.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeeContact.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "contact",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                return GetEmployeeContacts(new EmployeeActionRequest
                {
                    EmployeeId = request.EmployeeId,
                    UserIdNum = request.UserIdNum,
                    UserId = request.UserId
                });
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeeFamilyResponse UpdateEmployeeFamily(UpdateEmployeeFamilyRequest request, bool removeExisting = false)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeFamilyEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                if (removeExisting)
                {
                    dbContext.EmployeeFamily.RemoveRange(employee.EmployeeFamilyEmployee);
                }
                foreach (var family in request.EmployeeFamily)
                {
                    if (string.IsNullOrWhiteSpace(family.EmployeeFamilyId))
                    {
                        var newfamily = new EmployeeFamily()
                        {
                            CompanyId = request.CompanyIdNum,
                            Guid = CustomGuid.NewGuid(),
                            AddedOn = DateTime.UtcNow,
                            AddedBy = request.UserIdNum,
                            IsActive = true,
                            Dob = family.Dob,
                            Occupation = family.Occupation,
                            Name = family.Name,
                            Relationship = family.Relation,
                            EmailId = family.Email,
                            IsAlive = family.IsAlive,
                            IsDependant = family.IsDependant,
                            IsEmergencyContact = family.IsEmergencyContact,
                            Phone = family.Phone,
                            Address = family.Address,
                            IsOptedForMediclaim = family.IsOptedForMediclaim
                        };
                        employee.EmployeeFamilyEmployee.Add(newfamily);
                    }
                    else
                    {
                        var addedFamily = employee.EmployeeFamilyEmployee.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(family.EmployeeFamilyId));
                        if (addedFamily != null)
                        {
                            if (family.IsActive)
                            {
                                addedFamily.Dob = family.Dob;
                                addedFamily.Occupation = family.Occupation;
                                addedFamily.Name = family.Name;
                                addedFamily.Relationship = family.Relation;
                                addedFamily.EmailId = family.Email;
                                addedFamily.IsAlive = family.IsAlive;
                                addedFamily.IsDependant = family.IsDependant;
                                addedFamily.IsEmergencyContact = family.IsEmergencyContact;
                                addedFamily.IsOptedForMediclaim = family.IsOptedForMediclaim;
                                addedFamily.Phone = family.Phone;
                                addedFamily.Address = family.Address;
                                addedFamily.UpdatedBy = request.UserIdNum;
                                addedFamily.UpdatedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                addedFamily.IsActive = false;
                                addedFamily.UpdatedBy = request.UserIdNum;
                                addedFamily.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                    }
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeeFamily.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeeFamily.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "family",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                if (removeExisting)
                {
                    return new GetEmployeeFamilyResponse()
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return GetEmployeeFamily(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeeEducationResponse UpdateEmployeeEducation(UpdateEmployeeEducationRequest request, bool removeExisting = false)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeEducationEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                if (removeExisting)
                {
                    dbContext.EmployeeEducation.RemoveRange(employee.EmployeeEducationEmployee);
                }
                foreach (var education in request.EmployeeEducation)
                {
                    if (string.IsNullOrWhiteSpace(education.EmployeeEducationId))
                    {
                        var newEducation = new EmployeeEducation()
                        {
                            Guid = CustomGuid.NewGuid(),
                            AddedOn = DateTime.UtcNow,
                            AddedBy = request.UserIdNum,
                            IsActive = true,
                            City = education.City,
                            Grade = education.Grade,
                            Institute = education.Institute,
                            State = education.State,
                            CompletedYear = education.CompletedYear,
                            CourseDuration = education.CourseDuration,
                            CourseName = education.CourseName,
                            CourseType = education.CourseType,
                            MajorSubject = education.MajorSubject,
                            MarksPercentage = education.Percentage,
                            StartedYear = education.StartedYear,
                            CompanyId = request.CompanyIdNum
                        };
                        employee.EmployeeEducationEmployee.Add(newEducation);
                    }
                    else
                    {
                        var addedEducation = employee.EmployeeEducationEmployee.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(education.EmployeeEducationId));
                        if (addedEducation != null)
                        {
                            if (education.IsActive)
                            {
                                addedEducation.City = education.City;
                                addedEducation.Grade = education.Grade;
                                addedEducation.Institute = education.Institute;
                                addedEducation.State = education.State;
                                addedEducation.CompletedYear = education.CompletedYear;
                                addedEducation.CourseDuration = education.CourseDuration;
                                addedEducation.CourseName = education.CourseName;
                                addedEducation.CourseType = education.CourseType;
                                addedEducation.MajorSubject = education.MajorSubject;
                                addedEducation.MarksPercentage = education.Percentage;
                                addedEducation.StartedYear = education.StartedYear;
                                addedEducation.UpdatedBy = request.UserIdNum;
                                addedEducation.UpdatedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                addedEducation.IsActive = false;
                                addedEducation.UpdatedBy = request.UserIdNum;
                                addedEducation.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                    }
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeeEducation.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeeEducation.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "education",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                if (removeExisting)
                {
                    return new GetEmployeeEducationResponse()
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return GetEmployeeEducation(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeeLanguageResponse UpdateEmployeeLanguage(UpdateEmployeeLanguageRequest request, bool removeExisting = false)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeLanguageEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                if (removeExisting)
                {
                    dbContext.EmployeeLanguage.RemoveRange(employee.EmployeeLanguageEmployee);
                }
                foreach (var langugage in request.EmployeeLanguage)
                {
                    if (string.IsNullOrWhiteSpace(langugage.EmployeeLanguageId))
                    {
                        var newLanguage = new EmployeeLanguage()
                        {
                            Guid = CustomGuid.NewGuid(),
                            AddedOn = DateTime.UtcNow,
                            AddedBy = request.UserIdNum,
                            IsActive = true,
                            CompanyId = request.CompanyIdNum,
                            LanguageName = langugage.Language,
                            CanRead = langugage.CanRead,
                            CanSpeak = langugage.CanSpeak,
                            CanWrite = langugage.CanWrite,
                            Level = langugage.Level
                        };
                        employee.EmployeeLanguageEmployee.Add(newLanguage);
                    }
                    else
                    {
                        var addedLanguage = employee.EmployeeLanguageEmployee.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(langugage.EmployeeLanguageId));
                        if (addedLanguage != null)
                        {
                            if (langugage.IsActive)
                            {
                                addedLanguage.LanguageName = langugage.Language;
                                addedLanguage.Level = langugage.Level;
                                addedLanguage.CanRead = langugage.CanRead;
                                addedLanguage.CanSpeak = langugage.CanSpeak;
                                addedLanguage.CanWrite = langugage.CanWrite;
                                addedLanguage.UpdatedBy = request.UserIdNum;
                                addedLanguage.UpdatedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                addedLanguage.IsActive = false;
                                addedLanguage.UpdatedBy = request.UserIdNum;
                                addedLanguage.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                    }
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeeLanguage.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeeLanguage.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "language",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                if (removeExisting)
                {
                    return new GetEmployeeLanguageResponse()
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return GetEmployeeLanguage(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeeReferenceResponse UpdateEmployeeReference(UpdateEmployeeReferenceRequest request, bool removeExisting = false)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeReferenceEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                if (removeExisting)
                {
                    dbContext.EmployeeReference.RemoveRange(employee.EmployeeReferenceEmployee);
                }
                foreach (var reference in request.EmployeeReference)
                {
                    if (string.IsNullOrWhiteSpace(reference.EmployeeReferenceId))
                    {
                        var newReference = new EmployeeReference()
                        {
                            Guid = CustomGuid.NewGuid(),
                            AddedOn = DateTime.UtcNow,
                            AddedBy = request.UserIdNum,
                            IsActive = true,
                            CompanyId = request.CompanyIdNum,
                            Address = reference.Address,
                            Designation = reference.Designation,
                            Phone = reference.Phone,
                            Remarks = reference.Remarks,
                            ReferenceName = reference.Name,
                            Company = reference.Company,
                        };
                        employee.EmployeeReferenceEmployee.Add(newReference);
                    }
                    else
                    {
                        var addedLanguage = employee.EmployeeReferenceEmployee.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(reference.EmployeeReferenceId));
                        if (addedLanguage != null)
                        {
                            if (reference.IsActive)
                            {
                                addedLanguage.Address = reference.Address;
                                addedLanguage.Designation = reference.Designation;
                                addedLanguage.Phone = reference.Phone;
                                addedLanguage.Remarks = reference.Remarks;
                                addedLanguage.ReferenceName = reference.Name;
                                addedLanguage.Company = reference.Company;
                                addedLanguage.UpdatedBy = request.UserIdNum;
                                addedLanguage.UpdatedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                addedLanguage.IsActive = false;
                                addedLanguage.UpdatedBy = request.UserIdNum;
                                addedLanguage.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                    }
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeeReference.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeeReference.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "reference",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                if (removeExisting)
                {
                    return new GetEmployeeReferenceResponse()
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return GetEmployeeReference(new EmployeeActionRequest
                    {
                        EmployeeId = request.EmployeeId,
                        UserIdNum = request.UserIdNum,
                        UserId = request.UserId
                    });
                }
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public BaseResponse ConvertEmployeeToOnRoll(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var employeeCompany = employee.EmployeeCompanyEmployee.FirstOrDefault();
                if (employeeCompany != null)
                {
                    employeeCompany.OnRollDate = DateTime.UtcNow;
                    employeeCompany.UpdatedBy = request.UserIdNum;
                    employeeCompany.UpdatedOn = DateTime.UtcNow;
                    employeeCompany.Status = "on-roll";

                    Save();

                    return new BaseResponse
                    {
                        IsSuccess = true
                    };
                }

                throw new Exception("Employee company details not found.");
            }

            throw new Exception("Employee details not found.");
        }

        public BaseResponse DeleteEmployee(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.Role)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                employee.IsActive = false;
                employee.UpdatedBy = request.UserIdNum;
                employee.UpdatedOn = DateTime.UtcNow;

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.DeleteEmployee.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.DeleteEmployee.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Employee details not found.");
        }

        public BaseResponse ToggleEmployeeLogin(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.Role)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                employee.CanLogin = !employee.CanLogin;
                employee.UpdatedBy = request.UserIdNum;
                employee.UpdatedOn = DateTime.UtcNow;

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.ToggleEmployeeLogin.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.ToggleEmployeeLogin.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeeAccountResponse GetEmployeeAccount(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.Role)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetEmployeeAccount.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetEmployeeAccount.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(19) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));

                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(19) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(19) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(19) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(19) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(19) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

                return new GetEmployeeAccountResponse
                {
                    Name = employee.Name,
                    CanLogin = employee.CanLogin ?? false,
                    EmployeeId = employee.Guid,
                    IsSuccess = true,
                    LoginEmail = employee.EmailId,
                    RoleId = employee.Role.Guid,
                    UniqueCode = employee.EmployeeCompanyEmployee.FirstOrDefault()?.UniqueCode,
                    Status = employee.EmployeeCompanyEmployee.FirstOrDefault()?.Status,
                    AddressingName = employee.EmployeeCompanyEmployee.FirstOrDefault()?.AddressingName,
                    OffRoleCode = employee.EmployeeCompanyEmployee.FirstOrDefault()?.OffRoleCode,
                    EmployeeCode = employee.EmployeeCompanyEmployee.FirstOrDefault()?.EmployeeCode,
                    Grade = employee.EmployeeCompanyEmployee.FirstOrDefault().Grade != null ? employee.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade : string.Empty,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess
                };
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeePersonalResponse GetEmployeePersonal(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeePersonalEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));

            if (employee != null)
            {
                var personal = employee.EmployeePersonalEmployee.FirstOrDefault();

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetEmployeePersonal.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetEmployeePersonal.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(1) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));

                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(1) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(1) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(1) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(1) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(1) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

                if (personal != null)
                {
                    return new GetEmployeePersonalResponse
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        Gender = personal.Gender,
                        Height = personal.Height ?? 0,
                        Nationality = personal.Nationality,
                        Sports = personal.Sports,
                        Weight = personal.Weight ?? 0,
                        ActualDob = personal.DobActual,
                        BloodGroup = personal.BloodGroup,
                        EmployeeId = employee.Guid,
                        MaritalStatus = personal.MaritalStatus,
                        MarriageDate = personal.MarriageDate,
                        RecordDob = personal.DobRecord,
                        SpecializedTraining = personal.SpecializedTraining,
                        PhotoUrl = personal.PhotoUrl,
                        PhotoLinkUrl = personal.PhotoLinkUrl,
                        HideBirthday = personal.HideBirthday ?? false
                    };
                }

                return new GetEmployeePersonalResponse
                {
                    IsSuccess = true,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                };
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeeCompanyResponse GetEmployeeCompany(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Region)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Team)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Category)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var company = employee.EmployeeCompanyEmployee.FirstOrDefault();
                if (company != null)
                {
                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.GetEmployeeCompany.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.GetEmployeeCompany.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();


                    var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                    var role = employee.RoleId;
                    var empid = request.UserIdNum;

                    var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(2) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                    var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(2) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                    var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(2) && a.Ismanager.Equals(1));
                    var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(2) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                    var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(2) && a.Ismanager.Equals(0));
                    var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(2) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                    return new GetEmployeeCompanyResponse
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        Doj = company.Doj,
                        Status = company.Status,
                        AddressingName = company.AddressingName,
                        CategoryId = company.Category != null ? company.Category.Guid : string.Empty,
                        ConfirmationRemarks = company.ConfirmationRemarks,
                        DepartmentId = company.Department != null ? company.Department.Guid : string.Empty,
                        DesignationId = company.Designation != null ? company.Designation.Guid : string.Empty,
                        EmployeeCode = company.EmployeeCode,
                        EmployeeId = employee.Guid,
                        GradeId = company.Grade != null ? company.Grade.Guid : string.Empty,
                        IsConfirmed = company.ConfirmationStatus ?? false,
                        LocationId = company.Location != null ? company.Location.Guid : string.Empty,
                        RegionId = company.Region != null ? company.Region.Guid : string.Empty,
                        TeamId = company.Team != null ? company.Team.Guid : string.Empty,
                        OffRoleCode = company.OffRoleCode,
                        ProbationEndDate = company.ProbationEndDate,
                        ProbationStartDate = company.ProbationStartDate,
                        Vendor = company.Vendor,
                        ConfirmedOn = company.ConfirmedOn,
                        ReportingToId = company.ReportingTo != null ? company.ReportingTo.Guid : string.Empty,
                        ReportingToName = company.ReportingTo != null ? company.ReportingTo.Name : string.Empty,
                        LocationBifurcation = company.LocationBifurcation,
                        ProbationExtraDays = company.ProbationExtension ?? 0,
                        StatusCategory = company.StatusCategory,
                        UniqueCode = company.UniqueCode,
                        LocationForField = company.LocationForField,
                        Division = company.Division
                    };
                }

                throw new Exception("Employee company details not found.");
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeeStatutoryResponse GetEmployeeStatutory(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeStatutoryEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var statutory = employee.EmployeeStatutoryEmployee.FirstOrDefault();
                if (statutory != null)
                {
                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.GetEmployeeStatutory.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.GetEmployeeStatutory.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                    var role = employee.RoleId;
                    var empid = request.UserIdNum;

                    var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(4) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                    var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(4) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                    var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(4) && a.Ismanager.Equals(1));
                    var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(4) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                    var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(4) && a.Ismanager.Equals(0));
                    var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(4) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

                    return new GetEmployeeStatutoryResponse
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        AadharName = statutory.AadharName,
                        AadharNumber = statutory.AadharNumber,
                        EmployeeId = employee.Guid,
                        EsiNumber = statutory.EsiNumber,
                        PanNumber = statutory.PanNumber,
                        PassportNumber = statutory.PassportNumber,
                        PassportValidity = statutory.PassportValidity,
                        PfNumber = statutory.PfNumber,
                        UanNumber = statutory.UanNumber,
                        DrivingLicenseNumber = statutory.DrivingLicenseNumber,
                        DrivingLicenseValidity = statutory.DrivingLicenseValidity,
                        LicIdNumber = statutory.LicIdNumber,
                        PreviousEmployeePensionNumber = statutory.PreviousEmpPension
                    };
                }

                return new GetEmployeeStatutoryResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeePreviousCompanyResponse GetPreviousCompany(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.EmployeePreviousCompanyEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetEmployeePreviousCompany.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetEmployeePreviousCompany.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(10) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(10) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(10) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(10) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(10) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(10) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;
                var previousCompanies = employee.EmployeePreviousCompanyEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeePreviousCompanyDto
                    {
                        PreviousCompanyId = var.Guid,
                        Department = var.Department,
                        Designation = var.Designation,
                        Employer = var.Employer,
                        DateOfExit = var.Doe,
                        DateOfJoin = var.Doj,
                        ReasonForChange = var.ReasonForChange,
                        Duration = var.Duration ?? 0,
                        Ctc = var.LastCtc ?? 0,
                        IsActive = true
                    }).ToList();

                return new GetEmployeePreviousCompanyResponse
                {
                    IsSuccess = true,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    PreviousCompanies = previousCompanies,
                    CurrentDateOfJoin = employee.EmployeeCompanyEmployee.FirstOrDefault().Doj
                };
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeeDocumentsResponse GetEmployeeDocuments(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeDocumentEmployee)
                .ThenInclude(var => var.DocumentType)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var loggedInUserCode = GetAll()
                    .Include(var => var.EmployeeCompanyEmployee)
                    .FirstOrDefault(var => var.Id == request.UserIdNum)?
                    .EmployeeCompanyEmployee.FirstOrDefault()?.EmployeeCode;

                var canGet = false;

                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(18) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(18) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(18) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(18) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(18) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(18) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                if (!string.IsNullOrWhiteSpace(loggedInUserCode))
                {
                    if (employee.Id == request.UserIdNum
                        || loggedInUserCode.Equals("9707")
                        || loggedInUserCode.Equals("9265")
                        || loggedInUserCode.Equals("9288")
                        || loggedInUserCode.Equals("9346")
                    )
                    {
                        canGet = true;
                    }
                }
                if (canGet)
                {
                    var documents = employee.EmployeeDocumentEmployee
                        .Where(var => var.IsActive)
                        .Select(var => new EmployeeDocumentDto
                        {
                            Name = var.Name,
                            Size = var.Size ?? 0,
                            IsActive = true,
                            FileUrl = var.FileUrl,
                            DocumentTypeId = var.DocumentType.Guid,
                            EmployeeDocumentId = var.Guid
                        }).ToList();

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.GetEmployeeDocuments.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.GetEmployeeDocuments.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    return new GetEmployeeDocumentsResponse
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        IsAllowed = true,
                        EmployeeDocuments = documents
                    };
                }
                else
                {
                    return new GetEmployeeDocumentsResponse
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        IsAllowed = false
                    };
                }
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeeBankResponse GetEmployeeBanks(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeBankEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var banks = employee.EmployeeBankEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeBankDto()
                    {
                        Branch = var.BankBranch,
                        AccountNumber = var.BankAccountNumber,
                        AccountType = var.AccountType,
                        BankName = var.BankName,
                        IfscCode = var.IfscCode,
                        IsActive = true,
                        EmployeeBankId = var.Guid,
                        EffectiveDate = var.EffectiveDate
                    }).ToList();

                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(6) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));

                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(6) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(6) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(6) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(6) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(6) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;



                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetEmployeeBanks.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetEmployeeBanks.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                return new GetEmployeeBankResponse()
                {
                    IsSuccess = true,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    EmployeeBanks = banks
                };
            }

            throw new Exception("Employee details not found.");
        }
        public GetEmployeeCareerResponse GetEmployeeCareers(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCareerEmployee)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                .Include(var => var.SettingsGradeAddedByNavigation)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var career = employee.EmployeeCareerEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeCareerDto()
                    {
                        AppraisalYear = var.AppraisalYear,
                        AppraisalType = var.AppraisalType,
                        Rating = var.Rating,
                        Description = var.Description,
                        Department = var.DepartmentId != 0 ? dbContext.SettingsDepartment.Where(a => a.Id.Equals(var.DepartmentId)).FirstOrDefault().Name : null,
                        Grade = var.GradeId != 0 ? dbContext.SettingsGrade.Where(a => a.Id.Equals(var.GradeId)).FirstOrDefault().Grade : null,
                        Location = var.LocationId != 0 ? dbContext.SettingsCategory.Where(a => a.Id.Equals(var.LocationId)).FirstOrDefault().Description : null,
                        Designation = var.DesignationId != 0 ? dbContext.SettingsDepartmentDesignation.Where(a => a.Id.Equals(var.DesignationId)).FirstOrDefault().Name : null,
                        RnR = var.RnR,
                        Remarks = var.Remarks,
                        ReasonForChange = var.ReasonForChange,
                        IsActive = var.IsActive,
                        EmployeeCareerId = var.Guid,
                        EffectiveFrom = var.EffectiveFrom,
                        DateofChange = var.DateofChange,
                        MovementStatus = var.MovementStatus,
                        EmployeeCode = var.EmployeeCode,
                        EmployeeName = var.AddressingName,
                        PreDepartment = var.Predepartment,
                        PreLocation = var.Prelocation
                    }).ToList();

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetEmployeeCareers.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetEmployeeCareers.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();


                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(16) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(16) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(16) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(16) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(16) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(16) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                return new GetEmployeeCareerResponse()
                {
                    IsSuccess = true,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    EmployeeCareers = career
                };
            }

            throw new Exception("Employee details not found.");
        }
        public GetEmployeeContactResponse GetEmployeeContacts(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeContactEmployee)
                .ThenInclude(var => var.PermanentAddress)
                .Include(var => var.EmployeeContactEmployee)
                .ThenInclude(var => var.PresentAddress)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var contact = employee.EmployeeContactEmployee.FirstOrDefault();
                if (contact != null)
                {
                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.GetEmployeeContacts.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.GetEmployeeContacts.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();
                    var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                    var role = employee.RoleId;
                    var empid = request.UserIdNum;

                    var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(5) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                    var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(5) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                    var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(5) && a.Ismanager.Equals(1));
                    var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(5) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                    var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(5) && a.Ismanager.Equals(0));
                    var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(5) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;



                    return new GetEmployeeContactResponse()
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        EmployeeId = employee.Guid,
                        ContactNumber = contact.ContactNumber,
                        OfficialEmail = contact.OfficialEmailId,
                        PermanentAddress = contact.PermanentAddress != null
                            ? new EmployeeAddressDto
                            {
                                City = contact.PermanentAddress.City,
                                Country = contact.PermanentAddress.Country,
                                District = contact.PermanentAddress.District,
                                Landmark = contact.PermanentAddress.Landmark,
                                Pincode = contact.PermanentAddress.Pincode,
                                State = contact.PermanentAddress.State,
                                Street = contact.PermanentAddress.StreetName,
                                Village = contact.PermanentAddress.Village,
                                DoorNo = contact.PermanentAddress.DoorNo
                            }
                            : null,
                        PersonalEmail = contact.PersonalEmailId,
                        PresentAddress = contact.PresentAddress != null
                            ? new EmployeeAddressDto
                            {
                                City = contact.PresentAddress.City,
                                Country = contact.PresentAddress.Country,
                                District = contact.PresentAddress.District,
                                Landmark = contact.PresentAddress.Landmark,
                                Pincode = contact.PresentAddress.Pincode,
                                State = contact.PresentAddress.State,
                                Street = contact.PresentAddress.StreetName,
                                Village = contact.PresentAddress.Village,
                                DoorNo = contact.PresentAddress.DoorNo
                            }
                            : null,
                        AlternateContactNumber = contact.AlternateNumber,
                        OfficialContactNumber = contact.OfficialNumber,
                        PermanentAddressSame = contact.PermanentAddressSame ?? false
                    };
                }

                return new GetEmployeeContactResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeeFamilyResponse GetEmployeeFamily(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeFamilyEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var family = employee.EmployeeFamilyEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeFamilyDto()
                    {
                        Dob = var.Dob,
                        Address = var.Address,
                        Email = var.EmailId,
                        Gender = var.Gender,
                        Name = var.Name,
                        Occupation = var.Occupation,
                        Phone = var.Phone,
                        Relation = var.Relationship,
                        IsActive = true,
                        IsAlive = var.IsAlive,
                        IsDependant = var.IsDependant ?? false,
                        EmployeeFamilyId = var.Guid,
                        IsEmergencyContact = var.IsEmergencyContact ?? false,
                        IsOptedForMediclaim = var.IsOptedForMediclaim ?? false
                    }).ToList();

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetEmployeeFamily.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetEmployeeFamily.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();


                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(8) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(8) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(8) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(8) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(8) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(8) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

                return new GetEmployeeFamilyResponse()
                {
                    IsSuccess = true,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    EmployeeFamily = family
                };
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeeEducationResponse GetEmployeeEducation(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeEducationEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var education = employee.EmployeeEducationEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeEducationDto()
                    {
                        City = var.City,
                        Grade = var.Grade,
                        Institute = var.Institute,
                        Percentage = var.MarksPercentage ?? 0,
                        State = var.State,
                        CompletedYear = var.CompletedYear ?? 0,
                        CourseDuration = var.CourseDuration ?? 0,
                        CourseName = var.CourseName,
                        CourseType = var.CourseType,
                        IsActive = true,
                        MajorSubject = var.MajorSubject,
                        StartedYear = var.StartedYear ?? 0,
                        EmployeeEducationId = var.Guid
                    }).ToList();

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetEmployeeEducation.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetEmployeeEducation.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();


                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(7) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));

                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(7) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(7) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(7) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(7) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(7) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                return new GetEmployeeEducationResponse()
                {
                    IsSuccess = true,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    EmployeeEducation = education
                };
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeeLanguageResponse GetEmployeeLanguage(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeLanguageEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var language = employee.EmployeeLanguageEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeLanguageDto()
                    {
                        IsActive = true,
                        Language = var.LanguageName,
                        CanRead = var.CanRead ?? false,
                        CanSpeak = var.CanSpeak ?? false,
                        CanWrite = var.CanWrite ?? false,
                        EmployeeLanguageId = var.Guid,
                        Level = var.Level
                    }).ToList();

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetEmployeeLanguage.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetEmployeeLanguage.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();


                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(9) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(9) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(9) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(9) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(9) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(9) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                return new GetEmployeeLanguageResponse()
                {
                    IsSuccess = true,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    EmployeeLanguage = language
                };
            }

            throw new Exception("Employee details not found.");
        }

        public GetEmployeeReferenceResponse GetEmployeeReference(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.EmployeeReferenceEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var reference = employee.EmployeeReferenceEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeReferenceDto()
                    {
                        IsActive = true,
                        Address = var.Address,
                        Company = var.Company,
                        Designation = var.Designation,
                        Email = var.EmailId,
                        Name = var.ReferenceName,
                        Phone = var.Phone,
                        Remarks = var.Remarks,
                        EmployeeReferenceId = var.Guid
                    }).ToList();

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetEmployeeReference.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetEmployeeReference.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();


                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(11) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));

                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(11) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(11) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(11) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(11) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(11) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                int empCode = 0;
                var isInt = Int32.TryParse(employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode, out empCode);
                var canAdd = employee.EmployeeCompanyEmployee.FirstOrDefault().Doj >= new DateTime(2020, 08, 1)
                    || (isInt && empCode > 9609);

                return new GetEmployeeReferenceResponse()
                {
                    CanAdd = canAdd,
                    IsSuccess = true,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    EmployeeReference = reference
                };
            }

            throw new Exception("Employee details not found.");
        }

        public EmployeeExistResponse CheckIfEmployeeExist(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.Role)
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.EmployeeCompanyReportingTo)
                .Include(var => var.SettingsAssetTypeOwner)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));

            if (employee != null)
            {
                var isHr = request.UserRole.Equals("HR");
                var isAdmin = request.UserRole.Equals("Admin");
                var isSelf = employee.Id == request.UserIdNum;
                var employeeCompany = employee.EmployeeCompanyEmployee.FirstOrDefault();
                var hasReportees = isSelf && employee.EmployeeCompanyReportingTo != null &&
                                   employee.EmployeeCompanyReportingTo.Any();
                var hasSigningAssets = isSelf && employee.SettingsAssetTypeOwner != null &&
                                   employee.SettingsAssetTypeOwner.Any();

                if (employeeCompany != null)
                {
                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.CheckIfEmployeeExist.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.CheckIfEmployeeExist.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    var code = employeeCompany.Status.Equals("on-roll")
                        ? employeeCompany.EmployeeCode
                        : employeeCompany.OffRoleCode;

                    var managerHierarchy = new List<long>();
                    var reportingManager = employeeCompany.ReportingToId;
                    while (reportingManager != null)
                    {
                        if (managerHierarchy.Contains(reportingManager ?? 0))
                        {
                            break;
                        }

                        managerHierarchy.Add(reportingManager ?? 0);
                        var managerEmployee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var => var.Id == reportingManager && var.IsActive);
                        if (managerEmployee != null)
                        {
                            reportingManager = managerEmployee.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId;
                        }
                        else
                        {
                            reportingManager = null;
                        }
                    }

                    //var isManager = managerHierarchy.Contains(request.UserIdNum);
                    var isManager = hasReportees;
                    return new EmployeeExistResponse
                    {
                        IsSuccess = true,
                        IsExist = true,
                        Name = employee.Name,
                        Code = code,
                        Role = isHr ? "hr" : isManager ? "manager" : isSelf ? "self" : isAdmin ? "Admin" : null,
                        HasAssetSignings = hasSigningAssets,
                        HasReportees = hasReportees
                    };
                }
                else
                {
                    return new EmployeeExistResponse
                    {
                        IsSuccess = true,
                        IsExist = true,
                        Name = employee.Name,
                        Code = string.Empty,
                        Role = isHr ? "hr" : isSelf ? "self" : isAdmin ? "Admin" : null,
                        HasAssetSignings = hasSigningAssets,
                        HasReportees = hasReportees
                    };
                }

            }
            else
            {
                return new EmployeeExistResponse
                {
                    IsSuccess = true,
                    IsExist = false
                };
            }
        }

        public GetTasksResponse GetAllEmployeeTasks(EmployeeTaskFilterRequest request)
        {
            var employee = GetAll()
                .Include(var => var.TaskAddedByNavigation).ThenInclude(var => var.AssignedToNavigation)
                .Include(var => var.TaskAssignedToNavigation).ThenInclude(var => var.AddedByNavigation)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var tasksForOthers = employee.TaskAddedByNavigation
                    .Where(var => var.IsActive
                                  && var.AddedBy != var.AssignedTo
                                  && (string.IsNullOrWhiteSpace(request.Title) ||
                                      var.TaskContent.Contains(request.Title, StringComparison.InvariantCultureIgnoreCase))
                                  && (request.StartDate == null || request.EndDate == null ||
                                      (request.StartDate <= var.DueDate && request.EndDate >= var.DueDate))
                                  && (request.Started == null || !request.Started.Any() ||
                                      request.Started.Contains(var.IsStarted))
                                  && (request.Completed == null || !request.Completed.Any() ||
                                      request.Completed.Contains(var.IsCompleted))
                                  && (request.Verified == null || !request.Verified.Any() ||
                                      request.Verified.Contains(var.IsVerified))
                                  && (request.AssignedTo == null || !request.AssignedTo.Any()
                                                                 || request.AssignedTo.Contains(var.AssignedToNavigation
                                                                     .Guid))
                    )
                    .Select(var => new TaskDto
                    {
                        Content = var.TaskContent,
                        AddedBy = employee.Name,
                        AddedOn = var.AddedOn,
                        AssignedTo = var.AssignedToNavigation.Name,
                        DueOn = var.DueDate,
                        TaskId = var.Guid,
                        AssignedToId = var.AssignedToNavigation.Guid,
                        IsCompleted = var.IsCompleted,
                        IsIrrelevant = var.IsIrrelevant ?? false,
                        IsStarted = var.IsStarted,
                        IsVerified = var.IsVerified,
                        IsSelf = false,
                        IsCreator = true,
                        StartedOn = var.IsStarted ? var.StartedOn : null,
                        CompletedOn = var.IsCompleted ? var.CompletedOn : null,
                        Priority = var.Priority
                    }).ToList();

                var tasksForSelf = employee.TaskAssignedToNavigation
                    .Where(var => var.IsActive
                                  && (string.IsNullOrWhiteSpace(request.Title) ||
                                      var.TaskContent.Contains(request.Title, StringComparison.InvariantCultureIgnoreCase))
                                  && (request.StartDate == null || request.EndDate == null ||
                                      (request.StartDate <= var.DueDate && request.EndDate >= var.DueDate))
                                  && (request.Started == null || !request.Started.Any() ||
                                      request.Started.Contains(var.IsStarted))
                                  && (request.Completed == null || !request.Completed.Any() ||
                                      request.Completed.Contains(var.IsCompleted))
                                  && (request.Verified == null || !request.Verified.Any() ||
                                      request.Verified.Contains(var.IsVerified))
                                  && (request.AssignedBy == null || !request.AssignedBy.Any()
                                                                 || request.AssignedBy.Contains(var.AddedByNavigation
                                                                     .Guid)))
                    .Select(var => new TaskDto
                    {
                        Content = var.TaskContent,
                        AddedBy = var.AddedByNavigation.Name,
                        AddedOn = var.AddedOn,
                        AssignedTo = employee.Name,
                        DueOn = var.DueDate,
                        TaskId = var.Guid,
                        AssignedToId = var.AssignedToNavigation.Guid,
                        IsCompleted = var.IsCompleted,
                        IsIrrelevant = var.IsIrrelevant ?? false,
                        IsStarted = var.IsStarted,
                        IsVerified = var.IsVerified,
                        IsSelf = true,
                        IsCreator = request.EmployeeId.Equals(var.AddedByNavigation.Guid),
                        StartedOn = var.IsStarted ? var.StartedOn : null,
                        CompletedOn = var.IsCompleted ? var.CompletedOn : null,
                        Priority = var.Priority
                    }).ToList();


                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(26) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(26) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(26) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(26) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(26) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(26) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                var finalList = tasksForOthers
                    .Concat(tasksForSelf)
                    .OrderBy(var => var.IsStarted)
                    .ThenBy(var => var.IsCompleted)
                    .ThenBy(var => var.IsVerified)
                    .ThenBy(var => var.DueOn)
                    .ThenByDescending(var => var.AddedOn)
                    .ToList();
                return new GetTasksResponse
                {
                    Tasks = finalList,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    IsSuccess = true
                };
            }

            throw new Exception("Employee not found");
        }

        public EmployeeBaseInfoResponse GetAllEmployeesBaseInfo(EmployeeInfoRequest request)
        {
            var employees = GetAll()
                .Include(var => var.Role)
                .Include(var => var.EmployeeCompanyEmployee)
                .Where(var => var.IsActive
                              && (request.IncludeSelf || var.Guid != request.EmployeeId)
                              && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                              && (request.Role == null || var.Role.RoleName == request.Role)
                              )
                .Select(var => new EmployeeBaseInfoDto
                {
                    EmployeeCode = var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                    EmployeeId = var.Guid,
                    EmployeeName = var.Name
                })
                .ToList();

            return new EmployeeBaseInfoResponse
            {
                IsSuccess = true,
                Employees = employees
            };
        }

        public EmployeeBaseInfoResponse GetReportingEmployeesBaseInfo(EmployeeActionRequest request)
        {
            var employees = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .ThenInclude(var => var.ReportingTo)
                .Where(var => var.IsActive
                              && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                              && (var.Guid == request.EmployeeId
                                  ||
                                  var.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo.Guid == request.EmployeeId)
                )
                .Select(var => new EmployeeBaseInfoDto
                {
                    EmployeeCode = var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                    EmployeeId = var.Guid,
                    EmployeeName = var.Name
                })
                .ToList();

            return new EmployeeBaseInfoResponse
            {
                IsSuccess = true,
                Employees = employees
            };
        }

        public GetTicketsResponse GetEmployeeTickets(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.TicketAddedByNavigation).ThenInclude(var => var.Category)
                .Include(var => var.TicketAddedByNavigation).ThenInclude(var => var.SubCategory)
                .Include(var => var.TicketAddedByNavigation).ThenInclude(var => var.Status)
                .Include(var => var.TicketAddedByNavigation).ThenInclude(var => var.AddedByNavigation)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var tickets = employee.TicketAddedByNavigation
                    .Where(var => var.IsActive)
                    .Select(var => new TicketDto
                    {
                        Category = var.Category != null ? var.Category.Name : string.Empty,
                        SubCategory = var.SubCategory != null ? var.SubCategory.Name : string.Empty,
                        Status = var.Status != null ? var.Status.Name : string.Empty,
                        AddedBy = employee.Name,
                        Title = var.Title,
                        AddedOn = var.AddedOn,
                        TicketId = var.Guid,
                        StartedOn = var.TicketStatus
                                        .OrderByDescending(var1 => var1.ChangedOn)
                                        .FirstOrDefault(var1 => var1.Status.Name.Equals("Working")) != null
                            ? var.TicketStatus
                                .OrderByDescending(var1 => var1.ChangedOn)
                                .FirstOrDefault(var1 => var1.Status.Name.Equals("Working"))
                                .ChangedOn
                            : (DateTime?)null,
                        ClosedOn = var.TicketStatus
                                       .OrderByDescending(var1 => var1.ChangedOn)
                                       .FirstOrDefault(var1 => var1.Status.Name.Equals("Closed")) != null
                            ? var.TicketStatus
                                .OrderByDescending(var1 => var1.ChangedOn)
                                .FirstOrDefault(var1 => var1.Status.Name.Equals("Closed"))
                                .ChangedOn
                            : (DateTime?)null,
                    })
                    .ToList();
            }

            throw new Exception("Employee not found.");
        }

        public EmployeeListResponse GetEmployeeReportees(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeePersonalEmployee)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Region)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Team)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo).ThenInclude(var => var.EmployeeCompanyEmployee)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Category)
                .Include(var => var.EmployeeCompanyReportingTo)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {

                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(20) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(20) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(20) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(20) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(20) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(20) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

                var employeeCompany = employee.EmployeeCompanyEmployee.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(employeeCompany.EmployeeCode) && employeeCompany.Department != null)
                {
                    var managerHierarchy = new List<EmployeeCardDto>();
                    managerHierarchy = GetEmployeesReporting(employee.Guid, managerHierarchy, true);

                    var employees =
                        GetAll()
                            .Include(var => var.Role)
                            .Include(var => var.EmployeeCompanyEmployee)
                            .Include(var => var.EmployeeContactEmployee)
                            .Where(var => managerHierarchy.Any(var1 => var1.EmployeeId.Equals(var.Guid)))
                            .Select(var => new EmployeeDto
                            {
                                Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                                Grade = var.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade,
                                Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                                Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                                Region = var.EmployeeCompanyEmployee.FirstOrDefault().Region.Name,
                                Team = var.EmployeeCompanyEmployee.FirstOrDefault().Team.Name,
                                EmployeeId = var.Guid,
                                EmailId = var.EmailId,
                                Name = var.Name,
                                CanLogin = var.CanLogin ?? false,
                                Status = var.EmployeeCompanyEmployee.FirstOrDefault().Status,
                                Role = var.Role.RoleName,
                                Code = var.EmployeeCompanyEmployee.FirstOrDefault().Status.Equals("on-roll")
                                    ? var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode
                                    : var.EmployeeCompanyEmployee.FirstOrDefault().OffRoleCode
                            })
                            .ToList();

                    return new EmployeeListResponse
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        Employees = employees
                    };
                }
                else
                {
                    return new EmployeeListResponse
                    {
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        IsSuccess = true,
                    };
                }
            }
            else
            {
                throw new Exception("Employee details not found.");
            }
        }

        public GetEmployeeSigningsResponse GetEmployeeAssetSignings(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.SettingsAssetTypeOwner)
                .ThenInclude(var => var.AssetType)
                .ThenInclude(var => var.EmployeeAsset)
                .ThenInclude(var => var.Employee)
                .ThenInclude(var => var.EmployeeCompanyEmployee)
                .Include(var => var.SettingsAssetTypeOwner)
                .ThenInclude(var => var.AssetType)
                .ThenInclude(var => var.EmployeeAsset)
                .ThenInclude(var => var.UpdatedByNavigation)
                .Include(var => var.SettingsAssetTypeOwner)
                .ThenInclude(var => var.AssetType)
                .ThenInclude(var => var.EmployeeAsset)
                .ThenInclude(var => var.AddedByNavigation)
                .FirstOrDefault(var => var.IsActive && var.Id.Equals(request.UserIdNum));
            if (employee != null)
            {
                var employeesList = new List<EmployeeSigningAssetDto>();

                foreach (var owningAsset in employee.SettingsAssetTypeOwner)
                {
                    var employees = owningAsset.AssetType.EmployeeAsset
                        .Where(var => var.IsActive)
                        .Select(var => new EmployeeSigningAssetDto
                        {
                            EmployeeId = var.Employee.Guid,
                            IsActive = true,
                            AssetId = var.Asset.Guid,
                            AssetName = var.Asset.AssetType,
                            AssetUniqueId = var.AssetUniqueId,
                            Description = var.Description,
                            EmployeeAssetId = var.Guid,
                            EmployeeCode = var.Employee.EmployeeCompanyEmployee.FirstOrDefault()?.EmployeeCode,
                            EmployeeName = var.Employee.Name,
                            GivenOn = var.GivenOn,
                            Doj = var.Employee.EmployeeCompanyEmployee.FirstOrDefault()?.Doj,
                            LastUpdatedBy = var.UpdatedByNavigation != null ? var.UpdatedByNavigation.Name : var.AddedByNavigation.Name,
                            LastUpdatedOn = var.UpdatedOn.HasValue ? var.UpdatedOn : var.AddedOn
                        })
                        .ToList();

                    employeesList.AddRange(employees);
                }

                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(13) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(13) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(13) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(13) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(13) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(13) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                return new GetEmployeeSigningsResponse
                {
                    Assets = employeesList,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    IsSuccess = true
                };
            }

            throw new Exception("Employee not found");
        }

        public GetEmployeeSigningsResponse UpdateEmployeeAssetSignings(UpdateEmployeeAssetSigningRequest request)
        {
            foreach (var asset in request.Assets)
            {
                var employee = GetAll()
                    .Include(var => var.EmployeeAssetEmployee)
                    .FirstOrDefault(var => var.IsActive && var.Guid.Equals(asset.EmployeeId));
                if (employee != null)
                {
                    var addedAsset = employee.EmployeeAssetEmployee.FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(asset.EmployeeAssetId));

                    var assetType = dbContext.SettingsAssetTypes
                        .Include(var => var.EmployeeAsset)
                        .FirstOrDefault(var => var.IsActive && var.Guid.Equals(asset.AssetId));

                    var isAssetUniqueIdTaken = assetType.EmployeeAsset.Any(var =>
                        var.IsActive && !string.IsNullOrWhiteSpace(asset.AssetUniqueId) && asset.AssetUniqueId.Equals(var.AssetUniqueId) && var.Id != addedAsset.Id);

                    if (isAssetUniqueIdTaken)
                    {
                        return new GetEmployeeSigningsResponse
                        {
                            IsSuccess = true,
                            AssetCodeTaken = true,
                            AssetUniqueId = asset.AssetUniqueId
                        };
                    }

                    if (addedAsset != null)
                    {
                        addedAsset.Description = asset.Description;
                        addedAsset.AssetUniqueId = asset.AssetUniqueId;
                        addedAsset.GivenOn = asset.GivenOn;
                        addedAsset.UpdatedBy = request.UserIdNum;
                        addedAsset.UpdatedOn = DateTime.UtcNow;
                    }

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.UpdateAssets.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.UpdateAssets.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                }
                else
                {
                    throw new Exception("Employee not found - " + asset.EmployeeId);
                }
            }
            Save();

            return GetEmployeeAssetSignings(new EmployeeActionRequest
            {
                EmployeeId = request.UserId,
                UserIdNum = request.UserIdNum,
                UserId = request.UserId
            });
        }

        public GetEmployeeAppraisalResponse GetEmployeeSingleAppraisalDetails(EmployeeAppraisalActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .ThenInclude(var => var.AppraisalQuestion)
                .ThenInclude(var => var.Question)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalAnswer)
                .ThenInclude(var => var.AppraisalQuestion)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalSelfAnswer)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalBusinessNeed)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalFeedback)
                .ThenInclude(var => var.GivenByNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalFeedback)
                .ThenInclude(var => var.AppraiseeTypeNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.RatingNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.VariableBonusRatingNavigation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Category)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalTraining).ThenInclude(var => var.AddedByNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalTraining).ThenInclude(var => var.Training)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {
                var grade = employee.EmployeeCompanyEmployee.FirstOrDefault().Grade != null ? employee.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade : string.Empty;
                var empCategory = employee.EmployeeCompanyEmployee.FirstOrDefault().Category != null ? employee.EmployeeCompanyEmployee.FirstOrDefault().Category.Category : string.Empty;
                var isManager = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId == request.UserIdNum;
                var managerId = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId;
                var isL2Manager = false;
                if (managerId != null)
                {
                    isL2Manager = GetAll().Include(var => var.EmployeeCompanyEmployee)
                                         .FirstOrDefault(var => var.Id.Equals(managerId))?.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId == request.UserIdNum;
                }
                var appraisals = new List<EmployeeAppraisalDto>();
                var allAppraisals = employee.AppraisalEmployeeEmployee.Where(var => var.IsActive && var.Guid.Equals(request.EmployeeAppraisalId));
                foreach (var appraisal in allAppraisals)
                {
                    var appraisalMode = appraisal.Appraisal.Mode;
                    var appraisalDto = new EmployeeAppraisalDto
                    {
                        IsManager = isManager,
                        IsSelf = appraisal.EmployeeId == request.UserIdNum,
                        IsL2Manager = isL2Manager,
                        Title = appraisal.Appraisal.Title,
                        Rating = (appraisalMode == 2 ? (appraisal.VariableBonusRatingNavigation != null ? appraisal.VariableBonusRatingNavigation.Guid : null)
                            : (appraisal.RatingNavigation != null ? appraisal.RatingNavigation.Guid : null)),
                        IsActive = appraisal.Appraisal.IsOpen,
                        StartDate = appraisal.Appraisal.StartDate,
                        EndDate = appraisal.Appraisal.EndDate,
                        SelfAppraisalDoneOn = appraisalMode == 2 ? appraisal.SelfVariableSubmittedOn : appraisal.SelfSubmittedOn,
                        ShowCalculation = appraisal.Appraisal.ShowCalculation ?? false,
                        AppraisalClosedOn = appraisalMode == 2 ? appraisal.HrVariableSubmittedOn : appraisal.ClosedOn,
                        RmSubmittedOn = appraisalMode == 2 ? appraisal.RmVariableSubmittedOn : appraisal.RmSubmittedOn,
                        L2SubmittedOn = appraisalMode == 2 ? appraisal.L2VariableSubmittedOn : appraisal.L2SubmittedOn,
                        EmployeeAppraisalId = appraisal.Guid,
                        InternalMgmt = appraisalMode == 2 ? appraisal.InternalVariableMgmt ?? 0 : appraisal.InternalMgmt ?? 0,
                        InternalL2 = appraisalMode == 2 ? appraisal.InternalVariableL2 ?? 0 : appraisal.InternalL2 ?? 0,
                        InternalSelf = appraisalMode == 2 ? appraisal.InternalVariableSelf ?? 0 : appraisal.InternalSelf ?? 0,
                        Category = appraisal.Appraisal.Category,
                        Mode = appraisal.Appraisal.Mode == 1 ? "objective" : appraisal.Appraisal.Mode == 2 ? "variablebonus" : "appraisal",
                        CalculationMethod = appraisal.Appraisal.CalculationMethod,
                        SelfObjectiveSubmittedOn = appraisal.SelfObjectiveSubmittedOn,
                        RmObjectiveSubmittedOn = appraisal.RmObjectiveSubmittedOn,
                        L2ObjectiveSubmittedOn = appraisal.L2ObjectiveSubmittedOn,
                        HrObjectiveSubmittedOn = appraisal.HrObjectiveSubmittedOn,
                        Grade = grade,
                        EmployeeCateogry = empCategory,
                        IsPromotionRecommended = appraisal.IsRecommendedForPromotion.HasValue ? appraisal.IsRecommendedForPromotion.Value : false,
                        IsFitmentRecommended = appraisal.IsRecommendedForFigment.HasValue ? appraisal.IsRecommendedForFigment.Value : false,
                        TrainingComments = appraisal.TrainingComments,
                        Feedbacks = appraisal.AppraisalFeedback.Where(var => var.IsActive)
                            .Select(var => new EmployeeAppraisalFeedbackDto
                            {
                                Feedback = var.Feedback,
                                AppraiseeType = var.AppraiseeTypeNavigation.AppraiseeType,
                                GivenByName = var.GivenByNavigation.Name,
                                GivenBy = var.GivenByNavigation.Guid,
                                GivenOn = var.AddedOn,
                                EmployeeFeedbackId = var.Guid,
                                AppraisalMode = var.AppraisalMode
                            })
                            .ToList(),
                        SelfAnswers = appraisal.AppraisalSelfAnswer.Where(var => var.IsActive)
                            .Select(var => new EmployeeAppraisalSelfAnswerDto
                            {
                                Description = var.Answer,
                                Title = var.Title,
                                Weightage = var.Percentage,
                                IsActive = true,
                                //ManagementWeightage = var.ManagementWeightage,
                                //SelfWeightage = var.SelfWeightage,
                                ManagementWeightage = appraisalMode == 2 ? (var.ManagementVariableWeightage != null ? var.ManagementVariableWeightage.Value : 0) : var.ManagementWeightage,
                                SelfWeightage = appraisalMode == 2 ? (var.SelfVariableWeightage != null ? var.SelfVariableWeightage.Value : 0) : var.SelfWeightage,
                                SelfAppraisalAnswerId = var.Guid,
                                //L2Weightage = var.L2Weightage ?? 0,
                                L2Weightage = appraisalMode == 2 ? (var.L2VariableWeightage != null ? var.L2VariableWeightage.Value : 0) : (var.L2Weightage != null ? var.L2Weightage.Value : 0),
                                AppraisalMode = var.AppraisalMode
                            })
                            .ToList(),
                        BusinessNeeds = appraisal.AppraisalBusinessNeed.Where(var => var.IsActive)
                            .Select(var => new EmployeeAppraisalBusinessNeedAnswerDto
                            {
                                Description = var.Answer,
                                Title = var.Title,
                                Weightage = var.Percentage,
                                IsActive = true,
                                ManagementWeightage = var.ManagementWeightage,
                                SelfWeightage = var.SelfWeightage,
                                BusinessNeedAnswerId = var.Guid,
                                L2Weightage = var.L2Weightage ?? 0,
                                AppraisalMode = var.AppraisalMode
                            })
                            .ToList(),
                        Trainings = appraisal.AppraisalTraining.Where(var => var.IsActive)
                            .Select(var => new AppraisalTrainingDto
                            {
                                TrainingId = var.Training.Guid,
                                AddedBy = var.AddedByNavigation.Guid,
                                IsSelf = var.AddedBy == appraisal.EmployeeId
                            })
                            .ToList()
                    };

                    var answers = new List<EmployeeAppraisalQuestionAnswerDto>();
                    if (appraisal.AppraisalAnswer != null && appraisal.AppraisalAnswer.Any())
                    {
                        answers =
                            appraisal.AppraisalAnswer.Where(var => var.IsActive)
                                .Select(var => new EmployeeAppraisalQuestionAnswerDto
                                {
                                    Description = var.AppraisalQuestion.Question.Description,
                                    Weightage = var.AppraisalQuestion.Percentage,
                                    Title = var.AppraisalQuestion.Question.Question,
                                    ManagementWeightage = appraisalMode == 2 ? (var.ManagementVariableWeightage != null ? var.ManagementVariableWeightage.Value : 0) : var.ManagementWeightage,
                                    SelfWeightage = appraisalMode == 2 ? (var.SelfVariableWeightage != null ? var.SelfVariableWeightage.Value : 0) : var.SelfWeightage,
                                    EmployeeAnswerId = var.Guid,
                                    Answer = var.Answer,
                                    EmployeeQuestionId = var.AppraisalQuestion.Guid,
                                    L2Weightage = appraisalMode == 2 ? (var.L2VariableWeightage != null ? var.L2VariableWeightage.Value : 0) : var.L2Weightage
                                })
                                .ToList();
                    }
                    else
                    {
                        answers =
                            appraisal.Appraisal.AppraisalQuestion.Where(var => var.IsActive)
                                .Select(var => new EmployeeAppraisalQuestionAnswerDto
                                {
                                    Description = var.Question.Description,
                                    Weightage = var.Percentage,
                                    Title = var.Question.Question,
                                    ManagementWeightage = 0,
                                    SelfWeightage = 0,
                                    L2Weightage = 0,
                                    EmployeeAnswerId = null,
                                    Answer = string.Empty,
                                    EmployeeQuestionId = var.Guid
                                })
                                .ToList();
                    }

                    appraisalDto.Questions = answers;
                    appraisals.Add(appraisalDto);
                }

                return new GetEmployeeAppraisalResponse
                {
                    IsSuccess = true,
                    Appraisals = appraisals
                };
            }

            throw new Exception("Employee not found.");
        }

        public GetEmployeeAppraisalResponse GetEmployeeAppraisalDetails(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeDocumentEmployee).ThenInclude(var => var.DocumentType)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .ThenInclude(var => var.AppraisalQuestion)
                .ThenInclude(var => var.Question)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalAnswer)
                .ThenInclude(var => var.AppraisalQuestion)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalSelfAnswer)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalBusinessNeed)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalFeedback)
                .ThenInclude(var => var.GivenByNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalFeedback)
                .ThenInclude(var => var.AppraiseeTypeNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.RatingNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.VariableBonusRatingNavigation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Category)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalTraining).ThenInclude(var => var.AddedByNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalTraining).ThenInclude(var => var.Training)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {


                //var isManagerrole = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

                var canGet = true;
                //var canGet = false;

                //if (request.UserRole != "HR")
                //{
                //    canGet = true;
                //}
                //else
                //{
                //    var loggedInUserCode = GetAll()
                //     .Include(var => var.EmployeeCompanyEmployee)
                //     .FirstOrDefault(var => var.Id == request.UserIdNum)?
                //     .EmployeeCompanyEmployee.FirstOrDefault()?.EmployeeCode;
                //    if (!string.IsNullOrWhiteSpace(loggedInUserCode))
                //    {
                //        if (loggedInUserCode.Equals("9265") || loggedInUserCode.Equals("K-22222"))
                //        {
                //            canGet = true;
                //        }
                //    }
                //}

                if (canGet)
                {
                    var grade = employee.EmployeeCompanyEmployee.FirstOrDefault().Grade != null ? employee.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade : string.Empty;
                    var empCategory = employee.EmployeeCompanyEmployee.FirstOrDefault().Category != null ? employee.EmployeeCompanyEmployee.FirstOrDefault().Category.Category : string.Empty;
                    var isManager = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId == request.UserIdNum;
                    var managerId = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId;
                    var isL2Manager = false;
                    if (managerId != null)
                    {
                        isL2Manager = GetAll().Include(var => var.EmployeeCompanyEmployee)
                                          .FirstOrDefault(var => var.Id.Equals(managerId))?.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId == request.UserIdNum;
                    }
                    var appraisals = new List<EmployeeAppraisalDto>();
                    var allAppraisals = employee.AppraisalEmployeeEmployee.Where(var => var.IsActive);
                    foreach (var appraisal in allAppraisals)
                    {
                        var appraisalMode = appraisal.Appraisal.Mode;
                        var appraisalDto = new EmployeeAppraisalDto
                        {
                            IsManager = isManager,
                            IsSelf = appraisal.EmployeeId == request.UserIdNum,
                            IsL2Manager = isL2Manager,
                            Title = appraisal.Appraisal.Title,
                            Rating = (appraisalMode == 2 ? (appraisal.VariableBonusRatingNavigation != null ? appraisal.VariableBonusRatingNavigation.Guid : null)
                                : (appraisal.RatingNavigation != null ? appraisal.RatingNavigation.Guid : null)),
                            IsActive = appraisal.Appraisal.IsOpen,
                            StartDate = appraisal.Appraisal.StartDate,
                            EndDate = appraisal.Appraisal.EndDate,
                            SelfAppraisalDoneOn = appraisalMode == 2 ? appraisal.SelfVariableSubmittedOn : appraisal.SelfSubmittedOn,
                            AppraisalClosedOn = appraisalMode == 2 ? appraisal.HrVariableSubmittedOn : appraisal.ClosedOn,
                            RmSubmittedOn = appraisalMode == 2 ? appraisal.RmVariableSubmittedOn : appraisal.RmSubmittedOn,
                            EmployeeAppraisalId = appraisal.Guid,
                            ShowCalculation = appraisal.Appraisal.ShowCalculation ?? false,
                            L2SubmittedOn = appraisalMode == 2 ? appraisal.L2VariableSubmittedOn : appraisal.L2SubmittedOn,
                            InternalMgmt = appraisalMode == 2 ? appraisal.InternalVariableMgmt ?? 0 : appraisal.InternalMgmt ?? 0,
                            InternalL2 = appraisalMode == 2 ? appraisal.InternalVariableL2 ?? 0 : appraisal.InternalL2 ?? 0,
                            InternalSelf = appraisalMode == 2 ? appraisal.InternalVariableSelf ?? 0 : appraisal.InternalSelf ?? 0,
                            CalculationMethod = appraisal.Appraisal.CalculationMethod,
                            Mode = appraisal.Appraisal.Mode == 1 ? "objective" : appraisal.Appraisal.Mode == 2 ? "variablebonus" : "appraisal",
                            Category = appraisal.Appraisal.Category,
                            HrSubmittedOn = appraisal.HrSubmittedOn,
                            SelfObjectiveSubmittedOn = appraisal.SelfObjectiveSubmittedOn,
                            RmObjectiveSubmittedOn = appraisal.RmObjectiveSubmittedOn,
                            L2ObjectiveSubmittedOn = appraisal.L2ObjectiveSubmittedOn,
                            HrObjectiveSubmittedOn = appraisal.HrObjectiveSubmittedOn,
                            Grade = grade,
                            EmployeeCateogry = empCategory,
                            IsPromotionRecommended = appraisal.IsRecommendedForPromotion.HasValue ? appraisal.IsRecommendedForPromotion.Value : false,
                            IsFitmentRecommended = appraisal.IsRecommendedForFigment.HasValue ? appraisal.IsRecommendedForFigment.Value : false,
                            TrainingComments = appraisal.TrainingComments,
                            Feedbacks = appraisal.AppraisalFeedback.Where(var => var.IsActive)
                                .Select(var => new EmployeeAppraisalFeedbackDto
                                {
                                    Feedback = var.Feedback,
                                    AppraiseeType = var.AppraiseeTypeNavigation.AppraiseeType,
                                    GivenByName = var.GivenByNavigation.Name,
                                    GivenBy = var.GivenByNavigation.Guid,
                                    GivenOn = var.AddedOn,
                                    EmployeeFeedbackId = var.Guid,
                                    AppraisalMode = var.AppraisalMode
                                })
                                .ToList(),
                            SelfAnswers = appraisal.AppraisalSelfAnswer.Where(var => var.IsActive)
                                .Select(var => new EmployeeAppraisalSelfAnswerDto
                                {
                                    Description = var.Answer,
                                    Title = var.Title,
                                    Weightage = var.Percentage,
                                    IsActive = true,
                                    //ManagementWeightage = var.ManagementWeightage,
                                    //SelfWeightage = var.SelfWeightage,
                                    ManagementWeightage = appraisalMode == 2 ? (var.ManagementVariableWeightage != null ? var.ManagementVariableWeightage.Value : 0) : var.ManagementWeightage,
                                    SelfWeightage = appraisalMode == 2 ? (var.SelfVariableWeightage != null ? var.SelfVariableWeightage.Value : 0) : var.SelfWeightage,
                                    SelfAppraisalAnswerId = var.Guid,
                                    //L2Weightage = var.L2Weightage ?? 0,
                                    L2Weightage = appraisalMode == 2 ? (var.L2VariableWeightage != null ? var.L2VariableWeightage.Value : 0) : (var.L2Weightage != null ? var.L2Weightage.Value : 0),
                                    AppraisalMode = var.AppraisalMode
                                })
                                .ToList(),
                            BusinessNeeds = appraisal.AppraisalBusinessNeed.Where(var => var.IsActive)
                                .Select(var => new EmployeeAppraisalBusinessNeedAnswerDto
                                {
                                    Description = var.Answer,
                                    Title = var.Title,
                                    Weightage = var.Percentage,
                                    IsActive = true,
                                    ManagementWeightage = var.ManagementWeightage,
                                    SelfWeightage = var.SelfWeightage,
                                    BusinessNeedAnswerId = var.Guid,
                                    L2Weightage = var.L2Weightage ?? 0,
                                    AppraisalMode = var.AppraisalMode
                                })
                                .ToList(),
                            Trainings = appraisal.AppraisalTraining.Where(var => var.IsActive)
                                .Select(var => new AppraisalTrainingDto
                                {
                                    TrainingId = var.Training.Guid,
                                    AddedBy = var.AddedByNavigation.Guid,
                                    IsSelf = var.AddedBy == appraisal.EmployeeId
                                })
                                .ToList()
                        };

                        var answers = new List<EmployeeAppraisalQuestionAnswerDto>();
                        if (appraisal.AppraisalAnswer != null && appraisal.AppraisalAnswer.Any())
                        {
                            answers =
                                appraisal.AppraisalAnswer.Where(var => var.IsActive)
                                    .Select(var => new EmployeeAppraisalQuestionAnswerDto
                                    {
                                        Description = var.AppraisalQuestion.Question.Description,
                                        Weightage = var.AppraisalQuestion.Percentage,
                                        Title = var.AppraisalQuestion.Question.Question,
                                        ManagementWeightage = appraisalMode == 2 ? (var.ManagementVariableWeightage != null ? var.ManagementVariableWeightage.Value : 0) : var.ManagementWeightage,
                                        SelfWeightage = appraisalMode == 2 ? (var.SelfVariableWeightage != null ? var.SelfVariableWeightage.Value : 0) : var.SelfWeightage,
                                        EmployeeAnswerId = var.Guid,
                                        Answer = var.Answer,
                                        EmployeeQuestionId = var.AppraisalQuestion.Guid,
                                        L2Weightage = appraisalMode == 2 ? (var.L2VariableWeightage != null ? var.L2VariableWeightage.Value : 0) : var.L2Weightage
                                    })
                                    .ToList();
                        }
                        else
                        {
                            answers =
                                appraisal.Appraisal.AppraisalQuestion.Where(var => var.IsActive)
                                    .Select(var => new EmployeeAppraisalQuestionAnswerDto
                                    {
                                        Description = var.Question.Description,
                                        Weightage = var.Percentage,
                                        Title = var.Question.Question,
                                        ManagementWeightage = 0,
                                        SelfWeightage = 0,
                                        L2Weightage = 0,
                                        EmployeeAnswerId = null,
                                        Answer = string.Empty,
                                        EmployeeQuestionId = var.Guid
                                    })
                                    .ToList();
                        }

                        appraisalDto.Questions = answers;
                        appraisals.Add(appraisalDto);
                    }

                    var objectiveDocuments = employee.EmployeeDocumentEmployee.Where(var =>
                        var.IsActive && var.DocumentType.DocumentType.Equals("Appraisal Objectives"))
                        .Select(var => new EmployeeDocumentDto
                        {
                            IsActive = true,
                            Name = var.Name,
                            EmployeeDocumentId = var.Guid,
                            FileLocation = var.FileLocation,
                            FileUrl = var.FileUrl
                        })
                        .ToList();

                    return new GetEmployeeAppraisalResponse
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        Appraisals = appraisals,
                        IsAllowed = true,
                        AppraisalObjectiveDocuments = objectiveDocuments
                    };
                }
                else
                {
                    return new GetEmployeeAppraisalResponse
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        IsAllowed = false,
                    };
                }
            }

            throw new Exception("Employee not found.");
        }

        public GetEmployeeAppraisalResponse GetEmployeeAppraisalReport(EmployeeAppraisalActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeDocumentEmployee).ThenInclude(var => var.DocumentType)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .ThenInclude(var => var.AppraisalQuestion)
                .ThenInclude(var => var.Question)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalAnswer)
                .ThenInclude(var => var.AppraisalQuestion)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalSelfAnswer)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalBusinessNeed)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalFeedback)
                .ThenInclude(var => var.GivenByNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalFeedback)
                .ThenInclude(var => var.AppraiseeTypeNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.RatingNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.VariableBonusRatingNavigation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Category)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalTraining).ThenInclude(var => var.AddedByNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalTraining).ThenInclude(var => var.Training)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {
                var grade = employee.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade;
                var empCategory = employee.EmployeeCompanyEmployee.FirstOrDefault().Category.Category;
                var isManager = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId == request.UserIdNum;
                var managerId = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId;
                var isL2Manager = false;
                if (managerId != null)
                {
                    isL2Manager = GetAll().Include(var => var.EmployeeCompanyEmployee)
                                      .FirstOrDefault(var => var.Id.Equals(managerId))?.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId == request.UserIdNum;
                }
                var appraisals = new List<EmployeeAppraisalDto>();
                var allAppraisals = employee.AppraisalEmployeeEmployee.Where(var => var.IsActive && var.Appraisal.Guid.Equals(request.EmployeeAppraisalId));
                foreach (var appraisal in allAppraisals)
                {
                    var appraisalMode = request.AppraisalMode;
                    var appraisalDto = new EmployeeAppraisalDto
                    {
                        IsManager = isManager,
                        IsSelf = appraisal.EmployeeId == request.UserIdNum,
                        IsL2Manager = isL2Manager,
                        Title = appraisal.Appraisal.Title,
                        Rating = (appraisalMode == 2 ? (appraisal.VariableBonusRatingNavigation != null ? appraisal.VariableBonusRatingNavigation.Guid : null)
                            : (appraisal.RatingNavigation != null ? appraisal.RatingNavigation.Guid : null)),
                        IsActive = appraisal.Appraisal.IsOpen,
                        StartDate = appraisal.Appraisal.StartDate,
                        EndDate = appraisal.Appraisal.EndDate,
                        SelfAppraisalDoneOn = appraisalMode == 2 ? appraisal.SelfVariableSubmittedOn : appraisal.SelfSubmittedOn,
                        AppraisalClosedOn = appraisalMode == 2 ? appraisal.HrVariableSubmittedOn : appraisal.ClosedOn,
                        RmSubmittedOn = appraisalMode == 2 ? appraisal.RmVariableSubmittedOn : appraisal.RmSubmittedOn,
                        EmployeeAppraisalId = appraisal.Guid,
                        ShowCalculation = appraisal.Appraisal.ShowCalculation ?? false,
                        L2SubmittedOn = appraisalMode == 2 ? appraisal.L2VariableSubmittedOn : appraisal.L2SubmittedOn,
                        InternalMgmt = appraisalMode == 1 ? 0 : appraisalMode == 2 ? appraisal.InternalVariableMgmt ?? 0 : appraisal.InternalMgmt ?? 0,
                        InternalL2 = appraisalMode == 1 ? 0 : appraisalMode == 2 ? appraisal.InternalVariableL2 ?? 0 : appraisal.InternalL2 ?? 0,
                        InternalSelf = appraisalMode == 1 ? 0 : appraisalMode == 2 ? appraisal.InternalVariableSelf ?? 0 : appraisal.InternalSelf ?? 0,
                        CalculationMethod = appraisal.Appraisal.CalculationMethod,
                        Mode = appraisalMode == 1 ? "objective" : appraisalMode == 2 ? "variablebonus" : "appraisal",
                        Category = appraisal.Appraisal.Category,
                        HrSubmittedOn = appraisal.HrSubmittedOn,
                        SelfObjectiveSubmittedOn = appraisal.SelfObjectiveSubmittedOn,
                        RmObjectiveSubmittedOn = appraisal.RmObjectiveSubmittedOn,
                        L2ObjectiveSubmittedOn = appraisal.L2ObjectiveSubmittedOn,
                        HrObjectiveSubmittedOn = appraisal.HrObjectiveSubmittedOn,
                        Grade = grade,
                        EmployeeCateogry = empCategory,
                        IsPromotionRecommended = appraisal.IsRecommendedForPromotion.HasValue ? appraisal.IsRecommendedForPromotion.Value : false,
                        IsFitmentRecommended = appraisal.IsRecommendedForFigment.HasValue ? appraisal.IsRecommendedForFigment.Value : false,
                        TrainingComments = appraisal.TrainingComments,
                        Feedbacks = appraisal.AppraisalFeedback.Where(var => var.IsActive && var.AppraisalMode == appraisalMode)
                            .Select(var => new EmployeeAppraisalFeedbackDto
                            {
                                Feedback = var.Feedback,
                                AppraiseeType = var.AppraiseeTypeNavigation.AppraiseeType,
                                GivenByName = var.GivenByNavigation.Name,
                                GivenBy = var.GivenByNavigation.Guid,
                                GivenOn = var.AddedOn,
                                EmployeeFeedbackId = var.Guid,
                                AppraisalMode = var.AppraisalMode
                            })
                            .ToList(),
                        SelfAnswers = appraisal.AppraisalSelfAnswer.Where(var => var.IsActive
                            && appraisalMode == 1 ? var.AppraisalMode == appraisalMode :
                                appraisalMode == 2 ? var.AppraisalMode != 3 : var.AppraisalMode != 2)
                            .Select(var => new EmployeeAppraisalSelfAnswerDto
                            {
                                Description = var.Answer,
                                Title = var.Title,
                                Weightage = var.Percentage,
                                IsActive = true,
                                // ManagementWeightage = var.ManagementWeightage,
                                // SelfWeightage = var.SelfWeightage,
                                ManagementWeightage = appraisalMode == 1 ? 0 : appraisalMode == 2 ? (var.ManagementVariableWeightage != null ? var.ManagementVariableWeightage.Value : 0) : var.ManagementWeightage,
                                SelfWeightage = appraisalMode == 1 ? 0 : appraisalMode == 2 ? (var.SelfVariableWeightage != null ? var.SelfVariableWeightage.Value : 0) : var.SelfWeightage,
                                SelfAppraisalAnswerId = var.Guid,
                                //L2Weightage = var.L2Weightage ?? 0,
                                L2Weightage = appraisalMode == 1 ? 0 : appraisalMode == 2 ? (var.L2VariableWeightage != null ? var.L2VariableWeightage.Value : 0) : (var.L2Weightage != null ? var.L2Weightage.Value : 0),
                                AppraisalMode = var.AppraisalMode
                            })
                            .ToList(),
                        BusinessNeeds = appraisal.AppraisalBusinessNeed.Where(var => var.IsActive && var.AppraisalMode == appraisalMode)
                            .Select(var => new EmployeeAppraisalBusinessNeedAnswerDto
                            {
                                Description = var.Answer,
                                Title = var.Title,
                                Weightage = var.Percentage,
                                IsActive = true,
                                ManagementWeightage = var.ManagementWeightage,
                                SelfWeightage = var.SelfWeightage,
                                BusinessNeedAnswerId = var.Guid,
                                L2Weightage = var.L2Weightage ?? 0,
                                AppraisalMode = var.AppraisalMode
                            })
                            .ToList(),
                        Trainings = appraisal.AppraisalTraining.Where(var => var.IsActive)
                            .Select(var => new AppraisalTrainingDto
                            {
                                TrainingId = var.Training.Guid,
                                AddedBy = var.AddedByNavigation.Guid,
                                IsSelf = var.AddedBy == appraisal.EmployeeId
                            })
                            .ToList()
                    };

                    var answers = new List<EmployeeAppraisalQuestionAnswerDto>();
                    if (appraisal.AppraisalAnswer != null && appraisal.AppraisalAnswer.Any())
                    {
                        answers =
                            appraisal.AppraisalAnswer.Where(var => var.IsActive)
                                .Select(var => new EmployeeAppraisalQuestionAnswerDto
                                {
                                    Description = var.AppraisalQuestion.Question.Description,
                                    Weightage = var.AppraisalQuestion.Percentage,
                                    Title = var.AppraisalQuestion.Question.Question,
                                    ManagementWeightage = appraisalMode == 1 ? 0 : appraisalMode == 2 ? (var.ManagementVariableWeightage != null ? var.ManagementVariableWeightage.Value : 0) : var.ManagementWeightage,
                                    SelfWeightage = appraisalMode == 1 ? 0 : appraisalMode == 2 ? (var.SelfVariableWeightage != null ? var.SelfVariableWeightage.Value : 0) : var.SelfWeightage,
                                    EmployeeAnswerId = var.Guid,
                                    Answer = var.Answer,
                                    EmployeeQuestionId = var.AppraisalQuestion.Guid,
                                    L2Weightage = appraisalMode == 1 ? 0 : appraisalMode == 2 ? (var.L2VariableWeightage != null ? var.L2VariableWeightage.Value : 0) : var.L2Weightage
                                })
                                .ToList();
                    }
                    else
                    {
                        answers =
                            appraisal.Appraisal.AppraisalQuestion.Where(var => var.IsActive)
                                .Select(var => new EmployeeAppraisalQuestionAnswerDto
                                {
                                    Description = var.Question.Description,
                                    Weightage = var.Percentage,
                                    Title = var.Question.Question,
                                    ManagementWeightage = 0,
                                    SelfWeightage = 0,
                                    L2Weightage = 0,
                                    EmployeeAnswerId = null,
                                    Answer = string.Empty,
                                    EmployeeQuestionId = var.Guid
                                })
                                .ToList();
                    }

                    appraisalDto.Questions = answers;
                    appraisals.Add(appraisalDto);
                }

                var objectiveDocuments = employee.EmployeeDocumentEmployee.Where(var =>
                    var.IsActive && var.DocumentType.DocumentType.Equals("Appraisal Objectives"))
                    .Select(var => new EmployeeDocumentDto
                    {
                        IsActive = true,
                        Name = var.Name,
                        EmployeeDocumentId = var.Guid,
                        FileLocation = var.FileLocation,
                        FileUrl = var.FileUrl
                    })
                    .ToList();

                return new GetEmployeeAppraisalResponse
                {
                    IsSuccess = true,
                    Appraisals = appraisals,
                    IsAllowed = true,
                    AppraisalObjectiveDocuments = objectiveDocuments
                };
            }

            throw new Exception("Employee not found.");
        }

        public GetEmployeeAppraisalQuestionsResponse SaveRecommendedFitmentOrPromotion(SaveEmployeeAnswersRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.AppraisalEmployeeEmployee)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {
                if (!string.IsNullOrWhiteSpace(request.EmployeeAppraisalId))
                {
                    var employeeAppraisal = employee.AppraisalEmployeeEmployee.FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(request.EmployeeAppraisalId));

                    if (employeeAppraisal != null)
                    {
                        employeeAppraisal.IsRecommendedForFigment = request.IsFitmentRecommended;
                        employeeAppraisal.IsRecommendedForPromotion = request.IsPromotionRecommended;

                        Save();
                        return new GetEmployeeAppraisalQuestionsResponse
                        {
                            IsSuccess = true,
                        };
                    }

                    throw new Exception("Employee appraisal not found.");
                }

                throw new Exception("Employee appraisal cannot be null.");
            }

            throw new Exception("Employee not found.");
        }


        public GetEmployeeAppraisalQuestionsResponse SaveAppraisalAnswers(SaveEmployeeAnswersRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .ThenInclude(var => var.AppraisalQuestion)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalSelfAnswer)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalBusinessNeed)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalAnswer)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {
                if (!string.IsNullOrWhiteSpace(request.EmployeeAppraisalId))
                {
                    var employeeAppraisal = employee.AppraisalEmployeeEmployee.FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(request.EmployeeAppraisalId));
                    var appraisalMode = employeeAppraisal.Appraisal.Mode;

                    if (employeeAppraisal != null)
                    {
                        if (request.SelfAnswers != null)
                        {
                            foreach (var self in request.SelfAnswers)
                            {
                                if (string.IsNullOrWhiteSpace(self.SelfAppraisalAnswerId) && request.IsSelf)
                                {
                                    var newAnswer = new AppraisalSelfAnswer
                                    {
                                        AddedBy = request.UserIdNum,
                                        AddedOn = DateTime.UtcNow,
                                        CompanyId = request.CompanyIdNum,
                                        IsActive = true,
                                        Guid = CustomGuid.NewGuid(),
                                        Answer = self.Description,
                                        Percentage = self.Weightage,
                                        Title = self.Title,
                                        //SelfWeightage = self.SelfWeightage,
                                        AppraisalMode = self.AppraisalMode
                                    };
                                    if (employeeAppraisal.Appraisal.Mode == 2)
                                    {
                                        newAnswer.SelfWeightage = self.SelfWeightage;
                                    }
                                    else
                                    {
                                        newAnswer.SelfVariableWeightage = self.Weightage;
                                    }
                                    employeeAppraisal.AppraisalSelfAnswer.Add(newAnswer);
                                }
                                else
                                {
                                    var addedAnswer = employeeAppraisal.AppraisalSelfAnswer
                                        .FirstOrDefault(var =>
                                            var.IsActive && var.Guid.Equals(self.SelfAppraisalAnswerId));
                                    if (addedAnswer != null)
                                    {
                                        if (!self.IsActive)
                                        {
                                            addedAnswer.IsActive = false;
                                            addedAnswer.UpdatedBy = request.UserIdNum;
                                            addedAnswer.UpdatedOn = DateTime.UtcNow;
                                        }
                                        else
                                        {
                                            if (request.IsSelf)
                                            {
                                                addedAnswer.Answer = self.Description;
                                                addedAnswer.Percentage = self.Weightage;
                                                addedAnswer.Title = self.Title;
                                                //addedAnswer.SelfWeightage = self.SelfWeightage;
                                                if (employeeAppraisal.Appraisal.Mode == 2)
                                                {
                                                    addedAnswer.SelfVariableWeightage = self.SelfWeightage;
                                                }
                                                else
                                                {
                                                    addedAnswer.SelfWeightage = self.SelfWeightage;
                                                }
                                                addedAnswer.UpdatedBy = request.UserIdNum;
                                                addedAnswer.UpdatedOn = DateTime.UtcNow;
                                            }
                                            else
                                            {
                                                if (request.UserIdNum == employee.EmployeeCompanyEmployee
                                                        .FirstOrDefault()?.ReportingToId)
                                                {
                                                    //addedAnswer.ManagementWeightage = self.ManagementWeightage;
                                                    if (employeeAppraisal.Appraisal.Mode == 2)
                                                    {
                                                        addedAnswer.ManagementVariableWeightage = self.ManagementWeightage;
                                                    }
                                                    else
                                                    {
                                                        addedAnswer.ManagementWeightage = self.ManagementWeightage;
                                                    }
                                                }
                                                else
                                                {
                                                    //addedAnswer.L2Weightage = self.L2Weightage;
                                                    if (employeeAppraisal.Appraisal.Mode == 2)
                                                    {
                                                        addedAnswer.L2VariableWeightage = self.L2Weightage;
                                                    }
                                                    else
                                                    {
                                                        addedAnswer.L2Weightage = self.L2Weightage;
                                                    }
                                                }
                                                addedAnswer.UpdatedBy = request.UserIdNum;
                                                addedAnswer.UpdatedOn = DateTime.UtcNow;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Employee appraisal answer not found.");
                                    }
                                }
                            }
                        }

                        if (request.BusinessNeeds != null)
                        {
                            foreach (var self in request.BusinessNeeds)
                            {
                                if (string.IsNullOrWhiteSpace(self.BusinessNeedAnswerId) && request.IsSelf)
                                {
                                    var newAnswer = new AppraisalBusinessNeed
                                    {
                                        AddedBy = request.UserIdNum,
                                        AddedOn = DateTime.UtcNow,
                                        CompanyId = request.CompanyIdNum,
                                        IsActive = true,
                                        Guid = CustomGuid.NewGuid(),
                                        Answer = self.Description,
                                        Percentage = self.Weightage,
                                        Title = self.Title,
                                        SelfWeightage = self.SelfWeightage,
                                        AppraisalMode = appraisalMode
                                    };
                                    employeeAppraisal.AppraisalBusinessNeed.Add(newAnswer);
                                }
                                else
                                {
                                    var addedAnswer = employeeAppraisal.AppraisalBusinessNeed
                                        .FirstOrDefault(var =>
                                            var.IsActive && var.Guid.Equals(self.BusinessNeedAnswerId));
                                    if (addedAnswer != null)
                                    {
                                        if (!self.IsActive)
                                        {
                                            addedAnswer.IsActive = false;
                                            addedAnswer.UpdatedBy = request.UserIdNum;
                                            addedAnswer.UpdatedOn = DateTime.UtcNow;
                                        }
                                        else
                                        {
                                            if (request.IsSelf)
                                            {
                                                addedAnswer.Answer = self.Description;
                                                addedAnswer.Percentage = self.Weightage;
                                                addedAnswer.Title = self.Title;
                                                addedAnswer.SelfWeightage = self.SelfWeightage;
                                                addedAnswer.UpdatedBy = request.UserIdNum;
                                                addedAnswer.UpdatedOn = DateTime.UtcNow;
                                            }
                                            else
                                            {
                                                if (request.UserIdNum == employee.EmployeeCompanyEmployee
                                                        .FirstOrDefault()?.ReportingToId)
                                                {
                                                    addedAnswer.ManagementWeightage = self.ManagementWeightage;
                                                }
                                                else
                                                {
                                                    addedAnswer.L2Weightage = self.L2Weightage;
                                                }
                                                addedAnswer.UpdatedBy = request.UserIdNum;
                                                addedAnswer.UpdatedOn = DateTime.UtcNow;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Employee appraisal answer not found.");
                                    }
                                }
                            }
                        }

                        if (request.Questions != null)
                        {
                            foreach (var question in request.Questions)
                            {
                                var addedQuestion = employeeAppraisal.Appraisal.AppraisalQuestion.FirstOrDefault(var =>
                                    var.IsActive && var.Guid.Equals(question.EmployeeQuestionId));
                                if (addedQuestion != null)
                                {
                                    if (string.IsNullOrWhiteSpace(question.EmployeeAnswerId) && request.IsSelf)
                                    {
                                        var newAnswer = new AppraisalAnswer
                                        {
                                            AddedBy = request.UserIdNum,
                                            AddedOn = DateTime.UtcNow,
                                            CompanyId = request.CompanyIdNum,
                                            IsActive = true,
                                            Guid = CustomGuid.NewGuid(),
                                            Answer = question.Answer,
                                            AppraisalQuestion = addedQuestion
                                        };
                                        if (employeeAppraisal.Appraisal.Mode == 2)
                                        {
                                            newAnswer.SelfVariableWeightage = question.SelfWeightage;
                                        }
                                        else if (employeeAppraisal.Appraisal.Mode == 3)
                                        {
                                            newAnswer.SelfWeightage = question.SelfWeightage;
                                        }
                                        employeeAppraisal.AppraisalAnswer.Add(newAnswer);
                                    }
                                    else
                                    {
                                        var addedAnswer = employeeAppraisal.AppraisalAnswer
                                            .FirstOrDefault(var =>
                                                var.IsActive && var.Guid.Equals(question.EmployeeAnswerId));
                                        if (addedAnswer != null)
                                        {
                                            if (request.IsSelf)
                                            {
                                                addedAnswer.Answer = question.Answer;
                                                addedAnswer.UpdatedBy = request.UserIdNum;
                                                addedAnswer.UpdatedOn = DateTime.UtcNow;

                                                if (employeeAppraisal.Appraisal.Mode == 2)
                                                {
                                                    addedAnswer.SelfVariableWeightage = question.SelfWeightage;
                                                }
                                                else if (employeeAppraisal.Appraisal.Mode == 3)
                                                {
                                                    addedAnswer.SelfWeightage = question.SelfWeightage;
                                                }
                                            }
                                            else
                                            {
                                                if (request.UserIdNum == employee.EmployeeCompanyEmployee
                                                        .FirstOrDefault()?.ReportingToId)
                                                {
                                                    if (employeeAppraisal.Appraisal.Mode == 2)
                                                    {
                                                        addedAnswer.ManagementVariableWeightage = question.ManagementWeightage;
                                                    }
                                                    else if (employeeAppraisal.Appraisal.Mode == 3)
                                                    {
                                                        addedAnswer.ManagementWeightage = question.ManagementWeightage;
                                                    }

                                                    var isL2Present = GetAll()
                                                     .Include(var => var.EmployeeCompanyEmployee)
                                                     .FirstOrDefault(var => var.Id == request.UserIdNum)?
                                                     .EmployeeCompanyEmployee
                                                     .FirstOrDefault()?
                                                     .ReportingToId != null;

                                                    if (!isL2Present)
                                                    {
                                                        if (employeeAppraisal.Appraisal.Mode == 2)
                                                        {
                                                            addedAnswer.L2VariableWeightage = question.L2Weightage;
                                                        }
                                                        else if (employeeAppraisal.Appraisal.Mode == 3)
                                                        {
                                                            addedAnswer.L2Weightage = question.L2Weightage;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (employeeAppraisal.Appraisal.Mode == 2)
                                                    {
                                                        addedAnswer.L2VariableWeightage = question.L2Weightage;
                                                    }
                                                    else if (employeeAppraisal.Appraisal.Mode == 3)
                                                    {
                                                        addedAnswer.L2Weightage = question.L2Weightage;
                                                    }
                                                }
                                                addedAnswer.UpdatedBy = request.UserIdNum;
                                                addedAnswer.UpdatedOn = DateTime.UtcNow;
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Employee appraisal question answer not found.");
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Appraisal question not found.");
                                }
                            }
                        }

                        _eventLogRepo.AddAuditLog(new AuditInfo
                        {
                            AuditText = string.Format(EventLogActions.SaveAppraisalAnswers.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                            PerformedBy = request.UserIdNum,
                            ActionId = EventLogActions.SaveAppraisalAnswers.ActionId,
                            Data = JsonConvert.SerializeObject(new
                            {
                                userId = request.UserId,
                                userName = request.UserName
                            })
                        });

                        Save();
                        return new GetEmployeeAppraisalQuestionsResponse
                        {
                            IsSuccess = true,
                        };
                    }

                    throw new Exception("Employee appraisal not found.");
                }

                throw new Exception("Employee appraisal cannot be null.");
            }

            throw new Exception("Employee not found.");
        }

        public BaseResponse SaveAppraisalTrainings(SaveAppraisalTrainingsRequest request)
        {

            var employee = GetAll()
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalTraining)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {
                if (!string.IsNullOrWhiteSpace(request.EmployeeAppraisalId))
                {
                    var employeeAppraisal = employee.AppraisalEmployeeEmployee.FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(request.EmployeeAppraisalId));

                    if (employeeAppraisal != null)
                    {
                        var trainings = dbContext.SettingsTraining.Where(var =>
                            var.IsActive && request.Trainings.Contains(var.Guid));

                        var addedTrainings =
                            employeeAppraisal.AppraisalTraining.Where(var =>
                                var.IsActive && var.AddedBy == request.UserIdNum);

                        dbContext.AppraisalTraining.RemoveRange(addedTrainings);

                        var newTrainings = trainings.Select(var => new AppraisalTraining
                        {
                            AppraisalEmployee = employeeAppraisal,
                            AddedBy = request.UserIdNum,
                            AppraisalId = employeeAppraisal.AppraisalId,
                            CompanyId = employeeAppraisal.CompanyId,
                            IsActive = true,
                            TrainingId = var.Id
                        }).ToList();

                        employeeAppraisal.TrainingComments = request.Comments;

                        foreach (var training in newTrainings)
                        {
                            employeeAppraisal.AppraisalTraining.Add(training);
                        }

                        _eventLogRepo.AddAuditLog(new AuditInfo
                        {
                            AuditText = string.Format(EventLogActions.SaveAppraisalTrainings.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                            PerformedBy = request.UserIdNum,
                            ActionId = EventLogActions.SaveAppraisalTrainings.ActionId,
                            Data = JsonConvert.SerializeObject(new
                            {
                                userId = request.UserId,
                                userName = request.UserName
                            })
                        });

                        Save();

                        return new BaseResponse
                        {
                            IsSuccess = true
                        };
                    }

                    throw new Exception("Employee appraisal not found.");
                }

                throw new Exception("Employee appraisal cannot be null.");
            }

            throw new Exception("Employee not found.");

        }

        public GetEmployeeAppraisalQuestionsResponse SubmitObjective(EmployeeAppraisalActionRequest request, ApplicationSettings settings)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalSelfAnswer)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {
                var employeeAppraisal = employee.AppraisalEmployeeEmployee.FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.EmployeeAppraisalId));
                var appraisalMode = employeeAppraisal.Appraisal.Mode;
                var modeDescription = appraisalMode == 1 ? "Objective" : appraisalMode == 2 ? "Variable Bonus" : "Appraisal";
                var l2ManagerName = "";
                var mailTo = "";

                if (employeeAppraisal != null)
                {
                    var managerName = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingTo?.Name;
                    if (request.UserRole == "HR" && !request.IsSelf)
                    {
                        if (employeeAppraisal.RmObjectiveSubmittedOn != null && employeeAppraisal.L2ObjectiveSubmittedOn != null)
                        {
                            employeeAppraisal.UpdatedBy = request.UserIdNum;
                            employeeAppraisal.UpdatedOn = DateTime.UtcNow;
                            employeeAppraisal.HrObjectiveSubmittedOn = DateTime.UtcNow;
                            mailTo = "HR";
                        }
                        else
                        {
                            var manager = employee.EmployeeCompanyEmployee
                                   .FirstOrDefault();
                            var managerId = manager?.ReportingToId;
                            if (managerId != null && request.UserIdNum == managerId)
                            {
                                employeeAppraisal.RmObjectiveSubmittedOn = DateTime.UtcNow;

                                var l2Employee = GetAll()
                                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                                    .FirstOrDefault(var => var.Id == managerId)?
                                    .EmployeeCompanyEmployee
                                    .FirstOrDefault();

                                var isL2Present = l2Employee?.ReportingToId != null;

                                if (!isL2Present)
                                {
                                    employeeAppraisal.L2ObjectiveSubmittedOn = DateTime.UtcNow;

                                    dbContext.Notification.Add(new Notification
                                    {
                                        Guid = CustomGuid.NewGuid(),
                                        ActionId = NotificationTemplates.L2FilledObjective,
                                        NotificationTime = DateTime.UtcNow,
                                        IsActive = true,
                                        IsRead = false,
                                        NotificationTo = employee.Id,
                                        NotificationData = JsonConvert.SerializeObject(
                                        new
                                        {
                                            userName = request.UserName
                                        })
                                    });

                                    mailTo = "L2";
                                    l2ManagerName = managerName;
                                }
                                else
                                {
                                    l2ManagerName = l2Employee.ReportingTo.Name;
                                    mailTo = "RM";
                                }
                            }
                            else
                            {
                                employeeAppraisal.L2ObjectiveSubmittedOn = DateTime.UtcNow;

                                dbContext.Notification.Add(new Notification
                                {
                                    Guid = CustomGuid.NewGuid(),
                                    ActionId = NotificationTemplates.L2FilledObjective,
                                    NotificationTime = DateTime.UtcNow,
                                    IsActive = true,
                                    IsRead = false,
                                    NotificationTo = employee.Id,
                                    NotificationData = JsonConvert.SerializeObject(
                                    new
                                    {
                                        userName = request.UserName
                                    })
                                });

                                mailTo = "L2";
                                l2ManagerName = request.UserName;

                            }

                            employeeAppraisal.UpdatedBy = request.UserIdNum;
                            employeeAppraisal.UpdatedOn = DateTime.UtcNow;
                        }
                    }
                    else if (request.IsSelf)
                    {
                        employeeAppraisal.SelfObjectiveSubmittedOn = DateTime.UtcNow;
                        employeeAppraisal.UpdatedBy = request.UserIdNum;
                        employeeAppraisal.UpdatedOn = DateTime.UtcNow;
                        employeeAppraisal.RmObjectiveSubmittedOn = null;
                        employeeAppraisal.L2ObjectiveSubmittedOn = null;

                        if (employee.EmployeeCompanyEmployee
                                .FirstOrDefault()?.ReportingToId == null)
                        {
                            foreach (var self in employeeAppraisal.AppraisalSelfAnswer)
                            {
                                self.ManagementWeightage = self.SelfWeightage;
                                self.L2Weightage = self.SelfWeightage;
                            }

                            employeeAppraisal.RmObjectiveSubmittedOn = DateTime.UtcNow;
                            employeeAppraisal.L2ObjectiveSubmittedOn = DateTime.UtcNow;

                            dbContext.Notification.Add(new Notification
                            {
                                Guid = CustomGuid.NewGuid(),
                                ActionId = NotificationTemplates.HrFilledObjective,
                                NotificationTime = DateTime.UtcNow,
                                IsActive = true,
                                IsRead = false,
                                NotificationTo = employee.Id,
                                NotificationData = null
                            });

                        }
                        else
                        {
                            dbContext.Notification.Add(new Notification
                            {
                                Guid = CustomGuid.NewGuid(),
                                ActionId = NotificationTemplates.SelfFilledObjective,
                                NotificationTime = DateTime.UtcNow,
                                IsActive = true,
                                IsRead = false,
                                NotificationTo = employee.Id,
                                NotificationData = null
                            });
                        }

                        mailTo = "SELF";

                    }
                    else
                    {
                        var manager = employee.EmployeeCompanyEmployee
                            .FirstOrDefault();
                        var managerId = manager?.ReportingToId;
                        if (managerId != null && request.UserIdNum == managerId)
                        {
                            employeeAppraisal.RmObjectiveSubmittedOn = DateTime.UtcNow;

                            var l2Employee = GetAll()
                                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                                .FirstOrDefault(var => var.Id == managerId)?
                                .EmployeeCompanyEmployee
                                .FirstOrDefault();

                            var isL2Present = l2Employee?.ReportingToId != null;

                            if (!isL2Present)
                            {
                                employeeAppraisal.L2ObjectiveSubmittedOn = DateTime.UtcNow;

                                dbContext.Notification.Add(new Notification
                                {
                                    Guid = CustomGuid.NewGuid(),
                                    ActionId = NotificationTemplates.L2FilledObjective,
                                    NotificationTime = DateTime.UtcNow,
                                    IsActive = true,
                                    IsRead = false,
                                    NotificationTo = employee.Id,
                                    NotificationData = JsonConvert.SerializeObject(
                                    new
                                    {
                                        userName = request.UserName
                                    })
                                });

                                mailTo = "L2";
                                l2ManagerName = managerName;
                            }
                            else
                            {
                                l2ManagerName = l2Employee.ReportingTo.Name;
                                mailTo = "RM";
                            }
                        }
                        else
                        {
                            employeeAppraisal.L2ObjectiveSubmittedOn = DateTime.UtcNow;

                            dbContext.Notification.Add(new Notification
                            {
                                Guid = CustomGuid.NewGuid(),
                                ActionId = NotificationTemplates.L2FilledObjective,
                                NotificationTime = DateTime.UtcNow,
                                IsActive = true,
                                IsRead = false,
                                NotificationTo = employee.Id,
                                NotificationData = JsonConvert.SerializeObject(
                                new
                                {
                                    userName = request.UserName
                                })
                            });

                            mailTo = "L2";
                            l2ManagerName = request.UserName;

                        }

                        employeeAppraisal.UpdatedBy = request.UserIdNum;
                        employeeAppraisal.UpdatedOn = DateTime.UtcNow;
                    }

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.SubmitObjective.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.SubmitObjective.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    if (mailTo == "SELF")
                    {
                        EmailSender.SendSelfAppraisalFilledEmail(employee.Name, employee.EmailId, managerName, settings, modeDescription);
                    }
                    else if (mailTo == "RM")
                    {
                        EmailSender.SendRmAppraisalFilledEmail(employee.Name, employee.EmailId, managerName, settings, modeDescription, l2ManagerName);
                    }
                    else if (mailTo == "L2")
                    {
                        EmailSender.SendL2AppraisalFilledEmail(employee.Name, employee.EmailId, l2ManagerName, settings, modeDescription);
                    }
                    else if (mailTo == "HR")
                    {
                        EmailSender.SendHrObjectiveFilledEmail(employee.Name, employee.EmailId, settings);
                    }

                    return new GetEmployeeAppraisalQuestionsResponse
                    {
                        IsSuccess = true
                    };
                }

                throw new Exception("Employee appraisal not found.");
            }

            throw new Exception("Employee not found.");
        }


        public GetEmployeeAppraisalQuestionsResponse SubmitAppraisalAnswers(EmployeeAppraisalActionRequest request, ApplicationSettings settings)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .ThenInclude(var => var.AppraisalQuestion)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalSelfAnswer)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalBusinessNeed)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalAnswer)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {
                var employeeAppraisal = employee.AppraisalEmployeeEmployee.FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.EmployeeAppraisalId));
                var appraisalMode = employeeAppraisal.Appraisal.Mode;
                var modeDescription = appraisalMode == 1 ? "Objective" : appraisalMode == 2 ? "Variable Bonus" : "Appraisal";
                var mailTo = "";
                var l2ManagerName = "";
                if (employeeAppraisal != null)
                {
                    var managerName = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingTo?.Name;
                    if (request.IsSelf)
                    {
                        if (appraisalMode == 2)
                        {
                            employeeAppraisal.SelfVariableSubmittedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            employeeAppraisal.SelfSubmittedOn = DateTime.UtcNow;
                        }
                        employeeAppraisal.UpdatedBy = request.UserIdNum;
                        employeeAppraisal.UpdatedOn = DateTime.UtcNow;

                        if (employee.EmployeeCompanyEmployee
                                .FirstOrDefault()?.ReportingToId == null)
                        {
                            foreach (var self in employeeAppraisal.AppraisalSelfAnswer)
                            {
                                self.ManagementWeightage = self.SelfWeightage;
                                self.L2Weightage = self.SelfWeightage;
                            }
                            foreach (var self in employeeAppraisal.AppraisalBusinessNeed)
                            {
                                self.ManagementWeightage = self.SelfWeightage;
                                self.L2Weightage = self.SelfWeightage;

                            }
                            foreach (var self in employeeAppraisal.AppraisalAnswer)
                            {
                                self.ManagementWeightage = self.SelfWeightage;
                                self.L2Weightage = self.SelfWeightage;
                            }

                            if (appraisalMode == 2)
                            {
                                employeeAppraisal.RmVariableSubmittedOn = DateTime.UtcNow;
                                employeeAppraisal.L2VariableSubmittedOn = DateTime.UtcNow;

                                employeeAppraisal.InternalVariableMgmt = employeeAppraisal.InternalVariableSelf;
                                employeeAppraisal.InternalVariableL2 = employeeAppraisal.InternalVariableSelf;
                            }
                            else
                            {
                                employeeAppraisal.RmSubmittedOn = DateTime.UtcNow;
                                employeeAppraisal.L2SubmittedOn = DateTime.UtcNow;

                                employeeAppraisal.InternalMgmt = employeeAppraisal.InternalSelf;
                                employeeAppraisal.InternalL2 = employeeAppraisal.InternalSelf;
                            }

                            var rating = dbContext.SettingsAppraisalRatings.FirstOrDefault(var =>
                                var.IsActive && var.Guid.Equals(request.RatingId));

                            if (rating != null)
                            {
                                if (appraisalMode == 2)
                                {
                                    employeeAppraisal.VariableBonusRatingNavigation = rating;
                                }
                                else
                                {
                                    employeeAppraisal.RatingNavigation = rating;
                                }
                            }
                        }

                        mailTo = "SELF";

                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.SelfFilledAppraisal,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = employee.Id,
                            NotificationData = null
                        });

                    }
                    else
                    {
                        var rating = dbContext.SettingsAppraisalRatings.FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.RatingId));
                        if (rating != null)
                        {
                            if (appraisalMode == 2)
                            {
                                employeeAppraisal.VariableBonusRatingNavigation = rating;
                            }
                            else
                            {
                                employeeAppraisal.RatingNavigation = rating;
                            }
                        }

                        var manager = employee.EmployeeCompanyEmployee
                            .FirstOrDefault();
                        var managerId = manager?.ReportingToId;

                        if (managerId != null && request.UserIdNum == managerId)
                        {
                            if (appraisalMode == 2)
                            {
                                employeeAppraisal.RmVariableSubmittedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                employeeAppraisal.RmSubmittedOn = DateTime.UtcNow;
                            }

                            var l2Manager = GetAll()
                                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                                .FirstOrDefault(var => var.Id == managerId)?
                                .EmployeeCompanyEmployee
                                .FirstOrDefault();
                            var isL2Present = l2Manager?.ReportingToId != null;

                            if (!isL2Present)
                            {
                                foreach (var self in employeeAppraisal.AppraisalSelfAnswer)
                                {
                                    self.L2Weightage = self.ManagementWeightage;
                                }
                                foreach (var self in employeeAppraisal.AppraisalBusinessNeed)
                                {
                                    self.L2Weightage = self.ManagementWeightage;

                                }
                                foreach (var self in employeeAppraisal.AppraisalAnswer)
                                {
                                    self.L2Weightage = self.ManagementWeightage;
                                }


                                if (appraisalMode == 2)
                                {
                                    employeeAppraisal.InternalVariableL2 = employeeAppraisal.InternalVariableMgmt;
                                    employeeAppraisal.L2VariableSubmittedOn = DateTime.UtcNow;
                                }
                                else
                                {
                                    employeeAppraisal.InternalL2 = employeeAppraisal.InternalMgmt;
                                    employeeAppraisal.L2SubmittedOn = DateTime.UtcNow;
                                }

                                mailTo = "L2";
                                l2ManagerName = manager?.ReportingTo.Name;
                            }
                            else
                            {
                                mailTo = "RM";
                                l2ManagerName = l2Manager.ReportingTo.Name;
                            }
                        }
                        else
                        {
                            if (appraisalMode == 2)
                            {
                                employeeAppraisal.L2VariableSubmittedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                employeeAppraisal.L2SubmittedOn = DateTime.UtcNow;
                            }

                            mailTo = "L2";
                            l2ManagerName = request.UserName;
                        }

                        employeeAppraisal.UpdatedBy = request.UserIdNum;
                        employeeAppraisal.UpdatedOn = DateTime.UtcNow;


                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.RmFilledAppraisal,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = employee.Id,
                            NotificationData = JsonConvert.SerializeObject(
                                new
                                {
                                    userName = request.UserName
                                })
                        });

                    }

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.SubmitAppraisalAnswers.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.SubmitAppraisalAnswers.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();


                    if (mailTo == "SELF")
                    {
                        EmailSender.SendSelfAppraisalFilledEmail(employee.Name, employee.EmailId, managerName, settings, modeDescription);
                    }
                    else if (mailTo == "RM")
                    {
                        EmailSender.SendRmAppraisalFilledEmail(employee.Name, employee.EmailId, managerName, settings, modeDescription, l2ManagerName);
                    }
                    else if (mailTo == "L2")
                    {
                        EmailSender.SendL2AppraisalFilledEmail(employee.Name, employee.EmailId, l2ManagerName, settings, modeDescription);
                    }
                    else if (mailTo == "HR")
                    {
                        EmailSender.SendHrAppraisalFilledEmail(employee.Name, employee.EmailId, settings, modeDescription);
                    }

                    return new GetEmployeeAppraisalQuestionsResponse
                    {
                        IsSuccess = true
                    };
                }

                throw new Exception("Employee appraisal not found.");
            }

            throw new Exception("Employee not found.");
        }


        public GetEmployeeAppraisalFeedbackResponse UpdateFeedback(EmployeeAppraisalFeedbackRequest request)
        {
            var employee = GetAll()
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalFeedback)
                .ThenInclude(var => var.AppraiseeTypeNavigation)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.AppraisalFeedback)
                .ThenInclude(var => var.GivenByNavigation)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {
                var employeeAppraisal = employee.AppraisalEmployeeEmployee.FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.EmployeeAppraisalId));
                if (employeeAppraisal != null)
                {
                    var appraisee = dbContext.SettingsAppraiseeType.FirstOrDefault(var =>
                        var.IsActive && var.AppraiseeType.Equals(request.AppraiseeType));

                    if (appraisee != null)
                    {
                        if (string.IsNullOrWhiteSpace(request.EmployeeFeedbackId))
                        {
                            var newFeedback = new AppraisalFeedback
                            {
                                CompanyId = request.CompanyIdNum,
                                Feedback = request.Feedback,
                                Guid = CustomGuid.NewGuid(),
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.UtcNow,
                                AppraiseeTypeNavigation = appraisee,
                                IsActive = true,
                                GivenBy = request.UserIdNum,
                                AppraisalMode = request.AppraisalMode
                            };

                            employeeAppraisal.AppraisalFeedback.Add(newFeedback);
                        }
                        else
                        {
                            var addedFeedback = employeeAppraisal.AppraisalFeedback.FirstOrDefault(var =>
                                var.IsActive && var.Guid.Equals(request.EmployeeFeedbackId));
                            if (addedFeedback != null)
                            {
                                addedFeedback.Feedback = request.Feedback;
                                addedFeedback.UpdatedBy = request.UserIdNum;
                                addedFeedback.UpdatedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                throw new Exception("Employee feedback not found.");
                            }
                        }

                        _eventLogRepo.AddAuditLog(new AuditInfo
                        {
                            AuditText = string.Format(EventLogActions.UpdateAppraisalFeedback.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                            PerformedBy = request.UserIdNum,
                            ActionId = EventLogActions.UpdateAppraisalFeedback.ActionId,
                            Data = JsonConvert.SerializeObject(new
                            {
                                userId = request.UserId,
                                userName = request.UserName
                            })
                        });

                        Save();

                        var feedbacks = employeeAppraisal.AppraisalFeedback.Where(var => var.IsActive)
                            .Select(var => new EmployeeAppraisalFeedbackDto
                            {
                                Feedback = var.Feedback,
                                AppraiseeType = var.AppraiseeTypeNavigation?.AppraiseeType,
                                GivenByName = var.GivenByNavigation?.Name,
                                GivenBy = var.GivenByNavigation?.Guid,
                                GivenOn = var.AddedOn,
                                EmployeeFeedbackId = var.Guid
                            })
                            .ToList();

                        return new GetEmployeeAppraisalFeedbackResponse
                        {
                            IsSuccess = true,
                            Feedbacks = feedbacks
                        };
                    }

                    throw new Exception("Employee appraisee not found.");
                }

                throw new Exception("Employee appraisal not found.");
            }

            throw new Exception("Employee not found.");
        }

        public GetEmployeeAppraisalDetailsResponse SaveAppraisalRating(SaveEmployeeAppraisalRatingRequest request, ApplicationSettings settings)
        {
            var employee = GetAll()
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {
                var employeeAppraisal = employee.AppraisalEmployeeEmployee.FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.EmployeeAppraisalId));
                var appraisalMode = employeeAppraisal.Appraisal.Mode;
                var modeDescription = appraisalMode == 3 ? "Appraisal" : "Variable Bonus";

                if (employeeAppraisal != null)
                {
                    var rating = dbContext.SettingsAppraisalRatings.FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(request.RatingId));

                    if (rating != null)
                    {
                        if (appraisalMode == 2)
                        {
                            employeeAppraisal.HrVariableSubmittedOn = DateTime.UtcNow;
                            employeeAppraisal.VariableBonusRatingNavigation = rating;
                        }
                        else
                        {
                            employeeAppraisal.HrSubmittedOn = DateTime.UtcNow;
                            employeeAppraisal.RatingNavigation = rating;
                            employeeAppraisal.ClosedOn = DateTime.UtcNow;
                        }


                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.HrFilledObjective,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = employee.Id,
                            NotificationData = JsonConvert.SerializeObject(
                                new
                                {
                                    userName = request.UserName
                                })
                        });

                        _eventLogRepo.AddAuditLog(new AuditInfo
                        {
                            AuditText = string.Format(EventLogActions.SaveAppraisalRating.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                            PerformedBy = request.UserIdNum,
                            ActionId = EventLogActions.SaveAppraisalRating.ActionId,
                            Data = JsonConvert.SerializeObject(new
                            {
                                userId = request.UserId,
                                userName = request.UserName
                            })
                        });

                        Save();

                        EmailSender.SendHrAppraisalFilledEmail(employee.Name, employee.EmailId, settings, modeDescription);

                        return new GetEmployeeAppraisalDetailsResponse
                        {
                            IsSuccess = true
                        };
                    }

                    throw new Exception("Employee appraisal rating not found.");

                    throw new Exception("Employee appraisal rating not found.");
                }

                throw new Exception("Employee appraisal not found.");
            }

            throw new Exception("Employee not found.");
        }

        public EmployeeReportingToResponse GetEmployeeAppraisalsAsManager(EmployeeActionRequest request)
        {
            var reportingEmployeesList = new List<long>();
            var allReportingEmployees = GetEmployeesReportingToForAppraisals(request.EmployeeId, reportingEmployeesList);

            var appraisalIds = dbContext.Appraisal.Where(var1 => var1.IsActive && var1.IsOpen && var1.StartDate.Date <= DateTime.Today
                                  && var1.EndDate >= DateTime.Today).Select(var => var.Id).ToList();

            var employees = (from employee in GetAll()
                             join empCompany in dbContext.EmployeeCompany on employee.Id equals empCompany.EmployeeId
                             join empAppraisal in dbContext.AppraisalEmployee on employee.Id equals empAppraisal.EmployeeId
                             join appraisal in dbContext.Appraisal on empAppraisal.AppraisalId equals appraisal.Id
                             join company in dbContext.EmployeeCompany on employee.Id equals company.EmployeeId
                             where employee.IsActive && empAppraisal.IsActive && empAppraisal.ClosedOn == null
                               && appraisalIds.Contains(appraisal.Id)
                               && allReportingEmployees.Contains((employee.Id))
                               && !employee.Guid.Equals(request.EmployeeId)
                             select new EmployeesReportingDto
                             {
                                 Code = empCompany.EmployeeCode,
                                 Department = empCompany.Department.Name,
                                 Grade = empCompany.Grade.Grade,
                                 Location = empCompany.Location.Name,
                                 Designation = empCompany.Designation.Name,
                                 Region = empCompany.Region.Name,
                                 Team = empCompany.Team.Name,
                                 EmployeeId = employee.Guid,
                                 EmailId = employee.EmailId,
                                 ManagerName = empCompany.ReportingToId != null ? empCompany.ReportingTo.Name : null,
                                 L2ManagerName = empCompany.ReportingToId != null
                                                   && empCompany.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != null
                                                   ? empCompany.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo.Name : null,
                                 Name = employee.Name,
                                 SelfFilledOn = empAppraisal.SelfSubmittedOn,
                                 ManagerFilledOn = empAppraisal.RmSubmittedOn,
                                 HrFilledOn = empAppraisal.HrSubmittedOn,
                                 L2FilledOn = empAppraisal.L2SubmittedOn,
                                 Rating = empAppraisal.Appraisal.Mode == 3
                                       ? empAppraisal.RatingNavigation.RatingTitle
                                       : empAppraisal.VariableBonusRatingNavigation.RatingTitle,
                                 AppraisalMode = appraisal.Mode,
                                 SelfObjectiveFilledOn = empAppraisal.SelfObjectiveSubmittedOn,
                                 ManagerObjectiveFilledOn = empAppraisal.RmObjectiveSubmittedOn,
                                 L2ObjectiveFilledOn = empAppraisal.L2ObjectiveSubmittedOn,
                                 HrObjectiveFilledOn = empAppraisal.HrObjectiveSubmittedOn,
                                 SelfVariableFilledOn = empAppraisal.SelfVariableSubmittedOn,
                                 ManagerVariableFilledOn = empAppraisal.RmVariableSubmittedOn,
                                 L2VariableFilledOn = empAppraisal.L2VariableSubmittedOn,
                                 HrVariableFilledOn = empAppraisal.HrVariableSubmittedOn,
                                 IsReportingToMe = empCompany.ReportingTo.Guid
                                   .Equals(request.EmployeeId),
                                 AppraisalName = appraisal.Title
                             }).ToList();

            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(1));
            var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

            var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0));
            var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


            return new EmployeeReportingToResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                MgAccess = mgAccess,
                EmpAccess = empAccess,
                Employees = employees
            };
        }

        private List<long> GetEmployeesReportingToForAppraisals(string guid, List<long> reportingList)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyReportingTo).ThenInclude(var => var.Employee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(guid));

            if (employee != null && !reportingList.Contains(employee.Id))
            {
                reportingList.Add(employee.Id);
                var employeesReporting = employee.EmployeeCompanyReportingTo;
                if (employeesReporting != null && employeesReporting.Any())
                {
                    foreach (var emp in employeesReporting)
                    {
                        GetEmployeesReportingToForAppraisals(emp.Employee.Guid, reportingList);
                    }
                }
            }

            return reportingList;
        }

        public BaseResponse SaveAppraisalInternalAnswers(SaveEmployeeInternalAnswersRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .Include(var => var.AppraisalEmployeeEmployee)
                .ThenInclude(var => var.Appraisal)
                .FirstOrDefault(var => var.Guid.Equals(request.EmployeeId) && var.IsActive);

            if (employee != null)
            {
                if (!string.IsNullOrWhiteSpace(request.EmployeeAppraisalId))
                {
                    var employeeAppraisal = employee.AppraisalEmployeeEmployee.FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(request.EmployeeAppraisalId));
                    var appraisalMode = employeeAppraisal.Appraisal.Mode;

                    if (employeeAppraisal != null)
                    {
                        if (request.IsSelf)
                        {
                            if (appraisalMode == 2)
                            {
                                employeeAppraisal.InternalVariableSelf = request.Weightage;
                            }
                            else
                            {
                                employeeAppraisal.InternalSelf = request.Weightage;
                            }
                            employeeAppraisal.UpdatedBy = request.UserIdNum;
                            employeeAppraisal.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            employeeAppraisal.UpdatedBy = request.UserIdNum;
                            employeeAppraisal.UpdatedOn = DateTime.UtcNow;
                            if (request.UserIdNum == employee.EmployeeCompanyEmployee
                                    .FirstOrDefault()?.ReportingToId)
                            {
                                if (appraisalMode == 2)
                                {
                                    employeeAppraisal.InternalVariableMgmt = request.Weightage;
                                }
                                else
                                {
                                    employeeAppraisal.InternalMgmt = request.Weightage;
                                }
                            }
                            else
                            {
                                if (appraisalMode == 2)
                                {
                                    employeeAppraisal.InternalVariableL2 = request.Weightage;
                                }
                                else
                                {
                                    employeeAppraisal.InternalL2 = request.Weightage;
                                }
                            }
                        }

                        _eventLogRepo.AddAuditLog(new AuditInfo
                        {
                            AuditText = string.Format(EventLogActions.SaveAppraisalInternalAnswers.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                            PerformedBy = request.UserIdNum,
                            ActionId = EventLogActions.SaveAppraisalInternalAnswers.ActionId,
                            Data = JsonConvert.SerializeObject(new
                            {
                                userId = request.UserId,
                                userName = request.UserName
                            })
                        });

                        Save();

                        return new BaseResponse
                        {
                            IsSuccess = true
                        };
                    }


                    throw new Exception("Employee appraisee not found.");
                }

                throw new Exception("Employee appraisal not found.");
            }

            throw new Exception("Employee not found.");
        }

        public EmployeeReportingToResponse GetAllAppraisalsPendingWithHr(EmployeeActionRequest request)
        {
            var appraisalIds = dbContext.Appraisal.Where(var1 => var1.IsActive && var1.IsOpen && var1.StartDate.Date <= DateTime.Today
                                  && var1.EndDate >= DateTime.Today).Select(var => var.Id).ToList();

            var employees = (from employee in GetAll()
                             join empCompany in dbContext.EmployeeCompany on employee.Id equals empCompany.EmployeeId
                             join empAppraisal in dbContext.AppraisalEmployee on employee.Id equals empAppraisal.EmployeeId
                             join appraisal in dbContext.Appraisal on empAppraisal.AppraisalId equals appraisal.Id
                             join company in dbContext.EmployeeCompany on employee.Id equals company.EmployeeId
                             where employee.IsActive && empAppraisal.IsActive && empAppraisal.ClosedOn == null
                               && appraisalIds.Contains(appraisal.Id)
                               && !employee.Guid.Equals(request.EmployeeId)
                             select new EmployeesReportingDto
                             {
                                 Code = empCompany.EmployeeCode,
                                 Department = empCompany.Department.Name,
                                 Grade = empCompany.Grade.Grade,
                                 Location = empCompany.Location.Name,
                                 Designation = empCompany.Designation.Name,
                                 Region = empCompany.Region.Name,
                                 Team = empCompany.Team.Name,
                                 EmployeeId = employee.Guid,
                                 EmailId = employee.EmailId,
                                 ManagerName = empCompany.ReportingToId != null ? empCompany.ReportingTo.Name : null,
                                 L2ManagerName = empCompany.ReportingToId != null
                                                   && empCompany.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != null
                                                   ? empCompany.ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo.Name : null,
                                 Name = employee.Name,
                                 SelfFilledOn = empAppraisal.SelfSubmittedOn,
                                 ManagerFilledOn = empAppraisal.RmSubmittedOn,
                                 HrFilledOn = empAppraisal.HrSubmittedOn,
                                 L2FilledOn = empAppraisal.L2SubmittedOn,
                                 Rating = empAppraisal.Appraisal.Mode == 3
                                       ? empAppraisal.RatingNavigation.RatingTitle
                                       : empAppraisal.VariableBonusRatingNavigation.RatingTitle,
                                 AppraisalMode = appraisal.Mode,
                                 SelfObjectiveFilledOn = empAppraisal.SelfObjectiveSubmittedOn,
                                 ManagerObjectiveFilledOn = empAppraisal.RmObjectiveSubmittedOn,
                                 L2ObjectiveFilledOn = empAppraisal.L2ObjectiveSubmittedOn,
                                 HrObjectiveFilledOn = empAppraisal.HrObjectiveSubmittedOn,
                                 SelfVariableFilledOn = empAppraisal.SelfVariableSubmittedOn,
                                 ManagerVariableFilledOn = empAppraisal.RmVariableSubmittedOn,
                                 L2VariableFilledOn = empAppraisal.L2VariableSubmittedOn,
                                 HrVariableFilledOn = empAppraisal.HrVariableSubmittedOn
                             }).ToList();

            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(1));
            var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

            var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0));
            var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

            return new EmployeeReportingToResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                MgAccess = mgAccess,
                EmpAccess = empAccess,
                Employees = employees
            };
        }

        public GetAnnouncementsResponse GetEmployeeAnnouncements(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .ThenInclude(var => var.Location)
                .ThenInclude(var => var.AnnouncementLocation)
                .ThenInclude(var => var.Announcement)
                .ThenInclude(var => var.Type)

                .Include(var => var.EmployeeCompanyEmployee)
                .ThenInclude(var => var.Location)
                .ThenInclude(var => var.AnnouncementLocation)
                .ThenInclude(var => var.Announcement)
                .ThenInclude(var => var.AnnouncementAttachment)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                if (employee.EmployeeCompanyEmployee.FirstOrDefault() == null || employee.EmployeeCompanyEmployee.FirstOrDefault().Location == null)
                {
                    return new GetAnnouncementsResponse
                    {
                        Announcements = new List<AnnouncementDto>(),
                        IsSuccess = true
                    };
                }

                var announcements = employee.EmployeeCompanyEmployee.FirstOrDefault()
                    .Location.AnnouncementLocation
                    .Where(var => var.Announcement.IsActive)
                    .Select((var => new AnnouncementDto
                    {
                        Content = var.Announcement.Content,
                        Date = var.Announcement.Date,
                        Title = var.Announcement.Title,
                        AnnouncementId = var.Announcement.Guid,
                        AnnouncementType = var.Announcement.Type.Type,
                        EndDate = var.Announcement.EndDate,
                        IsHidden = var.Announcement.IsHidden ?? false,
                        IsPublished = var.Announcement.IsPublished,
                        StartDate = var.Announcement.StartDate,
                        SubTitle = var.Announcement.SubTitle,
                        Location = var.Location.Name,
                        AttachmentCount = var.Announcement.AnnouncementAttachment.Count(var1 => var1.IsActive)
                    }))
                    .ToList();

                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(24) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(24) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(24) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(24) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(24) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(24) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                return new GetAnnouncementsResponse
                {
                    Announcements = announcements,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    IsSuccess = true
                };
            }

            throw new Exception("Employee details not found.");
        }

        public void UpdateEmployeeDataVerification(EmployeeDataVerificationRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeDataVerificationEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));

            if (employee != null)
            {
                var activeUpdate = employee.EmployeeDataVerificationEmployee.FirstOrDefault(var =>
                    var.IsActive && var.VerifiedOn == null && var.EmployeeSection.Equals(request.Section));

                if (activeUpdate != null)
                {
                    if (request.UserIdNum == employee.Id)
                    {
                        activeUpdate.UpdatedOn = DateTime.UtcNow;

                        if (request.UserRole == "HR")
                        {
                            activeUpdate.VerifiedBy = request.UserIdNum;
                            activeUpdate.VerifiedOn = DateTime.UtcNow;
                        }

                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.EmployeeSelfUpdate,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = employee.Id,
                            NotificationData = JsonConvert.SerializeObject(new
                            {
                                employee = employee.Guid,
                                userName = request.UserName,
                                section = request.Section
                            })
                        });
                    }
                    else
                    {
                        activeUpdate.VerifiedBy = request.UserIdNum;
                        activeUpdate.VerifiedOn = DateTime.UtcNow;

                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.EmployeeVerified,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = employee.Id,
                            NotificationData = JsonConvert.SerializeObject(new
                            {
                                employee = employee.Guid,
                                userName = request.UserName,
                                section = request.Section
                            })
                        });
                    }
                }
                else
                {
                    if (request.UserIdNum == employee.Id)
                    {
                        var data = new EmployeeDataVerification
                        {
                            Guid = CustomGuid.NewGuid(),
                            CompanyId = request.CompanyIdNum,
                            EmployeeSection = request.Section,
                            IsActive = true,
                            UpdatedBy = request.UserIdNum,
                            UpdatedOn = DateTime.UtcNow,
                        };

                        if (request.UserRole == "HR")
                        {
                            data.VerifiedBy = request.UserIdNum;
                            data.VerifiedOn = DateTime.UtcNow;
                        }

                        employee.EmployeeDataVerificationEmployee.Add(data);

                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.EmployeeSelfUpdate,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = employee.Id,
                            NotificationData = JsonConvert.SerializeObject(new
                            {
                                employee = employee.Guid,
                                userName = request.UserName,
                                section = request.Section
                            })
                        });
                    }
                    else
                    {
                        employee.EmployeeDataVerificationEmployee.Add(new EmployeeDataVerification
                        {
                            Guid = CustomGuid.NewGuid(),
                            CompanyId = request.CompanyIdNum,
                            EmployeeSection = request.Section,
                            IsActive = true,
                            UpdatedBy = request.UserIdNum,
                            UpdatedOn = DateTime.UtcNow,
                            VerifiedOn = DateTime.UtcNow,
                            VerifiedBy = request.UserIdNum
                        });

                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.EmployeeHrUpdate,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = employee.Id,
                            NotificationData = JsonConvert.SerializeObject(new
                            {
                                employee = employee.Guid,
                                userName = request.UserName,
                                section = request.Section
                            })
                        });
                    }
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.VerifyEmployeeDataUpdate.Template, request.UserName, request.UserId, employee.Name, employee.Guid, request.Section),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.VerifyEmployeeDataUpdate.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();
            }
            else
            {
                throw new Exception("Employee details not found.");
            }
        }

        public EmployeeDataVerificationResponse GetEmployeeDataVerification(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeDataVerificationEmployee).ThenInclude(var => var.UpdatedByNavigation)
                .Include(var => var.EmployeeDataVerificationEmployee).ThenInclude(var => var.VerifiedByNavigation)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));

            if (employee != null)
            {
                var updates = new List<EmployeeDataVerificationDto>();
                var sections = employee
                    .EmployeeDataVerificationEmployee
                    .Where(var => var.IsActive)
                    .DistinctBy(var => var.EmployeeSection)
                    .ToList();

                foreach (var section in sections)
                {
                    var latest = employee.EmployeeDataVerificationEmployee
                        .Where(var => var.IsActive && var.EmployeeSection.Equals(section.EmployeeSection))
                        .OrderByDescending(var => var.UpdatedOn)
                        .FirstOrDefault();

                    updates.Add(new EmployeeDataVerificationDto
                    {
                        Section = section.EmployeeSection,
                        UpdatedBy = latest.UpdatedByNavigation.Name,
                        UpdatedOn = latest.UpdatedOn,
                        VerifiedBy = latest.VerifiedOn != null ? latest.VerifiedByNavigation.Name : null,
                        VerifiedOn = latest.VerifiedOn
                    });
                }

                return new EmployeeDataVerificationResponse
                {
                    Verifications = updates,
                    IsSuccess = true
                };
            }
            else
            {
                return new EmployeeDataVerificationResponse
                {
                    IsSuccess = true
                };
            }
        }

        public EmployeeCardResponse GetEmployeesBirthday(BaseRequest request)
        {
            var employees = GetAll()
                .Include(var => var.EmployeePersonalEmployee)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                .Where(var => var.IsActive
                && var.EmployeePersonalEmployee.FirstOrDefault().HideBirthday != true
                && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode != null
                && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "on-roll"
                && var.EmployeePersonalEmployee.FirstOrDefault().DobActual.HasValue
                && var.EmployeePersonalEmployee.FirstOrDefault().DobActual.Value.BirthdayImminent(DateTime.Today, 7)
                )
                .Select(var => new EmployeeCardDto
                {
                    Department = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                    Designation = var.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name,
                    Location = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name,
                    Name = var.EmployeeCompanyEmployee.FirstOrDefault().AddressingName,
                    Image = var.EmployeePersonalEmployee.FirstOrDefault() == null ? null : var.EmployeePersonalEmployee.FirstOrDefault().PhotoLinkUrl,
                    EmployeeId = var.Guid,
                    BirthDate = var.EmployeePersonalEmployee.FirstOrDefault().DobActual.Value.ToString("dd, MMMM")
                })
                .OrderBy(var => DateTime.Parse(var.BirthDate))
                .ToList();

            return new EmployeeCardResponse
            {
                IsSuccess = true,
                Employees = employees
            };
        }

        public EmployeeCardResponse GetEmployeeOrgChart(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeePersonalEmployee)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Region)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Team)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo).ThenInclude(var => var.EmployeeCompanyEmployee)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Category)
                .Include(var => var.EmployeeCompanyReportingTo)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {

                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(3) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(3) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(3) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(3) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(3) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(3) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

                var employeeCompany = employee.EmployeeCompanyEmployee.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(employeeCompany.EmployeeCode) && employeeCompany.Department != null)
                {
                    var managerHierarchy = new List<EmployeeCardDto>();
                    var reportingManager = employeeCompany.ReportingTo;

                    if (reportingManager != null)
                    {
                        var managerEmployee = GetAll()
                            .Include(var => var.EmployeeCompanyReportingTo).ThenInclude(var => var.Employee)
                            .FirstOrDefault(var => var.Id == reportingManager.Id && var.IsActive);
                        if (managerEmployee != null && managerEmployee.EmployeeCompanyReportingTo != null && managerEmployee.EmployeeCompanyReportingTo.Any())
                        {
                            foreach (var emp in managerEmployee.EmployeeCompanyReportingTo)
                            {
                                var reportingEmp = GetAll()
                                    .Include(var => var.EmployeePersonalEmployee)
                                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Region)
                                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Team)
                                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                                    .FirstOrDefault(var => var.Id == emp.EmployeeId && var.IsActive);

                                if (reportingEmp != null)
                                {
                                    var reportingEmpCompany = reportingEmp.EmployeeCompanyEmployee.FirstOrDefault();
                                    if (managerHierarchy.Any(var => var.EmployeeId.Equals(reportingEmp.Guid)))
                                    {
                                        break;
                                    }

                                    managerHierarchy.Add(new EmployeeCardDto
                                    {
                                        Department = reportingEmpCompany?.Department?.Name ?? "NA",
                                        Designation = reportingEmpCompany?.Designation?.Name ?? "NA",
                                        Location = reportingEmpCompany?.Location?.Name ?? "NA",
                                        Name = reportingEmpCompany?.AddressingName,
                                        EmployeeId = reportingEmp.Guid,
                                        Code = reportingEmpCompany?.EmployeeCode ?? "NA",
                                        Manager = reportingManager != null ? reportingManager.Guid : null,
                                        Image = reportingEmp.EmployeePersonalEmployee.FirstOrDefault()?.PhotoLinkUrl
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        managerHierarchy.Add(new EmployeeCardDto
                        {
                            Department = employeeCompany?.Department?.Name ?? "NA",
                            Designation = employeeCompany?.Designation?.Name ?? "NA",
                            Location = employeeCompany?.Location?.Name ?? "NA",
                            Name = employeeCompany?.AddressingName,
                            EmployeeId = employee.Guid,
                            Code = employeeCompany?.EmployeeCode ?? "NA",
                            Manager = reportingManager != null ? reportingManager.Guid : null,
                            Image = employee.EmployeePersonalEmployee.FirstOrDefault()?.PhotoLinkUrl
                        });
                    }

                    while (reportingManager != null)
                    {
                        var managerEmployee = GetAll()
                                    .Include(var => var.EmployeePersonalEmployee)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Region)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Team)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                            .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                            .FirstOrDefault(var => var.Id == reportingManager.Id && var.IsActive);
                        if (managerEmployee != null)
                        {
                            reportingManager = managerEmployee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingTo;
                        }
                        else
                        {
                            reportingManager = null;
                        }

                        if (managerHierarchy.Any(var => var.EmployeeId.Equals(managerEmployee.Guid)))
                        {
                            break;
                        }

                        managerHierarchy.Add(new EmployeeCardDto
                        {
                            Department = managerEmployee.EmployeeCompanyEmployee.FirstOrDefault()?.Department?.Name ?? "NA",
                            Designation = managerEmployee.EmployeeCompanyEmployee.FirstOrDefault()?.Designation?.Name ?? "NA",
                            Location = managerEmployee.EmployeeCompanyEmployee.FirstOrDefault()?.Location?.Name ?? "NA",
                            Name = managerEmployee.EmployeeCompanyEmployee.FirstOrDefault()?.AddressingName,
                            EmployeeId = managerEmployee.Guid,
                            Code = managerEmployee?.EmployeeCompanyEmployee.FirstOrDefault()?.EmployeeCode ?? "NA",
                            Manager = reportingManager != null ? reportingManager.Guid : null,
                            Image = managerEmployee.EmployeePersonalEmployee.FirstOrDefault()?.PhotoLinkUrl
                        });
                    }

                    managerHierarchy = GetEmployeesReporting(employee.Guid, managerHierarchy, true);

                    var topEmployee = managerHierarchy.FirstOrDefault(var => var.Manager == null);
                    topEmployee = OrganizeEmployeeHierarchy(topEmployee, managerHierarchy);

                    return new EmployeeCardResponse
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        Employees = new List<EmployeeCardDto> { topEmployee }
                    };
                }
                else
                {
                    return new EmployeeCardResponse
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                    };
                }
            }
            else
            {
                throw new Exception("Employee details not found.");
            }
        }

        private EmployeeCardDto OrganizeEmployeeHierarchy(EmployeeCardDto topEmployee, List<EmployeeCardDto> hierarchy)
        {
            var reportingEmps = hierarchy.Where(var => var.Manager != null && var.Manager.Equals(topEmployee.EmployeeId)).ToList();
            foreach (var emp in reportingEmps)
            {
                OrganizeEmployeeHierarchy(emp, hierarchy);
            }

            topEmployee.Children = reportingEmps;
            return topEmployee;
        }

        private List<EmployeeCardDto> GetEmployeesReporting(string guid, List<EmployeeCardDto> reportingList, bool isParent)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Region)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Team)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .Include(var => var.EmployeeCompanyReportingTo).ThenInclude(var => var.Employee)
                .FirstOrDefault(var => var.IsActive
                                       && var.Guid.Equals(guid));

            if (employee != null && (!reportingList.Any(var => var.EmployeeId.Equals(employee.Guid)) || isParent))
            {
                var employeeCompany = employee.EmployeeCompanyEmployee.FirstOrDefault();
                if (!isParent)
                {
                    reportingList.Add(new EmployeeCardDto
                    {
                        Department = employeeCompany.Department?.Name,
                        Designation = employeeCompany.Designation?.Name,
                        Location = employeeCompany.Location?.Name,
                        Code = employeeCompany.EmployeeCode ?? "NA",
                        Name = employeeCompany.AddressingName,
                        EmployeeId = employee.Guid,
                        Manager = employeeCompany.ReportingTo?.Guid,
                        Image = employee.EmployeePersonalEmployee.FirstOrDefault()?.PhotoLinkUrl
                    });
                }

                var employeesReporting = employee
                    .EmployeeCompanyReportingTo;
                if (employeesReporting != null && employeesReporting.Any())
                {
                    foreach (var emp in employeesReporting)
                    {
                        GetEmployeesReporting(emp.Employee.Guid, reportingList, false);
                    }
                }
            }

            return reportingList;
        }

        public GetEmployeeAssetResponse UpdateEmployeeAssets(UpdateEmployeeAssetRequest request, bool removeExisting)
        {
            var employee = GetAll()
                   .Include(var => var.EmployeeAssetEmployee)
                   .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                if (removeExisting)
                {
                    dbContext.EmployeeAsset.RemoveRange(employee.EmployeeAssetEmployee);
                }

                foreach (var asset in request.Assets)
                {
                    var assetType = dbContext.SettingsAssetTypes
                        .Include(var => var.EmployeeAsset)
                        .FirstOrDefault(var => var.IsActive && var.Guid.Equals(asset.AssetId));

                    var isAssetUniqueIdTaken = assetType.EmployeeAsset.Any(var =>
                        var.IsActive
                        && !string.IsNullOrWhiteSpace(asset.EmployeeAssetId)
                        && !string.IsNullOrWhiteSpace(asset.AssetUniqueId)
                        && asset.AssetUniqueId.Equals(var.AssetUniqueId)
                        && !var.Guid.Equals(asset.EmployeeAssetId));

                    if (isAssetUniqueIdTaken)
                    {
                        return new GetEmployeeAssetResponse
                        {
                            IsSuccess = true,
                            AssetCodeTaken = true,
                            AssetUniqueId = asset.AssetUniqueId
                        };
                    }

                    if (string.IsNullOrWhiteSpace(asset.EmployeeAssetId))
                    {
                        var newAsset = new EmployeeAsset()
                        {
                            CompanyId = request.CompanyIdNum,
                            Guid = CustomGuid.NewGuid(),
                            AddedOn = DateTime.UtcNow,
                            AddedBy = request.UserIdNum,
                            IsActive = true,
                            Asset = assetType,
                            Description = asset.Description,
                            AssetUniqueId = asset.AssetUniqueId,
                            GivenOn = asset.GivenOn,
                        };
                        employee.EmployeeAssetEmployee.Add(newAsset);
                    }
                    else
                    {
                        var addedAsset = employee.EmployeeAssetEmployee.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(asset.EmployeeAssetId));
                        if (addedAsset != null)
                        {
                            if (asset.IsActive)
                            {
                                addedAsset.Asset = assetType;
                                addedAsset.Description = asset.Description;
                                addedAsset.AssetUniqueId = asset.AssetUniqueId;
                                addedAsset.GivenOn = asset.GivenOn;
                                addedAsset.UpdatedBy = request.UserIdNum;
                                addedAsset.UpdatedOn = DateTime.UtcNow;
                            }
                            else
                            {
                                addedAsset.IsActive = false;
                                addedAsset.UpdatedBy = request.UserIdNum;
                                addedAsset.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                    }
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateAssets.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateAssets.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "asset",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                return GetEmployeeAssets(new EmployeeActionRequest
                {
                    EmployeeId = request.EmployeeId,
                    UserIdNum = request.UserIdNum,
                    UserId = request.UserId
                });
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public GetEmployeeAssetResponse GetEmployeeAssets(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.EmployeeAssetEmployee).ThenInclude(var => var.UpdatedByNavigation)
                .Include(var => var.EmployeeAssetEmployee).ThenInclude(var => var.AddedByNavigation)
                .Include(var => var.EmployeeAssetEmployee).ThenInclude(var => var.Asset)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var assets = employee.EmployeeAssetEmployee
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeAssetDto
                    {
                        IsActive = true,
                        Description = var.Description,
                        AssetUniqueId = var.AssetUniqueId,
                        EmployeeAssetId = var.Guid,
                        AssetId = var.Asset.Guid,
                        AssetName = var.Asset.AssetType,
                        GivenOn = var.GivenOn,
                        LastUpdatedBy = var.UpdatedByNavigation != null ? var.UpdatedByNavigation.Name : var.AddedByNavigation.Name,
                        LastUpdatedOn = var.UpdatedOn != null ? var.UpdatedOn : var.AddedOn
                    })
                    .ToList();

                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(13) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(13) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(13) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(13) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(13) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(13) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                return new GetEmployeeAssetResponse
                {
                    IsSuccess = true,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    Assets = assets
                };
            }

            throw new Exception("Employee details not found.");
        }

        public ManagerDashboardStatResponse GetManagerStats(BaseRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .FirstOrDefault(var => var.IsActive && var.Id.Equals(request.UserIdNum));
            if (employee != null)
            {
                var managerHierarchy = new List<EmployeeCardDto>();
                managerHierarchy = GetEmployeesReporting(employee.Guid, managerHierarchy, true);

                var allEmployees = GetAll()
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                    .Where(var => var.IsActive && managerHierarchy.Any(var1 => var1.EmployeeId.Equals(var.Guid)))
                    .ToList();

                var totalOnRoll = allEmployees.Count(var => var.EmployeeCompanyEmployee != null && (var.EmployeeCompanyEmployee.FirstOrDefault()?.Status.Equals("on-roll") ?? false));
                var totalOffRoll = allEmployees.Count(var => var.EmployeeCompanyEmployee != null && (var.EmployeeCompanyEmployee.FirstOrDefault()?.Status.Equals("off-roll") ?? false));

                var distinctLocations = allEmployees
                    .Where(var => var.EmployeeCompanyEmployee != null && var.EmployeeCompanyEmployee.FirstOrDefault()?.Location != null)
                    .Select(var => new { LocationId = var.EmployeeCompanyEmployee.FirstOrDefault().LocationId, LocationName = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name })
                    .Distinct();

                var locationWiseList = new List<LocationWiseHeadCountDto>();

                foreach (var location in distinctLocations)
                {
                    var onRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "on-roll"
                    && var.EmployeeCompanyEmployee.FirstOrDefault().LocationId == location.LocationId);

                    var offRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "off-roll"
                    && var.EmployeeCompanyEmployee.FirstOrDefault().LocationId == location.LocationId);

                    locationWiseList.Add(new LocationWiseHeadCountDto
                    {
                        Location = location.LocationName,
                        OffRollCount = offRollCount,
                        OnRollCount = onRollCount
                    });
                }

                return new ManagerDashboardStatResponse
                {
                    IsSuccess = true,
                    LocationWiseHeadCount = locationWiseList,
                    OffRollPercent = totalOffRoll,
                    OnRollPercent = totalOnRoll
                };
            }

            throw new Exception("Employee details not found.");
        }

        public HrDashboardStatResponse GetHrStats(BaseRequest request)
        {
            var employee = GetAll()
                .FirstOrDefault(var => var.IsActive && var.Id.Equals(request.UserIdNum) && request.UserRole.Equals("HR"));
            if (employee != null)
            {
                var allEmployees = GetAll()
                    .Include(var => var.EmployeePersonalEmployee)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                    .Where(var => var.IsActive && var.EmployeeCompanyEmployee != null && var.EmployeeCompanyEmployee.Any())
                    .ToList();

                var totalOnRoll = allEmployees.Count(var => var.EmployeeCompanyEmployee != null && (var.EmployeeCompanyEmployee.FirstOrDefault()?.Status?.Equals("on-roll") ?? false));
                var totalOffRoll = allEmployees.Count(var => var.EmployeeCompanyEmployee != null && (var.EmployeeCompanyEmployee.FirstOrDefault()?.Status?.Equals("off-roll") ?? false));

                var distinctLocations = allEmployees
                    .Where(var => var.EmployeeCompanyEmployee != null && var.EmployeeCompanyEmployee.FirstOrDefault()?.Location != null)
                    .Select(var => new { LocationId = var.EmployeeCompanyEmployee.FirstOrDefault().LocationId, LocationName = var.EmployeeCompanyEmployee.FirstOrDefault().Location.Name })
                    .Distinct();

                var locationWiseList = new List<LocationWiseHeadCountDto>();

                foreach (var location in distinctLocations)
                {
                    var onRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "on-roll"
                    && var.EmployeeCompanyEmployee.FirstOrDefault().LocationId == location.LocationId);

                    var offRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Location != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "off-roll"
                    && var.EmployeeCompanyEmployee.FirstOrDefault().LocationId == location.LocationId);

                    locationWiseList.Add(new LocationWiseHeadCountDto
                    {
                        Location = location.LocationName,
                        OffRollCount = offRollCount,
                        OnRollCount = onRollCount
                    });
                }

                var distinctDepartments = allEmployees
                    .Where(var => var.EmployeeCompanyEmployee != null && var.EmployeeCompanyEmployee.FirstOrDefault()?.Department != null)
                    .Select(var => new { DepartmentId = var.EmployeeCompanyEmployee.FirstOrDefault().DepartmentId, DepartmentName = var.EmployeeCompanyEmployee.FirstOrDefault().Department.Name })
                    .Distinct();

                var departmentWiseList = new List<LocationWiseHeadCountDto>();

                foreach (var department in distinctDepartments)
                {
                    var onRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Department != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "on-roll"
                    && var.EmployeeCompanyEmployee.FirstOrDefault().DepartmentId == department.DepartmentId);

                    var offRollCount = allEmployees.Count(var => var.EmployeeCompanyEmployee != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Department != null
                    && var.EmployeeCompanyEmployee.FirstOrDefault().Status == "off-roll"
                    && var.EmployeeCompanyEmployee.FirstOrDefault().DepartmentId == department.DepartmentId);

                    departmentWiseList.Add(new LocationWiseHeadCountDto
                    {
                        Location = department.DepartmentName,
                        OffRollCount = offRollCount,
                        OnRollCount = onRollCount
                    });
                }

                var avgEmployeeInfo = allEmployees
                    .Where(var =>
                        var.EmployeePersonalEmployee.Any() &&
                        var.EmployeePersonalEmployee.FirstOrDefault()?.DobRecord != null);

                var averageAge = avgEmployeeInfo.Any()
                    ? avgEmployeeInfo.Average(var => var.EmployeePersonalEmployee.FirstOrDefault()?.DobRecord.Value.GetAge()) ?? 0
                    : 0;

                return new HrDashboardStatResponse
                {
                    IsSuccess = true,
                    LocationWiseHeadCount = locationWiseList,
                    OffRollPercent = totalOffRoll,
                    OnRollPercent = totalOnRoll,
                    DepartmentWiseHeadCount = departmentWiseList,
                    AverageAge = Convert.ToInt32(Math.Round(averageAge, 0))
                };

            }

            throw new Exception("Employee details not found.");
        }

        public BaseResponse UploadEmployeeData(UploadDataRequest request, ApplicationSettings appSettings)
        {
            var lineCount = 1;
            var roles = dbContext.SettingsRole.ToList();
            var allLines = request.FileContent.Split("\r\n");

            switch (request.Type)
            {
                case "Create":

                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var req = new CreateEmployeeRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };
                        req.CanLogin = values[3] == "Y";
                        req.Name = values[0];
                        var emailRandom = RandomString.GetRandomString(5);
                        req.OfficialEmail = (values[1] == "0" || values[1] == "-") ? emailRandom + "@kubota.com" : values[1];
                        req.Status = values[2] == "Live - On-Roll" ? "on-roll" : "off-roll";
                        req.RoleId = roles.FirstOrDefault(var => var.RoleName.Equals(values[4]))?.Guid;
                        req.EmployeCode = values[5];

                        CreateEmployee(req, appSettings, false);
                    }
                    break;

                case "Account":

                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        var req = new UpdateEmployeeAccountRequest()
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum,
                            EmployeeId = employee.Guid
                        };
                        req.CanLogin = values[3] == "Y";
                        req.Status = values[2] == "Live - On-Roll" ? "on-roll" : "off-roll";

                        UpdateEmployeeAccount(req, true);
                    }
                    break;

                case "PersonalDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));


                        var req = new UpdateEmployeePersonalRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };

                        if (employee == null)
                        {
                            continue;
                        }

                        req.EmployeeId = employee.Guid;
                        req.Nationality = values[1];
                        req.Gender = values[2].Equals("M") ? "Male" : "Female";
                        req.BloodGroup = values[3];
                        if (!string.IsNullOrWhiteSpace(values[4]))
                        {
                            req.RecordDob = DateTime.Parse(values[4]);
                        }
                        if (!string.IsNullOrWhiteSpace(values[5]))
                        {
                            req.ActualDob = DateTime.Parse(values[5]);
                        }
                        req.MaritalStatus = values[6];
                        req.MarriageDate = string.IsNullOrWhiteSpace(values[7]) ? (DateTime?)null : DateTime.Parse(values[7]);

                        UpdateEmployeePersonal(req);
                    }
                    break;

                case "StatutoryDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        var req = new UpdateEmployeeStatutoryRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };

                        req.EmployeeId = employee.Guid;
                        req.PanNumber = values[1];
                        req.PfNumber = values[2];
                        req.UanNumber = values[3];
                        req.AadharNumber = values[4];
                        req.DrivingLicenseNumber = values[5];
                        req.DrivingLicenseValidity = string.IsNullOrWhiteSpace(values[6])
                            ? (DateTime?)null
                            : DateTime.Parse(values[6]);
                        req.PassportNumber = values[7];
                        req.PassportValidity = string.IsNullOrWhiteSpace(values[8])
                            ? (DateTime?)null
                            : DateTime.Parse(values[8]);
                        req.EsiNumber = values[9];
                        req.LicIdNumber = values[10];

                        UpdateEmployeeStatutory(req);
                    }
                    break;

                case "ContactDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        var req = new UpdateEmployeeContactRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };

                        req.EmployeeId = employee.Guid;
                        req.ContactNumber = values[1];
                        req.PersonalEmail = values[2];
                        req.AlternateContactNumber = values[3];
                        req.OfficialEmail = values[4];
                        req.OfficialContactNumber = values[5];

                        req.PresentAddress = new EmployeeAddressDto
                        {
                            DoorNo = values[6],
                            Street = values[7],
                            Landmark = values[8],
                            City = values[9],
                            District = values[10],
                            Pincode = values[11],
                            State = values[12],
                            Country = values[13],
                        };

                        req.PermanentAddress = new EmployeeAddressDto
                        {
                            DoorNo = values[15],
                            Street = values[16],
                            Landmark = values[17],
                            City = values[18],
                            District = values[19],
                            Pincode = values[20],
                            State = values[21],
                            Country = values[22],
                        };

                        req.PermanentAddressSame = values[14] == "Y";
                        req.PermanentAddress = values[14] == "Y" ? req.PresentAddress : req.PermanentAddress;

                        UpdateEmployeeContact(req);
                    }
                    break;

                case "BankDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        var req = new UpdateEmployeeBankRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };

                        req.EmployeeId = employee.Guid;
                        req.EmployeeBanks = new List<EmployeeBankDto>();
                        req.EmployeeBanks.Add(new EmployeeBankDto
                        {
                            IsActive = true,
                            AccountNumber = values[2],
                            BankName = values[1],
                            IfscCode = values[3],
                            AccountType = values[4],
                            Branch = values[5],
                            EffectiveDate = string.IsNullOrWhiteSpace(values[6]) ? (DateTime?)null : DateTime.Parse(values[6])
                        });

                        UpdateEmployeeBank(req, true);
                    }
                    break;

                case "FamilyDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        var req = new UpdateEmployeeFamilyRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };

                        req.EmployeeId = employee.Guid;
                        req.EmployeeFamily = new List<EmployeeFamilyDto>();
                        req.EmployeeFamily.Add(new EmployeeFamilyDto
                        {
                            IsActive = true,
                            Name = values[1],
                            Email = values[4],
                            Relation = values[2],
                            Dob = values[5],
                            Phone = values[3].Length > 10 ? values[3].Substring(0, 10) : values[3],
                            IsEmergencyContact = values[7] == "Y",
                            IsDependant = values[8] == "Y",
                        });

                        UpdateEmployeeFamily(req, false);
                    }
                    break;

                case "LanguageDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        var req = new UpdateEmployeeLanguageRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };

                        req.EmployeeId = employee.Guid;
                        req.EmployeeLanguage = new List<EmployeeLanguageDto>();
                        req.EmployeeLanguage.Add(new EmployeeLanguageDto
                        {
                            IsActive = true,
                            Language = values[1],
                            Level = values[2],
                            CanSpeak = string.IsNullOrWhiteSpace(values[3]) || values[3] == "Y",
                            CanWrite = string.IsNullOrWhiteSpace(values[4]) || values[4] == "Y",
                            CanRead = string.IsNullOrWhiteSpace(values[5]) || values[5] == "Y",
                        });

                        UpdateEmployeeLanguage(req, false);
                    }
                    break;

                case "AssetDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        var req = new UpdateEmployeeAssetRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };

                        var assetType = dbContext.SettingsAssetTypes.FirstOrDefault(var => var.IsActive && var.AssetType.Equals(values[1]));
                        if (assetType != null)
                        {
                            req.EmployeeId = employee.Guid;
                            req.Assets = new List<EmployeeAssetDto>();
                            var newAsset = new EmployeeAssetDto
                            {
                                IsActive = true,
                                AssetId = assetType.Guid,
                                AssetName = values[2],
                                Description = values[3],
                                AssetUniqueId = values[4]
                            };
                            if (!string.IsNullOrWhiteSpace(values[5]))
                            {
                                newAsset.GivenOn = DateTime.Parse(values[5]);
                            }
                            req.Assets.Add(newAsset);
                            UpdateEmployeeAssets(req, false);
                        }
                    }
                    break;

                case "EducationDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        var req = new UpdateEmployeeEducationRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };

                        req.EmployeeId = employee.Guid;
                        req.EmployeeEducation = new List<EmployeeEducationDto>();
                        var perc = 0.0;
                        Double.TryParse(values[8], out perc);

                        req.EmployeeEducation.Add(new EmployeeEducationDto
                        {
                            IsActive = true,
                            CourseName = values[1],
                            Institute = values[2],
                            StartedYear = string.IsNullOrWhiteSpace(values[3]) ? (int?)null : Convert.ToInt32(values[3]),
                            CompletedYear = string.IsNullOrWhiteSpace(values[4]) ? (int?)null : Convert.ToInt32(values[4]),
                            MajorSubject = values[5],
                            CourseType = values[6],
                            Grade = values[7],
                            Percentage = string.IsNullOrWhiteSpace(values[8]) ? (double?)null : perc
                        });

                        UpdateEmployeeEducation(req, false);
                    }
                    break;

                case "PreviousEmploymentDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        var req = new UpdateEmployeePreviousCompanyRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };

                        req.EmployeeId = employee.Guid;
                        req.PreviousCompanies = new List<EmployeePreviousCompanyDto>();

                        var ctc = 0.0;
                        DateTime dateOfExit;
                        DateTime dateOfJoin;

                        Double.TryParse(values[4], out ctc);
                        DateTime.TryParse(values[5], out dateOfExit);
                        DateTime.TryParse(values[6], out dateOfJoin);

                        req.PreviousCompanies.Add(new EmployeePreviousCompanyDto
                        {
                            IsActive = true,
                            Employer = values[1],
                            Department = values[2],
                            Designation = values[3],
                            Ctc = ctc,
                            DateOfExit = dateOfExit,
                            DateOfJoin = dateOfJoin,
                            ReasonForChange = values[7]
                        });

                        UpdateEmployeePreviousCompany(req, false);
                    }
                    break;

                case "CompensationDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }
                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        string departmentId = null;
                        string designationId = null;
                        string gradeId = null;

                        var req = new EmployeeCompensationRequest
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum
                        };
                        req.EmployeeId = employee.Guid;

                        //if (values.Length > 24 && !string.IsNullOrWhiteSpace(values[24]))
                        //{
                        //    var department = dbContext.SettingsDepartment.FirstOrDefault(var =>
                        //        var.IsActive && var.Name.ToLower().Equals(values[24]));
                        //    if (department != null)
                        //    {
                        //        departmentId = department.Guid;
                        //    }
                        //}

                        //if (values.Length > 24 && !string.IsNullOrWhiteSpace(values[25]))
                        //{
                        //    var designation = dbContext.SettingsDepartmentDesignation.FirstOrDefault(var =>
                        //        var.IsActive && var.Name.ToLower().Equals(values[25]));
                        //    if (designation != null)
                        //    {
                        //        designationId = designation.Guid;
                        //    }
                        //}

                        //if (values.Length > 24 && !string.IsNullOrWhiteSpace(values[26]))
                        //{
                        //    var grade = dbContext.SettingsGrade.FirstOrDefault(var =>
                        //        var.IsActive && var.Grade.ToLower().Equals(values[26]));
                        //    if (grade != null)
                        //    {
                        //        gradeId = grade.Guid;
                        //    }
                        //}

                        req.EmployeeCompensation = new List<EmployeeCompensationDto>();
                        req.EmployeeCompensation.Add(new EmployeeCompensationDto
                        {
                            IsActive = true,
                            Year = Convert.ToInt32(values[1]),
                            AnnualBasic = Convert.ToDouble(values[2]),
                            AnnualHra = Convert.ToDouble(values[3]),
                            AnnualConvAllow = Convert.ToDouble(values[4]),
                            AnnualSplAllow = Convert.ToDouble(values[5]),
                            AnnualMedAllow = Convert.ToDouble(values[6]),
                            AnnualLta = Convert.ToDouble(values[7]),
                            AnnualWashing = Convert.ToDouble(values[8]),
                            AnnualChildEdu = Convert.ToDouble(values[9]),
                            StatutoryBonus = Convert.ToDouble(values[10]),
                            AnnualGross = Convert.ToDouble(values[11]),
                            AnnualVarBonus = Convert.ToDouble(values[12]),
                            AnnualVarBonusPaid1 = Convert.ToDouble(string.IsNullOrWhiteSpace(values[13]) ? "0" : values[13]),
                            AnnualVarBonusPaid2 = Convert.ToDouble(string.IsNullOrWhiteSpace(values[14]) ? "0" : values[14]),
                            AnnualAccidIns = Convert.ToDouble(values[15]),
                            AnnualHealthIns = Convert.ToDouble(values[16]),
                            AnnualGratuity = Convert.ToDouble(values[17]),
                            AnnualPf = Convert.ToDouble(values[18]),
                            AnnualEsi = Convert.ToDouble(values[19]),
                            OtherBenefits = Convert.ToDouble(values[20]),
                            AnnualCtc = Convert.ToDouble(values[21]),
                            VendorCharges = Convert.ToDouble(values[22]),
                            OffrollCtc = Convert.ToDouble(values[23]),
                            DesignationId = designationId,
                            DepartmentId = departmentId,
                            GradeId = gradeId,
                        });
                        UpdateEmployeeCompensation(req, false);
                    }
                    break;

                case "TrainingDetails":
                    var trainings = dbContext.Training.Where(var => var.IsActive);
                    var trainingNumber = trainings.Any() ? trainings.Max(var => var.TrainingId) : 1;
                    var combination = string.Empty;
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }

                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .Include(var => var.TrainingNomineesEmployee).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingDate)
                            .Include(var => var.TrainingNomineesEmployee).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingAttendance)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        var trainingName = values[1];
                        var trainingSettingsAdded =
                            dbContext.SettingsTraining.FirstOrDefault(var => var.IsActive && var.Name.Equals(trainingName));
                        SettingsTraining settingsTraining = null;

                        if (trainingSettingsAdded != null)
                        {
                            settingsTraining = trainingSettingsAdded;
                        }
                        else
                        {
                            settingsTraining = new SettingsTraining
                            {
                                Guid = CustomGuid.NewGuid(),
                                IsActive = true,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.Now,
                                CompanyId = 1,
                                Name = trainingName,
                                TrainingCode = values[1]
                            };
                            dbContext.SettingsTraining.Add(settingsTraining);
                            Save();
                        }

                        if (!combination.Equals(string.Concat(values[1], values[2])))
                        {
                            ++trainingNumber;
                            combination = string.Concat(values[1], values[2]);
                        }

                        var trainingAdded = dbContext.Training
                            .FirstOrDefault(var => var.IsActive
                                                   && var.TrainingNavigation.Id == settingsTraining.Id &&
                                                   var.TrainingNumber == trainingNumber);
                        if (trainingAdded == null)
                        {
                            var organizer = GetAll()
                                .Include(var => var.EmployeeCompanyEmployee)
                                .FirstOrDefault(var =>
                                    var.IsActive &&
                                    var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[4])));

                            trainingAdded = new Training
                            {
                                Guid = CustomGuid.NewGuid(),
                                IsActive = true,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.Now,
                                IsOfficeLocation = true,
                                //TotalDays = Convert.ToInt32(values[5]),
                                TrainingNumber = Convert.ToInt32(trainingNumber),
                                TrainingNavigation = settingsTraining,
                                OtherLocation = values[3],
                                TrainerName = values[6],
                                IsConfirmed = true,
                                TrainingOrganizer = new List<TrainingOrganizer>
                                {
                                    new TrainingOrganizer
                                    {
                                        Employee = organizer
                                    }
                                }
                            };
                            dbContext.Training.Add(trainingAdded);
                        }

                        var days = new List<DateTime>();
                        if (values[2].Contains("-"))
                        {
                            var startDate = DateTime.Parse(values[2].Split("-")[0].Trim());
                            var endDate = DateTime.Parse(values[2].Split("-")[1].Trim());

                            if (startDate < DateTime.Today)
                            {
                                trainingAdded.IsStarted = true;
                            }

                            if (endDate < DateTime.Today)
                            {
                                trainingAdded.IsCompleted = true;
                                trainingAdded.IsFeedbackClosed = true;
                            }

                            if (startDate <= endDate)
                            {
                                while (startDate <= endDate)
                                {
                                    days.Add(startDate);
                                    startDate = startDate.AddDays(1);
                                }
                            }
                        }
                        else
                        {
                            days.Add(DateTime.Parse(values[2]));

                            if (DateTime.Parse(values[2]) < DateTime.Today)
                            {
                                trainingAdded.IsStarted = true;
                                trainingAdded.IsCompleted = true;
                                trainingAdded.IsFeedbackClosed = true;
                            }
                        }

                        foreach (var day in days)
                        {
                            var addedDate = trainingAdded.TrainingDate.FirstOrDefault(var =>
                                var.Date.Value.Date.Equals(day));
                            if (addedDate == null)
                            {
                                addedDate = new TrainingDate
                                {
                                    Date = day,
                                    Training = trainingAdded
                                };
                                trainingAdded.TrainingDate.Add(addedDate);
                            }

                            var addedNominee = trainingAdded.TrainingNominees.FirstOrDefault(var =>
                                var.Employee.Guid.Equals(employee.Guid));
                            if (addedNominee == null)
                            {
                                var nominee = new TrainingNominees
                                {
                                    Guid = CustomGuid.NewGuid(),
                                    Training = trainingAdded,
                                    Employee = employee,
                                    IsRejected = false,
                                    SelfAccepted = true,
                                    SelfUpdatedOn = DateTime.Now,
                                    ManagerAccepted = true,
                                    HrAccepted = true,
                                    HrUpdatedOn = DateTime.Now,
                                    ManagerUpdatedOn = DateTime.Now,
                                    TrainingAttendance = new List<TrainingAttendance>
                                    {
                                        new TrainingAttendance
                                        {
                                            TrainingDateNavigation = addedDate,
                                            HasAttended = true,
                                            Training = trainingAdded
                                        }
                                    }
                                };
                                trainingAdded.TrainingNominees.Add(nominee);
                            }
                            else
                            {
                                var attendancePresent = addedNominee.TrainingAttendance.FirstOrDefault(var =>
                                    var.TrainingDateNavigation == addedDate && var.Training == trainingAdded);
                                if (attendancePresent != null)
                                {
                                    attendancePresent.HasAttended = values[5] == "Y";
                                }
                                else
                                {
                                    addedNominee.TrainingAttendance.Add(new TrainingAttendance
                                    {
                                        TrainingDateNavigation = addedDate,
                                        HasAttended = values[5] == "Y",
                                        Training = trainingAdded
                                    });
                                }
                            }
                        }

                        Save();
                    }
                    break;

                case "CompanyDetails":
                    foreach (var line in allLines)
                    {
                        if (lineCount <= request.SkipLines)
                        {
                            lineCount++;
                            continue;
                        }

                        var values = line.Split("\t");
                        if (string.IsNullOrWhiteSpace(values[0]))
                        {
                            continue;
                        }
                        var employee = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                                var.IsActive &&
                                var.EmployeeCompanyEmployee.Any(var1 => var1.EmployeeCode.Equals(values[0])));

                        if (employee == null)
                        {
                            continue;
                        }

                        string categoryId = null;
                        string departmentId = null;
                        string designationId = null;
                        string locationId = null;
                        string reportingToId = null;
                        string regionId = null;
                        string gradeId = null;
                        string teamId = null;

                        var category = dbContext.SettingsCategory.FirstOrDefault(var =>
                            var.IsActive && var.Category.ToLower().Equals(values[3]));
                        if (category != null)
                        {
                            categoryId = category.Guid;
                        }

                        var department = dbContext.SettingsDepartment.FirstOrDefault(var =>
                            var.IsActive && var.Name.ToLower().Equals(values[1]));
                        if (department != null)
                        {
                            departmentId = department.Guid;
                        }

                        var designation = dbContext.SettingsDepartmentDesignation.FirstOrDefault(var =>
                            var.IsActive && var.Name.ToLower().Equals(values[2]));
                        if (designation != null)
                        {
                            designationId = designation.Guid;
                        }

                        var location = dbContext.SettingsLocation.FirstOrDefault(var =>
                            var.IsActive && var.Name.ToLower().Equals(values[9]));
                        if (location != null)
                        {
                            locationId = location.Guid;
                        }

                        var region = dbContext.SettingsRegion.FirstOrDefault(var =>
                            var.IsActive && var.Name.ToLower().Equals(values[5]));
                        if (region != null)
                        {
                            regionId = region.Guid;
                        }

                        var grade = dbContext.SettingsGrade.FirstOrDefault(var =>
                            var.IsActive && var.Grade.ToLower().Equals(values[6]));
                        if (grade != null)
                        {
                            gradeId = grade.Guid;
                        }

                        var team = dbContext.SettingsTeam.FirstOrDefault(var =>
                            var.IsActive && var.Name.ToLower().Equals(values[8]));
                        if (team != null)
                        {
                            teamId = team.Guid;
                        }

                        var reportingTo = GetAll()
                            .Include(var => var.EmployeeCompanyEmployee)
                            .FirstOrDefault(var =>
                            var.IsActive && var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode.Equals(values[4]));
                        if (reportingTo != null)
                        {
                            reportingToId = reportingTo.Guid;
                        }

                        var req = new UpdateEmployeeCompanyRequest()
                        {
                            UserIdNum = request.UserIdNum,
                            UserId = request.UserId,
                            UserName = request.UserName,
                            CompanyIdNum = request.CompanyIdNum,
                            EmployeeId = employee.Guid,
                            DesignationId = designationId,
                            ReportingToId = reportingToId,
                            DepartmentId = departmentId,
                            Doj = DateTime.Parse(values[11]),
                            TeamId = teamId,
                            EmployeeCode = values[0],
                            CategoryId = categoryId,
                            LocationId = locationId,
                            GradeId = gradeId,
                            RegionId = regionId,
                            LocationBifurcation = values[10],
                            Division = values[7],
                            AddressingName = employee.EmployeeCompanyEmployee.FirstOrDefault()?.AddressingName,
                        };

                        UpdateEmployeeCompany(req, true);
                    }
                    break;
            }

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public EmployeeCompensationResponse GetEmployeeCompensation(EmployeeActionRequest request)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .Include(var => var.EmployeeCompensationEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var loggedInUserCode = GetAll()
                    .Include(var => var.EmployeeCompanyEmployee)
                    .FirstOrDefault(var => var.Id == request.UserIdNum)?
                    .EmployeeCompanyEmployee.FirstOrDefault()?.EmployeeCode;


                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(14) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));

                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(14) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(14) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(14) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(14) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(14) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                var canGet = true;
                //if (!string.IsNullOrWhiteSpace(loggedInUserCode))
                //{
                //    if (employee.Id == request.UserIdNum
                //        || loggedInUserCode.Equals("9707")
                //        || loggedInUserCode.Equals("9265")
                //        || loggedInUserCode.Equals("9288")
                //        || loggedInUserCode.Equals("9346")
                //    )
                //    {
                //        canGet = true;
                //    }
                //}

                if (canGet)
                {
                    var comps = employee.EmployeeCompensationEmployee
                        .Where(compensation => compensation.IsActive)
                        .Select(compensation => new EmployeeCompensationDto()
                        {
                            EmployeeCompensationId = compensation.Guid,
                            AnnualAccidIns = compensation.AnnualAccidIns,
                            AnnualBasic = compensation.AnnualBasic,
                            AnnualChildEdu = compensation.AnnualChildEdu,
                            AnnualConvAllow = compensation.AnnualConvAllow,
                            AnnualCtc = compensation.AnnualCtc,
                            AnnualEsi = compensation.AnnualEsi,
                            AnnualGratuity = compensation.AnnualGratuity,
                            AnnualGross = compensation.AnnualGross,
                            AnnualHealthIns = compensation.AnnualHealthIns,
                            AnnualHra = compensation.AnnualHra,
                            AnnualLta = compensation.AnnualLta,
                            AnnualMedAllow = compensation.AnnualMedAllow,
                            AnnualPf = compensation.AnnualPf,
                            AnnualSplAllow = compensation.AnnualSplAllow,
                            AnnualVarBonus = compensation.AnnualVarBonus,
                            AnnualVarBonusPaid1 = compensation.AnnualVarBonusPaid1,
                            AnnualVarBonusPaid2 = compensation.AnnualVarBonusPaid2,
                            AnnualWashing = compensation.AnnualWashing,
                            OtherBenefits = compensation.OtherBenefits,
                            OffrollCtc = compensation.OffrollCtc,
                            StatutoryBonus = compensation.StatutoryBonus,
                            VendorCharges = compensation.VendorCharges,
                            Year = compensation.Year,
                            IsActive = true
                        }).ToList();

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.GetEmployeeFamily.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.UpdateEmployeeCompensation.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName
                        })
                    });

                    Save();

                    return new EmployeeCompensationResponse()
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        IsAllowed = true,
                        EmployeeCompensation = comps,
                        IsOnRoll = ("on-roll").Equals(employee.EmployeeCompanyEmployee.FirstOrDefault().Status),
                    };
                }
                else
                {
                    return new EmployeeCompensationResponse()
                    {
                        IsSuccess = true,
                        HrAccess = hrAccess,
                        MgAccess = mgAccess,
                        EmpAccess = empAccess,
                        IsAllowed = false
                    };
                }
            }

            throw new Exception("Employee details not found.");
        }

        public EmployeeCompensationResponse UpdateEmployeeCompensation(EmployeeCompensationRequest request, bool removeExisting = false)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeCompensationEmployee)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                if (removeExisting)
                {
                    dbContext.EmployeeCompensation.RemoveRange(employee.EmployeeCompensationEmployee);
                }
                foreach (var compensation in request.EmployeeCompensation)
                {
                    var grade = !string.IsNullOrWhiteSpace(compensation.GradeId) ?
                        dbContext.SettingsGrade.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(compensation.GradeId))
                        : null;

                    var department = !string.IsNullOrWhiteSpace(compensation.DepartmentId) ?
                        dbContext.SettingsDepartment.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(compensation.DepartmentId))
                        : null;

                    var designation = !string.IsNullOrWhiteSpace(compensation.DesignationId) ?
                        dbContext.SettingsDepartmentDesignation.FirstOrDefault(
                            var => var.IsActive && var.Guid.Equals(compensation.DesignationId))
                        : null;

                    if (string.IsNullOrWhiteSpace(compensation.EmployeeCompensationId))
                    {
                        var newCompensation = new EmployeeCompensation()
                        {
                            CompanyId = request.CompanyIdNum,
                            Guid = CustomGuid.NewGuid(),
                            AddedOn = DateTime.UtcNow,
                            AddedBy = request.UserIdNum,
                            IsActive = true,
                            AnnualAccidIns = compensation.AnnualAccidIns,
                            AnnualBasic = compensation.AnnualBasic,
                            AnnualChildEdu = compensation.AnnualChildEdu,
                            AnnualConvAllow = compensation.AnnualConvAllow,
                            AnnualCtc = compensation.AnnualCtc,
                            AnnualEsi = compensation.AnnualEsi,
                            AnnualGratuity = compensation.AnnualGratuity,
                            AnnualGross = compensation.AnnualGross,
                            AnnualHealthIns = compensation.AnnualHealthIns,
                            AnnualHra = compensation.AnnualHra,
                            AnnualLta = compensation.AnnualLta,
                            AnnualMedAllow = compensation.AnnualMedAllow,
                            AnnualPf = compensation.AnnualPf,
                            AnnualSplAllow = compensation.AnnualSplAllow,
                            AnnualVarBonus = compensation.AnnualVarBonus,
                            AnnualVarBonusPaid1 = compensation.AnnualVarBonusPaid1,
                            AnnualVarBonusPaid2 = compensation.AnnualVarBonusPaid2,
                            AnnualWashing = compensation.AnnualWashing,
                            OtherBenefits = compensation.OtherBenefits,
                            OffrollCtc = compensation.OffrollCtc,
                            StatutoryBonus = compensation.StatutoryBonus,
                            VendorCharges = compensation.VendorCharges,
                            Year = compensation.Year,
                            DesignationId = designation?.Id,
                            GradeId = grade?.Id,
                            DepartmentId = department?.Id
                        };
                        employee.EmployeeCompensationEmployee.Add(newCompensation);
                    }
                    else
                    {
                        var addedCompensation = employee.EmployeeCompensationEmployee.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(compensation.EmployeeCompensationId));
                        if (addedCompensation != null)
                        {
                            if (compensation.IsActive)
                            {
                                addedCompensation.AnnualAccidIns = compensation.AnnualAccidIns;
                                addedCompensation.AnnualBasic = compensation.AnnualBasic;
                                addedCompensation.AnnualChildEdu = compensation.AnnualChildEdu;
                                addedCompensation.AnnualConvAllow = compensation.AnnualConvAllow;
                                addedCompensation.AnnualCtc = compensation.AnnualCtc;
                                addedCompensation.AnnualEsi = compensation.AnnualEsi;
                                addedCompensation.AnnualGratuity = compensation.AnnualGratuity;
                                addedCompensation.AnnualGross = compensation.AnnualGross;
                                addedCompensation.AnnualHealthIns = compensation.AnnualHealthIns;
                                addedCompensation.AnnualHra = compensation.AnnualHra;
                                addedCompensation.AnnualLta = compensation.AnnualLta;
                                addedCompensation.AnnualMedAllow = compensation.AnnualMedAllow;
                                addedCompensation.AnnualPf = compensation.AnnualPf;
                                addedCompensation.AnnualSplAllow = compensation.AnnualSplAllow;
                                addedCompensation.AnnualVarBonus = compensation.AnnualVarBonus;
                                addedCompensation.AnnualVarBonusPaid1 = compensation.AnnualVarBonusPaid1;
                                addedCompensation.AnnualVarBonusPaid2 = compensation.AnnualVarBonusPaid2;
                                addedCompensation.AnnualWashing = compensation.AnnualWashing;
                                addedCompensation.OffrollCtc = compensation.OffrollCtc;
                                addedCompensation.StatutoryBonus = compensation.StatutoryBonus;
                                addedCompensation.VendorCharges = compensation.VendorCharges;
                                addedCompensation.OtherBenefits = compensation.OtherBenefits;
                                addedCompensation.Year = compensation.Year;
                                addedCompensation.UpdatedBy = request.UserIdNum;
                                addedCompensation.UpdatedOn = DateTime.UtcNow;
                                addedCompensation.DesignationId = designation?.Id;
                                addedCompensation.GradeId = grade?.Id;
                                addedCompensation.DepartmentId = department?.Id;
                            }
                            else
                            {
                                addedCompensation.IsActive = false;
                                addedCompensation.UpdatedBy = request.UserIdNum;
                                addedCompensation.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                    }
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateEmployeeFamily.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeeFamily.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();

                UpdateEmployeeDataVerification(new EmployeeDataVerificationRequest
                {
                    Section = "compensation",
                    EmployeeId = request.EmployeeId,
                    CompanyIdNum = request.CompanyIdNum,
                    UserIdNum = request.UserIdNum
                });

                if (removeExisting)
                {
                    return new EmployeeCompensationResponse
                    {
                        IsSuccess = true
                    };
                }

                return GetEmployeeCompensation(new EmployeeActionRequest
                {
                    EmployeeId = request.EmployeeId,
                    UserIdNum = request.UserIdNum,
                    UserId = request.UserId
                });
            }

            throw new Exception("Employee not found - " + request.EmployeeId);
        }

        public EmployeeTrainingResponse GetEmployeeTrainings(EmployeeActionRequest request)
        {
            var employee = GetAll()

                .Include(var => var.TrainingOrganizer).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingNavigation)
                .Include(var => var.TrainingOrganizer).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingDate)
                .Include(var => var.TrainingOrganizer).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingLocationNavigation)
                .Include(var => var.TrainingOrganizer).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingOrganizer).ThenInclude(var => var.Employee)
                .Include(var => var.TrainingOrganizer).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingAttendance)
                .Include(var => var.TrainingOrganizer).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingFeedback)

                .Include(var => var.TrainingNomineesManager).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingNavigation)
                .Include(var => var.TrainingNomineesManager).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingDate)
                .Include(var => var.TrainingNomineesManager).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingLocationNavigation)
                .Include(var => var.TrainingNomineesManager).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingOrganizer).ThenInclude(var => var.Employee)
                .Include(var => var.TrainingNomineesManager).ThenInclude(var => var.Manager)
                .Include(var => var.TrainingNomineesManager).ThenInclude(var => var.Hr)
                .Include(var => var.TrainingNomineesManager).ThenInclude(var => var.Employee)
                .Include(var => var.TrainingNomineesManager).ThenInclude(var => var.TrainingFeedback)

                .Include(var => var.TrainingNomineesEmployee).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingNavigation)
                .Include(var => var.TrainingNomineesEmployee).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingDate)
                .Include(var => var.TrainingNomineesEmployee).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingLocationNavigation)
                .Include(var => var.TrainingNomineesEmployee).ThenInclude(var => var.Training).ThenInclude(var => var.TrainingOrganizer).ThenInclude(var => var.Employee)
                .Include(var => var.TrainingNomineesEmployee).ThenInclude(var => var.Manager)
                .Include(var => var.TrainingNomineesEmployee).ThenInclude(var => var.Hr)
                .Include(var => var.TrainingNomineesEmployee).ThenInclude(var => var.TrainingFeedback)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.EmployeeId));
            if (employee != null)
            {
                var trainings = employee.TrainingNomineesEmployee
                    .Where(var => var.Training.IsActive && (var.Training.IsConfirmed ?? false))
                    .Select(var => new EmployeeTrainingDto
                    {
                        HrUpdatedOn = var.HrUpdatedOn,
                        ManagerUpdatedOn = var.ManagerUpdatedOn,
                        SelfUpdatedOn = var.SelfUpdatedOn,
                        NomineeId = var.Guid,
                        TrainerName = var.Training.TrainerName,
                        TrainingName = var.Training.TrainingNavigation?.Name,
                        TrainingId = var.Training.Guid,
                        IsSelfAccepted = var.SelfAccepted,
                        IsMangerAccepted = var.ManagerAccepted,
                        IsHrAccepted = var.HrAccepted,
                        IsRejected = var.IsRejected,
                        IsFeedbackCompleted = var.TrainingFeedback != null ? var.TrainingFeedback.Any() : false,
                        ManagerName = var.Manager == null ? string.Empty : var.Manager.Name,
                        HrName = var.Hr == null ? string.Empty : var.Hr.Name,
                        StartDate = var.Training.TrainingDate.Min(var1 => var1.Date.Value),
                        EndDate = var.Training.TrainingDate.Max(var1 => var1.Date.Value),
                        IsConfirmed = var.Training.IsConfirmed ?? false,
                        IsCompleted = var.Training.IsCompleted ?? false,
                        IsStarted = var.Training.IsStarted ?? false,
                        Location = var.Training.IsOfficeLocation ? var.Training.TrainingLocationNavigation?.Name : var.Training.OtherLocation,
                        Organizers = var.Training.TrainingOrganizer.Any() ? string.Join(", ", var.Training.TrainingOrganizer.Select(var1 => var1.Employee.Name).ToList()) : string.Empty,
                    })
                    .OrderByDescending(var => var.StartDate)
                    .ToList();

                var reportees = employee.TrainingNomineesManager
                    .Where(var => var.Training.IsActive && (var.Training.IsConfirmed ?? false))
                    .Select(var => new EmployeeTrainingDto
                    {
                        HrUpdatedOn = var.HrUpdatedOn,
                        ManagerUpdatedOn = var.ManagerUpdatedOn,
                        SelfUpdatedOn = var.SelfUpdatedOn,
                        NomineeId = var.Guid,
                        TrainerName = var.Training.TrainerName,
                        EmployeeName = var.Employee.Name,
                        TrainingName = var.Training?.TrainingNavigation?.Name,
                        TrainingId = var.Training.Guid,
                        IsSelfAccepted = var.SelfAccepted,
                        IsMangerAccepted = var.ManagerAccepted,
                        IsHrAccepted = var.HrAccepted,
                        IsRejected = var.IsRejected,
                        IsFeedbackCompleted = var.TrainingFeedback != null ? var.TrainingFeedback.Any() : false,
                        ManagerName = var.Manager == null ? string.Empty : var.Manager.Name,
                        HrName = var.Hr == null ? string.Empty : var.Hr.Name,
                        StartDate = var.Training.TrainingDate.Min(var1 => var1.Date.Value),
                        EndDate = var.Training.TrainingDate.Max(var1 => var1.Date.Value),
                        IsConfirmed = var.Training.IsConfirmed ?? false,
                        IsCompleted = var.Training.IsCompleted ?? false,
                        IsStarted = var.Training.IsStarted ?? false,
                        Location = var.Training.IsOfficeLocation && var.Training.TrainingLocationNavigation != null ? var.Training.TrainingLocationNavigation.Name : var.Training.OtherLocation,
                        Organizers = var.Training.TrainingOrganizer != null ? string.Join(", ", var.Training.TrainingOrganizer.Select(var1 => var1.Employee.Name).ToList()) : string.Empty,
                    })
                    .OrderByDescending(var => var.StartDate)
                    .ToList();


                var organized = employee.TrainingOrganizer
                    .Where(var => var.Training.IsActive)
                    .Select(var => new EmployeeTrainingDto
                    {
                        Effectiveness = var.Training.TrainingFeedback.Any()
                        ? Math.Round(var.Training.TrainingFeedback
                            .Average(var1 => Convert.ToDouble(var1.Answer ?? "0")), 1)
                            .ToString(CultureInfo.InvariantCulture)
                        : "0",
                        Attendance = var.Training.TrainingAttendance != null ? (var.Training.TrainingAttendance.Count(var1 => var1.HasAttended ?? false) + " of " + var.Training.TrainingAttendance.Count) : string.Empty,
                        TrainerName = var.Training.TrainerName,
                        EmployeeName = var.Employee.Name,
                        TrainingName = var.Training.TrainingNavigation.Name,
                        TrainingId = var.Training.Guid,
                        StartDate = var.Training.TrainingDate.Min(var1 => var1.Date.Value),
                        EndDate = var.Training.TrainingDate.Max(var1 => var1.Date.Value),
                        IsConfirmed = var.Training.IsConfirmed ?? false,
                        IsCompleted = var.Training.IsCompleted ?? false,
                        IsFeedbackClosed = var.Training.IsFeedbackClosed ?? false,
                        IsStarted = var.Training.IsStarted ?? false,
                        Location = var.Training.IsOfficeLocation && var.Training.TrainingLocationNavigation != null ? var.Training.TrainingLocationNavigation.Name : var.Training.OtherLocation,
                        Organizers = string.Join(", ", var.Training.TrainingOrganizer.Select(var1 => var1.Employee.Name).ToList()),
                    })
                    .OrderByDescending(var => var.StartDate)
                    .ToList();

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetEmployeeFamily.Template, request.UserName, request.UserId, employee.Name, employee.Guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateEmployeeCompensation.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName
                    })
                });

                Save();


                var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
                var role = employee.RoleId;
                var empid = request.UserIdNum;

                var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(15) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));

                var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(15) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

                var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(15) && a.Ismanager.Equals(1));
                var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(15) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

                var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(15) && a.Ismanager.Equals(0));
                var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(15) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


                return new EmployeeTrainingResponse()
                {
                    IsSuccess = true,
                    HrAccess = hrAccess,
                    MgAccess = mgAccess,
                    EmpAccess = empAccess,
                    TrainingsForMe = trainings,
                    MyTrainings = organized,
                    TrainingForReportees = reportees
                };
            }

            throw new Exception("Employee details not found.");
        }

        public GetTaskFilterResponse GetEmployeeTaskFilter(BaseRequest request)
        {
            var employee = GetAll()
                .Include(var => var.TaskAddedByNavigation).ThenInclude(var => var.AssignedToNavigation).ThenInclude(var => var.EmployeeCompanyEmployee)
                .Include(var => var.TaskAssignedToNavigation).ThenInclude(var => var.AddedByNavigation).ThenInclude(var => var.EmployeeCompanyEmployee)
                .FirstOrDefault(var => var.IsActive && var.Id == request.UserIdNum);
            if (employee != null)
            {
                var createdEmployees = employee.TaskAssignedToNavigation
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeBaseInfoDto
                    {
                        EmployeeId = var.AssignedToNavigation.Guid,
                        EmployeeCode = var.AssignedToNavigation.EmployeeCompanyEmployee.FirstOrDefault()?.EmployeeCode,
                        EmployeeName = var.AssignedToNavigation.Name
                    })
                    .DistinctBy(var => var.EmployeeId)
                    .ToList();

                var assignedEmployees = employee.TaskAddedByNavigation
                    .Where(var => var.IsActive)
                    .Select(var => new EmployeeBaseInfoDto
                    {
                        EmployeeId = var.AssignedToNavigation.Guid,
                        EmployeeCode = var.AssignedToNavigation.EmployeeCompanyEmployee.FirstOrDefault()?.EmployeeCode,
                        EmployeeName = var.AssignedToNavigation.Name
                    })
                    .DistinctBy(var => var.EmployeeId)
                    .ToList();

                return new GetTaskFilterResponse
                {
                    IsSuccess = true,
                    AssignedTo = assignedEmployees,
                    CreatedBy = createdEmployees
                };
            }

            throw new Exception("Employee not found");
        }

        public BaseResponse HRRaisedResignation(HRResignationRequest request, ApplicationSettings appSettings)
        {
            var employee = GetAll()
             .Include(var => var.EmployeeExitEmployee)
             .Include(var => var.EmployeeBankEmployee)
             .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
             .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
             .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
             .Include(var => var.EmployeeContactEmployee).ThenInclude(var => var.PresentAddress)
             .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
             .ThenInclude(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
             .FirstOrDefault(var => var.Guid == request.EmployeeId && var.IsActive);

            var employeeName = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode + " - " + employee.Name;
            var manager = employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo;


            var employeeResignation = new EmployeeExit()
            {
                ResignationReason = request.ResignationReason,
                Status = "Exit-Processing",
                //ResignedOn = DateTime.UtcNow,
                ResignedOn = request.ResignedOn,
                PreferredRelievingDate = request.PreferredRelievingDate,
                RelievingDateAsPerPolicy = request.RelievingDateAsPerPolicy,
                CompanyId = request.CompanyIdNum,
                AddedOn = DateTime.UtcNow,
                AddedBy = request.UserIdNum,
                ManagerEmployee = request.UserIdNum,
                L1approvalFeedback = request.ResignationType,
                L1approvalFeedbackForOthers = request.ResignationType,
                UpdatedOnRM = DateTime.Now,
                SeniorManagerEmployee = request.UserIdNum,
                L2approvalFeedback = request.ResignationType,
                L2approvalFeedbackForOthers = request.ResignationType,
                UpdatedOnSM = DateTime.Now,
                HrEmployee = request.UserIdNum,
                HrapprovalFeedback = request.ResignationType,
                HrapprovalFeedbackForOthers = request.ResignationType,
                UpdatedOnHR = DateTime.Now,
                ActualRelievingDate = request.PreferredRelievingDate,
                EmployeeId  = employee.Id
                //confirmedrelivingdate = request.PreferredRelievingDate.ToString(),


            };

            employee.EmployeeExitEmployee.Add(employeeResignation);

            var employeeExit = employee.EmployeeExitEmployee.LastOrDefault(var => var.EmployeeId.Equals(employee.Id));

            if (request.Status == "Exit-Processing")
            {
                employeeExit.Status = "Exit-Processing";
                employeeExit.ConfirmedOn = request.PreferredRelievingDate;
                //confirmedrelivingdate = request.RelievingDate.ToString();
                var employeeAssets = (from employeeAsset in dbContext.EmployeeAsset
                                      where employeeAsset.EmployeeId == employeeExit.EmployeeId && employeeAsset.IsActive
                                      select employeeAsset).ToList();


                foreach (var employeeAsset in employeeAssets)
                {
                    var employeeExitAsset = new EmployeeExitAsset()
                    {
                        EmployeeAssetId = employeeAsset.Id,
                        EmployeeExitId = employeeExit.Id,
                        AssetTypeId = employeeAsset.AssetId,
                        Manager = employeeExit.ManagerEmployee.HasValue ? employeeExit.ManagerEmployee : null,
                        SeniorManager = employeeExit.SeniorManagerEmployee.HasValue ? employeeExit.SeniorManagerEmployee : null,
                        AddedOn = DateTime.UtcNow,
                        AddedBy = request.UserIdNum,
                        Status = "Pending"
                    };

                    employeeExit.EmployeeExitAsset.Add(employeeExitAsset);
                }

                //var otherAssetTypeIds = new long[] { 10, 11, 12, 13, 14, 15, 16, 22, 23, 24, 25, 26, 27, 28 };
                var otherAssetTypes = new string[] {
                   "IT - Disable - Domain Account / Accpac",
                   "Finance - Imprest / Travel Advance",
                   "Finance - Bills to be settled (Payable)",
                   "HR  - Recovery - Agreements/Training Bond",
                   "HR  - Recovery - Notice buyout/Relocation /Transfer / Initial acc.",
                   "HR  - Gratuity Settlement – Eligible?",
                   "HR  - ID card",
                   "HR  - Excess  Leave / Leave availed in notice period",
                   "HR  - Tax Declaration Proofs",
                   "HR  - Disable – LMS/HRMS/ESS-Payroll /K Success/ Email ID",
                   "HR  - Reimbursements – Tel, Conv, LTA, Meal  & Gift card",
                   "HOD /RM - Knowledge Transfer / Passwords Handover",
                   "HOD /RM - Intimate -  Departments/ Dealers /Customers"
                };


                foreach (var assetType in otherAssetTypes)
                {
                    var assetTypeId = (from at in dbContext.SettingsAssetTypes where at.AssetType == assetType select at.Id).FirstOrDefault();
                    var employeeExitAsset = new EmployeeExitAsset()
                    {

                        EmployeeExitId = employeeExit.Id,
                        AssetTypeId = assetTypeId,
                        Manager = employeeExit.ManagerEmployee.HasValue ? employeeExit.ManagerEmployee : null,
                        SeniorManager = employeeExit.SeniorManagerEmployee.HasValue ? employeeExit.SeniorManagerEmployee : null,
                        AddedOn = DateTime.UtcNow,
                        AddedBy = request.UserIdNum,
                        Status = "Pending"
                    };

                    employeeExit.EmployeeExitAsset.Add(employeeExitAsset);
                }

                //status = "Exit-Processing";

                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.ExitProcessingEmp,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = employee.Id,
                    NotificationData = JsonConvert.SerializeObject(
                                   new
                                   {
                                       userName = request.UserName
                                   })
                });


                if (manager != null)
                {
                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.ExitProcessingRM,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = manager.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                  new
                                  {
                                      empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                      userName = request.UserName
                                  })
                    });
                }

            }

            var employeeExitForm = new EmployeeExitForm()
            {
                ExitId = employeeExit.Id,
                EmployeeId = employeeExit.EmployeeId,
                //TenureInKai = request.EmployeeExitForm.TenureInKai,
                //TotalExperience = request.EmployeeExitForm.TotalExperience,
                LikeAboutKai = "Not Applicable",
                DislikeAboutKai = "Not Applicable",
                ThingsKaiMustChange = "Not Applicable",
                ThingsKaiMustContinue = "Not Applicable",
                WhatPromptedToChange = "Not Applicable",
                ReasonForLeavingKai = request.ResignationType,
                RejoinKaiLater = false,
                AssociateWhom = "Not Applicable",
                WhichOrganization = "Not Applicable",
                Designation = "Not Applicable",
                Ctc = "Not Applicable",
                EmailId = employee.EmployeeContactEmployee.FirstOrDefault()?.PersonalEmailId,
                MobileNumber = employee.EmployeeContactEmployee.FirstOrDefault()?.ContactNumber,
                AccountNo = employee.EmployeeBankEmployee.FirstOrDefault()?.BankAccountNumber,
                BankName = employee.EmployeeBankEmployee.FirstOrDefault()?.BankName,
                Ifsccode = employee.EmployeeBankEmployee.FirstOrDefault()?.IfscCode,
                Address = employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress != null ?
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.DoorNo + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.StreetName + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.Village + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.District + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.City + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.Pincode + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.State + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.Country
                               : "",
                AddedOn = DateTime.UtcNow,
                AddedBy = request.UserIdNum,
                UpdatedBy = request.UserIdNum,
                UpdatedOn = DateTime.Now
            };


            employeeExit.EmployeeExitForm.Add(employeeExitForm);



            Save();

            //EmailSender.SendExitRequestEmail(employeeName, manager.Name, "", "HR", "", manager.EmailId, seniorManager.EmailId, "suganthan.l@kubota.com;duraimurugan.n@kubota.com;neelima.c@kubota.com;vijayakumar.g@kubota.com", position, appSettings);

            return new BaseResponse
            {
                IsSuccess = true
            };
        }
        public BaseResponse InitiateEmployeeResignation(EmployeeResignationRequest request, ApplicationSettings appSettings)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeExitEmployee)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .ThenInclude(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .FirstOrDefault(var => var.Id == request.UserIdNum && var.IsActive);


            var employeeResignation = new EmployeeExit()
            {
                ResignationReason = request.ResignationReason,
                Status = "Pending",
                ResignedOn = DateTime.UtcNow,
                PreferredRelievingDate = request.PreferredRelievingDate,
                RelievingDateAsPerPolicy = request.RelievingDateAsPerPolicy,
                CompanyId = request.CompanyIdNum,
                AddedOn = DateTime.UtcNow,
                AddedBy = request.UserIdNum
            };

            employee.EmployeeExitEmployee.Add(employeeResignation);

            var employeeName = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode + '-' + employee.Name;
            var employeeEmailId = employee.EmailId;
            var manager = employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo;
            var seniorManager = employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo?.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo;
            var position = employee.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name;


            if (manager != null)
            {
                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.InitiateResgination,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = manager.Id,
                    NotificationData = JsonConvert.SerializeObject(
                                   new
                                   {
                                       userName = request.UserName
                                   })
                });
            }
            if (seniorManager != null)
            {
                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.InitiateResgination,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = seniorManager.Id,
                    NotificationData = JsonConvert.SerializeObject(
                                   new
                                   {
                                       userName = request.UserName
                                   })
                });
            }

            Save();

            EmailSender.SendExitRequestEmail(employeeName, manager.Name, seniorManager.Name, "HR", employeeEmailId, manager.EmailId, seniorManager.EmailId, "suganthan.l@kubota.com,duraimurugan.n@kubota.com,neelima.c@kubota.com,vijayakumar.g@kubota.com", position, appSettings);
            

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse UpdateEmployeeResignation(UpdateEmployeeExit request, ApplicationSettings appSettings)
        {
            var employee = GetAll()
                .Include(var => var.EmployeeExitEmployee)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .ThenInclude(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .FirstOrDefault(var => var.Guid == request.EmployeeId && var.IsActive);
            var employeeasset = GetAll()
                .Include(var => var.EmployeeAssetEmployee)
                .FirstOrDefault(var => var.Guid == request.EmployeeId);


            var employeeName = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode + " - " + employee.Name;
            var employeeEmailId = employee.EmailId;
            var manager = employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo;
            var seniorManager = employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo?.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo;
            var status = "";

            var employeeExit = employee.EmployeeExitEmployee.FirstOrDefault(var => var.Id.Equals(request.EmployeeExitId));
            var employeeresignationdate = employee.EmployeeExitEmployee.FirstOrDefault().ResignedOn;
            var position = employee.EmployeeCompanyEmployee.FirstOrDefault().Designation.Name;
            var confirmedrelivingdate = "";
            if (request.Status == "Approved")
            {
                if (employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId == request.UserIdNum)
                {
                    employeeExit.ManagerEmployee = request.UserIdNum;
                    employeeExit.L1approvalFeedback = request.Feedback;
                    employeeExit.L1approvalFeedbackForOthers = request.FeedbackForOthers;
                    employeeExit.Status = "L1-Approved";
                    employeeExit.UpdatedOnRM = DateTime.Now;

                    status = "L1-Approved";

                    if (seniorManager != null)
                    {
                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.RMApprovedResignation,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = seniorManager.Id,
                            NotificationData = JsonConvert.SerializeObject(
                                       new
                                       {
                                           empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                           userName = request.UserName
                                       })
                        });
                    }

                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.RMApprovedResignationToEmp,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = employee.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                       new
                                       {
                                           userName = request.UserName
                                       })
                    });
                }
                else if (employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo?.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId == request.UserIdNum)
                {
                    employeeExit.SeniorManagerEmployee = request.UserIdNum;
                    employeeExit.L2approvalFeedback = request.Feedback;
                    employeeExit.L2approvalFeedbackForOthers = request.FeedbackForOthers;
                    employeeExit.Status = "L2-Approved";
                    employeeExit.UpdatedOnSM = DateTime.Now;
                    status = "L2-Approved";

                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.L2ApprovedResignation,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = employee.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                       new
                                       {
                                           userName = request.UserName
                                       })
                    });

                    if (manager != null)
                    {
                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.L2ApprovedResginationToRM,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = manager.Id,
                            NotificationData = JsonConvert.SerializeObject(
                                       new
                                       {
                                           empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                           userName = request.UserName
                                       })
                        });
                    }

                }
                else if (employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != request.UserIdNum && employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo?.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != request.UserIdNum)
                {
                    var hrEmpolyee = GetAll()
                        .Include(var => var.Role)
                        .FirstOrDefault(var => var.Id == request.UserIdNum && var.IsActive);

                    if (hrEmpolyee.Role.RoleName == "HR")
                    {
                        employeeExit.HrEmployee = request.UserIdNum;
                        employeeExit.HrapprovalFeedback = request.Feedback;
                        employeeExit.HrapprovalFeedbackForOthers = request.FeedbackForOthers;
                        employeeExit.Status = "HR-Approved";
                        employeeExit.UpdatedOnHR = DateTime.Now;
                        employeeExit.ActualRelievingDate = request.RelievingDate;
                        status = "HR-Approved";
                        confirmedrelivingdate = request.RelievingDate.ToString();
                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.HrApprovedResgination,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = employee.Id,
                            NotificationData = JsonConvert.SerializeObject(
                                           new
                                           {
                                               userName = request.UserName
                                           })
                        });

                        if (seniorManager != null)
                        {
                            dbContext.Notification.Add(new Notification
                            {
                                Guid = CustomGuid.NewGuid(),
                                ActionId = NotificationTemplates.HrApprovedResginationToL2,
                                NotificationTime = DateTime.UtcNow,
                                IsActive = true,
                                IsRead = false,
                                NotificationTo = seniorManager.Id,
                                NotificationData = JsonConvert.SerializeObject(
                                          new
                                          {
                                              empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                              userName = request.UserName
                                          })
                            });
                        }

                        if (manager != null)
                        {
                            dbContext.Notification.Add(new Notification
                            {
                                Guid = CustomGuid.NewGuid(),
                                ActionId = NotificationTemplates.HrApprovedResginationToRM,
                                NotificationTime = DateTime.UtcNow,
                                IsActive = true,
                                IsRead = false,
                                NotificationTo = manager.Id,
                                NotificationData = JsonConvert.SerializeObject(
                                          new
                                          {
                                              empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                              userName = request.UserName
                                          })
                            });
                        }
                    }
                }
            }

            if (request.Status == "Rejected")
            {
                if (employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId == request.UserIdNum)
                {
                    employeeExit.ManagerEmployee = request.UserIdNum;
                    employeeExit.Feedback = request.Feedback;
                    employeeExit.FeedbackForOthers = request.FeedbackForOthers;
                    employeeExit.Status = "L1-Rejected";
                    status = "L1-Rejected";

                    if (seniorManager != null)
                    {
                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.RMRejectedResignation,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = seniorManager.Id,
                            NotificationData = JsonConvert.SerializeObject(
                                       new
                                       {
                                           empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                           userName = request.UserName
                                       })
                        });
                    }

                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.RMRejectedResignationToEmp,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = employee.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                       new
                                       {
                                           userName = request.UserName
                                       })
                    });
                }
                else if (employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo?.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId == request.UserIdNum)
                {
                    employeeExit.SeniorManagerEmployee = request.UserIdNum;
                    employeeExit.Feedback = request.Feedback;
                    employeeExit.FeedbackForOthers = request.FeedbackForOthers;
                    employeeExit.Status = "L2-Rejected";
                    status = "L2-Rejected";

                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.L2RejectedResignation,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = employee.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                       new
                                       {
                                           userName = request.UserName
                                       })
                    });

                    if (manager != null)
                    {
                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.L2RejectedResignationToRM,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = manager.Id,
                            NotificationData = JsonConvert.SerializeObject(
                                       new
                                       {
                                           empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                           userName = request.UserName
                                       })
                        });
                    }
                }
                else if (employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != request.UserIdNum && employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo?.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != request.UserIdNum)
                {
                    var hrEmpolyee = GetAll()
                     .Include(var => var.Role)
                      .FirstOrDefault(var => var.Id == request.UserIdNum && var.IsActive);

                    if (hrEmpolyee.Role.RoleName == "HR")
                    {
                        employeeExit.HrEmployee = request.UserIdNum;
                        employeeExit.Feedback = request.Feedback;
                        employeeExit.FeedbackForOthers = request.FeedbackForOthers;
                        employeeExit.Status = "HR-Rejected";

                        status = "HR-Rejected";

                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.HrRejectedResgination,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = employee.Id,
                            NotificationData = JsonConvert.SerializeObject(
                                           new
                                           {
                                               userName = request.UserName
                                           })
                        });

                        if (seniorManager != null)
                        {
                            dbContext.Notification.Add(new Notification
                            {
                                Guid = CustomGuid.NewGuid(),
                                ActionId = NotificationTemplates.HrRejectedResginationToL2,
                                NotificationTime = DateTime.UtcNow,
                                IsActive = true,
                                IsRead = false,
                                NotificationTo = seniorManager.Id,
                                NotificationData = JsonConvert.SerializeObject(
                                          new
                                          {
                                              empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                              userName = request.UserName
                                          })
                            });
                        }

                        if (manager != null)
                        {
                            dbContext.Notification.Add(new Notification
                            {
                                Guid = CustomGuid.NewGuid(),
                                ActionId = NotificationTemplates.HrRejectedResginationToRM,
                                NotificationTime = DateTime.UtcNow,
                                IsActive = true,
                                IsRead = false,
                                NotificationTo = manager.Id,
                                NotificationData = JsonConvert.SerializeObject(
                                          new
                                          {
                                              empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                              userName = request.UserName
                                          })
                            });
                        }
                    }
                }
            }

            if (request.Status == "Exit-Processing")
            {
                employeeExit.Status = "Exit-Processing";
                if (request.RelievingDate != null)
                {
                    employeeExit.ActualRelievingDate = request.RelievingDate;

                }
                confirmedrelivingdate = request.RelievingDate.ToString();
                var employeeAssets = (from employeeAsset in dbContext.EmployeeAsset
                                      where employeeAsset.EmployeeId == employeeExit.EmployeeId && employeeAsset.IsActive
                                      select employeeAsset).ToList();


                foreach (var employeeAsset in employeeAssets)
                {
                    var employeeExitAsset = new EmployeeExitAsset()
                    {
                        EmployeeAssetId = employeeAsset.Id,
                        EmployeeExitId = employeeExit.Id,
                        AssetTypeId = employeeAsset.AssetId,
                        Manager = employeeExit.ManagerEmployee.HasValue ? employeeExit.ManagerEmployee : null,
                        SeniorManager = employeeExit.SeniorManagerEmployee.HasValue ? employeeExit.SeniorManagerEmployee : null,
                        AddedOn = DateTime.UtcNow,
                        AddedBy = request.UserIdNum,
                        Status = "Pending"
                    };

                    employeeExit.EmployeeExitAsset.Add(employeeExitAsset);
                }

                //var otherAssetTypeIds = new long[] { 10, 11, 12, 13, 14, 15, 16, 22, 23, 24, 25, 26, 27, 28 };
                var otherAssetTypes = new string[] {
                   "IT - Disable - Domain Account / Accpac",
                   "Finance - Imprest / Travel Advance",
                   "Finance - Bills to be settled (Payable)",
                   "HR  - Recovery - Agreements/Training Bond",
                   "HR  - Recovery - Notice buyout/Relocation /Transfer / Initial acc.",
                   "HR  - Gratuity Settlement – Eligible?",
                   "HR  - ID card",
                   "HR  - Excess  Leave / Leave availed in notice period",
                   "HR  - Tax Declaration Proofs",
                   "HR  - Disable – LMS/HRMS/ESS-Payroll /K Success/ Email ID",
                   "HR  - Reimbursements – Tel, Conv, LTA, Meal  & Gift card",
                   "HOD /RM - Knowledge Transfer / Passwords Handover",
                   "HOD /RM - Intimate -  Departments/ Dealers /Customers"
                };


                foreach (var assetType in otherAssetTypes)
                {
                    var assetTypeId = (from at in dbContext.SettingsAssetTypes where at.AssetType == assetType select at.Id).FirstOrDefault();
                    var employeeExitAsset = new EmployeeExitAsset()
                    {

                        EmployeeExitId = employeeExit.Id,
                        AssetTypeId = assetTypeId,
                        Manager = employeeExit.ManagerEmployee.HasValue ? employeeExit.ManagerEmployee : null,
                        SeniorManager = employeeExit.SeniorManagerEmployee.HasValue ? employeeExit.SeniorManagerEmployee : null,
                        AddedOn = DateTime.UtcNow,
                        AddedBy = request.UserIdNum,
                        Status = "Pending"
                    };

                    employeeExit.EmployeeExitAsset.Add(employeeExitAsset);
                }

                status = "Exit-Processing";

                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.ExitProcessingEmp,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = employee.Id,
                    NotificationData = JsonConvert.SerializeObject(
                                   new
                                   {
                                       userName = request.UserName
                                   })
                });

                if (seniorManager != null)
                {
                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.ExitProcessingL2,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = seniorManager.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                  new
                                  {
                                      empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                      userName = request.UserName
                                  })
                    });
                }

                if (manager != null)
                {
                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.ExitProcessingRM,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = manager.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                  new
                                  {
                                      empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                      userName = request.UserName
                                  })
                    });
                }

            }

            if (request.Status == "Completed")
            {
                employeeExit.Status = "Completed";
                employeeExit.ClearanceComments = request.ClearanceComments;
                employeeExit.EligibleForRehire = request.EligibleForRehire;
                employee.CanLogin = false;
                employee.EmployeeCompanyEmployee.FirstOrDefault().IsResigned = true;
                employee.IsActive = false;
                employeeasset.IsActive = false;
                status = "Completed";

                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.ClearanceCompletedEmp,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = employee.Id,
                    NotificationData = JsonConvert.SerializeObject(
                                   new
                                   {
                                       userName = request.UserName
                                   })
                });

                if (seniorManager != null)
                {
                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.ClearanceCompletedL2,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = seniorManager.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                  new
                                  {
                                      empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                      userName = request.UserName
                                  })
                    });
                }

                if (manager != null)
                {
                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.ClearanceCompletedRM,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = manager.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                  new
                                  {
                                      empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                      userName = request.UserName
                                  })
                    });
                }
            }

            if (request.IsRevoked.HasValue && request.IsRevoked.Value)
            {
                employeeExit.IsRevoked = request.IsRevoked.Value;
                employeeExit.RevokedOn = DateTime.UtcNow;

                status = "Revoked";

                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.ResignationRevokedToEmp,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = employee.Id,
                    NotificationData = JsonConvert.SerializeObject(
                                   new
                                   {
                                       userName = request.UserName
                                   })
                });

                if (seniorManager != null)
                {
                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.ResignationRevokedToL2,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = seniorManager.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                  new
                                  {
                                      empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                      userName = request.UserName
                                  })
                    });
                }

                if (manager != null)
                {
                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.ResignationRevokedToRM,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = manager.Id,
                        NotificationData = JsonConvert.SerializeObject(
                                  new
                                  {
                                      empCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                      userName = request.UserName
                                  })
                    });
                }
            }

            employeeExit.UpdatedBy = request.UserIdNum;
            employeeExit.UpdatedOn = DateTime.UtcNow;

            Save();

            //Send email to L1,L2 and HR
            EmailSender.SendExitUpdateEmail(status, employeeName, manager.Name, seniorManager.Name, "HR", employeeEmailId, manager.EmailId, seniorManager.EmailId, "suganthan.l@kubota.com,duraimurugan.n@kubota.com,neelima.c@kubota.com,vijayakumar.g@kubota.com", employeeExit.Feedback, employeeExit.FeedbackForOthers, employeeresignationdate, position, confirmedrelivingdate, appSettings);





            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public EmployeeExitResponse GetEmployeeExits(BaseRequest request)
        {
            var employee = GetAll()
                          .Include(var => var.EmployeeExitEmployee)
                          .Include(var => var.EmployeeBankEmployee)
                          .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                          .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                          .Include(var => var.EmployeeContactEmployee).ThenInclude(var => var.PresentAddress)
                          .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo).ThenInclude(var => var.EmployeeCompanyEmployee)
                          .FirstOrDefault(var => var.Id == request.UserIdNum && var.IsActive);

            var employeeExits = employee.EmployeeExitEmployee
                         .Select(x => new EmployeeExitDto
                         {
                             ExitId = x.Id,
                             EmployeeId = employee.Guid,
                             EmployeeCode = employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                             EmployeeName = employee.Name,
                             Department = employee.EmployeeCompanyEmployee.FirstOrDefault().Department?.Description,
                             DateOfJoining = employee.EmployeeCompanyEmployee.FirstOrDefault().Doj,
                             EmployeeResignationReason = x.ResignationReason,
                             PreferredRelievingDate = x.PreferredRelievingDate,
                             ResignationDate = x.ResignedOn,
                             RelievingDate = x.ActualRelievingDate,
                             Feedback = x.Feedback,
                             ManangerName = x.ManagerEmployeeNavigation?.Name,
                             SeniorManangerName = x.SeniorManagerEmployeeNavigation?.Name,
                             HRName = x.HrEmployeeNavigation?.Name,
                             IsRevoked = x.IsRevoked,
                             Status = x.Status,
                             EmployeeHasCompanyAsset = (from employeeAsset in dbContext.EmployeeAsset where employeeAsset.EmployeeId == employee.Id select employeeAsset.Id).Any(),
                             IsAssetHandOverCompleted = (from exitAsset in dbContext.EmployeeExitAsset where exitAsset.Status == "Completed" select exitAsset.Id).Any(),
                             IsEmployeeExitFormSubmitted = (from exitForm in dbContext.EmployeeExitForm where exitForm.ExitId == x.Id select exitForm.Id).Any(),
                             IsHODFeedBackFormSubmitted = (from hodForm in dbContext.EmployeeExitHodfeedBackForm where hodForm.ExitId == x.Id select hodForm.Id).Any(),
                             IsHRFeedBackFormSubmitted = (from hrForm in dbContext.EmployeeExitHrfeedBackForm where hrForm.ExitId == x.Id select hrForm.Id).Any(),
                             L1ApprovalFeedback = x.L1approvalFeedback,
                             L2ApprovalFeedback = x.L2approvalFeedback,
                             HRApprovalFeedback = x.HrapprovalFeedback,
                             L1ApprovalFeedbackForOthers = x.L1approvalFeedbackForOthers,
                             L2ApprovalFeedbackForOthers = x.L2approvalFeedbackForOthers,
                             HRApprovalFeedbackForOthers = x.HrapprovalFeedbackForOthers,
                             FeedbackForOthers = x.FeedbackForOthers,
                             AccountNo = employee.EmployeeBankEmployee.FirstOrDefault()?.BankAccountNumber,
                             BankName = employee.EmployeeBankEmployee.FirstOrDefault()?.BankName,
                             IFSCCode = employee.EmployeeBankEmployee.FirstOrDefault()?.IfscCode,
                             RelievingDateAsPerPolicy = x.RelievingDateAsPerPolicy,
                             Address = employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress != null ?
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.DoorNo + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.StreetName + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.Village + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.District + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.City + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.Pincode + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.State + ", " +
                                employee.EmployeeContactEmployee.FirstOrDefault()?.PresentAddress.Country
                               : "",
                             ShortfallDays = x.ActualRelievingDate.HasValue ? (x.RelievingDateAsPerPolicy.Value - x.ActualRelievingDate.Value).TotalDays : 0
                         }).OrderByDescending(x => x.ExitId).ToList();

            if (employeeExits.Count == 0)
            {
                employee = GetAll()
                         .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                         .FirstOrDefault(var => var.Id == request.UserIdNum && var.IsActive);
                var _employeeExits = new EmployeeExitDto
                {
                    RelievingDateAsPerPolicy = GetRelievingDateAsPerPolicy(employee)
                };
                employeeExits.Add(_employeeExits);
            }


            var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
            var role = employee.RoleId;
            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));

            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(1));
            var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

            var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0));
            var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

            return new EmployeeExitResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                MgAccess = mgAccess,
                EmpAccess = empAccess,
                EmployeeExits = employeeExits
            };
        }

        private DateTime GetRelievingDateAsPerPolicy(Employee employee)
        {
            var dateAsPerPolicy = (from grade in dbContext.SettingsExitGrade where employee.EmployeeCompanyEmployee.FirstOrDefault().Grade.Grade.Contains(grade.Grade) select employee.EmployeeCompanyEmployee.FirstOrDefault().ProbationEndDate > DateTime.Now ? DateTime.Now.AddDays(grade.PreConfirmDays.Value - 1) : DateTime.Now.AddDays(grade.PostConfirmDays.Value - 1)).FirstOrDefault();

            //Check date falls on weekend / holiday
            dateAsPerPolicy = CheckDateForHolidays(dateAsPerPolicy, employee);
            return dateAsPerPolicy;
        }

        private DateTime CheckDateForHolidays(DateTime date, Employee employee)
        {
            var day = date.DayOfWeek;
            if ((day == DayOfWeek.Saturday) || (day == DayOfWeek.Sunday))
            {
                if (day == DayOfWeek.Saturday)
                {
                    date = date.AddDays(-1);
                }
                else
                {
                    date = date.AddDays(-2);
                }
            }

            var holidayDates = (from holidays in dbContext.SettingsHoliday where holidays.CompanyId == employee.EmployeeCompanyEmployee.FirstOrDefault().CompanyId select holidays.Date).ToList();
            if (holidayDates.Contains(Convert.ToDateTime(date.ToShortDateString())))
            {
                date = date.AddDays(-1);
                CheckDateForHolidays(date, employee);
            }
            else
            {
                return date;
            }
            return date;
        }

        public EmployeeExitResponse GetAllEmployeeExitRequest(BaseRequest request)
        {
            List<EmployeeExitDto> employeeExits = new List<EmployeeExitDto>();

            var userRole = GetAll()
                        .Where(var => var.IsActive && var.Id == request.UserIdNum)
                        .Select(x => x.Role.RoleName).FirstOrDefault();

            //var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();
            //var role = userRole.
            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(1));
            var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

            var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0));
            var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

            if (hrAccess == 0)
            {
                userRole = "Employee";
            }


            if (userRole == "Employee" && mgAccess == 1)
            {
                var managerIds = GetAll()
                          .Where(var => var.IsActive && var.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId == request.UserIdNum)
                          .Select(x => x.Id).ToList();

                managerIds.Add(request.UserIdNum);

                var managerExits = (from exit in dbContext.EmployeeExit
                                    join employee in dbContext.Employee on exit.EmployeeId equals employee.Id
                                    join company in dbContext.EmployeeCompany on employee.Id equals company.EmployeeId
                                    join manager in dbContext.Employee on company.ReportingToId equals manager.Id
                                    join sm in dbContext.EmployeeCompany on manager.Id equals sm.EmployeeId into senior
                                    from seniorManager in senior.DefaultIfEmpty()
                                    where managerIds.Contains(manager.Id)
                                    // && (exit.ActualRelievingDate != null ) || exit.ActualRelievingDate >= DateTime.Today
                                    select new EmployeeExitDto
                                    {
                                        ExitId = exit.Id,
                                        EmployeeId = employee.Guid,
                                        EmployeeName = employee.Name,
                                        EmployeeCode = company.EmployeeCode,
                                        Department = company.Department.Description,
                                        PreferredRelievingDate = exit.PreferredRelievingDate,
                                        DateOfJoining = company.Doj,
                                        ResignationDate = exit.ResignedOn,
                                        EmployeeResignationReason = exit.ResignationReason,
                                        ManangerId = manager.Id,
                                        ManangerName = manager.Name,
                                        SeniorManangerId = seniorManager.ReportingTo.Id,
                                        SeniorManangerName = seniorManager.ReportingTo.Name,
                                        HrId = exit.HrEmployeeNavigation.Id,
                                        HRName = exit.HrEmployeeNavigation.Name,
                                        Feedback = exit.Feedback,
                                        RelievingDate = exit.ActualRelievingDate,
                                        IsRevoked = exit.IsRevoked,
                                        Status = exit.Status,
                                        EmployeeHasCompanyAsset = (from employeeAsset in dbContext.EmployeeAsset where employeeAsset.EmployeeId == employee.Id select employeeAsset.Id).Any(),
                                        IsAssetHandOverCompleted = (from exitAsset in dbContext.EmployeeExitAsset where exitAsset.Status == "Completed" select exitAsset.Id).Any(),
                                        IsEmployeeExitFormSubmitted = (from exitForm in dbContext.EmployeeExitForm where exitForm.ExitId == exit.Id select exitForm.Id).Any(),
                                        IsHODFeedBackFormSubmitted = (from hodForm in dbContext.EmployeeExitHodfeedBackForm where hodForm.ExitId == exit.Id select hodForm.Id).Any(),
                                        IsHRFeedBackFormSubmitted = (from hrForm in dbContext.EmployeeExitHrfeedBackForm where hrForm.ExitId == exit.Id select hrForm.Id).Any(),
                                        L1ApprovalFeedback = exit.L1approvalFeedback,
                                        L2ApprovalFeedback = exit.L2approvalFeedback,
                                        HRApprovalFeedback = exit.HrapprovalFeedback,
                                        L1ApprovalFeedbackForOthers = exit.L1approvalFeedbackForOthers,
                                        L2ApprovalFeedbackForOthers = exit.L2approvalFeedbackForOthers,
                                        HRApprovalFeedbackForOthers = exit.HrapprovalFeedbackForOthers,
                                        FeedbackForOthers = exit.FeedbackForOthers,
                                        RelievingDateAsPerPolicy = exit.RelievingDateAsPerPolicy,
                                        ShortfallDays = exit.ActualRelievingDate.HasValue ? (exit.RelievingDateAsPerPolicy.Value - exit.ActualRelievingDate.Value).TotalDays : 0
                                    }).ToList();

                employeeExits = managerExits;
            }

            if (userRole == "HR" && hrAccess == 1)
            {
                var hrExits = (from exit in dbContext.EmployeeExit
                               join employee in dbContext.Employee on exit.EmployeeId equals employee.Id
                               join company in dbContext.EmployeeCompany on employee.Id equals company.EmployeeId
                               join manager in dbContext.Employee on company.ReportingToId equals manager.Id
                               join sm in dbContext.EmployeeCompany on manager.Id equals sm.EmployeeId into senior
                               from seniorManager in senior.DefaultIfEmpty()
                               where exit.Status != "Completed"
                               //where (exit.ActualRelievingDate != null )|| exit.ActualRelievingDate >= DateTime.Today
                               select new EmployeeExitDto
                               {
                                   ExitId = exit.Id,
                                   EmployeeId = employee.Guid,
                                   EmployeeName = employee.Name,
                                   EmployeeCode = company.EmployeeCode,
                                   Department = company.Department.Description,
                                   DateOfJoining = company.Doj,
                                   PreferredRelievingDate = exit.PreferredRelievingDate,
                                   ResignationDate = exit.ResignedOn,
                                   EmployeeResignationReason = exit.ResignationReason,
                                   ManangerId = manager.Id,
                                   ManangerName = manager.Name,
                                   SeniorManangerId = seniorManager.ReportingTo.Id,
                                   SeniorManangerName = seniorManager.ReportingTo.Name,
                                   HrId = exit.HrEmployeeNavigation.Id,
                                   HRName = exit.HrEmployeeNavigation.Name,
                                   Feedback = exit.Feedback,
                                   RelievingDate = exit.ActualRelievingDate,
                                   IsRevoked = exit.IsRevoked,
                                   Status = exit.Status,
                                   EmployeeHasCompanyAsset = (from employeeAsset in dbContext.EmployeeAsset where employeeAsset.EmployeeId == employee.Id select employeeAsset.Id).Any(),
                                   IsAssetHandOverCompleted = (from exitAsset in dbContext.EmployeeExitAsset where exitAsset.Status == "Completed" select exitAsset.Id).Any(),
                                   IsEmployeeExitFormSubmitted = (from exitForm in dbContext.EmployeeExitForm where exitForm.ExitId == exit.Id select exitForm.Id).Any(),
                                   IsHODFeedBackFormSubmitted = (from hodForm in dbContext.EmployeeExitHodfeedBackForm where hodForm.ExitId == exit.Id select hodForm.Id).Any(),
                                   IsHRFeedBackFormSubmitted = (from hrForm in dbContext.EmployeeExitHrfeedBackForm where hrForm.ExitId == exit.Id select hrForm.Id).Any(),
                                   L1ApprovalFeedback = exit.L1approvalFeedback,
                                   L2ApprovalFeedback = exit.L2approvalFeedback,
                                   HRApprovalFeedback = exit.HrapprovalFeedback,
                                   L1ApprovalFeedbackForOthers = exit.L1approvalFeedbackForOthers,
                                   L2ApprovalFeedbackForOthers = exit.L2approvalFeedbackForOthers,
                                   HRApprovalFeedbackForOthers = exit.HrapprovalFeedbackForOthers,
                                   FeedbackForOthers = exit.FeedbackForOthers,
                                   RelievingDateAsPerPolicy = exit.RelievingDateAsPerPolicy,
                                   UpdatedCode = exit.UpdatedBy != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exit.UpdatedBy select employeecompany.EmployeeCode).FirstOrDefault() : null,
                                   UpdatedName = exit.UpdatedBy != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exit.UpdatedBy select employeecompany.AddressingName).FirstOrDefault() : null,
                                   UpdatedOn = exit.UpdatedOn,
                                   AddedCode = (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exit.AddedBy select employeecompany.EmployeeCode).FirstOrDefault(),
                                   AddedName = (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exit.AddedBy select employeecompany.AddressingName).FirstOrDefault(),
                                   AddedOn = exit.AddedOn,
                                   UpdatedCodeRM = exit.ManagerEmployee != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exit.ManagerEmployee select employeecompany.EmployeeCode).FirstOrDefault() : null,
                                   UpdatedNameRM = exit.ManagerEmployee != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exit.ManagerEmployee select employeecompany.AddressingName).FirstOrDefault() : null,
                                   UpdatedOnRM = exit.UpdatedOnRM,
                                   UpdatedCodeSM = exit.SeniorManagerEmployee != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exit.SeniorManagerEmployee select employeecompany.EmployeeCode).FirstOrDefault() : null,
                                   UpdatedNameSM = exit.SeniorManagerEmployee != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exit.SeniorManagerEmployee select employeecompany.AddressingName).FirstOrDefault() : null,
                                   UpdatedOnSM = exit.UpdatedOnSM,
                                   UpdatedCodeHR = exit.HrEmployee != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exit.HrEmployee select employeecompany.EmployeeCode).FirstOrDefault() : null,
                                   UpdatedNameHR = exit.HrEmployee != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exit.HrEmployee select employeecompany.AddressingName).FirstOrDefault() : null,
                                   UpdatedOnHR = exit.UpdatedOnHR,

                                   ShortfallDays = exit.ActualRelievingDate.HasValue ? (exit.RelievingDateAsPerPolicy.Value - exit.ActualRelievingDate.Value).TotalDays : 0
                               }).ToList();

                employeeExits = hrExits;
            }

            return new EmployeeExitResponse
            {
                IsSuccess = true,
                EmpAccess = empAccess,
                HrAccess=hrAccess,
                MgAccess=mgAccess,
                EmployeeExits = employeeExits
            };
        }

        public BaseResponse CreateEmployeeExitForm(EmployeeExitFormRequest request)
        {
            var employeeExit = dbContext.EmployeeExit.FirstOrDefault(x => x.Id == request.EmployeeExitId);

            var employeeExitForm = new EmployeeExitForm()
            {
                ExitId = employeeExit.Id,
                EmployeeId = employeeExit.EmployeeId,
                TenureInKai = request.EmployeeExitForm.TenureInKai,
                TotalExperience = request.EmployeeExitForm.TotalExperience,
                LikeAboutKai = request.EmployeeExitForm.LikeAboutKai,
                DislikeAboutKai = request.EmployeeExitForm.DislikeAboutKai,
                ThingsKaiMustChange = request.EmployeeExitForm.ThingsKaiMustChange,
                ThingsKaiMustContinue = request.EmployeeExitForm.ThingsKaiMustContinue,
                WhatPromptedToChange = request.EmployeeExitForm.WhatPromptedToChange,
                ReasonForLeavingKai = request.EmployeeExitForm.ReasonForLeavingKai,
                RejoinKaiLater = request.EmployeeExitForm.RejoinKaiLater,
                AssociateWhom = request.EmployeeExitForm.AssociateWhom,
                WhichOrganization = request.EmployeeExitForm.WhichOrganization,
                Designation = request.EmployeeExitForm.Designation,
                Ctc = request.EmployeeExitForm.Ctc,
                EmailId = request.EmployeeExitForm.EmailId,
                MobileNumber = request.EmployeeExitForm.MobileNumber,
                AccountNo = request.EmployeeExitForm.AccountNo,
                BankName = request.EmployeeExitForm.BankName,
                Ifsccode = request.EmployeeExitForm.IFSCCode,
                Address = request.EmployeeExitForm.Address,
                AddedOn = DateTime.UtcNow,
                AddedBy = request.UserIdNum,
                UpdatedBy = request.UserIdNum,
                UpdatedOn = DateTime.Now
            };


            employeeExit.EmployeeExitForm.Add(employeeExitForm);

            Save();

            //Send email to L1,L2 and HR

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse CreateHODFeedBackForm(HODFeedBackFormRequest request)
        {
            var employeeExit = dbContext.EmployeeExit.FirstOrDefault(x => x.Id == request.EmployeeExitId);

            var hodFeedBackForm = new EmployeeExitHodfeedBackForm()
            {
                ExitId = employeeExit.Id,
                EmployeeId = employeeExit.EmployeeId,
                IsDesiredAttrition = request.HodFeedBackForm.IsDesiredAttrition,
                IntentionToLeaveKai = request.HodFeedBackForm.IntentionToLeaveKai,
                AttemptsToRetainEmployee = request.HodFeedBackForm.AttemptsToRetainEmployee,
                EligibleToRehire = request.HodFeedBackForm.EligibleToRehire,
                Comments = request.HodFeedBackForm.Comments,
                AddedOn = DateTime.UtcNow,
                AddedBy = request.UserIdNum,
                UpdatedBy = request.UserIdNum,
                UpdatedOn = DateTime.Now
            };

            employeeExit.EmployeeExitHodfeedBackForm.Add(hodFeedBackForm);

            Save();

            //Send email to L1,L2 and HR

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse CreateHRFeedBackForm(HRFeedBackFormRequest request)
        {
            var employeeExit = dbContext.EmployeeExit.FirstOrDefault(x => x.Id == request.EmployeeExitId);

            var hrFeedBackForm = new EmployeeExitHrfeedBackForm()
            {
                ExitId = employeeExit.Id,
                EmployeeId = employeeExit.EmployeeId,
                EmployeeThoughtOnKai = request.HrFeedBackForm.EmployeeThoughtOnKai,
                EmployeeLikeToChange = request.HrFeedBackForm.EmployeeLikeToChange,
                EmployeeRejoinLater = request.HrFeedBackForm.EmployeeRejoinLater,
                SalaryAndDesignationOffered = request.HrFeedBackForm.SalaryAndDesignationOffered,
                Comments = request.HrFeedBackForm.Comments,
                AddedOn = DateTime.UtcNow,
                AddedBy = request.UserIdNum,
                UpdatedBy = request.UserIdNum,
                UpdatedOn = DateTime.Now

            };

            employeeExit.EmployeeExitHrfeedBackForm.Add(hrFeedBackForm);

            Save();

            //Send email to L1,L2 and HR

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public EmployeeExitFormResponse GetEmployeeExitForm(ExitFormRequest request)
        {
            var employeeExitForm = (from exitform in dbContext.EmployeeExitForm
                                    where exitform.ExitId == request.EmployeeExitId
                                    select new EmployeeExitFormReponseDto
                                    {
                                        Id = exitform.Id,
                                        ExitId = exitform.ExitId,
                                        EmployeeName = exitform.Employee.Name,
                                        EmployeeCode = exitform.Employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                        Department = exitform.Employee.EmployeeCompanyEmployee.FirstOrDefault().Department.Description,
                                        TenureInKai = exitform.TenureInKai,
                                        LikeAboutKai = exitform.LikeAboutKai,
                                        TotalExperience = exitform.TotalExperience,
                                        DislikeAboutKai = exitform.DislikeAboutKai,
                                        ThingsKaiMustChange = exitform.ThingsKaiMustChange,
                                        ThingsKaiMustContinue = exitform.ThingsKaiMustContinue,
                                        WhatPromptedToChange = exitform.WhatPromptedToChange,
                                        ReasonForLeavingKai = exitform.ReasonForLeavingKai,
                                        RejoinKaiLater = exitform.RejoinKaiLater,
                                        AssociateWhom = exitform.AssociateWhom,
                                        WhichOrganization = exitform.WhichOrganization,
                                        Designation = exitform.Designation,
                                        Ctc = exitform.Ctc,
                                        EmailId = exitform.EmailId,
                                        MobileNumber = exitform.MobileNumber,
                                        AccountNo = exitform.AccountNo,
                                        BankName = exitform.BankName,
                                        IFSCCode = exitform.Ifsccode,
                                        Address = exitform.Address,
                                        UpdatedBy = exitform.UpdatedBy,
                                        UpdatedCode = exitform.UpdatedBy != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exitform.UpdatedBy select employeecompany.EmployeeCode).FirstOrDefault() : null,
                                        UpdatedName = exitform.UpdatedBy != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == exitform.UpdatedBy select employeecompany.AddressingName).FirstOrDefault() : null,
                                        UpdatedOn = exitform.UpdatedOn
                                    }).FirstOrDefault();



            return new EmployeeExitFormResponse
            {
                IsSuccess = true,
                EmployeeExitForm = employeeExitForm
            };
        }

        public HODFeedBackFormResponse GetHODFeedBackForm(ExitFormRequest request)
        {
            var hodformResult = (from hodform in dbContext.EmployeeExitHodfeedBackForm
                                 where hodform.ExitId == request.EmployeeExitId
                                 select new HODFeedBackFormResponseDto
                                 {
                                     Id = hodform.Id,
                                     ExitId = hodform.ExitId,
                                     EmployeeName = hodform.Employee.Name,
                                     EmployeeCode = hodform.Employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                     IsDesiredAttrition = hodform.IsDesiredAttrition,
                                     IntentionToLeaveKai = hodform.IntentionToLeaveKai,
                                     AttemptsToRetainEmployee = hodform.AttemptsToRetainEmployee,
                                     EligibleToRehire = hodform.EligibleToRehire,
                                     Comments = hodform.Comments,
                                     UpdatedBy = hodform.UpdatedBy,
                                     UpdatedCode = hodform.UpdatedBy != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == hodform.UpdatedBy select employeecompany.EmployeeCode).FirstOrDefault() : null,
                                     UpdatedName = hodform.UpdatedBy != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == hodform.UpdatedBy select employeecompany.AddressingName).FirstOrDefault() : null,
                                     UpdatedOn = hodform.UpdatedOn
                                 }).FirstOrDefault();

            return new HODFeedBackFormResponse
            {
                IsSuccess = true,
                HODFeedBackForm = hodformResult
            };
        }

        public HRFeedBackFormResponse GetHRFeedBackForm(ExitFormRequest request)
        {
            var HrFormResult = (from hrform in dbContext.EmployeeExitHrfeedBackForm
                                where hrform.ExitId == request.EmployeeExitId
                                select new HRFeedBackFormResponseDto
                                {
                                    Id = hrform.Id,
                                    ExitId = hrform.ExitId,
                                    EmployeeName = hrform.Employee.Name,
                                    EmployeeCode = hrform.Employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                    Department = hrform.Employee.EmployeeCompanyEmployee.FirstOrDefault().Department.Description,
                                    DateOfJoining = hrform.Employee.EmployeeCompanyEmployee.FirstOrDefault().Doj,
                                    DateOfResignation = hrform.Exit.ResignedOn,
                                    DateOfRelieving = hrform.Exit.ActualRelievingDate,
                                    EmployeeThoughtOnKai = hrform.EmployeeThoughtOnKai,
                                    EmployeeLikeToChange = hrform.EmployeeLikeToChange,
                                    EmployeeRejoinLater = hrform.EmployeeRejoinLater,
                                    SalaryAndDesignationOffered = hrform.SalaryAndDesignationOffered,
                                    Comments = hrform.Comments,
                                    UpdatedBy = hrform.UpdatedBy,
                                    UpdatedCode = hrform.UpdatedBy != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == hrform.UpdatedBy select employeecompany.EmployeeCode).FirstOrDefault() : null,
                                    UpdatedName = hrform.UpdatedBy != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == hrform.UpdatedBy select employeecompany.AddressingName).FirstOrDefault() : null,
                                    UpdatedOn = hrform.UpdatedOn
                                }).FirstOrDefault();

            return new HRFeedBackFormResponse
            {
                IsSuccess = true,
                HRFeedBackForm = HrFormResult
            };
        }

        public EmployeeExitAssetResponse GetAllEmployeeExitWithAssets(BaseRequest request)
        {

            var userRole = GetAll()
                     .Where(var => var.IsActive && var.Id == request.UserIdNum)
                     .Select(x => x.Role.RoleName).FirstOrDefault();

            List<EmployeeExitAssetDto> employeeExitAssets = new List<EmployeeExitAssetDto>();

            var isLoggedInEmpResigned = (from exit in dbContext.EmployeeExit
                                         where exit.Status == "Exit-Processing" && !exit.IsRevoked && request.UserIdNum == exit.Employee.Id
                                         select exit.Id).Any();

            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(1));
            var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

            var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0));
            var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(17) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;

            if (hrAccess == 0)
            {
                userRole = "Employee";
            }

            if (isLoggedInEmpResigned)
            {
                employeeExitAssets = (from exit in dbContext.EmployeeExit
                                      join employeeAsset in dbContext.EmployeeExitAsset on exit.Id equals employeeAsset.EmployeeExitId
                                      where exit.Status == "Exit-Processing" && !exit.IsRevoked && exit.Employee.Id == request.UserIdNum
                                      select new EmployeeExitAssetDto
                                      {
                                          EmpoyeeExitId = exit.Id,
                                          EmployeeId = exit.EmployeeId,
                                          EmployeeCode = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                          EmployeeName = exit.Employee.Name,
                                          Department = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                                          RelievingDate = exit.ActualRelievingDate.Value,
                                      }).Distinct().ToList();
            }
            else if (userRole == "Employee" && mgAccess == 1)
            {
                var employeeExitWithAssetForManager = (from exit in dbContext.EmployeeExit
                                                       join employeeAsset in dbContext.EmployeeExitAsset on exit.Id equals employeeAsset.EmployeeExitId
                                                       where employeeAsset.Manager == request.UserIdNum && exit.Status == "Exit-Processing" && !exit.IsRevoked
                                                       select new EmployeeExitAssetDto
                                                       {
                                                           EmpoyeeExitId = exit.Id,
                                                           EmployeeId = exit.EmployeeId,
                                                           EmployeeCode = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                                           EmployeeName = exit.Employee.Name,
                                                           Department = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                                                           RelievingDate = exit.ActualRelievingDate.Value,
                                                       }).Distinct().ToList();

                var employeeExitWithAssetForSeniorManager = (from exit in dbContext.EmployeeExit
                                                             join employeeAsset in dbContext.EmployeeExitAsset on exit.Id equals employeeAsset.EmployeeExitId
                                                             where employeeAsset.SeniorManager == request.UserIdNum && exit.Status == "Exit-Processing" && !exit.IsRevoked
                                                             select new EmployeeExitAssetDto
                                                             {
                                                                 EmpoyeeExitId = exit.Id,
                                                                 EmployeeId = exit.EmployeeId,
                                                                 EmployeeCode = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                                                 EmployeeName = exit.Employee.Name,
                                                                 Department = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                                                                 RelievingDate = exit.ActualRelievingDate.Value,
                                                             }).Distinct().ToList();

                employeeExitWithAssetForManager.AddRange(employeeExitWithAssetForSeniorManager);
                employeeExitAssets.AddRange(employeeExitWithAssetForManager);

                var assetTypeIds = (from assetOwner in dbContext.SettingsAssetTypeOwner
                                    where assetOwner.OwnerId == request.UserIdNum
                                    select assetOwner.AssetTypeId).ToList();

                if (assetTypeIds != null && assetTypeIds.Count() > 0)
                {
                    var employeeExitWithAsseForOwner = (from exit in dbContext.EmployeeExit
                                                        join employeeAsset in dbContext.EmployeeExitAsset on exit.Id equals employeeAsset.EmployeeExitId
                                                        where exit.Status == "Exit-Processing" && !exit.IsRevoked && assetTypeIds.Contains(employeeAsset.AssetTypeId)
                                                        //|| assetTypeIds.Contains(employeeAsset.AssetTypeId) 
                                                        select new EmployeeExitAssetDto
                                                        {
                                                            EmpoyeeExitId = exit.Id,
                                                            EmployeeId = exit.EmployeeId,
                                                            EmployeeCode = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                                            EmployeeName = exit.Employee.Name,
                                                            Department = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                                                            RelievingDate = exit.ActualRelievingDate.Value,
                                                        }).Distinct().ToList();

                    employeeExitAssets.AddRange(employeeExitWithAsseForOwner);
                }


            }
            else if (userRole == "HR" && hrAccess == 1)
            {
                employeeExitAssets = (from exit in dbContext.EmployeeExit
                                      join employeeAsset in dbContext.EmployeeExitAsset on exit.Id equals employeeAsset.EmployeeExitId
                                      where exit.Status == "Exit-Processing" && !exit.IsRevoked
                                      select new EmployeeExitAssetDto
                                      {
                                          EmpoyeeExitId = exit.Id,
                                          EmployeeId = exit.EmployeeId,
                                          EmployeeCode = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                          EmployeeName = exit.Employee.Name,
                                          Department = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().Department.Name,
                                          RelievingDate = exit.ActualRelievingDate.Value,
                                      }).Distinct().ToList();

            }

            employeeExitAssets = employeeExitAssets.GroupBy(x => x.EmployeeId).Select(x => x.First()).ToList();

            return new EmployeeExitAssetResponse
            {
                IsSuccess = true,
                MgAccess=1,
                HrAccess=1,
                EmpAccess =1,
                EmployeeExitAssets = employeeExitAssets
            };
        }

        public EmployeeExitAssetDetailsResponse GetEmployeeExitAssetDetails(ExitFormRequest request)
        {
            //var defaulltRMHODAssetTypeIds = new long[] { 27, 28 };
            var otherAssetTypes = new string[] {
                    "HOD /RM - Knowledge Transfer / Passwords Handover",
                    "HOD /RM - Intimate -  Departments/ Dealers /Customers"
                };

            var employeeExitWithAsset = (from exit in dbContext.EmployeeExit
                                         join employeeAsset in dbContext.EmployeeExitAsset on exit.Id equals employeeAsset.EmployeeExitId
                                         join asset in dbContext.SettingsAssetTypes on employeeAsset.AssetTypeId equals asset.Id
                                         join employee in dbContext.Employee on exit.EmployeeId equals employee.Id
                                         join company in dbContext.EmployeeCompany on employee.Id equals company.EmployeeId
                                         join m in dbContext.Employee on company.ReportingToId equals m.Id into manag
                                         from manager in manag.DefaultIfEmpty()
                                         join smc in dbContext.EmployeeCompany on manager.Id equals smc.EmployeeId into seniorcompany
                                         from seniorManagerCompany in seniorcompany.DefaultIfEmpty()
                                         join sm in dbContext.Employee on seniorManagerCompany.ReportingToId equals sm.Id into seniormanag
                                         from seniorManager in seniormanag.DefaultIfEmpty()
                                         where exit.Id == request.EmployeeExitId && exit.Status == "Exit-Processing" && !exit.IsRevoked
                                         select new EmployeeExitAssetDetailsDto
                                         {
                                             EmployeeAssetId = employeeAsset.Id,
                                             EmployeeCode = exit.Employee.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode,
                                             EmployeeName = exit.Employee.Name,
                                             LoggedInUserAssetOwner = (from owner in dbContext.SettingsAssetTypeOwner where owner.Owner.Id == request.UserIdNum && owner.AssetTypeId == employeeAsset.AssetTypeId select owner.Id).Any(),
                                             Manager = manager.Guid,
                                             SeniorManager = seniorManager.Guid,
                                             AssetType = asset.AssetType,
                                             AssetUniqueId = employeeAsset.EmployeeAsset.AssetUniqueId,
                                             AssetBreakageFee = employeeAsset.AssetBreakageFee,
                                             Status = employeeAsset.Status,
                                             IsDefaultRMHODAssets = otherAssetTypes.Contains(asset.AssetType),
                                             Comments = employeeAsset.Comments,
                                             HodComments = employeeAsset.Hodcomments,
                                             AssetOwner = employeeAsset.AssetOwner,
                                             AddedBy = employeeAsset.AddedBy,
                                             AddedOn = employeeAsset.AddedOn,
                                             UpdatedBy = employeeAsset.UpdatedBy,
                                             UpdatedCode = employeeAsset.UpdatedBy != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == employeeAsset.UpdatedBy select employeecompany.EmployeeCode).FirstOrDefault() : null,
                                             UpdatedName = employeeAsset.UpdatedBy != null ? (from employeecompany in dbContext.EmployeeCompany where employeecompany.EmployeeId == employeeAsset.UpdatedBy select employeecompany.AddressingName).FirstOrDefault() : null,
                                             UpdatedOn = employeeAsset.UpdatedOn

                                         }).Distinct().OrderBy(x => x.AssetType).ThenBy(x => x.AssetUniqueId).ToList();


            return new EmployeeExitAssetDetailsResponse
            {
                IsSuccess = true,
                EmployeeExitAssetDetails = employeeExitWithAsset
            };
        }


        public BaseResponse UpdateEmployeeExitAssetDetails(UpdateEmployeeExitAsset request)
        {
            var exitAssetResult = (from exitAsset in dbContext.EmployeeExitAsset
                                   where exitAsset.Id == request.EmployeeExitAssetId
                                   select exitAsset).FirstOrDefault();


            if (request.Status == "Completed")
            {
                exitAssetResult.Status = request.Status;
                exitAssetResult.AssetOwner = request.UserIdNum;

                if (request.BreakageFee.HasValue)
                {
                    exitAssetResult.AssetBreakageFee = request.BreakageFee;
                }
                exitAssetResult.Comments = request.comments;
                exitAssetResult.Hodcomments = request.hodComments;
                exitAssetResult.UpdatedBy = request.UserIdNum;
                exitAssetResult.UpdatedOn = DateTime.Now;
            }

            if (request.Status == "Owner-Approved")
            {
                exitAssetResult.Status = request.Status;
                exitAssetResult.AssetOwner = request.UserIdNum;

                if (request.BreakageFee.HasValue)
                {
                    exitAssetResult.AssetBreakageFee = request.BreakageFee;
                }

                exitAssetResult.Comments = request.comments;
                exitAssetResult.Hodcomments = request.hodComments;
            }

            if (request.Status == "Approved")
            {
                if (exitAssetResult.Manager == request.UserIdNum)
                {
                    if (exitAssetResult.SeniorManager.HasValue)
                    {
                        exitAssetResult.Status = "Completed";
                        exitAssetResult.UpdatedBy = request.UserIdNum;
                        exitAssetResult.UpdatedOn = DateTime.Now;
                        //exitAssetResult.Status = "L1-Approved";
                        //exitAssetResult.AddedBy = request.UserIdNum;
                        //exitAssetResult.AddedOn = DateTime.Now;
                    }
                    else
                    {
                        exitAssetResult.Status = "Completed";
                    }
                }

                else if (exitAssetResult.SeniorManager == request.UserIdNum)
                {
                    exitAssetResult.Status = "Completed";
                    exitAssetResult.UpdatedBy = request.UserIdNum;
                    exitAssetResult.UpdatedOn = DateTime.Now;
                }

                if (request.BreakageFee.HasValue)
                {
                    exitAssetResult.AssetBreakageFee = request.BreakageFee;
                }

                exitAssetResult.Comments = request.comments;
                exitAssetResult.Hodcomments = request.hodComments;
            }

            Save();

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        //public SettingsMoodResponse GetAllMoods()
        //{

        //    var Moodinfo = (from mood in dbContext.SettingsMood
        //                    select new SettingsMoodDto
        //                    {
        //                        isActive = mood.IsActive,
        //                        mood = mood.Mood,
        //                        id = mood.Id

        //                    }).ToList();
        //    return new SettingsMoodResponse
        //    {
        //        IsSuccess = true,
        //        SettingsMoods = Moodinfo
        //    };
        //}
        //public SettingsMoodTagsResponse GetAllMoodTags(MoodTags request)
        //{

        //    var Moodtaginfo = (from moodtag in dbContext.SettingsMoodTags
        //                    join mood in dbContext.SettingsMood on moodtag.MoodId equals mood.Id
        //                    where mood.Id == request.MoodId
        //                    select new SettingsMoodTagsDto
        //                    {
        //                        Tag = moodtag.Tag,
        //                        mood = mood.Mood,
        //                        description = moodtag.Description,
        //                        isActive = mood.IsActive
        //                    }).ToList();
        //    return new SettingsMoodTagsResponse
        //    {
        //        IsSuccess = true,
        //        SettingsMoodTags = Moodtaginfo
        //    };
        //}
        //public SettingsMoodTagsResponse UpdateMoodTags(MoodTags request)
        //{

        //    if (string.IsNullOrWhiteSpace(request.Tags))
        //    {
        //        var newMoodTag = new SettingsMoodTags
        //        {
        //            Guid = CustomGuid.NewGuid(),
        //            AddedBy = request.UserIdNum,
        //            AddedOn = DateTime.UtcNow,
        //            Description = request.Description,
        //            IsActive = true,
        //            Tag = request.Tags,
        //            MoodId = request.MoodId
        //        };
        //    }
        //    else
        //    {
        //        var addMoodTags = dbContext.SettingsMoodTags.FirstOrDefault(var => var.Id.Equals(request.TagId));
        //        if (addMoodTags != null)
        //        {
        //            addMoodTags.UpdatedBy = request.UserIdNum;
        //            addMoodTags.UpdatedOn = DateTime.UtcNow;
        //            addMoodTags.Description = request.Description;
        //            addMoodTags.IsActive = request.IsActive;

        //        }
        //    }
        //    Save();

        //    return new SettingsMoodTagsResponse
        //    {
        //        IsSuccess = true
        //    };
        //}
    }
}
