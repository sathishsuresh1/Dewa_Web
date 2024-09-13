using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Requests.QmsSvc
{
    [XmlRoot(ElementName = "WsIssueTicketReq")]
    public class IssueTicketReq
    {
        [XmlElement(ElementName = "WsAuth")]
        public WsAuth WsAuth { get; set; }
        [XmlElement(ElementName = "BranchCode")]
        public string BranchCode { get; set; }
        [XmlElement(ElementName = "IndService")]
        public IndService IndService { get; set; }
        [XmlElement(ElementName = "Cust")]
        public Cust Cust { get; set; }
    }
   
    public class WsAuth
    {
        [XmlElement(ElementName = "WsClientName")]
        public string WsClientName { get; set; }
        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }
    }
    public class IndService
    {
        [XmlElement(ElementName = "ServiceNo")]
        public string ServiceNo { get; set; }
    }
    public class Cust
    {
        [XmlElement(ElementName = "IndCust")]
        public IndCust IndCust { get; set; }
    }
    public class IndCust
    {
        [XmlElement(ElementName = "SyncId")]
        public string SyncId { get; set; }
    }
}
