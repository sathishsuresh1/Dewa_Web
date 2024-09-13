using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar
{
    public class MasarProfileFetchResponse
    {
        public string description { get; set; }
        public List<Profilelist> profilelist { get; set; }
        public string responsecode { get; set; }
    }

    public class Profilelist
    {
        public string bpcategory { get; set; }
        public string city { get; set; }
        public string companyname { get; set; }
        public string country { get; set; }
        public string email { get; set; }
        public string emiratesid { get; set; }
        public string extension { get; set; }
        public string issuedby { get; set; }
        public string mobile { get; set; }
        public string name1 { get; set; }
        public string name2 { get; set; }
        public string pobox { get; set; }
        public string region { get; set; }
        public string street { get; set; }
        public string telephone { get; set; }
        public string title { get; set; }
        public string tradelicense { get; set; }
        public string tradelicensevalidfrom { get; set; }
        public string tradelicensevalidto { get; set; }
        public string vatregno { get; set; }
        public string issueauthoritytext { get; set; }
        public string emiratevalidto { get; set;}
        public string telephonedialcode { get; set; }
        public string mobiledialcode { get; set; }
        public string vatregistrationnumber { get; set; }
        public string tradenumberflag { get; set; }
        public string issuedateflag { get; set; }
        public string expirydateflag { get; set; }
        public string attachmentflag { get; set; }
        public string issueauthorityflag { get; set; }

    }
}
