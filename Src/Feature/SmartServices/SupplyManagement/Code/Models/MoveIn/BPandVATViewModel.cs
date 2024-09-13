using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveIn
{
    public class BPandVATViewModel
    {
      public BusinessPartner bp { get; set; }

      public businessPartnerVat vatdetails { get; set; }
    }
}
