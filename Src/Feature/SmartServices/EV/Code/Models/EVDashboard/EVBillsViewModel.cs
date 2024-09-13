// <copyright file="EVBillsViewModel.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Models.EVDashboard
{
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using global::Sitecore.Globalization;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="EVBillsViewModel" />.
    /// </summary>
    public class EVBillsViewModel
    {
        /// <summary>
        /// Gets or sets the electricity.
        /// </summary>
        public decimal electricity { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        public decimal total { get; set; }

        /// <summary>
        /// Gets or sets the others.
        /// </summary>
        public decimal others { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ischargingdate.
        /// </summary>
        public bool ischargingdate { get; set; }

        /// <summary>
        /// Gets or sets the chargingdate.
        /// </summary>
        public string chargingdate { get; set; }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// The From.
        /// </summary>
        /// <param name="outstandingbreakdowns">The outstandingbreakdowns<see cref="List{Outstandingbreakdown}"/>.</param>
        /// <param name="accountnumber">The accountnumber<see cref="string"/>.</param>
        /// <returns>The <see cref="EVBillsViewModel"/>.</returns>
        public static EVBillsViewModel From(Outstandingbreakdown outstandingbreakdown)
        {
            EVBillsViewModel model = new EVBillsViewModel {
                account = outstandingbreakdown.contractaccount,
                electricity = outstandingbreakdown.electricity,
                others = outstandingbreakdown.others,
                total = outstandingbreakdown.total,
                ischargingdate = !string.IsNullOrWhiteSpace(outstandingbreakdown.discount),
                chargingdate = !string.IsNullOrWhiteSpace(outstandingbreakdown.discount)?
                string.Format(Translate.Text("EV_Dashboard_chargingdatetext"), DateTime.ParseExact(outstandingbreakdown.discount, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd MMM yyyy"))
                : string.Empty
            };

            return model;
        }
    }
}
