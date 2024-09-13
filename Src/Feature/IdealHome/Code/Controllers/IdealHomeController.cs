using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DEWAXP.Feature.IdealHome.Models.IdealHome;
using Sitecore.Data;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Glass.Sitecore.Mapper;
using Sitecore.Data.Fields;
using Sitecore.Form.Core.Configuration;
using Sitecore.StringExtensions;
using Sitecore.Resources.Media;
using Sitecore.Utils;
using DEWAXP.Foundation.Integration.Responses;
using System.Text;
using DEWAXP.Foundation.Integration;
using System.Data;
using DEWA.Website.Pipelines.Providers;
using DEWA.Website.Services.Impl;

namespace DEWA.Website.Controllers
{
    public static class StringExtension
    {
        public static string Value(this string str)
        {
            string[] strValue = str.Split(',');
            return strValue[0];

        }
        public static string Code(this string str)
        {
            string[] strCode = str.Split(',');
            return strCode[1];

        }
       

    }
    public class IdealHomeController : BaseController
    {
        #region Actions
        [HttpGet]
        public ActionResult LoginMain()
        {
            return PartialView("~/Views/IdealHome/_LoginFormMain.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LoginMain(SurveyLogin model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string error;
                    if (TryLogin(model, out error))
                    {
                        
                        Session["model"] = (SurveyLogin)model;
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOME_LANDINGPAGE);

                    }
                    else
                    {
                        
                    }

                }
                catch (Exception ex)
                {
                    //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
                }
            }
            else
            {
                //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Invalid details"), Times.Once));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOME_LOGIN);
        }

