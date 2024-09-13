using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DEWAXP.Feature.Bills.Models.Premise
{
    
    public class Attachments
    {
        public string Filename1 { get; set; }

        public byte[] Filedata1 { get; set; }

        public string Filename2 { get; set; }

        public byte[] Filedata2 { get; set; }

        public string Filename3 { get; set; }

        public byte[] Filedata3 { get; set; }

        public string Filename4 { get; set; }

        public byte[] Filedata4 { get; set; }
    }

    public class Ownerlist
    {
        public string Type { get; set; }

        public string Nationality { get; set; }

        public string TradeLicense { get; set; }

        public string Issuingauthority { get; set; }

        public string PassportNumber { get; set; }

        public string EmiratesIdNumber { get; set; }

        public HttpPostedFileBase PassportFiledata { get; set; }

        public HttpPostedFileBase TradeFiledata { get; set; }

        public HttpPostedFileBase EmiratesIdDataFile { get; set; }
    }

    public class Premiselist
    {
        public string Premisenumber { get; set; }
    }

    public class JointOwnerModel
    {
        public JointOwnerModel()
        {
            Ownerlist = new List<Ownerlist>();
            Premiselist = new List<Premiselist>();
        }
        public string Pobox { get; set; }

        public string Region { get; set; }
        
        public string Sessionid { get; set; }

        public string Contractaccount { get; set; }

        public string Email { get; set; }

        public string Mobilenumber { get; set; }

        public Attachments Attachments { get; set; }

        public bool IsLoginUser { get; set; }

        public List<Ownerlist> Ownerlist { get; set; }

        public List<Premiselist> Premiselist { get; set; }

        public int TypeOfAccount { get; set; }
        public string AccountList { get; set; }
        public HttpPostedFileBase ContractAccountfile { get; set; }

        public string PropertyType { get; set; }

        public HttpPostedFileBase PurchaseAgreementFile { get; set; }
    }

    public class SuccessModel
    {
        public string ReferenceNumber { get; set; }

        public string Name { get; set; }

        public string Date { get; set; }
    }
}
