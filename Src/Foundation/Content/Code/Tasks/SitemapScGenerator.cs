using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Models.TaskModels;
using DEWAXP.Foundation.CustomDB.DataModel;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc;
using global::Sitecore.Data;
using global::Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using _scTemplate = DEWAXP.Foundation.Content.ScTemplate;
using SitecoreX = global::Sitecore;

namespace DEWAXP.Foundation.Content.Tasks
{
    public class SitemapScGenerator
    {
        #region [variable]

        private readonly Guid myAccount = new Guid(SitecoreItemIdentifiers.MY_ACCOUNT);
        private readonly Guid loginPage = new Guid(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

        private static Database webDB = Database.GetDatabase("web");
        private PageBase _homePage = null;
        private SiteContext rootPage = global::Sitecore.Context.Site;
        private SitecoreService service = new SitecoreService(webDB);
        private SitemapInfo _sitemapInfo = new SitemapInfo();
        private SitemapGenerationConfig sitemapGenerationConfig = new SitemapGenerationConfig();

        #endregion [variable]

        public SitemapScGenerator()
        {
            try
            {
                if (rootPage != null)
                {
                    _homePage = service.GetItem<PageBase>(SitecoreItemIdentifiers.HOME);
                    _sitemapInfo.children = new List<SitemapInfo>();

                    #region [Setting Output Details]

                    sitemapGenerationConfig = service.GetItem<SitemapGenerationConfig>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.SITEMAP_GENERATION_CONFIG)) { Language = Language.Parse(SitecoreX.Context.Language.Name) });
                    if (sitemapGenerationConfig == null)
                    {
                        sitemapGenerationConfig = new SitemapGenerationConfig()
                        {
                            MaxCount = "4",
                            DomainUrl = "https://www.dewa.gov.ae"
                        };
                    }

                    #endregion [Setting Output Details]
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
        }

        public void Execute(Item[] items, SitecoreX.Tasks.CommandItem command, SitecoreX.Tasks.ScheduleItem schedule)
        {
            try
            {
                if (_homePage != null)
                {
                    _sitemapInfo = GetSitemapInfo(_homePage, 0);
                    foreach (Item landing in _homePage.Item.Children)
                    {
                        var _landigPage = service.GetItem<PageBase>(landing);
                        if (!IsExcludeTemplateType(_landigPage)) //not a folder
                        {
                            SitemapInfo d = RenderSiteMapLevelOneAndTwo(_sitemapInfo, _landigPage, true);
                            if (d != null)
                            {
                                _sitemapInfo.children.Add(d);
                            }
                        }
                    }
                }

                if (_sitemapInfo != null)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var jsonString = js.Serialize(_sitemapInfo);

                    if (!string.IsNullOrWhiteSpace(sitemapGenerationConfig.OutputPath))
                    {
                        string file = String.Format("{0}\\dewa-sitemap.json", sitemapGenerationConfig.OutputPath);
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }

                        using (StreamWriter fs = new StreamWriter(file, true))
                        {
                            fs.WriteLine(jsonString);
                            fs.Close();
                        }
                    }

                    using (var context = new Entities())
                    {
                        if (context.PROC_StoreSitemapData(jsonString) <= 0)
                        {
                            LogService.Fatal(new Exception("unable to dump sitemap data"), this);
                        }
                    }
                }
                else
                {
                    LogService.Fatal(new Exception("unable to fetch siitemap data"), this);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
        }

        #region [function]

        private bool IsExcludeTemplateType(PageBase page)
        {
            if (page == null || page.Id == Guid.Empty || page.TemplateId == new Guid(Folder.TemplateIdString))
            {
                return true;
            }

            return false;
        }

        private string GetPageName(PageBase item)
        {
            string title = item.MenuLabel;

            if (string.IsNullOrEmpty(title) && item.Item != null && !ID.IsNullOrEmpty(item.Item.ID))
            {
                if (!string.IsNullOrWhiteSpace(item.Name))
                {
                    title = item.Name;
                }
                else if (item.Item.Fields["Browser Title"] != null && !string.IsNullOrEmpty(item.Item.Fields["Browser Title"].Value))
                {
                    title = item.Item.Fields["Browser Title"].Value;
                }
            }

            return title;
        }

