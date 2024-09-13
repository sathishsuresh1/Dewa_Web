// <copyright file="greenribonpledgecontroller.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    using DEWAXP.Feature.GeneralServices.Models.GreenRibonPledge;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Logger;
    using Glass.Mapper;
    using global::Sitecore.Configuration;
    using global::Sitecore.Data;
    using global::Sitecore.Data.Items;
    using global::Sitecore.Globalization;
    using global::Sitecore.SecurityModel;
    using System;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    /// <summary>
    /// Defines the <see cref="GreenRibonPledgeController" />
    /// </summary>
    public class GreenRibonPledgeController : BaseController
    {
        /// <summary>
        /// The PledgeApartment
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpGet]
        public ActionResult PledgeApartment()
        {
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            ViewBag.NumberOfMinutesAppt = GetLstDataSource(DataSources.NumberOfMinutes).ToList();
            return View("~/Views/Feature/GeneralServices/GreenRibonPledge/_PledgeApartment.cshtml", null);
        }

        /// <summary>
        /// The PledgeApartment
        /// </summary>
        /// <param name="model">The model<see cref="PledgeForm"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PledgeApartment(PledgeForm model)
        {
            bool status = false;
            string recaptchaResponse = System.Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

            if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(recaptchaResponse))
            {
                status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
            }
            else if (!ReCaptchaHelper.Recaptchasetting())
            {
                status = true;
            }

            if (!status)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
            }

            if (ModelState.IsValid)
            {
                // create item based on model
                //if (model != null)

                using (new SecurityDisabler())
                {
                    Database masterDb = Factory.GetDatabase("master");

                    // first get the parent item from Sitecore
                    Item parentItem = masterDb.GetItem(SitecoreItemIdentifiers.J40_GreenPledgeFormEntriesFolder);

                    // now get the template from which the item will be created
                    TemplateItem template = masterDb.GetTemplate(SitecoreItemIdentifiers.J40_GreenPledgeFormEntriesTemplate);

                    // item name created based on Datetime, so to uniquely identify from each other
                    string itemName = GetPledgeEntryName();

                    Item newItem = parentItem.Add(itemName, template);

                    // start item editing
                    newItem.Editing.BeginEdit();
                    try
                    {
                        model.PledgeType = GreenPledgeTypes.Apartment.ToString();
                        newItem = InsertFormEnteries(newItem, model);

                        // lets find out the Starting pledge number
                        Item PledgeForm = masterDb.GetItem(SitecoreItemIdentifiers.J40_GreenPledgeForm);

                        var PledgeNo = PledgeForm.Fields["Set Global Pledge Number"].Value;

                        int NewPledgeNo = int.Parse(PledgeNo) + 1;

                        newItem.Fields["PledgeNo"].Value = NewPledgeNo.ToString();

                        // before we endediting, setting 3 other fields which were not part of the Model at Form Presentation
                        model.LblWaterBottlesSaved = newItem.Fields["LblWaterBottlesSaved"].Value;
                        model.LblTreesOffset = newItem.Fields["LblTreesOffset"].Value;
                        model.LblCflLamps = newItem.Fields["LblCflLamps"].Value;

                        newItem.Editing.EndEdit(); // this has now stored the item in cms

                        // lets update the Global plege no.
                        UpdatePledgeFormPledgeNumber(PledgeForm, NewPledgeNo);

                        //now allocate pledge no to the model
                        model.PledgeNumber = NewPledgeNo;

                        // Lets send the email to customer
                        if ((model.email != null) && (model.email.Length > 5))
                        {
                            SendEmailToCustomer(model);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the message on any failure to sitecore log
                        //Sitecore.Diagnostics.Log.Error("Could not update item " + newItem.Paths.FullPath + ": " + ex.Message, this);

                        // Cancel the edit (not really needed, as Sitecore automatically aborts
                        // the transaction on exceptions, but it wont hurt your code)
                        LogService.Fatal(ex, this);
                        newItem.Editing.CancelEdit();
                    }
                }
            }
            else
            {
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                    ViewBag.Recaptcha = true;
                }
                else
                {
                    ViewBag.Recaptcha = false;
                }
                ViewBag.NumberOfMinutesAppt = GetLstDataSource(DataSources.NumberOfMinutes).ToList();
                return View("~/Views/Feature/GeneralServices/GreenRibonPledge/_PledgeApartment.cshtml", model);
            }

            model.formdata = GetFormEntries(model);
            //redirect to success page or return a view to success page with message.
            return PartialView("~/Views/Feature/GeneralServices/GreenRibonPledge/_PledgeFormThankyou.cshtml", model);
        }

        /// <summary>
        /// The PledgeVilla
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpGet]
        public ActionResult PledgeVilla()
        {
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            ViewBag.NumberOfMinutes = GetLstDataSource(DataSources.NumberOfMinutes).ToList();
            return View("~/Views/Feature/GeneralServices/GreenRibonPledge/_PledgeVilla.cshtml", null);
        }

        /// <summary>
        /// The PledgeVilla
        /// </summary>
        /// <param name="model">The model<see cref="PledgeForm"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PledgeVilla(PledgeForm model)
        {
            bool status = false;
            string recaptchaResponse = System.Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

            if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(recaptchaResponse))
            {
                status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
            }
            else if (!ReCaptchaHelper.Recaptchasetting())
            {
                status = true;
            }
            if (!status)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
            }
            // create item based on model
            if (ModelState.IsValid)
            {
                using (new SecurityDisabler())
                {
                    Database masterDb = Factory.GetDatabase("master");

                    // first get the parent item from Sitecore
                    Item parentItem = masterDb.GetItem(SitecoreItemIdentifiers.J40_GreenPledgeFormEntriesFolder);

                    // now get the template from which the item will be created
                    TemplateItem template = masterDb.GetTemplate(SitecoreItemIdentifiers.J40_GreenPledgeFormEntriesTemplate);

                    // item name created based on Datetime, so to uniquely identify from each other
                    string itemName = GetPledgeEntryName();

                    Item newItem = parentItem.Add(itemName, template);

                    // start item editing
                    newItem.Editing.BeginEdit();
                    try
                    {
                        model.PledgeType = GreenPledgeTypes.Villa.ToString();
                        newItem = InsertFormEnteries(newItem, model);

                        // lets find out the Starting pledge number
                        Item PledgeForm = masterDb.GetItem(SitecoreItemIdentifiers.J40_GreenPledgeForm);

                        var PledgeNo = PledgeForm.Fields["Set Global Pledge Number"].Value;

                        int NewPledgeNo = int.Parse(PledgeNo) + 1;

                        newItem.Fields["PledgeNo"].Value = NewPledgeNo.ToString();

                        // before we endediting, setting 3 other fields which were not part of the Model at Form Presentation
                        model.LblWaterBottlesSaved = newItem.Fields["LblWaterBottlesSaved"].Value;
                        model.LblTreesOffset = newItem.Fields["LblTreesOffset"].Value;
                        model.LblCflLamps = newItem.Fields["LblCflLamps"].Value;

                        newItem.Editing.EndEdit(); // this has now stored the item in cms

                        // lets update the Global plege no.
                        UpdatePledgeFormPledgeNumber(PledgeForm, NewPledgeNo);

                        //now allocate pledge no to the model
                        model.PledgeNumber = NewPledgeNo;

                        // Lets send the email to customer
                        if ((model.email != null) && (model.email.Length > 5))
                        {
                            SendEmailToCustomer(model);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the message on any failure to sitecore log
                        //Sitecore.Diagnostics.Log.Error("Could not update item " + newItem.Paths.FullPath + ": " + ex.Message, this);

                        // Cancel the edit (not really needed, as Sitecore automatically aborts
                        // the transaction on exceptions, but it wont hurt your code)
                        LogService.Fatal(ex, this);
                        newItem.Editing.CancelEdit();
                    }
                }
            }
            else
            {
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                    ViewBag.Recaptcha = true;
                }
                else
                {
                    ViewBag.Recaptcha = false;
                }
                ViewBag.NumberOfMinutes = GetLstDataSource(DataSources.NumberOfMinutes).ToList();
                return View("~/Views/Feature/GeneralServices/GreenRibonPledge/_PledgeVilla.cshtml", model);
            }
            model.formdata = GetFormEntries(model);
            //redirect to success page or return a view to success page with message.
            return PartialView("~/Views/Feature/GeneralServices/GreenRibonPledge/_PledgeFormThankyou.cshtml", model);
        }

        // GET: GreenRibonPledge
        /// <summary>
        /// The PledgeForm
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpGet]
        public ActionResult PledgeForm()
        {
            return View("~/Views/Feature/GeneralServices/GreenRibonPledge/_PledgeForm.cshtml", null);
        }

        /// <summary>
        /// The PledgeForm
        /// </summary>
        /// <param name="model">The model<see cref="PledgeForm"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PledgeForm(PledgeForm model)
        {
            // create item based on model
            if (model != null)
            {
                using (new SecurityDisabler())
                {
                    Database masterDb = Factory.GetDatabase("master");

                    // first get the parent item from Sitecore
                    Item parentItem = masterDb.GetItem(SitecoreItemIdentifiers.J40_GreenPledgeFormEntriesFolder);

                    // now get the template from which the item will be created
                    TemplateItem template = masterDb.GetTemplate(SitecoreItemIdentifiers.J40_GreenPledgeFormEntriesTemplate);

                    // item name created based on Datetime, so to uniquely identify from each other
                    string itemName = GetPledgeEntryName();

                    Item newItem = parentItem.Add(itemName, template);

                    // start item editing
                    newItem.Editing.BeginEdit();
                    try
                    {
                        newItem = InsertFormEnteries(newItem, model);

                        // lets find out the Starting pledge number
                        Item PledgeForm = masterDb.GetItem(SitecoreItemIdentifiers.J40_GreenPledgeForm);

                        var PledgeNo = PledgeForm.Fields["Set Global Pledge Number"].Value;

                        int NewPledgeNo = int.Parse(PledgeNo) + 1;

                        newItem.Fields["PledgeNo"].Value = NewPledgeNo.ToString();

                        // before we endediting, setting 3 other fields which were not part of the Model at Form Presentation
                        model.LblWaterBottlesSaved = newItem.Fields["LblWaterBottlesSaved"].Value;
                        model.LblTreesOffset = newItem.Fields["LblTreesOffset"].Value;
                        model.LblCflLamps = newItem.Fields["LblCflLamps"].Value;

                        newItem.Editing.EndEdit(); // this has now stored the item in cms

                        // lets update the Global plege no.
                        UpdatePledgeFormPledgeNumber(PledgeForm, NewPledgeNo);

                        //now allocate pledge no to the model
                        model.PledgeNumber = NewPledgeNo;

                        // Lets send the email to customer
                        if ((model.email != null) && (model.email.Length > 5))
                        {
                            SendEmailToCustomer(model);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the message on any failure to sitecore log
                        //Sitecore.Diagnostics.Log.Error("Could not update item " + newItem.Paths.FullPath + ": " + ex.Message, this);

                        // Cancel the edit (not really needed, as Sitecore automatically aborts
                        // the transaction on exceptions, but it wont hurt your code)
                        LogService.Fatal(ex, this);
                        newItem.Editing.CancelEdit();
                    }
                }
            }
            model.formdata = GetFormEntries(model);
            //redirect to success page or return a view to success page with message.
            return PartialView("~/Views/Feature/GeneralServices/GreenRibonPledge/_PledgeFormThankyou.cshtml", model);
        }

        /// <summary>
        /// The UpdatePledgeFormPledgeNumber
        /// </summary>
        /// <param name="pledgeForm">The pledgeForm<see cref="Item"/></param>
        /// <param name="PledgeNo">The PledgeNo<see cref="int"/></param>
        public void UpdatePledgeFormPledgeNumber(Item pledgeForm, int PledgeNo)
        {
            foreach (var itemLanguage in pledgeForm.Languages)
            {
                var item = pledgeForm.Database.GetItem(pledgeForm.ID, itemLanguage);
                if (item.Versions.Count > 0)
                {
                    item.Editing.BeginEdit();
                    try
                    {
                        item.Fields["Set Global Pledge Number"].Value = PledgeNo.ToString();
                        item.Editing.EndEdit();
                    }
                    catch (Exception ex)
                    {
                        LogService.Fatal(ex, this);
                        item.Editing.CancelEdit();
                    }
                }
            }
        }

        /// <summary>
        /// The SendEmailToCustomer
        /// </summary>
        /// <param name="model">The model<see cref="PledgeForm"/></param>
        public void SendEmailToCustomer(PledgeForm model)
        {
            // lets pull in the Email template from cms
            Database masterDb = Factory.GetDatabase("master");
            Item emailTemplateItem = masterDb.GetItem(SitecoreItemIdentifiers.J40_GreenPledgeEmailTemplate);

            string Body = "";
            Body = this.CreateEmailBody(model, emailTemplateItem.Fields["Message"].Value);//string.Format(emailTemplateItem.Fields["Message"].Value, model.PledgeNumber));

            try
            {
                string from = emailTemplateItem.Fields["FromEmail"] != null
                    ? emailTemplateItem.Fields["FromEmail"].Value
                    : "no-reply@dewa.gov.ae";
                string subject = emailTemplateItem.Fields["EmailSubject"] != null
                    ? emailTemplateItem.Fields["EmailSubject"].Value
                    : "Pledge results";
                EmailServiceClient.SendEmail(from, model.email, subject, Body);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                //Response.Write(ex.ToString());
            }
        }

        /// <summary>
        /// The CreateEmailBody
        /// </summary>
        /// <param name="model">The model<see cref="PledgeForm"/></param>
        /// <param name="emailTemplate">The emailTemplate<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string CreateEmailBody(PledgeForm model, string emailTemplate)
        {
            emailTemplate = emailTemplate.Replace("{Name}", model.name);
            emailTemplate = emailTemplate.Replace("{PledgeNumber}", model.PledgeNumber.ToString());
            emailTemplate = emailTemplate.Replace("{Carbon}", model.LblKGOfCarbonDioxide);
            emailTemplate = emailTemplate.Replace("{Gallon}", model.LblGallonsOfWater);
            emailTemplate = emailTemplate.Replace("{Bottle}", model.LblWaterBottlesSaved);
            emailTemplate = emailTemplate.Replace("{KwElectricity}", model.LblKilloWattOfElectricity);
            emailTemplate = emailTemplate.Replace("{CflLamp}", model.LblCflLamps);
            emailTemplate = emailTemplate.Replace("{Trees}", model.LblTreesOffset);

            emailTemplate = emailTemplate.Replace("{Formdata}", GetFormEntries(model));
            return emailTemplate;
        }

        /// <summary>
        /// The GetFormEntries
        /// </summary>
        /// <param name="model">The model<see cref="PledgeForm"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string GetFormEntries(PledgeForm model)
        {
            StringBuilder formdata = new StringBuilder();
            if (model.ChkReplaceBulbs.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.EmailBulbPledgeText).Replace("#num#", model.NumberOfBulbs));
                formdata.Append("<br/><br/>");
            }
            if (model.ChkTempTo24.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.AirConditionerSetTemparture).Replace("#num#", model.ACTemperature));
                formdata.Append("<br/><br/>");
            }
            if (model.ChkMaintenanceTwice.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.AirConditionerMaintenance).Replace("#num#", model.Periodicmaintenance));
                formdata.Append("<br/><br/>");
            }
            if (model.ChkDryingClothes.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.OpenAirDryingClothes));
                formdata.Append("<br/><br/>");
            }
            if (model.ChkSolarWaterHeaters.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.EmailSolarWaterHeaters).Replace("#num#", model.NumberOfElectricHeaters));
                formdata.Append("<br/><br/>");
            }
            if (model.ChkBucketInsteadOfWaterHose.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.UseBucketInsteadOfWaterHose));
                formdata.Append("<br/><br/>");
            }
            if (model.ChkReduceMinutesInShower.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.EmailReduceMinutesInShower).Replace("#num#", model.EnterNumberOfMinutes));
                formdata.Append("<br/><br/>");
            }
            if (model.ChkFixWaterFlowReducer.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.FixWaterFlowReducer) + " " + model.NumberOfKits);
                formdata.Append("<br/><br/>");
            }
            if (model.ChkReduceAmountOfWater.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.ReduceAmountOfWater));
                formdata.Append("<br/><br/>");
            }
            if (model.ChkWaterTapWhileBrushing.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.WaterTapWhileBrushing) + " " + model.NumberOfPersons);
                formdata.Append("<br/><br/>");
            }
            if (model.ChkDewaGreenBox.HasValue())
            {
                formdata.Append(Translate.Text(DictionaryKeys.GreenRibonPledge.Labels.DewaGreenBox));
                formdata.Append("<br/><br/>");
            }

            return formdata.ToString();
        }

        /// <summary>
        /// The GetPledgeEntryName
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public string GetPledgeEntryName()
        {
            DateTime date = DateTime.Now;

            return ("PledgeEntry " + date.ToString("dd-MM-yy HH-mm-ss"));
        }

        /// <summary>
        /// The InsertFormEnteries
        /// </summary>
        /// <param name="item">The item<see cref="Item"/></param>
        /// <param name="model">The model<see cref="PledgeForm"/></param>
        /// <returns>The <see cref="Item"/></returns>
        public Item InsertFormEnteries(Item item, PledgeForm model)
        {
            item.Fields["ChkReplaceBulbs"].Value = model.ChkReplaceBulbs.HasValue() ? "1" : "0";
            item.Fields["NumberOfBulbs"].Value = model.NumberOfBulbs;
            item.Fields["ACTemperature"].Value = model.ACTemperature;
            item.Fields["Periodicmaintenance"].Value = model.Periodicmaintenance;
            item.Fields["ChkTempTo24"].Value = model.ChkTempTo24.HasValue() ? "1" : "0";
            item.Fields["ChkMaintenanceTwice"].Value = model.ChkMaintenanceTwice.HasValue() ? "1" : "0";
            item.Fields["ChkDryingClothes"].Value = model.ChkDryingClothes.HasValue() ? "1" : "0";
            item.Fields["ChkSolarWaterHeaters"].Value = model.ChkSolarWaterHeaters.HasValue() ? "1" : "0";
            item.Fields["NumberOfElectricHeaters"].Value = model.NumberOfElectricHeaters;
            item.Fields["ChkBucketInsteadOfWaterHose"].Value = model.ChkBucketInsteadOfWaterHose.HasValue() ? "1" : "0";
            item.Fields["ChkReduceMinutesInShower"].Value = model.ChkReduceMinutesInShower.HasValue() ? "1" : "0";
            item.Fields["EnterNumberOfMinutes"].Value = model.EnterNumberOfMinutes;
            item.Fields["ChkFixWaterFlowReducer"].Value = model.ChkFixWaterFlowReducer.HasValue() ? "1" : "0";
            item.Fields["NumberOfKits"].Value = model.NumberOfKits;
            item.Fields["ChkReduceAmountOfWater"].Value = model.ChkReduceAmountOfWater.HasValue() ? "1" : "0";
            item.Fields["ChkWaterTapWhileBrushing"].Value = model.ChkWaterTapWhileBrushing.HasValue() ? "1" : "0";
            item.Fields["NumberOfPersons"].Value = model.NumberOfPersons;
            item.Fields["ChkDewaGreenBox"].Value = model.ChkDewaGreenBox.HasValue() ? "1" : "0";
            item.Fields["LblKGOfCarbonDioxide"].Value = model.LblKGOfCarbonDioxide;
            item.Fields["LblKilloWattOfElectricity"].Value = model.LblKilloWattOfElectricity;
            item.Fields["LblGallonsOfWater"].Value = model.LblGallonsOfWater;
            item.Fields["name"].Value = model.name;
            item.Fields["account_number"].Value = model.account_number;
            item.Fields["contact_number"].Value = model.contact_number;
            item.Fields["email"].Value = model.email;
            item.Fields["Pledge Type"].Value = model.PledgeType;

            // 3 item calculations needs doing in here as they were not done in at front end.

            // WaterBottlesSaved Calculation
            double OneGallon = 4.546, WaterBottlesSaved = 0;

            double SavingLiterPerYear = ((int.Parse(model.LblGallonsOfWater) * OneGallon));

            double SavingKwPerYear = int.Parse(model.LblKilloWattOfElectricity);

            if (model.LblGallonsOfWater.Length > 0)
            {
                WaterBottlesSaved = Math.Round(int.Parse(model.LblGallonsOfWater) * 9.09218); //getWaterBottle(model);
            }
            item.Fields["LblWaterBottlesSaved"].Value = WaterBottlesSaved.ToString();

            // CFL LAMPS Calculation
            double CflLamps = 0;
            if (model.LblKilloWattOfElectricity.Length > 0)
            {
                CflLamps = Math.Round(SavingKwPerYear / 16.2);
            }
            item.Fields["LblCflLamps"].Value = CflLamps.ToString();

            // Tree Off set calculation.
            double TreeOffSet = 0;
            //TreeOffSet = Math.Round(((SavingLiterPerYear * 0.0045517) + (SavingKwPerYear * 0.4491)) * 0.001136);
            TreeOffSet = Math.Round(int.Parse(model.LblKGOfCarbonDioxide) * 1.136);
            item.Fields["LblTreesOffset"].Value = TreeOffSet.ToString();

            return item;
        }

        /// <summary>
        /// The getWaterBottle
        /// </summary>
        /// <param name="model">The model<see cref="PledgeForm"/></param>
        /// <returns>The <see cref="double"/></returns>
        private double getWaterBottle(PledgeForm model)
        {
            double savings = 0;
            if (model.ChkBucketInsteadOfWaterHose.HasValue())
            {
                savings = 15600;
            }
            if (model.ChkReduceMinutesInShower.HasValue() && model.EnterNumberOfMinutes != null && model.EnterNumberOfMinutes.HasValue())
            {
                double mins = double.Parse(model.EnterNumberOfMinutes) * 2190;
                savings = savings + mins;
            }
            if (model.ChkFixWaterFlowReducer.HasValue() && model.NumberOfKits != null && model.NumberOfKits.HasValue())
            {
                double kits = double.Parse(model.NumberOfKits) * 10950;
                savings = savings + kits;
            }
            if (model.ChkReduceAmountOfWater.HasValue())
            {
                savings = savings + 547.5;
            }
            if (model.ChkWaterTapWhileBrushing.HasValue() && model.NumberOfPersons != null && model.NumberOfPersons.HasValue())
            {
                double persons = double.Parse(model.NumberOfPersons) * 7300;
                savings = savings + persons;
            }
            return Math.Round(savings / 1.5);
        }
    }
}