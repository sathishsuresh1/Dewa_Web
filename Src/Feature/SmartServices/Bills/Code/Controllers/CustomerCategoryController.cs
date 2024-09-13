using DEWAXP.Feature.Bills.Models.ChangeCustomerCategory;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Globalization;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Controllers
{
    [TwoPhaseAuthorize]
    public class CustomerCategoryController : BaseController
    {
        [HttpGet]
        public ActionResult ChangeCustomerCategory()
        {
            CacheProvider.Remove(CacheKeys.CHANGE_CUSTOMER_CATEGORY_STATE);

            var accountNumber = string.Empty;
            var premiseNumber = string.Empty;
            var bpNumber = string.Empty;

            var primaryAccountResponse = DewaApiClient.GetPrimaryAccount(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            //var accountListResponse = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
            var accountListResponse = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

            if (primaryAccountResponse.Succeeded && accountListResponse.Succeeded)
            {
                accountNumber = primaryAccountResponse.Payload.AccountNumber;

                var account = accountListResponse.Payload.SingleOrDefault(a => a.AccountNumber == accountNumber);
                if (account != null)
                {
                    premiseNumber = account.PremiseNumber;
                    bpNumber = account.BusinessPartnerNumber;
                }
                else
                {
                    accountNumber = accountListResponse.Payload[0].AccountNumber;
                    premiseNumber = accountListResponse.Payload[0].PremiseNumber;
                    bpNumber = accountListResponse.Payload[0].BusinessPartnerNumber;
                }
            }

            return PartialView("~/Views/Feature/Bills/CustomerCategory/_ChangeCustomerCategory.cshtml", new ChangeCustomerCategoryModel
            {
                ContractAccountNumber = accountNumber,
                BusinessPartnerNumber = bpNumber,
                PremiseNumber = premiseNumber
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangeCustomerCategory(ChangeCustomerCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                string error;
                if (model.IdUploader != null && !AttachmentIsValid(model.IdUploader, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    try
                    {
                        byte[] attachmentBytes = new byte[0];
                        var attachmentType = string.Empty;

                        if (model.IdUploader != null)
                        {
                            attachmentBytes = model.IdUploader.ToArray();
                            attachmentType = model.IdUploader.GetTrimmedFileExtension();
                        }

                        var response = DewaApiClient.ChangeCustomerCategory(CurrentPrincipal.UserId,
                            CurrentPrincipal.SessionToken, model.PremiseNumber, model.ContractAccountNumber,
                            model.BusinessPartnerNumber, model.Mobile.AddMobileNumberZeroPrefix(), model.Description, attachmentBytes, attachmentType, RequestLanguage, Request.Segment());
                        if (response.Succeeded)
                        {
                            var referenceNumber = response.Payload.NotificationNumber;

                            CacheProvider.Store(CacheKeys.CHANGE_CUSTOMER_CATEGORY_STATE, new CacheItem<string>(referenceNumber));

                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J88_CHANGE_CUSTOMER_CATEGORY_SUCCESS);
                        }
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                    }
                }
            }
            return PartialView("~/Views/Feature/Bills/CustomerCategory/_ChangeCustomerCategory.cshtml", model);
        }

        [HttpGet]
        public ActionResult ChangeCustomerCategorySuccess()
        {
            string refNum;
            if (!CacheProvider.TryGet(CacheKeys.CHANGE_CUSTOMER_CATEGORY_STATE, out refNum))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J88_CHANGE_CUSTOMER_CATEGORY);
            }

            ViewBag.ReferenceNumber = refNum;

            return PartialView("~/Views/Feature/Bills/CustomerCategory/_ChangeCustomerCategorySuccess.cshtml", refNum);
        }
    }
}