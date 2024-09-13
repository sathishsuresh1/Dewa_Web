using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace DEWAXP.Foundation.Integration
{
    internal class ConfigKeys
    {
        internal const string DEWA_MERCHANT_ID_DESKTOP = "DEWA_MERCHANT_ID_DESKTOP";

        internal const string DEWA_VENDOR_ID_DESKTOP = "DEWA_VENDOR_ID_DESKTOP";

        internal const string Jobseeker_VENDOR_ID_DESKTOP = "Jobseeker_VENDOR_ID_DESKTOP";

        internal const string DEWA_MERCHANT_PWD_DESKTOP = "DEWA_MERCHANT_PWD_DESKTOP";

        internal const string DEWA_MERCHANT_ID_MOBILE = "DEWA_MERCHANT_ID_MOBILE";

        internal const string DEWA_VENDOR_ID_MOBILE = "DEWA_VENDOR_ID_MOBILE";

        internal const string Jobseeker_VENDOR_ID_MOBILE = "Jobseeker_VENDOR_ID_MOBILE";

        internal const string Jobseeker_NEW_VENDOR_ID_DESKTOP = "Jobseeker_NEW_VENDOR_ID_DESKTOP";

        internal const string Jobseeker_NEW_VENDOR_ID_MOBILE = "Jobseeker_NEW_VENDOR_ID_MOBILE";

        internal const string DEWA_MERCHANT_PWD_MOBILE = "DEWA_MERCHANT_PWD_MOBILE";

        internal const string DEWA_APP_ID = "DEWA_APP_ID";

        internal const string DEWA_APP_VER = "DEWA_APP_VER";

        internal const string DEWA_SERVICE_VER = "DEWA_SERVICE_VER";

        internal const string BB_UN = "BB_UN";

        internal const string BB_PWD = "BB_PWD";

        internal const string RECRUITMENT_UN = "RECRUITMENT_UN";

        internal const string RECRUITMENT_PWD = "RECRUITMENT_PWD";

        internal const string DUBAI_MODEL_UN = "DUBAI_MODEL_UN";

        internal const string DUBAI_MODEL_PWD = "DUBAI_MODEL_PWD";
        internal const string ESERVICE_UN = "ESERVICE_UN";

        internal const string ESERVICE_PWD = "ESERVICE_PWD";
        internal const string DMS_USERNAME = "DMS_USERNAME";
        internal const string DMS_PASSWORD = "DMS_PASSWORD";
        internal const string CREDDB_UN = "CREDDB_UN";
        internal const string CREDDB_PWD = "CREDDB_PWD";
        internal const string HM_UID = "HEADER_EHM_UN";
        internal const string HM_PWD = "HEADER_EHM_PWD";
        internal const string EMAIL_UID = "EMAIL_USER";
        internal const string EMAIL_PWD = "EMAIL_PWD";
        internal const string CREDDBBPM_UN = "CREDDBBPM_UN";
        internal const string CREDDBBPM_PWD = "CREDDBBPM_PWD";
        internal const string DCTM_SmartOffice_UN = "DCTM_SmartOffice_UN";
        internal const string DCTM_SmartOffice_PWD = "DCTM_SmartOffice_PWD";
        internal const string DCTM_SmartOffice_SSL_SETTING = "DCTM_SmartOffice_SSL_SETTING";
        internal const string CPPORTAL_REPOSITORY = "CPPORTAL_REPOSITORY";
        internal const string CPPORTAL_USERID = "CPPORTAL_USERID";
        internal const string CPPORTAL_PWD = "CPPORTAL_PWD";
        internal const string SSL_SETTING = "SSL_SETTING";
        internal const string Rammas_SSL_SETTING = "Rammas_SSL_SETTING";

        //Rest API
        internal const string RestAPI_Smart_Customer = "RestAPI_Smart_Customer";
        internal const string RestAPI_Oauth_URL = "RestAPI_Oauth_URL";
        internal const string RestAPI_Client_Id = "RestAPI_Client_Id";
        internal const string RestAPI_Client_Secret = "RestAPI_Client_Secret";
        internal const string RestAPI_Grant_Type = "RestAPI_Grant_Type";
        internal const string RestAPI_Dev_Id = "RestAPI_Dev_Id";
        internal const string SmartCustomerV3_ApiUrl = "SmartCustomerV3_ApiUrl";
        internal const string SmartCustomerV4_ApiUrl = "SmartCustomerV4_ApiUrl";
        internal const string CVI_Vendor_ID_DESKTOP = "DEWA_VENDOR_ID_DESKTOP";
        internal const string SmartCustomerV3_LoginApiUrl = "SmartCustomerV3_LoginApiUrl";
        internal const string SmartCustomerV4_MerchantId = "SmartCustomerV4_MerchantId";
        internal const string SmartCustomerV4_MerchantPass = "SmartCustomerV4_MerchantPass";
        internal const string SmartMeterV2_ApiUrl = "SmartMeterV2_ApiUrl";
        internal const string SmartBooking_ApiUrl = "SmartBooking_ApiUrl";
        internal const string RestAPIUrl_SmartDubai = "RestAPIUrl_SmartDubai";
        internal const string DEWAScholarship_ApiUrl = "DEWAScholarship_ApiUrl";
        internal const string JobseekerV2_ApiUrl = "JobseekerV2_ApiUrl";
        internal const string UsersV3_ApiUrl = "DEWASMARTUSER"; //"UsersV3_ApiUrl";
        internal const string NBDPay_ApiUrl = "NBDPay_ApiUrl";
        internal const string SmartSurvey_ApiUrl = "SmartSurveyURL";

        #region [epass RestAPI]
        internal const string RestAPI_Oauth_URL_Epass = "RestAPI_Oauth_URL_Epass";
        internal const string RestAPI_Client_Id_Epass = "RestAPI_Client_Id_Epass";
        internal const string RestAPI_Client_Secret_Epass = "RestAPI_Client_Secret_Epass";
        internal const string RestAPI_Dev_Id_Epass = "RestAPI_Dev_Id_Epass";
        public const string Gatepass_UN = "Gatepass_UN";
        public const string Gatepass_PWD = "Gatepass_PWD";
        #endregion

        #region [Api Webservice Config]
        internal const string APIURL_SMARTCUSTOMER = "apiurl_smartcustomer";
        #endregion

        #region [Kadamtech Webservice Config]
        public static string Kadamtech_DEWA_Username => "Kadamtech_DEWA_Username";
        public static string Kadamtech_DEWA_Password => "Kadamtech_DEWA_Password";
        #endregion


        #region [DTMC Vendor]
        internal const string dtmc_desktop_vendor = "dtmc_desktop_vendor";
        internal const string dtmc_mobile_vendor = "dtmc_desktop_vendor";
        #endregion

        #region Universal Service Centre
        internal const string DEWA_USC_MERCHANT_ID_DESKTOP = "DEWA_USC_MERCHANT_ID_DESKTOP";

        internal const string DEWA_USC_MERCHANT_PWD_DESKTOP = "DEWA_USC_MERCHANT_PWD_DESKTOP";

        internal const string DEWA_USC_MERCHANT_ID_MOBILE = "DEWA_USC_MERCHANT_ID_MOBILE";

        internal const string DEWA_USC_MERCHANT_PWD_MOBILE = "DEWA_USC_MERCHANT_PWD_MOBILE";
        internal const string Qms_WsAuth_Username = "Qms_WsAuth_Username";
        internal const string Qms_WsAuth_Pwd = "Qms_WsAuth_Pwd";
        #endregion

        #region "DEWA Web API"

        internal const string DewaWebApiUrlKey = "DEWAWebApiURL";
        internal const string DewaWebApiUsernameKey = "DEWAWebApiUserName";
        internal const string DewaWebApiUserpassKey = "DEWAWebApiPassword";

        #endregion

        #region **SMART VENDOR**
        internal const string DEWASMARTVENDOR_URL = "DEWASMARTVENDOR_URL";
        #endregion

        #region UAEPASS
        public const string UAEPASS_RETURNOAUTHURL = "UAEPASS_RETURNOAUTHURL";
        public const string UAEPASS_CLIENT_ID = "UAEPASS_CLIENT_ID";
        public const string UAEPASS_CLIENT_SECRETID = "UAEPASS_CLIENT_SECRETID";
        public const string UAEPASS_OPENID_ACCESSCODE = "UAEPASS_OPENID_ACCESSCODE";
        public const string UAEPASS_OAUTH_URL = "UAEPASS_OAUTH_URL";
        public const string UAEPASS_STATE = "UAEPASS_STATE";
        public const string UAEPASS_ACCESSRETURNURL = "UAEPASS_ACCESSRETURNURL";
        public const string UAEPASS_USER_INFO = "UAEPASS_USER_INFO";
        #endregion
        public const string ALEXA_API_KEY = "ALEXA_API_KEY";
        public const string ALEXA_LOGIN_URL = "ALEXA_LOGIN_URL";
        public const string SM_PREDICT_API = "SM_PREDICT_API";
        public const string SM_PREDICT_APIKEY = "SM_PREDICT_APIKEY";
        public const string CCR_PREDICT_API = "CCR_PREDICT_API";
        public const string CCR_PREDICT_APIKEY = "CCR_PREDICT_APIKEY";



    }
    public static class SmartResponseConfig
    {

        internal static string SM_PREDICT_API
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.SM_PREDICT_API]; }
        }
        internal static string SM_PREDICT_APIKEY
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.SM_PREDICT_APIKEY]; }
        }
    }
    public static class ConsumptionComplaintResponseCofig
    {
        internal static string CC_PREDICT_API
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.CCR_PREDICT_API]; }
        }
        internal static string CC_PREDICT_APIKEY
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.CCR_PREDICT_APIKEY]; }
        }
    }
    internal class ErrorMessages
    {
        internal static string FRONTEND_ERROR_MESSAGE
        { get { return Translate.Text("Webservice Error"); } }

        internal static string PLEASETRYAGAIN_ERROR_MESSAGE { get { return Translate.Text("Please try again"); } }

        internal static string AccountLocked { get { return Translate.Text("Account_Locked_ErrorMessage"); } }
        internal static string InvalidCredential { get { return Translate.Text("InvalidCredential_ErrorMessage"); } }
    }

    public static class KofaxSponsorConstnats
    {
        public static string SPONSORSHIP_PROJECT = "ITF_ES_eServices_UC000121/Robots/000121_";
        public static string SPONSORSHIP_FORM = SPONSORSHIP_PROJECT + "SponsorshipSubmission.robot";
        public static string SPONSORNAME = "t000121Sponsorship";
    }

    internal static class LogMessageTemplate
    {
        private const string errorMessage = "response value: Status : {0} Content: {1} Description: {2}";
        public static System.Exception BuildException(string statusCode, string content, string statusDescription="")
        {
            return new Exception(string.Format(errorMessage, statusCode, content, statusDescription));
        }
    }

    internal static class DewaWebApi
    {
        /// <summary>
        /// Defines the RESTClientURL
        /// </summary>
        internal static string RESTClientURL => WebConfigurationManager.AppSettings[ConfigKeys.DewaWebApiUrlKey];

        /// <summary>
        /// Defines the RESTClientUserName
        /// </summary>
        internal static string RESTClientUserName => WebConfigurationManager.AppSettings[ConfigKeys.DewaWebApiUsernameKey];

        /// <summary>
        /// Defines the RESTClientPassword
        /// </summary>
        internal static string RESTClientPassword => WebConfigurationManager.AppSettings[ConfigKeys.DewaWebApiUserpassKey];

        /// <summary>
        /// Defines the DBUserName
        /// </summary>
        internal static string DBUserName => WebConfigurationManager.AppSettings[ConfigKeys.CREDDB_UN];

        /// <summary>
        /// Defines the DBPassword
        /// </summary>
        internal static string DBPassword => WebConfigurationManager.AppSettings[ConfigKeys.CREDDB_PWD];
    }
    internal static class SitecoreItemId
    {
        internal static string KofaxconfigItem = "{11A3D3BC-5BF1-487A-B5A7-4AF9F5E44AE3}";
    }
}
