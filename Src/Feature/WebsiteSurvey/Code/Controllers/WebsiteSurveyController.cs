using DEWAXP.Feature.WebsiteSurvey.Models.Survey;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration;
using DEWAXP.Foundation.Integration.Impl.WebsiteSurveySvc;
using Sitecore.Globalization;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.WebsiteSurvey.Controllers
{
    public class WebsiteSurveyController : BaseController
    {
        //private IContentRepository _contentRepository;
        //private IContextRepository _contextRepository;
        //private IRenderingRepository RenderingRepository;
        //private readonly IWebsiteSurveyServiceClient WebsiteSurveyServiceClient;
        //public WebsiteSurveyController(IContentRepository contentRepository,
        //    IContextRepository contextRepository,
        //    IRenderingRepository renderingRepository,
        //    IWebsiteSurveyServiceClient websiteSurveyServiceClient)
        //{
        //    _contentRepository = contentRepository;
        //    _contextRepository = contextRepository;
        //    RenderingRepository = renderingRepository;
        //    WebsiteSurveyServiceClient = websiteSurveyServiceClient;
        //}
        [HttpGet]
        public ActionResult Survey()
        {
            var surveydatasource = RenderingContext.CurrentOrNull.Rendering?.DataSource;
            if (RenderingRepository.HasDataSource)
            {
                var inquirydatasourceitem = RenderingRepository.GetDataSourceItem<SurveyType>();

                if (inquirydatasourceitem != null)
                {
                    return View("~/Views/Feature/WebsiteSurvey/Survey/Survey.cshtml",new WebsiteSurveyQuestionAnswers
                    {
                        SurveyType = inquirydatasourceitem,
                        datasource = surveydatasource,
                    });
                }
            }
            return View("~/Views/Feature/WebsiteSurvey/Survey/Survey.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Survey(WebsiteSurveyQuestionAnswers model)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.datasource))
            {
                var suveydatasourceitem = ContentRepository.GetItem<SurveyType>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(model.datasource)));

                if (suveydatasourceitem != null)
                {
                    model.surveyInputs.ToList().ForEach(c => { c.OptionId = Math.Abs(c.OptionId); });
                    var response = WebsiteSurveyServiceClient.SaveWebsiteSurvey(new QuestionAnswers { Condition = 1, LanguageCode = ContextRepository.RequestLanguage.ToString(), ChannelId = 1, SurveyData = model.surveyInputs.ToList(), SuggestionText = model.suggestionText });

                    if (response != null && response.Succeeded && response.Payload != null && response.Payload.Data != null && response.Payload.Data.Result.ToLower().Equals("success"))
                    {
                        ViewBag.success = Translate.Text("websitesurvey.success");//response.Payload.Data.Result;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("websitesurvey.invalidlink"));
            }
            return View("~/Views/Feature/WebsiteSurvey/Survey/Survey.cshtml");
        }
    }
}