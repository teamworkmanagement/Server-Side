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
        Task<PagedResponse<NotificationResponse>> GetPaging(NotificationRequestParameter parameter);
        Task<bool> ReadNotificationSet(string notiId);
        Task<string> PushNoti(string token, string title, string body);
    }
}
