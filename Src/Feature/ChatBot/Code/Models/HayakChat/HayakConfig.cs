using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Repositories;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.Web;
using System;

namespace DEWAXP.Feature.ChatBot.Models.HayakChat
{
    [SitecoreType(TemplateId = "{244FA884-07FB-4E6D-B246-8DD246620A44}", AutoMap = true)]
    public class HayakConfig
    {
        [SitecoreField("serverTf")]
        public virtual string serverTf { get; set; }

        [SitecoreField("portTf")]
        public virtual string portTf { get; set; }

        [SitecoreField("amcUrlPathTf")]
        public virtual string amcUrlPathTf { get; set; }

        [SitecoreField("displayNameTf")]
        public virtual string displayNameTf { get; set; }

        [SitecoreField("fromTf")]
        public virtual string fromTf { get; set; }

        [SitecoreField("destinationTf")]
        public virtual string destinationTf { get; set; }

        [SitecoreField("contextTf")]
        public virtual string contextTf { get; set; }

        [SitecoreField("aawgServerTf")]
        public virtual string aawgServerTf { get; set; }

        [SitecoreField("aawgPortTf")]
        public virtual string aawgPortTf { get; set; }

        [SitecoreField("topicTf")]
        public virtual string topicTf { get; set; }

        [SitecoreField("tokenServerTf")]
        public virtual string tokenServerTf { get; set; }

        [SitecoreField("tokenPortTf")]
        public virtual string tokenPortTf { get; set; }

        [SitecoreField("tokenUrlPathTf")]
        public virtual string tokenUrlPathTf { get; set; }

        [SitecoreField("priorityTf")]
        public virtual string priorityTf { get; set; }

        [SitecoreField("localeTf")]
        public virtual string localeTf { get; set; }

        [SitecoreField("strategyTf")]
        public virtual string strategyTf { get; set; }

        [SitecoreField("sourceNameTf")]
        public virtual string sourceNameTf { get; set; }

        [SitecoreField("nativeResourceIdTf")]
        public virtual string nativeResourceIdTf { get; set; }
    }
    public static class HayakConstant
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
        public static HayakConfig config
        {
            get
            {
                return _contentRepository.GetItem<HayakConfig>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.HAYAK_CONFIG)));
            }
        }
    }
}