        private SitemapInfo GetSitemapInfo(PageBase page, int level)
        {
            SitemapInfo pageInfo = null;
            if (page != null)
            {
                pageInfo = new SitemapInfo();
                pageInfo.page_id = page.Id.ToString();
                pageInfo.level = level;
                pageInfo.page_type = page.TemplateName;
                pageInfo.page_type_id = page.TemplateId.ToString();
                foreach (Language lang in webDB.Languages.ToList())
                {
                    var pageDetail = service.GetItem<PageBase>(new GetItemByIdOptions(Guid.Parse(page.Id.ToString())) { Language = lang });

                    if (pageDetail != null)
                    {
                        var options = SitecoreX.Links.LinkManager.GetDefaultUrlBuilderOptions();
                        options.Language = lang;
                        options.LowercaseUrls = true;
                        options.LanguageEmbedding = SitecoreX.Links.LanguageEmbedding.Always;

                        if (lang.CultureInfo.TwoLetterISOLanguageName == "en")
                        {
                            pageInfo.name = GetPageName(pageDetail);
                            pageInfo.url = SitecoreX.Links.LinkManager.GetItemUrl(pageDetail.Item, options);

                            pageInfo.url = BeautfyPageUrl(pageInfo.url);
                        }

                        if (lang.CultureInfo.TwoLetterISOLanguageName == "ar")
                        {
                            pageInfo.name_ar = GetPageName(pageDetail);
                            pageInfo.url_ar = SitecoreX.Links.LinkManager.GetItemUrl(pageDetail.Item, options);
                            pageInfo.url_ar = BeautfyPageUrl(pageInfo.url_ar);
                        }
                    }
                }
            }

            return pageInfo;
        }

        private string BeautfyPageUrl(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (!url.Contains("http"))
                {
                    url = sitemapGenerationConfig.DomainUrl + url;
                }

                if (url.Contains("/sitecore/content/"))
                {
                    url = url.Replace("/sitecore/content/dewa/home", "");
                }
            }
            return url;
        }

        private bool IsExcludebelowLevel1(PageBase page)
        {
            if (page == null || page.Id == Guid.Empty || page.HideFromSitemapPage)
            {
                return true;
            }

            //|| page.TemplateId == subLandingFolder  // as per obesrvations
            return false;
        }

