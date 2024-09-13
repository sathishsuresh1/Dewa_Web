using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.Common
{
    [SitecoreType(TemplateId = "{F0BCCAA8-8B8F-435C-AF43-9452CC814AAC}", AutoMap = true)]
    public class DataSourceItems : ContentBase
    {
        public virtual string Text { get; set; }
        public virtual string Value { get; set; }
    }
}