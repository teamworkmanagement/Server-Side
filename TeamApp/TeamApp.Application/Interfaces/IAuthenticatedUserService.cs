using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.Interfaces
{
    public interface IAuthenticatedUserService
    {
        string UserId { get; }
    }
}
