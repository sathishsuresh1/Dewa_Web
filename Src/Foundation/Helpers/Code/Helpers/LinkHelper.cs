// <copyright file="LinkHelper.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Helpers
{
    using DEWAXP.Foundation.Logger;
    using global::Sitecore.Configuration;
    using global::Sitecore.Data;
    using global::Sitecore.Data.Items;
    using global::Sitecore.Links;
    using System;
    using System.Text;
    using SitecoreX = global::Sitecore.Context;
    using Media = global::Sitecore.Resources.Media;
    using Sitecore.Links.UrlBuilders;

    /// <summary>
    /// Defines the <see cref="LinkHelper" />
    /// </summary>
    public static class LinkHelper
    {
        /// <summary>
        /// The GetItemUrl
        /// </summary>
        /// <param name="sitecoreItemId">The sitecoreItemId<see cref="string"/></param>
        /// <param name="makeRelative">The makeRelative<see cref="bool"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetItemUrl(string sitecoreItemId, bool makeRelative = true)
        {
            if (SitecoreX.Database.GetItem(sitecoreItemId) != null)
            {
                return GetItemUrl(SitecoreX.Database.GetItem(sitecoreItemId), makeRelative);
            }
            return string.Empty;
        }

        /// <summary>
        /// The GetItemUrl
        /// </summary>
        /// <param name="sitecoreItemId">The sitecoreItemId<see cref="Item"/></param>
        /// <param name="makeRelative">The makeRelative<see cref="bool"/></param>
        /// <param name="setSite">The setSite<see cref="bool"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetItemUrl(Item sitecoreItemId, bool makeRelative = true, bool setSite = false)
        {
            ItemUrlBuilderOptions urlOptions = new ItemUrlBuilderOptions()
            {
                AlwaysIncludeServerUrl = !makeRelative,
                LowercaseUrls = true,
                LanguageEmbedding = LanguageEmbedding.Always
            };
            if (setSite)
            {
                urlOptions.Site = Factory.GetSite("website");
            }

            if (sitecoreItemId != null)
            {
                string url = LinkManager.GetItemUrl(sitecoreItemId, urlOptions);
                Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);

                return (makeRelative && uri.IsAbsoluteUri) ? uri.PathAndQuery : url;
            }
            return string.Empty;
        }

        /// <summary>
        /// The GetPath
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetPath(string id)
        {
            string path = SitecoreX.Database.GetItem(new ID(id)).Paths.FullPath;
            return path;
        }

        /// <summary>
        /// The Base64Decode
        /// </summary>
        /// <param name="base64EncodedData">The base64EncodedData<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, new object());
                return string.Empty;
            }
        }

        public static string GetMediaItemURL(string sitecoreItemId, bool makeRelative = true)
        {
            if (SitecoreX.Database.GetItem(sitecoreItemId) != null)
            {
                MediaItem sampleMedia =new MediaItem(SitecoreX.Database.GetItem(sitecoreItemId));
                if (sampleMedia != null)
                {
                    MediaUrlBuilderOptions mediaOptions = new MediaUrlBuilderOptions { Thumbnail=true};
                    return global::Sitecore.StringUtil.EnsurePrefix('/', Media.MediaManager.GetMediaUrl(sampleMedia, mediaOptions));
                }
            }
            return string.Empty;
        }
    }
}
