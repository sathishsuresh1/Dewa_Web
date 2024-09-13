using DEWAXP.Feature.ShamsDubai.Models.SolarCalculator;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DEWAXP.Feature.ShamsDubai.Controllers.Api
{
    public class SolarCalculatorController : BaseApiController
    {
        private static double[] CommelectyBreaks = { 0.0, 2000.0, 4000.0, 6000.0 };
        private static double[] CommelectyPrices = { 0.23, 0.28, 0.32, 0.38 };
        private static double CommelectyFuelCharge = 0.065;
        private static Tariff CommelectyTariff = new Tariff(CommelectyBreaks, CommelectyPrices, CommelectyFuelCharge);

        private static double[] GovelectyBreaks = { 0, 10000 };
        private static double[] GovelectyPrices = { 0.23, 0.38 };
        private static double GovelectyFuelCharge = 0.065;
        private static Tariff GovelectyTariff = new Tariff(GovelectyBreaks, GovelectyPrices, GovelectyFuelCharge);

        private static double[] IndelectBreaks = { 0, 10000 };
        private static double[] IndelectPrices = { 0.23, 0.38 };
        private static double IndelectFuelCharge = 0.065;
        private static Tariff IndelectTariff = new Tariff(IndelectBreaks, IndelectPrices, IndelectFuelCharge);

        private static double[] ResiquanteBreaks = { 0, 2000, 4000, 6000 };
        private static double[] ResiquantePrices = { 0.23, 0.28, 0.32, 0.38 };
        private static double ResiquanteFuelCharge = 0.065;
        private static Tariff ResiquanteTariff = new Tariff(ResiquanteBreaks, ResiquantePrices, ResiquanteFuelCharge);

        private static double[] NresiqnteBreaks = { 0, 2000, 4000, 6000 };
        private static double[] NresiqntePrices = { 0.075, 0.09, 0.105, 0.125 };
        private static double NresiqnteFuelCharge = 0;
        private static Tariff NresiqnteTariff = new Tariff(NresiqnteBreaks, NresiqntePrices, NresiqnteFuelCharge);

        // GET: SolarCalculator
        public IHttpActionResult Account()
        {
            try
            {
                SharedAccount selectedAccount = null;
                CacheProvider.TryGet<SharedAccount>(CacheKeys.SOLAR_CALCULATOR_SELECTED_ACCOUNT, out selectedAccount);
                string id = string.Empty;
                List<ConsumptionAccount> lstAccounts = null;
                double annualUsageKWh = 0;
                double annualUsageAED = 0;
                string accountCategory = string.Empty;
                string accountName = string.Empty;
                var isCorrectMonths = true;
                SolarCalculatorUsageItem[] usageItems = new SolarCalculatorUsageItem[12];

                if (selectedAccount == null)
                {
                    throw new Exception("No Contract Account found in cache");
                }

                id = selectedAccount.AccountNumber;
                string[] contracts = { id };
                accountCategory = selectedAccount.Type;
                accountName = selectedAccount.Name;

                var response = DewaApiClient.GetComparativeConsumption(CurrentPrincipal.SessionToken, contracts, RequestLanguage, DEWAXP.Foundation.Integration.Enums.RequestSegment.Desktop);

                if (response.Succeeded)
                {
                    if (response.Payload != null)
                    {
                        lstAccounts = response.Payload.Accounts;
                        if (lstAccounts.Count > 0)
                        {
                            ConsumptionAccount _account = lstAccounts[0];
                            var correctMonths = getLastYear();
                            ConsumptionDataPoint item = null;
                            var currentMonthIndex = 0;
                            var monthCount = 0;
                            int arrCounter = 12;
                            for (var i = _account.DataPoints.Count - 1; i > 0; i--)
                            {
                                item = _account.DataPoints[i];
                                if (item.Utility == DEWAXP.Foundation.Integration.Enums.MunicipalService.Electricity)
                                {
                                    monthCount++;
                                    annualUsageKWh += item.Value;
                                    annualUsageAED += calculateTariff(item.Value, accountCategory.Substring(2));
                                    isCorrectMonths = isCorrectMonths && (item.BillingMonth == correctMonths[currentMonthIndex]);
                                    if (currentMonthIndex == 0 && !isCorrectMonths)
                                    {
                                        currentMonthIndex++;
                                        isCorrectMonths = item.BillingMonth == correctMonths[currentMonthIndex];
                                    }
                                    usageItems[arrCounter - 1] = new SolarCalculatorUsageItem
                                    {
                                        index = 12 - arrCounter,
                                        month = int.Parse(item.BillingMonth.Substring(item.BillingMonth.IndexOf("/") + 1)),
                                        year = int.Parse(item.BillingMonth.Substring(0, item.BillingMonth.IndexOf("/"))),
                                        usageDhm = (Decimal)calculateTariff(item.Value, accountCategory.Substring(2)),
                                        usageKWh = item.Value
                                    };
                                    arrCounter--;
                                    currentMonthIndex++;
                                    if (monthCount == 12)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (monthCount != 12)
                            {
                                isCorrectMonths = false;
                            }
                        }
                        else
                        {
                            throw new Exception("No Contract Accounts Found");
                        }
                    }
                    else
                    {
                        throw new Exception("No Contract Accounts Found");
                    }
                }
                else
                {
                    throw new Exception("Account Response Failed");
                }

                if (isCorrectMonths)
                {
                    var account = new SolarCalculatorAccountWithUsage
                    {
                        CustomerName = accountName,
                        AccountNumber = id,
                        YearlyUsageDhm = (Decimal)annualUsageAED,
                        YearlyUsageKWh = (Decimal)annualUsageKWh,
                        TariffType = TariffDescriptionToCode(accountCategory.Substring(2)),
                        CorrectYear = isCorrectMonths ? "true" : "false",
                        monthlyusage = usageItems
                    };

                    var accountResponse = new AccountResponseWithUsage
                    {
                        locale = "en-us",
                        account = account
                    };

                    return Ok(accountResponse);
                }
                else
                {
                    var account = new SolarCalculatorAccount
                    {
                        CustomerName = accountName,
                        AccountNumber = id,
                        YearlyUsageDhm = (Decimal)annualUsageAED,
                        YearlyUsageKWh = (Decimal)annualUsageKWh,
                        TariffType = TariffDescriptionToCode(accountCategory.Substring(2)),
                        CorrectYear = isCorrectMonths ? "true" : "false"
                    };

                    var accountResponse = new AccountResponse
                    {
                        locale = "en-us",
                        account = account
                    };

                    return Ok(accountResponse);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private string[] getLastYear()
        {
            DateTime time = DateTime.Today;
            string[] entries = new String[13];
            for (var i = 0; i < 13; i++)
            {
                entries[i] = time.Year + "/" + (time.Month.ToString().Length == 1 ? "0" + time.Month.ToString() : time.Month.ToString());
                time = time.AddMonths(-1);
            }
            return entries;
        }

        private double calculateTariff(double usage, string tariff)
        {
            Dictionary<string, Tariff> Tariffs = new Dictionary<string, Tariff>();
            Tariffs["COMMELECTY"] = CommelectyTariff;
            Tariffs["GOVELECTY"] = GovelectyTariff;
            Tariffs["INDELECT"] = IndelectTariff;
            Tariffs["RESIQUANTE"] = ResiquanteTariff;
            Tariffs["NRESIQNTE"] = NresiqnteTariff;

            tariff = TariffDescriptionToCode(tariff);

            var breaks = Tariffs[tariff].Breaks;
            var prices = Tariffs[tariff].Prices;
            var fuelcharge = Tariffs[tariff].FuelCharges;
            var remainingUsage = usage;

            var cost = usage * fuelcharge;
            for (var i = 1; i < breaks.Length; i++)
            {
                if (remainingUsage == 0)
                {
                    break;
                }
                else
                {
                    if (usage < breaks[i])
                    {
                        cost += remainingUsage * prices[i - 1];
                        remainingUsage = 0;
                    }
                    else
                    {
                        cost += (breaks[i] - breaks[i - 1]) * prices[i - 1];
                        remainingUsage -= (breaks[i] - breaks[i - 1]);
                        if (i + 1 == breaks.Length)
                        {
                            cost += remainingUsage * prices[i];
                            remainingUsage = 0;
                        }
                    }
                }
            }

            return cost;
        }

        private string TariffDescriptionToCode(string TariffDescription)
        {
            switch (TariffDescription)
            {
                case "Commercial":
                    return "COMMELECTY";

                case "Government":
                    return "GOVELECTY";

                case "Industrial":
                    return "INDELECT";

                case "Resi-Expat":
                    return "RESIQUANTE";

                case "Resi-National":
                    return "NRESIQNTE";

                default:
                    throw new Exception("Tariff not recognized " + TariffDescription);
            }
        }
    }
}