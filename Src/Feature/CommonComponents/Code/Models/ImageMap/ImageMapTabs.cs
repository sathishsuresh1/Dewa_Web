using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace DEWAXP.Feature.CommonComponents.Models.ImageMap
{
    [SitecoreType(TemplateName = "Image Map Tabs", TemplateId = "{8EAECB1A-C5F2-4991-82B7-598BE70A4AC4}", AutoMap = true)]
    public class ImageMapTabs : GlassBase
    {


        /// <summary>
        /// Hero Slides Children
        /// </summary>
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<ImageMapDetail> Children { get; set; }

        public virtual string GetImageCoordJson(ImageMapDetail imageMapDetail)
        {
            if (imageMapDetail != null && imageMapDetail.Children != null)
            {
               var d = imageMapDetail.Children.Select(x => new { coords = x.Coordinates, content = x.ContentTitles, shape = x.ShapeType, side = x.Side }).ToList();
                if (d != null)
                {
                    return new JavaScriptSerializer().Serialize(d);
                }
            }
            return "[]";
        }

        public virtual string GetImageDimensionJson(ImageMapDetail imageMapDetail)
        {
            if (imageMapDetail != null)
            {
                return new JavaScriptSerializer().Serialize(new { width = imageMapDetail.ImageWidth, height = imageMapDetail.ImageHeight });
            }
            return "{\"width\": \"968\", \"height\": \"1370\"}";
        }
    }
    
}