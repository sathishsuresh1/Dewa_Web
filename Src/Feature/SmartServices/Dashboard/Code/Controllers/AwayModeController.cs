using DEWAXP.Foundation.Content.Filters.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScContext = Sitecore.Context;
using ScData = Sitecore.Data;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using _commonUtility = DEWAXP.Foundation.Content.Utils.CommonUtility;
using Newtonsoft.Json;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Premise;

namespace DEWAXP.Feature.Dashboard.Controllers
{
    public class AwayModeController : BaseController
    {

        #region [Private Variable]



        #region [Process & Action Code]
        private const string AwayModeCreate = "01";
        private const string AwayModeExtended = "02";
        private const string AwayModeDeactivate = "03";
        private const string AwayModeOTP = "04";
        private const string AwayModeRead = "05";
        private const string AwayModeResendOTP = "06";

        private const string AwayModeLoggedIn = "L";
        private const string AwayModeAnonymous = "A";


        private const string _S_CreateKey = "AWMCreate";
        private const string _S_ExtendKey = "AWMExtendRead";
        private const string _S_DeactiveKey = "AWMDeactiveRead";
        #endregion
        #endregion

        #region [Action]
        // GET: AwayMode
        [HttpGet, TwoPhaseAuthorizeAttribute]
        public ActionResult CreateAwayMode()
        {
            Models.AwayMode.CreateModel model = new Models.AwayMode.CreateModel()
            {
                IsLoggedIn = IsLoggedIn,
                FrequencyList = GetDataSource(SitecoreItemIdentifiers.DTMC_AwayMode_Frequency_List)

            };
            return View("~/Views/Feature/Dashboard/AwayMode/CreateAwayMode.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken, TwoPhaseAuthorizeAttribute]
        public ActionResult CreateAwayMode(Models.AwayMode.CreateModel model)
        {
            try
            {

                string msg = ValidateDateRange(model.BeginDate, model.EndDate);

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    ModelState.AddModelError("", msg);
                }
                if (ModelState.IsValid)
                {
                    var response = AwayModeClient.Manageawaymode(new Foundation.Integration.APIHandler.Models.Request.AwayMode.ManageAwayModeRequest()
                    {
                        beginDate = GetSubmittedFormattedDate(model.BeginDate),
                        endDate = GetSubmittedFormattedDate(model.EndDate),
                        email = model.Email,
                        contractAccount = model.ContractAccount,
                        lang = RequestLanguageCode,
                        loginType = AwayModeLoggedIn,
                        frequency = model.Frequency,
                        action = AwayModeCreate,
                        credential = AuthStateService.GetActiveProfile().SessionToken,
                    }, Request.Segment());


                    if (response != null)
                    {
                        if (response.Succeeded)
                        {
                            model.RequestId = response.Payload.requestDetails.request;                           
                            model.BeginDate = GetShowFormattedDate(model.BeginDate);
                            model.EndDate = GetShowFormattedDate(model.EndDate);
                            model.ActionType = AwayModeCreate;
                            return View("~/Views/Feature/Dashboard/AwayMode/SuccessCreateAwayMode.cshtml", model);
                        }
                        else
                        {
                            ModelState.AddModelError("", response.Message);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                LogService.Error(ex, this);
            }

            model.IsLoggedIn = IsLoggedIn;
            model.FrequencyList = GetDataSource(SitecoreItemIdentifiers.DTMC_AwayMode_Frequency_List);
            return View("~/Views/Feature/Dashboard/AwayMode/CreateAwayMode.cshtml", model);
        }

        [HttpGet]
        public ActionResult CreateAnonymousAwayMode()
        {
            if (IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.DTMC_LOGGEDIN_AWAYMODE_CREATE);
            }
            Models.AwayMode.CreateModel model = new Models.AwayMode.CreateModel();
            model.FrequencyList = GetDataSource(SitecoreItemIdentifiers.DTMC_AwayMode_Frequency_List);
            model.key = _S_CreateKey;
            return View("~/Views/Feature/Dashboard/AwayMode/CreateAwayMode.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateAnonymousAwayMode(Models.AwayMode.CreateModel model)
        {
            try
            {
                model.key = _S_CreateKey;
                string msg = ValidateDateRange(model.BeginDate, model.EndDate);

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    ModelState.AddModelError("", msg);
                }

                if (ModelState.IsValid)
                {
                    if (model.ActionType == "get_otp")
                    {

                        var responseCreate = AwayModeClient.Manageawaymode(new Foundation.Integration.APIHandler.Models.Request.AwayMode.ManageAwayModeRequest()
                        {
                            beginDate = GetSubmittedFormattedDate(model.BeginDate),
                            endDate = GetSubmittedFormattedDate(model.EndDate),
                            email = model.Email,
                            contractAccount = model.ContractAccount,
                            lang = RequestLanguageCode,
                            loginType = AwayModeAnonymous,
                            frequency = model.Frequency,
                            action = AwayModeCreate,
                        }, Request.Segment());


                        if (responseCreate != null)
                        {

                            if (responseCreate.Succeeded)
                            {
                                model.RequestId = responseCreate.Payload.requestNumber;
                                CacheProvider.Store(_S_CreateKey, new CacheItem<Models.AwayMode.CreateModel>(model));
                                return View("~/Views/Feature/Dashboard/AwayMode/OTPValidation.cshtml", model);
                            }
                            else
                            {
                                ModelState.AddModelError("", responseCreate.Message);
                            }
                        }
                    }


                    if (model.ActionType == "submit_otp")
                    {
                        Models.AwayMode.CreateModel storeData = null;
                        if (CacheProvider.TryGet("AWMCreate", out storeData))
                        {
                            var responseOTPSubmit = AwayModeClient.Manageawaymode(new Foundation.Integration.APIHandler.Models.Request.AwayMode.ManageAwayModeRequest()
                            {
                                lang = RequestLanguageCode,
                                loginType = AwayModeAnonymous,
                                action = AwayModeOTP,
                                request = storeData.RequestId,
                                otp = model.otp
                            }, Request.Segment());

                            //presession Id
                            model.RequestId = storeData.RequestId;

                            if (responseOTPSubmit != null)
                            {
                                if (responseOTPSubmit.Succeeded)
                                {
                                    //updatesession Id
                                    storeData.RequestId = responseOTPSubmit.Payload.requestNumber;
                                    CacheProvider.Remove(_S_CreateKey);
                                    storeData.ActionType = AwayModeCreate;
                                    model = storeData;

                                    model.BeginDate = GetShowFormattedDate(model.BeginDate);
                                    model.EndDate = GetShowFormattedDate(model.EndDate);
                                    return View("~/Views/Feature/Dashboard/AwayMode/SuccessCreateAwayMode.cshtml", model);
                                }
                                else
                                {
                                    ModelState.AddModelError("", responseOTPSubmit.Message);

                                }
                            }
                            return View("~/Views/Feature/Dashboard/AwayMode/OTPValidation.cshtml", model);
                        }
                        else
                        {
                            ModelState.AddModelError("", Translate.Text("AWM_CreateSessionError"));
                        }


                    }
                }

            }
            catch (Exception ex)
            {

                LogService.Error(ex, this);
            }

            model.FrequencyList = GetDataSource(SitecoreItemIdentifiers.DTMC_AwayMode_Frequency_List);
            return View("~/Views/Feature/Dashboard/AwayMode/CreateAwayMode.cshtml", model);
        }


        [HttpGet]
        public ActionResult ExtendAnonymousAwayMode(string r)
        {
            Models.AwayMode.CreateModel model = new Models.AwayMode.CreateModel();
            try
            {

                //Read exiting data from request no and prefill the data.
                #region [Read Data]
                var responseReadData = AwayModeClient.Manageawaymode(new Foundation.Integration.APIHandler.Models.Request.AwayMode.ManageAwayModeRequest()
                {
                    lang = RequestLanguageCode,
                    action = AwayModeRead,
                    loginType = AwayModeAnonymous,
                    code = r
                }, Request.Segment());

                if (responseReadData != null)
                {

                    if (responseReadData.Succeeded)
                    {

                        var _d = responseReadData.Payload.requestDetails;
                        //updatesession Id
                        model.RequestId = _d.request;
                        model.BeginDate = GetBindedFormattedDate(_d.beginDate);
                        model.ContractAccount = _d.contractAccount;
                        model.Email = _d.email;
                        model.Frequency = _d.frequency;
                        model.EndDate = GetBindedFormattedDate(_d.endDate);
                        CacheProvider.Store(_S_ExtendKey, new CacheItem<Models.AwayMode.CreateModel>(model));
                    }
                    else
                    {
                        ModelState.AddModelError("", responseReadData.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError("", ex.Message);
            }
            #endregion
            model.FrequencyList = GetDataSource(SitecoreItemIdentifiers.DTMC_AwayMode_Frequency_List);
            model.key = _S_ExtendKey;
            return View("~/Views/Feature/Dashboard/AwayMode/ExtendAwayMode.cshtml", model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ExtendAnonymousAwayMode(Models.AwayMode.CreateModel model)
        {
            model.key = _S_ExtendKey;
            Models.AwayMode.CreateModel storeData = null;
            try
            {

                string msg = ValidateDateRange(model.BeginDate, model.EndDate);

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    ModelState.AddModelError("", msg);
                }

                if (ModelState.IsValid)
                {
                    if (model.ActionType == "get_otp")
                    {

                        if (CacheProvider.TryGet(_S_ExtendKey, out storeData))
                        {
                            var responseCreateWithOtp = AwayModeClient.Manageawaymode(new Foundation.Integration.APIHandler.Models.Request.AwayMode.ManageAwayModeRequest()
                            {
                                beginDate = GetSubmittedFormattedDate(storeData.BeginDate),
                                endDate = GetSubmittedFormattedDate(model.EndDate),
                                email = storeData.Email,
                                contractAccount = storeData.ContractAccount,
                                lang = RequestLanguageCode,
                                loginType = AwayModeAnonymous,// IsLoggedIn ? AwayModeLoggedIn : AwayModeAnonymous,
                                frequency = model.Frequency,
                                action = AwayModeExtended,
                            }, Request.Segment());

                            storeData.EndDate = model.EndDate;
                            storeData.Frequency = model.Frequency;
                            model = storeData;

                            //presession Id
                            if (responseCreateWithOtp != null)
                            {

                                if (responseCreateWithOtp.Succeeded)
                                {
                                    //Update Session Id

                                    //storeData.RequestId = responseCreateWithOtp.Payload.requestNumber;
                                    
                                    CacheProvider.Store(_S_ExtendKey, new CacheItem<Models.AwayMode.CreateModel>(model));
                                    return View("~/Views/Feature/Dashboard/AwayMode/OTPValidation.cshtml", model);
                                }
                                else
                                {
                                    ModelState.AddModelError("", responseCreateWithOtp.Message);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", Translate.Text("AWM_CreateSessionError"));
                        }

                    }


                    if (model.ActionType == "submit_otp")
                    {

                        if (CacheProvider.TryGet(_S_ExtendKey, out storeData))
                        {
                            var responseOTPSubmit = AwayModeClient.Manageawaymode(new Foundation.Integration.APIHandler.Models.Request.AwayMode.ManageAwayModeRequest()
                            {
                                lang = RequestLanguageCode,
                                loginType = AwayModeAnonymous,
                                action = AwayModeOTP,
                                request = storeData.RequestId,
                                otp = model.otp
                            }, Request.Segment());
                            //presesion Id                          

                            storeData.ActionType = AwayModeExtended;
                            model = storeData;
                           
                          

                            if (responseOTPSubmit != null)
                            {
                                if (responseOTPSubmit.Succeeded)
                                {
                                    CacheProvider.Remove(_S_ExtendKey);
                                    //Updated Session ID
                                    model.BeginDate = GetShowFormattedDate(model.BeginDate);
                                    model.EndDate = GetShowFormattedDate(model.EndDate);
                                    return View("~/Views/Feature/Dashboard/AwayMode/SuccessCreateAwayMode.cshtml", model);
                                }
                                else
                                {
                                    ModelState.AddModelError("", responseOTPSubmit.Message);

                                }
                            }
                            return View("~/Views/Feature/Dashboard/AwayMode/OTPValidation.cshtml", model);
                        }
                        else
                        {
                            ModelState.AddModelError("", Translate.Text("AWM_CreateSessionError"));
                        }

                    }
                }

            }
            catch (Exception ex)
            {

                LogService.Error(ex, this);
            }


            if (storeData != null)
            {

                storeData.Frequency = model.Frequency;
                storeData.EndDate = GetBindedFormattedDate(model.EndDate);
                model = storeData;
                CacheProvider.Store(_S_ExtendKey, new CacheItem<Models.AwayMode.CreateModel>(storeData));
            }
            model.FrequencyList = GetDataSource(SitecoreItemIdentifiers.DTMC_AwayMode_Frequency_List);

            return View("~/Views/Feature/Dashboard/AwayMode/ExtendAwayMode.cshtml", model);
        }


        [HttpGet]
        public ActionResult DeactivateAnonymousAwayMode(string r)
        {
            Models.AwayMode.CreateModel model = new Models.AwayMode.CreateModel();
            model.FrequencyList = GetDataSource(SitecoreItemIdentifiers.DTMC_AwayMode_Frequency_List);
            try
            {

                //Read exiting data from request no and prefill the data.
                #region [Read Data]
                var responseReadData = AwayModeClient.Manageawaymode(new Foundation.Integration.APIHandler.Models.Request.AwayMode.ManageAwayModeRequest()
                {
                    lang = RequestLanguageCode,
                    action = AwayModeRead,
                    loginType = AwayModeAnonymous,
                    code = r
                }, Request.Segment());


                if (responseReadData != null)
                {

                    if (responseReadData.Succeeded)
                    {
                        var _d = responseReadData.Payload.requestDetails;
                        //updated session id
                        model.RequestId = _d.request;
                        model.BeginDate = GetShowFormattedDate(_d.beginDate);
                        model.ContractAccount = _d.contractAccount;
                        model.Email = _d.email;
                        model.Frequency = _d.frequency;
                        model.EndDate = GetShowFormattedDate(_d.endDate);
                        model.FrequencyText = Convert.ToString(model.FrequencyList.Where(x => x.Value == _d.frequency)?.FirstOrDefault()?.Text);
                        CacheProvider.Store(_S_DeactiveKey, new CacheItem<Models.AwayMode.CreateModel>(model));
                    }
                    else
                    {
                        ModelState.AddModelError("", responseReadData.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError("", ex.Message);
            }
            #endregion
            model.key = _S_DeactiveKey;
            return View("~/Views/Feature/Dashboard/AwayMode/DeactiveAwayMode.cshtml", model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeactivateAnonymousAwayMode(Models.AwayMode.CreateModel model)
        {
            Models.AwayMode.CreateModel storeData = null;

            model.key = _S_DeactiveKey;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.ActionType == "get_otp")
                    {
                        if (CacheProvider.TryGet(_S_DeactiveKey, out storeData))
                        {
                            var responseCreateWithOtp = AwayModeClient.Manageawaymode(new Foundation.Integration.APIHandler.Models.Request.AwayMode.ManageAwayModeRequest()
                            {
                                beginDate = GetSubmittedFormattedDate(storeData.BeginDate),
                                endDate = GetSubmittedFormattedDate(storeData.EndDate),
                                email = storeData.Email,
                                contractAccount = storeData.ContractAccount,
                                lang = RequestLanguageCode,
                                loginType = AwayModeAnonymous,// IsLoggedIn ? AwayModeLoggedIn : AwayModeAnonymous,
                                frequency = storeData.Frequency,
                                action = AwayModeDeactivate,
                            }, Request.Segment());

                            //pre session id
                            model = storeData;
                            model.ActionType = AwayModeDeactivate;

                            if (responseCreateWithOtp != null)
                            {

                                if (responseCreateWithOtp.Succeeded)
                                {
                                    //updated session id
                                   
                                    CacheProvider.Store(_S_DeactiveKey, new CacheItem<Models.AwayMode.CreateModel>(model));
                                    return View("~/Views/Feature/Dashboard/AwayMode/OTPValidation.cshtml", model);
                                }
                                else
                                {
                                    ModelState.AddModelError("", responseCreateWithOtp.Message);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", Translate.Text("AWM_CreateSessionError"));
                        }

                    }


                    if (model.ActionType == "submit_otp")
                    {

                        if (CacheProvider.TryGet(_S_DeactiveKey, out storeData))
                        {
                            var responseOTPSubmit = AwayModeClient.Manageawaymode(new Foundation.Integration.APIHandler.Models.Request.AwayMode.ManageAwayModeRequest()
                            {

                                lang = RequestLanguageCode,
                                loginType = AwayModeAnonymous,
                                action = AwayModeOTP,
                                request = storeData.RequestId,
                                otp = model.otp
                            }, Request.Segment());

                            model = storeData;
                            model.ActionType = AwayModeDeactivate;
                            //pre session id
                            if (responseOTPSubmit != null)
                            {
                                if (responseOTPSubmit.Succeeded)
                                {
                                    //updated session id
                                    CacheProvider.Remove(_S_DeactiveKey);
                                 
                                    return View("~/Views/Feature/Dashboard/AwayMode/SuccessCreateAwayMode.cshtml", model);
                                }
                                else
                                {
                                    ModelState.AddModelError("", responseOTPSubmit.Message);

                                }
                            }
                            return View("~/Views/Feature/Dashboard/AwayMode/OTPValidation.cshtml", model);
                        }
                        else
                        {
                            ModelState.AddModelError("", Translate.Text("AWM_CreateSessionError"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            if (storeData != null)
            {
                model = storeData;
                CacheProvider.Store(_S_DeactiveKey, new CacheItem<Models.AwayMode.CreateModel>(model));
            }


            model.FrequencyList = GetDataSource(SitecoreItemIdentifiers.DTMC_AwayMode_Frequency_List);
            return View("~/Views/Feature/Dashboard/AwayMode/DeactiveAwayMode.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ResendOTP(string sn)
        {
            string msg = null;
            Models.AwayMode.CreateModel storeData = null;
            if (CacheProvider.TryGet(sn, out storeData))
            {
                var responseResendOTPSubmit = AwayModeClient.Manageawaymode(new Foundation.Integration.APIHandler.Models.Request.AwayMode.ManageAwayModeRequest()
                {
                    lang = RequestLanguageCode,
                    loginType = AwayModeAnonymous,
                    action = AwayModeResendOTP,
                    request = storeData.RequestId
                }, Request.Segment());


                if (responseResendOTPSubmit != null)
                {
                    if (responseResendOTPSubmit.Succeeded)
                    {
                        CacheProvider.Store(sn, new CacheItem<Models.AwayMode.CreateModel>(storeData));
                    }
                    else
                    {
                        msg = responseResendOTPSubmit.Message;

                    }

                }
            }
            else
            {
                msg = Translate.Text("AWM_CreateSessionError");
            }
            return Json(new { success = string.IsNullOrWhiteSpace(msg), desc = msg, data = storeData }, JsonRequestBehavior.DenyGet);
        }
        [HttpPost, ValidateAntiForgeryToken, TwoPhaseAuthorizeAttribute]
        public JsonResult GetDetail(string b)
        {
            var response = DewaApiClient.GetProfileDetails(FormatBusinessPartner(b), CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (response != null && response.Payload != null && response.Payload.profileMain != null)
            {
                return Json(new { e = response.Payload.profileMain.emailAddress }, JsonRequestBehavior.DenyGet);
            }
            return Json(new { e = "" }, JsonRequestBehavior.DenyGet);
        }


        public ActionResult GetConsumptionGraph(string r)
        {
            Models.AwayMode.ConsumptionGraphSettingModel model = new Models.AwayMode.ConsumptionGraphSettingModel(); ;

            try
            {
                model.dates = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
                model.DateJsonArray = JsonConvert.SerializeObject(model.dates);//, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented }
                var colors = GetDataSource(SitecoreItemIdentifiers.ColorList).Select(x => x.Value).ToList();

                var responseData = AwayModeClient.GetConsumptionData(r, Request.Segment());
                if (responseData.Succeeded)
                {
                    int i = 0;
                    if (responseData.Payload.electricityDataList != null && responseData.Payload.electricityDataList.Count() > 0)
                    {
                        model.ElectricityConsumptionDetail = new List<Models.AwayMode.ConsumptionDataSeries>();
                        #region [Complex code]
                        //var electricData = responseData.Payload.electricityDataList.Where(x => x != null).Select(x => new { MMyyyy = _commonUtility.DateTimeFormatParse(x.date, "dd.MM.yyyy").ToString("MMMM yyyy"), MM = _commonUtility.DateTimeFormatParse(x.date, "dd.MM.yyyy").Day, consumption = x.consumption });

                        //if (electricData != null)
                        //{
                        //    model.ElectricityConsumptionDetail = new List<Models.AwayMode.ConsumptionDataSeries>();
                        //    foreach (var monthlyData in electricData.GroupBy(x => x.MMyyyy))
                        //    {
                        //        Models.AwayMode.ConsumptionDataSeries consumptionDataSeries = new Models.AwayMode.ConsumptionDataSeries();
                        //        if (monthlyData != null)
                        //        {
                        //            i = Convert.ToInt32(i > 3 ? 0 : i);
                        //            consumptionDataSeries.data = new List<double?>();
                        //            consumptionDataSeries.name = monthlyData.FirstOrDefault().MMyyyy;
                        //            consumptionDataSeries.color = colors[i];
                        //            foreach (int date in model.dates)
                        //            {
                        //                var data = monthlyData.FirstOrDefault(x => x.MM == date)?.consumption;

                        //                if (!string.IsNullOrWhiteSpace(data))
                        //                {
                        //                    consumptionDataSeries.data.Add(Convert.ToDouble(data));
                        //                }
                        //                else
                        //                {
                        //                    consumptionDataSeries.data.Add(null);
                        //                }

                        //            }
                        //            model.ElectricityConsumptionDetail.Add(consumptionDataSeries);
                        //            i++;
                        //        }
                        //    }

                        //    model.ElectricityConsumptionDetailJsonData = JsonConvert.SerializeObject(model.ElectricityConsumptionDetail);
                        //}
                        #endregion
                        #region [Simple code]
                        model.ElectricityDates = responseData.Payload.electricityDataList.Where(x => !string.IsNullOrWhiteSpace(x.date))?.Select(x => x.date)?.ToList() ?? new List<string>();
                        model.ElectricityConsumptionDetail.Add(new Models.AwayMode.ConsumptionDataSeries()
                        {
                            data = responseData.Payload.electricityDataList.Where(x => !string.IsNullOrEmpty(x.date))?.Select(x => x.consumption)?.ToList(),
                            color = Translate.Text("electricityColor"),
                            name = " "
                        });
                        model.ElectricityConsumptionDetailJsonData = JsonConvert.SerializeObject(model.ElectricityConsumptionDetail);
                        #endregion
                    }

                    if (responseData.Payload.waterDataList != null && responseData.Payload.waterDataList.Count() > 0)
                    {
                        i = 0;
                        model.WaterConsumptionDetail = new List<Models.AwayMode.ConsumptionDataSeries>();
                        #region [Complex code]
                        //var waterData = responseData.Payload.waterDataList.Where(x => x != null).Select(x => new { MMyyyy = _commonUtility.DateTimeFormatParse(x.date, "dd.MM.yyyy").ToString("MMMM yyyy"), MM = _commonUtility.DateTimeFormatParse(x.date, "dd.MM.yyyy").Day, consumption = x.consumption });

                        //if (waterData != null)
                        //{
                        //    model.WaterConsumptionDetail = new List<Models.AwayMode.ConsumptionDataSeries>();
                        //    foreach (var monthlyData in waterData.GroupBy(x => x.MMyyyy))
                        //    {
                        //        Models.AwayMode.ConsumptionDataSeries consumptionDataSeries = new Models.AwayMode.ConsumptionDataSeries();
                        //        if (monthlyData != null)
                        //        {
                        //            i = Convert.ToInt32(i > 3 ? 0 : i);
                        //            consumptionDataSeries.data = new List<double?>();
                        //            consumptionDataSeries.name = monthlyData.FirstOrDefault().MMyyyy;
                        //            consumptionDataSeries.color = colors[i];
                        //            foreach (int date in model.dates)
                        //            {
                        //                var data = monthlyData.FirstOrDefault(x => x.MM == date)?.consumption;

                        //                if (!string.IsNullOrWhiteSpace(data))
                        //                {
                        //                    consumptionDataSeries.data.Add(Convert.ToDouble(data));
                        //                }
                        //                else
                        //                {
                        //                    consumptionDataSeries.data.Add(null);
                        //                }

                        //            }
                        //            model.WaterConsumptionDetail.Add(consumptionDataSeries);
                        //            i++;
                        //        }
                        //    }


                        //    model.WaterConsumptionDetailJsonData = JsonConvert.SerializeObject(model.WaterConsumptionDetail);
                        //}
                        #endregion

                        #region [Simple code]
                        model.WaterDates = responseData.Payload.waterDataList.Where(x => !string.IsNullOrWhiteSpace(x.date))?.Select(x => x.date)?.ToList() ?? new List<string>();
                        model.WaterConsumptionDetail.Add(new Models.AwayMode.ConsumptionDataSeries()
                        {
                            data = responseData.Payload.waterDataList.Where(x=> !string.IsNullOrEmpty(x.date))?.Select(x => x.consumption)?.ToList(),
                            color = Translate.Text("waterColor"),
                            name = " "
                        });
                        model.WaterConsumptionDetailJsonData = JsonConvert.SerializeObject(model.WaterConsumptionDetail);
                        #endregion
                    }


                }
                else
                {
                    ModelState.AddModelError("", responseData.Message);
                }
            }
            catch (Exception ex)
            {

                LogService.Error(ex, this);
                CacheProvider.Remove(r);
                ModelState.AddModelError("", ex.Message);
            }


            return View("~/Views/Feature/Dashboard/AwayMode/ConsumptionGraph.cshtml", model);
        }

        [HttpGet, TwoPhaseAuthorizeAttribute(AllowEVUsers =true)]
        public JsonResult IsSmartMeter(string ac)
        {
            string erroMsg = "";
            try
            {
                var _issueRepsonse = PremiseHandler.GetDetails(new PremiseDetailsRequest()
                {
                    PremiseDetailsIN = new PremiseDetailsIN()
                    {
                        contractaccount = ac,
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
                    return Json(new { success = true, description = _issueRepsonse.Message, data = _issueRepsonse.Payload.meter }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    erroMsg = _issueRepsonse.Message;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                erroMsg = ex.Message;
            }
            return Json(new { success = false, description = erroMsg }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [methods]
        protected List<SelectListItem> GetDataSource(string sourceID)
        {
            var item = ScContext.Database.GetItem(new ScData.ID(sourceID));
            if (item != null && item.Children != null)
            {
                return item.Children?.Select(c => new SelectListItem
                {
                    Text = c.Fields["Text"].ToString(),
                    Value = c.Fields["Value"].ToString()
                })?.ToList();
            }
            return new List<SelectListItem>();
        }

        protected string GetSubmittedFormattedDate(string date)
        {
            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    return Convert.ToDateTime(ConvertDateArToEn(date)).ToString("yyyyMMdd");
                }
            }
            catch (Exception ex)
            {

                LogService.Fatal(ex, this);
            }

            return null;
        }

        protected string GetShowFormattedDate(string date)
        {
            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    return Convert.ToDateTime(ConvertDateArToEn(date)).ToString("dd-MM-yyyy");
                }
            }
            catch (Exception ex)
            {

                LogService.Fatal(ex, this);
            }

            return null;
        }


        protected string GetBindedFormattedDate(string date)
        {
            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    string d = Convert.ToDateTime(date).ToString("dd MMMM yyy");


                    if (RequestLanguage == SupportedLanguage.Arabic)
                    {
                        d = ConvertDateEnToAr(d);
                    }

                    return d;
                }
            }
            catch (Exception ex)
            {

                LogService.Fatal(ex, this);
            }

            return null;
        }

        protected string ValidateDateRange(string fromDate, string toDate)
        {
            string ErrorMsg = "";
            try
            {
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    ErrorMsg = Convert.ToDateTime(toDate).Subtract(Convert.ToDateTime(fromDate)).Ticks > 0 ? Translate.Text("AWM_EndDateLessMessage") : "";
                }
                else
                {

                    ErrorMsg = Translate.Text("AWM_BothDateRequired");
                }
            }
            catch (Exception ex)
            {

                LogService.Fatal(ex, this);
                ErrorMsg = ex.Message;
            }

            return null;
        }

        private string ConvertDateEnToAr(string date)
        {
            if (!string.IsNullOrWhiteSpace(date))
            {
                date = date.Replace("January", "يناير").Replace("February", "فبراير").Replace("March", "مارس").Replace("April", "أبريل").Replace("May", "مايو").Replace("June", "يونيو").Replace("July", "يوليو").Replace("August", "أغسطس").Replace("September", "سبتمبر").Replace("October", "أكتوبر").Replace("November", "نوفمبر").Replace("December", "ديسمبر");
            }
            return date;
        }

        private string ConvertDateArToEn(string date)
        {
            if (!string.IsNullOrWhiteSpace(date))
            {
                date = date.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            }
            return date;
        }
        #endregion
    }
}