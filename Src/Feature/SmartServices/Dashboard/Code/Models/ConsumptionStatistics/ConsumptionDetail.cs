// <copyright file="ConsumptionDetail.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>
using Sitecore.Globalization;

namespace DEWAXP.Feature.Dashboard.Models.ConsumptionStatistics
{
    using DEWAXP.Foundation.Integration.APIHandler.Models.Response.meterreading;
    using DEWAXP.Foundation.Integration.DewaSvc;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Responses;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="ConsumptionDetail" />.
    /// </summary>
    public class ConsumptionDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumptionDetail"/> class.
        /// </summary>
        public ConsumptionDetail()
        {
            dataSeries = new List<OptionDataSeries>();
        }

        /// <summary>
        /// Gets or sets the dataSeries.
        /// </summary>
        public List<OptionDataSeries> dataSeries { get; set; }

        public static string slabdouble(string value)
        {
            string output = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                double count;
                if (double.TryParse(value, out count))
                {
                    if (count > 0.0)
                    {
                        output = count.ToString();
                    }
                }
            }
            return output;
        }
        public static string slabList(slabTarrifOut slabTarrifOut,string division)
        {
            string electricityslabs = "0,0,0,0";
            if (slabTarrifOut != null && slabTarrifOut.slabCaps != null && slabTarrifOut.slabCaps.Count() > 0)
            {
                var electricitydata = slabTarrifOut.slabCaps.ToList().Where(x => x.division.Equals(division)).FirstOrDefault();
                if (electricitydata != null)
                {
                    List<string> lstelecstring = new List<string>();
                    var slab1 = slabdouble(electricitydata.slab1);
                    if (!string.IsNullOrWhiteSpace(slab1))
                        lstelecstring.Add(slab1);
                    var slab2 = slabdouble(electricitydata.slab2);
                    if (!string.IsNullOrWhiteSpace(slab2))
                        lstelecstring.Add(slab2);
                    var slab3 = slabdouble(electricitydata.slab3);
                    if (!string.IsNullOrWhiteSpace(slab3))
                        lstelecstring.Add(slab3);
                    var slab4 = slabdouble(electricitydata.slab4);
                    if (!string.IsNullOrWhiteSpace(slab4))
                        lstelecstring.Add(slab4);
                    var slab5 = slabdouble(electricitydata.slab5);
                    if (!string.IsNullOrWhiteSpace(slab5))
                        lstelecstring.Add(slab5);

                    electricityslabs = string.Join(",", lstelecstring.ToArray());
                }
            }
            return electricityslabs;
        }

        public static ConsumptionDetail From(List<DataPointwithutility> dataPoints, slabTarrifOut slabTarrifOut, MeterreadingResponse meterreadingresponse)
        {
            
            ConsumptionDetail model = new ConsumptionDetail();
            string electricityslabs = slabList(slabTarrifOut, "01");
            string waterslabs = slabList(slabTarrifOut, "02");

            CultureInfo culture;
            culture = global::Sitecore.Context.Culture;

            if (meterreadingresponse != null && (meterreadingresponse.errorCodeE.Equals("000") || meterreadingresponse.errorCodeW.Equals("000"))
                && (Convert.ToDecimal(meterreadingresponse.electricity_Reading) > 0 || Convert.ToDecimal(meterreadingresponse.water_Reading) > 0))
            {

                string FormattedDate = Translate.Text("Unbilled Consumption");
                if (meterreadingresponse.start_date.HasValue)
                {
                    string _dFormate = "dd {0} yyyy";
                    FormattedDate = string.Format(meterreadingresponse.start_date?.ToString(_dFormate), GetCustomCultureMonth(meterreadingresponse.start_date)) + Translate.Text(" - till yesterday.");
                }

                // Add the Unbilled consumption
                model.dataSeries.Add(new OptionDataSeries
                {
                    monthText = FormattedDate,
                    monthValue = "UnbilledConsumption",
                    electricityconsumption = meterreadingresponse.electricity_Reading.ToString(),
                    carbonconsumption = meterreadingresponse.electricity_Reading != 0 ? (meterreadingresponse.electricity_Reading * float.Parse("0.4")).ToString() : "0",
                    waterconsumption = meterreadingresponse.water_Reading.ToString(),
                    electricitySlabs = electricityslabs,
                    waterSlabs = waterslabs,
                    carbonSlabs = "500,750,750,750"
                });
            }



            if (dataPoints != null && dataPoints.Count > 1)
            {
                var dataPointsModels = dataPoints.OrderByDescending(x => new DateTime(x.year, x.month,1)).GroupBy(x => new { x.month, x.year });
                if (dataPointsModels != null)
                {
                    dataPointsModels.ToList()?.ForEach(x => model.dataSeries.Add(new OptionDataSeries
                    {
                        monthText = (culture.ToString().Equals("ar-AE") ? DateTime.Parse(new DateTime(DateTime.Now.Year,x.Key.month, 1).ToString(), CultureInfo.InvariantCulture).ToString("MMM", culture) : string.Format("{0:MMM}", new DateTime(DateTime.Now.Year,x.Key.month, 1)))+" "   + x.Key.year.ToString(),
                        monthValue = x.Key.month.ToString() + x.Key.year.ToString(),
                        electricityconsumption = x.Where(y => y.service.Equals(MunicipalService.Electricity)).Any() ? x.Where(y => y.service.Equals(MunicipalService.Electricity)).FirstOrDefault().value.ToString() : "0",
                        carbonconsumption = x.Where(y => y.service.Equals(MunicipalService.Electricity)).Any() ? Math.Ceiling(x.Where(y => y.service.Equals(MunicipalService.Electricity)).FirstOrDefault().value * decimal.Parse("0.4")).ToString() : "0",
                        waterconsumption = x.Where(y => y.service.Equals(MunicipalService.Water)).Any() ? x.Where(y => y.service.Equals(MunicipalService.Water)).FirstOrDefault().value.ToString() : "0",
                        electricitySlabs = electricityslabs,
                        waterSlabs = waterslabs,
                        carbonSlabs ="500,750,750,750"
                    }));
                }
            }
            return model;
        }

        private static string GetCustomCultureMonth(DateTime? date)
        {
            string _month = "";
            CultureInfo culture;
            culture = global::Sitecore.Context.Culture;
            if (date.HasValue)
            {
                _month = DateTime.Parse(date?.ToString(), CultureInfo.InvariantCulture).ToString("MMM", culture);
            }
            return _month;
        }
    }


    /// <summary>
    /// Defines the <see cref="OptionDataSeries" />.
    /// </summary>
    public class OptionDataSeries
    {
        /// <summary>
        /// Gets or sets the Electricityconsumption.
        /// </summary>
        public string electricityconsumption { get; set; }

        /// <summary>
        /// Gets or sets the Waterconsumption.
        /// </summary>
        public string waterconsumption { get; set; }

        /// <summary>
        /// Gets or sets the Carbonconsumption.
        /// </summary>
        public string carbonconsumption { get; set; }

        /// <summary>
        /// Gets or sets the ElectricitySlabs.
        /// </summary>
        public string electricitySlabs { get; set; }

        /// <summary>
        /// Gets or sets the WaterSlabs.
        /// </summary>
        public string waterSlabs { get; set; }

        /// <summary>
        /// Gets or sets the CarbonSlabs.
        /// </summary>
        public string carbonSlabs { get; set; }

        /// <summary>
        /// Gets or sets the monthText.
        /// </summary>
        public string monthText { get; set; }

        /// <summary>
        /// Gets or sets the monthValue.
        /// </summary>
        public string monthValue { get; set; }

        public bool iselectricityconsumed
        {
            get
            {
                return Convert.ToDecimal(electricityconsumption ?? "0") > 0;
            }
        }

        public bool iswaterconsumed
        {
            get
            {
                return Convert.ToDecimal(waterconsumption ?? "0") > 0;
            }
        }
    }

    public class DataPointwithutility
    {
        public DataPointwithutility(DataPointV1 dataPointV1,MunicipalService municipalService)
        {

            this.year = dataPointV1.year;
            this.month = dataPointV1.month;
            this.value = dataPointV1.value;
            this.service = municipalService;
        }
        public MunicipalService service { get; set; }
        public decimal value { get; set; }

        public int year { get; set; }

        public int month { get; set; }
    }
}