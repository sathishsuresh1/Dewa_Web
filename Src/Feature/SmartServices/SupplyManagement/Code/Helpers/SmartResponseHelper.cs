using DEWAXP.Foundation.Logger;
using DEWAXP.Feature.SupplyManagement.Models.SmartResponseModel;
using Glass.Mapper.Sc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using SitecoreX = Sitecore.Context;
using _sm_commonConst = DEWAXP.Feature.SupplyManagement.Models.SmartResponseModel.CommonConst;
using Sitecore.Globalization;
using System.Globalization;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Repositories;
using Glass.Mapper.Sc.Configuration.Fluent;
using Sitecore.Data;
using Sitecore.Data.Items;
using static Sitecore.Configuration.Settings;
using Glass.Mapper.Sc.Web;
using System.Web.Mvc;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartResponseModel;
using DEWAXP.Foundation.Content.Models.SmartResponseModel;

namespace DEWAXP.Feature.SupplyManagement.Helpers.SmartResponse
{
    public class SmartResponseHelper
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(SitecoreX.Database)));
        private static ICacheProvider CacheProvider = DependencyResolver.Current.GetService<ICacheProvider>();
        public static List<Question> SetQuestionAndAnswerModel(ref int settingNodeCount, List<Question> model, int trackingId, int parentAnsId = 0)
        {
            settingNodeCount = trackingId + 1;

            foreach (Question item in model)
            {
                item.TrackingId = settingNodeCount;

                if (item.InfoAnswer != null)
                {
                    item.InfoAnswer = SetInfoAnswerAndQuestionModel(ref settingNodeCount, item.InfoAnswer, item.TrackingId, parentAnsId);
                    item.Answers = SetAnswerAndQuestionModel(ref settingNodeCount, item.Answers, (item.InfoAnswer.TrackingId + 1), parentAnsId);
                }
                else
                {
                    item.Answers = SetAnswerAndQuestionModel(ref settingNodeCount, item.Answers, item.TrackingId, parentAnsId);
                }

                settingNodeCount = settingNodeCount + 1;

                //#region [translation from sc]
                //item.Value = GetSMTransaltion(item.Value);
                //item.Infotext = GetSMTransaltion(item.Infotext);
                //#endregion
            };

            return model;
        }
        public static List<Answer> SetAnswerAndQuestionModel(ref int settingNodeCount, List<Answer> model, int trackingId, int parentAnsId = 0)
        {
            settingNodeCount = trackingId + 1;

            foreach (Answer item in model)
            {
                item.disabled = IsAnsDeactive(item);
                item.TrackingId = settingNodeCount;
                item.ParentTrackingId = parentAnsId;
                //int parentAnsId = settingNodeCount;
                item.Questions = SetQuestionAndAnswerModel(ref settingNodeCount, item.Questions, item.TrackingId, item.TrackingId);
                settingNodeCount = settingNodeCount + 1;
                if (string.IsNullOrEmpty(item.BtnValue))
                {
                    item.BtnValue = item.Btntitle;
                }

                //#region [translation from sc]
                //item.Btntitle = GetSMTransaltion(item.Btntitle);
                //item.Placeholder = GetSMTransaltion(item.Placeholder);
                //#endregion

            };




            return model;
        }

        public static Answer SetInfoAnswerAndQuestionModel(ref int settingNodeCount, Answer item, int trackingId, int parentAnsId = 0)
        {
            if (item != null)
            {
                settingNodeCount = trackingId + 1;

                item.disabled = IsAnsDeactive(item);
                item.TrackingId = settingNodeCount;
                item.ParentTrackingId = parentAnsId;
                //int parentAnsId = settingNodeCount;
                item.Questions = SetQuestionAndAnswerModel(ref settingNodeCount, item.Questions, item.TrackingId, item.TrackingId);
                settingNodeCount = settingNodeCount + 1;
                if (string.IsNullOrEmpty(item.BtnValue))
                {
                    item.BtnValue = item.Btntitle;
                }
            }
            //#region [translation from sc]
            //item.Btntitle = GetSMTransaltion(item.Btntitle);
            //item.Placeholder = GetSMTransaltion(item.Placeholder);
            //#endregion




            return item;
        }


        public static CommonRender GetQuestionAndAnswerModel(List<Question> model, int trackingId, bool isGuestNotification = false)
        {
            CommonRender commonRender = new CommonRender();
            foreach (Question item in model)
            {


                if (item.TrackingId == trackingId)
                {
                    //#region [translation from sc]
                    //item.Value = GetSMTransaltion(item.Value);
                    //item.Infotext = GetSMTransaltion(item.Infotext);
                    //#endregion

                    commonRender.Question = item;
                    return commonRender;
                }
                //if (isGuestNotification && item.Answers?.Where(x => x.Action == SM_Action.GuestGetNotificationList).Count() > 0)
                //{
                //    commonRender.Question = item;
                //    return commonRender;
                //}

                if (commonRender.Answer == null && commonRender.Question == null)
                {
                    var d = GetAnswerAndQuestionModel(item.Answers, trackingId, isGuestNotification);
                    commonRender.Answer = d.Answer;

                    if (commonRender.Answer != null)
                    {
                        commonRender.Answer.disabled = IsAnsDeactive(commonRender.Answer);
                    }
                    commonRender.Question = d.Question;
                    if (commonRender.Answer != null || commonRender.Question != null)
                    {
                        if (commonRender.Question == null)
                        {
                            commonRender.Question = item;
                        }
                        return commonRender;
                    }
                    else if (item.InfoAnswer != null)
                    {
                        //mapping InfoAnswer - Nested Qustion
                        var infoD = GetInfoAnswerAndQuestionModel(item.InfoAnswer, trackingId, isGuestNotification);
                        commonRender.Answer = infoD.Answer;
                        if (commonRender.Answer != null)
                        {
                            commonRender.Answer.disabled = IsAnsDeactive(commonRender.Answer);
                        }
                        commonRender.Question = infoD.Question;
                        if (commonRender.Answer != null || commonRender.Question != null)
                        {
                            if (commonRender.Question == null)
                            {
                                commonRender.Question = item;
                            }
                            return commonRender;
                        }
                    }

                }
            };
            return commonRender;
        }
        public static CommonRender GetAnswerAndQuestionModel(List<Answer> model, int trackingId, bool isGuestNotification = false)
        {
            CommonRender commonRender = new CommonRender();
            foreach (Answer item in model)
            {
                item.disabled = IsAnsDeactive(item);
                //#region [translation from sc]
                //item.Btntitle = GetSMTransaltion(item.Btntitle);
                //item.Placeholder = GetSMTransaltion(item.Placeholder);
                //#endregion
                if (item.TrackingId == trackingId)
                {
                    commonRender.Answer = item;
                    return commonRender;
                }
                if (isGuestNotification && item.Action == SM_Action.GuestGetNotificationList)
                {
                    commonRender.Answer = item;
                    return commonRender;
                }

                if (commonRender.Answer == null && commonRender.Question == null)
                {
                    var d = GetQuestionAndAnswerModel(item.Questions, trackingId, isGuestNotification);
                    commonRender.Question = d.Question;
                    commonRender.Answer = d.Answer;
                    if (commonRender.Answer != null || commonRender.Question != null)
                    {
                        return commonRender;
                    }
                }
            };
            return commonRender;
        }


        public static CommonRender GetInfoAnswerAndQuestionModel(Answer item, int trackingId, bool isGuestNotification = false)
        {
            CommonRender commonRender = new CommonRender();
            if (item != null)
            {
                item.disabled = IsAnsDeactive(item);
                //#region [translation from sc]
                //item.Btntitle = GetSMTransaltion(item.Btntitle);
                //item.Placeholder = GetSMTransaltion(item.Placeholder);
                //#endregion
                if (item.TrackingId == trackingId)
                {
                    commonRender.Answer = item;
                    return commonRender;
                }
                if (isGuestNotification && item.Action == SM_Action.GuestGetNotificationList)
                {
                    commonRender.Answer = item;
                    return commonRender;
                }

                if (commonRender.Answer == null && commonRender.Question == null)
                {
                    var d = GetQuestionAndAnswerModel(item.Questions, trackingId, isGuestNotification);
                    commonRender.Question = d.Question;
                    commonRender.Answer = d.Answer;
                    if (commonRender.Answer != null || commonRender.Question != null)
                    {
                        return commonRender;
                    }
                }
            };
            return commonRender;
        }


        public static CommonRender GetRenderModel(CommonRenderRequest request, JsonMasterModel jsonModel)
        {

            CommonRender data = new CommonRender();
            if (request.TckId > 1)
            {
                data = GetQuestionAndAnswerModel(jsonModel.Questions, request.TckId);
                data.IsQuestion = data.Question != null;
                data.PreviousAnsTrackingId = request.TckId;
            }
            else
            {
                data.Answer = new Answer()
                {
                    Questions = jsonModel.Questions,
                };

                SmartResponseSessionHelper.UserSelectedIdAnswer = null;
            }

            //validate if  user loggedin session required.

            #region [mofification]
            SetUserSelectedAnsTypeAndValue(SM_Id.LangType, request.LangType.ToString());
            #region trcodegroup
            if (data.Answer != null && data.Answer.Id == SM_Id.trcodegroup)
            {
                if (request == null)
                {
                    request.SelectedValues = new List<SmartDataValue>();
                    request.SelectedValues.Add(new SmartDataValue()
                    {
                        Type = SM_Id.trcodegroup,
                        Value = data.Answer.BtnValue
                    });
                }
                if (request.SelectedValues != null && !string.IsNullOrWhiteSpace(data.Answer.BtnValue2))
                {
                    request.SelectedValues.Add(new SmartDataValue()
                    {
                        Type = SM_Id.trcode,
                        Value = data.Answer.BtnValue2
                    });
                }

            }

            #endregion

            #region codegroup
            if (data.Answer != null && data.Answer.Id == SM_Id.codegroup)
            {
                var d = SmartResponseHelper.GetUserSubmittedData();
                if (request == null)
                {
                    request.SelectedValues = new List<SmartDataValue>();
                    request.SelectedValues.Add(new SmartDataValue()
                    {
                        Type = SM_Id.codegroup,
                        Value = data.Answer.BtnValue
                    });
                }

                if (request.SelectedValues != null && !string.IsNullOrEmpty(data.Answer.BtnValue2) && string.IsNullOrEmpty(d.Code))
                {
                    request.SelectedValues.Add(new SmartDataValue()
                    {
                        Type = SM_Id.code,
                        Value = data.Answer.BtnValue2
                    });
                }

            }
            #endregion


            #region [Fileter By Code]
            if (request.FilterCode != SM_Code.Empty)
            {
                data.Answer.Questions = data.Answer.Questions.Where(x => x.Code == request.FilterCode).ToList();
            }
            #endregion
            #endregion



            if (request.SelectedValues != null && request.SelectedValues.Count() > 0)
            {

                foreach (var item in request.SelectedValues)
                {
                    if (data.Answer.Actiondata != "forgotaccount" || data.Answer.Actiondata == "forgotaccount" && item.Type != SM_Id.Account)
                    {
                        SetUserSelectedAnsTypeAndValue(item.Type, item.Value);
                    }
                }

            }


            if (data.ErrorDetails == null)
            {
                data.ErrorDetails = new List<SM_ErrorDetail>();
            }

            return data;

        }
        public static int Percentageprevioustrackid(JsonMasterModel jsonModel, int TckId)
        {
            var data = GetQuestionAndAnswerModel(jsonModel.Questions, TckId);
            return data.Answer.ParentTrackingId;
        }

        public static CommonRender GetGuestNotiicationListModel(string notificationNo, int backTrackingId)
        {
            CommonRender data = null; ;
            if (!string.IsNullOrWhiteSpace(notificationNo))
            {
                data = GetQuestionAndAnswerModel(SmartResponseSessionHelper.ElectricityComplaintJsonSetting.Questions, 0, true);
                data.IsQuestion = data.Question != null;
                data.PreviousAnsTrackingId = backTrackingId;

                if (data != null && data.Answer.Action == SM_Action.GuestGetNotificationList)
                {
                    var qus = data.Answer.Questions?.FirstOrDefault();
                    if (qus != null)
                    {
                        SetUserSelectedAnsTypeAndValue(SM_Id.NotificationNumber, notificationNo);
                        //qus.Value = qus.Value?.Replace("{{notif}}", TextHighlight(notificationNo));
                        var ans = qus?.Answers.Where(x => x.Action == SM_Action.GetNotificationList).FirstOrDefault();
                        ans.Actiondata = notificationNo;
                    }

                    data.Answer.ParentTrackingId = backTrackingId;
                }

            }
            //validate if  user loggedin session required.
            return data;

        }

        public static CommonRender GetReportTechnicalIncident(JsonMasterModel jsonMasterModel)
        {
            CommonRender data = new CommonRender(); ;
            data.Answer = jsonMasterModel.Questions.FirstOrDefault().Answers.FirstOrDefault();
            if (data.ErrorDetails == null)
            {
                data.ErrorDetails = new List<SM_ErrorDetail>();
            }

            return data;
        }

        public static CommonRender GetTrackOtherIncident(JsonMasterModel jsonMasterModel)
        {
            CommonRender data = new CommonRender(); ;
            var ans = jsonMasterModel.Questions.FirstOrDefault()?.Answers.LastOrDefault()?.Questions?.LastOrDefault()?.Answers.LastOrDefault();
            if (ans != null)
            {
                data.Answer = ans;
            }

            if (data.ErrorDetails == null)
            {
                data.ErrorDetails = new List<SM_ErrorDetail>();
            }
            return data;
        }

        public static Models.SmartResponseModel.JsonMasterModel LoadUpdatedSmartDubaiModelJson()
        {

            Models.SmartResponseModel.JsonMasterModel sr = CustomFileUtility.LoadSmartDubaiModelJson(SmartResponseCofig.SMART_RESPONSE_JSON_PATH, true);
            int settingnodeCount = 0;
            sr.Questions = SmartResponseHelper.SetQuestionAndAnswerModel(ref settingnodeCount, sr.Questions, 0);
            return sr;
        }
        /// <summary>
        /// set or update the selected answer value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ans"></param>
        /// <returns></returns>
        public static bool SetUserSelectedAnsTypeAndValue(SM_Id type, string ans)
        {
            try
            {

                if (SmartResponseSessionHelper.UserSelectedIdAnswer == null)
                {
                    SmartResponseSessionHelper.UserSelectedIdAnswer = new Dictionary<SM_Id, string>();
                }

                if (SmartResponseSessionHelper.UserSelectedIdAnswer.Where(x => x.Key == type).Count() > 0)
                {
                    SmartResponseSessionHelper.UserSelectedIdAnswer[type] = ans;
                }
                else
                {
                    SmartResponseSessionHelper.UserSelectedIdAnswer.Add(type, ans);
                }

                return true;
            }
            catch (Exception ex)
            {

                LogService.Error(ex, null);
            }

            return false;
        }
        /// <summary>
        /// Get Selected Answer Value by 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSelectedAnswerValue(SM_Id type)
        {
            if (SmartResponseSessionHelper.UserSelectedIdAnswer != null && SmartResponseSessionHelper.UserSelectedIdAnswer.Where(x => x.Key == type).Count() > 0)
            {
                return SmartResponseSessionHelper.UserSelectedIdAnswer[type];
            }
            return "";
        }

        public static bool Clear_SM_Session()
        {

            try
            {
                SmartResponseSessionHelper.CurrentUserAnswer = null;
                SmartResponseSessionHelper.ElectricityComplaintJsonSetting = null;
                SmartResponseSessionHelper.UserSelectedIdAnswer = null;
                SmartResponseSessionHelper.WaterComplaintJsonSetting = null;
                return true;
            }
            catch (Exception ex)
            {

                LogService.Error(ex, null);
            }

            return false;
        }


        public static Dictionary<string, string> GetValidationBySM_IdType(SM_Id smId)
        {
            Dictionary<string, string> validtionList = new Dictionary<string, string>();
            Dictionary<string, string> validations = new Dictionary<string, string>();

            string ErrorMessage = GetSMTranslation("Enter valid details.");
            string HtmlValidationRule = "";
            string ElementId = $"{smId.ToString()}-{Guid.NewGuid()}";
            string ElementErrorContainerId = ElementId + "ErrorContainer";
            string ElementType = "text";

            //type = "number" lang = "en" -
            //name = "ContractAccountNumber" -
            //id = "form-field-ca-number"  -
            //class="form-field__input form-field__input--text"
            //required=""
            //aria-required="true"
            //aria-describedby="description-for-ca-number"
            //placeholder="@Translate.Text(DictionaryKeys.Global.Account.ContractAccountNumber)"
            //data-parsley-errors-container="#description-for-ca-number"
            //data-parsley-error-message="@Translate.Text(DictionaryKeys.BuildingAudit.ContractAccountNumberValidationMessage)"
            //data-parsley-trigger="focusout"
            //data-parsley-account_number=""


            if (smId != SM_Id.More)
            {
                validations.Add("required", "");
                validations.Add("aria-required", "true");
            }

            if (smId == SM_Id.Location)
            {
                ErrorMessage = GetSMTranslation("Please add valid Location.");
            }

            if (smId == SM_Id.Mobile)
            {

                validations.Add("data-parsley-mobile_number", "");
                validations.Add("data-parsley-mobile_number-message", ErrorMessage);
                ElementType = "tel";
                validations.Add("leng", "en");
                validations.Add("maxlength", "9");
                ErrorMessage = GetSMTranslation("Please enter valid Mobile No.");// Translate.Text("SM_Error_MobileValidationMessage"); 
            }
            if (smId == SM_Id.Account)
            {
                validations.Add("data-parsley-account_number", "");
                ElementType = "number";
                validations.Add("leng", "en");
                validations.Add("data-parsley-maxnumber", "10");
                validations.Add("maxlength", "10");
                validations.Add("data-parsley-account_premise_number", "");
                validations.Add("pattern", "[0-9]{9,10}");
                validations.Add("data-parsley-pattern", "[0-9]{9,10}");
                ErrorMessage = GetSMTranslation("Please enter valid account no.");
            }
            if (smId == SM_Id.Media)
            {

            }
            if (smId == SM_Id.ContactPersonName)
            {
                validations.Add("data-parsley-name", "");
                validations.Add("maxlength", "50");
                validations.Add("data-parsley-maxlength", "50");
                validations.Add("data-parsley-length", "[2,50]");
                ErrorMessage = GetSMTranslation("Please enter valid Contact Name.");

            }
            if (smId == SM_Id.Otp)
            {
                validations.Add("data-parsley-maxnumber", "6");
                ElementType = "number";
                ErrorMessage = GetSMTranslation("Please enter Valid OTP.");
            }
            if (smId == SM_Id.More)
            {

                validations.Add("maxlength", "255");
                validations.Add("data-parsley-maxlength", "255");
                ErrorMessage = GetSMTranslation("Please enter valid Descriptions.");
            }

            if (smId == SM_Id.NotificationNumber || smId == SM_Id.SearchTxt)
            {
                string maxlength = "12";
                validations.Add("maxlength", maxlength);
                validations.Add("data-parsley-maxlength", "12");
                ErrorMessage = GetSMTranslation("Please enter valid Notification No.");
                //oninput = "this.value = this.value.replace(/[^0-9.]/g, ''); this.value = this.value.replace(/(\..*)\./g, '$1');" onKeyDown = "if(this.value.length==10 && event.keyCode!=8) return false;"
                validations.Add("oninput", @"this.value = this.value.replace(/[^0-9.]/g, ''); this.value = this.value.replace(/(\..*)\./g, '$1');");
                validations.Add("onKeyDown", $"if(this.value.length=={maxlength} && event.keyCode!=8) return false;");
            }
            validations.Add("Type", ElementType);
            validations.Add("id", ElementId);
            validations.Add("name", ElementId);
            validations.Add("data-parsley-error-message", ErrorMessage);
            validations.Add("aria-describedby", ElementErrorContainerId);
            validations.Add("data-parsley-errors-container", "#" + ElementErrorContainerId);

            HtmlValidationRule = string.Join(" ", validations.ToList().Select(x => $"{x.Key}=\"{x.Value}\""));

            validtionList.Add(_sm_commonConst.ErrorMessage, ErrorMessage);
            validtionList.Add(_sm_commonConst.ElementId, ElementId);
            validtionList.Add(_sm_commonConst.ElementErrorContainerId, ElementErrorContainerId);
            validtionList.Add(_sm_commonConst.ValidationRule, HtmlValidationRule);

            return validtionList;

        }

        public static string TextHighlight(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = $"<span class='j120-smart-response--blue1'>{text}</span>";

            }
            return text;
        }

        public static string MobileNineFormat(string text)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    text = Convert.ToInt32(text).ToString("000000000");

                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, null);
            }

            return text;
        }

        public static string MobileTenFormat(string text)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    text = Convert.ToInt32(text).ToString("0000000000");

                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, null);
            }

            return text;
        }

        public static CustomerSubmittedData GetUserSubmittedData()
        {
            CustomerSubmittedData d = new CustomerSubmittedData();
            d.Location = GetSelectedAnswerValue(SM_Id.Location);
            d.Mobile = MobileNineFormat(GetSelectedAnswerValue(SM_Id.Mobile));

            if (!string.IsNullOrWhiteSpace(d.Mobile))
            {
                SetUserSelectedAnsTypeAndValue(SM_Id.Mobile, d.Mobile);
            }

            d.ContractAccountNo = GetSelectedAnswerValue(SM_Id.Account);
            d.Success = GetSelectedAnswerValue(SM_Id.Success);
            d.Media = GetSelectedAnswerValue(SM_Id.Media);
            d.ContactPersonName = GetSelectedAnswerValue(SM_Id.ContactPersonName);
            d.MobileOtp = GetSelectedAnswerValue(SM_Id.Otp);
            d.SessionType = GetSelectedAnswerValue(SM_Id.SessionType);
            d.ComplaintId = GetSelectedAnswerValue(SM_Id.ComplaintId);
            d.CustomerType = GetSelectedAnswerValue(SM_Id.customertype);
            d.CustomerCategory = GetSelectedAnswerValue(SM_Id.customercategory);
            if (string.IsNullOrEmpty(d.CustomerCategory))
            {
                d.CustomerCategory = _sm_commonConst.CusSubTyp_Normal;
            }
            d.CodeGroup = GetSelectedAnswerValue(SM_Id.codegroup);
            d.Code = GetSelectedAnswerValue(SM_Id.code);
            d.TrCodeGroup = GetSelectedAnswerValue(SM_Id.trcodegroup);
            d.TrCode = GetSelectedAnswerValue(SM_Id.trcode);
            d.Latitude = GetSelectedAnswerValue(SM_Id.Latitude);
            d.Longitude = GetSelectedAnswerValue(SM_Id.Longitude);
            d.OtpCount = GetSelectedAnswerValue(SM_Id.OtpCount);
            d.ElectricityMeterType = GetSelectedAnswerValue(SM_Id.ElectricityMeterType);
            d.ElectricityIsSmartMeter = GetSelectedAnswerValue(SM_Id.ElectricityIsSmartMeter);
            d.WaterMeterType = GetSelectedAnswerValue(SM_Id.WaterMeterType);
            d.WaterIsSmartMeter = GetSelectedAnswerValue(SM_Id.WaterIsSmartMeter);
            d.Street = GetSelectedAnswerValue(SM_Id.Street);
            d.CA_Location = GetSelectedAnswerValue(SM_Id.CA_Location);
            d.WaterMeterNo = GetSelectedAnswerValue(SM_Id.WaterMeterNo);
            d.ElectricityMeterNo = GetSelectedAnswerValue(SM_Id.ElectricityMeterNo);
            d.CheckSmartMeter = Convert.ToBoolean(GetSelectedAnswerValue(SM_Id.CheckSmartMeter) == "1");
            d.NotificationNumber = GetSelectedAnswerValue(SM_Id.NotificationNumber);
            d.Image1 = GetSelectedAnswerValue(SM_Id.Image1) ?? "";
            d.Image2 = GetSelectedAnswerValue(SM_Id.Image2) ?? "";
            //d.Image1Preview = GetSelectedAnswerValue(SM_Id.Image1Preivew) ?? "";
            d.MoreDescription = GetSelectedAnswerValue(SM_Id.More);
            d.LangType = GetSMLangType(GetSelectedAnswerValue(SM_Id.LangType));
            d.IsUserLogined = Convert.ToBoolean(GetSelectedAnswerValue(SM_Id.IsUserLoggedIn)=="1");
            d.PartnerNo = Convert.ToString(GetSelectedAnswerValue(SM_Id.PartnerId));
            //if (string.IsNullOrEmpty(d.ParentActionType))
            //{
            //    d.ParentActionType = GetParentActionType(d);
            //}
            d.SmartMeterLoader = GetSelectedAnswerValue(SM_Id.SmartMeterLoader);
            return d;
        }


        public static string GetParentActionType(CustomerSubmittedData d)
        {
            if (string.IsNullOrEmpty(d.TrCodeGroup))
            {
                if (d.CodeGroup == _sm_commonConst.PARENT_INCIDENT_ELECTRICITY)
                {
                    if (d.Code == _sm_commonConst.INCIDENT_FIRE)
                    {

                    }

                    if (d.Code == _sm_commonConst.INCIDENT_NOPOWER_POD)
                    {
                    }
                    if (d.Code == _sm_commonConst.INCIDENT_SPARK_SHOCK_SMOKE_SMELL)
                    {
                        return _sm_commonConst.TRCODE_GP_SparkShockSmokeSmell;
                    }
                    if (d.Code == _sm_commonConst.INCIDENT_ELECTRICITY_FLUCTUATION)
                    {
                        return _sm_commonConst.TRCODE_GP_Electricityluctuation;
                    }

                }
                if (d.CodeGroup == _sm_commonConst.PARENT_INCIDENT_WATER)
                {

                }
                if (d.CodeGroup == _sm_commonConst.PARENT_INCIDENT_WATER)
                {

                }
            }

            return d.TrCodeGroup;
        }

        public static string GetErrorMsgByErrorId(SM_Id errorId)
        {
            string ErrorMsg = GetSMTranslation("Enter valid details.");
            switch (errorId)
            {
                case SM_Id.Empty:
                    break;
                case SM_Id.Location:
                    ErrorMsg = GetSMTranslation("Please add valid Location.");
                    break;
                case SM_Id.Mobile:
                    ErrorMsg = GetSMTranslation("Please enter valid Mobile No.");
                    break;
                case SM_Id.Account:
                    //ErrorMsg = $"{ErrorMsg} { errorId.ToString()}";
                    ErrorMsg = GetSMTranslation("No Account Exist");
                    break;
                case SM_Id.Success:
                    break;
                case SM_Id.Media:
                    break;
                case SM_Id.Error:
                    break;
                case SM_Id.ContactPersonName:
                    break;
                case SM_Id.More:
                    break;
                case SM_Id.Otp:
                    ErrorMsg = GetSMTranslation("Please enter Valid OTP.");
                    break;
                case SM_Id.SessionType:
                    break;
                case SM_Id.ComplaintId:
                    ErrorMsg = GetSMTranslation("Unable to create notification. Please try again.");
                    break;
                case SM_Id.customertype:
                    break;
                case SM_Id.customercategory:
                    break;
                case SM_Id.codegroup:
                    break;
                case SM_Id.code:
                    break;
                case SM_Id.trcodegroup:
                    break;
                case SM_Id.trcode:
                    break;
                case SM_Id.Latitude:
                    break;
                case SM_Id.Longitude:
                    break;
                case SM_Id.OtpCount:
                    ErrorMsg = GetSMTranslation("You can only generate OTP for 3 time.");
                    break;
                case SM_Id.ElectricityMeterType:
                    break;
                case SM_Id.ElectricityIsSmartMeter:
                    break;
                case SM_Id.WaterMeterType:
                    break;
                case SM_Id.WaterIsSmartMeter:
                    break;
                case SM_Id.Street:
                    break;
                case SM_Id.CA_Location:
                    break;
                case SM_Id.SearchTxt:
                    break;
                case SM_Id.NotificationNumber:
                    break;
                case SM_Id.WaterMeterNo:
                    break;
                default:
                    break;
            }
            //ErrorMsg = $"{ErrorMsg} { errorId.ToString()}";
            return ErrorMsg;
        }

        public static bool IsAnsDeactive(Answer ans)
        {
            if (ans.Action == SM_Action.UploadMedia && ans.Actiondata == "CAMERA")
            {
                return true;
            }

            if (ans.Action == SM_Action.UploadMedia_ssd && ans.Actiondata == "CAMERA")
            {
                return true;
            }
            return Convert.ToBoolean(ans.disabled);

        }
        //public static PredictState GetPredict(string imagePath, string imageName)
        //{
        //    PredictState response = new PredictState()
        //    {
        //        boxes = new List<List<double>>(),
        //    };
        //    if (System.IO.File.Exists(imagePath))
        //    {
        //        byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
        //        string base64ImageRepresentation = Convert.ToBase64String(imageArray);

        //        //var client = new RestClient(SmartResponseCofig.SM_PREDICT_API);
        //        //var request = new RestRequest(Method.POST);
        //        //request.AddHeader("apikey", SmartResponseCofig.SM_PREDICT_APIKEY);
        //        //request.AddHeader("Content-Type", "multipart/form-data");
        //        //request.AddFile("file", imagePath, "file"); ;
        //        //IRestResponse<PredictState> res = client.Execute<PredictState>(request);
        //        //if (res.StatusCode == System.Net.HttpStatusCode.OK && res.Data != null)
        //        //{
        //        //    response = res.Data;
        //        //}
        //        //response.image = base64ImageRepresentation;
        //        response.prev = System.IO.Path.Combine(SmartResponseCofig.SMART_RESPONSE_UPLOADPATH, imageName).TrimStart('~');
        //    }

        //    return response;
        //}
        public static System.Drawing.Image FixImageOrientation(System.Drawing.Image srce)
        {
            const int ExifOrientationId = 0x112;
            // Read orientation tag
            if (!srce.PropertyIdList.Contains(ExifOrientationId)) return srce;
            var prop = srce.GetPropertyItem(ExifOrientationId);
            var orient = BitConverter.ToInt16(prop.Value, 0);
            // Force value to 1
            prop.Value = BitConverter.GetBytes((short)1);
            srce.SetPropertyItem(prop);

            // Rotate/flip image according to <orient>
            switch (orient)
            {
                case 1:
                    srce.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    return srce;


                case 2:
                    srce.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    return srce;

                case 3:
                    srce.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    return srce;

                case 4:
                    srce.RotateFlip(RotateFlipType.Rotate180FlipX);
                    return srce;

                case 5:
                    srce.RotateFlip(RotateFlipType.Rotate90FlipX);
                    return srce;

                case 6:
                    srce.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    return srce;

                case 7:
                    srce.RotateFlip(RotateFlipType.Rotate270FlipX);
                    return srce;

                case 8:
                    srce.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    return srce;

                default:
                    srce.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    return srce;
            }
        }

        public static SmlangCode GetSMLangType(string lang)
        {
            SmlangCode code;
            try
            {
                if (!string.IsNullOrWhiteSpace(lang))
                {
                    Enum.TryParse(lang, out code);

                }
                else
                {
                    Language l;

                    Language.TryParse(lang, out l);
                    if (l.ToString() == "ar-ae")
                    {
                        code = SmlangCode.ar;
                    }
                    else
                    {
                        code = SmlangCode.en;
                    }
                }
            }
            catch (Exception)
            {
                code = SmlangCode.en;
            }

            return code;
        }


        /// <summary>
        /// Smart response multiple translation
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSMTranslation(string key)
        {
            try
            {
                //Controllers.Api.ContentApiController obj = new Controllers.Api.ContentApiController();
                SmlangCode code;
                code = GetSMLangType(GetSelectedAnswerValue(SM_Id.LangType));
                return GetSmTranslation(key, code);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, new object());
            }
            return key;


        }

        public static string GetSMTranslation(string key, SmlangCode? langCode = SmlangCode.en)
        {
            try
            {
                //Controllers.Api.ContentApiController obj = new Controllers.Api.ContentApiController();
                SmlangCode code;
                if (!langCode.HasValue)
                {
                    code = GetSMLangType(GetSelectedAnswerValue(SM_Id.LangType));
                }
                else
                {
                    code = langCode.Value;
                }
                return GetSmTranslation(key, code);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, new object());
            }
            return key;


        }

        internal static string GetSmTranslation(string key, SmlangCode lang = SmlangCode.en)
        {
            string traslationValue = key;

            if (string.IsNullOrWhiteSpace(key) || lang == SmlangCode.en)
            {
                return key ?? "";
            }
            #region [Fetch Translation Value]
            var item = GetSmSitecoreTranslation();
            if (item != null)
            {
                var scItem = item.FirstOrDefault(x => x != null &&
                                                x.KeyEnglish.Replace("\\n", "").Trim().ToLower() == key?.Replace("\n", "").Trim().ToLower());

                if (scItem != null)
                {


                    switch (lang)
                    {
                        case SmlangCode.ar:
                            traslationValue = scItem.Arabic;
                            break;
                        case SmlangCode.zh:
                            traslationValue = scItem.Chinese;
                            break;
                        case SmlangCode.ur:
                            traslationValue = scItem.Urdu;
                            break;
                        case SmlangCode.tl:
                            traslationValue = scItem.Philippines;
                            break;
                        case SmlangCode.en:
                            traslationValue = scItem.KeyEnglish;
                            break;
                        default:
                            traslationValue = key;
                            break;
                    }
                }

            }
            #endregion
            return !string.IsNullOrWhiteSpace(traslationValue) ? traslationValue : (key ?? "");

        }
        internal static List<SmartResposeDictionary> GetSmSitecoreTranslation()
        {
            List<SmartResposeDictionary> smTranslations = new List<SmartResposeDictionary>();
            Item smTranslationScItem = null;
            CacheProvider.TryGet(CacheKeys.SM_SC_TRANSLATION_ITEMS, out smTranslations);
            if (smTranslations == null)
            {
                smTranslations = new List<SmartResposeDictionary>();
                //var sitecoreService = new SitecoreContext();
                SetContextLanguage("en");
                smTranslationScItem = Sitecore.Context.Database.GetItem(new ID("{C13C2AE8-6422-4EE6-9CEA-A7711495BF56}"));
                foreach (Item scItem in smTranslationScItem.Children.ToList<Item>())
                {
                    var t = _contentRepository.GetItem<SmartResposeDictionary>(new GetItemByItemOptions(scItem));
                    smTranslations.Add(t);
                }
                CacheProvider.Store(CacheKeys.SM_SC_TRANSLATION_ITEMS, new CacheItem<List<SmartResposeDictionary>>(smTranslations, TimeSpan.FromHours(1)));
            }
            return smTranslations;
        }
        private static void SetContextLanguage(string lang)
        {
            SitecoreX.Language = Language.Parse("en");
            if (!string.IsNullOrEmpty(lang))
            {
                if (lang.ToLower() == "ar")
                {
                    SitecoreX.Language = Language.Parse("ar-AE");
                }
            }
        }

        //TODO:sm-- need to update  -- DONE
        public static SM_SessionType Get_SM_SessionType()
        {
            CustomerSubmittedData smDetail = GetUserSubmittedData();

            if (smDetail.IsUserLogined && (smDetail.CustomerType == Models.SmartResponseModel.CommonConst.SmUserTypeMyself))
            {
                return SM_SessionType.IsLoggedIn;
            }
            return SM_SessionType.IsGuest;
        }

        public static DateTime getCultureDate(string strDate)
        {
            CultureInfo culture;
            DateTimeStyles styles;


            culture = SitecoreX.Culture;
            if (culture.ToString().Equals("ar-AE"))
            {
                strDate = strDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            }
            styles = DateTimeStyles.None;
            DateTime dateResult;
            DateTime.TryParse(strDate, culture, styles, out dateResult);
            return dateResult;
        }
    }
}