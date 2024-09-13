// <copyright file="EVChargerPayment.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Models.EVSmartCharger
{
    using DEWAXP.Foundation.Content.Models.Payment;

    /// <summary>
    /// Defines the <see cref="EVChargerPayment" />.
    /// </summary>
    public class EVChargerPayment
    {
        /// <summary>
        /// Gets or sets the paymentMethod.
        /// </summary>
        public PaymentMethod paymentMethod { get; set; }

        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}
