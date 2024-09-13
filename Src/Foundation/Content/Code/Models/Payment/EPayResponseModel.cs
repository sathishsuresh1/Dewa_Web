using DEWAXP.Foundation.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DEWAXP.Foundation.Content.Models.Payment
{
    public class EPayResponseModel
    {
        /// <summary>
        /// Indicates whether payment succeeded or failed
        /// </summary>
        public EPayResponse s { get; set; }

        /// <summary>
        /// Gets or sets the DEWA transaction reference.
        /// If payment failed, the value will be null.
        /// </summary>
        public string t { get; set; }

        /// <summary>
        /// Gets or sets the DEG transaction reference.
        /// If payment failed, the value will be null.
        /// </summary>
        public string g { get; set; }

        /// <summary>
        /// Gets or sets the payment timestamp
        /// </summary>
        public string m { get; set; }

        /// <summary>
        /// Retrieves the payment context code (e.g. m1b1; m1b2)
        /// </summary>
        public string ptype { get; set; }

        public string epnum { get; set; }
        public string epf { get; set; }

        public DateTime PaymentDate
        {
            get
            {
                var formats = new List<string>
                {
                    "dd/MM/yyyy HH:mm:ss",
                    "dd/MM/yyyy H:mm:ss",
                    "d/MM/yyyy HH:mm:ss",
                    "d/MM/yyyy H:mm:ss"
                };

                foreach (string formatter in formats)
                {
                    DateTime @return;
                    if (DateTime.TryParseExact(this.m, formatter, CultureInfo.InvariantCulture, DateTimeStyles.None, out @return))
                    {
                        return @return;
                    }
                }

                return DateHelper.DubaiNow();
            }
        }

        /// <summary>
        /// dewatoken - Encrypted token response
        /// </summary>
        public string dewatoken { get; set; }

        public string pgspending { get; set; }
    }

    public enum EPayResponse
    {
        Failure,
        Success
    }
}