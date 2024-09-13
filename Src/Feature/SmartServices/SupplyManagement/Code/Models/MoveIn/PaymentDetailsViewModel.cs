// <copyright file="PaymentDetailsViewModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

using DEWAXP.Foundation.Content.Models.Payment;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveIn
{
    public class PaymentDetailsViewModel
    {
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
        /// Gets or sets the TotalOutstandingAmt.
        /// </summary>
        public double TotalOutstandingAmt { get; set; }

        /// <summary>
        /// Gets the Total.
        /// </summary>
        public double Total => SecurityDeposit + ReconnectionRegistrationFee + AddressRegistrationFee + KnowledgeFee + TotalOutstandingAmt + InnovationFee + ReconnectionVATamt + AddressVAtamt;

        /// <summary>
        /// Gets or sets the TermsLink.
        /// </summary>
        public string TermsLink { get; set; }

        /// <summary>
        /// Gets or sets the NationalTermsLink.
        /// </summary>
        public bool NationalTermsLink { get; set; }

        /// <summary>
        /// Gets or sets the BusinessPartner.
        /// </summary>
        public string BusinessPartner { get; set; }

        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PayLater.
        /// </summary>
        public bool PayLater { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PayOther.
        /// </summary>
        public bool PayOther { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPaylaterSelected.
        /// </summary>
        public bool IsPaylaterSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsPayotherSelected.
        /// </summary>
        public bool IsPayotherSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DiscountApplied.
        /// </summary>
        public bool DiscountApplied { get; set; }

        /// <summary>
        /// Gets or sets the paymentMethod
        /// Gets or sets a value indicating whether paymentMethod..
        /// </summary>
        public PaymentMethod paymentMethod { get; set; }

        /// <summary>
        /// Gets or sets the PayChannelList.
        /// </summary>
        public IEnumerable<SelectListItem> PayChannelList { get; set; }

        /// <summary>
        /// Gets or sets the messagepaychannel.
        /// </summary>
        public string[] messagepaychannel { get; set; }

        public string bankkey { get;set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }

        public bool MaiDubaiContribution { get; set; }
        public bool MaiDubaiGift { get; set; }
        public string MaiDubaiTitle { get; set; }
        public string MaiDubaiMsg { get; set; }
    }
}
