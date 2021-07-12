using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamApp.Infrastructure.Persistence.Entities;
using Task = System.Threading.Tasks.Task;

namespace TeamApp.Infrastructure.Persistence.Hubs.App
{
    [Authorize]
    public class HubAppClient: Hub<IHubAppClient>
    {
        private readonly TeamAppContext _dbContext;
        private readonly UserManager<User> _userManager;
        public HubAppClient(TeamAppContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.Identities.ToList()[0].Claims.ToList()[1].Value.ToString();
            Console.WriteLine("App connected: " + userId);
            var uc = new UserConnection
            {
                ConnectionId = Context.ConnectionId,
                UserId = userId,
                Type = "app"
            };

            await _dbContext.UserConnection.AddAsync(uc);
            await _dbContext.SaveChangesAsync();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.Identities.ToList()[0].Claims.ToList()[1].Value.ToString();
            Console.WriteLine("App disconnected: " + userId);
            var userCon = await _dbContext.UserConnection.Where(x => x.UserId == userId && x.ConnectionId == Context.ConnectionId).FirstOrDefaultAsync();

            _dbContext.UserConnection.Remove(userCon);

            await _dbContext.SaveChangesAsync();

            await base.OnDisconnectedAsync(exception);
        }
    }
}
