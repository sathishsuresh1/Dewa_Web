using System;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.Dashboard.Models.Enquiries
{
    public class EnquiryModel
    {
        public string Reference { get; set; }

        public string BusinessPartnerNumber { get; set; }

        public string AccountNumber { get; set; }

        public string CodeGroup { get; set; }

        public string Status { get; set; }

        public string RequestType { get; set; }

        public string RequestDate { get; set; }

        public bool Completed { get; set; }

        public static EnquiryModel From(DEWAXP.Foundation.Integration.Responses.CustomerEnquiry response)
        {
            return new EnquiryModel()
            {
                Reference = response.Reference,
                BusinessPartnerNumber = response.BusinessPartnerNumber,
                AccountNumber = response.AccountNumber,
                Status = response.Status,
                CodeGroup = response.CodeGroup,
                RequestDate = DateTime.Parse(response.RequestDate).ToString("dd MMM yyyy", SitecoreX.Culture),
                RequestType = response.RequestType,
                Completed = response.Completed
            };
        }
    }
}