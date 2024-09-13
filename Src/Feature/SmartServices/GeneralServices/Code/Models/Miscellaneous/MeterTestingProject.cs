// <copyright file="MeterTestingProject.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

using System.Web;

namespace DEWAXP.Feature.GeneralServices.Models.Miscellaneous
{
    /// <summary>
    /// Defines the <see cref="MeterTestingProject" />
    /// </summary>
    public class MeterTestingProject : BaseMiscellanous
    {
        /// <summary>
        /// Gets or sets the SubstationNumber
        /// </summary>
        public string SubstationNumber { get; set; }

        /// <summary>
        /// Gets or sets the PurchaseOrderNumber
        /// </summary>
        public string PurchaseOrderNumber { get; set; }
    }
}
