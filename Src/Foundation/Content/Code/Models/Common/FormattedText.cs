using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.Content.Models.Common
{
    [SitecoreType(TemplateId = "{BBDC399C-1225-4667-BD85-5818A4F966E7}", AutoMap = true)]
    public class FormattedText : ContentBase
    {
        [SitecoreField("Rich Text", Setting = SitecoreFieldSettings.RichTextRaw)]
        public virtual string RichText { get; set; }
    }
}