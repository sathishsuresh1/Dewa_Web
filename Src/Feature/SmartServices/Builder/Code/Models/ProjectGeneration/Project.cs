using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.Builder.Models.ProjectGeneration
{
    [SitecoreType(TemplateId = "{6EA199B3-EEDE-4402-8671-FC9B59928475}", AutoMap = true)]
    public class Project : GlassBase
    {
        [SitecoreField(FieldName = "ProjectName")]
        public virtual string ProjectName { get; set; }

        [SitecoreField(FieldName = "ContractNumber")]
        public virtual string ContractNumber { get; set; }

        [SitecoreField(FieldName = "DMSFolder")]
        public virtual string DMSFolder { get; set; }

        [SitecoreField(FieldName = "AclName")]
        public virtual string AclName { get; set; }

        [SitecoreField(FieldName = "AclDomain")]
        public virtual string AclDomain { get; set; }
    }

    [SitecoreType(TemplateId = "{9A97EEFC-2EF3-441F-BF37-92E312DDA38A}", AutoMap = true)]
    public class ProjectConfiguration : GlassBase
    {
        [SitecoreField(FieldName = "Subject")]
        public virtual string Subject { get; set; }

        [SitecoreField(FieldName = "EmailFrom")]
        public virtual string EmailFrom { get; set; }

        [SitecoreField(FieldName = "AttachmentSize")]
        public virtual int AttachmentSize { get; set; }

        [SitecoreField(FieldName = "ErrorMessage")]
        public virtual string ErrorMessage { get; set; }
    }
}