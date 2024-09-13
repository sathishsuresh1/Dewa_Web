using DEWAXP.Foundation.Content.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Models.ConnectionEnquiries
{
    public class ConnectionEnquiry : GenericPageWithIntro
    {
        public IEnumerable<SelectListItem> QueryTypes { get; set; }

        public string SelectedQueryType { get; set; }

        public string MobileNumber { get; set; }

        public string Details { get; set; }

        public byte[] Attachment { get; set; }

        public string PremiseNumber { get; set; }

        public string BusinessPartnerNo { get; set; }

        public string ContractAccountNo { get; set; }

        public string UserId { get; set; }
    }
}