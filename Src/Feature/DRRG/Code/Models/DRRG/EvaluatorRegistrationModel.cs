// <copyright file="EvaluatorRegistrationModel.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

using System.Web;

namespace DEWAXP.Feature.DRRG.Models
{
    /// <summary>
    /// Defines the <see cref="EvaluatorRegistrationModel" />.
    /// </summary>
    public class EvaluatorRegistrationModel
    {
        /// <summary>
        /// Gets or sets the Username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }
    }

    public class RejectedViewModel
    {
        public string regnumber { get; set; }
        public string modelname { get; set; }
        public string rejectionreason { get; set; }
        public HttpPostedFileBase file { get; set; }
    }
}
