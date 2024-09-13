using DEWAXP.Feature.Bills.CollectiveStatement;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Globalization;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Controllers
{
    [TwoPhaseAuthorize]
    public class CollectiveStatementController : BaseController
    {
        #region Actions

        [HttpGet]
        public ActionResult NewRequest()
        {
            try
            {
                CollectiveStatementModel model;
                if (!CacheProvider.TryGet(CacheKeys.COLLECTIVE_STATEMENT_CACHE, out model))
                {
                    model = ContextRepository.GetCurrentItem<CollectiveStatementModel>();
                    var response = DewaApiClient.GetCollectiveAccounts(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                    if (response.Payload != null)
                    {
                        if (response.Payload.AccountsList != null)
                        {
                            var accounts = response.Payload.AccountsList;
                            var convertedAccounts = accounts.Select(x => new SelectListItem { Text = x.AccountNumber, Value = x.AccountNumber });
                            model.AccountsList = convertedAccounts;
                        }
                        CacheProvider.Store(CacheKeys.COLLECTIVE_STATEMENT_CACHE, new CacheItem<CollectiveStatementModel>(model));
                    }
                    else
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.COLLECTIVE_STATEMENT_FAILED);
                    }
                }
                var content = ContextRepository.GetCurrentItem<GenericPageWithIntro>();
                model.Intro = content.Intro;

                var downloadOptions = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.DOWNLOAD_OPTIONS)).Items;
                var convertedOptions = downloadOptions.Select(y => new SelectListItem { Text = y.Text, Value = y.Value });
                model.DownloadOptions = convertedOptions;
                return View("~/Views/Feature/Bills/CollectiveStatement/_NewRequest.cshtml", model);
            }
            catch (Exception)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.COLLECTIVE_STATEMENT_FAILED);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewRequest(CollectiveStatementModel model)
        {
            try
            {
                CollectiveStatementModel cachedModel;
                CacheProvider.TryGet(CacheKeys.COLLECTIVE_STATEMENT_CACHE, out cachedModel);

                if (ModelState.IsValid)
                {
                    if (model.SelectedDownloadOption == "1")
                    {
                        byte[] pdf = DewaApiClient.GetCollectiveStatementPDF(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, model.SelectedAccount, string.Empty, string.Empty, RequestLanguage, Request.Segment());

                        if (pdf != null)
                        {
                            var url = "/api/CollectiveStatementDownload/GetPDF?accountno={0}";
                            model.DownloadLink = string.Format(url, model.SelectedAccount);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("Error Message Green"));
                        }
                    }
                    else
                    {
                        var downloadLink = Translate.Text(DictionaryKeys.CollectiveStatement.DOWNLOAD_LINK);
                        var url = string.Format(downloadLink, model.SelectedAccount, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
                        model.DownloadLink = url;
                    }
                }

                var content = ContextRepository.GetCurrentItem<GenericPageWithIntro>();
                model.Intro = content.Intro;
                model.AccountsList = cachedModel.AccountsList;
                model.DownloadOptions = cachedModel.DownloadOptions;
                return View("~/Views/Feature/Bills/CollectiveStatement/_NewRequest.cshtml", model);
            }
            catch (Exception)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.COLLECTIVE_STATEMENT_FAILED);
            }
        }

        [HttpGet]
        public ActionResult RequestFailed()
        {
            ConfirmModel confirmModel = new ConfirmModel();
            try
            {
                if (confirmModel.rc == "0")
                {
                    confirmModel.ErrorMessage = Translate.Text("Error Message Individual");
                    confirmModel.ButtonUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.COLLECTIVE_STATEMENT);
                }
                else
                {
                    confirmModel.ErrorMessage = Translate.Text("No Collective Account");
                    confirmModel.ButtonUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                }

                return View("~/Views/Feature/Bills/CollectiveStatement/_RequestFailed.cshtml", confirmModel);
            }
            catch (Exception)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.COLLECTIVE_STATEMENT_FAILED);
            }
        }

        #endregion Actions
    }
}