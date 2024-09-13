using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Feature.GeneralServices.Models.CCSurvey;
using Sitecore.Globalization;
using Sitecore.Mvc.Presentation;
using System;
using System.Web.Mvc;
using DEWAXP.Foundation.Content.Controllers;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    public class CCSurveyController : BaseController
    {

        [HttpGet]
        public ActionResult Inquries(string q)
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                var inquirydatasource = RenderingContext.CurrentOrNull.Rendering?.DataSource;
                if (RenderingRepository.HasDataSource)
                {
                    var inquirydatasourceitem = RenderingRepository.GetDataSourceItem<InquiryType>();

                    if (inquirydatasourceitem != null)
                    {
                        var response = DewaApiClient.GetSurveyV3Validation(q, RequestLanguage, Request.Segment());
                        if (response.Succeeded)
                        {
                            return View("~/Views/Feature/GeneralServices/CCSurvey/Inquries.cshtml",new SurveyQuestionandAnswers
                            {
                                InquiryType = inquirydatasourceitem,
                                datasource = inquirydatasource,
                                Transactioncode = q
                            });
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty,response.Message);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("ccsurvey.invalidlink"));
            }
            return View("~/Views/Feature/GeneralServices/CCSurvey/Inquries.cshtml");
        }

        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Inquries(SurveyQuestionandAnswers  model)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.datasource) && !string.IsNullOrWhiteSpace(model.Transactioncode))
            {
                var inquirydatasourceitem = ContentRepository.GetItem<InquiryType>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(model.datasource)));

                if (inquirydatasourceitem != null)
                {
                    var response = DewaApiClient.SetSurveyV3(new SetSurveyV3 { surveyLinkInput = new surveyInput { dynlink = model.Transactioncode,surveyid = inquirydatasourceitem.Surveyid },InputList = model.enquiryInputs }, RequestLanguage, Request.Segment());
                    if(response.Succeeded)
                    {
                        ViewBag.success = response.Message;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("ccsurvey.invalidlink"));
            }
            return View("~/Views/Feature/GeneralServices/CCSurvey/Inquries.cshtml");
        }
    }
}