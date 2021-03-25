using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces;
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
            Console.WriteLine($"Connected: " + Context.ConnectionId);

            var userName = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = await _dbContext.User.Where(x => x.UserName == userName)
                .FirstOrDefaultAsync();

            await _dbContext.UserConnection.AddAsync(
                new UserConnection
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = userId.Id,
                    UserName = userId.UserName,
                });

            await _dbContext.SaveChangesAsync();

            await base.OnConnectedAsync();
        }

        public override async System.Threading.Tasks.Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Disconnected: " + Context.ConnectionId);

            var user = await _dbContext.UserConnection
                .Where(x => x.UserName == Context.User.FindFirstValue(ClaimTypes.NameIdentifier) && x.ConnectionId == Context.ConnectionId)
                .FirstOrDefaultAsync();

            _dbContext.UserConnection.Remove(user);

            await _dbContext.SaveChangesAsync();

            await base.OnDisconnectedAsync(exception);
        }
    }
}
