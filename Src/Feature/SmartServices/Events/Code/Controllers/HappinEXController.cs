using DEWAXP.Feature.Events.Models.HappinEX;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Logger;
using Glass.Mapper.Sc;
using Sitecore.Data;
using Sitecore.SecurityModel;
using System;
using System.Linq;
using System.Web.Mvc;
using SitecoreX = global::Sitecore;

namespace DEWAXP.Feature.Events.Controllers
{
    public class HappinEXController : BaseController
    {
        private const string SURVEYS_ROOT_ITEM_PATH = "/sitecore/content/Global References/Happinex Survey/Surveys_Root";
        private const string SURVEYS_RESPONSE_ROOT_ITEM_ID = "{3302F407-74FD-4D4A-A61B-163278D184BE}";
        private readonly ISitecoreService _sitecoreService;

        public HappinEXController()
        {
            _sitecoreService = DependencyResolver.Current.GetService<ISitecoreService>();
        }

        public ActionResult Survey(string survey_key = "", string tid = "")
        {
            SurveyPageModel model = new SurveyPageModel();
            string _SurveyType = "";

            if (string.IsNullOrEmpty(survey_key) || string.IsNullOrEmpty(tid)) { model.ShowError = true; return View(model); }
            try
            {
                var surveys = _sitecoreService.GetItems<BasicSurvey>(new GetItemsByQueryOptions(new Query(SURVEYS_ROOT_ITEM_PATH + "/*")));
                if (surveys == null && surveys.Count() == 0) { model.ShowError = true; return View(model); };

                var currentSurvey = surveys.Where(x => x.ProductOrService.Equals(survey_key)).FirstOrDefault();

                if (currentSurvey == null)
                {
                    LogService.Error(new Exception(string.Format("survey: {0} not found, redirecting to not found page", survey_key), null), this);
                    model.ShowError = true;
                    return View("~/Views/Feature/Events/HappinEX/Survey.cshtml",model);
                }

                model.Survey = currentSurvey;
                model.Key = currentSurvey.Id.ToString();
                model.TrackingID = tid;
                model.SurveyID = survey_key;

                if (model.Survey.IsPostServiceSurvey)
                {
                    _SurveyType = "~/Views/Feature/Events/HappinEX/_PostServiceSurvey.cshtml";
                }
                if (model.Survey.IsPre2020Survey)
                {
                    _SurveyType = "~/Views/Feature/Events/HappinEX/_PreSurvey2020.cshtml";
                }

                if (!IsValidSurvey(survey_key, tid, currentSurvey)) { model.ShowError = true; return View(model); }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                model.ShowError = true;
            }

            if (!string.IsNullOrWhiteSpace(_SurveyType))
            {
                return View(_SurveyType, model);
            }

            return View("~/Views/Feature/Events/HappinEX/Survey.cshtml",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Survey(string Key, string __RequestVerificationToken, string SurveyJson, string tracking_id, string survey_id)
        {
            JsonObject jo = string.IsNullOrEmpty(SurveyJson) ? new JsonObject() : CustomJsonConvertor.DeserializeObject<JsonObject>(SurveyJson);

            if (jo == null || (jo.sid != survey_id || jo.tid != tracking_id)) { return Json(null, JsonRequestBehavior.AllowGet); }

            SurveyPageModel model = new SurveyPageModel();

            if (string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(tracking_id)) { model.ShowError = true; return View("~/Views/Feature/Events/HappinEX/Survey.cshtml",model); }

            var lang = SitecoreX.Data.Managers.LanguageManager.GetLanguage("en");

            var dbs = ContentRepository.GetItem<BasicSurvey>(new GetItemByIdOptions(Guid.Parse(Key)));

            if (dbs == null || IsValidSurvey(survey_id, tracking_id, dbs) == false) { model.ShowError = true; return View("~/Views/Feature/Events/HappinEX/Survey.cshtml",model); }

            string name = survey_id + "_" + tracking_id;

            using (new SecurityDisabler())
            {
                try
                {
                    SurveyAnswer sa = new SurveyAnswer() { ResponseJson = SurveyJson, Name = name };
                    sa.Language = lang;

                    var parent = ContentRepository.GetItem<ResponseFolder>(new GetItemByIdOptions(Guid.Parse(dbs.DataStorageLocation)));

                    var newItem = _sitecoreService.CreateItem(parent, sa);
                    _sitecoreService.SaveItem(newItem);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #region Private methods

        //private SitecoreService _masterService = new SitecoreService("master");
        //private SitecoreService _webService = new SitecoreService("web");
        //private SitecoreService GetMasterService
        //{
        //    get
        //    {
        //        return this._masterService;
        //    }
        //}

        //private SitecoreService GetWebService
        //{
        //    get
        //    {
        //        return this._webService;
        //    }
        //}

        private bool IsValidSurvey(string survey_key, string tracking_id, BasicSurvey currentSurvey)
        {
            var surveyReponseLoc = Database.GetDatabase("master").GetItem(currentSurvey.DataStorageLocation);
            if (surveyReponseLoc == null)
            {
                LogService.Error(new Exception("HappinEX Survey key: " + survey_key + " DataStorageLocation property is not defined or path missting.."), this);
                return false;
            }
            var existingResponses = Database.GetDatabase("master").SelectItems(surveyReponseLoc.Paths.Path + "/*");

            var jsonresponses = existingResponses?.Select(x => CustomJsonConvertor.DeserializeObject<JsonObject>(x.Fields["Response Json"].Value)).ToList();

            if (jsonresponses != null && jsonresponses.Count() > 0)
            {
                var check_tid_exist = jsonresponses.Where(x => x.tid == tracking_id && x.sid == survey_key).FirstOrDefault();
                if (check_tid_exist == null) //if not found in existing surveys, let user post the survey
                {
                    return true;
                }
                else //otherwise invalid context
                {
                    return false;
                }
            }

            return true;
        }

        #endregion Private methods
    }
}