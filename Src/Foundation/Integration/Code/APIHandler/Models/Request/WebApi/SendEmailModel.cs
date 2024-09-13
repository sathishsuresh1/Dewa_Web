using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.WebApi
{
    internal class SendEmailModel
    {
        public SendEmailModel()
        {
            //to avoid Null Reference Error in case no attachment
            this.Attachments = new List<Attachment>();
        }

        public string From { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }

        public List<Attachment> Attachments { get; set; }
    }
    internal class Attachment
    {
        public string FileName { get; set; }
        public string FileBase64String { get; set; }
    }
}
