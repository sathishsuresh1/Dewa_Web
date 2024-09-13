using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Content.Models.TariffCalculator
{
    [SitecoreType(TemplateId = "{81C4B51D-08A5-441D-86F0-87B23BBD5DCE}", AutoMap = true)]
    public class TariffType : GlassBase
    {
        [SitecoreChildren]
        public virtual IEnumerable<Consumption> Consumption { get; set; }

        public virtual string FuelSurchargeTariff { get; set; }
    }
}