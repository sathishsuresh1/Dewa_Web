// <copyright file="YoutubeAPIEn.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Tasks
{
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Models.YoutubeAPI;
    using Glass.Mapper.Sc;
    using global::Sitecore.Data.Items;
    using global::Sitecore.Globalization;
    using System;
    using Item = Sitecore.Data.Items.Item;
    using SitecoreX = global::Sitecore;

    /// <summary>
    /// Defines the <see cref="YoutubeAPIAr" />
    /// </summary>
    public class YoutubeAPIAr
    {
        private static ISitecoreService _service = new SitecoreService("master");
        public static YoutubeVideoConfiguration YoutubeItem = _service.GetItem<YoutubeVideoConfiguration>(new Guid(SitecoreItemIdentifiers.YOUTUBE_CONFIG));

        /// <summary>
        /// The Execute
        /// </summary>
        /// <param name="items">The items<see cref="Item[]"/></param>
        /// <param name="command">The command<see cref="SitecoreX.Tasks.CommandItem"/></param>
        /// <param name="schedule">The schedule<see cref="SitecoreX.Tasks.ScheduleItem"/></param>
        public void Execute(Item[] items, SitecoreX.Tasks.CommandItem command, SitecoreX.Tasks.ScheduleItem schedule)
        {
            SitecoreX.Diagnostics.Log.Info("Youtube Arabic API Scheduled task is being run!", this);
            if (YoutubeItem != null && !string.IsNullOrWhiteSpace(YoutubeItem.APIKey))
            {
                try
                {
                    SitecoreX.Context.Language = Language.Parse("ar-AE");
                    string nextpagetoken = string.Empty;
                    do
                    {
                        string apiUrl = string.Format("{0}playlists?part=snippet,contentDetails&key={1}&maxResults=50&channelId={2}&pageToken={3}", YoutubeItem.APIUrl, YoutubeItem.APIKey, YoutubeItem.DEWAChannelEnglish, nextpagetoken);
                        SitecoreX.Diagnostics.Log.Info("Youtube Arabic API Url : " + apiUrl, this);
                        DEWAYoutubeHelper.ProcessPlaylist(apiUrl, ref nextpagetoken, YoutubeVideoLanguage.Arabic);
                    } while (!string.IsNullOrWhiteSpace(nextpagetoken));

                    if (!string.IsNullOrWhiteSpace(YoutubeItem.UnlistedPlaylist))
                    {
                        var playlistids = YoutubeItem.UnlistedPlaylist.Split(';');
                        foreach (var playlist in playlistids)
                        {
                            string apiUrl = string.Format("{0}playlists?part=snippet,contentDetails&key={1}&maxResults=50&id={2}", YoutubeItem.APIUrl, YoutubeItem.APIKey, playlist);
                            SitecoreX.Diagnostics.Log.Info("Youtube Arabic API Url : " + apiUrl, this);
                            DEWAYoutubeHelper.ProcessPlaylist(apiUrl, ref nextpagetoken, YoutubeVideoLanguage.Arabic);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SitecoreX.Diagnostics.Log.Error("Youtube PlayList API Scheduled task is getting Error -" + ex.ToString() + " Message - " + ex.Message.ToString(), this);
                }
            }
        }
    }
}