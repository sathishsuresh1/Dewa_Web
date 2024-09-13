using System.ServiceModel.Security;

namespace DEWAXP.Foundation.Integration.Helpers
{
	internal class DubaiModelServiceSecurityTokenManager : DewaApiSecurityTokenManager
	{
		public DubaiModelServiceSecurityTokenManager(DubaiModelServiceCredentials cred) 
			: base(cred)
		{
		}

		public override System.IdentityModel.Selectors.SecurityTokenSerializer CreateSecurityTokenSerializer(System.IdentityModel.Selectors.SecurityTokenVersion version)
		{
			return new DubaiModelServiceTokenSerializer(SecurityVersion.WSSecurity11);
		}
	}
}