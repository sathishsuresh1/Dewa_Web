using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Web.Configuration;

namespace DEWAXP.Foundation.Content.Models.RammasLogin
{
    /// <summary>
    /// The Token Validator
    /// </summary>
    public static class TokenValidator
    {
        /// <summary>
        /// Returns true if token is valid.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="securedToken">The secured token.</param>
        /// <returns>
        ///   <c>true</c> if the specified access token is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(this JwtSecurityTokenHandler handler, string accessToken, out SecurityToken securedToken)
        {
            var jwtAudience = WebConfigurationManager.AppSettings["JWT.Audience"];
            var jwtIssuer = WebConfigurationManager.AppSettings["JWT.AuthenticationUrl"];
            var jwtManager = new JwtManager();

            var receivedJwt = new JwtSecurityToken(accessToken);

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = jwtAudience, // should have same audience as the creator (something only 2 parties know)
                ValidIssuer = jwtIssuer, // should have same issuer where the bot redirected
                ValidateLifetime = true, // check time expiry 
                IssuerSigningKey = jwtManager.GenerateSecurityKey() // check who has signed it
            };
            try
            {
                handler.ValidateToken(accessToken, validationParameters, out securedToken);
            }
            catch (Exception ex)
            {
                //IDX10223: Lifetime validation failed. The token is expired.
                if (ex.Message.Contains("IDX10223"))
                    securedToken = null;
                else
                    throw;
            }
            return securedToken != null;
        }
    }
}