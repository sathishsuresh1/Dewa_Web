using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response
{
    public class SetNewEVGreenCardV3Response
    {
        public List<ContractAccount> accountList { get; set; }
        public string bpNumber { get; set; }
        public string description { get; set; }
        public string emailId { get; set; }
        public string firstName { get; set; }
        public string idNumber { get; set; }
        public string idType { get; set; }
        public string lastName { get; set; }
        public string mobileNumber { get; set; }
        public string nationality { get; set; }
        public string poBox { get; set; }
        public string region { get; set; }
        public string requestNumber { get; set; }
        public string responseCode { get; set; }
        public string userId { get; set; }
    }

    public class ContractAccount
    {
        public string accountNumber { get; set; }
        public string amount1 { get; set; }
        public string amount2 { get; set; }
        public string courierCharge { get; set; }
        public string courierVatAmount { get; set; }
        public string evCardNumber { get; set; }
        public string plateNumber { get; set; }
        public string sdAmount { get; set; }
        public string totalAmount { get; set; }
    }
}
