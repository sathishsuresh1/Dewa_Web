using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Models.SupplyManagement.MoveTo;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DEWAXP.Feature.SupplyManagement.Controllers.Api
{
    public class MoveToController : BaseApiController
    {
        public HttpResponseMessage Get(string accountnumber)
        {
            string[] selectedaccount = new string[] { accountnumber };

            var state = new MoveToWorkflowState();
            //var accountFromService = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment()).Payload
            //    .FirstOrDefault(x => x.AccountNumber == accountnumber);
            var accountFromService = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment()).Payload.FirstOrDefault(x => x.AccountNumber == accountnumber);
            var response = DewaApiClient.GetMoveOutDetails(selectedaccount, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, "X", "O", "X", RequestLanguage, Request.Segment());
            var account = SharedAccount.CreateFrom(accountFromService);
            var validator = new MoveToValidator(state);
            foreach (var error in validator.ValidateSelectedAccount(account))
            {
                return Request.CreateResponse(HttpStatusCode.OK, false);
            }

            state.Account = account;
            state.page = string.Empty;
            CacheProvider.Store(CacheKeys.MOVE_TO_WORKFLOW_STATE, new CacheItem<MoveToWorkflowState>(state, TimeSpan.FromMinutes(20)));
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        public HttpResponseMessage Get(string idtype, string businesspartner, string moveoutaccount, string moveoutdate, string idnumber, string customerCategory, string customerType, [FromUri] string[] premiselist)
        {
            //Added for Future Center
            var _fc = FetchFutureCenterValues();

            string ejarinumber = string.Empty;
            string ejaritype = string.Empty;
            //DateTime moveoutDate = !string.IsNullOrWhiteSpace(moveoutdate) ?  : null;
            string moveoutaccountnumber = !string.IsNullOrWhiteSpace(moveoutaccount) ? moveoutaccount : string.Empty;
            if (string.Equals(idtype, "EJ"))
            {
                idtype = string.Empty;
                ejarinumber = idnumber;
                idnumber = string.Empty;
            }
            var response = DewaApiClient.MoveInEjariRequest(
            CurrentPrincipal.UserId ?? string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            CurrentPrincipal.SessionToken ?? string.Empty,
            customerType,
            idtype,
            idnumber,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            null,
            Convert.ToDateTime(moveoutdate),
            moveoutaccountnumber,
            null,
            null,
            businesspartner,
            null,
            string.Empty,
            customerCategory,
            null,
            string.Empty,
            IsAuthenticated ? "R" : "A",
            ejarinumber,
            premiselist,
            string.Empty,
            string.Empty,
            RequestLanguage,
            Request.Segment(),
            _fc.Branch

            );

            return Request.CreateResponse(HttpStatusCode.OK, response.Payload);
        }
    }
}