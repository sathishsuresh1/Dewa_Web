// <copyright file="VideoPlayerModalPopup.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;
    using System;

    /// <summary>
    /// Defines the <see cref="VideoPlayerModalPopup" />.
    /// </summary>
    [SitecoreType(TemplateName = "VideoPlayer Modal Popup", TemplateId = "{9933F381-3961-42AE-9021-AAAD0FC9B7A4}", AutoMap = true)]
    public class VideoPlayerModalPopup : GlassBase
    {
        /// <summary>
        /// Gets or sets the VideoTitle
        /// VideoTitle..
        /// </summary>
        [SitecoreField("VideoTitle")]
        public virtual string VideoTitle { get; set; }

        /// <summary>
        /// Gets or sets the VideoDate
        /// VideoDate..
        /// </summary>
        [SitecoreField("VideoDate")]
       public virtual DateTime VideoDate { get; set; }

        /// <summary>
        /// Gets or sets the VideoPreviewImage
        /// VideoPreviewImage..
        /// </summary>
        [SitecoreField("VideoPreviewImage")]
        public virtual Image VideoPreviewImage { get; set; }

        /// <summary>
        /// Gets or sets the VideoDescription.
        /// </summary>
        [SitecoreField("Video Description")]
        public virtual string VideoDescription { get; set; }

        /// <summary>
        /// Gets or sets the ModalPopupTitle
        /// ModalPopupTitle..
        /// </summary>
        [SitecoreField("ModalPopupTitle")]
        public virtual string ModalPopupTitle { get; set; }

        /// <summary>
        /// Gets or sets the ModalPopupHtml
        /// ModalPopupHtml..
        /// </summary>
        [SitecoreField("ModalPopupHtml")]
        public virtual string ModalPopupHtml { get; set; }

        /// <summary>
        /// Gets or sets the ModalPopupDescription.
        /// </summary>
        [SitecoreField("ModalPopupDescription")]
        public virtual string ModalPopupDescription { get; set; }
    }
}
