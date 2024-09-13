using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.TaskModels
{
    [SitecoreType(TemplateId = "{C03278C6-C2F2-4479-B2C8-35A980CA9CE2}", AutoMap = true)]
    public class SitemapGenerationConfig
    {
        [SitecoreField(FieldName = "Output Path")]
        public virtual string OutputPath { get; set; }

        [SitecoreField(FieldName = "Max Count")]
        public virtual string MaxCount { get; set; }

        [SitecoreField(FieldName = "Domain Url")]
        public virtual string DomainUrl { get; set; }
    }
}