using DEWAXP.Foundation.Content.Controllers.Api;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;

namespace DEWAXP.Feature.Builder.Controllers.Api
{
    public class RfxDownloadController : BaseApiController
    {
        // GET: RfxDownload
        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            var rfxContentResponse = VendorApiClient.GetExportRfx(id);

            var result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(new MemoryStream(rfxContentResponse.Payload));
            result.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = string.Format("{0}.pdf", id);

            return result;
        }
    }
}