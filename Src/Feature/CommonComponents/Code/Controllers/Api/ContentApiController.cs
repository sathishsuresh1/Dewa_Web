using DEWAXP.Feature.CommonComponents.Helpers;
using DEWAXP.Feature.CommonComponents.Models.Renderings;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models.SmartResponseModel;
using DEWAXP.Foundation.Content.Utils;
using DEWAXP.Foundation.Helpers.Common;
using Glass.Mapper.Sc;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Layouts;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecorex = Sitecore.Context;

namespace DEWAXP.Feature.CommonComponents.Controllers.Api
{
    [AuthorizedCustomRoleFilter("ContentAPI")]
    public class ContentApiController : BaseApiController
    {
        public int defcount
        {
            get
            {
                var accountlimit = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.Api_Count_Config)));
                return accountlimit != null ? System.Convert.ToInt32(accountlimit.Fields["Clearance Certificate Cost"].Value) : 10;
                //var accountlimit = SCService.GetItem<DataSourceItems>(Guid.Parse(SitecoreItemIdentifiers.Api_Count_Config), false);
                //return accountlimit.Value != null ? System.Convert.ToInt32(accountlimit.Value) : 10;
            }
        }

        //
        // GET /api/News/1
        [System.Web.Mvc.HttpGet]
        [System.Web.Http.ActionName("GetNews")]
        public Newsfeed GetNews(string lang, int month = int.MinValue, int year = int.MinValue, int count = 20, int skip = 0)
        {
            SetContextLanguage(lang);
            int childrencount = 0;
            IEnumerable<Article> filteredSet = null;

            //filteredSet = NewsHelper.GetNewsListing(sitecoreService,
            //              Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE), out childrencount, month, year);
            if (count > defcount)
            {
                count = defcount;
            }
            filteredSet = NewsHelper.GetNewsListingIndex(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE), month, year, skip, count, out childrencount);

            var subset = filteredSet.Take(count);

            var newsfeed = new Newsfeed { Newsitems = new List<Newsfeed.item>(), nxtskip = childrencount };

            foreach (var newsitem in subset)
            {
                string _publishDate = newsitem.PublishDate.ToString("ddd, dd MMM yyyy", Context.Language.CultureInfo).Replace("يوليه", "يوليو");
                if (lang?.ToLower() == "ar")
                {
                    _publishDate = newsitem.PublishDate.ToString("ddd، dd MMM yyyy", Context.Language.CultureInfo).Replace("يوليه", "يوليو");
                }
                var item = new Newsfeed.item
                {
                    id = newsitem.Id.ToString(),
                    pubdate = _publishDate,
                    title = newsitem.Header,
                    brief = newsitem.Summary,
                    details = RemoveExtraTags(newsitem),
                    thumbnail = newsitem.TeaserImage != null ? GetMediaAbsoluteUrl(newsitem.TeaserImage.MediaId, "?h=289&w=386&la=en") : string.Empty,
                    iPadthumbnail = newsitem.TeaserImage != null ? GetMediaAbsoluteUrl(newsitem.TeaserImage.MediaId, "?h=105&w=210&la=en") : string.Empty,
                    largeimage = newsitem.TeaserImage != null ? GetMediaAbsoluteUrl(newsitem.TeaserImage.MediaId, string.Empty) : string.Empty,
                    link = BaseSiteHelper.GetSecureLocalURLWithoutPort(newsitem.FullUrl)
                };
                newsfeed.Newsitems.Add(item);
            }
            return newsfeed;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.ActionName("GetSmartTranslation")]
        public SmartTranslation GetSmartTranslation(string lang, string pDate = null)
        {
            SetContextLanguage("en");
            var constips = new SmartTranslation { Translations = new List<SmartTranslation.item>(), datetime = DateTime.Now.ToString(CommonUtility.DF_yyyyMMddHHmmss) };
            var item = Sitecorex.Database.GetItem(new ID("{C13C2AE8-6422-4EE6-9CEA-A7711495BF56}"));
            if (item == null) return constips;
            int counter = 0;

            foreach (var child in item.Children.ToList<Item>())
            {
                //string _date = System.Convert.ToString(child.Fields["__Updated"].Value);
                var c = ContentRepository.GetItem<SmartResposeDictionary>(new GetItemByItemOptions(child));

                bool isAdd = string.IsNullOrWhiteSpace(pDate);

                if (!isAdd)
                {
                    DateTime rqstDate = CommonUtility.DateTimeFormatParse(pDate, CommonUtility.DF_yyyyMMddHHmmss);
                    isAdd = rqstDate != DateTime.MinValue && CommonUtility.DateDiffrence(c.Item.Statistics.Updated, rqstDate) >= 0;
                }

                if (isAdd)
                {
                    constips.Translations.Add(new SmartTranslation.item { id = counter, KeyEnglish = c.KeyEnglish, Chinese = c.Chinese, Arabic = c.Arabic, Philippines = c.Philippines, Urdu = c.Urdu });
                    counter++;
                }
            }
            return constips;
        }

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetTips")]
        //public ConservationTipsFeed GetTips(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var constips = new ConservationTipsFeed { ConservationTips = new List<ConservationTipsFeed.item>() };
        //    var item = Sitecorex.Database.GetItem(new ID(SitecoreItemIdentifiers.CONSERVATIONTIPS));
        //    if (item == null) return constips;
        //    var datasource = GetDatasource(item, "{E3B2496E-D074-46B1-9363-C065A7AE16D0}", "content");
        //    var tipsContentItem = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(datasource)));
        //    var tipsTabs = ContentRepository.GetItem<Tabs>(new GetItemByItemOptions(tipsContentItem));
        //    if (tipsTabs == null) return constips;
        //    int counter = 0;
        //    foreach (var child in tipsTabs.Children)
        //    {
        //        constips.ConservationTips.Add(new ConservationTipsFeed.item { id = counter, title = child.Title, details = child.Body });
        //        counter++;
        //    }
        //    return constips;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetVideoTips")]
        //public ConservationTipVideos GetVideoTips(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var constips = new ConservationTipVideos { Videos = new List<ConservationTipVideos.item>() };
        //    var item = Sitecorex.Database.GetItem(new ID(SitecoreItemIdentifiers.CONSERVATIONTIPVIDEOS));
        //    if (item == null) return constips;
        //    var videolistitems = GetDatasourceList(item, "{F3163199-BFD3-4115-9A98-63471863B658}", "content");
        //    int counter = 0;
        //    foreach (var child in videolistitems)
        //    {
        //        var videoitem = ContentRepository.GetItem<Video>(new GetItemByItemOptions(child));
        //        constips.Videos.Add(new ConservationTipVideos.item { id = counter, title = videoitem.DisplayName, link = videoitem.Source });
        //        counter++;
        //    }
        //    return constips;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetAboutUs")]
        //public AboutUs GetAboutUs(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var aboutus = new AboutUs { aboutus = new List<AboutUs.item>() };
        //    var item = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.ABOUTUSFOLDER)));
        //    if (item == null) return aboutus;
        //    int counter = 0;
        //    foreach (var child in item.Children.ToList())
        //    {
        //        var aboutusitem = ContentRepository.GetItem<M9Teaser>(new GetItemByItemOptions(child));
        //        //var formattedtexts = this.GetFormattedContentList(child.ID.Guid);
        //        var aboutuscontentitems = GetFormattedContentListWithTitle(child.ID.Guid);
        //        foreach (var formattext in aboutuscontentitems)
        //        {
        //            aboutus.aboutus.Add(new AboutUs.item
        //            {
        //                id = counter,
        //                title = string.IsNullOrEmpty(formattext.Title) ? aboutusitem.Header : formattext.Title,
        //                brief = aboutusitem.Summary,
        //                thumbnail = aboutusitem.Image != null ? GetMediaAbsoluteUrl(aboutusitem.Image.MediaId, string.Empty) : string.Empty,
        //                details = formattext.Content
        //            });
        //            counter++;
        //        }

        //        counter++;
        //    }
        //    return aboutus;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetVideos")]
        //public Videos GetVideos(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var videos = new Videos { videos = new List<Videos.item>() };
        //    var item = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.VIDEOGALLERYPAGE)));
        //    if (item == null) return videos;
        //    int counter = 0;
        //    foreach (var child in item.Children.ToList())
        //    {
        //        var videolistitems = GetDatasourceList(child, "{F3163199-BFD3-4115-9A98-63471863B658}", "content");
        //        foreach (var videoplayeritem in videolistitems)
        //        {
        //            var videoitem = ContentRepository.GetItem<Video>(new GetItemByItemOptions(videoplayeritem));
        //            videos.videos.Add(new Videos.item { id = counter, title = videoitem.DisplayName, link = videoitem.Source });
        //            counter++;
        //        }
        //    }
        //    return videos;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetNationalIdentity")]
        //public NationalIdentity GetNationalIdentity(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var nationalid = new NationalIdentity { nationalid = new List<NationalIdentity.item>() };
        //    var item = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.NATIONALIDENTITYPAGE)));
        //    if (item == null) return nationalid;
        //    int counter = 0;
        //    var contentlist = GetFormattedContentList(item.ID.Guid);
        //    foreach (var child in contentlist)
        //    {
        //        if (string.IsNullOrEmpty(child)) continue;
        //        nationalid.nationalid.Add(new NationalIdentity.item
        //        {
        //            id = counter,
        //            details = child,
        //            largeimage = string.Empty,
        //            thumbnail = string.Empty,
        //            title = string.Empty
        //        });
        //        counter++;
        //    }
        //    return nationalid;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetPublications")]
        //public Publications GetPublications(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var publications = new Publications { publications = new List<Publications.item>() };
        //    var contentpagelist = new List<string> { SitecoreItemIdentifiers.DEWAPUBLICATIONS, SitecoreItemIdentifiers.CIRCULARSFORMS, SitecoreItemIdentifiers.SUSTAINABLEENERGY };
        //    int counter = 0;
        //    foreach (var contentitemstr in contentpagelist)
        //    {
        //        var item = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(contentitemstr)));
        //        if (item == null) continue;
        //        foreach (var contentitemchild in item.Children.ToList())
        //        {
        //            var casteditem = ContentRepository.GetItem<PageBase>(new GetItemByItemOptions(contentitemchild));

        //            var publication = new Publications.item { id = counter, title = casteditem.MenuLabel, magazine = new List<Publications.Magazine>() };
        //            var magazine = new Publications.Magazine { magazine = new List<Publications.Magazine.item>() };

        //            var downloadlist = GetDownloadList(contentitemchild.ID.Guid);
        //            int downloadcounter = 0;
        //            foreach (var downloader in downloadlist)
        //            {
        //                magazine.magazine.Add(new Publications.Magazine.item { id = downloadcounter, title = downloader.title, pdate = downloader.pdate, pdf = downloader.pdf });
        //                downloadcounter++;
        //            }
        //            publication.magazine.Add(magazine);
        //            publications.publications.Add(publication);
        //            counter++;
        //        }
        //    }

        //    return publications;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetHhquotes")]
        //public HHQuotes GetHhquotes(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var hhquotes = new HHQuotes { hhquotes = new List<HHQuotes.item>() };
        //    var item = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.HISHIGHNESSQUOTES)));
        //    if (item == null) return hhquotes;
        //    int counter = 0;
        //    var contentlist = GetQuoteswithHeaderList(item.ID.Guid);
        //    foreach (var child in contentlist)
        //    {
        //        hhquotes.hhquotes.Add(new HHQuotes.item
        //        {
        //            id = counter,
        //            title = child.header,
        //            details = child.quote
        //        });
        //        counter++;
        //    }
        //    return hhquotes;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetPhotos")]
        //public photos GetPhotos(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var photos = new photos { photo = new List<photos.item>() };
        //    var item = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.PHOTOGALLERYPAGE)));
        //    if (item == null) return photos;
        //    int counter = 0;
        //    foreach (var child in item.Children.ToList())
        //    {
        //        var galleryitem = ContentRepository.GetItem<PageBase>(new GetItemByItemOptions(child));
        //        var photoitems = GetImageList(child.ID.Guid, "?w=300&h=200");
        //        foreach (var photoitem in photoitems)
        //        {
        //            photos.photo.Add(new photos.item
        //            {
        //                id = counter,
        //                category = galleryitem.Header,
        //                image = photoitem.image,
        //                thumbnail = photoitem.thumbnail
        //            });
        //        }

        //        counter++;
        //    }
        //    return photos;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetFeaturedLinks")]
        //public FeaturedLinks GetFeaturedLinks(string lang)
        //{
        //    var sitecoreService = new SitecoreContext();
        //    SetContextLanguage(lang);
        //    var featuredlinks = new FeaturedLinks { flinks = new List<FeaturedLinks.item>() };
        //    var item = sitecoreService.GetItem<Item>(SitecoreItemIdentifiers.GOVERNMENTENTITIES);
        //    if (item == null) return featuredlinks;
        //    int counter = 0;
        //    var entitiesdatasource = GetDatasourceRenderingTemplate(item, "{D5761696-FD12-4FFE-97DE-D0352FE19E72}",
        //        "grey-box-content");
        //    if (string.IsNullOrEmpty(entitiesdatasource)) return featuredlinks;
        //    var entityitem = sitecoreService.GetItem<Item>(entitiesdatasource);
        //    if (entityitem == null) return featuredlinks;
        //    var featuredentityteaserset = sitecoreService.Cast<M6TeaserSet>(entityitem);
        //    foreach (var featuredlinkitem in featuredentityteaserset.Children)
        //    {
        //        var thumbnail = featuredlinkitem.Image != null
        //            ? GetMediaAbsoluteUrl(featuredlinkitem.Image.MediaId, string.Empty)
        //            : string.Empty;
        //        featuredlinks.flinks.Add(new FeaturedLinks.item
        //            {
        //                id = counter,
        //                thumbnail = thumbnail,
        //                title = featuredlinkitem.Header,
        //                websiteurl = featuredlinkitem.Link != null ? featuredlinkitem.Link.Url : string.Empty,
        //                details = featuredlinkitem.Subheader
        //            });

        //        counter++;
        //    }
        //    return featuredlinks;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetGreenBill")]
        //public Greenbill GetGreenBill(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var greenbill = new Greenbill { greenbill = new List<Greenbill.item>() };
        //    var item = ContentRepository.GetItem<Tab>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.GREENBILLTAB)));
        //    if (item == null) return greenbill;
        //    greenbill.greenbill.Add(new Greenbill.item
        //    {
        //        id = 0,
        //        description = item.Body,
        //        largeimage = string.Empty,
        //        thumbnail = string.Empty,
        //        title = item.Title
        //    });
        //    return greenbill;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetPrivacyPolicy")]
        //public PrivacyPolicy GetPrivacyPolicy(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var privacypolicy = new PrivacyPolicy { privacypolicy = new List<PrivacyPolicy.item>() };
        //    var item = ContentRepository.GetItem<PageBase>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.SECURITYANDPOLICYPAGE)));
        //    if (item == null) return privacypolicy;
        //    var privacypolicycontent = this.GetFormattedContent(item.Id);
        //    privacypolicy.privacypolicy.Add(new PrivacyPolicy.item
        //    {
        //        id = 0,
        //        description = privacypolicycontent,
        //        largeimage = string.Empty,
        //        thumbnail = string.Empty,
        //        title = item.Header
        //    });
        //    return privacypolicy;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetCareforEarth")]
        //public CareForEarth GetCareforEarth(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var careforearth = new CareForEarth { careforearth = new List<CareForEarth.item>() };
        //    var item = ContentRepository.GetItem<PageBase>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.CAREFOREARTHPAGE)));
        //    if (item == null) return careforearth;
        //    var content = this.GetFormattedContent(item.Id);
        //    careforearth.careforearth.Add(new CareForEarth.item
        //    {
        //        id = 0,
        //        description = content,
        //        largeimage = string.Empty,
        //        thumbnail = string.Empty,
        //        title = item.Header
        //    });
        //    return careforearth;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetSlabTariff")]
        //public SlabTariff GetSlabTariff(string lang)
        //{
        //    SetContextLanguage(lang);

        //    var item = ContentRepository.GetItem<PageBase>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.TARIFFPAGE)));
        //    var slabtariff = new SlabTariff
        //    {
        //        slabtariff = new SlabTariff.item { id = 0, largeimage = string.Empty, thumbnail = string.Empty, details = new List<SlabTariff.details>() }
        //    };
        //    if (item == null) return slabtariff;
        //    var contentlist = this.GetFormattedContentList(item.Id);
        //    slabtariff.slabtariff.title = item.Header;
        //    foreach (var child in contentlist)
        //    {
        //        if (string.IsNullOrEmpty(child)) continue;
        //        slabtariff.slabtariff.details.Add(new SlabTariff.details
        //        {
        //            description = child
        //        });
        //    }

        //    return slabtariff;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetSlabTariffCalc")]
        //public SlabTariffCalc GetSlabTariffCalc(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var slabtariff = new SlabTariffCalc
        //    {
        //        slabtariff = new SlabTariffCalc.mySlabTariff()
        //    };
        //    var calcpage = ContentRepository.GetItem<PageBase>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.TARIFFCALCULATORPAGE)));
        //    if (calcpage == null) return slabtariff;
        //    var content = this.GetFormattedContent(calcpage.Id);
        //    slabtariff.slabtariff.item = new SlabTariffCalc.myitem
        //    {
        //        id = 0,
        //        largeimage = string.Empty,
        //        thumbnail = string.Empty,
        //        title = calcpage.Header,
        //        details = new SlabTariffCalc.mydetails { description = content }
        //    };
        //    var tariffcalsource =
        //        this.GetDatasource(Sitecorex.Database.GetItem(new ID(SitecoreItemIdentifiers.TARIFFCALCULATORPAGE)),
        //            "{6E3D3A51-AE20-480A-AEA2-F3D9A9364D92}", "content");
        //    if (string.IsNullOrEmpty(tariffcalsource)) return slabtariff;
        //    var source = ContentRepository.GetItem<Calculator>(new GetItemByIdOptions(Guid.Parse(tariffcalsource)));

        //    if (source == null) return slabtariff;

        //    slabtariff.slabtariff.tariffitem = new SlabTariffCalc.tariffcalc
        //    {
        //        Electricity = new SlabTariffCalc.tarifftype
        //        {
        //            title = Translate.Text(DictionaryKeys.TariffCalculator.Labels.Electricity) + " " + Translate.Text(DictionaryKeys.TariffCalculator.Labels.Tariff),
        //            Commercial = new SlabTariffCalc.tariffitemtype { item = new List<SlabTariffCalc.tariffitem>() },
        //            Residential = new SlabTariffCalc.tariffitemtype { item = new List<SlabTariffCalc.tariffitem>() },
        //            Industrial = new SlabTariffCalc.tariffitemtype { item = new List<SlabTariffCalc.tariffitem>() }
        //        },
        //        Fuel = new SlabTariffCalc.tarifftype
        //        {
        //            title = Translate.Text(DictionaryKeys.TariffCalculator.Labels.FuelSurcharge),
        //            Electricity = new SlabTariffCalc.tariffitem
        //            {
        //                Unit = Translate.Text(DictionaryKeys.TariffCalculator.Labels.KiloWattsPerHour),
        //                Currency = Translate.Text(DictionaryKeys.TariffCalculator.Labels.Currency)
        //            },
        //            Water = new SlabTariffCalc.tariffitem
        //            {
        //                Unit = Translate.Text(DictionaryKeys.TariffCalculator.Labels.ImperialGallon),
        //                Currency = Translate.Text(DictionaryKeys.TariffCalculator.Labels.Currency)
        //            }
        //        },
        //        Water = new SlabTariffCalc.tarifftype
        //        {
        //            title = Translate.Text(DictionaryKeys.TariffCalculator.Labels.Water) + " " + Translate.Text(DictionaryKeys.TariffCalculator.Labels.Tariff),
        //            Commercial = new SlabTariffCalc.tariffitemtype { item = new List<SlabTariffCalc.tariffitem>() },
        //            Residential = new SlabTariffCalc.tariffitemtype { item = new List<SlabTariffCalc.tariffitem>() },
        //            Industrial = new SlabTariffCalc.tariffitemtype { item = new List<SlabTariffCalc.tariffitem>() }
        //        }
        //    };
        //    this.AddCustomerTypeValues(source, "electricity", "residential", slabtariff.slabtariff.tariffitem.Electricity.Residential,
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.KiloWattsPerHour),
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.Currency), slabtariff.slabtariff.tariffitem.Fuel.Electricity, true);
        //    this.AddCustomerTypeValues(source, "electricity", "commercial", slabtariff.slabtariff.tariffitem.Electricity.Commercial,
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.KiloWattsPerHour),
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.Currency), slabtariff.slabtariff.tariffitem.Fuel.Electricity, false);
        //    this.AddCustomerTypeValues(source, "electricity", "industrial", slabtariff.slabtariff.tariffitem.Electricity.Industrial,
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.KiloWattsPerHour),
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.Currency), slabtariff.slabtariff.tariffitem.Fuel.Electricity, false);
        //    this.AddCustomerTypeValues(source, "water", "residential", slabtariff.slabtariff.tariffitem.Water.Residential,
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.ImperialGallon),
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.Currency), slabtariff.slabtariff.tariffitem.Fuel.Water, true);
        //    this.AddCustomerTypeValues(source, "water", "commercial", slabtariff.slabtariff.tariffitem.Water.Commercial,
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.ImperialGallon),
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.Currency), slabtariff.slabtariff.tariffitem.Fuel.Water, false);
        //    this.AddCustomerTypeValues(source, "water", "industrial", slabtariff.slabtariff.tariffitem.Water.Industrial,
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.ImperialGallon),
        //        Translate.Text(DictionaryKeys.TariffCalculator.Labels.Currency), slabtariff.slabtariff.tariffitem.Fuel.Water, false);
        //    return slabtariff;
        //}

        //obsolete
        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetPartners")]
        //public PartnerList.Partner GetPartners(string lang)
        //{
        //    SetContextLanguage(lang);
        //    var DewaPartners = sitecoreService.GetItem<DewaPartners>(SitecoreItemPaths.DEWAPARTNERLIST);
        //    var partners = new PartnerList.Partner() { partners = new List<PartnerList.item>() };
        //    int counter = 1;
        //    if (DewaPartners != null && DewaPartners.Children != null)
        //    {
        //        foreach (var partner in DewaPartners.Children)
        //        {
        //            partners.partners.Add(new PartnerList.item
        //            {
        //                id = counter.ToString(),
        //                title = partner.PartnerName,
        //                websiteurl = partner.WebsiteUrl
        //            });
        //            counter++;
        //        }
        //    }
        //    return partners;

        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetAccountSelectorItem")]
        //public AccountSelectorApiModel GetAccountSelectorItem(string lang, string itemId)
        //{
        //    var sitecoreService = new SitecoreContext();
        //    SetContextLanguage(lang);
        //    var accountSelector = sitecoreService.GetItem<AccountSelector>(new Guid(itemId));

        //    var accountSel = new AccountSelectorApiModel()
        //    {
        //        MaxSelection = accountSelector.MaxSelection,
        //        MinSelection = accountSelector.MinSelection,
        //        ExcludeInactiveAccounts = accountSelector.ExcludeInactiveAccounts,
        //        SelectedAccountNumber = accountSelector.SelectedAccountNumber,
        //        PageSize = accountSelector.PageSize,
        //        MultiSelect = accountSelector.MultiSelect,
        //        SecondaryDatasource = accountSelector.SecondaryDatasource
        //    };

        //    return accountSel;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetMoveOutPurposes")]
        //public MoveOutPurposesApiModel GetMoveOutPurposes(string lang, string itemId)
        //{
        //    var sitecoreService = new SitecoreContext();
        //    SetContextLanguage(lang);
        //    var purposesRoot = sitecoreService.GetItem<ListDataSources>(new Guid(itemId));

        //    var lstPurposes = purposesRoot.Items;

        //    var purposes = new MoveOutPurposesApiModel()
        //    {
        //        Purposes = lstPurposes.ToDictionary(x => x.Value, x => x.Text)
        //    };

        //    return purposes;
        //}

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Http.ActionName("GetMoveOutLabels")]
        //public MoveOutContentApiModel GetMoveOutLabels(string lang)
        //{
        //    var lstLabels = new Dictionary<string, string>();
        //    SetContextLanguage(lang);

        //    lstLabels.Add("AccountDetails", Translate.Text("Account Details"));
        //    lstLabels.Add("PremiseType", Translate.Text(DictionaryKeys.ChangePremiseType.PremiseType));
        //    lstLabels.Add("PremiseNumber", Translate.Text("Premise"));
        //    lstLabels.Add("BusinessPartner", Translate.Text("Business Partner"));
        //    lstLabels.Add("CertificateRequest", Translate.Text("moveout.clearancerequest"));
        //    lstLabels.Add("CertificateRequestOptional", "(" + Translate.Text("moveout.optional") + ")");
        //    lstLabels.Add("ClearanceDisclaimer", Translate.Text("moveout.clearancedisclaimer"));
        //    lstLabels.Add("ClearanceDisclaimerVAT", Translate.Text("moveout.clearancedisclaimerVAT"));

        //    var model = new MoveOutContentApiModel()
        //    {
        //        ContentLabels = lstLabels
        //    };

        //    return model;
        //}

        private void SetContextLanguage(string lang)
        {
            Context.Language = Language.Parse("en");
            if (!string.IsNullOrEmpty(lang))
            {
                if (lang.ToLower() == "ar")
                {
                    Context.Language = Language.Parse("ar-AE");
                }
            }
        }

        private string GetFormattedContentFromRenderingReference(RenderingReference renderingReference,
            string templateid, string fieldname)
        {
            if (renderingReference != null)
            {
                var datasource = renderingReference.Settings.DataSource;
                if (!string.IsNullOrEmpty(datasource))
                {
                    var datasourceItem = Sitecorex.Database.GetItem(datasource);
                    if (datasourceItem != null && datasourceItem.TemplateID.ToString() == templateid)
                    {
                        return datasourceItem[fieldname];
                    }
                }
            }

            return string.Empty;
        }

        //private string GetFormattedContentTitleFromRenderingReference(RenderingReference renderingReference,
        //    string templateid)
        //{
        //    if (renderingReference != null)
        //    {
        //        var datasource = renderingReference.Settings.DataSource;
        //        if (!string.IsNullOrEmpty(datasource))
        //        {
        //            var datasourceItem = Sitecorex.Database.GetItem(datasource);
        //            if (datasourceItem != null && datasourceItem.TemplateID.ToString() == templateid)
        //            {
        //                return datasourceItem["Header"];
        //            }
        //        }
        //    }

        //    return string.Empty;
        //}

        private string GetFormattedContent(Guid itemid)
        {
            const string placeholder = "content";
            var item = Sitecorex.Database.GetItem(new ID(itemid));
            if (item == null) return string.Empty;
            var renderingReferences = item.Visualization.GetRenderings(Sitecorex.Device, true);
            if (renderingReferences == null || !renderingReferences.Any()) return string.Empty;
            var renderingsInPlaceholder = renderingReferences.Where(r => r.Placeholder.EndsWith('/' + placeholder, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (RenderingReference renderingReference in renderingsInPlaceholder)
            {
                if (renderingReference.RenderingID.Guid == Guid.Parse("{C3EC4780-2E5B-42F2-8C94-C9F7BF830D6D}"))
                {
                    return GetFormattedContentFromRenderingReference(renderingReference,
                    "{BBDC399C-1225-4667-BD85-5818A4F966E7}", "Rich text");
                }
            }
            return string.Empty;
        }

        //private IEnumerable<string> GetFormattedContentList(Guid itemid)
        //{
        //    var returnlist = new List<string>();
        //    const string placeholder = "content";
        //    var item = Sitecorex.Database.GetItem(new ID(itemid));
        //    if (item == null) return returnlist;
        //    var renderingReferences = item.Visualization.GetRenderings(Sitecorex.Device, true);
        //    if (renderingReferences == null || !renderingReferences.Any()) return returnlist;
        //    var renderingsInPlaceholder = renderingReferences.Where(r => r.Placeholder.EndsWith('/' + placeholder, StringComparison.OrdinalIgnoreCase)).ToList();

        //    foreach (RenderingReference renderingReference in renderingsInPlaceholder)
        //    {
        //        returnlist.Add(GetFormattedContentFromRenderingReference(renderingReference,
        //            "{BBDC399C-1225-4667-BD85-5818A4F966E7}", "Rich text"));
        //    }
        //    return returnlist;
        //}

        //private IEnumerable<AboutUsContentItem> GetFormattedContentListWithTitle(Guid itemid)
        //{
        //    var returnlist = new List<AboutUsContentItem>();
        //    const string placeholder = "content";
        //    var item = Sitecorex.Database.GetItem(new ID(itemid));
        //    if (item == null) return returnlist;
        //    var renderingReferences = item.Visualization.GetRenderings(Sitecorex.Device, true);
        //    if (renderingReferences == null || !renderingReferences.Any()) return returnlist;
        //    var renderingsInPlaceholder = renderingReferences.Where(r => r.Placeholder.EndsWith('/' + placeholder, StringComparison.OrdinalIgnoreCase)).ToList();
        //    int counter = 0;
        //    foreach (RenderingReference renderingReference in renderingsInPlaceholder)
        //    {
        //        if (renderingReference.RenderingID.Guid == Guid.Parse("{C3EC4780-2E5B-42F2-8C94-C9F7BF830D6D}"))
        //        {
        //            var AboutUsContentItem = new AboutUsContentItem();
        //            AboutUsContentItem.Content = GetFormattedContentFromRenderingReference(renderingReference,
        //                "{BBDC399C-1225-4667-BD85-5818A4F966E7}", "Rich text");

        //            AboutUsContentItem.Title = this.GetFormattedContentTitleFromRenderingReference(
        //                renderingsInPlaceholder[counter - 1],
        //                "{9D7313FA-D5AE-45D8-A959-2F5CB59BB4F8}");
        //            returnlist.Add(AboutUsContentItem);
        //        }
        //        counter++;
        //    }
        //    return returnlist;
        //}

        //public class AboutUsContentItem
        //{
        //    public string Content { get; set; }
        //    public string Title { get; set; }
        //}

        //private IEnumerable<HHQuotes.quoteswithheader> GetQuoteswithHeaderList(Guid itemid)
        //{
        //    var returnlist = new List<HHQuotes.quoteswithheader>();
        //    const string placeholder = "content";
        //    var item = Sitecorex.Database.GetItem(new ID(itemid));
        //    if (item == null) return returnlist;
        //    var renderingReferences = item.Visualization.GetRenderings(Sitecorex.Device, true);
        //    if (renderingReferences == null || !renderingReferences.Any()) return returnlist;
        //    var renderingsInPlaceholder = renderingReferences.Where(r => r.Placeholder.EndsWith('/' + placeholder, StringComparison.OrdinalIgnoreCase)).ToList();
        //    int counter = 0;
        //    foreach (RenderingReference renderingReference in renderingsInPlaceholder)
        //    {
        //        if (renderingReference.RenderingID.Guid == Guid.Parse("{F8D000F6-B811-4DE4-9871-988BB64D76A4}"))
        //        {
        //            var header = GetFormattedContentFromRenderingReference(renderingReference,
        //            "{9D7313FA-D5AE-45D8-A959-2F5CB59BB4F8}", "Header");
        //            var quoteswithheader = new HHQuotes.quoteswithheader { header = header };
        //            var descrip = GetFormattedContentFromRenderingReference(renderingsInPlaceholder[counter + 1],
        //                "{BBDC399C-1225-4667-BD85-5818A4F966E7}", "Rich text");
        //            returnlist.Add(quoteswithheader);
        //            quoteswithheader.quote = descrip;
        //        }

        //        counter++;
        //    }
        //    return returnlist;
        //}

        //private string GetDatasource(Item item, string datasourceTemplateId, string placeholder)
        //{
        //    if (item == null) return string.Empty;
        //    var renderingReferences = item.Visualization.GetRenderings(Sitecorex.Device, true);
        //    if (renderingReferences == null || !renderingReferences.Any()) return string.Empty;
        //    var renderingsInPlaceholder = renderingReferences.Where(r => r.Placeholder.EndsWith('/' + placeholder, StringComparison.OrdinalIgnoreCase)).ToList();

        //    foreach (RenderingReference renderingReference in renderingsInPlaceholder)
        //    {
        //        if (renderingReference != null)
        //        {
        //            var datasource = renderingReference.Settings.DataSource;
        //            if (!string.IsNullOrEmpty(datasource))
        //            {
        //                var datasourceItem = Sitecorex.Database.GetItem(datasource);
        //                if (datasourceItem != null && datasourceItem.TemplateID.ToString() == datasourceTemplateId)
        //                {
        //                    return datasource;
        //                }
        //            }
        //        }
        //    }
        //    return string.Empty;
        //}

        //private string GetDatasourceRenderingTemplate(Item item, string renderingTemplateId, string placeholder)
        //{
        //    if (item == null) return string.Empty;
        //    var renderingReferences = item.Visualization.GetRenderings(Sitecorex.Device, true);
        //    if (renderingReferences == null || !renderingReferences.Any()) return string.Empty;
        //    var renderingsInPlaceholder = renderingReferences.Where(r => r.Placeholder.EndsWith('/' + placeholder, StringComparison.OrdinalIgnoreCase)).ToList();

        //    foreach (RenderingReference renderingReference in renderingsInPlaceholder)
        //    {
        //        if (renderingReference != null && renderingReference.RenderingID.Guid == Guid.Parse(renderingTemplateId))
        //        {
        //            return renderingReference.Settings.DataSource;
        //        }
        //    }
        //    return string.Empty;
        //}

        //private IEnumerable<Item> GetDatasourceList(Item item, string datasourceTemplateId, string placeholder)
        //{
        //    var returnlist = new List<Item>();
        //    if (item == null) return returnlist;
        //    var renderingReferences = item.Visualization.GetRenderings(Sitecorex.Device, true);
        //    if (renderingReferences == null || !renderingReferences.Any()) return returnlist;
        //    var renderingsInPlaceholder = renderingReferences.Where(r => r.Placeholder.EndsWith('/' + placeholder, StringComparison.OrdinalIgnoreCase)).ToList();

        //    foreach (RenderingReference renderingReference in renderingsInPlaceholder)
        //    {
        //        if (renderingReference != null)
        //        {
        //            var datasource = renderingReference.Settings.DataSource;
        //            if (!string.IsNullOrEmpty(datasource))
        //            {
        //                var datasourceItem = Sitecorex.Database.GetItem(datasource);
        //                if (datasourceItem != null && datasourceItem.TemplateID.ToString() == datasourceTemplateId)
        //                {
        //                    returnlist.Add(datasourceItem);
        //                }
        //            }
        //        }
        //    }
        //    return returnlist;
        //}

        private string GetMediaAbsoluteUrl(Guid mediaId, string additionalParams)
        {
            var item = ContentRepository.GetItem<Item>(new GetItemByIdOptions(mediaId));
            if (item == null) return string.Empty;
            var muo = new MediaUrlBuilderOptions { AlwaysIncludeServerUrl = true };
            string url = MediaManager.GetMediaUrl(item, muo);
            if (string.IsNullOrEmpty(url)) return string.Empty;
            url = BaseSiteHelper.GetSecureLocalURLWithoutPort(HashingUtils.ProtectAssetUrl(url + additionalParams));
            return url;
        }

        //private IEnumerable<Publications.downloadpdf> GetDownloadList(Guid itemid)
        //{
        //    var returnlist = new List<Publications.downloadpdf>();
        //    const string placeholder = "content";
        //    var item = Sitecorex.Database.GetItem(new ID(itemid));
        //    if (item == null) return returnlist;
        //    var renderingReferences = item.Visualization.GetRenderings(Sitecorex.Device, true);
        //    if (renderingReferences == null || !renderingReferences.Any()) return returnlist;
        //    var renderingsInPlaceholder = renderingReferences.Where(r => r.Placeholder.EndsWith('/' + placeholder, StringComparison.OrdinalIgnoreCase)).ToList();

        //    foreach (RenderingReference renderingReference in renderingsInPlaceholder)
        //    {
        //        if (renderingReference != null)
        //        {
        //            var datasource = renderingReference.Settings.DataSource;
        //            if (!string.IsNullOrEmpty(datasource))
        //            {
        //                var datasourceItem = Sitecorex.Database.GetItem(datasource);
        //                if (datasourceItem != null && datasourceItem.TemplateID.ToString() == "{BE114D2B-7917-4922-A26C-BBEC3AE8C310}")
        //                {
        //                    var downloadmodel = ContentRepository.GetItem<Download>(new GetItemByItemOptions(datasourceItem));
        //                    var downloadurl = downloadmodel.File != null ? GetMediaAbsoluteUrl(downloadmodel.File.Id, string.Empty) : string.Empty;
        //                    returnlist.Add(new Publications.downloadpdf { pdf = downloadurl, title = downloadmodel.Text, pdate = datasourceItem.Statistics.Updated.ToString("MM-yyyy", Context.Culture) });
        //                }
        //            }
        //        }
        //    }
        //    return returnlist;
        //}

        //private IEnumerable<photos.photoitem> GetImageList(Guid itemid, string thumbnailparams)
        //{
        //    var returnlist = new List<photos.photoitem>();
        //    const string placeholder = "content";
        //    var item = Sitecorex.Database.GetItem(new ID(itemid));
        //    if (item == null) return returnlist;
        //    var renderingReferences = item.Visualization.GetRenderings(Sitecorex.Device, true);
        //    if (renderingReferences == null || !renderingReferences.Any()) return returnlist;
        //    var renderingsInPlaceholder = renderingReferences.Where(r => r.Placeholder.EndsWith('/' + placeholder, StringComparison.OrdinalIgnoreCase)).ToList();

        //    foreach (RenderingReference renderingReference in renderingsInPlaceholder)
        //    {
        //        if (renderingReference.RenderingID.Guid == Guid.Parse("{C626EC10-33A9-4CE2-B3D0-BADA5075807A}"))
        //        {
        //            var datasource = renderingReference.Settings.DataSource;
        //            if (!string.IsNullOrEmpty(datasource))
        //            {
        //                var datasourceItem = Sitecorex.Database.GetItem(datasource);
        //                var gallery =
        //                    ContentRepository.GetItem<ThumbnailGallery>(new GetItemByItemOptions(datasourceItem));
        //                foreach (var photoitem in gallery.Images)
        //                {
        //                    if (photoitem != null)
        //                    {
        //                        returnlist.Add(new photos.photoitem
        //                        {
        //                            thumbnail = GetMediaAbsoluteUrl(photoitem.MediaId, thumbnailparams),
        //                            image = GetMediaAbsoluteUrl(photoitem.MediaId, string.Empty)
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return returnlist;
        //}

        //tarifftype can be Electricity, Water
        //premisetype can be Residential, Industrial
        //tariffunit can be IG or Kwh
        //tariffcurrency can be fils
        //private void AddCustomerTypeValues(Calculator calculator, string tarifftype, string premisetype,
        //    SlabTariffCalc.tariffitemtype tariffitemtype, string tariffunit, string tariffcurrency,
        //    SlabTariffCalc.tariffitem tariffitem, bool addFuelSurcharge)
        //{
        //    var custType = calculator.CustomerTypes.Where(x => x.Name.ToLowerInvariant().Equals(premisetype)).FirstOrDefault();
        //    if (custType != null)
        //    {
        //        tariffitemtype.title = custType.Title;
        //        var custTypeElect = custType.TariffTypes.Where(o => o.Name.ToLowerInvariant().Equals(tarifftype)).FirstOrDefault();
        //        if (custTypeElect != null)
        //        {
        //            if (addFuelSurcharge)
        //            {
        //                tariffitem.Amount = (Decimal.Parse(custTypeElect.FuelSurchargeTariff) * 100).ToString("G29");
        //            }
        //            if (custTypeElect.Consumption != null && custTypeElect.Consumption.Any())
        //            {
        //                int counter = 1;
        //                foreach (var consumption in custTypeElect.Consumption)
        //                {
        //                    tariffitemtype.item.Add(new SlabTariffCalc.tariffitem
        //                    {
        //                        category = counter,
        //                        Color = consumption?.Name?.Substring(0, 1) ?? "",
        //                        Unit = tariffunit,
        //                        Currency = tariffcurrency,
        //                        Amount = (decimal.Parse(consumption.Tariff) * 100).ToString("G29"),
        //                        ConsFrom = consumption.From,
        //                        ConsTo = consumption.To
        //                    });
        //                    counter++;
        //                }
        //            }
        //        }
        //    }
        //}

        private string RemoveExtraTags(Article newsitem)
        {
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();
            string removeImgTag = Regex.Replace(!string.IsNullOrEmpty(newsitem.NewsArticle) ? newsitem.NewsArticle : GetFormattedContent(newsitem.Id), @"(<img\/?[^>]+>)", @"", RegexOptions.IgnoreCase);
            removeImgTag = removeImgTag.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
            string replacebrTag = removeImgTag.Replace("<br><br>", "<br>");
            string removeSpace = replacebrTag.Contains("<p") ? replacebrTag.Substring(replacebrTag.IndexOf("<p")) : replacebrTag;
            return removeSpace;
        }

        //#region [smart repsonse]

        ///// <summary>
        ///// Get Smart reponse Translation from sitecore.
        ///// https://www.andiamo.co.uk/resources/iso-language-codes/
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="lang"></param>
        ///// <returns></returns>
        //internal string GetSmTranslation(string key, SmlangCode lang = SmlangCode.en)
        //{
        //    string traslationValue = key;

        //    if (string.IsNullOrWhiteSpace(key) || lang == SmlangCode.en)
        //    {
        //        return key ?? "";
        //    }

        //    #region [Fetch Translation Value]

        //    var item = GetSmSitecoreTranslation();
        //    if (item != null)
        //    {
        //        var scItem = item.FirstOrDefault(x => x != null &&
        //                                        x.KeyEnglish.Replace("\\n", "").Trim().ToLower() == key?.Replace("\n", "").Trim().ToLower());

        //        if (scItem != null)
        //        {
        //            switch (lang)
        //            {
        //                case SmlangCode.ar:
        //                    traslationValue = scItem.Arabic;
        //                    break;

        //                case SmlangCode.zh:
        //                    traslationValue = scItem.Chinese;
        //                    break;

        //                case SmlangCode.ur:
        //                    traslationValue = scItem.Urdu;
        //                    break;

        //                case SmlangCode.tl:
        //                    traslationValue = scItem.Philippines;
        //                    break;

        //                case SmlangCode.en:
        //                    traslationValue = scItem.KeyEnglish;
        //                    break;

        //                default:
        //                    traslationValue = key;
        //                    break;
        //            }
        //        }
        //    }

        //    #endregion [Fetch Translation Value]

        //    return !string.IsNullOrWhiteSpace(traslationValue) ? traslationValue : (key ?? "");
        //}

        //internal List<SmartResposeDictionary> GetSmSitecoreTranslation()
        //{
        //    List<SmartResposeDictionary> smTranslations = new List<SmartResposeDictionary>();
        //    Item smTranslationScItem = null;
        //    CacheProvider.TryGet(CacheKeys.SM_SC_TRANSLATION_ITEMS, out smTranslations);
        //    if (smTranslations == null)
        //    {
        //        smTranslations = new List<SmartResposeDictionary>();
        //        SetContextLanguage("en");
        //        smTranslationScItem = Sitecorex.Database.GetItem(new ID("{C13C2AE8-6422-4EE6-9CEA-A7711495BF56}"));
        //        foreach (Item scItem in smTranslationScItem.Children.ToList<Item>())
        //        {
        //            var t = ContentRepository.GetItem<SmartResposeDictionary>(new GetItemByItemOptions(scItem));
        //            smTranslations.Add(t);
        //        }
        //        CacheProvider.Store(CacheKeys.SM_SC_TRANSLATION_ITEMS, new CacheItem<List<SmartResposeDictionary>>(smTranslations, TimeSpan.FromHours(1)));
        //    }
        //    return smTranslations;
        //}

        //#endregion [smart repsonse]

        //#region [Consumption Complaint]

        ///// <summary>
        ///// Get Consumption Complaint Reponse Translation from sitecore.
        ///// https://www.andiamo.co.uk/resources/iso-language-codes/
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="lang"></param>
        ///// <returns></returns>
        //internal string GetCCTranslation(string key, _CC_models.SmlangCode lang = _CC_models.SmlangCode.en)
        //{
        //    string traslationValue = key;

        //    if (string.IsNullOrWhiteSpace(key) || lang == _CC_models.SmlangCode.en)
        //    {
        //        return key ?? "";
        //    }

        //    #region [Fetch Translation Value]

        //    var item = GetCCSitecoreTranslation();
        //    if (item != null)
        //    {
        //        var scItem = item.FirstOrDefault(x => x != null &&
        //                                        x.KeyEnglish.Replace("\\n", "").Trim().ToLower() == key?.Replace("\n", "").Trim().ToLower());

        //        if (scItem != null)
        //        {
        //            switch (lang)
        //            {
        //                case _CC_models.SmlangCode.ar:
        //                    traslationValue = scItem.Arabic;
        //                    break;

        //                case _CC_models.SmlangCode.zh:
        //                    traslationValue = scItem.Chinese;
        //                    break;

        //                case _CC_models.SmlangCode.ur:
        //                    traslationValue = scItem.Urdu;
        //                    break;

        //                case _CC_models.SmlangCode.tl:
        //                    traslationValue = scItem.Philippines;
        //                    break;

        //                case _CC_models.SmlangCode.en:
        //                    traslationValue = scItem.KeyEnglish;
        //                    break;

        //                default:
        //                    traslationValue = key;
        //                    break;
        //            }
        //        }
        //    }

        //    #endregion [Fetch Translation Value]

        //    return !string.IsNullOrWhiteSpace(traslationValue) ? traslationValue : (key ?? "");
        //}

        //internal string GetUSCTranslation(string key, _CC_models.SmlangCode lang = _CC_models.SmlangCode.en)
        //{
        //    string traslationValue = key;

        //    if (string.IsNullOrWhiteSpace(key) || lang == _CC_models.SmlangCode.en)
        //    {
        //        return key ?? "";
        //    }

        //    #region [Fetch Translation Value]

        //    var item = GetUSCSitecoreTranslation();
        //    if (item != null)
        //    {
        //        var scItem = item.FirstOrDefault(x => x != null &&
        //                                        x.KeyEnglish.Replace("\\n", "").Trim().ToLower() == key?.Replace("\n", "").Trim().ToLower());

        //        if (scItem != null)
        //        {
        //            switch (lang)
        //            {
        //                case _CC_models.SmlangCode.ar:
        //                    traslationValue = scItem.Arabic;
        //                    break;

        //                case _CC_models.SmlangCode.zh:
        //                    traslationValue = scItem.Chinese;
        //                    break;

        //                case _CC_models.SmlangCode.ur:
        //                    traslationValue = scItem.Urdu;
        //                    break;

        //                case _CC_models.SmlangCode.tl:
        //                    traslationValue = scItem.Philippines;
        //                    break;

        //                case _CC_models.SmlangCode.en:
        //                    traslationValue = scItem.KeyEnglish;
        //                    break;

        //                default:
        //                    traslationValue = key;
        //                    break;
        //            }
        //        }
        //    }

        //    #endregion [Fetch Translation Value]

        //    return !string.IsNullOrWhiteSpace(traslationValue) ? traslationValue : (key ?? "");
        //}

        //internal List<SmartResposeDictionary> GetUSCSitecoreTranslation()
        //{
        //    List<SmartResposeDictionary> smTranslations = new List<SmartResposeDictionary>();
        //    Item smTranslationScItem = null;
        //    CacheProvider.TryGet(CacheKeys.SM_SC_TRANSLATION_ITEMS, out smTranslations);
        //    if (smTranslations == null)
        //    {
        //        smTranslations = new List<SmartResposeDictionary>();
        //        smTranslationScItem = Sitecorex.Database.GetItem(new ID("{C13C2AE8-6422-4EE6-9CEA-A7711495BF56}"), LanguageManager.DefaultLanguage);
        //        foreach (Item scItem in smTranslationScItem.Children.ToList<Item>())
        //        {
        //            var t = ContentRepository.GetItem<SmartResposeDictionary>(new GetItemByItemOptions(scItem));
        //            smTranslations.Add(t);
        //        }
        //        CacheProvider.Store(CacheKeys.SM_SC_TRANSLATION_ITEMS, new CacheItem<List<SmartResposeDictionary>>(smTranslations, TimeSpan.FromHours(1)));
        //    }
        //    return smTranslations;
        //}

        //internal List<SmartResposeDictionary> GetCCSitecoreTranslation()
        //{
        //    List<SmartResposeDictionary> smTranslations = new List<SmartResposeDictionary>();
        //    Item smTranslationScItem = null;
        //    CacheProvider.TryGet(CacheKeys.SM_SC_TRANSLATION_ITEMS, out smTranslations);
        //    if (smTranslations == null)
        //    {
        //        smTranslations = new List<SmartResposeDictionary>();
        //        SetContextLanguage("en");
        //        smTranslationScItem = Sitecorex.Database.GetItem(new ID("{C13C2AE8-6422-4EE6-9CEA-A7711495BF56}"));
        //        foreach (Item scItem in smTranslationScItem.Children.ToList<Item>())
        //        {
        //            var t = ContentRepository.GetItem<SmartResposeDictionary>(new GetItemByItemOptions(scItem));
        //            smTranslations.Add(t);
        //        }
        //        CacheProvider.Store(CacheKeys.SM_SC_TRANSLATION_ITEMS, new CacheItem<List<SmartResposeDictionary>>(smTranslations, TimeSpan.FromHours(1)));
        //    }
        //    return smTranslations;
        //}

        //#endregion [Consumption Complaint]
    }

    public class SmartTranslation
    {
        public List<item> Translations;

        /// <summary>
        /// dateTime in yyyyMMddhhmmss
        /// </summary>
        public string datetime { get; set; }

        public class item
        {
            public int id { get; set; }
            public string KeyEnglish { get; set; }

            public string Arabic { get; set; }
            public string Chinese { get; set; }
            public string Philippines { get; set; }

            public string Urdu { get; set; }
        }
    }

    public class Newsfeed
    {
        public List<item> Newsitems;

        public class item
        {
            public string id { get; set; }
            public string title { get; set; }
            public string pubdate { get; set; }

            public string brief { get; set; }

            public string details { get; set; }

            public string thumbnail { get; set; }
            public string iPadthumbnail { get; set; }
            public string largeimage { get; set; }
            public string link { get; set; }
        }

        public int nxtskip { get; set; }
    }

    //public class ConservationTipsFeed
    //{
    //    public List<item> ConservationTips;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string details { get; set; }
    //    }
    //}

    //public class ConservationTipVideos
    //{
    //    public List<item> Videos;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string link { get; set; }
    //    }
    //}

    //public class AboutUs
    //{
    //    public List<item> aboutus;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string thumbnail { get; set; }
    //        public string brief { get; set; }
    //        public string details { get; set; }
    //    }
    //}

    //public class Videos
    //{
    //    public List<item> videos;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string link { get; set; }
    //    }
    //}

    //public class NationalIdentity
    //{
    //    public List<item> nationalid;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string thumbnail { get; set; }
    //        public string largeimage { get; set; }

    //        public string details { get; set; }
    //    }
    //}

    //public class HHQuotes
    //{
    //    public List<item> hhquotes;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string details { get; set; }
    //    }

    //    public class quoteswithheader
    //    {
    //        public string header { get; set; }
    //        public string quote { get; set; }
    //    }
    //}

    //public class Publications
    //{
    //    public List<item> publications;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string thumbnail { get; set; }
    //        public List<Magazine> magazine { get; set; }
    //    }

    //    public class Magazine
    //    {
    //        public List<item> magazine;

    //        public class item
    //        {
    //            public int id { get; set; }
    //            public string title { get; set; }
    //            public string pdate { get; set; }
    //            public string pdf { get; set; }
    //        }
    //    }

    //    public class downloadpdf
    //    {
    //        public string pdf { get; set; }
    //        public string pdate { get; set; }

    //        public string title { get; set; }
    //    }
    //}

    //public class photos
    //{
    //    public List<item> photo;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string category { get; set; }
    //        public string thumbnail { get; set; }

    //        public string image { get; set; }
    //    }

    //    public class photoitem
    //    {
    //        public string thumbnail { get; set; }
    //        public string image { get; set; }
    //    }
    //}

    //public class FeaturedLinks
    //{
    //    public List<item> flinks;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string thumbnail { get; set; }

    //        public string websiteurl { get; set; }
    //        public string details { get; set; }
    //    }
    //}

    //public class Greenbill
    //{
    //    public List<item> greenbill;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string thumbnail { get; set; }

    //        public string largeimage { get; set; }
    //        public string description { get; set; }
    //    }
    //}

    //public class PrivacyPolicy
    //{
    //    public List<item> privacypolicy;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string thumbnail { get; set; }

    //        public string largeimage { get; set; }
    //        public string description { get; set; }
    //    }
    //}

    //public class CareForEarth
    //{
    //    public List<item> careforearth;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string thumbnail { get; set; }

    //        public string largeimage { get; set; }
    //        public string description { get; set; }
    //    }
    //}

    //public class SlabTariff
    //{
    //    public item slabtariff;

    //    public class item
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string thumbnail { get; set; }

    //        public string largeimage { get; set; }
    //        public List<details> details { get; set; }
    //    }

    //    public class details
    //    {
    //        public string description { get; set; }
    //    }
    //}

    //public class SlabTariffCalc
    //{
    //    public mySlabTariff slabtariff;

    //    public class mySlabTariff
    //    {
    //        public myitem item { get; set; }
    //        public tariffcalc tariffitem { get; set; }
    //    }

    //    public class myitem
    //    {
    //        public int id { get; set; }
    //        public string title { get; set; }
    //        public string thumbnail { get; set; }

    //        public string largeimage { get; set; }
    //        public mydetails details { get; set; }
    //    }

    //    public class mydetails
    //    {
    //        public string description { get; set; }
    //    }

    //    public class tariffcalc
    //    {
    //        public tarifftype Electricity { get; set; }
    //        public tarifftype Water { get; set; }
    //        public tarifftype Fuel { get; set; }
    //    }

    //    //Electricity, Water,Fuel
    //    public class tarifftype
    //    {
    //        public string title { get; set; }
    //        public tariffitemtype Residential { get; set; }
    //        public tariffitemtype Commercial { get; set; }
    //        public tariffitemtype Industrial { get; set; }

    //        //Used for Fuel
    //        public tariffitem Electricity { get; set; }

    //        public tariffitem Water { get; set; }
    //    }

    //    //Residential,Commericial
    //    public class tariffitemtype
    //    {
    //        public string title { get; set; }
    //        public List<tariffitem> item { get; set; }
    //    }

    //    public class tariffitem
    //    {
    //        public int category { get; set; }

    //        public string Color { get; set; }

    //        public string Unit { get; set; }

    //        public string Currency { get; set; }

    //        public string Amount { get; set; }

    //        public string ConsFrom { get; set; }

    //        public string ConsTo { get; set; }
    //    }
    //}

    //public class PartnerList
    //{
    //    public class Partner
    //    {
    //        public List<item> partners { get; set; }
    //    }

    //    public class item
    //    {
    //        public string id { get; set; }
    //        public string title { get; set; }
    //        public string websiteurl { get; set; }
    //    }
    //}
}