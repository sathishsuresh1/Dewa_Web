using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace DEWAXP.Foundation.Integration.Helpers
{
    public static class EPayNonceGenerator
    {
        private static readonly Random Rng = new Random((int)DateTime.Now.Ticks);
        
        public static string GenerateNonce(string sharedSecret)
        {
            var random = GenerateRandomValue();
            var timestamp = DateTime.UtcNow.ToString("DD/MM/YYYY HH:MM:ss");

            var nonce = string.Concat(random, timestamp, sharedSecret);
            var nonceBytes = Encoding.ASCII.GetBytes(nonce);

            using (var sha = new SHA512Managed())
            {
                var encoded = sha.ComputeHash(nonceBytes, 0, nonceBytes.Length);

                return Convert.ToBase64String(encoded);
            }
        }

        private static string GenerateRandomValue() => Membership.GeneratePassword(12, 0);
    }
}