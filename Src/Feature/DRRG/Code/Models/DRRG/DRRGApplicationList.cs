// <copyright file="DRRGApplicationList.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="DRRGApplicationList" />
    /// </summary>
    public class DRRGApplicationList
    {
        /// <summary>
        /// Defines the PVModule
        /// </summary>
        public List<DRRGApplicationStatus> PVModule = new List<DRRGApplicationStatus>();

        /// <summary>
        /// Defines the InverterModule
        /// </summary>
        public List<DRRGApplicationStatus> InverterModule = new List<DRRGApplicationStatus>();

        /// <summary>
        /// Defines the IPModule
        /// </summary>
        public List<DRRGApplicationStatus> IPModule = new List<DRRGApplicationStatus>();
    }
}
