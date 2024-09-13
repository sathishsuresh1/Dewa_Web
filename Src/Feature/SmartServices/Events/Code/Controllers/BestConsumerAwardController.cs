using DEWAXP.Feature.Events.Models.BestConsumerAward;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Integration;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DEWAXP.Feature.Events.Controllers
{
    public class BestConsumerAwardController : BaseController
    {
        private IEFormServiceClient EFormsServiceClient
        {
            get { return DependencyResolver.Current.GetService<IEFormServiceClient>(); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SubmitForm(BestConsumerAwardModel model)
        {
            if (ModelState.IsValid)
            {
                if (CheckAccount(model.ContractAccountNumber) == false)
                {
                    SaveData(model);
                    return PartialView("~/Views/Feature/Events/BestConsumerAward/_RequestSent.cshtml");
                }
                else
                {
                    return PartialView("~/Views/Feature/Events/BestConsumerAward/_RequestFailed.cshtml");
                }
            }
            else
            {
                model = GetDropDownList(model);
                return PartialView("~/Views/Feature/Events/BestConsumerAward/_NewRequest.cshtml", model);
            }
        }

        private BestConsumerAwardModel GetDropDownList(BestConsumerAwardModel model)
        {
            var residenceTypes = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.RESIDENCETYPELIST)).Items;
            var residenceItems = residenceTypes.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });

            model.ResidenceList = residenceItems.ToList();

            var acTypes = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.ACTYPELIST)).Items;
            var acItems = acTypes.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });

            model.ACList = acItems.ToList();

            return model;
        }

        [HttpGet]
        public ActionResult SubmitForm()
        {
            BestConsumerAwardModel model = new BestConsumerAwardModel();

            model.TypeAC = "null";
            model.TypeResidence = "null";

            return PartialView("~/Views/Feature/Events/BestConsumerAward/_NewRequest.cshtml", GetDropDownList(model));
        }

        private void SaveData(BestConsumerAwardModel model)
        {
            string mapName = "Best_Consumer_Award";
            string mapAction = "Best_Consumer_Award_Form";

            var sb = new StringBuilder();

            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""0""><Value>{0}</Value></FieldInput>", model.ContractAccountNumber ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""1""><Value>{0}</Value></FieldInput>", model.AccountOwnerName ?? string.Empty));

            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""2""><Value>{0}</Value></FieldInput>", model.MobileNumber ?? string.Empty));

            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""3""><Value>{0}</Value></FieldInput>", model.EmailAddress ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""4""><Value>{0}</Value></FieldInput>", model.ResidenceAddress ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""5""><Value>{0}</Value></FieldInput>", model.TypeResidence ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""NumberField"" Field=""6""><Value>{0}</Value></FieldInput>", model.NumberResidents ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""NumberField"" Field=""7""><Value>{0}</Value></FieldInput>", model.Numberbedrooms ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""NumberField"" Field=""8""><Value>{0}</Value></FieldInput>", model.Numberbathrooms ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""NumberField"" Field=""9""><Value>{0}</Value></FieldInput>", model.NumberCars ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""10""><Value>{0}</Value></FieldInput>", Sitecore.Context.Language.Name == "en" ? "EN" : "AR"));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""12""><Value>{0}</Value></FieldInput>", model.TypeAC ?? string.Empty));
            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""15""><Value>{0}</Value></FieldInput>", model.Measures ?? string.Empty));

            sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""16""><Value>{0}</Value></FieldInput>", model.SourceType ?? string.Empty));

            if (model.SourceType == "Educational Institution")
            {
                sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""17""><Value>{0}</Value></FieldInput>", model.InstituteName ?? string.Empty));
            }

            var response = EFormsServiceClient.SubmitNewForm(sb.ToString(), mapName, mapAction);
        }

        private bool CheckAccount(string accountNumber)
        {
            bool isExists = false;

            string eFormQuery = "select nvl(Acc_No,0) from ext_best_consumer_award where Acc_No = " + "'" + accountNumber + "'";

            var response = EFormsServiceClient.Query_Ework_DB(eFormQuery);

            if (response.Payload.Tables[0].Rows.Count != 0)
            {
                isExists = true;
            }
            return isExists;
        }
    }
}