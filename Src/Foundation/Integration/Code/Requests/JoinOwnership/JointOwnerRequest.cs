using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.JoinOwnership
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Attachments
    {
        [JsonProperty("filename1")]
        public string filename1 { get; set; }

        [JsonProperty("filedata1")]
        public string filedata1 { get; set; }

        [JsonProperty("filename2")]
        public string filename2 { get; set; }

        [JsonProperty("filedata2")]
        public string filedata2 { get; set; }

        [JsonProperty("filename3")]
        public string filename3 { get; set; }

        [JsonProperty("filedata3")]
        public string filedata3 { get; set; }

        [JsonProperty("filename4")]
        public string filename4 { get; set; }

        [JsonProperty("filedata4")]
        public string filedata4 { get; set; }
    }

    public class Jointownerinputs
    {
        public Jointownerinputs()
        {
            ownerlist = new List<Ownerlist>();
            premiselist = new List<Premiselist>();
            attachments = new Attachments();
        }
        [JsonProperty("pobox")]
        public string pobox { get; set; }

        [JsonProperty("region")]
        public string region { get; set; }

        [JsonProperty("sessionid")]
        public string sessionid { get; set; }

        [JsonProperty("contractaccount")]
        public string contractaccount { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("mobilenumber")]
        public string mobilenumber { get; set; }

        [JsonProperty("lang")]
        public string lang { get; set; }

        [JsonProperty("attachments")]
        public Attachments attachments { get; set; }

        [JsonProperty("ownerlist")]
        public List<Ownerlist> ownerlist { get; set; }

        [JsonProperty("premiselist")]
        public List<Premiselist> premiselist { get; set; }
    }

    public class Ownerlist
    {
        [JsonProperty("idtye")]
        public string idtye { get; set; }

        [JsonProperty("idno")]
        public string idno { get; set; }

        [JsonProperty("nationality")]
        public string nationality { get; set; }

        [JsonProperty("issuingauthority")]
        public string issuingauthority { get; set; }

        [JsonProperty("filename")]
        public string filename { get; set; }

        [JsonProperty("filedata")]
        public string filedata { get; set; }
    }

    public class Premiselist
    {
        [JsonProperty("premisenumber")]
        public string premisenumber { get; set; }
    }

    public class JointOwnerRequest
    {
        public JointOwnerRequest()
        {
            jointownerinputs = new Jointownerinputs();
        }
        [JsonProperty("jointownerinputs")]
        public Jointownerinputs jointownerinputs { get; set; }
    }
}
