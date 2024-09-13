using DEWAXP.Feature.SupplyManagement.Models.MoveIn;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.SupplyManagement.Movein;
using DEWAXP.Foundation.Content.Models.SupplyManagement.MoveTo;
using Sitecore.Globalization;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class MoveIntoController : BaseServiceActivationController
    {
        [HttpGet]
        public ActionResult MoveIntoLanding()
        {
            MoveIntoIntro model = new MoveIntoIntro();
            model.IsLoggedIn = IsLoggedIn;
            return PartialView("~/Views/Feature/SupplyManagement/MoveIn/_MoveIntoLanding.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MoveIntoLanding(FormCollection collection)
        {
            string redirectItem = string.Empty;

            if (collection != null)
            {
                int Selvalue = 0;

                if (collection.Keys.Count > 0)
                {
                    int.TryParse(collection.Get("radios_group1"), out Selvalue);
                }

                switch (Selvalue)
                {
                    case 1: //Move Into
                        ClearMoveInCache();
                        ClearMoveToCache();
                        redirectItem = SitecoreItemIdentifiers.MOVEINTOLANDING_PRELOGIN;
                        break;

                    case 2: // Move Into
                        ClearMoveInCache();
                        ClearMoveToCache();
                        redirectItem = SitecoreItemIdentifiers.MOVEINTOLANDING_LOGIN;
                        break;

                    case 3: // Move To
                        ClearMoveToCache();
                        ClearMoveInCache();
                        redirectItem = SitecoreItemIdentifiers.MOVEINTOLANDING_MOVETO;
                        break;

                    case 4: //Move Out
                        ClearMoveOutCache();
                        redirectItem = SitecoreItemIdentifiers.MOVEINTOLANDING_MOVEOUT;
                        break;

                    case 5: //Move Out
                        redirectItem = SitecoreItemIdentifiers.MOVEINTOLANDING_MOVEOUT_DEMOLISH;
                        break;
                }
            }
            return RedirectToSitecoreItem(redirectItem);
        }

        //[HttpGet]
        //public ActionResult MoveInAccountRegister()
        //{
        //    string errorMessage;
        //    bool moveinindicator = false;
        //    if (CacheProvider.TryGet(CacheKeys.MOVEIN_ERROR_MESSAGE, out errorMessage))
        //    {
        //        ModelState.AddModelError(string.Empty, errorMessage);
        //        CacheProvider.Remove(CacheKeys.MOVEIN_ERROR_MESSAGE);
        //    }
        //    string journey = string.Empty;
        //    if (!CacheProvider.TryGet(CacheKeys.MOVEIN_JOURNEY, out journey) && string.IsNullOrWhiteSpace(errorMessage))
        //    {
        //        Clear();
        //    }
        //    if (CacheProvider.TryGet(CacheKeys.MOVEIN_INDICATOR, out moveinindicator))
        //    {
        //        State.moveinindicator = true;
        //        CacheProvider.Remove(CacheKeys.MOVEIN_INDICATOR);
        //    }
        //    var model = new AccountDetailsViewModel
        //    {
        //        UserId = CurrentPrincipal.UserId,
        //        SessionToken = CurrentPrincipal.SessionToken,
        //        BusinessPartner = !string.IsNullOrWhiteSpace(State.BusinessPartner) ? State.BusinessPartner : string.Empty,
        //        CustomerCategory = State.CustomerCategory,
        //        CustomerType = State.CustomerType,
        //        AccountType = State.AccountType,
        //        IdType = !string.IsNullOrWhiteSpace(State.IdType) ? State.IdType : (!string.IsNullOrWhiteSpace(State.Ejari) ? "EJ" : string.Empty),
        //        IdNumber = !string.IsNullOrWhiteSpace(State.IdNumber) ? State.IdNumber : (!string.IsNullOrWhiteSpace(State.Ejari) ? State.Ejari : string.Empty),
        //        DocumentExpiryDate = State.ExpiryDate,
        //        loggedinuser = this.IsLoggedIn ? true : false,
        //        SecurityDeposit = State.SecurityDeposit,
        //        ReconnectionRegistrationFee = State.ReconnectionRegistrationFee,
        //        AddressRegistrationFee = State.AddressRegistrationFee,
        //        ReconnectionVATrate = State.ReconnectionVATrate,
        //        ReconnectionVATamt = State.ReconnectionVATamt,
        //        AddressVATrate = State.AddressVATrate,
        //        AddressVAtamt = State.AddressVAtamt,
        //        OutstandingBalance = State.OutstandingBalance,
        //        NumberOfRooms = State.NumberOfRooms,
        //        owner = State.Owner,
        //        MoveinStartDate = State.MoveinStartDate,
        //        PremiseAccount = State.PremiseAccount,
        //        Moveinindicator = State.moveinindicator,
        //        VatNumber = State.Vatnumber
        //    };
        //    State.UserId = !string.IsNullOrWhiteSpace(CurrentPrincipal.UserId) ? CurrentPrincipal.UserId : State.UserId;
        //    State.LandingPage = true;
        //    State.CreatePage = false;
        //    State.ContactPage = false;
        //    State.TenantPage = false;
        //    State.PaymentPage = false;
        //    ViewBag.IdTypes = GetResMoveinIdTypes();
        //    if (this.IsLoggedIn)
        //    {
        //        var personlist = GetPersonBusinessPartners();
        //        model.personbusinesspartner = personlist.Count > 0 ? true : false;
        //        ViewBag.PersonBusinessPartners = personlist;
        //        var organisationlist = GetOrganizationBusinessPartners();
        //        model.organisationbusinesspartner = organisationlist.Count > 0 ? true : false;
        //        ViewBag.OrganizationBusinessPartners = organisationlist;
        //    }
        //    ViewBag.CustomerTypes = GetResMoveinCustomerTypes();
        //    ViewBag.IdNonTypes = GetNonResMoveinIdTypes();
        //    ViewBag.NonCustomerTypes = GetNonResMoveinCustomerTypes();
        //    ViewBag.NumberOfRoomsBag = GetNumberOfRooms();
        //    return View("AccountRegister", model);
        //}

        //private List<SelectListItem> GetPersonBusinessPartners()
        //{
        //    var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken,
        //        RequestLanguage, Request.Segment());
        //    List<BusinessPartner> lstBusinessPartner = UserDetails.Payload.BusinessPartners;
        //    CacheProvider.Store(CacheKeys.MOVEIN_lST_BUSINESSPARTNER, new CacheItem<List<BusinessPartner>>(lstBusinessPartner));
        //    var bp = UserDetails.Payload.BusinessPartners.Where(x => x.CustomerType == "Person").Select(c => new SelectListItem
        //    {
        //        Text = c.businesspartnernumber,
        //        Value = c.businesspartnernumber
        //    }).ToList();

        //    return bp;
        //}

        //private List<SelectListItem> GetOrganizationBusinessPartners()
        //{
        //    List<BusinessPartner> selection;
        //    List<SelectListItem> bp;
        //    if (CacheProvider.TryGet(CacheKeys.MOVEIN_lST_BUSINESSPARTNER, out selection))
        //    {
        //        bp = selection.Where(x => x.CustomerType == "Organization").Select(c => new SelectListItem
        //        {
        //            Text = c.businesspartnernumber,
        //            Value = c.businesspartnernumber
        //        }).ToList();
        //    }
        //    else
        //    {
        //        var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
        //        bp = UserDetails.Payload.BusinessPartners.Where(x => x.CustomerType == "Organization").Select(c => new SelectListItem
        //        {
        //            Text = c.businesspartnernumber,
        //            Value = c.businesspartnernumber
        //        }).ToList();
        //    }
        //    return bp;
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult MoveInAccountRegister(string businesspartner, string idtype, string idexprirydate, string idnumber, string customerCategory, string custtype, string numberofrooms, string startindate, string[] premiseaccount, string vatnumber, HttpPostedFileBase vatdocument)
        //{
        //    //Added for Future Center
        //    var _fc = FetchFutureCenterValues();

        //    string ejarinumber = string.Empty;
        //    string ejaritype = string.Empty;

        //    if (string.Equals(idtype, "EJ"))
        //    {
        //        ejaritype = idtype;
        //        idtype = string.Empty;
        //        ejarinumber = idnumber;
        //        idnumber = string.Empty;
        //    }
        //    if (string.Equals("03", custtype) || string.Equals("04", custtype) || string.Equals("05", custtype) || string.Equals("06", custtype) || string.Equals("07", custtype))
        //    {
        //        State.Owner = true;
        //    }
        //    else
        //    {
        //        State.Owner = false;
        //    }
        //    string error;
        //    if (vatdocument != null && vatdocument.ContentLength > 0)
        //    {
        //        if (!AttachmentIsValid(vatdocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
        //        {
        //            ModelState.AddModelError(string.Empty, error);
        //        }
        //        else
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                vatdocument.InputStream.CopyTo(memoryStream);
        //                State.Attachment5 = memoryStream.ToArray();
        //                State.Attachment5Filename = vatdocument.FileName.GetFileNameWithoutPath();
        //                State.Attachment5FileType = AttachmentTypeCodes.VatDocument;
        //            }
        //        }
        //    }
        //    State.PremiseAccount = premiseaccount;
        //    State.IdType = idtype;
        //    State.CustomerType = custtype;
        //    State.IdNumber = idnumber;
        //    State.ExpiryDate = string.IsNullOrWhiteSpace(idexprirydate) ? (DateTime?)null : Convert.ToDateTime(idexprirydate);
        //    State.MoveinStartDate = Convert.ToDateTime(System.DateTime.Now);
        //    State.CustomerCategory = customerCategory;
        //    State.Ejari = ejarinumber;
        //    int resultroom;
        //    int.TryParse(numberofrooms, out resultroom);
        //    State.NumberOfRooms = resultroom;
        //    State.BusinessPartner = businesspartner;
        //    State.Vatnumber = vatnumber;
        //    var response = DewaApiClient.MoveInEjariRequest(
        //   State.UserId,
        //   State.createuseraccount,
        //   string.Empty,
        //   string.Empty,
        //   CurrentPrincipal.SessionToken ?? string.Empty,
        //   State.CustomerType,
        //   State.IdType,
        //   State.IdNumber,
        //   string.Empty,
        //   string.Empty,
        //   string.Empty,
        //   string.Empty,
        //   string.Empty,
        //   string.Empty,
        //   State.MoveinStartDate,
        //   null,
        //   string.Empty,
        //   null,
        //   null,
        //   State.BusinessPartner,
        //   State.ExpiryDate,
        //   string.Empty,
        //   State.CustomerCategory,
        //   null,
        //   string.Empty,
        //   this.IsLoggedIn ? "R" : "A",
        //   State.Ejari,
        //   State.PremiseAccount,
        //   string.Empty,
        //   State.Vatnumber,
        //   RequestLanguage,
        //   Request.Segment(),
        //   _fc.Branch
        //   );
        //    if (response.Succeeded)
        //    {
        //        if (this.IsLoggedIn)
        //        {
        //            State.UserId = CurrentPrincipal.UserId;
        //        }
        //        else
        //        {
        //            State.UserId = response.Payload.moveinEjari.userID;
        //        }
        //        //State.UserId = response.Payload.moveinEjari.userID;
        //        State.SecurityDeposit = response.Payload.moveinEjari.securityDepositAmount;
        //        State.InnovationFee = response.Payload.moveinEjari.innovationFees;
        //        State.KnowledgeFee = response.Payload.moveinEjari.knowledgeFees;
        //        State.ReconnectionRegistrationFee = (double.Parse(response.Payload.moveinEjari.reconnectionChargeAmount)).ToString();
        //        State.AddressRegistrationFee = (double.Parse(response.Payload.moveinEjari.addressChangeAmount)).ToString();
        //        State.ReconnectionVATamt = (double.Parse(response.Payload.moveinEjari.vatAmountRC)).ToString();
        //        State.AddressVAtamt = (double.Parse(response.Payload.moveinEjari.vatAmountAddressChange)).ToString();
        //        State.ReconnectionVATrate = response.Payload.moveinEjari.vatRateRC;
        //        State.AddressVATrate = response.Payload.moveinEjari.vatRateAddressChange;
        //        State.FirstName = response.Payload.moveinEjari.firstName;
        //        State.LastName = response.Payload.moveinEjari.lastName;
        //        State.MobilePhone = !string.IsNullOrWhiteSpace(response.Payload.moveinEjari.mobile) ? response.Payload.moveinEjari.mobile.First().Equals('0') ? response.Payload.moveinEjari.mobile.Remove(0, 1) : response.Payload.moveinEjari.mobile : string.Empty;
        //        State.EmailAddress = response.Payload.moveinEjari.email;
        //        State.Nationality = response.Payload.moveinEjari.nationality;
        //        State.PoBox = response.Payload.moveinEjari.address;
        //        State.BusinessPartner = response.Payload.moveinEjari.businessPartner;
        //        State.AttachmentFlag = ((string.Equals(idtype, "ED") && string.IsNullOrWhiteSpace(response.Payload.moveinEjari.firstName)) || string.Equals(idtype, "PN")) ? true : false;
        //        Save();
        //        CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));
        //        CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATH, new CacheItem<string>(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE));
        //        CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATHTEXT, new CacheItem<string>(Translate.Text("movein.landingpagebacktext")));
        //        if (State.CustomerCategory == "P")
        //        {
        //            if (this.IsLoggedIn)
        //            {
        //                if (string.Equals(ejaritype, "EJ"))
        //                {
        //                    if (!Request.AcceptTypes.Contains("application/json"))
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                    else
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false), JsonRequestBehavior.AllowGet);
        //                    // return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false));
        //                }
        //                else if (!State.Owner)
        //                {
        //                    if (!Request.AcceptTypes.Contains("application/json"))
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_TENANT_DETAILS, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                    else
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_TENANT_DETAILS, false), JsonRequestBehavior.AllowGet);
        //                    //return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_TENANT_DETAILS, false));
        //                }
        //                if (!Request.AcceptTypes.Contains("application/json"))
        //                    return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                else
        //                    return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false), JsonRequestBehavior.AllowGet);
        //                //return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false));
        //            }
        //            else
        //            {
        //                if (string.IsNullOrWhiteSpace(State.UserId))
        //                {
        //                    if (!Request.AcceptTypes.Contains("application/json"))
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNT, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                    else
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNT, false), JsonRequestBehavior.AllowGet);
        //                    //return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNT, false));
        //                }
        //                else if (string.Equals(ejaritype, "EJ"))
        //                {
        //                    if (!Request.AcceptTypes.Contains("application/json"))
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                    else
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false), JsonRequestBehavior.AllowGet);
        //                    //return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false));
        //                }
        //                else
        //                {
        //                    if (!Request.AcceptTypes.Contains("application/json"))
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                    else
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false), JsonRequestBehavior.AllowGet);
        //                    //return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false));
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (this.IsLoggedIn)
        //            {
        //                if (!string.Equals(State.CustomerType, "TO"))
        //                {
        //                    if (!Request.AcceptTypes.Contains("application/json"))
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                    else
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false), JsonRequestBehavior.AllowGet);
        //                    // return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false));
        //                }
        //                if (!Request.AcceptTypes.Contains("application/json"))
        //                    return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                else
        //                    return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false), JsonRequestBehavior.AllowGet);
        //                // return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false));
        //            }
        //            else
        //            {
        //                if (string.IsNullOrWhiteSpace(State.UserId))
        //                {
        //                    if (!Request.AcceptTypes.Contains("application/json"))
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNT, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                    else
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNT, false), JsonRequestBehavior.AllowGet);
        //                    // return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNT, false));
        //                }
        //                if (!string.Equals(State.CustomerType, "TO") && !string.Equals(ejaritype, "EJ"))
        //                {
        //                    if (!Request.AcceptTypes.Contains("application/json"))
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                    else
        //                        return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false), JsonRequestBehavior.AllowGet);
        //                    //return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS, false));
        //                }
        //                if (!Request.AcceptTypes.Contains("application/json"))
        //                    return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false), "text/plain", JsonRequestBehavior.AllowGet);
        //                else
        //                    return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false), JsonRequestBehavior.AllowGet);
        //                //return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE, false));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        CacheProvider.Store(CacheKeys.MOVEIN_ERROR_MESSAGE, new CacheItem<string>(response.Message));
        //        if (response.Payload != null && response.Payload.moveinEjari != null && response.Payload.moveinEjari.moveInIndicator == "000010")
        //        {
        //            CacheProvider.Store(CacheKeys.MOVEIN_INDICATOR, new CacheItem<bool>(true));
        //        }
        //        if (response.Payload.ResponseCode != 399)
        //        {
        //            if (!Request.AcceptTypes.Contains("application/json"))
        //                return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE, false), "text/plain", JsonRequestBehavior.AllowGet);
        //            else
        //                return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE, false), JsonRequestBehavior.AllowGet);
        //            // return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE);
        //        }
        //        if (!Request.AcceptTypes.Contains("application/json"))
        //            return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE, false), "text/plain", JsonRequestBehavior.AllowGet);
        //        else
        //            return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE, false), JsonRequestBehavior.AllowGet);
        //        // return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE, false));
        //    }
        //}

        //[HttpGet]
        //public ActionResult MoveInEjariPage()
        //{
        //    if (State.PremiseAccount == null || State.IdNumber == null)
        //    {
        //        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE);
        //    }

        //    EjariDetailsViewModel ejaridetails = new Models.MoveIn.EjariDetailsViewModel
        //    {
        //        BusinessPartner = State.BusinessPartner,
        //        UserName = State.FirstName,
        //        SecurityDepositAmount = double.Parse(State.SecurityDeposit)
        //    };

        //    return View("EjariDetails", ejaridetails);
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult MoveInEjariPage(EjariDetailsViewModel ejari)
        //{
        //    return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE);
        //}

        //[HttpGet]
        //public ActionResult MoveInContactPage()
        //{
        //    if (!InProgress())
        //    {
        //        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE);
        //    }
        //    var Nationalities = FormExtensions.GetNationalities(DropDownTermValues);
        //    ViewBag.Emirates = GetEmirates();
        //    ViewBag.Nationalities = Nationalities;
        //    ViewBag.NumberOfRoomsBag = GetNumberOfRooms();
        //    State.ContactPage = true;
        //    ContactDetailsViewModel contactDetails = new ContactDetailsViewModel
        //    {
        //        FirstName = State.FirstName,
        //        LastName = State.LastName,
        //        POBox = State.PoBox,
        //        Emirate = State.Emirate,
        //        MobilePhone = State.MobilePhone,
        //        Email = State.EmailAddress,
        //        Nationality = State.Nationality,
        //        AttachmentFlag = State.AttachmentFlag,
        //        Owner = State.Owner,
        //        CustomerType = State.CustomerCategory
        //    };
        //    if (Nationalities.Any(c => c.Value == State.Nationality))
        //    {
        //        contactDetails.NationalalityText = Nationalities.FirstOrDefault(c => c.Value == State.Nationality) != null ? Nationalities.First(c => c.Value == State.Nationality).Text : string.Empty;
        //    }
        //    if (State.AttachmentFlag)
        //    {
        //        contactDetails.PassportorEmiratesLabel = State.IdType == "ED" ? Translate.Text("movein.contact.emirates") : Translate.Text("movein.contact.passport");
        //    }

        //    return View("ContactDetails", contactDetails);
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult MoveInContactPage(ContactDetailsViewModel contact)
        //{
        //    //Added for Future Center
        //    var _fc = FetchFutureCenterValues();

        //    if (!InProgress())
        //    {
        //        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE);
        //    }
        //    State.FirstName = contact.FirstName;
        //    State.LastName = contact.LastName;
        //    State.MobilePhone = contact.MobilePhone;
        //    State.Emirate = contact.Emirate;
        //    State.EmailAddress = contact.Email;
        //    State.PoBox = contact.POBox;
        //    //State.NumberOfRooms = contact.NumberOfRooms;
        //    State.Nationality = contact.Nationality;
        //    string error = "";
        //    if (contact.IdentityDocument != null && contact.IdentityDocument.ContentLength > 0)
        //    {
        //        if (!AttachmentIsValid(contact.IdentityDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
        //        {
        //            ModelState.AddModelError(string.Empty, error);
        //        }
        //        else
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                contact.IdentityDocument.InputStream.CopyTo(memoryStream);
        //                State.Attachment1 = memoryStream.ToArray();
        //                State.Attachment1Filename = contact.IdentityDocument.FileName.GetFileNameWithoutPath();
        //                if (State.IdType == "PN")
        //                {
        //                    State.Attachment1FileType = AttachmentTypeCodes.Passport;
        //                }
        //                else
        //                {
        //                    State.Attachment1FileType = AttachmentTypeCodes.EmiratesIdDocument;
        //                }
        //            }
        //        }
        //    }
        //    if (contact.OwnerDocument != null && contact.OwnerDocument.ContentLength > 0)
        //    {
        //        if (!AttachmentIsValid(contact.OwnerDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
        //        {
        //            ModelState.AddModelError(string.Empty, error);
        //        }
        //        else
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                contact.OwnerDocument.InputStream.CopyTo(memoryStream);
        //                State.Attachment2 = memoryStream.ToArray();
        //                State.Attachment2Filename = contact.OwnerDocument.FileName.GetFileNameWithoutPath();
        //                State.Attachment2FileType = AttachmentTypeCodes.PurchaseAgreement;
        //            }
        //        }
        //    }
        //    if (contact.TitleDeed != null && contact.TitleDeed.ContentLength > 0)
        //    {
        //        if (!AttachmentIsValid(contact.TitleDeed, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
        //        {
        //            ModelState.AddModelError(string.Empty, error);
        //        }
        //        else
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                contact.TitleDeed.InputStream.CopyTo(memoryStream);
        //                State.Attachment3 = memoryStream.ToArray();
        //                State.Attachment3Filename = contact.TitleDeed.FileName.GetFileNameWithoutPath();
        //                State.Attachment3FileType = AttachmentTypeCodes.PurchaseAgreement;
        //            }
        //        }
        //    }
        //    if (contact.TradeLicense != null && contact.TradeLicense.ContentLength > 0)
        //    {
        //        if (!AttachmentIsValid(contact.TradeLicense, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
        //        {
        //            ModelState.AddModelError(string.Empty, error);
        //        }
        //        else
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                contact.TradeLicense.InputStream.CopyTo(memoryStream);
        //                State.Attachment4 = memoryStream.ToArray();
        //                State.Attachment4Filename = contact.TradeLicense.FileName.GetFileNameWithoutPath();
        //                State.Attachment4FileType = AttachmentTypeCodes.TradeLicense;
        //            }
        //        }
        //    }
        //    var response = DewaApiClient.MoveInEjariRequest(
        //       State.UserId,
        //       State.createuseraccount,
        //       string.Empty,
        //       string.Empty,
        //       CurrentPrincipal.SessionToken ?? string.Empty,
        //       State.CustomerType,
        //       State.IdType,
        //       State.IdNumber,
        //       State.FirstName,
        //      State.LastName,
        //      State.PoBox,
        //      State.Nationality,
        //      State.MobilePhone.AddMobileNumberZeroPrefix(),
        //      State.EmailAddress,
        //      State.MoveinStartDate,
        //       null,
        //   string.Empty,
        //       null,
        //       null,
        //       State.BusinessPartner,
        //       State.ExpiryDate,
        //       string.Empty,
        //       State.CustomerCategory,
        //       contact.NumberOfRooms,
        //       contact.Emirate,
        //       this.IsLoggedIn ? "R" : "A",
        //       State.Ejari,
        //       State.PremiseAccount,
        //       string.Empty,
        //       State.Vatnumber,
        //       RequestLanguage,
        //       Request.Segment(), _fc.Branch);

        //    if (response.Succeeded)
        //    {
        //        Save();
        //        CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));
        //        CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATH, new CacheItem<string>(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS));
        //        CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATHTEXT, new CacheItem<string>(Translate.Text("movein.contactpagebacktext")));
        //        if (!State.Owner)
        //        {
        //            return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_TENANT_DETAILS);
        //        }
        //        else
        //        {
        //            return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE);
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, response.Message);
        //        var Nationalities = FormExtensions.GetNationalities(DropDownTermValues);
        //        ViewBag.Emirates = GetEmirates();
        //        ViewBag.Nationalities = Nationalities;
        //        ViewBag.NumberOfRoomsBag = GetNumberOfRooms();
        //        State.ContactPage = true;
        //        ContactDetailsViewModel contactDetails = new ContactDetailsViewModel
        //        {
        //            FirstName = State.FirstName,
        //            LastName = State.LastName,
        //            POBox = State.PoBox,
        //            Emirate = State.Emirate,
        //            MobilePhone = State.MobilePhone,
        //            Email = State.EmailAddress,
        //            Nationality = State.Nationality,
        //            AttachmentFlag = State.AttachmentFlag,
        //            Owner = State.Owner,
        //            CustomerType = State.CustomerCategory
        //        };
        //        if (Nationalities.Any(c => c.Value == State.Nationality))
        //        {
        //            contactDetails.NationalalityText = Nationalities.FirstOrDefault(c => c.Value == State.Nationality) != null ? Nationalities.First(c => c.Value == State.Nationality).Text : string.Empty;
        //        }
        //        if (State.AttachmentFlag)
        //        {
        //            contactDetails.PassportorEmiratesLabel = State.IdType == "ED" ? Translate.Text("movein.contact.emirates") : Translate.Text("movein.contact.passport");
        //        }

        //        return View("ContactDetails", contactDetails);
        //    }
        //}

        //[HttpGet]
        //public ActionResult MoveInTenantPage()
        //{
        //    if (!InProgress())
        //    {
        //        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE);
        //    }

        //    ViewBag.NumberOfRoomsBag = GetNumberOfRooms();

        //    State.TenantPage = true;
        //    var model = new TenancyDetailsViewModel
        //    {
        //        ContractStartDate = State.ContractStartDate,
        //        ContractEndDate = State.ContractEndDate,
        //        NumberOfRooms = State.NumberOfRooms,
        //        ContractValue = State.ContractValue,
        //        CustomerType = State.CustomerType,
        //        SecurityDeposit = double.Parse(State.SecurityDeposit),
        //        ContractLabel1 = Translate.Text("Add Tenancy Contract")
        //    };
        //    return View("TenancyDetails", model);
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult MoveInTenantPage(TenancyDetailsViewModel model)
        //{
        //    //Added for Future Center
        //    var _fc = FetchFutureCenterValues();

        //    if (!InProgress())
        //    {
        //        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE);
        //    }

        //    if (!model.ContractStartDate.HasValue || !model.ContractEndDate.HasValue)
        //    {
        //        ModelState.AddModelError(string.Empty, Translate.Text("activation.invalid.expat-contract-period"));
        //    }
        //    else
        //    {
        //        var delta = model.ContractEndDate.Value.Subtract(model.ContractStartDate.Value);
        //        if (delta.TotalDays < 30 || delta.TotalDays > 365)
        //        {
        //            ModelState.AddModelError(string.Empty, Translate.Text("activation.invalid.expat-contract-period"));
        //        }
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        State.ContractStartDate = model.ContractStartDate;
        //        State.ContractEndDate = model.ContractEndDate;
        //        State.NumberOfRooms = model.NumberOfRooms;
        //        State.ContractValue = model.ContractValue;
        //        string error = "";
        //        if (model.UploadContract != null && model.UploadContract.ContentLength > 0)
        //        {
        //            if (!AttachmentIsValid(model.UploadContract, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
        //            {
        //                ModelState.AddModelError(string.Empty, error);
        //            }
        //            else
        //            {
        //                using (var memoryStream = new MemoryStream())
        //                {
        //                    model.UploadContract.InputStream.CopyTo(memoryStream);

        //                    State.Attachment2 = memoryStream.ToArray();
        //                    State.Attachment2Filename = model.UploadContract.FileName.GetFileNameWithoutPath();
        //                    State.Attachment2FileType = AttachmentTypeCodes.TenancyContract;
        //                }
        //            }
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            var response = DewaApiClient.MoveInEjariRequest(
        //      State.UserId,
        //      State.createuseraccount,
        //      string.Empty,
        //      string.Empty,
        //      CurrentPrincipal.SessionToken ?? string.Empty,
        //      State.CustomerType,
        //      State.IdType,
        //      State.IdNumber,
        //      State.FirstName,
        //      State.LastName,
        //      State.PoBox,
        //      State.Nationality,
        //      State.MobilePhone.AddMobileNumberZeroPrefix(),
        //      State.EmailAddress,
        //      State.MoveinStartDate,
        //       null,
        //   string.Empty,
        //      State.ContractStartDate,
        //      State.ContractEndDate,
        //      State.BusinessPartner,
        //      State.ExpiryDate,
        //      State.ContractValue,
        //      State.CustomerCategory,
        //      State.NumberOfRooms,
        //      State.Emirate,
        //      this.IsLoggedIn ? "R" : "A",
        //      State.Ejari,
        //      State.PremiseAccount,
        //      string.Empty,
        //      State.Vatnumber,
        //      RequestLanguage,
        //      Request.Segment(), _fc.Branch);

        //            if (!response.Succeeded)
        //            {
        //                ModelState.AddModelError(string.Empty, response.Message);
        //            }
        //            else
        //            {
        //                Save();
        //                CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));
        //                CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATH, new CacheItem<string>(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_TENANT_DETAILS));
        //                CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATHTEXT, new CacheItem<string>(Translate.Text("movein.tenantpagebacktext")));
        //                CacheProvider.Store(CacheKeys.MoveInCacheKey, new CacheItem<MoveInEjariResponse>(response.Payload));
        //                return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE);
        //            }
        //        }
        //    }

        //    ViewBag.NumberOfRoomsBag = GetNumberOfRooms();
        //    model.ContractLabel1 = Translate.Text("Add Tenancy Contract");
        //    //model.ContractLabel1 = Translate.Text("Add Purchase Agreement");
        //    // model.ContractLabel2 = Translate.Text("Add Purchase Agreement 2");
        //    model.ContractStartDate = State.ContractStartDate;
        //    model.ContractEndDate = State.ContractEndDate;
        //    model.NumberOfRooms = State.NumberOfRooms;
        //    model.ContractValue = State.ContractValue;
        //    model.CustomerType = State.CustomerType;
        //    model.SecurityDeposit = double.Parse(State.SecurityDeposit);
        //    return PartialView("TenancyDetails", model);
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult PaymentDetails(PaymentDetailsViewModel model)
        //{
        //    //Added for Future Center
        //    var _fc = FetchFutureCenterValues();

        //    //var amount = (decimal.Parse(State.SecurityDeposit ?? "0.00") + decimal.Parse(State.ReconnectionAddressRegistrationFee ?? "0.00")
        //    //             + decimal.Parse(State.KnowledgeFee ?? "0.00") + decimal.Parse(State.InnovationFee ?? "0.00"));
        //    CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));
        //    var response = DewaApiClient.MoveInEjariRequest(
        // State.UserId,
        // State.createuseraccount,
        // string.Empty,
        // string.Empty,
        // CurrentPrincipal.SessionToken ?? string.Empty,
        // State.CustomerType,
        // State.IdType,
        // State.IdNumber,
        // State.FirstName,
        // State.LastName,
        // State.PoBox,
        // State.Nationality,
        // State.MobilePhone.AddMobileNumberZeroPrefix(),
        // State.EmailAddress,
        // State.MoveinStartDate,
        //  null,
        //   string.Empty,
        // State.ContractStartDate,
        // State.ContractEndDate,
        // State.BusinessPartner,
        // State.ExpiryDate,
        // State.ContractValue,
        // State.CustomerCategory,
        // State.NumberOfRooms,
        // State.Emirate,
        // this.IsLoggedIn ? "R" : "A",
        // State.Ejari,
        // State.PremiseAccount,
        // "X",
        // State.Vatnumber,
        // RequestLanguage,
        // Request.Segment(), _fc.Branch);
        //    if (response.Succeeded)
        //    {
        //        CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATH, new CacheItem<string>(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE));
        //        CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATHTEXT, new CacheItem<string>(Translate.Text("movein.paymentpagebacktext")));
        //        if (State.Attachment1.Length > 0)
        //        {
        //            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.moveinEjari.premiseAmountList.Select(x => x.premiseTransacionID).ToArray(), State.Attachment1Filename, State.Attachment1FileType, State.Attachment1);
        //        }
        //        if (State.Attachment2.Length > 0)
        //        {
        //            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.moveinEjari.premiseAmountList.Select(x => x.premiseTransacionID).ToArray(), State.Attachment2Filename, State.Attachment2FileType, State.Attachment2);
        //        }
        //        if (State.Attachment3.Length > 0)
        //        {
        //            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.moveinEjari.premiseAmountList.Select(x => x.premiseTransacionID).ToArray(), State.Attachment3Filename, State.Attachment3FileType, State.Attachment3);
        //        }
        //        if (State.Attachment4.Length > 0)
        //        {
        //            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.moveinEjari.premiseAmountList.Select(x => x.premiseTransacionID).ToArray(), State.Attachment4Filename, State.Attachment4FileType, State.Attachment4);
        //        }
        //        if (State.Attachment5.Length > 0)
        //        {
        //            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, response.Payload.moveinEjari.premiseAmountList.Select(x => x.premiseTransacionID).ToArray(), State.Attachment5Filename, State.Attachment5FileType, State.Attachment5);
        //        }
        //        //return RedirectToAction("RedirectForServiceActivation", "Payment", new ServiceActivationPaymentRequestModel()
        //        //{
        //        //    Amounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => x.amount)),
        //        //    BusinessPartnerNumber = State.BusinessPartner ?? string.Empty,
        //        //    EmailAddress = State.EmailAddress,
        //        //    PremiseNumber = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => (!string.IsNullOrWhiteSpace(x.contractAccount) ? x.contractAccount : x.premiseNumber))),
        //        //    Username = State.UserId,
        //        //    MobileNumber = State.MobilePhone.AddMobileNumberZeroPrefix(),
        //        //    IsAnonymous = true
        //        //});

        //        #region [MIM Payment Implementation]

        //        var payRequest = new CipherPaymentModel();
        //        payRequest.PaymentData.amounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => x.amount));
        //        payRequest.PaymentData.contractaccounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => (!string.IsNullOrWhiteSpace(x.contractAccount) ? x.contractAccount : x.premiseNumber)));
        //        payRequest.PaymentData.businesspartner = State.BusinessPartner ?? string.Empty;
        //        payRequest.PaymentData.email = State.EmailAddress;
        //        payRequest.PaymentData.mobile = State.MobilePhone.AddMobileNumberZeroPrefix();
        //        payRequest.PaymentData.userid = State.UserId;
        //        payRequest.ServiceType = ServiceType.ServiceActivation;
        //        payRequest.IsThirdPartytransaction = true;

        //        var payResponse = ExecutePaymentGateway(payRequest);
        //        if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
        //        {
        //            return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
        //        }
        //        ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

        //        #endregion [MIM Payment Implementation]
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, response.Message);
        //    }
        //    string termsLink;

        //    if (State.Nationality != null && State.Nationality.ToLower().Equals("ae"))
        //    {
        //        termsLink = UtilExtensions.GetLinkUrl(Sitecorex.Database.GetItem(SitecoreItemIdentifiers.MOVEIN_CONSTANTS),
        //                    "Terms and Conditions Link National");
        //    }
        //    else
        //    {
        //        termsLink = UtilExtensions.GetLinkUrl(Sitecorex.Database.GetItem(SitecoreItemIdentifiers.MOVEIN_CONSTANTS),
        //                      "Terms and Conditions Link Expat");
        //    }
        //    return View("PaymentDetails", new PaymentDetailsViewModel()
        //    {
        //        SecurityDeposit = double.Parse(State.SecurityDeposit),
        //        ReconnectionRegistrationFee = double.Parse(State.ReconnectionRegistrationFee),
        //        AddressRegistrationFee = double.Parse(State.AddressRegistrationFee),
        //        ReconnectionVATrate = State.ReconnectionVATrate,
        //        ReconnectionVATamt = double.Parse(State.ReconnectionVATamt),
        //        AddressVATrate = State.AddressVATrate,
        //        AddressVAtamt = double.Parse(State.AddressVAtamt),
        //        KnowledgeFee = double.Parse(State.KnowledgeFee),
        //        InnovationFee = double.Parse(State.InnovationFee),
        //        TermsLink = termsLink,
        //        BusinessPartner = State.BusinessPartner,
        //        UserName = State.FirstName
        //    });
        //}

        //[HttpGet]
        //public ActionResult PaymentDetails()
        //{
        //    if (!InProgress())
        //    {
        //        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE);
        //    }
        //    string errorMessage;
        //    if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
        //    {
        //        ModelState.AddModelError(string.Empty, errorMessage);
        //    }
        //    string termsLink;

        //    if (State.Nationality != null && State.Nationality.ToLower().Equals("ae"))
        //    {
        //        termsLink = UtilExtensions.GetLinkUrl(Sitecorex.Database.GetItem(SitecoreItemIdentifiers.MOVEIN_CONSTANTS),
        //                    "Terms and Conditions Link National");
        //    }
        //    else
        //    {
        //        termsLink = UtilExtensions.GetLinkUrl(Sitecorex.Database.GetItem(SitecoreItemIdentifiers.MOVEIN_CONSTANTS),
        //                      "Terms and Conditions Link Expat");
        //    }
        //    State.PaymentPage = true;
        //    return View("PaymentDetails", new PaymentDetailsViewModel()
        //    {
        //        SecurityDeposit = double.Parse(State.SecurityDeposit),
        //        ReconnectionRegistrationFee = double.Parse(State.ReconnectionRegistrationFee),
        //        AddressRegistrationFee = double.Parse(State.AddressRegistrationFee),
        //        ReconnectionVATrate = State.ReconnectionVATrate,
        //        ReconnectionVATamt = double.Parse(State.ReconnectionVATamt),
        //        AddressVATrate = State.AddressVATrate,
        //        AddressVAtamt = double.Parse(State.AddressVAtamt),
        //        KnowledgeFee = double.Parse(State.KnowledgeFee),
        //        InnovationFee = double.Parse(State.InnovationFee),
        //        TermsLink = termsLink,
        //        BusinessPartner = State.BusinessPartner,
        //        UserName = State.FirstName
        //    });
        //}

        //[HttpGet]
        //public ActionResult GetCreateAccountForm()
        //{
        //    State.CreatePage = true;
        //    var model = new CreateOnlineAccountModel();
        //    model.Termslink = UtilExtensions.GetLinkUrl(Sitecorex.Database.GetItem(SitecoreItemIdentifiers.MOVEIN_CONSTANTS), "Terms and Condition Create Account");
        //    return View("CreateAccount", model);
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult GetCreateAccountForm(CreateOnlineAccountModel model)
        //{
        //    //Added for Future Center
        //    var _fc = FetchFutureCenterValues();

        //    if (IsLoggedIn)
        //    {
        //        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_LANDING_PAGE);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var availability = DewaApiClient.VerifyUserIdentifierAvailable(model.UserId, RequestLanguage, Request.Segment());
        //        if (availability.Succeeded && availability.Payload.IsAvailableForUse)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                State.BusinessPartner = model.BusinessPartnerNumber ?? string.Empty;
        //                State.UserId = model.UserId;
        //                State.Password = model.Password;
        //                State.ConfirmPassword = model.ConfirmationPassword;
        //                var response = DewaApiClient.MoveInEjariRequest(
        //                      State.UserId,
        //                      State.createuseraccount,
        //                       State.Password,
        //                      State.ConfirmPassword,
        //                      CurrentPrincipal.SessionToken ?? string.Empty,
        //                      State.CustomerType,
        //                      State.IdType,
        //                      State.IdNumber,
        //                      string.Empty,
        //                      string.Empty,
        //                      string.Empty,
        //                      string.Empty,
        //                      string.Empty,
        //                      string.Empty,
        //                      State.MoveinStartDate,
        //                       null,
        //                        string.Empty,
        //                      null,
        //                      null,
        //                      State.BusinessPartner,
        //                      State.ExpiryDate,
        //                      string.Empty,
        //                      State.CustomerCategory,
        //                      null,
        //                      string.Empty,
        //                      this.IsLoggedIn ? "R" : "A",
        //                      State.Ejari,
        //                      State.PremiseAccount,
        //                      string.Empty,
        //                      State.Vatnumber,
        //                      RequestLanguage,
        //                      Request.Segment(), _fc.Branch);
        //                if (response.Succeeded)
        //                {
        //                    Save();
        //                    CacheProvider.Store(CacheKeys.MOVEIN_JOURNEY, new CacheItem<string>("moveinjourney"));
        //                    CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATH, new CacheItem<string>(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CREATE_ACCOUNT));
        //                    CacheProvider.Store(CacheKeys.MOVEIN_PREVIOUSPATHTEXT, new CacheItem<string>(Translate.Text("movein.createpagebacktext")));
        //                    State.createuseraccount = "X";
        //                    if (!string.IsNullOrWhiteSpace(State.Ejari))
        //                    {
        //                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_PAYMENT_PAGE);
        //                    }
        //                    else
        //                    {
        //                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILS);
        //                    }
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError(string.Empty, response.Message);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, availability.Message);
        //        }
        //    }
        //    return PartialView("CreateAccount", model);
        //}

        public ActionResult M26PageTitleMovein()
        {
            var model = ContextRepository.GetCurrentItem<MoveInPageTitle>();
            if (!IsLoggedIn)
            {
                model.PageLink = SitecoreItemIdentifiers.PRE_LOGIN_MOVEIN_CONTACT_DETAILSv3;
                model.PageLinkText = Translate.Text("movein.contactpagebacktext");
            }
            else
            {
                model.PageLink = SitecoreItemIdentifiers.LOGIN_MOVEIN_CONTACT_DETAILSv3;
                model.PageLinkText = Translate.Text("movein.contactpagebacktext");
            }
            return View("~/Views/Feature/SupplyManagement/MoveIn/M26 Page Title Movein.cshtml", model);
        }

        private void ClearMoveInCache()
        {
            MoveInViewModelv3 state;
            if (CacheProvider.TryGet(CacheKeys.MOVE_IN_3_WORKFLOW_STATE, out state))
            {
                CacheProvider.Remove(CacheKeys.MOVE_IN_3_WORKFLOW_STATE);
            }
            CacheProvider.Remove(CacheKeys.MOVEIN_JOURNEY);
            CacheProvider.Remove(CacheKeys.MOVEIN_PASSEDMODEL);
            CacheProvider.Remove(CacheKeys.MOVEIN_PASSEDRESPONSE);
        }

        private void ClearMoveToCache()
        {
            MoveToWorkflowState state;
            if (CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out state))
            {
                CacheProvider.Remove(CacheKeys.MOVE_TO_WORKFLOW_STATE);
                CacheProvider.Remove(CacheKeys.MOVETOPAYMENT);
            }
        }

        private void ClearMoveOutCache()
        {
            string state;
            if (CacheProvider.TryGet(CacheKeys.MOVE_OUT_SELECTEDACCOUNTS, out state))
            {
                CacheProvider.Remove(CacheKeys.MOVE_OUT_SELECTEDACCOUNTS);
                CacheProvider.Remove(CacheKeys.MOVE_OUT_RESULT);
            }
            CacheProvider.Remove(CacheKeys.MOVEOUT_OTP_RESPONSE);
        }
    }
}