using System.ServiceModel;
using System.ServiceModel.Security;

namespace DEWAXP.Foundation.Integration.Helpers
{
	internal class DewaApiSecurityTokenManager : ClientCredentialsSecurityTokenManager
	{
		public DewaApiSecurityTokenManager(DewaApiCredentials cred)
			: base(cred)
		{

		}

		public override System.IdentityModel.Selectors.SecurityTokenSerializer CreateSecurityTokenSerializer(System.IdentityModel.Selectors.SecurityTokenVersion version)
		{
			return new DewaApiTokenSerializer(SecurityVersion.WSSecurity11);
		}
	}
}