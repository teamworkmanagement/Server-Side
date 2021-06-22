using Microsoft.AspNetCore.Identity;
using System;

namespace TeamApp.Infrastructure.Persistence.Services
{
    public class CustomPasswordResetTokenProviderOptions: DataProtectionTokenProviderOptions
    {
        public CustomPasswordResetTokenProviderOptions()
        {
            TokenLifespan = TimeSpan.FromMinutes(1);
        }
    }
}