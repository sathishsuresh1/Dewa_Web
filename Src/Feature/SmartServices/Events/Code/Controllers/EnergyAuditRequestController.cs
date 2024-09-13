using DEWAXP.Feature.Events.Models.EnergyAudit;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration;
using DEWAXP.Foundation.Integration.Enums;
using Sitecore.Globalization;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DEWAXP.Feature.Events.Controllers
{
    public class EnergyAuditRequestController : BaseController
    {
        private IEFormServiceClient EFormsServiceClient
        {
            get { return DependencyResolver.Current.GetService<IEFormServiceClient>(); }
        }

        //private IDubaiModelServiceClient DubaiModelServiceClient
        //{
        //    get { return DependencyResolver.Current.GetService<IDubaiModelServiceClient>(); }
        //}

        [HttpGet]
        public ActionResult InitialBuildingInformation()
        {
            CacheProvider.Remove(CacheKeys.ENERGY_AUDIT_BUILDING_INFO);
            CacheProvider.Remove(CacheKeys.ENERGY_AUDIT_CONFIRMATION_CODE);
            if (this.IsMaxBuildingSet())
            {
                return PartialView("~/Views/Feature/Events/EnergyAuditRequest/MaxRequestSent.cshtml", new EnergyAuditViewModel());
            }
            return PartialView("~/Views/Feature/Events/EnergyAuditRequest/_InitialBuildingInformation.cshtml", new EnergyAuditViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InitialBuildingInformation(EnergyAuditViewModel model)
        {
            if (model != null && model.InitialBuilding != null)
            {
                var response = DubaiModelServiceClient.GetAccountClassification(model.InitialBuilding.ContractAccountNumber, RequestLanguage, Request.Segment());
                if (!response.Succeeded || response.Payload == null)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text(DictionaryKeys.BuildingAudit.ContractAccountNotEligible));
                }

                if (response.Payload != null && (response.Payload.BillingClass == BillingClassification.Residential && response.Payload.PremiseType != GenericConstants.LABOURCAMPPREMISETYPE))
                {
                    ModelState.AddModelError(string.Empty, Translate.Text(DictionaryKeys.BuildingAudit.ContractAccountNotEligible));
                }

                if (ModelState.IsValid)
                {
                    CacheProvider.Store(CacheKeys.ENERGY_AUDIT_BUILDING_INFO, new CacheItem<EnergyAuditViewModel>(model));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J85_BUILDING_INFO);
                }
            }
            return PartialView("~/Views/Feature/Events/EnergyAuditRequest/_InitialBuildingInformation.cshtml", model);
        }

        [HttpGet]
        public ActionResult BuildingInformation()
        {
            EnergyAuditViewModel state;
            if (!CacheProvider.TryGet(CacheKeys.ENERGY_AUDIT_BUILDING_INFO, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J85_INITIAL_BUILDING_INFO);
            }
            return PartialView("~/Views/Feature/Events/EnergyAuditRequest/_BuildingInformation.cshtml", state);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BuildingInformation(EnergyAuditViewModel model)
        {
            if (!model.Buildings.Any())
            {
                return PartialView("~/Views/Feature/Events/EnergyAuditRequest/_BuildingInformation.cshtml");
            }

            CacheProvider.Store(CacheKeys.ENERGY_AUDIT_BUILDING_INFO, new CacheItem<EnergyAuditViewModel>(model));

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J85_CUSTOMER_DETAILS);
        }

        [HttpGet]
        public ActionResult CustomerInformation()
        {
            EnergyAuditViewModel state;
            if (!CacheProvider.TryGet(CacheKeys.ENERGY_AUDIT_BUILDING_INFO, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J85_BUILDING_INFO);
            }
            return PartialView("~/Views/Feature/Events/EnergyAuditRequest/_CustomerInformation.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CustomerInformation(CustomerDetails model)
        {
            EnergyAuditViewModel state;
            if (!CacheProvider.TryGet(CacheKeys.ENERGY_AUDIT_BUILDING_INFO, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J85_BUILDING_INFO);
            }

            if (ModelState.IsValid)
            {
                int numberofbuildings = state.Buildings.Sum(c => c.NumberOfBuildingsToBeAudited);
                var sb = new StringBuilder();
                //sb.Append(string.Format(@"<FieldInput Field=""lang""><Value>{0}</Value></FieldInput>", ContextItem.Language.Name == "en" ? "EN" : "AR"));
                //sb.Append(string.Format(@"<FieldInput Field=""Customer_name""><Value>{0}</Value></FieldInput>", model.CustomerName ?? string.Empty));
                //sb.Append(string.Format(@"<FieldInput Field=""Contact_name""><Value>{0}</Value></FieldInput>", model.ContactPerson ?? string.Empty));
                //sb.Append(string.Format(@"<FieldInput Field=""No_of_buildings""><Value>{0}</Value></FieldInput>", numberofbuildings));
                //sb.Append(string.Format(@"<FieldInput Field=""Contact_mobile""><Value>{0}</Value></FieldInput>", model.ContactMobile));
                //sb.Append(string.Format(@"<FieldInput Field=""Contact_land_line""><Value>{0}</Value></FieldInput>", model.ContactTelephone ?? string.Empty));
                //sb.Append(string.Format(@"<FieldInput Field=""Contact_email""><Value>{0}</Value></FieldInput>", model.ContactEmail));
                //sb.Append(string.Format(@"<FieldInput Field=""Address""><Value>{0}</Value></FieldInput>", model.Address ?? string.Empty));
                //sb.Append(@"<FieldInput Field=""Building_info""><Value>");

                //Modified by Syed Shujaat Ali
                //New eForm WebService Function.
                sb.Append(string.Format(@"<FieldInput Type=""DropDownField"" Field=""10""><Value>{0}</Value></FieldInput>", Sitecore.Context.Item.Language.Name == "en" ? "EN" : "AR"));
                sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""0""><Value>{0}</Value></FieldInput>", model.CustomerName ?? string.Empty));
                sb.Append(string.Format(@"<FieldInput Type=""TextField""  Field=""3""><Value>{0}</Value></FieldInput>", model.ContactPerson ?? string.Empty));
                sb.Append(string.Format(@"<FieldInput Type=""NumberField"" Field=""2""><Value>{0}</Value></FieldInput>", numberofbuildings));
                sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""5""><Value>{0}</Value></FieldInput>", model.ContactMobile));
                sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""6""><Value>{0}</Value></FieldInput>", model.ContactTelephone ?? string.Empty));
                sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""4""><Value>{0}</Value></FieldInput>", model.ContactEmail));
                sb.Append(string.Format(@"<FieldInput Type=""TextField"" Field=""7""><Value>{0}</Value></FieldInput>", model.Address ?? string.Empty));
                sb.Append(@"<FieldInput Type=""MemoField"" Field=""1""><Value>");

                foreach (var building in state.Buildings)
                {
                    sb.Append(building.ContractAccountNumber + (char)32 + (char)32 + (char)32 + building.BuildingName + (char)32 + (char)32 + (char)32 + building.FloorArea + (char)32 + (char)32 + (char)32 + building.Detail.Replace(" & ", "&amp;") + '\n');
                }

                sb.Append(@"</Value></FieldInput>");

                var response = EFormsServiceClient.SubmitNewForm(sb.ToString(), "Energy_Audit_Request", "Submit");

                if (response.Succeeded && response.Payload != null && response.Payload.Length == 31)
                {
                    CacheProvider.Store(CacheKeys.ENERGY_AUDIT_CONFIRMATION_CODE, new CacheItem<string>(response.Payload));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J85_CONFIRMATION);
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            return PartialView("~/Views/Feature/Events/EnergyAuditRequest/_CustomerInformation.cshtml", model);
        }

        [HttpGet]
        public ActionResult Confirmation()
        {
            string confirmCode;
            if (!CacheProvider.TryGet(CacheKeys.ENERGY_AUDIT_CONFIRMATION_CODE, out confirmCode))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J85_INITIAL_BUILDING_INFO);
            }

            ViewBag.Code = confirmCode;

            return PartialView("~/Views/Feature/Events/EnergyAuditRequest/_AuditRequestConfirmation.cshtml");
        }

        /// <summary>
        /// Get max building
        /// </summary>
        /// <returns></returns>
        private string GetMaxiBuilding()
        {
            var _energyAuditglobalItemField = Sitecore.Context.Database.GetItem(SitecoreItemIdentifiers.GLOBAL_ENERGYAUDIT_MAX_REQUEST);

            var _maxBuildingRequest = _energyAuditglobalItemField.Fields["Max Number"].Value;

            return _maxBuildingRequest;
        }

        private bool IsMaxBuildingSet()
        {
            string Sql = "select nvl(sum(no_of_buildings),0) as SOB from energy_audit_request where isvalid=1 and to_char(submit_date ,'yyyy')=" + DateTime.Now.Year;

            var response = EFormsServiceClient.Query_Ework_DB(Sql);
            if (response != null && response.Payload != null)
            {
                System.Data.DataSet Ds = response.Payload;
                int sumAuditRequest = Convert.ToInt32(Ds.Tables[0].Rows[0]["SOB"].ToString());

                return sumAuditRequest > Convert.ToInt32(GetMaxiBuilding());
            }
            return false;
        }
    }
}