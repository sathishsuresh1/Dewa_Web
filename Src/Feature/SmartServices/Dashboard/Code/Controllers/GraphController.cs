using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Feature.Dashboard.Models.Graph;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using System;
using System.Linq;
using System.Web.Mvc;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Premise;

namespace DEWAXP.Feature.Dashboard.Controllers
{
    public class GraphController : BaseController
    {
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult MyConsumptionDashboard()
        {
            var model = new GraphRenderingModel();
            //IsSmartMetering(CurrentPrincipal.PrimaryAccount, model);
            string Message= string.Empty;
            var outage = AMIGraphglobalConfiguration.ServiceOutage(out Message);
            ViewBag.OutageMessage = Message;
            ViewBag.Outage = outage;
            return PartialView("~/Views/Feature/Dashboard/Graph/GraphRendering.cshtml", model);
        }

        //[Authorize]
        //public PartialViewResult GraphRendering()
        //{
        //    var model = new GraphRenderingModel();
        //    IsSmartMetering(CurrentPrincipal.PrimaryAccount, model);
        //    return PartialView("GraphRendering", model);
        //}

        // GET: Graph
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult GetReadings(string accountnumber, string date, string month, string year, string usagetype, string rtype)
        {
            if (string.IsNullOrEmpty(accountnumber) || string.IsNullOrEmpty(rtype) || (!rtype.Contains('E') && !rtype.Contains('W'))) { return Json("User must be logged in!"); }

            try
            {
                string day = "1";
                accountnumber += "_" + rtype;

                ServiceResponse<DEWAXP.Foundation.Integration.Responses.GraphSvc.RootObject> response = null;

                switch (usagetype.ToUpper())
                {
                    case "D":
                        DateTime myDate = DateTime.ParseExact(date, "d MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        day = myDate.Day.ToString();
                        response = IGraphServiceClient.GetGraph(accountnumber, day, myDate.Month.ToString(), myDate.Year.ToString(), usagetype + rtype.ToUpper(), CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                        break;

                    case "M":

                        if (!ValidateMonth(month))
                        {
                            goto InvalidData;
                        }
                        response = IGraphServiceClient.GetGraph(accountnumber, day, month, year, usagetype + rtype.ToUpper(), CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                        break;

                    case "Y":
                        if (!ValidateYear(year))
                        {
                            goto InvalidData;
                        }
                        response = IGraphServiceClient.GetGraph(accountnumber, day, month, year, usagetype + rtype.ToUpper(), CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                        break;

                    default:
                        goto InvalidData;
                }
                //DateTime myDate = DateTime.ParseExact(date, "d MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //var serviceClient = IGraphServiceClient.GetGraph(accountnumber + "_E", day, month, year, usagetype);
                if (response != null && response.Succeeded)
                {
                    var d = response.Payload?.ReplyMessage?.Payload?.MeterReading?.IntervalBlock?.IReading?.Select(x => (x.Quality == null) ? x.value : ((x.Quality != null && !x.Quality.noData) ? x.value : null)).ToList();

                    if (d == null || d.Count == 0) { return Json("No Data Found"); }
                    /*switch (usagetype.ToUpper())
                    {
                        case "M":
                            switch (d.Count)
                            {
                                case 31:
                                    break;

                                case 30:
                                    d.Add(0.0);
                                    break;

                                case 29:
                                    d.Add(0.0);
                                    d.Add(0.0);
                                    break;

                                case 28:
                                    d.Add(0.0);
                                    d.Add(0.0);
                                    d.Add(0.0);
                                    break;
                            }
                            break;

                        default:
                            break;
                    }*/

                    return Json(new { data = d });
                }
                else
                { return Json(response.Message); }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                return Json(ErrorMessages.UNEXPECTED_ERROR);
            }

        InvalidData:
            LogService.Warn(new System.Exception("Invalid data submitted to get Electricity, Water related graphs", null), this);
            return Json("Invalid data submitted.");
        }

        //[HttpPost,ValidateAntiForgeryToken]
        //public JsonResult GetSmartMeterDetails(string accountnumber)
        //{
        //    var TaskResponse = FnGetSmartMeterDetails(accountnumber);
        //    return TaskResponse.Result;
        //}
        //private async Task<JsonResult> FnGetSmartMeterDetails(string accountnumber)
        //{
        //     string ErrorMessage = "";
        //    return await Task.FromResult(((Func<JsonResult>)(() =>
        //    {
        //        try
        //        {
        //            var model = new GraphRenderingModel();
        //            IsSmartMetering(accountnumber, model);
        //            return Json(model, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (System.Exception ex)
        //        {
        //            ErrorMessage = ex.Message;
        //        }
        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //        GC.Collect();
        //        return Json(ErrorMessage, JsonRequestBehavior.AllowGet);
        //    }))());
        //}
        private bool ValidateMonth(string intVal)
        {
            int y = 0;
            int.TryParse(intVal, out y);

            return y > 0 && y < 32;
        }

        private bool ValidateYear(string intVal)
        {
            int y = 0;
            int.TryParse(intVal, out y);

            return y > 2010 && y <= DateTime.Now.Year;
        }

        private bool IsSmartMetering(string contractAccNo, GraphRenderingModel model)
        {
            model.IsSmartElectricityMeter = false;
            model.IsSmartWaterMeter = false;

            try
            {
                var _issueRepsonse = PremiseHandler.GetDetails(new PremiseDetailsRequest()
                {
                    PremiseDetailsIN = new PremiseDetailsIN()
                    {
                        contractaccount = contractAccNo,
                        dminfo = false,
                        meterstatusinfo = true,
                        outageinfo = false,
                        podcustomer = false,
                        seniorcitizen = false,
                        userid = CurrentPrincipal.Username,
                        sessionid = CurrentPrincipal.SessionToken
                    }
                }, RequestLanguage, Request.Segment());

                if (_issueRepsonse.Succeeded && _issueRepsonse.Payload != null)
                {
                    var _responseData = _issueRepsonse.Payload;

                    model.IsSmartElectricityMeter = _responseData?.meter?.electricitySmartMeter == true ? (_responseData?.meter?.electricitymeterType?.Equals("03") == false ? true : false) : false;
                    model.IsSmartWaterMeter = _responseData?.meter?.waterSmartMeter == true ? (_responseData?.meter?.watermeterType?.Equals("03") == false ? true : false) : false;
                    model.MoveInDate = _responseData?.meter?.moveinDate;
                    model.MoveOutDate = _responseData?.meter?.moveoutDate;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return false;
        }

        //#region Download Graph
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public ActionResult PostDownload(string graphhtml, string __RequestVerificationToken)
        //{
        //    var res = new JsonResult() { Data = null };
        //    try
        //    {
        //        //SitecoreX.Data.Items.Item rammasItem = SitecoreX.Context.Database.GetItem(SitecoreX.Data.ID.Parse("{26DCE97E-5B85-4145-BB03-7997F2430F9A}"));

        //        //var from = rammasItem?.Fields["From Email"]?.Value ?? "no-reply@dewa.gov.ae";
        //        //var fileName = (rammasItem?.Fields["Attachment Name"]?.Value ?? "RammasChat-") + DateTime.Now.ToFileTime() + ".pdf";
        //        //string body = rammasItem?.Fields["Export Email Body"]?.Value ?? "[Content Item Missing]";

        //        //if (SitecoreX.Context.Language.CultureInfo.TextInfo.IsRightToLeft) { body = string.Format("<div style=\"direction:rtl\">{0}</div>", body); }

        //        IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();

        //        string handle = Guid.NewGuid().ToString();

        //        TempData[handle] = Renderer.RenderHtmlAsPdf(System.Web.HttpUtility.UrlDecode(graphhtml)).Stream.ToArray();

        //        res.Data = new { FileGuid = handle, FileName = "dewa_cons_graph_" + DateTime.Now.Ticks.ToString() + ".pdf" };

        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogService.Error(ex, this);
        //        res.Data = new { Error = ErrorMessages.UNEXPECTED_ERROR };
        //    }

        //    return res;
        //}

        //[HttpGet]
        //public virtual ActionResult Download(string gid, string fileName)
        //{
        //    if (TempData[gid] != null)
        //    {
        //        byte[] data = TempData[gid] as byte[];
        //        return File(data, "application/pdf", fileName);
        //    }
        //    else
        //    {
        //        // Problem - Log the error, generate a blank file,
        //        //           redirect to another controller action - whatever fits with your application
        //        return new EmptyResult();
        //    }
        //}

        //#endregion

        #region Smart Alert

        [HttpGet]
        public ActionResult SmartAlert(string a)
        {
            var model = new GraphRenderingModel();
            string accountNumber = !string.IsNullOrWhiteSpace(a) ? a : CurrentPrincipal.PrimaryAccount;
            if (IsSmartMetering(accountNumber, model))
            {
                return View("~/Views/Feature/Dashboard/Dashboard/SmartAlert.cshtml");
            }
            if (!string.IsNullOrWhiteSpace(a))
            {
                QueryString qs = new QueryString();
                qs.With("a", qs, true);
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD, qs);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SmartAlertSetSubscribe(string accountNumber,
            string division, string subscriptiontype, string unsubscribecompletly,
            bool janflag = false, bool febflag = false, bool marflag = false, bool aprflag = false, bool mayflag = false, bool junflag = false, bool julflag = false, bool augflag = false, bool sptflag = false,
            bool octflag = false, bool novflag = false, bool decflag = false, string quantity = "", string unit = "")
        {
            var consumptionResponse = DewaApiClient.SetSubscribeSmartAlert(
                        new SetSubscribeSmartAlert
                        {
                            SetSubscribeSmartAlert1 = new smartAlertSubwIn
                            {
                                contractAccount = accountNumber,
                                credential = CurrentPrincipal.SessionToken,
                                alerts = new smartAlert[]
                              {
                                  new smartAlert
                                  {
                                      division = division,
                                      subscriptionType = subscriptiontype,
                                      unsubscribeCompletly = unsubscribecompletly,
                                      apr = aprflag,
                                      aug = augflag,
                                      dec = decflag,
                                      feb = febflag,
                                      jan = janflag,
                                      jul = julflag,
                                      jun = junflag,
                                      mar = marflag,
                                      may = mayflag,
                                      nov =novflag,
                                      oct = octflag,
                                      sep= sptflag,
                                      quantity =quantity,
                                      unit = unit
                                  }
                              }
                            }
                        }, RequestLanguage, Request.Segment());
            if (consumptionResponse != null && consumptionResponse.Succeeded)
            {
                return Json(new { status = true });
            }
            return Json(new { status = false, Message = consumptionResponse.Message });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SlabWisePercentage(string accountNumber, string consumption, string division, string unit)
        {
            var slabResponse = DewaApiClient.GetSlabWisePercentage(
                        new GetSlabWisePercentage
                        {
                            slabPercentIn = new slabPercentIn
                            {
                                consumption = consumption,
                                contractAccount = accountNumber,
                                credential = CurrentPrincipal.SessionToken,
                                division = division,
                                unit = unit
                            }
                        },
                        RequestLanguage, Request.Segment());
            if (slabResponse != null && slabResponse.Succeeded)
            {
                return Json(new { status = true, result = slabResponse.Payload });
            }
            return Json(new { status = false, Message = slabResponse.Message });
        }

        #endregion Smart Alert
    }
}