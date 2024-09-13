using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Models.RammasLogin;
using DEWAXP.Foundation.Content.Repositories;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static System.Net.WebRequestMethods;

namespace DEWAXP.Feature.ChatBot.Controllers.Api
{
    public class RammasApiController : BaseApiController
    {
        [AcceptVerbs(Http.Get)]
        public async Task<HttpResponseMessage> RammasFetchBotState(string ConversationId)
        {
            await Task.FromResult(0);
            RammasParameter ramasparameter = new RammasParameter();
            RammasGateWayResponse rammasgatewayresponse = new RammasGateWayResponse();

            try
            {
                var data = await ramasparameter.FetchBotState(ConversationId);
                if (data != null)
                {
                    //rammasgatewayresponse.Message = Translate.Text("Success");
                    rammasgatewayresponse.ConversationId = ConversationId;

                    CacheProvider.Store(CacheKeys.RAMMAS_LOGIN, new CacheItem<RammasGateWayResponse>(rammasgatewayresponse));

                    #region [MIM Payment Implementation]

                    var payRequest = new CipherPaymentModel();
                    payRequest.PaymentData.amounts = Convert.ToString(data.Amount);
                    payRequest.PaymentData.contractaccounts = data.ContractAccounts;
                    payRequest.PaymentData.transactiontype = data.TransactionType;
                    payRequest.PaymentData.userid = data.UserId;
                    payRequest.ServiceType = ServiceType.PayBill;
                    payRequest.IsThirdPartytransaction = data.ThirdPartyPayment;
                    payRequest.SuqiaAmt = data.Suqiaamt;
                    payRequest.SuqiaValue = data.SuqiaValue;
                    var payResponse = ExecutePaymentGateway(payRequest);
                    return Request.CreateResponse(HttpStatusCode.OK, new { mim = true, data = payResponse });
                    #endregion [MIM Payment Implementation]

                }

                return Request.CreateErrorResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}