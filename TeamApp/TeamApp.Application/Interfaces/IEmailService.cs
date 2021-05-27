using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Email;

namespace TeamApp.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
        Task SendAsyncAWS(EmailRequest request);
    }
}
