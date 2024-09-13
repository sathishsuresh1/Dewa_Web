using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.Analytics
{
    [SitecoreType(TemplateId = "{2EAED81F-A5E7-4990-B522-AFDC58158A65}", AutoMap = true)]
    public class LinkedInAnalyticConfig 
    {
        #region [Auth Details]
        [SitecoreField(FieldName = "Client Id")]
        public virtual string ClientId { get; set; }

        [SitecoreField(FieldName = "Client Secret Key")]
        public virtual string ClientSecretKey { get; set; }

        [SitecoreField(FieldName = "Permission Scope")]
        public virtual string PermissionScope { get; set; }

        [SitecoreField(FieldName = "CallBack Url")]
        public virtual string CallBackUrl { get; set; }
        #endregion
        #region [Custom details]
        [SitecoreField(FieldName = "Custom Script")]
        public virtual string CustomScript { get; set; }

        [SitecoreField(FieldName = "Enabled Custom")]
        public virtual bool EnabledCustom { get; set; }
        #endregion

        [SitecoreField(FieldName = "Enabled All Page")]
        public virtual bool EnabledAllPage { get; set; }
    }
}