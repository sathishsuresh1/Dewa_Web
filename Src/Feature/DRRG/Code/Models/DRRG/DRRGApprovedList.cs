// <copyright file="DRRGApprovedList.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="DRRGApprovedList" />
    /// </summary>
    public class DRRGApprovedList
    {
        /// <summary>
        /// Gets or sets the ManufacturerName
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Gets or sets the productlist
        /// </summary>
        public List<DRRGProduct> productlist { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="DRRGProduct" />
    /// </summary>
    public class DRRGProduct
    {
        /// <summary>
        /// Gets or sets the ProductName
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the ProductDescription
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the UsageCategory
        /// </summary>
        public string UsageCategory { get; set; }
    }
}
