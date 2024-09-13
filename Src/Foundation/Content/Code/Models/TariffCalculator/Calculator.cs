using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Content.Models.TariffCalculator
{
    [SitecoreType(TemplateId = "{6E3D3A51-AE20-480A-AEA2-F3D9A9364D92}", AutoMap = true)]
    public class Calculator : ContentBase
    {
        [SitecoreChildren]
        public virtual IEnumerable<CustomerType> CustomerTypes { get; set; }
    }
}