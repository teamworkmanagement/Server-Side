using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TeamApp.Application;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence;
using TeamApp.Infrastructure.Persistence.Hubs.App;
using TeamApp.Infrastructure.Persistence.Hubs.Chat;
using TeamApp.Infrastructure.Persistence.Hubs.Kanban;
using TeamApp.Infrastructure.Persistence.Hubs.Notification;
using TeamApp.Infrastructure.Persistence.Hubs.Post;
using TeamApp.Infrastructure.Shared;
using TeamApp.WebApi.Extensions;

namespace TeamApp.WebApi
{
    public class Startup
    {
        public IConfiguration _config { get; }
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationLayer();
            services.AddPersistenceInfrastructure(_config);
            services.AddSharedInfrastructure(_config);
            services.AddSwaggerExtension();
            services.AddControllers();
            services.AddHealthChecks();

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseMemoryStorage();
            });

            services.AddSignalR();
            services.AddHangfireServer();
            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(origin => true)
                        .AllowCredentials();
                });
            });

            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            app.UseCors("ClientPermission");
            app.UseRouting();

            app.UseStaticFiles();
            app.UseErrorHandlingMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseSwaggerExtension();


            app.UseHealthChecks("/health");

            app.UseEndpoints(endpoints =>
             {
                 endpoints.MapControllers();
                 endpoints.MapHub<HubChatClient>("/hubchat");
                 endpoints.MapHub<HubPostClient>("/hubpost");
                 endpoints.MapHub<HubKanbanClient>("/hubkanban");
                 endpoints.MapHub<HubNotificationClient>("/hubnoti");
                 endpoints.MapHub<HubAppClient>("/hubapp");
             });

            recurringJobManager.AddOrUpdate(
                "Run nmins interval",
                () => serviceProvider.GetService<INotificationRepository>().Reminder()
                ,
                "*/1 * * * *");
        }
    }
}
