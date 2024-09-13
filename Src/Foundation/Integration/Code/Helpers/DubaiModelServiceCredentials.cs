using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.ServiceModel.Description;

namespace DEWAXP.Foundation.Integration.Helpers
{
	public class DubaiModelServiceCredentials : DewaApiCredentials
	{
		public DubaiModelServiceCredentials()
		{ }

		protected DubaiModelServiceCredentials(DubaiModelServiceCredentials cc)
			: base(cc)
		{
		}

		public override SecurityTokenManager CreateSecurityTokenManager()
		{
			return new DubaiModelServiceSecurityTokenManager(this);
		}

		protected override ClientCredentials CloneCore()
		{
			return new DubaiModelServiceCredentials(this);
		}
	}
}