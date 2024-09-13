using DEWAXP.Feature.Bills.ClearanceCertificate;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Logger;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;

namespace DEWAXP.Feature.Bills.Controllers.Api
{
    public class ClearanceCertificateV2Controller : BaseApiController
    {
        /// <summary>
        /// Api to download clearance certificate and reset PIN.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="urlParameter"></param>
        /// <param name="isDownload"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Get(string id = "", string urlParameter = "", string isDownload = "")
        {
            string pinFlag = "X";

            if (string.IsNullOrEmpty(isDownload))
            {
                var _response = DewaApiClient.GetVerifyClearanceCertificate(id, string.Empty, string.Empty, RequestLanguage, Request.Segment(), pinFlag);

                if (_response.Payload.@return.responseCode.Equals("000"))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { description = _response.Payload.@return.description, message = _response.Message });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { description = _response.Payload.@return.description, message = _response.Message });
            }
            else
            {
                var prefix = "CC";
                var _response = DewaApiClient.GetVerifyClearanceCertificate(string.Empty, string.Empty, urlParameter, RequestLanguage, Request.Segment(), string.Empty);
                var formattedIdentifier = !urlParameter.StartsWith(prefix) ? string.Concat(prefix, urlParameter) : urlParameter;
                var result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new MemoryStream(_response.Payload.@return.pdfData));
                result.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                result.Content.Headers.ContentDisposition.FileName = string.Format("{0}.pdf", formattedIdentifier);
                if (result.Content.Headers.ContentLength == 0)
                {
                    HttpContext.Current.Response.Redirect(RedirectUrl(SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_REFERENCE_NO_ERROR) + "?msg=" + _response.Message);
                    return null;

                    //return Request.CreateResponse(HttpStatusCode.OK, new { description = _response.Payload.@return.description, message = _response.Message });
                }
                return result;
            }
        }

        [HttpGet, TwoPhaseAuthorize]
        public HttpResponseMessage GetAcccount(string contractAccount, string param)
        {
            var cert = DewaApiClient.GetContractAccountClearanceDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, contractAccount, RequestLanguage, Request.Segment(), param);

            if (cert.Succeeded)
            {
                //var accountDetailsResponse = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());

                if (cert.Succeeded)
                {
                    //var accountDetails = accountDetailsResponse.Payload.First(x => x.AccountNumber == contractAccount);

                    var model = new AuthenticatedClearanceCertificateModel
                    {
                        FirstName = cert.Payload.@return.firstName ?? string.Empty,
                        LastName = cert.Payload.@return.lastName ?? string.Empty,
                        EmailAddress = cert.Payload.@return.email,
                        MobileNumber = cert.Payload.@return.mobile.RemoveMobileNumberZeroPrefix(),
                        ReceivedContractAccountNumber = cert.Payload.@return.contractAccountList != null && cert.Payload.@return.contractAccountList.Length > 0 ? cert.Payload.@return.contractAccountList.Select(x => x.contractAccount1).Aggregate((i, j) => i + "," + j) : contractAccount,
                        ContractAccountNumber = contractAccount,
                        MoveOutDate = cert.Payload.@return.moveOutDate,
                        Amount = cert.Payload.@return.amount,
                        Amounts = cert.Payload.@return.contractAccountList != null && cert.Payload.@return.contractAccountList.Length > 0 ? cert.Payload.@return.contractAccountList.Select(x => x.totalAmount).Aggregate((i, j) => i + "," + j) : cert.Payload.@return.amount.ToString()
                    };

                    DateTime tmpDate = DateTime.ParseExact(model.MoveOutDate, "dd.MM.yyyy", null);

                    model.MoveOutDate = tmpDate.ToString("dd MMM yyyy");

                    return Request.CreateResponse(HttpStatusCode.OK, model);
                }
                //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, accountDetailsResponse.Message);
            }
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, cert.Message);
        }

        [HttpPost, System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage GetIdentityDetails([FromBody] ClearanceCertificateId clid)
        {
            try
            {
                AntiForgery.Validate();
                string identityType = clid.identityType;
                string identityNumber = clid.identityNumber;
                string purpose = clid.purpose;
                string courtReference = clid.courtReference;
                var contractAccountClearanceDetails = new GetContractAccountClearanceDetails
                {
                    contractAccount = clid.ca,
                    emiratesId = identityType.Equals("ED") ? identityNumber : string.Empty,
                    passportNumber = identityType.Equals("PP") ? identityNumber : string.Empty,
                    tradeLicenseNumber = identityType.Equals("TN") || identityType.Equals("TO") ? identityNumber : string.Empty,
                    purpose = purpose,
                };

                var resp = DewaApiClient.GetContractAccountIdentityDetails(contractAccountClearanceDetails, "", "", "", RequestLanguage, Request.Segment());

                if (resp != null && resp.Payload != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, resp.Payload);
                }

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (System.Web.Mvc.HttpAntiForgeryException ex)
            {
                LogService.Fatal(ex, this);
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }

    public class ClearanceCertificateId
    {
        public string identityType { get; set; }
        public string identityNumber { get; set; }
        public string purpose { get; set; }
        public string courtReference { get; set; }
        public string ca { get; set; }
    }
}