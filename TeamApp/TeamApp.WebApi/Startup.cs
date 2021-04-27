using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TeamApp.Application;
using TeamApp.Application.Interfaces;
using TeamApp.Infrastructure.Persistence;
using TeamApp.Infrastructure.Shared;
using TeamApp.WebApi.Extensions;
using TeamApp.WebApi.Hubs.Chat;
using TeamApp.WebApi.Services;

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

            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        //.SetIsOriginAllowed(origin => true)
                        .WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "http://192.168.137.1:3000",
                        "https://localhost:3000", "https://localhost:3001", "https://localhost:3002", "https://192.168.137.1:3000",
                        "http://localhost:9000", "http://localhost:9001",
                        "https://ezteamapp.space", "https://*.ezteamapp.space")
                        .AllowCredentials();
                });
            });

            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            //app.UseHttpsRedirection();
            app.UseCors("ClientPermission");
            app.UseRouting();

            app.UseErrorHandlingMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseSwaggerExtension();

            app.UseHealthChecks("/health");

            app.UseEndpoints(endpoints =>
             {
                 endpoints.MapControllers();
                 endpoints.MapHub<HubChatClient>("/hubchat");
             });
        }
    }
}
