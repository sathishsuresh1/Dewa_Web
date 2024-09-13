using DEWAXP.Feature.CommonComponents.Helpers;
using DEWAXP.Feature.CommonComponents.Models.Footer;
using DEWAXP.Feature.CommonComponents.Models.Gallery;
using DEWAXP.Feature.CommonComponents.Models.Header;
using DEWAXP.Feature.CommonComponents.Models.IdealHomeConsumer;
using DEWAXP.Feature.CommonComponents.Models.ImageMap;
using DEWAXP.Feature.CommonComponents.Models.Persona;
using DEWAXP.Feature.CommonComponents.Models.Renderings;
using DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers;
using DEWAXP.Feature.CommonComponents.Repositories;
using DEWAXP.Feature.CommonComponents.Utils;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LanguageSwitcher = DEWAXP.Feature.CommonComponents.Models.LanguageSwitcher;

namespace DEWAXP.Feature.CommonComponents.Controllers
{
    public class RenderingsController : BaseController
    {
        private ICMSComponentRepositories _CMSComponentRepositories;
        public RenderingsController(ICMSComponentRepositories cMSComponentRepositories)
        {
            _CMSComponentRepositories = cMSComponentRepositories;
        }

        /// <summary>
        /// Header2V1
        /// </summary>
        /// <returns></returns>
        public ActionResult Header()
        {
            // Mobile view header and footer hide from webpages.
            if (_CMSComponentRepositories.MobileAppsView())
            {
                return new EmptyResult();
            }

            // Filter for the main landings based on their templateid (main landing a, b, c and home)
            var mainLandingFilter = String.Format("contains('{0}', @@templateid)", "{0B9C9818-969A-42A4-AF17-F86C837F51DA}|{E68FAAEC-A8AD-4B9D-AC12-BBFA82CF717F}|{8E888B2C-0A52-47C3-8F2B-A3E37680296B}|{816DAADA-314D-4314-98D5-B52957B99BAB}|{5A16A792-DFD8-45E2-A6A4-9401F6CF213C}|" + ScTemplate.DEWAHome_PageTemplateId + "|" + ScTemplate.Persona_PageTemplateId);
            //var subLandingFilter = String.Format("contains('{0}', @@templateid)", "{939234A8-2467-4237-BBA5-6813E7788C34}");

            // Get home and then all main landings
            var home = ContextRepository.GetHomeItem<Item>();
            var mainLandings = home.Axes.SelectItems(String.Format("*[{0}]", mainLandingFilter)).Where(x => x != null)?.Select(x => ContentRepository.GetItem<PersonaLanding>(new Glass.Mapper.Sc.GetItemByItemOptions(x)));

            // Get the ancestor-or-self landing to determine which landing is active
            var currentItem = ContextRepository.GetCurrentItem<Item>();
            var activeMainLanding = ContentRepository.GetItem<ContentBase>(new GetItemByItemOptions(currentItem.Axes.SelectSingleItem(String.Format("ancestor-or-self::*[{0}]", mainLandingFilter)))) ?? mainLandings.First();
            //var activeSubLanding = SitecoreContext.Cast<ContentBase>(currentItem.Axes.SelectSingleItem(String.Format("ancestor-or-self::*[{0}]", subLandingFilter)));

            //Get the MY Account Item
            var myAccountItem = ContentRepository.GetItem<MyAccountMenu>(new GetItemByIdOptions(Guid.Parse("1D1DB969-BB35-4DD0-AE42-08867981FA45")));

            // Initialize the header viewmodel            
            var header = ContentRepository.GetItem<MastheadItem>(new GetItemByIdOptions(Guid.Parse(DataSources.Masthead2v1)));
            var viewModel = new Masthead(ContentRepository.GetItem<Home>(new GetItemByItemOptions(home)), mainLandings, activeMainLanding, header, CurrentPrincipal.Username, myAccountItem, CurrentUserProfilePhoto, CurrentPrincipal.Role);
            //var viewModel = new Masthead(SitecoreContext.Cast<Home>(home), header, CurrentPrincipal.Username, myAccountItem, CurrentUserProfilePhoto, CurrentPrincipal.Role);

            ViewBag.IsRTL = currentItem.Language.CultureInfo.TextInfo.IsRightToLeft;
            ViewBag.IsLoggedIn = IsLoggedIn;
            viewModel.CustomerProfile = AuthStateService.GetActiveProfile();//CurrentPrincipal.IsContactUpdated;


            return PartialView("~/Views/Feature/CommonComponents/Renderings/Header/Header.cshtml", viewModel);
        }

        public ActionResult HeaderNoMenu()
        {
            // Mobile view header and footer hide from webpages.
            if (_CMSComponentRepositories.MobileAppsView())
            {
                return new EmptyResult();
            }
            // Get the ancestor-or-self landing to determine which landing is active
            var currentItem = ContextRepository.GetCurrentItem<Item>();

            // Initialize the header viewmodel            
            var header = ContentRepository.GetItem<Header>(new GetItemByPathOptions("/sitecore/content/global references/header and footer/header no menu"));
            var viewModel = new Masthead(header);

            ViewBag.IsRTL = currentItem.Language.CultureInfo.TextInfo.IsRightToLeft;
            ViewBag.IsLoggedIn = IsLoggedIn;
            return PartialView("~/Views/Feature/CommonComponents/Renderings/Header/M12 Masthead No Menu.cshtml", viewModel);
        }
        public PartialViewResult HeaderOutage()
        {
            // Get the ancestor-or-self landing to determine which landing is active
            var home = ContextRepository.GetHomeItem<Item>();
            // Initialize the header viewmodel            
            var header = ContentRepository.GetItem<Header>(new GetItemByPathOptions("/sitecore/content/global references/header and footer/header no menu"));
            var viewModel = new Masthead(ContentRepository.GetItem<Home>(new GetItemByItemOptions(home)), header);

            ViewBag.IsRTL = Sitecore.Context.Language.CultureInfo.TextInfo.IsRightToLeft;
            ViewBag.IsLoggedIn = IsLoggedIn;
            return PartialView("~/Views/Feature/CommonComponents/Renderings/Header/M12 Mastheadoutage.cshtml", viewModel);
        }

