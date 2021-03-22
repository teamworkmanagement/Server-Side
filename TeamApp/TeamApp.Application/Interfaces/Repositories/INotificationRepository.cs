using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Models.Notification;

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