        [HttpGet]
        public ActionResult Survey()
        {
            Session["folderid"] = Request.QueryString["folderid"];

            var model = Session["LoginDetails"] as SurveyLogin;

            var sitecoreService = new SitecoreContext();


            ID loginItemID = model.UserItemID;//Session["loginItemID"] as ID;

            ViewBag.EntityLogo = model.MediaUrl;//Session["imageUrl"].ToString();

            var entityItem = sitecoreService.Database.GetItem(loginItemID);

            MultilistField multiselectField = entityItem.Fields["AssignedSurvey"];

            Item[] Listitems = multiselectField.GetItems();


            SurveyQuestions surveyModel;

            Surveyoptions surveyoptions;

            List<SurveyQuestions> subcriteriaList = new List<SurveyQuestions>();

            List<Surveyoptions> criteriaList;

            Survey survey = new Survey();


            Item item = null;

            for (int i = 0; i < Listitems.Length; i++)
            {

                item = sitecoreService.Database.GetItem(Listitems[i].ID);


                surveyModel = new SurveyQuestions();

                ViewBag.MainCriteria = item.Fields["Main Criteria"].Value;


                surveyModel.SubCriteria = item.Fields["Sub Criteria"].Value;

                subcriteriaList.Add(surveyModel);

                criteriaList = new List<Surveyoptions>();

                foreach (var myInnerItem in item.Children.ToList())
                {
                    surveyoptions = new Surveyoptions();
                    surveyoptions.Text = myInnerItem.Fields["Text"].Value;
                    surveyoptions.Value = myInnerItem.Fields["Value"].Value;
                    surveyoptions.Selection_Code = myInnerItem.Fields["Selection Code"].Value;


                    criteriaList.Add(surveyoptions);

                }


                surveyModel.OptionList = new List<Surveyoptions>();

                surveyModel.OptionList.AddRange(criteriaList);

            }

            survey.Questions = subcriteriaList;

            return PartialView("~/Views/IdealHome/_NewRequest.cshtml", survey);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Survey(Survey model)
        {
            if (ModelState.IsValid)
            {

                var response = SaveData(model);

                if (response.Payload.Length == 31)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEAL_HOME_SURVEY_THANKS);
                }
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEAL_HOME_DISPLAY_CUSTOMER);
        }

        [HttpGet]
        public ActionResult DisplayCustomers(IdealHomeCustomers model)
        {
            if (!IsLoggedIn)
            {
               return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOME_LOGIN);
            }
            else
            {
                return PartialView("~/Views/IdealHome/_DisplayCustomers.cshtml", GetCustomerList(model));
            }
        }
        #endregion

        #region Helpers
        private bool TryLogin(SurveyLogin model, out string error)
        {
            error = null;
            var sitecoreService = new SitecoreContext();

            var entityLogin = sitecoreService.Database.GetItem("{ED1AC403-8F40-4CA9-BF4E-B0EB40C6B386}");

            Session["loginItemID"] = null;

            foreach (Item useritem in entityLogin.GetChildren().ToList())
            {
                if ((useritem.Fields["UserName"].Value == model.Username) && (useritem.Fields["Password"].Value == model.Password))
                {
                    var currentUser = model.Username;

                    string imageUrl = string.Empty;

                    ImageField imageField = useritem.Fields["Logo"];
                    MediaItem image = null;

                    if (imageField.MediaItem != null)
                    {
                        image = new MediaItem(imageField.MediaItem);
                        model.MediaUrl = MediaManager.GetMediaUrl(image);
                    }

                    Session["UserName"] = model.Username;

                    model.UserItemID = useritem.ID;
                    model.EntityCode = useritem.Fields["Entity Code"].Value;

                    Session["LoginDetails"] = model;

                    //CacheProvider.Store(CacheKeys.IDEAL_HOME_USER_DATA, new AccessCountingCacheItem<SurveyLogin>(currentUser, Times.Max));
                    
                    AuthStateService.Save(new DewaProfile(model.Username, string.Empty, Roles.Partner));

                    return true;
                }
            }

            error = Translate.Text(DictionaryKeys.ProjectGeneration.InvalidAccount);
            return false;
        }
        
        private IdealHomeCustomers GetCustomerList(IdealHomeCustomers model)
        {

            var entitymodel = Session["LoginDetails"] as SurveyLogin;

            ViewBag.EntityCode = entitymodel.EntityCode;


            string year = "2017";
            string eFormQuery = "select * from ideal_home_survey where year = '" + year + "'";

            var response = eFormServiceClient.Query_Ework_DB(eFormQuery);

            List<IdealHomeCustomers> Items = new List<IdealHomeCustomers>();

            model.CustomerList = new List<IdealHomeCustomers>();

            foreach (DataRow row in response.Payload.Tables[0].Rows)
            {
                model = new IdealHomeCustomers();

                model.EfolderID = row["efolderid"].ToString();
                model.CustomerName = row["CUSTOMER_NAME"].ToString();
                model.CustomerName_AR = row["CUSTOMER_NAME_AR"].ToString();
                model.Evaluation = row["status"].ToString();
                

                //DM
                model.DM_1_VALUE = (row["DM_1_VALUE"] == DBNull.Value) ? 0 : Convert.ToDecimal(row["DM_1_VALUE"]);
                model.DM_TOTAL = (row["DM_1_VALUE"] == DBNull.Value) ? 0 : Convert.ToDecimal(row["DM_1_VALUE"]);
                model.DM_COMP = Convert.ToInt32(row["DM_COMP"]);

                //DUBAI POLICE
                model.DP_1_VALUE = (row["DP_1_VALUE"] == DBNull.Value) ? 0 : Convert.ToDecimal(row["DP_1_VALUE"]);
                model.DP_2_VALUE = (row["DP_2_VALUE"] == DBNull.Value) ? 0 : Convert.ToDecimal(row["DP_2_VALUE"]);
                model.DP_3_VALUE = (row["DP_3_VALUE"] == DBNull.Value) ? 0 : Convert.ToDecimal(row["DP_3_VALUE"]);
                model.DP_TOTAL = (row["DP_1_VALUE"] == DBNull.Value) ? 0 : Convert.ToDecimal(row["DP_1_VALUE"]) + Convert.ToDecimal(row["DP_2_VALUE"]) + Convert.ToDecimal(row["DP_3_VALUE"]);
                model.DP_COMP = Convert.ToInt32(row["DP_COMP"]);

                Items.Add(model);

            }

            model.CustomerList = Items;

            return model;
        }

        private ServiceResponse<string> SaveData(Survey model)
        {
            
            
            var entitymodel = Session["LoginDetails"] as SurveyLogin;

            string mapName = "Ideal_Home_Survey";
            string mapAction = "Evaluate";

            var sb = new StringBuilder();

            //Passing entity code
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""50""><Value>{0}</Value></FieldInput>", entitymodel.EntityCode ?? string.Empty));

            //Pass value for Dubai Police
            if (Session["UserName"].ToString() == "dubaipolice")
            {

                sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""3""><Value>{0}</Value></FieldInput>", Request.Form["ddlQuestion_1"].Code() ?? string.Empty));
                sb.Append(string.Format(@"<FieldInput Type=""DoubleField"" Field=""4""><Value>{0}</Value></FieldInput>", Request.Form["ddlQuestion_1"].Value() ?? string.Empty));

                sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""5""><Value>{0}</Value></FieldInput>", Request.Form["ddlQuestion_2"].Code() ?? string.Empty));
                sb.Append(string.Format(@"<FieldInput Type=""DoubleField"" Field=""6""><Value>{0}</Value></FieldInput>", Request.Form["ddlQuestion_2"].Value() ?? string.Empty));

                sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""7""><Value>{0}</Value></FieldInput>", Request.Form["ddlQuestion_3"].Code() ?? string.Empty));
                sb.Append(string.Format(@"<FieldInput Type=""DoubleField"" Field=""8""><Value>{0}</Value></FieldInput>", Request.Form["ddlQuestion_3"].Value() ?? string.Empty));
            }
            //Pass Value for Dubai Muncipality
            else if (Session["UserName"].ToString() == "dubaimunicipality")
            {
                sb.Append(string.Format(@"<FieldInput Type=""DoubleField"" Field=""1""><Value>{0}</Value></FieldInput>", Request.Form["ddlQuestion_1"].Value() ?? string.Empty));
                sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""2""><Value>{0}</Value></FieldInput>", Request.Form["ddlQuestion_1"].Code() ?? string.Empty));
            }

            //sb.Append(string.Format(@"<FieldInput Type=""DoubleField"" Field=""59""><Value>{0}</Value></FieldInput>", "111.12" ?? string.Empty));

            string folderID = Session["folderid"].ToString();
            var response = eFormServiceClient.UpdateForm(folderID, sb.ToString(), mapName, mapAction);

            return response;

        }
        #endregion
    }
}