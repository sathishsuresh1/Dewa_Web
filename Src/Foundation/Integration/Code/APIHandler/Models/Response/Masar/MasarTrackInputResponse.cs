using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar
{
    public class MasarTrackInputResponse
    {
        public string description { get; set; }
        public string responsecode { get; set; }
        public List<Trackdetailattach> trackdetailattach { get; set; }
        public List<Trackdetailslist> trackdetailslist { get; set; }
    }

    public class Trackdetailattach
    {
        public string attachtype { get; set; }
        public byte[] content { get; set; }
        public object contenttype { get; set; }
        public object description { get; set; }
        public string filename { get; set; }
        public string filesize { get; set; }
        public object identificationnumber { get; set; }
        public object lastdocumentflag { get; set; }
        public string mimetype { get; set; }
        public object processtype { get; set; }
        public string referencenumber { get; set; }
        public object status { get; set; }
        public object suppliernumber { get; set; }
    }

    public class Trackdetailslist
    {
        public string applicationitem { get; set; }
        public string applicationnumber { get; set; }
        public string approvalstatus { get; set; }
        public string businesspartner { get; set; }
        public string businesspartnercategory { get; set; }
        public string city { get; set; }
        public string citycode { get; set; }
        public string country { get; set; }
        public string customeraccountgroup { get; set; }
        public string customernumber { get; set; }
        public string customertype { get; set; }
        public string dateofbirth { get; set; }
        public string emailaddress { get; set; }
        public string emailflag { get; set; }
        public string emiratesid { get; set; }
        public string emiratesidvalidfrom { get; set; }
        public string emiratesidvalidto { get; set; }
        public string extensionflag { get; set; }
        public string firstname { get; set; }
        public string issueauthority { get; set; }
        public string issueauthoritytext { get; set; }
        public string lastname { get; set; }
        public string companyname { get; set; }
        public string mobile { get; set; }
        public string mobiledialcode { get; set; }
        public string passport { get; set; }
        public string postbox { get; set; }
        public string region { get; set; }
        public string remarks { get; set; }
        public string scraptenderregistrationfor { get; set; }
        public string smsflag { get; set; }
        public string street { get; set; }
        public string telephone { get; set; }
        public string telephoneedialcode { get; set; }
        public string telephoneextension { get; set; }
        public string title { get; set; }
        public string tradelicensenumber { get; set; }
        public string tradelicensevalidfrom { get; set; }
        public string tradelicensevalidto { get; set; }
        public string transactiontype { get; set; }
        public string userid { get; set; }
        public string vatregistrationnumber { get; set; }
        public string workflowcomments { get; set; }
        public string workflowid { get; set; }
        public string workflowstatus { get; set; }
    }
}