using DEWAXP.Feature.Events.Models.ERMConference;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Integration;
using DEWAXP.Foundation.Integration.Responses;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DEWAXP.Feature.Events.Controllers
{
    public class ERMConferenceController : BaseController
    {
        private IEFormServiceClient EFormsServiceClient
        {
            get { return DependencyResolver.Current.GetService<IEFormServiceClient>(); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ERMConferenceModel model)
        {
            if (ModelState.IsValid)
            {
                if (CheckAccount(model.EmailAddress) == false)
                {
                    var response = SaveData(model);

                    if (response.Payload.Length == 31)
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CONFERENCE_THANKS);
                    }
                    else
                    {
                        model = GetDropDownList(model);

                        ModelState.AddModelError(string.Empty, response.Message);

                        return PartialView("~/Views/Feature/Events/ERMConference/_NewRequest.cshtml", model);
                    }
                }
                else
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CONFERENCE_ERROR);
                }
            }
            else
            {
                model = GetDropDownList(model);
                return PartialView("~/Views/Feature/Events/ERMConference/_NewRequest.cshtml", model);
            }
        }

        private ERMConferenceModel GetDropDownList(ERMConferenceModel model)
        {
            var titleTypes = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.TITLELIST)).Items;
            var titleItems = titleTypes.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
            model.TitleList = titleItems.ToList();

            var industryTypes = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.INDUSTRYLIST)).Items;
            var industryItems = industryTypes.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
            model.IndustryList = industryItems.ToList();

            return model;
        }

        [HttpGet]
        public ActionResult SubmitForm()
        {
            ERMConferenceModel model = new ERMConferenceModel();

            return PartialView("~/Views/Feature/Events/ERMConference/_NewRequest.cshtml", GetDropDownList(model));
        }

        private ServiceResponse<string> SaveData(ERMConferenceModel model)
        {
            string mapName = "ERMConference";
            string mapAction = "Submit";
            //string formName = "Conference Registration";
            //int mapVersion = 0;

            var sb = new StringBuilder();

            //sb.Append(string.Format(@"<FieldInput Field=""ddlTitle""><Value>{0}</Value></FieldInput>", model.Title ?? string.Empty));

            //sb.Append(string.Format(@"<FieldInput Field=""txtFirstName""><Value>{0}</Value></FieldInput>", model.FirstName ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtMiddleName""><Value>{0}</Value></FieldInput>", model.MiddleName ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtLastName""><Value>{0}</Value></FieldInput>", model.LastName ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtContNo""><Value>{0}</Value></FieldInput>", model.TelephoneNumber ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtFaxNo""><Value>{0}</Value></FieldInput>", model.FaxNumber ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtMobileNo""><Value>{0}</Value></FieldInput>", model.MobileNumber ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtEmail""><Value>{0}</Value></FieldInput>", model.EmailAddress ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""ddlNationality""><Value>{0}</Value></FieldInput>", model.Nationality ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""gender""><Value>{0}</Value></FieldInput>", model.Gender ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtCompany""><Value>{0}</Value></FieldInput>", model.Company ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtCompWS""><Value>{0}</Value></FieldInput>", model.CompanyWebsite ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtDepartment""><Value>{0}</Value></FieldInput>", model.Department ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtJobTitle""><Value>{0}</Value></FieldInput>", model.JobTitle ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""ddlCountry""><Value>{0}</Value></FieldInput>", model.Country ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtPoBox""><Value>{0}</Value></FieldInput>", model.POBOX ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtAddress""><Value>{0}</Value></FieldInput>", model.Address ?? string.Empty));
            //sb.Append(string.Format(@"<FieldInput Field=""txtindustry""><Value>{0}</Value></FieldInput>", model.Industry ?? string.Empty));

            //if (model.IsInterested == "true")
            //{
            //	sb.Append(string.Format(@"<FieldInput Field=""IsInterested""><Value>{0}</Value></FieldInput>", model.IsInterested ?? string.Empty));
            //}
            //else
            //{
            //	sb.Append(string.Format(@"<FieldInput Field=""IsInterested""><Value>{0}</Value></FieldInput>", "false" ?? string.Empty));
            //}

            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""2""><Value>{0}</Value></FieldInput>", model.FirstName ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""3""><Value>{0}</Value></FieldInput>", model.EmailAddress ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""4""><Value>{0}</Value></FieldInput>", model.TelephoneNumber ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""5""><Value>{0}</Value></FieldInput>", model.Nationality ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""6""><Value>{0}</Value></FieldInput>", model.LastName ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""7""><Value>{0}</Value></FieldInput>", model.MobileNumber ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""8""><Value>{0}</Value></FieldInput>", model.Gender ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""9""><Value>{0}</Value></FieldInput>", model.Title ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""10""><Value>{0}</Value></FieldInput>", model.FaxNumber ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""11""><Value>{0}</Value></FieldInput>", model.MiddleName ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""12""><Value>{0}</Value></FieldInput>", model.Company ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""13""><Value>{0}</Value></FieldInput>", model.CompanyWebsite ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""14""><Value>{0}</Value></FieldInput>", model.JobTitle ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""15""><Value>{0}</Value></FieldInput>", model.Department ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""16""><Value>{0}</Value></FieldInput>", model.Country ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""17""><Value>{0}</Value></FieldInput>", model.POBOX ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""18""><Value>{0}</Value></FieldInput>", model.Address ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""20""><Value>{0}</Value></FieldInput>", model.Industry ?? string.Empty));

            if (model.IsInterested == "true")
            {
                sb.Append(string.Format(@"<FieldInput Type=""CheckField"" Field=""21""><Value>{0}</Value></FieldInput>", "true" ?? string.Empty));
            }
            else
            {
                sb.Append(string.Format(@"<FieldInput Type=""CheckField"" Field=""21""><Value>{0}</Value></FieldInput>", "false" ?? string.Empty));
            }

            var response = EFormsServiceClient.SubmitNewForm(sb.ToString(), mapName, mapAction);

            return response;
        }

        private bool CheckAccount(string emailAddress)
        {
            bool isExists = false;
            string validation_date = string.Empty;

            string _submissionDate = RegistrationDate();

            string eFormQuery = "select * from ERMCONFERENCE where lower(txtEmail)=lower('" + emailAddress + "') and submit_date >= to_date('" + _submissionDate + " 00:00:00','mm/dd/yyyy HH24:MI:SS')";

            var response = EFormsServiceClient.Query_Ework_DB(eFormQuery);

            if (response.Payload.Tables[0].Rows.Count != 0)
            {
                isExists = true;
            }
            return isExists;
        }

        /// <summary>
        /// Get Submission Date from Sitecore
        /// </summary>
        /// <returns></returns>
        private string RegistrationDate()
        {
            var _ermConferenceItem = Sitecore.Context.Database.GetItem(SitecoreItemIdentifiers.GLOBAL_ERM_CONFERENCE);

            var _submissionDate = _ermConferenceItem.Fields["Submission Start Date"].Value;

            return _submissionDate;
        }
    }
}