// <copyright file="DisplayConsultantTrainings.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartConsultant
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    // DisplayConsultantTrainings myDeserializedClass = JsonConvert.DeserializeObject<DisplayConsultantTrainings>(myJsonResponse);
    /// <summary>
    /// Defines the <see cref="TrainingDetail" />.
    /// </summary>
    public class TrainingDetail
    {
        /// <summary>
        /// Defines the Delimitationdate.
        /// </summary>
        [JsonProperty("delimitationdate")]
        public string Delimitationdate;

        /// <summary>
        /// Defines the Enddate.
        /// </summary>
        [JsonProperty("enddate")]
        public string Enddate;

        /// <summary>
        /// Defines the Historicalrecord.
        /// </summary>
        [JsonProperty("historicalrecord")]
        public string Historicalrecord;

        /// <summary>
        /// Defines the Infotype.
        /// </summary>
        [JsonProperty("infotype")]
        public int Infotype;

        /// <summary>
        /// Defines the Language.
        /// </summary>
        [JsonProperty("language")]
        public string Language;

        /// <summary>
        /// Defines the Objectid.
        /// </summary>
        [JsonProperty("objectid")]
        public long Objectid;

        /// <summary>
        /// Defines the Objecttype.
        /// </summary>
        [JsonProperty("objecttype")]
        public string Objecttype;

        /// <summary>
        /// Defines the Planningstatus.
        /// </summary>
        [JsonProperty("planningstatus")]
        public int Planningstatus;

        /// <summary>
        /// Defines the Planversion.
        /// </summary>
        [JsonProperty("planversion")]
        public string Planversion;

        /// <summary>
        /// Defines the Priority.
        /// </summary>
        [JsonProperty("priority")]
        public string Priority;

        /// <summary>
        /// Defines the Reason.
        /// </summary>
        [JsonProperty("reason")]
        public string Reason;

        /// <summary>
        /// Defines the Sequencenumber.
        /// </summary>
        [JsonProperty("sequencenumber")]
        public string Sequencenumber;

        /// <summary>
        /// Defines the Shorttext.
        /// </summary>
        [JsonProperty("shorttext")]
        public string Shorttext;

        /// <summary>
        /// Defines the Shorttextname.
        /// </summary>
        [JsonProperty("shorttextname")]
        public string Shorttextname;

        /// <summary>
        /// Defines the Startdate.
        /// </summary>
        [JsonProperty("startdate")]
        public string Startdate;

        /// <summary>
        /// Defines the Subtype.
        /// </summary>
        [JsonProperty("subtype")]
        public string Subtype;

        /// <summary>
        /// Defines the Text.
        /// </summary>
        [JsonProperty("text")]
        public string Text;
    }

    /// <summary>
    /// Defines the <see cref="Return" />.
    /// </summary>
    public class DisplayConsultantTrainings
    {
        /// <summary>
        /// Defines the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description;

        /// <summary>
        /// Defines the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode;

        /// <summary>
        /// Defines the TrainingDetails.
        /// </summary>
        [JsonProperty("trainingDetails")]
        public List<TrainingDetail> TrainingDetails;
    }
}
