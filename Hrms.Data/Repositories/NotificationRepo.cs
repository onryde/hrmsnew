using System;
using System.Linq;
using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Data.Interfaces;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Hrms.Helper.Settings;
using Newtonsoft.Json;

namespace Hrms.Data.Repositories
{
    public class NotificationRepo : BaseRepository<Notification>, INotificationRepo
    {
        private IEventLogRepo _eventLogRepo;
        public NotificationRepo(HrmsContext context, IEventLogRepo eventLogRepo) : base(context)
        {
            _eventLogRepo = eventLogRepo;
        }

        public NotificationResponse GetRecentNotifications(BaseRequest request)
        {
            var recentNotifications = GetAll()
                .Where(var => var.NotificationTo == request.UserIdNum && var.IsActive)
                .OrderByDescending(var => var.NotificationTime)
                .Take(15)
                .Select(var => new NotificationDto
                {
                    Text = var.Content,
                    Data = var.NotificationData,
                    ReceivedOn = var.NotificationTime,
                    IsRead = var.IsRead,
                    NotificationId = var.Guid,
                    ReadOn = var.ReadOn,
                    ActionId = var.ActionId
                })
                .ToList();

            var count = GetAll()
                .Count(var => var.NotificationTo == request.UserIdNum && var.IsActive && !var.IsRead);

            //_eventLogRepo.AddAuditLog(new AuditInfo
            //{
            //    AuditText = string.Format(EventLogActions.GetRecentNotifications.Template, request.UserName, request.UserId),
            //    PerformedBy = request.UserIdNum,
            //    ActionId = EventLogActions.GetRecentNotifications.ActionId,
            //    Data = JsonConvert.SerializeObject(new
            //    {
            //        userId = request.UserId,
            //        userName = request.UserName
            //    })
            //});

            //Save();

            return new NotificationResponse
            {
                Notifications = recentNotifications,
                IsSuccess = true,
                UnReadNotifications = count
            };
        }

        public NotificationResponse GetAllNotifications(BaseRequest request)
        {
            var recentNotifications = GetAll()
                .Where(var => var.NotificationTo == request.UserIdNum && var.IsActive)
                .OrderByDescending(var => var.NotificationData)
                .Select(var => new NotificationDto
                {
                    Text = var.Content,
                    Data = var.NotificationData,
                    ReceivedOn = var.NotificationTime,
                    IsRead = var.IsRead,
                    NotificationId = var.Guid,
                    ReadOn = var.ReadOn,
                    ActionId = var.ActionId
                })
                .ToList();

            //_eventLogRepo.AddAuditLog(new AuditInfo
            //{
            //    AuditText = string.Format(EventLogActions.GetAllNotifications.Template, request.UserName, request.UserId),
            //    PerformedBy = request.UserIdNum,
            //    ActionId = EventLogActions.GetAllNotifications.ActionId,
            //    Data = JsonConvert.SerializeObject(new
            //    {
            //        userId = request.UserId,
            //        userName = request.UserName
            //    })
            //});

            //Save();

            return new NotificationResponse
            {
                Notifications = recentNotifications,
                IsSuccess = true
            };
        }

        public NotificationResponse MarkNotificationsAsRead(ReadNotificationRequest request)
        {
            if (request.Notifications != null)
            {
                foreach (var notification in request.Notifications)
                {
                    var addedNotificataion = GetAll()
                        .FirstOrDefault(var => var.Guid.Equals(notification.NotificationId) && !var.IsRead);
                    if (addedNotificataion != null)
                    {
                        addedNotificataion.IsRead = true;
                        addedNotificataion.ReadOn = DateTime.UtcNow;

                        Save();
                    }
                }
            }

            //_eventLogRepo.AddAuditLog(new AuditInfo
            //{
            //    AuditText = string.Format(EventLogActions.MarkReadNotifications.Template, request.UserName, request.UserId),
            //    PerformedBy = request.UserIdNum,
            //    ActionId = EventLogActions.MarkReadNotifications.ActionId,
            //    Data = JsonConvert.SerializeObject(new
            //    {
            //        userId = request.UserId,
            //        userName = request.UserName
            //    })
            //});

            //Save();

            return new NotificationResponse
            {
                IsSuccess = true
            };
        }
    }
}