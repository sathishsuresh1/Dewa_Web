using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Publishing;
using Sitecore.Resources.Media;
using SC = Sitecore;

namespace DEWAXP.Foundation.Helpers.Extensions
{
    public static class UtilExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>(comparer);
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string AddToTermDictionary(this string value, string key, Dictionary<string, string> dictionary, string mod = "")
        {
            var languages = SC.Context.Database.Languages.ToList();

            languages = ReorderLanguageList(languages);

            foreach (var language in languages)
            {
                var fullKey = string.Format("{0}_{1}_{2}", key, language.Name, mod);
                var translatedValue = Translate.TextByLanguage(value, language);

                if (dictionary == null)
                    dictionary = new Dictionary<string, string>();

                if (dictionary.ContainsKey(fullKey))
                {
                    return translatedValue;
                }

                dictionary.Add(fullKey, translatedValue);
            }
            var currentText = Translate.TextByLanguage(value, SC.Context.Language);
            return currentText;
        }

        public static string GetTermFromDictionary(this string key, Dictionary<string, string> dictionary, string mod = "")
        {
            if (key == null)
                return "";

            var language = SC.Context.Language.Name;
            var fullKey = string.Format("{0}_{1}_{2}", key, language, mod);
            var val = key;

            if (dictionary.ContainsKey(fullKey))
            {
                dictionary.TryGetValue(fullKey, out val);
                return val;
            }

            return val;
        }

        private static List<Language> ReorderLanguageList(List<Language> languages)
        {
            var index = languages.FindIndex(x => x.Name == SC.Context.Language.Name);
            var item = languages[index];
            languages[index] = languages[0];
            languages[0] = item;
            return languages;
        }
        public static string GetLinkUrl(Item item, string fieldName)
        {

            if (item != null)
            {
                LinkField lf = item.Fields[fieldName];

                if (lf != null)
                {
                    var url = string.Empty;

                    switch (lf.LinkType.ToLower())
                    {
                        case "internal":
                            // Use LinkMananger for internal links, if link is not empty
                            url = lf.TargetItem != null ? LinkManager.GetItemUrl(lf.TargetItem) : string.Empty;
                            break;
                        case "media":
                            // Use MediaManager for media links, if link is not empty
                            url = lf.TargetItem != null ? MediaManager.GetMediaUrl(lf.TargetItem) : string.Empty;
                            break;
                        case "external":
                            // Just return external links
                            url = lf.Url;
                            break;
                        case "anchor":
                            // Prefix anchor link with # if link if not empty
                            url = !string.IsNullOrEmpty(lf.Anchor) ? "#" + lf.Anchor : string.Empty;
                            break;
                        case "mailto":
                            // Just return mailto link
                            url = lf.Url;
                            break;
                        case "javascript":
                            // Just return javascript
                            url = lf.Url;
                            break;
                        default:
                            // Just please the compiler, this
                            // condition will never be met
                            url = lf.Url;
                            break;
                    }

                    if (!string.IsNullOrEmpty(lf.QueryString))
                    {
                        if (!string.IsNullOrEmpty(url))
                            url = string.Format("{0}?{1}", url, HttpContext.Current.Server.UrlDecode(lf.QueryString));
                    }

                    return (url);
                }
            }


            return (string.Empty);
        }

        public static void PublishItem(Item item, string sitecoreDb = "master")
        {
            // The publishOptions determine the source and target database,
            // the publish mode and language, and the publish date
            if (sitecoreDb != "master")
            {
                var DB = Database.GetDatabase(sitecoreDb);
                if (DB != null)
                {
                    PublishOptions publishOptions =
                                                    new PublishOptions(item.Database,
                                                     DB,
                                                     PublishMode.SingleItem,
                                                     item.Language,
                                                     System.DateTime.Now);  // Create a publisher with the publishoptions
                    Publisher publisher = new Publisher(publishOptions);

                    // Choose where to publish from
                    publisher.Options.RootItem = item;

                    // Publish children as well?
                    publisher.Options.Deep = true;

                    // Do the publish!
                    publisher.Publish();
                }
            }
            else
            {
                var targets = PublishManager.GetPublishingTargets(Database.GetDatabase(sitecoreDb));
                foreach (var target in targets)
                {
                    var useDatabaseName = target.Fields["Target database"] != null ? target.Fields["Target database"].Value : "";
                    if (string.IsNullOrEmpty(useDatabaseName))
                    {
                        continue;

                    }
                    var DB = Database.GetDatabase(useDatabaseName);
                    if (DB != null)
                    {
                        PublishOptions publishOptions =
                                                        new PublishOptions(item.Database,
                                                         DB,
                                                         PublishMode.SingleItem,
                                                         item.Language,
                                                         System.DateTime.Now);  // Create a publisher with the publishoptions
                        Publisher publisher = new Publisher(publishOptions);

                        // Choose where to publish from
                        publisher.Options.RootItem = item;

                        // Publish children as well?
                        publisher.Options.Deep = true;

                        // Do the publish!
                        publisher.Publish();
                    }

                }
            }


        }
    }
}
