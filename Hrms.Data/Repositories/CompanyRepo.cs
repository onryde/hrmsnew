using System;
using System.Collections.Generic;
using System.IO;
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
    public class CompanyRepo : BaseRepository<AppCompany>, ICompanyRepo
    {
        private IEventLogRepo _eventLogRepo;
        public CompanyRepo(HrmsContext context, IEventLogRepo eventLogRepo) : base(context)
        {
            _eventLogRepo = eventLogRepo;
        }

        public UpdateCompanySettingsResponse GetAllSettings(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsDepartmentDesignation)
                .Include(var => var.SettingsGrade).ThenInclude(var => var.EmployeeCompany)
                .ThenInclude(var => var.Employee)
                .Include(var => var.SettingsLocation).ThenInclude(var => var.EmployeeCompany)
                .ThenInclude(var => var.Employee)
                .Include(var => var.SettingsTicketCategory).ThenInclude(var => var.SettingsTicketSubCategory)
                .Include(var => var.SettingsTicketCategory).ThenInclude(var => var.SettingsTicketCategoryOwner).ThenInclude(var => var.Employee)
                .Include(var => var.SettingsCategory).ThenInclude(var => var.EmployeeCompany)
                .ThenInclude(var => var.Employee)
                .Include(var => var.SettingsDepartment).ThenInclude(var => var.EmployeeCompany)
                .ThenInclude(var => var.Employee)
                .Include(var => var.SettingsDepartment).ThenInclude(var => var.SettingsDepartmentDesignation)
                .ThenInclude(var => var.EmployeeCompany).ThenInclude(var => var.Employee)
                .Include(var => var.SettingsDepartment).ThenInclude(var => var.SettingsDepartmentDesignation)
                .ThenInclude(var => var.ReportingTo)
                .Include(var => var.SettingsHoliday).ThenInclude(var => var.SettingsHolidayLocation)
                .Include(var => var.SettingsRegion).ThenInclude(var => var.EmployeeCompany)
                .ThenInclude(var => var.Employee)
                .Include(var => var.SettingsTeam).ThenInclude(var => var.EmployeeCompany)
                .ThenInclude(var => var.Employee)
                .Include(var => var.SettingsAnnouncementType).ThenInclude(var => var.Announcement)
                .Include(var => var.SettingsAppraisalQuestion).ThenInclude(var => var.AppraisalQuestion).ThenInclude(var => var.Appraisal)
                .Include(var => var.SettingsTicketFaq)
                .Include(var => var.SettingsProductLine)
                .Include(var => var.SettingsDocumentType)
                .Include(var => var.SettingsAssetTypes).ThenInclude(var => var.SettingsAssetTypeOwner).ThenInclude(var => var.Owner)
                .Include(var => var.SettingsAssetTypes).ThenInclude(var => var.EmployeeAsset)
                .FirstOrDefault();

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetCompanySettings.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetCompanySettings.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new UpdateCompanySettingsResponse
            {
                IsSuccess = true,
                Locations = company
                    .SettingsLocation.Where(var => var.IsActive)
                    .Select(var => new LocationDto
                    {
                        GstNumber = var.GstNumber,
                        Address = var.Address,
                        State = var.State,
                        Email = var.Email,
                        Phone = var.Phone,
                        Country = var.Country,
                        EmployeesCount = var.EmployeeCompany.Count(var1 => var1.Employee.IsActive),
                        Location = var.Name,
                        IsActive = true,
                        LocationId = var.Guid
                    }).ToList(),
                AnnouncementTypes = company
                    .SettingsAnnouncementType.Where(var => var.IsActive)
                    .Select(var => new AnnouncementTypeDto
                    {
                        Description = var.Description,
                        AnnouncementType = var.Type,
                        IsActive = true,
                        AnnouncementCount = var.Announcement.Count(var1 => var1.IsActive),
                        AnnouncementTypeId = var.Guid
                    }).ToList(),
                Categories = company
                    .SettingsCategory.Where(var => var.IsActive)
                    .Select(var => new CategoryDto()
                    {
                        Description = var.Description,
                        Category = var.Category,
                        IsActive = true,
                        CategoryId = var.Guid,
                        EmployeesCount = var.EmployeeCompany.Count(var1 => var1.Employee.IsActive),
                    }).ToList(),
                DocumentTypes = company
                    .SettingsDocumentType.Where(var => var.IsActive)
                    .Select(var => new DocumentTypeDto()
                    {
                        Description = var.Description,
                        DocumentType = var.DocumentType,
                        IsActive = true,
                        DocumentTypeId = var.Guid,
                        IsRestricted = var.IsRestricted
                    }).ToList(),
                Grades = company
                    .SettingsGrade.Where(var => var.IsActive)
                    .Select(var => new GradeDto()
                    {
                        Description = var.Description,
                        Grade = var.Grade,
                        IsActive = true,
                        GradeId = var.Guid,
                        EmployeesCount = var.EmployeeCompany.Count(var1 => var1.Employee.IsActive),
                    }).ToList(),
                ProductLines = company
                    .SettingsProductLine.Where(var => var.IsActive)
                    .Select(var => new ProductLineDto
                    {
                        Description = var.Description,
                        ProductLine = var.ProductLine,
                        IsActive = true,
                        ProductLineId = var.Guid,
                    }).ToList(),
                Regions = company
                    .SettingsRegion.Where(var => var.IsActive)
                    .Select(var => new RegionDto()
                    {
                        Description = var.Description,
                        Region = var.Name,
                        IsActive = true,
                        RegionId = var.Guid,
                        EmployeesCount = var.EmployeeCompany.Count(var1 => var1.Employee.IsActive),
                    }).ToList(),
                Teams = company
                    .SettingsTeam.Where(var => var.IsActive)
                    .Select(var => new TeamDto()
                    {
                        Description = var.Description,
                        Team = var.Name,
                        IsActive = true,
                        TeamId = var.Guid,
                        EmployeesCount = var.EmployeeCompany.Count(var1 => var1.Employee.IsActive),
                    }).ToList(),
                TicketFaqs = company
                    .SettingsTicketFaq.Where(var => var.IsActive)
                    .Select(var => new TicketFaqDto
                    {
                        Description = var.Explanation,
                        FaqTitle = var.Title,
                        IsActive = true,
                        TicketFaqId = var.Guid,
                    }).ToList(),
                Holidays = company
                    .SettingsHoliday.Where(var => var.IsActive)
                    .Select(var => new HolidayDto
                    {
                        Reason = var.Reason,
                        Date = var.Date,
                        IsActive = true,
                        HolidayId = var.Guid,
                        Type = var.HolidayType,
                        LocationIds = var.SettingsHolidayLocation.Any()
                            ? var.SettingsHolidayLocation.Select(var1 => var1.Location.Guid).ToList()
                            : null
                    })
                    .OrderBy(var => var.Date)
                    .ToList(),
                TicketCategories = company.SettingsTicketCategory
                    .Where(var => var.IsActive)
                    .Select(var => new TicketCategoryDto
                    {
                        Description = var.Description,
                        Name = var.Name,
                        IsActive = var.IsActive,
                        TicketsCount = 0,
                        TicketCategoryId = var.Guid,
                        Owners = var.SettingsTicketCategoryOwner != null
                        ? var.SettingsTicketCategoryOwner.Select(var1 => var1.Employee.Guid).ToList()
                        : null,
                        TicketSubCategories = var.SettingsTicketSubCategory
                            .Where(var1 => var1.IsActive)
                            .Select(var1 => new TicketSubCategoryDto
                            {
                                Description = var1.Description,
                                Name = var1.Name,
                                IsActive = var1.IsActive,
                                TicketsCount = 0,
                                TicketSubCategoryId = var1.Guid
                            }).ToList()
                    }).ToList(),
                Departments = company.SettingsDepartment
                    .Where(var => var.IsActive)
                    .Select(var => new DepartmentDto
                    {
                        Description = var.Description,
                        Name = var.Name,
                        DepartmentId = var.Guid,
                        EmployeesCount = var.EmployeeCompany.Count(var1 => var1.Employee.IsActive),
                        IsActive = var.IsActive,
                    })
                    .ToList(),
                Designations = company.SettingsDepartmentDesignation
                    .Where(var1 => var1.IsActive)
                    .Select(var1 => new DesignationDto
                    {
                        Description = var1.Description,
                        Name = var1.Name,
                        DesignationId = var1.Guid,
                        EmployeesCount = var1.EmployeeCompany.Count(var2 => var2.Employee.IsActive),
                        IsActive = var1.IsActive,
                    })
                    .ToList(),
                Company = new CompanyDto
                {
                    Name = company.Name,
                    Address = company.Address,
                    GstNumber = company.GstNumber,
                    PanNumber = company.PanNumber,
                    Email = company.Email,
                    Phone = company.Phone,
                    FullLogo = company.FullLogo,
                    SmallLogo = company.SmallLogo
                },
                AssetTypes = company.SettingsAssetTypes
                .Where(var => var.IsActive)
                .Select(var => new AssetTypeDto
                {
                    AssetType = var.AssetType,
                    AssetTypeId = var.Guid,
                    Description = var.Description,
                    IsActive = true,
                    EmployeesActive = var.EmployeeAsset.Count(var1 => var1.IsActive),
                    SigningAuthorities = var.SettingsAssetTypeOwner.Select(var1 => var1.Owner.Guid).ToList()
                })
                .ToList()
            };
        }

        public UpdateCompanySettingsResponse UpdateHolidays(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsLocation)
                .Include(var => var.SettingsHoliday).ThenInclude(var => var.SettingsHolidayLocation)
                .FirstOrDefault();

            foreach (var holiday in request.Holidays)
            {
                var locations = holiday.LocationIds != null && holiday.LocationIds.Any()
                    ? company.SettingsLocation.Where(var => holiday.LocationIds.Contains(var.Guid))
                    : new List<SettingsLocation>();

                if (string.IsNullOrEmpty(holiday.HolidayId) && holiday.IsActive)
                {
                    company.SettingsHoliday.Add(new SettingsHoliday
                    {
                        Company = company,
                        Reason = holiday.Reason,
                        Guid = CustomGuid.NewGuid(),
                        Date = holiday.Date,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true,
                        HolidayType = holiday.Type,
                        SettingsHolidayLocation = locations
                            .Select((var => new SettingsHolidayLocation
                            {
                                Location = var,
                                Company = company,
                            }))
                            .ToList()
                    });
                }
                else
                {
                    var addedHoliday =
                        company.SettingsHoliday.FirstOrDefault(var =>
                            var.Guid.Equals(holiday.HolidayId) && var.IsActive);
                    if (addedHoliday != null)
                    {
                        if (!holiday.IsActive)
                        {
                            addedHoliday.IsActive = false;
                            addedHoliday.UpdatedBy = request.UserIdNum;
                            addedHoliday.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedHoliday.Reason = holiday.Reason;
                            addedHoliday.Date = holiday.Date.Date;
                            addedHoliday.UpdatedBy = request.UserIdNum;
                            addedHoliday.UpdatedOn = DateTime.UtcNow;
                            addedHoliday.HolidayType = holiday.Type;

                            dbContext.SettingsHolidayLocation.RemoveRange(company.SettingsHolidayLocation);

                            addedHoliday.SettingsHolidayLocation = locations
                                .Select((var => new SettingsHolidayLocation
                                {
                                    Location = var,
                                    Company = company,
                                }))
                                .ToList();
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateHolidaysSettings.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetCompanySettings.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateDepartments(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsDepartment)
                .ThenInclude(var => var.SettingsDepartmentDesignation)
                .Include(var => var.SettingsDepartmentDesignation)
                .FirstOrDefault();

            #region Department Update

            foreach (var department in request.Departments)
            {
                if (string.IsNullOrEmpty(department.DepartmentId))
                {
                    var newDepartment = new SettingsDepartment
                    {
                        Company = company,
                        Guid = CustomGuid.NewGuid(),
                        Name = department.Name,
                        Description = department.Description,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true,
                    };

                    company.SettingsDepartment.Add(newDepartment);

                    //
                    // foreach (var designation in department.Designations)
                    // {
                    //     var newDesignation = new SettingsDepartmentDesignation
                    //     {
                    //         Company = company,
                    //         Guid = CustomGuid.NewGuid(),
                    //         Name = designation.Name,
                    //         Description = designation.Description,
                    //         AddedBy = request.UserIdNum,
                    //         AddedOn = DateTime.UtcNow,
                    //         Order = designation.Order,
                    //         IsActive = true
                    //     };
                    //
                    //     newDepartment.SettingsDepartmentDesignation.Add(newDesignation);
                    //     Save();
                    //
                    //     designation.DesignationId = newDesignation.Guid;
                    // }
                }
                else
                {
                    var addedDepartment =
                        company.SettingsDepartment.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(department.DepartmentId));
                    if (addedDepartment != null)
                    {
                        if (!department.IsActive)
                        {
                            addedDepartment.IsActive = false;
                            addedDepartment.UpdatedBy = request.UserIdNum;
                            addedDepartment.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedDepartment.Name = department.Name;
                            addedDepartment.Description = department.Description;
                            addedDepartment.UpdatedBy = request.UserIdNum;
                            addedDepartment.UpdatedOn = DateTime.UtcNow;
                            //
                            // #region Designations Updates
                            //
                            // foreach (var designation in department.Designations)
                            // {
                            //     if (string.IsNullOrEmpty(designation.DesignationId))
                            //     {
                            //         var newDesignation = new SettingsDepartmentDesignation
                            //         {
                            //             Company = company,
                            //             Guid = CustomGuid.NewGuid(),
                            //             Name = designation.Name,
                            //             Description = designation.Description,
                            //             AddedBy = request.UserIdNum,
                            //             AddedOn = DateTime.UtcNow,
                            //             Order = designation.Order,
                            //             IsActive = true
                            //         };
                            //
                            //         addedDepartment.SettingsDepartmentDesignation.Add(newDesignation);
                            //         Save();
                            //
                            //         designation.DesignationId = newDesignation.Guid;
                            //     }
                            //     else
                            //     {
                            //         var addedDesignation =
                            //             addedDepartment.SettingsDepartmentDesignation.FirstOrDefault(var =>
                            //                 var.IsActive && var.Guid.Equals(designation.DesignationId));
                            //         if (addedDesignation != null)
                            //         {
                            //             if (!designation.IsActive)
                            //             {
                            //                 addedDesignation.IsActive = false;
                            //                 addedDesignation.UpdatedBy = request.UserIdNum;
                            //                 addedDesignation.UpdatedOn = DateTime.UtcNow;
                            //             }
                            //             else
                            //             {
                            //                 addedDesignation.Order = designation.Order;
                            //                 addedDesignation.Name = designation.Name;
                            //                 addedDesignation.Description = designation.Description;
                            //                 addedDesignation.UpdatedBy = request.UserIdNum;
                            //                 addedDesignation.UpdatedOn = DateTime.UtcNow;
                            //             }
                            //         }
                            //     }
                            // }
                            //
                            // #endregion
                        }
                    }
                }

                //
                // foreach (var designation in department.Designations)
                // {
                //     var addedDesignation =
                //         company.SettingsDepartmentDesignation.FirstOrDefault(var =>
                //             var.IsActive && var.Guid.Equals(designation.DesignationId));
                //
                //     if (!string.IsNullOrWhiteSpace(designation.ReportingTo))
                //     {
                //         var reportingTo =
                //             department.Designations.FirstOrDefault(var =>
                //                 var.RefId.ToString() == designation.ReportingTo
                //                 || var.DesignationId.Equals(designation.ReportingTo));
                //
                //         if (reportingTo != null)
                //         {
                //             var reportingDesignation =
                //                 company.SettingsDepartmentDesignation.FirstOrDefault(var =>
                //                     var.IsActive && var.Guid.Equals(reportingTo.DesignationId));
                //
                //             if (addedDesignation != null && reportingDesignation != null)
                //             {
                //                 addedDesignation.ReportingTo = reportingDesignation;
                //             }
                //         }
                //     }
                //     else
                //     {
                //         addedDesignation.ReportingTo = null;
                //     }
                // }

                Save();
            }

            #endregion

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateDepartments.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateDepartments.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateLocations(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsLocation)
                .ThenInclude(var => var.SettingsHolidayLocation)
                .ThenInclude(var => var.Holiday)
                .ThenInclude(var => var.SettingsHolidayLocation)
                .FirstOrDefault();

            foreach (var location in request.Locations)
            {
                // Newly added locations
                if (string.IsNullOrEmpty(location.LocationId) && location.IsActive)
                {
                    company.SettingsLocation.Add(new SettingsLocation
                    {
                        Company = company,
                        GstNumber = location.GstNumber,
                        Address = location.Address,
                        State = location.State,
                        Email = location.Email,
                        Phone = location.Phone,
                        Country = location.Country,
                        Guid = CustomGuid.NewGuid(),
                        Name = location.Location,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }
                else
                {
                    var addedLocation =
                        company.SettingsLocation.FirstOrDefault(var =>
                            var.Guid.Equals(location.LocationId) && var.IsActive);
                    if (addedLocation != null)
                    {
                        // If the location is deleted or updated
                        if (!location.IsActive)
                        {
                            addedLocation.IsActive = false;
                            addedLocation.UpdatedBy = request.UserIdNum;
                            addedLocation.UpdatedOn = DateTime.UtcNow;

                            dbContext.SettingsHolidayLocation.RemoveRange(addedLocation.SettingsHolidayLocation);
                            foreach (var holidayLocation in addedLocation.SettingsHolidayLocation)
                            {
                                var otherLocationsPresent = holidayLocation.Holiday.SettingsHolidayLocation
                                    .Any(var => var.Location != addedLocation);
                                if (!otherLocationsPresent)
                                {
                                    holidayLocation.Holiday.IsActive = false;
                                    holidayLocation.Holiday.UpdatedBy = request.UserIdNum;
                                    holidayLocation.Holiday.UpdatedOn = DateTime.UtcNow;
                                }
                            }
                        }
                        else
                        {
                            addedLocation.Name = location.Location;
                            addedLocation.GstNumber = location.GstNumber;
                            addedLocation.Address = location.Address;
                            addedLocation.State = location.State;
                            addedLocation.Email = location.Email;
                            addedLocation.Phone = location.Phone;
                            addedLocation.Country = location.Country;
                            addedLocation.UpdatedBy = request.UserIdNum;
                            addedLocation.UpdatedOn = DateTime.UtcNow;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateLocations.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateLocations.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateTicketCategory(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsTicketCategory).ThenInclude(var => var.SettingsTicketSubCategory)
                .Include(var => var.SettingsTicketCategory).ThenInclude(var => var.SettingsTicketCategoryOwner)
                .FirstOrDefault();

            #region Category Update

            foreach (var category in request.TicketCategories)
            {

                // Newly added category
                if (string.IsNullOrEmpty(category.TicketCategoryId))
                {
                    var owners = category.Owners != null
                        ? dbContext.Employee
                            .Where(var => var.IsActive && category.Owners.Contains(var.Guid))
                            .Select(var => new SettingsTicketCategoryOwner
                            {
                                Company = company,
                                EmployeeId = var.Id,
                            }).ToList()
                        : null;

                    company.SettingsTicketCategory.Add(new SettingsTicketCategory
                    {
                        Company = company,
                        Guid = CustomGuid.NewGuid(),
                        Name = category.Name,
                        Description = category.Description,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true,
                        SettingsTicketCategoryOwner = owners,
                        SettingsTicketSubCategory = category.TicketSubCategories != null
                        ? category.TicketSubCategories.Select(var =>
                            new SettingsTicketSubCategory
                            {
                                Description = var.Description,
                                Guid = CustomGuid.NewGuid(),
                                Name = var.Name,
                                AddedBy = request.UserIdNum,
                                AddedOn = DateTime.UtcNow,
                                IsActive = true,
                                Company = company,
                            }).ToList()
                        : null
                    });
                }
                else
                {
                    var addedCategory =
                        company.SettingsTicketCategory.FirstOrDefault(var =>
                            var.IsActive && var.Guid.Equals(category.TicketCategoryId));
                    if (addedCategory != null)
                    {
                        var owners = dbContext.Employee
                            .Where(var => var.IsActive && category.Owners.Contains(var.Guid))
                            .Select(var => new SettingsTicketCategoryOwner
                            {
                                Company = company,
                                EmployeeId = var.Id,
                                Category = addedCategory
                            })
                            .ToList();

                        // If the location is deleted or updated
                        if (!category.IsActive)
                        {
                            addedCategory.IsActive = false;
                            addedCategory.UpdatedBy = request.UserIdNum;
                            addedCategory.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            dbContext.SettingsTicketCategoryOwner.RemoveRange(addedCategory.SettingsTicketCategoryOwner);

                            addedCategory.Name = category.Name;
                            addedCategory.Description = category.Description;
                            addedCategory.UpdatedBy = request.UserIdNum;
                            addedCategory.UpdatedOn = DateTime.UtcNow;
                            addedCategory.SettingsTicketCategoryOwner = owners;

                            #region Subcategory Updates

                            foreach (var subCategory in category.TicketSubCategories)
                            {
                                if (string.IsNullOrEmpty(subCategory.TicketSubCategoryId))
                                {
                                    addedCategory.SettingsTicketSubCategory.Add(new SettingsTicketSubCategory
                                    {
                                        Company = company,
                                        Guid = CustomGuid.NewGuid(),
                                        Name = subCategory.Name,
                                        Description = subCategory.Description,
                                        AddedBy = request.UserIdNum,
                                        AddedOn = DateTime.UtcNow,
                                        IsActive = true
                                    });
                                }
                                else
                                {
                                    var addedSubCategory =
                                        addedCategory.SettingsTicketSubCategory.FirstOrDefault(var =>
                                            var.IsActive && var.Guid.Equals(subCategory.TicketSubCategoryId));
                                    if (addedSubCategory != null)
                                    {
                                        if (!subCategory.IsActive)
                                        {
                                            addedSubCategory.IsActive = false;
                                            addedSubCategory.UpdatedBy = request.UserIdNum;
                                            addedSubCategory.UpdatedOn = DateTime.UtcNow;
                                        }
                                        else
                                        {
                                            addedSubCategory.Name = subCategory.Name;
                                            addedSubCategory.Description = subCategory.Description;
                                            addedSubCategory.UpdatedBy = request.UserIdNum;
                                            addedSubCategory.UpdatedOn = DateTime.UtcNow;
                                        }
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }
            }

            #endregion

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateTicketCategory.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateTicketCategory.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateGrades(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsGrade)
                .FirstOrDefault();

            foreach (var grade in request.Grades)
            {
                // Newly added productLines
                if (string.IsNullOrEmpty(grade.GradeId))
                {
                    company.SettingsGrade.Add(new SettingsGrade
                    {
                        Company = company,
                        Description = grade.Description,
                        Guid = CustomGuid.NewGuid(),
                        Grade = grade.Grade,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }
                else
                {
                    var addedGrade =
                        company.SettingsGrade.FirstOrDefault(var =>
                            var.Guid.Equals(grade.GradeId) && var.IsActive);
                    if (addedGrade != null)
                    {
                        // If the productLine is deleted or updated
                        if (!grade.IsActive)
                        {
                            addedGrade.IsActive = false;
                            addedGrade.UpdatedBy = request.UserIdNum;
                            addedGrade.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedGrade.Grade = grade.Grade;
                            addedGrade.Description = grade.Description;
                            addedGrade.UpdatedBy = request.UserIdNum;
                            addedGrade.UpdatedOn = DateTime.UtcNow;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateGrades.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateGrades.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateDesignations(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsDepartmentDesignation)
                .FirstOrDefault();

            foreach (var designation in request.Designations)
            {
                if (string.IsNullOrEmpty(designation.DesignationId))
                {
                    company.SettingsDepartmentDesignation.Add(new SettingsDepartmentDesignation()
                    {
                        Company = company,
                        Description = designation.Description,
                        Guid = CustomGuid.NewGuid(),
                        Name = designation.Name,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }
                else
                {
                    var addedDesignation =
                        company.SettingsDepartmentDesignation.FirstOrDefault(var =>
                            var.Guid.Equals(designation.DesignationId) && var.IsActive);
                    if (addedDesignation != null)
                    {
                        if (!designation.IsActive)
                        {
                            addedDesignation.IsActive = false;
                            addedDesignation.UpdatedBy = request.UserIdNum;
                            addedDesignation.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedDesignation.Name = designation.Name;
                            addedDesignation.Description = designation.Description;
                            addedDesignation.UpdatedBy = request.UserIdNum;
                            addedDesignation.UpdatedOn = DateTime.UtcNow;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateDesignations.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateDesignations.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateProductLines(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsProductLine)
                .FirstOrDefault();

            foreach (var productLine in request.ProductLines)
            {
                // Newly added productLines
                if (string.IsNullOrEmpty(productLine.ProductLineId))
                {
                    company.SettingsProductLine.Add(new SettingsProductLine
                    {
                        Company = company,
                        Description = productLine.Description,
                        Guid = CustomGuid.NewGuid(),
                        ProductLine = productLine.ProductLine,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }
                else
                {
                    var addedProductLine =
                        company.SettingsProductLine.FirstOrDefault(var =>
                            var.Guid.Equals(productLine.ProductLineId) && var.IsActive);
                    if (addedProductLine != null)
                    {
                        // If the productLine is deleted or updated
                        if (!productLine.IsActive)
                        {
                            addedProductLine.IsActive = false;
                            addedProductLine.UpdatedBy = request.UserIdNum;
                            addedProductLine.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedProductLine.ProductLine = productLine.ProductLine;
                            addedProductLine.Description = productLine.Description;
                            addedProductLine.UpdatedBy = request.UserIdNum;
                            addedProductLine.UpdatedOn = DateTime.UtcNow;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateProductLines.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateProductLines.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateTicketFaq(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsTicketCategory)
                .Include(var => var.SettingsTicketSubCategory)
                .Include(var => var.SettingsTicketFaq)
                .FirstOrDefault();

            foreach (var faq in request.TicketFaqs)
            {
                long? ticketCategoryId = null;
                long? ticketSubCategoryId = null;

                if (!string.IsNullOrWhiteSpace(faq.TicketCategoryId))
                {
                    var ticketCategory = company.SettingsTicketCategory.FirstOrDefault(var =>
                        var.Guid.Equals(faq.TicketCategoryId) && var.IsActive);
                    if (ticketCategory != null)
                        ticketCategoryId = ticketCategory.Id;
                }

                if (!string.IsNullOrWhiteSpace(faq.TicketSubCategoryId))
                {
                    var ticketSubCategory = company.SettingsTicketSubCategory.FirstOrDefault(var =>
                        var.Guid.Equals(faq.TicketSubCategoryId) && var.IsActive);
                    if (ticketSubCategory != null)
                        ticketSubCategoryId = ticketSubCategory.Id;
                }

                // Newly added faqs
                if (string.IsNullOrEmpty(faq.TicketFaqId))
                {
                    company.SettingsTicketFaq.Add(new SettingsTicketFaq
                    {
                        Company = company,
                        Explanation = faq.Description,
                        Guid = CustomGuid.NewGuid(),
                        Title = faq.FaqTitle,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        TicketCategoryId = ticketCategoryId,
                        TicketSubCategoryId = ticketSubCategoryId,
                        IsActive = true,
                    });
                }
                else
                {
                    var addedFaq =
                        company.SettingsTicketFaq.FirstOrDefault(
                            var => var.Guid.Equals(faq.TicketFaqId) && var.IsActive);
                    if (addedFaq != null)
                    {
                        // If the faq is deleted or updated
                        if (!faq.IsActive)
                        {
                            addedFaq.IsActive = false;
                            addedFaq.UpdatedBy = request.UserIdNum;
                            addedFaq.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedFaq.Title = faq.FaqTitle;
                            addedFaq.Explanation = faq.Description;
                            addedFaq.UpdatedBy = request.UserIdNum;
                            addedFaq.UpdatedOn = DateTime.UtcNow;
                            addedFaq.TicketCategoryId = ticketCategoryId;
                            addedFaq.TicketSubCategoryId = ticketSubCategoryId;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateTicketFaq.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateTicketFaq.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateCategories(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsCategory)
                .FirstOrDefault();

            foreach (var category in request.Categories)
            {
                // Newly added productLines
                if (string.IsNullOrEmpty(category.CategoryId))
                {
                    company.SettingsCategory.Add(new SettingsCategory
                    {
                        Company = company,
                        Description = category.Description,
                        Guid = CustomGuid.NewGuid(),
                        Category = category.Category,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }
                else
                {
                    var addedCategory =
                        company.SettingsCategory.FirstOrDefault(var =>
                            var.Guid.Equals(category.CategoryId) && var.IsActive);
                    if (addedCategory != null)
                    {
                        // If the productLine is deleted or updated
                        if (!category.IsActive)
                        {
                            addedCategory.IsActive = false;
                            addedCategory.UpdatedBy = request.UserIdNum;
                            addedCategory.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedCategory.Category = category.Category;
                            addedCategory.Description = category.Description;
                            addedCategory.UpdatedBy = request.UserIdNum;
                            addedCategory.UpdatedOn = DateTime.UtcNow;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateCategories.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateCategories.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateRegions(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsRegion)
                .FirstOrDefault();

            foreach (var region in request.Regions)
            {
                // Newly added productLines
                if (string.IsNullOrEmpty(region.RegionId))
                {
                    company.SettingsRegion.Add(new SettingsRegion
                    {
                        Company = company,
                        Description = region.Description,
                        Guid = CustomGuid.NewGuid(),
                        Name = region.Region,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }
                else
                {
                    var addedRegion =
                        company.SettingsRegion.FirstOrDefault(var =>
                            var.Guid.Equals(region.RegionId) && var.IsActive);
                    if (addedRegion != null)
                    {
                        // If the productLine is deleted or updated
                        if (!region.IsActive)
                        {
                            addedRegion.IsActive = false;
                            addedRegion.UpdatedBy = request.UserIdNum;
                            addedRegion.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedRegion.Name = region.Region;
                            addedRegion.Description = region.Description;
                            addedRegion.UpdatedBy = request.UserIdNum;
                            addedRegion.UpdatedOn = DateTime.UtcNow;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateRegions.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateRegions.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateTeams(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsTeam)
                .FirstOrDefault();

            foreach (var team in request.Teams)
            {
                // Newly added teams
                if (string.IsNullOrEmpty(team.TeamId))
                {
                    company.SettingsTeam.Add(new SettingsTeam
                    {
                        Company = company,
                        Description = team.Description,
                        Guid = CustomGuid.NewGuid(),
                        Name = team.Team,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }
                else
                {
                    var addedTeam =
                        company.SettingsTeam.FirstOrDefault(var =>
                            var.Guid.Equals(team.TeamId) && var.IsActive);
                    if (addedTeam != null)
                    {
                        // If the productLine is deleted or updated
                        if (!team.IsActive)
                        {
                            addedTeam.IsActive = false;
                            addedTeam.UpdatedBy = request.UserIdNum;
                            addedTeam.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedTeam.Name = team.Team;
                            addedTeam.Description = team.Description;
                            addedTeam.UpdatedBy = request.UserIdNum;
                            addedTeam.UpdatedOn = DateTime.UtcNow;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateTeams.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateTeams.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateDocumentTypes(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsDocumentType)
                .FirstOrDefault();

            foreach (var documentType in request.DocumentTypes)
            {
                // Newly added teams
                if (string.IsNullOrEmpty(documentType.DocumentTypeId))
                {
                    company.SettingsDocumentType.Add(new SettingsDocumentType
                    {
                        Company = company,
                        Description = documentType.Description,
                        Guid = CustomGuid.NewGuid(),
                        DocumentType = documentType.DocumentType,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsRestricted = documentType.IsRestricted,
                        IsActive = true
                    });
                }
                else
                {
                    var addedDocumentType =
                        company.SettingsDocumentType.FirstOrDefault(var =>
                            var.Guid.Equals(documentType.DocumentTypeId) && var.IsActive);
                    if (addedDocumentType != null)
                    {
                        // If the productLine is deleted or updated
                        if (!documentType.IsActive)
                        {
                            addedDocumentType.IsActive = false;
                            addedDocumentType.UpdatedBy = request.UserIdNum;
                            addedDocumentType.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedDocumentType.DocumentType = documentType.DocumentType;
                            addedDocumentType.Description = documentType.Description;
                            addedDocumentType.UpdatedBy = request.UserIdNum;
                            addedDocumentType.UpdatedOn = DateTime.UtcNow;
                            addedDocumentType.IsRestricted = documentType.IsRestricted;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateDocumentTypes.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateDocumentTypes.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateAnnouncementTypes(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsAnnouncementType)
                .FirstOrDefault();

            foreach (var announcementType in request.AnnouncementTypes)
            {
                // Newly added teams
                if (string.IsNullOrEmpty(announcementType.AnnouncementTypeId))
                {
                    company.SettingsAnnouncementType.Add(new SettingsAnnouncementType
                    {
                        Company = company,
                        Description = announcementType.Description,
                        Guid = CustomGuid.NewGuid(),
                        Type = announcementType.AnnouncementType,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }
                else
                {
                    var addedAnnouncementType =
                        company.SettingsAnnouncementType.FirstOrDefault(var =>
                            var.Guid.Equals(announcementType.AnnouncementTypeId) && var.IsActive);
                    if (addedAnnouncementType != null)
                    {
                        // If the productLine is deleted or updated
                        if (!announcementType.IsActive)
                        {
                            addedAnnouncementType.IsActive = false;
                            addedAnnouncementType.UpdatedBy = request.UserIdNum;
                            addedAnnouncementType.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedAnnouncementType.Type = announcementType.AnnouncementType;
                            addedAnnouncementType.Description = announcementType.Description;
                            addedAnnouncementType.UpdatedBy = request.UserIdNum;
                            addedAnnouncementType.UpdatedOn = DateTime.UtcNow;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateAnnouncementTypes.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateAnnouncementTypes.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public UpdateCompanySettingsResponse UpdateCompany(UpdateCompanyDetailsRequest request)
        {
            var company = GetAll()
                .FirstOrDefault();

            var fullLogoPath = "";
            if (request.FullLogoFile != null)
            {
                var path = Path.Combine(new string[]
                    {request.FileBasePath, "logo"});
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fileName = "full.png";

                using (var memoryStream = new MemoryStream())
                {
                    request.FullLogoFile.CopyTo(memoryStream);
                    File.WriteAllBytes(
                        string.Concat(path, "\\", fileName),
                        memoryStream.ToArray());
                }

                fullLogoPath = string.Concat(path, "\\", fileName);
            }

            var smallLogoPath = "";
            if (request.SmallLogoFile != null)
            {
                var path = Path.Combine(new string[]
                    {request.FileBasePath, "logo"});
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fileName = "small.png";

                using (var memoryStream = new MemoryStream())
                {
                    request.SmallLogoFile.CopyTo(memoryStream);
                    File.WriteAllBytes(string.Concat(path, "\\", fileName), memoryStream.ToArray());
                }

                smallLogoPath = string.Concat(path, "\\", fileName);
            }

            var alternateLogoPath = "";
            if (request.AlternateLogoFile != null)
            {
                var path = Path.Combine(new string[]
                    {request.FileBasePath, "logo"});
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fileName = "alternate.png";

                using (var memoryStream = new MemoryStream())
                {
                    request.AlternateLogoFile.CopyTo(memoryStream);
                    File.WriteAllBytes(string.Concat(path, "\\", fileName),
                        memoryStream.ToArray());
                }

                alternateLogoPath = string.Concat(path, "\\", fileName);
            }

            if (company != null)
            {
                company.Name = request.Name;
                company.Address = request.Address;
                company.Email = request.Email;
                company.Phone = request.Phone;
                company.GstNumber = request.GstNumber;
                company.PanNumber = request.PanNumber;
                company.FullLogo = fullLogoPath.Replace("C:\\\\", "").Replace("\\", "/");
                company.SmallLogo = smallLogoPath.Replace("C:\\\\", "").Replace("\\", "/");
                company.AlternateLogo = alternateLogoPath.Replace("C:\\\\", "").Replace("\\", "/");
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateCompanyDetails.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateCompanyDetails.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public GetAppraisalQuestionsResponse UpdateAppraisalQuestions(UpdateAppraisalQuestionsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsAppraisalQuestion)
                .FirstOrDefault();

            foreach (var question in request.AppraisalQuestions)
            {
                if (string.IsNullOrEmpty(question.QuestionId))
                {
                    company.SettingsAppraisalQuestion.Add(new SettingsAppraisalQuestion
                    {
                        Company = company,
                        Description = question.Description,
                        Guid = CustomGuid.NewGuid(),
                        Question = question.Question,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }
                else
                {
                    var addedQuestion =
                        company.SettingsAppraisalQuestion.FirstOrDefault(var =>
                            var.Guid.Equals(question.QuestionId) && var.IsActive);
                    if (addedQuestion != null)
                    {
                        if (!question.IsActive)
                        {
                            addedQuestion.IsActive = false;
                            addedQuestion.UpdatedBy = request.UserIdNum;
                            addedQuestion.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            addedQuestion.Question = question.Question;
                            addedQuestion.Description = question.Description;
                            addedQuestion.UpdatedBy = request.UserIdNum;
                            addedQuestion.UpdatedOn = DateTime.UtcNow;
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateAppraisalQuestions.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateAppraisalQuestions.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAppraisalQuestions(request);
        }

        public GetAppraisalQuestionsResponse GetAppraisalQuestions(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsAppraisalQuestion)
                .ThenInclude(var => var.AppraisalQuestion)
                .ThenInclude(var => var.Appraisal)
                .FirstOrDefault();

            var questions = company.SettingsAppraisalQuestion
                .Where(var => var.IsActive)
                .Select(var => new AppraisalQuestionDto
                {
                    Description = var.Description,
                    Question = var.Question,
                    IsActive = var.IsActive,
                    QuestionId = var.Guid,
                    AppraisalsCount = var.AppraisalQuestion.Count(var1 =>
                        var1.IsActive && var1.Appraisal.IsActive && var1.Appraisal.IsOpen &&
                        var1.Appraisal.EndDate >= DateTime.Today)
                })
                .ToList();

            return new GetAppraisalQuestionsResponse
            {
                AppraisalQuestions = questions,
                IsSuccess = true
            };
        }

        public GetAppraisalRatingsResponse GetAppraisalRatings(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsAppraisalRatings)
                .FirstOrDefault();

            var questions = company.SettingsAppraisalRatings
                .Where(var => var.IsActive)
                .Select(var => new AppraisalRatingDto()
                {
                    Description = var.Description,
                    Rating = var.RatingTitle,
                    Tag = var.RatingTag,
                    RatingId = var.Guid,
                    Score = var.RatingTotal ?? 0
                })
                .ToList();

            return new GetAppraisalRatingsResponse
            {
                AppraisalRatings = questions,
                IsSuccess = true
            };
        }

        public GetTicketFaqResponse GetTicketFaqs(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsTicketFaq)
                .FirstOrDefault();

            return new GetTicketFaqResponse()
            {
                IsSuccess = true,
                TicketFaqs = company
                    .SettingsTicketFaq
                    .Where(var => var.IsActive)
                    .Select(var => new TicketFaqDto
                    {
                        Description = var.Explanation,
                        FaqTitle = var.Title,
                        IsActive = true,
                        TicketFaqId = var.Guid,
                    })
                    .OrderBy(var => var.FaqTitle)
                    .ToList(),
            };
        }

        public SelectOptionResponse GetLocationsForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsLocation)
                .FirstOrDefault();

            var locations = company.SettingsLocation
                .Where(var => var.IsActive)
                .Select(var => new SelectOptionDto
                {
                    Label = var.Name,
                    Value = var.Guid
                })
                .OrderBy(var => var.Label)
                .ToList();

            return new SelectOptionResponse
            {
                Options = locations,
                IsSuccess = true
            };
        }

        public TicketCategoriesListResponse GetTicketCategoriesForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsTicketCategory)
                .ThenInclude(var => var.SettingsTicketSubCategory)
                .FirstOrDefault();

            var categories = company
                .SettingsTicketCategory
                .Where(var => var.IsActive)
                .Select(var => new TicketCategoryDto
                {
                    Name = var.Name,
                    TicketCategoryId = var.Guid,
                    TicketSubCategories = var.SettingsTicketSubCategory
                        .Where(var1 => var1.IsActive)
                        .Select(var1 => new TicketSubCategoryDto
                        {
                            Name = var1.Name,
                            TicketSubCategoryId = var1.Guid
                        }).ToList()
                })
                .ToList();

            return new TicketCategoriesListResponse
            {
                TicketCategories = categories,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetRolesForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsRole)
                .FirstOrDefault();

            var roles = company.SettingsRole
                .Where(var => var.RoleName.ToLower() != "admin")
                .Select(var => new SelectOptionDto
                {
                    Label = var.RoleName,
                    Value = var.Guid
                })
                .OrderBy(var => var.Label)
                .ToList();

            return new SelectOptionResponse
            {
                Options = roles,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetTrainingsForDropdown(GetTrainingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsTraining).ThenInclude(var => var.SettingsTrainingGrade)
                .ThenInclude(var => var.Grade)
                .FirstOrDefault();

            var trainings = company.SettingsTraining
                .Where(var => var.IsActive && (request.GradeIds == null || !request.GradeIds.Any() || var.SettingsTrainingGrade.Any(var1 => request.GradeIds.Contains(var1.Grade.Guid))))
                .Select(var => new SelectOptionDto
                {
                    Label = var.Name,
                    Value = var.Guid
                })
                .OrderBy(var => var.Label)
                .ToList();

            return new SelectOptionResponse
            {
                Options = trainings,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetTrainingCodeForDropdown(GetTrainingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsTraining).ThenInclude(var => var.SettingsTrainingGrade)
                .ThenInclude(var => var.Grade)
                .FirstOrDefault();

            var trainings = company.SettingsTraining
                .Where(var => var.IsActive && (request.GradeIds == null || !request.GradeIds.Any() || var.SettingsTrainingGrade.Any(var1 => request.GradeIds.Contains(var1.Grade.Guid))))
                .Select(var => new SelectOptionDto
                {
                    Label = var.TrainingCode,
                    Value = var.Guid
                })
                .OrderBy(var => var.Label)
                .ToList();

            return new SelectOptionResponse
            {
                Options = trainings,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetDepartmentsForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsDepartment)
                .FirstOrDefault();

            var departments = company.SettingsDepartment
                .Where(var => var.IsActive)
                .Select(var => new SelectOptionDto
                {
                    Label = var.Name,
                    Value = var.Guid
                })
                .OrderBy(var => var.Label)
                .ToList();

            return new SelectOptionResponse
            {
                Options = departments,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetDesignationsForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsDepartmentDesignation)
                .FirstOrDefault();

            var designations = company.SettingsDepartmentDesignation
                .Where(var => var.IsActive)
                .Select(var => new SelectOptionDto
                {
                    Label = var.Name,
                    Value = var.Guid
                })
                .OrderBy(var => var.Label)
                .ToList();

            return new SelectOptionResponse
            {
                Options = designations,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetDocumentTypesForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsDocumentType)
                .FirstOrDefault();

            var options = company.SettingsDocumentType
                .Where(var => var.IsActive)
                .Select(var => new SelectOptionDto
                {
                    Label = var.DocumentType,
                    Value = var.Guid
                }).ToList();

            return new SelectOptionResponse
            {
                Options = options,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetTeamsForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsTeam)
                .FirstOrDefault();

            var options = company.SettingsTeam
                .Where(var => var.IsActive)
                .Select(var => new SelectOptionDto
                {
                    Label = var.Name,
                    Value = var.Guid
                }).ToList();

            return new SelectOptionResponse
            {
                Options = options,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetCategoriesForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsCategory)
                .FirstOrDefault();

            var options = company.SettingsCategory
                .Where(var => var.IsActive)
                .Select(var => new SelectOptionDto
                {
                    Label = var.Category,
                    Value = var.Guid
                })
                .OrderBy(var => var.Label)
                .ToList();

            return new SelectOptionResponse
            {
                Options = options,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetAnnouncementTypesForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsAnnouncementType)
                .FirstOrDefault();

            var options = company.SettingsAnnouncementType
                .Where(var => var.IsActive)
                .Select(var => new SelectOptionDto
                {
                    Label = var.Type,
                    Value = var.Guid
                }).ToList();

            return new SelectOptionResponse
            {
                Options = options,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetGradesForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsGrade)
                .FirstOrDefault();

            var options = company.SettingsGrade
                .Where(var => var.IsActive)
                .Select(var => new SelectOptionDto
                {
                    Label = var.Grade,
                    Value = var.Guid
                })
                .OrderBy(var => var.Label)
                .ToList();

            return new SelectOptionResponse
            {
                Options = options,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetRegionsForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsRegion)
                .FirstOrDefault();

            var options = company.SettingsRegion
                .Where(var => var.IsActive)
                .Select(var => new SelectOptionDto
                {
                    Label = var.Name,
                    Value = var.Guid
                }).ToList();

            return new SelectOptionResponse
            {
                Options = options,
                IsSuccess = true
            };
        }

        public SelectOptionResponse GetReportingToForDropdown(ReportingToForDropdownRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsDepartment)
                .ThenInclude(var => var.SettingsDepartmentDesignation)
                .ThenInclude(var => var.ReportingTo)
                .ThenInclude(var => var.EmployeeCompany)
                .ThenInclude(var => var.Employee)
                .FirstOrDefault();

            var department = company.SettingsDepartment.FirstOrDefault(var =>
                var.IsActive && var.Guid.Equals(request.DepartmentId));

            if (department != null)
            {
                var designation = department.SettingsDepartmentDesignation.FirstOrDefault(var1 =>
                    var1.IsActive && var1.Guid.Equals(request.DesignationId));

                if (designation != null)
                {
                    if (designation.ReportingTo != null)
                    {
                        var allEmployees = designation.ReportingTo.EmployeeCompany
                            .Where(var => var.Employee.IsActive && var.Status.Equals("on-roll"))
                            .Select(var => new SelectOptionDto
                            {
                                Label = var.AddressingName,
                                Value = var.Employee.Guid
                            })
                            .ToList();

                        return new SelectOptionResponse
                        {
                            Options = allEmployees,
                            IsSuccess = true
                        };
                    }
                    else
                    {
                        return new SelectOptionResponse
                        {
                            Options = new List<SelectOptionDto>(),
                            IsSuccess = true
                        };
                    }
                }

                throw new Exception("Designation not found.");
            }

            throw new Exception("Department not found.");
        }

        public UpdateCompanySettingsResponse GetHolidays(GetHolidaysRequset request)
        {
            var company = GetAll()
                .Include(var => var.SettingsHoliday).ThenInclude(var => var.SettingsHolidayLocation).ThenInclude(var => var.Location)
                .FirstOrDefault();

            
            var startDate = new DateTime(request.Year, request.Month == 0 ? 1 : request.Month, 1);
            var endDate = request.Month == 0 ? startDate.AddYears(1).AddDays(-1) : startDate.AddMonths(1).AddDays(-1);

            return new UpdateCompanySettingsResponse
            {
                IsSuccess = true,
                Holidays = company
                    .SettingsHoliday.Where(var => var.IsActive
                    && var.Date <= endDate
                    && var.Date >= startDate
                    && (string.IsNullOrWhiteSpace(request.Location) || var.SettingsHolidayLocation == null || var.SettingsHolidayLocation.Any(var1 => var1.Location.Guid.Equals(request.Location)))
                    )
                    .Select(var => new HolidayDto
                    {
                        Reason = var.Reason,
                        Date = var.Date,
                        Type = var.HolidayType
                    }).ToList()
            };
        }

        public UploadResponse UploadDataToSettings(UploadDataRequest request)
        {
            var lineCount = 1;
            var allLines = request.FileContent.Split("\r\n");
            var dataRequest = new UpdateCompanySettingsRequest
            {
                UserIdNum = request.UserIdNum,
                UserId = request.UserId,
                UserName = request.UserName
            };

            switch (request.Type)
            {
                case "Announcement":
                    dataRequest.AnnouncementTypes = new List<AnnouncementTypeDto>();
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

                        dataRequest.AnnouncementTypes.Add(new AnnouncementTypeDto
                        {
                            IsActive = true,
                            Description = values[1],
                            AnnouncementType = values[0]
                        });
                    }

                    UpdateAnnouncementTypes(dataRequest);
                    break;

                case "AssetType":
                    dataRequest.AssetTypes = new List<AssetTypeDto>();

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
                        dataRequest.AssetTypes.Add(new AssetTypeDto
                        {
                            IsActive = true,
                            Description = values[1],
                            AssetType = values[0]
                        });
                    }

                    UpdateAssetTypes(dataRequest);
                    break;

                case "Category":
                    dataRequest.Categories = new List<CategoryDto>();

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
                        dataRequest.Categories.Add(new CategoryDto
                        {
                            IsActive = true,
                            Description = values[1],
                            Category = values[0]
                        });
                    }

                    UpdateCategories(dataRequest);
                    break;

                case "Department":
                    dataRequest.Departments = new List<DepartmentDto>();

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
                        dataRequest.Departments.Add(new DepartmentDto()
                        {
                            IsActive = true,
                            Description = values[1],
                            Name = values[0]
                        });
                    }

                    UpdateDepartments(dataRequest);
                    break;

                case "Designation":
                    dataRequest.Designations = new List<DesignationDto>();

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
                        dataRequest.Designations.Add(new DesignationDto()
                        {
                            IsActive = true,
                            Description = values[1],
                            Name = values[0]
                        });
                    }

                    UpdateDesignations(dataRequest);
                    break;

                case "DocumentType":
                    dataRequest.DocumentTypes = new List<DocumentTypeDto>();

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
                        dataRequest.DocumentTypes.Add(new DocumentTypeDto()
                        {
                            IsActive = true,
                            Description = values[1],
                            DocumentType = values[0]
                        });
                    }

                    UpdateDocumentTypes(dataRequest);
                    break;

                case "Grades":
                    dataRequest.Grades = new List<GradeDto>();

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
                        dataRequest.Grades.Add(new GradeDto()
                        {
                            IsActive = true,
                            Description = values[1],
                            Grade = values[0]
                        });
                    }

                    UpdateGrades(dataRequest);
                    break;

                case "Location":
                    dataRequest.Locations = new List<LocationDto>();

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
                        dataRequest.Locations.Add(new LocationDto
                        {
                            IsActive = true,
                            Address = values[1],
                            Location = values[0],
                            GstNumber = values[2],
                            Phone = values[3],
                            Email = values[4],
                            State = values[5],
                            Country = values[6]
                        });
                    }

                    UpdateLocations(dataRequest);
                    break;

                case "ProductLine":
                    dataRequest.ProductLines = new List<ProductLineDto>();

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
                        dataRequest.ProductLines.Add(new ProductLineDto()
                        {
                            IsActive = true,
                            Description = values[1],
                            ProductLine = values[0],
                        });
                    }

                    UpdateProductLines(dataRequest);
                    break;

                case "Region":
                    dataRequest.Regions = new List<RegionDto>();

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
                        dataRequest.Regions.Add(new RegionDto()
                        {
                            IsActive = true,
                            Description = values[1],
                            Region = values[0],
                        });
                    }

                    UpdateRegions(dataRequest);
                    break;

                case "Team":
                    dataRequest.Teams = new List<TeamDto>();

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
                        dataRequest.Teams.Add(new TeamDto()
                        {
                            IsActive = true,
                            Description = values[1],
                            Team = values[0],
                        });
                    }

                    UpdateTeams(dataRequest);
                    break;

                case "Tickets":
                    dataRequest.TicketCategories = new List<TicketCategoryDto>();

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
                        dataRequest.TicketCategories.Add(new TicketCategoryDto()
                        {
                            IsActive = true,
                            Description = values[2],
                            Name = values[0],
                            TicketSubCategories = new List<TicketSubCategoryDto>
                            {
                                new TicketSubCategoryDto
                                {
                                    Description = values[2],
                                    Name = values[1],
                                    IsActive = true
                                }
                            }
                        });
                    }

                    UpdateTicketCategory(dataRequest);
                    break;

                case "TicketFAQ":
                    dataRequest.TicketFaqs = new List<TicketFaqDto>();

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
                        dataRequest.TicketFaqs.Add(new TicketFaqDto()
                        {
                            IsActive = true,
                            Description = values[1],
                            FaqTitle = values[0]
                        });
                    }

                    UpdateTicketFaq(dataRequest);
                    break;

                case "Holidays":
                    dataRequest.Holidays = new List<HolidayDto>();

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
                        var location =
                            dbContext.SettingsLocation.FirstOrDefault(var =>
                                var.IsActive && var.Name.Equals(values[1]));

                        if (location != null)
                        {
                            dataRequest.Holidays.Add(new HolidayDto()
                            {
                                IsActive = true,
                                LocationIds = new string[] { location.Guid }.ToList(),
                                Date = DateTime.Parse(values[0]),
                                Reason = values[2],
                                Type = values[3]
                            });
                        }
                    }

                    var sortedHolidays = new List<HolidayDto>();
                    foreach (var hol in dataRequest.Holidays.OrderBy(var => var.Date))
                    {
                        var isSame = sortedHolidays.FirstOrDefault(var =>
                            var.Date == hol.Date && hol.Reason == var.Reason && hol.Type == var.Type);
                        if (isSame != null)
                        {
                            isSame.LocationIds.AddRange(hol.LocationIds);
                        }
                        else
                        {
                            sortedHolidays.Add(hol);
                        }
                    }

                    dataRequest.Holidays = sortedHolidays;
                    UpdateHolidays(dataRequest);
                    break;
            }

            return new UploadResponse
            {
                IsSuccess = true
            };
        }

        public UpdateCompanySettingsResponse UpdateAssetTypes(UpdateCompanySettingsRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsAssetTypes).ThenInclude(var => var.SettingsAssetTypeOwner)
                .FirstOrDefault();

            foreach (var asset in request.AssetTypes)
            {
                var owners = asset.SigningAuthorities != null
                    ? dbContext.Employee
                        .Where(var => var.IsActive && asset.SigningAuthorities.Contains(var.Guid))
                        .Select(var => var.Id)
                    : null;

                if (string.IsNullOrEmpty(asset.AssetTypeId))
                {
                    company.SettingsAssetTypes.Add(new SettingsAssetTypes()
                    {
                        Company = company,
                        Description = asset.Description,
                        Guid = CustomGuid.NewGuid(),
                        AssetType = asset.AssetType,
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true,
                        SettingsAssetTypeOwner = owners != null
                            ? owners.Select(var => new SettingsAssetTypeOwner
                            {
                                OwnerId = var
                            }).ToList()
                            : null
                    });
                }
                else
                {
                    var addedAssetType =
                        company.SettingsAssetTypes.FirstOrDefault(var =>
                            var.Guid.Equals(asset.AssetTypeId) && var.IsActive);
                    if (addedAssetType != null)
                    {
                        if (!asset.IsActive)
                        {
                            addedAssetType.IsActive = false;
                            addedAssetType.UpdatedBy = request.UserIdNum;
                            addedAssetType.UpdatedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            dbContext.SettingsAssetTypeOwner.RemoveRange(addedAssetType.SettingsAssetTypeOwner);

                            addedAssetType.AssetType = asset.AssetType;
                            addedAssetType.Description = asset.Description;
                            addedAssetType.UpdatedBy = request.UserIdNum;
                            addedAssetType.UpdatedOn = DateTime.UtcNow;
                            addedAssetType.SettingsAssetTypeOwner = owners.Select(var => new SettingsAssetTypeOwner
                            {
                                OwnerId = var
                            }).ToList();
                        }
                    }
                }
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateAssets.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateAssets.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return GetAllSettings(request);
        }

        public SelectOptionResponse GetAssetTypesForDropdown(BaseRequest request)
        {
            var company = GetAll()
                .Include(var => var.SettingsAssetTypes)
                .FirstOrDefault();

            var assetTypes = company.SettingsAssetTypes
                .Where(var => var.IsActive)
                .Select(var => new SelectOptionDto
                {
                    Label = var.AssetType,
                    Value = var.Guid
                })
                .ToList();

            return new SelectOptionResponse
            {
                Options = assetTypes,
                IsSuccess = true
            };
        }
    }
}