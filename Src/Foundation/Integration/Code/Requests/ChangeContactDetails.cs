using DEWAXP.Foundation.Integration.Enums;

namespace DEWAXP.Foundation.Integration.Requests
{
    public class ChangeContactDetails
    {
        public string ContractAccountNumber { get; set; }

        public string PoBox { get; set; }

        public string Emirate { get; set; }

        public string TelephoneNumber { get; set; }

        public string MobileNumber { get; set; }

        public string EmailAddress { get; set; }

        public string FaxNumber { get; set; }

        public SupportedLanguage PreferredLanguage { get; set; }

        public string NickName { get; set; }

        public string BusinessPartnerNumber { get; set; }
    }
}
