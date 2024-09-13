using DEWAXP.Feature.Bills.VAT;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;

namespace DEWAXP.Feature.Bills.Controllers
{
    [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
    public class VATController : BaseController
    {
        #region VATSelectAccountGet

        [HttpGet]
        public ActionResult VATSelectAccount()
        {
            ViewBag.Emirates = GetEmirates();
            List<SelectListItem> objListofBusinessPartner = GetBusinessPartner();
            ViewBag.BusinessPartnerNum = objListofBusinessPartner;
            if (objListofBusinessPartner == null || objListofBusinessPartner.Count <= 0)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("No Commercial Account"));
            }
            return View("~/Views/Feature/Bills/VAT/SetVATIDPage.cshtml");
        }

        #endregion VATSelectAccountGet

        #region VATSelectAccountPost

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VATSelectAccount(VATModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string error;
                    if (model.VatDocument != null && !AttachmentIsValid(model.VatDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        try
                        {
                            byte[] attachmentBytes = new byte[0];
                            var attachmentType = string.Empty;

                            if (model.VatDocument != null)
                            {
                                attachmentBytes = model.VatDocument.ToArray();
                                attachmentType = model.VatDocument.GetTrimmedFileExtension();
                            }
                            var response = DewaApiClient.SetVatNumber(string.Empty, model.BusinessPartnerNumber.Split('-')[0].Trim(), model.Emirate, model.VatNumber, attachmentBytes, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                            if (response.Succeeded)
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SET_VAT_SUCCESS);
                            }
                            ModelState.AddModelError(string.Empty, response.Message);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                        }
                    }
                }
                ViewBag.Emirates = GetEmirates();
                ViewBag.BusinessPartnerNum = GetBusinessPartner();
                return View("~/Views/Feature/Bills/VAT/SetVATIDPage.cshtml", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return View("~/Views/Feature/Bills/VAT/SetVATIDPage.cshtml", model);
            }
        }

        #endregion VATSelectAccountPost

        #region RequestSent

        [HttpGet]
        public ActionResult RequestSent()
        {
            return PartialView("~/Views/Feature/Bills/VAT/_RequestSent.cshtml");
        }

        #endregion RequestSent

        #region GetEmirates

        protected List<SelectListItem> GetEmirates()
        {
            try
            {
                var emirates = GetDictionaryListByKey(DictionaryKeys.Global.Emirates);

                var result = from itm in emirates.ToList()
                             select new SelectListItem()
                             {
                                 Text = itm["Phrase"].AddToTermDictionary(itm.DisplayName, DropDownTermValues),
                                 Value = itm.DisplayName
                             };

                return result.ToList();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return null;
            }
        }

        protected List<SelectListItem> GetBusinessPartner()
        {
            List<SelectListItem> objSelectListItem = new List<SelectListItem>();
            try
            {
                var VatDetails = DewaApiClient.GetVatNumber(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (VatDetails.Succeeded && VatDetails.Payload.@return.bpVatList != null && VatDetails.Payload.@return.bpVatList.ToList().Count > 0)
                {
                    var result = from itm in VatDetails.Payload.@return.bpVatList.ToList()
                                 select new SelectListItem()
                                 {
                                     Text = itm.bpnumber + '-' + itm.firstName,
                                     Value = itm.bpnumber + '-' + itm.vatNumber
                                 };
                    return result.ToList();
                }

                return objSelectListItem;
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return objSelectListItem;
            }
        }

        #endregion GetEmirates

        #region DropDownTermValues

        public Dictionary<string, string> DropDownTermValues
        {
            get
            {
                Dictionary<string, string> dictionary;
                if (!CacheProvider.TryGet(CacheKeys.TERMS, out dictionary))
                {
                    dictionary = new Dictionary<string, string>();

                    CacheProvider.Store(CacheKeys.TERMS, new CacheItem<Dictionary<string, string>>(dictionary));
                }
                return dictionary;
            }
        }

        #endregion DropDownTermValues
    }
}