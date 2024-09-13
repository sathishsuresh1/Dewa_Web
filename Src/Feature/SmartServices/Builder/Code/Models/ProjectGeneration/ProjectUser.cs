using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.Builder.Models.ProjectGeneration
{
    [SitecoreType(TemplateId = "{BBAFD552-DE4B-44D0-8BDD-A30AD6A421FD}", AutoMap = true)]
    public class ProjectUser : GlassBase
    {
        [SitecoreField(FieldName = "UserName")]
        public virtual string UserName { get; set; }

        [SitecoreField(FieldName = "Password")]
        public virtual string Password { get; set; }

        [SitecoreField(FieldName = "AssignedProjects")]
        public virtual IEnumerable<Project> Projects
        {
            get;
            set;
        }

        [SitecoreField(FieldName = "Emailaddress")]
        public virtual string Emailaddress { get; set; }

        [SitecoreField(FieldName = "CompanyName")]
        public virtual string CompanyName { get; set; }

        [SitecoreField(FieldName = "City")]
        public virtual string City { get; set; }

        [SitecoreField(FieldName = "POBox")]
        public virtual string POBox { get; set; }

        [SitecoreField(FieldName = "Telephone")]
        public virtual string Telephone { get; set; }

        [SitecoreField(FieldName = "Mobile")]
        public virtual string Mobile { get; set; }

        [SitecoreField(FieldName = "Fax")]
        public virtual string Fax { get; set; }

        [SitecoreField(FieldName = "CompanyLocation")]
        public virtual string CompanyLocation { get; set; }

        [SitecoreField(FieldName = "SecurityCode")]
        public virtual string SecurityCode { get; set; }
    }
}