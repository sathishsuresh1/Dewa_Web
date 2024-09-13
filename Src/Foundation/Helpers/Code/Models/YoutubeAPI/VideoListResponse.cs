using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Helpers.Models.YoutubeAPI
{
    #region PlaylistResponse

    public class PlaylistListResponse : BaseResponse
    {
        [JsonProperty("items")]
        public virtual IList<Playlist> Items
        {
            get;
            set;
        }

        public PlaylistListResponse()
        { }
    }

    //
    // Summary:
    //     A playlist resource represents a YouTube playlist. A playlist is a collection
    //     of videos that can be viewed sequentially and shared with other users. A playlist
    //     can contain up to 200 videos, and YouTube does not limit the number of playlists
    //     that each user creates. By default, playlists are publicly visible to other users,
    //     but playlists can be public or private. YouTube also uses playlists to identify
    //     special collections of videos for a channel, such as: - uploaded videos - favorite
    //     videos - positively rated (liked) videos - watch history - watch later To be
    //     more specific, these lists are associated with a channel, which is a collection
    //     of a person, group, or company's videos, playlists, and other YouTube information.
    //     You can retrieve the playlist IDs for each of these lists from the channel resource
    //     for a given channel. You can then use the playlistItems.list method to retrieve
    //     any of those lists. You can also add or remove items from those lists by calling
    //     the playlistItems.insert and playlistItems.delete methods.
    public class Playlist
    {
        public Playlist()
        { }

        //
        // Summary:
        //     The contentDetails object contains information like video count.
        [JsonProperty("contentDetails")]
        public virtual PlaylistContentDetails ContentDetails { get; set; }

        //
        // Summary:
        //     Etag of this resource.
        [JsonProperty("etag")]
        public virtual string ETag { get; set; }

        //
        // Summary:
        //     The ID that YouTube uses to uniquely identify the playlist.
        [JsonProperty("id")]
        public virtual string Id { get; set; }

        //
        // Summary:
        //     Identifies what kind of resource this is. Value: the fixed string "youtube#playlist".
        [JsonProperty("kind")]
        public virtual string Kind { get; set; }

        //
        // Summary:
        //     Localizations for different languages
        [JsonProperty("localizations")]
        public virtual IDictionary<string, PlaylistLocalization> Localizations { get; set; }

        //
        // Summary:
        //     The player object contains information that you would use to play the playlist
        //     in an embedded player.
        [JsonProperty("player")]
        public virtual PlaylistPlayer Player { get; set; }

        //
        // Summary:
        //     The snippet object contains basic details about the playlist, such as its title
        //     and description.
        [JsonProperty("snippet")]
        public virtual PlaylistSnippet Snippet { get; set; }

        //
        // Summary:
        //     The status object contains status information for the playlist.
        [JsonProperty("status")]
        public virtual PlaylistStatus Status { get; set; }
    }

    public class PlaylistContentDetails
    {
        public PlaylistContentDetails()
        { }

        //
        // Summary:
        //     The ETag of the item.
        public virtual string ETag { get; set; }

        //
        // Summary:
        //     The number of videos in the playlist.
        [JsonProperty("itemCount")]
        public virtual long? ItemCount { get; set; }
    }

    //
    // Summary:
    //     Playlist localization setting
    public class PlaylistLocalization
    {
        public PlaylistLocalization()
        { }

        //
        // Summary:
        //     The localized strings for playlist's description.
        [JsonProperty("description")]
        public virtual string Description { get; set; }

        //
        // Summary:
        //     The ETag of the item.
        public virtual string ETag { get; set; }

        //
        // Summary:
        //     The localized strings for playlist's title.
        [JsonProperty("title")]
        public virtual string Title { get; set; }
    }

    public class PlaylistPlayer
    {
        public PlaylistPlayer()
        { }

        //
        // Summary:
        //     An tag that embeds a player that will play the playlist.
        [JsonProperty("embedHtml")]
        public virtual string EmbedHtml { get; set; }

        //
        // Summary:
        //     The ETag of the item.
        public virtual string ETag { get; set; }
    }

    //
    // Summary:
    //     Basic details about a playlist, including title, description and thumbnails.
    public class PlaylistSnippet
    {
        public PlaylistSnippet()
        { }

        //
        // Summary:
        //     The ID that YouTube uses to uniquely identify the channel that published the
        //     playlist.
        [JsonProperty("channelId")]
        public virtual string ChannelId { get; set; }

        //
        // Summary:
        //     The channel title of the channel that the video belongs to.
        [JsonProperty("channelTitle")]
        public virtual string ChannelTitle { get; set; }

        //
        // Summary:
        //     The language of the playlist's default title and description.
        [JsonProperty("defaultLanguage")]
        public virtual string DefaultLanguage { get; set; }

        //
        // Summary:
        //     The playlist's description.
        [JsonProperty("description")]
        public virtual string Description { get; set; }

        //
        // Summary:
        //     The ETag of the item.
        public virtual string ETag { get; set; }

        //
        // Summary:
        //     Localized title and description, read-only.
        [JsonProperty("localized")]
        public virtual PlaylistLocalization Localized { get; set; }

        //
        // Summary:
        //     System.DateTime representation of Google.Apis.YouTube.v3.Data.PlaylistSnippet.PublishedAtRaw.
        [JsonIgnore]
        public virtual DateTime? PublishedAt { get; set; }

        //
        // Summary:
        //     The date and time that the playlist was created. The value is specified in ISO
        //     8601 (YYYY-MM- DDThh:mm:ss.sZ) format.
        [JsonProperty("publishedAt")]
        public virtual string PublishedAtRaw { get; set; }

        //
        // Summary:
        //     Keyword tags associated with the playlist.
        [JsonProperty("tags")]
        public virtual IList<string> Tags { get; set; }

        //
        // Summary:
        //     A map of thumbnail images associated with the playlist. For each object in the
        //     map, the key is the name of the thumbnail image, and the value is an object that
        //     contains other information about the thumbnail.
        [JsonProperty("thumbnails")]
        public virtual ThumbnailDetails Thumbnails { get; set; }

        //
        // Summary:
        //     The playlist's title.
        [JsonProperty("title")]
        public virtual string Title { get; set; }
    }

    public class PlaylistStatus
    {
        public PlaylistStatus()
        { }

        //
        // Summary:
        //     The ETag of the item.
        public virtual string ETag { get; set; }

        //
        // Summary:
        //     The playlist's privacy status.
        [JsonProperty("privacyStatus")]
        public virtual string PrivacyStatus { get; set; }
    }

    #endregion PlaylistResponse

    #region VideoResponse

    public class VideoListResponse : BaseResponse
    {
        [JsonProperty("items")]
        public virtual IList<Video> Items
        {
            get;
            set;
        }

        public VideoListResponse()
        {
        }
    }

    #endregion VideoResponse

    public class BaseResponse
    {
        [JsonProperty("etag")]
        public virtual string ETag
        {
            get;
            set;
        }

        [JsonProperty("eventId")]
        public virtual string EventId
        {
            get;
            set;
        }

        [JsonProperty("kind")]
        public virtual string Kind
        {
            get;
            set;
        }

        [JsonProperty("nextPageToken")]
        public virtual string NextPageToken
        {
            get;
            set;
        }

        [JsonProperty("pageInfo")]
        public virtual PageInfo PageInfo
        {
            get;
            set;
        }

        [JsonProperty("prevPageToken")]
        public virtual string PrevPageToken
        {
            get;
            set;
        }

        [JsonProperty("tokenPagination")]
        public virtual TokenPagination TokenPagination
        {
            get;
            set;
        }

        [JsonProperty("visitorId")]
        public virtual string VisitorId
        {
            get;
            set;
        }
    }

    public class PageInfo
    {
        public virtual string ETag
        {
            get;
            set;
        }

        [JsonProperty("resultsPerPage")]
        public virtual int? ResultsPerPage
        {
            get;
            set;
        }

        [JsonProperty("totalResults")]
        public virtual int? TotalResults
        {
            get;
            set;
        }

        public PageInfo()
        {
        }
    }

    public class TokenPagination
    {
        public virtual string ETag
        {
            get;
            set;
        }

        public TokenPagination()
        {
        }
    }

    public class Video
    {
        [JsonProperty("contentDetails")]
        public virtual VideoContentDetails ContentDetails
        {
            get;
            set;
        }

        [JsonProperty("etag")]
        public virtual string ETag
        {
            get;
            set;
        }

        [JsonProperty("id")]
        public virtual string Id
        {
            get;
            set;
        }

        [JsonProperty("kind")]
        public virtual string Kind
        {
            get;
            set;
        }

        [JsonProperty("snippet")]
        public virtual VideoSnippet Snippet
        {
            get;
            set;
        }

        public Video()
        {
        }
    }

    public class VideoSnippet
    {
        [JsonProperty("categoryId")]
        public virtual string CategoryId
        {
            get;
            set;
        }

        [JsonProperty("channelId")]
        public virtual string ChannelId
        {
            get;
            set;
        }

        [JsonProperty("channelTitle")]
        public virtual string ChannelTitle
        {
            get;
            set;
        }

        [JsonProperty("defaultAudioLanguage")]
        public virtual string DefaultAudioLanguage
        {
            get;
            set;
        }

        [JsonProperty("defaultLanguage")]
        public virtual string DefaultLanguage
        {
            get;
            set;
        }

        [JsonProperty("description")]
        public virtual string Description
        {
            get;
            set;
        }

        public virtual string ETag
        {
            get;
            set;
        }

        [JsonProperty("liveBroadcastContent")]
        public virtual string LiveBroadcastContent
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual DateTime? PublishedAt
        {
            get
            {
                DateTime dateTime;
                if (DateTime.TryParse(this.PublishedAtRaw, out dateTime))
                {
                    return new DateTime?(dateTime);
                }
                return null;
            }
            set
            {
                this.PublishedAtRaw = value.ToString();
            }
        }

        [JsonProperty("publishedAt")]
        public virtual string PublishedAtRaw
        {
            get;
            set;
        }

        [JsonProperty("tags")]
        public virtual IList<string> Tags
        {
            get;
            set;
        }

        [JsonProperty("thumbnails")]
        public virtual ThumbnailDetails Thumbnails
        {
            get;
            set;
        }

        [JsonProperty("title")]
        public virtual string Title
        {
            get;
            set;
        }

        public VideoSnippet()
        {
        }
    }

    public class ThumbnailDetails
    {
        [JsonProperty("default")]
        public virtual Thumbnail Default__
        {
            get;
            set;
        }

        public virtual string ETag
        {
            get;
            set;
        }

        [JsonProperty("high")]
        public virtual Thumbnail High
        {
            get;
            set;
        }

        [JsonProperty("maxres")]
        public virtual Thumbnail Maxres
        {
            get;
            set;
        }

        [JsonProperty("medium")]
        public virtual Thumbnail Medium
        {
            get;
            set;
        }

        [JsonProperty("standard")]
        public virtual Thumbnail Standard
        {
            get;
            set;
        }

        public ThumbnailDetails()
        {
        }
    }

    public class Thumbnail
    {
        public virtual string ETag
        {
            get;
            set;
        }

        [JsonProperty("height")]
        public virtual long? Height
        {
            get;
            set;
        }

        [JsonProperty("url")]
        public virtual string Url
        {
            get;
            set;
        }

        [JsonProperty("width")]
        public virtual long? Width
        {
            get;
            set;
        }

        public Thumbnail()
        {
        }
    }

    public class VideoContentDetails
    {
        [JsonProperty("videoId")]
        public virtual string videoId
        {
            get;
            set;
        }

        [JsonProperty("videoPublishedAt")]
        public virtual string videoPublishedAt
        {
            get;
            set;
        }

        public VideoContentDetails()
        {
        }

        [JsonProperty("duration")]
        public virtual string Duration
        {
            get;
            set;
        }
    }
}