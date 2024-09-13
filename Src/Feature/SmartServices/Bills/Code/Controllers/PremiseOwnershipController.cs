using DEWAXP.Feature.Bills.Models.Premise;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Logger;
using System;
using System.Web.Mvc;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Integration.Requests.JoinOwnership;
using System.Linq;
using Sitecore.Mvc.Names;
using System.IO;
using System.Web;
using static Glass.Mapper.Constants;
using DEWAXP.Foundation.Helpers.Extensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using DEWAXP.Feature.Bills.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Integration.Responses.GraphSvc;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword;
using Glass.Mapper.Sc.Configuration.Fluent;
using SitecoreX = Sitecore.Context;
using static Sitecore.Configuration.State;
using System.Security.Cryptography;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;

namespace DEWAXP.Feature.Bills.Controllers
{
    public class PremiseOwnershipController : BaseServiceActivationController
    {
        private const string PassportDocType = "Z00001";
        private const string TradLicenseDocType = "Z00005";
        private const string EmiratesDocType = "Z00002";
        private const string ObjectType = "NT";
        private const string PremiseNumList = "premisenumlist_";
        private const string PurchaseAgreement = "purchaseagreement_";
        private const string TitleDeed = "titledeed_";
        private const string IdNumber = "idno_";

        public PremiseOwnershipController() : base()
        {
        }

        [HttpGet, TwoPhaseAuthorize]
        public ActionResult Joint()
        {
            JointOwnerModel model = new JointOwnerModel();
            model.IsLoginUser = IsLoggedIn;
            ViewBag.Nationalities = FormExtensions.GetNationalities(DropDownTermValues);
            ViewBag.Regions = GetEmirates();
            ViewBag.IssuingAuthority = GetIssuingAuthourityList();

            if (IsLoggedIn)
            {
                model.Contractaccount = CurrentPrincipal.PrimaryAccount;
                model.Email = CurrentPrincipal.EmailAddress;
                var mobileNumbers = CurrentPrincipal.MobileNumber;
                if (!string.IsNullOrEmpty(mobileNumbers))
                {
                    var num = mobileNumbers.Substring(0, 1);
                    if (num == "0")
                    {
                        mobileNumbers = mobileNumbers.Substring(1, mobileNumbers.Length - 1);
                    }
                }
                model.Mobilenumber = mobileNumbers;
            }
            return PartialView("~/Views/Feature/Bills/Ownership/_Joint.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken, TwoPhaseAuthorize]
        public ActionResult Joint(JointOwnerModel model)
        {
            string _errorMessage = "";
            try
            {
                if (!ModelState.IsValid)
                {
                    _errorMessage = string.Join(" | ", ModelState.Values
                                                                .SelectMany(v => v.Errors)
                                                                .Select(e => e.ErrorMessage));
                }

                JointOwnerRequest request = new JointOwnerRequest();
                request.jointownerinputs.email = model.Email;
                request.jointownerinputs.mobilenumber = "0" + model.Mobilenumber;
                request.jointownerinputs.pobox = model.Pobox;
                request.jointownerinputs.region = model.Region;
                request.jointownerinputs.contractaccount = model.Contractaccount;
                request.jointownerinputs.sessionid = CurrentPrincipal.SessionToken ?? string.Empty;

                if (model.TypeOfAccount == 1 && model.ContractAccountfile != null)
                {
                    if (model.ContractAccountfile != null && model.ContractAccountfile.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.ContractAccountfile, General.MaxAttachmentSize, out _errorMessage, General.AcceptedFileTypes))
                        {
                            ModelState.AddModelError(string.Empty, _errorMessage);
                        }
                        else
                        {
                            request.jointownerinputs.attachments.filename1 = PremiseNumList + model.ContractAccountfile.FileName.GetFileNameWithoutPath();
                            request.jointownerinputs.attachments.filedata1 = Convert.ToBase64String(model.ContractAccountfile.ToArray());
                        }
                    }
                }
                else if (model.TypeOfAccount == 2)
                {
                    var array = model.AccountList.Split(',').ToList<string>();

                    foreach (var item in array)
                    {
                        request.jointownerinputs.premiselist.Add(new
                            Foundation.Integration.Requests.JoinOwnership.Premiselist
                        {
                            premisenumber = item.ToString()
                        });
                    }
                }

                if (model.PurchaseAgreementFile != null && model.PurchaseAgreementFile.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.PurchaseAgreementFile, General.MaxAttachmentSize, out _errorMessage, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, _errorMessage);
                    }
                    else
                    {
                        request.jointownerinputs.attachments.filename2 = (model.PropertyType == "1" ? PurchaseAgreement : TitleDeed) + model.PurchaseAgreementFile.FileName.GetFileNameWithoutPath();
                        request.jointownerinputs.attachments.filedata2 = Convert.ToBase64String(model.PurchaseAgreementFile.ToArray());
                    }
                }

                foreach (var item in model.Ownerlist.Where(x => x.PassportNumber != null || x.TradeLicense != null || x.EmiratesIdNumber != null))
                {
                    request.jointownerinputs.ownerlist.Add(new Foundation.Integration.Requests.JoinOwnership.Ownerlist
                    {
                        nationality = item.Nationality,
                        issuingauthority = item.Issuingauthority,
                        filename = null,
                        idno = GetDocumentNumber(item),
                        idtye = GetIdType(item),
                    });
                }

                var response = PremiseHandler.PostJointOwnershipRequest(request, RequestLanguage);

