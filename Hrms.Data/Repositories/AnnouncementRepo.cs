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
    public class AnnouncementRepo : BaseRepository<Announcement>, IAnnouncementRepo
    {
        private IEventLogRepo _eventLogRepo;
        public AnnouncementRepo(HrmsContext context, IEventLogRepo eventLogRepo) : base(context)
        {
            _eventLogRepo = eventLogRepo;
        }

        public GetAnnouncementsResponse GetAllAnnouncements(AnnouncementFilterRequest request)
        {
            var announcements = GetAll()
                .Include(var => var.AnnouncementLocation).ThenInclude(var => var.Location)
                .Include(var => var.AnnouncementAttachment)
                .Include(var => var.Type)
                .Where(var => var.IsActive
                              && (string.IsNullOrWhiteSpace(request.Title) || var.Title.Contains(request.Title))
                              && (request.Date == null || var.Date.Equals(request.Date))
                              && (request.Publish == null || !request.Publish.Any() ||
                                  request.Publish.Contains(var.IsPublished))
                              && (request.Types == null || !request.Types.Any() ||
                                  request.Types.Contains(var.Type.Guid))
                              && (request.Locations == null || !request.Locations.Any() ||
                                  var.AnnouncementLocation != null && var.AnnouncementLocation.Any() &&
                                  var.AnnouncementLocation.Any(var1 => request.Locations.Contains(var1.Location.Guid)))
                )
                .Select((var => new AnnouncementDto
                {
                    Content = var.Content,
                    Date = var.Date,
                    Title = var.Title,
                    AnnouncementId = var.Guid,
                    AnnouncementType = var.Type.Type,
                    EndDate = var.EndDate,
                    IsHidden = var.IsHidden ?? false,
                    IsPublished = var.IsPublished,
                    StartDate = var.StartDate,
                    SubTitle = var.SubTitle,
                    AttachmentCount = var.AnnouncementAttachment.Count(var1 => var1.IsActive)
                }))
                .ToList();

            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(25) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(25) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllAnnouncements.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllAnnouncements.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new GetAnnouncementsResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                Announcements = announcements
            };
        }

        public GetAnnouncementsResponse GetAnnouncementToShow(BaseRequest request)
        {
            var announcements = GetAll()
                .Include(var => var.AnnouncementLocation)
                .Include(var => var.AnnouncementAttachment)
                .Include(var => var.Type)
                .Where(var => var.IsActive
                              && !(var.IsHidden ?? false)
                              && var.IsPublished
                              && var.StartDate <= DateTime.Today
                              && (var.EndDate == null || var.EndDate >= DateTime.Today)
                              && (var.AnnouncementLocation == null || !var.AnnouncementLocation.Any()))
                .Select((var => new AnnouncementDto
                {
                    Content = var.Content,
                    Date = var.Date,
                    Title = var.Title,
                    AnnouncementId = var.Guid,
                    AnnouncementType = var.Type.Type,
                    EndDate = var.EndDate,
                    IsHidden = var.IsHidden ?? false,
                    IsPublished = var.IsPublished,
                    StartDate = var.StartDate,
                    SubTitle = var.SubTitle,
                    AttachmentCount = var.AnnouncementAttachment.Count(var1 => var1.IsActive)
                }))
                .ToList();

            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(24) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(24) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(24) && a.Ismanager.Equals(1));
            var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(24) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

            var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(24) && a.Ismanager.Equals(0));
            var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(24) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAnnouncementToShow.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAnnouncementToShow.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            Save();

            return new GetAnnouncementsResponse
            {
                IsSuccess = true,
                HrAccess = hrAccess,
                MgAccess = mgAccess,
                EmpAccess = empAccess,
                Announcements = announcements
            };
        }

        public BaseResponse UpdateAnnouncement(UpdateAnnouncementRequest request)
        {
            var guid = CustomGuid.NewGuid();
            var announcementType =
                dbContext.SettingsAnnouncementType.FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.AnnouncementType));

            var locations = request.Locations != null
                ? dbContext.SettingsLocation.Where(var => var.IsActive && request.Locations.Contains(var.Guid))
                : null;

            if (string.IsNullOrWhiteSpace(request.AnnouncementId))
            {
                var newAnnouncement = new Announcement
                {
                    Content = request.Content,
                    Date = request.Date,
                    Guid = guid,
                    Title = request.Title,
                    Type = announcementType,
                    AddedBy = request.UserIdNum,
                    AddedOn = DateTime.UtcNow,
                    EndDate = request.EndDate,
                    IsActive = true,
                    IsPublished = request.IsPublished,
                    StartDate = request.StartDate,
                    SubTitle = request.SubTitle,
                    CompanyId = request.CompanyIdNum,
                    AnnouncementLocation = locations != null
                        ? locations.Select(var => new AnnouncementLocation
                        {
                            Location = var,
                        }).ToList()
                        : null
                };

                AddItem(newAnnouncement);

                Save();
            }
            else
            {
                var addedAnnouncement = GetAll()
                    .Include(var => var.AnnouncementLocation)
                    .Include(var => var.AnnouncementAttachment).ThenInclude(var => var.Attachment)
                    .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.AnnouncementId));
                if (addedAnnouncement != null)
                {
                    guid = addedAnnouncement.Guid;
                    dbContext.AnnouncementLocation.RemoveRange(addedAnnouncement.AnnouncementLocation);

                    if (request.Attachments != null)
                    {
                        foreach (var attachment in request.Attachments)
                        {
                            var addedAttachment = addedAnnouncement.AnnouncementAttachment.FirstOrDefault(var =>
                                var.Guid.Equals(attachment.AttachmentId) && var.IsActive);
                            if (addedAttachment != null)
                            {
                                if (!attachment.IsActive)
                                {
                                    addedAttachment.IsActive = false;
                                    addedAttachment.UpdatedBy = request.UserIdNum;
                                    addedAttachment.UpdatedOn = DateTime.UtcNow;

                                    var path = Path.Combine(new string[] {request.FileBasePath, addedAttachment.Attachment.AttachmentUrl});
                                    File.Delete(path);
                                }
                                else
                                {
                                    addedAttachment.Attachment.FileName = attachment.FileName;
                                    addedAttachment.Attachment.AttachmentType = attachment.Type;
                                    addedAttachment.UpdatedBy = request.UserIdNum;
                                    addedAttachment.UpdatedOn = DateTime.UtcNow;
                                }
                            }
                            else
                            {
                                throw new Exception("Attachment not found");
                            }
                        }
                    }

                    addedAnnouncement.Content = request.Content;
                    addedAnnouncement.Date = request.Date;
                    addedAnnouncement.Title = request.Title;
                    addedAnnouncement.Type = announcementType;
                    addedAnnouncement.UpdatedBy = request.UserIdNum;
                    addedAnnouncement.UpdatedOn = DateTime.UtcNow;
                    addedAnnouncement.EndDate = request.EndDate;
                    addedAnnouncement.IsPublished = request.IsPublished;
                    addedAnnouncement.StartDate = request.StartDate;
                    addedAnnouncement.SubTitle = request.SubTitle;
                    addedAnnouncement.AnnouncementLocation = locations != null
                        ? locations.Select(var => new AnnouncementLocation
                        {
                            Location = var,
                        }).ToList()
                        : null;

                    Save();
                }
                else
                {
                    throw new Exception("Announcement not found");
                }
            }

            if (request.NewAttachments != null)
            {
                var ann = GetAll()
                    .Include(var => var.AnnouncementLocation)
                    .Include(var => var.AnnouncementAttachment)
                    .FirstOrDefault(var => var.IsActive && var.Guid.Equals(guid));
                if (ann != null)
                {
                    var i = 0;
                    foreach (var file in request.NewAttachments)
                    {
                        var path = Path.Combine(new string[]
                            {request.FileBasePath, "announcements", guid, "attachments"});
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        var fileName = string.Concat(file.Name.Replace(" ", "_"), "_", CustomGuid.NewGuid(),
                            Path.GetExtension(file.FileName));

                        using (var memoryStream = new MemoryStream())
                        {
                            file.CopyTo(memoryStream);
                            File.WriteAllBytes(
                                Path.Combine(path, fileName),
                                memoryStream.ToArray());
                        }

                        var fullPath = Path.Combine(new string[]
                            {"announcements", guid, "attachments", fileName});

                        var attachment = new Attachment
                        {
                            Guid = CustomGuid.NewGuid(),
                            AddedBy = request.UserIdNum,
                            AddedOn = DateTime.UtcNow,
                            AttachmentSize = file.Length,
                            FileName = string.Concat(file.Name, Path.GetExtension(file.FileName)),
                            IsActive = true,
                            ContentType = file.ContentType,
                            AttachmentUrl = fullPath,
                            AttachmentType = request.AttachmentType.ElementAt(i)
                        };

                        ann.AnnouncementAttachment.Add(new AnnouncementAttachment
                        {
                            Guid = CustomGuid.NewGuid(),
                            IsActive = true,
                            Attachment = attachment,
                            AddedBy = request.UserIdNum,
                            AddedOn = DateTime.UtcNow,
                        });

                        i++;
                    }
                }

                Save();
            }

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.UpdateAnnouncement.Template, request.UserName, request.UserId, guid),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateAnnouncement.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName,
                    announcementId = guid
                })
            });

            Save();

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse DeleteAnnouncement(AnnouncementActionRequest request)
        {
            var addedAnnouncement = GetAll()
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.AnnouncementId));
            if (addedAnnouncement != null)
            {
                addedAnnouncement.IsActive = false;
                addedAnnouncement.UpdatedBy = request.UserIdNum;
                addedAnnouncement.UpdatedOn = DateTime.UtcNow;

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.DeleteAnnouncement.Template, request.UserName, request.UserId, request.AnnouncementId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.DeleteAnnouncement.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        announcementId = request.AnnouncementId
                    })
                });

                Save();
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Announcement not found");
        }

        public AnnouncementDetailsResponse GetAnnouncementDetails(AnnouncementActionRequest request)
        {
            var addedAnnouncement = GetAll()
                .Include(var => var.Type)
                .Include(var => var.AnnouncementLocation).ThenInclude(var => var.Location)
                .Include(var => var.AnnouncementAttachment).ThenInclude(var => var.Attachment)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.AnnouncementId));
            if (addedAnnouncement != null)
            {
                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetAnnouncementDetails.Template, request.UserName, request.UserId, request.AnnouncementId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetAnnouncementDetails.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        announcementId = request.AnnouncementId
                    })
                });

                Save();

                return new AnnouncementDetailsResponse
                {
                    AnnouncementId = addedAnnouncement.Guid,
                    IsSuccess = true,
                    Content = addedAnnouncement.Content,
                    Date = addedAnnouncement.Date,
                    Title = addedAnnouncement.Title,
                    AnnouncementType = addedAnnouncement.Type.Guid,
                    EndDate = addedAnnouncement.EndDate,
                    IsPublished = addedAnnouncement.IsPublished,
                    StartDate = addedAnnouncement.StartDate,
                    SubTitle = addedAnnouncement.SubTitle,
                    Locations = addedAnnouncement.AnnouncementLocation != null
                        ? addedAnnouncement.AnnouncementLocation.Select(var => var.Location.Guid).ToList()
                        : null,
                    Attachments = addedAnnouncement.AnnouncementAttachment.Where(var => var.IsActive)
                        .Select(var => new AttachmentDto
                        {
                            Size = var.Attachment.AttachmentSize,
                            AttachmentId = var.Guid,
                            FileName = var.Attachment.FileName,
                            FileUrl = var.Attachment.AttachmentUrl.Replace("\\", "/"),
                            IsActive = true,
                            Type = var.Attachment.AttachmentType,
                            ContentType = var.Attachment.ContentType
                        }).ToList()
                };
            }

            throw new Exception("Announcement not found");
        }
    }
}