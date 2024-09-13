using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Requests.QmsSvc
{
    [XmlRoot(ElementName = "WsGetServiceStatusListReq")]
    public class ServiceStatusListReq
    {
        [XmlElement(ElementName = "WsAuth")]
        public WsAuth WsAuth { get; set; }
        [XmlElement(ElementName = "BranchCode")]
        public string BranchCode { get; set; }
    }
}
