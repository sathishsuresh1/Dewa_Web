using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{
    [Serializable]
    [XmlRoot(ElementName = "GetEstimateHist")]
    public class EstimateHistoryResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "DateTimeStamp")]
        public string DateTimeStamp { get; set; }

        [XmlElement("Iteam")]
        public List<EstimateHistory> Estimates { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "Iteam")]
    public class EstimateHistory
    {
        [Serializable]
        [XmlRoot(ElementName = "Account")]
        public class Account
        {
            [XmlElement(ElementName = "ESTIMATIONNO")]
            public string EstimationNo { get; set; }

            [XmlElement(ElementName = "DEWA_TRANSID")]
            public string DewaTransId { get; set; }

            [XmlElement(ElementName = "DATEANDTIME")]
            public string Date { get; set; }

            [XmlElement(ElementName = "ReceiptID")]
            public string ReceiptID { get; set; }

            [XmlElement(ElementName = "CONTRACT_ACCOUNT")]
            public string ContractAccount { get; set; }

            [XmlElement(ElementName = "APPNO")]
            public string AppNo { get; set; }

            [XmlElement(ElementName = "VENDORID")]
            public string VendorID { get; set; }

            [XmlElement(ElementName = "CREDENTIAL")]
            public string Credential { get; set; }

            [XmlElement(ElementName = "TRANS_TYPE")]
            public string TransType { get; set; }

            [XmlElement(ElementName = "TRAN_AMMOUNT")]
            public string TranAmmount { get; set; }

            [XmlElement(ElementName = "STATUS")]
            public string Status { get; set; }

            [XmlElement(ElementName = "PAYMENT_DIV")]
            public string PaymentDiv { get; set; }

            [XmlElement(ElementName = "LOT_NO")]
            public string LotNo { get; set; }

            [XmlElement(ElementName = "DEG_TRANSID")]
            public string DegTransID { get; set; }

            [XmlElement(ElementName = "CREATE_DATE")]
            public DateTime CreateDate { get; set; }

            [XmlElement(ElementName = "SPCODE")]
            public string SpCode { get; set; }

            [XmlElement(ElementName = "SERVCODE")]
            public string ServerCode { get; set; }

            [XmlElement(ElementName = "PMTCHNL")]
            public string PaymentChannel { get; set; }

            [XmlElement(ElementName = "PMTMODE")]
            public string PaymentMode { get; set; }

            [XmlElement(ElementName = "PMTMETHOD")]
            public string PaymentMethod { get; set; }

            [XmlElement(ElementName = "TRNAMOUNT")]
            public string TrnAmmount { get; set; }

            [XmlElement(ElementName = "PMTGATENAME")]
            public string PmtGateName { get; set; }

            [XmlElement(ElementName = "PMTGATETXNNO")]
            public string PmtGateTxnno { get; set; }

            [XmlElement(ElementName = "CARDTYPE")]
            public string CardType { get; set; }

            [XmlElement(ElementName = "DEGAMOUNT")]
            public decimal DegAmmout { get; set; }

            [XmlElement(ElementName = "PMTGATERECSTAT")]
            public string PmtGateRecStat { get; set; }

            [XmlElement(ElementName = "DEWAAMOUNT")]
            public string DewaAmmount { get; set; }

            [XmlElement(ElementName = "REFUNDID")]
            public string RefundID { get; set; }

            [XmlElement(ElementName = "REFUNDSTAT")]
            public string RefundStat { get; set; }

            [XmlElement(ElementName = "DISPUTEID")]
            public string DisputeID { get; set; }

            [XmlElement(ElementName = "MESSAGECODE")]
            public string MessageCode { get; set; }

            [XmlElement(ElementName = "MESSAGE")]
            public string Message { get; set; }

            [XmlElement(ElementName = "ADD_INFO1")]
            public string AddInfo01 { get; set; }

            [XmlElement(ElementName = "ADD_INFO2")]
            public string AddInfo02 { get; set; }

            [XmlElement(ElementName = "ENCRYPTEDRESMSG")]
            public string EncriptedResMsg { get; set; }

            [XmlElement(ElementName = "EXE_DATE")]
            public string ExeDate { get; set; }

            [XmlElement(ElementName = "REQ_MODE")]
            public string ReqMode { get; set; }

            [XmlElement(ElementName = "OLD_STATUS")]
            public string OldStatus { get; set; }

            [XmlElement(ElementName = "BPDETAILS")]
            public string BPDetails { get; set; }

            [XmlElement(ElementName = "OPBEL")]
            public string Opbel { get; set; }

            [XmlElement(ElementName = "STAZS")]
            public string Stazs { get; set; }

            [XmlElement(ElementName = "BILL_MESSAGE")]
            public string BillMessage { get; set; }

            /*
            public DateTime PaymentDate
            {
                get
                {
                    //EXE format : //yyyyMMddhhmmss
                    int second = int.Parse(ExeDate.Substring(12, 2));
                    int minute = int.Parse(ExeDate.Substring(10, 2));
                    int hour = int.Parse(ExeDate.Substring(8, 2));
                    int day = int.Parse(ExeDate.Substring(6, 2));
                    int month = int.Parse(ExeDate.Substring(4, 2));
                    int year = int.Parse(ExeDate.Substring(0, 2));

                    return new DateTime(year, month, day, hour, minute, second);
                }
            }
            */
        }

        [XmlElement(ElementName = "Account")]
        public Account Acc { get; set; }
    }
}
