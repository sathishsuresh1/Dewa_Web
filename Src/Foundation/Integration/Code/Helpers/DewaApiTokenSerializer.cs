using System;
using System.Security.Cryptography;
using System.ServiceModel.Security;
using System.Text;

namespace DEWAXP.Foundation.Integration.Helpers
{
	internal class DewaApiTokenSerializer : WSSecurityTokenSerializer
	{
		public DewaApiTokenSerializer(SecurityVersion sv)
			: base(sv)
		{

		}

		protected override void WriteTokenCore(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityToken token)
		{
			var userToken = token as System.IdentityModel.Tokens.UserNameSecurityToken;
			if (userToken == null) return;

			var created = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
			var phrase = Guid.NewGuid().ToString();
			var nonce = Hash(phrase);
			var password = userToken.Password;

			var headerFormat = "<{0}:UsernameToken u:Id=\"{1}\" xmlns=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">" +
			                   "<{0}:Username>{2}</{0}:Username>" +
			                   "<{0}:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">{3}</{0}:Password>" +
			                   "<{0}:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">{4}</{0}:Nonce>" +
			                   "<u:Created>{5}</u:Created>" +
			                   "</{0}:UsernameToken>";

			writer.WriteRaw(string.Format(headerFormat, "o", userToken.Id, userToken.UserName, password, nonce, created));
		}

		protected string Hash(string phrase)
		{
			using (var sha = new SHA1CryptoServiceProvider())
			{
				var hashedDataBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(phrase));
				return Convert.ToBase64String(hashedDataBytes);
			}
		}
	}
}