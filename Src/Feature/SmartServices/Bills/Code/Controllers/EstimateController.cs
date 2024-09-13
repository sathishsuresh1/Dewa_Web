using DEWAXP.Feature.Bills.Models.Estimates;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Utils;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;
using _commonUtility = DEWAXP.Foundation.Content.Utils.CommonUtility;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.Bills.Controllers
{
    [TwoPhaseAuthorize]
    public class EstimateController : BaseController
    {
        #region Friend's estimates

        [HttpGet]
        public ActionResult FindFriendsEstimate()
        {
            CacheProvider.Remove(CacheKeys.FRIENDS_ESTIMATE_STATE);
            CacheProvider.Remove(CacheKeys.FRIENDS_ESTIMATE_PAYMENT_STATE);

            string paymentError;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out paymentError))
            {
                ModelState.AddModelError(string.Empty, paymentError);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }

            return View("~/Views/Feature/Bills/Estimates/_FindFriendsEstimate.cshtml", new FindEstimateModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FindFriendsEstimate(FindEstimateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var estimate = EstimateRestClient.EstimateDetails(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.EstimateDetailsRequest()
                    {
                        userid = CurrentPrincipal.UserId,
                        sessionid = CurrentPrincipal.SessionToken,
                        estimatenumber = model.EstimateNumber
                    }, Request.Segment(), RequestLanguage);
                    if (estimate.Succeeded)
                    {
                        CacheProvider.Store(CacheKeys.FRIENDS_ESTIMATE_STATE, new CacheItem<DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateDetailitemResponse>(estimate.Payload.estimatedetailitem.FirstOrDefault()));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J82_VIEW_ESTIMATE);
                    }

                    ModelState.AddModelError(string.Empty, estimate.Message);

                    //model.History = GetEstimatePaymentHistory(true);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
                return View("~/Views/Feature/Bills/Estimates/_FindFriendsEstimate.cshtml", model);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J82_ESTIMATE_LANDING);
        }

        [HttpGet]
        public ActionResult ViewPayFriendsEstimate()
        {
            DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateDetailitemResponse estimate;
            if (!CacheProvider.TryGet(CacheKeys.FRIENDS_ESTIMATE_STATE, out estimate))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J82_ESTIMATE_LANDING);
            }

            return PartialView("~/Views/Feature/Bills/Estimates/_ViewPayFriendsEstimate.cshtml", new ViewFriendsEstimateModel
            {
                Estimate = estimate,
                LoginLink = GetSitecoreUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE)
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ViewPayFriendsEstimate(ViewFriendsEstimateModel m, string estimatedamount, PaymentMethod paymentMethod)
        {
            DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateDetailitemResponse estimate;
            if (!CacheProvider.TryGet(CacheKeys.FRIENDS_ESTIMATE_STATE, out estimate))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J82_ESTIMATE_LANDING);
            }
            estimate.PartialPayment = decimal.Parse(estimatedamount);
            CacheProvider.Store(CacheKeys.FRIENDS_ESTIMATE_STATE, new CacheItem<DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateDetailitemResponse>(estimate));

            #region [MIM Payment Implementation]

            var payRequest = new CipherPaymentModel();
            payRequest.PaymentData.amounts = Convert.ToString(estimatedamount);
            payRequest.PaymentData.contractaccounts = estimate?._ca_number;
            payRequest.PaymentData.estimatenumber = estimate?.estimateno;
            payRequest.PaymentData.businesspartner = CurrentPrincipal.BusinessPartner;
            payRequest.PaymentData.ownerbusinesspartnernumber = estimate?._sold_to_party;
            payRequest.PaymentData.consultantbusinesspartnernumber = estimate?._consultantno;
            payRequest.PaymentData.email = estimate?.con_email;
            payRequest.ServiceType = ServiceType.EstimatePayment;
            payRequest.PaymentMethod = paymentMethod;
            payRequest.IsThirdPartytransaction = true;
            payRequest.BankKey = m.bankkey;
            payRequest.SuqiaValue = m.SuqiaDonation;
            payRequest.SuqiaAmt = m.SuqiaDonationAmt;
            var payResponse = ExecutePaymentGateway(payRequest);
            if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
            {
                return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
            }
            ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

            #endregion [MIM Payment Implementation]

            return PartialView("~/Views/Feature/Bills/Estimates/_ViewPayFriendsEstimate.cshtml", new ViewFriendsEstimateModel
            {
                Estimate = estimate,
                LoginLink = GetSitecoreUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE)
            });
        }

        [HttpGet]
        public ActionResult FriendsEstimateConfirmation()
        {
            DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateDetailitemResponse state;
            DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateDetailitemResponse UpdatedEstimate = new DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateDetailitemResponse();

            if (!CacheProvider.TryGet(CacheKeys.FRIENDS_ESTIMATE_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J82_ESTIMATE_LANDING);
            }

            PaymentCompletionModel paymentState;
            if (!CacheProvider.TryGet(CacheKeys.FRIENDS_ESTIMATE_PAYMENT_STATE, out paymentState))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J82_ESTIMATE_LANDING);
            }

            var estimates = EstimateRestClient.EstimateDetails(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.EstimateDetailsRequest()
            {
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken,
                estimatenumber = state?.estimateno
            }, Request.Segment(), RequestLanguage);
            if (estimates.Succeeded && estimates.Payload != null)
            {
                UpdatedEstimate = estimates.Payload.estimatedetailitem.FirstOrDefault();
            }

            if (UpdatedEstimate == null)
            {
                UpdatedEstimate = state;
                UpdatedEstimate.netvalue3 = decimal.Parse("0");
            }
            UpdatedEstimate.PartialPayment = paymentState.Total;

            var model = new FriendsEstimatePaymentConfirmationModel()
            {
                Estimate = UpdatedEstimate,
                SuqiaAmount = paymentState.SuqiaAmount,
                EstimateHistory = new EstimateHistory.Account
                {
                    Date = paymentState.PaymentDate.ToString("dd MMM yyyy | HH:mm:ss", SitecoreX.Culture),
                    ReceiptID = string.Format("{0:yyMMdd}{1}", paymentState.PaymentDate, paymentState.DegTransactionId),
                    DegTransID = paymentState.DegTransactionId,
                    DewaTransId = paymentState.DewaTransactionId,
                    ContractAccount = state.ca_number,
                    BPDetails = state._ownernum
                }
            };

            #region Not currently working as expected. This is a WS shortcoming

            //var history = GetEstimatePaymentHistory(true);
            //if (history.HistoricalEstimates.Any())
            //{
            //	foreach (var item in history.HistoricalEstimates)
            //	{
            //		if (item.EstimationNo == state.EstimateDetail.EstimateNo)
            //		{
            //			model.EstimateHistory = item;
            //		}
            //	}
            //}

            #endregion Not currently working as expected. This is a WS shortcoming

            ViewBag.PaymentSucceeded = true;

            return View("~/Views/Feature/Bills/Estimates/_FriendsEstimateConfirmation.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetFriendsEstimateHistory()
        {
            EstimatePaymentHistoryModel model = GetEstimatePaymentHistory(true);

            if (model.HistoricalEstimates == null || !(model.HistoricalEstimates != null && model.HistoricalEstimates.Any()))
            {
                ModelState.AddModelError(string.Empty, Translate.Text("NoDataAvailable"));
            }
            return View("~/Views/Feature/Bills/Estimates/_FriendsEstimateHistory.cshtml", model);
        }

        #endregion Friend's estimates

        #region My estimates

        [HttpGet]
        public ActionResult MyEstimates()
        {
            string myestimatePayment;
            if (!CacheProvider.TryGet(CacheKeys.MY_ESTIMATE_STATE_Payment, out myestimatePayment))
            {
                CacheProvider.Remove(CacheKeys.MY_ESTIMATE_STATE);
            }
            CacheProvider.Remove(CacheKeys.MY_ESTIMATES);
            CacheProvider.Remove(CacheKeys.MY_ESTIMATE_PAYMENT_STATE);

            string paymentError;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out paymentError))
            {
                ModelState.AddModelError(string.Empty, paymentError);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }

            var model = new MyEstimates();
            var estimates = EstimateRestClient.EstimateCustomerlist(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.EstimateCustomerListResquest()
            {
                sessionid = CurrentPrincipal.SessionToken,
                userid = CurrentPrincipal.UserId,
            }, Request.Segment(), RequestLanguage);
            //var estimates = DewaApiClient.GetEstimates(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (estimates.Succeeded && estimates.Payload != null)
            {
                model.Estimates = estimates.Payload.estimatedetailitem.ToList();

                CacheProvider.Store(CacheKeys.MY_ESTIMATES, new CacheItem<MyEstimates>(model));
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text(estimates.Message));
            }
            return View("~/Views/Feature/Bills/Estimates/_MyEstimates.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MyEstimates(string estimateNumber, string estimatedamount, PaymentMethod paymentMethod, string bankkey, string SuqiaDonation, string SuqiaDonationAmt)
        {
            MyEstimates state;
            if (!CacheProvider.TryGet(CacheKeys.MY_ESTIMATES, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J81_ESTIMATES_LANDING);
            }

            var estimate = new DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateCustomerItemResponse();

            foreach (var li in state.Estimates)
            {
                if (li.estimateno == estimateNumber)
                {
                    estimate = li;
                    estimate.PartialPayment = decimal.Parse(estimatedamount);
                    CacheProvider.Store(CacheKeys.MY_ESTIMATE_STATE, new CacheItem<DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateCustomerItemResponse>(li));
                    break;
                }
            }

            #region [MIM Payment Implementation]

            var payRequest = new CipherPaymentModel();
            payRequest.PaymentData.amounts = Convert.ToString(estimate.PartialPayment);
            payRequest.PaymentData.contractaccounts = estimate.ca_number.TrimStart('0');
            payRequest.PaymentData.estimatenumber = estimate.estimateno;
            payRequest.PaymentData.businesspartner = Convert.ToString(estimate.ownernum?.TrimStart('0'));
            payRequest.PaymentData.ownerbusinesspartnernumber = Convert.ToString(estimate.sold_to_party?.TrimStart('0'));
            payRequest.PaymentData.consultantbusinesspartnernumber = Convert.ToString(estimate.consultantno?.TrimStart('0'));
            payRequest.PaymentData.email = estimate.con_email;
            payRequest.ServiceType = ServiceType.EstimatePayment;
            payRequest.PaymentMethod = paymentMethod;
            payRequest.IsThirdPartytransaction = false;
            payRequest.BankKey = bankkey;
            payRequest.SuqiaValue = SuqiaDonation;
            payRequest.SuqiaAmt = SuqiaDonationAmt;
            var payResponse = ExecutePaymentGateway(payRequest);
            if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
            {
                CacheProvider.Store(CacheKeys.MY_ESTIMATE_STATE_Payment, new AccessCountingCacheItem<string>("proceed",Times.Once));
                return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
            }
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(string.Join("\n", payResponse.ErrorMessages.Values.ToList())));

            #endregion [MIM Payment Implementation]

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J81_ESTIMATES_LANDING);
        }

        [HttpGet]
        public ActionResult MyEstimatesConfirmation()
        {
            DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateCustomerItemResponse state;
            DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateCustomerItemResponse UpdatedEstimate = new DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.EstimateCustomerItemResponse();

            if (!CacheProvider.TryGet(CacheKeys.MY_ESTIMATE_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J81_ESTIMATES_LANDING);
            }

            PaymentCompletionModel paymentState;
            if (!CacheProvider.TryGet(CacheKeys.MY_ESTIMATE_PAYMENT_STATE, out paymentState))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J81_ESTIMATES_LANDING);
            }
            var estimates = EstimateRestClient.EstimateCustomerlist(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.EstimateCustomerListResquest()
            {
                sessionid = CurrentPrincipal.SessionToken,
                userid = CurrentPrincipal.UserId,
            }, Request.Segment(), RequestLanguage);

            //var estimates = DewaApiClient.GetEstimates(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (estimates.Succeeded && estimates.Payload != null)
            {
                UpdatedEstimate = estimates.Payload.estimatedetailitem.ToList().Where(x => x.estimateno == state.estimateno).FirstOrDefault();
            }

            if (UpdatedEstimate == null)
            {
                UpdatedEstimate = state;
                UpdatedEstimate.netvalue3 = decimal.Parse("0");
            }
            UpdatedEstimate.PartialPayment = paymentState.Total;

            var model = new EstimatePaymentConfirmationModel()
            {
                Estimate = UpdatedEstimate,
                SuqiaAmount = paymentState.SuqiaAmount,
                EstimateHistory = new EstimateHistory.Account
                {
                    Date = paymentState.PaymentDate.ToString("dd MMM yyyy | HH:mm:ss", SitecoreX.Culture),
                    ReceiptID = string.Format("{0:yyMMdd}{1}", paymentState.PaymentDate, paymentState.DegTransactionId),
                    DegTransID = paymentState.DegTransactionId,
                    DewaTransId = paymentState.DewaTransactionId,
                    ContractAccount = state.ca_number.TrimStart('0'),
                    BPDetails = state.ownernum.TrimStart('0')
                }
            };

            ViewBag.PaymentSucceeded = true;

            return View("~/Views/Feature/Bills/Estimates/_MyEstimatesConfirmation.cshtml", model);
        }

        #endregion My estimates

        [HttpGet]
        public PartialViewResult EstimatePaymentHistory(bool payForFriend)
        {
            EstimatePaymentHistoryModel model = GetEstimatePaymentHistory(payForFriend);

            return PartialView("~/Views/Feature/Bills/Estimates/_EstimatePaymentHistory.cshtml", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DisplayPdf(string estimateNumber)
        {
            var response = EstimateRestClient.EstimatePDF(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.EstimatePdfRequest()
            {
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken,
                estimatenumber = estimateNumber,
            }, Request.Segment(), RequestLanguage);
            string filename = getClearstr(string.Format("{0}.pdf", Convert.ToString($"{estimateNumber}{Guid.NewGuid().ToString()}")));
            byte[] fileContent = new byte[0];

            if (response != null && response.Succeeded &&
                response.Payload != null && !string.IsNullOrWhiteSpace(response.Payload.filecontent))
            {
                fileContent = Convert.FromBase64String(response.Payload.filecontent ?? "");
            }
            return File(fileContent, "application/pdf", filename);
        }

        private EstimatePaymentHistoryModel GetEstimatePaymentHistory(bool friendsHistory)
        {
            var model = new EstimatePaymentHistoryModel()
            {
                ThirdPartyPayments = friendsHistory
            };
            var history = EstimateRestClient.EstimateHistory(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.EstimatePaymentHistoryRequest()
            {
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken,
                payforfriend = friendsHistory ? "X" : null,
            }, Request.Segment(), RequestLanguage);

            if (history.Succeeded && history.Payload != null
                && history.Payload.estimatehistory != null && history.Payload.estimatehistory.Any())
            {
                var sorted = history.Payload.estimatehistory.OrderByDescending(c => c.dateandtime);
                foreach (var item in sorted)
                {
                    if (item != null)
                    {
                        model.HistoricalEstimates.Add(item);
                    }
                }
            }
            return model;
        }

        [HttpGet]
        public ActionResult EstimateHistory()
        {
            GetEstimateHistoryData(new EstimateRequest());
            return View("~/Views/Feature/Bills/Estimates/_EstimateHistory.cshtml", new MyEstimates());
        }

        [HttpGet]
        public PartialViewResult EstimateFilterList(EstimateRequest request)
        {
            EstimateHistoryData model = GetEstimateHistoryData(request);
            if (model != null && model.EstimateDetails != null && model.EstimateDetails.Count() > 0)
            {
                model.EstimateNoList = model.EstimateDetails.Select(x => x.estimatenumber).ToArray();
                model.PagedEstimateDetails = model.EstimateDetails.ToPagedList(Convert.ToInt32(request.PageNo ?? 1), 10);
            }
            if (request.Type == "subrender")
            {
                return PartialView($"~/Views/Feature/Bills/Estimates/_EstimateDataList.cshtml", model);
            }
            return PartialView($"~/Views/Feature/Bills/Estimates/_EstimateFilterList.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetEstiamteTaxInvoicePDF(EstimateTaxInvoicePDFRequest rqst)
        {
            var pdfBytes = GetTaxInvoicePDFByEstimateNo(rqst);
            return File(pdfBytes, "application/pdf", getClearstr(string.Format("{0}.pdf", (rqst.EstNo ?? rqst.DocNo))));
        }

        [HttpGet]
        public ActionResult GetTransactionList(EstimateRequest request)
        {
            request.PageNo = Convert.ToInt32(request.PageNo ?? 1);

            EstimateReceiptHistoryData model = GetPaymentHistoryData(request);
            model.sdType = request.sdType;
            ////Sachin new logic as to show all tax invoice pdf.
            //if (model.PaymentHistoryDetails != null && model.PaymentHistoryDetails.Count == 1)
            //{
            //    return Json(model, JsonRequestBehavior.AllowGet);
            //}
            model.PagedPaymentHistoryDetails = model.PaymentHistoryDetails?.ToPagedList<EstimateTransaction>(request.PageNo.GetValueOrDefault(), 5);
            return PartialView($"~/Views/Feature/Bills/Estimates/_EstimateRecieptListModalContent.cshtml", model);
        }

        #region logicalfunction

        /// <summary>
        /// Get Estimate History Data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private EstimateHistoryData GetEstimateHistoryData(EstimateRequest request)
        {
            EstimateHistoryData returnData = null;
            EstimateHistoryData dbData = null;
            CacheProvider.TryGet(CacheKeys.ESTIMATIONT_DATALIST, out dbData); // check the cookies
            if (dbData == null ||
                (dbData != null && Convert.ToInt32(dbData.EstimateDetails?.Count()) == 0) ||
                (dbData != null && Convert.ToInt32(dbData.ContractAccountDetails?.Count()) == 0) ||
                (dbData != null && dbData.SessionToken != CurrentPrincipal.SessionToken))
            {
                dbData = new EstimateHistoryData()
                {
                    //Accountnumber = request.IsForceAll ? "" : request.Accountnumber,
                    EstimateNo = request.EstimateNo
                };

                try
                {
                    var serviceResponse = EstimateRestClient.EstimateAmountDisplay(
                    new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.EstimateAmountDisplayRequest()
                    {
                        EstimateDetailsRetrieve = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.EstimateDetailsRetrieve
                        {
                            contractaccountnumber = dbData.Accountnumber,
                            indicator = "01",
                            sessionid = CurrentPrincipal.SessionToken,
                        }
                    }, Request.Segment(), RequestLanguage);

                    //var servcieResponse = DewaApiClient.GetEstimateAmountDisplay(new DEWAXP.Foundation.Integration.DewaSvc.estimateRequestParams()
                    //{
                    //    contractaccountnumber = dbData.Accountnumber,
                    //    //enddate = "",
                    //    indicator = "01",
                    //    //projectdefination = "",
                    //    //sdnumber = "",
                    //    sessionid = CurrentPrincipal.SessionToken,
                    //    //startdate = "",
                    //}, RequestLanguage, Request.Segment());

                    if (serviceResponse.Succeeded && serviceResponse.Payload != null)
                    {
                        dbData.EstimateDetails = serviceResponse.Payload.estimate_details;
                        dbData.ContractAccountDetails = serviceResponse.Payload.CA_details;
                        dbData.SessionToken = CurrentPrincipal.SessionToken;
                    }
                    if (dbData.ContractAccountDetails.Any() || dbData.EstimateDetails.Any())
                    {
                        CacheProvider.Store(CacheKeys.ESTIMATIONT_DATALIST, new CacheItem<EstimateHistoryData>(dbData, TimeSpan.FromMinutes(20)));
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                }
            }
            returnData = new EstimateHistoryData()
            {
                Accountnumber = request.Accountnumber,
                EstimateNo = request.EstimateNo,
                EstimateDetails = dbData.EstimateDetails,
                ContractAccountDetails = dbData.ContractAccountDetails,
            };

            //filterdata
            if (!string.IsNullOrEmpty(request.Accountnumber))
            {
                returnData.EstimateDetails = returnData.EstimateDetails.Where(x => x.contractaccount == request.Accountnumber).ToList();
            }
            else
            {
                returnData.EstimateDetails = null;
            }

            if (!string.IsNullOrEmpty(request.EstimateNo))
            {
                returnData.EstimateDetails = returnData.EstimateDetails.Where(x => x != null && x.estimatenumber == request.EstimateNo).ToList();
            }
            return returnData;
        }

        /// <summary>
        /// Get TaxInvoicePDF By EstimateNo
        /// </summary>
        /// <param name="estimateNo"></param>
        /// <returns></returns>
        private byte[] GetTaxInvoicePDFByEstimateNo(EstimateTaxInvoicePDFRequest rqst)
        {
            var response = EstimateRestClient.NewConnectionTaxInvoicePdf(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.NewConnectionTaxInvoicePdfRequest()
            {
                sddocumentnumber = rqst.EstNo,
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken,
                sdtype = rqst.Sdtype,
                ficadocumentnumber = rqst.DocNo
            }, Request.Segment(), RequestLanguage);
            //var response = DewaApiClient.GetNewConnectionTaxInvoicePDF(new DEWAXP.Foundation.Integration.DewaSvc.GetNewConnectionTaxInvoicePDF()
            //{
            //    sddocumentnumber = rqst.EstNo,
            //    userid = CurrentPrincipal.UserId,
            //    sessionid = CurrentPrincipal.SessionToken,
            //    sdtype = rqst.Sdtype,
            //    ficadocumentnumber = rqst.DocNo,
            //}, RequestLanguage, Request.Segment());

            byte[] pdfFile = response?.Payload?.content ?? new byte[0];

            return pdfFile;
        }

        private EstimateReceiptHistoryData GetPaymentHistoryData(EstimateRequest request)
        {
            string code = "";
            EstimateReceiptHistoryData returnData = new EstimateReceiptHistoryData()
            {
                Accountnumber = request.Accountnumber,
                EstimateNo = request.EstimateNo,
            };

            //spliting & setting the value
            var estArray = request.EstimateNo.Split('-');
            if (estArray != null && estArray.Count() > 1)
            {
                code = estArray[0];
                returnData.EstimateNo = estArray[1];
            }

            //request.EstimateNo = code;
            try
            {
                var servcieResponse = DewaApiClient.GetPaymentHistoryV1(new DEWAXP.Foundation.Integration.DewaSvc.GetPaymentHistory()
                {
                    userid = CurrentPrincipal.UserId,
                    sessionid = CurrentPrincipal.SessionToken,
                    contractaccountnumber = request.Accountnumber,
                    estimatenumber = code,
                }, RequestLanguage, Request.Segment());

                if (servcieResponse != null && servcieResponse.Payload != null && servcieResponse.Payload.paymentlist != null && servcieResponse.Payload.paymentlist.Any())
                {
                    DEWAXP.Foundation.Integration.DewaSvc.paymentDetails[] paymentList = servcieResponse.Payload.paymentlist.Where(x => x.applicationnumber.Contains(returnData.EstimateNo)).ToArray();
                    if (paymentList.Any())
                    {
                        returnData.PaymentHistoryDetails = new List<EstimateTransaction>();
                        foreach (var item in paymentList.Where(x => x != null))
                        {
                            returnData.PaymentHistoryDetails.Add(EstimateTransaction.From(item));
                        }
                        CacheProvider.Store(CacheKeys.SELECTED_ESTIMATIONTRANSACTIONACCOUNT, new CacheItem<string>(request.Accountnumber, TimeSpan.FromMinutes(20)));
                    }
                }
                else
                {
                    CacheProvider.Remove(CacheKeys.SELECTED_ESTIMATIONTRANSACTIONACCOUNT);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return returnData;
        }

        #endregion logicalfunction

        [HttpGet]
        [TwoPhaseAuthorize]
        public ActionResult PaymentDetails(string id = "", string d = "", string t = "", string dNo = "")
        {
            string date = d;
            string type = t;
            if (!string.IsNullOrWhiteSpace(dNo))
            {
                string transactionaccount;
                if (CacheProvider.TryGet(CacheKeys.SELECTED_ESTIMATIONTRANSACTIONACCOUNT, out transactionaccount))
                {
                    var response = DewaApiClient.GetPaymentHistoryV1(new DEWAXP.Foundation.Integration.DewaSvc.GetPaymentHistory()
                    {
                        userid = CurrentPrincipal.UserId,
                        sessionid = CurrentPrincipal.SessionToken,
                        contractaccountnumber = transactionaccount,
                        estimatenumber = "X",
                    });

                    if (response.Succeeded)
                    {
                        if (response.Payload != null && response.Payload.paymentlist != null)
                        {
                            var docrecipt = response.Payload.paymentlist.Where(x => x.documentnumber.Equals(dNo));
                            if (docrecipt.Any())
                            {
                                EstimateReceiptDetailModel model = EstimateReceiptDetailModel.From(docrecipt.FirstOrDefault());
                                return PartialView($"~/Views/Feature/Bills/Estimates/_PaymentDetails.cshtml", model);
                            }
                        }
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(date) && !string.IsNullOrWhiteSpace(type))
            {
                DateTime parseddate;
                if (DateTime.TryParseExact(date, _commonUtility.DF_yyyyMMddHHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out parseddate))
                {
                    //var onlinereceipt = DewaApiClient.GetOnlinePaymentReceipt(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id, date, type, RequestLanguage, Request.Segment());
                    var onlinereceipt = EstimateRestClient.OnlinePaymentReceipt(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.PaymentReceiptDetailsRequest()
                    {
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId,
                        transactionid = id,
                        datetimestamp = date,
                        paymenttypetext = type
                    }, Request.Segment(), RequestLanguage);
                    if (onlinereceipt.Succeeded && onlinereceipt.Payload != null)
                    {
                        EstimateReceiptDetailModel model = EstimateReceiptDetailModel.From(onlinereceipt.Payload);
                        return PartialView($"~/Views/Feature/Bills/Estimates/_PaymentDetails.cshtml", model);
                    }
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EstimateHistoryPage);
        }

        [HttpGet]
        public ActionResult GetEstimateAccountSelector(EstimateAccountSelectorModel model)
        {
            if (model == null)
            {
                model = new EstimateAccountSelectorModel();
            }

            //get accountno From session
            if (string.IsNullOrEmpty(model.SelectedContractAccNo))
            {
                string accNo = "";
                CacheProvider.TryGet(CacheKeys.SELECTED_ESTIMATIONTRANSACTIONACCOUNT, out accNo);
                model.SelectedContractAccNo = accNo;
            }

            var d = GetEstimateHistoryData(new EstimateRequest() { IsForceAll = true });

            if (d != null && d.ContractAccountDetails != null && d.ContractAccountDetails.Any())
            {
                model.EstimateAccountSelectorList = model.BindFrom(d.ContractAccountDetails?.ToList() ?? null, model.SelectedContractAccNo);

                if (model.EstimateAccountSelectorList != null && !string.IsNullOrEmpty(model.SearchText))
                {
                    string search = model.SearchText.ToLower();
                    model.EstimateAccountSelectorList = model.EstimateAccountSelectorList.
                                                        Where(x => (x.BusinessPartnerName ?? "").ToLower().Contains(search) || (x.ContractAccNo ?? "").ToLower().Contains(search)).ToList();

                    if (model.EstimateAccountSelectorList != null &&
                        model.EstimateAccountSelectorList.Count > 0)
                    {
                        var selectedExitedInFilterData = model.EstimateAccountSelectorList.Where(x => x.ContractAccNo == model.SelectedContractAccNo).FirstOrDefault();
                        if (selectedExitedInFilterData == null)
                        {
                            model.SelectedContractAccNo = model.EstimateAccountSelectorList.FirstOrDefault().ContractAccNo;
                        }
                    }
                    else
                    {
                        model.SelectedContractAccNo = null;
                    }
                }
            }

            CacheProvider.Store(CacheKeys.SELECTED_ESTIMATIONTRANSACTIONACCOUNT, new CacheItem<string>(model.SelectedContractAccNo, TimeSpan.FromMinutes(20)));

            var currentItem = ContextRepository.GetCurrentItem<Item>();
            ViewBag.IsRTL = currentItem != null && currentItem.Language.CultureInfo.TextInfo.IsRightToLeft;

            if (model.OnlyPartailList)
            {
                return PartialView("~/Views/Feature/Bills/Estimates/Partail/_EstimateAccountModalFilterList.cshtml", model);
            }
            return PartialView("~/Views/Feature/Bills/Estimates/Partail/_EstimateAccountSelector.cshtml", model);
        }

        #region [Estimate Refund]

        #region [actions]

        [HttpGet]
        public ActionResult EstimateRefund()
        {
            EsitimateRefundViewModel model = new EsitimateRefundViewModel();
            CacheProvider.Remove("er_confirmedData");
            model.ConnectionDetails = PostSmartCommunicationData(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.NewConnectionRefundRequest()
            {
                mode = "G",
            })?.connectiondetails ?? null;
            if (model.ConnectionDetails != null && model.ConnectionDetails.Count > 0)
            {
                DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.NewConnectionRefundResponse userDetail = null;
                bool isPredatavaliable = CacheProvider.TryGet("er_appid", out userDetail) && userDetail != null;
                if (isPredatavaliable)
                {
                    model.ApplicationNumber = userDetail.notificationnumber;
                }
                //else
                //{
                //    var defaultConnectionDetail = model.ConnectionDetails?.FirstOrDefault();
                //    if (defaultConnectionDetail != null)
                //    {
                //        model.ApplicationNumber = defaultConnectionDetail?.applicationno ?? null;
                //        model.EstimateNo = defaultConnectionDetail?.posid ?? null;
                //        userDetail = PostSmartCommunicationData(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.NewConnectionRefundRequest()
                //        {
                //            applicationnumber = model.ApplicationNumber,
                //            estimatenumber = model.EstimateNo,
                //            mode = "R",
                //        });
                //    }
                //}

                if (userDetail != null)
                {
                    model.CustomerName = userDetail.fullname;
                    model.RegisteredEmail = userDetail.emailaddress;
                    model.RegisteredMobileNumber = userDetail.mobile;
                    model.IbanDetails = userDetail.ibandetails;
                    model.OkCheque = userDetail.okcheque;
                    model.OkIBAN = userDetail.okiban;
                    model.AttachFlag = userDetail.attachflag;
                    model.IsServiceFailure = userDetail.IsServiceFailure;
                    if (model.IsServiceFailure)
                    {
                        ModelState.AddModelError("", userDetail.description);
                    }
                }
                else
                {
                    model.IsServiceFailure = true;
                }
                model = SetEsitimateRefundViewModelDefaultValue(model);
            }
            else
            {
                ModelState.AddModelError("", Translate.Text("ER_ServiceUnavailable"));
            }
            return View("~/Views/Feature/Bills/Estimates/_EstimateRefund.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EstimateRefund(EsitimateRefundViewModel model)
        {
            if (ModelState.IsValid)
            {
                var submissionRequest = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.NewConnectionRefundRequest()
                {
                    applicationnumber = model.ApplicationNumber,
                    email = model.RegisteredEmail,
                    mobile = model.RegisteredMobileNumber,
                    refundmode = model.RefundMode,
                    reason = model.ReasonForRefund,
                    estimatenumber = model.EstimateNo,
                    iban = model.RefundMode == "I" ? string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, model.ConfirmIBANno) : model.ChequeNO,
                    mode = "W",
                    filecontent = Convert.ToBase64String(model.FTADeclarationFormFile != null ? model.FTADeclarationFormFile.ToArray() : new byte[0]),
                    filename = model.FTADeclarationFormFile != null ? model.FTADeclarationFormFile.FileName?.ToUpper() : string.Empty,
                };

                var submissionReponse = PostSmartCommunicationData(submissionRequest);
                if (submissionReponse != null)
                {
                    if (string.IsNullOrEmpty(model.ConfirmIBANno) && model.RefundMode == "Z")
                    {
                        model.ConfirmIBANno = _commonUtility.GetMaskedIBAN(model.IBANNo);
                    }
                    if (!string.IsNullOrWhiteSpace(submissionReponse.notificationnumber))
                    {
                        model.ReferenceNo = submissionReponse.notificationnumber;

                        CacheProvider.Store("er_confirmedData", new AccessCountingCacheItem<EsitimateRefundViewModel>(model, Times.Exactly(5)));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.EstimateRefundConfrimationPage);
                    }
                }

                ModelState.AddModelError("", submissionReponse.description ?? ErrorMessages.UNEXPECTED_ERROR);
            }
            return View("~/Views/Feature/Bills/Estimates/_EstimateRefund.cshtml", model);
        }

        public ActionResult EsitimateRefundConfirmation()
        {
            EsitimateRefundViewModel model = null;
            if (CacheProvider.TryGet("er_confirmedData", out model) && model != null)
            {
                return View("~/Views/Feature/Bills/Estimates/_EstimateRefundSuccess.cshtml", model);
            }
            ModelState.AddModelError("", ErrorMessages.UNEXPECTED_ERROR);
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EstimateRefundPage);
        }

        [HttpGet]
        public JsonResult GetSmartCommunicationData(GetAppilcationDetails model)
        {
            if (!string.IsNullOrEmpty(model.AppID) && !string.IsNullOrWhiteSpace(model.EstNo))
            {
                var userDetail = PostSmartCommunicationData(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.NewConnectionRefundRequest()
                {
                    applicationnumber = model.AppID,
                    estimatenumber = model.EstNo,
                    reason = model.reason,
                    refundmode = model.rmode,
                    mode = "R",
                });
                if (userDetail.IsServiceFailure)
                {
                    if (!(!string.IsNullOrWhiteSpace(model.reason) && !string.IsNullOrWhiteSpace(model.rmode)))
                    {
                        userDetail.notificationnumber = model.AppID;
                        //CacheProvider.Store("er_appid", new AccessCountingCacheItem<DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.NewConnectionRefundResponse>(userDetail, Times.Once));
                    }
                }
                return Json(new { success = userDetail != null, data = userDetail }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, data = new { description = Translate.Text("ER_ApplicationNumberRequiredMsg"), IsServiceFailure = true } }, JsonRequestBehavior.AllowGet);
        }

        #endregion [actions]

        #region [Function]

        private DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.NewConnectionRefundResponse PostSmartCommunicationData(DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate.NewConnectionRefundRequest request)
        {
            DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.NewConnectionRefundResponse returnData = null;
            request.userid = CurrentPrincipal.UserId;
            request.sessionid = CurrentPrincipal.SessionToken;
            var r = EstimateRestClient.SmartCommunicationSubmit(request, Request.Segment(), RequestLanguage);
            if (r != null)
            {
                if (r.Succeeded)
                {
                    returnData = r.Payload;
                    if (returnData != null &&
                        returnData.ibandetails != null && returnData.ibandetails.Any())
                    {
                        returnData.ibandetails = returnData.ibandetails.Select(x => new DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.NewConnectionRefundIbandetail()
                        {
                            iban = x.iban.Replace("AE", ""),
                            maskiban = x.maskiban,
                            sequenceno = x.sequenceno,
                        })?.ToList();
                    }
                }
                else
                {
                    returnData = new DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate.NewConnectionRefundResponse()
                    {
                        IsServiceFailure = true,
                        description = r.Message
                    };
                }
            }

            return returnData;
        }

        private EsitimateRefundViewModel SetEsitimateRefundViewModelDefaultValue(EsitimateRefundViewModel value)
        {
            #region [Get value]

            List<SelectListItem> _resonCode, _refundMode = null;
            string _reasonCodeCacheKey = "EstReasonCodeList" + RequestLanguageCode;
            string _refundModeCacheKey = "EstRefundModeList" + RequestLanguageCode;

            if (!(CacheProvider.TryGet(_reasonCodeCacheKey, out _resonCode) && _resonCode != null))
            {
                _resonCode = ScHelper.GetDataSourceItemList(SitecoreItemIdentifiers.EstimateReasonCode);
                CacheProvider.Store(_reasonCodeCacheKey, new CacheItem<List<SelectListItem>>(_resonCode, TimeSpan.FromHours(1)));
            }

            if (!(CacheProvider.TryGet(_refundModeCacheKey, out _refundMode) && _refundMode != null))
            {
                _refundMode = ScHelper.GetDataSourceItemList(SitecoreItemIdentifiers.EstimateRefundMode);
                CacheProvider.Store(_refundModeCacheKey, new CacheItem<List<SelectListItem>>(_refundMode, TimeSpan.FromHours(1)));
            }

            #endregion [Get value]

            #region [Set value]

            value.ReasonCodeList = _resonCode;
            value.RefundModeList = _refundMode;

            #endregion [Set value]

            return value;
        }

        #endregion [Function]

        #endregion [Estimate Refund]
    }
}