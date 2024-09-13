using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.CommonComponents.Models.ImageMap
{
    [SitecoreType(TemplateName = "Image Map Detail", TemplateId = "{CCDA1050-79A5-44C0-8B5E-60F7D662117A}", AutoMap = true)]
    public class ImageMapDetail : GlassBase
    {
        //Header
        [SitecoreField("Header")]
        public virtual string Header { get; set; }
        //Image Map
        [SitecoreField("Image Map")]
        public virtual Image ImageMap { get; set; }
        //Image Width
        [SitecoreField("Image Width")]
        public virtual string ImageWidth { get; set; }
        //Image Height
        [SitecoreField("Image Height")]
        public virtual string ImageHeight { get; set; }
        /// <summary>
        /// ImageMapCoords
        /// </summary>
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<ImageMapCoords> Children { get; set; }

    }
}