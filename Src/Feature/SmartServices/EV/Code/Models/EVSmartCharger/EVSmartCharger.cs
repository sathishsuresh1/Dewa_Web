// <copyright file="EVSmartCharger.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Models.EVSmartCharger
{
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVSmartCharger;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// Defines the <see cref="EVSmartCharger" />.
    /// </summary>
    public class EVSmartCharger
    {
        /// <summary>
        /// Gets or sets the ChargerLocation.
        /// </summary>
        public string ChargerLocation { get; set; }

        /// <summary>
        /// Gets or sets the ChargerID.
        /// </summary>
        public string ChargerID { get; set; }

        /// <summary>
        /// Gets or sets the DeviceID.
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// Gets or sets the ChargerType.
        /// </summary>
        public string ChargerType { get; set; }

        /// <summary>
        /// Gets or sets the Connector.
        /// </summary>
        public string Connector { get; set; }

        /// <summary>
        /// Gets or sets the ConnectorNumber.
        /// </summary>
        public string ConnectorNumber { get; set; }

        /// <summary>
        /// Gets or sets the ConnectorStatus.
        /// </summary>
        public string ConnectorStatus { get; set; }

        /// <summary>
        /// Gets or sets the FullName.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the CountryCallingCodesList.
        /// </summary>
        public List<SelectListItem> CountryCallingCodesList { get; set; }

        /// <summary>
        /// Gets or sets the CountryCode.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the VatNumber.
        /// </summary>
        public string VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the MobileNumber.
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the PlateNumber.
        /// </summary>
        public string PlateNumber { get; set; }

        /// <summary>
        /// Gets or sets the Requestid.
        /// </summary>
        public string Requestid { get; set; }

        /// <summary>
        /// Gets or sets the Duration.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Gets or sets the Amount.
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Gets or sets the Currency.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PaymentProcessed.
        /// </summary>
        public bool PaymentProcessed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Maxotp.
        /// </summary>
        public bool Maxotp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether VerifiedEmail.
        /// </summary>
        public bool VerifiedEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RequestSubmitted.
        /// </summary>
        public bool RequestSubmitted { get; set; }

        /// <summary>
        /// Gets or sets the Otcpacks.
        /// </summary>
        public List<Otcpack> Otcpacks { get; set; }

        /// <summary>
        /// Gets or sets the Otcpayment.
        /// </summary>
        public List<Otcpayment> Otcpayment { get; set; }

        /// <summary>
        /// Gets or sets the EVChargerStatus.
        /// </summary>
        public string EVChargerStatus { get; set; }

        /// <summary>
        /// Gets or sets the Timer.
        /// </summary>
        public string Timer { get; set; }

        /// <summary>
        /// Gets or sets the PaymentDate.
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the Durationcount.
        /// </summary>
        public int Durationcount { get; set; }
    }
}
