using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Globalization;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace DEWAXP.Feature.Bills.Controllers.Api
{
    public class TransactionStatementDownloadController : BaseApiController
    {
        [HttpGet, TwoPhaseAuthorize]
        public HttpResponseMessage Account(string id, string numberofmonths, string frommonth, string tomonth)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var response = DewaApiClient.GetStatementofAccountsPDF(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id, numberofmonths, frommonth, tomonth, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    if (response.Payload.Length > 0)
                    {
                        CacheProvider.Store(CacheKeys.STATEMENT_DOWNLOAD, new CacheItem<byte[]>(response.Payload, TimeSpan.FromMinutes(20)));
                        //var result = Request.CreateResponse(HttpStatusCode.OK);
                        //result.Content = new StreamContent(new MemoryStream(response.Payload));
                        //result.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                        //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                        //result.Content.Headers.ContentDisposition.FileName = string.Format("{0}.pdf", id);

                        //return result;
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, url = "/api/TransactionStatementDownload/Getdownload/?id=" + id });
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, Message = response.Message });
            }
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, Translate.Text("Unexpected error"));
        }

        [HttpGet, TwoPhaseAuthorize]
        public HttpResponseMessage Getdownload(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                byte[] filedownload;
                if (CacheProvider.TryGet(CacheKeys.STATEMENT_DOWNLOAD, out filedownload))
                {
                    if (filedownload.Length > 0)
                    {
                        var result = Request.CreateResponse(HttpStatusCode.OK);
                        result.Content = new StreamContent(new MemoryStream(filedownload));
                        result.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                        result.Content.Headers.ContentDisposition.FileName = string.Format("{0}.pdf", id);

                        return result;
                        //return Request.CreateResponse(HttpStatusCode.OK, response.Payload);
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, Translate.Text("Unexpected error"));
            }
            HttpContext.Current.Response.Redirect(RedirectUrl(SitecoreItemIdentifiers.J1_VIEW_PAST_BILLS));
            return null;
        }
    }
}