// <copyright file="GetSmartConsumptionResponse.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

using DEWAXP.Foundation.Integration.DewaSvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEWAXP.Feature.Dashboard.Models.SmartAlerts
{
    /// <summary>
    /// Defines the <see cref="GetSmartAlertResponse" />.
    /// </summary>
    public class GetSmartAlertResponseModel
    {
        public SlabTariff slabTariff { get; set; } = new SlabTariff();
        public List<CellingTariff> lstcellingTariff { get; set; } = new List<CellingTariff>();

        public static GetSmartAlertResponseModel From(smartAlert[] reading)
        {
            var model = new GetSmartAlertResponseModel();
            if(reading != null & reading.Length > 0)
            {
                if(reading.ToList().Where(x => x.division.Equals("01") && x.subscriptionType.Equals("S")).Any())
                {
                    var alert = reading.ToList().Where(x => x.division.Equals("01") && x.subscriptionType.Equals("S")).FirstOrDefault();
                    if(string.IsNullOrWhiteSpace(alert.unsubscribeCompletly))
                    {
                        model.slabTariff.electricityflag = true;
                    }
                    
                }
                if (reading.ToList().Where(x => x.division.Equals("02") && x.subscriptionType.Equals("S")).Any())
                {
                    var alert = reading.ToList().Where(x => x.division.Equals("02") && x.subscriptionType.Equals("S")).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(alert.unsubscribeCompletly))
                    {
                        model.slabTariff.waterflag = true;
                    }

                }
                
                var selectedCellingModel = reading.ToList().Where(x => x.subscriptionType.Equals("U"));
                if (selectedCellingModel != null && selectedCellingModel.Count() > 0)
                {
                    Array.ForEach(selectedCellingModel.ToArray(), x => model.lstcellingTariff.Add(new CellingTariff {
                        aprflag = x.apr,
                        augflag = x.aug,
                        decflag = x.dec,
                        febflag = x.feb,
                        janflag = x.jan,
                        julflag = x.jul,
                        junflag = x.jun,
                        marflag = x.mar,
                        mayflag = x.may,
                        novflag = x.nov,
                        octflag = x.oct,
                        division = x.division,
                        quantity = !string.IsNullOrWhiteSpace(x.quantity) ? Math.Round(double.Parse(x.quantity),2).ToString():"0.0",
                        unit = x.unit,
                        sptflag = x.sep,
                        subscribed = string.IsNullOrWhiteSpace(x.unsubscribeCompletly)? true : (x.unsubscribeCompletly.Equals("U") ? false: true)
                    }
                    ));
                }
            }
            return model;
        }
    }

    public class SlabTariff
    {
        public bool electricityflag { get; set; } = false;
        public bool waterflag { get; set; } = false;
    }
    public class CellingTariff
    {
        public bool janflag { get; set; }
        public bool febflag { get; set; }
        public bool marflag { get; set; }
        public bool aprflag { get; set; }
        public bool mayflag { get; set; }
        public bool junflag { get; set; }
        public bool julflag { get; set; }
        public bool augflag { get; set; }
        public bool sptflag { get; set; }
        public bool octflag { get; set; }
        public bool novflag { get; set; }
        public bool decflag { get; set; }
        public bool subscribed { get; set; }
        public string unit { get; set; }
        public string quantity { get; set; }
        public string division { get; set; }
    }

}
