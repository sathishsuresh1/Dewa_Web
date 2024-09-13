// <copyright file="DisplayTraining.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.ShamsDubai.Models.ShamsDubaiTraining
{
    /// <summary>
    /// Defines the <see cref="DisplayTraining" />.
    /// </summary>
    public class DisplayTraining
    {
        /// <summary>
        /// Gets or sets the TrainingDate.
        /// </summary>
        public string TrainingDate { get; set; }

        /// <summary>
        /// Gets or sets the TrainingDescription.
        /// </summary>
        public string TrainingDescription { get; set; }

        /// <summary>
        /// Gets or sets the TrainingId.
        /// </summary>
        public long TrainingId { get; set; }

        /// <summary>
        /// Gets or sets the TrainingYear.
        /// </summary>
        public int TrainingYear { get; set; }
        public string TraningStartDate { get; set; }
        public string TraningEndDate { get; set; }
    }
}
