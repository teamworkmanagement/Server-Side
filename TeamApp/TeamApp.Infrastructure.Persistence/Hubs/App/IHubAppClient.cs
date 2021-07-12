using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TeamApp.Infrastructure.Persistence.Hubs.App
{
    public interface IHubAppClient
    {
        Task UpdateUserInfo(object updateUserInfo);
        Task UpdateTeamInfo(object updateTeamInfo);
    }
}
