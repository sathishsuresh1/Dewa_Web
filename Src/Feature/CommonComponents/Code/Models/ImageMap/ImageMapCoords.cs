using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.CommonComponents.Models.ImageMap
{
    [SitecoreType(TemplateName = "Image Map Coords", TemplateId = "{2C6FFD8B-C2F9-43CD-A0C4-3152C869A8FE}", AutoMap = true)]

    public class ImageMapCoords : GlassBase
    {
        /// <summary>
        /// Co-ordinates
        /// </summary>
        [SitecoreField("Co-ordinates")]
        public virtual string Coordinates { get; set; }
        /// <summary>
        /// Shape Type
        /// </summary>
        [SitecoreField("Shape Type")]
        public virtual string ShapeType { get; set; }

        /// <summary>
        /// Sides
        /// </summary>
        [SitecoreField("Side")]
        public virtual string Side { get; set; }

        /// <summary>
        /// Content Titles
        /// </summary>
        [SitecoreField("Content Titles")]
        public virtual string ContentTitles { get; set; }
        /// <summary>
        /// Link
        /// </summary>
        [SitecoreField("Link")]
       public virtual Link Link { get; set; }
    }
}