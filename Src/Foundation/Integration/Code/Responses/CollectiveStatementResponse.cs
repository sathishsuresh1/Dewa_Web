using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [Serializable]
    [XmlRoot(ElementName = "GetCollectiveAccountList")]
    public class CollectiveStatementResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "CollectiveList")]
        public List<CollectiveAccountListResponse> AccountsList { get; set; }
    }
}
