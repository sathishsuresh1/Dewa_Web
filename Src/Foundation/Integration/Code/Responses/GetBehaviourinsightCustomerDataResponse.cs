using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "Item")]
    public class BehaviourinsightCustomerItem
    {
        

        [XmlElement(ElementName = "ContractAccountNumber")]
        public string ContractAccountNumber { get; set; }

        [XmlElement(ElementName = "BillingDateMonth")]
        public string BillingDateMonth { get; set; }

        [XmlElement(ElementName = "ConsumtionValue")]
        public string ConsumtionValue { get; set; }

        [XmlElement(ElementName = "AvgConsumtionValue")]
        public string AvgConsumtionValue { get; set; }

        [XmlElement(ElementName = "LowConsumtionValue")]
        public string LowConsumtionValue { get; set; }

        [XmlElement(ElementName = "BestCustomer")]
        public string BestCustomer { get; set; }
    }
        
    [XmlRoot(ElementName = "Electricity")]
    public class BehaviourinsightCustomerElectricity
    {
        [XmlElement(ElementName = "Item")]
        public List<BehaviourinsightCustomerItem> Item { get; set; }
    }
    
    [XmlRoot(ElementName = "AccountNo")]
    public class BehaviourinsightCustomerAccountNo
    {

        [XmlElement(ElementName = "Electricity")]
        public BehaviourinsightCustomerElectricity Electricity { get; set; }
      
    }

    [XmlRoot(ElementName = "BehaviourinsightCustomer")]
    public class GetBehaviourinsightCustomerDataResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }


        [XmlElement(ElementName = "AccountNo")]
        public BehaviourinsightCustomerAccountNo AccountNo { get; set; }
    }


}
