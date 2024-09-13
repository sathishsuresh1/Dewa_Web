using DEWAXP.Feature.GeneralServices.Models.Infrastructure_Noc;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
using DEWAXP.Foundation.Integration.Responses.SmartCustomer;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    public class InfrastructureNocController : BaseController
    {
        private long FileSize = (5 * 1024 * 1024);
        private string[] FileType = { ".PDF", ".JPG", ".JPEG", ".PNG", ".BMP", };

        // GET: InfrastructureNoc

        #region Action Result

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpGet]
        public ActionResult NocRequest(string id)
        {
            InfrastructureNocReqModel model = new InfrastructureNocReqModel();
            SharedAccount selectedAccounts = new SharedAccount();
            try
            {
                // Get Infrastructure NOC details
                model = GetNocRequestDetails(id);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return PartialView("~/Views/Feature/GeneralServices/InfrastructureNoc/InfrastructureNocRequest.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpPost, ValidateAntiForgeryToken]
        public ActionResult NocRequest(InfrastructureNocReqModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string error = null;
                    List<Attach> nocRequestAttachments = new List<Attach>();
                    List<SelectListItem> workTypes = new List<SelectListItem>();
                    if (CacheProvider.TryGet("WorkTypes", out workTypes))
                    {
                        model.WorkTypeList = workTypes;
                    }
                    if (model != null && model.Copy_AffectionPlan != null && model.Copy_AffectionPlan.ContentLength > 0)
                    {
                        if (CustomeAttachmentIsValid(model.Copy_AffectionPlan, FileSize, out error, FileType))
                        {
                            using (var memoryStream_8 = new MemoryStream())
                            {
                                model.Copy_AffectionPlan.InputStream.CopyTo(memoryStream_8);
                                nocRequestAttachments.Add(new Attach
                                {
                                    filename = model.Copy_AffectionPlan.FileName,
                                    filecontent = Convert.ToBase64String(memoryStream_8.ToArray() ?? new byte[0]),
                                    filesize = model.Copy_AffectionPlan.ContentLength.ToString(),
                                    mimetype = model.Copy_AffectionPlan.ContentType,
                                    fileid = Guid.NewGuid().ToString(),
                                });
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", error);
                            ModelState.AddModelError("Copy_AffectionPlan", error);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(model.TransactionId))
                            ModelState.AddModelError(string.Empty, Translate.Text("NOC_File2RequiredField"));
                    }
                    if (model != null && model.Cover_Letter != null && model.Cover_Letter.ContentLength > 0)
                    {
                        if (CustomeAttachmentIsValid(model.Cover_Letter, FileSize, out error, FileType))
                        {
                            using (var memoryStream_8 = new MemoryStream())
                            {
                                model.Cover_Letter.InputStream.CopyTo(memoryStream_8);
                                nocRequestAttachments.Add(new Attach
                                {
                                    filename = model.Cover_Letter.FileName,
                                    filecontent = Convert.ToBase64String(memoryStream_8.ToArray() ?? new byte[0]),
                                    filesize = model.Cover_Letter.ContentLength.ToString(),
                                    mimetype = model.Cover_Letter.ContentType,
                                    fileid = Guid.NewGuid().ToString(),
                                });
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", error);
                            ModelState.AddModelError("Cover_Letter", error);
                        }
                    }
                    if (model != null && model.ProposedWorked_Sketch != null && model.ProposedWorked_Sketch.ContentLength > 0)
                    {
                        if (CustomeAttachmentIsValid(model.ProposedWorked_Sketch, FileSize, out error, FileType))
                        {
                            using (var memoryStream_8 = new MemoryStream())
                            {
                                model.ProposedWorked_Sketch.InputStream.CopyTo(memoryStream_8);
                                nocRequestAttachments.Add(new Attach
                                {
                                    filename = model.ProposedWorked_Sketch.FileName,
                                    filecontent = Convert.ToBase64String(memoryStream_8.ToArray() ?? new byte[0]),
                                    filesize = model.ProposedWorked_Sketch.ContentLength.ToString(),
                                    mimetype = model.ProposedWorked_Sketch.ContentType,
                                    fileid = Guid.NewGuid().ToString(),
                                });
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", error);
                            ModelState.AddModelError("ProposedWorked_Sketch", error);
                        }
                    }
                    if (ModelState.IsValid)
                    {
                        if (IsValidNOCAccount(model.ContractAccount))
                        {
                            var response = SmartCustomerClient.SubmitNewNocRequest(new InfraNocSubmitRequest
                            {
                                InfraNocRequest = new InfraNocRequest
                                {
                                    businesspartner = model.BusinessPartner,
                                    contractaccount = model.ContractAccount,
                                    plotnumber = model.PlotNumber,
                                    proposedWorktype = model.SelectedWorkType,
                                    descproposedWorktype = model.DescProposedWorkYype,
                                    attach = nocRequestAttachments.ToArray(),
                                    customernotes = model.CustomerNotes,
                                    transactionid = model.TransactionId
                                }
                            }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                            if (response != null && response.Payload != null && response.Succeeded && response.Payload.responsecode.Equals("000"))
                            {
                                SuccessModel successModel = new SuccessModel();
                                successModel.IsSuccess = true;
                                if (model.TransactionId != null)
                                    successModel.DEWAnum = model.TransactionId;
                                else
                                    successModel.DEWAnum = response.Payload.DEWAnum;
                                successModel.responsecode = response.Payload.responsecode;
                                successModel.description = response.Payload.description;
                                return PartialView("~/Views/Feature/GeneralServices/InfrastructureNoc/_Success.cshtml", successModel);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, response.Message);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("PrimseTypeInValidationMessage"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return PartialView("~/Views/Feature/GeneralServices/InfrastructureNoc/InfrastructureNocRequest.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpGet]
        public ActionResult ViewNocRequest(string id)
        {
            InfrastructureNocReqModel model = new InfrastructureNocReqModel();

            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    model = GetNocRequestDetails(id);
                    if (!ModelState.IsValid)
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.INFRASTRUCTURE_NOC_STATUS);
                }
                else
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.INFRASTRUCTURE_NOC_STATUS);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return PartialView("~/Views/Feature/GeneralServices/InfrastructureNoc/ViewNocRequest.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetDocuments(string transactionId, string folderName, string docType)
        {
            InfrastructureNocReqModel model = new InfrastructureNocReqModel();
            model.TransactionId = transactionId;
            model.Revision = folderName;
            try
            {
                List<NocRequestAttachments> cacheModel = new List<NocRequestAttachments>();
                if (!string.IsNullOrWhiteSpace(docType) && docType.Equals("UD"))
                {
                    if (CacheProvider.TryGet("UploadedDocuments", out cacheModel))
                    {
                        model.nocReqAttachments = cacheModel.Where(x => x.Folder == folderName).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(docType) && docType.Equals("DD"))
                {
                    if (CacheProvider.TryGet("DewaDocuments", out cacheModel))
                    {
                        model.nocReqDewaAttachments = cacheModel.Where(x => x.Folder == folderName).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return PartialView("~/Views/Feature/GeneralServices/InfrastructureNoc/_viewDocuments.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetStatusComment(string transactionId)
        {
            InfrastructureNocReqModel model = new InfrastructureNocReqModel();
            try
            {
                model.TransactionId = transactionId;
                var response = SmartCustomerClient.GetStatusDetails(new InfrastructureNocRequest
                {
                    transactionid = transactionId
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response != null && response.Payload != null && response.Payload.responsecode.Equals("000"))
                {
                    if (response.Payload.interactionhistory != null)
                    {
                        model.InteractionHistory = response.Payload.interactionhistory;
                    }
                    if (response.Payload.status != null)
                    {
                        model.statusList = response.Payload.status;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return PartialView("~/Views/Feature/GeneralServices/InfrastructureNoc/_viewStatusComment.cshtml", model);
        }

        [HttpGet]
        public FileResult DownloadAttachment(string transactionid, string fileid)
        {
            if (!string.IsNullOrEmpty(transactionid) && !string.IsNullOrEmpty(fileid))
            {
                var response = SmartCustomerClient.DownloadFile(new InfrastructureNocRequest
                {
                    transactionid = transactionid,
                    fileid = fileid
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null)
                {
                    byte[] bytes = Convert.FromBase64String(response.Payload.attach.filecontent);
                    return File(bytes, response.Payload.attach.mimetype, response.Payload.attach.filename);
                }
                return null;
            }
            return null;
        }

        [HttpGet]
        public JsonResult CheckAccountNoValid(string acNo)
        {
            return Json(new { success = IsValidNOCAccount(acNo) }, JsonRequestBehavior.AllowGet);
        }

        #endregion Action Result

        #region Private Methods

        /// <summary>
        /// Get Infrastructure NOC details
        /// </summary>
        /// <param name="id">Transaction ID</param>
        /// <returns>InfrastructureNocReqModel</returns>
        private InfrastructureNocReqModel GetNocRequestDetails(string id)
        {
            InfrastructureNocReqModel model = new InfrastructureNocReqModel();
            try
            {
                Account selectedAccounts = new Account();
                model.BusinessPartner = CurrentPrincipal.BusinessPartner;

                if (!string.IsNullOrWhiteSpace(id))
                {
                    model.TransactionId = id;
                }
                if (!string.IsNullOrWhiteSpace(model.TransactionId))
                {
                    var responseDetails = SmartCustomerClient.GetInfrastructureNocDetails(new InfrastructureNocRequest
                    {
                        transactionid = model.TransactionId
                    }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                    if (responseDetails != null && responseDetails.Succeeded && responseDetails.Payload != null && responseDetails.Payload.responsecode.Equals("000"))
                    {
                        model.BusinessPartner = responseDetails.Payload.businesspartner;
                        model.PlotNumber = responseDetails.Payload.plotnumber;
                        model.SelectedWorkType = responseDetails.Payload.proposedWorktype;
                        model.DescProposedWorkYype = responseDetails.Payload.descproposedWorktype;
                        model.Status = responseDetails.Payload.status;
                        model.StatusDescription = responseDetails.Payload.statusdescription;
                        model.SubmittedDate = !string.IsNullOrWhiteSpace(responseDetails.Payload.submissiondate) && responseDetails.Payload.submissiondate != "0000-00-00" ? DateTime.Parse(responseDetails.Payload.submissiondate).ToString("dd MMM yyyy", SitecoreX.Culture) : "";
                        model.ContractAccount = responseDetails.Payload.contractaccount;
                        model.Revision = responseDetails.Payload.revision;
                        model.CustomerNotes = responseDetails.Payload.customernotes;

                        if (responseDetails.Payload != null && responseDetails.Payload.uploadedDocuments != null)
                        {
                            List<UploadedDocuments> uploadedDocuments = new List<UploadedDocuments>();
                            List<NocRequestAttachments> requestAttachments = new List<NocRequestAttachments>();
                            uploadedDocuments = responseDetails.Payload.uploadedDocuments.ToList();
                            if (uploadedDocuments != null)
                            {
                                foreach (UploadedDocuments item in uploadedDocuments)
                                {
                                    if (item != null && item.AttachedDocuments != null)
                                    {
                                        foreach (var doc in item.AttachedDocuments)
                                        {
                                            requestAttachments.Add(new NocRequestAttachments
                                            {
                                                DocDate = !string.IsNullOrWhiteSpace(doc.docdate) && !doc.docdate.Equals("0000-00-00") ?
                                                     DateTime.ParseExact(doc.docdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMMM yyyy", SitecoreX.Culture) : string.Empty,
                                                DocType = doc.doctype,
                                                FileContent = doc.filecontent,
                                                FileId = doc.fileid,
                                                FileName = doc.filename,
                                                FileSize = doc.filesize,
                                                Folder = doc.folder
                                            });
                                        }
                                        //model.nocReqAttachments.AddRange( item.AttachedDocuments.Select(y => new NocRequestAttachments
                                        //{
                                        //    DocDate = !string.IsNullOrWhiteSpace(y.docdate) && !y.docdate.Equals("0000-00-00") ?
                                        //             DateTime.ParseExact(y.docdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMMM yyyy", SitecoreX.Culture) : string.Empty,
                                        //    DocType = y.doctype,
                                        //    FileContent = y.filecontent,
                                        //    FileId = y.fileid,
                                        //    FileName = y.filename,
                                        //    FileSize = y.filesize,
                                        //    Folder = y.folder
                                        //}).ToList());
                                    }
                                }
                                model.nocReqAttachments = requestAttachments;
                                CacheProvider.Store("UploadedDocuments", new CacheItem<List<NocRequestAttachments>>(model.nocReqAttachments));
                            }
                        }
                        if (responseDetails.Payload != null && responseDetails.Payload.dewadocuments != null)
                        {
                            List<UploadedDocuments> dewaDocuments = new List<UploadedDocuments>();
                            List<NocRequestAttachments> requestAttachments = new List<NocRequestAttachments>();
                            dewaDocuments = responseDetails.Payload.dewadocuments.ToList();
                            if (dewaDocuments != null)
                            {
                                foreach (UploadedDocuments item in dewaDocuments)
                                {
                                    if (item != null && item.AttachedDocuments != null)
                                    {
                                        foreach (var doc in item.AttachedDocuments)
                                        {
                                            requestAttachments.Add(new NocRequestAttachments
                                            {
                                                DocDate = !string.IsNullOrWhiteSpace(doc.docdate) && !doc.docdate.Equals("0000-00-00") ?
                                                     DateTime.ParseExact(doc.docdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMMM yyyy", SitecoreX.Culture) : string.Empty,
                                                DocType = doc.doctype,
                                                FileContent = doc.filecontent,
                                                FileId = doc.fileid,
                                                FileName = doc.filename,
                                                FileSize = doc.filesize,
                                                Folder = doc.folder
                                            });
                                        }
                                        //model.nocReqDewaAttachments.AddRange(item.AttachedDocuments.Select(y => new NocRequestAttachments
                                        //{
                                        //    DocType = y.doctype,
                                        //    FileContent = y.filecontent,
                                        //    FileId = y.fileid,
                                        //    FileName = y.filename,
                                        //    FileSize = y.filesize,
                                        //    Folder = y.folder
                                        //}).ToList());
                                    }
                                    model.nocReqDewaAttachments = requestAttachments;
                                    CacheProvider.Store("DewaDocuments", new CacheItem<List<NocRequestAttachments>>(model.nocReqDewaAttachments));
                                }
                            }
                        }

                        var accounDetail = GetCADetailbyAccountNo(model.ContractAccount);
                        if (accounDetail != null)
                        {
                            model.selectedAccounts = new SharedAccount
                            {
                                Name = !string.IsNullOrWhiteSpace(accounDetail.AccountName) ? accounDetail.AccountName : accounDetail.NickName,
                                NickName = !string.IsNullOrWhiteSpace(accounDetail.NickName) ? accounDetail.NickName : string.Empty,
                                AccountNumber = "00" + accounDetail.AccountNumber,
                                Active = accounDetail.IsActive,
                                BusinessPartner = accounDetail.BusinessPartnerNumber,
                                Type = accounDetail.Category,
                                ImageUrl = accounDetail.HasPhoto ? string.Format("/account_thumbs.ashx?id={0}", accounDetail.AccountNumber) : null,
                                Premise = accounDetail.CustomerPremiseNumber,
                                InternalPremise = accounDetail.PremiseNumber,
                                PartialPaymentPermitted = accounDetail.PartialPaymentPermitted,
                                BillingClass = (accounDetail.BillingClass == BillingClassification.Residential) ? Translate.Text("Residential") : (accounDetail.BillingClass == BillingClassification.ElectricVehicle) ? Translate.Text("Electric Vehicle") : (accounDetail.BillingClass == BillingClassification.NonResidential) ? Translate.Text("Non-residential") : "-",
                                ShowAccountSummary = true,
                                BillingClassification = accounDetail.BillingClass,
                                Street = accounDetail.Street,
                                Location = accounDetail.Location,
                            };
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, responseDetails.Message);
                    }
                }
                else
                {
                    CacheProvider.Remove("UploadedDocuments");
                    CacheProvider.Remove("DewaDocuments");
                }

                var response = SmartCustomerClient.GeWorkTypes(new InfrastructureNocRequest
                {
                    businesspartner = CurrentPrincipal.BusinessPartner ?? string.Empty,
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (response != null && response.Succeeded && response.Payload != null && response.Payload.WorkTypeList != null)
                {
                    model.WorkTypeList = response.Payload.WorkTypeList.Select(x => new SelectListItem { Text = x.DescProposedWorkType, Value = x.ProposedWorkType }).ToList();
                    CacheProvider.Store("WorkTypes", new CacheItem<List<SelectListItem>>(model.WorkTypeList));
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return model;
        }

        private DEWAXP.Foundation.Integration.Responses.AccountDetails GetCADetailbyAccountNo(string acNo)
        {
            DEWAXP.Foundation.Integration.Responses.AccountDetails accountDetail = null;

            var d = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());
            if (d != null && d.Succeeded & d.Payload != null)
            {
                accountDetail = d.Payload.FirstOrDefault(x => x.AccountNumber.Contains(acNo));
            }
            return accountDetail;
        }

        /// <summary>
        /// Validate Primse Type
        /// </summary>
        /// <param name="acNO"></param>
        /// <returns></returns>
        private bool IsValidNOCAccount(string acNO)
        {
            var acDetail = GetCADetailbyAccountNo(acNO);
            var d = SmartCustomerClient.InfranocActiveAccount(new InfranocActiveAccountRequest()
            {
                //premisetype = acDetail?.PremiseType,
                premisedescription = acDetail?.PremiseType,
            }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            return Convert.ToBoolean(d.Succeeded && d.Payload.validaccount == "X");
        }

        #endregion Private Methods
    }
}