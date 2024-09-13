using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Enums;
using System;

namespace DEWAXP.Foundation.Content.Models.ContactDetails
{
	public class ContactDetails
	{
		public string MoveOutDate { get; set; }

		public double Amount { get; set; }

		public string AccountName { get; set; }

		public string AccountNumber { get; set; }

		public string BusinessPartnerNumber { get; set; }
		public string TradeLicenseNumber { get; set; }
		public string PremiseNumber { get; set; }

		public BillingClassification BillingClass { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }

		public string Mobile { get; set; }

		public string PoBox { get; set; }
		public string City { get; set; }
		public string Emirate { get; set; }

		public string Telephone { get; set; }

		public string Fax { get; set; }

		public string NickName { get; set; }

		public SupportedLanguage PreferredLang { get; set; }

		public bool IsAccountActive { get; set; }
		public decimal Balance { get; set; }

		public static ContactDetails From(DEWAXP.Foundation.Integration.Responses.AccountContactDetails contactDetails)
		{
			return new ContactDetails
			{
				AccountNumber = contactDetails.Account,
				Email = contactDetails.Email,
				Mobile = contactDetails.Mobile
			};
		}

		public static ContactDetails From(DEWAXP.Foundation.Integration.Responses.AccountContactDetails contactDetails,
			DEWAXP.Foundation.Integration.Responses.AccountDetails accountDetails)
		{
			return new ContactDetails
			{
				AccountName = accountDetails.AccountName,
				AccountNumber = contactDetails.Account,
				BusinessPartnerNumber = accountDetails.BusinessPartnerNumber,
				PremiseNumber = accountDetails.CustomerPremiseNumber,
				BillingClass = accountDetails.BillingClass,
				Email = contactDetails.Email,
				Mobile = contactDetails.Mobile.RemoveMobileNumberZeroPrefix(),
				PoBox = contactDetails.PoBox,
				Emirate = contactDetails.Emirate,
				Telephone = contactDetails.Telephone.RemoveMobileNumberZeroPrefix(),
				Fax = contactDetails.Fax.RemoveMobileNumberZeroPrefix(),
				NickName = contactDetails.NickName,
				PreferredLang = "EN".Equals(contactDetails.PreferredLang) ? SupportedLanguage.English : SupportedLanguage.Arabic,
				IsAccountActive = accountDetails.IsActive,
				Balance = accountDetails.Balance
			};
		}

		public static ContactDetails From(DEWAXP.Foundation.Integration.Responses.ClearanceCertificateDetails clearanceCertificateDetails, DEWAXP.Foundation.Integration.Responses.AccountDetails accountDetails)
		{
			return new ContactDetails
			{
				FirstName = clearanceCertificateDetails.FirstName,
				LastName = clearanceCertificateDetails.LastName,
				Email = clearanceCertificateDetails.Email,
				Mobile = clearanceCertificateDetails.Mobile,
				PoBox = clearanceCertificateDetails.PoBox,
				AccountNumber = accountDetails.AccountNumber,
				TradeLicenseNumber = clearanceCertificateDetails.TradeLicenseNumber,
				City = clearanceCertificateDetails.Region,
                Balance = clearanceCertificateDetails.Amount,
				PremiseNumber = accountDetails.CustomerPremiseNumber,
				AccountName = accountDetails.NickName ?? accountDetails.AccountName,
				IsAccountActive = accountDetails.IsActive,
				BillingClass = accountDetails.BillingClass,
				BusinessPartnerNumber = accountDetails.BusinessPartnerNumber
			};
		}

		public static ContactDetails From(DEWAXP.Foundation.Integration.DewaSvc.GetContractAccountClearanceDetailsResponse cert, DEWAXP.Foundation.Integration.Responses.AccountDetails accountDetails)
		{
			return new ContactDetails
			{
				FirstName = cert.@return.firstName ?? string.Empty,
				LastName = cert.@return.lastName ?? string.Empty,
				Email = cert.@return.email,
				Mobile = cert.@return.mobile.RemoveMobileNumberZeroPrefix(),
				AccountNumber = accountDetails.AccountNumber,
				MoveOutDate = cert.@return.moveOutDate,
				Amount = cert.@return.amount

			};
		}
	}
}