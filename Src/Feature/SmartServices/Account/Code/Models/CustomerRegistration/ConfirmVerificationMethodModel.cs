using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Feature.Account.Models.CustomerRegistration
{
	public class ConfirmVerificationMethodModel
	{
		public string Email { get; set; }

		public string Mobile { get; set; }
		
		public VerificationMethods SelectedMethod { get; set; }

		public VerificationMethods AvailableMethods
		{
			get
			{
				var availableMethods = VerificationMethods.None;
				if (!string.IsNullOrWhiteSpace(Email))
				{
					availableMethods |= VerificationMethods.Email;
				}
				if (!string.IsNullOrWhiteSpace(Mobile))
				{
					availableMethods |= VerificationMethods.Mobile;
				}
				return availableMethods;
			}
		}
	}
}