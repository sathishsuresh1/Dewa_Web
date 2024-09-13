using Glass.Mapper.Sc.Configuration.Attributes;


namespace DEWAXP.Feature.CommonComponents.Models.Analytics
{
    [SitecoreType(TemplateId = "{4DFEE9EE-6ADE-4DAC-86A9-11D8654DDB98}", AutoMap = true)]
    public class GoogleAnalyticsConfig
    {
        public virtual string GoogleAnalytics { get; set; }

        public virtual string GoogleAnalyticsHead { get; set; }
    }
}