        public ActionResult Footer()
        {
            // Mobile view header and footer hide from webpages.
            if (_CMSComponentRepositories.MobileAppsView())
            {
                return new EmptyResult();
            }
            var currentItem = ContextRepository.GetCurrentItem<Item>();
            var footer = ContentRepository.GetItem<Footer>(new GetItemByPathOptions("/sitecore/content/Global References/Header and Footer/Footer2"));

            DewaProfile _userProfile = AuthStateService.GetActiveProfile();
            if (_userProfile != null)
            {
                if (_userProfile.PopupFlag)
                {
                    Item PromotionalModalPopupItem = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(SitecoreItemIdentifiers.PromotionalModalPopup));
                    if (PromotionalModalPopupItem != null)
                    {
                        footer.PopupModal = PromotionalModalPopupItem["Rich Text"];
                        footer.IsPopupModal = true;
                    }
                }
            }
            if (currentItem != null)
            {
                //Check if it is home then display the last publish date ELSE display item last update date
                if (currentItem.ID.ToString() == SitecoreItemIdentifiers.HOME)
                {
                    footer.Updated = UtilSitecore.GetLastPublishDate();
                }
                else if (currentItem.ID.ToString() == SitecoreItemIdentifiers.ABOUTDEWA || currentItem.ID.ToString() == SitecoreItemIdentifiers.LATESTNEWS || currentItem.ID.ToString() == SitecoreItemIdentifiers.NEWSANDMEDIA)
                {
                    footer.Updated = UtilSitecore.GetLatestNewsPublishDate();
                }
                else
                {
                    footer.Updated = currentItem.Statistics.Updated;
                }
            }
            return PartialView("~/Views/Feature/CommonComponents/Renderings/Footer/Footer.cshtml", footer);
        }

        public PartialViewResult FooterNoMenu()
        {
            var currentItem = ContextRepository.GetCurrentItem<Item>();
            var footer = ContentRepository.GetItem<Footer>(new GetItemByPathOptions("/sitecore/content/global references/header and footer/footer no menu"));
            if (currentItem != null)
                footer.Updated = currentItem.Statistics.Updated;
            return PartialView("~/Views/Feature/CommonComponents/Renderings/Footer/Footer No Menu.cshtml", footer);
        }

        public PartialViewResult Languages(string tabindex, int type = 0)
        {
            var qs = QueryString.Parse(Request.RawUrl);
            var currentItem = ContextRepository.GetCurrentItem<Item>();
            var currentLanguage = Sitecore.Context.Language;
            var otherLanguage = Sitecore.Context.Database.Languages.FirstOrDefault(l => l.Name != currentLanguage.Name);
            var hasOtherLanguage = ContentRepository.GetItem<Item>(new GetItemByIdOptions(currentItem.ID.Guid) { Language = otherLanguage }) != null;

            var options = Sitecore.Links.LinkManager.GetDefaultUrlBuilderOptions();
            options.Language = otherLanguage;

            var altUrl = qs.CopyTo(Sitecore.Links.LinkManager.GetItemUrl(currentItem, options));
            var langViewModel = new LanguageSwitcher.LanguageSwitcher(currentLanguage, otherLanguage, hasOtherLanguage, altUrl, tabindex);
            langViewModel.SwitcherType = type;

            return PartialView("~/Views/Feature/CommonComponents/Renderings/Language/_LanguageSwitcher.cshtml", langViewModel);
        }
        [HttpPost]
        public PartialViewResult ListingV1(Guid itemId, int? month, int? year, int page = 1)//Used in Listing page
        {
            int childrencount;
            IEnumerable<Article> filteredSet;
            //NewsHelper newsHelper = new NewsHelper();
            filteredSet = NewsHelper.GetNewsListingNew(itemId, month, year, out childrencount);
            //filteredSet = NewsHelper.GetNewsListingIndex(itemId, month==null?int.MinValue:month.Value, year==null?int.MinValue:year.Value,0,0, out childrencount,true);
            var model = ContentRepository.GetItem<ArticleListing>(new GetItemByIdOptions(itemId));
            //var model = ContentRepository.GetItem<ArticleListing>(new GetItemByItemOptions(ContentRepository.GetItem<Item>(new GetItemByIdOptions(itemId))));

            // Apply paging
            var pagedChildren = filteredSet.ApplyPaging(page, model.PageSize).ToList();

            // Setup the model
            var totalPages = Pager.CalculateTotalPages(childrencount, model.PageSize);
            model.Children = pagedChildren;
            model.Pagination = new PaginationModel("ListingV1", "Renderings", page, totalPages);

            return PartialView("~/Views/Feature/CommonComponents/Renderings/Articles/_ArticleListing.cshtml", model);
        }

        public ActionResult M1Hero()
        {
            if (RenderingRepository.HasDataSource)
            {
                var herodatasource = RenderingRepository.GetDataSourceItem<HeroDatasource>(); //SitecoreContext.GetItem<HeroDatasource>(RenderingContext.Current.Rendering.DataSource);
                int herocount;
                if (herodatasource != null)
                {
                    herocount = herodatasource.numberofhero != null
                        && ContentRepository.GetItem<DataSourceTemplateValue>(new GetItemByItemOptions(herodatasource.numberofhero.Item)) != null
                        && !string.IsNullOrWhiteSpace((ContentRepository.GetItem<DataSourceTemplateValue>(new GetItemByItemOptions(herodatasource.numberofhero.Item)).value))
                        ? Convert.ToInt32(ContentRepository.GetItem<DataSourceTemplateValue>(new GetItemByItemOptions(herodatasource.numberofhero.Item)).value) : 3;
                    var listhero = herodatasource.HeroList.ToList().Take(herocount).ToList();
                    return PartialView("~/Views/Feature/CommonComponents/Renderings/Hero/M1-2 Hero.cshtml", listhero);
                }
            }
            return new EmptyResult();
        }

