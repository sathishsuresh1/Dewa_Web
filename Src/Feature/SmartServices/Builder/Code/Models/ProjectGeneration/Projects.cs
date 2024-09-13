using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.Builder.Models.ProjectGeneration
{
    [SitecoreType(TemplateId = "{201BB311-CBA6-4BE8-9AFB-F94805BFA44A}", AutoMap = true)]
    public class ProjectsFolder : ContentBase
    {
        [SitecoreChildren]
        public virtual IEnumerable<Project> Projects { get; set; }
    }
}