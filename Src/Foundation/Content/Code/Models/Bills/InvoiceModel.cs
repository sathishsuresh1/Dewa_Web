using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
using System;

namespace DEWAXP.Foundation.Content.Models.Bills
{
    public class InvoiceModel : TransactionModel
    {
        public InvoiceModel()
        {
            Type = TransactionType.Invoice;
        }

        public decimal adjamount;

        public string billingmonth;

        public string collectiveaccount;

        public string consumernumber;

        public string curencykey;

        public decimal currentamount;

        public decimal dewaamount;

        public decimal dmamount;

        public decimal discountamount;

        public decimal electricityamount;

        public string finalbillflag;

        public string myear;

        public decimal nakamount;

        public decimal netamount;

        public decimal otheramount;

        public decimal paidamount;

        public decimal previousamount;

        public decimal wateramount;

        //public static InvoiceModel From(Invoicelist invoice)
        //{
        //    return new InvoiceModel
        //    {
        //        Amount = !string.IsNullOrWhiteSpace(invoice.netamount) ? decimal.Parse(invoice.netamount) : decimal.Parse("0.00"),
        //        DegTransactionReference = invoice.invoiceno,
        //        Type = TransactionType.Invoice,
        //        Date = DateTime.Parse(invoice.invoicedate),
        //        billingmonth = invoice.billingmonth,
        //        collectiveaccount = invoice.collectiveaccount,
        //        consumernumber = invoice.consumernumber,
        //        curencykey = invoice.curencykey,
        //        finalbillflag = invoice.finalbillflag,
        //        myear = invoice.myear,
        //        adjamount = !string.IsNullOrWhiteSpace(invoice.adjamount) ? decimal.Parse(invoice.adjamount) : decimal.Parse("0.00"),
        //        dewaamount = !string.IsNullOrWhiteSpace(invoice.dewaamount) ? decimal.Parse(invoice.dewaamount) : decimal.Parse("0.00"),
        //        currentamount = !string.IsNullOrWhiteSpace(invoice.currentamount) ? decimal.Parse(invoice.currentamount) : decimal.Parse("0.00"),
        //        dmamount = !string.IsNullOrWhiteSpace(invoice.dmamount) ? decimal.Parse(invoice.dmamount) : decimal.Parse("0.00"),
        //        electricityamount = !string.IsNullOrWhiteSpace(invoice.electricityamount) ? decimal.Parse(invoice.electricityamount) : decimal.Parse("0.00"),
        //        nakamount = !string.IsNullOrWhiteSpace(invoice.nakamount) ? decimal.Parse(invoice.nakamount) : decimal.Parse("0.00"),
        //        otheramount = !string.IsNullOrWhiteSpace(invoice.otheramount) ? decimal.Parse(invoice.otheramount) : decimal.Parse("0.00"),
        //        paidamount = !string.IsNullOrWhiteSpace(invoice.paidamount) ? decimal.Parse(invoice.paidamount) : decimal.Parse("0.00"),
        //        previousamount = !string.IsNullOrWhiteSpace(invoice.previousamount) ? decimal.Parse(invoice.previousamount) : decimal.Parse("0.00"),
        //        wateramount = !string.IsNullOrWhiteSpace(invoice.wateramount) ? decimal.Parse(invoice.wateramount) : decimal.Parse("0.00"),
        //    };
        //}

        public static InvoiceModel FromAPI(Invoicelist invoice)
        {
            return new InvoiceModel
            {
                Amount = !string.IsNullOrWhiteSpace(invoice.Netamount) ? decimal.Parse(invoice.Netamount) : decimal.Parse("0.00"),
                DegTransactionReference = invoice.Invoiceno,
                Type = TransactionType.Invoice,
                Date = DateTime.Parse(invoice.Invoicedate),
                billingmonth = invoice.Billingmonth,
                collectiveaccount = invoice.Collectiveaccount,
                consumernumber = invoice.Consumernumber,
                curencykey = invoice.Curencykey,
                finalbillflag = invoice.Finalbillflag,
                myear = invoice.Myear,
                adjamount = !string.IsNullOrWhiteSpace(invoice.Adjamount) ? decimal.Parse(invoice.Adjamount) : decimal.Parse("0.00"),
                dewaamount = !string.IsNullOrWhiteSpace(invoice.Dewaamount) ? decimal.Parse(invoice.Dewaamount) : decimal.Parse("0.00"),
                currentamount = !string.IsNullOrWhiteSpace(invoice.Currentamount) ? decimal.Parse(invoice.Currentamount) : decimal.Parse("0.00"),
                dmamount = !string.IsNullOrWhiteSpace(invoice.Dmamount) ? decimal.Parse(invoice.Dmamount) : decimal.Parse("0.00"),
                electricityamount = !string.IsNullOrWhiteSpace(invoice.Electricityamount) ? decimal.Parse(invoice.Electricityamount) : decimal.Parse("0.00"),
                nakamount = !string.IsNullOrWhiteSpace(invoice.Nakamount) ? decimal.Parse(invoice.Nakamount) : decimal.Parse("0.00"),
                otheramount = !string.IsNullOrWhiteSpace(invoice.Otheramount) ? decimal.Parse(invoice.Otheramount) : decimal.Parse("0.00"),
                paidamount = !string.IsNullOrWhiteSpace(invoice.Paidamount) ? decimal.Parse(invoice.Paidamount) : decimal.Parse("0.00"),
                previousamount = !string.IsNullOrWhiteSpace(invoice.Previousamount) ? decimal.Parse(invoice.Previousamount) : decimal.Parse("0.00"),
                wateramount = !string.IsNullOrWhiteSpace(invoice.Wateramount) ? decimal.Parse(invoice.Wateramount) : decimal.Parse("0.00"),
                discountamount = !string.IsNullOrWhiteSpace(invoice.Discountamount) ? decimal.Parse(invoice.Discountamount) : decimal.Parse("0.00"),
            };
        }
    }
}