        public ActionResult M1HeroV1()
        {
            if (RenderingRepository.HasDataSource)
            {
                HeroSilderSet heroDatasource = RenderingRepository.GetDataSourceItem<HeroSilderSet>();//ContentRepository.GetItem<HeroSilderSet>(RenderingContext.Current.Rendering.DataSource);

                if (heroDatasource != null && heroDatasource.Children.Any())
                {
                    var filterHeroSliders = heroDatasource.Children?.Where(x => x.IsDisabled != "1")?.ToList().Take(Convert.ToInt32(heroDatasource.SlideLimitCount.DataValue)).ToList();
                    return PartialView("~/Views/Feature/CommonComponents/Renderings/Hero/M1-2 Herov1.cshtml", filterHeroSliders);
                }
            }
            return new EmptyResult();

        }
        public ActionResult SubLandingFolder()
        {
            var parentItem = ContextRepository.GetCurrentItem<Item>().Parent;
            var redirectUrl = Sitecore.Links.LinkManager.GetItemUrl(parentItem);
            return RedirectPermanent(redirectUrl);
        }

        public ActionResult Downloads(Guid itemId, int page = 1)
        {
            var listing = ContentRepository.GetItem<Item>(new GetItemByIdOptions(itemId));
            var children = listing.Axes.GetDescendants()
                .Select(c => ContentRepository.GetItem<Download>(new GetItemByItemOptions(c)))
                .ToList();

            var model = ContentRepository.GetItem<DownloadListing>(new GetItemByItemOptions(listing));

            // Apply paging
            var pagedChildren = children.ApplyPaging(page, model.PageSize).ToList();

            // Setup the model
            var totalPages = Pager.CalculateTotalPages(children.Count, model.PageSize);
            model.Children = pagedChildren;
            model.Pagination = new PaginationModel("Downloads", "Renderings", page, totalPages);

            return PartialView("~/Views/Feature/CommonComponents/Renderings/Generic/_DownloadListing.cshtml", model);
        }


