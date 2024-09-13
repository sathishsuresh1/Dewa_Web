// <copyright file="DRRGIPApproveViewModel.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="DRRGIPApproveViewModel" />
    /// </summary>
    public class DRRGIPApproveViewModel
    {
        /// <summary>
        /// Gets or sets the manufacturerDetails
        /// </summary>
        public ManufacturerRegistration manufacturerDetails { get; set; }

        /// <summary>
        /// Defines the lstIPModule
        /// </summary>
        public List<DRRGInterfaceProtectionModule> lstIPModule = new List<DRRGInterfaceProtectionModule>();

        /// <summary>
        /// Gets or sets a value indicating whether ApproveorReject
        /// </summary>
        public bool ApproveorReject { get; set; }

        /// <summary>
        /// Gets or sets the RejectRemark
        /// </summary>
        public string RejectRemark { get; set; }
    }
}
