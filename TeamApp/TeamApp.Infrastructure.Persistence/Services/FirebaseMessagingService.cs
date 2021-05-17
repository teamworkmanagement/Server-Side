using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Notification;
using TeamApp.Application.Interfaces;

namespace TeamApp.Infrastructure.Persistence.Services
{
    public class FirebaseMessagingService : IFirebaseMessagingService
    {
        private readonly FirebaseMessaging _messaging;
        private readonly IConfiguration _config;
        public FirebaseMessagingService(IConfiguration configuration)
        {
            _config = configuration;
            FirebaseApp app;

            var outPut = JsonSerializer.Serialize(_config.GetSection("FirebaseMessaging").Get<FirebaseConfig>());

            if (FirebaseApp.DefaultInstance == null)
            {
                app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(outPut)
                });

            }

            else app = FirebaseApp.DefaultInstance;

            _messaging = FirebaseMessaging.GetMessaging(app);


        }

        public Message CreateNotification(string title, string notificationBody, string token)
        {
            return new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Body = notificationBody,
                    Title = title
                },
                Data = new Dictionary<string, string>()
                {
                    { "AdditionalData1", "data 1" },
                },
            };
        }

        public async Task<string> SendNotification(string token, string title, string body)
        {
            Message message = new Message()
            {
                Token = token,
                Notification = new Notification
                {
                    Title = "My push notification title",
                    Body = "Content for this push notification"
                },
                Data = new Dictionary<string, string>(),
            };

            var mes = CreateNotification(title, body, token);
            var result = await _messaging.SendAsync(mes);
            Console.WriteLine(result);
            return result;
        }
    }
}
