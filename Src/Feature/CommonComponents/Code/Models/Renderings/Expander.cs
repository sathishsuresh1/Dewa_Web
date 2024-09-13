using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    public class Expander : ContentBase
    {
        public virtual string Heading { get; set; }
        public virtual string Content { get; set; }
    }
}