// <copyright file="OfferBaseResponse.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Responses.DewaStoreSvc
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="OfferBaseResponse" />.
    /// </summary>
    public class OfferBaseResponse
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [JsonProperty("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the ResponseCode.
        /// </summary>
        [JsonProperty("ResponseCode")]
        public virtual int ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the Data.
        /// </summary>
        [JsonProperty("Data")]
        public virtual IList<OfferData> Data { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="OfferData" />.
    /// </summary>
    public class OfferData
    {
        /// <summary>
        /// Gets or sets the Row_Id.
        /// </summary>
        [JsonProperty("Row_Id")]
        public string Row_Id { get; set; }

        /// <summary>
        /// Gets or sets the OfferName.
        /// </summary>
        [JsonProperty("OfferName")]
        public string OfferName { get; set; }

        /// <summary>
        /// Gets or sets the OfferDetails.
        /// </summary>
        [JsonProperty("OfferDetails")]
        public string OfferDetails { get; set; }

        /// <summary>
        /// Gets or sets the OfferBenefits.
        /// </summary>
        [JsonProperty("OfferBenefits")]
        public string OfferBenefits { get; set; }

        /// <summary>
        /// Gets or sets the DiscountPercentage.
        /// </summary>
        [JsonProperty("DiscountPercentage")]
        public virtual double DiscountPercentage { get; set; }

        /// <summary>
        /// Gets or sets the ThumbnailUrl.
        /// </summary>
        [JsonProperty("ThumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the LogoUrl.
        /// </summary>
        [JsonProperty("LogoUrl")]
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the CompanyName.
        /// </summary>
        [JsonProperty("CompanyName")]
        public string CompanyName { get; set; }

        [JsonProperty("ImageJsonValue")]
        public string ImageJsonValue { get; set; }
    }
    public class ImageJsonValue
    {
        [JsonProperty("OfferImages")]
        public virtual IList<string> OfferImages { get; set; }
    }

}
