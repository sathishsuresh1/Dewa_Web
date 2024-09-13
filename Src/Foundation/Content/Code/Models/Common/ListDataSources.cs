using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.Content.Models.Common
{
    [SitecoreType(TemplateId = "{F191F090-0DCF-4948-A833-99B578CCC7AA}", AutoMap = true)]
    public class ListDataSources
    {
        [SitecoreChildren]
        public virtual IEnumerable<DataSourceItems> Items { get; set; }
    }
}