using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Notification;
using TeamApp.Application.Filters;
using TeamApp.Application.Wrappers;


namespace TeamApp.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<List<NotificationResponse>> GetAllByUserId(string userId);
        Task<PagedResponse<NotificationResponse>> GetPaging(RequestParameter parameter);
        Task<bool> ReadNotificationSet(string notiId);
        Task<bool> DeleteNotification(string notiId);
    }
}
