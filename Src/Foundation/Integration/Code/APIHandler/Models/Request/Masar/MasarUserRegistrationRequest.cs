using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarUserRegistrationRequest
    {
        public RegistrationInfo customerinputs { get; set; }
    }


    public class RegistrationInfo
    {
        public string requesttype { get; set; }
        public string applicationnumber { get; set; }
        public string customertype { get; set; }
        public string customercategory { get; set; }
        public string title { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string companyname { get; set; }
        public string street { get; set; }
        public string citycode { get; set; }
        public string pobox { get; set; }
        public string region { get; set; }
        public string tradelicense { get; set; }
        public string tradelicensevalidfrom { get; set; }
        public string tradelicensevalidto { get; set; }
        public string issueauthority { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string telephone { get; set; }
        public string extension { get; set; }
        public string emiratesid { get; set; }
        public string country { get; set; }
        public string lang { get; set; }
        public string userid { get; set; }
        public string sessionid { get; set; }
        public string noattachflag { get; set; }
        public string issueauthoritytext { get; set; }
        public string eidexpirydate { get; set; }
        public string testflag { get; set; }
        public string telephonedialcode { get; set; }
        public string mobiledialcode { get; set; }
        public string vatregistrationnumber { get; set; }
    }

}
