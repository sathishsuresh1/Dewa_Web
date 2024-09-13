// <copyright file="BaseMiscellanous.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GeneralServices.Models.Miscellaneous
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using DEWAXP.Foundation.DataAnnotations;
    using DEWAXP.Foundation.Integration.DewaSvc;

    /// <summary>
    /// Defines the <see cref="BaseMiscellanous" />
    /// </summary>
    public class BaseMiscellanous
    {
        public IEnumerable<SelectListItem> BusinessPartners { get; set; }
        public string BusinessPartner { get; set; }
        public string BusinessPartnerDisplay { get; set; }
        /// <summary>
        /// Gets or sets the Materials
        /// </summary>
        public IEnumerable<codeGroupsList> Materials { get; set; }

        /// <summary>
        /// Gets or sets the selectedMaterial
        /// </summary>
        public string selectedMaterial { get; set; }

        /// <summary>
        /// Gets or sets the selectedMaterialJSON
        /// </summary>
        public string selectedMaterialJSON { get; set; }

        /// <summary>
        /// Gets or sets the Attachment1
        /// </summary>
        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase Attachment1 { get; set; }

        
        /// <summary>
        /// Gets or sets the Remarks
        /// </summary>
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
        [MinLength(0, ValidationMessageKey = "min length validation message")]
        [MaxLength(500, ValidationMessageKey = "max length validation message")]
        public string Remarks { get; set; }
    }
}
