using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Hrms.Data.Repositories
{
    public class TicketRepo : BaseRepository<Ticket>, ITicketRepo
    {
        private IEventLogRepo _eventLogRepo;
        public TicketRepo(HrmsContext context, IEventLogRepo eventLogRepo) : base(context)
        {
            _eventLogRepo = eventLogRepo;
        }

        public GetTicketsResponse GetAllTickets(TicketFilterRequest request)
        {
            var tickets = GetAll()
                .Include(var => var.Category).ThenInclude(var => var.SettingsTicketCategoryOwner).ThenInclude(var => var.Employee)
                .Include(var => var.SubCategory)
                .Include(var => var.Status)
                .Include(var => var.AddedByNavigation)
                .Where(var =>
                    var.IsActive
                    && (string.IsNullOrWhiteSpace(request.Title) || var.Title.Contains(request.Title))
                    && (request.StartDate == null || request.EndDate == null ||
                        (request.StartDate <= var.AddedOn && request.EndDate >= var.AddedOn))
                    && (request.Category == null || var.Category.Guid.Equals(request.Category))
                    && (request.SubCategory == null || var.SubCategory.Guid.Equals(request.SubCategory))
                    && (request.UserRole.Equals("Admin") || var.AddedBy == request.UserIdNum
                        || (request.UserRole.Equals("HR") && (var.Category.SettingsTicketCategoryOwner == null) || (var.Category.SettingsTicketCategoryOwner.Any(var1 => var1.EmployeeId == request.UserIdNum))))
                    && (request.Status == null || !request.Status.Any() ||
                        (var.Status != null && request.Status.Contains(var.Status.Name)))
                    && (request.AddedBy == null || !request.AddedBy.Any() ||
                        request.AddedBy.Contains(var.AddedByNavigation.Guid))
                )
                .Select(var => new TicketDto
                {
                    IsCreator = var.AddedBy == request.UserIdNum,
                    Category = var.Category != null ? var.Category.Name : string.Empty,
                    SubCategory = var.SubCategory != null ? var.SubCategory.Name : string.Empty,
                    Status = var.Status != null ? var.Status.Name : string.Empty,
                    AddedBy = var.AddedByNavigation != null ? var.AddedByNavigation.Name : string.Empty,
                    Title = var.Title,
                    AddedOn = var.AddedOn,
                    IsStarted = var.Status != null ? var.Status.Name.Equals("Working") : false,
                    IsCompleted = var.Status != null ? var.Status.Name.Equals("Closed") : false,
                    TicketId = var.Guid,
                    StartedOn =
                        var.TicketStatus
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

            
            var empid = request.UserIdNum;

            var hrcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(2) && a.ModuleID.Equals(28) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid));
            var hrAccess = hrcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(2) && a.ModuleID.Equals(28) && a.Ismanager.Equals(0) && a.EmployeeId.Equals(empid)).FirstOrDefault().CanAccess : 0;

            var mgcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(28) && a.Ismanager.Equals(1));
            var mgAccess = mgcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(28) && a.Ismanager.Equals(1)).FirstOrDefault().CanAccess : 0;

            var empcnt = dbContext.SettingsModuleAccess.Count(a => a.RoleId.Equals(3) && a.ModuleID.Equals(28) && a.Ismanager.Equals(0));
            var empAccess = empcnt != 0 ? dbContext.SettingsModuleAccess.Where(a => a.RoleId.Equals(3) && a.ModuleID.Equals(28) && a.Ismanager.Equals(0)).FirstOrDefault().CanAccess : 0;


            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.GetAllTickets.Template, request.UserName, request.UserId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.GetAllTickets.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName
                })
            });

            return new GetTicketsResponse
            {
                Tickets = tickets,
                HrAccess = hrAccess,
                EmpAccess = empAccess,
                MgAccess = mgAccess,
                IsSuccess = true
            };
        }

        public TicketDetailsResponse GetTicketDetails(TicketActionRequest request)
        {
            var ticket = GetAll()
                .Include(var => var.TicketAttachment).ThenInclude(var => var.Attachment)
                .Include(var => var.Category)
                .Include(var => var.SubCategory)
                .Include(var => var.Status)
                .Include(var => var.AddedByNavigation)
                .Include(var => var.TicketComment)
                .ThenInclude(var => var.Comment)
                .ThenInclude(var => var.AddedByNavigation)
                .FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.TicketId));

            if (ticket != null)
            {
                var ticketInfo = new TicketDto
                {
                    IsCreator = request.UserIdNum == ticket.AddedBy,
                    Category = ticket.Category != null ? ticket.Category.Guid : string.Empty,
                    SubCategory = ticket.SubCategory != null ? ticket.SubCategory.Guid : string.Empty,
                    Status = ticket.Status != null ? ticket.Status.Name : string.Empty,
                    AddedBy = ticket.AddedByNavigation != null ? ticket.AddedByNavigation.Name : string.Empty,
                    Title = ticket.Title,
                    Explanation = ticket.Explanation,
                    AddedOn = ticket.AddedOn,
                    TicketId = ticket.Guid,
                    IsStarted = ticket.Status != null ? ticket.Status.Name.Equals("Working") : false,
                    IsCompleted = ticket.Status != null ? ticket.Status.Name.Equals("Closed") : false,
                    StartedOn = ticket.TicketStatus
                                    .OrderByDescending(var1 => var1.ChangedOn)
                                    .FirstOrDefault(var1 => var1.Status.Name.Equals("Working")) != null
                        ? ticket.TicketStatus
                            .OrderByDescending(var1 => var1.ChangedOn)
                            .FirstOrDefault(var1 => var1.Status.Name.Equals("Working"))
                            .ChangedOn
                        : (DateTime?)null,
                    ClosedOn = ticket.TicketStatus
                                   .OrderByDescending(var1 => var1.ChangedOn)
                                   .FirstOrDefault(var1 => var1.Status.Name.Equals("Closed")) != null
                        ? ticket.TicketStatus
                            .OrderByDescending(var1 => var1.ChangedOn)
                            .FirstOrDefault(var1 => var1.Status.Name.Equals("Closed"))
                            .ChangedOn
                        : (DateTime?)null,
                };

                var attachments = ticket.TicketAttachment.Where(var => var.IsActive)
                    .Select(var => new AttachmentDto
                    {
                        Size = var.Attachment.AttachmentSize,
                        AttachmentId = var.Guid,
                        FileName = var.Attachment.FileName,
                        FileUrl = var.Attachment.AttachmentUrl.Replace("\\", "/"),
                        IsActive = true,
                        Type = var.Attachment.AttachmentType,
                        ContentType = var.Attachment.ContentType
                    }).ToList();

                var comments = ticket.TicketComment
                    .Where(var => var.IsActive)
                    .Select(var => new CommentDto
                    {
                        Comment = var.Comment.Content,
                        AddedBy = var.Comment.AddedByNavigation.Name,
                        AddedOn = var.Comment.AddedOn,
                        CommentId = var.Comment.Guid,
                        IsCreator = var.Comment.AddedBy == request.UserIdNum
                    })
                    .ToList();

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetTicketDetails.Template, request.UserName, request.UserId, request.TicketId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetTicketDetails.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TicketId
                    })
                });

                return new TicketDetailsResponse
                {
                    TicketInfo = ticketInfo,
                    Attachments = attachments,
                    Comments = comments,
                    IsSuccess = true
                };
            }

            throw new Exception("Ticket not found");
        }

        public BaseResponse UpdateTicket(UpdateTicketRequest request, ApplicationSettings settings)
        {
            var guid = string.Empty;
            if (string.IsNullOrWhiteSpace(request.TicketId))
            {
                var status =
                    dbContext.SettingsTicketStatus.FirstOrDefault(var => var.IsActive && var.Name.Equals("Open"));

                SettingsTicketCategory category = null;
                if (!string.IsNullOrWhiteSpace(request.CategoryId))
                {
                    category = dbContext.SettingsTicketCategory
                        .Include(var => var.SettingsTicketCategoryOwner)
                        .ThenInclude(var => var.Employee)
                        .FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(request.CategoryId));
                }

                SettingsTicketSubCategory subCategory = null;
                if (!string.IsNullOrWhiteSpace(request.SubCategoryId))
                {
                    subCategory = dbContext.SettingsTicketSubCategory.FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(request.SubCategoryId));
                }

                var newTicket = new Ticket
                {
                    CompanyId = request.CompanyIdNum,
                    Explanation = request.Explanation,
                    Guid = CustomGuid.NewGuid(),
                    Title = request.Title,
                    AddedBy = request.UserIdNum,
                    AddedOn = DateTime.UtcNow,
                    IsActive = true,
                    Category = category,
                    SubCategory = subCategory,
                    Status = status,
                    TicketComment = new List<TicketComment>(),
                    TicketStatus = new List<TicketStatus>
                    {
                        new TicketStatus
                        {
                            Status = status,
                            ChangedBy = request.UserIdNum,
                            ChangedOn = DateTime.UtcNow,
                            CompanyId = request.CompanyIdNum,
                        }
                    }
                };

                AddItem(newTicket);

                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.TicketUpdate,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = request.UserIdNum,
                    NotificationData = JsonConvert.SerializeObject(new
                    {
                        ticketId = guid,
                        userName = request.UserName
                    })
                });

                if (category.SettingsTicketCategoryOwner != null)
                {
                    foreach (var owner in category.SettingsTicketCategoryOwner)
                    {
                        EmailSender.SendTicketCreatedEmail(
                            owner.Employee.Name,
                            owner.Employee.EmailId,
                            newTicket.Title,
                            DateTime.Now.ToCustomDateTimeFormat(0),
                            newTicket.Guid,
                            category.Name,
                            request.UserName,
                            settings
                            );

                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.TicketUpdateOwner,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = owner.EmployeeId,
                            NotificationData = JsonConvert.SerializeObject(new
                            {
                                ticketId = guid,
                                userName = request.UserName,
                                category = category.Name
                            })
                        });
                    }
                }

                Save();

                guid = newTicket.Guid;
            }
            else
            {
                var addedTicket = GetAll()
                    .Include(var => var.Status)
                    .Include(var => var.TicketAttachment)
                    .FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(request.TicketId) && !var.Status.Name.Equals("Closed"));

                if (addedTicket != null)
                {
                    guid = request.TicketId;
                    if (request.Attachments != null)
                    {
                        foreach (var attachment in request.Attachments)
                        {
                            var addedAttachment = addedTicket.TicketAttachment.FirstOrDefault(var =>
                                var.Guid.Equals(attachment.AttachmentId) && var.IsActive);
                            if (addedAttachment != null)
                            {
                                if (!attachment.IsActive)
                                {
                                    addedAttachment.IsActive = false;
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
                }
                else
                {
                    throw new Exception("Ticket not found");
                }
            }

            if (request.NewAttachments != null)
            {
                var ticket = GetAll()
                    .Include(var => var.Status)
                    .Include(var => var.TicketAttachment)
                    .FirstOrDefault(var => var.IsActive && var.Guid.Equals(guid));
                if (ticket != null)
                {
                    var i = 0;
                    foreach (var file in request.NewAttachments)
                    {
                        var path = Path.Combine(new string[]
                            {request.FileBasePath, "tickets", guid, "attachments"});
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        var fileName = string.Concat(file.FileName.Replace(" ", "_"), "_", CustomGuid.NewGuid(),
                            Path.GetExtension(file.FileName));

                        using (var memoryStream = new MemoryStream())
                        {
                            file.CopyTo(memoryStream);
                            File.WriteAllBytes(
                                Path.Combine(path, fileName),
                                memoryStream.ToArray());
                        }

                        var fullPath = Path.Combine(new string[]
                            {"tickets", guid, "attachments", fileName});

                        var attachment = new Attachment
                        {
                            Guid = CustomGuid.NewGuid(),
                            AddedBy = request.UserIdNum,
                            AddedOn = DateTime.UtcNow,
                            AttachmentSize = file.Length,
                            FileName = string.Concat(file.FileName, Path.GetExtension(file.FileName)),
                            IsActive = true,
                            ContentType = file.ContentType,
                            AttachmentUrl = fullPath,
                            AttachmentType = "Document"
                        };

                        ticket.TicketAttachment.Add(new TicketAttachment
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
                AuditText = string.Format(EventLogActions.UpdateTicket.Template, request.UserName, request.UserId, request.TicketId),
                PerformedBy = request.UserIdNum,
                ActionId = EventLogActions.UpdateTicket.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = request.UserId,
                    userName = request.UserName,
                    taskId = request.TicketId
                })
            });

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse StartTicket(TicketActionRequest request)
        {
            var ticket = GetAll()
                .Include(var => var.Status)
                .FirstOrDefault(var =>
                    var.IsActive && var.Status.Name.Equals("Open") && var.Guid.Equals(request.TicketId));

            if (ticket != null)
            {
                var status =
                    dbContext.SettingsTicketStatus.FirstOrDefault(var => var.IsActive && var.Name.Equals("Working"));

                ticket.UpdatedBy = request.UserIdNum;
                ticket.UpdatedOn = DateTime.UtcNow;
                ticket.Status = status;
                ticket.TicketStatus.Add(new TicketStatus
                {
                    Status = status,
                    ChangedBy = request.UserIdNum,
                    ChangedOn = DateTime.UtcNow,
                    CompanyId = request.CompanyIdNum,
                });

                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.TicketStarted,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = ticket.AddedBy,
                    NotificationData = JsonConvert.SerializeObject(new
                    {
                        ticketId = ticket.Guid,
                        userName = request.UserName
                    })
                });

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.StartTicket.Template, request.UserName, request.UserId, request.TicketId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.StartTicket.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TicketId
                    })
                });

                Save();
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Ticket not found");
        }

        public BaseResponse CloseTicket(TicketActionRequest request)
        {
            var ticket = GetAll()
                .Include(var => var.Status)
                .FirstOrDefault(var =>
                    var.IsActive && var.Status.Name.Equals("Working") && var.Guid.Equals(request.TicketId));

            if (ticket != null)
            {
                var status =
                    dbContext.SettingsTicketStatus.FirstOrDefault(var => var.IsActive && var.Name.Equals("Closed"));

                ticket.UpdatedBy = request.UserIdNum;
                ticket.UpdatedOn = DateTime.UtcNow;
                ticket.Status = status;
                ticket.TicketStatus.Add(new TicketStatus
                {
                    Status = status,
                    ChangedBy = request.UserIdNum,
                    ChangedOn = DateTime.UtcNow,
                    CompanyId = request.CompanyIdNum,
                });

                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.TicketClosed,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = ticket.AddedBy,
                    NotificationData = JsonConvert.SerializeObject(new
                    {
                        ticketId = ticket.Guid,
                        userName = request.UserName
                    })
                });

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.CloseTicket.Template, request.UserName, request.UserId, request.TicketId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.CloseTicket.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TicketId
                    })
                });

                Save();
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Ticket not found");
        }

        public BaseResponse ReopenTicket(TicketActionRequest request)
        {
            var ticket = GetAll()
                .Include(var => var.Status)
                .FirstOrDefault(var =>
                    var.IsActive && var.Status.Name.Equals("Closed") && var.Guid.Equals(request.TicketId));

            if (ticket != null)
            {
                var status =
                    dbContext.SettingsTicketStatus.FirstOrDefault(var => var.IsActive && var.Name.Equals("ReOpen"));

                ticket.UpdatedBy = request.UserIdNum;
                ticket.UpdatedOn = DateTime.UtcNow;
                ticket.Status = status;
                ticket.TicketStatus.Add(new TicketStatus
                {
                    Status = status,
                    ChangedBy = request.UserIdNum,
                    ChangedOn = DateTime.UtcNow,
                    CompanyId = request.CompanyIdNum,
                });

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.ReopenTicket.Template, request.UserName, request.UserId, request.TicketId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.ReopenTicket.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TicketId
                    })
                });

                Save();
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Ticket not found");
        }

        public BaseResponse UndoStartTicket(TicketActionRequest request)
        {
            var ticket = GetAll()
                .Include(var => var.Status)
                .FirstOrDefault(var =>
                    var.IsActive
                    && var.Status.Name.Equals("Working")
                    && var.Guid.Equals(request.TicketId));

            if (ticket != null)
            {
                var status =
                    dbContext.SettingsTicketStatus.FirstOrDefault(var => var.IsActive && var.Name.Equals("Open"));

                ticket.UpdatedBy = request.UserIdNum;
                ticket.UpdatedOn = DateTime.UtcNow;

                ticket.Status = status;
                ticket.TicketStatus.Add(new TicketStatus
                {
                    Status = status,
                    ChangedBy = request.UserIdNum,
                    ChangedOn = DateTime.UtcNow,
                    CompanyId = request.CompanyIdNum,
                });

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UndoStartTicket.Template, request.UserName, request.UserId, request.TicketId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UndoStartTicket.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TicketId
                    })
                });

                Save();
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Ticket not found");
        }

        public BaseResponse UndoCloseTicket(TicketActionRequest request)
        {
            var ticket = GetAll()
                .Include(var => var.Status)
                .FirstOrDefault(var =>
                    var.IsActive && var.Status.Name.Equals("Closed") && var.Guid.Equals(request.TicketId));
            
            if (ticket != null)
            {
                var status =
                    dbContext.SettingsTicketStatus.FirstOrDefault(var => var.IsActive && var.Name.Equals("Working"));

                ticket.UpdatedBy = request.UserIdNum;
                ticket.UpdatedOn = DateTime.UtcNow;
                ticket.Status = status;
                ticket.TicketStatus.Add(new TicketStatus
                {
                    Status = status,
                    ChangedBy = request.UserIdNum,
                    ChangedOn = DateTime.UtcNow,
                    CompanyId = request.CompanyIdNum,
                });

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UndoCloseTicket.Template, request.UserName, request.UserId, request.TicketId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UndoCloseTicket.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TicketId
                    })
                });

                Save();
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Ticket not found");
        }

        public BaseResponse UndoReopenTicket(TicketActionRequest request)
        {
            var ticket = GetAll()
                .Include(var => var.Status)
                .FirstOrDefault(var =>
                    var.IsActive && var.Status.Name.Equals("ReOpen") && var.Guid.Equals(request.TicketId));

            if (ticket != null)
            {
                var status =
                    dbContext.SettingsTicketStatus.FirstOrDefault(var => var.IsActive && var.Name.Equals("Closed"));

                ticket.UpdatedBy = request.UserIdNum;
                ticket.UpdatedOn = DateTime.UtcNow;
                ticket.Status = status;
                ticket.TicketStatus.Add(new TicketStatus
                {
                    Status = status,
                    ChangedBy = request.UserIdNum,
                    ChangedOn = DateTime.UtcNow,
                    CompanyId = request.CompanyIdNum,
                });

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UndoReopenTicket.Template, request.UserName, request.UserId, request.TicketId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UndoReopenTicket.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TicketId
                    })
                });

                Save();
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Ticket not found");
        }

        public GetTicketCommentsResponse AddComment(AddTicketCommentRequest request)
        {
            var ticket = GetAll()
                .Include(var => var.AddedByNavigation)
                .Include(var => var.TicketComment)
                .ThenInclude(var => var.Comment)
                .ThenInclude(var => var.AddedByNavigation)
                .FirstOrDefault(var => var.IsActive
                                       && var.Guid.Equals(request.TicketId)
                                       && (request.UserRole.Equals("HR") ||
                                           var.AddedBy.Equals(request.UserIdNum)));

            if (ticket != null)
            {
                var newComment = new TicketComment
                {
                    CompanyId = request.CompanyIdNum,
                    Guid = CustomGuid.NewGuid(),
                    Ticket = ticket,
                    IsActive = true,
                    Comment = new Comment
                    {
                        Content = request.Comment,
                        Guid = CustomGuid.NewGuid(),
                        AddedBy = request.UserIdNum,
                        AddedOn = DateTime.UtcNow,
                        IsActive = true
                    }
                };

                ticket.TicketComment.Add(newComment);

                dbContext.Notification.Add(new Notification
                {
                    Guid = CustomGuid.NewGuid(),
                    ActionId = NotificationTemplates.TicketAddComment,
                    NotificationTime = DateTime.UtcNow,
                    IsActive = true,
                    IsRead = false,
                    NotificationTo = ticket.AddedBy,
                    NotificationData = JsonConvert.SerializeObject(new
                    {
                        ticketId = ticket.Guid,
                        userName = request.UserName
                    })
                });

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.AddCommentToTicket.Template, request.UserName, request.UserId, request.TicketId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.AddCommentToTicket.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TicketId
                    })
                });

                Save();

                var addedTicket = GetAll()
                    .Include(var => var.TicketComment)
                    .ThenInclude(var => var.Comment)
                    .ThenInclude(var => var.AddedByNavigation)
                    .FirstOrDefault(var => var.Id == ticket.Id);

                var comments = addedTicket.TicketComment
                    .Where(var => var.IsActive)
                    .Select(var => new CommentDto
                    {
                        Comment = var.Comment.Content,
                        AddedBy = var.Comment.AddedByNavigation.Name,
                        AddedOn = var.Comment.AddedOn,
                        CommentId = var.Comment.Guid,
                        IsCreator = var.Comment.AddedBy == request.UserIdNum
                    })
                    .ToList();

                return new GetTicketCommentsResponse
                {
                    IsSuccess = true,
                    Comments = comments
                };
            }

            throw new Exception("Task not found or the user doesnt have permission to add comment.");
        }

        public GetTicketCommentsResponse DeleteComment(TicketCommentActionRequest request)
        {
            var ticket = GetAll()
                .Include(var => var.TicketComment).ThenInclude(var => var.Comment)
                .FirstOrDefault(var => var.IsActive
                                       && var.Guid.Equals(request.TicketId));
            if (ticket != null)
            {
                var addedComment = ticket.TicketComment
                    .FirstOrDefault(var => var.IsActive
                                           && var.Comment.IsActive  
                                           && var.Comment.AddedBy == request.UserIdNum
                                           && var.Comment.Guid.Equals(request.CommentId));
                if (addedComment != null)
                {
                    addedComment.IsActive = false;
                    addedComment.Comment.IsActive = false;
                    addedComment.Comment.UpdatedBy = request.UserIdNum;
                    addedComment.Comment.UpdatedOn = DateTime.UtcNow;

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.DeleteCommentToTicket.Template, request.UserName, request.UserId, request.TicketId),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.DeleteCommentToTicket.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName,
                            taskId = request.TicketId
                        })
                    });

                    Save();

                    var addedTicket = GetAll()
                        .Include(var => var.TicketComment)
                        .ThenInclude(var => var.Comment)
                        .ThenInclude(var => var.AddedByNavigation)
                        .FirstOrDefault(var => var.Id == ticket.Id);

                    var comments = addedTicket.TicketComment
                        .Where(var => var.IsActive)
                        .Select(var => new CommentDto
                        {
                            Comment = var.Comment.Content,
                            AddedBy = var.Comment.AddedByNavigation.Name,
                            AddedOn = var.Comment.AddedOn,
                            CommentId = var.Comment.Guid,
                            IsCreator = var.Comment.AddedBy == request.UserIdNum
                        })
                        .ToList();

                    return new GetTicketCommentsResponse
                    {
                        IsSuccess = true,
                        Comments = comments
                    };
                }

                throw new Exception("Comment not found.");
            }

            throw new Exception("Ticket not found or the user doesnt have permission to add comment.");
        }
    }
}