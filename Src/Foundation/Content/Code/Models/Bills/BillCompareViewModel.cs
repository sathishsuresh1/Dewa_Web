// <copyright file="BillCompareViewModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Models.Bills
{
    using DEWAXP.Foundation.Content.Utils;
    using DEWAXP.Foundation.Integration.DewaSvc;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="BillCompareViewModel" />.
    /// </summary>
    public class BillCompareViewModel
    {
        /// <summary>
        /// Defines the Separator.
        /// </summary>
        private const string Separator = ",";

        /// <summary>
        /// Gets the Transactions.
        /// </summary>
        public List<TransactionModel> Transactions { get; private set; } = new List<TransactionModel>();

        /// <summary>
        /// Gets the Invoices.
        /// </summary>
        public List<InvoiceModel> Invoices { get; private set; } = new List<InvoiceModel>();

        /// <summary>
        /// Gets the Receipts.
        /// </summary>
        public List<ReceiptModel> Receipts { get; private set; } = new List<ReceiptModel>();

        /// <summary>
        /// Gets or sets the GroupedReceipts.
        /// </summary>
        public List<GroupedReceiptModel> GroupedReceipts { get; set; } = new List<GroupedReceiptModel>();

        /// <summary>
        /// Gets or sets the Lastyeartext.
        /// </summary>
        public string Lastyeartext { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether Lastyearflag.
        /// </summary>
        public bool Lastyearflag { get; set; }

        /// <summary>
        /// Gets or sets the Lastyearpercentage.
        /// </summary>
        public string Lastyearpercentage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Previousmonthtext.
        /// </summary>
        public string Previousmonthtext { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Previousmonthpercentage.
        /// </summary>
        public string Previousmonthpercentage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether Previousmonthflag.
        /// </summary>
        public bool Previousmonthflag { get; set; }

        /// <summary>
        /// Gets or sets the BilledAmountXaxis.
        /// </summary>
        public string BilledAmountXaxis { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the BilledAmountSeries.
        /// </summary>
        public string BilledAmountSeries { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the BilledAmountEVSeries.
        /// </summary>
        public string BilledAmountEVSeries { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the BilledAmountElectricityseries.
        /// </summary>
        public string BilledAmountElectricityseries { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the BilledAmountWaterSeries.
        /// </summary>
        public string BilledAmountWaterSeries { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the PaidAmountXaxis.
        /// </summary>
        public string PaidAmountXaxis { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the PaidAmountSeries.
        /// </summary>
        public string PaidAmountSeries { get; set; } = string.Empty;

        /// <summary>
        /// The From.
        /// </summary>
        /// <param name="serviceResponse">The serviceResponse<see cref="BillHistoryResponse"/>.</param>
        /// <returns>The <see cref="BillCompareViewModel"/>.</returns>
        public static BillCompareViewModel From(BillHistoryResponse serviceResponse)
        {
            BillCompareViewModel model = new BillCompareViewModel();
            List<TransactionModel> mergedSet = new List<TransactionModel>();
            if (serviceResponse != null)
            {
                if (serviceResponse.invoicelist != null && serviceResponse.invoicelist.Count() > 0)
                {
                    IEnumerable<InvoiceModel> invoices = serviceResponse.invoicelist.Select(InvoiceModel.FromAPI);
                    mergedSet.AddRange(invoices);
                    model.Invoices = invoices.OrderByDescending(x => x.myear).ToList();
                    model.BilledAmountXaxis = XaxiscalculationInvoice(model.Invoices);
                    model.BilledAmountSeries = InVoiceDataSeries(model.Invoices, false, false);
                    model.BilledAmountElectricityseries = InVoiceDataSeries(model.Invoices, true, false);
                    model.BilledAmountWaterSeries = InVoiceDataSeries(model.Invoices, false, true);
                }
                if (serviceResponse.paymentlist != null && serviceResponse.paymentlist.Count() > 0)
                {
                    IEnumerable<ReceiptModel> receipts = serviceResponse.paymentlist.Where(x =>string.IsNullOrWhiteSpace(x.Chequereturn) ||(!string.IsNullOrWhiteSpace(x.Chequereturn) && !x.Chequereturn.Equals("X"))).Select(ReceiptModel.FromAPI);
                    mergedSet.AddRange(receipts);
                    model.Receipts = receipts.ToList();


                    IOrderedEnumerable<ReceiptModel> orderReceipts = model.Receipts.OrderByDescending(x => x.Date);

                    var receiptModels = model.Receipts != null && model.Receipts.Count() > 0 ? orderReceipts.GroupBy(x => new { x.Date.Month, x.Date.Year }) : null;
                    if (receiptModels != null)
                    {
                        foreach (var grp in receiptModels)
                        {
                            GroupedReceiptModel groupedReceipt = new GroupedReceiptModel
                            {
                                Month = grp.Key.Month,
                                Year = grp.Key.Year,
                                TotalAmount = grp.ToList().Sum(x => x.Amount),
                                Receipts = grp.ToList()
                            };
                            model.GroupedReceipts.Add(groupedReceipt);
                        }
                    }
                    model.PaidAmountSeries = ReceiptDataSeries(model.GroupedReceipts);
                    model.PaidAmountXaxis = XaxiscalculationReceipts(model.GroupedReceipts);
                }
                model.Lastyeartext = serviceResponse.LyComparisonText;
                model.Lastyearpercentage = !string.IsNullOrWhiteSpace(serviceResponse.LyComparison) ? Math.Abs(double.Parse(serviceResponse.LyComparison)).ToString() : string.Empty;
                model.Lastyearflag = !string.IsNullOrWhiteSpace(serviceResponse.LyComparison) ? Math.Sign(double.Parse(serviceResponse.LyComparison)) < 0 : false;

                model.Previousmonthtext = serviceResponse.LmComparisonText;
                model.Previousmonthpercentage = !string.IsNullOrWhiteSpace(serviceResponse.LmComparison) ? Math.Abs(double.Parse(serviceResponse.LmComparison)).ToString() : string.Empty;
                model.Previousmonthflag = !string.IsNullOrWhiteSpace(serviceResponse.LmComparison) ? Math.Sign(double.Parse(serviceResponse.LmComparison)) < 0 : false;
            }
            return model;
        }

        /// <summary>
        /// The FromAPI.
        /// </summary>
        /// <param name="serviceResponse">The serviceResponse<see cref="BillHistoryResponse"/>.</param>
        /// <param name="EVaccount">The EVaccount<see cref="bool"/>.</param>
        /// <returns>The <see cref="BillCompareViewModel"/>.</returns>
        public static BillCompareViewModel FromAPI(BillHistoryResponse serviceResponse, bool EVaccount = false)
        {
            BillCompareViewModel model = new BillCompareViewModel();
            List<TransactionModel> mergedSet = new List<TransactionModel>();
            if (serviceResponse != null)
            {
                if (serviceResponse.invoicelist != null && serviceResponse.invoicelist.Count() > 0)
                {
                    IEnumerable<InvoiceModel> invoices = serviceResponse.invoicelist.Select(InvoiceModel.FromAPI);
                    mergedSet.AddRange(invoices);
                    model.Invoices = invoices.OrderByDescending(x => x.myear).ToList();
                    model.BilledAmountXaxis = XaxiscalculationInvoice(model.Invoices);
                    model.BilledAmountSeries = InVoiceDataSeries(model.Invoices, false, false);
                    model.BilledAmountEVSeries = InVoiceDataSeries(model.Invoices, false, false, true);
                    model.BilledAmountElectricityseries = InVoiceDataSeries(model.Invoices, true, false);
                    model.BilledAmountWaterSeries = InVoiceDataSeries(model.Invoices, false, true);
                }
                if (serviceResponse.paymentlist != null && serviceResponse.paymentlist.Count() > 0)
                {
                    IEnumerable<ReceiptModel> receipts = serviceResponse.paymentlist.Where(x => string.IsNullOrWhiteSpace(x.Chequereturn) || (!string.IsNullOrWhiteSpace(x.Chequereturn) && !x.Chequereturn.Equals("X"))).Select(ReceiptModel.FromAPI);
                    mergedSet.AddRange(receipts);
                    model.Receipts = receipts.ToList();


                    IOrderedEnumerable<ReceiptModel> orderReceipts = model.Receipts.OrderByDescending(x => x.Date);

                    var receiptModels = model.Receipts != null && model.Receipts.Count() > 0 ? orderReceipts.GroupBy(x => new { x.Date.Month, x.Date.Year }) : null;
                    if (receiptModels != null)
                    {
                        foreach (var grp in receiptModels)
                        {
                            GroupedReceiptModel groupedReceipt = new GroupedReceiptModel
                            {
                                Month = grp.Key.Month,
                                Year = grp.Key.Year,
                                TotalAmount = grp.ToList().Sum(x => x.Amount),
                                Receipts = grp.ToList()
                            };
                            model.GroupedReceipts.Add(groupedReceipt);
                        }
                    }
                    model.PaidAmountSeries = ReceiptDataSeries(model.GroupedReceipts);
                    model.PaidAmountXaxis = XaxiscalculationReceipts(model.GroupedReceipts);
                }
                model.Lastyeartext = serviceResponse.LyComparisonText;
                model.Lastyearpercentage = !string.IsNullOrWhiteSpace(serviceResponse.LyComparison) ? Math.Abs(double.Parse(serviceResponse.LyComparison)).ToString() : string.Empty;
                model.Lastyearflag = !string.IsNullOrWhiteSpace(serviceResponse.LyComparison) ? Math.Sign(double.Parse(serviceResponse.LyComparison)) < 0 : false;

                model.Previousmonthtext = serviceResponse.LmComparisonText;
                model.Previousmonthpercentage = !string.IsNullOrWhiteSpace(serviceResponse.LmComparison) ? Math.Abs(double.Parse(serviceResponse.LmComparison)).ToString() : string.Empty;
                model.Previousmonthflag = !string.IsNullOrWhiteSpace(serviceResponse.LmComparison) ? Math.Sign(double.Parse(serviceResponse.LmComparison)) < 0 : false;
            }
            return model;
        }

        /// <summary>
        /// The Xaxiscalculation.
        /// </summary>
        /// <param name="transactionmodel">The transactionmodel<see cref="List{TransactionModel}"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string XaxiscalculationInvoice(List<InvoiceModel> transactionmodel)
        {
            string xaxis = string.Empty;
            CultureInfo culture;
            culture = global::Sitecore.Context.Culture;
            List<string> lstxaxis = new List<string>();

            if (transactionmodel != null && transactionmodel.Count > 0)
            {
                foreach (InvoiceModel i in transactionmodel)
                {
                    DateTime _formattedDate = CommonUtility.DateTimeFormatParse(i.myear, "yyyyMM");
                    lstxaxis.Add(string.Format(@"<div class=""m84-bills-compare--graph_xaxis""><span class=""month""> {0}</span><br> <span class=""year"">{1}<span></span></span></div>", _formattedDate.ToString("MMM", culture), _formattedDate.Year));
                }
            }
            xaxis = string.Join(Separator, lstxaxis.ToArray());
            return xaxis;
        }

        /// <summary>
        /// The XaxiscalculationReceipts.
        /// </summary>
        /// <param name="transactionmodel">The transactionmodel<see cref="List{GroupedReceiptModel}"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string XaxiscalculationReceipts(List<GroupedReceiptModel> transactionmodel)
        {
            string xaxis = string.Empty;
            CultureInfo culture;
            culture = global::Sitecore.Context.Culture;
            List<string> lstxaxis = new List<string>();

            if (transactionmodel != null && transactionmodel.Count > 0)
            {
                foreach (GroupedReceiptModel i in transactionmodel)
                {
                    lstxaxis.Add(string.Format(@"<div class=""m84-bills-compare--graph_xaxis""><span class=""month""> {0}</span><br> <span class=""year"">{1}<span></span></span></div>", (culture.ToString().Equals("ar-AE") ? DateTime.Parse(new DateTime(DateTime.Now.Year, i.Month, 1).ToString(), CultureInfo.InvariantCulture).ToString("MMM", culture) : string.Format("{0:MMM}", new DateTime(DateTime.Now.Year, i.Month, 1))), i.Year));
                }
            }
            xaxis = string.Join(Separator, lstxaxis.ToArray());
            return xaxis;
        }

        /// <summary>
        /// The InVoiceDataSeries.
        /// </summary>
        /// <param name="transactionmodel">The transactionmodel<see cref="List{TransactionModel}"/>.</param>
        /// <param name="electricity">The electricity<see cref="bool"/>.</param>
        /// <param name="water">The water<see cref="bool"/>.</param>
        /// <param name="evaccount">The evaccount<see cref="bool"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string InVoiceDataSeries(List<InvoiceModel> transactionmodel, bool electricity, bool water, bool evaccount = false)
        {
            string series = string.Empty;
            if (transactionmodel != null && transactionmodel.Count > 0)
            {
                if (electricity)
                {
                    IEnumerable<decimal> selectedmodel = transactionmodel.Select(y => y.electricityamount);
                    series = string.Join(Separator, selectedmodel.ToArray());
                }
                else if (water)
                {
                    IEnumerable<decimal> selectedmodel = transactionmodel.Select(y => y.wateramount);
                    series = string.Join(Separator, selectedmodel.ToArray());
                }
                else if (evaccount)
                {
                    List<EVMapping> mappings = new List<EVMapping>();
                    transactionmodel.ForEach(y => mappings.Add(new EVMapping {
                        y = y.Amount > 0.00m ? y.Amount : (y.discountamount == 0.00m ? y.Amount : y.discountamount),
                        striked = y.Amount > 0.00m ? "none" : (y.discountamount == 0.00m ? "none" : "line-through")
                    }));
                    series = JsonConvert.SerializeObject(mappings);
                }
                else
                {
                    IEnumerable<decimal> selectedmodel = transactionmodel.Select(y => y.currentamount);
                    series = string.Join(Separator, selectedmodel.ToArray());
                }
            }
            return series;
        }

        /// <summary>
        /// The ReceiptDataSeries.
        /// </summary>
        /// <param name="transactionmodel">The transactionmodel<see cref="List{TransactionModel}"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string ReceiptDataSeries(List<GroupedReceiptModel> transactionmodel)
        {
            string series = string.Empty;
            if (transactionmodel != null && transactionmodel.Count > 0)
            {
                IEnumerable<decimal> selectedmodel = transactionmodel.Select(y => y.TotalAmount);
                series = string.Join(Separator, selectedmodel.ToArray());
            }
            return series;
        }
    }
    public class EVMapping
    {
        public decimal y { get; set; }
        public string striked { get; set; }
}
    /// <summary>
    /// Defines the <see cref="GroupedReceiptModel" />.
    /// </summary>
    public class GroupedReceiptModel : TransactionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedReceiptModel"/> class.
        /// </summary>
        public GroupedReceiptModel()
        {
            Type = TransactionType.Receipt;
        }

        /// <summary>
        /// Gets or sets the Receipts.
        /// </summary>
        public List<ReceiptModel> Receipts { get; set; } = new List<ReceiptModel>();

        /// <summary>
        /// Gets or sets the Month.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Gets or sets the TotalAmount.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the Year.
        /// </summary>
        public int Year { get; set; }
    }
}