                if (response.Succeeded && response.Payload != null)
                {
                    try
                    {
                        SaveAttachments(model.Ownerlist, response?.Payload?.NotificationNumber);
                    }
                    catch (System.Exception exx)
                    {
                        LogService.Error(exx, this);
                    }

                    string _refKey = $"refKey{response.Payload?.NotificationNumber}";
                    CacheProvider.Store(_refKey, new AccessCountingCacheItem<SuccessModel>(new SuccessModel()
                    {
                        ReferenceNumber = response.Payload?.NotificationNumber,
                        Name = CurrentPrincipal?.FullName,
                        Date = DateTime.Now.ToString("dd MMM yyyy | HH:mm:ss", SitecoreX.Culture)
                    }, Times.Exactly(2)));
                    QueryString q = new QueryString(true);
                    q.With("n", _refKey, true);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTOWNERSUCESS, q);
                }
                else
                {
                    _errorMessage = response.Message;
                }

            }
            catch (System.Exception ex)
            {
                _errorMessage = ex.Message;
                LogService.Error(ex, this);
            }
            if (!string.IsNullOrWhiteSpace(_errorMessage))
            {
                ModelState.AddModelError("", _errorMessage);
            }
            ViewBag.Nationalities = FormExtensions.GetNationalities(DropDownTermValues);
            ViewBag.IssuingAuthority = GetIssuingAuthourityList();
            ViewBag.Regions = GetEmirates();
            model.IsLoginUser = IsLoggedIn;
            RemoveFileData(model.Ownerlist);
            return PartialView("~/Views/Feature/Bills/Ownership/_Joint.cshtml", model);
        }

        [HttpGet]
        [TwoPhaseAuthorize]
        public ActionResult Success(string n)
        {
            SuccessModel m = null;
            if (CacheProvider.TryGet(n, out m) && !string.IsNullOrWhiteSpace(m.ReferenceNumber))
            {
                return View("~/Views/Feature/Bills/Ownership/_JointSuccess.cshtml", m);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTOWNERPAGE);
        }

        #region CustomMethods

        private void SaveAttachments(List<DEWAXP.Feature.Bills.Models.Premise.Ownerlist> model, string notificationNumber)
        {
            string _errorMessage = string.Empty;

            foreach (var obj in model)
            {
                if (obj.PassportFiledata == null && obj.TradeFiledata == null && obj.EmiratesIdDataFile == null)
                {
                    continue;
                }

                var req = new JointOwnerAttachmentRequest();
                var file = GetFileData(obj);

                if (file != null && file.ContentLength > 0)
                {
                    if (!AttachmentIsValid(file, General.MaxAttachmentSize, out _errorMessage, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, _errorMessage);
                    }
                    else
                    {
                        req.saveattachmentinputs.objectid = notificationNumber;
                        req.saveattachmentinputs.objecttype = ObjectType;
                        req.saveattachmentinputs.filename = GetDocumentNumber(obj) + "_" + file?.FileName.GetFileNameWithoutPath();
                        req.saveattachmentinputs.filedata = Convert.ToBase64String(file?.ToArray());

                        var response = PremiseHandler.SaveJointOwnershipAttachment(req);
                    }
                }
            }
        }

        private List<SelectListItem> GetEmirates()
        {
            List<SelectListItem> rgns = new List<SelectListItem>();
            rgns = GetLstDataSource(DataSources.EmiratesList).ToList();
            return rgns;
        }

        private void RemoveFileData(List<DEWAXP.Feature.Bills.Models.Premise.Ownerlist> model)
        {
            foreach (var item in model)
            {
                item.TradeFiledata = null;
                item.PassportFiledata = null;
                item.EmiratesIdDataFile = null;
            }
        }

        private string GetIdType(DEWAXP.Feature.Bills.Models.Premise.Ownerlist model)
        {
            string type = null;
            switch (model.Type)
            {
                case "0":
                    type = PassportDocType;
                    break;
                    case "1":
                    type = TradLicenseDocType;
                    break;
                case "2":
                    type = EmiratesDocType;
                    break;
                default:
                    break;
            }
            return type;
        }

        private string GetDocumentNumber(DEWAXP.Feature.Bills.Models.Premise.Ownerlist model)
        {
            string doc = null;
            switch (model.Type)
            {
                case "0":
                    doc = model.PassportNumber;
                    break;
                case "1":
                    doc = model.TradeLicense;
                    break;
                case "2":
                    doc = model.EmiratesIdNumber;
                    break;
                default:
                    break;
            }
            return doc;
        }

        private HttpPostedFileBase GetFileData(DEWAXP.Feature.Bills.Models.Premise.Ownerlist model)
        {
            HttpPostedFileBase doc = null;
            switch (model.Type)
            {
                case "0":
                    doc = model.PassportFiledata;
                    break;
                case "1":
                    doc = model.TradeFiledata;
                    break;
                case "2":
                    doc = model.EmiratesIdDataFile;
                    break;
                default:
                    break;
            }
            return doc;
        }

        private IEnumerable<SelectListItem> GetIssuingAuthourityList()
        {
            IEnumerable<SelectListItem> IssuingAuthorityList = null;
            try
            {
                var response = DewaApiClient.SetMoveInReadRequest(new moveInReadInput
                {
                    lang = RequestLanguage.Code(),
                    userid = CurrentPrincipal.UserId,
                    sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                    gccflag = "",
                    nationflag = "X",
                    regionflag = "X",
                    licenseauthority = "X",
                    paychannelflag = "",
                    govtflag = string.Empty
                }, Request.Segment());

                if (response.Succeeded && response.Payload != null)
                {
                    IssuingAuthorityList = response.Payload.licenseDetailsList != null ? response.Payload.licenseDetailsList.Select(x => new SelectListItem { Text = x.description, Value = x.authritycode }) : Enumerable.Empty<SelectListItem>();
                }
            }
            catch (System.Exception)
            {
                throw;
            }

            return IssuingAuthorityList;
        }
        #endregion
    }
}