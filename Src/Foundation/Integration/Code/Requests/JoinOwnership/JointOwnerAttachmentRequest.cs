using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.JoinOwnership
{
    public class JointOwnerAttachmentRequest
    {
        public JointOwnerAttachmentRequest() {
            saveattachmentinputs = new Saveattachmentinputs();
        }
        [JsonProperty("saveattachmentinputs")]
        public Saveattachmentinputs saveattachmentinputs { get; set; }
    }
    public class Saveattachmentinputs
    {
        [JsonProperty("filename")]
        public string filename { get; set; }

        [JsonProperty("filedata")]
        public string filedata { get; set; }

        [JsonProperty("objectid")]
        public string objectid { get; set; }

        [JsonProperty("objecttype")]
        public string objecttype { get; set; }
    }
}
