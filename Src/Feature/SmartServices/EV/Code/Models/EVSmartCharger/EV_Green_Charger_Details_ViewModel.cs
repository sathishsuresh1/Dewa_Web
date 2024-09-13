// <copyright file="EV_Green_Charger_Details_Param.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Models.EVSmartCharger
{
    /// <summary>
    /// Defines the <see cref="EV_Green_Charger_Details_ViewModel" />.
    /// </summary>
    public class EV_Green_Charger_Details_ViewModel
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
        /// Gets or sets the ChargerType.
        /// </summary>
        public string ChargerType { get; set; }

        /// <summary>
        /// Gets or sets the Connector.
        /// </summary>
        public string Connector { get; set; }

        /// <summary>
        /// Gets or sets the ConnectorStatus.
        /// </summary>
        public string ConnectorStatus { get; set; }

        /// <summary>
        /// Gets or sets the ConnectorIcon.
        /// </summary>
        public string ConnectorIcon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ConnectorAvailable.
        /// </summary>
        public bool ConnectorAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Status.
        /// </summary>
        public bool Status { get; set; }
    }
}
