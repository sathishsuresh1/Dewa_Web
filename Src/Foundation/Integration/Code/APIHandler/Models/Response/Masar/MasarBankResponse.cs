using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar
{
    public class MasarBankResponse
    {
        public MasarBankResponse() { bankmasterdetails = new List<Bankmasterdetail>(); }
        public List<Bankattachment> bankattachment { get; set; }
        public List<Bankheaderdetail> bankheaderdetails { get; set; }
        public List<Bankmasterdetail> bankmasterdetails { get; set; }
        public string description { get; set; }
        public string responsecode { get; set; }
    }

    public class Bankattachment
    {
        public string attachtype { get; set; }
        public byte[] content { get; set; }
        public string contenttype { get; set; }
        public string description { get; set; }
        public string filename { get; set; }
        public string filesize { get; set; }
        public object identificationnumber { get; set; }
        public object lastdocumentflag { get; set; }
        public string mimetype { get; set; }
        public string processtype { get; set; }
        public string referencenumber { get; set; }
        public object status { get; set; }
        public string suppliernumber { get; set; }
    }

    public class Bankheaderdetail
    {
        public string bankkey { get; set; }
        public string bankaccount { get; set; }
        public object bankaddress { get; set; }
        public string country { get; set; }
        public string bankname { get; set; }
        public object city { get; set; }
        public object comments { get; set; }
        public string currencykey { get; set; }
        public string currencykeydescription { get; set; }
        public string financialinstitution { get; set; }
        public object financialinstitutionaddress { get; set; }
        public object financialswift { get; set; }
        public string generalflag { get; set; }
        public string holdername { get; set; }
        public string iban { get; set; }
        public string referencenumber { get; set; }
        public string regionbank { get; set; }
        public string regionbankdescription { get; set; }
        public object route { get; set; }
        public string status { get; set; }
        public string statusdescription { get; set; }
        public string swift { get; set; }
    }

    public class Bankmasterdetail
    {
        public string bankaccount { get; set; }
        public string bankkey { get; set; }
        public string bankname { get; set; }
        public string country { get; set; }
        public string holdername { get; set; }
        public string iban { get; set; }
        public string swift { get; set; }
    }


    public class AddBankResponse
    {
       
        public string description { get; set; }       
        public string responsecode { get; set; }
        public string referencenumber { get; set; }
    }

}
