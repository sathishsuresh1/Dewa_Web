using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class CollectiveAccountListResponse
    {
        [XmlElement(ElementName = "CollectiveAccountNumber")]
        public string AccountNumber { get; set; }
    }
}
