using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Foundation.Content.Models.Redirect
{
    [SitecoreType(TemplateId = "{954B7389-A0CF-4512-BFB3-6D6DF1BF6003}", AutoMap = true)]
    public class RedirectPage : ContentBase
    {
        [SitecoreField("Redirect Link")]
        public virtual Link RedirectLink { get; set; }

        [SitecoreField("AppendUserLoginParameters")]
        public virtual bool AppendUserLoginParameters { get; set; }

        [SitecoreField("srvparameter")]
        public virtual string srvparameter { get; set; }
    }
}