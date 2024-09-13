// <copyright file="BillHistoryRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Invoicelist
    {
        [JsonProperty("adjamount")]
        public string Adjamount { get; set; }

        [JsonProperty("billingmonth")]
        public string Billingmonth { get; set; }

        [JsonProperty("collectiveaccount")]
        public string Collectiveaccount { get; set; }

        [JsonProperty("consumernumber")]
        public string Consumernumber { get; set; }

        [JsonProperty("curencykey")]
        public string Curencykey { get; set; }

        [JsonProperty("currentamount")]
        public string Currentamount { get; set; }

        [JsonProperty("dewaamount")]
        public string Dewaamount { get; set; }

        [JsonProperty("discountamount")]
        public string Discountamount { get; set; }

        [JsonProperty("dmamount")]
        public string Dmamount { get; set; }

        [JsonProperty("electricityamount")]
        public string Electricityamount { get; set; }

        [JsonProperty("finalbillflag")]
        public string Finalbillflag { get; set; }

        [JsonProperty("invoicedate")]
        public string Invoicedate { get; set; }

        [JsonProperty("invoiceno")]
        public string Invoiceno { get; set; }

        [JsonProperty("myear")]
        public string Myear { get; set; }

        [JsonProperty("nakamount")]
        public string Nakamount { get; set; }

        [JsonProperty("netamount")]
        public string Netamount { get; set; }

        [JsonProperty("otheramount")]
        public string Otheramount { get; set; }

        [JsonProperty("paidamount")]
        public string Paidamount { get; set; }

        [JsonProperty("previousamount")]
        public string Previousamount { get; set; }

        [JsonProperty("wateramount")]
        public string Wateramount { get; set; }
    }

    public class Paymentlist
    {
        [JsonProperty("accountnumber")]
        public string Accountnumber { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("applicationnumber")]
        public string Applicationnumber { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("chequereturn")]
        public string Chequereturn { get; set; }

        [JsonProperty("currencykey")]
        public string Currencykey { get; set; }

        [JsonProperty("documentnumber")]
        public string Documentnumber { get; set; }

        [JsonProperty("externalTransactionID")]
        public string ExternalTransactionID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("noreceipt")]
        public string Noreceipt { get; set; }

        [JsonProperty("paymentdescription")]
        public string Paymentdescription { get; set; }

        [JsonProperty("paymentmode")]
        public string Paymentmode { get; set; }

        [JsonProperty("paymenttype")]
        public string Paymenttype { get; set; }

        [JsonProperty("taxinvoiceapplicability")]
        public string Taxinvoiceapplicability { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("transactionid")]
        public string Transactionid { get; set; }
    }

    public class BillHistoryResponse
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("invoicelist")]
        public List<Invoicelist> invoicelist { get; set; }

        [JsonProperty("lmComparison")]
        public string LmComparison { get; set; }

        [JsonProperty("lmComparisonText")]
        public string LmComparisonText { get; set; }

        [JsonProperty("lyComparison")]
        public string LyComparison { get; set; }

        [JsonProperty("lyComparisonText")]
        public string LyComparisonText { get; set; }

        [JsonProperty("paymentlist")]
        public List<Paymentlist> paymentlist { get; set; }

        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }
    }


}
