using System;
using System.Security.Cryptography;
using System.ServiceModel.Security;
using System.Text;

namespace DEWAXP.Foundation.Integration.Helpers
{
	internal class DubaiModelServiceTokenSerializer : DewaApiTokenSerializer
	{
		public DubaiModelServiceTokenSerializer(SecurityVersion sv) 
			: base(sv)
		{

		}

		protected override void WriteTokenCore(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityToken token)
		{
			var userToken = token as System.IdentityModel.Tokens.UserNameSecurityToken;
			if (userToken == null) return;

			// 2016-02-04T11:23:12Z
			var created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
			var nonce = "9e42ad828f93e28e";
			var password = CreateDigestPassword(nonce, created, userToken.Password);
			
			var headerFormat = "<{0}:UsernameToken u:Id=\"{1}\" xmlns=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">" +
			                   "<{0}:Username>{2}</{0}:Username>" +
			                   "<{0}:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">{3}</{0}:Password>" +
			                   "<{0}:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">{4}</{0}:Nonce>" +
			                   "<u:Created>{5}</u:Created>" +
			                   "</{0}:UsernameToken>";

			writer.WriteRaw(string.Format(headerFormat, "o", userToken.Id, userToken.UserName, password, nonce, created));
		}

		protected new string Hash(string phrase)
		{
			using (var sha = new SHA1CryptoServiceProvider())
			{
				var hashedDataBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(phrase));

				return Convert.ToBase64String(hashedDataBytes);
			}
		}

		private string CreateDigestPassword(string nonce, string timestamp, string password)
		{
			var mash = string.Concat(nonce, timestamp, password);
			using (var sha1 = new SHA1CryptoServiceProvider())
			{
				var input = Encoding.UTF8.GetBytes(mash);
				var hash = sha1.ComputeHash(input);

				return Convert.ToBase64String(hash);
			}
		}

		//private string CreatePasswordDigest(byte[] nonce, string createdTime, string password)
		//{
		//	//// combine three byte arrays into one
		//	//byte[] time = Encoding.UTF8.GetBytes(createdTime);
		//	//byte[] pwd = Encoding.UTF8.GetBytes(password);
		//	//byte[] operand = new byte[nonce.Length + time.Length + pwd.Length];

		//	//Array.Copy(nonce, operand, nonce.Length);
		//	//Array.Copy(time, 0, operand, nonce.Length, time.Length);
		//	//Array.Copy(pwd, 0, operand, nonce.Length + time.Length, pwd.Length);

		//	//// create the hash
		//	//using (var sha1 = new SHA1CryptoServiceProvider())
		//	//{
		//	//	byte[] hashedDataBytes = sha1.ComputeHash(operand);
		//	//	return Convert.ToBase64String(hashedDataBytes);
		//	//}
		//}

	}
}