using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Responses.VendorSvc;
using System.Configuration;
using System.Web.Mvc;
using Sitecorex = Sitecore.Context;

namespace DEWAXP.Feature.Builder.Controllers
{
    public class TenderDocumentListController : BaseController
    {
        //public static string GetFormatedDate(string date)
        //{
        //    var _formatteddate = date.ToString().FormatDate("dd.MM.yyyy");
        //    return _formatteddate.Value.ToString("dd-MMM-yyyy", Sitecorex.Culture);
        //}

        // GET: TenderDocumentList
        public PartialViewResult GetTenderList()
        {
            //var tenderList = TenderServiceClient.GetOpenTenderList(RequestLanguage, Request.Segment());
            var tenderList = VendorServiceClient.GetOpenTenderList(RequestLanguage, Request.Segment());

            ViewBag.Repo = ConfigurationManager.AppSettings["Tender_Doc_Repo"];
            ViewBag.PayUrl = ConfigurationManager.AppSettings["Tender_Payment_Url"];

            return PartialView("~/Views/Feature/Builder/Tenders/_TenderDocumentList.cshtml", tenderList.Payload);
        }

        public PartialViewResult GetTenderOpeningList()
        {
            GetTenderResultListDataResponse model = new GetTenderResultListDataResponse();
            //var tenderList = TenderServiceClient.GetTenderOpeningList(RequestLanguage, Request.Segment());
            var tenderList = VendorServiceClient.GetTenderResultList(RequestLanguage, Request.Segment());
            model = tenderList.Payload;

            return PartialView("~/Views/Feature/Builder/Tenders/_TenderDocumentResultList.cshtml", model);
        }

        [HttpGet]
        public PartialViewResult GetTenderResult(string fid, string tno, string fdate, string cdate, string tdesc, string ttype)
        {
            GetTenderResultDisplayDataResponse model = new GetTenderResultDisplayDataResponse();

            // var tender = TenderServiceClient.GetTenderOpeningResult(fid, RequestLanguage, Request.Segment());
            var tender = VendorServiceClient.GetTenderResultDisplay(tno, RequestLanguage, Request.Segment());

            //var Fdate = tender.Payload.FloatingDate.ToString().FormatDate("yyyy-MM-dd");
            //var Cdate = tender.Payload.ClosingDate.ToString().FormatDate("yyyy-MM-dd");

            var Fdate = tender.Payload.FloatingDate.ToString().FormatDate("dd.MM.yyyy");
            var Cdate = tender.Payload.ClosingDate.ToString().FormatDate("dd.MM.yyyy");

            ViewBag.Tno = tno;
            ViewBag.FDate = Fdate.HasValue ? Fdate.Value.ToString("dd-MMM-yyyy", Sitecorex.Culture) : string.Empty;
            ViewBag.CDate = Cdate.HasValue ? Cdate.Value.ToString("dd-MMM-yyyy", Sitecorex.Culture) : string.Empty;
            ViewBag.TDesc = Server.HtmlDecode(tender.Payload.TenderDescription);
            ViewBag.TType = tender.Payload.TenderType;

            if (tender.Payload == null)
            {
                ViewBag.HasResult = false;
                //return PartialView("~/Views/Feature/Builder/Tenders/_TenderResult.cshtml", new TenderItemResultResponse());
                return PartialView("~/Views/Feature/Builder/Tenders/_TenderResult.cshtml", new GetTenderResultDisplayDataResponse());
            }
            ViewBag.HasResult = true;
            return PartialView("~/Views/Feature/Builder/Tenders/_TenderResult.cshtml", tender.Payload);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public FileResult ViewAttachment(string tenderNumber)
        {
            if (!string.IsNullOrEmpty(tenderNumber))
            {
                var tenderFileResponse = VendorServiceClient.GetTenderAdvertisment(tenderNumber, RequestLanguage, Request.Segment());

                if (tenderFileResponse != null && tenderFileResponse.Payload != null && tenderFileResponse.Payload.filebytes != null)
                {
                    return File(tenderFileResponse.Payload.filebytes, "application/octet-stream",
                                tenderFileResponse.Payload.FileName);
                }
            }
            return null;
        }
    }
}