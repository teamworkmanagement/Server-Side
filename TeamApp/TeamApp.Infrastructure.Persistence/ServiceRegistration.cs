using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Infrastructure.Persistence.Repositories;

namespace TeamApp.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<KhoaLuanContext>(options =>
                    options.UseInMemoryDatabase("ApplicationDb"));
            }
            else
            {
                services.AddDbContext<KhoaLuanContext>(options =>
               options.UseMySql(
                   configuration.GetConnectionString("DefaultConnection")));
            }
            #region Repositories
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<IParticipationRepository, ParticipationRepository>();
            services.AddTransient<ITaskRepository, TaskRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddTransient<IHandleTaskRepository, HandleTaskRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<ITaskVersionRepository, TaskVersionRepository>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IGroupChatRepository, GroupChatRepository>();
            services.AddTransient<IGroupChatUserRepository, GroupChatUserRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            #endregion
        }
    }
}