        private SitemapInfo RenderSiteMapLevelOneAndTwo(SitemapInfo sitemapInfo, PageBase item, bool isMainLanding = false)
        {
            SitemapInfo sublevelSitemapInfo = new SitemapInfo();
            var liClass = string.Empty;
            var linkClass = string.Empty;
            var ulClass = string.Empty;

            if (item == null || item.Id == Guid.Empty || item.HideFromSitemapPage)
            {
                return null;
            }
            sublevelSitemapInfo = GetSitemapInfo(item, Convert.ToInt32(sitemapInfo?.level + 1));
            bool ItemHasChildren = item.Item.HasChildren;
            if (item.Id == myAccount)
            {
                var loginItem = service.GetItem<PageBase>(loginPage);
                if (loginItem != null && loginItem.Id != Guid.Empty)
                {
                    sublevelSitemapInfo = GetSitemapInfo(loginItem, Convert.ToInt32(sitemapInfo?.level + 1));
                    //sitemapInfo.url = loginItem.Url;
                }
            }

            if (ItemHasChildren)
            {
                int level2 = Convert.ToInt32(sublevelSitemapInfo?.level + 1);

                //only common folder & has children & not null after casting to PageBase
                var CommonFolderItem = item.Item.Children.Where(x => !IsExcludebelowLevel1(service.GetItem<PageBase>(x)) && x.Children.Count > 0 && x.TemplateID.Guid == Guid.Parse(_scTemplate.Common_FolderPgTmpId)).ToList();
                //only persona folder & has children & not null after casting to PageBase
                var SectionFolderItem = item.Item.Children.Where(x => !IsExcludebelowLevel1(service.GetItem<PageBase>(x)) && x.Children.Count > 0 && x.TemplateID.Guid == Guid.Parse(_scTemplate.Section_FolderPgTmpId)).ToList();
                //Other than common and persona folder & has children & not null after casting to PageBase
                var OtherItem = item.Item.Children.Where(x => !IsExcludebelowLevel1(service.GetItem<PageBase>(x)) && x.Children.Count > 0 && x.TemplateID.Guid != Guid.Parse(_scTemplate.Section_FolderPgTmpId) && x.TemplateID.Guid != Guid.Parse(_scTemplate.Common_FolderPgTmpId)).ToList();

                List<SitemapInfo> SectionAndOtherItemInfo = new List<SitemapInfo>();
                if (CommonFolderItem != null && CommonFolderItem.Count() > 0)
                {
                    foreach (PageBase childItem in CommonFolderItem.Where(x => x != null).Select(x => service.GetItem<PageBase>(x)).ToList())
                    {
                        var pageInfo = GetSitemapInfo(childItem, level2);
                        pageInfo = RenderLevelThirdandFourth(pageInfo, childItem, false);
                        pageInfo.is_label_only = true;
                        SectionAndOtherItemInfo.Add(pageInfo);
                    }
                }

                if (SectionFolderItem != null && SectionFolderItem.Count() > 0)
                {
                    foreach (PageBase childItem in SectionFolderItem.Where(x => x != null).Select(x => service.GetItem<PageBase>(x)).ToList())
                    {
                        var pageInfo = GetSitemapInfo(childItem, level2);
                        pageInfo = RenderLevelThirdandFourth(pageInfo, childItem, false);
                        pageInfo.is_label_only = true;
                        SectionAndOtherItemInfo.Add(pageInfo);
                    }
                }

                if (OtherItem != null && OtherItem.Count() > 0)
                {
                    foreach (PageBase childItem in OtherItem.Where(x => x != null).Select(x => service.GetItem<PageBase>(x)).ToList())
                    {
                        var pageInfo = GetSitemapInfo(childItem, level2);
                        pageInfo = RenderLevelThirdandFourth(pageInfo, childItem, false);
                        pageInfo.is_label_only = true;
                        SectionAndOtherItemInfo.Add(pageInfo);
                    }
                }
                if (sublevelSitemapInfo.children == null)
                {
                    sublevelSitemapInfo.children = new List<SitemapInfo>();
                }
                sublevelSitemapInfo.children.AddRange(SectionAndOtherItemInfo);
            }

            return sublevelSitemapInfo;
        }

        private SitemapInfo RenderLevelThirdandFourth(SitemapInfo sitemapInfo, PageBase item, bool isMainLanding = false)
        {
            /**/

            int leveNo = Convert.ToInt32(sitemapInfo.level + 1);
            if (IsExcludebelowLevel1(item) ||
                Convert.ToInt32(sitemapGenerationConfig.MaxCount ?? "4") <= leveNo)
            {
                return sitemapInfo;
            }
            bool ItemHasChildren = item.Item.HasChildren;
            if (ItemHasChildren)
            {
                foreach (Item subitem in item.Item.Children)
                {
                    var childContent = service.GetItem<PageBase>(subitem);
                    if (!IsExcludebelowLevel1(childContent) && childContent.TemplateId != new Guid(Folder.TemplateIdString))
                    {
                        var d = GetSitemapInfo(childContent, leveNo);
                        if (childContent.Item.Children.Count > 0)
                        {
                            d = RenderLevelThirdandFourth(d, childContent, false);
                        }
                        if (sitemapInfo.children == null)
                        {
                            sitemapInfo.children = new List<SitemapInfo>();
                        }
                        sitemapInfo.children.Add(d);
                    }
                }
            }
            return sitemapInfo;
        }

        #endregion [function]
    }
}