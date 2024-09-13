using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Feature.SupplyManagement.Models.MoveIn;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Glass.Mapper.Sc;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Sitecorex = Sitecore.Context;
using DEWAXP.Foundation.Content.Models.SupplyManagement.Movein;
using Sitecore.ContentSearch.Extracters.IFilterTextExtraction;



namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class MoveInController : BaseMoveInController
    {
        #region Actions

        #region Anonymous

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult AccountRegister()
        {
            if (this.IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
            }
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.MOVEIN_ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.MOVEIN_ERROR_MESSAGE);
            }
            MoveInViewModelv3 model = new MoveInViewModelv3();
            string journey = string.Empty;
            if (!CacheProvider.TryGet(CacheKeys.MOVEIN_JOURNEY, out journey) && string.IsNullOrWhiteSpace(errorMessage))
            {
                CacheProvider.Remove(CacheKeys.MOVE_IN_3_WORKFLOW_STATE);
            }
            else
            {
                model = State;
            }

            model.loggedinuser = this.IsLoggedIn ? true : false;
            if (model.loggedinuser)
            {
                model.PBusinessPartners = GetPersonBusinessPartners();
                model.OBusinessPartners = GetOrganizationBusinessPartners();
                model.GBusinessPartners = GetGovBusinessPartners();
            }
            model.CustomerTypeList = GetCustomerTypes();
            model.AccountTypeList = GetAccountTypes();
            model.OwnerTypeList = GetOwnerType();
            model.NumberOfRoomsList = GetNumberOfRooms();
            return View("~/Views/Feature/SupplyManagement/MoveIn/AccountRegister.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AccountRegister(MoveInViewModelv3 model)
        {
            //if (ModelState.IsValid)
            {
                //Added for Future Center
                var _fc = FetchFutureCenterValues();
                if (model.CustomerCategory.Equals("P"))
                {
                    model.BusinessPartner = model.PBusinessPartner;
                }
                else if (model.CustomerCategory.Equals("G"))
                {
                    model.BusinessPartner = model.GBusinessPartner;
                }
                else if (model.CustomerCategory.Equals("O"))
                {
                    model.BusinessPartner = model.OBusinessPartner;
                }
                if (!string.IsNullOrWhiteSpace(model.strPremiseNumberdetails))
                {
                    model.PremiseNumberdetails = CustomJsonConvertor.DeserializeObject<List<AddedPremiseDetails>>(model.strPremiseNumberdetails);

                    // model.PremiseNumberdetails = JsonConvert.DeserializeObject<List<AddedPremiseDetails>>(model.strPremiseNumberdetails);
                }
                //Save to cache
                model.CustomerType = model.CustomerCategory != "P" ? string.Empty : model.CustomerType;
                model.Propertyid = model.Owner && !model.CustomerCategory.Equals("G") ? model.Propertyid : string.Empty;
                //model.PremiseAccount = model.PremiseAccount != null && model.PremiseAccount.Length > 0 ? model.PremiseAccount : Enumerable.Range(0, 1).Select(x => model.PremiseNo).ToArray();
                model.PremiseAccount = model.PremiseNumberdetails != null && model.PremiseNumberdetails.Count > 0 ? model.PremiseNumberdetails.Select(x => x.premisenumber).ToArray() : Enumerable.Range(0, 1).Select(x => model.PremiseNo).ToArray();
                Save(model);

                try
                {
                    var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                    {
                        premiseDetailsList = model.PremiseAccount.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new premiseDetails { premise = x }).ToArray(),
                        moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           customercategory=model.CustomerCategory,
                           customertype= !string.IsNullOrEmpty(model.CustomerTypeForNonIndividual) ? model.CustomerTypeForNonIndividual : model.CustomerType,
                           accounttype =model.AccountType,
                           occupiedby=model.Owner?(model.occupiedbyowner?"O":"T"):string.Empty,
                           businesspartnernumber=model.BusinessPartner,
                           ejarinumber=!model.Owner?(model.isEjari?model.Ejari:string.Empty):string.Empty,
                           moveindate = model.MoveinStartDate.HasValue ? model.MoveinStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           contractnumber=model.Owner?model.Purchase_TitleDeed :( model.isEjari!=true? model.Tenancy_Contract_Number:string.Empty),
                           tenancystartdate=model.ContractStartDate.HasValue?model.ContractStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancyenddate=model.ContractEndDate.HasValue?model.ContractEndDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancycontractvalue=model.isEjari!=true?model.ContractValue:string.Empty,
                           noofrooms=(model.Owner || model.isEjari!=true)?model.NumberOfRooms.ToString():string.Empty,
                           vatnumber=model.Vatnumber,
                           propertyid = model.Propertyid,
                           center =_fc.Branch
                       }
                    },
                        userid = CurrentPrincipal.UserId,
                        sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                        lang = RequestLanguage.Code(),
                        executionflag = "R",
                        channel = "A",
                        applicationflag = "M"
                    }, Request.Segment());

                    if (response.Succeeded && response.Payload != null && response.Payload.moveinnotif != null)
                    {
                        if (this.IsLoggedIn)
                        {
                            State.UserId = CurrentPrincipal.UserId;
                            State.FirstUserId = CurrentPrincipal.UserId;
                        }
                        else
                        {
                            State.UserId = response.Payload.moveinnotif.userid;
                            State.FirstUserId = response.Payload.moveinnotif.userid;
                        }
                        State.SecurityDeposit = response.Payload.moveinnotif.totalsecuritydepositamount;
                        State.InnovationFee = response.Payload.moveinnotif.totalinnovationfeesamount;
                        State.KnowledgeFee = response.Payload.moveinnotif.totalknowledgefeesamount;
                        State.TotalOutstandingFee = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.totaloutstandingamount) ? (double.Parse(response.Payload.moveinnotif.totaloutstandingamount) > 0.0 ? double.Parse(response.Payload.moveinnotif.totaloutstandingamount).ToString() : "0") : "0";
                        State.ReconnectionRegistrationFee = (double.Parse(response.Payload.moveinnotif.totalreconnectionchargeamount ?? "0")).ToString();
                        State.AddressRegistrationFee = (double.Parse(response.Payload.moveinnotif.totaladdresschangeamount ?? "0")).ToString();
                        State.ReconnectionVATrate = response.Payload.moveinnotif.reconnectionvatpercentage;
                        State.ReconnectionVATamt = (double.Parse(response.Payload.moveinnotif.reconnectionvatamount ?? "0")).ToString();
                        State.AddressVATrate = response.Payload.moveinnotif.addresschangevatpercentage;
                        State.AddressVAtamt = (double.Parse(response.Payload.moveinnotif.addresschangevatamount ?? "0")).ToString();
                        State.FirstName = response.Payload.moveinnotif.firtname;
                        State.LastName = response.Payload.moveinnotif.lastname;
                        State.MobilePhone = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.mobile) ? response.Payload.moveinnotif.mobile.First().Equals('0') ? response.Payload.moveinnotif.mobile.Remove(0, 1) : response.Payload.moveinnotif.mobile : string.Empty;
                        State.EmailAddress = response.Payload.moveinnotif.email;
                        State.unmaskedMobilePhone = response.Payload.moveinnotif.unmaskedmobile;
                        State.unmaskedEmailAddress = response.Payload.moveinnotif.unmaskedemail;
                        State.Nationality = response.Payload.moveinnotif.nationality;
                        State.PoBox = response.Payload.moveinnotif.pobox;
                        State.BusinessPartner = !string.IsNullOrWhiteSpace(model.BusinessPartner) ? model.BusinessPartner : response.Payload.moveinnotif.businesspartnernumber;
                        State.IdType = response.Payload.moveinnotif.idtype;
                        State.AttachmentFlag = string.IsNullOrWhiteSpace(response.Payload.moveinnotif.idnumber);//((string.Equals(model.IdType, "ED") && string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname)) || (string.Equals(model.IdType, "PN") && string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname)) || (string.Equals(model.IdType, "TN") && string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname))|| (string.Equals(model.IdType, "IA") && string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname))) ? true : false;
                        State.SkiptoPayment = response.Payload.moveinnotif.skippayment;
                        State.ShowDiscount = response.Payload.moveinnotif.showdiscount;
                        State.Guranteeletterflag = response.Payload.moveinnotif.guranteeletterflag;
                        if (State.CustomerCategory == "P" && !string.IsNullOrEmpty(State.IdType))
                        {
                            State.IdType = "ED";
                        }
                        else if (State.CustomerCategory == "O" && !string.IsNullOrEmpty(State.IdType))
                        {
                            State.IdType = "TN";
                        }
                        State.IdNumber = response.Payload.moveinnotif.idnumber;
                        State.IsIdNumber = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.idnumber);



                        if (State.IdType.Equals("ED") && !string.IsNullOrWhiteSpace(State.IdNumber))
                        {
                            State.IdNumber = string.Concat(response.Payload.moveinnotif.idnumber.Where(char.IsDigit));
                            State.IsIdNumber = !string.IsNullOrWhiteSpace(string.Concat(response.Payload.moveinnotif.idnumber.Where(char.IsDigit)));



                        }
                        DateTime _idExpiry = DateTime.MinValue;
                        DateTime _dob = DateTime.MinValue;

                        DateTime.TryParse(response.Payload.moveinnotif.idexpiry, out _idExpiry);
                        string _sendIdExpiry = _idExpiry == DateTime.MinValue ? string.Empty : _idExpiry.ToString("dd MMMM yyyy");

                        DateTime.TryParse(response.Payload.moveinnotif.dateofbirth, out _dob);
                        string _sendDOB = _dob == DateTime.MinValue ? string.Empty : _dob.ToString("dd MMMM yyyy");
                        if (_idExpiry != DateTime.MinValue)
                            State.ExpiryDate = _idExpiry.ToString("dd MMMM yyyy");
                        if (_dob != DateTime.MinValue)
                            State.DateOfBirth = _dob.ToString("dd MMMM yyyy");

                        //State.Isfirstname = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname);
                        // State.Islastname = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.lastname);
                        State.IssuingAuthority = response.Payload.moveinnotif.idauthority;
                        State.IssuingAuthorityName = response.Payload.moveinnotif.issuingauthorityothers;
                        State.departmentnameothers = response.Payload.moveinnotif.departmentnameothers;
                        State.departmentnameid = response.Payload.moveinnotif.departmentid;
                        State.IsDeptid = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.departmentid);
                        State.Emirate = response.Payload.moveinnotif.region;
                        State.Vatnumber = response.Payload.moveinnotif.vatnumber;
                        State.IsVATNumber = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.vatnumber);

                        State.Isfirstname = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname);
                        State.Islastname = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.lastname);
                        State.IsMobilePhone = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.unmaskedmobile);
                        State.IsEmailAddress = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.email);
                        State.IsunmaskedMobilePhone = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.unmaskedmobile);
                        State.IsunmaskedEmailAddress = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.unmaskedemail);
                        State.IsNationality = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.nationality);
                        State.IsPoBox = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.pobox);
                        State.Isbp = !string.IsNullOrWhiteSpace(State.BusinessPartner);
                        State.IsEmirate = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.region);
                        State.CustomerTypeForNonIndividual = !string.IsNullOrEmpty(model.CustomerTypeForNonIndividual) ? model.CustomerTypeForNonIndividual : string.Empty;

                        Save();
                        CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));

                        if (this.IsLoggedIn)
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILSv3);
                        }
                        else
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILSv3);
                        }
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.MOVEIN_ERROR_MESSAGE, new CacheItem<string>(response.Message));

                        if (response.Payload.responsecode != "399")
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
            }
        }

        [HttpGet]
        public ActionResult ContactDetails()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.MOVEIN_ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.MOVEIN_ERROR_MESSAGE);
            }
            if (!InProgress())
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
            }
            try
            {
                var response = DewaApiClient.SetMoveInReadRequest(new moveInReadInput
                {
                    lang = RequestLanguage.Code(),
                    userid = State.UserId,
                    sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                    gccflag = "X",
                    nationflag = "X",
                    regionflag = "X",
                    licenseauthority = "X",
                    paychannelflag = "X",
                    govtflag = State.CustomerCategory == "G" ? "X" : string.Empty
                }, Request.Segment());
                if (response.Succeeded && response.Payload != null)
                {
                    if (State.CustomerType == "G")
                    {
                        State.NationalityList = response.Payload.GCCCountryList != null ? response.Payload.GCCCountryList.Select(x => new SelectListItem { Text = x.countryname, Value = x.countrykey }) : Enumerable.Empty<SelectListItem>();
                    }
                    else
                    {
                        State.NationalityList = response.Payload.nationDetailsList != null ? response.Payload.nationDetailsList.Select(x => new SelectListItem { Text = x.countryname, Value = x.countrykey }) : Enumerable.Empty<SelectListItem>();
                    }
                    State.RegionList = response.Payload.regionDetailsList != null ? response.Payload.regionDetailsList.Select(x => new SelectListItem { Text = x.description, Value = x.region }) : Enumerable.Empty<SelectListItem>();
                    State.GovernmentDDList = response.Payload.governmentDetailsList != null ? response.Payload.governmentDetailsList.Select(x => new SelectListItem { Text = x.firstname + " " + x.lastname, Value = x.businesspartnernumber }) : Enumerable.Empty<SelectListItem>();
                    State.IssuingAuthorityList = response.Payload.licenseDetailsList != null ? response.Payload.licenseDetailsList.Select(x => new SelectListItem { Text = x.description, Value = x.authritycode }) : Enumerable.Empty<SelectListItem>();

                    State.PayChannelList = response.Payload.paymentChannelList != null ? response.Payload.paymentChannelList.Select(x => new SelectListItem { Text = x.channelname, Value = x.channelid }) : Enumerable.Empty<SelectListItem>();

                    if (State.NationalityList != null && State.NationalityList.Count() > 0 && State.NationalityList.Any(c => c.Value == State.Nationality))
                    {
                        State.Nationality = State.NationalityList.FirstOrDefault(c => c.Value == State.Nationality) != null ? State.NationalityList.First(c => c.Value == State.Nationality).Value : string.Empty;
                    }
                    State.IDTypeList = GetMoveinIdTypes(State.CustomerCategory);
                    if (State.CustomerType == "E" || State.CustomerType == "U")
                    {
                        State.IDTypeList = State.IDTypeList.Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).Where(x => x.Value != "PN");
                    }
                    State.ContactPage = true;
                    Save();
                    State.loggedinuser = this.IsLoggedIn ? true : false;
                }
                else
                {
                    CacheProvider.Store(CacheKeys.MOVEIN_ERROR_MESSAGE, new CacheItem<string>(response.Message));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return View("~/Views/Feature/SupplyManagement/MoveIn/ContactDetails.cshtml", State);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContactDetails(MoveInViewModelv3 model)
        {
            {
                try
                {
                    //Added for Future Center
                    var _fc = FetchFutureCenterValues();

                    //Save to cache
                    State.BusinessPartner = !string.IsNullOrWhiteSpace(model.BusinessPartner) ? model.BusinessPartner : State.BusinessPartner;
                    State.Vatnumber = model.Vatnumber;
                    State.IdType = model.IdType;
                    State.IdNumber = model.IdNumber;
                    State.FirstName = State.CustomerCategory.Equals("G") && State.GovernmentDDList != null ? (model.departmentnameid.Equals("1111111111") ? model.departmentnameothers : State.GovernmentDDList.Where(x => x.Value.Equals(model.departmentnameid)).FirstOrDefault().Text) : (State.FirstnameHide ? State.FirstName : model.FirstName);
                    State.LastName = (State.LastnameHide ? State.LastName : model.LastName);
                    State.EmailAddress = model.EmailAddress.Contains("*") ? State.unmaskedEmailAddress : model.EmailAddress;
                    State.MobilePhone = model.MobilePhone.Contains("*") ? (!string.IsNullOrWhiteSpace(State.unmaskedMobilePhone) ? (State.unmaskedMobilePhone.First().Equals('0') ? State.unmaskedMobilePhone.Remove(0, 1) : State.unmaskedMobilePhone) : model.unmaskedMobilePhone) : model.MobilePhone;
                    State.PoBox = model.PoBox;
                    State.Nationality = model.Nationality;
                    State.Emirate = model.Emirate;
                    State.DateOfBirth = !string.IsNullOrEmpty(model.DateOfBirth) ? model.DateOfBirth.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December") : string.Empty;
                    State.ExpiryDate = !string.IsNullOrEmpty(model.ExpiryDate) ? model.ExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December") : string.Empty;
                    State.departmentnameothers = model.departmentnameothers;
                    State.departmentnameid = model.departmentnameid;
                    State.IssuingAuthority = model.IssuingAuthority;
                    State.IssuingAuthorityName = model.IssuingAuthorityName;
                    State.uidnumber = (model.IdType != null && model.IdType.Equals("PN")) ? model.uidnumber : string.Empty;
                    State.thukercardnumber = model.thukercardnumber;
                    State.nationalsocialcardnumber = model.nationalsocialcardnumber;
                    State.sanadsmartcardnumber = model.sanadsmartcardnumber;
                    State.isguaranteeleter = model.isguaranteeleter;
                    State.UserId = !string.IsNullOrWhiteSpace(State.FirstUserId) ? State.FirstUserId : State.TempUserId;
                    Save();

                    #region Attachments

                    string error;

                    //id (PN,ED) 1
                    if (model.idupload != null && model.idupload.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.idupload, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.idupload.InputStream.CopyTo(memoryStream);
                                State.Attachment1 = memoryStream.ToArray();
                                State.Attachment1Filename = model.idupload.FileName.GetFileNameWithoutPath();
                                if (State.IdType == "PN")
                                {
                                    State.Attachment1FileType = AttachmentTypeCodes.Passport;
                                }
                                else
                                {
                                    State.Attachment1FileType = AttachmentTypeCodes.EmiratesIdDocument;
                                }
                            }
                        }
                    }
                    //id (TN,IA) 1
                    if (model.TradeLicense != null && model.TradeLicense.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.TradeLicense, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.TradeLicense.InputStream.CopyTo(memoryStream);
                                State.Attachment1 = memoryStream.ToArray();
                                State.Attachment1Filename = model.TradeLicense.FileName.GetFileNameWithoutPath();

                                if (State.IdType == "TN")
                                {
                                    State.Attachment1FileType = AttachmentTypeCodes.TradeLicense;
                                }
                                else
                                {
                                    State.Attachment1FileType = AttachmentTypeCodes.InitialApproval;
                                }
                            }
                        }
                    }
                    //Tenancy Contract 2
                    if (model.tenancycontract != null && model.tenancycontract.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.tenancycontract, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.tenancycontract.InputStream.CopyTo(memoryStream);
                                State.Attachment2 = memoryStream.ToArray();
                                State.Attachment2Filename = model.tenancycontract.FileName.GetFileNameWithoutPath();
                                if (State.Owner && !string.IsNullOrWhiteSpace(State.Propertyid) && !State.CustomerCategory.Equals("G") && State.Propertyid == "PA")
                                {
                                    State.Attachment2FileType = AttachmentTypeCodes.PurchaseAgreement;
                                }
                                else if (State.Owner && !string.IsNullOrWhiteSpace(State.Propertyid) && !State.CustomerCategory.Equals("G") && State.Propertyid == "TD")
                                {
                                    State.Attachment2FileType = AttachmentTypeCodes.PurchaseAgreement;
                                }
                                else if (!State.isEjari)
                                {
                                    State.Attachment2FileType = AttachmentTypeCodes.TenancyContract;
                                }
                            }
                        }
                    }
                    //Tax Certificate ( if VAT no is entered) 3
                    if (model.VatDocument != null && model.VatDocument.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.VatDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.VatDocument.InputStream.CopyTo(memoryStream);
                                State.Attachment3 = memoryStream.ToArray();
                                State.Attachment3Filename = model.VatDocument.FileName.GetFileNameWithoutPath();
                                State.Attachment3FileType = AttachmentTypeCodes.VatDocument;
                            }
                        }
                    }
                    //National Social Card 3
                    if (model.isnationalsocialcard && model.nationalsocialcard != null && model.nationalsocialcard.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.nationalsocialcard, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.nationalsocialcard.InputStream.CopyTo(memoryStream);
                                State.Attachment3 = memoryStream.ToArray();
                                State.Attachment3Filename = model.nationalsocialcard.FileName.GetFileNameWithoutPath();
                                State.Attachment3FileType = AttachmentTypeCodes.NationalSocialcard;
                            }
                        }
                    }
                    //Thuker Card 4
                    if (model.isthukercard && model.thukercard != null && model.thukercard.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.thukercard, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.thukercard.InputStream.CopyTo(memoryStream);
                                State.Attachment4 = memoryStream.ToArray();
                                State.Attachment4Filename = model.thukercard.FileName.GetFileNameWithoutPath();
                                State.Attachment4FileType = AttachmentTypeCodes.Thukercard;
                            }
                        }
                    }
                    //Sanad Smart Card 5
                    if (model.issanadsmartcard && model.sanadsmartcard != null && model.sanadsmartcard.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.sanadsmartcard, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.sanadsmartcard.InputStream.CopyTo(memoryStream);
                                State.Attachment6 = memoryStream.ToArray();
                                State.Attachment6Filename = model.sanadsmartcard.FileName.GetFileNameWithoutPath();
                                State.Attachment6FileType = AttachmentTypeCodes.SanadSmartcard;
                            }
                        }
                    }
                    //Decree letter
                    if (model.Decreeletter != null && model.Decreeletter.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.Decreeletter, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.Decreeletter.InputStream.CopyTo(memoryStream);
                                State.Attachment4 = memoryStream.ToArray();
                                State.Attachment4Filename = model.Decreeletter.FileName.GetFileNameWithoutPath();
                                State.Attachment4FileType = AttachmentTypeCodes.DecreeLetter;
                            }
                        }
                    }
                    //Garantee Leter GL 6
                    if (model.guranteeleter != null && model.guranteeleter.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.guranteeleter, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.guranteeleter.InputStream.CopyTo(memoryStream);
                                State.Attachment5 = memoryStream.ToArray();
                                State.Attachment5Filename = model.guranteeleter.FileName.GetFileNameWithoutPath();
                                State.Attachment5FileType = AttachmentTypeCodes.GuaranteeLetter;
                            }
                        }
                    }

                    #endregion Attachments

                    var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                    {
                        premiseDetailsList = State.PremiseAccount.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new premiseDetails { premise = x }).ToArray(),
                        moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           customercategory=State.CustomerCategory,
                           customertype= !string.IsNullOrEmpty(State.CustomerTypeForNonIndividual) ? State.CustomerTypeForNonIndividual : State.CustomerType,
                           accounttype =State.AccountType,
                           occupiedby=State.Owner?(State.occupiedbyowner?"O":"T"):string.Empty,
                           businesspartnernumber=State.BusinessPartner,
                           ejarinumber=State.isEjari?State.Ejari:string.Empty,
                           moveindate = State.MoveinStartDate.HasValue ? State.MoveinStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           contractnumber=State.Owner?State.Purchase_TitleDeed :( State.isEjari!=true? State.Tenancy_Contract_Number:string.Empty),
                           tenancystartdate=State.ContractStartDate.HasValue?State.ContractStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancyenddate=State.ContractEndDate.HasValue?State.ContractEndDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancycontractvalue=State.isEjari!=true?State.ContractValue:string.Empty,
                           noofrooms=(State.Owner || State.isEjari!=true)?State.NumberOfRooms.ToString():string.Empty,
                           vatnumber=State.Vatnumber,
                           center =_fc.Branch,
                           idtype=State.IdType,
                           idnumber=State.IdNumber,
                           firtname= State.FirstName,
                           lastname=State.LastName,
                           mobile=State.MobilePhone.AddMobileNumberZeroPrefix(),
                           email=State.EmailAddress,
                           pobox=State.PoBox,
                           idexpiry =  !string.IsNullOrEmpty(State.ExpiryDate) ? Convert.ToDateTime(State.ExpiryDate).ToString("yyyyMMdd") : string.Empty,
                           dateofbirth = !string.IsNullOrEmpty(State.DateOfBirth) ? Convert.ToDateTime(State.DateOfBirth).ToString("yyyyMMdd") : string.Empty,
                           nationality=State.Nationality,
                           region=State.Emirate,
                           nationalsocialnumber=State.nationalsocialcardnumber,
                           sanadnumber=State.sanadsmartcardnumber,
                           thukuhrnumber=State.thukercardnumber,
                           guranteeletterflag=State.isguaranteeleter?"X":string.Empty,
                           departmentnameothers = State.departmentnameothers,
                           departmentid = State.departmentnameid,
                           propertyid = State.Propertyid
                       }
                    },
                        userid = State.UserId,
                        sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                        lang = RequestLanguage.Code(),
                        executionflag = "M",// change for master data earlier war R
                        channel = "A",
                        applicationflag = "M"
                    }, Request.Segment());

                    if (response.Succeeded && response.Payload != null && response.Payload.moveinnotif != null)
                    {
                        State.SecurityDeposit = response.Payload.moveinnotif.totalsecuritydepositamount;
                        State.InnovationFee = response.Payload.moveinnotif.totalinnovationfeesamount;
                        State.KnowledgeFee = response.Payload.moveinnotif.totalknowledgefeesamount;
                        State.TotalOutstandingFee = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.totaloutstandingamount) ? (double.Parse(response.Payload.moveinnotif.totaloutstandingamount) > 0.0 ? double.Parse(response.Payload.moveinnotif.totaloutstandingamount).ToString() : "0") : "0";
                        State.ReconnectionRegistrationFee = (double.Parse(response.Payload.moveinnotif.totalreconnectionchargeamount ?? "0")).ToString();
                        State.AddressRegistrationFee = (double.Parse(response.Payload.moveinnotif.totaladdresschangeamount ?? "0")).ToString();
                        State.ReconnectionVATrate = response.Payload.moveinnotif.reconnectionvatpercentage;
                        State.ReconnectionVATamt = (double.Parse(response.Payload.moveinnotif.reconnectionvatamount ?? "0")).ToString();
                        State.AddressVATrate = response.Payload.moveinnotif.addresschangevatpercentage;
                        State.AddressVAtamt = (double.Parse(response.Payload.moveinnotif.addresschangevatamount ?? "0")).ToString();
                        State.paylater = response.Payload.moveinnotif.sdpaylaterflag;
                        State.payother = response.Payload.moveinnotif.sdpayotherflag;
                        State.maidubaigift = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.maidubaigift) && response.Payload.moveinnotif.maidubaigift.Equals("X") ? true : false;
                        State.maidubaimsgtext = response.Payload.moveinnotif.maidubaimsgtext;
                        State.maidubaimsgtitle = response.Payload.moveinnotif.maidubaimsgtitle;
                        State.maidubaicontribution = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.maidubaigift) && response.Payload.moveinnotif.maidubaigift.Equals("X") ? true : false;
                        State.Easypayflag = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.easypayflag) && response.Payload.moveinnotif.easypayflag.Equals("X") ? true : false;
                        State.payotherchannelflag = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.payotherchannelflag) && response.Payload.moveinnotif.payotherchannelflag.Equals("X") ? true : false;
                        if (response.Payload.moveinScreenMessageList != null && response.Payload.moveinScreenMessageList.Count() > 0)
                        {
                            State.messagepaychannel = response.Payload.moveinScreenMessageList.ToList().Where(x => x.category.Equals("PC")) != null ? response.Payload.moveinScreenMessageList.ToList().Where(x => x.category.Equals("PC")).Select(y => y.message).ToArray() : new string[] { };
                            State.messagewhatsnext = response.Payload.moveinScreenMessageList.ToList().Where(x => x.category.Equals("WN")) != null ? response.Payload.moveinScreenMessageList.ToList().Where(x => x.category.Equals("WN")).Select(y => y.message).ToArray() : new string[] { };
                        }
                        if (this.IsLoggedIn)
                        {
                            State.UserId = CurrentPrincipal.UserId;
                        }
                        else
                        {
                            State.UserId = !string.IsNullOrWhiteSpace(State.UserId) ? State.UserId : response.Payload.moveinnotif.userid;
                        }
                        State.BusinessPartner = !string.IsNullOrWhiteSpace(State.BusinessPartner) ? State.BusinessPartner : response.Payload.moveinnotif.businesspartnernumber;
                        State.SkiptoPayment = response.Payload.moveinnotif.skippayment;
                        State.premiseAmountDetails = response.Payload.premiseAmountDetailsList;
                        State.MovetoTransactionNumber = response.Payload.moveinnotif.movetotransactionid;
                        Save();
                        CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));
                        CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATH, new CacheItem<string>(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILSv3));
                        CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATHTEXT, new CacheItem<string>(Translate.Text("movein.landingpagebacktext")));
                        if (!string.IsNullOrWhiteSpace(State.SkiptoPayment))
                        {
                            State.Total = double.Parse(State.SecurityDeposit) + double.Parse(State.ReconnectionRegistrationFee) + double.Parse(State.AddressRegistrationFee) + double.Parse(State.KnowledgeFee) + double.Parse(State.TotalOutstandingFee) + double.Parse(State.InnovationFee) + double.Parse(State.ReconnectionVATamt) + double.Parse(State.AddressVAtamt);
                            State.MoveInNotificationNumber = response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).FirstOrDefault();
                            State.transactionList = response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray();
                            Save();
                            if (string.IsNullOrWhiteSpace(State.UserId))
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNTv3);
                            }
                            if (State.Attachment1.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment1Filename, State.Attachment1FileType, State.Attachment1, RequestLanguage, Request.Segment());
                            }
                            if (State.Attachment2.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment2Filename, State.Attachment2FileType, State.Attachment2, RequestLanguage, Request.Segment());
                            }
                            if (State.Attachment3.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment3Filename, State.Attachment3FileType, State.Attachment3, RequestLanguage, Request.Segment());
                            }
                            if (State.Attachment4.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment4Filename, State.Attachment4FileType, State.Attachment4, RequestLanguage, Request.Segment());
                            }
                            if (State.Attachment5.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment5Filename, State.Attachment5FileType, State.Attachment5, RequestLanguage, Request.Segment());
                            }
                            if (State.Attachment6.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment6Filename, State.Attachment6FileType, State.Attachment6, RequestLanguage, Request.Segment());
                            }
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_CONFIRMATION_PAGEv3);
                        }

                        if (this.IsLoggedIn)
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_PAYMENT_PAGEv3);
                        }
                        else if (string.IsNullOrWhiteSpace(State.UserId))
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNTv3);
                        }
                        else
                        {
                            ISitecoreContext isitecoreContext = new SitecoreContext();
                            var currentitem = isitecoreContext.GetCurrentItem<Item>();
                            var lang = currentitem.Language.CultureInfo.TextInfo.IsRightToLeft ? "ar-AE" : "en";
                            CacheProvider.Store(CacheKeys.MOVEIN_USERID, new CacheItem<string>(State.UserId));
                            QueryString a = new QueryString();
                            a.With("returnUrl", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.LOGIN_MOVEIN_PAYMENT_PAGEv3), true);
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE, a);
                        }
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.MOVEIN_ERROR_MESSAGE, new CacheItem<string>(response.Message));

                        if (response.Payload.responsecode != "399")
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILSv3);
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILSv3);
                    }
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
                return View("~/Views/Feature/SupplyManagement/MoveIn/ContactDetails.cshtml", State);
            }
        }

        [HttpGet]
        public ActionResult GetCreateAccountForm()
        {
            if (!InProgress())
            {
                if (this.IsLoggedIn)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
            }
            State.CreatePage = true;
            var model = new CreateOnlineAccountModel();
            model.Termslink = UtilExtensions.GetLinkUrl(Sitecorex.Database.GetItem(SitecoreItemIdentifiers.MOVEIN_CONSTANTS), "Terms and Condition Create Account");
            return View("~/Views/Feature/SupplyManagement/MoveIn/CreateAccount.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetCreateAccountForm(CreateOnlineAccountModel model)
        {
            CacheProvider.Remove(CacheKeys.MOVEIN_PROCESSED);
            //Added for Future Center
            var _fc = FetchFutureCenterValues();

            if (IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
            }

            if (ModelState.IsValid)
            {
                var availability = DewaApiClient.VerifyUserIdentifierAvailable(model.UserId, RequestLanguage, Request.Segment());
                if (availability.Succeeded && availability.Payload.IsAvailableForUse)
                {
                    if (ModelState.IsValid)
                    {
                        // State.BusinessPartner = model.BusinessPartnerNumber ?? string.Empty;
                        State.UserId = model.UserId;
                        State.Password = model.Password;
                        State.ConfirmPassword = model.ConfirmationPassword;
                        State.createuseraccount = "X";
                        Save();
                        CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));
                        if (!string.IsNullOrWhiteSpace(State.SkiptoPayment))
                        {
                            try
                            {
                                var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                                {
                                    premiseDetailsList = State.PremiseAccount.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new premiseDetails { premise = x }).ToArray(),
                                    moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           customercategory=State.CustomerCategory,
                           customertype= !string.IsNullOrEmpty(State.CustomerTypeForNonIndividual) ? State.CustomerTypeForNonIndividual : State.CustomerType,
                           accounttype =State.AccountType,
                           occupiedby=State.Owner?(State.occupiedbyowner?"O":"T"):string.Empty,
                           businesspartnernumber=State.BusinessPartner,
                           ejarinumber=State.isEjari?State.Ejari:string.Empty,
                           moveindate = State.MoveinStartDate.HasValue ? State.MoveinStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           contractnumber=State.Owner?State.Purchase_TitleDeed :( State.isEjari!=true? State.Tenancy_Contract_Number:string.Empty),
                           tenancystartdate=State.ContractStartDate.HasValue?State.ContractStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancyenddate=State.ContractEndDate.HasValue?State.ContractEndDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancycontractvalue=State.isEjari!=true?State.ContractValue:string.Empty,
                           noofrooms=(State.Owner || State.isEjari!=true)?State.NumberOfRooms.ToString():string.Empty,
                           vatnumber=State.Vatnumber,
                           center =_fc.Branch,
                           idtype=State.IdType,
                           idnumber=State.IdNumber,
                           idexpiry =  !string.IsNullOrEmpty(State.ExpiryDate) ? Convert.ToDateTime(State.ExpiryDate).ToString("yyyyMMdd") : string.Empty,
                           dateofbirth = !string.IsNullOrEmpty(State.DateOfBirth) ? Convert.ToDateTime(State.DateOfBirth).ToString("yyyyMMdd") : string.Empty,
                           firtname= State.FirstName,
                           lastname=State.LastName,
                           mobile=State.MobilePhone.AddMobileNumberZeroPrefix(),
                           email=State.EmailAddress,
                           pobox=State.PoBox,
                           nationality=State.Nationality,
                           region=State.Emirate,
                           nationalsocialnumber=State.nationalsocialcardnumber,
                           sanadnumber=State.sanadsmartcardnumber,
                           thukuhrnumber=State.thukercardnumber,
                           password=State.Password,
                           createuseraccount=State.createuseraccount,
                           guranteeletterflag=State.isguaranteeleter?"X":string.Empty,
                           departmentnameothers = State.departmentnameothers,
                           departmentid = State.departmentnameid,
                           propertyid = State.Propertyid
                       }
                    },
                                    userid = State.UserId,
                                    sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                                    lang = RequestLanguage.Code(),
                                    executionflag = "U",// change for the flag as "U" if its userid creation on skiptopayment case
                                    transactionid = State.transactionList != null ? State.transactionList.FirstOrDefault() : string.Empty,
                                    channel = "A",
                                    applicationflag = "M"
                                }, Request.Segment());

                                if (response.Succeeded && response.Payload != null && response.Payload.moveinnotif != null)
                                {
                                    if (State.Attachment1.Length > 0)
                                    {
                                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment1Filename, State.Attachment1FileType, State.Attachment1, RequestLanguage, Request.Segment());
                                    }
                                    if (State.Attachment2.Length > 0)
                                    {
                                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment2Filename, State.Attachment2FileType, State.Attachment2, RequestLanguage, Request.Segment());
                                    }
                                    if (State.Attachment3.Length > 0)
                                    {
                                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment3Filename, State.Attachment3FileType, State.Attachment3, RequestLanguage, Request.Segment());
                                    }
                                    if (State.Attachment4.Length > 0)
                                    {
                                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment4Filename, State.Attachment4FileType, State.Attachment4, RequestLanguage, Request.Segment());
                                    }
                                    if (State.Attachment5.Length > 0)
                                    {
                                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment5Filename, State.Attachment5FileType, State.Attachment5, RequestLanguage, Request.Segment());
                                    }
                                    if (State.Attachment6.Length > 0)
                                    {
                                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment6Filename, State.Attachment6FileType, State.Attachment6, RequestLanguage, Request.Segment());
                                    }
                                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_CONFIRMATION_PAGEv3);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                                return PartialView("~/Views/Feature/SupplyManagement/MoveIn/CreateAccount.cshtml", model);
                            }
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_PAYMENT_PAGEv3);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, availability.Message);
                }
            }
            return PartialView("~/Views/Feature/SupplyManagement/MoveIn/CreateAccount.cshtml", model);
        }

        #endregion Anonymous

        #region Logged in

        [AcceptVerbs("GET", "HEAD")]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult AccountRegisterAuth()
        {
            if (!this.IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
            }
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.MOVEIN_ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.MOVEIN_ERROR_MESSAGE);
            }
            MoveInViewModelv3 model = new MoveInViewModelv3();
            string journey = string.Empty;
            if (!CacheProvider.TryGet(CacheKeys.MOVEIN_JOURNEY, out journey) && string.IsNullOrWhiteSpace(errorMessage))
            {
                CacheProvider.Remove(CacheKeys.MOVE_IN_3_WORKFLOW_STATE);
            }
            else
            {
                model = State;
            }
            model.loggedinuser = this.IsLoggedIn ? true : false;
            if (model.loggedinuser)
            {
                model.PBusinessPartners = GetPersonBusinessPartners();
                model.OBusinessPartners = GetOrganizationBusinessPartners();
                model.GBusinessPartners = GetGovBusinessPartners();
                model.GBusinessPartners = GetGovBusinessPartners();
            }
            model.CustomerTypeList = GetCustomerTypes();
            model.AccountTypeList = GetAccountTypes();
            model.OwnerTypeList = GetOwnerType();
            model.NumberOfRoomsList = GetNumberOfRooms();
            return View("~/Views/Feature/SupplyManagement/MoveIn/AccountRegister.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult AccountRegisterAuth(MoveInViewModelv3 model)
        {
            //if (ModelState.IsValid)
            {
                //Added for Future Center
                var _fc = FetchFutureCenterValues();
                if (model.CustomerCategory.Equals("P"))
                {
                    model.BusinessPartner = model.PBusinessPartner;
                }
                else if (model.CustomerCategory.Equals("G"))
                {
                    model.BusinessPartner = model.GBusinessPartner;
                }
                else if (model.CustomerCategory.Equals("O"))
                {
                    model.BusinessPartner = model.OBusinessPartner;
                }

                if (!string.IsNullOrWhiteSpace(model.strPremiseNumberdetails))
                {
                    model.PremiseNumberdetails = CustomJsonConvertor.DeserializeObject<List<AddedPremiseDetails>>(model.strPremiseNumberdetails);
                }
                //Save to cache
                model.CustomerType = model.CustomerCategory != "P" ? string.Empty : model.CustomerType;
                model.Propertyid = model.Owner && !model.CustomerCategory.Equals("G") ? model.Propertyid : string.Empty;
                model.PremiseAccount = model.PremiseNumberdetails != null && model.PremiseNumberdetails.Count > 0 ? model.PremiseNumberdetails.Select(x => x.premisenumber).ToArray() : Enumerable.Range(0, 1).Select(x => model.PremiseNo).ToArray();

                Save(model);

                try
                {
                    var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                    {
                        premiseDetailsList = model.PremiseAccount.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new premiseDetails { premise = x }).ToArray(),
                        moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           customercategory=model.CustomerCategory,
                          customertype= !string.IsNullOrEmpty(State.CustomerTypeForNonIndividual) ? State.CustomerTypeForNonIndividual : State.CustomerType,
                           accounttype =model.AccountType,
                           occupiedby=model.Owner?(model.occupiedbyowner?"O":"T"):string.Empty,
                           businesspartnernumber=model.BusinessPartner,
                           ejarinumber=!model.Owner?(model.isEjari?model.Ejari:string.Empty):string.Empty,
                           moveindate = model.MoveinStartDate.HasValue ? model.MoveinStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           contractnumber=model.Owner?model.Purchase_TitleDeed :( model.isEjari!=true? model.Tenancy_Contract_Number:string.Empty),
                           tenancystartdate=model.ContractStartDate.HasValue?model.ContractStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancyenddate=model.ContractEndDate.HasValue?model.ContractEndDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancycontractvalue=model.isEjari!=true?model.ContractValue:string.Empty,
                           noofrooms=(model.Owner || model.isEjari!=true)?model.NumberOfRooms.ToString():string.Empty,
                           vatnumber=model.Vatnumber,
                           propertyid = model.Propertyid,
                           center =_fc.Branch
                       }
                    },
                        userid = CurrentPrincipal.UserId,
                        sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                        lang = RequestLanguage.Code(),
                        executionflag = "R",
                        channel = "A",
                        applicationflag = "M"
                    }, Request.Segment());

                    if (response.Succeeded && response.Payload != null && response.Payload.moveinnotif != null)
                    {
                        if (this.IsLoggedIn)
                        {
                            State.UserId = CurrentPrincipal.UserId;
                            State.FirstUserId = CurrentPrincipal.UserId;
                        }
                        else
                        {
                            State.UserId = response.Payload.moveinnotif.userid;
                            State.FirstUserId = response.Payload.moveinnotif.userid;
                        }

                        State.SecurityDeposit = response.Payload.moveinnotif.totalsecuritydepositamount;
                        State.InnovationFee = response.Payload.moveinnotif.totalinnovationfeesamount;
                        State.KnowledgeFee = response.Payload.moveinnotif.totalknowledgefeesamount;
                        State.TotalOutstandingFee = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.totaloutstandingamount) ? (double.Parse(response.Payload.moveinnotif.totaloutstandingamount) > 0.0 ? double.Parse(response.Payload.moveinnotif.totaloutstandingamount).ToString() : "0") : "0";
                        State.ReconnectionRegistrationFee = (double.Parse(response.Payload.moveinnotif.totalreconnectionchargeamount ?? "0")).ToString();
                        State.AddressRegistrationFee = (double.Parse(response.Payload.moveinnotif.totaladdresschangeamount ?? "0")).ToString();
                        State.ReconnectionVATrate = response.Payload.moveinnotif.reconnectionvatpercentage;
                        State.ReconnectionVATamt = (double.Parse(response.Payload.moveinnotif.reconnectionvatamount ?? "0")).ToString();
                        State.AddressVATrate = response.Payload.moveinnotif.addresschangevatpercentage;
                        State.AddressVAtamt = (double.Parse(response.Payload.moveinnotif.addresschangevatamount ?? "0")).ToString();
                        State.FirstName = response.Payload.moveinnotif.firtname;
                        State.LastName = response.Payload.moveinnotif.lastname;
                        State.MobilePhone = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.mobile) ? response.Payload.moveinnotif.mobile.First().Equals('0') ? response.Payload.moveinnotif.mobile.Remove(0, 1) : response.Payload.moveinnotif.mobile : string.Empty;
                        State.EmailAddress = response.Payload.moveinnotif.email;
                        State.unmaskedMobilePhone = response.Payload.moveinnotif.unmaskedmobile;
                        State.unmaskedEmailAddress = response.Payload.moveinnotif.unmaskedemail;
                        State.Nationality = response.Payload.moveinnotif.nationality;
                        State.PoBox = response.Payload.moveinnotif.pobox;
                        State.BusinessPartner = !string.IsNullOrWhiteSpace(model.BusinessPartner) ? model.BusinessPartner : response.Payload.moveinnotif.businesspartnernumber;
                        State.IdType = response.Payload.moveinnotif.idtype;
                        State.AttachmentFlag = string.IsNullOrWhiteSpace(response.Payload.moveinnotif.idnumber);//((string.Equals(model.IdType, "ED") && string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname)) || (string.Equals(model.IdType, "PN") && string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname)) || (string.Equals(model.IdType, "TN") && string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname))|| (string.Equals(model.IdType, "IA") && string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname))) ? true : false;
                        State.SkiptoPayment = response.Payload.moveinnotif.skippayment;
                        State.ShowDiscount = response.Payload.moveinnotif.showdiscount;
                        State.Guranteeletterflag = response.Payload.moveinnotif.guranteeletterflag;
                        if (State.CustomerCategory == "P" && State.IdType.Equals(string.Empty))
                        {
                            State.IdType = "ED";
                        }
                        else if (State.CustomerCategory == "O" && State.IdType.Equals(string.Empty))
                        {
                            State.IdType = "TN";
                        }
                        State.IdNumber = response.Payload.moveinnotif.idnumber;
                        State.IsIdNumber = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.idnumber);
                        if (State.IdType.Equals("ED") && !string.IsNullOrWhiteSpace(State.IdNumber))
                        {
                            State.IdNumber = string.Concat(response.Payload.moveinnotif.idnumber.Where(char.IsDigit));
                            State.IsIdNumber = !string.IsNullOrWhiteSpace(string.Concat(response.Payload.moveinnotif.idnumber.Where(char.IsDigit)));

                            DateTime _idExpiry = DateTime.MinValue;
                            DateTime _dob = DateTime.MinValue;

                            DateTime.TryParse(response.Payload.moveinnotif.idexpiry, out _idExpiry);
                            string _sendIdExpiry = _idExpiry == DateTime.MinValue ? string.Empty : _idExpiry.ToString("dd MMMM yyyy");

                            DateTime.TryParse(response.Payload.moveinnotif.dateofbirth, out _dob);
                            string _sendDOB = _dob == DateTime.MinValue ? string.Empty : _dob.ToString("dd MMMM yyyy");
                            if (_idExpiry != DateTime.MinValue)
                                State.ExpiryDate = _idExpiry.ToString("dd MMMM yyyy");
                            if (_dob != DateTime.MinValue)
                                State.DateOfBirth = _dob.ToString("dd MMMM yyyy");


                        }
                        //State.Isfirstname = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname);
                        //State.Islastname = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.lastname);
                        State.IssuingAuthority = response.Payload.moveinnotif.idauthority;
                        State.IssuingAuthorityName = response.Payload.moveinnotif.issuingauthorityothers;
                        State.departmentnameothers = response.Payload.moveinnotif.departmentnameothers;
                        State.departmentnameid = response.Payload.moveinnotif.departmentid;
                        State.IsDeptid = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.departmentid);
                        State.Emirate = response.Payload.moveinnotif.region;
                        State.Vatnumber = response.Payload.moveinnotif.vatnumber;
                        State.IsVATNumber = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.vatnumber);

                        State.Isfirstname = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname);
                        State.Islastname = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.lastname);
                        State.IsMobilePhone = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.unmaskedmobile);
                        State.IsEmailAddress = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.email);
                        State.IsunmaskedMobilePhone = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.unmaskedmobile);
                        State.IsunmaskedEmailAddress = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.unmaskedemail);
                        State.IsNationality = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.nationality);
                        State.IsPoBox = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.pobox);
                        State.Isbp = !string.IsNullOrWhiteSpace(State.BusinessPartner);
                        State.IsEmirate = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.region);

                        Save();
                        CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));

                        if (this.IsLoggedIn)
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_CONTACT_DETAILSv3);
                        }
                        else
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_CONTACT_DETAILSv3);
                        }
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.MOVEIN_ERROR_MESSAGE, new CacheItem<string>(response.Message));

                        if (response.Payload.responsecode != "399")
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    //return View(model);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
            }
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ContactDetailsAuth()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.MOVEIN_ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.MOVEIN_ERROR_MESSAGE);
            }
            if (!InProgress())
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
            }
            try
            {
                var response = DewaApiClient.SetMoveInReadRequest(new moveInReadInput
                {
                    lang = RequestLanguage.Code(),
                    userid = State.UserId,
                    sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                    gccflag = "X",
                    nationflag = "X",
                    regionflag = "X",
                    licenseauthority = "X",
                    paychannelflag = "X",
                    govtflag = State.CustomerCategory == "G" ? "X" : string.Empty
                }, Request.Segment());
                if (response.Succeeded && response.Payload != null)
                {
                    if (State.CustomerType == "G")
                    {
                        State.NationalityList = response.Payload.GCCCountryList != null ? response.Payload.GCCCountryList.Select(x => new SelectListItem { Text = x.countryname, Value = x.countrykey }) : Enumerable.Empty<SelectListItem>();
                    }
                    else
                    {
                        State.NationalityList = response.Payload.nationDetailsList != null ? response.Payload.nationDetailsList.Select(x => new SelectListItem { Text = x.countryname, Value = x.countrykey }) : Enumerable.Empty<SelectListItem>();
                    }
                    State.RegionList = response.Payload.regionDetailsList != null ? response.Payload.regionDetailsList.Select(x => new SelectListItem { Text = x.description, Value = x.region }) : Enumerable.Empty<SelectListItem>();
                    State.GovernmentDDList = response.Payload.governmentDetailsList != null ? response.Payload.governmentDetailsList.Select(x => new SelectListItem { Text = x.firstname + " " + x.lastname, Value = x.businesspartnernumber }) : Enumerable.Empty<SelectListItem>();
                    State.IssuingAuthorityList = response.Payload.licenseDetailsList != null ? response.Payload.licenseDetailsList.Select(x => new SelectListItem { Text = x.description, Value = x.authritycode }) : Enumerable.Empty<SelectListItem>();

                    State.PayChannelList = response.Payload.paymentChannelList != null ? response.Payload.paymentChannelList.Select(x => new SelectListItem { Text = x.channelname, Value = x.channelid }) : Enumerable.Empty<SelectListItem>();

                    if (State.NationalityList != null && State.NationalityList.Count() > 0 && State.NationalityList.Any(c => c.Value == State.Nationality))
                    {
                        State.Nationality = State.NationalityList.FirstOrDefault(c => c.Value == State.Nationality) != null ? State.NationalityList.First(c => c.Value == State.Nationality).Value : string.Empty;
                    }

                    State.IDTypeList = GetMoveinIdTypes(State.CustomerCategory);
                    if (State.CustomerType == "E" || State.CustomerType == "U")
                    {
                        State.IDTypeList = State.IDTypeList.Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).Where(x => x.Value != "PN");
                    }
                    State.ContactPage = true;
                    Save();
                    State.loggedinuser = this.IsLoggedIn ? true : false;
                }
                else
                {
                    CacheProvider.Store(CacheKeys.MOVEIN_ERROR_MESSAGE, new CacheItem<string>(response.Message));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return View("~/Views/Feature/SupplyManagement/MoveIn/ContactDetails.cshtml", State);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ContactDetailsAuth(MoveInViewModelv3 model)
        {
            {
                //Added for Future Center
                var _fc = FetchFutureCenterValues();

                //Save to cache
                State.BusinessPartner = !string.IsNullOrWhiteSpace(model.BusinessPartner) ? model.BusinessPartner : State.BusinessPartner;
                State.Vatnumber = model.Vatnumber;
                State.IdType = model.IdType;
                State.IdNumber = model.IdNumber;
                State.FirstName = State.CustomerCategory.Equals("G") && State.GovernmentDDList != null ? (model.departmentnameid.Equals("1111111111") ? model.departmentnameothers : State.GovernmentDDList.Where(x => x.Value.Equals(model.departmentnameid)).FirstOrDefault().Text) : (State.FirstnameHide ? State.FirstName : model.FirstName);
                State.LastName = (State.LastnameHide ? State.LastName : model.LastName);
                State.EmailAddress = model.EmailAddress.Contains("*") ? State.unmaskedEmailAddress : model.EmailAddress;
                State.MobilePhone = model.MobilePhone.Contains("*") ? (!string.IsNullOrWhiteSpace(State.unmaskedMobilePhone) ? (State.unmaskedMobilePhone.First().Equals('0') ? State.unmaskedMobilePhone.Remove(0, 1) : State.unmaskedMobilePhone) : model.unmaskedMobilePhone) : model.MobilePhone;
                State.PoBox = model.PoBox;
                State.Nationality = model.Nationality;
                State.Emirate = model.Emirate;
                State.DateOfBirth = !string.IsNullOrEmpty(model.DateOfBirth) ? model.DateOfBirth.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December") : string.Empty;
                State.ExpiryDate = !string.IsNullOrEmpty(model.ExpiryDate) ? model.ExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December") : string.Empty;
                State.departmentnameothers = model.departmentnameothers;
                State.departmentnameid = model.departmentnameid;
                State.IssuingAuthority = model.IssuingAuthority;
                State.IssuingAuthorityName = model.IssuingAuthorityName;
                State.uidnumber = (model.IdType != null && model.IdType.Equals("PN")) ? model.uidnumber : string.Empty;
                State.thukercardnumber = model.thukercardnumber;
                State.nationalsocialcardnumber = model.nationalsocialcardnumber;
                State.sanadsmartcardnumber = model.sanadsmartcardnumber;
                State.isguaranteeleter = model.isguaranteeleter;
                State.UserId = !string.IsNullOrWhiteSpace(State.FirstUserId) ? State.FirstUserId : State.TempUserId;
                Save();

                #region Attachments

                string error;

                //id (PN,ED) 1
                if (model.idupload != null && model.idupload.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.idupload, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.idupload.InputStream.CopyTo(memoryStream);
                            State.Attachment1 = memoryStream.ToArray();
                            State.Attachment1Filename = model.idupload.FileName.GetFileNameWithoutPath();
                            if (State.IdType == "PN")
                            {
                                State.Attachment1FileType = AttachmentTypeCodes.Passport;
                            }
                            else
                            {
                                State.Attachment1FileType = AttachmentTypeCodes.EmiratesIdDocument;
                            }
                        }
                    }
                }
                //id (TN,IA) 1
                if (model.TradeLicense != null && model.TradeLicense.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.TradeLicense, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.TradeLicense.InputStream.CopyTo(memoryStream);
                            State.Attachment1 = memoryStream.ToArray();
                            State.Attachment1Filename = model.TradeLicense.FileName.GetFileNameWithoutPath();

                            if (State.IdType == "TN")
                            {
                                State.Attachment1FileType = AttachmentTypeCodes.TradeLicense;
                            }
                            else
                            {
                                State.Attachment1FileType = AttachmentTypeCodes.InitialApproval;
                            }
                        }
                    }
                }
                //Tenancy Contract 2
                if (model.tenancycontract != null && model.tenancycontract.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.tenancycontract, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.tenancycontract.InputStream.CopyTo(memoryStream);
                            State.Attachment2 = memoryStream.ToArray();
                            State.Attachment2Filename = model.tenancycontract.FileName.GetFileNameWithoutPath();
                            if (State.Owner && !string.IsNullOrWhiteSpace(State.Propertyid) && !State.CustomerCategory.Equals("G") && State.Propertyid == "PA")
                            {
                                State.Attachment2FileType = AttachmentTypeCodes.PurchaseAgreement;
                            }
                            else if (State.Owner && !string.IsNullOrWhiteSpace(State.Propertyid) && !State.CustomerCategory.Equals("G") && State.Propertyid == "TD")
                            {
                                State.Attachment2FileType = AttachmentTypeCodes.PurchaseAgreement;
                            }
                            else if (!State.isEjari)
                            {
                                State.Attachment2FileType = AttachmentTypeCodes.TenancyContract;
                            }
                        }
                    }
                }
                //Tax Certificate ( if VAT no is entered) 3
                if (model.VatDocument != null && model.VatDocument.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.VatDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.VatDocument.InputStream.CopyTo(memoryStream);
                            State.Attachment3 = memoryStream.ToArray();
                            State.Attachment3Filename = model.VatDocument.FileName.GetFileNameWithoutPath();
                            State.Attachment3FileType = AttachmentTypeCodes.VatDocument;
                        }
                    }
                }
                //National Social Card 3
                if (model.isnationalsocialcard && model.nationalsocialcard != null && model.nationalsocialcard.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.nationalsocialcard, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.nationalsocialcard.InputStream.CopyTo(memoryStream);
                            State.Attachment3 = memoryStream.ToArray();
                            State.Attachment3Filename = model.nationalsocialcard.FileName.GetFileNameWithoutPath();
                            State.Attachment3FileType = AttachmentTypeCodes.NationalSocialcard;
                        }
                    }
                }
                //Thuker Card 4
                if (model.isthukercard && model.thukercard != null && model.thukercard.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.thukercard, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.thukercard.InputStream.CopyTo(memoryStream);
                            State.Attachment4 = memoryStream.ToArray();
                            State.Attachment4Filename = model.thukercard.FileName.GetFileNameWithoutPath();
                            State.Attachment4FileType = AttachmentTypeCodes.Thukercard;
                        }
                    }
                }
                //Sanad Smart Card 5
                if (model.issanadsmartcard && model.sanadsmartcard != null && model.sanadsmartcard.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.sanadsmartcard, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.sanadsmartcard.InputStream.CopyTo(memoryStream);
                            State.Attachment6 = memoryStream.ToArray();
                            State.Attachment6Filename = model.sanadsmartcard.FileName.GetFileNameWithoutPath();
                            State.Attachment6FileType = AttachmentTypeCodes.SanadSmartcard;
                        }
                    }
                }
                //Decree letter
                if (model.Decreeletter != null && model.Decreeletter.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.Decreeletter, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.Decreeletter.InputStream.CopyTo(memoryStream);
                            State.Attachment4 = memoryStream.ToArray();
                            State.Attachment4Filename = model.Decreeletter.FileName.GetFileNameWithoutPath();
                            State.Attachment4FileType = AttachmentTypeCodes.DecreeLetter;
                        }
                    }
                }
                //Garantee Leter GL 6
                if (model.guranteeleter != null && model.guranteeleter.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.guranteeleter, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.guranteeleter.InputStream.CopyTo(memoryStream);
                            State.Attachment5 = memoryStream.ToArray();
                            State.Attachment5Filename = model.guranteeleter.FileName.GetFileNameWithoutPath();
                            State.Attachment5FileType = AttachmentTypeCodes.GuaranteeLetter;
                        }
                    }
                }

                #endregion Attachments

                try
                {
                    var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                    {
                        premiseDetailsList = State.PremiseAccount.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new premiseDetails { premise = x }).ToArray(),
                        moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           customercategory=State.CustomerCategory,
                          customertype= !string.IsNullOrEmpty(State.CustomerTypeForNonIndividual) ? State.CustomerTypeForNonIndividual : State.CustomerType,
                           accounttype =State.AccountType,
                           occupiedby=State.Owner?(State.occupiedbyowner?"O":"T"):string.Empty,
                           businesspartnernumber=State.BusinessPartner,
                           ejarinumber=State.isEjari?State.Ejari:string.Empty,
                           moveindate = State.MoveinStartDate.HasValue ? State.MoveinStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           contractnumber=State.Owner?State.Purchase_TitleDeed :( State.isEjari!=true? State.Tenancy_Contract_Number:string.Empty),
                           tenancystartdate=State.ContractStartDate.HasValue?State.ContractStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancyenddate=State.ContractEndDate.HasValue?State.ContractEndDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancycontractvalue=State.isEjari!=true?State.ContractValue:string.Empty,
                           noofrooms=(State.Owner || State.isEjari!=true)?State.NumberOfRooms.ToString():string.Empty,
                           vatnumber=State.Vatnumber,
                           center =_fc.Branch,
                           idtype=State.IdType,
                           idnumber=State.IdNumber,
                           idexpiry =  !string.IsNullOrEmpty(State.ExpiryDate) ? Convert.ToDateTime(State.ExpiryDate).ToString("yyyyMMdd") : string.Empty,
                           dateofbirth = !string.IsNullOrEmpty(State.DateOfBirth) ? Convert.ToDateTime(State.DateOfBirth).ToString("yyyyMMdd") : string.Empty,
                           firtname= State.FirstName,
                           lastname=State.LastName,
                           mobile=State.MobilePhone.AddMobileNumberZeroPrefix(),
                           email=State.EmailAddress,
                           pobox=State.PoBox,
                           nationality=State.Nationality,
                           region=State.Emirate,
                           nationalsocialnumber=State.nationalsocialcardnumber,
                           sanadnumber=State.sanadsmartcardnumber,
                           thukuhrnumber=State.thukercardnumber,
                           guranteeletterflag=State.isguaranteeleter?"X":string.Empty,
                           departmentnameothers = State.departmentnameothers,
                           departmentid = State.departmentnameid,
                           propertyid = State.Propertyid
                       }
                    },
                        userid = State.UserId,
                        sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                        lang = RequestLanguage.Code(),
                        executionflag = "M",// Change for master data earlier war R
                        channel = "A",
                        applicationflag = "M"
                    }, Request.Segment());

                    if (response.Succeeded && response.Payload != null && response.Payload.moveinnotif != null)
                    {
                        State.SecurityDeposit = response.Payload.moveinnotif.totalsecuritydepositamount;
                        State.InnovationFee = response.Payload.moveinnotif.totalinnovationfeesamount;
                        State.KnowledgeFee = response.Payload.moveinnotif.totalknowledgefeesamount;
                        State.TotalOutstandingFee = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.totaloutstandingamount) ? (double.Parse(response.Payload.moveinnotif.totaloutstandingamount) > 0.0 ? double.Parse(response.Payload.moveinnotif.totaloutstandingamount).ToString() : "0") : "0";
                        State.ReconnectionRegistrationFee = (double.Parse(response.Payload.moveinnotif.totalreconnectionchargeamount ?? "0")).ToString();
                        State.AddressRegistrationFee = (double.Parse(response.Payload.moveinnotif.totaladdresschangeamount ?? "0")).ToString();
                        State.ReconnectionVATrate = response.Payload.moveinnotif.reconnectionvatpercentage;
                        State.ReconnectionVATamt = (double.Parse(response.Payload.moveinnotif.reconnectionvatamount ?? "0")).ToString();
                        State.AddressVATrate = response.Payload.moveinnotif.addresschangevatpercentage;
                        State.AddressVAtamt = (double.Parse(response.Payload.moveinnotif.addresschangevatamount ?? "0")).ToString();
                        State.paylater = response.Payload.moveinnotif.sdpaylaterflag;
                        State.payother = response.Payload.moveinnotif.sdpayotherflag;
                        State.maidubaigift = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.maidubaigift) && response.Payload.moveinnotif.maidubaigift.Equals("X") ? true : false;
                        State.maidubaimsgtext = response.Payload.moveinnotif.maidubaimsgtext;
                        State.maidubaimsgtitle = response.Payload.moveinnotif.maidubaimsgtitle;
                        State.maidubaicontribution = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.maidubaigift) && response.Payload.moveinnotif.maidubaigift.Equals("X") ? true : false;
                        State.Easypayflag = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.easypayflag) && response.Payload.moveinnotif.easypayflag.Equals("X") ? true : false;
                        State.payotherchannelflag = !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.payotherchannelflag) && response.Payload.moveinnotif.payotherchannelflag.Equals("X") ? true : false;
                        if (response.Payload.moveinScreenMessageList != null && response.Payload.moveinScreenMessageList.Count() > 0)
                        {
                            State.messagepaychannel = response.Payload.moveinScreenMessageList.ToList().Where(x => x.category.Equals("PC")) != null ? response.Payload.moveinScreenMessageList.ToList().Where(x => x.category.Equals("PC")).Select(y => y.message).ToArray() : new string[] { };
                            State.messagewhatsnext = response.Payload.moveinScreenMessageList.ToList().Where(x => x.category.Equals("WN")) != null ? response.Payload.moveinScreenMessageList.ToList().Where(x => x.category.Equals("WN")).Select(y => y.message).ToArray() : new string[] { };
                        }
                        if (this.IsLoggedIn)
                        {
                            State.UserId = CurrentPrincipal.UserId;
                        }
                        else
                        {
                            State.UserId = !string.IsNullOrWhiteSpace(State.UserId) ? State.UserId : response.Payload.moveinnotif.userid;
                        }
                        State.BusinessPartner = !string.IsNullOrWhiteSpace(State.BusinessPartner) ? State.BusinessPartner : response.Payload.moveinnotif.businesspartnernumber;
                        State.SkiptoPayment = response.Payload.moveinnotif.skippayment;
                        State.premiseAmountDetails = response.Payload.premiseAmountDetailsList;
                        State.MovetoTransactionNumber = response.Payload.moveinnotif.movetotransactionid;
                        Save();
                        CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));
                        if (!string.IsNullOrWhiteSpace(State.SkiptoPayment))
                        {
                            State.Total = double.Parse(State.SecurityDeposit) + double.Parse(State.ReconnectionRegistrationFee) + double.Parse(State.AddressRegistrationFee) + double.Parse(State.KnowledgeFee) + double.Parse(State.TotalOutstandingFee) + double.Parse(State.InnovationFee) + double.Parse(State.ReconnectionVATamt) + double.Parse(State.AddressVAtamt);
                            State.MoveInNotificationNumber = response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).FirstOrDefault();
                            State.transactionList = response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray();
                            Save();
                            if (string.IsNullOrWhiteSpace(State.UserId))
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNTv3);
                            }
                            if (State.Attachment1.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment1Filename, State.Attachment1FileType, State.Attachment1, RequestLanguage, Request.Segment());
                            }
                            if (State.Attachment2.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment2Filename, State.Attachment2FileType, State.Attachment2, RequestLanguage, Request.Segment());
                            }
                            if (State.Attachment3.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment3Filename, State.Attachment3FileType, State.Attachment3, RequestLanguage, Request.Segment());
                            }
                            if (State.Attachment4.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment4Filename, State.Attachment4FileType, State.Attachment4, RequestLanguage, Request.Segment());
                            }
                            if (State.Attachment5.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment5Filename, State.Attachment5FileType, State.Attachment5, RequestLanguage, Request.Segment());
                            }
                            if (State.Attachment6.Length > 0)
                            {
                                DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, State.transactionList, State.Attachment6Filename, State.Attachment6FileType, State.Attachment6, RequestLanguage, Request.Segment());
                            }
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_CONFIRMATION_PAGEv3);
                        }

                        if (this.IsLoggedIn)
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_PAYMENT_PAGEv3);
                        }
                        else if (string.IsNullOrWhiteSpace(State.UserId))
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNTv3);
                        }
                        else
                        {
                            ISitecoreContext isitecoreContext = new SitecoreContext();
                            var currentitem = isitecoreContext.GetCurrentItem<Item>();
                            var lang = currentitem.Language.CultureInfo.TextInfo.IsRightToLeft ? "ar-AE" : "en";
                            CacheProvider.Store(CacheKeys.MOVEIN_USERID, new CacheItem<string>(State.UserId));
                            QueryString a = new QueryString();
                            a.With("returnUrl", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.LOGIN_MOVEIN_PAYMENT_PAGEv3), true);
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE, a);
                        }
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.MOVEIN_ERROR_MESSAGE, new CacheItem<string>(response.Message));

                        if (response.Payload.responsecode != "399")
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_CONTACT_DETAILSv3);
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_CONTACT_DETAILSv3);
                    }
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
                return View("~/Views/Feature/SupplyManagement/MoveIn/ContactDetails.cshtml", State);
            }
        }

        [HttpGet]
        public ActionResult PaymentDetails()
        {
            if (!InProgress())
            {
                if (this.IsLoggedIn)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
            }
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            CacheProvider.Remove(CacheKeys.MOVEIN_USERID);
            string termsLink;
            bool nationalTermsLink = false;
            if (State.CustomerType.Equals("U"))
            {
                nationalTermsLink = true;
                termsLink = UtilExtensions.GetLinkUrl(Sitecorex.Database.GetItem(SitecoreItemIdentifiers.MOVEIN_CONSTANTS),
                            "Terms and Conditions Link National");
            }
            else
            {
                termsLink = UtilExtensions.GetLinkUrl(Sitecorex.Database.GetItem(SitecoreItemIdentifiers.MOVEIN_CONSTANTS),
                              "Terms and Conditions Link Expat");
            }

            State.PaymentPage = true;
            return View("~/Views/Feature/SupplyManagement/MoveIn/PaymentDetails.cshtml", new PaymentDetailsViewModel()
            {
                SecurityDeposit = double.Parse(State.SecurityDeposit),
                ReconnectionRegistrationFee = double.Parse(State.ReconnectionRegistrationFee),
                AddressRegistrationFee = double.Parse(State.AddressRegistrationFee),
                ReconnectionVATrate = State.ReconnectionVATrate,
                ReconnectionVATamt = double.Parse(State.ReconnectionVATamt),
                AddressVATrate = State.AddressVATrate,
                AddressVAtamt = double.Parse(State.AddressVAtamt),
                KnowledgeFee = double.Parse(State.KnowledgeFee),
                TotalOutstandingAmt = double.Parse(State.TotalOutstandingFee),
                InnovationFee = double.Parse(State.InnovationFee),
                TermsLink = termsLink,
                NationalTermsLink = nationalTermsLink,
                BusinessPartner = State.BusinessPartner,
                UserName = State.FirstName + " " + ((!string.IsNullOrWhiteSpace(State.LastName) && !State.LastName.Equals(".")) ? State.LastName : string.Empty),
                PayLater = State.paylater != null ? State.paylater.Equals("X") : false,
                PayOther = State.payother != null ? State.payother.Equals("X") : false,
                DiscountApplied = !string.IsNullOrWhiteSpace(State.MovetoTransactionNumber) ? true : false,
                PayChannelList = State.PayChannelList,
                messagepaychannel = State.messagepaychannel,
                MaiDubaiContribution = State.maidubaicontribution,
                MaiDubaiGift = State.maidubaigift,
                MaiDubaiTitle = State.maidubaimsgtitle,
                MaiDubaiMsg = State.maidubaimsgtext,
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PaymentDetails(PaymentDetailsViewModel model)
        {
            try
            {
                if (!InProgress())
                {
                    if (this.IsLoggedIn)
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
                    }
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
                }
                if (model.IsPaylaterSelected)
                {
                    State.Total = double.Parse(State.SecurityDeposit) + double.Parse(State.ReconnectionRegistrationFee) + double.Parse(State.AddressRegistrationFee) + double.Parse(State.KnowledgeFee) + double.Parse(State.TotalOutstandingFee) + double.Parse(State.InnovationFee) + double.Parse(State.ReconnectionVATamt) + double.Parse(State.AddressVAtamt);
                    Save();
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_REVIEW_PAGEv3);
                }
                else
                {
                    bool processed = false;
                    if (CacheProvider.TryGet(CacheKeys.MOVEIN_PROCESSED, out processed))
                    {
                        processed = true;
                    }
                    //Added for Future Center
                    var _fc = FetchFutureCenterValues();

                    CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));
                    var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                    {
                        premiseDetailsList = State.PremiseAccount.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new premiseDetails { premise = x }).ToArray(),
                        moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           customercategory=State.CustomerCategory,
                           customertype= !string.IsNullOrEmpty(State.CustomerTypeForNonIndividual) ? State.CustomerTypeForNonIndividual : State.CustomerType,
                           accounttype =State.AccountType,
                           occupiedby=State.Owner?(State.occupiedbyowner?"O":"T"):string.Empty,
                           businesspartnernumber=State.BusinessPartner,
                           ejarinumber=State.isEjari?State.Ejari:string.Empty,
                           moveindate = State.MoveinStartDate.HasValue ? State.MoveinStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           contractnumber=State.Owner?State.Purchase_TitleDeed :( State.isEjari!=true? State.Tenancy_Contract_Number:string.Empty),
                           tenancystartdate=State.ContractStartDate.HasValue?State.ContractStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancyenddate=State.ContractEndDate.HasValue?State.ContractEndDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancycontractvalue=State.isEjari!=true?State.ContractValue:string.Empty,
                           noofrooms=(State.Owner || State.isEjari!=true)?State.NumberOfRooms.ToString():string.Empty,
                           vatnumber=State.Vatnumber,
                           center =_fc.Branch,
                           idtype=State.IdType,
                           idexpiry =  !string.IsNullOrEmpty(State.ExpiryDate) ? Convert.ToDateTime(State.ExpiryDate).ToString("yyyyMMdd") : string.Empty,
                           dateofbirth = !string.IsNullOrEmpty(State.DateOfBirth) ? Convert.ToDateTime(State.DateOfBirth).ToString("yyyyMMdd") : string.Empty,
                           idnumber=State.IdNumber,
                           firtname= State.FirstName,
                           lastname=State.LastName,
                           mobile=State.MobilePhone.AddMobileNumberZeroPrefix(),
                           email=State.EmailAddress,
                           pobox=State.PoBox,
                           nationality=State.Nationality,
                           region=State.Emirate,
                           nationalsocialnumber=State.nationalsocialcardnumber,
                           sanadnumber=State.sanadsmartcardnumber,
                           thukuhrnumber=State.thukercardnumber,
                           password=State.Password,
                           createuseraccount=processed?string.Empty:State.createuseraccount,
                           departmentid =State.departmentnameid,
                           propertyid = State.Propertyid,
                           departmentnameothers = State.departmentnameothers,
                           sdpaylaterflag=model.IsPaylaterSelected?"X":string.Empty,
                           sdpayotherflag=model.IsPayotherSelected?"X":string.Empty,
                           guranteeletterflag=State.isguaranteeleter?"X":string.Empty,
                           maidubaicond = State.maidubaigift && model.MaiDubaiContribution?"X":string.Empty,
                       }
                    },
                        userid = State.UserId,
                        sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                        lang = RequestLanguage.Code(),
                        executionflag = "W",
                        channel = "A",
                        applicationflag = "M"
                    }, Request.Segment());
                    if (response.Succeeded)
                    {
                        if (State.Attachment1.Length > 0)
                        {
                            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment1Filename, State.Attachment1FileType, State.Attachment1, RequestLanguage, Request.Segment());
                        }
                        if (State.Attachment2.Length > 0)
                        {
                            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment2Filename, State.Attachment2FileType, State.Attachment2, RequestLanguage, Request.Segment());
                        }
                        if (State.Attachment3.Length > 0)
                        {
                            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment3Filename, State.Attachment3FileType, State.Attachment3, RequestLanguage, Request.Segment());
                        }
                        if (State.Attachment4.Length > 0)
                        {
                            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment4Filename, State.Attachment4FileType, State.Attachment4, RequestLanguage, Request.Segment());
                        }
                        if (State.Attachment5.Length > 0)
                        {
                            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment5Filename, State.Attachment5FileType, State.Attachment5, RequestLanguage, Request.Segment());
                        }
                        if (State.Attachment6.Length > 0)
                        {
                            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment6Filename, State.Attachment6FileType, State.Attachment6, RequestLanguage, Request.Segment());
                        }
                        State.ContractAccountnumber = response.Payload.premiseAmountDetailsList.Where(x => !string.IsNullOrWhiteSpace(x.contractaccountnumber)).Any() ? response.Payload.premiseAmountDetailsList.Where(x => !string.IsNullOrWhiteSpace(x.contractaccountnumber)).Select(x => x.contractaccountnumber).ToArray() : new string[0];
                        State.premiseAmountDetails = response.Payload.premiseAmountDetailsList;
                        State.MoveInNotificationNumber = response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).FirstOrDefault();
                        State.maidubaicontribution = State.maidubaigift ? model.MaiDubaiContribution: false;
                        if (response.Payload.moveinScreenMessageList != null && response.Payload.moveinScreenMessageList.Count() > 0)
                        {
                            State.messagewhatsnext = response.Payload.moveinScreenMessageList.ToList().Where(x => x.category.Equals("WN")) != null ? response.Payload.moveinScreenMessageList.ToList().Where(x => x.category.Equals("WN")).Select(y => y.message).ToArray() : new string[] { };
                        }
                        Save();
                        CacheProvider.Store(CacheKeys.MOVEIN_PROCESSED, new CacheItem<bool>(true));
                        if (!model.IsPayotherSelected)
                        {
                            #region [MIM Payment Implementation]

                            var payRequest = new CipherPaymentModel();
                            payRequest.PaymentData.amounts = string.Join(",", response.Payload.premiseAmountDetailsList.ToList().Select(x => x.amount));
                            payRequest.PaymentData.contractaccounts = string.Join(",", response.Payload.premiseAmountDetailsList.ToList().Select(x => (!string.IsNullOrWhiteSpace(x.contractaccountnumber) ? x.contractaccountnumber : x.premise)));
                            payRequest.PaymentData.businesspartner = State.BusinessPartner ?? string.Empty;
                            payRequest.PaymentData.email = State.UsemaskedEmailAddress ? State.unmaskedEmailAddress : State.EmailAddress;
                            payRequest.PaymentData.mobile = State.UsemaskedMobilenumber ? (!string.IsNullOrWhiteSpace(State.unmaskedMobilePhone) ? State.unmaskedMobilePhone.AddMobileNumberZeroPrefix() : string.Empty) : State.MobilePhone.AddMobileNumberZeroPrefix();
                            payRequest.PaymentData.userid = State.UserId;
                            payRequest.ServiceType = ServiceType.ServiceActivation;
                            payRequest.PaymentMethod = model.paymentMethod;
                            payRequest.IsThirdPartytransaction = true;
                            payRequest.BankKey = model.bankkey;
                            payRequest.SuqiaValue = model.SuqiaDonation;
                            payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                            var payResponse = ExecutePaymentGateway(payRequest);
                            if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                            {
                                CacheProvider.Store(CacheKeys.MOVEIN_MIM_MODEL, new CacheItem<CipherPaymentModel>(payRequest));
                                return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                            }
                            ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

                            #endregion [MIM Payment Implementation]
                        }
                        else
                        {
                            State.Total = double.Parse(State.SecurityDeposit) + double.Parse(State.ReconnectionRegistrationFee) + double.Parse(State.AddressRegistrationFee) + double.Parse(State.KnowledgeFee) + double.Parse(State.TotalOutstandingFee) + double.Parse(State.InnovationFee) + double.Parse(State.ReconnectionVATamt) + double.Parse(State.AddressVAtamt);
                            State.MoveInNotificationNumber = response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).FirstOrDefault();
                            Save();
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_CONFIRMATION_PAGEv3);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_PAYMENT_PAGEv3);
            }
            string termsLink;
            if (State.Nationality != null && State.Nationality.ToLower().Equals("ae"))
            {
                termsLink = UtilExtensions.GetLinkUrl(Sitecorex.Database.GetItem(SitecoreItemIdentifiers.MOVEIN_CONSTANTS),
                            "Terms and Conditions Link National");
            }
            else
            {
                termsLink = UtilExtensions.GetLinkUrl(Sitecorex.Database.GetItem(SitecoreItemIdentifiers.MOVEIN_CONSTANTS),
                              "Terms and Conditions Link Expat");
            }
            return View("~/Views/Feature/SupplyManagement/MoveIn/PaymentDetails.cshtml", new PaymentDetailsViewModel()
            {
                SecurityDeposit = double.Parse(State.SecurityDeposit),
                ReconnectionRegistrationFee = double.Parse(State.ReconnectionRegistrationFee),
                AddressRegistrationFee = double.Parse(State.AddressRegistrationFee),
                ReconnectionVATrate = State.ReconnectionVATrate,
                ReconnectionVATamt = double.Parse(State.ReconnectionVATamt),
                AddressVATrate = State.AddressVATrate,
                AddressVAtamt = double.Parse(State.AddressVAtamt),
                KnowledgeFee = double.Parse(State.KnowledgeFee),
                TotalOutstandingAmt = double.Parse(State.TotalOutstandingFee),
                InnovationFee = double.Parse(State.InnovationFee),
                TermsLink = termsLink,
                BusinessPartner = State.BusinessPartner,
                UserName = State.FirstName + " " + ((!string.IsNullOrWhiteSpace(State.LastName) && !State.LastName.Equals(".")) ? State.LastName : string.Empty),
                PayLater = State.paylater != null ? State.paylater.Equals("X") : false,
                PayOther = State.payother != null ? State.payother.Equals("X") : false,
                DiscountApplied = !string.IsNullOrWhiteSpace(State.MovetoTransactionNumber) ? true : false,
                PayChannelList = State.PayChannelList,
                messagepaychannel = State.messagepaychannel,
                MaiDubaiContribution = State.maidubaicontribution,
                MaiDubaiGift = State.maidubaigift,
                MaiDubaiTitle = State.maidubaimsgtitle,
                MaiDubaiMsg = State.maidubaimsgtext,
            });
        }

        [HttpGet]
        public ActionResult PaylaterReview()
        {
            if (!InProgress())
            {
                if (this.IsLoggedIn)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
            }
            MoveInViewModelv3 state;
            if (CacheProvider.TryGet(CacheKeys.MOVE_IN_3_WORKFLOW_STATE, out state))
            {
                return View("~/Views/Feature/SupplyManagement/MoveIn/PaylaterReview.cshtml", state);
            }
            if (IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PaylaterReview(MoveInViewModelv3 model)
        {
            try
            {
                bool processed = false;
                if (CacheProvider.TryGet(CacheKeys.MOVEIN_PROCESSED, out processed))
                {
                    processed = true;
                }
                //Added for Future Center
                var _fc = FetchFutureCenterValues();

                CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));
                var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                {
                    premiseDetailsList = State.PremiseAccount.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new premiseDetails { premise = x }).ToArray(),
                    moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           customercategory=State.CustomerCategory,
                           customertype= !string.IsNullOrEmpty(State.CustomerTypeForNonIndividual) ? State.CustomerTypeForNonIndividual : State.CustomerType,
                           accounttype =State.AccountType,
                           occupiedby=State.Owner?(State.occupiedbyowner?"O":"T"):string.Empty,
                           businesspartnernumber=State.BusinessPartner,
                           ejarinumber=State.isEjari?State.Ejari:string.Empty,
                           moveindate = State.MoveinStartDate.HasValue ? State.MoveinStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           contractnumber=State.Owner?State.Purchase_TitleDeed :( State.isEjari!=true? State.Tenancy_Contract_Number:string.Empty),
                           tenancystartdate=State.ContractStartDate.HasValue?State.ContractStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancyenddate=State.ContractEndDate.HasValue?State.ContractEndDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancycontractvalue=State.isEjari!=true?State.ContractValue:string.Empty,
                           noofrooms=(State.Owner || State.isEjari!=true)?State.NumberOfRooms.ToString():string.Empty,
                           vatnumber=State.Vatnumber,
                           center =_fc.Branch,
                           idtype=State.IdType,
                           idexpiry =  !string.IsNullOrEmpty(State.ExpiryDate) ? Convert.ToDateTime(State.ExpiryDate).ToString("yyyyMMdd") : string.Empty,
                           dateofbirth = !string.IsNullOrEmpty(State.DateOfBirth) ? Convert.ToDateTime(State.DateOfBirth).ToString("yyyyMMdd") : string.Empty,
                           idnumber=State.IdNumber,
                           firtname= State.FirstName,
                           lastname=State.LastName,
                           mobile=State.MobilePhone.AddMobileNumberZeroPrefix(),
                           email=State.EmailAddress,
                           pobox=State.PoBox,
                           nationality=State.Nationality,
                           region=State.Emirate,
                           nationalsocialnumber=State.nationalsocialcardnumber,
                           sanadnumber=State.sanadsmartcardnumber,
                           thukuhrnumber=State.thukercardnumber,
                           password=State.Password,
                           createuseraccount=processed?string.Empty:State.createuseraccount,
                           departmentid =State.departmentnameid,
                           propertyid = State.Propertyid,
                           departmentnameothers = State.departmentnameothers,
                           sdpaylaterflag="X",
                           guranteeletterflag=State.isguaranteeleter?"X":string.Empty
                       }
                    },
                    userid = State.UserId,
                    sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                    lang = RequestLanguage.Code(),
                    executionflag = "W",
                    channel = "A",
                    applicationflag = "M"
                }, Request.Segment());
                if (response.Succeeded)
                {
                    if (State.Attachment1.Length > 0)
                    {
                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment1Filename, State.Attachment1FileType, State.Attachment1, RequestLanguage, Request.Segment());
                    }
                    if (State.Attachment2.Length > 0)
                    {
                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment2Filename, State.Attachment2FileType, State.Attachment2, RequestLanguage, Request.Segment());
                    }
                    if (State.Attachment3.Length > 0)
                    {
                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment3Filename, State.Attachment3FileType, State.Attachment3, RequestLanguage, Request.Segment());
                    }
                    if (State.Attachment4.Length > 0)
                    {
                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment4Filename, State.Attachment4FileType, State.Attachment4, RequestLanguage, Request.Segment());
                    }
                    if (State.Attachment5.Length > 0)
                    {
                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment5Filename, State.Attachment5FileType, State.Attachment5, RequestLanguage, Request.Segment());
                    }
                    if (State.Attachment6.Length > 0)
                    {
                        DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).ToArray(), State.Attachment6Filename, State.Attachment6FileType, State.Attachment6, RequestLanguage, Request.Segment());
                    }
                    State.ContractAccountnumber = response.Payload.premiseAmountDetailsList.Where(x => !string.IsNullOrWhiteSpace(x.contractaccountnumber)).Any() ? response.Payload.premiseAmountDetailsList.Where(x => !string.IsNullOrWhiteSpace(x.contractaccountnumber)).Select(x => x.contractaccountnumber).ToArray() : new string[0];
                    State.premiseAmountDetails = response.Payload.premiseAmountDetailsList;
                    Save();
                    CacheProvider.Store(CacheKeys.MOVEIN_PROCESSED, new CacheItem<bool>(true));
                    State.Total = double.Parse(State.SecurityDeposit) + double.Parse(State.ReconnectionRegistrationFee) + double.Parse(State.AddressRegistrationFee) + double.Parse(State.KnowledgeFee) + double.Parse(State.TotalOutstandingFee) + double.Parse(State.InnovationFee) + double.Parse(State.ReconnectionVATamt) + double.Parse(State.AddressVAtamt);
                    State.MoveInNotificationNumber = response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).FirstOrDefault();
                    Save();
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_CONFIRMATION_PAGEv3);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return View("~/Views/Feature/SupplyManagement/MoveIn/PaylaterReview.cshtml", State);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_REVIEW_PAGEv3);
            }
        }

        [HttpGet]
        public ActionResult PaylaterConfirmation()
        {
            if (!InProgress())
            {
                if (this.IsLoggedIn)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_LANDING_PAGEv3);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGEv3);
            }
            MoveInViewModelv3 model = State;
            MoveInViewModelv3 state;
            if (CacheProvider.TryGet(CacheKeys.MOVE_IN_3_WORKFLOW_STATE, out state))
            {
                CacheProvider.Remove(CacheKeys.MOVE_IN_3_WORKFLOW_STATE);
            }
            CacheProvider.Remove(CacheKeys.MOVEIN_JOURNEY);
            return View("~/Views/Feature/SupplyManagement/MoveIn/PaylaterConfirmation.cshtml", model);
        }

        #endregion Logged in

        #region Security Deposit Document download

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult SDDocumentdownload(string q)
        {
            if (!string.IsNullOrWhiteSpace(q))
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
                return View("~/Views/Feature/SupplyManagement/MoveIn/SDDocumentdownload.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ERROR_404);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SDDocumentdownload(string q, string a)
        {
            bool status = false;

            if (!string.IsNullOrWhiteSpace(q))
            {
                string recaptchaResponse = System.Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

                if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(recaptchaResponse))
                {
                    status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
                }
                else if (!ReCaptchaHelper.Recaptchasetting())
                {
                    status = true;
                }

                if (status)
                {
                    var response = DewaApiClient.GetBillDetailsPDF(q, RequestLanguage, Request.Segment());

                    if (response.Succeeded && response.Payload.billData != null && response.Payload.billData.Length > 0)
                    {
                        CacheProvider.Store(CacheKeys.anonymous_bill_download, new AccessCountingCacheItem<byte[]>(response.Payload.billData, Times.Once));
                        return RedirectToAction("SDDocdownload", "MoveIn");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                    ViewBag.Recaptcha = true;
                }
                else
                {
                    ViewBag.Recaptcha = false;
                }
                return View("~/Views/Feature/SupplyManagement/MoveIn/SDDocumentdownload.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ERROR_404);
        }

        [HttpGet]
        public ActionResult SDDocdownload()
        {
            byte[] bytes;
            if (CacheProvider.TryGet(CacheKeys.anonymous_bill_download, out bytes))
            {
                return File(bytes, System.Net.Mime.MediaTypeNames.Application.Pdf);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ERROR_404);
        }

        #endregion Security Deposit Document download

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetDetails(string idtype, string idnumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(idtype))
                {
                    State.IdType = idtype;
                }
                if (!string.IsNullOrEmpty(idnumber))
                {
                    State.IdNumber = idnumber;
                }
                var _fc = FetchFutureCenterValues();
                //string bp = string.Empty;
                string userid = string.Empty;
                if (IsLoggedIn)
                {
                    //bp = State.BusinessPartner;
                    userid = State.UserId;
                }
                var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                {
                    premiseDetailsList = State.PremiseAccount.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new premiseDetails { premise = x }).ToArray(),
                    moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           customercategory=State.CustomerCategory,
                           customertype= !string.IsNullOrEmpty(State.CustomerTypeForNonIndividual) ? State.CustomerTypeForNonIndividual : State.CustomerType,
                           accounttype =State.AccountType,
                           occupiedby=State.Owner?(State.occupiedbyowner?"O":"T"):string.Empty,
                           businesspartnernumber=State.Isbp ? State.BusinessPartner : string.Empty,
                           ejarinumber=State.isEjari?State.Ejari:string.Empty,
                           moveindate = State.MoveinStartDate.HasValue ? State.MoveinStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           contractnumber=State.Owner?State.Purchase_TitleDeed :( State.isEjari!=true? State.Tenancy_Contract_Number:string.Empty),
                           tenancystartdate=State.ContractStartDate.HasValue?State.ContractStartDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancyenddate=State.ContractEndDate.HasValue?State.ContractEndDate.Value.ToString("yyyyMMdd") : string.Empty,
                           tenancycontractvalue=State.isEjari!=true?State.ContractValue:string.Empty,
                           noofrooms=(State.Owner || State.isEjari!=true)?State.NumberOfRooms.ToString():string.Empty,
                           vatnumber=State.Vatnumber,
                           propertyid = State.Propertyid,
                           idtype=idtype,
                           idnumber=idnumber,
                           center =_fc.Branch
                       }
                    },
                    userid = userid,
                    sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                    lang = RequestLanguage.Code(),
                    executionflag = "I",
                    channel = "A",
                    applicationflag = "M"
                }, Request.Segment());

                if (response.Succeeded && response.Payload != null)
                {
                    if (response.Succeeded && response.Payload != null && response.Payload.moveinnotif != null)
                    {
                        State.FirstnameHide = State.Isfirstname ? false : !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname);
                        State.LastnameHide = State.Islastname ? false : !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.lastname);
                        State.FirstName = State.Isfirstname ? State.FirstName : response.Payload.moveinnotif.firtname;
                        State.LastName = State.Islastname ? State.LastName : response.Payload.moveinnotif.lastname;
                        State.MobilePhone = State.IsMobilePhone ? State.MobilePhone : !string.IsNullOrWhiteSpace(response.Payload.moveinnotif.mobile) ? response.Payload.moveinnotif.mobile.First().Equals('0') ? response.Payload.moveinnotif.mobile.Remove(0, 1) : response.Payload.moveinnotif.mobile : string.Empty;
                        State.EmailAddress = State.IsEmailAddress ? State.EmailAddress : response.Payload.moveinnotif.email;
                        State.unmaskedMobilePhone = State.IsunmaskedMobilePhone ? State.unmaskedMobilePhone : response.Payload.moveinnotif.unmaskedmobile;
                        State.unmaskedEmailAddress = State.IsunmaskedEmailAddress ? State.unmaskedEmailAddress : response.Payload.moveinnotif.unmaskedemail;
                        State.Nationality = State.IsNationality ? State.Nationality : response.Payload.moveinnotif.nationality;
                        State.PoBox = State.IsPoBox ? State.PoBox : response.Payload.moveinnotif.pobox;
                        State.BusinessPartner = State.Isbp ? State.BusinessPartner : response.Payload.moveinnotif.businesspartnernumber;
                        State.Emirate = State.IsEmirate ? State.Emirate : response.Payload.moveinnotif.region;
                        State.TempUserId = response.Payload.moveinnotif.userid;
                        //State.UserId = !string.IsNullOrWhiteSpace(State.UserId) ? State.UserId : response.Payload.moveinnotif.userid;
                        //State.AttachmentFlag = ((string.Equals(State.IdType, "ED") && string.IsNullOrWhiteSpace(response.Payload.moveinnotif.firtname)) || string.Equals(State.IdType, "PN")) ? true : false;
                        Save();

                        DateTime _idExpiry = DateTime.MinValue;
                        DateTime _dob = DateTime.MinValue;

                        DateTime.TryParse(response.Payload.moveinnotif.idexpiry, out _idExpiry);
                        string _sendIdExpiry = _idExpiry == DateTime.MinValue ? string.Empty : _idExpiry.ToString("dd MMMM yyyy");

                        DateTime.TryParse(response.Payload.moveinnotif.dateofbirth, out _dob);
                        string _sendDOB = _dob == DateTime.MinValue ? string.Empty : _dob.ToString("dd MMMM yyyy");


                        return Json(new { DOB = _sendDOB, IdExp = _sendIdExpiry, FirstName = State.FirstName, LastName = State.LastName, MobilePhone = State.MobilePhone, EmailAddress = State.EmailAddress, Nationality = State.Nationality, PoBox = State.PoBox, Emirate = State.Emirate, Message = string.Empty });
                    }
                }
                else
                {
                    return Json(new { DOB = !string.IsNullOrEmpty(State.DateOfBirth) ? Convert.ToDateTime(State.DateOfBirth).ToString("yyyyMMdd") : string.Empty, IdExp = !string.IsNullOrEmpty(State.ExpiryDate) ? Convert.ToDateTime(State.ExpiryDate).ToString("yyyyMMdd") : string.Empty, FirstName = State.FirstName, LastName = State.LastName, MobilePhone = State.MobilePhone, EmailAddress = State.EmailAddress, Nationality = State.Nationality, PoBox = State.PoBox, Emirate = State.Emirate, Message = response.Message });
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return null;
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult ClearMoveInCache()
        {
            MoveInViewModelv3 state;
            if (CacheProvider.TryGet(CacheKeys.MOVE_IN_3_WORKFLOW_STATE, out state))
            {
                CacheProvider.Remove(CacheKeys.MOVE_IN_3_WORKFLOW_STATE);
            }
            CacheProvider.Remove(CacheKeys.MOVEIN_JOURNEY);
            CacheProvider.Remove(CacheKeys.MOVEIN_PASSEDMODEL);
            CacheProvider.Remove(CacheKeys.MOVEIN_PASSEDRESPONSE);
            return new EmptyResult();
        }

        #endregion Actions

        #region Methods

        private List<SelectListItem> GetPersonBusinessPartners()
        {
            var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken,
                RequestLanguage, Request.Segment());

            if (UserDetails.Payload != null && UserDetails.Payload.BusinessPartners != null)
            {
                List<BusinessPartner> lstBusinessPartner = UserDetails.Payload.BusinessPartners;
                CacheProvider.Store(CacheKeys.MOVEIN_lST_BUSINESSPARTNER, new CacheItem<List<BusinessPartner>>(lstBusinessPartner));
                var bp = UserDetails.Payload.BusinessPartners.Where(x => x.CustomerType == "Person").Select(c => new SelectListItem
                {
                    Text = c.businesspartnernumber + '-' + c.bpname,
                    Value = c.businesspartnernumber
                }).ToList();
                return bp;
            }

            List<SelectListItem> _emptyList = new List<SelectListItem>();

            return _emptyList;
        }

        private List<SelectListItem> GetOrganizationBusinessPartners()
        {
            List<BusinessPartner> selection;
            List<SelectListItem> bp;
            if (CacheProvider.TryGet(CacheKeys.MOVEIN_lST_BUSINESSPARTNER, out selection))
            {
                bp = selection.Where(x => x.CustomerType == "Organization").Select(c => new SelectListItem
                {
                    Text = c.businesspartnernumber + '-' + c.bpname,
                    Value = c.businesspartnernumber
                }).ToList();
            }
            else
            {
                var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (UserDetails.Payload != null && UserDetails.Payload.BusinessPartners != null)
                {
                    bp = UserDetails.Payload.BusinessPartners.Where(x => x.CustomerType == "Organization").Select(c => new SelectListItem
                    {
                        Text = c.businesspartnernumber + '-' + c.bpname,
                        Value = c.businesspartnernumber
                    }).ToList();
                }
                else
                {
                    bp = new List<SelectListItem>();
                }
            }
            return bp;
        }

        private List<SelectListItem> GetGovBusinessPartners()
        {
            List<BusinessPartner> selection;
            List<SelectListItem> bp;
            if (CacheProvider.TryGet(CacheKeys.MOVEIN_lST_BUSINESSPARTNER, out selection))
            {
                bp = selection.Where(x => x.CustomerType == "Organization" && x.BPType == "Z001").Select(c => new SelectListItem
                {
                    Text = c.businesspartnernumber + '-' + c.bpname,
                    Value = c.businesspartnernumber
                }).ToList();
            }
            else
            {
                var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (UserDetails.Payload != null && UserDetails.Payload.BusinessPartners != null)
                {
                    bp = UserDetails.Payload.BusinessPartners.Where(x => x.CustomerType == "Organization").Select(c => new SelectListItem
                    {
                        Text = c.businesspartnernumber + '-' + c.bpname,
                        Value = c.businesspartnernumber
                    }).ToList();
                }
                else
                {
                    bp = new List<SelectListItem>();
                }
            }
            return bp;
        }

        #endregion Methods
    }
}