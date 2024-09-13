using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.Builder.Models.ProjectGeneration
{
    [SitecoreType(TemplateId = "{A83D2F19-10E5-48FA-A521-B2C17042C057}", AutoMap = true)]
    public class ProjectUsers : ContentBase
    {
        [SitecoreChildren]
        public virtual IEnumerable<ProjectUser> Users { get; set; }
    }
}