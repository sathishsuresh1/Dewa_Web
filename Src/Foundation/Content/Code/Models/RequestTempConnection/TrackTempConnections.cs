using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Foundation.Content.Models.RequestTempConnection
{
    public class TrackTempConnections
    {
        public TrackTempConnections(List<TrackTempConnectionRequestItem> items)
        {
            Items = items;
        }

        public List<TrackTempConnectionRequestItem> Items { get; private set; }
    }

    [Serializable]
    public class TrackTempConnectionRequestItem
    {
        public static TrackTempConnectionRequestItem From(TemporaryConnectionDetails details)
        {
            var model = new TrackTempConnectionRequestItem
            {
                EventType = details.EventType,
                Reference = details.NotificationNumber,
                Date = DateTime.ParseExact(details.DateTimeStamp.Trim(), "yyyyMMddHHmmss", null).ToString("dd MMM yyyy", SitecoreX.Culture),
                EventStartDate = FormatDateStamp(details.FromDate),
                EventEndDate = FormatDateStamp(details.ToDate),
                Amount = details.Amount,
                AccountNumber = details.ContractAccountNumber,
                Location = details.City,
                MobileNumber = details.MobileNumber,
                PaymentStatus = details.TaskStatus,
                PayDate = details.PayDate,
                TaxAmount = details.TaxAmount,
                NetAmount = details.NetAmount,
                NotificationStatus = details.NotificationStatus,
                PayButton = details.PayButton
            };
            
            return model;
        }

        private static string FormatDateStamp(string datestamp)
        {
            if (!string.IsNullOrWhiteSpace(datestamp))
            {
                var trimmed = datestamp.Replace("PM", string.Empty).Replace("AM", string.Empty).Replace("M", string.Empty).Trim();

                DateTime d;
                if (DateTime.TryParse(trimmed, out d))
                {
                    return d.ToString("dd MMM yyyy", SitecoreX.Culture);
                }
            }
            return string.Empty;
        }

        public string Reference { get; set; }

        public string Date { get; set; }

        public string EventType { get; set; }

        public string AccountNumber { get; set; }

        public string MobileNumber { get; set; }

        public string EventStartDate { get; set; }

        public string EventEndDate { get; set; }

        public string Location { get; set; }

        public decimal Amount { get; set; }

        public decimal NetAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public string PayButton { get; set; }

        public string PayDate { get; set; }

        //TODO: ???
        public string PaymentStatus { get; set; }

        public string NotificationStatus { get; set; }

        public string StatusClass
        {
            get
            {
                switch (NotificationStatus.ToUpper())
                {
                    case "OPEN":
                    case "IN PROCESS":
                        return "inprogress";

                    case "CLOSED":
                        return "closed";
                }
                return "closed";
            }
        }
    }
}