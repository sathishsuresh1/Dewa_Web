// <copyright file="EV_Green_Charger_Details_Param.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Models.EVSmartCharger
{
    using DEWAXP.Foundation.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="EV_Green_Charger_Details_Param" />.
    /// </summary>
    public class EV_Green_Charger_Details_Param
    {
        /// <summary>
        /// Gets or sets the evqr.
        /// </summary>
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public string evqr { get; set; }

        /// <summary>
        /// Gets or sets the cc.
        /// </summary>
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        public int cc { get; set; }
    }
}
