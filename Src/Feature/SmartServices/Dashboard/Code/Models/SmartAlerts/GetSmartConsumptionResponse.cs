// <copyright file="GetSmartConsumptionResponse.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEWAXP.Feature.Dashboard.Models.SmartAlerts
{
    /// <summary>
    /// Defines the <see cref="GetSmartConsumptionResponse" />.
    /// </summary>
    public class GetSmartConsumptionResponse
    {
        public string electricityconsumption { get; set; } = "0.0";
        public string waterconsumption { get; set; } = "0.0";
        public string carbonconsumption { get; set; } = "0.0";
        public string electricitybillingmonth { get; set; } = "";
        public string waterbillingmonth { get; set; } = "";
        public string electricityalerts { get; set; } = "0,0,0";
        public int electricityalertscount { get; set; } = 0;
        public bool iselectricityalert { get; set; } = false;
        public string wateralerts { get; set; } = "0,0,0";
        public string waterslabs { get; set; } = "0,0,0,0";
        public string electricityslabs { get; set; } = "0,0,0,0";
        public int wateralertscount { get; set; } = 0;
        public bool iswateralert { get; set; } = false;
        public BillingClassification accountype { get; set; }
       

        public static GetSmartConsumptionResponse From(smartAlertReadingOut smartAlertReadingOut,string account,ServiceResponse<AccountDetails[]> response)
        {
            var model = new GetSmartConsumptionResponse();
            if(smartAlertReadingOut != null)
            {
                if(smartAlertReadingOut.smartalert != null && smartAlertReadingOut.smartalert.Count() > 0)
                {
                    if(smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("01")).Any())
                    {
                        double count;
                        if (double.TryParse(smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("01")).FirstOrDefault().consumption , out count))
                        {
                            if (count > 0.0)
                            {
                                count = Math.Round(count, 2);
                                model.electricityconsumption =count.ToString();

                                var electricitydata = smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("01")).FirstOrDefault();
                                List<string> lstelecstring = new List<string>();
                                var slab1 = slabdouble(electricitydata.slab1);
                                if(!string.IsNullOrWhiteSpace(slab1))
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
                                
                                model.electricityslabs = string.Join(",", lstelecstring.ToArray());
                            }
                        }
                    }
                    if (smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("02")).Any())
                    {
                        double count;
                        if (double.TryParse(smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("02")).FirstOrDefault().consumption, out count))
                        {
                            if (count > 0.0)
                            {
                                count = Math.Round(count, 2);
                                model.waterconsumption = count.ToString();
                                var waterdata = smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("02")).FirstOrDefault();
                                List<string> lstelecstring = new List<string>();
                                var slab1 = slabdouble(waterdata.slab1);
                                if (!string.IsNullOrWhiteSpace(slab1))
                                    lstelecstring.Add(slab1);
                                var slab2 = slabdouble(waterdata.slab2);
                                if (!string.IsNullOrWhiteSpace(slab2))
                                    lstelecstring.Add(slab2);
                                var slab3 = slabdouble(waterdata.slab3);
                                if (!string.IsNullOrWhiteSpace(slab3))
                                    lstelecstring.Add(slab3);
                                var slab4 = slabdouble(waterdata.slab4);
                                if (!string.IsNullOrWhiteSpace(slab4))
                                    lstelecstring.Add(slab4);
                                var slab5 = slabdouble(waterdata.slab5);
                                if (!string.IsNullOrWhiteSpace(slab5))
                                    lstelecstring.Add(slab5);
                                model.waterslabs = string.Join(",", lstelecstring.ToArray());
                            }
                        }
                    }
                    if (smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("01")).Any())
                    {
                        double count;
                        if (double.TryParse(smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("01")).FirstOrDefault().carbon, out count))
                        {
                            if (count > 0.0)
                            {
                                count = Math.Round(count, 2);
                                model.carbonconsumption = count.ToString();
                            }
                        }
                    }
                    //model.electricityconsumption = smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("01")).Any() ?
                    //    smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("01")).FirstOrDefault().consumption : string.Empty;
                    //model.waterconsumption = smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("02")).Any() ?
                    //    smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("02")).FirstOrDefault().consumption : string.Empty;
                    //model.carbonconsumption = smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("01")).Any() ?
                    //    smartAlertReadingOut.smartalert.ToList().Where(x => x.division.Equals("01")).FirstOrDefault().carbon : string.Empty;

                    string selectedaccount = account.TrimStart(new char[] { '0' });
                    
                    var accountsresponse = response;
                    if (accountsresponse != null && accountsresponse.Payload != null && accountsresponse.Payload.Count() > 0 && accountsresponse.Payload.Where(x => x.AccountNumber.Equals(account)).Any())
                    {
                        var selectedaccountList = accountsresponse.Payload.Where(x => x.AccountNumber.Equals(selectedaccount));
                        if (selectedaccountList != null && selectedaccountList.Count() > 0)
                        {
                            var selectedaccountdetails = selectedaccountList.FirstOrDefault();
                            model.accountype = selectedaccountdetails.BillingClass;
                        }

                    }
                }
                if(smartAlertReadingOut.smartalertdetails !=null && smartAlertReadingOut.smartalertdetails.Count() >0)
                {
                    
                    if(smartAlertReadingOut.smartalertdetails.ToList().Where(x => x.division.Equals("01")).Any())
                    {
                        var electricityalert = smartAlertReadingOut.smartalertdetails.ToList().Where(x => x.division.Equals("01")).FirstOrDefault();
                       List<string> lstelecstring = new List<string>();
                       if(!string.IsNullOrWhiteSpace(electricityalert.consumptionAlert1))
                        {
                            double count;
                           if(double.TryParse(electricityalert.consumptionAlert1,out count))
                            {
                                if (count > 0.0)
                                {
                                    count = Math.Round(count, 2);
                                    lstelecstring.Add(count.ToString());
                                    model.electricityalertscount += 1;
                                }
                                else
                                {
                                    lstelecstring.Add("0");
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(electricityalert.consumptionAlert2))
                        {
                            double count;
                            if (double.TryParse(electricityalert.consumptionAlert2, out count))
                            {
                                if (count > 0.0)
                                {
                                    count = Math.Round(count, 2);
                                    lstelecstring.Add(count.ToString());
                                    model.electricityalertscount += 1;
                                }
                                else
                                {
                                    lstelecstring.Add("0");
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(electricityalert.consumptionAlert3))
                        {
                            double count;
                            if (double.TryParse(electricityalert.consumptionAlert3, out count))
                            {
                                if (count > 0.0)
                                {
                                    count = Math.Round(count, 2);
                                    lstelecstring.Add(count.ToString());
                                    model.electricityalertscount += 1;
                                }
                                else
                                {
                                    lstelecstring.Add("0");
                                }
                            }
                        }
                       //lstelecstring.Add(electricityalert.consumptionAlert1);
                       //lstelecstring.Add(electricityalert.consumptionAlert2);
                       //lstelecstring.Add(electricityalert.consumptionAlert3);
                       model.electricityalerts = string.Join(",", lstelecstring.ToArray());
                       model.iselectricityalert = model.electricityalertscount > 0;
                        model.electricitybillingmonth = electricityalert.billingMonthYear;
                    }
                    if (smartAlertReadingOut.smartalertdetails.ToList().Where(x => x.division.Equals("02")).Any())
                    {
                        var wateralert = smartAlertReadingOut.smartalertdetails.ToList().Where(x => x.division.Equals("02")).FirstOrDefault();
                        List<string> lstwaterstring = new List<string>();
                        if (!string.IsNullOrWhiteSpace(wateralert.consumptionAlert1))
                        {
                            double count;
                            if (double.TryParse(wateralert.consumptionAlert1, out count))
                            {
                                if (count > 0.0)
                                {
                                    count = Math.Round(count, 2);
                                    lstwaterstring.Add(count.ToString());
                                    model.wateralertscount += 1;
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(wateralert.consumptionAlert2))
                        {
                            double count;
                            if (double.TryParse(wateralert.consumptionAlert2, out count))
                            {
                                if (count > 0.0)
                                {
                                    count = Math.Round(count, 2);
                                    lstwaterstring.Add(count.ToString());
                                    model.wateralertscount += 1;
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(wateralert.consumptionAlert3))
                        {
                            double count;
                            if (double.TryParse(wateralert.consumptionAlert3, out count))
                            {
                                if (count > 0.0)
                                {
                                    count = Math.Round(count, 2);
                                    lstwaterstring.Add(count.ToString());
                                    model.wateralertscount += 1;
                                }
                            }
                        }
                        //lstwaterstring.Add(smartAlertReadingOut.smartalertdetails.ToList().Where(x => x.division.Equals("02")).FirstOrDefault().consumptionAlert1);
                        //lstwaterstring.Add(smartAlertReadingOut.smartalertdetails.ToList().Where(x => x.division.Equals("02")).FirstOrDefault().consumptionAlert2);
                        //lstwaterstring.Add(smartAlertReadingOut.smartalertdetails.ToList().Where(x => x.division.Equals("02")).FirstOrDefault().consumptionAlert3);
                        model.wateralerts = string.Join(",", lstwaterstring.ToArray());
                        model.iswateralert = model.wateralertscount > 0;
                        model.waterbillingmonth = wateralert.billingMonthYear;
                    }

                }
            }
            
            return model;
        }

        public static string slabdouble(string value)
        {
            string output = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                var arrelec = value.Split(';');
                if (arrelec.Length > 1)
                {
                    double count;
                    if (double.TryParse(arrelec[1], out count))
                    {
                        if (count > 0.0)
                        {
                            output = arrelec[1];
                        }
                    }
                }
            }
            return output;
        }
    }

    public class SmartConsumptionChart
    {
        public string electricity { get; set; }
        public string water { get; set; }
        public string carbon { get; set; }
    }

    
}
