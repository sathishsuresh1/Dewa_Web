using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.Dashboard.Models.BES
{
    public class Bes : ContentBase
    {
        public virtual string Component { get; set; }
        public virtual string ComponentId { get; set; }
        public virtual string Endpoint { get; set; }
        public virtual string cssclass { get; set; }
        public virtual string returnurl { get; set; }
        public virtual Link CalltoAction { get; set; }
        public virtual string Header { get; set; }
        public virtual string Footer { get; set; }

        /// <summary>
        /// Enable New BES Layout
        /// </summary>
        [SitecoreField("Enable New BES Layout")]
        public virtual string EnableNewBESLayout { get; set; }
    }
}