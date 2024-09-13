using DEWAXP.Feature.SupplyManagement.Models.ComplaintSurvey;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.SmartResponseModel;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Requests;
using Sitecore.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using _SM_CommonHelper = DEWAXP.Feature.SupplyManagement.Helpers.SmartResponse.SmartResponseHelper;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class ComplaintSurveyController : BaseController
    {
        [HttpPost]
        public ActionResult GetQuestions(FormCollection form, string notif)
        {
            //string answer1 = Request.Form["RdbGrp_1"];
            //string answer2 = Request.Form["RdbGrp_2"];
            //string answer3 = Request.Form["RdbGrp_3"];
            //string answer4 = Request.Form["RdbGrp_4"];

            string comments = Request.Form["Comment"];
            string notificationNumber = notif;

            ComplaintSurveyModel objModel = new ComplaintSurveyModel();

            objModel.QuestionNoList = new string[10];
            objModel.AnswerChoice = new string[10];

            int questionGroupCount = 0;
            foreach (string keys in form)
            {
                if (keys.StartsWith("RdbGrp"))
                {
                    string groupType = form.Keys.ToString();

                    objModel.QuestionNoList[questionGroupCount] = System.Convert.ToString(questionGroupCount + 1);
                    objModel.AnswerChoice[questionGroupCount] = Request.Form[keys];

                    questionGroupCount++;
                }
            }

            //objModel.QuestionNoList[0] = "1";
            //objModel.QuestionNoList[1] = "2";
            //objModel.QuestionNoList[2] = "3";
            //objModel.QuestionNoList[3] = "4";

            //objModel.AnswerChoice[0] = answer1;
            //objModel.AnswerChoice[1] = answer2;
            //objModel.AnswerChoice[2] = answer3;
            //objModel.AnswerChoice[3] = answer4;

            ComplaintsSurveyFeedback ObjRequest = new ComplaintsSurveyFeedback();
            ObjRequest.AnswerChoice = objModel.AnswerChoice;
            ObjRequest.QuestionNumber = objModel.QuestionNoList;
            ObjRequest.Comment = comments;
            ObjRequest.notificationkey = notificationNumber;

            var response = DewaApiClient.SetComplaintsSurveyFeedback(ObjRequest, RequestLanguage, Request.Segment());

            if (response.Succeeded)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.COMPLAINTSURVEY_THANKS);
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.COMPLAINTSURVEY_ERROR);
            }
        }


        [HttpGet]
        public ActionResult GetQuestions(string notif)
        {
            Questions QS = null;
            List<Questions> QIList = new List<Questions>();
            ViewBag.BtnVisible = "true";

            try
            {
                string notificationNumber = notif;

                var response = DewaApiClient.GetComplaintsSurveyQuestionnaire(notificationNumber, RequestLanguage, Request.Segment());

                if (response.Succeeded == false)
                {
                    //TempData["Error"] = response.Message;
                    ViewBag.BtnVisible = "false";
                    ModelState.AddModelError(string.Empty, response.Message);
                }

                ComplaintSurveyModel objModel = new ComplaintSurveyModel();

                if (RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.English)
                {
                    objModel.QuestionList = response.Payload.QuestionEnglish;
                }
                else
                {
                    objModel.QuestionList = response.Payload.QuestionArabic;
                }

                objModel.QuestionNoList = response.Payload.QuestionNo;

                for (int i = 0; i <= objModel.QuestionList.Length - 1; i++)
                {
                    QS = new Questions();

                    QS.Question = objModel.QuestionList[i];
                    QS.QuestionNo = objModel.QuestionNoList[i];

                    if (QS.QuestionNo == "1")
                    {
                        QS.QuestionGroup = Translate.Text("AccessibilityEasyAccess");
                    }
                    else if (QS.QuestionNo == "2")
                    {
                        QS.QuestionGroup = Translate.Text("TimetoRespond");
                    }
                    else if (QS.QuestionNo == "4")
                    {
                        QS.QuestionGroup = Translate.Text("Professionalism");
                    }
                    else if (QS.QuestionNo == "6")
                    {
                        QS.QuestionGroup = Translate.Text("RespectingCustomer");
                    }
                    else if (QS.QuestionNo == "7")
                    {
                        QS.QuestionGroup = Translate.Text("EaseofUse");
                    }
                    else if (QS.QuestionNo == "8")
                    {
                        QS.QuestionGroup = Translate.Text("InformationQuality");
                    }

                    QIList.Add(QS);
                }
                //objModel.QuestionList = response.Payload.QuestionEnglish;
                //objModel.QuestionNoList = response.Payload.QuestionNo;
            }
            catch (System.Exception)
            {
                RedirectToSitecoreItem(SitecoreItemIdentifiers.COMPLAINTSURVEY_ERROR);
            }

            return PartialView("~/Views/Feature/SupplyManagement/ComplaintSurvey/Survey.cshtml", QIList.ToList());
        }

        public ActionResult GetSurvey(string n)
        {
            List<questions> questions = new List<DEWAXP.Foundation.Integration.DewaSvc.questions>();
            List<relations> relationlist = new List<relations>();
            List<options> optionlist = new List<options>();
            Survey model = null;

            try
            {
                var lanType = _SM_CommonHelper.GetSelectedAnswerValue(Models.SmartResponseModel.SM_Id.LangType);
                if (lanType == null)
                {
                    if (RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.Arabic)
                    {
                        _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(Models.SmartResponseModel.SM_Id.LangType, SmlangCode.ar.ToString());
                    }
                    else if (RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.Arabic)
                    {
                        _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(Models.SmartResponseModel.SM_Id.LangType, SmlangCode.en.ToString());
                    }
                }
                string notificationNumber = n;
                if (!string.IsNullOrEmpty(notificationNumber))
                {
                    var response = DewaApiClient.GetCustomerSurveyQuestions(new DEWAXP.Foundation.Integration.DewaSvc.GetCustomerSurveyQuestions
                    {
                        surveyquestioninput = new DEWAXP.Foundation.Integration.DewaSvc.customerSurveyQuestionInput { questionkey = notificationNumber }
                    }, RequestLanguage, Request.Segment());

                    if (response.Succeeded == true && response.Payload != null && response.Payload.@return != null)
                    {
                        var qus = response.Payload.@return.questionslist;
                        var rela = response.Payload.@return.relationslist;
                        var option = response.Payload.@return.optionslist;

                        if (qus != null)
                        {
                            questions = response.Payload.@return.questionslist.ToList();
                        }
                        if (rela != null)
                        {
                            relationlist = response.Payload.@return.relationslist.ToList();
                        }
                        if (option != null)
                        {
                            optionlist = response.Payload.@return.optionslist.ToList();
                        }

                        model = new Survey()
                        {
                            questions = questions.Select(qu => new SurveyQuestion()
                            {
                                lang = qu.lang,
                                questionname = qu.questionname,
                                questionnumber = qu.questionnumber,
                                questiontype = qu.questiontype,
                                subquestionnumber = qu.subquestionnumber,
                                surveytype = qu.surveytype,
                                SrNo = !string.IsNullOrEmpty(qu.subquestionnumber) ? qu.subquestionnumber : qu.questionnumber,
                                options = optionlist.Select(op => new SurveyOption()
                                {
                                    lang = op.lang,
                                    optionnumber = op.optionnumber,
                                    optiontext = op.optiontext,
                                    questionnumber = op.questionnumber,
                                    subquestionnumber = op.subquestionnumber,
                                    surveytype = op.surveytype
                                }).Where(x => x.questionnumber == qu.questionnumber && x.subquestionnumber == qu.subquestionnumber).ToList()
                            }).ToList()
                        };

                        CacheProvider.Store("SurveyQuestions" + RequestLanguage, new CacheItem<Survey>(model));
                        ViewBag.BtnVisible = "true";
                    }
                    else
                    {
                        ViewBag.BtnVisible = "false";
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                }
            }
            catch (System.Exception)
            {
                RedirectToSitecoreItem(SitecoreItemIdentifiers.COMPLAINTSURVEY_ERROR);
            }

            return PartialView("~/Views/Feature/SupplyManagement/ComplaintSurvey/GetSurvey.cshtml", model);
        }

        [HttpPost]
        public ActionResult GetSurvey(FormCollection form, string n)
        {
            string notificationNumber = n;
            List<answers> ans = new List<answers>();
            Survey q = new Survey();
            CacheProvider.TryGet("SurveyQuestions" + RequestLanguage, out q);
            foreach (string key in form)
            {
                var keyc = key.Split('-');
                if (keyc.Length >= 2)
                {
                    if (keyc[1] != "__RequestVerificationToken" || keyc[1] != "notif")
                    {
                        var questionnumber = keyc[0] == "Q" ? keyc[2] : (keyc[0] == "SQ" ? keyc[2].Split('.')[0] : "");
                        var subquestionnumber = keyc[0] == "SQ" ? keyc[2] : "";
                        var option = keyc[1] != "TB" ? (keyc[1] == "CB" ? (Request.Form[key] == "true,false" ? "Y" : "N") : Request.Form[key]) : "";
                        var surveytype = q.questions.Where(x => x.questionnumber == questionnumber).FirstOrDefault().surveytype.ToString();
                        //var Q = q.questions.Where(x => x.questionnumber == questionnumber).FirstOrDefault().questionname.ToString();

                        var SubQ = "";
                        string A = "";
                        string sa = "";
                        if (subquestionnumber != "")
                        {
                            // SubQ = q.questions.Where(x => x.subquestionnumber == subquestionnumber).FirstOrDefault().questionname.ToString();
                        }
                        if (keyc[1] == "CB" || keyc[1] == "RB")
                        {
                            if (subquestionnumber != "")
                            {
                                // sa = q.questions.Where(x => x.subquestionnumber == subquestionnumber).FirstOrDefault().options.Where(x => x.optionnumber == option).FirstOrDefault().optiontext.ToString();
                            }
                            else
                            {
                                // A = q.questions.Where(x => x.questionnumber == questionnumber).FirstOrDefault().options.Where(x => x.optionnumber == option).FirstOrDefault().optiontext.ToString();
                            }
                        }

                        string selectedans = System.Convert.ToString(ans?.Where(x => x.questionnumber == questionnumber && string.IsNullOrWhiteSpace(x.subquestionnumber)).FirstOrDefault()?.optionnumber);
                        if (string.IsNullOrEmpty(selectedans) || System.Convert.ToInt32(selectedans) <= 3 || string.IsNullOrEmpty(subquestionnumber))
                        {
                            answers a = new answers()
                            {
                                questionnumber = questionnumber,
                                subquestionnumber = subquestionnumber,
                                optionnumber = option,
                                suggestion = keyc[1] == "TB" ? Request.Form[key] : "",
                                surveytype = surveytype,
                                //optiontext = subquestionnumber != "" ? sa : A,
                                //questionname = subquestionnumber != "" ? SubQ : Q
                            };
                            ans.Add(a);
                        }
                    }
                }
            }

            SetCustomerSurveyAnswers ObjRequest = new SetCustomerSurveyAnswers()
            {
                answersinput = new customerSurveyAnswersInput()
                {
                    answerslist = ans.ToArray(),
                    questionkey = n,
                    lang = RequestLanguage.ToString()
                }
            };
            var response = DewaApiClient.SetCustomerSurveyAnswers(ObjRequest, RequestLanguage, Request.Segment());

            if (response.Succeeded)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.COMPLAINTSURVEY_THANKS);
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.COMPLAINTSURVEY_ERROR);
            }
        }
    }
}