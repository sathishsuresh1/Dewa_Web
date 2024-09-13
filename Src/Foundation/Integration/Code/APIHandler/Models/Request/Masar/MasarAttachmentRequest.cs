using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarAttachmentRequest
    {
        public Attachmentinputs attachmentinputs { get; set; }
    }

    public class Attachmentlist
    {
        public string attachmenttype { get; set; }
        public string filename { get; set; }
        public string filedata { get; set; }
        public string mimetype { get; set; }
    }

    public class Attachmentinputs
    {
        public string appno { get; set; }
        public string lastdoc { get; set; }
        public string userid { get; set; }
        public string sessionid { get; set; }
        public string requesttype { get; set; }
        public string reference { get; set; }
        public List<Attachmentlist> attachmentlist { get; set; }
    }

}
