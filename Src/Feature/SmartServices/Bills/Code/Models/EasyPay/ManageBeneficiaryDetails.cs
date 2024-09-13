using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Bills.Models.EasyPay
{
    [Serializable]
    public class ManageBeneficiaryDetails
    {
        public string ContractAccount { get; set; }
        public string Name { get; set; }
        public List<BeneficiaryDetail> BeneficiaryDetailLists { get; set; }
    }
}