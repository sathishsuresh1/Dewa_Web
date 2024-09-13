using DEWAXP.Feature.Bills.Models.PremiseTypeChange;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Requests;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Controllers
{
    [TwoPhaseAuthorize]
    public class ChangePremiseTypeController : BaseController
    {
        public ActionResult NewRequest()
        {
            CacheProvider.Remove(CacheKeys.SET_PREMISE_TYPE_SENT);
            CacheProvider.Remove(CacheKeys.SET_PREMISE_TYPE_FAILED);

            var model = ContextRepository.GetCurrentItem<ChangePremiseType>();

            return PartialView("~/Views/Feature/Bills/ChangePremiseType/_NewRequest.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewRequest(ChangePremiseType model, HttpPostedFileBase file)
        {
            if (file != null)
            {
                string error;
                if (!AttachmentIsValid(file, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            if (ModelState.IsValid)
            {
                //var accounts = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken,false,RequestLanguage, Request.Segment()).Payload.FirstOrDefault(x => x.AccountNumber == model.ContractAccountNumber);
                var accounts = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment()).Payload.FirstOrDefault(x => x.AccountNumber == model.ContractAccountNumber);

                var account = SharedAccount.CreateFrom(accounts);

                if (account != null)
                {
                    model.PremiseNumber = account.InternalPremise;
                }

                var request = new PremiseTypeChangeRequest()
                {
                    ContractAccountNumber = model.ContractAccountNumber,
                    MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                    PremiseNumber = model.PremiseNumber,
                    Remarks = model.Remarks,
                    Attachment = file != null ? file.ToArray() : new byte[0],
                    AttachmentType = file != null ? file.GetTrimmedFileExtension() : string.Empty
                };

                var response = DewaApiClient.ChangePremiseType(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, request, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    var successModel = new PremiseTypeChangeSucceeded
                    {
                        Account = account,
                        MobileNumber = request.MobileNumber,
                        Documentation = request.Attachment,
                        FurtherComments = request.Remarks,
                        Reference = response.Payload.NotificationNumber,
                        AttachmentType = request.AttachmentType,
                        FileName = file != null ? file.FileName.GetFileNameWithoutPath() : string.Empty
                    };

                    CacheProvider.Store(CacheKeys.SET_PREMISE_TYPE_SENT, new CacheItem<PremiseTypeChangeSucceeded>(successModel));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J89_PREMISE_CHANGE_SUCCESS);
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            var content = ContextRepository.GetCurrentItem<GenericPageWithIntro>();
            model.Intro = content.Intro;

            return PartialView("~/Views/Feature/Bills/ChangePremiseType/_NewRequest.cshtml", model);
        }

        public ActionResult RequestSent()
        {
            PremiseTypeChangeSucceeded model;
            if (!CacheProvider.TryGet(CacheKeys.SET_PREMISE_TYPE_SENT, out model))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J89_PREMISE_CHANGE_REQUEST);
            }
            return PartialView("~/Views/Feature/Bills/ChangePremiseType/_RequestSent.cshtml", model);
        }

        public ActionResult RequestFailed()
        {
            string message;
            if (!CacheProvider.TryGet(CacheKeys.SET_PREMISE_TYPE_FAILED, out message))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J89_PREMISE_CHANGE_REQUEST);
            }
            ViewBag.Message = message;

            CacheProvider.Remove(CacheKeys.SET_PREMISE_TYPE_SENT);
            CacheProvider.Remove(CacheKeys.SET_PREMISE_TYPE_FAILED);

            return PartialView("~/Views/Feature/Bills/ChangePremiseType/_RequestFailed.cshtml");
        }
    }
}