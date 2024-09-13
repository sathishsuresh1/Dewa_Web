using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Models.YoutubeAPI;
using Glass.Mapper.Sc;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using Item = Sitecore.Data.Items.Item;
using SitecoreX = global::Sitecore;

namespace DEWAXP.Foundation.Helpers
{
    public static class DEWAYoutubeHelper
    {
        private static ISitecoreService _service = new SitecoreService("master");
        public static YoutubeVideoConfiguration YoutubeItem = _service.GetItem<YoutubeVideoConfiguration>(new Guid(SitecoreItemIdentifiers.YOUTUBE_CONFIG));

        public static void PublishItem(Item item, bool publishParent = false, bool publishChildren = false, PublishMode publishMode = PublishMode.SingleItem)
        {
            List<Language> _Languages = new List<Language>();

            _Languages.Add(LanguageManager.GetLanguage("ar-AE"));
            _Languages.Add(LanguageManager.GetLanguage("en"));

            foreach (var _lang in _Languages)
            {
                PublishOptions publishOptions =
                                             new PublishOptions(item.Database,
                                                       Database.GetDatabase("web"),
                                                       publishMode,
                                                       _lang,
                                                       System.DateTime.Now);  // Create a publisher with the publishoptions
                Publisher publisher = new Publisher(publishOptions);

                // Choose where to publish from
                publisher.Options.RootItem = publishParent ? item.Parent : item;

                // Publish children as well?
                publisher.Options.Deep = publishChildren;

                // Do the publish!
                publisher.Publish();
            }
        }

