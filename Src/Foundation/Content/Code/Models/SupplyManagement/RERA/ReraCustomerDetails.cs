using System;

namespace DEWAXP.Foundation.Content.Models.RERA
{
    [Serializable]
    public class ReraCustomerDetails
    {
        public string UserId { get; set; }

        public string CustomerName { get; set; }

        public string ContractAccountNumber { get; set; }

        public string BusinessPartnerNumber { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }
    }
}