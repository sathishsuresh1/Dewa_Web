// <copyright file="ServiceActivationPaymentCompletionModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Models.Payment
{
    using DEWAXP.Foundation.Integration.DewaSvc;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="ServiceActivationPaymentCompletionModel" />.
    /// </summary>
    public class ServiceActivationPaymentCompletionModel : PaymentCompletionModel
    {
        /// <summary>
        /// Gets or sets the PaymentAmount.
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// Gets or sets the PremiseNo.
        /// </summary>
        public string PremiseNo { get; set; }

        /// <summary>
        /// Gets or sets the FullName.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the premiseAmountDetails.
        /// </summary>
        public List<premiseAmountDetails> premiseAmountDetails { get; set; }

        /// <summary>
        /// Gets or sets the BusinessPartner.
        /// </summary>
        public string BusinessPartner { get; set; }

        /// <summary>
        /// Gets or sets the MoveInNotificationNumber.
        /// </summary>
        public string MoveInNotificationNumber { get; set; }

        /// <summary>
        /// Gets or sets the Whatsnexttext.
        /// </summary>
        public string[] Whatsnexttext { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether landloardmsg.
        /// </summary>
        public bool landloardmsg { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceActivationPaymentCompletionModel"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="PaymentContext"/>.</param>
        /// <param name="succeeded">The succeeded<see cref="bool"/>.</param>
        public ServiceActivationPaymentCompletionModel(PaymentContext context, bool succeeded)
            : base(context, succeeded)
        {
        }
    }
}
