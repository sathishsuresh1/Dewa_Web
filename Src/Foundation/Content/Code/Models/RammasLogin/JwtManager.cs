using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web.Configuration;


namespace DEWAXP.Foundation.Content.Models.RammasLogin
{
    public class FusoClaims
    {
        public const string UserId = "user.upn";
        public const string BotChannelId = "bot.channel";
        public const string BotConversationId = "bot.conv";
        public const string ServiceUri = "bot.svuri";
        public const string ActivityId = "bot.activity";
        public const string FromId = "bot.fromId";
        public const string FromName = "bot.fromName";
        public const string ToId = "bot.toId";
        public const string ToName = "bot.toName";
        public const string LoginType = "bot.loginType";
        public const string SessionId = "bot.sessionId";
        public const string LanguageCode = "bot.languageCode";
        public const string MagicNumber = "bot.magicNumber";
    }

    /// <summary>
    /// The JWT Manager
    /// </summary>
    public class JwtManager
    {
        private string _secret = "856FECBA3B06519C8DDDBC80BB080553"; // your symmetric

        public JwtManager()
        {
        }

        /// <summary>
        /// Generates the token.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="expireMinutes">The expire minutes.</param>
        /// <returns></returns>
        //public string GenerateToken(string userId, string sessionId, string channelId, string conversationId, string serviceUri, string activityId, string fromId, string fromName, string toId, string toName, string languageCode, int magicNumber, string loginType, int expireMinutes = 20)
        public string GenerateToken(string userId, string sessionId, string channelId, string conversationId,  string languageCode, int magicNumber, string loginType, int expireMinutes = 20)
        {
            //var jwtAudience = WebConfigurationManager.AppSettings["JWT.Audience"];
            //var jwtIssuer = WebConfigurationManager.AppSettings["JWT.AuthenticationUrl"];

            var jwtAudience = WebConfigurationManager.AppSettings["JWT.Audience"];
            var jwtIssuer = WebConfigurationManager.AppSettings["JWT.AuthenticationUrl"];

            // Generate Token Header
            SigningCredentials signingCredentials = GenerateSigningCredentials();
            var header = new JwtHeader(signingCredentials);

            // Generate Payload
            var now = DateTime.UtcNow;
            List<Claim> claims = new List<Claim>()
            {
                new Claim(FusoClaims.UserId, userId),
                new Claim(FusoClaims.BotChannelId, channelId),
                new Claim(FusoClaims.BotConversationId, conversationId),
                //new Claim(FusoClaims.ServiceUri, serviceUri),
                //new Claim(FusoClaims.ActivityId, activityId),
                //new Claim(FusoClaims.FromId, fromId),
                //new Claim(FusoClaims.FromName, fromName),
                //new Claim(FusoClaims.ToId, toId),
                //new Claim(FusoClaims.ToName, toName),
                new Claim(FusoClaims.LoginType, loginType),
                new Claim(FusoClaims.SessionId, sessionId),
                new Claim(FusoClaims.LanguageCode, languageCode),
                new Claim(FusoClaims.MagicNumber, Convert.ToString(magicNumber))
            };

            var payload = new JwtPayload(jwtIssuer, jwtAudience, claims, now, now.AddMinutes(expireMinutes));

            JwtSecurityToken token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates the signing credentials.
        /// </summary>
        /// <returns></returns>
        private SigningCredentials GenerateSigningCredentials()
        {
            var hmac = new HMACSHA256(Convert.FromBase64String(_secret));
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(hmac.Key),
                                         SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest); // need to check  - Mayank
            return signingCredentials;
        }

        /// <summary>
        /// Generates the security key.
        /// </summary>
        /// <returns></returns>
        public SecurityKey GenerateSecurityKey()
        {
            return GenerateSigningCredentials().Key as SymmetricSecurityKey;
        }
    }
}