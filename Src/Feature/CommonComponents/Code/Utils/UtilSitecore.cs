// <copyright file="UtilSitecore.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.CommonComponents.Utils
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Models.Redirect;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc;
    using Glass.Mapper.Sc.Fields;
    using Glass.Mapper.Sc.Web;
    using global::Sitecore;
    using global::Sitecore.Data;
    using Sitecore.Data.Items;
    using System;

    /// <summary>
    /// Defines the <see cref="UtilSitecore" />
    /// </summary>
    public static class UtilSitecore
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));

        /// <summary>
        /// Get last publish date
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DateTime GetLastPublishDate()
        {
            DateTime publishDate = DateTime.Now;
            Database master = Database.GetDatabase("master");
            var item = master.GetItem(SitecoreItemIdentifiers.PUBLISHING_STATISTICS);

            if (item != null && item.Fields["Published On"] != null)
            {
                publishDate = DateUtil.ParseDateTime(item.Fields["Published On"].Value, publishDate, Context.Language.CultureInfo);
            }
            return publishDate;
        }

        /// <summary>
        /// The GetLatestNewsPublishDate
        /// </summary>
        /// <param name="context">The context<see cref="ISitecoreContext"/></param>
        /// <returns>The <see cref="DateTime"/></returns>
        public static DateTime GetLatestNewsPublishDate()
        {
            DateTime publishDate = DateTime.Now;
            Database master = Database.GetDatabase("master");
            var item = master.GetItem(SitecoreItemIdentifiers.PUBLISHING_STATISTICS);

            if (item != null && item.Fields["Latest Published News"] != null)
            {
                publishDate = DateUtil.ParseDateTime(item.Fields["Latest Published News"].Value, publishDate, Context.Language.CultureInfo);
            }
            return publishDate;
        }

        public static string GetMenuURL(ContentBase item)
        {
            Guid redirectPage = new Guid(SitecoreItemIdentifiers.RedirectItemTemplate);
            var url = item.Url;
            if (item.TemplateId == redirectPage)
            {
                var redirectItem = _contentRepository.GetItem<RedirectPage>(new GetItemByIdOptions(item.Id));
                if (redirectItem.RedirectLink != null)
                {
                    url = redirectItem.RedirectLink.Url;
                }
            }
            return url;
        }

        public static string GetMenuURLV1(ContentBase item, Item CurrentItem, out string targettext, out bool menuactive)
        {
            menuactive = false;
            targettext = "_self";
            Guid redirectPage = new Guid(SitecoreItemIdentifiers.RedirectItemTemplate);
            var url = item.Url;           
            if (item != null && CurrentItem != null && item.Id.Equals(CurrentItem.ID.Guid))
            {
                menuactive = true;
            }
            
            if (item.TemplateId == redirectPage)
            {
                var redirectItem = _contentRepository.GetItem<RedirectPage>(new GetItemByIdOptions(item.Id));
                if (redirectItem.RedirectLink != null)
                {
                    url = redirectItem.RedirectLink.Url;
                    if (redirectItem.RedirectLink.Type.Equals(LinkType.External))
                    {
                        if(!string.IsNullOrWhiteSpace(redirectItem.RedirectLink.Target))
                        {
                            targettext = redirectItem.RedirectLink.Target;
                        }
                        else
                        {
                         targettext = "_blank";
                        }
                       
                    }
                    if (redirectItem.RedirectLink.Type.Equals(LinkType.Internal) && redirectItem.RedirectLink.TargetId != null && redirectItem.RedirectLink.TargetId.Equals(CurrentItem.ID.Guid))
                    {
                        menuactive = true;
                    }
                    
                }
            }
            return url;
        }
    }
}