        public PartialViewResult M3NewsCarousal()//not in use
        {

            int childrencount;
            //NewsHelper newsHelper = new NewsHelper();
            IEnumerable<Article> filteredSet = NewsHelper.GetNewsListing(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE),
                out childrencount);
            var model = ContentRepository.GetItem<ArticleListing>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE)));

            // Setup the model
            var enumerable = filteredSet as IList<Article> ?? filteredSet.ToList();
            if (enumerable.Any())
            {
                var homepagearticles = enumerable.Take(3);

                model.Children = homepagearticles;
            }


            return PartialView("~/Views/Feature/CommonComponents/Renderings/Carousel/M3 News Carousel.cshtml", model);
        }

        public PartialViewResult M9LatestNews()//not in use
        {

            int childrencount;
            //NewsHelper newsHelper = new NewsHelper();
            IEnumerable<Article> filteredSet = NewsHelper.GetNewsListing(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE),
                out childrencount);
            var model = ContentRepository.GetItem<ArticleListing>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE)));

            // Setup the model
            var enumerable = filteredSet as IList<Article> ?? filteredSet.ToList();
            if (enumerable.Any())
            {
                var homepagearticles = enumerable.Take(4);

                model.Children = homepagearticles;
            }


            return PartialView("~/Views/Feature/CommonComponents/Renderings/Teasers/M9 Teaser News Set.cshtml", model);
        }

        public ActionResult M9LatestNewsV1()
        {
            int childrencount;
            //NewsHelper newsHelper = new NewsHelper();
            IEnumerable<Article> filteredSet = NewsHelper.GetNewsListing(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE), out childrencount);
            //IEnumerable<Article> filteredSet = NewsHelper.GetNewsListingIndex(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE), int.MinValue, int.MinValue, 0, 2, out childrencount);
            if (childrencount > 0)
            {

                var model = ContentRepository.GetItem<ArticleListing>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE)));

                // Setup the model
                var enumerable = filteredSet as IList<Article> ?? filteredSet.ToList();
                if (enumerable.Any())
                {
                    var homepagearticles = enumerable.Take(2);

                    model.Children = homepagearticles;
                }


                return PartialView("~/Views/Feature/CommonComponents/Renderings/Teasers/M9 Teaser News Set.cshtml", model);
            }
            return new EmptyResult();
        }

        public PartialViewResult M65GalleryTeaser()
        {
            var model = new M65GalleryTeaserSet();
            int imgcount;
            int videocount;
            GalleryHelper galleryHelper = new GalleryHelper();
            model.VideoChildren = galleryHelper.GetGallery(Guid.Parse(SitecoreItemIdentifiers.VIDEO_GALLERY), out imgcount);
            model.ImageChildren = galleryHelper.GetGallery(Guid.Parse(SitecoreItemIdentifiers.IMAGE_GALLERY), out videocount);

            return PartialView("~/Views/Feature/CommonComponents/Renderings/Teasers/M65 Gallery Teaser Set.cshtml", model);
        }

        public ActionResult M3v1newscampaignservices()//not in use
        {
            NewsCampaignServices newsCampaignServices = new NewsCampaignServices();
            //var contextid = RenderingContext.Current.Rendering.DataSource;

            if (RenderingRepository.HasDataSource)
            {
                var newscampaignservicedatasourceitem = RenderingRepository.GetDataSourceItem<NewsCampaignServices>(); // SitecoreContext.GetItem<NewsCampaignServices>(contextid);
                if (newscampaignservicedatasourceitem != null)
                {
                    newsCampaignServices = newscampaignservicedatasourceitem;
                }
            }
            else
            {
                return new EmptyResult();
            }
            int childrencount;
            //NewsHelper newsHelper = new NewsHelper();
            IEnumerable<Article> filteredSet = NewsHelper.GetNewsListing(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE), out childrencount);
            //IEnumerable<Article> filteredSet = NewsHelper.GetNewsListingIndex(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE),int.MinValue,int.MinValue,0,3,out childrencount);
            ArticleListing articles = ContentRepository.GetItem<ArticleListing>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE)));

            // Setup the model
            var enumerable = filteredSet as IList<Article> ?? filteredSet.ToList();
            if (enumerable.Any())
            {
                var homepagearticles = enumerable.Take(3);

                articles.Children = homepagearticles;
            }
            newsCampaignServices.articlesListing = articles;


            return PartialView("~/Views/Feature/CommonComponents/Renderings/Teasers/M3v1 NewsCampaignServices.cshtml", newsCampaignServices);
        }

        public PartialViewResult LatestUpdateListing() //Not in use
        {
            var currentItem = ContextRepository.GetCurrentItem<Item>();
            Guid itemId = Guid.Parse(SitecoreItemIdentifiers.LATESTUPDATESPAGE);
            int childrencount;
            IEnumerable<LatestUpdate> filteredSet = NewsHelper.GetLatestUpdates(itemId, out childrencount);
            if (filteredSet == null)
            {
                return PartialView("~/Views/Feature/CommonComponents/Renderings/Generic/_LatestUpdates.cshtml", null);
            }
            var model = ContentRepository.GetItem<LatestUpdateListing>(new GetItemByIdOptions(itemId));
            filteredSet = filteredSet.RemoveWhere(c => c.Id == currentItem.ID.Guid);

            model.Children = filteredSet;
            if (currentItem.ID.Guid == Guid.Parse(SitecoreItemIdentifiers.LATESTUPDATESPAGE))
            {
                model.ShowFirstDetail = true;
            }

            return PartialView("~/Views/Feature/CommonComponents/Renderings/Generic/_LatestUpdates.cshtml", model);
        }

        public ActionResult VideoPlaylists()
        {
            var videoPlaylists = new YoutubeVideoPlaylists();
            var youtubeFolder = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.YOUTUBE_VIDEOS_FOLDER)));
            if (Sitecore.Context.Language.Name.Equals("ar-AE"))
            {
                youtubeFolder = ContentRepository.GetItem<Item>(new GetItemByPathOptions(SitecoreItemPaths.YOUTUBE_VIDEOS_FOLDER_AR));
            }
            else
            {
                youtubeFolder = ContentRepository.GetItem<Item>(new GetItemByPathOptions(SitecoreItemPaths.YOUTUBE_VIDEOS_FOLDER_EN));
            }

            if (youtubeFolder != null && youtubeFolder.HasChildren)
            {
                var playlists = youtubeFolder.Axes.GetDescendants().Select(x => ContentRepository.GetItem<YoutubeVideoPlaylist>(new GetItemByItemOptions(x))).ToList().Where(x => !string.IsNullOrWhiteSpace(x.PlaylistId) && x.Deactivate == false);

                if (playlists != null && playlists.Any())
                {
                    videoPlaylists.Playlists = playlists.ToList();

                    var videoPlaylistPage = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.VIDEOPLAYLISTPAGE)));

                    if (videoPlaylistPage != null)
                    {
                        foreach (var videoPlaylist in videoPlaylists.Playlists)
                        {
                            videoPlaylist.VideoPlaylistPageLink = Sitecore.Links.LinkManager.GetItemUrl(videoPlaylistPage) + "?id=" + videoPlaylist.Id.ToString();
                        }
                    }

                    return PartialView("~/Views/Feature/CommonComponents/Renderings/VideoComponent/VideoPlaylists.cshtml", videoPlaylists);
                }
            }

            return PartialView("~/Views/Feature/CommonComponents/Renderings/VideoComponent/VideoPlaylists.cshtml", videoPlaylists);
        }

        public ActionResult VideoGallery()
        {
            if (RenderingRepository.HasDataSource)
            {
                var playlistItemList = RenderingRepository.GetDataSourceItem<YoutubeVideoPlaylist>();// SitecoreContext.GetItem<YoutubeVideoPlaylist>(RenderingContext.Current.Rendering?.DataSource);

                string playlistId = Request.QueryString["id"];

                if (!string.IsNullOrWhiteSpace(playlistId))
                {
                    List<YoutubeVideo> videosList = new List<YoutubeVideo>();
                    var selectVideo = playlistItemList.Videos.Where(x => x.VideoId == playlistId).FirstOrDefault();
                    if (selectVideo != null)
                    {
                        videosList.Add(selectVideo);
                    }

                    foreach (YoutubeVideo item in playlistItemList.Videos.Where(x => x.VideoId != playlistId))
                    {
                        videosList.Add(item);
                    }
                    playlistItemList.Videos = videosList;
                }

                if (playlistItemList != null)
                {
                    return PartialView("~/Views/Feature/CommonComponents/Renderings/VideoComponent/VideoGallery.cshtml", playlistItemList);
                }
            }

            return new EmptyResult();
        }

        #region [Career Fair]
        public PartialViewResult HeaderCareerFair()
        {
            // Get the ancestor-or-self landing to determine which landing is active
            var currentItem = ContextRepository.GetCurrentItem<Item>();

            // Initialize the header viewmodel            
            var header = ContentRepository.GetItem<Header>(new GetItemByPathOptions("/sitecore/content/global references/header and footer/header no menu"));
            var viewModel = new Masthead(header);

            ViewBag.IsRTL = currentItem.Language.CultureInfo.TextInfo.IsRightToLeft;
            ViewBag.IsLoggedIn = IsLoggedIn;
            return PartialView("~/Views/Feature/CommonComponents/Renderings/Header/M12 Masthead Career Fair.cshtml", viewModel);
        }

        public PartialViewResult CareerFairStickyFooter()
        {
            // Get the ancestor-or-self landing to determine which landing is active
            var currentItem = ContextRepository.GetCurrentItem<Item>();

            // Initialize the header viewmodel            
            var header = ContentRepository.GetItem<Header>(new GetItemByPathOptions("/sitecore/content/Global References/Header and Footer/Career Fair Sticky Footer"));
            var viewModel = new Masthead(header);

            ViewBag.IsRTL = currentItem.Language.CultureInfo.TextInfo.IsRightToLeft;
            ViewBag.IsLoggedIn = IsLoggedIn;
            return PartialView("~/Views/Feature/CommonComponents/Renderings/Header/M13 Career Fair Sticky footer.cshtml", viewModel);
        }
        #endregion
        [HttpGet]
        public ActionResult RadioButtonComponent()
        {
            if (RenderingRepository.HasDataSource)
            {
                var context = RenderingRepository.GetDataSourceItem<RadioButtoncomponent>();// SitecoreContext.GetItem<RadioButtoncomponent>(RenderingContext.Current.Rendering.DataSource);
                if (context != null && context.Children != null && context.Children.Count() > 0)
                {
                    context.datasourceid = RenderingRepository.DataSourceItem.ID.ToString();
                    context.IsLoggedIn = IsLoggedIn;
                    return PartialView("~/Views/Feature/CommonComponents/Renderings/Generic/RadioButtonComponent.cshtml", context);
                }
            }
            return new EmptyResult();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RadioButtonComponent(FormCollection collection)
        {
            string redirectItem = string.Empty;

            if (collection != null)
            {
                int Selvalue = 0;
                if (collection.Keys.Count > 0)
                {
                    int.TryParse(collection.Get("radios_group1"), out Selvalue);
                    string datasource = collection.Get("datasourceid");
                    var context = ContentRepository.GetItem<RadioButtoncomponent>(new GetItemByIdOptions(Guid.Parse(datasource)));
                    var result = context.Children.Select((item, index) => new { index, item });
                    var selectedindex = result.Where(x => x.index.Equals(Selvalue));
                    if (selectedindex.HasAny())
                    {
                        var link = selectedindex.FirstOrDefault().item.URLLink;
                        if (link != null && !string.IsNullOrWhiteSpace(link.Url))
                        {
                            return Redirect(link.Url);
                            //return RedirectToSitecoreItem(link.TargetId.ToString());
                        }
                    }
                }
            }
            return new EmptyResult();
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult M41PersonaTabsBox()
        {
            if (RenderingRepository.HasDataSource)
            {
                PersonaTabBox data = RenderingRepository.GetDataSourceItem<PersonaTabBox>();// SitecoreContext.GetItem<DEWAXP.Feature.CommonComponents.Models.V1.Persona.PersonaTabBox>(RenderingContext.Current.Rendering.DataSource);

                if (data != null && data.PersonaList.Any())
                {
                    return PartialView("~/Views/Feature/CommonComponents/Renderings/Persona/M41-PersonaTabsBox.cshtml", data);
                }

            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult M4PersonaMasonary()
        {
            if (RenderingRepository.HasDataSource)
            {
                PersonaCategory data = RenderingRepository.GetDataSourceItem<PersonaCategory>();// SitecoreContext.GetItem<DEWAXP.Feature.CommonComponents.Models.V1.Persona.PersonaCategory>(RenderingContext.Current.Rendering.DataSource);

                if (data != null && data.PersonaSectionMenuList.Any())
                {
                    return PartialView("~/Views/Feature/CommonComponents/Renderings/Persona/M4_Masonary.cshtml", data);
                }

            }
            return new EmptyResult();
        }


        public ActionResult M19Carousel()
        {
            if (RenderingRepository.HasDataSource)
            {
                ImageCarouselList data = RenderingRepository.GetDataSourceItem<ImageCarouselList>();// SitecoreContext.GetItem<Models.V1.Rendering.ImageCarouselList>(RenderingContext.Current.Rendering.DataSource);
                return PartialView("~/Views/Feature/CommonComponents/Renderings/Carousel/m19-ql-carousel.cshtml", data);
            }
            return new EmptyResult();
        }

        public ActionResult M18QuickTools()
        {
            if (RenderingRepository.HasDataSource)
            {
                QuickImageLinksSection data = RenderingRepository.GetDataSourceItem<QuickImageLinksSection>();// SitecoreContext.GetItem<Models.V1.Rendering.ImageCarouselList>(RenderingContext.Current.Rendering.DataSource);
                return PartialView("~/Views/Feature/CommonComponents/Renderings/Teasers/m18-quick-tools.cshtml", data);
            }
            return new EmptyResult();
        }

        //M16SmartApp
        public ActionResult M16SmartApp()
        {
            if (RenderingRepository.HasDataSource)
            {
                SmartAppSection data = RenderingRepository.GetDataSourceItem<SmartAppSection>();// SitecoreContext.GetItem<Models.V1.Rendering.ImageCarouselList>(RenderingContext.Current.Rendering.DataSource);
                return PartialView("~/Views/Feature/CommonComponents/Renderings/Teasers/m16-smart-app.cshtml", data);
            }
            return new EmptyResult();
        }

        public ActionResult M9TeaserSlider()
        {
            if (RenderingRepository.HasDataSource)
            {
                CarouselSet data = RenderingRepository.GetDataSourceItem<CarouselSet>();// SitecoreContext.GetItem<Models.V1.Rendering.ImageCarouselList>(RenderingContext.Current.Rendering.DataSource);
                return PartialView("~/Views/Feature/CommonComponents/Renderings/Teasers/M9 Teaser Slider.cshtml", data);
            }
            return new EmptyResult();
        }

        public ActionResult M6TeaserSliderWithIcon()
        {
            if (RenderingRepository.HasDataSource)
            {
                CarouselSet data = RenderingRepository.GetDataSourceItem<CarouselSet>();// SitecoreContext.GetItem<Models.V1.Rendering.ImageCarouselList>(RenderingContext.Current.Rendering.DataSource);
                return PartialView("~/Views/Feature/CommonComponents/Renderings/Teasers/M6TeaserSliderWithIcon.cshtml", data);
            }
            return new EmptyResult();
        }

        public ActionResult VideoPlayerModalPopup()
        {
            if (RenderingRepository.HasDataSource)
            {
                VideoPlayerModalPopup data = RenderingRepository.GetDataSourceItem<VideoPlayerModalPopup>();// SitecoreContext.GetItem<Models.V1.Rendering.ImageCarouselList>(RenderingContext.Current.Rendering.DataSource);
                return PartialView("~/Views/Feature/CommonComponents/Renderings/VideoComponent/VideoPlayerModalPopup.cshtml", data);
            }
            return new EmptyResult();
        }

        public ActionResult ImageMapTabSet()
        {
            if (RenderingRepository.HasDataSource)
            {
                ImageMapTabs data = RenderingRepository.GetDataSourceItem<ImageMapTabs>();// SitecoreContext.GetItem<Models.V1.Rendering.ImageCarouselList>(RenderingContext.Current.Rendering.DataSource);
                return PartialView("~/Views/Feature/CommonComponents/Renderings/Tabs/ImageMapTabs.cshtml", data);
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult M79DashboardTools()
        {
            if (RenderingRepository.HasDataSource)
            {
                M79DashboardTool dashboardtooldatasource = RenderingRepository.GetDataSourceItem<M79DashboardTool>();
                if (dashboardtooldatasource != null)
                {
                    dashboardtooldatasource.FullName = CurrentPrincipal.FullName;
                    return View("~/Views/Feature/CommonComponents/Renderings/Teasers/M79 Dashboard Tools.cshtml", dashboardtooldatasource);
                }
            }
            return new EmptyResult();

        }

        [HttpGet]
        public ActionResult PhotoAlbumWithFilter()
        {
            MediaGalleryModel model = new MediaGalleryModel();
            var currentItem = ContextRepository.GetCurrentItem<Item>();
            var PhotoAlbumsList = currentItem.Axes.GetDescendants().Where(x => x.TemplateID.Guid == Guid.Parse(SitecoreItemIdentifiers.T2ArticalPageTemplateID)).Select(x => ContentRepository.GetItem<M9Teaser>(new GetItemByItemOptions(x)));

            if (PhotoAlbumsList != null)
            {
                model.FilterDataList = PhotoAlbumsList.GroupBy(x => x.AlbumCategory.DataValue).Select(x => x.Key).ToList();

                if (model.FilterDataList != null && model.FilterDataList.Count > 0)
                {
                    model.AlbumPageList = new List<BaseMediaListItem>();
                    foreach (string albumType in model.FilterDataList)
                    {
                        var groupData = new BaseMediaListItem()
                        {
                            Key = albumType,
                            MediaItems = new List<M9Teaser>(),
                        };

                        foreach (var mediaItem in PhotoAlbumsList.Where(x => x.AlbumCategory.DataValue == albumType))
                        {
                            groupData.MediaItems.Add(mediaItem);
                        }
                        model.AlbumPageList.Add(groupData);
                    }
                }

                return View("~/Views/Feature/CommonComponents/Renderings/Gallery/_PhotoAlbumWithFilter.cshtml", model);
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult M9TeasersetWithTextSearch(string i = null, string s = "", int p = 1, bool m = false)
        {
            M9TeaserSetWithSearchTextModel model = new M9TeaserSetWithSearchTextModel()
            {
                SearchText = s,
                CurrentPage = ContextRepository.GetCurrentItem<ContentBase>(),
            };
            try
            {
                int totalPages = 10;
                DEWAXP.Feature.CommonComponents.Models.Renderings.CarouselSet teaserSet = null;
                if (string.IsNullOrEmpty(i))
                {
                    if (RenderingRepository.HasDataSource)
                    {
                        teaserSet = RenderingRepository.GetDataSourceItem<CarouselSet>();// SitecoreContext.GetItem<DEWAXP.Feature.CommonComponents.Models.Renderings.CarouselSet>(RenderingContext.Current.Rendering.DataSource);
                    }
                }
                else
                {
                    teaserSet = ContentRepository.GetItem<CarouselSet>(new GetItemByIdOptions(new Guid(i)));
                }

                if (teaserSet != null)
                {
                    var FilteredData = teaserSet.Children.Where(x => Convert.ToBoolean(x.Header?.ToLower().Contains(model.SearchText?.ToLower())));
                    model.CarouselSlides = FilteredData?.ApplyPaging(p, teaserSet.PageSize)?.ToList();
                    model.DatasourceId = teaserSet.Id.ToString();
                    totalPages = Pager.CalculateTotalPages(FilteredData.Count(), teaserSet.PageSize);
                    model.PaginationInfo = new PaginationModel("M9TeasersetWithTextSearch", "Renderings", p, totalPages);


                    if (m)
                    {
                        return View("~/Views/Feature/CommonComponents/Renderings/Teasers/M9TeaserSetLoadedSection.cshtml", model);
                    }
                    return View("~/Views/Feature/CommonComponents/Renderings/Teasers/M9TeasersetWithTextSearch.cshtml", model);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return new EmptyResult();
        }


        [HttpGet]
        public ActionResult M9TeasersetWithYearFilter(string i = null, string s = "", int p = 1, bool m = false)
        {
            M9TeasersetWithYearFilterModel model = new M9TeasersetWithYearFilterModel()
            {
                SearchText = s,
                CurrentPage = ContextRepository.GetCurrentItem<ContentBase>(),
            };
            try
            {
                int totalPages = 10;
                DEWAXP.Feature.CommonComponents.Models.Renderings.CarouselSet teaserSet = null;
                if (string.IsNullOrEmpty(i))
                {
                    if (RenderingRepository.HasDataSource)
                    {
                        teaserSet = RenderingRepository.GetDataSourceItem<CarouselSet>();// SitecoreContext.GetItem<DEWAXP.Feature.CommonComponents.Models.Renderings.CarouselSet>(RenderingContext.Current.Rendering.DataSource);
                    }
                }
                else
                {
                    teaserSet = ContentRepository.GetItem<CarouselSet>(new GetItemByIdOptions(new Guid(i)));
                }

                if (teaserSet != null)
                {
                    var sortedItemlist = teaserSet.Children.Where(x => x.CustomPublishedDate != null).OrderByDescending(x => x.CustomPublishedDate.ToString("yyyyMMdd"));
                    model.FilterDataList = sortedItemlist.Where(x => x.CustomPublishedDate != null).Select(x => x.CustomPublishedDate.ToString("yyyy")).Distinct().OrderByDescending(x => x).ToList();

                    var FilteredData = sortedItemlist.Where(x => Convert.ToBoolean((x.Header + x.CustomPublishedDate.ToString("yyyy"))?.ToLower().Contains(model.SearchText?.ToLower())));

                    model.CarouselSlides = FilteredData?.ApplyPaging(p, teaserSet.PageSize)?.ToList();
                    model.DatasourceId = teaserSet.Id.ToString();
                    totalPages = Pager.CalculateTotalPages(FilteredData.Count(), teaserSet.PageSize);
                    model.PaginationInfo = new PaginationModel("M9TeasersetWithYearFilter", "Renderings", p, totalPages);


                    if (m)
                    {
                        return View("~/Views/Feature/CommonComponents/Renderings/Teasers/M9TeaserSetLoadedSectionWithYearFilter.cshtml", model);
                    }
                    return View("~/Views/Feature/CommonComponents/Renderings/Teasers/M9TeasersetWithYearFilter.cshtml", model);
                }
            }
            catch (Exception ex)
            {

                LogService.Error(ex, this);
            }
            return new EmptyResult();
        }



        [HttpGet]
        public ActionResult VideoGalleryWithFilter(string i = null, string s = "", string li = null, int p = 1, bool m = false)
        {
            MediaVidoeGalleryModel model = new MediaVidoeGalleryModel()
            {
                SearchText = s,
                CurrentPage = ContextRepository.GetCurrentItem<ContentBase>(),
            };
            try
            {
                int totalPages = 10;
                DEWAXP.Feature.CommonComponents.Models.Renderings.YoutubeVideoPlaylist youtubePlayList = null;
                if (string.IsNullOrEmpty(i))
                {
                    if (RenderingRepository.HasDataSource)
                    {
                        youtubePlayList = RenderingRepository.GetDataSourceItem<YoutubeVideoPlaylist>();// SitecoreContext.GetItem<DEWAXP.Feature.CommonComponents.Models.Renderings.CarouselSet>(RenderingContext.Current.Rendering.DataSource);
                    }
                }
                else
                {
                    youtubePlayList = ContentRepository.GetItem<YoutubeVideoPlaylist>(new GetItemByIdOptions(new Guid(i)));
                }

                if (youtubePlayList != null)
                {
                    IEnumerable<YoutubeVideo> FilteredData = null;
                    var sortedItemlist = youtubePlayList.Videos.Where(x => x.AlbumCategory != null);
                    model.FilterDataList = sortedItemlist.Where(x => x.AlbumCategory != null).Select(x => x.AlbumCategory.DataValue).Distinct().OrderByDescending(x => x)?.ToList();

                    if (!string.IsNullOrWhiteSpace(li))
                    {
                        model.SelectedFilter = li;
                        FilteredData = sortedItemlist.Where(x => x.AlbumCategory.DataValue == model.SelectedFilter && Convert.ToBoolean(x.Title?.ToLower().Contains(model.SearchText?.ToLower())));
                    }
                    else
                    {
                        FilteredData = sortedItemlist.Where(x => Convert.ToBoolean(x.Title?.ToLower().Contains(model.SearchText?.ToLower())));
                    }

                    model.Slides = FilteredData?.ApplyPaging(p, youtubePlayList.PageSize)?.ToList();
                    model.DatasourceId = youtubePlayList.Id.ToString();
                    totalPages = Pager.CalculateTotalPages(FilteredData.Count(), youtubePlayList.PageSize);
                    model.PaginationInfo = new PaginationModel("VideoGalleryWithFilter", "Renderings", p, totalPages);


                    if (m)
                    {
                        return View("~/Views/Feature/CommonComponents/Renderings/Gallery/_VideoGalleryWithFilterLoadSection.cshtml", model);
                    }
                    return View("~/Views/Feature/CommonComponents/Renderings/Gallery/_VideoGalleryWithFilter.cshtml", model);
                }
            }
            catch (Exception ex)
            {

                LogService.Error(ex, this);
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult AllVideoGalleryWithFilter()
        {
            MediaVidoeGalleryModel model = new MediaVidoeGalleryModel()
            {
                CurrentPage = ContextRepository.GetCurrentItem<ContentBase>(),
            };
            try
            {
                if (RenderingRepository.HasDataSource)
                {
                    var youtubePlayList = RenderingRepository.GetDataSourceItem<YoutubeVideoPlaylist>();
                    if (youtubePlayList != null)
                    {

                        var sortedItemlist = youtubePlayList.Videos.Where(x => x.AlbumCategory != null);
                        model.FilterDataList = sortedItemlist.Where(x => x.AlbumCategory != null).Select(x => x.AlbumCategory.DataValue).Distinct().OrderByDescending(x => x)?.ToList();

                        if (model.FilterDataList != null && model.FilterDataList.Count > 0)
                        {
                            model.AlbumPageList = new List<BaseMediaVideoListItem>();
                            foreach (string albumType in model.FilterDataList)
                            {
                                var groupData = new BaseMediaVideoListItem()
                                {
                                    Key = albumType,
                                    MediaItems = new List<YoutubeVideo>(),
                                };

                                foreach (var mediaItem in youtubePlayList.Videos.Where(x => x.AlbumCategory.DataValue == albumType))
                                {
                                    groupData.MediaItems.Add(mediaItem);
                                }
                                model.AlbumPageList.Add(groupData);
                            }
                        }

                        return View("~/Views/Feature/CommonComponents/Renderings/Gallery/_AllVideoGalleryWithFilter.cshtml", model);
                    }
                }
            }
            catch (Exception ex)
            {

                LogService.Error(ex, this);
            }
            return new EmptyResult();
        }
        /// <summary>
        /// sitecore Item path : /sitecore/layout/Renderings/DEWA Rendering/Generic/RegisterSuccessNotificationBar
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("GET", "HEAD")]
        public ActionResult RegisterSuccessNotificationBar()
        {
            bool isShow = false;
            if (CurrentPrincipal.IsFirstRegistration || Convert.ToBoolean(CacheProvider.TryGet("SHOWSUCCESS" + CurrentPrincipal.PrimaryAccount, out isShow) && isShow))
            {
                CacheProvider.Store("SHOWSUCCESS" + CurrentPrincipal.PrimaryAccount, new CacheItem<bool>(CurrentPrincipal.IsFirstRegistration, TimeSpan.FromHours(1)));
                CurrentPrincipal.IsFirstRegistration = false;

                return View("~/Views/Feature/CommonComponents/Renderings/Notification/RegisterSuccessNotificationBar.cshtml");
            }
            CacheProvider.Remove("SHOWSUCCESS" + CurrentPrincipal.PrimaryAccount);
            return new EmptyResult();
        }
        public ActionResult PODSmallHeader()
        {
            // Mobile view header and footer hide from webpages.
            if (_CMSComponentRepositories.MobileAppsView())
            {
                return new EmptyResult();
            }
            // Get the ancestor-or-self landing to determine which landing is active
            var currentItem = ContextRepository.GetCurrentItem<Item>();

            // Initialize the header viewmodel            
            var header = ContentRepository.GetItem<Header>(new GetItemByPathOptions("/sitecore/content/global references/header and footer/header no menu"));
            var viewModel = new Masthead(header);

            ViewBag.IsRTL = currentItem.Language.CultureInfo.TextInfo.IsRightToLeft;
            ViewBag.IsLoggedIn = this.IsLoggedIn;
            return PartialView("~/Views/Feature/CommonComponents/Renderings/PODEvent/PODSmallHeader.cshtml", viewModel);
        }

        public ActionResult SmartCommunicationsHeader()
        {
            if (!RenderingRepository.HasDataSource)
            {
                return new EmptyResult();
            }
            Header header = RenderingRepository.GetDataSourceItem<Header>();
            return View("~/Views/Feature/CommonComponents/Renderings/SmartCommunications/Header.cshtml", header);
        }

        /// <summary>
        /// The LanguageSwitcher.
        /// </summary>
        /// <param name="tabindex">The tabindex<see cref="string"/>.</param>
        /// <param name="type">The type<see cref="int"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult SmartCommunicationsLanguageSwitcher()
        {
            LanguageSwitcher.LanguageSwitcher langViewModel = LanguageHelper.languageIdentification(string.Empty, 0, Request);
            return View("~/Views/Feature/CommonComponents/Renderings/SmartCommunications/LanguageSwitcher.cshtml", langViewModel);
        }
        public ActionResult SmartCommunicationsFooterTeaser()
        {
            if (RenderingRepository.HasDataSource)
            {
                QuickImageLinksSection data = RenderingRepository.GetDataSourceItem<QuickImageLinksSection>();
                return PartialView("~/Views/Feature/CommonComponents/Renderings/SmartCommunications/FooterTeaser.cshtml", data);
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult IdealHomePillarSet()
        {
            if (RenderingRepository.HasDataSource)
            {
                M6TeaserSet m6TeaserSet = RenderingRepository.GetDataSourceItem<M6TeaserSet>();
                return PartialView("~/Views/Feature/CommonComponents/Renderings/IdealHome/IdealHomePillarSet.cshtml", m6TeaserSet);
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult IdealHomeAccordion()
        {
            if (RenderingRepository.HasDataSource)
            {
                AccordionSet accordionSet = RenderingRepository.GetDataSourceItem<AccordionSet>();
                return PartialView("~/Views/Feature/CommonComponents/Renderings/IdealHome/IdealHomeAccordion.cshtml", accordionSet);
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult IdealHomeDocumentaryVideo()
        {
            if (RenderingRepository.HasDataSource)
            {
                VideoModelSet videomodelSet = RenderingRepository.GetDataSourceItem<VideoModelSet>();
                return PartialView("~/Views/Feature/CommonComponents/Renderings/IdealHome/IdealHomeDocumentaryVideo.cshtml", videomodelSet);
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult IdealHomeAwarenessVideo()
        {
            if (RenderingRepository.HasDataSource)
            {
                VideoModelSet videomodelSet = RenderingRepository.GetDataSourceItem<VideoModelSet>();
                return PartialView("~/Views/Feature/CommonComponents/Renderings/IdealHome/IdealHomeAwarenessVideo.cshtml", videomodelSet);
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult IdealHomeUsefulLinks()
        {
            if (RenderingRepository.HasDataSource)
            {
                M6TeaserSet m6TeaserSet = RenderingRepository.GetDataSourceItem<M6TeaserSet>();
                return PartialView("~/Views/Feature/CommonComponents/Renderings/IdealHome/IdealHomeUsefulLinks.cshtml", m6TeaserSet);
            }
            return new EmptyResult();
        }
    }
}
