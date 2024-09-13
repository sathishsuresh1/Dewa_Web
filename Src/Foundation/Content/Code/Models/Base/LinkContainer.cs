using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Content.Models.Base
{
    [SitecoreType(TemplateName = "Link Container", TemplateId = "{7E4DFD5E-F3AE-4321-B992-7556DE897879}", AutoMap = true)]
    public class LinkContainer : ContentBase
    {
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<sLink> Children { get; set; }
    }

    [SitecoreType(TemplateName = "Link", TemplateId = "{C5CE1669-9C7C-4650-AA8A-CC97D42D357C}", AutoMap = true)]
    public class sLink : ContentBase
    {        
        public virtual Link Link { get; set; }

    }
}