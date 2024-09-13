using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Globalization;
using System;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.SupplyManagement.Models.Complaints
{
    public class ComplaintRequest
    {
        public static ComplaintRequest From(CustomerEnquiry enquiry)
        {
            var request = new ComplaintRequest
            {
                RequestDate = DateTime.Parse(enquiry.RequestDate).ToString("dd MMM yyyy", SitecoreX.Culture),
                CompletedDate = enquiry.CompletedDate,
                Reference = enquiry.Reference,
                Status = enquiry.Status,
                RequestType = enquiry.RequestType,
                StatusDate = !string.IsNullOrWhiteSpace(enquiry.StatusDate) && enquiry.StatusDate != "0000-00-00" ? DateTime.Parse(enquiry.StatusDate).ToString("dd MMM yyyy", SitecoreX.Culture) : "",
                StatusTime = enquiry.StatusTime,
                StatusCode = enquiry.StatusCode,
                StatusDescription = enquiry.StatusDescription,
                NocEdit = enquiry.NocEdit,
                CodeGroup = enquiry.CodeGroup,
            };

            return request;
        }

        public string RequestDate { get; set; }
        public string CompletedDate { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string RequestType { get; set; }
        public string StatusDate { get; set; }
        public string StatusTime { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string NocEdit { get; set; }
        public string CodeGroup { get; set; }
        public string AccountNo { get; set; }

        public string State
        {
            get { return string.IsNullOrEmpty(CompletedDate) ? Translate.Text("In progress") : Translate.Text("Closed"); }
        }

        public string StatusClass
        {
            get
            {
                return string.IsNullOrEmpty(CompletedDate) ? Translate.Text("In progress") : Translate.Text("Closed");
            }
        }
    }
}