using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.IdealHome.Models.Configurations.Email
{

    [SitecoreType(TemplateId = "{9A97EEFC-2EF3-441F-BF37-92E312DDA38A}", AutoMap = true)]
    public class EmailConfigurations : GlassBase
    {
        [SitecoreField(FieldName = "Subject")]
        public virtual string Subject { get; set; }

        [SitecoreField(FieldName = "From Address")]
        public virtual string From { get; set; }

        [SitecoreField(FieldName = "Body")]
        public virtual string Body { get; set; }

        [SitecoreField(FieldName = "CC")]
        public virtual string CC { get; set; }

    }
}