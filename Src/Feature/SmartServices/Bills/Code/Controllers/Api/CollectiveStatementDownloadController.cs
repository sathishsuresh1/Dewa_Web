using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Helpers.Extensions;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Controllers.Api
{
    public class CollectiveStatementDownloadController : BaseApiController
    {
        [HttpGet, TwoPhaseAuthorize]
        public HttpResponseMessage GetPDF(string accountno)
        {
            byte[] pdf = DewaApiClient.GetCollectiveStatementPDF(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, accountno, string.Empty, string.Empty, RequestLanguage, Request.Segment());
            if (pdf != null && pdf.Length > 0)
            {
                var result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new MemoryStream(pdf));
                result.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                result.Content.Headers.ContentDisposition.FileName = string.Format("{0}.pdf", accountno);

                return result;
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to retrieve collective Statement.");
        }
    }
}