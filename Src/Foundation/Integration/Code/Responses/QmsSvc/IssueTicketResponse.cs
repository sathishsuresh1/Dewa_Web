using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.QmsSvc
{
   
    [XmlRoot(ElementName = "WsIssueTicketResp")]
    public class IssueTicketResponse
    {
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "TicketWaiting")]
        public TicketWaiting TicketWaiting { get; set; }

        [XmlElement(ElementName = "TpFieldExList")]
        public TpFieldExList TpFieldExList { get; set; }

        [XmlElement("WsIssueTicketReq")]
        public WsIssueTicketReq WsIssueTicketReq { get; set; }

        [XmlElement(ElementName = "Error")]
        public Error Error { get; set; }
       
    }
    public class Error
    {
        [XmlElement(ElementName = "ErrorCode")]
        public int ErrorCode { get; set; }

        [XmlElement(ElementName = "Reason")]
        public string Reason { get; set; }
    }
    public class TicketWaiting
    {
        [XmlElement(ElementName = "TicketNo")]
        public string TicketNo { get; set; }

        [XmlElement(ElementName = "IssuedTime")]
        public string IssuedTime { get; set; }

        [XmlElement(ElementName = "TicketStr")]
        public string TicketStr { get; set; }

        [XmlElement(ElementName = "IndService")]
        public IndService IndService { get; set; }
    }

    public class IndService
    {
        [XmlElement(ElementName = "ServiceNo")]
        public string ServiceNo { get; set; }

        [XmlElement(ElementName = "ServiceName")]
        public string ServiceName { get; set; }
    }

    public class TpFieldExList
    {
        [XmlElement(ElementName = "TpFieldEx")]
        public TpFieldEx[] TpFieldEx { get; set; }
    }

    public class TpFieldEx
    {
        [XmlElement(ElementName = "TpFieldExName")]
        public string TpFieldExName { get; set; }

        [XmlElement(ElementName = "TpFieldExValue")]
        public string TpFieldExValue { get; set; }
    }

    public class WsIssueTicketReq
    {
        [XmlElement(ElementName = "WsAuth")]
        public WsAuth WsAuth { get; set; }

        [XmlElement(ElementName = "BranchCode")]
        public string BranchCode { get; set; }

        [XmlElement(ElementName = "IssueType")]

        public string IssueType { get; set; }
    }

    public class WsAuth
    {
        [XmlElement(ElementName = "WsClientName")]
        public string WsClientName { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }
    }
}
