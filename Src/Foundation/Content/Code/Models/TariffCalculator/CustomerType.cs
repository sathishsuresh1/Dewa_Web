using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Content.Models.TariffCalculator
{
    [SitecoreType(TemplateId = "{2ED34EAF-E456-45CD-9534-26CD1ACCE7CB}", AutoMap = true)]
    public class CustomerType : GlassBase
    {
        public virtual string Title { get; set; }

        [SitecoreChildren]
        public virtual IEnumerable<TariffType> TariffTypes { get; set; }
    }
}