using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Text;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Settings;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Infrastructure.Persistence.Repositories;
using TeamApp.Infrastructure.Persistence.Services;

namespace TeamApp.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<TeamAppContext>(options =>
                    options.UseInMemoryDatabase("ApplicationDb"));
            }
            else
            {
                services.AddDbContext<TeamAppContext>(options =>
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
            services.AddTransient<IKanbanBoardRepository, KanbanBoardRepository>();
            services.AddTransient<IKanbanListRepository, KanbanListRepository>();
            services.AddTransient<IStatisticsRepository, StatisticsRepository>();
            #endregion
            services.AddTransient<IFirebaseMessagingService, FirebaseMessagingService>();
            ConfigAuthService(services, configuration);
        }
        public static void ConfigAuthService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuthorizationHandler, IpCheckHandler>();
            services.AddSingleton<IAuthorizationHandler, TeamCheckHandler>();
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Tokens.PasswordResetTokenProvider = "CutomPasswordReset";
            }).AddEntityFrameworkStores<TeamAppContext>().AddDefaultTokenProviders()
            .AddTokenProvider<CustomPasswordResetTokenProvider<User>>("CutomPasswordReset");
                
            #region Services
            services.AddTransient<IAccountService, AccountService>();
            #endregion
            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            services.Configure<MyAppSettings>(configuration.GetSection("MyApp"));
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SameIpPolicy",
                    policy => policy.Requirements.Add(new IpCheckRequirement { IpClaimRequired = true }));

                options.AddPolicy("TeamPolicy",
                    policy => policy.Requirements.Add(new TeamCheckRequirement { }));
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };
                    o.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = c =>
                        {
                            c.NoResult();
                            if (c.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                c.Response.Headers.Add("X-Token-Expired", "true");
                                c.Response.Cookies.Append("TokenExpired", "true", new CookieOptions
                                {
                                    Domain = configuration["MyApp:Url"],
                                    Expires = DateTime.UtcNow.AddMinutes(5),
                                    Secure = true,
                                    HttpOnly = false,
                                    SameSite = SameSiteMode.None,
                                });
                            }
                            c.Response.StatusCode = 500;
                            c.Response.ContentType = "text/plain";
                            var responseModel = new ApiResponse<string>() { Succeeded = false, Message = c.Exception.ToString(), };
                            return c.Response.WriteAsync(JsonConvert.SerializeObject(responseModel));
                        },
                        /*OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new ApiResponse<string>("You are not Authorized"));
                            return context.Response.WriteAsync(result);
                        },*/
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new ApiResponse<string>("You are not authorized to access this resource"));
                            return context.Response.WriteAsync(result);
                        },
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubchat"))
                            {

                                context.Token = accessToken;
                            }
                            return System.Threading.Tasks.Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
