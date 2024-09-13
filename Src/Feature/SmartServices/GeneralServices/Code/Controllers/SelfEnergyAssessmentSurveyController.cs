using DEWAXP.Feature.GeneralServices.Models.SelfEnergyAssessmentSurvey;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SelfEnergyAssessmentSurvey;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ResponseModels = DEWAXP.Foundation.Integration.APIHandler.Models.Response.SelfEnergyAssessmentSurvey;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    [TwoPhaseAuthorize]
    public class SelfEnergyAssessmentSurveyController : BaseController
    {
        private DEWAXP.Foundation.Integration.APIHandler.Clients.ISelfEnergyAssessmentSurveyClient restClient;

        private const string ExistingSurvey = "ES11";

        private const string ViewPath = "~/Views/Feature/GeneralServices/SelfEnergyAssessmentSurvey/{0}.cshtml";
        private const string ViewName = "SurveyMaster";
        private const string EncodePhrase = "OnCeUpOnATime";
        private const string SelectedAccountKey = "SAK001";
        private const string RESULT_JSON_MAIN = "{{\"name\":\"{0}\", \"data\":[{1}]}}";
        private const string RESULT_JSON_SUB = "{{\"name\":\"{0}\", \"color\":\"#{1}\", \"y\":\"{2}\"}}";

        public SelfEnergyAssessmentSurveyController()
        {
            this.restClient = new DEWAXP.Foundation.Integration.APIHandler.Impl.SelfEnergyAssessmentSurveyClient();
        }

        #region Actions

        public ActionResult LandingPage(string a)
        {
            if (!string.IsNullOrEmpty(a)) { SelectedAccount = HttpUtility.UrlEncode(a); }

            LandingPageViewModel model = new LandingPageViewModel() { Downloads = new List<KeyValuePair<string, string>>(), IsStarted = false, PercentageDone = 0 };
            //try
            //{
            //    var res = restClient.GetSavedAnswers(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, SelectedAccount, Request.Segment(), RequestLanguage);
            //    if (res != null)
            //    {
            //        if (res?.Payload != null && res.Payload.Answerlist != null && res.Payload.Answerlist.Count > 0)
            //        {
            //            if (res.Payload.Answerlist.Count > 0)
            //            {
            //                int percent = 0;
            //                int.TryParse(res.Payload.Percentage, out percent);
            //                model.PercentageDone = percent; model.IsStarted = true;
            //            }
            //        }
            //        var res1 = restClient.GetSubmittedSurveys(CurrentPrincipal.SessionToken, CurrentPrincipal.Username,!string.IsNullOrWhiteSpace(SelectedAccount)? SelectedAccount: CurrentPrincipal.PrimaryAccount, Request.Segment(), RequestLanguage);
            //        if (res1.Succeeded && res1?.Payload?.Versiondatelist?.Count > 0)
            //        {
            //            foreach (var d in res1.Payload.Versiondatelist)
            //            {
            //                model.Downloads.Add(new KeyValuePair<string, string>(d.Createddate, GetEncodedString(d.Contractaccountnumber, d.Createddate, d.Version)));
            //            }
            //            //model.Downloads = res1.Payload.Versiondatelist.ToDictionary(x => x.Createddate,  x=> GetEncodedString(x.Contractaccountnumber,x.Createddate, x.Version));
            //        }
            //        //SelectedAccount = CurrentPrincipal.PrimaryAccount;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogService.Error(ex, this);
            //}

            return View(string.Format(ViewPath, "LandingPage"), model);
        }

        //[HttpPost, ValidateAntiForgeryToken]
        public ActionResult StartSurvey(string a)
        {
            if (string.IsNullOrEmpty(a))
            {
                return View(string.Format(ViewPath, "_SuccessMessage"), new MessageViewModel() { Title = "Invalid Account!", Description = "Invalid User account provided or user data missing." });
            }

            Survey model;

            SelectedAccount = HttpUtility.UrlEncode(a); int percentCompleted = 0;
            model = new Survey() { Stage = Stage.One, Answers = new List<Answerlist>() };

            try
            {
                var res1 = restClient.GetSavedAnswers(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, SelectedAccount, Request.Segment(), RequestLanguage);
                var savedAnswersModel = res1.Payload?.Answerlist;
                if (res1 != null && !string.IsNullOrEmpty(res1.Payload?.Percentage)) { percentCompleted = Convert.ToInt32(res1.Payload.Percentage); }
                var res = restClient.GetQuestionsAndAnswers(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, SelectedAccount, Request.Segment(), RequestLanguage);

                if (res.Payload != null && res.Payload.Sectionlist.Count > 0)
                {
                    model.Sections = new List<Section>(); int counter = 1;

                    foreach (var section in res.Payload.Sectionlist)
                    {
                        var s = new Section() { Name = section.Section, Description = section.Description, Type = section.SectionType.SectionTypeFromString(), Questions = new List<SurveyQuestionViewModel>() };
                        foreach (var sl in section.Subsectionlist.FirstOrDefault().Questionlist)
                        {
                            SurveyQuestionViewModel qvm = new SurveyQuestionViewModel() { Counter = counter++, Description = sl.Description, Number = int.Parse(sl.Questionnumber), Type = sl.Type.QuestionTypeFromString(), ParentSection = s.Name, Answers = new List<SurveyAnswerViewModel>(), Max = string.IsNullOrEmpty(sl.Maximum) ? 0 : Convert.ToInt32(sl.Maximum), Min = string.IsNullOrEmpty(sl.Minimum) ? 0 : Convert.ToInt32(sl.Minimum), MaxSelection = sl.Unit == null ? 0 : Convert.ToInt32(sl.Unit) };

                            var thisAnswers = GetAnswers(sl.Answerlist, res.Payload.Dynamicquestionlist, qvm, savedAnswersModel);

                            if (thisAnswers != null && thisAnswers.Count > 0) qvm.Answers.AddRange(thisAnswers);

                            if (savedAnswersModel != null)
                            {
                                var ad = savedAnswersModel.Where(x => x.Questionnumber.Equals(sl.Questionnumber)).FirstOrDefault();
                                if (ad != null)
                                    FillSavedAnswers(ad?.Answerdetaillist, qvm);
                            }
                            s.Questions.Add(qvm);
                        }

                        model.Sections.Add(s);
                    }
                }
                CacheProvider.Store<Survey>(ExistingSurvey, new CacheItem<Survey>(model));
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            //JumpHere:
            Section currentSection = model.Sections != null && model.Sections.Count > 0 ? model.Sections.FirstOrDefault() : new Section();
            currentSection.PercentageCompleted = percentCompleted;

            if (currentSection != null && currentSection.Questions != null) { currentSection.Questions.ForEach(x => x.SortAnswers()); }
            return PartialView(string.Format(ViewPath, "SurveyPage"), new SurveyPageViewModel() { FirstSection = currentSection });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSurvey(string form, bool exit = false, bool back = false)
        {
            NameValueCollection data = HttpUtility.ParseQueryString(form);
            Survey savedModel;
            Section model = new Section();
            if (exit)
            {
                if (data.Count == 1)
                {
                    return Json(new { status = "OK", reload = true }, JsonRequestBehavior.DenyGet);
                }
            }
            if (!CacheProvider.TryGet<Survey>(ExistingSurvey, out savedModel))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J54_LANDING_PAGE);
            }

            Section currentSection = null; bool saveSuccess = false; ResponseModels.SaveSurveyResponse saveResponse = null;
            try
            {
                switch (savedModel.Stage)
                {
                    case Stage.One:
                        currentSection = savedModel.Sections.Where(x => x.GetSectionNumber() == 1).FirstOrDefault();
                        break;

                    case Stage.Two:
                        currentSection = savedModel.Sections.Where(x => x.GetSectionNumber() == 2).FirstOrDefault();
                        break;

                    case Stage.Three:
                        currentSection = savedModel.Sections.Where(x => x.GetSectionNumber() == 3).FirstOrDefault();
                        break;

                    case Stage.Four:
                        currentSection = savedModel.Sections.Where(x => x.GetSectionNumber() == 4).FirstOrDefault();
                        break;

                    case Stage.Five:
                        currentSection = savedModel.Sections.Where(x => x.GetSectionNumber() == 5).FirstOrDefault();
                        break;

                    case Stage.Six:
                        currentSection = savedModel.Sections.Where(x => x.GetSectionNumber() == 6).FirstOrDefault();
                        break;

                    case Stage.Seven:
                        currentSection = savedModel.Sections.Where(x => x.GetSectionNumber() == 7).FirstOrDefault();
                        break;

                    case Stage.Eight:
                        currentSection = savedModel.Sections.Where(x => x.GetSectionNumber() == 8).FirstOrDefault();
                        break;

                    default:
                        break;
                }

                List<Answerlist> ans1 = new List<Answerlist>();

                if (back) { saveSuccess = true; goto jumpHere; }
                ans1 = ComposeAnswers(currentSection, data);

                if (ans1.Count > 0)
                {
                    switch (savedModel.Stage)
                    {
                        case Stage.One:
                            savedModel.JourneyType = (ans1.Where(x => x.questionnumber == "0001").FirstOrDefault().answerdetaillist.FirstOrDefault().answernumber.Equals("0001") ? SectionType.Villa : SectionType.Apartment);
                            int totalQuestions = 1;
                            List<Section> newSections = new List<Section>();
                            foreach (var s in savedModel.Sections.Where(x => x.Type == SectionType.Both || x.Type == savedModel.JourneyType))
                            {
                                s.JourneyType = savedModel.JourneyType;
                                foreach (var q in s.Questions)
                                {
                                    q.Counter = totalQuestions++;
                                    foreach (var a in q.Answers)
                                    {
                                        if (a.DynamicQuestions != null && a.DynamicQuestions.Count > 0)
                                        {
                                            int dcounter = 1;
                                            foreach (var dq in a.DynamicQuestions)
                                            {
                                                dq.Counter = dcounter++;
                                                dq.DQDisplayNumber = string.Format("{0}. {1} -", q.Counter, a.Description);
                                                //int parentQCounter=savedModel.
                                            }
                                        }
                                    }
                                }
                                newSections.Add(s);
                            }

                            savedModel.Sections.Clear(); savedModel.Sections.AddRange(newSections);

                            totalQuestions--;

                            //savedModel.QuestionFactor = Convert.ToInt32(Math.Round(100 / (double)totalQuestions, 0));
                            savedModel.TotalQuestions = totalQuestions;

                            saveResponse = restClient.SaveSurveyAnswers(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, SelectedAccount, ans1, Request.Segment(), RequestLanguage).Payload;
                            saveSuccess = saveResponse == null ? false : true;
                            break;

                        case Stage.Seven:
                        case Stage.Eight:
                            if ((savedModel.JourneyType == SectionType.Villa && savedModel.Stage == Stage.Seven) || exit == true)
                            {
                                saveResponse = restClient.SaveSurveyAnswers(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, SelectedAccount, ans1, Request.Segment(), RequestLanguage).Payload;
                                saveSuccess = saveResponse == null ? false : true;
                                break;
                            }
                            savedModel.Answers.AddRange(ans1);
                            var res = restClient.SubmitSurveyAnswers(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, SelectedAccount, savedModel.Answers, Request.Segment(), RequestLanguage).Payload;
                            if (res != null && res.Breakdownlist != null && res.Breakdownlist.Count > 0)
                            {
                                ReportPageViewModel vm = new ReportPageViewModel() { ContentKey = GetEncodedString(SelectedAccount, DateTime.Now.ToString("yyyy-MMM-dd"), res.Version) };

                                var elec = res.Breakdownlist.Where(x => x.Type.Equals("1")).FirstOrDefault();
                                var water = res.Breakdownlist.Where(x => x.Type.Equals("0")).FirstOrDefault();

                                System.Text.StringBuilder sb = new StringBuilder(string.Format(RESULT_JSON_SUB, Translate.Text("j154_HVAC"), "69AE84", elec.Value1) + ",");
                                sb.Append(string.Format(RESULT_JSON_SUB, Translate.Text("j154_Lighting"), "65ADD8", elec.Value2) + ",");
                                sb.Append(string.Format(RESULT_JSON_SUB, Translate.Text("j154_Others"), "5C687E", elec.Value3));

                                vm.ElectricityDataSeriesJson = string.Format(RESULT_JSON_MAIN, "E", sb.ToString()); sb.Clear();

                                sb.Append(string.Format(RESULT_JSON_SUB, Translate.Text("j154_WOU"), "69AE84", water.Value1) + ",");
                                sb.Append(string.Format(RESULT_JSON_SUB, Translate.Text("j154_WBSS"), "65ADD8", water.Value2) + ",");
                                sb.Append(string.Format(RESULT_JSON_SUB, Translate.Text("j154_TF"), "5C687E", water.Value3));

                                vm.WaterDataSeriesJson = string.Format(RESULT_JSON_MAIN, "E", sb.ToString());
                                vm.Tips = new List<Tips>();
                                if (res.Tipslist.Count > 0) { vm.Tips.AddRange(res.Tipslist.Select(x => new Tips() { Header = x.Header, Title = x.Title, Body = x.Tips ?? string.Empty, ImgUrl = x.Tipurl ?? string.Empty }).ToList()); }

                                CacheProvider.Remove(ExistingSurvey);
                                return PartialView(string.Format(ViewPath, "SurveyReportPage"), vm);
                            }
                            else
                            {
                                CacheProvider.Remove(ExistingSurvey);
                                ModelState.AddModelError("", ErrorMessages.UNEXPECTED_ERROR);
                                return PartialView("~/Views/Feature/CommonComponents/Shared/_SubmissionError.cshtml");
                            }

                        default:
                            saveResponse = restClient.SaveSurveyAnswers(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, SelectedAccount, ans1, Request.Segment(), RequestLanguage).Payload;
                            saveSuccess = saveResponse == null ? false : true;
                            break;
                    }
                }
            jumpHere:
                if (saveSuccess)
                {
                    if (exit == true)
                    {
                        CacheProvider.Remove(ExistingSurvey);
                        return Json(new { status = "OK", reload = true }, JsonRequestBehavior.DenyGet);
                    }

                    if (back == true)
                    {
                        savedModel.Stage = savedModel.GetPreviousStage();
                        if (savedModel.Stage == Stage.One)
                        {
                            model = savedModel.Sections.FirstOrDefault();
                            //model.PercentageCompleted = 0;
                            savedModel.QuestionSaved = 0;
                        }
                        else
                        {
                            model = savedModel.Sections.Where(x => x.GetSectionNumber() == (currentSection.GetSectionNumber() - 1)).FirstOrDefault();
                            savedModel.QuestionSaved = savedModel.QuestionSaved - model.Questions.Count;
                            //model.PercentageCompleted = Convert.ToInt32(Math.Round((savedModel.QuestionSaved / (double)savedModel.TotalQuestions) * 100, 0));
                            //Convert.ToInt32(Math.Round(100 / (double)totalQuestions, 0))
                        }
                        FillPreviousSectionAnswers(savedModel.Answers, model);
                    }
                    else
                    {
                        savedModel.Stage = savedModel.GetNextStage();
                        model = savedModel.Sections.Where(x => x.GetSectionNumber() == (currentSection.GetSectionNumber() + 1)).FirstOrDefault();

                        savedModel.QuestionSaved += currentSection.Questions.Count;
                        //model.PercentageCompleted = savedModel.QuestionSaved * savedModel.QuestionFactor;
                        //model.PercentageCompleted = model.PercentageCompleted = Convert.ToInt32(Math.Round((savedModel.QuestionSaved / (double)savedModel.TotalQuestions) * 100, 0));

                        savedModel.Answers.AddRange(ans1);
                    }
                    int p = 0;
                    if (!back && saveResponse != null && !string.IsNullOrEmpty(saveResponse.Percentage))
                    {
                        int.TryParse(saveResponse.Percentage, out p);
                        model.PercentageCompleted = savedModel.LastPercentageCompleted = p;
                    }
                    else
                    {
                        model.PercentageCompleted = savedModel.LastPercentageCompleted;
                    }

                    CacheProvider.Store<Survey>(ExistingSurvey, new CacheItem<Survey>(savedModel));
                }
                else
                {
                    ModelState.AddModelError("", ErrorMessages.UNEXPECTED_ERROR);
                    model = savedModel.Sections.Where(x => x.GetSectionNumber() == currentSection.GetSectionNumber()).FirstOrDefault();
                    if (model != null && model.Questions != null) { model.Questions.ForEach(x => x.Answers.OrderBy(y => y.Description)); }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return PartialView(string.Format(ViewPath, ViewName), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UserSurveys(string account)
        {
            //LandingPageViewModel model = new LandingPageViewModel() { Downloads = new Dictionary<string, string>(), IsCompleted = false, PercentageDone = 0 };
            if (!string.IsNullOrEmpty(account)) { SelectedAccount = HttpUtility.UrlEncode(account); }
            else { return Json(new { started = false, percentage = 0, pdf = new List<KeyValuePair<string, string>>() }, JsonRequestBehavior.DenyGet); }

            bool started = false; int percent = 0; List<KeyValuePair<string, string>> Downloads = new List<KeyValuePair<string, string>>();
            try
            {
                var res = restClient.GetSavedAnswers(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, account, Request.Segment(), RequestLanguage);
                if (res.Payload != null && res.Payload.Answerlist!=null && res.Payload.Answerlist.Count > 0)
                {
                    if (res.Payload.Answerlist.Count > 0)
                    {
                        int.TryParse(res.Payload.Percentage, out percent);
                        started = percent == 0 ? false : true;
                    }
                }
                var res1 = restClient.GetSubmittedSurveys(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, account, Request.Segment(), RequestLanguage);
                if (res1.Succeeded && res1.Payload.Versiondatelist.Count > 0)
                {
                    foreach (var d in res1.Payload.Versiondatelist)
                    {
                        Downloads.Add(new KeyValuePair<string, string>(d.Createddate, GetEncodedString(d.Contractaccountnumber, d.Createddate, d.Version)));
                    }
                    //model.Downloads = res1.Payload.Versiondatelist.ToDictionary(x => x.Createddate,  x=> GetEncodedString(x.Contractaccountnumber,x.Createddate, x.Version));
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { started, percentage = percent, pdf = Downloads }, JsonRequestBehavior.DenyGet);
        }

        [HttpGet, TwoPhaseAuthorize]
        public ActionResult Download(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                var validKey = GetDecryptedValues(key);
                string version = validKey.Item4.Replace("\0", "");
                if (validKey.Item1 == true)
                {
                    var res = restClient.DownloadSurveyReport(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, validKey.Item2, version, Request.Segment(), RequestLanguage);

                    if (res.Succeeded && !string.IsNullOrEmpty(res.Payload))
                    {
                        return File(Convert.FromBase64String(res.Payload), "application/pdf", string.Format("CA{0}_{1}_{2}.pdf", validKey.Item2, validKey.Item3, version));
                    }
                }
            }

            LogService.Error(new Exception("Key is empty or it is invalid: " + key), this);

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ERROR_404);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EmailReport(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                var validKey = GetDecryptedValues(key);
                if (validKey.Item1 == true)
                {
                    var res = restClient.EmailSurveyReport(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, validKey.Item2, validKey.Item4, Request.Segment(), RequestLanguage);

                    if (res.Succeeded)
                    {
                        //return View(string.Format(ViewPath, "_SusscessMessage"));
                        return Json(new { status = true, message = Translate.Text("j154_ReportSentMessage") }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        LogService.Error(new Exception("SAP Service failed to send email report: " + res.Message) { }, this);
                        return Json(new { status = false, message = ErrorMessages.UNEXPECTED_ERROR }, JsonRequestBehavior.DenyGet);
                    }
                }
            }

            //LogService.Error(new Exception("Key is empty or it is invalid: " + key), this);
            return Json(new { status = false, message = "Key is empty or it is invalid" }, JsonRequestBehavior.DenyGet);
            //return RedirectToSitecoreItem(SitecoreItemIdentifiers.ERROR_404);
        }

        #endregion Actions

        #region Helper Methods

        private List<Answerlist> ComposeAnswers(Section currentSection, NameValueCollection form)
        {
            List<Answerlist> retList = new List<Answerlist>();
            const string cbName = "{0}_{1}_{2}"; // "LVA_39_0004";
            try
            {
                foreach (var ql in currentSection.Questions)
                {
                    Answerlist alist = new Answerlist() { questionnumber = ql.GetNumberWithLeadingZeros(), answerdetaillist = new List<Answerdetaillist>(), section = ql.ParentSection };

                    switch (ql.Type)
                    {
                        case QuestionType.SelectList:
                        case QuestionType.CheckBox:
                            foreach (var ans in ql.Answers)
                            {
                                if (form[string.Format(cbName, ql.ParentSection.ToUpper(), ql.Number.ToString(), ans.Number)] != null)
                                {
                                    alist.answerdetaillist.Add(new Answerdetaillist() { answernumber = ans.Number });

                                    var selectedAns = ql.Answers.Where(x => x.Number.Equals(ans.Number)).FirstOrDefault();
                                    if (selectedAns != null)
                                    {
                                        selectedAns.Selected = true; selectedAns.AnswerInputText = ans.Number;
                                    }
                                    ComposeDynamicAnswers(retList, selectedAns, form);
                                    /*if (selectedAns.DynamicQuestions != null && selectedAns.DynamicQuestions.Count > 0)
                                    {
                                        foreach (var dq in selectedAns.DynamicQuestions)
                                        {
                                            Answerlist dlist = new Answerlist() { questionnumber = dq.GetNumberWithLeadingZeros(), answerdetaillist = new List<Answerdetaillist>(), section = dq.ParentSection };
                                            switch (dq.Type)
                                            {
                                                case QuestionType.RadioButton:
                                                case QuestionType.CheckBox:
                                                    if (form[dq.GetKey()] != null)
                                                    {
                                                        dlist.answerdetaillist.Add(new Answerdetaillist() { answernumber = form[dq.GetKey()].ToString() });
                                                        var dsAns = dq.Answers.Where(x => x.Number.Equals(form[dq.GetKey()].ToString())).FirstOrDefault();
                                                        if (dsAns != null) { dsAns.Selected = true; dsAns.AnswerInputText = dsAns.Number; }
                                                    }
                                                    break;

                                                case QuestionType.Rating:
                                                    if (form[dq.GetKey()] != null)
                                                    {
                                                        var dsAns = dq.Answers.Where(x => x.Number.Equals(form[dq.GetKey()].ToString())).FirstOrDefault();
                                                        if (dsAns != null)
                                                        {
                                                            dsAns.Selected = true; dsAns.AnswerInputText = dsAns.Number;
                                                            dlist.answerdetaillist.Add(new Answerdetaillist() { answernumber = dsAns.Number });
                                                        }
                                                    }
                                                    break;

                                                default:
                                                    LogService.Error(new Exception("Unknow Dynamic Question:" + Environment.NewLine + Newtonsoft.Json.JsonConvert.SerializeObject(dq)), this);
                                                    continue;
                                            }
                                            retList.Add(dlist);
                                        }
                                    }*/
                                }
                            }
                            break;

                        case QuestionType.RadioButton:
                            if (form[ql.GetKey()] != null)
                            {
                                if (form[ql.GetKey()] != null)
                                {
                                    alist.answerdetaillist.Add(new Answerdetaillist() { answernumber = form[ql.GetKey()].ToString() });

                                    var selectedAns = ql.Answers.Where(x => x.Number.Equals(form[ql.GetKey()].ToString())).FirstOrDefault();
                                    if (selectedAns != null)
                                    {
                                        selectedAns.Selected = true; selectedAns.AnswerInputText = selectedAns.Number;
                                        ComposeDynamicAnswers(retList, selectedAns, form);
                                    }
                                }
                            }
                            break;

                        case QuestionType.TextBox:
                            if (form[ql.GetKey()] != null)
                            {
                                alist.answerdetaillist.Add(new Answerdetaillist() { answertext = form[ql.GetKey()].ToString() });
                                var selectedAns = ql.Answers.Where(x => x.Number.Equals(form[ql.GetKey()].ToString())).FirstOrDefault();
                                if (selectedAns != null)
                                {
                                    selectedAns.Selected = true; selectedAns.AnswerInputText = form[ql.GetKey()].ToString();
                                }
                            }
                            break;

                        case QuestionType.Slider:
                            if (form[ql.GetKey()] != null)
                            {
                                alist.answerdetaillist.Add(new Answerdetaillist() { answertext = form[ql.GetKey()].ToString() });
                                var selectedAns = ql.Answers.Where(x => x.Number.Equals(form[ql.GetKey()].ToString())).FirstOrDefault();
                                if (selectedAns != null)
                                {
                                    selectedAns.Selected = true; selectedAns.AnswerInputText = form[ql.GetKey()].ToString();
                                }
                            }
                            break;

                        case QuestionType.Rating:
                            if (form[ql.GetKey()] != null)
                            {
                                var selectedAns = ql.Answers.Where(x => x.Number.Equals(form[ql.GetKey()].ToString())).FirstOrDefault();
                                if (selectedAns != null)
                                {
                                    selectedAns.Selected = true; selectedAns.AnswerInputText = selectedAns.Description;
                                    alist.answerdetaillist.Add(new Answerdetaillist() { answernumber = selectedAns.Number });
                                }
                            }
                            break;
                    }
                    if (alist.answerdetaillist.Count > 0)
                    { retList.Add(alist); }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return retList;
        }

        private void ComposeDynamicAnswers(List<Answerlist> saveList, SurveyAnswerViewModel selectedAns, NameValueCollection form)
        {
            try
            {
                if (selectedAns.DynamicQuestions != null && selectedAns.DynamicQuestions.Count > 0)
                {
                    foreach (var dq in selectedAns.DynamicQuestions)
                    {
                        Answerlist dlist = new Answerlist() { questionnumber = dq.GetNumberWithLeadingZeros(), answerdetaillist = new List<Answerdetaillist>(), section = dq.ParentSection };
                        switch (dq.Type)
                        {
                            case QuestionType.RadioButton:
                            case QuestionType.CheckBox:
                                if (form[dq.GetKey()] != null)
                                {
                                    dlist.answerdetaillist.Add(new Answerdetaillist() { answernumber = form[dq.GetKey()].ToString() });
                                    var dsAns = dq.Answers.Where(x => x.Number.Equals(form[dq.GetKey()].ToString())).FirstOrDefault();
                                    if (dsAns != null) { dsAns.Selected = true; dsAns.AnswerInputText = dsAns.Number; }
                                }
                                break;

                            case QuestionType.Rating:
                                if (form[dq.GetKey()] != null)
                                {
                                    var dsAns = dq.Answers.Where(x => x.Number.Equals(form[dq.GetKey()].ToString())).FirstOrDefault();
                                    if (dsAns != null)
                                    {
                                        dsAns.Selected = true; dsAns.AnswerInputText = dsAns.Number;
                                        dlist.answerdetaillist.Add(new Answerdetaillist() { answernumber = dsAns.Number });
                                    }
                                }
                                break;

                            default:
                                LogService.Error(new Exception("Unknow Dynamic Question:" + Environment.NewLine + Newtonsoft.Json.JsonConvert.SerializeObject(dq)), this);
                                continue;
                        }
                        saveList.Add(dlist);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
        }

        private string GetEncodedString(string contractNumber, string createdDate, string version)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(EncodePhrase));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider
            {
                Key = TDESKey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros
            };
            byte[] DataToEncrypt = UTF8.GetBytes(string.Concat(contractNumber, "|", createdDate, "|", version));
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            return HttpUtility.UrlEncode(Convert.ToBase64String(Results));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="encodedString"></param>
        /// <returns>status, contract number, createdDate, version</returns>
        private Tuple<bool, string, string, string> GetDecryptedValues(string encodedString)
        {
            string decodedString = HttpUtility.UrlDecode(encodedString);
            Tuple<bool, string, string, string> retval = new Tuple<bool, string, string, string>(false, string.Empty, string.Empty, string.Empty);

            try
            {
                byte[] Results;
                UTF8Encoding UTF8 = new UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(EncodePhrase));
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider
                {
                    Key = TDESKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.Zeros
                };
                byte[] DataToDecrypt = Convert.FromBase64String(decodedString);
                try
                {
                    ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
                string resulttext = UTF8.GetString(Results);
                string[] result = resulttext.Split('|');

                retval = new Tuple<bool, string, string, string>(true, result[0], result[1], result[2]);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            return retval;
        }

        private string SelectedAccount
        {
            get
            {
                string sa;
                if (CacheProvider.TryGet<string>(SelectedAccountKey, out sa)) return sa;
                //if (CacheProvider.TryGet<string>(CacheKeys.Dashboard_SELECTEDACCOUNT, out sa)) return sa;

                //CacheProvider.Store<string>(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(CurrentPrincipal.PrimaryAccount, Times.Once));
                return CurrentPrincipal.PrimaryAccount;
            }
            set
            {
                //CacheProvider.Store<string>(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(value, Times.Once));
                CacheProvider.Store<string>(SelectedAccountKey, new CacheItem<string>(value));
            }
        }

        private void FillSavedAnswers(List<ResponseModels.SavedAnswerDetail> savedAnswers, SurveyQuestionViewModel question)
        {
            if (savedAnswers == null || savedAnswers.Count() < 1) return;

            //string anstr = q.GetNumberWithLeadingZeros();
            //var al = res.Where(x => x.Questionnumber.Equals(anstr)).FirstOrDefault();
            //if (al == null) continue;
            try
            {
                foreach (var a in question.Answers)
                {
                    switch (question.Type)
                    {
                        case QuestionType.CheckBox:
                        case QuestionType.Rating:
                        case QuestionType.RadioButton:
                            var dd = savedAnswers.Where(x => x.Answernumber.Equals(a.Number)).FirstOrDefault();
                            if (dd == null) continue;
                            a.Selected = true;
                            a.AnswerInputText = dd != null ? (string.IsNullOrEmpty(dd.Answernumber) ? string.Empty : dd.Answernumber) : string.Empty;
                            break;

                        case QuestionType.SelectList:
                        case QuestionType.Slider:
                        case QuestionType.TextBox:
                            var aa = savedAnswers.FirstOrDefault();
                            if (aa == null) continue;
                            a.Selected = true;
                            a.AnswerInputText = aa.Answertext.ToString();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
        }

        private List<SurveyAnswerViewModel> GetAnswers(List<ResponseModels.Answerlist> al, List<ResponseModels.Questionlist> dList, SurveyQuestionViewModel parentQuestion, List<ResponseModels.SavedAnswer> savedAnswers)
        {
            List<SurveyAnswerViewModel> retList = new List<SurveyAnswerViewModel>();
            foreach (var l in al)
            {
                SurveyAnswerViewModel sav = new SurveyAnswerViewModel() { Description = l.Description, Number = l.Answernumber, DynamicQuestions = new List<SurveyQuestionViewModel>(), ImageUrl = string.IsNullOrEmpty(l.Imgurl) ? string.Empty : l.Imgurl };

                if (l.Dynamicquestionlist != null && l.Dynamicquestionlist.Count > 0)
                {
                    try
                    {
                        int counter = 1;
                        foreach (string dq in l.Dynamicquestionlist)
                        {
                            var dmq = dList.Where(x => x.Questionnumber.Equals(dq)).FirstOrDefault();
                            if (dmq != null)
                            {
                                SurveyQuestionViewModel dquestion = new SurveyQuestionViewModel() { DQDisplayNumber = parentQuestion.Counter.ToString() + "." + counter.ToString(), IsDynamic = true, Counter = counter++, Description = dmq.Description, Number = int.Parse(dmq.Questionnumber), Type = dmq.Type.QuestionTypeFromString(), ParentSection = parentQuestion.ParentSection, ParentQuestionNumber = parentQuestion.Number, ParentAnswer = new SurveyAnswerViewModel() { Number = l.Answernumber }, Answers = dmq.Answerlist.Select(x => new SurveyAnswerViewModel() { Description = x.Description, Number = x.Answernumber, ImageUrl = string.IsNullOrEmpty(x.Imgurl) ? string.Empty : x.Imgurl }).ToList() };
                                if (savedAnswers != null)
                                {
                                    var saForThisQ = savedAnswers.Where(x => x.Questionnumber.Equals(dq)).FirstOrDefault();

                                    if (saForThisQ != null && saForThisQ.Answerdetaillist != null) FillSavedAnswers(saForThisQ.Answerdetaillist, dquestion);
                                }
                                sav.DynamicQuestions.Add(dquestion);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex, this);
                    }
                }
                retList.Add(sav);
            }
            return retList;
        }

        private void FillPreviousSectionAnswers(List<Answerlist> savedAnswers, Section currentSection)
        {
            try
            {
                foreach (var q in currentSection.Questions)
                {
                    var al = savedAnswers.Where(x => x.questionnumber.Equals(q.GetNumberWithLeadingZeros())).FirstOrDefault();
                    if (al == null) continue;

                    foreach (var qa in q.Answers)
                    {
                        qa.Selected = false; //clear any existing selection

                        if (q.Type == QuestionType.Slider || q.Type == QuestionType.TextBox)
                        {
                            var inAns = al.answerdetaillist.FirstOrDefault();
                            if (inAns != null && !string.IsNullOrEmpty(inAns.answertext)) { qa.Selected = true; qa.AnswerInputText = inAns.answertext; }
                        }
                        else
                        {
                            foreach (var ans in al.answerdetaillist.Where(x => x.answernumber.Equals(qa.Number)).ToList())
                            {
                                qa.Selected = true; qa.AnswerInputText = ans.answernumber;
                                break;
                            }
                        }
                        if (qa.DynamicQuestions != null && qa.DynamicQuestions.Count > 0)
                        {
                            foreach (var dq in qa.DynamicQuestions)
                            {
                                var dql = savedAnswers.Where(x => x.questionnumber.Equals(dq.GetNumberWithLeadingZeros())).FirstOrDefault();
                                foreach (var da in dq.Answers)
                                {
                                    da.Selected = false;
                                    if (dq.Type == QuestionType.Slider || dq.Type == QuestionType.TextBox)
                                    {
                                        var inAns = dql.answerdetaillist.FirstOrDefault();
                                        if (inAns != null && !string.IsNullOrEmpty(inAns.answertext)) { da.Selected = true; da.AnswerInputText = inAns.answertext; }
                                    }
                                    else
                                    {
                                        foreach (var ans in dql.answerdetaillist.Where(x => x.answernumber.Equals(da.Number)).ToList())
                                        {
                                            da.Selected = true; da.AnswerInputText = ans.answernumber;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //var fans=aq.Where(x=>x.answerdetaillist.Where))
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
        }

        #endregion Helper Methods
    }
}