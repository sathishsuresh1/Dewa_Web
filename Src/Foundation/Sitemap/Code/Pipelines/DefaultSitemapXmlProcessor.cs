using System;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Sites;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Configuration;

namespace DEWAXP.Foundation.Sitemap.Pipelines
{
    public class DefaultSitemapXmlProcessor : CreateSitemapXmlProcessor
    {
        private string indexName;

        public DefaultSitemapXmlProcessor(string indexName)
        {
            this.indexName = indexName;
        }

        private IEnumerable<UrlDefinition> ProcessSite(Sitecore.Data.Items.Item homeItem, SiteDefinition def, Sitecore.Globalization.Language language)
        {
            IProviderSearchContext ctx;
            if (string.IsNullOrEmpty(this.indexName))
                ctx = ContentSearchManager.GetIndex((SitecoreIndexableItem)homeItem).CreateSearchContext();
            else
                ctx = ContentSearchManager.GetIndex(this.indexName).CreateSearchContext();

            try
            {
                var results = ctx.GetQueryable<SitemapResultItem>()
                    .Where(i => i.Paths.Contains(homeItem.ID) && i.Language == language.Name);

				var tmplPred = PredicateBuilder.False<SitemapResultItem>();
	            foreach (var tmpl in def.IncludedBaseTemplates.Where(i => Sitecore.Data.ID.IsID(i)).Select(i => Sitecore.Data.ID.Parse(i)))
	            {
					tmplPred = tmplPred.Or(i => i.AllTemplates.Contains(tmpl));
				}

	            foreach (var tmpl in def.IncludedTemplates.Where(i => Sitecore.Data.ID.IsID(i)).Select(i => Sitecore.Data.ID.Parse(i)))
	            {
					tmplPred = tmplPred.Or(i => i.TemplateId == tmpl);
				}

				var itemPred = PredicateBuilder.True<SitemapResultItem>();
	            foreach (var id in def.ExcludedItems.Where(i => Sitecore.Data.ID.IsID(i)).Select(i => Sitecore.Data.ID.Parse(i)))
	            {
					itemPred = itemPred.And(i => i.ItemId != id);
				}
                    
                results = results.Where(tmplPred.And(itemPred));

                var items = results
                    .Select(i => Sitecore.Configuration.Factory.GetDatabase(i.DatabaseName).GetItem(i.ItemId, Sitecore.Globalization.Language.Parse(i.Language), Sitecore.Data.Version.Latest))
                    .ToList();

	            var itemsMinusExclusions = items.Where(i => !IsExplicitlyHidden(i));

                var sb = new StringBuilder();
                var options = Sitecore.Links.UrlOptions.DefaultOptions;
                options.SiteResolving = Sitecore.Configuration.Settings.Rendering.SiteResolving;
                options.Site = SiteContext.GetSite(def.SiteName);
                if (def.EmbedLanguage)
                    options.LanguageEmbedding = Sitecore.Links.LanguageEmbedding.Always;
                else
                    options.LanguageEmbedding = Sitecore.Links.LanguageEmbedding.Never;
                options.AlwaysIncludeServerUrl = true;
                options.Language = language;

                foreach (var item in itemsMinusExclusions)
                {
	                if (item != null && item.Versions != null && item.Versions.Count > 0)
	                {
						yield return new UrlDefinition(Sitecore.Links.LinkManager.GetItemUrl(item, options), item.Statistics.Updated);
					}
                }
            }
            finally
            {
                ctx.Dispose();
            }
        }

	    private bool IsExplicitlyHidden(Item item)
	    {
		    if (item != null)
		    {
				const string explicitExclusionFieldID = "{478A0F03-ABA9-4B5F-93F5-093EB3678DEC}";
                const string explicitExclusionFieldKeyName = "HideFromSitemapPage";

                var field = item.Fields[explicitExclusionFieldKeyName];

                if(field==null)
                {
                    field = item.Fields[ID.Parse(explicitExclusionFieldID)];
                }

				if (field != null)
				{
					int excluded;
					if (int.TryParse(field.Value, out excluded))
					{
						return Convert.ToBoolean(excluded);
					}
				}
				return false;
			}
		    return true;
	    }

	    public override void Process(CreateSitemapXmlArgs args)
        {
            var scDBContext = Factory.GetDatabase(args.Site.Database);
            var langs = Sitecore.Data.Managers.LanguageManager.GetLanguages(scDBContext);
            var homeItem = scDBContext.GetItem(args.Site.RootPath + args.Site.StartItem);

            var def = this.Configuration[args.Site.Name];
            if (def.EmbedLanguage)
            {
                foreach (var lang in langs)
                    args.Nodes.AddRange(ProcessSite(homeItem, def, lang));
            }
            else
            {
                args.Nodes.AddRange(ProcessSite(homeItem, def, Sitecore.Context.Language));
            }
        }
    }

    class SitemapResultItem : SearchResultItem
    {
        [IndexField("_templates")]
        public IEnumerable<Sitecore.Data.ID> AllTemplates { get; set; }
	}
}
