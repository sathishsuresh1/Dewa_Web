// <copyright file="M78Timer.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    /// <summary>
    /// Defines the <see cref="M78Timer" />
    /// </summary>
    [SitecoreType(TemplateId = "{6CF0DF12-FF08-4B3C-B349-43EF145BEC52}", AutoMap = true)]
    public class M78Timer : GlassBase
    {
        /// <summary>
        /// Gets or sets the BackgroundImage
        /// </summary>
        [SitecoreField("Background Image")]
        public virtual Image BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the StartDate
        /// </summary>
        [SitecoreField("Start Date")]
        public virtual string StartDate { get; set; }

        /// <summary>
        /// Gets or sets the EndDate
        /// </summary>
        [SitecoreField("End Date")]
        public virtual string EndDate { get; set; }

        /// <summary>
        /// Gets or sets the SecondsColor
        /// </summary>
        [SitecoreField("Seconds Color")]
        public virtual string SecondsColor { get; set; }

        /// <summary>
        /// Gets or sets the MinutesColor
        /// </summary>
        [SitecoreField("Minutes Color")]
        public virtual string MinutesColor { get; set; }

        /// <summary>
        /// Gets or sets the HoursColor
        /// </summary>
        [SitecoreField("Hours Color")]
        public virtual string HoursColor { get; set; }

        /// <summary>
        /// Gets or sets the DaysColor
        /// </summary>
        [SitecoreField("Days Color")]
        public virtual string DaysColor { get; set; }

        /// <summary>
        /// Gets or sets the SecondsText
        /// </summary>
        [SitecoreField("Seconds Text")]
        public virtual string SecondsText { get; set; }

        /// <summary>
        /// Gets or sets the MinutesText
        /// </summary>
        [SitecoreField("Minutes Text")]
        public virtual string MinutesText { get; set; }

        /// <summary>
        /// Gets or sets the HoursText
        /// </summary>
        [SitecoreField("Hours Text")]
        public virtual string HoursText { get; set; }

        /// <summary>
        /// Gets or sets the DaysText
        /// </summary>
        [SitecoreField("Days Text")]
        public virtual string DaysText { get; set; }
    }
}
