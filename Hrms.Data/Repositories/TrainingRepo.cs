using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Hrms.Data.Repositories
{
    public class TrainingRepo : BaseRepository<Training>, ITrainingRepo
    {
        private IEmployeeRepo _empRepo;
        private readonly ApplicationSettings appSettings;

        public TrainingRepo(
            IOptions<ApplicationSettings> settings,
            HrmsContext context,
            IEmployeeRepo empRepo
        ) : base(context)
        {
            appSettings = settings.Value;
            _empRepo = empRepo;
        }

        public TrainingListResponse GetTrainingCalendar(GetHolidaysRequset request)
        {
            var startDate = new DateTime(request.Year, request.Month == 0 ? 1 : request.Month, 1);
            var endDate = request.Month == 0 ? startDate.AddYears(1).AddDays(-1) : startDate.AddMonths(1).AddDays(-1);

            var trainings = GetAll()
                .Include(var => var.TrainingNavigation)
                .Include(var => var.TrainingDate)
                .Include(var => var.TrainingLocationNavigation)
                .Where(var => var.IsActive
                && var.TrainingDate.FirstOrDefault().Date <= endDate
                && var.TrainingDate.FirstOrDefault().Date >= startDate
                && (string.IsNullOrWhiteSpace(request.Location) || var.TrainingLocation1 == null || var.TrainingLocation1.Any(var1 => var1.Location.Guid.Equals(request.Location)))
                && var.IsFeedbackClosed == null
                ).Select(var => new TrainingCalDto
                {
                    TrainingId = var.Guid,
                    TrainingName = var.TrainingNavigation.Name,
                    TrainerName = var.TrainerName,
                    isConfirmed = var.IsConfirmed != null ? var.IsConfirmed : false,
                    isStartd = var.IsConfirmed != null ? var.IsStarted : false,
                    isClosed = var.IsConfirmed != null ? var.IsCompleted : false,
                    Date = var.TrainingDate.FirstOrDefault().Date,
                    MaxNominees = var.MaxNominees,
                    Location = var.IsOfficeLocation ? var.TrainingLocationNavigation.Name : var.OtherLocation,
                    Organizers = var.TrainingOrganizer.Any() ? string.Join(", ", var.TrainingOrganizer.Select(var1 => var1.Employee.Name).ToList()) : string.Empty,
                }).ToList();
            return new TrainingListResponse
            {
                IsSuccess = true,
                TrainingsCalendar = trainings
                    .ToList()
            };
        }
        public TrainingListResponse GetTrainings(TrainingFilterRequest request)
        {
            var trainings = GetAll()
                .Include(var => var.TrainingNavigation)
                .Include(var => var.TrainingDate)
                .Include(var => var.TrainingLocationNavigation)
                .Include(var => var.TrainingAttendance)
                .Include(var => var.TrainingNominees).ThenInclude(var => var.Employee).ThenInclude(var => var.EmployeeCompanyEmployee)
                .Include(var => var.TrainingFeedback)
                .Include(var => var.TrainingOrganizer).ThenInclude(var => var.Employee).ThenInclude(var => var.EmployeeCompanyEmployee)
                .Where(var => var.IsActive &&
                              (string.IsNullOrWhiteSpace(request.TrainingName) || var.TrainingNavigation.Name.Contains(request.TrainingName))
                              && (request.StartDate == null || var.TrainingDate.Min(var1 => var1.Date) >= request.StartDate)
                              && (request.EndDate == null || var.TrainingDate.Max(var1 => var1.Date) <= request.EndDate)
                              && (request.EmployeeIds == null || !request.EmployeeIds.Any() || var.TrainingNominees.Any(var1 => request.EmployeeIds.Contains(var1.Employee.Guid)))
                              )
                .Select(var => new TrainingDto
                {
                    Effectiveness = var.TrainingFeedback != null && var.TrainingFeedback.Any()
                        ? Math.Round(var.TrainingFeedback
                            .Average(var1 => Convert.ToDouble(var1.Answer ?? "0")), 1)
                            .ToString(CultureInfo.InvariantCulture)
                        : "0",
                    TrainingId = var.Guid,
                    TrainingName = var.TrainingNavigation.Name,
                    TrainerName = var.TrainerName,
                    StartDate = var.TrainingDate.Any() ? var.TrainingDate.Min(var1 => var1.Date.Value) : (DateTime?)null,
                    EndDate = var.TrainingDate.Any() ? var.TrainingDate.Max(var1 => var1.Date.Value) : (DateTime?)null,
                    TotalDays = var.TrainingDate.Count,
                    MaxNominees = var.MaxNominees,
                    IsConfirmed = var.IsConfirmed ?? false,
                    IsCompleted = var.IsCompleted ?? false,
                    IsStarted = var.IsStarted ?? false,
                    IsFeedbackClosed = var.IsFeedbackClosed ?? false,
                    Location = var.IsOfficeLocation ? var.TrainingLocationNavigation.Name : var.OtherLocation,
                    Organizers = var.TrainingOrganizer.Any() ? string.Join(", ", var.TrainingOrganizer.Select(var1 => var1.Employee.Name).ToList()) : string.Empty,
                    AttendancePercentage = var.TrainingAttendance.Any() ? var.TrainingAttendance.Count(var1 => var1.HasAttended ?? false) + " of " + var.TrainingAttendance.Count : string.Empty,
                    FeedbackPercentage = (var.IsCompleted.HasValue && var.IsCompleted.Value) ? var.TrainingFeedback.Select(var1 => var1.NomineeId).Distinct().Count().ToString() + " of " + var.TrainingNominees.Count : string.Empty
                })
                .OrderByDescending(var => var.StartDate)
                .ToList();
            
            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(15) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(15) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            return new TrainingListResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                Trainings = trainings
                    .Where(var => var.IsFeedbackClosed)
                    .OrderByDescending(var => var.StartDate)
                    .ThenBy(var => var.TrainerName)
                    .ToList(),
                UpcomingTrainings = trainings
                    .Where(var => !var.IsFeedbackClosed)
                    .OrderBy(var => var.StartDate)
                    .ThenBy(var => var.TrainingName)
                    .ToList()
            };
        }

        public BaseResponse UpdateTraining(UpdateTrainingRequest request)
        {
            var trainingType =
                dbContext.SettingsTraining.FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.TrainingType));

            var grades = new List<SettingsGrade>();
            var departments = new List<SettingsDepartment>();
            var designations = new List<SettingsDepartmentDesignation>();
            var locations = new List<SettingsLocation>();
            var officeLocation = new SettingsLocation();
            var organizers = new List<Employee>();

            if (request.Grades != null)
            {
                grades = dbContext.SettingsGrade.Where(var => var.IsActive && request.Grades.Contains(var.Guid))
                    .ToList();
            }
            if (request.Departments != null)
            {
                departments = dbContext.SettingsDepartment.Where(var => var.IsActive && request.Departments.Contains(var.Guid))
                    .ToList();
            }
            if (request.Designations != null)
            {
                designations = dbContext.SettingsDepartmentDesignation.Where(var => var.IsActive && request.Designations.Contains(var.Guid))
                    .ToList();
            }
            if (request.Locations != null)
            {
                locations = dbContext.SettingsLocation.Where(var => var.IsActive && request.Locations.Contains(var.Guid))
                    .ToList();
            }

            if (request.IsOfficeLocation)
            {
                officeLocation = dbContext.SettingsLocation.FirstOrDefault(var =>
                    var.IsActive && var.Guid == request.OfficeLocationId);
            }

            if (request.Organizers != null)
            {
                organizers = dbContext.Employee.Where(var => var.IsActive && request.Organizers.Contains(var.Guid))
                    .ToList();
            }

            if (string.IsNullOrWhiteSpace(request.TrainingId))
            {
                var newTraining = new Training
                {
                    Guid = CustomGuid.NewGuid(),
                    AddedBy = request.UserIdNum,
                    AddedOn = DateTime.UtcNow,
                    Description = request.Description,
                    IsActive = true,
                    IsCompleted = false,
                    IsConfirmed = false,
                    IsOfficeLocation = request.IsOfficeLocation,
                    MaxNominees = request.MaxNominees,
                    OtherLocation = request.OtherLocation,
                    TimeOfDay = request.TimeOfDay,
                    TrainerName = request.TrainerName,
                    Title = request.TrainingTitle,
                    Code = request.TrainingCode,
                    Category = request.TrainingCategory,

                    TrainingDate = request.Dates.Select(var => new TrainingDate
                    {
                        Date = var,
                    }).ToList(),
                    TrainingDepartment = departments != null ? departments.Select(var => new TrainingDepartment
                    {
                        Department = var
                    }).ToList() : null,
                    TrainingDesignation = designations != null ? designations.Select(var => new TrainingDesignation
                    {
                        Designation = var
                    }).ToList() : null,
                    TrainingLocation1 = locations != null ? locations.Select(var => new TrainingLocation
                    {
                        Location = var
                    }).ToList() : null,
                    TrainingGrade = grades != null ? grades.Select(var => new TrainingGrade
                    {
                        Grade = var
                    }).ToList() : null,
                    TrainingNavigation = trainingType,
                    TrainingLocationNavigation = request.IsOfficeLocation ? officeLocation : null,
                    TrainingOrganizer = organizers != null ? organizers.Select(var => new TrainingOrganizer
                    {
                        Employee = var
                    }).ToList() : null,
                };

                if (request.Nominees != null && request.Nominees.Any())
                {
                    var employees = dbContext.Employee
                        .Include(var => var.EmployeeCompanyEmployee)
                        .Where(var => request.Nominees.Contains(var.Guid) && var.IsActive)
                        .ToList();

                    foreach (var employee in employees)
                    {
                        var newNominee = new TrainingNominees
                        {
                            Guid = CustomGuid.NewGuid(),
                            EmployeeId = employee.Id,
                        };

                        var employeeManager = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId;

                        if (employeeManager != null)
                        {
                            newNominee.ManagerId = employeeManager;
                        }
                        else
                        {
                            newNominee.ManagerAccepted = true;
                        }

                        newTraining.TrainingNominees.Add(newNominee);
                    }
                }

                AddItem(newTraining);
                Save();
            }
            else
            {
                var addedTraining = GetAll()
                    .Include(var => var.TrainingDate)
                    .Include(var => var.TrainingAttendance)
                    .Include(var => var.TrainingDesignation)
                    .Include(var => var.TrainingDepartment)
                    .Include(var => var.TrainingLocation1)
                    .Include(var => var.TrainingGrade)
                    .Include(var => var.TrainingNominees)
                    .Include(var => var.TrainingOrganizer)
                    .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.TrainingId));
                if (addedTraining != null)
                {
                    if (addedTraining.IsConfirmed ?? false)
                    {
                        addedTraining.TrainerName = request.TrainerName;
                    }
                    else
                    {
                        dbContext.TrainingNominees.RemoveRange(addedTraining.TrainingNominees);
                        dbContext.TrainingDate.RemoveRange(addedTraining.TrainingDate);
                        dbContext.TrainingAttendance.RemoveRange(addedTraining.TrainingAttendance);
                        dbContext.TrainingDepartment.RemoveRange(addedTraining.TrainingDepartment);
                        dbContext.TrainingGrade.RemoveRange(addedTraining.TrainingGrade);
                        dbContext.TrainingDesignation.RemoveRange(addedTraining.TrainingDesignation);
                        dbContext.TrainingLocation.RemoveRange(addedTraining.TrainingLocation1);
                        dbContext.TrainingOrganizer.RemoveRange(addedTraining.TrainingOrganizer);

                        addedTraining.Description = request.Description;
                        addedTraining.IsOfficeLocation = request.IsOfficeLocation;
                        addedTraining.MaxNominees = request.MaxNominees;
                        addedTraining.OtherLocation = request.OtherLocation;
                        addedTraining.TimeOfDay = request.TimeOfDay;
                        addedTraining.TrainerName = request.TrainerName;
                        addedTraining.Title = request.TrainingTitle;
                        addedTraining.Code = request.TrainingCode;
                        addedTraining.TrainingDate = request.Dates.Select(var => new TrainingDate
                        {
                            Date = var,
                        }).ToList();
                        addedTraining.TrainingDepartment = departments != null
                            ? departments.Select(var => new TrainingDepartment
                            {
                                Department = var
                            }).ToList()
                            : null;
                        addedTraining.TrainingDesignation = designations != null
                            ? designations.Select(var => new TrainingDesignation
                            {
                                Designation = var
                            }).ToList()
                            : null;
                        addedTraining.TrainingLocation1 = locations != null
                            ? locations.Select(var => new TrainingLocation
                            {
                                Location = var
                            }).ToList()
                            : null;
                        addedTraining.TrainingGrade = grades != null
                            ? grades.Select(var => new TrainingGrade
                            {
                                Grade = var
                            }).ToList()
                            : null;
                        addedTraining.TrainingOrganizer = organizers != null
                            ? organizers.Select(var => new TrainingOrganizer
                            {
                                Employee = var
                            }).ToList()
                            : null;
                        addedTraining.TrainingNavigation = trainingType;
                        addedTraining.TrainingLocationNavigation = request.IsOfficeLocation ? officeLocation : null;


                        if (request.Nominees != null && request.Nominees.Any())
                        {
                            var employees = dbContext.Employee
                                .Include(var => var.EmployeeCompanyEmployee)
                                .Where(var => request.Nominees.Contains(var.Guid) && var.IsActive)
                                .ToList();

                            foreach (var employee in employees)
                            {
                                var newNominee = new TrainingNominees
                                {
                                    Guid = CustomGuid.NewGuid(),
                                    EmployeeId = employee.Id,
                                };

                                var employeeManager = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId;

                                if (employeeManager != null)
                                {
                                    newNominee.ManagerId = employeeManager;
                                }
                                else
                                {
                                    newNominee.ManagerAccepted = true;
                                }

                                addedTraining.TrainingNominees.Add(newNominee);
                            }
                        }
                    }

                    Save();
                }
                else
                {
                    throw new Exception("Training not found.");
                }
            }

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse UpdateNomineeResponse(TrainingNomineeRequest request)
        {
            var training = GetAll()
                .Include(var => var.TrainingDate)
                .Include(var => var.TrainingNominees).ThenInclude(var => var.TrainingAttendance)
                .FirstOrDefault(var =>
                    var.Guid.Equals(request.TrainingId) && var.IsActive && (var.IsConfirmed ?? false) &&
                    !(var.IsCompleted ?? false));

            if (training == null)
            {
                throw new Exception("Training not found or is completed.");
            }

            foreach (var nominee in request.Nominees)
            {
                var addedNominee = training.TrainingNominees.FirstOrDefault(var => var.Guid.Equals(nominee.NomineeId));
                if (addedNominee == null)
                {
                    throw new Exception("Nominee not found.");
                }

                if (nominee.IsSelf)
                {
                    addedNominee.SelfUpdatedOn = DateTime.Now;
                    addedNominee.SelfAccepted = nominee.IsAccepted;

                    var employee = dbContext.Employee
                        .Include(var => var.EmployeeCompanyEmployee)
                        .FirstOrDefault(var => var.Id == request.UserIdNum && var.IsActive);

                    var employeeManager = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId;

                    if (employeeManager != null)
                    {
                        addedNominee.ManagerId = employeeManager;
                    }
                    else
                    {
                        addedNominee.ManagerAccepted = true;
                    }

                }
                else if (nominee.IsManager)
                {
                    addedNominee.ManagerUpdatedOn = DateTime.Now;
                    addedNominee.ManagerAccepted = nominee.IsAccepted;
                    addedNominee.ManagerId = request.UserIdNum;
                }
                else if (nominee.IsHr)
                {
                    addedNominee.HrUpdatedOn = DateTime.Now;
                    addedNominee.HrAccepted = nominee.IsAccepted;
                    addedNominee.HrId = request.UserIdNum;

                    addedNominee.IsRejected = !nominee.IsAccepted;
                    addedNominee.RejectedReason = nominee.RejectionReason;
                }

                if (nominee.IsAccepted && request.UserRole.Equals("HR") && nominee.IsHr)
                {
                    foreach (var trainingDate in training.TrainingDate)
                    {
                        addedNominee.TrainingAttendance.Add(new TrainingAttendance
                        {
                            Training = training,
                            TrainingDate = trainingDate.Id,
                        });
                    }
                }
                else if (!nominee.IsAccepted && request.UserRole.Equals("HR") && nominee.IsHr)
                {
                    dbContext.TrainingAttendance.RemoveRange(addedNominee.TrainingAttendance);
                }
            }

            Save();
            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse ConfirmTraining(TrainingActionRequest request)
        {
            var training = GetAll()
                .Include(var => var.TrainingDate)
                .Include(var => var.TrainingDesignation).ThenInclude(var => var.Designation)
                .Include(var => var.TrainingDepartment).ThenInclude(var => var.Department)
                .Include(var => var.TrainingLocation1).ThenInclude(var => var.Location)
                .Include(var => var.TrainingGrade).ThenInclude(var => var.Grade)
                .Include(var => var.TrainingNominees).ThenInclude(var => var.Employee).ThenInclude(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .Include(var => var.TrainingLocationNavigation)
                .FirstOrDefault(var =>
                    var.Guid.Equals(request.TrainingId) && var.IsActive && !(var.IsConfirmed ?? false) &&
                    !(var.IsCompleted ?? false));

            if (training == null)
            {
                throw new Exception("Training not found or is completed.");
            }

            training.IsConfirmed = true;

            Save();

            var startDate = training.TrainingDate.Min(var => var.Date.Value).ToString("D");
            var endDate = training.TrainingDate.Max(var => var.Date.Value).ToString("D");
            var acceptanceLastDate = training.TrainingDate.Min(var => var.Date.Value).AddDays(-4).ToString("D");

            var trainingEmailInfo = new TrainingEmailDto
            {
                Date = startDate.Equals(endDate) ? startDate : string.Concat(startDate, " to ", endDate),
                Designations = string.Join(" ,", training.TrainingDesignation.Select(var => var.Designation.Name)),
                Locations = string.Join(" ,", training.TrainingLocation1.Select(var => var.Location.Name)),
                Grades = string.Join(" ,", training.TrainingGrade.Select(var => var.Grade.Grade)),
                Departments = string.Join(" ,", training.TrainingDepartment.Select(var => var.Department.Name)),
                TrainingName = training.Title,
                Mode = training.IsOfficeLocation && training.TrainingLocationNavigation != null ? training.TrainingLocationNavigation.Name : training.OtherLocation,
                Time = training.TimeOfDay,
                AcceptanceLastDate = acceptanceLastDate,
                TrainingDescription = training.Description
            };

            var trainingNominees = training.TrainingNominees.ToList();
            var managers = new Dictionary<string, List<string[]>>();


            foreach (var nominee in trainingNominees)
            {
                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.EmpTrainingConfirmation,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = nominee.Employee.Id,
                    NotificationData = JsonConvert.SerializeObject(
                                  new
                                  {
                                      userName = request.UserName
                                  })
                });

                EmailSender.SendTrainingConfirmedEmail(nominee.Employee.Name, nominee.Employee.EmailId, trainingEmailInfo, appSettings);

                var companyInfo = nominee.Employee.EmployeeCompanyEmployee.FirstOrDefault();
                if (companyInfo.ReportingTo != null)
                {
                    if (!managers.ContainsKey(companyInfo.ReportingTo.EmailId))
                    {
                        string[] _managerIdName = { companyInfo.ReportingTo.Id.ToString(), companyInfo.ReportingTo.Name, nominee.Employee.Name };
                        List<string[]> lstManagerEmpDetails = new List<string[]>();
                        lstManagerEmpDetails.Add(_managerIdName);
                        managers.Add(companyInfo.ReportingTo.EmailId, lstManagerEmpDetails);
                    }
                }
            }

            foreach (var manager in managers)
            {

                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.RMTrainingConfirmation,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = Convert.ToInt64(manager.Value[0][0]),
                    NotificationData = JsonConvert.SerializeObject(
                              new
                              {
                                  empCode = manager.Value[0][2]
                              })
                });
                EmailSender.SendTrainingConfirmedManagerEmail(manager.Value[0][1], manager.Key, trainingEmailInfo, appSettings);

            }

            Save();

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse FillAttendance(TrainingAttendanceRequest request)
        {
            var training = GetAll()
                .Include(var => var.TrainingNominees).ThenInclude(var => var.TrainingAttendance).ThenInclude(var => var.TrainingDateNavigation)
                .Include(var => var.TrainingAttendance)
                .FirstOrDefault(var =>
                    var.Guid.Equals(request.TrainingId) && var.IsActive && (var.IsConfirmed ?? false) &&
                    !(var.IsCompleted ?? false));

            if (training == null)
            {
                throw new Exception("Training not found or is completed.");
            }

            foreach (var attendance in request.Attendance)
            {
                var nominee = training.TrainingNominees.FirstOrDefault(var => var.Guid.Equals(attendance.NomineeId));
                if (nominee == null)
                {
                    throw new Exception("Nominee not found");
                }

                var date = training.TrainingDate.FirstOrDefault(var =>
                    var.Date.Value.Date.Equals(attendance.Date.Value.Date));

                if (date == null)
                {
                    throw new Exception("Training date not found");
                }

                var nomineeAttendance = nominee.TrainingAttendance.FirstOrDefault(var => var.TrainingDate == date.Id);
                if (nomineeAttendance == null)
                {
                    nominee.TrainingAttendance.Add(new TrainingAttendance
                    {
                        TrainingDate = date.Id,
                        HasAttended = attendance.IsAttended,
                        Remark = attendance.Remark,
                        Training = training,
                        TrainingNomineeNavigation = nominee
                    });
                }
                else
                {
                    nomineeAttendance.HasAttended = attendance.IsAttended;
                    nomineeAttendance.Remark = attendance.Remark;
                }
            }

            Save();
            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse AddMoreNominees(AddNomineesRequest request)
        {
            var training = GetAll()
                .Include(var => var.TrainingNominees).ThenInclude(var => var.TrainingAttendance).ThenInclude(var => var.TrainingDateNavigation)
                .Include(var => var.TrainingNominees).ThenInclude(var => var.Employee)

                .Include(var => var.TrainingDate)
                .Include(var => var.TrainingDesignation).ThenInclude(var => var.Designation)
                .Include(var => var.TrainingDepartment).ThenInclude(var => var.Department)
                .Include(var => var.TrainingLocation1).ThenInclude(var => var.Location)
                .Include(var => var.TrainingGrade).ThenInclude(var => var.Grade)
                .Include(var => var.TrainingNominees).ThenInclude(var => var.Employee).ThenInclude(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.ReportingTo)
                .Include(var => var.TrainingLocationNavigation)
                .FirstOrDefault(var =>
                    var.Guid.Equals(request.TrainingId) && var.IsActive && (var.IsConfirmed ?? false) &&
                    !(var.IsCompleted ?? false));

            if (training == null)
            {
                throw new Exception("Training not found or is completed.");
            }

            if (request.UserRole.Equals("HR"))
            {
                var alreadyAddedNominees =
                    training.TrainingNominees
                        .Where(var => request.Nominees.Contains(var.Employee.Guid))
                        .Select(var => var.Employee.Guid)
                        .ToList();
                var remainingNominees = request.Nominees.Except(alreadyAddedNominees);
                var remainingEmployeeIds =
                    dbContext.Employee
                        .Where(var => var.IsActive && remainingNominees.Contains(var.Guid))
                        .Select(var => var.Id);

                foreach (var employeeId in remainingEmployeeIds)
                {
                    var newNominee = new TrainingNominees
                    {
                        Guid = CustomGuid.NewGuid(),
                        EmployeeId = employeeId,
                    };

                    var employee = dbContext.Employee
                        .Include(var => var.EmployeeCompanyEmployee)
                        .FirstOrDefault(var => var.Id == employeeId && var.IsActive);

                    var employeeManager = employee.EmployeeCompanyEmployee.FirstOrDefault()?.ReportingToId;

                    if (employeeManager != null)
                    {
                        newNominee.ManagerId = employeeManager;
                    }
                    else
                    {
                        newNominee.ManagerAccepted = true;
                    }

                    training.TrainingNominees.Add(newNominee);
                }

                Save();

                var startDate = training.TrainingDate.Min(var => var.Date.Value).ToString("D");
                var endDate = training.TrainingDate.Max(var => var.Date.Value).ToString("D");
                var acceptanceLastDate = training.TrainingDate.Min(var => var.Date.Value).AddDays(-4).ToString("D");

                var trainingEmailInfo = new TrainingEmailDto
                {
                    Date = startDate.Equals(endDate) ? startDate : string.Concat(startDate, " to ", endDate),
                    Designations = string.Join(" ,", training.TrainingDesignation.Select(var => var.Designation.Name)),
                    Locations = string.Join(" ,", training.TrainingLocation1.Select(var => var.Location.Name)),
                    Grades = string.Join(" ,", training.TrainingGrade.Select(var => var.Grade.Grade)),
                    Departments = string.Join(" ,", training.TrainingDepartment.Select(var => var.Department.Name)),
                    TrainingName = training.Title,
                    Mode = training.IsOfficeLocation && training.TrainingLocationNavigation != null ? training.TrainingLocationNavigation.Name : training.OtherLocation,
                    Time = training.TimeOfDay,
                    AcceptanceLastDate = acceptanceLastDate,
                    TrainingDescription = training.Description
                };

                var managers = new Dictionary<string, List<string[]>>();
                foreach (var employeeId in remainingEmployeeIds)
                {
                    var employee = dbContext.Employee
                        .Include(var => var.EmployeeCompanyEmployee)
                        .FirstOrDefault(var => var.Id == employeeId && var.IsActive);


                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.EmpTrainingConfirmation,
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

                    EmailSender.SendTrainingConfirmedEmail(employee.Name, employee.EmailId, trainingEmailInfo, appSettings);

                    var companyInfo = employee.EmployeeCompanyEmployee.FirstOrDefault();
                    if (companyInfo.ReportingTo != null)
                    {
                        if (!managers.ContainsKey(companyInfo.ReportingTo.EmailId))
                        {
                            string[] _managerIdName = { companyInfo.ReportingTo.Id.ToString(), companyInfo.ReportingTo.Name, employee.Name };
                            List<string[]> lstManagerEmpDetails = new List<string[]>();
                            lstManagerEmpDetails.Add(_managerIdName);
                            managers.Add(companyInfo.ReportingTo.EmailId, lstManagerEmpDetails);
                        }
                    }
                }

                foreach (var manager in managers)
                {
                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.RMTrainingConfirmation,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = Convert.ToInt64(manager.Value[0][0]),
                        NotificationData = JsonConvert.SerializeObject(
                              new
                              {
                                  empCode = manager.Value[0][2]
                              })
                    });

                    EmailSender.SendTrainingConfirmedManagerEmail(manager.Value[0][1], manager.Key, trainingEmailInfo, appSettings);
                }
            }

            Save();

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse CompleteTraining(TrainingActionRequest request)
        {
            var training = GetAll()
                .Include(var => var.TrainingNominees).ThenInclude(var => var.TrainingAttendance).ThenInclude(var => var.TrainingDateNavigation)
                .Include(var => var.TrainingNominees).ThenInclude(var => var.Employee)
                .FirstOrDefault(var =>
                    var.Guid.Equals(request.TrainingId) && var.IsActive && (var.IsConfirmed ?? false) &&
                    !(var.IsCompleted ?? false));

            if (training == null)
            {
                throw new Exception("Training not found or is completed.");
            }

            training.IsCompleted = true;
            Save();

            var nominees = training.TrainingNominees.Where(var => !var.IsRejected && (var.HrAccepted ?? false))
                .ToList();
            foreach (var nominee in nominees)
            {
                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.EmpTrainingCompleted,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = nominee.Id,
                    NotificationData = JsonConvert.SerializeObject(
                                  new
                                  {
                                      userName = request.UserName
                                  })
                });

                EmailSender.SendTrainingCompletedEmail(nominee.Employee.Name, nominee.Employee.EmailId, training.Title,
                    appSettings);
            }

            Save();

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse SubmitTrainingFeedback(SubmitFeedbackRequest request)
        {
            var training = GetAll()
                .Include(var => var.TrainingFeedback)
                .Include(var => var.TrainingNominees)
                .FirstOrDefault(var =>
                    var.Guid.Equals(request.TrainingId) && var.IsActive && (var.IsConfirmed ?? false) &&
                    (var.IsCompleted ?? false));

            if (training == null)
            {
                throw new Exception("Training not found or is completed.");
            }

            var nominee = training.TrainingNominees.FirstOrDefault(var => var.EmployeeId == request.UserIdNum);
            if (nominee == null)
            {
                throw new Exception("Nominee not found.");
            }

            foreach (var answer in request.Answers)
            {
                var addedAnswer =
                    nominee.TrainingFeedback.FirstOrDefault(var => var.QuestionId.Equals(answer.QuestionId));
                if (addedAnswer != null)
                {
                    addedAnswer.Answer = answer.Answer;
                }
                else
                {
                    nominee.TrainingFeedback.Add(new TrainingFeedback
                    {
                        Answer = answer.Answer,
                        Nominee = nominee,
                        QuestionId = answer.QuestionId,
                        Training = training
                    });
                }
            }

            nominee.FeedbackContent = request.FeedbackContent;

            Save();

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public TrainingDetailsResponse GetTrainingDetails(TrainingActionRequest request)
        {
            var training = GetAll()
                .Include(var => var.TrainingAttendance).ThenInclude(var => var.TrainingDateNavigation)
                .Include(var => var.TrainingAttendance).ThenInclude(var => var.TrainingNomineeNavigation).ThenInclude(var => var.Employee)
                .Include(var => var.TrainingDate)
                .Include(var => var.TrainingFeedback)
                .Include(var => var.TrainingLocationNavigation)
                .Include(var => var.TrainingNavigation)
                .Include(var => var.TrainingOrganizer).ThenInclude(var => var.Employee)
                .Include(var => var.TrainingDesignation).ThenInclude(var => var.Designation)
                .Include(var => var.TrainingDepartment).ThenInclude(var => var.Department)
                .Include(var => var.TrainingLocation1).ThenInclude(var => var.Location)
                .Include(var => var.TrainingGrade).ThenInclude(var => var.Grade)
                .Include(var => var.TrainingNominees).ThenInclude(var => var.Employee).ThenInclude(var => var.EmployeeCompanyEmployee)
                .Include(var => var.TrainingNominees).ThenInclude(var => var.Manager)
                .Include(var => var.TrainingNominees).ThenInclude(var => var.Hr)
                .Include(var => var.TrainingNominees).ThenInclude(var => var.TrainingFeedback).ThenInclude(var => var.Question)
                .FirstOrDefault(var =>
                    var.Guid.Equals(request.TrainingId) && var.IsActive);

            if (training == null)
            {
                throw new Exception("Training not found or is completed.");
            }

            var response = new TrainingDetailsResponse
            {
                IsSuccess = true,
                IsStarted = training.IsStarted ?? false,
                Description = training.Description,
                IsCompleted = training.IsCompleted ?? false,
                IsConfirmed = training.IsConfirmed ?? false,
                IsOfficeLocation = training.IsOfficeLocation,
                TrainingTitle = training.Title,
                TrainingCode = training.Code,
                OfficeLocationId = training.TrainingLocationNavigation != null ? training.TrainingLocationNavigation.Guid : string.Empty,
                OtherLocation = training.OtherLocation,
                TrainingTiming = training.TimeOfDay,
                TrainingTypeId = training.TrainingNavigation.Guid,
                Designations = training.TrainingDesignation.Select(var => var.Designation.Guid).ToList(),
                Departments = training.TrainingDepartment.Select(var => var.Department.Guid).ToList(),
                Grades = training.TrainingGrade.Select(var => var.Grade.Guid).ToList(),
                Locations = training.TrainingLocation1.Select(var => var.Location.Guid).ToList(),
                TrainerName = training.TrainerName,
                TrainingId = training.Guid,
                TrainingCategory = training.Category,
                Organizers = training.TrainingOrganizer.Select(var => var.Employee.Guid).ToList(),
                Dates = training.TrainingDate.Select(var => var.Date).ToList(),
                MaxNominees = training.MaxNominees,
                IsFeedbackClosed = training.IsFeedbackClosed ?? false,
                HrAccess=1,
                EmpAccess=1,
                MgAccess=1


            };

            var allQuestions = dbContext.SettingsTrainingFeedbackQuestion.Where(var => var.IsActive).ToList();
            var trainingEffectiveness = new List<TrainingEffectivenessDto>();
            var currentNominee = training.TrainingNominees
                    .FirstOrDefault(var => var.EmployeeId == request.UserIdNum && !var.IsRejected);

            response.SelfFeedback = new List<TrainingQuestionDto>();

            foreach (var question in allQuestions)
            {
                var answer = training.TrainingFeedback
                    .Where(var => var.QuestionId == question.Id)
                    .ToList();

                trainingEffectiveness.Add(new TrainingEffectivenessDto
                {
                    QuestionId = question.Id,
                    Question = question.Question,
                    Answer = answer.Any()
                        ? Math.Round(
                                answer
                                    .Average(var => Convert.ToDouble(var.Answer ?? "0")), 1)
                            .ToString(CultureInfo.InvariantCulture)
                        : "0"
                });

                if (!request.GetAllFeedbacks && currentNominee != null)
                {
                    var currentNomineeFeedback = currentNominee.TrainingFeedback.FirstOrDefault(var => var.QuestionId == question.Id);
                    response.SelfFeedback.Add(new TrainingQuestionDto
                    {
                        Question = question.Question,
                        QuestionId = question.Id,
                        Answer = currentNomineeFeedback != null ? currentNomineeFeedback.Answer : null,
                    });
                    response.FeedbackContent = currentNominee.FeedbackContent;
                }
            }

            response.Effectiveness = trainingEffectiveness;

            response.Feedbacks = request.GetAllFeedbacks
                ? training.TrainingNominees
                .Select(var => new TrainingQuestionDto
                {
                    NomineeId = var.Guid,
                    EmployeeId = var.Employee.Guid,
                    Name = var.Employee.Name,
                    Answer = var.TrainingFeedback != null ? var.TrainingFeedback.FirstOrDefault()?.Answer : null,
                    Question = var.TrainingFeedback != null ? var.TrainingFeedback.FirstOrDefault()?.Question.Question : null,
                    QuestionId = var.TrainingFeedback != null ? var.TrainingFeedback.FirstOrDefault()?.QuestionId : null,
                }).ToList()
                : null;

            response.Nominees = training.TrainingNominees
                .Select(var => new TrainingNomineeDto
                {
                    EmployeeId = var.Employee.Guid,
                    Name = var.Employee.Name,
                    IsSelf = var.EmployeeId == request.UserIdNum,
                    HrName = var.Hr == null ? string.Empty : var.Hr.Name,
                    HrUpdatedOn = var.HrUpdatedOn,
                    IsHrAccepted = var.HrAccepted,
                    IsMangerAccepted = var.ManagerAccepted,
                    IsSelfAccepted = var.SelfAccepted,
                    ManagerUpdatedOn = var.ManagerUpdatedOn,
                    ManagerName = var.Manager == null ? string.Empty : var.Manager.Name,
                    NomineeId = var.Guid,
                    RejectionReason = var.RejectedReason,
                    SelfUpdatedOn = var.SelfUpdatedOn,
                    IsRejected = var.IsRejected,
                    IsFeedbackDone = var.TrainingFeedback != null ? var.TrainingFeedback.Any() : false,
                    FeedbackContent = var.FeedbackContent,
                    Code = var.Employee.EmployeeCompanyEmployee.FirstOrDefault()?.EmployeeCode,
                    FeedbackRating = var.TrainingFeedback != null && var.TrainingFeedback.Any()
                                     ? Math.Round(var.TrainingFeedback.Average(var1 => Convert.ToDouble(var1.Answer ?? "0")), 1)
                                     : 0
                }).ToList();

            response.Attendance = training.TrainingAttendance.Select(var => new TrainingAttendanceDto
            {
                Date = var.TrainingDateNavigation.Date,
                Remark = var.Remark,
                IsAttended = var.HasAttended,
                NomineeId = var.TrainingNomineeNavigation.Guid,
                EmployeeName = var.TrainingNomineeNavigation.Employee.Name
               
            }).ToList();

            return response;
        }

        public BaseResponse CancelTraining(TrainingCancellationActionRequest request)
        {
            var training = GetAll()
                .FirstOrDefault(var =>
                    var.Guid.Equals(request.TrainingId) && var.IsActive && !(var.IsStarted ?? false) &&
                    !(var.IsCompleted ?? false));

            if (training == null)
            {
                throw new Exception("Training not found or is completed.");
            }

            training.IsActive = false;
            training.CancellationReason = request.Reason;
            Save();

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse StartTraining(TrainingActionRequest request)
        {
            var training = GetAll()
                .FirstOrDefault(var =>
                    var.Guid.Equals(request.TrainingId) && var.IsActive && (var.IsConfirmed ?? false) &&
                    !(var.IsCompleted ?? false) && !(var.IsStarted ?? false));

            if (training == null)
            {
                throw new Exception("Training not found or is completed.");
            }

            training.IsStarted = true;

            Save();

            return new BaseResponse
            {
                IsSuccess = true
            };
        }
        public BaseResponse CloseFeedbackForTraining(TrainingActionRequest request)
        {
            var training = GetAll()
                .FirstOrDefault(var =>
                    var.Guid.Equals(request.TrainingId) && var.IsActive && (var.IsConfirmed ?? false) &&
                    (var.IsCompleted ?? false) && (var.IsStarted ?? false));

            if (training == null)
            {
                throw new Exception("Training not found or is completed.");
            }

            training.IsFeedbackClosed = true;

            Save();

            return new BaseResponse
            {
                IsSuccess = true
            };
        }
    }
}
