using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Integration;
using DEWAXP.Foundation.Logger;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;

namespace DEWAXP.Feature.IdealHome.Controllers.Api
{
    public class IdealHomeConsumerAPIController : BaseApiController
    {
        public class MyModel
        {
            public string UserName { get; set; }
            public string to { get; set; }

            public string cc { get; set; }
            public string bcc { get; set; }
            public string subject { get; set; }
            public string body { get; set; }

            public string ImgPath { get; set; }
            public string base64string { get; set; }
        }

        [System.Web.Http.AcceptVerbs(Http.Post)]
        public HttpResponseMessage Exportemail(MyModel model)
        {
            IdealHomeConsumerController _idealHomeController = new IdealHomeConsumerController();

            var emailserviceclient = DependencyResolver.Current.GetService<IEmailServiceClient>();
            var filename = model.UserName;

            byte[] filebytearray = _idealHomeController.ExportPdf(model.ImgPath, model.UserName);//Convert.FromBase64String(model.base64string);

            List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();
            attList.Add(new Tuple<string, byte[]>(filename, filebytearray));

            var response = emailserviceclient.SendEmail("no-reply@dewa.gov.ae", model.to, model.cc, model.bcc, model.subject, model.body, attList);

            //return Request.CreateResponse(HttpStatusCode.OK,response);

            return Request.CreateResponse(HttpStatusCode.OK, new { message = response.Message });
        }

        //private byte[] GetPdf(MyModel model)
        //{
        //    var sitecoreService = new SitecoreContext();
        //    var _htmlConfig = sitecoreService.GetItem<HtmlTemplateConfigurations>(SitecoreItemIdentifiers.IDEALHOMECONSUMER_HTML_TEMPLATE_CONFIG);

        //    HtmlToPdf converter = new HtmlToPdf();

        //    _htmlConfig.HtmlText = _htmlConfig.HtmlText.Replace("$Name", model.UserName);
        //    _htmlConfig.HtmlText = _htmlConfig.HtmlText.Replace("urltoken",model.urltoken);
        //    // create a new pdf document converting an url
        //    PdfDocument doc = converter.ConvertHtmlString(_htmlConfig.HtmlText);

        //    // save pdf document
        //    byte[] pdf = doc.Save();

        //    // close pdf document
        //    doc.Close();

        //    return pdf;
        //}
        [System.Web.Http.HttpPost]
        public HttpResponseMessage VideoPost([FromBody] VideoApiRequest _request)
        {
            try
            {
                AntiForgery.Validate();
                IdealHomeConsumerController _idealHomeController = new IdealHomeConsumerController();
                //VideoApiRequest _request = new VideoApiRequest();
                _request.Watched = false;

                _request.Watched = _idealHomeController.InsertVideoResponse(_request.videoItemID);
                return Request.CreateResponse(HttpStatusCode.OK, _request);
                //return Request.CreateErrorResponse(HttpStatusCode.NotFound,_request.Watched.ToString());
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
        public class VideoApiRequest
        {
            public string videoItemID { get; set; }
            public bool Watched { get; set; }
        }
    }
}