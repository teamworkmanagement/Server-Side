using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamApp.Infrastructure.Persistence.Hubs.Notification
{
    public interface IHubNotificationClient
    {
        Task SendNoti(object message);
        Task Reminder(object reminder);
    }
}
