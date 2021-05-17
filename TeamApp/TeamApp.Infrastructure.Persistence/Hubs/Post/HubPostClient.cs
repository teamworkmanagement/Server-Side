using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamApp.Infrastructure.Persistence.Entities;
using Task = System.Threading.Tasks.Task;

namespace TeamApp.Infrastructure.Persistence.Hubs.Post
{
    [Authorize]
    public class HubPostClient : Hub<IHubPostClient>
    {
        private readonly TeamAppContext _dbContext;
        public HubPostClient(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public override async Task OnConnectedAsync()
        {
            var userName = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Post: Connected {Context.ConnectionId}, Usename {userName}");
            var user = await _dbContext.User.AsNoTracking().Where(x => x.UserName == userName).FirstOrDefaultAsync();

            var uc = new UserConnection
            {
                ConnectionId = Context.ConnectionId,
                UserName = userName,
                UserId = user.Id,
                Type = "post",
            };

            await _dbContext.UserConnection.AddAsync(uc);
            await _dbContext.SaveChangesAsync();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userName = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Post: Disconnected {Context.ConnectionId}, Username {userName}");

            var userCon = await _dbContext.UserConnection.Where(x => x.UserName == userName && x.ConnectionId == Context.ConnectionId).FirstOrDefaultAsync();

            _dbContext.UserConnection.Remove(userCon);

            await _dbContext.SaveChangesAsync();

            await base.OnDisconnectedAsync(exception);
        }
    }
}
