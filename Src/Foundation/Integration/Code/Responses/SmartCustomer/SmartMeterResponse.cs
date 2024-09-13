// <copyright file="SmartMeterResponse.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartCustomer
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Standardrequest" />.
    /// </summary>
    public class Standardrequest
    {
        /// <summary>
        /// Gets or sets the Equipmentno.
        /// </summary>
        [JsonProperty("equipmentno")]
        public string Equipmentno { get; set; }

        /// <summary>
        /// Gets or sets the Opstateid.
        /// </summary>
        [JsonProperty("opstateid")]
        public string Opstateid { get; set; }

        /// <summary>
        /// Gets or sets the Remarks.
        /// </summary>
        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the Requestdate.
        /// </summary>
        [JsonProperty("requestdate")]
        public string Requestdate { get; set; }

        /// <summary>
        /// Gets or sets the Requestsetflag.
        /// </summary>
        [JsonProperty("requestsetflag")]
        public string Requestsetflag { get; set; }

        /// <summary>
        /// Gets or sets the Requesttime.
        /// </summary>
        [JsonProperty("requesttime")]
        public string Requesttime { get; set; }

        /// <summary>
        /// Gets or sets the Responsedate.
        /// </summary>
        [JsonProperty("responsedate")]
        public string Responsedate { get; set; }

        /// <summary>
        /// Gets or sets the Responsetime.
        /// </summary>
        [JsonProperty("responsetime")]
        public string Responsetime { get; set; }

        /// <summary>
        /// Gets or sets the Sourceflag.
        /// </summary>
        [JsonProperty("sourceflag")]
        public string Sourceflag { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Voltagerequest" />.
    /// </summary>
    public class Voltagerequest
    {
        /// <summary>
        /// Gets or sets the Contractaccount.
        /// </summary>
        [JsonProperty("contractaccount")]
        public string Contractaccount { get; set; }

        /// <summary>
        /// Gets or sets the Device.
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; set; }

        /// <summary>
        /// Gets or sets the Equipmentno.
        /// </summary>
        [JsonProperty("equipmentno")]
        public string Equipmentno { get; set; }

        /// <summary>
        /// Gets or sets the MdmMsgid.
        /// </summary>
        [JsonProperty("mdm_msgid")]
        public string MdmMsgid { get; set; }

        /// <summary>
        /// Gets or sets the PoReqid.
        /// </summary>
        [JsonProperty("po_reqid")]
        public string PoReqid { get; set; }

        /// <summary>
        /// Gets or sets the PoResid.
        /// </summary>
        [JsonProperty("po_resid")]
        public string PoResid { get; set; }

        /// <summary>
        /// Gets or sets the Remarks.
        /// </summary>
        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the Requestno.
        /// </summary>
        [JsonProperty("requestno")]
        public string Requestno { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the VoltageG.
        /// </summary>
        [JsonProperty("voltage_g")]
        public string VoltageG { get; set; }

        /// <summary>
        /// Gets or sets the VoltageR.
        /// </summary>
        [JsonProperty("voltage_r")]
        public string VoltageR { get; set; }

        /// <summary>
        /// Gets or sets the VoltageY.
        /// </summary>
        [JsonProperty("voltage_y")]
        public string VoltageY { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SmartMeterResponse" />.
    /// </summary>
    public class SmartMeterResponse
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Device.
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; set; }

        /// <summary>
        /// Gets or sets the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }

        /// <summary>
        /// Gets or sets the Retry.
        /// </summary>
        [JsonProperty("retry")]
        public string Retry { get; set; }

        /// <summary>
        /// Gets or sets the Standardrequest.
        /// </summary>
        [JsonProperty("standardrequest")]
        public Standardrequest Standardrequest { get; set; }

        /// <summary>
        /// Gets or sets the Voltagerequest.
        /// </summary>
        [JsonProperty("voltagerequest")]
        public Voltagerequest Voltagerequest { get; set; }
    }
}
