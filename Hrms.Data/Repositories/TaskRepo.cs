using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;

namespace Hrms.Data.Repositories
{
    public class TaskRepo : BaseRepository<Task>, ITaskRepo
    {
        private IEventLogRepo _eventLogRepo;
        public TaskRepo(HrmsContext context, IEventLogRepo eventLogRepo) : base(context)
        {
            _eventLogRepo = eventLogRepo;
        }

        public TaskDetailsResponse GetTaskDetails(TaskActionRequest request)
        {
            var addedTask = GetAll()
                .Include(var => var.TaskComment).ThenInclude(var => var.Comment)
                .ThenInclude(var => var.AddedByNavigation)
                .Include(var => var.AssignedToNavigation)
                .Include(var => var.AddedByNavigation)
                .FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.TaskId) &&
                    (var.AssignedTo == request.UserIdNum || var.AddedBy == request.UserIdNum));

            if (addedTask != null)
            {
                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetTaskDetails.Template, request.UserName, request.UserId, request.TaskId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetTaskDetails.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TaskId
                    })
                });

                return new TaskDetailsResponse
                {
                    Content = addedTask.TaskContent,
                    AddedBy = addedTask.AddedByNavigation.Name,
                    AddedOn = addedTask.AddedOn,
                    AssignedTo = addedTask.AssignedToNavigation.Name,
                    DueOn = addedTask.DueDate,
                    TaskId = addedTask.Guid,
                    AssignedToId = addedTask.AssignedToNavigation.Guid,
                    IsCompleted = addedTask.IsCompleted,
                    IsIrrelevant = addedTask.IsIrrelevant ?? false,
                    IsStarted = addedTask.IsStarted,
                    IsVerified = addedTask.IsVerified,
                    IsCreator = addedTask.AddedBy == request.UserIdNum,
                    IsSelf = addedTask.AssignedTo == request.UserIdNum,
                    IsSuccess = true,
                    CompletedOn = addedTask.CompletedOn,
                    StartedOn = addedTask.StartedOn,
                    VerifiedOn = addedTask.VerifiedOn,
                    HourTime = addedTask.HourTime ?? 10,
                    TaskPriority = addedTask.Priority,
                    MinuteTime = addedTask.MinuteTime ?? 0,
                    Comments = addedTask.TaskComment
                        .Where(var => var.IsActive)
                        .Select(var => new CommentDto
                        {
                            Comment = var.Comment.Content,
                            AddedBy = var.Comment.AddedByNavigation.Name,
                            AddedOn = var.Comment.AddedOn,
                            CommentId = var.Comment.Guid,
                            IsCreator = var.Comment.AddedBy == request.UserIdNum
                        })
                        .OrderByDescending(var => var.AddedOn)
                        .ToList()
                };
            }

            throw new Exception("Task not found");
        }

        public GetTasksResponse UpdateTask(UpdateTaskRequest request)
        {
            var assignedTo = string.IsNullOrWhiteSpace(request.AssignedTo)
                ? request.UserIdNum
                : dbContext.Employee.FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.AssignedTo)).Id;

            var guid = CustomGuid.NewGuid();
            if (string.IsNullOrWhiteSpace(request.TaskId))
            {
                var newTask = new Task
                {
                    CompanyId = request.CompanyIdNum,
                    Guid = guid,
                    AddedBy = request.UserIdNum,
                    AddedOn = DateTime.UtcNow,
                    AssignedTo = assignedTo,
                    DueDate = request.DueDate,
                    IsActive = true,
                    IsCompleted = false,
                    IsStarted = false,
                    TaskComment = new List<TaskComment>(),
                    TaskContent = request.TaskContent,
                    HourTime = request.HourTime,
                    MinuteTime = request.MinuteTime,
                    Priority = request.TaskPriority
                };

                AddItem(newTask);

                if (request.UserIdNum != assignedTo)
                {
                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.NewTask,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = assignedTo,
                        NotificationData = JsonConvert.SerializeObject(new
                        {
                            taskId = guid,
                            userName = request.UserName
                        })
                    });
                }

                Save();

                var addedTask = GetAll()
                    .Include(var => var.AddedByNavigation)
                    .Include(var => var.AssignedToNavigation)
                    .FirstOrDefault(var => var.IsActive && var.Guid.Equals(newTask.Guid));

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.UpdateTask.Template, request.UserName, request.UserId, guid),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.UpdateTask.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = guid
                    })
                });

                return new GetTasksResponse
                {
                    IsSuccess = true,
                    Tasks = new List<TaskDto>
                    {
                        new TaskDto
                        {
                            Content = addedTask.TaskContent,
                            AddedBy = addedTask.AddedByNavigation.Name,
                            AddedOn = addedTask.AddedOn,
                            AssignedTo = addedTask.AssignedToNavigation.Name,
                            TaskId = addedTask.Guid,
                            DueOn = addedTask.DueDate,
                            AssignedToId = addedTask.AssignedToNavigation.Guid
                        }
                    }
                };
            }
            else
            {
                var addedTask = GetAll()
                    .FirstOrDefault(var =>
                        var.IsActive && var.Guid.Equals(request.TaskId) && var.AddedBy == request.UserIdNum);
                if (addedTask != null)
                {
                    guid = request.TaskId;

                    addedTask.AssignedTo = assignedTo;
                    addedTask.DueDate = request.DueDate;
                    addedTask.TaskContent = request.TaskContent;
                    addedTask.UpdatedBy = request.UserIdNum;
                    addedTask.UpdatedOn = DateTime.UtcNow;
                    addedTask.HourTime = request.HourTime;
                    addedTask.MinuteTime = request.MinuteTime;
                    addedTask.Priority = request.TaskPriority;

                    if (assignedTo == addedTask.AssignedTo)
                    {
                        if (addedTask.AddedBy != assignedTo)
                        {
                            dbContext.Notification.Add(new Notification
                            {
                                Guid = CustomGuid.NewGuid(),
                                ActionId = NotificationTemplates.UpdateTask,
                                NotificationTime = DateTime.UtcNow,
                                IsActive = true,
                                IsRead = false,
                                NotificationTo = assignedTo,
                                NotificationData = JsonConvert.SerializeObject(new
                                {
                                    taskId = addedTask.Guid,
                                    userName = request.UserName
                                })
                            });
                        }
                    }
                    else
                    {
                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.ChangeAssignedTask,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = assignedTo,
                            NotificationData = JsonConvert.SerializeObject(new
                            {
                                taskId = addedTask.Guid,
                                userName = request.UserName
                            })
                        });
                    }

                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.UpdateTask.Template, request.UserName, request.UserId, request.TaskId),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.UpdateTask.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName,
                            taskId = request.TaskId
                        })
                    });

                    Save();
                    return new GetTasksResponse
                    {
                        IsSuccess = true,
                    };
                }

                throw new Exception("Task not found");
            }
        }

        public BaseResponse DeleteTask(TaskActionRequest request)
        {
            var addedTask = GetAll()
                .FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.TaskId) && var.AddedBy == request.UserIdNum);
            if (addedTask != null)
            {
                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.DeleteTask.Template, request.UserName, request.UserId, request.TaskId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.DeleteTask.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TaskId
                    })
                });

                addedTask.IsActive = false;
                addedTask.UpdatedBy = request.UserIdNum;
                addedTask.UpdatedOn = DateTime.UtcNow;

                if (addedTask.AddedBy != addedTask.AssignedTo)
                {
                    dbContext.Notification.Add(new Notification
                    {
                        Guid = CustomGuid.NewGuid(),
                        ActionId = NotificationTemplates.DeleteTask,
                        NotificationTime = DateTime.UtcNow,
                        IsActive = true,
                        IsRead = false,
                        NotificationTo = addedTask.AssignedTo,
                        NotificationData = null
                    });
                }

                Save();

                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Task not found");
        }

        public BaseResponse ToggleStartTask(TaskActionRequest request)
        {
            var addedTask = GetAll()
                .FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.TaskId) && var.AssignedTo == request.UserIdNum);
            if (addedTask != null)
            {
                addedTask.StartedOn = addedTask.IsStarted ? (DateTime?) null : DateTime.UtcNow;
                addedTask.IsStarted = !addedTask.IsStarted;
                addedTask.UpdatedBy = request.UserIdNum;
                addedTask.UpdatedOn = DateTime.UtcNow;

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.ToggleStartTask.Template, request.UserName, request.UserId, request.TaskId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.ToggleStartTask.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TaskId
                    })
                });

                Save();
                return new BaseResponse
                {
                    IsSuccess = true,
                };
            }

            throw new Exception("Task not found");
        }

        public BaseResponse ToggleCompleteTask(TaskActionRequest request)
        {
            var addedTask = GetAll()
                .FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.TaskId) && var.AssignedTo == request.UserIdNum);
            if (addedTask != null)
            {
                addedTask.CompletedOn = addedTask.IsCompleted ? (DateTime?) null : DateTime.UtcNow;
                addedTask.IsCompleted = !addedTask.IsCompleted;
                addedTask.UpdatedBy = request.UserIdNum;
                addedTask.UpdatedOn = DateTime.UtcNow;

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.ToggleCompleteTask.Template, request.UserName, request.UserId, request.TaskId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.ToggleCompleteTask.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TaskId
                    })
                });

                if (addedTask.IsCompleted)
                {
                    if (addedTask.AddedBy != addedTask.AssignedTo)
                    {
                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.CompleteTask,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = addedTask.AddedBy,
                            NotificationData = JsonConvert.SerializeObject(new
                            {
                                taskId = addedTask.Guid,
                                userName = request.UserName
                            })
                        });
                    }
                }

                Save();
                return new BaseResponse
                {
                    IsSuccess = true,
                };
            }

            throw new Exception("Task not found");
        }

        public BaseResponse ToggleVerifyTask(TaskActionRequest request)
        {
            var addedTask = GetAll()
                .FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.TaskId) && var.AddedBy == request.UserIdNum);
            if (addedTask != null)
            {
                addedTask.VerifiedOn = addedTask.IsVerified ? (DateTime?) null : DateTime.UtcNow;
                addedTask.IsVerified = !addedTask.IsVerified;
                addedTask.UpdatedBy = request.UserIdNum;
                addedTask.UpdatedOn = DateTime.UtcNow;

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.ToggleVerifyTask.Template, request.UserName, request.UserId, request.TaskId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.ToggleVerifyTask.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TaskId
                    })
                });

                if (addedTask.IsVerified)
                {
                    if (addedTask.AddedBy != addedTask.AssignedTo)
                    {
                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.VerifyTask,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = addedTask.AssignedTo,
                            NotificationData = JsonConvert.SerializeObject(new
                            {
                                taskId = addedTask.Guid,
                                userName = request.UserName
                            })
                        });
                    }
                }

                Save();
                return new BaseResponse
                {
                    IsSuccess = true,
                };
            }

            throw new Exception("Task not found");
        }

        public BaseResponse ToggleIrrelevant(TaskActionRequest request)
        {
            var addedTask = GetAll()
                .FirstOrDefault(var =>
                    var.IsActive && var.Guid.Equals(request.TaskId) && var.AssignedTo == request.UserIdNum);
            if (addedTask != null)
            {
                addedTask.IsIrrelevant = !addedTask.IsIrrelevant;
                addedTask.UpdatedBy = request.UserIdNum;
                addedTask.UpdatedOn = DateTime.UtcNow;

                Save();
                return new BaseResponse
                {
                    IsSuccess = true,
                };
            }

            throw new Exception("Task not found");
        }

        public BaseResponse AddCommentToTask(AddTaskCommentRequest request)
        {
            var task = GetAll()
                .Include(var => var.TaskComment)
                .FirstOrDefault(var => var.IsActive
                                       && var.Guid.Equals(request.TaskId)
                                       && (var.AssignedTo.Equals(request.UserIdNum) ||
                                           var.AddedBy.Equals(request.UserIdNum)));
            if (task != null)
            {
                var newComment = new TaskComment
                {
                    CompanyId = request.CompanyIdNum,
                    Guid = CustomGuid.NewGuid(),
                    Task = task,
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

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.AddCommentToTask.Template, request.UserName, request.UserId, request.TaskId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.AddCommentToTask.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TaskId
                    })
                });

                task.TaskComment.Add(newComment);

                if (task.AddedBy != task.AssignedTo)
                {
                    if (request.UserIdNum == task.AddedBy)
                    {
                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.AddTaskCommentAssigned,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = task.AssignedTo,
                            NotificationData = JsonConvert.SerializeObject(new
                            {
                                taskId = task.Guid,
                                userName = request.UserName
                            })
                        });
                    }
                    else
                    {
                        dbContext.Notification.Add(new Notification
                        {
                            Guid = CustomGuid.NewGuid(),
                            ActionId = NotificationTemplates.AddTaskCommentAddedBy,
                            NotificationTime = DateTime.UtcNow,
                            IsActive = true,
                            IsRead = false,
                            NotificationTo = task.AddedBy,
                            NotificationData = JsonConvert.SerializeObject(new
                            {
                                taskId = task.Guid,
                                userName = request.UserName
                            })
                        });
                    }
                }

                Save();
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            throw new Exception("Task not found or the user doesnt have permission to add comment.");
        }

        public BaseResponse DeleteCommentOnTask(TaskCommentActionRequest request)
        {
            var task = GetAll()
                .Include(var => var.TaskComment).ThenInclude(var => var.Comment)
                .FirstOrDefault(var => var.IsActive
                                       && var.Guid.Equals(request.TaskId));
            if (task != null)
            {
                var addedComment = task.TaskComment
                    .FirstOrDefault(var => var.IsActive
                                           && var.Comment.IsActive
                                           && var.Comment.AddedBy == request.UserIdNum
                                           && var.Comment.Guid.Equals(request.CommentId));
                if (addedComment != null)
                {
                    _eventLogRepo.AddAuditLog(new AuditInfo
                    {
                        AuditText = string.Format(EventLogActions.DeleteCommentOnTask.Template, request.UserName, request.UserId, request.TaskId),
                        PerformedBy = request.UserIdNum,
                        ActionId = EventLogActions.DeleteCommentOnTask.ActionId,
                        Data = JsonConvert.SerializeObject(new
                        {
                            userId = request.UserId,
                            userName = request.UserName,
                            taskId = request.TaskId
                        })
                    });

                    addedComment.IsActive = false;
                    addedComment.Comment.IsActive = false;
                    addedComment.Comment.UpdatedBy = request.UserIdNum;
                    addedComment.Comment.UpdatedOn = DateTime.UtcNow;

                    Save();

                    return new BaseResponse
                    {
                        IsSuccess = true
                    };
                }

                throw new Exception("Comment not found.");
            }

            throw new Exception("Task not found or the user doesnt have permission to add comment.");
        }

        public GetTaskCommentsResponse GetTaskComments(TaskActionRequest request)
        {
            var addedTask = GetAll()
                .Include(var => var.TaskComment).ThenInclude(var => var.Comment)
                .ThenInclude(var => var.AddedByNavigation)
                .FirstOrDefault(var => var.IsActive && var.Guid.Equals(request.TaskId));
            if (addedTask != null)
            {
                var comments = addedTask.TaskComment
                    .Where(var => var.IsActive)
                    .Select(var => new CommentDto
                    {
                        Comment = var.Comment.Content,
                        AddedBy = var.Comment.AddedByNavigation.Name,
                        AddedOn = var.Comment.AddedOn,
                        CommentId = var.Comment.Guid,
                        IsCreator = var.Comment.AddedBy == request.UserIdNum
                    })
                    .OrderByDescending(var => var.AddedOn)
                    .ToList();

                _eventLogRepo.AddAuditLog(new AuditInfo
                {
                    AuditText = string.Format(EventLogActions.GetTaskComments.Template, request.UserName, request.UserId, request.TaskId),
                    PerformedBy = request.UserIdNum,
                    ActionId = EventLogActions.GetTaskComments.ActionId,
                    Data = JsonConvert.SerializeObject(new
                    {
                        userId = request.UserId,
                        userName = request.UserName,
                        taskId = request.TaskId
                    })
                });

                return new GetTaskCommentsResponse
                {
                    Comments = comments,
                    IsSuccess = true
                };
            }

            throw new Exception("Task not found");
        }
    }
}