using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TeamApp.Infrastructure.Persistence.Services
{
    internal sealed class MySecurityToken
    {
        private readonly byte[] _data;

        public MySecurityToken(byte[] data)
        {
            _data = (byte[])data.Clone();
        }

        internal byte[] GetDataNoClone()
        {
            return _data;
        }
    }

    internal static class Rfc6238AuthenticationService
    {
        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly TimeSpan _timestep = TimeSpan.FromMinutes(3);
        private static readonly Encoding _encoding = new UTF8Encoding(false, true);

        private static int ComputeTotp(HashAlgorithm hashAlgorithm, ulong timestepNumber, string modifier, int numberOfDigits = 6)
        {
            // # of 0's = length of pin
            //const int mod = 1000000;
            var mod = (int)Math.Pow(10, numberOfDigits);

            // See https://tools.ietf.org/html/rfc4226
            // We can add an optional modifier
            var timestepAsBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)timestepNumber));
            var hash = hashAlgorithm.ComputeHash(ApplyModifier(timestepAsBytes, modifier));

            // Generate DT string
            var offset = hash[hash.Length - 1] & 0xf;
            Debug.Assert(offset + 4 < hash.Length);
            var binaryCode = (hash[offset] & 0x7f) << 24
                             | (hash[offset + 1] & 0xff) << 16
                             | (hash[offset + 2] & 0xff) << 8
                             | (hash[offset + 3] & 0xff);

            var code = binaryCode % mod;
            return code;
        }

        private static byte[] ApplyModifier(byte[] input, string modifier)
        {
            if (String.IsNullOrEmpty(modifier))
            {
                return input;
            }

            var modifierBytes = _encoding.GetBytes(modifier);
            var combined = new byte[checked(input.Length + modifierBytes.Length)];
            Buffer.BlockCopy(input, 0, combined, 0, input.Length);
            Buffer.BlockCopy(modifierBytes, 0, combined, input.Length, modifierBytes.Length);
            return combined;
        }

        // More info: https://tools.ietf.org/html/rfc6238#section-4
        private static ulong GetCurrentTimeStepNumber()
        {
            var delta = DateTime.UtcNow - _unixEpoch;
            return (ulong)(delta.Ticks / _timestep.Ticks);
        }

        public static int GenerateCode(MySecurityToken securityToken, string modifier = null, int numberOfDigits = 6)
        {

            if (securityToken == null)
            {
                throw new ArgumentNullException("securityToken");
            }

            // Allow a variance of no greater than 90 seconds in either direction
            var currentTimeStep = GetCurrentTimeStepNumber();
            using (var hashAlgorithm = new HMACSHA1(securityToken.GetDataNoClone()))
            {
                var code = ComputeTotp(hashAlgorithm, currentTimeStep, modifier, numberOfDigits);
                return code;
            }
        }

        public static bool ValidateCode(MySecurityToken securityToken, int code, string modifier = null, int numberOfDigits = 6)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException("securityToken");
            }

            // Allow a variance of no greater than 90 seconds in either direction
            var currentTimeStep = GetCurrentTimeStepNumber();
            using (var hashAlgorithm = new HMACSHA1(securityToken.GetDataNoClone()))
            {
                for (var i = -2; i <= 2; i++)
                {
                    var computedTotp = ComputeTotp(hashAlgorithm, (ulong)((long)currentTimeStep + i), modifier, numberOfDigits);
                    if (computedTotp == code)
                    {
                        return true;
                    }
                }
            }

            // No match
            return false;
        }
    }
    public class CustomPasswordResetTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomPasswordResetTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<CustomPasswordResetTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
        {

        }
		public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
		{
			return System.Threading.Tasks.Task.FromResult(false);
		}

		public override async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
		{
			var token = new MySecurityToken(await manager.CreateSecurityTokenAsync(user));
			var modifier = await GetUserModifierAsync(purpose, manager, user);
			var code = Rfc6238AuthenticationService.GenerateCode(token, modifier, 6).ToString("D6", CultureInfo.InvariantCulture);

			return code;
		}

        private async Task<string> GetUserModifierAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
			var email = await manager.GetEmailAsync(user);
			return "PasswordlessLogin:" + purpose + ":" + email;
		}

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
		{
			int code;
			if (!int.TryParse(token, out code))
			{
				return false;
			}
			var securityToken = new MySecurityToken(await manager.CreateSecurityTokenAsync(user));
			var modifier = await GetUserModifierAsync(purpose, manager, user);
			var valid = Rfc6238AuthenticationService.ValidateCode(securityToken, code, modifier, token.Length);
			return valid;
		}
	}
}
