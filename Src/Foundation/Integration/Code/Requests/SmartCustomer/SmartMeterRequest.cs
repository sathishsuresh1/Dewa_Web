// <copyright file="SmartMeterRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartCustomer
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Smartmeterinputs" />.
    /// </summary>
    public class Smartmeterinputs
    {
        /// <summary>
        /// Gets or sets the Contractaccount.
        /// </summary>
        [JsonProperty("contractaccount")]
        public string contractaccount { get; set; }

        /// <summary>
        /// Gets or sets the Sessionid.
        /// </summary>
        [JsonProperty("sessionid")]
        public string sessionid { get; set; }

        /// <summary>
        /// Gets or sets the Lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang { get; set; }

        /// <summary>
        /// Gets or sets the Process.
        /// </summary>
        [JsonProperty("process")]
        public string process { get; set; }

        /// <summary>
        /// Gets or sets the Standardrequest.
        /// </summary>
        [JsonProperty("standardrequest")]
        public string standardrequest { get; set; }

        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        [JsonProperty("type")]
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the Voltagerequest.
        /// </summary>
        [JsonProperty("voltagerequest")]
        public string voltagerequest { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SmartMeterRequest" />.
    /// </summary>
    public class SmartMeterRequest
    {
        /// <summary>
        /// Gets or sets the Smartmeterinputs.
        /// </summary>
        [JsonProperty("smartmeterinputs")]
        public Smartmeterinputs smartmeterinputs { get; set; }
    }
}
