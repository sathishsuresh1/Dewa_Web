// <copyright file="OfferDataViewModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Dashboard.Models.DewaStore
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="DewaStoreOfferViewModel" />.
    /// </summary>
    public class DewaStoreOfferViewModel
    {
        /// <summary>
        /// Gets or sets the BackgroundImage.
        /// </summary>
        public string BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the ModuleTitle.
        /// </summary>
        public string ModuleTitle { get; set; }

        /// <summary>
        /// Gets or sets the ModuleLinkUrl.
        /// </summary>
        public string ModuleLinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the ModuleLinkdesc.
        /// </summary>
        public string ModuleLinkdesc { get; set; }

        /// <summary>
        /// Gets or sets the offerDataViewModels.
        /// </summary>
        public List<OfferDataViewModel> offerDataViewModels { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="OfferDataViewModel" />.
    /// </summary>
    public class OfferDataViewModel
    {
        /// <summary>
        /// Gets or sets the Row_Id.
        /// </summary>
        public string Row_Id { get; set; }

        /// <summary>
        /// Gets or sets the OfferName.
        /// </summary>
        public string OfferName { get; set; }

        /// <summary>
        /// Gets or sets the OfferDetails.
        /// </summary>
        public string OfferDetails { get; set; }

        /// <summary>
        /// Gets or sets the OfferBenefits.
        /// </summary>
        public string OfferBenefits { get; set; }

        /// <summary>
        /// Gets or sets the DiscountPercentage.
        /// </summary>
        public double DiscountPercentage { get; set; }

        /// <summary>
        /// Gets or sets the ThumbnailUrl.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the Imagevalue.
        /// </summary>
        public string Imagevalue { get; set; }

        /// <summary>
        /// Gets or sets the LogoUrl.
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the CompanyName.
        /// </summary>
        public string CompanyName { get; set; }
    }
}
