using DEWAXP.Feature.Bills.Models.ConnectionEnquiries;
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
    public class ConnectionEnquiryController : BaseController
    {
        #region Actions

        [HttpGet]
        public ActionResult NewRequest()
        {
            var model = PopulateQueryTypes();
            return PartialView("~/Views/Feature/Bills/ConnectionEnquiry/_NewRequest.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewRequest(ConnectionEnquiry model, HttpPostedFileBase file)
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
                //var accounts = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken).Payload.FirstOrDefault(x => x.AccountNumber == model.ContractAccountNo);
                var accounts = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment()).Payload.FirstOrDefault(x => x.AccountNumber == model.ContractAccountNo);

                var account = SharedAccount.CreateFrom(accounts);

                if (account != null)
                {
                    model.PremiseNumber = account.InternalPremise.AssertAccountNumberPrefix();
                    model.BusinessPartnerNo = account.BusinessPartner;
                    model.ContractAccountNo = account.AccountNumber;
                }

                var request = new RequestConnectionEnquiry()
                {
                    BusinessPartnerNo = model.BusinessPartnerNo,
                    ContractAccountNo = model.ContractAccountNo,
                    MobileNo = model.MobileNumber,
                    PremiseNo = model.PremiseNumber,
                    QueryType = model.SelectedQueryType,
                    Remarks = model.Details,
                    Attachment = file != null ? file.ToArray() : new byte[0],
                    AttachmentType = file != null ? file.GetTrimmedFileExtension() : string.Empty,
                    UserId = CurrentPrincipal.UserId,
                    SessionNo = CurrentPrincipal.SessionToken
                };

                var response = DewaApiClient.SubmitConnectionEnquiry(request, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    var successModel = new ConnectionEnquirySucceeded
                    {
                        Account = account,
                        MobileNumber = request.MobileNo,
                        Documentation = request.Attachment,
                        FurtherComments = request.Remarks,
                        Reference = response.Payload.Reference,
                        AttachmentType = request.AttachmentType,
                        FileName = file != null ? file.FileName.GetFileNameWithoutPath() : string.Empty
                    };
                    CacheProvider.Store(CacheKeys.SET_CONNECTION_ENQUIRY_SET, new CacheItem<ConnectionEnquirySucceeded>(successModel));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J100_CONNECTION_ENQUIRY_REQUEST_SENT);
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            var content = ContextRepository.GetCurrentItem<GenericPageWithIntro>();
            model.Intro = content.Intro;
            model = PopulateQueryTypes();
            return PartialView("~/Views/Feature/Bills/ConnectionEnquiry/_NewRequest.cshtml", model);
        }

        public ActionResult RequestSent()
        {
            ConnectionEnquirySucceeded model;
            if (!CacheProvider.TryGet(CacheKeys.SET_CONNECTION_ENQUIRY_SET, out model))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J100_CONNECTION_ENQUIRY);
            }
            return PartialView("~/Views/Feature/Bills/ConnectionEnquiry/_RequestSent.cshtml", model);
        }

        #endregion Actions

        #region Helpers

        private ListDataSources GetQueryTypes()
        {
            return this.ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.QUERY_TYPES));
        }

        private ConnectionEnquiry PopulateQueryTypes()
        {
            var queryTypes = GetQueryTypes().Items;
            var convertedItems = queryTypes.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
            var model = ContextRepository.GetCurrentItem<ConnectionEnquiry>();
            model.QueryTypes = convertedItems;
            return model;
        }

        #endregion Helpers
    }
}