using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Requests.QmsSvc
{
    [XmlRoot(ElementName = "WsGetBranchListReq")]
    public class BranchServiceStatusReq
    {
        [XmlElement(ElementName = "WsAuth")]
        public WsAuth WsAuth { get; set; }
    }
}
