using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamApp.Infrastructure.Persistence.Entities;

namespace TeamApp.WebApi.Hubs.Chat
{
    [Authorize]
    public class HubChatClient : Hub<IHubChatClient>
    {
        private readonly TeamAppContext _dbContext;
        public HubChatClient(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public override async System.Threading.Tasks.Task OnConnectedAsync()
        {
            var userName = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Connected {Context.ConnectionId}, Usename {userName}");
            var userId = await _dbContext.User.Where(x => x.UserName == userName).FirstOrDefaultAsync();

            var uc = new UserConnection
            {
                ConnectionId = Context.ConnectionId,
                UserName = userName,
                UserId = userId.Id,
            };

            await _dbContext.UserConnection.AddAsync(uc);
            await _dbContext.SaveChangesAsync();

            await base.OnConnectedAsync();
        }

        public override async System.Threading.Tasks.Task OnDisconnectedAsync(Exception exception)
        {
            var userName = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Disconnected {Context.ConnectionId}, Username {userName}");

            var userCon = await _dbContext.UserConnection.Where(x => x.UserName == userName && x.ConnectionId == Context.ConnectionId).FirstOrDefaultAsync();

            _dbContext.UserConnection.Remove(userCon);

            await _dbContext.SaveChangesAsync();

            await base.OnDisconnectedAsync(exception);
        }
    }
}
