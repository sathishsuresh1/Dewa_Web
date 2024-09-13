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
    public class ConsumptionDataPoint
    {
	    private string _accountNumber;
	    private string _businessPartnerNumber;

	    [XmlElement(ElementName = "PrintDocNumber")]
        public string DocumentReference { get; set; }

	    [XmlElement(ElementName = "ContractAccount")]
	    public string AccountNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_accountNumber); }
			set { _accountNumber = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "BusinessPartner")]
	    public string BusinessPartnerNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_businessPartnerNumber); }
			set { _businessPartnerNumber = value ?? string.Empty; }
	    }

	    [XmlElement(ElementName = "BillingDateMonth")]
        public string BillingMonth { get; set; }
        
        [XmlElement(ElementName = "ConsumtionValue")]
        public int Value { get; set; }

        [XmlElement(ElementName = "Type")]
        public MunicipalService Utility { get; set; }

        public DateTime Period
        {
            get { return DateTime.ParseExact(BillingMonth, "yyyy/MM", CultureInfo.InvariantCulture); }
        }
    }

    [XmlRoot(ElementName = "Item")]
    public class AverageConsumptionDataPoint : ConsumptionDataPoint
    {
	    private string _comparisonAccountNumber;

	    [XmlElement(ElementName = "ContractAccountNumber")]
	    public string ComparisonAccountNumber
	    {
            get { return DewaResponseFormatter.Trimmer(_comparisonAccountNumber); }
			set { _comparisonAccountNumber = value ?? string.Empty; }
		}

	    [XmlElement(ElementName = "AvgConsumtionValue")]
        public decimal Average { get; set; }
    }

    public class ConsumptionPeriod
    {
        [XmlElement(ElementName = "Item")]
        public List<ConsumptionDataPoint> DataPoints { get; set; }

	    public DateTime PeriodStart
	    {
			get { return DataPoints.Min(dp => dp.Period); }
	    }

		public DateTime PeriodEnd
		{
			get { return DataPoints.Max(dp => dp.Period); }
		}
	}

    [XmlRoot(ElementName = "Water")]
    public class Utility
    {
        [XmlElement(ElementName = "FifthYear")]
        public ConsumptionPeriod FifthYear { get; set; }
        [XmlElement(ElementName = "FourthYear")]
        public ConsumptionPeriod FourthYear { get; set; }
        [XmlElement(ElementName = "ThirdYear")]
        public ConsumptionPeriod ThirdYear { get; set; }

        [XmlElement(ElementName = "SecondYear")]
        public ConsumptionPeriod SecondYear { get; set; }

        [XmlElement(ElementName = "FirstYear")]
        public ConsumptionPeriod FirstYear { get; set; }

        [XmlElement(ElementName = "Item")]
        public List<AverageConsumptionDataPoint> Averages { get; set; }

        public IEnumerable<ConsumptionDataPoint> AllDataPoints
        {
            get
            {
                if (IsTimeSeries())
                {
                    return FirstYear.DataPoints
                    .Union(SecondYear.DataPoints)
                    .Union(ThirdYear.DataPoints)
                    .Union(FourthYear.DataPoints)
                    .Union(FifthYear.DataPoints)
                    .OrderBy(dp => dp.BillingMonth);
                }
                return Averages;
            }
        }

        private bool IsTimeSeries()
        {
            return FirstYear != null && SecondYear != null && ThirdYear != null && FourthYear != null && FifthYear != null;
        }
    }

    [XmlRoot(ElementName = "AccountNo")]
    public class Utilities
    {
        [XmlElement(ElementName = "Electricity")]
        public Utility Electricity { get; set; }

        [XmlElement(ElementName = "Water")]
        public Utility Water { get; set; }
    }

    [XmlRoot(ElementName = "YearlyConsumption")]
    public class YearlyConsumptionDataResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "AccountNo")]
        public Utilities Utilities { get; set; }
    }

    [XmlRoot(ElementName = "YearlyAverageConsumption")]
    public class YearlyAverageConsumptionDataResponse : YearlyConsumptionDataResponse
    {
        
    }
}
