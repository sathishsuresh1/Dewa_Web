using DEWAXP.Foundation.Integration.Enums;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Foundation.Content
{
    public static class Roles
    {
        public const string User = "user";
        public const string Government = "government";
        public const string Consultant = "consultant";
        public const string Partner = "partner";
        public const string IdealHome = "IdealHome";
        public const string DRRG = "drrg";
        public const string DRRGEvaulator = "drrgevaluator";
        public const string DRRGSchemaManager = "drrgschemamanager";
        public const string DewaAcademy = "dewaacademy";
        public const string DewaSupplier = "dewasupplier"; // Long term and short term Pass
        public const string DewaSupplierAdmin = "dewasupplieradmin"; // Admin security , ePass Admin
        public const string DewaSupplierSecurity = "dewasuppliersecurity"; // Deprt Coordinator, Sr. Manager
        public const string DewaonedayInitiator = "dewaonedayinitiator"; // One day pass
        public const string CorporatePortal = "corporateportal";
        public const string Jobseeker = "jobseeker";
        public const string ScrapeSale = "scrapesale";
        public const string scholarship = "scholarship";
        public const string Miscellaneous = "miscellaneous";
    }

    //public static class CacheKeys
    //{
    //    public const string PROFILE_PHOTO = "ProfilePhoto";
    //    public const string HAS_PROFILE_PHOTO = "HasProfilePhoto";
    //    #region EasyPay
    //    public const string Easy_Pay_Number = "MyEasyPayNumber";
    //    public const string Easy_Pay_Response = "MyEasyPayResponse";
    //    public const string Easy_Pay_Estimate = "MyEasyPayEstimate";
    //    public const string Easy_Pay_LoggedIn = "MyEasyPayLoggedin";
    //    #endregion
    //}
    //public class SitecoreItemIdentifiers
    //{
    //    public const string Header_Footer_Config = "{8E1D110B-B071-42F0-94B6-60FC62978D22}";
    //    public const string PromotionalModalPopup = "{A5C95236-4E7C-4BB7-B4C1-8BCA517B02BB}";
    //    public const string HOME = "{765CD703-6BA9-4981-88BA-434FE00DA824}";
    //    public const string ABOUTDEWA = "{20425B1E-570C-4C56-B0F7-332373AAF7F0}";
    //    public const string LATESTNEWS = "{D4A4A0BB-5C23-4B19-AD15-E2FF9C86A9FF}";
    //    public const string NEWSANDMEDIA = "{AC8C089C-54CE-4647-9718-EBF1E18DFEDC}";
    //    public const string NEWS_LANDING_PAGE = "{D4A4A0BB-5C23-4B19-AD15-E2FF9C86A9FF}";
    //    public const string PUBLISHING_STATISTICS = "{E658366B-59C9-463A-9654-7F791CA1EE33}";
    //    public const string IMAGE_GALLERY = "{CC59B1D6-C97A-4716-B415-F57CB02ABD77}";
    //    public const string VIDEO_GALLERY = "{BD93AF49-23C1-40DD-87D7-B140D7C4C9F4}";
    //    public const string LATESTUPDATESPAGE = "{D20EA8C9-2E8A-4040-8166-775C928EF27A}";
    //    public const string YOUTUBE_VIDEOS_FOLDER = "{776471E5-9AC8-4154-9057-100D59CC3EBA}";
    //    public const string VIDEOPLAYLISTPAGE = "{85351418-6080-4057-975B-816918101AAA}";
    //    public const string T2ArticalPageTemplateID = "{A72A1FCC-CDB9-4949-842F-A8001075A7EA}";
    //    public const string RedirectItemTemplate = "{954B7389-A0CF-4512-BFB3-6D6DF1BF6003}";
    //}
    //public class SitecoreItemPaths
    //{
    //    public const string YOUTUBE_VIDEOS_FOLDER_EN = "/sitecore/content/Global References/Youtube Video/EN";
    //    public const string YOUTUBE_VIDEOS_FOLDER_AR = "/sitecore/content/Global References/Youtube Video/AR";
    //}

    public static class ScTemplate
    {
        public const string Persona_PageTemplateId = "{5A16A792-DFD8-45E2-A6A4-9401F6CF213C}";
        public const string DEWAHome_PageTemplateId = "{C99FAAE0-7E43-41FC-A796-00561F6B62AC}";
        public const string Common_FolderPgTmpId = "{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}";
        public const string Section_FolderPgTmpId = "{ED10F60E-5431-41FE-BB77-83DD330CA4CB}";
        public const string CareerPortalTemplate = "{BB8EE209-29D5-40AD-9ABD-9C0BE63BF0DA}";
    }

    public static class ScPageItemId
    {
        public const string DewaHomePage = "{765CD703-6BA9-4981-88BA-434FE00DA824}";

        public const string ConsumerLandingPage = "{3CFB99A4-D8A5-4446-A1DC-07E9568062B6}";
        public const string BuilderLandingPage = "{00682022-9FBF-497E-B676-915389E793A6}";
        public const string PartnerLandingPage = "{518FED4F-BEDB-4743-BBF7-DAD1FB16A2A2}";
        public const string SupplierLandingPage = "{3627ADDC-C077-4057-B14A-69855019791C}";
        public const string StudentLandingPage = "{89D53DE6-C1C1-4934-9579-ECE824397DE3}";
        public const string AboutusPage = "{84C465E5-4FE7-482A-84E7-EA31A9D059E7}";
        public const string CareerPage = "{D79C0670-9EE4-429C-B873-8ED5A7F69FA1}";

        public const string ConsumerProfilePage = "{0C5FB144-897A-4B82-92D7-B6D76B69A805}";
        public const string CustomerProfilePage = "{DC919FDC-708B-4B87-AB29-2E0A0FB83101}";
        public const string ChangePassword = "{A269B2F5-D868-4EF5-A6C9-76C4E3398EB6}";

        public const string SCRAPESALE_INTROPAGE = "{42474A40-44ED-439A-AEF4-5594A3C43159}";
        public const string SCRAPESALE_PORTAL_LOGINPAGE = "{2675D1E4-E221-470B-A5D2-3AFF6C1FAA24}";
        public const string SCRAPESALE_PORTAL_DASHBOARD = "{76B78E62-3DFE-45BA-993C-2D679086EBF8}";
        public const string CAREER_POTAL_LOGIN = "{85CBFF97-CF17-477F-A172-FB205830BD64}";
        public const string CAREER_PORTAL_DASHBOARD = "{1DB1B717-5F09-4EE5-896F-4B562E7E262C}";

        public const string J7_LOGIN_PAGE = "{879F48A3-0C26-4C2C-AFB6-9C8FDDEA4EAB}";
        public const string SCHOLARSHIP_SIGNIN_PAGE = "{AEA8B835-6816-4DA6-ACD3-E9D33CBDBB62}";
        public const string EPASS_LOGIN = "{1CD256A2-AFC8-405C-BD65-A91B0250138F}";
        public const string EPASS_DASHBOARD = "{B329204C-AF30-47BD-B387-FFC3510F47B6}";
        public const string EPASS_SERVICES = "{47C5B959-BD55-4EEE-AC39-BB0FA80D4C41}";
        public const string EPASS_MYPASSESS = "{14EA5DCC-E571-4144-9930-30FF8D419A24}";
        public const string EPASS_ADMINLOGIN = "{5C66CD94-4FBA-4669-BBBE-36A544726E66}";
        public const string EPASS_ADMINDASHBOARD = "{26382987-A329-4CB7-8DE0-FDC81A51DAAD}";
        public const string EPASS_ALLPASSES = "{B2227C0E-480E-4755-A415-E573F942214C}";
        public const string EPASS_ADMINALLPASSESS = "{C5FB5052-5D77-4576-925F-36C23EAD0939}";
        public const string EPASS_BLOCKEDUSERS = "{E9708292-2A8C-407E-9F0D-E3763F14C5E0}";
        public const string J69_CUSTOMER_DASHBOARD = "{DBDD9E2E-9712-4135-8CF4-8F66B0095D74}";
        public const string IDEALHOMECONSUMER_LANDINGPAGE = "{7216D1BA-7EAF-4409-8AC3-D8A491CB33B8}";
        public const string IDEALHOMECONSUMER_LOGIN = "{D446DB8D-96BA-49AF-9CD9-AA9A7AE09DCE}";
        public const string IDEALHOMECONSUMER_DASHBOARD = "{96A56FFD-B982-40FC-A0C3-228BBC7A4C53}";
        public const string J7_MY_ID_LOGIN = "{48039B89-15C2-4097-A1BA-02CE3C88BEB0}";
        public const string J7_UAE_ID_LOGIN = "{86A8A341-5174-45BF-932E-AB08B1EC2864}";
        public const string J8_GOVT_LOGIN_PAGE = "{D9F875E4-62A3-4480-B80C-5122778CFA13}";
        public const string J7_LOGOUT = "{F0023F89-0269-4FDA-9CD4-F39C0A9D3C7E}";
        public const string J91_GOVT_OBS_REQUEST = "{BB354A72-1516-4BD1-8819-46A1DDCAF68C}";
        public const string J86_DOCUMENTUPLOADPAGE = "{BE7AD25E-AFE6-474D-8D25-A6CFD2641E63}";
        public const string J86_CHANGEPASSWORD_PAGE = "{5CE8A702-B1BC-4E9A-A3B5-829185AA46E3}";
        public const string J86_LOGOUT_PAGE = "{77E50C32-8FBF-4E81-BDFD-994F5868629A}";
        public const string J87_DOCUMENTUPLOADPAGE = "{D2F6B6B0-16C7-4E97-9898-726A0A8EF35B}";
        public const string J87_INBOXPAGE = "{AC6443B2-9E10-43D0-BCDB-02D6D40D722E}";
        public const string J87_CHANGEPASSWORD_PAGE = "{E1DF37F7-7C03-4F24-816F-C03388715F9F}";
        public const string J87_LOGOUT_PAGE = "{14702845-FCDB-49D3-8ECE-39E780E25D27}";
        public const string NotificationId = "{DC02062E-B3E2-446A-8CA4-FE009DAC6F68}";
        public const string RammasConfigID = "{26DCE97E-5B85-4145-BB03-7997F2430F9A}";
        public const string BottomNotificationConfigId = "{D5901F67-63B9-44AD-BD12-52FB0E5DC591}";
        public const string WebsiteSurveyConfigId = "{AC03C708-AB21-4026-9E20-45DE6EF59096}";
        public const string ACCESSIBILITY_OPTIONS = "{0BDC45E9-A004-45C4-99C1-8F9AC5952080}";
    }

    //public class DataSources
    //{
    //    public const string Masthead2v1 = "{E95A21A6-7C9D-4284-BDE8-6690FD6B47DC}";
    //    public const string RammasDatasource = "{A5FB0DB5-1AEE-42D8-8C08-42B761F66C44}";
    //}
    //public static class SitecoreRenderingIds
    //{
    //    public const string BREADCRUMB = "{CDA5C872-F7A7-497E-86EF-9AD0B579E7D3}";

    //    public const string HEROBREADCRUMB = "{73D36177-C53C-4909-AA8C-D7EC216C275A}";

    //    public const string M74Expander = "{C7421E72-093D-45CE-A7CB-180A20CF9F0B}";

    //    public const string M14FormattedText = "{C3EC4780-2E5B-42F2-8C94-C9F7BF830D6D}";
    //}
    public static class CacheKeys
    {
        public const string MOVE_IN_ACCOUNT_DETAILS = "MoveInAccountDetails";
        //public const string TEMP_CONN_REQ = "TempConnRequestObject";

        public const string ENERGY_AUDIT_BUILDING_INFO = "EnergyAuditBuildingInfo";

        public const string FIX_LOGINMODEL = "FixLoginModel";

        public const string ENERGY_AUDIT_CONFIRMATION_CODE = "EnergyAuditConfirmCode";

        public const string PROFILE_PHOTO = "ProfilePhoto";

        public const string CHANGE_LANDLORD_DETAILS_WORKFLOW_STATE = "LandlordDetailsSessionStore";

        public const string SET_PRIMARY_ACCOUNT_WORKFLOW_STATE = "SetPrimaryAccountModel";

        public const string TEMP_CONN_REQ_REF = "TempConnRequestRef";

        public const string TEMP_CON_REQ_FAILED = "TempConnRequestFailed";

        public const string ACCOUNT_LIST = "Accounts";

        public const string ACCOUNT_LIST_WITH_BILLING = "AccountsBilling";

        public const string FRIENDS_BILL = "FriendsBill";

        public const string FRIENDS_BILL_SEARCH_CRITERIA = "FriendsBillSearchCriteria";

        public const string FRIENDS_BILL_MODEL = "ViewFriendsBill";

        public const string SELECTED_TRANSACTION = "TransactionSelection";

        public const string SELECTED_TRANSACTIONACCOUNT = "TransactionAccount";

        public const string SELECTED_REFUND = "RefundSelection";

        public const string SELECTED_REFUNDACCOUNT = "RefundAccount";

        public const string SELECTED_REFUNDACCOUNT_LANGUAGE = "RefundAccountLanguage";

        public const string STATEMENT_DOWNLOAD = "stmtdownload";

        public const string ReceiptAccount = "Receiptaccount";

        public const string SELECTED_BILL_LIST = "BillSelection";

        public const string SELECTED_BILLS_TOTAL = "BillSelectionTotal";

        public const string MOVETOPAYMENT = "Movetopayment";

        public const string ManagePreferenceConfirmation = "ManagePreferenceConfirmation";
        public const string ManagePreferenceConfirmationtitle = "ManagePreferenceConfirmationtitle";
        public const string ManagePreference = "ManagePreference";

        public const string UnsubscribeConfirmation = "UnsubscribeConfirmation";

        public const string LandlordSupplyActivationSuccess = "LandlordSupplyActivationSuccess";

        #region SmartWallet Constant
        public const string SMARTWALLET_SELECTED_LIST = "SmartwalletSelection";

        public const string SMARTWALLET_SELECTED_TOTAL = "SmartwalletSelectionTotal";
        #endregion SmartWallet Constant

        #region Move in
        public const string MOVEIN_EJARI_NUMBER = "MoveinEjariNumber";

        public const string MOVEIN_PREMISELIST = "MoveinPremiseList";

        public const string MOVEIN_lST_BUSINESSPARTNER = "MoveInBusinesspartner";

        public const string MOVEIN_JOURNEY = "MoveInJourney";

        public const string MOVEIN_PREVIOUSPATH = "MoveInPreviousPath";

        public const string MOVEIN_PREVIOUSPATHTEXT = "MoveInPreviousPathtext";

        public const string MOVEIN_ERROR_MESSAGE = "MoveInErrorMessage";

        public const string MOVEIN_USERID = "MoveInuserid";

        public const string MOVEIN_PROCESSED = "MoveInprocessed";

        public const string MOVEIN_RERA_PROCESSED = "MoveInReraprocessed";

        public const string MOVEIN_INDICATOR = "MoveInIndicator";

        public const string MOVEIN_PASSEDMODEL = "MoveInpassedmodel";

        public const string MOVEIN_PASSEDRESPONSE = "MoveInpassedresponse";

        public const string MOVEIN_RERA_MIM_MODEL = "MOVEIN_RERA_MIM_MODEL";
        public const string MOVEIN_MIM_MODEL = "MOVEIN_MIM_MODEL";

        #endregion Move in
        public const string UPDATEIBAN = "UpdateIBAN";
        #region

        #endregion

        public const string COMPLAINT_SENT = "ComplaintSent";

        public const string COMPLAINT_FAILED = "ComplaintFailed";

        public const string PAYMENT_METADATA = "PaymentMetaData";

        public const string PAYMENT_PATH = "PaymentPath";

        public const string MOVEOUT_PAYMENT_PATH = "MoveoutPaymentPath";

        public const string MOVEOUT_PAYMENT_ANONYMOUS_PATH = "MoveoutAnonymousPaymentPath";

        public const string TAYSEER_PAYMENT_PATH = "TayseerGenerateReferencePath";

        public const string TAYSEER_HISTORY_PAYMENT_PATH = "TayseerHistoryPath";

        public const string MOVEOUT_ANONYMOUS_PAYMENT_MODEL = "MoveoutAnoymousPaymentModel";

        public const string CLEARANCE_PAYMENT_PATH = "ClearancePaymentPath";

        public const string CLEARANCE_CERTIFICATE_FinalFlow = "ClearanceCertificateFinalFlow";

        public const string CLEARANCE_CERTIFICATE_AccountList = "CL";

        public const string CLEARANCE_PAYMENT_MODEL = "ClearancePaymentModel";

        public const string CLEARANCE_PAYMENT_Details_Propertyseller = "ClearancePaymentModelDetails";

        public const string ACCOUNT_PATH = "AccountPath";

        public const string CLEARANCE_PAYMENT_DETAILS = "ClearancePaymentDetails";

        public const string CLEARANCE_SENT = "ClearanceSent";

        public const string CLEARANCE_FAILED = "ClearanceFailed";

        public const string GOV_OBS_SENT = "GovernmentObservationSent";

        public const string GOV_OBS_FAILED = "GovernmentObservationFailed";

        public const string SET_PREMISE_TYPE_SENT = "SetPremiseSent";

        public const string SET_PREMISE_TYPE_FAILED = "SetPremiseFailed";

        public const string REGISTRATION_WORKFLOW_STATE = "RegistrationSessionStore";

        public const string UPDATE_CONTACT_INFO_STATE = "UpdateContactInfoModel";

        public const string MY_ID_CLAIMS = "MyIDClaims";

        public const string UAEPASSINFO = "UAEPASSINFO";

        public const string ERROR_MESSAGE = "NextError";

        public const string ACCOUNTLOCK_ERROR_MESSAGE = "AccountlockError";

        public const string ERROR_MODEL = "NextErrorModel";

        public const string ADD_TO_COLLECTIVE_BILLING_STATE = "AddToCollectiveBillingState";

        public const string MY_ESTIMATES = "MyEstimates";

        public const string MY_ESTIMATE_STATE = "SelectedEstimate";
        public const string MY_ESTIMATE_STATE_Payment = "SelectedEstimatepayment";

        public const string MY_ESTIMATE_PAYMENT_STATE = "EstimatePaymentState";

        public const string FRIENDS_ESTIMATE_STATE = "FriendsEstimate";

        public const string FRIENDS_ESTIMATE_PAYMENT_STATE = "FriendsEstimatePaymentState";

        public const string MOVE_OUT_WORKFLOW_STATE = "MoveOutWorkflowState";
        public const string MOVE_OUT_RESULT = "MoveOutWorkresult";
        public const string MOVE_OUT_DETAILS = "Moveoutdetails";
        public const string MOVE_OUT_SELECTEDACCOUNTS = "MoveOutSelectedaccounts";
        public const string MOVE_OUT_HOLIDAYS = "MoveoutHolidays";
        public const string MOVE_OUT_MOB_EML_LST = "MOveOutMobEmlLst";
        public const string MOVE_OUT_CONFIRM = "MoveOutConfirm";
        public const string MOVE_OUT_DEMOLISH_CONFIRM = "MoveOutDemolishConfirm";
        public const string MOVE_OUT_DEMOLISH_GOV_CONFIRM = "MoveOutDemolishConfirmGov";

        public const string MOVE_OUT_CONFIRM_ACCOUNTS = "MoveOutConfirmAccounts";

        public const string MOVE_TO_WORKFLOW_STATE = "MoveToWorkflowState";

        public const string MOVE_TO_SELECTEDACCOUNT = "movetoaccountselected";

        public const string MOVE_IN_WORKFLOW_STATE = "MoveInAccount";

        public const string TERMS = "DD_TERMS";

        public const string VACANCIES_LIST = "VacancyList";

        public const string CHANGE_CUSTOMER_CATEGORY_STATE = "ChangeCustomerCategoryState";

        public const string REQUEST_COLLECTIVE_ACCOUNT_STATE = "RequestCollectiveAccountSessionStore";

        public const string RECOVERY_EMAIL_STATE = "EMAIL_RECOVERY_STATE";

        public const string REGISTRATION_SUCCESSFUL_STATE = "RegistrationSuccessful";

        public const string PROJECTGENERATION_USERDATA = "ProjectGenerationUserData";
        public const string PROJECTGENERATION_SUCCESSDATA = "ProjectGenerationSuccessData";
        public const string PaymentCacheKey = "ReraPaymentSessionStore";
        public const string PaymentReraCacheKey = "PaymentReraCacheKey";
        public const string MoveInCacheKey = "MoveInSessionStore";
        public const string BpDetailsCacheKey = "BpDetailsSessionStore";
        public const string MY_BILL = "MyBill";

        public const string MY_BILL_SEARCH_CRITERIA = "MyBillSearchCriteria";

        #region [UAEPGS]
        public const string UAEPGS_PaymentPending = "UAEPGS_PaymentPending";
        public const string UAEPGS_PaymentAmount = "UAEPGS_PaymentAmount"; 
        #endregion
        #region EasyPay
        public const string Easy_Pay_Number = "MyEasyPayNumber";
        public const string Easy_Pay_Response = "MyEasyPayResponse";
        public const string Easy_Pay_Estimate = "MyEasyPayEstimate";
        public const string Easy_Pay_LoggedIn = "MyEasyPayLoggedin";
        public const string Tayseer_EasyPay_Transaction_Type = "TayseerEasyPayTransactionType";
        #endregion

        #region ManageBeneficiary
        public const string ManageBeneficiary_Response = "ManageBeneficiaryResponse";
        public const string ManageBeneficiary_Error = "ManageBeneficiaryError";
        public const string ManageBeneficiary_Success = "ManageBeneficiarySuccess";

        #endregion

        #region WesternUnion
        public const string Western_Union_Response = "MyWesternUnionResponse";
        #endregion

        public const string ANONYMOUS_CLEARANCE_DETAILS = "AnonymousClearanceDetails";
        #region MoveoutAnonymous

        public const string MOVEOUT_OTP_RESPONSE = "SetMoveOutwithotpResponse";

        #endregion

        public const string AUTHENTICATED_CLEARANCE_DETAILS = "AuthenticatedClearanceDetails";
        public const string PropertySeller_CLEARANCE_DETAILS = "PropertySellerClearanceDetails";
        public const string SERVICECOMPLAINT_DETAILS = "ServiceComplaintDetails";
        public const string BILLINGCOMPLAINT_DETAILS = "BillingComplaintDetails";
        public const string HAS_PROFILE_PHOTO = "HasProfilePhoto";
        public const string PARTNERSHIP_USERDATA = "ParternshipUserData";
        public const string PARTNERSHIP_SUCCESSDATA = "ParternshipSuccessData";
        public const string PARTNERSHIP_USERDATAAR = "ParternshipUserDataAr";
        public const string SET_CONNECTION_ENQUIRY_SET = "SetConnectionEnquirySet";
        public const string SET_CONNECTION_ENQUIRY_SENT = "SetConnectionEnquirySent";
        public const string SET_CONNECTION_ENQUIRY_FAILED = "SetConnectionEnquiryFailed";
        public const string PAYMENT_ACCOUNTS_METADATA = "PaymentAccountList";
        public const string HAPPINESS_SURVEY = "HappinessSurvey";
        public const string HAPPINESS_SURVEY_ANSWERS = "HappinessSurveyAnswers";
        public const string COLLECTIVE_STATEMENT_CACHE = "CollectiveStatementStore";
        public const string BEST_CONSUMER_AWARD_CACHE = "BestConsumerAward";
        public const string SOLAR_CALCULATOR_SELECTED_ACCOUNT = "SolarCalcSelAccount";

        public const string IDEAL_HOME_USER_DATA = "IdealHomeUserData";
        public const string IDEAL_HOME_USER_MODEL = "IdealHomeUserModel";
        public const string IDEAL_HOME_SECTION_DETAILS = "IdealHomeSectionDetails";
        public const string IDEAL_HOME_SURVEY_DETAILS = "IdealHomeSurveyDetails";
        public const string IDEAL_HOME_VIDEO_DETAILS = "IdealHomeVideoDetails";

        #region V1
        public const string IDEAL_HOME_VIDEO_DETAILS_V1 = "IdealHomeVideoGroupDetails";
        #endregion
        #region DRRG
        public const string DRRG_MODULELIST = "drrgmodulelist";
        public const string DRRG_RESUBMISSIONAPPLICATIONLIST = "drrgresubmissionapplicationlist";
        public const string DRRG__EVALUATOR_MODULELIST = "drrgevaluatormodulelist";
        public const string DRRG__SCHEMAMGR_MODULELIST = "drrgevaluatormodulelist";
        public const string DRRG__BACKEND_LIST = "drrgebackendlist";
        public const string DRRG__ELIGIBLEEQUIPMENT_LIST = "drrgeeligibleequipmentlist";
        public const string DRRG__APPLICATIONLOG_LIST = "drrgapplicationloglist";
        public const string DRRG_SUBMITTED_MANUFACTURERLIST = "drrgsubmittedmanufacturerlist";
        public const string DRRG_MODULES_BYMANUCODE = "drrgmodulesbymanucode";
        public const string DRRG_UPDATED_MANUFACTURERLIST = "drrgupdatedmanufacturerlist";
        public const string DRRG__EVALUATOR_LIST = "drrgevaluatorlist";

        #endregion

        public const string RAMMAS_LOGIN = "RammasLogin";
        public const string REF_RAMMAS = "rammas";
        public const string RAMMAS_PAYMENT_CONFIRMED = "rammas_payment_done";
        public const string RAMMAS_CONVERSATION_ID = "rammas_conversation_id";
        public const string RAMMAS_DIRECTLINE_LOGIN = "rammas_directline_login";
        public const string RAMMAS_TRANSACTION = "rammas_transaction";
        public const string RAMMAS_TRANSACTION_SUCCESS = "rammas_transaction_success";

        public const string EV_RegistrationDetails = "EV_RegistrationDetails";
        public const string EV_RegistrationSuccess = "EV_RegistrationSuccess";
        public const string EV_ReplacePayment = "EV_ReplacePayment";
        public const string EV_SmartChargingPayment = "EV_SmartChargingPayment";
        public const string EV_SmartChargingSuqiaAmount = "EV_SmartChargingSuqiaAmount";
        public const string EV_SmartChargingconfirmationPayment = "EV_SmartChargingconfirmationPayment";
        public const string EV_Personaldetails = "EV_Personaldetails";
        public const string EV_CustomerTypePayment = "EV_CustomerTypePayment";
        public const string EV_CustomerTypePaymentRequestNumber = "EV_CustomerTypePaymentRequestNumber";
        public const string EV_CustomerTypePaymentdetails = "EV_CustomerTypePaymentdetails";
        public const string EV_ReplacePayment_AccountNumber = "EV_ReplacePayment_AccountNumber";
        public const string RegistrationReturnURL = "RegistrationReturnURL";
        public const string MY_ID_returnUrl = "MyIDreturnUrl";
        public const string EV_ENQUIRY_SENT_SET = "SetEVEnquirySentSet";
        public const string EV_SDPayment = "EV_SDPayment";
        public const string EV_SDPaymentParam = "EV_SDPaymentParam";

        // EV card Individual, Business and Government.
        public const string EV_CustomerLoginDetails = "EV_CustomerLoginDetails";

        public const string UAEPASS_returnUrl = "UAEPASSreturnUrl";
        public const string UAEPASS_USC_returnUrl = "UAEPASS_USC_returnUrl";

        public const string Support_kiosk = "Support_kiosk";
        #region DEWA ACADEMY
        public const string Academy_Success = "Academy_Success";
        #endregion
        #region MOVEIN v3
        public const string MOVE_IN_3_WORKFLOW_STATE = "MoveInAccountv3";
        #endregion

        #region Anonymous Bill Download
        public const string anonymous_bill_download = "anonymous_bill_download";
        #endregion

        #region Miscellaneous Project
        public const string Miscellaneous_Success = "MiscellaneousSuccess";
        public const string Miscellaneous_MeterTestingProjects = "MISCMeterTestingProjects";
        public const string Miscellaneous_MeterTestingNewconnection = "MISCMeterTestingNewconnection";
        public const string Miscellaneous_OilTesting = "MISCOilTesting";
        public const string Miscellaneous_DemineralizedWater = "MISCDemineralizedWater";
        public const string Miscellaneous_JointerTesting = "MISCJointerTesting";
        #endregion

        #region ePass

        public const string EPASS_USER_MODEL = "EpassHomeUserModel";
        public const string EPASS_MULTIPASS_LIST = "Epassmultipasslist";
        public const string EPASS_MULTIPASS_SHORTTERM_LIST = "Epassmultipassshorttermlist";
        public const string EPASS_MULTIPASS_REQUEST = "Epassmultipassreq";
        public const string EPASS_MULTIPASS_SHORTTERM_REQUEST = "Epassmultipassshorttermreq";
        public const string EPASS_MYPASS_LIST = "Epassmypasslist";
        public const string EPASS_ALLPASS_LIST = "Epassallpasslist";
        public const string EPASS_BLOCKEDUSERS = "Epassblockedusers";
        public const string EPASS_TRACKPASS = "EpassTrackpass";
        public const string EPASS_TRACKPASSKEYWORD = "EpassTrackpassKeyword";

        public const string EPASS_PENDINGAPPROVALPASS_LIST = "Epasspendingapprovalpasslist";

        public const string EPASS_CREATEACCOUNT_SUCCESS = "Epasscreateaccountsuccess";
        public const string EPASS_CREATEACCOUNT_USERREGISTER = "Epasscreateaccountuserregister";
        public const string EPASS_USERREGISTER_SUCCESS = "Epassuserregistersuccess";
        public const string EPASS_ERROR_MESSAGE = "EpassErrorMessage";
        public const string EPASS_CHANGEPASSWORD_MESSAGE = "EpasschangeMessage";
        public const string EPASS_RECENTPASS_LIST = "EpassRecentPassList";
        public const string EPASS_DETAILS = "EpassDetails";
        public const string EPASS_RENEWAL_DETAILS = "EpassRenewalDetails";
        public const string EPASS_PROFILEUPDATE_MESSAGE = "EpassprofileupdateMessage";
        public const string EPASS_PROFILEUPDATEERROR_MESSAGE = "EpassprofileupdateerrorMessage";
        public const string EPASS_CHANGEPASSWORDERROR_MESSAGE = "EpasschangepassworErrorMessage";
        public const string EPASS_PROJECT_LIST = "EpassProjectList";
        public const string EPASS_WORK_LOGS = "EpassWorkLogs";
        public const string EPASS_BLOCKED_USERS = "EpassBlockedUsers";
        public const string EPASS_ONEDAYPASS = "EpassOnedaypass";
        public const string EPASS_LOCMAIN = "EpassLocmain";
        public const string WORKPERMIT_PASSREQUEST = "WORKPERMIT_PASSREQUEST";
        #endregion
        #region Work Permit
        public const string WORK_PERMIT_PROJECT_LIST = "WPProjectList";
        public const string WORK_PERMIT_SUBCONTRACTOR_LIST = "WPSubcontractorlist";
        public const string WORK_PERMIT_COUNTRYLIST = "WPCountryList";
        public const string WORK_PERMIT_MULTIPASS_LIST = "WPmultipasslist";
        public const string WORK_PERMIT_MULTIPASS_REQUEST = "WPmultipassreq";
        #endregion

        #region [Estimation]
        public const string SELECTED_ESTIMATIONTRANSACTIONACCOUNT = "ESTIMATIONTRANSACTIONACCOUNT";
        public const string ESTIMATIONT_DATALIST = "ESTIMATIONT_DATALIST";
        #endregion

        #region Scholarship
        public const string SCHOLARSHIP_LOGIN_MODEL = "ScholarshipLoginModel";
        public const string SCHOLARSHIP_PASSWORD_RECOVERY_STATE = "SCHOLARSHIP_PASSWORD_RECOVERY_STATE";
        public const string SCHOLARSHIP_HELP_VALUES_ENGLISH_CACHE_KEY = "SCHOLARSHIP_HELP_VALUES_ENGLISH";
        public const string SCHOLARSHIP_HELP_VALUES_ARABIC_CACHE_KEY = "SCHOLARSHIP_COUNTRIES";

        //public const string SCHOLARSHIP_SOURCE_OF_APPLICATICATION_CACHE_KEY = "SCHOLARSHIP_SOURCES";
        public const string SCHOLARSHIP_PROFILE_UPDATE_STATE_CACHE_KEY = "SCHOLARSHIP_PROFILE_UPDATE_CACHE";

        #endregion

        #region Corporate Portal
        public const string Corporate_Portal_LOGIN_MODEL = "CorporatePortalLoginModel";
        public const string Corporate_Portal_USER_DETAILS = "CorporatePortalUserDetails";
        public const string Corporate_Portal_JointerServices = "CorporatePortalJointerServices";
        public const string Corporate_Portal_Projectandinitiatives = "CorporatePortalProjectandinitiatives";
        public const string Corporate_Portal_MANAGEUSERS = "CorporatePortalManageusers";
        public const string Corporate_Portal_INBOX = "CorporatePortalInbox";
        #endregion

        #region [Smart response]
        public const string SM_CITIES = "SM_CITIES";
        public const string SM_SC_TRANSLATION_ITEMS = "SM_SC_TRANSLATION_ITEMS";
        public const string SM_AI_IMG_PREDICT = "SM_AI_IMG_PREDICT";
        #endregion

        #region [Smart Communication]
        public const string USC_VERIFYOTP = "USC_VERIFYOTP";
        public const string USC_CONSUMPT_VERIFYOTP = "USC_CONSUMPT_VERIFYOTP";
        #endregion

        #region [DTMC Consumption]
        public const string CC_BILLING_MONTH = "CC_BILLING_MONTH";
        public const string FILTER_AMI_STEP = "FILTER_AMI_STEP";
        public const string GETWATERAI_RESPONSE = "GETWATERAI_RESPONSE";
        public const string CONSUMPTION_INSIGHT_DATAMODEL = "CONSUMPTION_INSIGHT_DATAMODEL";

        #endregion

        #region [Revamp 2019]
        public const string PROFILE_CHANGE_PWD = "PROFILE_CHANGE_PWD";
        public const string PROFILE_CHANGE_PWD_ERROR = "PROFILE_CHANGE_PWD_ERROR";
        public const string PROFILE_CONTACT_ERROR = "PROFILE_CONTACT_ERROR";
        public const string PROFILE_CONTACT_SUCCESS = "PROFILE_CONTACT_SUCCESSs";
        #endregion

        #region [Move-to]
        public const string MOVETO_PASSEDRESPONSE_PAYLOAD = "MoveTOpassedresponsepayload";
        public const string MOVETO_PASSEDMODEL = "MoveTOpassedmodel";
        public const string MOVETO_PASSEDRESPONSE = "MoveTOpassedresponse";
        #endregion

        #region EV V2
        public const string EV_DEACTIVATE_RESULT = "evdeactivateresult";
        public const string EVDEACTIVATE_PAYMENT_PATH = "evdeactivatePaymentPath";
        public const string EV_DEACTIVATE_DETAILS = "evdeactivatedetails";
        public const string EV_DEACTIVATE_CONFIRM_ACCOUNTS = "evdeactivateConfirmAccounts";
        public const string EV_DEACTIVATE_CONFIRM = "evdeactivateConfirm";
        public const string EV_DEACTIVATE_SELECTEDACCOUNT = "evdeactivateaccountselected";
        public const string EV_SELECTEDCARD = "evcardselected";
        #endregion

        #region "Career Portal"
        public const string CAREERPORTAL_SEARCH_JOBS = "CareerPortalSearchJobs";
        public const string CAREERPORTAL_POSTED_SECTION = "CareerPortalpostedsection";
        public const string CAREERPORTAL_ErrorMessage = "CareerPortalErrormessage";
        public const string CAREERPORTAL_ListErrorMessage = "CareerPortalListErrormessage";
        public const string CAREERPORTAL_QUALIFICATION_LIST = "CareerPortalQualifications";
        public const string CAREERPORTAL_QUALIFICATIONGRP_LIST = "CareerPortalQualificationsGrp";
        public const string CAREERPORTAL_HELP_VALUES = "CareerPortalHelpValues";
        public const string CAREERPORTAL_REGISTRATION_TYPE = "CareerPortalRegistrationType";
        public const string CAREERPORTAL_PERSONAL_INFO = "CareerPortalPersonalInfo";
        public const string CAREERPORTAL_WORK_EXP_LIST = "CareerPortaWorkExperienceList";
        public const string UAEPASS_USER_TYPE = "UAEPASS_USER_TYPE";

        #endregion

        #region Customer Profile
        public const string CUSTOMER_PROFILE_MASTERDATA = "CustomerProfileMasterData";
        public const string CUSTOMER_PROFILE_ERROR = "CustomerProfileError";
        public const string CUSTOMER_PROFILE_SUCCESS = "CustomerProfileSuccess";
        public const string CUSTOMER_PROFILE_PIC_SUCCESS = "CustomerProfilePicSuccess";
        public const string CUSTOMER_PROFILE_STATE = "CustomerProfileState";
        public const string CUSTOMER_PROFILE_SUCCESS_OTP = "CustomerInformation_Success_OTP";
        public const string CUSTOMER_PROFILE_SUCCESS_OTP2 = "CustomerInformation_Success_OTP2";
        public const string CUSTOMER_PROFILE_RESPONSE = "CustomerProfileResponse";
        public const string CUSTOMER_PROFILE_MAIN = "profileMain";

        #endregion

        #region Manage Account information
        public const string AccountInformation_Error = "AccountInformation_Error";
        public const string AccountInformation_Success = "AccountInformation_Success";
        public const string AccountInformation_Success_OTP = "AccountInformation_Success_OTP";
        public const string AccountInformation_Success_OTP2 = "AccountInformation_Success_OTP2";
        public const string AccountInformation_Request = "AccountInformation_Request";
        public const string AccountInformation_Request_Onetime = "AccountInformation_Request_onetime";
        public const string verifymobilenumber_error = "verifymobilenumber_error";
        public const string AccountInformation_SELECTEDACCOUNT = "manageaccountinfoaccountselected";
        public const string AccountInformation_profileContractAccount = "profileContractAccount";

        #endregion

        #region [Scrap Sale]
        public const string SCRAP_SALE_OPEN_TENDER_LIST = "SCRAP_SALE_OPEN_TENDER_LIST";
        public const string SCRAP_SALE_OPEN_TENDER_RESULT = "SCRAP_SALE_OPEN_TENDER_RESULT";
        #endregion

        #region Smart Living
        public const string Dashboard_SELECTEDACCOUNT = "dashboardselectedaccounts";
        public const string SELECTED_CONSUMPTION = "ConsumptionSelection";
        public const string SELECTED_CONSUMPTIONACCOUNT = "ConsumptionSelectionaccount";
        public const string SELECTED_CONSUMPTION_SLAB = "ConsumptionSelectionSlab";
        #endregion

        #region "Owner Tracking"
        public const string OWNER_TRACKING_RESULT = "OwnerTrackingResult";
        public const string OWNER_TRACKING_LOGIN_RESULT = "OwnerTrackingLoginResult";
        public const string OWNER_TRACKING_ORDER_RESULT = "OwnerTrackingOrderResult";
        public const string OWNER_TRACKING_SEARCHOPTIONS = "OwnerTrackingSearchOptions";

        #endregion

        #region Agile Governance Global Summit
        public const string AGILE_GOVERNANCE_SESSION = "AgileGovernanceGlobalSummit";
        public const string AGILE_GOVERNANCE_RESULT = "AgileGovtGlobalSummitResult";
        #endregion

        #region Premise Number Search
        public const string PREMISE_ACCOUNT_SEARCHOPTIONS = "PremiseAccountSearchOptions";
        #endregion

        #region POD Event Registration
        public const string POD_EVENT_REGISTRATION_RESULT = "PODEventRegistrationResult";

        #endregion

        #region Track Request Anonymous
        public const string TRACK_REQUEST_ANONYMOUS_RESPONSE = "TrackRequestAnonymousResponse";
        #endregion

        #region [Refund]
        public const string IBANDetail_PaymentData = "IBANDetail_PaymentData";
        #endregion

        #region [MIME]
        public const string LastestDewaToken = "LastestDewaToken";
        #endregion
        #region EV Smart Charger
        public const string EV_SMART_WORKFLOW_STATE = "EVSmartCharger";
        #endregion
        #region EVDashboard
        public const string LISTOFCARDSERVICE_SELECTEDACCOUNT = "Listofcardservice_selectedaccount";
        public const string LISTOFCARDSERVICE_RESPONSE = "Listofcardservice_response";
        public const string EV_Transaction_List = "EV_Transaction_List";
        #endregion

        #region Infrastructure NOC
        public const string INFRASTRUCTURE_NOC_SELECTEDACCOUNT = "infrastructureNOC";
        #endregion

        #region **Forgot Password**
        public const string ForgotPassword_OTP = "ForgotPassword_OTP";
        public const string ForgotPassword_Saveddata = "ForgotPassword_Saveddata";
        public const string ForgotPassword_Step = "ForgotPassword_Step";
        public const string ForgotPassword_Username = "ForgotPassword_Username";
        public const string ForgotPassword_accountunlock = "ForgotPassword_accountunlock";
        #endregion

        #region **Jobseeker Forgot Password**
        public const string Jobseeker_ForgotPassword_OTP = "Jobseeker_ForgotPassword_OTP";
        public const string Jobseeker_ForgotPassword_Saveddata = "Jobseeker_ForgotPassword_Saveddata";
        public const string Jobseeker_ForgotPassword_Step = "Jobseeker_ForgotPassword_Step";
        public const string Jobseeker_ForgotPassword_Username = "Jobseeker_ForgotPassword_Username";
        public const string Jobseeker_ForgotPassword_accountunlock = "Jobseeker_ForgotPassword_accountunlock";
        #endregion

        #region [Estimate Refund | VAT Refund]
        public const string PostSmartCommunicationData = "PostSmartCommunicationData";
        #endregion

        #region Shams Dubai Subscription
        public const string ShamsDubaisubscription = "ShamsDubaisubscription";
        public const string ShamsDubaisubscriptionmodel = "ShamsDubaisubscriptionmodel";
        #endregion

        #region Samsung Pay
        public const string Samsungpayaccounts = "Samsungpayaccounts";
        public const string Samsungpayment = "Samsungpayment";
        #endregion

        #region Work Placement Evaluation
        public const string Internship_Survey_key = "Internship_Survey_key";
        public const string Internship_Survey_Questions = "Internship_Survey_Questions";
        public const string Internship_Survey_OTPSend = "Internship_Survey_OTPSend";
        public const string Internship_Survey_OTPVerification = "Internship_Survey_OTPVerification";
        #endregion
    }

    public static class SitecoreItemPaths
    {
        public const string GOOGLE_CONFIG = "/sitecore/content/Global Config/Google Config";

        public const string COSTS_CONFIG = "/sitecore/content/Global Config/Costs Config";

        public const string PROJECTGENERATION_USERS = "/sitecore/content/Global References/Project Generation/Users";
        public const string PROJECTGENERATION_PROJECTS = "/sitecore/content/Global References/Project Generation/Projects";
        public const string PARTNERSHIP_USERS = "/sitecore/content/Global References/Partnership/Users";
        public const string PARTNERSHIP_PARTNERS = "/sitecore/content/Global References/Partnership/Partners";
        public const string PARTNERSHIP_COORDINATORS = "/sitecore/content/Global References/Partnership/Coordinators";
        public const string DEWAPARTNERLIST = "/sitecore/content/Global References/PartnerList";

        public const string IDEALHOMECONSUMERSURVEY_USER = "/sitecore/content/Global References/Ideal Home Consumer/Users";
        public const string IDEALHOMECONSUMERSURVEY_USERBranch = "{1F4C7C09-EC1E-490E-9DB6-4640BFF6C21F}";
        public const string IDEALHOMECONSUMERSURVEY_SECTIONLIST = "/sitecore/content/Global References/Ideal Home Consumer/Survey/Section List/List of Sections";
        public const string LINKEDIN_CONFIG = "/sitecore/content/Global Config/LinkedIn Config";
        public const string YOUTUBE_VIDEOS_FOLDER_EN = "/sitecore/content/Global References/Youtube Video/EN";
        public const string YOUTUBE_VIDEOS_FOLDER_AR = "/sitecore/content/Global References/Youtube Video/AR";
    }

    public static class SitecoreRenderingIds
    {
        public const string GOOGLE_ANALYTICS = "{6084D5FA-6958-4F3A-9E29-3A302FC8F6C0}";

        //public const string BREADCRUMB = "{7C17534A-7F46-4822-B7A0-89B8CB6DE191}";

        //public const string BREADCRUMB2 = "{7FB19753-4F32-4D3A-B999-C9240BA12929}";

        public const string BREADCRUMB = "{CDA5C872-F7A7-497E-86EF-9AD0B579E7D3}";

        public const string HEROBREADCRUMB = "{73D36177-C53C-4909-AA8C-D7EC216C275A}";

        public static readonly ID WFFM_MVC_FORM = new ID("{F2CCA16D-7524-4E99-8EE0-78FF6394A3B3}");

        public static readonly ID WFFM_FORM = new ID("{6D3B4E7D-FEF8-4110-804A-B56605688830}");

        public const string LINKEDIN_ANALYTICS = "{98FD9703-5DA0-4D2D-87D6-FBE6CE8771EA}";

        public const string M74Expander = "{C7421E72-093D-45CE-A7CB-180A20CF9F0B}";

        public const string GOOGLE_ANALYTICS_CONFIG_HEAD = "{6EF961F6-2952-49BC-ADC5-A6F017DE0F54}";

        public const string M14FormattedText = "{C3EC4780-2E5B-42F2-8C94-C9F7BF830D6D}";
        public const string HappinessIndicator = "{FF65F471-CCA1-4430-A9BA-83D7933136AC}";
        public const string WebsiteSurveyPopup = "{CA71B92A-6CB3-4A2A-9F60-8239151D902D}";
        public const string Search = "{31190EC1-7A94-401E-A15C-573284A5D4DB}";
    }

    public static class SitecoreItemIdentifiers
    {
        public const string GLOBAL_ENERGYAUDIT_MAX_REQUEST = "{69DEE081-57F6-411F-B846-62B35BF5F553}";

        public const string SHAMS_DUBAI_SUBSDONE = "{759EACAC-2B29-42A3-BEB0-5462053FF899}";
        public const string SHAMS_DUBAI_SUBSCRIPTION = "{6B34D123-77D7-4870-88AD-783A24C3FE66}";

        public const string GLOBAL_HRQHSE_CONFERENCE = "{6C563638-50F6-468B-9084-48A5A59D67C9}";
        public const string HRQHSE_CONFERENCE_THANKS = "{2C174502-1BFC-4059-B339-559672D8CC37}";
        public const string HRQHSE_CONFERENCE_ERROR = "{8D7B5105-627D-4E41-B676-0F4CA7E041AA}";

        //ERM Conference Item
        public const string GLOBAL_ERM_CONFERENCE = "{FC32B102-C474-41DE-800A-982B8750CE5B}";

        public const string GLOBAL_DISABILITY_CONFERENCE = "{8CA43890-214F-447F-A5C8-9F05B4FF7BFC}";

        public const string MOVE_IN_ACTIVATION_SERVICE_ACCOUNT_DETAILS = "{AD02F8BC-5A36-4B83-B733-28799B39D595}";

        public const string IDEAL_HOME_DISPLAY_CUSTOMER = "{20F4F1BD-D66D-4052-B8F1-A98BDCC88628}";
        public const string IDEAL_HOME_SURVEY_THANKS = "{D3B4748B-C25E-4B5B-BDFC-9A80F162AD23}";

        public const string UPDATE_IBANPAGE = "{33E51979-DEFF-4299-8B4D-2355124C98DE}";

        public const string UPDATE_IBANPAGE_CONFIRM = "{B98D3229-82B2-4015-9B5A-5651C1C5D8BC}";

        public const string UPDATE_IBANPAGE_REVIEWS = "{1B75259F-2015-4FBD-95E0-89506460C655}";

        public const string UPDATE_IBAN_THANKS = "{3BEF9890-4A3A-45A3-BB44-08237FC92517}";

        public const string DIS_CONFERENCE_THANKS = "{907FA1AB-0AF6-4E90-B1A7-34B6ABFEED20}";

        public const string DIS_CONFERENCE_ERROR = "{549E6858-B263-4D17-B9C9-9F16D9C1D217}";

        public const string CONFERENCE_THANKS = "{90562147-0228-4E97-A298-71FB4C3E9A81}";

        public const string CONFERENCE_ERROR = "{4569241F-F9DE-4677-B5E9-7556D76E6DAD}";

        public const string COMPLAINTSURVEY_ERROR = "{8D800DEF-6AD2-49F5-9391-5DBD32B141F7}";

        public const string COMPLAINTSURVEY_THANKS = "{D8700568-1CE3-40E0-8E32-69B1F19FB859}";

        public const string HOME = "{765CD703-6BA9-4981-88BA-434FE00DA824}";

        public const string ABOUTDEWA = "{20425B1E-570C-4C56-B0F7-332373AAF7F0}";

        public const string LATESTNEWS = "{D4A4A0BB-5C23-4B19-AD15-E2FF9C86A9FF}";

        public const string NEWSANDMEDIA = "{AC8C089C-54CE-4647-9718-EBF1E18DFEDC}";

        public const string CUSTOMER_SERVICES_FAQ = "{F59DE490-FF3E-4A18-9850-7262506F99BB}";

        public const string COSTS_CONFIG = "{73FF7A95-1473-41AD-A6A9-C119E9DD0C85}";

        public const string J7_LOGIN_PAGE = "{879F48A3-0C26-4C2C-AFB6-9C8FDDEA4EAB}";

        public const string EXPO_LOGIN_PAGE = "{3A66B0D6-8FAF-489D-AEDB-3E3A6C01A1BD}";

        public const string J40_GreenPledgeFormEntriesFolder = "{E12DA8AF-E50D-4DC6-ABA4-9A552CBD5AC5}";

        public const string J40_GreenPledgeFormEntriesTemplate = "{C8BD8855-8883-4F94-9100-B8165CB25E7A}";

        public const string J40_GreenPledgeForm = "{399B2892-05E9-4772-9A71-59E342BE910D}";

        public const string J40_GreenPledgeEmailTemplate = "{2CAD141A-38DF-4F8B-976C-4954F0022C6F}";

        public const string J40_GreenPledgeHome = "{D9029343-94EA-420E-960B-70D7E4341876}";

        public const string J1_VIEW_PAST_BILLS = "{D3B99696-111D-45CB-8C1E-AD77390C3848}";

        public const string J1_VIEW_RECEIPT = "{BB608B4E-8987-49B6-B1E9-699ED0C88C4E}";

        public const string J7_LOGOUT = "{F0023F89-0269-4FDA-9CD4-F39C0A9D3C7E}";

        public const string J7_FORGOT_PASSWORD = "{5CA25659-5302-41DE-B9F3-6FC2F2108F4D}";

        public const string ACCOUNT_UNLOCK = "{748AA29C-0276-4B7A-A5C2-61A74AD1876B}";

        public const string J7_FORGOT_USERNAME = "{94914776-AE46-409D-97B9-4972AD6A6BD4}";

        public const string J7_RECOVERY_EMAIL_SENT = "{676D904C-147D-4231-B311-04FBFAA2E851}";

        public const string J7_SET_NEW_PASSWORD = "{61B9D80E-B47D-4D7E-B9E6-782EABB4FC9E}";

        public const string J7_LINK_MY_ID_TO_BP = "{42A6444E-583A-4F15-8B56-FBBBF931746F}";

        public const string LINK_UAEPASS_TO_BP = "{95754F4A-9381-4583-BECC-97782566B7AC}";

        public const string J7_MY_ID_LOGIN = "{48039B89-15C2-4097-A1BA-02CE3C88BEB0}";

        public const string J7_UAE_ID_LOGIN = "{86A8A341-5174-45BF-932E-AB08B1EC2864}";

        public const string J8_GOVT_LOGIN_PAGE = "{D9F875E4-62A3-4480-B80C-5122778CFA13}";

        public const string J9_CHOOSE_VERIFICATION_METHOD = "{252950A1-0B62-439B-BDB2-EE9887349620}";

        public const string J9_CONFIRM_VERIFICATION_METHOD = "{08F66595-97F6-4562-8A71-87438CBAE1B3}";

        public const string J9_ERROR_VERIFICATION_METHOD = "{78D45AEF-A177-482F-848B-A54BEC4DD0ED}";

        public const string J9_SETACCOUNT = "{2AABADE8-F12D-4187-AFFE-BACED1C3BABC}";

        public const string J9_SET_USERNAME_PASSWORD = "{AD1653E0-406F-4DAA-894B-72513D64F9F4}";

        public const string J9_VERIFY = "{3AD559A8-B864-443C-A87D-B392A6600B00}";

        public const string J9_CUSTOMER_REGISTRATION = "{9280F721-AC3A-4539-8CF9-272421E1D29B}";

        public const string J11_REQUEST_COLLECTIVE_ACCOUNT = "{C5C73B4A-89CF-49A7-8997-5BDD7C3A551A}";

        public const string J11_REQUEST_COLLECTIVE_ACCOUNT_SUCCESS = "{483A0045-3FA0-4D19-9723-1FDE75F3AA92}";

        public const string J12_ADD_TO_COLLECTIVE_BILLING = "{C5C9A69C-489E-40F5-B19E-46BA288772A1}";

        public const string J12_ADD_TO_COLLECTIVE_BILLING_SUCCESS = "{64D47C99-F0DF-405A-9345-61EF6F82075E}";

        public const string J10_LANDING_PAGE = "{84700373-26D7-4A1E-82AB-359F8256278A}";

        public const string J10_ACCOUNT_DETAILS = "{AD02F8BC-5A36-4B83-B733-28799B39D595}";

        public const string J10_IDENTIFICATION = "{C2F5625F-83B3-4378-9638-ABCD0D9FD495}";

        public const string J10_TENANCY_DETAILS = "{9DB839FC-265F-4D42-9ACF-8DB7C3896972}";

        public const string J10_CONTACT_DETAILS = "{7E6064D6-0469-42FA-A450-584B0C4D661E}";

        public const string J10_REVIEW_DETAILS = "{14CA31AC-5B1A-4BCA-81E7-E6A62A615D99}";

        public const string J10_PAYMENT = "{CE3581D3-90C4-4F52-90C3-BC771D20BCF9}";

        public const string J10_CONFIRMATION = "{F03EF836-13AE-40B5-B563-B2A59A8BBEC3}";

        public const string J25_LANDING = "{39E1C592-B701-4484-8F7D-9FC3D020E26D}";

        public const string J25_CREATE_ACCOUNT = "{B7F6DE49-EE1B-400A-9BB9-F1BB0D0A6CB8}";

        //public const string J25_ACTIVATION = "{57A34A2F-C9CE-40B1-91CB-4260ACFB5151}";

        public const string J25_ACCOUNT_DETAILS = "{5562203B-323E-4431-A9E8-76ADDD9A8FBE}";

        public const string J25_IDENTIFICATION = "{D0C95467-548D-473D-9236-6621A79854C1}";

        public const string J25_TENANCY_DETAILS = "{59536156-7EF9-406C-AFC3-0DAD213134B7}";

        public const string J25_CONTACT_DETAILS = "{2238EFAC-AC54-44C6-BBD3-9897C3EA5B6F}";

        public const string J25_REVIEW_DETAILS = "{984573D6-81D4-4C8A-8C1E-E2F5F2114DE5}";

        public const string J25_PAYMENT = "{45DACBC2-AE1A-4E56-BE82-2A6C6D0819EC}";

        public const string J25_CONFIRMATIOM = "{E22441C7-0BDC-4025-9C1F-4A427E4EBCC9}";

        public const string J27_TENDERLIST_PAGE = "{9F3FD893-EE6D-4A60-B06F-D9B8F290B545}";

        public const string J28_OPEN_TENDER_LISTPAGE = "{16767053-5A70-45E5-A29F-B3CCED1FAA65}";

        public const string J28_TENDER_RESULT_PAGE = "{EE58BF15-CB56-4595-8460-22EA9BE8075F}";

        public const string J10_SUPPLIMENTARY = "{72E5C436-A8D7-4E4A-8B76-B37542236687}";

        public const string MOVEIN_CONSTANTS = "{C347B245-54E7-4D1A-BE37-EB8E217A99D4}";

        public const string J14_PAY_BILLS = "{F6828D0E-D178-42CD-A79C-1A4CE9267F1B}";

        public const string J14_PAYBILL_REVIEW = "{C08DF36C-40BF-4646-8655-D4FE54CE629C}";

        public const string M51_SMARTWALLET_SELECTACCOUNT = "{7EA671D2-AB0A-4CB1-8D74-5CD4ADA2B0E5}";

        public const string M51_SMARTWALLET_REVIEW = "{BC8FDF25-6989-4A73-83C8-430C163E7442}";

        public const string M51_SMARTWALLET_ADDACCOUNT = "{3A5C940E-8732-4FCD-A055-2E0E885F2571}";

        public const string J15_PAYFRIENDBILL_START = "{35B245F5-D258-47C9-8647-9F450A7241E8}";

        public const string J15_PAY_A_FRIENDS_BILL = "{431D6299-928A-44AF-914A-F781E8233753}";

        public const string J18_MOVE_OUT_START = "{E63B87EA-141E-4400-BAC5-6B97F6B3046C}";

        public const string MOVE_OUT_START_V2 = "{E63B87EA-141E-4400-BAC5-6B97F6B3046C}";

        public const string MOVE_OUT_START_V3 = "{E63B87EA-141E-4400-BAC5-6B97F6B3046C}";

        public const string MOVE_OUT_REVIEW_V3 = "{B202D803-EF04-444D-8321-4EDC819D0AC4}";

        public const string MOVE_OUT_DEMOLISH_START_V3 = "{67B639A5-02E7-40EB-B6F9-EAC7DF039D8C}";

        public const string MOVE_OUT_DEMOLISH_GOV_START_V3 = "{B772F1CD-1377-4540-864E-D1681D5E39D5}";

        public const string MOVE_TO_START = "{80BDC65E-E06E-46CE-9765-22BC7252FDE6}";

        public const string MOVE_TO_START_DETAILS = "{9522FDB7-9B51-4D43-A631-5F44BE342B35}";

        public const string MOVE_TO_CONFIRM = "{FF3B365A-5DD3-4B39-AFC9-198145D1D92A}";

        public const string MOVE_OUT_DETAILS_V2 = "{A302C9DB-06AA-478C-A823-7C5C083BD6A1}";

        public const string MOVE_OUT_DETAILS_V3 = "{A302C9DB-06AA-478C-A823-7C5C083BD6A1}";

        public const string J31_COMPARATIVE_CONSUMPTION_HISTORY = "{65BAFEB1-501A-41F8-A6C8-6939CB0DA627}";

        public const string J18_MOVE_OUT_DETAILS = "{434239D3-91E5-48CA-8C18-AD4F4720BF0A}";

        public const string J18_MOVE_OUT_REVIEWS = "{B202D803-EF04-444D-8321-4EDC819D0AC4}";

        public const string J18_MOVE_OUT_CONFIRM = "{53569A40-40C7-47BC-97AC-8FDCB4F23703}";

        public const string MOVE_OUT_ANONYMOUS_CONFIRM = "{4D4CFA72-E5F2-47DA-8E31-1D54F8399018}";

        public const string J18_MOVE_OUT_DEMOLISH_CONFIRM = "{49624A0A-2EDD-48BF-830D-5DF3365021DB}";

        public const string J18_MOVE_OUT_DEMOLISH_CONFIRM_GOV = "{1362602A-CC38-413A-B9E2-DE9CADE8232C}";

        public const string J21_UPDATE_CONTACT_DETAILS = "{EA55F0F0-1DB1-48B3-80DD-714B3A57796A}";

        public const string J21_UPDATE_CONTACT_DETAILS_CONFIRMATION = "{797189F1-3E41-4514-9B7D-80125075A436}";

        public const string J22_SERVICE_COMPLAINT = "{DBC96906-B165-4E59-9BE2-2A583DF47E48}";

        public const string J22_TRACK_COMPLAINTS = "{E96FA79D-D288-426C-AFE1-B0B85A9016B9}";

        public const string J24_BILLING_COMPLAINT = "{71AB1FED-AA70-4BEF-AF82-86F2A23FAB84}";

        public const string J24_COMPLAINT_SENT = "{9FAF1D8E-EEFF-42D1-A52A-AE23C0812A5F}";

        public const string J24_COMPLAINT_FAILED = "{97385B0C-7C4E-40E2-B58A-EAA79C4098FA}";

        public const string J26_NEW_REQUEST = "{0AF5FE23-7F1E-4D80-9462-CB4B62C39BD1}";

        public const string J26_REQUEST_POST_LOGIN = "{75D48730-EF38-4BB5-87F0-8D820271F983}";

        public const string CLEARANCE_CERTIFICATE_DEWA_CUSTOMER = "{BF5CBF3D-E49F-4AAB-8799-EA47E7F3D2EA}";

        public const string CLEARANCE_CERTIFICATE_PROPERTY_SELLER = "{8EF4B59E-6989-46A7-B6F0-0FE7589B9FBB}";

        public const string CLEARANCE_CERTIFICATE_NON_DEWA = "{AFD1B633-631C-4D4D-BD47-4751169EB16C}";

        public const string VERIFY_CLEARANCE_CERTIFICATE = "{5328ACAF-9714-4E51-82F0-86FEDA8107E7}";

        public const string J26_REQUEST_SENT = "{ABD18DC2-83F5-4441-8EEB-32E878E0721A}";

        public const string J26_REQUEST_FAILED = "{7275E681-5087-43F0-9796-3FA2F56E3228}";

        public const string J31_CONSUMPTION_HISTORY = "{1C75F8DD-9170-415D-B19B-1BCF114D16B5}";

        public const string J31_CONSUMPTION_COMPARISON = "{65BAFEB1-501A-41F8-A6C8-6939CB0DA627}";

        public const string J60_CHANGE_PASSWORD_SUCCESSFUL = "{D07D817C-ABB6-439E-AC62-8EE9E3802E32}";

        public const string J60_CANNOT_CHANGE_PASSWORD = "{1DC48FE7-EB79-48ED-8344-BC36FD9EEF54}";

        public const string J69_CUSTOMER_DASHBOARD = "{DBDD9E2E-9712-4135-8CF4-8F66B0095D74}";

        public const string J71_SET_PRIMARY_ACCOUNT = "{02FF6940-77B0-46E5-8011-C056A6DFCC72}";

        public const string J71_SET_PRIMARY_ACCOUNT_CONFIRM = "{0AD2E2C4-9962-41FE-A766-092991656D47}";

        public const string J75_NEW_REQUEST = "{AE877866-75D1-46B8-9D8D-94029B4A863A}";

        public const string J75_SUBMISSION_SUCCESSFUL = "{DF521D21-7A97-4DF4-AC90-66526EE77035}";

        public const string J75_SUBMISSION_FAILED = "{AB4AE273-9345-49BD-B4FC-C026D70762A6}";

        public const string J75_SEARCH_REQUEST = "{71DE964C-D8F2-4F08-AC61-7CA8194CB736}";

        public const string J75_TRACK_PAY_REQUEST = "{13194ED5-2699-4670-B43F-52A658E1202D}";

        public const string J81_ESTIMATES_LANDING = "{36C84DDD-34B8-426A-B1FE-F907CFDC6156}";

        public const string J81_ESTIMATE_PAYMENT_CONFIRMATION = "{D00E0D61-CA2E-47B4-84F7-80729D5FE31F}";

        public const string J82_ESTIMATE_LANDING = "{3E7C1205-1ACE-454B-A36C-436F2396BCF2}";

        public const string J82_ESTIMATE_PAYMENT_CONFIRMATION = "{322A2065-53F2-4BF3-BB8A-C4F16B0C630B}";

        public const string J82_VIEW_ESTIMATE = "{E92518A5-F3C7-43C2-A782-1019A7A1963C}";

        public const string J88_CHANGE_CUSTOMER_CATEGORY = "{01DA82CE-723F-44B5-976C-190E5660350B}";

        public const string J88_CHANGE_CUSTOMER_CATEGORY_SUCCESS = "{F7E720CB-1144-4B0B-8088-71B0840F26DC}";

        public const string J89_PREMISE_CHANGE_REQUEST = "{66824954-7051-4FEC-BA34-E80FB67BBEB7}";

        public const string J89_PREMISE_CHANGE_SUCCESS = "{EFD6B247-5227-49B6-B7DF-9B6F40FACC24}";

        public const string J91_GOVT_OBS_REQUEST = "{BB354A72-1516-4BD1-8819-46A1DDCAF68C}";

        public const string J91_GOVT_OBS_REQUEST_SENT = "{A9992528-BE22-433D-9FEF-F67FB13F85DF}";

        public const string J91_GOVT_OBS_REQUEST_FAILED = "{F6DF3376-D93A-414F-8C13-05E4C038C8EE}";

        public const string J98_START = "{02FF6940-77B0-46E5-8011-C056A6DFCC72}";

        public const string J98_CONFIRM = "{0AD2E2C4-9962-41FE-A766-092991656D47}";

        public const string NEWS_LANDING_PAGE = "{D4A4A0BB-5C23-4B19-AD15-E2FF9C86A9FF}";

        public const string M41_TAB_RENDERING = "{71322749-0FB8-423D-BCF5-99F80E3543AA}";

        public const string M43_MODAL_TABS = "{85DD64C5-FE26-4340-B334-4AACF4D9DFE8}";

        public const string ALL_DEWA_SERVICES = "{E7627E14-8095-4A5F-9ACD-FB4EBE1490A7}";

        public const string TARIFFS = "{439FB21B-1A7D-4186-9B5B-1A0773ABD7BF}";

        public const string J73_RERA_USER_REGISTRATION = "{A0A4741E-C0F1-4631-80A0-53B1CF85F7D6}";

        public const string J73_RERA_PAYMENT_DETAILS = "{EF100C33-0EC7-4292-ACD7-73434A0F5F6D}";

        public const string ACCESSIBILITY_OPTIONS = "{0BDC45E9-A004-45C4-99C1-8F9AC5952080}";

        public const string SITE_SEARCH_RESULTS = "{C02F5BBF-8A04-4D83-889A-2FDE2A2AEBC3}";

        public const string SEARCH_PAGE_EVENT = "{0C179613-2073-41AB-992E-027D03D523BF}";

        public const string ERROR_404 = "{8ADC7317-FC51-404F-8741-DCA913DCDDD4}";

        public const string J90_CHANGE_LANDLORD_INFORMATION = "{F3E105CF-3B83-4225-B47D-08ED953C750C}";

        public const string J90_CHANGE_LANDLORD_INFORMATION_SUCCESS = "{110B10AF-253E-49FD-AE0B-5CB766D8FE44}";

        public const string LANDLORD_SUPPLY_ACTIVATION_SUCCESS = "{6C001158-E59C-458C-9774-F64CA1EE3B96}";

        public const string LANDLORD_SUPPLY_ACTIVATION = "{004C6D04-2F8E-4659-93C6-1EDB0AE2488D}";

        public const string FAQ = "{F59DE490-FF3E-4A18-9850-7262506F99BB}";

        public const string J85_BUILDING_INFO = "{32F93E1D-4C92-470B-B441-073621F82D9D}";

        public const string UNDERSTAND_YOUR_BILL = "{20FCD38F-F220-411B-AD96-0334F7CFE101}";

        public const string EASYPAY_UNDERSTAND_YOUR_BILL = "{DD4943FA-10BA-4271-8C57-B1B7D482425D}";

        public const string EASYPAY = "{46FA36F5-75D2-47AD-B0EE-2F0769A7F2C8}";

        public const string TRANSACTION_HISTORY_PAGE = "{D3B99696-111D-45CB-8C1E-AD77390C3848}";

        public const string CAREER_PORTAL_DASHBOARD = "{1DB1B717-5F09-4EE5-896F-4B562E7E262C}";
        public const string CAREER_POTAL_LOGIN = "{85CBFF97-CF17-477F-A172-FB205830BD64}";
        public const string CAREER_POTAL_FORGOTPASSWORD = "{62734382-7797-4BA9-BE29-73591BC8D2DF}";
        public const string CAREER_POTAL_FORGOTUSERNAME = "{D10F840D-2EFE-4A63-BB9C-AF2234938A0A}";
        public const string CAREER_POTAL_CANDIDATEREGISTRATION = "{235C9A96-F113-4EFC-901D-B2CA037D4665}";
        public const string CAREER_POTAL_RECOVERY_EMAIL_SENT = "{B04B7FA0-5C55-4354-8990-AA84B2008AAD}";
        public const string CAREER_POTAL_JOBSEARCH = "{45475643-E16C-4C28-9A08-082E228FD382}";
        public const string CAREER_POTAL_VERIFICATION = "{40027692-AC05-4852-8232-B4FE6877E80F}";
        public const string CAREER_PORTAL_PROFILE = "{A67E55AB-4B48-4DBF-8FFE-A1246782E31C}";
        public const string CAREER_PORTAL_PROFILE_SUCCESS = "{C0953B0C-397C-42D5-8869-EC295C6650E0}";
        public const string CAREER_PORTAL_APPLICATION_WIZARD = "{37CF16A8-E0A7-44F4-B43B-682FDA874984}";
        public const string CAREER_PORTAL_APPLICATION_SUCCESS = "{B9BFF3A1-3A2D-4A2F-B8E1-279B12736A73}";
        public const string CAREER_PORTAL_POD = "{99D9FFED-EE8C-47FB-9D07-99C7C5D72CDB}";
        public const string CAREER_PORTAL_TELL_FRIEND = "{02C07A18-DB31-4AC5-9578-B010305DD4B3}";
        public const string CAREER_PORTAL_CAREER_FAIR = "{E623A98A-E961-4356-B01A-7D69B375C6C8}";
        public const string CAREER_PORTAL_JOBDETAILS = "{3F1A8A19-1585-44AA-B9CB-5C62873614F0}";
        public const string CAREER_PORTAL_ACCOUNT_UNLOCK = "{CA79A43B-D499-4131-BF6B-1F8335706E13}";
        public const string DEWA_TERMS_CONDITIONS = "{07E5D367-8BAD-43F0-ACE6-8597C5D31C0A}";
        public const string DEWA_SECURITY_POLICY = "{1AC02FF6-2C6F-4487-832D-E42C76367C9B}";

        #region [Manage Beneficiery]
        public const string ADD_BENEFICIARY = "{39AC89BE-5B5E-4A3B-9917-DA03F8A8C2A3}";
        public const string MANAGE_BENEFICIARY = "{E04E94BC-114B-46A7-97EA-EDE3DA3C68DD}";

        #endregion

        public const string J85_CONFIRMATION = "{7EFDE581-204C-4589-99CF-65E6223B97A9}";

        public const string J85_CUSTOMER_DETAILS = "{67BBF228-34A5-4273-8082-2817EDDA2F7D}";

        public const string J85_INITIAL_BUILDING_INFO = "{F519AC7D-955B-4E9C-AD9B-1C14A7AFB31F}";

        public const string MY_ACCOUNT = "{FB606B30-EC6F-4952-B680-1E2C050CDED0}";

        public const string J93_ALL_VACANCIES = "{32A4C8D6-4C95-4EF4-A95E-7E3C33543E22}";

        public const string J93_VACANCY_DETAILS = "{80E5BFB2-F433-45CB-90B7-8DD92C3846BC}";

        public const string CONSERVATIONTIPS = "{8E1DCC25-83BB-4719-BC72-C091C1921922}";

        public const string CONSERVATIONTIPVIDEOS = "{A5036EBA-BAB9-4770-B469-4EF6203E61A7}";

        public const string ABOUTUSFOLDER = "{A95D29C0-337F-4A3F-A42C-F961D96E07E1}";

        public const string VIDEOGALLERYPAGE = "{BD93AF49-23C1-40DD-87D7-B140D7C4C9F4}";

        public const string VIDEOPLAYLISTPAGE = "{85351418-6080-4057-975B-816918101AAA}";

        public const string YOUTUBE_VIDEOS_FOLDER = "{776471E5-9AC8-4154-9057-100D59CC3EBA}";

        public const string YOUTUBE_VIDEOS_FOLDER_EN = "/sitecore/content/Global References/Youtube Video/EN";

        public const string YOUTUBE_VIDEOS_FOLDER_AR = "/sitecore/content/Global References/Youtube Video/AR";

        public const string YOUTUBE_CONFIG = "{FCD459E7-4897-4981-BA3A-45EB754C0831}";

        public const string NATIONALIDENTITYPAGE = "{45FF03DB-40D8-49EC-9441-D2D6AB148617}";

        public const string DEWAPUBLICATIONS = "{936B59D6-FF94-432C-BF35-4B8587D3620E}";

        public const string CIRCULARSFORMS = "{FCF24F56-13B8-46AE-8D39-EFFA1B5B68CA}";

        public const string SUSTAINABLEENERGY = "{B7395AA1-C465-4A29-B896-7A7553471B76}";

        public const string HISHIGHNESSQUOTES = "{2BD47083-9C1B-4A82-BC08-806FE3EF302E}";

        public const string PHOTOGALLERYPAGE = "{CC59B1D6-C97A-4716-B415-F57CB02ABD77}";

        public const string GOVERNMENTENTITIES = "{2A82681D-9D0F-43B2-B7D6-70859350722F}";

        public const string GREENBILLTAB = "{59DD1E2B-F4E3-4C96-93C9-E16EEFC1EC9D}";

        public const string SECURITYANDPOLICYPAGE = "{1AC02FF6-2C6F-4487-832D-E42C76367C9B}";

        public const string CAREFOREARTHPAGE = "{CACFA2E2-AAE1-4E2D-8EA4-4CDAA55B869D}";

        public const string TARIFFPAGE = "{439FB21B-1A7D-4186-9B5B-1A0773ABD7BF}";

        public const string TARIFFCALCULATORPAGE = "{A378011F-E99B-4090-A14B-7544FCDAD22E}";

        public const string J86_LOGIN_PAGE = "{6DD1F5E4-4D02-4FF6-BF59-AFE92DD5CF69}";

        public const string J86_FORGOT_PASSWORD = "{1354CB78-3B72-4443-9C8E-808E51D011CA}";

        public const string J86_RECOVERY_EMAIL_SENT = "{BA5F85B4-DE57-4AC7-AF4B-90D64993F6D5}";
        public const string J86_SET_NEW_PASSWORD = "{78E87FC3-1E9C-4F69-8D5A-D1D9B4308349}";
        public const string J86_DOCUMENTUPLOADPAGE = "{BE7AD25E-AFE6-474D-8D25-A6CFD2641E63}";
        public const string J86_DOCUMENTSUCCESSPAGE = "{3DFCFE2F-5E59-47EC-944D-78B9822A1B9F}";

        public const string J86_PPGCONFIG = "{4D76A4A2-DE30-429E-891A-CD1B1835D8F2}";

        public const string IDEALHOME_LANDINGPAGE = "{9445532D-7C44-4AD9-B919-7450DF8A8054}";
        public const string IDEALHOME_SURVEY = "{717DE7E3-1100-4196-92B7-8AC28F6311B2}";
        public const string IDEALHOME_LOGIN = "{E9B59A26-6E14-42B3-96B3-291666FA07C6}";
        public const string SET_VAT_SUCCESS = "{5409A51A-9FF1-404D-BF70-B2BD74EA7AD6}";

        public const string RECAPTCHA_CONFIG = "{156F9379-983D-47E8-89FA-398650C5AD82}";

        public const string CLEARANCE_CERTIFICATE_ACCOUNT = "{27061FD5-6204-41FE-BC9E-8F2DD865F2DC}";

        public const string RammasLogin_SUCCESS = "{27864D1F-7B87-4448-BA0E-57BBA62C8B75}";

        public const string RammasLandingPage = "{D1668C6F-CE3C-4E3F-BD45-7AFEED32FCD6}";

        public const string Api_Count_Config = "{00BA1240-8D7C-4A66-9CF5-42C3631EB7DA}";

        #region Smart Response Config
        public const string Smart_Response_Config = "{F0C12D42-F6A3-4D00-B1C5-86963C55C372}";
        #endregion

        #region Mobile Header Footer Config
        public const string Header_Footer_Config = "{8E1D110B-B071-42F0-94B6-60FC62978D22}";
        #endregion

        /// <summary>
        /// Added by Syed Shujaat Ali
        /// </summary>
        public const string CLEARANCE_CERTIFICATE_REFERENCE_NO_ERROR = "{230F6E77-A44F-4118-859D-F3EF7F01C241}";

        public const string CLEARANCE_CERTIFICATE_DEWACUSTOMER = "{BF5CBF3D-E49F-4AAB-8799-EA47E7F3D2EA}";

        //Scholarship Implementation - forgot username/password
        public const string SCHOLARSHIP_FORGOT_PASSWORD = "{A6FB9121-4A1B-4020-8160-09E50F799B2A}";

        public const string SCHOLARSHIP_FORGOT_USERNAME = "{71B5661C-637A-4A8C-B343-6564B3191CEF}";
        public const string SCHOLARSHIP_SET_NEW_PASSWORD = "{E987F936-6759-47D7-9F88-ADDF0FDF2A5B}";
        public const string SCHOLARSHIP_RECOVERY_EMAIL_SENT = "{17B2B03F-61DC-4D3B-85C9-23C0EA513DDF}";
        public const string SCHOLARSHIP_REGISTRATION_PAGE = "{82D90FF9-791D-4F53-B450-A7BD776E2DA7}";
        public const string SCHOLARSHIP_SIGNIN_PAGE = "{AEA8B835-6816-4DA6-ACD3-E9D33CBDBB62}";

        public const string SCHOLARSHIP_PERSONAL_INFORMATION_PAGE = "{3465B21E-42A9-43D9-B1FD-4273CB2122CB}";
        public const string SCHOLARSHIP_CONTACT_DETAIL_PAGE = "{45B729FE-9B19-4B6E-A455-07E69C383ACD}";
        public const string SCHOLARSHIP_ACADEMIC_INFORMATION_PAGE = "{715770A7-F729-4277-9FA7-135350FC32DF}";
        public const string SCHOLARSHIP_QUESTIONNAIRE_PAGE = "{521BF876-9A5B-43D0-BC67-AC8C8C2798CB}";
        public const string SCHOLARSHIP_PROFILE_COMPLETED_PAGE = "{D9440E46-F7FC-4406-9154-1F207431E9EE}";
        public const string SCHOLARSHIP_PREAPPLY_PAGE = "{9ED2FADA-76EE-4882-9EF4-0ECA548F57A4}";
        public const string SCHOLARSHIP_EMAIL_VERIFICATION = "{C5CC2C1D-F547-4B7F-9F8D-E9EED367778E}";

        public const string CommonComplaint = "{9BC74757-459E-448F-A6B5-BA8D2101E703}";
        public const string SmPredcitPageID = "{F2ABF0DD-0DBB-4704-929E-141AD69F8815}";

        #region
        public const string REFUNDHISTORY = "{14AA3837-B48C-4AB8-B157-BB02E92D1C78}";
        #endregion

        #region Ideal Home consumer

        public const string IDEALHOMECONSUMER_VIDEO_RESPONSE_TEMPLATE = "{87124854-1533-4D95-A70F-FCE690A70C57}";
        public const string IDEALHOMECONSUMER_SURVEY_RESPONSE_TEMPLATE = "{A47BED1F-422D-4DEC-B16C-0CDF1B7F0141}";

        public const string IDEALHOMECONSUMER_VIDEOLIST_TEMPLATE = "{194227E7-6861-4DB2-82F1-7E662A674E34}";
        public const string IDEALHOMECONSUMER_VIDEOLIST = "{9B4A8B2A-6CCD-4537-BC7E-9695CCB98902}";

        public const string IDEALHOMECONSUMER_VIDEOGROUPLIST = "{00D35CED-031A-40D0-81D4-A95A7555476C}";
        public const string IDEALHOMECONSUMER_USER_REGISTRATION_SUCCESS = "{77385AFA-FD4D-49E3-B73B-5332E956D176}";

        public const string IDEALHOMECONSUMER_USER_REGISTRATION = "{453EDFA6-2988-4C65-B7AC-F2C3435D2558}";

        public const string IDEALHOMECONSUMER_SURVEY = "{5D8B780E-BDFA-47D4-AD83-7674F6811989}";

        public const string IDEALHOMECONSUMER_SECTIONLIST_TEMPLATE = "{5B9BE67C-BB20-4CDE-8C04-FAD2179B6A59}";
        public const string IDEALHOMECONSUMER_SECTIONLIST = "{3C6D8A54-1798-4A6D-98AC-D1517E1813FF}";

        public const string IDEALHOMECONSUMER_LOGIN = "{D446DB8D-96BA-49AF-9CD9-AA9A7AE09DCE}";

        public const string IDEALHOMECONSUMER_FORGOTPASSWORD = "{A6D93C40-05BD-4855-A609-195E6FC3A7FA}";
        public const string IDEALHOMECONSUMER_SETNEWPASSWORD = "{89498E1D-CE10-4B4E-A89D-16C4B00E96D2}";

        public const string IDEALHOMECONSUMER_DASHBOARD = "{96A56FFD-B982-40FC-A0C3-228BBC7A4C53}";

        public const string IDEALHOMECONSUMER_USER_DATA = "{11B52500-656B-42FD-A9E2-0A70781C8BB7}";

        public const string IDEALHOMECONSUMER_LOGOFF = "{8D65DD01-1DCF-4DCD-BBFD-4DBC506C10FA}";

        public const string IDEALHOMWCONSUMER_USER_PROFILE = "{BD4B2A24-89C4-4607-9C01-FE28205657D1}";
        public const string IDEALHOMECONSUMER_USER_PROFILE_UPDATED_SUCCESS = "{17D0B42F-329F-4FCB-A5DE-0FFAE87006FC}";

        public const string IDEALHOMECONSUMER_EMAIL_PASSWORD_CONFIG = "{82CC3F8C-E509-4FB0-9995-75A796429DCC}";

        public const string IDEALHOMECONSUMER_EMAIL_REGISTRATION_CONFIG = "{2AF8E8F4-CD3E-4D6F-BD71-4EA184BFCF01}";

        public const string IDEALHOMECONSUMER_EMAIL_SUCCESS = "{07647CF1-C2B2-4B86-989B-F17604FC510B}";

        public const string IDEALHOMECONSUMER_DISPLAY_VIDEO_PAGE = "{7EDCC065-F25C-4266-8923-2D26E105A8D2}";

        public const string IDEALHOMECONSUMER_DISPLAY_CERTIFICATE = "{B41D4998-EA43-4B1E-93CE-3AA86FB2B4E5}";//"{3D7D9F1D-E76B-4FC0-954C-0CCF80C002FC}";

        public const string IDEALHOMECONSUMER_DISPLAY_RESULT_PAGE = "{8D62CCFD-CF8E-4062-B8BC-2DD324C96DEF}";//"{9F50E0D2-696E-4B35-A524-C96D719244F5}";

        public const string IDEALHOMECONSUMER_HTML_TEMPLATE_CONFIG = "{79C6EC1D-D9D5-40C5-A2BC-9BF76DFADEF7}";

        public const string IDEALHOMECONSUMER_LANDINGPAGE = "{7216D1BA-7EAF-4409-8AC3-D8A491CB33B8}";

        #endregion

        #region Tayseer
        public const string TAYSEER_GENERATE_REF_NO = "{F28A44D1-EC02-496A-8E7B-ED98AB22D33C}";
        public const string TAYSEER_PAYMENT_REVIEW_EXISTING = "{0002A6D3-7020-4542-B1A3-391A4A03D354}";
        public const string TAYSEER_PAYMENT_REVIEW_UPLOAD = "{FA94A736-333F-47D6-A419-073000C5A905}";
        public const string TAYSEER_EXISTING_ACCOUNT = "{93FB586D-5FC4-4FC1-8FF3-3662F800413E}";
        public const string TAYSEER_RERERENCE_DETAILS_HISTORY = "{912D2D43-9A07-468B-8E53-C8F256DA8090}";
        public const string TAYSEER_PAYMENT_FILE_UPLOAD = "{A9018E31-BC73-4D51-9DB2-2ADBE844A957}";
        public const string TAYSEER_REF_NO_HISTORY = "{B3B29860-86C2-457E-A887-2B75DEE7F2F3}";
        #endregion

        #region VerifyDocument
        public const string Verify_Document_Type = "{A46CD26F-0A94-4EF8-AC81-D5B7E9CBA869}";
        public const string Verify_Document = "{F4385D54-2C1B-4379-ADC1-2F8C67C1FE35}";
        #endregion

        #region Authorized Move in
        public const string MOVE_INTO_INTRO = "{14A13C23-C5EF-4FCD-91E8-36D5456D32B1}";
        public const string MOVEINEJARIREQUEST = "{3D6A1969-0358-40AE-A8C8-014E7D64C91B}";
        public const string MOVEIN_TENANT_DETAILS = "{C787DB16-32D9-45CB-BD3C-263409A58204}";
        public const string MOVEIN_CREATE_ACCOUNT = "{D091DC21-4EA8-457A-8D3F-9123723A426C}";
        public const string MOVEIN_CONTACT_DETAILS = "{8ECDB068-082C-4E46-8A52-05FCE85BF91C}";
        public const string MOVEIN_LANDING_PAGE = "{25FA1138-38C9-4579-9A57-372C9AB7BE92}";
        public const string MOVEIN_PAYMENT_PAGE = "{80FBC1F4-2D65-421F-BFDE-8B2424BD3DB1}";
        public const string MOVEIN_TENANT_DETAILS_PAGE = "C787DB16-32D9-45CB-BD3C-263409A58204";
        public const string MOVEIN_CREATE_ACCOUNT_PAGE = "D091DC21-4EA8-457A-8D3F-9123723A426C";
        public const string MOVEIN_CONTACT_DETAILS_PAGE = "8ECDB068-082C-4E46-8A52-05FCE85BF91C";
        public const string MOVEIN_LANDING_PAGE_PAGE = "25FA1138-38C9-4579-9A57-372C9AB7BE92";
        public const string MOVEIN_PAYMENT_PAGE_PAGE = "80FBC1F4-2D65-421F-BFDE-8B2424BD3DB1";
        #endregion

        #region PreLogin Move in
        public const string PRE_LOGIN_MOVE_INTO_INTRO = "{14A13C23-C5EF-4FCD-91E8-36D5456D32B1}";
        public const string PRE_LOGIN_MOVEIN_LANDING_PAGE = "{AF0BA0FE-D52A-4479-9EA1-FACC4E507172}";
        public const string PRE_LOGIN_MOVEINEJARIREQUEST = "{3D6A1969-0358-40AE-A8C8-014E7D64C91B}";
        public const string PRE_LOGIN_MOVEIN_TENANT_DETAILS = "{181614FA-DF8A-4FBE-8228-8D3794963627}";
        public const string PRE_LOGIN_MOVEIN_CREATE_ACCOUNT = "{72360DBD-0B0F-4C41-9B53-AC1BAF28A3B7}";
        public const string PRE_LOGIN_MOVEIN_CONTACT_DETAILS = "{9D4635CE-EC67-4E49-AEEB-23DA76C9AE70}";
        public const string PRE_LOGIN_MOVEIN_PAYMENT_PAGE = "{895B22F6-AB7F-433A-AC28-7A962C38F5EE}";
        #endregion
        #region PreLogin Move in V3
        public const string PRE_LOGIN_MOVEIN_LANDING_PAGEv3 = "{F5BDBAEC-CC8C-4123-A1AC-7CE01F8FA3E7}";
        public const string PRE_LOGIN_MOVEIN_CREATE_ACCOUNTv3 = "{5016325E-E630-43BE-B3CA-A59851D546FC}";
        public const string PRE_LOGIN_MOVEIN_CONTACT_DETAILSv3 = "{2C73D165-88D6-4F73-A121-E1197C1613C7}";

        #endregion
        #region Login Move in V3
        public const string LOGIN_MOVEIN_LANDING_PAGEv3 = "{CFE76438-53ED-41D8-B807-676F3347D3E8}";
        public const string LOGIN_MOVEIN_CONTACT_DETAILSv3 = "{300B618D-6112-461E-82C7-A39947FADF2B}";
        public const string LOGIN_MOVEIN_PAYMENT_PAGEv3 = "{F1D5705A-C84E-479C-B7C2-C24CF6B46919}";
        public const string LOGIN_MOVEIN_REVIEW_PAGEv3 = "{9B0B3A33-A35C-4317-A88B-D9510DA62322}";
        public const string LOGIN_MOVEIN_PAYOTHER_REVIEW_PAGEv3 = "{2199A94C-8552-4B54-AC4A-493021DA41FC}";
        public const string LOGIN_MOVEIN_CONFIRMATION_PAGEv3 = "{7F8C3FA5-A510-4EA5-94E8-D567B635D933}";
        public const string RERA_MOVEIN_CONFIRMATION_PAGE = "{1F40D56B-646F-4CE5-9ED9-CB5E4A268439}";
        public const string RERA_MOVEIN_PAYOTHER_REVIEW_PAGEv3 = "{07B54009-1CF7-46F9-BE47-5E8F77340B22}";
        public const string RERA_MOVEIN_PAYLATER_REVIEW_PAGEv3 = "{4B2F9432-BD89-4C42-8AC6-D454A38AE437}";
        #endregion

        #region Unsubscribe
        public const string UnsubscribeThankyou = "{912E0C39-E1FE-474E-9DF1-280D0534F855}";
        public const string ManageSubscriptionerror = "{CAEBE6DC-C738-44D7-8537-30C2FE63D7D9}";
        public const string ManageSubscriptionconfirmation = "{F5EE7307-3CC6-4646-9AF4-7D20F9D8B7B5}";
        public const string Verifyerror = "{15E9A09C-B1A7-433A-96E6-7A029FD778BF}";
        #endregion

        #region
        public const string MOVEINTOLANDING_PRELOGIN = "{A29C4571-D023-48E0-8A43-9E5B392FF593}";
        public const string MOVEINTOLANDING_LOGIN = "{55D320CE-4CA2-4FE3-9584-E84001052879}";
        public const string MOVEINTOLANDING_MOVEOUT_DEMOLISH = "{8B9CDC2C-5CE0-4D83-8AB1-C2BF31BF7B7D}";
        public const string MOVEINTOLANDING_MOVEOUT = "{79CEC495-21A2-4821-8834-DD7393F04533}";
        public const string MOVEINTOLANDING_MOVETO = "{724B9177-AC3A-4C95-B433-0808B0D92256}";
        #endregion

        #region Move out

        #endregion

        #region DRRG
        public const string DRRG_PAGE_TEMPLATE = "{6F7B5344-6588-4CA7-854F-A8765E7E958A}";
        public const string DRRG_HOME = "{33E8E4A8-A58A-4CE2-9EE2-358DA0D4A171}";
        public const string DRRG_AUTHORIZED_LETTER = "{751DF9D1-274E-42B2-9C76-B4E831E67F01}";
        public const string DRRG_AUTHORIZED_LETTER_TEMPLATE = "{CC017FB1-9384-44CA-8B79-C2626AD57756}";
        public const string DRRG_AUTHORIZED_LETTER_MANUFAC_TEMPLATE = "{6BDB9A1F-1EC1-481D-851D-660F56E8D77A}";
        public const string DRRG_FACTORY_TEMPLATE = "{2AEE3102-30F1-495C-8F98-1096162EAFEE}";
        public const string DRRG_PENDING_EVALUATION = "{B8943502-C89E-46A8-895C-24CEC1340F70}";
        public const string DRRG_REJECTED = "{9B7EFB8C-D684-4D1B-AFC2-B8E04AE7FEB5}";
        public const string DRRG_REJECTED_APPLICATION = "{E06E3F68-A704-472B-87D6-FF23597FE661}";
        public const string DRRG_LOGOUT = "{09772E21-CD1C-46F7-A40A-A6078795C1FE}";
        public const string DRRG_DASHBOARD = "{560F9F52-F3CE-4F6D-90B5-D3B15BEDBD46}";
        public const string DRRG_FORGOT_PASSWORD = "{F5652155-6281-44EC-8487-D9074BC7374D}";
        public const string DRRG_SET_PASSWORD = "{0B940B48-3B04-4C72-9583-A56256F486C3}";
        public const string DRRG_REGISTRATION_HOME = "{0D81A277-25D2-4F10-9754-9505062E9019}";
        public const string DRRG_PVMODULE = "{5CEA10F1-4D2F-480E-B7B6-FD56E294F039}";
        public const string DRRG_INVERTERMODULE = "{5059C56D-25DE-42EA-8980-45524FD14E7B}";
        public const string DRRG_INTERFACEMODULE = "{426E011E-C076-499D-B9FE-59DF838DEC94}";
        public const string DRRG_VIEWAPPLICATION = "{EDF45903-3F7A-4A75-A1E7-C3FF4D21E2E2}";
        public const string DRRG_DETAILS = "{F23FF9AE-8EE3-40A1-9B8D-221E23BB97A3}";
        public const string DRRG_UPDATEPROFILE = "{264C267D-D13D-4B0E-973D-6DA8F5D6BF48}";
        public const string DRRG_UPDATEPROFILE_WARNING = "{ADF2C0C8-032A-4CBC-8603-6758D70F3E67}";
        public const string DRRG_CONFIG = "{9358A6D6-F812-4689-ADB6-E8CC2C609595}";
        public const string ePass_CONFIG = "{B46CBDA9-C2FB-4FB5-A4EF-093469D5ED30}";
        public const string DRRG_PV_DECLARATION_LETTER = "{40D3798A-0E13-4BBC-81BF-90C0B1847C57}";
        public const string DRRG_INV_DECLARATION_LETTER = "{59D8FEAB-7A40-4C54-B1EB-6BD3881FC93E}";
        public const string DRRG_IP_DECLARATION_LETTER = "{F52612B0-F5BC-463A-BE8E-683C3B5E9A2F}";
        public const string DRRG_UPDATEPROFILESUCCESS = "{0A749F1D-BCC6-4DEC-A986-9C04B7D346CD}";
        public const string DRRG_AUTHORIZATIONLETTERSUCCESS = "{67ECC603-3503-4756-8EFF-6C12D4F66139}";

        #endregion

        #region DRRG Evaluator
        public const string DRRG_ADMINPAGE_TEMPLATE = "{543522F0-0BDF-4AE4-883D-246E801E646F}";
        public const string DRRG__EVALUATOR_REGISTRATION = "{F97EAD11-3130-4C00-9EF9-087430290CDB}";
        public const string DRRG__EVALUATOR_LOGIN = "{A1B4D763-F4A3-471C-A3CF-85494822B1BA}";
        public const string DRRG__EVALUATOR_Dashboard = "{09C0C59C-794B-4BA0-93D7-601B1A0A5DFD}";
        public const string DRRG__EVALUATOR_Details = "{EFE02CA7-341B-41A7-8E78-EB9F08846D4C}";
        public const string DRRG__EVALUATOR_ManufactureList = "{16DF1CC9-09D2-4350-918D-573FA3F3BD74}";
        public const string DRRG__EVALUATOR_EditedProfileList = "{353C0220-E134-435E-AF15-79E056E4F7C6}";
        public const string DRRG__EVALUATOR_UpdatedProfile = "{7752961B-3969-4BCE-BB66-AC6FCDEBEED7}";
        public const string DRRG__EVALUATOR_RejectedApplicationList = "{0779C06B-1441-46FF-A879-68F352AE811C}";
        public const string DRRG__EVALUATOR_RejectedApplication = "{CAC57EE7-ECC1-4EEE-8551-90B0C1253F83}";
        public const string DRRG__EVALUATOR_EquipmentList = "{32553F56-ED02-4BDF-A309-38D3FBD5B260}";
        public const string DRRG__EVALUATOR_RegistrationList = "{C31442C2-8698-49CB-8A9E-58FCD300B743}";
        public const string LOG_OF_APPLICATION = "{C5C4373B-4E15-44A5-B6DD-EDC6993E0931}";
        public const string Eligible_List_APPLICATION = "{E8707305-C825-4C1F-B05E-358B096E7FEB}";
        #endregion

        #region EVCharging
        public const string EV_ApplyCard = "{E824D33F-EA8B-4F5A-93BD-A83EB59A8993}";
        public const string EV_Services = "{D4515897-72D9-44CA-B183-FBBBC55EF3D6}";
        public const string EV_Success = "{0A76AAD7-4D58-4795-918F-B6FA8387DC22}";
        public const string EV_NonDewa_Success = "{CA76CED3-5DA2-4C52-8DDC-217864D0C9B6}";
        public const string EV_Deactivate_Success = "{A1D8EDCD-7E72-4E0F-B9D8-D87B14730FF8}";
        public const string EV_Title = "{B48A3A79-81CE-45E9-A41D-F0C9747775ED}";
        public const string EV_NoOfCars = "{4B643EB0-E406-47D5-A413-E9D84074CF0B}";
        public const string EV_Registration = "{AB4C3FCD-31F7-4CA6-9D3B-C17315A8B55C}";
        public const string EV_ApplyCard_NonDewa = "{73FC8BDD-58C7-4CC7-AE3B-05AC926E7EFA}";
        public const string EV_ReplaceCard_Reason = "{5C14DB9F-E82A-45B3-9BF9-44CCA2AF5EFA}";
        public const string EV_ReplaceCard = "{1A442559-6AC8-459A-933A-F904993A9B80}";
        public const string EV_DeActivateCard = "{7E48566A-E33D-4383-9F91-4FA0171B6CB7}";
        public const string EV_DeActivateCard_Details = "{137A93AD-3E60-4B25-809D-8FAA0F7DF495}";
        public const string EV_DeActivateCard_Review = "{98640785-F56F-407B-A473-40A7EBE25357}";
        public const string EV_DeActivateLandingPage = "{DAF142BC-66C2-4342-82FB-A61CFFC8B233}";
        public const string EV_ReplaceLandingPage = "{012C8A3F-C41C-4C5C-86FA-0348E883B5F2}";
        public const string EV_TransactionListPage = "{E1FDA2A0-E06B-4250-BD34-A0AD41BD4BBE}";
        public const string EV_ApplyCardLandingPage = "{605F79C9-7FAD-4BAC-87F8-BB47764986EE}";
        public const string EV_NonDewaCustomerLandingpage = "{30DF2B5A-8EDD-4F31-A1B5-C9562C4B9C0C}";
        public const string EV_IndividualApplyCard = "{98E4728E-A3DC-4E27-B4DC-3694B4AA17F8}";
        public const string EV_EnquiryPage = "{13C1D0BD-D6D4-4AF0-A407-8EC634E3CCF0}";
        public const string EV_CardStatusPage = "{6BE20A91-FF66-4C7D-8636-149288CE7C67}";
        public const string EV_Enquiry_Sent = "{35FC8385-1FD5-4172-823D-D9DD0CEFB395}";
        public const string EV_Notifications_Filter = "{8076E8BD-3DCC-45D4-9B74-4F90FA3CDF15}";
        public const string EV_Notifications_Filter_V1 = "{87137C49-87E5-4145-8E6B-DDF25CFD0671}";
        public const string EV_Notifications_Config = "{524FC5FF-E894-43EB-88E8-8A88CBCE607A}";
        public const string EV_Issuing_Authorities = "{69884738-3A2F-4CB4-96E9-E09B2676FADE}";
        public const string EV_SDPayment = "{CF336811-34EB-4B50-8919-877E2253395D}";

        public const string EV_Supporting_doc = "{9E9D7169-4C96-468C-B4BC-3D8ADCF54071}";
        #endregion

        #region Miscellaneous Project
        public const string METER_TESTING_PROJECT = "{9DFB29FE-E94A-480A-AEF3-E20149E45F48}";
        public const string METER_TESTING_NEW_CONNECTION = "{CB8CD00C-F4AF-4627-891B-B47F77FF12BB}";
        public const string OIL_TESTING = "{097C3AA4-515E-4E2D-834C-124C2E524EA1}";
        public const string DEMINERALIZED_WATER = "{0BD29A4F-9822-4AF8-84D6-09BD0C30B23B}";

        public const string MISCELLANEOUS_LOGIN = "{904E5FA0-EA6F-4651-B260-7874A36B6464}";
        public const string MISCELLANEOUS_USER_REGISTRATION = "{F6197A77-E985-4D32-9022-E52BE4E73A44}";
        public const string MISCELLANEOUS_SERVICE_LIST = "{9999BE54-2F45-47D6-AFE9-3E328DB2CFE1}";
        public const string MISCELLANEOUS_DASHBOARD = "{9C1FFF43-C823-4736-9978-82EE0C587AAE}";
        public const string JOINTER_TESTING = "{7DC9FA6C-FF20-4584-B29B-1F94AA5B5EC6}";       
        public const string TESTING_SERVICES = "{1EAD68FC-3C98-4AAE-8B8B-7A002D435A92}";
        public const string JOINTER_CERTIFICATION = "{682682E1-C794-426B-ACA8-83FCDF41F241}";

        public const string METER_TESTING_PROJECT_SUCCESS = "{2AEBF705-8CF8-4745-AD31-54026E6DC0D9}";
        public const string METER_TESTING_NEW_CONNECTION_SUCCESS = "{6AA3FD40-25F0-4FDA-914F-0BDAD893884F}";
        public const string OIL_TESTING_SUCCESS = "{85A24CBF-3B9F-4663-A074-1CC00327715C}";
        public const string DEMINERALIZED_WATER_SUCCESS = "{4B366C9A-C60A-4F13-AD03-4C37D2DB2DD4}";

        public const string JOINTER_TESTING_SUCCESS = "{B3B3EBF2-678C-4DAE-8D64-179BB3C4F56D}";
        #endregion

        public const string MOVETO_TENANT_DETAILS = "{ADE5A956-4598-4EFE-B5C7-437198C9D207}";
        public const string J15_PAY_A_BILL = "{1D585AE1-7423-41E0-B264-E3FE167E6A41}";
        public const string J15_BILL_ENQUIRY = "{106201F3-EE94-4E8F-9E5E-D3056FDF2455}";
        public const string J87_LOGIN_PAGE = "{C103D096-7DF9-4BCA-B974-B9FEFE040C0F}";

        public const string J87_DOCUMENTUPLOADPAGE = "{D2F6B6B0-16C7-4E97-9898-726A0A8EF35B}";
        public const string J87_DOCUMENTSUCCESSPAGE = "{151E6154-D781-4D5F-A5C1-A3F3DC20A3C6}";
        public const string J87_INBOXPAGE = "{AC6443B2-9E10-43D0-BCDB-02D6D40D722E}";
        public const string J87_OUTBOXPAGE = "{06F395A3-A58C-4CCE-9531-DA611291AC18}";
        public const string LATESTUPDATESPAGE = "{D20EA8C9-2E8A-4040-8166-775C928EF27A}";
        public const string OUTAGEHOMEPAGE = "{2FF1BB88-23B4-4970-961A-7B8EEABBE0E7}";
        public const string OUTAGECONFIG = "{9EFA86A8-0049-40A3-9F95-DD18A805CC37}";
        public const string WEBSERVICEOUTAGEPAGE = "{623A2737-36FE-48C7-AF37-D9C12C65C8A5}";
        public const string J86_CHANGEPASSWORD_PAGE = "{5CE8A702-B1BC-4E9A-A3B5-829185AA46E3}";
        public const string J87_CHANGEPASSWORD_PAGE = "{E1DF37F7-7C03-4F24-816F-C03388715F9F}";
        public const string J86_LOGOUT_PAGE = "{77E50C32-8FBF-4E81-BDFD-994F5868629A}";
        public const string J87_LOGOUT_PAGE = "{14702845-FCDB-49D3-8ECE-39E780E25D27}";
        public const string J86_EMAILTEMPlATE = "{D92F2317-C290-4F79-9C54-93829EACFE49}";
        public const string J86_EMAILSUBJECT = "{E2C9E200-8B92-4B0D-B638-A9817767560A}";

        public const string J87_EMAILTEMPlATE = "{281CF0EE-137D-41BA-90B7-7D970634C760}";
        public const string J100_CONNECTION_ENQUIRY = "{51D6AF21-F887-443A-BE36-CED24E3047B2}";
        public const string J100_CONNECTION_ENQUIRY_REQUEST_SENT = "{FBCBE536-C826-4457-AC5D-95DEA5825739}";
        public const string PUBLISHING_STATISTICS = "{E658366B-59C9-463A-9654-7F791CA1EE33}";
        public const string COLLECTIVE_STATEMENT = "{7BAA1770-B90A-4394-98F1-039B308455CF}";
        public const string COLLECTIVE_STATEMENT_FAILED = "{E93BE877-C1BE-48A8-8620-33F790884AFC}";
        public const string HAPPINESS_SURVEY = "{7FB5E8AE-94F0-4269-B48F-D32EAB4338E4}";
        public const string HAPPINESS_SURVEY_REQUEST_SENT = "{22015FFF-9C9E-4A26-BD44-4D22343204CC}";
        public const string HAPPINESS_SURVEY_REQUEST_FAILED = "{DC49E9F9-D066-4803-9AB9-1D48BFD24E85}";
        public const string TERMS_AND_CONDITIONS = "{FB8A516F-34B2-4A76-91C0-028046444396}";
        public const string TERMS_AND_CONDITIONS_TEXT = "{5E7A5319-C396-4E8D-8D6A-65CEB21C97F7}";
        public const string TERMS_AND_CONDITIONS_TEXT_PLAIN = "{59D5DC34-6284-4786-B7C1-46B5618F6C26}";
        public const string SOLAR_CALCULATOR_ACCOUNTS = "{08DF0C38-7887-4670-AB2C-409F715A0C5D}";
        public const string SOLAR_CALCULATOR_MAP = "{00B0400B-3D2E-45E6-8F8E-ECA1F9709762}";
        public const string UPDATE_PRIMARY_CONTACT = "{668732EA-AC6E-42A6-BC64-AA1C9C9CD800}";
        public const string UPDATE_PRIMARY_CONTACT_SUCCESS = "{4B255CFD-57E8-4C89-9B14-3824A1F7EA2A}";
        public const string MOVE_OUT = "{26D619FD-89EE-43E1-B1C7-5884D1D4B834}";
        public const string MOVE_TO = "";

        #region Location
        public const string LocationJson = "{7182C7E4-00EA-4EFA-AF3A-6C0406EA01B9}";
        #endregion

        public const string GLOBAL_Top_Search = "{6AA7807D-86AB-45A4-BCDA-F018D7447BBC}";

        public const string IMAGE_GALLERY = "{CC59B1D6-C97A-4716-B415-F57CB02ABD77}";
        public const string VIDEO_GALLERY = "{BD93AF49-23C1-40DD-87D7-B140D7C4C9F4}";
        public const string PaymentRedirect = "{6139179D-AE6E-4A12-9929-7848EE41532E}";
        public const string NotificationId = "{DC02062E-B3E2-446A-8CA4-FE009DAC6F68}";
        public const string RammasConfigID = "{26DCE97E-5B85-4145-BB03-7997F2430F9A}";
        public const string BottomNotificationConfigId = "{D5901F67-63B9-44AD-BD12-52FB0E5DC591}";
        public const string WebsiteSurveyConfigId = "{AC03C708-AB21-4026-9E20-45DE6EF59096}";
        public const string DEWASiteMinificationConfigId = "{639EE1A1-E5C6-4E84-8C8E-FD9DF592C908}";
        #region VAT config
        public const string VAT = "{5911BB28-A09A-43AA-A54E-6895D4D3EF9D}";
        #endregion
        public const string HappinessConfig = "{E029A61E-7B39-4C8E-A65A-258D778A5A39}";

        public const string Bill_Download_Message = "{596C848B-F7C8-49F8-9EA4-EECEFB4431BD}";

        public const string TransactionConfig = "{CB3F7D13-0409-4BF9-803E-966B2FDE5A2C}";

        #region SupportKiosk
        public const string Kiosk_Service_Success = "{4BC5C551-EB42-4EE9-B812-9DB7B8C9CCBE}";
        public const string Kiosk_Service_Failed = "{6375C3BE-4E5F-4BEE-9459-CEECEE0345A6}";
        public const string Kiosk_HomePage = "{C33B00B4-C971-47D9-8724-60A3445DC07E}";
        public const string Kiosk_Configuration = "{E77C3151-1C33-4DCB-A7D7-30CF5F3494DE}";
        #endregion

        #region J100 Dewa Map
        public const string J110ServiceList = "{3B0DB280-1B1A-49C7-9F2F-53AAD387057A}";
        public const string J110LocationList = "{D225532F-CB02-4EFB-8D37-E14374A55793}";
        #endregion

        #region DEWAACADEMY

        public const string DEWAACADEMY_LOGIN = "{103FC616-5DBE-4A3A-BC59-89E16DE0E002}";
        public const string DEWAACADEMY_DASHBOARD = "{C8C0C3DC-6351-4B16-8D4B-0DB00C5960B0}";
        public const string DEWAACADEMY_SUCCESS = "{666C80EB-8657-428F-80AF-70737F31B6CE}";
        public const string DEWAACADEMY_REGISTRATIONS = "{B75F1B15-F7EB-4323-BCC6-06D5181641D1}";
        public const string DEWAACADEMY_LOGOUT = "{B75F1B15-F7EB-4323-BCC6-06D5181641D1}";

        #endregion

        #region MoveOut
        public const string MOVEOUT_DISCONNECTIONTIME_CONFIG = "{9A73D1D2-FF89-474E-A301-7A63E6A792F5}";
        #endregion
        #region MoveOutAnonymous

        public const string MOVEOUTANONYMOUS_DETAILS_OTP = "{6E9D95D5-6170-42FE-AB82-8CA4457B81C3}";
        public const string MOVEOUTANONYMOUS_VERIFICATION_CODE_OTP = "{164B2AF2-E439-4FB1-921D-50B40A047697}";
        public const string MOVEOUTANONYMOUS_MOVEOUT = "{5DD6C225-7317-481F-80AF-3ABEFC3CF5C3}";
        public const string MOVEOUTANONYMOUS_MOVEOUT_DETAIL = "{5DD6C225-7317-481F-80AF-3ABEFC3CF5C3}";
        public const string MOVEOUTANONYMOUS_MOVEOUT_SUBMIT_REQUEST = "{C75E74DC-340F-4D37-AA1D-91DA79BD6435}";
        public const string MOVEOUTANONYMOUS_INITIATE = "{9709387A-BA9A-47D4-B77B-05C1D78C35BF}";
        public const string MOVEOUTANONYMOUS_PAYMENT = "{A366F2F8-6CFE-4A1A-9D77-801B83B5BCC8}";
        public const string MOVEOUTANONYMOUS_REVIEW = "{A69CE0BF-065D-4BE0-B0DC-35CC33C0CA07}";
        #endregion

        #region EasyPay
        public const string EasyPay_Enquire = "{46FA36F5-75D2-47AD-B0EE-2F0769A7F2C8}";
        public const string EasyPay_Details = "{5C745309-B885-41F4-A23D-11A55798F221}";
        public const string Methodofbillpayment = "{F2887E58-C84E-4AA9-AC43-40F715160F7C}";
        #endregion

        #region ePass Service

        public const string EPASS_LOGIN = "{1CD256A2-AFC8-405C-BD65-A91B0250138F}";
        public const string EPASS_PERMANENT_PASS = "{4EEBC2FE-5F12-46BB-A1E8-7F1CD8412FC9}";
        public const string EPASS_SHORTTERM_PASS = "{06238AF2-E454-4BD6-B059-2481CEB85544}";
        public const string EPASS_DASHBOARD = "{B329204C-AF30-47BD-B387-FFC3510F47B6}";
        public const string EPASS_SUBCONTRACTOR = "{82C1D85E-303B-4DA9-80C3-0A2469546DF8}";
        public const string EPASS_MULTIPASS_REVIEW = "{9297A89C-D70D-4A78-BDF7-F79E56356C32}";
        public const string EPASS_MULTIPASS_SHORTTERM_REVIEW = "{8FA26E68-A4BA-45CC-89EC-9DEA4D9F89C7}";
        public const string EPASS_CREATEACCOUNT = "{6677A4B6-8C24-42F0-A9CE-7C0A0E7C563C}";
        public const string EPASS_CREATEACCOUNT_SUCCESS = "{387B89F6-A61A-4A73-83C8-AD99C3C33972}";
        public const string EPASS_USERREGISTRATION = "{ACB6A013-8FE3-4E0D-8C7E-2D8B22CE0E76}";
        public const string EPASS_USERREGISTRATION_SUCCESS = "{B05896DB-BF77-4977-9F6C-9A880EB03EC9}";
        public const string EPASS_SHOWDETAILS = "{8E5848A6-AF40-4D95-A643-BDB7160BA06B}";
        public const string EPASS_SHOWPENDINGAPPROVALDETAILS = "{E85FB30F-3D65-4902-9139-3D4F4623BAB1}";
        public const string EPASS_SERVICES = "{47C5B959-BD55-4EEE-AC39-BB0FA80D4C41}";
        public const string EPASS_MYPASSESS = "{14EA5DCC-E571-4144-9930-30FF8D419A24}";
        public const string EPASS_SETNEWPASSWORD = "{85D058E0-BCEB-47A1-92E2-EE83E7E427DB}";
        public const string EPASS_ADMINLOGIN = "{5C66CD94-4FBA-4669-BBBE-36A544726E66}";
        public const string EPASS_ADMINDASHBOARD = "{26382987-A329-4CB7-8DE0-FDC81A51DAAD}";
        public const string EPASS_ALLPASSES = "{B2227C0E-480E-4755-A415-E573F942214C}";
        public const string EPASS_FORGOTUSERID = "{AA816A77-086D-4300-97C6-B9F223BB399D}";
        public const string EPASS_FORGOTPASSWORD = "{E46FFF47-3668-4F66-9407-6DB5A4A06F8B}";
        public const string EPASS_ADMINALLPASSESS = "{C5FB5052-5D77-4576-925F-36C23EAD0939}";
        public const string EPASS_ONEDAYINITIATOR = "{069F7D96-7739-4659-8481-F4447CECA86D}";
        public const string EPASS_ONEDAYVISITOR = "{43564849-4941-47D9-B6B7-AD3D8516EE06}";
        public const string EPASS_MYPROFILE = "{EAFBB4F3-3D98-405A-A54F-1A7F64924DA4}";
        public const string EPASS_BLOCKEDUSERS = "{E9708292-2A8C-407E-9F0D-E3763F14C5E0}";
        public const string Epass_VisitorPass = "{996CDEB0-BDA5-472F-9239-1FA59FA8B3FD}";

        public const string EPASS_GENERATION_LANDING_PAGE = "{0258B0C5-992F-4E49-A4A3-F9ACFAA8B361}";
        public const string EPASS_OTHERS_LANDING_PAGE = "{11268F44-4BE4-46AB-8B2E-801470AE55B2}";
        public const string EPASS_SUBPASS_LANDING_PAGE = "{5F670584-E876-42EE-946A-FF5B05F0B08E}";
        public const string EPASS_NIGHT_PASS = "{43A835F7-5867-48DB-B960-08BF13136699}";
        public const string EPASS_HOLIDAY_PASS = "{82B840E0-559C-4492-A1D5-49C5E7DE04BE}";
        public const string EPASS_DEVICE_PASS = "{FEEF785E-32F0-424B-8E5E-4C60440954FA}";
        public const string EPASS_MATERIAL_PASS = "{B24B4EBB-B76A-428F-AABC-47934C538FF4}";
        public const string EPASS_SUPPLIERGENERATIONLANDINGPAGE = "{4B215168-802F-4F58-9465-688A1DA8BC68}";
        public const string EPASS_SUPPLIERBSHRLANDINGPAGE = "{962BE131-B94F-474A-A1FA-F2565FC5562E}";
        public const string EPASS_SUPPLIERSELECTIONPAGE = "{97501ACC-3F88-427D-BF30-970F635BE6AD}";
        public const string EPASS_VISITORLANDINGPAGE = "{26483F65-3CC1-436C-82A7-2AA2115CF50E}";
        #endregion

        #region Internship and Training

        public const string INTERNSHIP_LANDING = "{625FE542-9D14-4D10-A255-285FD535A003}";
        public const string INTERNSHIP_RESEARCH = "{8D511F30-891A-4333-ACDD-65181F089E76}";
        public const string INTERNSHIP_APPLY_INTERNSHIP = "{CB867710-937F-4F6F-B63C-5162D957EC28}";
        public const string INTERNSHIP_APPLY_SUCCESS = "{C6547A62-B1FB-4A2E-AC54-70703FCCE5A9}";
        public const string INTERNSHIP_SUMMER_TRAINING = "{098FB73C-637F-4767-9629-AE15BDCF2D2E}";

        #endregion

        #region [CAREER FAIR]
        public const string CAREER_FAIR_NORMAL_REGISTRATIOON_SUCCESS = "{B7ECE995-73B6-4C37-91C6-FB8D0150DED0}";
        #endregion

        #region [Estimation]
        public const string EstimateHistoryPage = "{95F94B87-4A5D-41ED-9C15-E535FA8C09EB}";
        public const string EstimatePaymentDetail = "{B4BBC129-5E10-4C38-9272-C98F8F0118E3}";
        public const string EstimateReasonCode = "{6C49F8D9-EB08-4088-8635-D2A585711A4D}";
        public const string EstimateRefundMode = "{EB631DC5-4949-40BC-B256-5FF76E6B111B}";
        public const string EstimateRefundPage = "{0D6260D9-F399-4560-9B2C-CC6C7DF6F303}";
        public const string EstimateRefundConfrimationPage = "{31CE940C-0D47-4F5C-A245-6A31AC1E9888}";
        #endregion

        #region [corporate portal]
        public const string CorporatePortalDashboard = "{9EF69FD4-4461-4516-90C5-780E0C7EC6BD}";
        public const string CorporatePortalLogin = "{EA52A46D-1F66-4A4D-B799-FBDE1049A002}";
        public const string CorporatePortalJointServices = "{72419157-1D20-4C0D-B752-7A96DAEB6CC4}";
        public const string CorporatePortalProjectandInitiatives = "{075941CB-23F2-409C-9B13-7B34FEE97E17}";
        public const string CorporatePortalapproveddocuments = "{8103B439-5996-42AB-A31E-D485E0594F3B}";
        public const string CorporatePortalMessageDetails = "{307A7350-9473-4702-9D44-40B55CCDBB8D}";
        public const string CorporatePortalmyInbox = "{FB29A241-2BF8-4FF8-82CE-4A575C8F4142}";
        public const string CorporatePortalsubmittedrequests = "{7D9D4F81-267D-4FD8-985C-FD78D6C80353}";
        public const string CorporatePortalsubmittedideas = "{BB96FB63-AFED-4F45-B160-25C66A704627}";
        public const string CorporatePortalReportedIssues = "{441D94C1-E660-41FC-823F-355B8F7A7328}";
        public const string CorporatePortal_CONFIG = "{DE425E3F-306B-4A8F-A439-288CD0FA7BE6}";
        #endregion

        #region [Raffle]
        public const string RaffleSubmissionpage = "{A7FCA632-652B-43A3-B9E2-855418BCC302}";
        #endregion

        #region MyRegion

        public const string JOINTOWNERSUCESS = "{7E881830-8784-460F-91B3-0601B914FF5C}";
        public const string JOINTOWNERPAGE = "{8DF2DF1C-5255-4A0B-A31B-05B27401778B}";

        #endregion
        public const string InterruptionService = "{DC306F8C-F462-43B3-9F90-2CC7EA5E248D}";

        #region  [Redirect Template]
        public const string RedirectItemTemplate = "{954B7389-A0CF-4512-BFB3-6D6DF1BF6003}";
        #endregion

        #region [Global Config]
        public const string SITEMAP_GENERATION_CONFIG = "{C464DDCD-0663-4825-A337-7EF74283A819}";
        #endregion

        #region [Conservation]
        public const string CONSERVATION_EDUCATIONALINSTITUTION_SUCCESSPAGE = "{88B25F64-6078-4A1D-840D-D69F5BF78B3B}";
        public const string CONSERVATION_LEADER_SUCCESSPAGE = "{88F19653-5CE7-49D5-904D-DCF4A2EC1FC5}";
        public const string CONSERVATION_TEAM_SUCCESSPAGE = "{6E0D550C-EC2B-4B4A-AE7E-EE7926DA7123}";
        public const string CONSERVATION_PROJECT_SUCCESSPAGE = "{68957776-5784-4A8F-BDD1-4F543A4B125C}";
        public const string CONSERVATION_LECTURE_SUCCESSPAGE = "{7A96FAB3-0E95-4405-B6A8-94267B6677EC}";
        #endregion

        #region [ScrapeSale]
        public const string SCRAPESALE_INTROPAGE = "{42474A40-44ED-439A-AEF4-5594A3C43159}";
        public const string SCRAPESALE_PORTAL_LOGINPAGE = "{2675D1E4-E221-470B-A5D2-3AFF6C1FAA24}";
        public const string SCRAPESALE_PORTAL_DASHBOARD = "{76B78E62-3DFE-45BA-993C-2D679086EBF8}";
        public const string SCRAPESALE_RECOVERY_EMAIL_SENT = "{32D69BBE-6E4E-4C5C-98E3-29629E0B3933}";
        public const string SCRAPESALE_FORGET_USERNAME = "{B264288E-7743-4A40-BABB-F2FBA76FBAC4}";
        public const string SCRAPESALE_FORGET_PASSWORD = "{B9C63B3B-5887-455A-8622-5C1680DBC2FF}";
        public const string SCRAPESALE_USER_REGISTRATION = "{72BCAA79-AEB1-44C6-A88A-2D79DC3FEF8A}";

        public const string SCRAPESALE_AccountRegistrationStep2 = "{15C71598-3253-44EE-BCB7-4B6F470AE9AB}";
        public const string SCRAPESALE_AccountRegistrationStep3 = "{95B013CF-DECC-479E-A0BD-119C638F0FC8}";
        public const string SCRAPESALE_AccountRegistrationStep4 = "{3F1AB099-400F-44D1-BF5D-A1B86668FE32}";
        public const string SCRAPESALE_AccountRegistrationSuccess = "{DB374BBB-185B-4283-A6C8-C4A332D6B647}";

        public const string SCRAPESALE_OPEN_TENDER_PUCHASE = "{4BB22F37-3AF3-47D7-B5BD-870DDA451E54}";
        public const string SCRAPESALE_TENDER_PUCHASE = "{0CF308AC-7DF1-490F-B517-0B0D924A18F7}";
        public const string SCRAPESALE_CHANGE_PASSWORD = "{59C45682-7051-4D34-AD4E-879BFA242E82}";
        public const string SCRAPESALE_TRACKAPPLICATION = "{2BC6042F-9FBD-42C3-AF23-DF95612F7066}";

        public const string SCRAPESALE_BANKLIST = "{B459E1A4-8CEE-4E53-B31D-6AF9DD494BA8}";


        #region Phase-2
        public const string SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP1 = "{ED03106C-5A16-4322-BD96-1BE98141C328}";
        public const string SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP2 = "{514C3E36-A5A5-493A-9921-76640071CC26}";
        public const string SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP3 = "{7E8B6165-B657-4069-A738-421D6AD4D3AB}";
        public const string SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP5 = "{5D9C64E5-C0FA-4A50-A0D3-36A5D8790740}";
        public const string SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP6 = "{F34088A3-7AF6-4A19-A763-67981EE5D086}";
        public const string SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP7 = "{CD7DED9E-67D9-41F5-9C97-39584974C630}";
        public const string SCRAPESALE_TENDER_BIDDING_PUCHASE = "{5260880F-C1CE-450B-B9FB-55D0C010EF82}";
        public const string SCRAPESALE_TENDER_BIDDING_SUCCESS = "{52014962-A350-41A2-963B-9147308150D1}";
        public const string SCRAPESALE_TENDER_OPENING_RESULTS_LIST = "{2C0A0A32-65B5-4A6C-918C-542E5E129B7D}";
        public const string SCRAPESALE_TENDER_OPENING_RESULTS = "{53AF2551-D233-406A-BA65-9F98E7505625}";

        // Sales Order
        public const string SCRAPESALE_SALESORDER_PAYMENT = "{E1F888F0-5E52-4826-8B0B-22F56A83E470}";

        public const string SCRAPESALE_SALESORDER_DASHBOARD = "{B7BDCF2E-3E6F-415C-A339-267D73C4D7C7}";

        // Update Profile
        public const string SCRAPESALE_PROFILEUPDATE_SUCCESS = "{DE621DFA-67E6-41CA-B0A2-AB37ABF19260}";

        public const string SCRAPESALE_PROFILEUPDATE = "{9C1FFF43-C823-4736-9978-82EE0C587AAE}";
        #endregion
        #endregion
        #region [Manage Account Information]
        public const string myprofile = "{0C5FB144-897A-4B82-92D7-B6D76B69A805}";
        public const string customerProfile = "{DC919FDC-708B-4B87-AB29-2E0A0FB83101}";

        public const string verifymobilenumber_page = "{DA58A2C6-1264-4B17-A332-4A379A5232F3}";
        #endregion

        #region
        public const string WORKPERMIT_PAGE = "{4CA3DFB8-0159-4C04-A189-E5DDB0F76F0F}";
        public const string WP_ADDSUBCONTRACTOR = "{FE88A8A0-35BF-4902-BD7A-D74B59517B0D}";
        public const string WP_SUBCONTRACTORLIST = "{460DC39D-DECF-46E1-80CD-FEF644D240E3}";
        public const string WP_PASSRENEWAL = "{AACD4C91-9386-488E-960F-DB662281565C}";
        public const string WP_RENEWAL_REVIEW_PAGE = "{7CAF651C-FE29-4021-80F8-A360CD7596FE}";
        public const string WORKPERMIT_REVIEW_PAGE = "{A06A113E-F273-422C-9536-067D8E1EC578}";
        #endregion

        #region DEWA Store
        public const string DewaStoreConfig = "{C0D61AEC-9F2C-4667-ADE9-D73A63D147EF}";
        #endregion

        #region Smart Living
        public const string MYSAVINGSTIPSPAGE = "{720BD26D-B1F4-459E-AFD6-528A19C8A329}";
        public const string DEWASTOREPAGE = "{8C75620E-0ABB-4EE7-8637-3244240FAF60}";
        public const string SMARTRESPONSEPAGE = "{A49C0E34-B67F-4644-BEFE-CC1F60A563FA}";
        #endregion
        #region [DTMC]
        public const string DTMC_SURVEY_DATALIST_SCPATH = "{5555738E-B822-4784-9568-E045EFE7D8BB}";
        public const string DTMC_AwayMode_Frequency_List = "{6C14CCDD-9CD3-48A2-9850-1A4DBF97B2C9}";
        public const string DTMC_LOGGEDIN_AWAYMODE_CREATE = "{25901C4F-9FE2-45ED-98F0-4AB19F3EA64F}";
        public const string SMART_LIVING_PAGE = "{15CCBD5E-8A7A-487E-9019-F4C5299B3BDF}";
        public const string DTMC_TRACKING_MAP = "{376ED6CA-67AD-4FBB-9DA0-A85699EB3C56}";
        public const string ColorList = "{2735E4E6-49B5-4885-9417-BE15DA45F889}";
        public const string DTMC_ConsumptionSlabData = "{25999FD3-018C-46E4-B74D-696236364EF2}";
        #endregion

        #region Owner Tracking
        public const string OWNERTRACKING_DASHBOARD = "{6FB266E0-4B37-4A55-9DF5-B93051225409}";
        public const string OWNERTRACKING_APPLICATION_DETAILS = "{A7A50F89-501A-44ED-9FCE-F41AB87D8323}";
        #endregion

        #region [GeneralTempalteId]
        public const string T2ArticalPageTemplateID = "{A72A1FCC-CDB9-4949-842F-A8001075A7EA}";
        #endregion

        #region Shams Dubai Training
        public const string SHAMS_SOLAR_PV_DISPLAYING = "{2D729F54-DEAB-4FBD-AE89-D81A83147D2E}";
        public const string SHAMS_BOOK_TRAINING = "{45641BBE-A6B1-46C2-8834-8354A96B676B}";
        public const string SHAMS_BOOK_TRAINING_SUCCESS = "{5BC4F4DE-54F6-4E11-BC15-52A5C7280F5D}";
        public const string SHAMS_MEDIA_ITEM = "{897DCE58-F0FB-4E16-8416-20FEF1FC72E0}";

        #endregion

        #region  [Smart Communications]
        public const string SMARTCOMMUNICATIONS_CONFIG = "{7E01D93D-6A9D-46B2-BDE1-25FD4E0E1C88}";
        public const string UNIVERSAL_SERVICE_CENTRE = "{E402441F-0206-45C8-9C13-00586AD0EEE4}";
        #endregion

        #region Sponsorship form
        public const string SPONSORSHIP_REQUEST = "{36AF93B2-6E07-4161-A646-02AA08C2C077}";
        public const string SPONSORSHIP_SUCCESS = "{1E6E2C65-B738-408A-B1FC-BCE270D9B067}";
        #endregion

        #region Agile Governance Global Summit
        public const string AgileGovtGlobalSummit = "{6EA0DBC1-5661-4FC5-A0AC-11C7FC44931A}";//"{6BC9D198-382C-4AEB-874D-6C6B89F8DE7A}";
        public const string AgileGovtGlobalSummitRegistration = "{861CC07F-BC62-49D9-AD4A-5DD28B9232CC}";
        public const string AgileGovtGlobalSummit_Verify = "{CCE8B705-3754-45F4-A069-5CA4E8ABF7AC}";
        public const string AgileGovtGlobalSummitRegistrationSuccess = "{C4CFB332-F2FF-42CD-9C80-F2F3838BE1D9}";
        public const string AgileGovtGlobalSummit_VerifyEmail = "{DA968244-C1CF-4731-80ED-6DB9EABBEA3E}";
        public const string AgileGovtGlobalSummit_ConfirmEmail = "{FA3C25CE-2FC9-4C67-90E6-175DF68FB187}";
        public const string AgileGovtGlobalSummit_EventCodeEmail = "{2D2514D6-95CC-475F-8038-9174F0BC54FE}";
        #endregion

        #region POD Event Registration
        public const string PODEventRegistration_VerifyEmail = "{C4F97F10-B0E0-4921-993C-7A2616CF0E89}";
        public const string PODEventRegistration_Success = "{61A67285-F457-4B29-8042-D2AAA7375315}";
        public const string PODEventRegistration = "{DAAA7B83-A347-4C56-B1AC-EBA5D3B9E443}";
        #endregion

        #region Promotional Modal popup
        public const string PromotionalModalPopup = "{A5C95236-4E7C-4BB7-B4C1-8BCA517B02BB}";//"{6BC9D198-382C-4AEB-874D-6C6B89F8DE7A}";
        #endregion

        #region Track Request Anonymouss
        public const string TRACK_REQUEST_ANONYMOUS = "{F82DE1B8-DF45-451B-BC9B-F29A3BFA73DA}";
        public const string TRACK_REQUEST_ANONYMOUS_DETAIL = "{BC38DB51-0B35-4A3A-8087-35CA0094EF8E}";
        public const string TRACK_REQUEST_ANONYMOUS_LIST = "{AFB915F6-A725-436C-A4CE-FE4846FF8494}";
        public const string TRACK_REQUEST_VIEW_STATUS = "{9DCCF1F4-BF4F-4AA3-AC38-39B3B175E51C}";
        #endregion

        #region Expo2020
        public const string Expo2020_Success = "{8CD69A48-D3C3-4302-90AD-00B059D9E42D}";
        public const string Expo2020_Inquiries = "{934687B5-2D50-4155-85E5-06DE9AD92657}";
        #endregion

        #region [power outage]
        public const string PowerOutageRequestConfirmationPage = "{748BC8CA-C68A-4204-9A42-7DB420DBD52B}";
        public const string PowerOutageTrackingPage = "{3298F7C3-46AD-423B-93E8-971467BA4E5F}";
        public const string PowerOutageRequestForm = "{02C54D1A-15B7-4318-9BCB-A576DD7E37CB}";
        #endregion
        #region EV Smart Charger
        public const string EVGREEN_VIEWLOCATIONS = "{1E2E1399-610C-47D1-B0CA-A89B9A8B3BC4}";
        public const string EVGREEN_DETAILS_PAGE = "{0A8A6A46-7E9B-4E09-88D8-E564FDCC4D31}";
        public const string EVGREEN_GENERAL_DETAILS_PAGE = "{E4536009-4FEA-4F44-B723-F264C00F006A}";
        public const string EVGREEN_VERIFY_EMAIL_PAGE = "{AF594F7B-E79B-4078-AC77-929F8CFACE9A}";
        public const string EVGREEN_CHARGING_TIMING_PAGE = "{CAE9628A-F21B-4CFC-A180-6E61F759EC6A}";
        public const string EVGREEN_PAYMENT_CONFIRMATION_PAGE = "{B9B1A275-EB6F-4014-91D4-8735EBC818E7}";
        public const string EVGREEN_EVCHARGING_PAGE = "{4E818B3F-4E84-44A6-904B-101E91352576}";
        #endregion

        public const string EW_SUCCESS_PAGE = "{63A80243-543D-456B-9432-5CA2893B5510}";
        #region Infrastructure NOC
        public const string INFRASTRUCTURE_NOC_SUCCESS = "{32AADD80-06CB-49AD-B988-74EE5A0457A7}";
        public const string INFRASTRUCTURE_NOC_REQUEST = "{81769DC0-AFDD-476E-AD79-4743E4D0F0A9}";
        public const string INFRASTRUCTURE_NOC_VIEW_REQUEST = "{8602519D-DA78-41B2-8333-80A8081EAE81}";
        public const string INFRASTRUCTURE_NOC_STATUS = "{E96FA79D-D288-426C-AFE1-B0B85A9016B9}";
        public const string INFRASTRUCTURE_NOC_LANDING_PAGE = "{FB7A7E47-4556-4951-86FB-E560CECC2E3C}";
        #endregion

        #region Selef Energy Assessment Survey

        public const string J54_SURVEY_PAGE = "{D5D5169B-E3BD-450A-AC2E-1798F8362F95}";
        public const string J54_LANDING_PAGE = "{CA9BA79A-FE43-4FB7-A549-6155CAD1CA72}";

        #endregion

        #region HH Villa
        public const string HH_VILLA_DASHBOARD = "{CD2176C7-8C63-489E-8DFC-43F6B2722FF3}";
        public const string HH_VILLA_NEWAPPLICATION = "{8B470E49-8BAC-4495-86A2-42240B982720}";
        public const string HH_VILLA_EDITAPPLICATION = "{85A8EC9E-FCAE-48ED-BB3A-1C4C43EDD2A1}";
        public const string HH_VILLA_EDITAPPLICATION_STEP1 = "{2A18D635-6998-488A-9825-1956575C2A06}";
        public const string HH_VILLA_EDITAPPLICATION_STEP2 = "{C5FA695F-DB14-4FFF-9521-78BAE9DC6FF5}";
        public const string HH_VILLA_EDITAPPLICATION_STEP3 = "{BC0F6199-7AA5-4942-8420-3D2B0D2432E2}";
        public const string HH_VILLA_VIEWAPPLICATION = "{6EC47613-0F46-42A6-823A-1E34FEFD4060}";
        #endregion

        public const string HAYAK_CONFIG = "{6D1AEA83-9A51-447F-9296-D0629495258E}";
        public const string ConsumerGuide = "{602DAD39-F2AC-4001-AF47-ECFB34DA0753}";

        #region AMI Graph
        public const string AMIGraphConfig = "{8DA79FED-F693-4F02-A2F3-B15D5632882B}";
        #endregion

        public const string Payment_Confirmation = "{4727D2DB-6A0F-4D5C-924F-08588494F9EE}";
        public const string Payment_Pending = "{4C6FEAEA-887E-46AE-ACD1-CCBE177EBBD5}";

        #region Work Placement Evaluation
        public const string INTERNSHIP_SURVEY_PAGE = "{21D1807D-251C-4386-849A-AE43738E956C}";
        public const string INTERNSHIP_SURVEY_OTPVERIFICATION_PAGE = "{82B0AC7B-F1E4-45A3-B71E-AF27160C74FE}";
        public const string INTERNSHIP_SURVEY_OTPSEND_PAGE = "{FB726CE7-47C8-486D-9193-BC215698F6F2}";
        #endregion

    }

    public static class QueryStringKeys
    {
        public const string ReraMovenInEncryptedNumbers = "dynbn";
    }

    public static class GenericConstants
    {
        public const string MoveOutIbanPrefix = "AE";

        public const string AntiHijackCookieName = "__DEWAAUTH__";
        public const string PROFILE_SESSION_KEY = "__DEWAPROFILE__";
        public const string LABOURCAMPPREMISETYPE = "K005";
        public const string HAPPINESS_SURVEY_FIRST_QUESTION = "0001";
        public const string TRANSACTIONAL_CHM_CODE = "CSSE";
        public const string DEWA_WEBSITE_ASSEMBLY = "DEWAXP.Foundation.Content";
        public const string DSCE_WEBSITE_ASSEMBLY = "DSCE.Website";
        public const string SUQIA_WEBSITE_ASSEMBLY = "Suqia.Web";
        public const string _conversation_key = "C111";
    }

    public static class General
    {
        public const string Max_Attachment_Size = "{61FBE8B3-D1C8-4D1F-9A63-EAD9BE23FCC2}";
        public const string Accepted_File_Types = "{94AA5427-590B-47E0-B520-F721986845A3}";
        public const string Accepted_File_Types_client = "{F2E3449A-2D5B-4B10-BB9C-8D659CC8B432}";
        public static readonly string[] AcceptedFileTypes = { ".PDF", ".DOC", ".JPG", ".JPEG", ".PNG", ".BMP", ".GIF", ".XLS", ".DOCX", ".XLSX", ".CSV" };
        public static readonly string[] AcceptedImageFileTypes = { ".JPG", ".JPEG", ".PNG", ".BMP", ".GIF" };
        public static readonly string[] AcceptedMasarFileTypes = { ".PDF", ".JPEG", ".JPG", ".PNG" };

        public const int MaxAttachmentSize = 2000000;
        public const int MaxPGPAttachmentSize = 26214400;
        public const string AcceptedTerms = "X";
    }

    public static class PaymentConstants
    {
        public static decimal MaximumAllowedPaymentAmount
        {
            get { return 999999999m; }
        }
        public static decimal MinimumAllowedPaymendAmount => 200000m;
        public static decimal EasyMaximumAllowedPaymentAmount => 500000m;

        public static decimal EstimateMaximumAllowedPaymentAmount
        {
            get { return 50000m; }
        }

        public static decimal MaximumAllowedSmartWalletPaymentAmount
        {
            get { return 99999.99m; }
        }
    }

    public static class eForm
    {
        #region Eform Parameters Types
        public const string eForm_Text = "TextField";
        public const string eForm_CheckBox = "CheckField";
        public const string eForm_DateTime = "DateTimeField";
        public const string eForm_MultilineText = "MemoField";
        #endregion
    }

    public class AttachmentValidation
    {
        public static long maximumFileSize
        {
            get
            {
                Item maxAttachmentSize = SitecoreX.Database.GetItem(General.Max_Attachment_Size);
                long maxBytes = maxAttachmentSize != null && maxAttachmentSize.Fields["Text"] != null ? Convert.ToInt64(maxAttachmentSize.Fields["Text"].ToString()) : General.MaxAttachmentSize;
                return maxBytes;
            }
        }

        public static string acceptedFileTypesClientSide
        {
            get
            {
                Item acceptedFileTypes = SitecoreX.Database.GetItem(General.Accepted_File_Types_client);
                string acceptedFileType = acceptedFileTypes != null && acceptedFileTypes.Fields["Text"] != null ? acceptedFileTypes.Fields["Text"].ToString() : string.Empty;
                return acceptedFileType;
            }
        }
    }

    public class DRRGAttachment
    {
        public static long maximumFileSize
        {
            get
            {
                Item maxAttachmentSize = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                long maxBytes = maxAttachmentSize != null && maxAttachmentSize.Fields["Maxsize"] != null ? Convert.ToInt64(maxAttachmentSize.Fields["Maxsize"].ToString()) : General.MaxAttachmentSize;
                return maxBytes;
            }
        }

        public static string acceptedFileTypesClientSide
        {
            get
            {
                Item acceptedFileTypes = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                string acceptedFileType = acceptedFileTypes != null && acceptedFileTypes.Fields["file types"] != null ? acceptedFileTypes.Fields["file types"].ToString() : string.Empty;
                return acceptedFileType;
            }
        }
    }

    public class SitecoreRoutes
    {
        public const string DETACHED_LOGIN = "/account/login";
    }

    public class BPTypes
    {
        public const string PERSON = "Person";
        public const string ORGANISATION = "Organization";
    }

    public static class DataSources
    {
        public const string QUERY_TYPES = "/sitecore/content/Global References/List Data Sources/Query Types";

        public static readonly Dictionary<int, AccountClassification> AccountStatus = new Dictionary<int, AccountClassification>()
        {
            {00, AccountClassification.Active},
            {10, AccountClassification.Inactive},
            {15, AccountClassification.Inactive},
            {17, AccountClassification.Inactive},
            {18, AccountClassification.Inactive},
            {-1, AccountClassification.Unknown},
        };

        public const string DOWNLOAD_OPTIONS = "/sitecore/content/Global References/List Data Sources/Download Options";
        public const string TIME_SLOTS = "/sitecore/content/Global References/List Data Sources/Time Slots";
        public const string HAPPINESS_SERVICE_CODES = "/sitecore/content/Global References/List Data Sources/Happiness Service Codes";
        public const string MONTHS_LIST = "/sitecore/content/Global References/List Data Sources/Months List";
        public const string SHORT_WORD_MONTHS_LIST = "{438BDC25-DA7C-46CE-B5CE-DB561786FEEC}";
        public const string RESIDENCETYPELIST = "/sitecore/content/Global References/List Data Sources/Residence Type";
        public const string ACTYPELIST = "/sitecore/content/Global References/List Data Sources/AC Type";
        public const string TITLELIST = "/sitecore/content/Global References/List Data Sources/Title";
        public const string INDUSTRYLIST = "/sitecore/content/Global References/List Data Sources/Industry";
        public const string PURPOSES = "/sitecore/content/Global References/List Data Sources/Purposes";
        public const string UNSUBSCRIBE_REASON = "/sitecore/content/Global References/List Data Sources/Unsubscribe Reason";
        public const string REFUND_OPTIONS = "/sitecore/content/Global References/List Data Sources/MoveOut Refund Options";
        public const string UPDATEIBAN_OPTIONS = "/sitecore/content/Global References/List Data Sources/UpdateIBAN Options";
        public const string MOVE_IN_IDTYPES_RES = "/sitecore/content/Global References/List Data Sources/MoveIn ID Types Res";
        public const string MOVE_IN_IDTYPES_NON_RES = "/sitecore/content/Global References/List Data Sources/MoveIn ID Types NonRes";
        public const string MOVE_IN_CUST_TYPES_RES = "/sitecore/content/Global References/List Data Sources/Move In Customer Types Res";
        public const string MOVE_IN_CUST_TYPES_NON_RES = "/sitecore/content/Global References/List Data Sources/Move In Customer Types NonRes";
        public const string SEARCHCATEGORIES = "/sitecore/content/Global References/List Data Sources/Search Categories";
        public const string TAYSEER_ACCOUNT_LIMIT = "{6514654B-CEC8-47E9-A684-8A71A954ACC8}";
        public const string MAX_PAGE_SIZE_DATALIST = "/sitecore/content/Global References/List Data Sources/Other/search-results-maxpage-sizes";
        public const string EV_ENQUIRY_TYPES = "/sitecore/content/Global References/List Data Sources/EV Enquiry Types";

        #region Customer Profile
        public const string CUSTOMER_PROFILE_LANGUAGE = "/sitecore/content/Global References/List Data Sources/Customer Profile/Preferred Language";
        public const string CUSTOMER_PROFILE_TRADE_LICENSE_ISSUING_AUTHOR = "/sitecore/content/Global References/List Data Sources/Customer Profile/Trade License Issuing Authority";

        #endregion

        #region Career Portal
        public const string CAREER_PORTAL_REGISTRATION_TYPES = "/sitecore/content/Global References/List Data Sources/Career Portal/Registration Types";
        public const string CAREER_PORTAL_PageSize = "/sitecore/content/Global References/List Data Sources/Career Portal/Page Size";
        public const string CAREER_PORTAL_GccCountries = "/sitecore/content/Global References/List Data Sources/Career Portal/GCC Country";
        public const string CAREER_PORTAL_Gender = "/sitecore/content/Global References/List Data Sources/Career Portal/Gender";
        public const string CAREER_PORTAL_MaritalStatus = "/sitecore/content/Global References/List Data Sources/Career Portal/Marital Status";
        public const string CAREER_PORTAL_Experience_Types = "/sitecore/content/Global References/List Data Sources/Career Portal/Registrations/Experience";
        public const string CAREER_PORTAL_Nature_Types = "/sitecore/content/Global References/List Data Sources/Career Portal/Registrations/Nature of Work";

        #endregion

        #region MoveIn v3
        public const string MOVE_IN_CUST_TYPES = "/sitecore/content/Global References/List Data Sources/MoveIn/MoveIn Customer Types";
        public const string MOVE_IN_ACC_TYPES = "/sitecore/content/Global References/List Data Sources/MoveIn/MoveIn Acc Types";
        public const string MOVE_IN_OWNER_TYPES = "/sitecore/content/Global References/List Data Sources/MoveIn/MoveIn ID Owner Types";
        public const string MOVE_IN_ID_TYPES_RES = "/sitecore/content/Global References/List Data Sources/MoveIn/MoveIn Res ID Types";
        public const string MOVE_IN_ID_TYPES_NON_RES = "/sitecore/content/Global References/List Data Sources/MoveIn/MoveIn ID NonRes Types";

        #endregion

        #region DRRG
        public const string DRRG_NOMINALPOWERENTRIES = "/sitecore/content/Global References/List Data Sources/DRRG/Nominal Power Entry";
        public const string DRRG_CELLTECHNOLOGY = "/sitecore/content/Global References/List Data Sources/DRRG/Cell Technology";
        public const string DRRG_YESNO = "/sitecore/content/Global References/List Data Sources/DRRG/YESNO";
        public const string DRRG_FRONTSUPERSTRATE = "/sitecore/content/Global References/List Data Sources/DRRG/Front Superstrated";
        public const string DRRG_ENCAPSULANT = "/sitecore/content/Global References/List Data Sources/DRRG/Encapsulant";
        public const string DRRG_DC_SYSTEM_GROUPING = "/sitecore/content/Global References/List Data Sources/DRRG/DC System Grounding";
        public const string DRRG_POSITIONOFJUNCTIONBOX = "/sitecore/content/Global References/List Data Sources/DRRG/Position of Junction Box";
        public const string DRRG_MATERIALOFJUNCTIONBOX = "/sitecore/content/Global References/List Data Sources/DRRG/Material of Junction Box";
        public const string DRRG_TERMINATIONS = "/sitecore/content/Global References/List Data Sources/DRRG/Terminations";
        public const string DRRG_FEATURESOFJUNCTIONBOX = "/sitecore/content/Global References/List Data Sources/DRRG/Features of Junction Box";
        public const string DRRG_INTERFACEAPPLICATION = "/sitecore/content/Global References/List Data Sources/DRRG/Interface Application";
        public const string DRRG_SALT_MIST_TEST_METHODS = "/sitecore/content/Global References/List Data Sources/DRRG/Salt Mist Test Methods";
        public const string DRRG_PowerFactorRange = "/sitecore/content/Global References/List Data Sources/DRRG/Power Factor Range";
        public const string DRRG_DCACGalvanicIsolation = "/sitecore/content/Global References/List Data Sources/DRRG/DC AC Galvanic Isolation";
        public const string DRRG_PowerDerating = "/sitecore/content/Global References/List Data Sources/DRRG/Power Derating Voltage";
        public const string DRRG_PossibilityEarthing = "/sitecore/content/Global References/List Data Sources/DRRG/Possibility of Earthing";
        public const string DRRG_CELLSTRUCTURE = "/sitecore/content/Global References/List Data Sources/DRRG/Cell Structure";

        #endregion

        //Shams Dubai Customer Type List
        public const string SHAMSDUBAICUSTOMERTYPE = "/sitecore/content/Global References/List Data Sources/Shams Dubai Customer Type";

        #region ShamsDubaiTraining
        public const string SHAMS_DUBAI_ENROLLMENT = "/sitecore/content/Global References/List Data Sources/ShamsDubaiTraining/Enrollment";
        public const string SHAMS_DUBAI_DESIGNATION = "/sitecore/content/Global References/List Data Sources/ShamsDubaiTraining/Designation";
        public const string SHAMS_DUBAI_ELECTRICALSYSTEM = "/sitecore/content/Global References/List Data Sources/ShamsDubaiTraining/Electrical System";
        public const string SHAMS_DUBAI_PVDESIGN = "/sitecore/content/Global References/List Data Sources/ShamsDubaiTraining/PV Design";
        public const string SHAMS_DUBAI_SOLARPVEXPERT = "/sitecore/content/Global References/List Data Sources/ShamsDubaiTraining/Solar PV Expert category";
        public const string SHAMS_DUBAI_GCCCOUNTRIES = "/sitecore/content/Global References/List Data Sources/ShamsDubaiTraining/GCC Country";
        #endregion

        #region Owner Tracking
        public const string OWNER_TRACKING_SEARCH_OPTIONS = "/sitecore/content/Global References/List Data Sources/Owner Tracking/Search Options";
        #endregion

        #region HH Villa
        public const string HH_VILLA_SEARCH_OPTIONS = "/sitecore/content/Global References/List Data Sources/HH Villa/Search Options";
        #endregion

        #region  Trasaction History
        public const string TRANSACTIONHISTORY_FILTERS = "/sitecore/content/Global References/List Data Sources/Transaction History/transaction-filter";
        public const string TRANSACTIONHISTORY_DOWNLOADHISTORY = "/sitecore/content/Global References/List Data Sources/Transaction History/Download History";

        #endregion

        #region  Refund History
        public const string REFUNDHISTORY_FILTERS = "/sitecore/content/Global References/List Data Sources/Refund History/Refund history Options";
        public const string ACCOUNTTRANSFERSTATUS = "/sitecore/content/Global References/List Data Sources/Refund History/Account Transfer Status";
        public const string CHEQUESTATUS = "/sitecore/content/Global References/List Data Sources/Refund History/Cheque Status";
        public const string IBANSTATUS = "/sitecore/content/Global References/List Data Sources/Refund History/IBAN Status";
        public const string WESTERNUNIONSTATUS = "/sitecore/content/Global References/List Data Sources/Refund History/Western Union status";
        public const string REFUNDAPPROVED = "APPROVED";
        public const string REFUNDREJECTED = "REJECTED";
        public const string REFUNDMTCNCANCELLED = "MTCN CANCELLED";
        #endregion

        #region Career Fair

        public const string QUALIFICATIONS = "/sitecore/content/Global References/List Data Sources/Career Fair/Qualifications";
        public const string SPECIALISATIONS = "/sitecore/content/Global References/List Data Sources/Career Fair/Specialisations";
        #endregion

        #region Internship
        public const string EmiratesList = "/sitecore/content/Global References/List Data Sources/Emirates/Emirates List";
        public const string GradeList = "/sitecore/content/Global References/List Data Sources/Training and Internship/Grade";
        public const string HighSchoolSectionsList = "/sitecore/content/Global References/List Data Sources/Training and Internship/High School Section";

        #endregion

        #region Green Pledge
        public const string NumberOfMinutes = "/sitecore/content/Global References/List Data Sources/Green Pledge/Number Of Minutes";
        #endregion

        #region rammas datasource
        public const string RammasDatasource = "{A5FB0DB5-1AEE-42D8-8C08-42B761F66C44}";
        #endregion

        #region Epass
        public const string EpassPageSize = "/sitecore/content/Global References/List Data Sources/Epass/Page Size";
        public const string EpassPageStatus = "/sitecore/content/Global References/List Data Sources/Epass/Page Status";
        public const string EpassapprovalPageStatus = "/sitecore/content/Global References/List Data Sources/Epass/Approval Page Status";
        public const string GccCountries = "/sitecore/content/Global References/List Data Sources/Epass/GCC Country";
        public const string EpassLocations = "/sitecore/content/Global References/List Data Sources/Epass/Locations";
        public const string EpassgenerationLocations = "/sitecore/content/Global References/List Data Sources/Epass/GenerationLocations";
        public const string wpGccCountries = "/sitecore/content/Global References/List Data Sources/Work Permit/GCC Country";
        public const string MaterialPasslocations = "/sitecore/content/Global References/List Data Sources/Epass/Material Pass Locations";

        #endregion

        #region [Website revamp 2019]
        public const String Masthead2v1 = "{E95A21A6-7C9D-4284-BDE8-6690FD6B47DC}";
        #endregion

        #region [M32 Map]
        public const string M32MapDatasource = "{A0C93DDC-43AC-401A-8B21-015893464E99}";
        #endregion

        #region  [MoveTo V1]
        public const string MOVE_TO_V1_CUSTOMERTYPES = "/sitecore/content/Global References/List Data Sources/MoveTo/MoveTo Customer Types";
        public const string MOVE_TO_V1_IDTYPES = "/sitecore/content/Global References/List Data Sources/MoveTo/MoveTo ID Types";
        #endregion

        #region [Conservation]
        public const string Conservation_SectionList = "/sitecore/content/Global References/List Data Sources/ConservationDatasources/Section List";
        public const string Conservation_AcademicList = "/sitecore/content/Global References/List Data Sources/ConservationDatasources/Academic List";
        #endregion
        #region [Corporate Partnership Portal]
        public const string CP_MEETINGTYPES = "/sitecore/content/Global References/List Data Sources/Corporate Parntership/Meeting Request Type";
        #endregion

        #region [ScrapSale]
        public const string SCRAP_SALE_BIDITEM_PAGESIZE = "/sitecore/content/Global References/List Data Sources/Scrap Sale/Page Size";
        public const string SCRAP_SALE_SALESORDER_RECEIPTS = "/sitecore/content/Global References/List Data Sources/Scrap Sale/Receipt-Filter";
        #endregion

        #region Sponsorship
        public const string SPONSORSHIP_TARGET_AUDIENCE = "/sitecore/content/Global References/List Data Sources/SponsorShip/Target Audience";
        public const string SPONSORSHIP_DURATION = "/sitecore/content/Global References/List Data Sources/SponsorShip/Sponsorship Duration";
        public const string SPONSORSHIP_DEWA = "/sitecore/content/Global References/List Data Sources/SponsorShip/Dewa Sponsorship";
        #endregion

        #region [Smart communications]
        public const string SC_Preferred_Communication_Channel = "/sitecore/content/Global References/List Data Sources/Smart Communications/Preferred_Communication_Channel";
        public const string SC_Discussion_Area = "/sitecore/content/Global References/List Data Sources/Smart Communications/Discussion_Area";
        public const string SC_Discussion_Area_Email = "/sitecore/content/Global References/List Data Sources/Smart Communications/Discussion_Area_Email";
        public const string SC_Discussion_Subject = "/sitecore/content/Global References/List Data Sources/Smart Communications/Discussion_Subject";
        public const string SC_General_Information = "/sitecore/content/Global References/List Data Sources/Smart Communications/General_Information";
        public const string SC_General_Information_Email = "/sitecore/content/Global References/List Data Sources/Smart Communications/General_Information_Email";
        public const string SC_Inquiry_Type = "/sitecore/content/Global References/List Data Sources/Smart Communications/Inquiry_Type";
        public const string SC_NOC_Category = "/sitecore/content/Global References/List Data Sources/Smart Communications/NOC_Category";
        public const string SC_NOC_Category_Email = "/sitecore/content/Global References/List Data Sources/Smart Communications/NOC_Category_Email";
        #endregion

        #region Agile Governance Global Summit
        public const string AGILE_GOVERNANCE_GLOBAL_SUMMIT_SESSION_OPTIONS = "/sitecore/content/Global References/List Data Sources/Agile Govt Global Summit/Session List";
        #endregion

        #region [Suqia Donation]
        public const string SuqiaDonationDatasource = "{AEB71F75-FD5B-4814-89D8-B0A1C2AD4E82}";
        #endregion
    }

    public enum ControlTypes
    {
        RB,
        DAT,
        TIM,
        List
    }

    public enum GreenPledgeTypes
    {
        Apartment,
        Villa
    }

    public static class BillingConstants
    {
        //public const string const_xml_root = "TotalOutstanding";
        public const string const_xml_root = "/GetBillEnquiryResponse";

        public const string const_xml_account_number = "AccountNumber";
        public const string const_xml_res_code = "ResponseCode";
        public const string const_xml_desc = "Description";
        public const string const_DateTime = "DateTimeStamp";
        public const string const_electricity = "Electricity";
        public const string const_water = "Water";
        public const string const_sewerage = "Sewerage";
        public const string const_housing_fee = "Housing";
        public const string const_cooling = "Cooling";
        public const string const_others = "Others";
        public const string const_total = "Total";
        public const string const_total_due = "TotalDue";
        public const string const_account_status = "AccountStatus";
        public const string const_contract_account = "ContractAccount";
        public const string const_final_bill = "FinalBill";
    }

    public static class VATConfig
    {
        public static int VatPercentage
        {
            get
            {
                Item VatItem = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.VAT);
                int vatpercentage = VatItem != null && VatItem.Fields["Vat Percentage"] != null ? Int32.Parse(VatItem.Fields["Vat Percentage"].ToString()) : 5;
                return vatpercentage;
            }
        }
    }

    

    public enum OwnerTrackerApplicationTypes
    {
        YBNE,
        YBNW,
        YBPE,
        YDA5,
        YAPW,
        YLVI
    }

    public static class TransactionHistoryConfiguration
    {
        public static int TransactionsCount()
        {
            Item configItem = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.TransactionConfig);
            return configItem != null && configItem.Fields["Transactions Count"] != null && configItem.Fields["Transactions Count"].Value != null ? int.Parse(configItem.Fields["Transactions Count"].Value.ToString()) : 5;
        }

        public static int ReceiptCount()
        {
            Item configItem = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.TransactionConfig);
            return configItem != null && configItem.Fields["Receipt Count"] != null && configItem.Fields["Receipt Count"].Value != null ? int.Parse(configItem.Fields["Receipt Count"].Value.ToString()) : 5;
        }

        public static int RefundCount()
        {
            Item configItem = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.TransactionConfig);
            return configItem != null && configItem.Fields["Refund count"] != null && configItem.Fields["Refund count"].Value != null ? int.Parse(configItem.Fields["Refund count"].Value.ToString()) : 5;
        }
    }

    public static class DewastoreglobalConfiguration
    {
        public static string OffersURL()
        {
            Item configItem = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.DewaStoreConfig);
            return configItem != null && configItem.Fields["Offers URL"] != null && configItem.Fields["Offers URL"].Value != null ? configItem.Fields["Offers URL"].Value.ToString() : string.Empty;
        }

        public static string LogoURL()
        {
            Item configItem = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.DewaStoreConfig);
            return configItem != null && configItem.Fields["Logo URL"] != null && configItem.Fields["Logo URL"].Value != null ? configItem.Fields["Logo URL"].Value.ToString() : string.Empty;
        }

        public static string ThumbnailsURL()
        {
            Item configItem = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.DewaStoreConfig);
            return configItem != null && configItem.Fields["Thumbnails URL"] != null && configItem.Fields["Thumbnails URL"].Value != null ? configItem.Fields["Thumbnails URL"].Value.ToString() : string.Empty;
        }

        public static bool ServiceOutage()
        {
            Item configItem = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.DewaStoreConfig);
            if (configItem != null)
            {
                global::Sitecore.Data.Fields.CheckboxField checkboxField = configItem.Fields["Service Outage"];
                if (checkboxField != null && checkboxField.Checked)
                {
                    global::Sitecore.Data.Fields.DateField FromdateTimeField = configItem.Fields["Service Outage From Time"];
                    global::Sitecore.Data.Fields.DateField TodateTimeField = configItem.Fields["Service Outage To Time"];

                    if (FromdateTimeField != null && TodateTimeField != null)
                    {
                        string FromdateTimeString = FromdateTimeField.Value;
                        string TodateTimeString = TodateTimeField.Value;
                        if (!string.IsNullOrWhiteSpace(FromdateTimeString) && !string.IsNullOrWhiteSpace(TodateTimeString))
                        {
                            DateTime FromdateTimeStruct = global::Sitecore.DateUtil.ToServerTime(global::Sitecore.DateUtil.IsoDateToDateTime(FromdateTimeString));
                            DateTime TodateTimeStruct = global::Sitecore.DateUtil.ToServerTime(global::Sitecore.DateUtil.IsoDateToDateTime(TodateTimeString));
                            return (DateTime.Compare(DateTime.Now, FromdateTimeStruct) >= 0 &&
                            DateTime.Compare(DateTime.Now, TodateTimeStruct) <= 0);
                        }
                    }
                }
            }
            return false;
        }
    }

    public static class AMIGraphglobalConfiguration
    {
        public static bool ServiceOutage(out string Message)
        {
            Message = string.Empty;
            Item configItem = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.AMIGraphConfig);
            if (configItem != null)
            {
                global::Sitecore.Data.Fields.CheckboxField checkboxField = configItem.Fields["Service Outage"];
                if (checkboxField != null && checkboxField.Checked)
                {
                    global::Sitecore.Data.Fields.DateField FromdateTimeField = configItem.Fields["Service Outage From Time"];
                    global::Sitecore.Data.Fields.DateField TodateTimeField = configItem.Fields["Service Outage To Time"];

                    if (FromdateTimeField != null && TodateTimeField != null)
                    {
                        string FromdateTimeString = FromdateTimeField.Value;
                        string TodateTimeString = TodateTimeField.Value;
                        if (!string.IsNullOrWhiteSpace(FromdateTimeString) && !string.IsNullOrWhiteSpace(TodateTimeString))
                        {
                            DateTime FromdateTimeStruct = global::Sitecore.DateUtil.ToServerTime(global::Sitecore.DateUtil.IsoDateToDateTime(FromdateTimeString));
                            DateTime TodateTimeStruct = global::Sitecore.DateUtil.ToServerTime(global::Sitecore.DateUtil.IsoDateToDateTime(TodateTimeString));
                            if (DateTime.Compare(DateTime.Now, FromdateTimeStruct) >= 0 &&
                            DateTime.Compare(DateTime.Now, TodateTimeStruct) <= 0)
                            {
                                Message = configItem.Fields["Message"]!=null ? FormattedDate(configItem.Fields["Message"].Value, FromdateTimeStruct, TodateTimeStruct):string.Empty;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        private static string FormattedDate(string message, DateTime FromdateTimeStruct, DateTime TodateTimeStruct)
        {
            if (DateTime.Compare(FromdateTimeStruct.Date, TodateTimeStruct.Date).Equals(0))
            {
                return string.Format(message, FromdateTimeStruct.ToString("dddd dd MMMM yyyy", SitecoreX.Culture),
                                         FromdateTimeStruct.ToString("hh:mm tt", SitecoreX.Culture), TodateTimeStruct.ToString("hh:mm tt", SitecoreX.Culture)).Replace("يوليه", "يوليو");
            }
            else
            {
                return string.Format(message, FromdateTimeStruct.ToString("dddd dd MMMM yyyy hh:mm tt", SitecoreX.Culture),
                                         TodateTimeStruct.ToString("dddd dd MMMM yyyy hh:mm tt", SitecoreX.Culture)).Replace("يوليه", "يوليو");
            }
        }
    }

    public class ErrorMessages
    {
        public static string EFORM__FRONTEND_ERROR_MESSAGE
        { get { return Translate.Text("Unable to Process the Request"); } }

        public static string UNEXPECTED_ERROR
        { get { return Translate.Text("Unexpected error"); } }

        public static string WEBSERVICE_ERROR
        { get { return Translate.Text("Scholarship Web Service Error"); } }
    }

    public static class KofaxConstants
    {
        public static string EPASS_PROJECT = "ITF_ES_eServices_UC000121/Robots/000121_";
        public static string Subpassesname = "t000121RequestIN";
        public static string Mainpassesname = "t000121EPassMainRequestIN";
        public static string UpdateStatus = "t000121UpdateStatus";
        public static string UpdateShareStatus = "t000121EPassUpdateIN";
        public static string GetPassesname = "t000121CurrentUser";
        public static string GetPassRequest = "t000121PassNo";
        public static string SearchPassRequest = "t000121Search";
        public static string AddSubContractors = "t000121EPassSubcontractors";
        public static string GetSubContractors = "t000121EPassSubcontractorsIN";
        public static string CheckExistingUser = "t000121EPassCheckUserIN";
        public static string Pendingapprovalname = "t000121PendingApproval";
        public static string CreateEpass = EPASS_PROJECT + "RequestSubmission.robot";

        //public static string GetmyEpass = EPASS_PROJECT + "GetRequestsByMe.robot";
        public static string GetmyEpass = EPASS_PROJECT + "GetePassRequestsByMe.robot";

        //public static string GetPendingApprovalEpass = EPASS_PROJECT + "GetPendingApproval.robot";
        public static string GetPendingApprovalEpass = EPASS_PROJECT + "GetPendingePassRequests.robot";

        public static string GetPassDetailByPassNo = EPASS_PROJECT + "GetePassRequestsByPassNo.robot";
        public static string SearchPassDetailByPassNo = EPASS_PROJECT + "GetePassRequestsSearch.robot";
        public static string CreateMainPass = EPASS_PROJECT + "RequestMainPass.robot";
        public static string UpdateApprovalStatus = EPASS_PROJECT + "UpdateePassApproval.robot";
        public static string UpdateShareStatusURL = EPASS_PROJECT + "UpdateePass.robot";
        public static string AddSubContractorsURL = EPASS_PROJECT + "Subcontractor_Submit.robot";
        public static string GetSubContractorsURL = EPASS_PROJECT + "GetSubcontractor.robot";
        public static string CheckExistingUserURL = EPASS_PROJECT + "CheckExistingUser.robot";
        public static string GetBlockeduserURL = EPASS_PROJECT + "GetBlockedUsers.robot";
        public static string SPONSORSHIP_PROJECT = "ITF_ES_eServices_UC000121/Robots/000121_";
        public static string SPONSORSHIP_FORM = SPONSORSHIP_PROJECT + "SponsorshipSubmission.robot";
        public static string SPONSORNAME = "t000121Sponsorship";
        public static string OneDayValidationIN = "t000121OneDayValidationIN";
        public static string OneDayPassRequestIn = "t000121EPassOneDayRequestIN";
        public static string OneDayPassValidateMethod = EPASS_PROJECT + "ValidateURL.robot";
        public static string OneDayPassUpdateMethod = EPASS_PROJECT + "UpdateOnedayPass.robot";
        public static string ShamsDubai_Project = "ITF_SD_Email_Subscription/Robots/00033_10_";
        public static string ShamsDubaiEmailSubscription = ShamsDubai_Project + "Email_Subscription.robot";
        public static string ShamsDubaiName = "T00033_Subscription";
        //CP Portal related end points
        private static string CP_BASE = "ITF_CP_Portal/Robots/";
        //private static string CP_BASE_ROBOTS = "Robots/";
        private static string CP_BASE_INITIALS = "00034_10_CPP_";
        //user management
        public static string CPUserManagement { get { return CP_BASE + CP_BASE_INITIALS + "UserManagment.robot"; } }
        public static string CPVariableUserManagement = "Input";
        //idea management
        private static string CPIMLastPart = "IdeaManagment.robot";
        public static string CPVariableIdeaList = "T00034_IdeaManagment";
        //public static string CPIMPathWithoutRobots { get { return CP_BASE + CP_BASE_INITIALS + CPIMLastPart; } }
        public static string CPIMPathWithRobots { get { return CP_BASE + "00034_30_CPP_" + CPIMLastPart; } }
        //counts
        public static string CPGetCountsPath { get { return CP_BASE + "00034_60_CPP_Counts.robot"; } }
        public static string CPVariableGetCountsInput = "T00034_InputVar";
        //REquest Management
        public static string CPRequestsPath { get { return CP_BASE + "00034_50_CPP_MeetingManagment.robot"; } }
        public static string CPVariableRequestManagement = "T00034_MeetingManagment";
        // Issue Management
        public static string IMRequestPath { get { return CP_BASE + "00034_20_CPP_IssueManagment.robot"; } }
        public static string IMVariableIssueManagement = "T00034_IssueManagment";
        // Message management
        public static string MMRequestPath { get { return CP_BASE + "00034_70_CPP_MessageManagment.robot"; } }
        public static string MMVariableMessageManagement = "T00034_MessageManagment";
        //https://rpa-dev.dewa.gov.ae/MC_106/rest/run/ITF_CP_Portal/Robots/00034_30_CPP_MessageManagment.robot
    }

    public static class DewaPaymentChannel
    {
        /// <summary>
        /// Refund Recovery
        /// </summary>
        public const string RR = "RR";
    }
    public class RegexConstant
    {
        /// <summary>
        /// Password Regex With Small Letter, Symbol And Number.
        /// </summary>
        public const string PasswordRegexWithSmallLetterSymbolAndNumber = @"(?=^.{6,25}$)(?=(?:.*?\d){2})(?=.*[a-z]{1})(?=(?:.*?[a-z]){1})(?=(?:.*?[!@#$%*()_+^&}{:;?.]){1})(?!.*\s)[0-9a-zA-Z!@#$%*()_+^&]*";
    }
}