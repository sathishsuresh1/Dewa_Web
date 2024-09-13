// <copyright file="ReraPaymentModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Models.Payment
{
    using DEWAXP.Foundation.Content.Models.RERA;
    using System;

    /// <summary>
    /// Defines the <see cref="ReraPaymentModel" />.
    /// </summary>
    [Serializable]
    public class ReraPaymentModel : ReraCustomerDetails
    {
        /// <summary>
        /// Gets or sets the ReceiptId.
        /// </summary>
        public string ReceiptId { get; set; }

        /// <summary>
        /// Gets or sets the DegTransactionId.
        /// </summary>
        public string DegTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the DewaTransactionId.
        /// </summary>
        public string DewaTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the Amount.
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Gets or sets the SecurityDeposit.
        /// </summary>
        public double SecurityDeposit { get; set; }

        /// <summary>
        /// Gets or sets the ReconnectionRegistrationFee.
        /// </summary>
        public double ReconnectionRegistrationFee { get; set; }

        /// <summary>
        /// Gets or sets the AddressRegistrationFee.
        /// </summary>
        public double AddressRegistrationFee { get; set; }

        /// <summary>
        /// Gets or sets the ReconnectionVATrate.
        /// </summary>
        public string ReconnectionVATrate { get; set; }

        /// <summary>
        /// Gets or sets the ReconnectionVATamt.
        /// </summary>
        public double ReconnectionVATamt { get; set; }

        /// <summary>
        /// Gets or sets the AddressVATrate.
        /// </summary>
        public string AddressVATrate { get; set; }

        /// <summary>
        /// Gets or sets the AddressVAtamt.
        /// </summary>
        public double AddressVAtamt { get; set; }

        /// <summary>
        /// Gets or sets the KnowledgeFee.
        /// </summary>
        public double KnowledgeFee { get; set; }

        /// <summary>
        /// Gets or sets the InnovationFee.
        /// </summary>
        public double InnovationFee { get; set; }

        /// <summary>
        /// Gets the Total.
        /// </summary>
        public double Total => SecurityDeposit + ReconnectionRegistrationFee + AddressRegistrationFee + KnowledgeFee + InnovationFee + ReconnectionVATamt + AddressVAtamt;

        /// <summary>
        /// Gets or sets the TermsLink.
        /// </summary>
        public string TermsLink { get; set; }

        /// <summary>
        /// Gets or sets the BusinessPartner.
        /// </summary>
        public string BusinessPartner { get; set; }

        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the messagewhatsnext.
        /// </summary>
        public string[] messagewhatsnext { get; set; }

        /// <summary>
        /// Gets or sets the messagepaychannel.
        /// </summary>
        public string[] messagepaychannel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PayLater.
        /// </summary>
        public bool PayLater { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PayOther.
        /// </summary>
        public bool PayOther { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPayLaterClicked.
        /// </summary>
        public bool IsPayLaterClicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPayotherSelected.
        /// </summary>
        public bool IsPayotherSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DiscountApplied.
        /// </summary>
        public bool DiscountApplied { get; set; }

        /// <summary>
        /// Gets or sets the MoveinNotificationnumber.
        /// </summary>
        public string MoveinNotificationnumber { get; set; }
        public bool Easypayflag { get; set; }
        public bool payotherchannelflag { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
        public bool MaiDubaiContribution { get; set; }
        public bool MaiDubaiGift { get; set; }
        public string MaiDubaiTitle { get; set; }
        public string MaiDubaiMsg { get; set; }
    }
}
