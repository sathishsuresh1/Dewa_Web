using DEWAXP.Foundation.Integration.DewaSvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Premise
{
    public class PremiseDetailsResponse
    {
        public PremiseDetailsResponse()
        {
            dubaiMunicipality = new DubaiMunicipality();
            meter = new Meter();
            outageStatus = new OutageStatus();
        }

        [JsonProperty("dubaimunicipality")]
        public DubaiMunicipality dubaiMunicipality { get; set; }

        [JsonProperty("meter")]
        public Meter meter { get; set; }

        [JsonProperty("outagestatus")]
        public OutageStatus outageStatus { get; set; }

        [JsonProperty("podcustomer")]
        public bool podCustomer { get; set; }

        [JsonProperty("responsecode")]
        public string responseCode { get; set; }

        [JsonProperty("responsemessage")]
        public string responseMessage { get; set; }

        [JsonProperty("seniorcitizen")]
        public bool seniorCitizen { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }
    }

    public class DubaiMunicipality
    {
        [JsonProperty("dmmukhalafanumber")]
        public string dmMukhalafaNumber { get; set; }
        [JsonProperty("dmclear")]
        public bool dmclear { get; set; }
        [JsonProperty("dmfine")]
        public bool dmfine { get; set; }
    }

    public class Meter
    {
        [JsonProperty("disconnectionalert")]
        public string disconnectionAlert { get; set; }
        [JsonProperty("disconnectionduedate")]
        public string disconnectionDueDate { get; set; }
        [JsonProperty("disconnectionreason")]
        public string disconnectionReason { get; set; }
        [JsonProperty("disconnectionreasoncode")]
        public string disconnectionReasonCode { get; set; }
        [JsonProperty("electicityactive")]
        public bool electicityActive { get; set; }
        [JsonProperty("electricityequipment")]
        public string electricityEquipment { get; set; }
        [JsonProperty("electricitymeter")]
        public string electricityMeter { get; set; }
        [JsonProperty("electricitysmartmeter")]
        public bool electricitySmartMeter { get; set; }
        [JsonProperty("electricitystatus")]
        public string electricityStatus { get; set; }
        [JsonProperty("electricitymetertype")]
        public string electricitymeterType { get; set; }
        [JsonProperty("electricitymetertypedesc")]
        public string electricitymeterTypeDesc { get; set; }
        [JsonProperty("moveindate")]
        public string moveinDate { get; set; }
        [JsonProperty("moveoutdate")]
        public string moveoutDate { get; set; }
        [JsonProperty("reconnectiondueamount")]
        public string reconnectionDueamount { get; set; }
        [JsonProperty("securitydepositdue")]
        public string securityDepositDue { get; set; }
        [JsonProperty("wateractive")]
        public bool waterActive { get; set; }
        [JsonProperty("waterequipment")]
        public string waterEquipment { get; set; }
        [JsonProperty("watermeter")]
        public string waterMeter { get; set; }
        [JsonProperty("watersmartmeter")]
        public bool waterSmartMeter { get; set; }
        [JsonProperty("waterstatus")]
        public string waterStatus { get; set; }
        [JsonProperty("watermetertype")]
        public string watermeterType { get; set; }
        [JsonProperty("watermetertypedesc")]
        public string watermeterTypeDesc { get; set; }
    }

    public class OutageStatus
    {
        [JsonProperty("electricityapplication")]
        public string electricityApplication { get; set; }
        [JsonProperty("electricityenddate")]
        public string electricityEndDate { get; set; }
        [JsonProperty("electricityendtime")]
        public string electricityEndTime { get; set; }
        [JsonProperty("electricityoutagemessage")]
        public string electricityOutageMessage { get; set; }
        [JsonProperty("electricitystartDate")]
        public string electricityStartDate { get; set; }
        [JsonProperty("electricitystarttime")]
        public string electricityStartTime { get; set; }
        [JsonProperty("plannedelectricityoutage")]
        public string plannedElectricityOutage { get; set; }
        [JsonProperty("plannedwateroutage")]
        public string plannedwaterOutage { get; set; }
        [JsonProperty("waterapplication")]
        public string waterApplication { get; set; }
        [JsonProperty("waterenddate")]
        public string waterEndDate { get; set; }
        [JsonProperty("waterendtime")]
        public string waterEndTime { get; set; }
        [JsonProperty("wateroutagemessage")]
        public string waterOutageMessage { get; set; }
        [JsonProperty("waterstartdate")]
        public string waterStartDate { get; set; }
        [JsonProperty("waterstarttime")]
        public string waterStartTime { get; set; }
    }
}
