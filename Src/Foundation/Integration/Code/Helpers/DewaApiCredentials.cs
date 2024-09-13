using System.ServiceModel.Description;

namespace DEWAXP.Foundation.Integration.Helpers
{
	public class DewaApiCredentials : ClientCredentials
	{
		public DewaApiCredentials()
		{ }

		protected DewaApiCredentials(DewaApiCredentials cc)
			: base(cc)
		{

		}

		public override System.IdentityModel.Selectors.SecurityTokenManager CreateSecurityTokenManager()
		{
			return new DewaApiSecurityTokenManager(this);
		}

		protected override ClientCredentials CloneCore()
		{
			return new DewaApiCredentials(this);
		}
	}
}
