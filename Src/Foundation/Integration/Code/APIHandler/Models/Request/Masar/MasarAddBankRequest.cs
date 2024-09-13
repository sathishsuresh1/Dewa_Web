using System.Collections.Generic;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    /// <summary>
    /// 
    /// </summary>
    public class MasarAddBankRequest
    {
        public MasarAddBankRequest() { addbankinputs = new Addbankinputs(); }
        public Addbankinputs addbankinputs { get; set; }
    }
    public class Addbankinputs
    {
        public Addbankinputs()
        {
            attachmentlist = new List<Attachmentlist>();
        }
        public string userid { get; set; }
        public string sessionid { get; set; }
        public string lang { get; set; }
        public string bankname { get; set; }
        public string region { get; set; }
        public string accountnumber { get; set; }
        public string accountholder { get; set; }
        public string currency { get; set; }
        public string swift { get; set; }
        public string route { get; set; }
        public string correspondence { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string correspondentbankregion { get; set; }
        public string correspondentbankname { get; set; }
        public List<Attachmentlist> attachmentlist { get; set; }
        public string correspondentswift { get; set; }
        public string iban { get; set; }
        public string correspondentbankaddress { get; set; }


    }
}
