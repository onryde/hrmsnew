using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Data.Interfaces;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Hrms.Helper.Settings;
using Hrms.Helper.StaticClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Hrms.Data.Repositories
{
    public class AppraisalRepo : BaseRepository<Appraisal>, IAppraisalRepo
    {
        private IEventLogRepo _eventLogRepo;
        public AppraisalRepo(HrmsContext context, IEventLogRepo eventLogRepo) : base(context)
        {
            _eventLogRepo = eventLogRepo;
        }

        public GetAppraisalsResponse GetAllAppraisals(AppraisalFilterRequest request)
        {
            var appraisals = GetAll()
                .Include(var => var.AppraisalQuestion)
                .ThenInclude(var => var.Question)
                .Include(var => var.AppraisalGrade)
                .ThenInclude(var => var.Grade)
                .Where(var => var.IsActive
                && (string.IsNullOrWhiteSpace(request.Title) || var.Title.Contains(request.Title))
                && (request.StartDate == null || request.EndDate == null || (var.StartDate >= request.StartDate && var.StartDate <= request.EndDate) || (var.EndDate >= request.StartDate && var.EndDate <= request.EndDate) || (var.StartDate < request.StartDate && var.EndDate > request.EndDate))
                && (request.GradeIds == null || !request.GradeIds.Any() || var.AppraisalGrade == null || !var.AppraisalGrade.Any() || var.AppraisalGrade.Any(var1 => request.GradeIds.Contains(var1.Grade.Guid)))
                )
                .Select(var => new AppraisalDto
                {
                    Description = var.Description,
                    Grades = var.AppraisalGrade.Select(var1 => var1.Grade.Guid).ToList(),
                    Title = var.Title,
                    AppraisalId = var.Guid,
                    EndDate = var.EndDate,
                    IsActive = var.IsActive,
                    IsCurrent = var.IsOpen,
                    ShowCalculation = var.ShowCalculation ?? false,
                    StartDate = var.StartDate,
                    IsLive = var.IsLive ?? false,
                    Category = var.Category,
                    Mode = var.Mode == 1 ? "objective" : var.Mode == 2 ? "variablebonus" : "appraisal",
                    CalculationMethod = var.CalculationMethod,
                    EligibleFrom = var.EligibleFrom,
                    EligibleTo = var.EligibleTo,
                    Year = var.Year,
                    Questions = var.AppraisalQuestion.Where(var1 => var1.IsActive)
                        .Select(var1 => new UpdateAppraisalQuestionDto
                        {
                            Percentage = var1.Percentage,
                            QuestionId = var1.Question.Guid
                        })
                        .ToList()
                })
                .ToList();


            //var role = employee.RoleId;
            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(12) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllAppraisals.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllAppraisals.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new GetAppraisalsResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                Appraisals = appraisals
            };
        }

        public BaseResponse UpdateAppraisal(UpdateAppraisalRequest request)
        {
            var guid = request.AppraisalId;
            if (request.GradeIds == null)
            {
                request.GradeIds = new List<string>();
            }

            var isPresent =
                GetAll()
                    .Include(var => var.AppraisalGrade)
                    .ThenInclude(var => var.Grade)
                    .Any(var => var.IsActive
                                && var.Guid != request.AppraisalId
                                && (
                                    ((!request.GradeIds.Any() && request.StartDate <= var.EndDate &&
                                     var.StartDate <= request.EndDate) && var.Year == request.Year)
                                    ||
                                    (((var.AppraisalGrade == null || !var.AppraisalGrade.Any()) &&
                                     request.StartDate <= var.EndDate && var.StartDate <= request.EndDate) && var.Year == request.Year)
                                    ||
                                    ((var.AppraisalGrade.Any(var1 => request.GradeIds.Contains(var1.Grade.Guid))
                                     && request.StartDate <= var.EndDate && var.StartDate <= request.EndDate
                                    ) && var.Year == request.Year)
                                )
                    );

            if (isPresent)
            {
                return new GetAppraisalsResponse
                {
                    IsSuccess = true,
                    IsGradeMismatch = true
                };
            }

            if (string.IsNullOrWhiteSpace(request.AppraisalId))
            {
                var questions = new List<AppraisalQuestion>();
                if (request.Questions != null && request.Questions.Any())
                {
                    foreach (var question in request.Questions)
                    {
                        var addedQuestion = dbContext.SettingsAppraisalQuestion
                            .FirstOrDefault(var => var.IsActive && var.Guid.Equals(question.QuestionId));
                        if (addedQuestion != null)
                        {
                            questions.Add(new AppraisalQuestion
                            {
                                Guid = CustomGuid.NewGuid(),
                                Percentage = question.Percentage,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.UtcNow,
                                IsActive = true,
                                Question = addedQuestion
                            });
                        }
                        else
                        {
                            throw new Exception("Question not found.");
                        }
                    }
                }

                var grades = new List<long>();
                if (request.GradeIds != null && request.GradeIds.Count > 0)
                {
                    grades = dbContext.SettingsGrade.Where(var =>
                            var.IsActive && var.CompanyId == request.CompanyIdNum && request.GradeIds.Contains(var.Guid))
                        .Select(var => var.Id)
                        .ToList();
                }

                guid = CustomGuid.NewGuid();
                var newAppraisal = new Appraisal
                {
                    CompanyId = request.CompanyIdNum,
                    Description = request.Description,
                    Guid = guid,
                    Title = request.Title,
                    AddedBy = request.UserIdNum,
                    AddedOn = DateTime.UtcNow,
                    EndDate = request.EndDate,
                    IsActive = true,
                    IsOpen = true,
                    StartDate = request.StartDate,
                    ShowCalculation = request.ShowCalculation,
                    IsLive = request.IsLive,
                    AppraisalGrade = grades.Select(var => new AppraisalGrade
                    {
                        GradeId = var
                    }).ToList(),
                    AppraisalQuestion = questions,
                    Category = request.Category,
                    Mode = request.Mode == "objective" ? 1 : request.Mode == "variablebonus" ? 2 : 3,
                    CalculationMethod = request.CalculationMethod,
                    EligibleFrom = request.EligibleFrom,
                    EligibleTo = request.EligibleTo,
                    Year = request.Year
                };

                if (request.IsLive)
                {
                    var employees = dbContext.Employee
                        .Include(var => var.EmployeeCompanyEmployee)
                        .ThenInclude(var => var.Grade)
                        .Where(var => var.IsActive
                                      && var.EmployeeCompanyEmployee.Any()
                                      && (!request.GradeIds.Any() ||
                                          grades.Contains(var.EmployeeCompanyEmployee.FirstOrDefault().GradeId ?? 0))
                                      && (var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned == null ||
                                        (var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned.HasValue && !var.EmployeeCompanyEmployee.FirstOrDefault().IsResigned.Value)));

                    foreach (var employee in employees)
                    {
                        newAppraisal.AppraisalEmployee.Add(new AppraisalEmployee
                        {
                            Employee = employee,
                            Guid = CustomGuid.NewGuid(),
                            Status = "",
                            CompanyId = request.CompanyIdNum,
                            AddedBy = request.UserIdNum,
                            AddedOn = DateTime.UtcNow,
                            IsActive = true,
                        });

                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.AddAppraisal,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = employee.Id,
                            NotificationData = null
                        });
                    }
                }

                AddItem(newAppraisal);
                Save();
            }
            else
            {
                var addedAppraisal = GetAll()
                    .Include(var => var.AppraisalEmployee)
                    .Include(var => var.AppraisalQuestion)
                    .Include(var => var.AppraisalGrade)
                    .Include(var => var.AppraisalEmployee)
                    .ThenInclude(var => var.AppraisalFeedback)
                    .Include(var => var.AppraisalEmployee)
                    .ThenInclude(var => var.AppraisalAnswer)
                    .Include(var => var.AppraisalEmployee)
                    .ThenInclude(var => var.AppraisalSelfAnswer)
                    .Include(var => var.AppraisalEmployee)
                    .ThenInclude(var => var.AppraisalTraining)
                    .Include(var => var.AppraisalEmployee)
                    .ThenInclude(var => var.AppraisalBusinessNeed)
                    .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.AppraisalId));

                if (addedAppraisal != null)
                {
                    addedAppraisal.Description = request.Description;
                    addedAppraisal.UpdatedBy = request.UserIdNum;
                    addedAppraisal.UpdatedOn = DateTime.UtcNow;
                    addedAppraisal.EndDate = request.EndDate;
                    addedAppraisal.StartDate = request.StartDate;
                    addedAppraisal.ShowCalculation = request.ShowCalculation;
                    addedAppraisal.Category = request.Category;
                    addedAppraisal.Mode = request.Mode == "objective" ? 1 : request.Mode == "variablebonus" ? 2 : 3;
                    addedAppraisal.CalculationMethod = request.CalculationMethod;
                    addedAppraisal.EligibleFrom = request.EligibleFrom;
                    addedAppraisal.EligibleTo = request.EligibleTo;
                    addedAppraisal.Year = request.Year;

                    if (request.IsLive && !(addedAppraisal.IsLive ?? false))
                    {
                        var questions = new List<AppraisalQuestion>();
                        if (request.Questions != null && request.Questions.Any())
                        {
                            foreach (var question in request.Questions)
                            {
                                var addedQuestion = dbContext.SettingsAppraisalQuestion
                                    .FirstOrDefault(var => var.IsActive && var.Guid.Equals(question.QuestionId));
                                if (addedQuestion != null)
                                {
                                    questions.Add(new AppraisalQuestion
                                    {
                                        Guid = CustomGuid.NewGuid(),
                                        Percentage = question.Percentage,
                                        AddedBy = request.UserIdNum,
                                        AddedOn = DateTime.UtcNow,
                                        IsActive = true,
                                        Question = addedQuestion
                                    });
                                }
                                else
                                {
                                    throw new Exception("Question not found.");
                                }
                            }
                        }

                        var grades = new List<long>();
                        if (request.GradeIds != null && request.GradeIds.Count > 0)
                        {
                            grades = dbContext.SettingsGrade.Where(var =>
                                    var.IsActive && var.CompanyId == request.CompanyIdNum && request.GradeIds.Contains(var.Guid))
                                .Select(var => var.Id)
                                .ToList();
                        }


                        dbContext.AppraisalGrade.RemoveRange(addedAppraisal.AppraisalGrade);
                        dbContext.AppraisalQuestion.RemoveRange(addedAppraisal.AppraisalQuestion);

                        foreach (var employee in addedAppraisal.AppraisalEmployee)
                        {
                            dbContext.AppraisalFeedback.RemoveRange(employee.AppraisalFeedback);
                            dbContext.AppraisalSelfAnswer.RemoveRange(employee.AppraisalSelfAnswer);
                            dbContext.AppraisalAnswer.RemoveRange(employee.AppraisalAnswer);
                            dbContext.AppraisalBusinessNeed.RemoveRange(employee.AppraisalBusinessNeed);
                            dbContext.AppraisalTraining.RemoveRange(employee.AppraisalTraining);
                        }

                        dbContext.AppraisalEmployee.RemoveRange(addedAppraisal.AppraisalEmployee);

                        addedAppraisal.Title = request.Title;
                        addedAppraisal.IsOpen = true;
                        addedAppraisal.IsLive = request.IsLive;
                        addedAppraisal.ShowCalculation = request.ShowCalculation;
                        addedAppraisal.AppraisalQuestion = questions;
                        addedAppraisal.Category = request.Category;
                        addedAppraisal.Mode = request.Mode == "objective" ? 1 : request.Mode == "variablebonus" ? 2 : 3;
                        addedAppraisal.CalculationMethod = request.CalculationMethod;
                        addedAppraisal.EligibleFrom = request.EligibleFrom;
                        addedAppraisal.EligibleTo = request.EligibleTo;
                        addedAppraisal.Year = request.Year;
                        addedAppraisal.AppraisalGrade = grades.Select(var => new AppraisalGrade
                        {
                            GradeId = var
                        }).ToList();

                        var employees = dbContext.Employee
                            .Include(var => var.EmployeeCompanyEmployee)
                            .ThenInclude(var => var.Grade)
                            .Where(var => var.IsActive
                                          && var.EmployeeCompanyEmployee.Any()
                                          && (!request.GradeIds.Any() ||
                                              grades.Contains(var.EmployeeCompanyEmployee.FirstOrDefault().GradeId ?? 0)));

                        foreach (var employee in employees)
                        {
                            addedAppraisal.AppraisalEmployee.Add(new AppraisalEmployee
                            {
                                Employee = employee,
                                Guid = CustomGuid.NewGuid(),
                                Status = "",
                                CompanyId = request.CompanyIdNum,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.UtcNow,
                                IsActive = true,
                            });

                            dbContext.Notification.Add(new Notification
                            {
                                Guid = CustomGuid.NewGuid(),
                                ActionId = NotificationTemplates.UpdateAppraisal,
                                NotificationTime = DateTime.UtcNow,
                                IsActive = true,
                                IsRead = false,
                                NotificationTo = employee.Id,
                                NotificationData = null
                            });
                        }
                    }

                    Save();
                }
                else
                {
                    throw new Exception("Appraisal not found");
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateAppraisal.Template, request.UserName, request.UserId, guid),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateAppraisal.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName,
                    appraisalId = guid
                })
            });

            Save();

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse DeleteAppraisal(AppraisalActionRequest request)
        {
            var addedAppraisal = GetAll()
                .Include(var => var.AppraisalEmployee)
                .Include(var => var.AppraisalQuestion)
                .Include(var => var.AppraisalGrade)
                .Include(var => var.AppraisalEmployee)
                .ThenInclude(var => var.AppraisalFeedback)
                .Include(var => var.AppraisalEmployee)
                .ThenInclude(var => var.AppraisalAnswer)
                .Include(var => var.AppraisalEmployee)
                .ThenInclude(var => var.AppraisalSelfAnswer)
                .Include(var => var.AppraisalEmployee)
                .ThenInclude(var => var.AppraisalTraining)
                .Include(var => var.AppraisalEmployee)
                .ThenInclude(var => var.AppraisalBusinessNeed)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.AppraisalId));

            if (addedAppraisal != null)
            {
                dbContext.AppraisalGrade.RemoveRange(addedAppraisal.AppraisalGrade);
                dbContext.AppraisalQuestion.RemoveRange(addedAppraisal.AppraisalQuestion);

                foreach (var employee in addedAppraisal.AppraisalEmployee)
                {
                    dbContext.AppraisalFeedback.RemoveRange(employee.AppraisalFeedback);
                    dbContext.AppraisalSelfAnswer.RemoveRange(employee.AppraisalSelfAnswer);
                    dbContext.AppraisalAnswer.RemoveRange(employee.AppraisalAnswer);
                    dbContext.AppraisalBusinessNeed.RemoveRange(employee.AppraisalBusinessNeed);
                    dbContext.AppraisalTraining.RemoveRange(employee.AppraisalTraining);

                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.DeleteAppraisal,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = employee.EmployeeId,
                        NotificationData = null
                    });
                }

                dbContext.AppraisalEmployee.RemoveRange(addedAppraisal.AppraisalEmployee);

                addedAppraisal.IsActive = false;
                addedAppraisal.UpdatedBy = request.UserIdNum;
                addedAppraisal.UpdatedOn = DateTime.UtcNow;

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.DeleteAppraisal.Template, request.UserName, request.UserId, request.AppraisalId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.DeleteAppraisal.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        appraisalId = request.AppraisalId
                    })
                });

                Save();

                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Appraisal not found");
        }

        public AppraisalDetailsResponse GetAppraisalDetails(AppraisalActionRequest request)
        {
            var addedAppraisal = GetAll()
                .Include(var => var.AppraisalEmployee).ThenInclude(var => var.Employee.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                .Include(var => var.AppraisalEmployee).ThenInclude(var => var.Employee.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                .Include(var => var.AppraisalEmployee).ThenInclude(var => var.Employee.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                .Include(var => var.AppraisalEmployee).ThenInclude(var => var.Employee.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                .Include(var => var.AppraisalEmployee).ThenInclude(var => var.Employee.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo).ThenInclude(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .Include(var => var.AppraisalEmployee).ThenInclude(var => var.RatingNavigation)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.AppraisalId));

            if (addedAppraisal != null)
            {
                var employees = addedAppraisal.AppraisalEmployee
                    .Where(var => var.IsActive
                    && var.Employee.EmployeeCompanyEmployee.Any(var1 => !string.IsNullOrWhiteSpace(var1.EmployeeCode) && var1.Grade != null))
                    .ToList();

                var employeesList = new List<AppraisalEmployeeDto>();
                foreach (var emp in employees)
                {
                    var employeeCompany = emp.Employee.EmployeeCompanyEmployee.FirstOrDefault(var => !string.IsNullOrWhiteSpace(var.EmployeeCode));
                    if (employeeCompany != null)
                    {
                        var empItem = new AppraisalEmployeeDto
                        {
                            AppraisalTitle = emp.Appraisal.Title,
                            Code = employeeCompany.EmployeeCode,
                            Department = employeeCompany.Department.Name,
                            Grade = employeeCompany.Grade.Grade,
                            Location = employeeCompany.Location.Name,
                            Designation = employeeCompany.Designation.Name,
                            EmployeeId = emp.Employee.Guid,
                            EmailId = emp.Employee.EmailId,
                            Name = emp.Employee.Name,
                            SelfFilledOn = emp.SelfSubmittedOn,
                            RmFilledOn = emp.RmSubmittedOn,
                            HrFilledOn = emp.HrSubmittedOn,
                            Rating = emp.RatingNavigation == null ? null : emp.RatingNavigation.RatingTitle,
                            ManagerName = employeeCompany.ReportingTo != null ? employeeCompany.ReportingTo.Name : null,
                            L2ManagerName = emp.Employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != null
                                            && emp.Employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingToId != null
                                ? emp.Employee.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo.EmployeeCompanyEmployee.FirstOrDefault().ReportingTo.Name
                                : null,
                            L2FilledOn = emp.L2SubmittedOn,
                            VariableRating = emp.VariableBonusRatingNavigation == null ? null : emp.VariableBonusRatingNavigation.RatingTitle,
                            SelfObjectiveFilledOn = emp.SelfObjectiveSubmittedOn,
                            RmObjectiveFilledOn = emp.RmObjectiveSubmittedOn,
                            L2ObjectiveFilledOn = emp.L2ObjectiveSubmittedOn,
                            HrObjectiveFilledOn = emp.HrObjectiveSubmittedOn,
                            SelfVariableFilledOn = emp.SelfVariableSubmittedOn,
                            RmVariableFilledOn = emp.RmVariableSubmittedOn,
                            L2VariableFilledOn = emp.L2VariableSubmittedOn,
                            HrVariableFilledOn = emp.HrVariableSubmittedOn,
                            AppraisalMode = emp.Appraisal.Mode
                        };
                        employeesList.Add(empItem);
                    }
                }

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetAppraisalDetails.Template, request.UserName, request.UserId, request.AppraisalId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetAppraisalDetails.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        appraisalId = request.AppraisalId
                    })
                });

                Save();

                return new AppraisalDetailsResponse
                {
                    AppraisalEmployees = employeesList,
                    IsSuccess = true
                };
            }

            throw new Exception("Appraisal not found");
        }
    }
}