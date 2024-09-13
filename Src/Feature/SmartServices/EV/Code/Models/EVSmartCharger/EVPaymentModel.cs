// <copyright file="EVPaymentModel.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Models.EVSmartCharger
{
    using DEWAXP.Foundation.Content.Models.Payment;
    using System;

    /// <summary>
    /// Defines the <see cref="EVPaymentModel" />.
    /// </summary>
    public class EVPaymentModel
    {
        /// <summary>
        /// Gets or sets the Context
        /// Gets the Context..
        /// </summary>
        public PaymentContext Context { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Succeeded
        /// Gets a value indicating whether Succeeded..
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the ReceiptIdentifiers.
        /// </summary>
        public string ReceiptIdentifiers { get; set; }

        /// <summary>
        /// Gets or sets the Total.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Gets or sets the PaymentDate.
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the DegTransactionId.
        /// </summary>
        public string DegTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the DewaTransactionId.
        /// </summary>
        public string DewaTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the Duration.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Gets or sets the Amount.
        /// </summary>
        public string Amount { get; set; }

        public decimal SuqiaAmount { get; set; }

        /// <summary>
        /// Gets or sets the Currency.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the Timer.
        /// </summary>
        public string Timer { get; set; }
    }
}
