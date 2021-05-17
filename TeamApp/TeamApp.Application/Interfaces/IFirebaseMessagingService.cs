using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TeamApp.Application.Interfaces
{
    public interface IFirebaseMessagingService
    {
        Task<string> SendNotification(string token, string title, string body);
    }
}
