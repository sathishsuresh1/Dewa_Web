using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.QmsSvc
{
    [XmlRoot(ElementName = "WsGetServiceStatusListResp")]
    public class ServiceStatusListResponse
    {
        [XmlElement(ElementName = "ServiceStatusList")]
        public ServiceStatusList ServiceStatusList { get; set; }
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "Error")]
        public Error Error { get; set; }
    }
    [XmlRoot(ElementName = "ServiceStatusList")]
    public class ServiceStatusList
    {
        [XmlElement(ElementName = "ServiceStatus")]
        public List<ServiceStatus> ServiceStatus { get; set; }
    }
    [XmlRoot(ElementName = "ServiceStatus")]
    public class ServiceStatus
    {
        [XmlElement(ElementName = "Service")]
        public Service Service { get; set; }
        [XmlElement(ElementName = "CounterOpened")]
        public string CounterOpened { get; set; }
        [XmlElement(ElementName = "TotalTicketWaiting")]
        public string TotalTicketWaiting { get; set; }
        [XmlElement(ElementName = "TotalTicketIssued")]
        public string TotalTicketIssued { get; set; }
        [XmlElement(ElementName = "TotalTicketCalled")]
        public string TotalTicketCalled { get; set; }
        [XmlElement(ElementName = "TransferTicketIssued")]
        public string TransferTicketIssued { get; set; }
        [XmlElement(ElementName = "CurrTicketStr")]
        public string CurrTicketStr { get; set; }
        [XmlElement(ElementName = "NextTicketStr")]
        public string NextTicketStr { get; set; }
        [XmlElement(ElementName = "CurrTicket")]
        public string CurrTicket { get; set; }
        [XmlElement(ElementName = "NextTicket")]
        public string NextTicket { get; set; }
        [XmlElement(ElementName = "AvgWtSec")]
        public string AvgWtSec { get; set; }
        [XmlElement(ElementName = "AvgStSec")]
        public string AvgStSec { get; set; }
        [XmlElement(ElementName = "TgtStSec")]
        public string TgtStSec { get; set; }
        [XmlElement(ElementName = "TgtWtSec")]
        public string TgtWtSec { get; set; }
        [XmlElement(ElementName = "TotalWithinTgtWtPer")]
        public string TotalWithinTgtWtPer { get; set; }
        [XmlElement(ElementName = "TotalWithinTgtStPer")]
        public string TotalWithinTgtStPer { get; set; }
        [XmlElement(ElementName = "TotalWithinTgtTtPer")]
        public string TotalWithinTgtTtPer { get; set; }
        [XmlElement(ElementName = "TotalTicketRenegaded")]
        public string TotalTicketRenegaded { get; set; }
        [XmlElement(ElementName = "EstimatedWaitingTimeSec")]
        public string EstimatedWaitingTimeSec { get; set; }
    }
    [XmlRoot(ElementName = "Service")]
    public class Service
    {

        [XmlElement(ElementName = "ShortName")]
        public string ShortName { get; set; }

        [XmlElement(ElementName = "IndService")]
        public IndService IndService { get; set; }

        [XmlElement(ElementName = "Limit")]
        public int Limit { get; set; }
    }
}
