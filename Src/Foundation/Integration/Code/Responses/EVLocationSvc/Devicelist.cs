// <copyright file="Devicelist.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Responses.EVLocationSvc
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Devicelist" />.
    /// </summary>
    public partial class Devicelist
    {
        /// <summary>
        /// Gets or sets the Devices.
        /// </summary>
        [JsonProperty("devices")]
        public Device[] Devices { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Connector" />.
    /// </summary>
    public class Connector
    {
        /// <summary>
        /// Gets or sets the ConnectorStatus.
        /// </summary>
        [JsonProperty("ConnectorStatus")]
        public string ConnectorStatus { get; set; }

        /// <summary>
        /// Gets or sets the LocalConnectorNumber.
        /// </summary>
        [JsonProperty("LocalConnectorNumber")]
        public int LocalConnectorNumber { get; set; }

        /// <summary>
        /// Gets or sets the ConnectorName.
        /// </summary>
        [JsonProperty("ConnectorName")]
        public string ConnectorName { get; set; }

        /// <summary>
        /// Gets or sets the ConnectorType.
        /// </summary>
        [JsonProperty("ConnectorType")]
        public string ConnectorType { get; set; }

        /// <summary>
        /// Gets or sets the ConnectorDisplay.
        /// </summary>
        [JsonProperty("ConnectorDisplay")]
        public string ConnectorDisplay { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Device" />.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Gets or sets the DeviceDBID.
        /// </summary>
        [JsonProperty("DeviceDBID")]
        public string DeviceDBID { get; set; }

        /// <summary>
        /// Gets or sets the HubeleonID.
        /// </summary>
        [JsonProperty("HubeleonID")]
        public string HubeleonID { get; set; }

        /// <summary>
        /// Gets or sets the LocationName.
        /// </summary>
        [JsonProperty("LocationName")]
        public string LocationName { get; set; }

        /// <summary>
        /// Gets or sets the LocationAddress.
        /// </summary>
        [JsonProperty("LocationAddress")]
        public string LocationAddress { get; set; }

        /// <summary>
        /// Gets or sets the LocationPostcode.
        /// </summary>
        [JsonProperty("LocationPostcode")]
        public string LocationPostcode { get; set; }

        /// <summary>
        /// Gets or sets the Longitude.
        /// </summary>
        [JsonProperty("Longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// Gets or sets the Latitude.
        /// </summary>
        [JsonProperty("Latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// Gets or sets the DeviceStatus.
        /// </summary>
        [JsonProperty("DeviceStatus")]
        public string DeviceStatus { get; set; }

        /// <summary>
        /// Gets or sets the DeviceLastMessage.
        /// </summary>
        [JsonProperty("DeviceLastMessage")]
        public string DeviceLastMessage { get; set; }

        /// <summary>
        /// Gets or sets the InstallationDate.
        /// </summary>
        [JsonProperty("InstallationDate")]
        public string InstallationDate { get; set; }

        /// <summary>
        /// Gets or sets the TotalNbOfConnectors.
        /// </summary>
        [JsonProperty("TotalNbOfConnectors")]
        public int TotalNbOfConnectors { get; set; }

        /// <summary>
        /// Gets or sets the Connectors.
        /// </summary>
        [JsonProperty("Connectors")]
        public List<Connector> Connectors { get; set; }
    }
}