        public static void GetVideoList(string id, Item parentItem, string pageToken, string language)
        {
            try
            {
                string nextvideopagetoken = string.Empty;
                do
                {
                    WebRequest request = BuildRequestObject(1, id, nextvideopagetoken);

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            //var result = JsonConvert.DeserializeObject<VideoListResponse>(streamReader.ReadToEnd());
                            var result = CustomJsonConvertor.Deserialize<VideoListResponse>(streamReader);
                            if (result != null && result.Items.Count > 0)
                            {
                                nextvideopagetoken = result.NextPageToken;
                                foreach (var item in result.Items)
                                {
                                    if (item.ContentDetails != null && !string.IsNullOrWhiteSpace(item.ContentDetails.videoId))
                                    {
                                        using (new SecurityDisabler())
                                        {
                                            // Get the master database
                                            Database master = Database.GetDatabase("master");

                                            // Get the template for which you need to create item
                                            TemplateItem template = master.GetItem("/sitecore/templates/User Defined/Content Templates/Modules Templates/Youtube Playlist Video");

                                            // Get the place in the site tree where the new item must be inserted
                                            // Item parentItem = master.GetItem("/sitecore/content/Global References/Youtube Video");
                                            if (parentItem != null && !string.IsNullOrEmpty(item.ContentDetails.videoId) && !parentItem.Children.Where(x => x.Fields["Video Id"].Value == item.ContentDetails.videoId.Trim()).Any())
                                            {
                                                using (new LanguageSwitcher(language))
                                                {
                                                    string itemName = !string.IsNullOrEmpty(item.Snippet.Title.Trim()) && !string.IsNullOrEmpty(Regex.Replace(item.Snippet.Title.Trim(), @"[^0-9a-zA-Z]+", "")) ? item.Snippet.Title.Trim() : item.ContentDetails.videoId;
                                                    // Add the item to the site tree
                                                    Item newItem = parentItem.Add(Regex.Replace(itemName, @"[^0-9a-zA-Z]+", " ").Trim(), template);

                                                    newItem.Editing.BeginEdit();
                                                    try
                                                    {
                                                        // Assign values to the fields of the new item
                                                        newItem.Fields["Title"].Value = item.Snippet != null ? item.Snippet.Title : string.Empty;
                                                        newItem.Fields["__Display name"].Value = Regex.Replace(itemName, @"[^0-9a-zA-Z]+", " ").Trim();
                                                        newItem.Fields["Image Link"].Value = item.Snippet != null && item.Snippet.Thumbnails != null ? item.Snippet.Thumbnails.High.Url : string.Empty;
                                                        newItem.Fields["Video Id"].Value = item.ContentDetails.videoId.Trim();
                                                        newItem.Fields["Description"].Value = item.Snippet.Description != null ? item.Snippet.Description : string.Empty;
                                                        newItem.Fields["Published"].Value = item.Snippet.PublishedAt.HasValue ? item.Snippet.PublishedAt.Value.ToString() : string.Empty;
                                                        newItem.Fields["VideoLength"].Value = GetDuration(item.ContentDetails.videoId.Trim());
                                                        newItem.Editing.EndEdit();
                                                    }
                                                    catch (System.Exception ex)
                                                    {
                                                        // Log the message on any failure to sitecore log
                                                        SitecoreX.Diagnostics.Log.Error("Could not update item " + newItem.Paths.FullPath + ": " + ex.Message, new object());
                                                        newItem.Editing.CancelEdit();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                } while (!string.IsNullOrWhiteSpace(nextvideopagetoken));
            }
            catch (Exception ex)
            {
                SitecoreX.Diagnostics.Log.Error("Youtube VideoList API Scheduled task is getting Error -" + ex.ToString() + " Message - " + ex.Message.ToString(), new object());
            }
        }

        public static string GetDuration(string videoId)
        {
            //https://www.googleapis.com/youtube/v3/videos?part=contentDetails&id=7iRX_Q5CPA8%2CDac5Fpc_eMA&fields=items%2FcontentDetails%2Fduration&key={YOUR_API_KEY}

            WebRequest request = BuildRequestObject(2, "", videoId);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = CustomJsonConvertor.Deserialize<VideoDurationResponse>(streamReader);
                    if (result != null) { return System.Xml.XmlConvert.ToTimeSpan(result.items.FirstOrDefault()?.contentDetails?.duration).ToString(); }
                }
            }
            return string.Empty;
        }

        public static void Playlist(Playlist item, Item parentItem, string language)
        {
            if (!string.IsNullOrWhiteSpace(item.Id) && item.ContentDetails.ItemCount > 0)
            {
                using (new SecurityDisabler())
                {
                    Database master = Database.GetDatabase("master");
                    TemplateItem template = master.GetItem("/sitecore/templates/User Defined/Content Templates/Modules Templates/Youtube Playlist");
                    if (parentItem != null && !parentItem.Children.Where(x => x.Fields["Playlist Id"].Value == item.Id).Any())
                    {
                        using (new LanguageSwitcher(language))
                        {
                            string itemName = !string.IsNullOrEmpty(item.Snippet.Title.Trim()) && !string.IsNullOrEmpty(Regex.Replace(item.Snippet.Title.Trim(), @"[^0-9a-zA-Z]+", "")) ? item.Snippet.Title.Trim() : item.Id;
                            // Add the item to the site tree
                            Item newItem = parentItem.Add(Regex.Replace(itemName, @"[^0-9a-zA-Z]+", " ").Trim(), template);

                            newItem.Editing.BeginEdit();
                            try
                            {
                                // Assign values to the fields of the new item
                                newItem.Fields["Title"].Value = item.Snippet != null ? item.Snippet.Title : string.Empty;
                                newItem.Fields["__Display name"].Value = Regex.Replace(itemName, @"[^0-9a-zA-Z]+", " ").Trim();
                                newItem.Fields["Image Link"].Value = item.Snippet != null && item.Snippet.Thumbnails != null ? item.Snippet.Thumbnails.High.Url : string.Empty;
                                newItem.Fields["Playlist Id"].Value = item.Id.Trim();
                                newItem.Editing.EndEdit();
                            }
                            catch (Exception ex)
                            {
                                // Log the message on any failure to sitecore log
                                SitecoreX.Diagnostics.Log.Error("Could not update item " + newItem.Paths.FullPath + ": " + ex.Message, new object());
                                newItem.Editing.CancelEdit();
                            }
                            GetVideoList(item.Id, newItem, "", language);
                        }
                    }
                    else
                    {
                        var playListItem = parentItem.Children.Where(x => x.Fields["Playlist Id"].Value == item.Id).FirstOrDefault();
                        GetVideoList(item.Id, playListItem, "", language);
                    }
                }
            }
        }

        public static void ProcessPlaylist(string apiUrl, ref string nextpagetoken, YoutubeVideoLanguage lang)
        {
            Uri api = new Uri(apiUrl);
            Database master = Database.GetDatabase("master");
            Item parentItem = null;
            string language = string.Empty;
            if (lang.Equals(YoutubeVideoLanguage.Arabic))
            {
                parentItem = master.GetItem("/sitecore/content/Global References/Youtube Video/AR");
                language = "ar-AE";
            }
            else if (lang.Equals(YoutubeVideoLanguage.English))
            {
                parentItem = master.GetItem("/sitecore/content/Global References/Youtube Video/EN");
                language = "en";
            }

            WebRequest request = WebRequest.Create(api);
            WebProxy proxy = new WebProxy(WebConfigurationManager.AppSettings["PROXYURL"]);
            proxy.Credentials = new NetworkCredential(WebConfigurationManager.AppSettings["PROXYUSER"], WebConfigurationManager.AppSettings["PROXYPASSWORD"], WebConfigurationManager.AppSettings["PROXYDOMAIN"]);
            request.Proxy = proxy;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = CustomJsonConvertor.Deserialize<PlaylistListResponse>(streamReader);
                    if (result != null && result.Items.Count > 0)
                    {
                        nextpagetoken = result.NextPageToken;

                        foreach (var item in result.Items)
                        {
                            Playlist(item, parentItem, language);
                        }

                        PublishItem(parentItem, false, true, PublishMode.Smart);
                    }
                }
            }
        }

        /// <summary>
        /// requestType=1 for playlistItems, 2 for videos (to retrieve duration)
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="id"></param>
        /// <param name="pageToken"></param>
        /// <returns></returns>
        private static WebRequest BuildRequestObject(short requestType, string playListId = "", string id = "", string nextpagetoken = "")
        {
            string apiUri = string.Empty;
            string youtubeAPIUrl = YoutubeItem.APIUrl;
            string youtubeAPIKey = YoutubeItem.APIKey;

            if (requestType == 2)
            {
                apiUri = string.Format("{0}videos?part=contentDetails&id={1}&fields=items/contentDetails/duration&key={2}&pageToken={3}", youtubeAPIUrl, id, youtubeAPIKey, nextpagetoken);
            }
            else
            {
                apiUri = string.Format("{0}playlistItems?part=snippet,contentDetails&key={1}&maxResults=50&playlistId={2}&pageToken={3}", youtubeAPIUrl, youtubeAPIKey, playListId, nextpagetoken);
            }

            WebRequest request = WebRequest.Create(new Uri(apiUri));

            WebProxy proxy = new WebProxy(WebConfigurationManager.AppSettings["PROXYURL"]);
            proxy.Credentials = new NetworkCredential(WebConfigurationManager.AppSettings["PROXYUSER"], WebConfigurationManager.AppSettings["PROXYPASSWORD"], WebConfigurationManager.AppSettings["PROXYDOMAIN"]);
            request.Proxy = proxy;

            return request;
        }
    }

}