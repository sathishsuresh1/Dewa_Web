using Glass.Mapper.Sc.Configuration.Fluent;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;
using System.Configuration;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Foundation.Content
{
    public static class Config
    {
        public static string EPaySecret
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.EPAY_SECRET]; }
        }

        public static string EPayUrl
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.EPAY_URL]; }
        }

        public static string EPayUrlDEWA
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.EPAY_URL_DEWA]; }
        }      

        public static string PrimaryDNS
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.PRIMARY_DNS]; }
        }

        public static string SecondaryDNS
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.SECONDARY_DNS]; }
        }

        public static string NoqodiPayUrl
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.NoqodiPAY_URL]; }
        }

        public static string NoqodiPayUrlDEWA
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.NoqodiPAY_URL_DEWA]; }
        }

        #region [Secure Payment Url]
        public static string SecuredNoqodiPayUrl => ConfigurationManager.AppSettings["SECURED_NOQODI_PAYURL"];
        public static string SecuredEasyPayUrl => ConfigurationManager.AppSettings["SECURED_EASY_PAYURL"];
        public static string SecuredPayurlRedirect => ConfigurationManager.AppSettings["SECURED_PAYURL_REDIRECT"];
        internal static string SecuredEmiratesNBDPayUrl => ConfigurationManager.AppSettings["SECURED_EmiratesNBD_PAYURL"];
        public static string Secured_UAEPGS_PayURL => ConfigurationManager.AppSettings["SECURED_UAEPGS_PAYURL"];
        /// <summary>
        /// IsSecuredMIMEnabled : it enable to service to pay through secure channel i.e MIM
        /// </summary>
        public static bool IsSecuredMIMEnabled => System.Convert.ToBoolean(ConfigurationManager.AppSettings["SECURED_MIM_ENABLED"]=="1");
        #endregion
    }

    public class HappinessIndicatorConfig
    {
        Item item = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.HappinessConfig);
        public string ClientIdentifier => item != null && item.Fields["HAPPINESS_IND_CLIENT_ID"] != null ? item.Fields["HAPPINESS_IND_CLIENT_ID"].ToString() : string.Empty;
        public string PostUrl => item != null && item.Fields["HAPPINESS_IND_POST_URL"] != null ? item.Fields["HAPPINESS_IND_POST_URL"].ToString() : string.Empty;
        public string SecretKey => SitecoreX.Database.GetItem(SitecoreItemIdentifiers.HappinessConfig) != null ? SitecoreX.Database.GetItem(SitecoreItemIdentifiers.HappinessConfig)["HAPPINESS_IND_SECRET_KEY"].ToString() : string.Empty;
        public string ServiceProvider => item != null && item.Fields["HAPPINESS_IND_SERVICE_PROVIDER"] != null ? item.Fields["HAPPINESS_IND_SERVICE_PROVIDER"].ToString() : string.Empty;
        public string Channel => item != null && item.Fields["HAPPINESS_IND_CHANNEL"] != null ? item.Fields["HAPPINESS_IND_CHANNEL"].ToString() : string.Empty;
        public string Source => item != null && item.Fields["HAPPINESS_IND_SOURCE"] != null ? item.Fields["HAPPINESS_IND_SOURCE"].ToString() : string.Empty;
        public string ServiceDescription => item != null && item.Fields["HAPPINESS_IND_SERVICE_DESC"] != null ? item.Fields["HAPPINESS_IND_SERVICE_DESC"].ToString() : string.Empty;
        public string ServiceCode => item != null && item.Fields["HAPPINESS_IND_SERVICE_CODE"] != null ? item.Fields["HAPPINESS_IND_SERVICE_CODE"].ToString() : string.Empty;
        public string ApplicationIdentifier => item != null && item.Fields["HAPPINESS_IND_APP_ID"] != null ? item.Fields["HAPPINESS_IND_APP_ID"].ToString() : string.Empty;
        public string GessEnabled => item != null && item.Fields["HAPPINESS_IND_GESS_ENABLED"] != null ? item.Fields["HAPPINESS_IND_GESS_ENABLED"].ToString() : string.Empty;
        public string ApplicationType => item != null && item.Fields["HAPPINESS_IND_APP_TYPE"] != null ? item.Fields["HAPPINESS_IND_APP_TYPE"].ToString() : string.Empty;
        public string Platform => item != null && item.Fields["HAPPINESS_IND_PLATFORM"] != null ? item.Fields["HAPPINESS_IND_PLATFORM"].ToString() : string.Empty;
        public string ApplicationUrl => item != null && item.Fields["HAPPINESS_IND_APP_URL"] != null ? item.Fields["HAPPINESS_IND_APP_URL"].ToString() : string.Empty;
        public string Version => item != null && item.Fields["HAPPINESS_IND_VER"] != null ? item.Fields["HAPPINESS_IND_VER"].ToString() : string.Empty;
        public string ThemeColor => item != null && item.Fields["HAPPINESS_IND_THEME_COLOR"] != null ? item.Fields["HAPPINESS_IND_THEME_COLOR"].ToString() : string.Empty;

    }

    public static class MyIdConfig
    {
        public static string PartnerIdentifier
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.MYID_PARTNER_ID]; }
        }

        public static string SsoUrl
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.MYID_SSO_URL]; }
        }

        public static string LogoutUrl
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.MYID_LOGOUT_URL]; }
        }

        public static string IssuerUrl
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.MYID_ISSUER_URL]; }
        }

        public static int ServiceIndex
        {
            get { return int.Parse(ConfigurationManager.AppSettings[ConfigKeys.MYID_SERVICE_INDEX]); }
        }

        public static string AssertionUrl
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.MYID_ASSERTION_URL]; }
        }

        public static string ServiceProviderCertKey
        {
            get { return "spX509Certificate"; }
        }

        public static string IdentityProviderCertKey
        {
            get { return "idpX509Certificate"; }
        }
    }

    //public static class UAEPassConfig
    //{
    //    public static string UAEPASS_RETURNOAUTHURL => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_RETURNOAUTHURL];
    //    public static string UAEPASS_CLIENT_ID => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_CLIENT_ID];
    //    public static string UAEPASS_CLIENT_SECRETID => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_CLIENT_SECRETID];
    //    public static string UAEPASS_OPENID_ACCESSCODE => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_OPENID_ACCESSCODE];
    //    public static string UAEPASS_OAUTH_URL => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_OAUTH_URL];
    //    public static string UAEPASS_STATE => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_STATE];
    //    public static string UAEPASS_ACCESSRETURNURL => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_ACCESSRETURNURL];
    //    public static string UAEPASS_USER_INFO => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_USER_INFO];
    //}

    public static class DocumentumConfig
    {
        public static string DMS_CONNECTUSER
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.DMS_CONNECTUSERNAME]; }
        }
        public static string DMS_CONNECTPASSWORD
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.DMS_CONNECTPASSWORD]; }
        }
        public static string DMS_DOCUMENTTYPE
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.DMS_DOCUMENTTYPE]; }
        }
        public static string DMS_DOCBASE
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.DMS_DOCBASE]; }
        }
        public static string PDMS_DOCUMENTTYPE
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.PDMS_DOCUMENTTYPE]; }
        }
    }

    
    public static class SmartResponseCofig
    {
        public static string SMART_RESPONSE_JSON_PATH
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.SMART_RESPONSE_JSON_PATH]; }
        }

        public static string SMART_RESPONSE_UPLOADPATH
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.SMART_RESPONSE_UPLOADPATH]; }
        }

        public static bool SM_DISBABLE_LOGIC
        {
            get { return System.Convert.ToBoolean(ConfigurationManager.AppSettings[ConfigKeys.SM_DISBABLELOGIC] == "1"); }
        }

        public static string SM_PREDICT_API
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.SM_PREDICT_API]; }
        }
        public static string SM_PREDICT_APIKEY
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.SM_PREDICT_APIKEY]; }
        }
    }

    public static class ConsumptionComplaintResponseCofig
    {
        public static string CC_RESPONSE_JSON_PATH
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.CCR_JSON_PATH]; }
        }

        public static string CC_RESPONSE_UPLOADPATH
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.CCR_UPLOADPATH]; }
        }

        public static bool CC_DISBABLE_LOGIC
        {
            get { return System.Convert.ToBoolean(ConfigurationManager.AppSettings[ConfigKeys.CCR_DISBABLELOGIC] == "1"); }
        }

        public static string CC_PREDICT_API
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.CCR_PREDICT_API]; }
        }
        public static string CC_PREDICT_APIKEY
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.CCR_PREDICT_APIKEY]; }
        }
    }
    public class ConfigKeys
    {
        public const string EPAY_SECRET = "EPAY_SECRET";

        public const string EPAY_URL = "EPAY_URL";        

        public const string EPAY_URL_DEWA = "EPAY_URL_DEWA";

        public const string NoqodiPAY_URL = "NoqodiPAY_URL";

        public const string NoqodiPAY_URL_DEWA = "NoqodiPAY_URL_DEWA";

        public const string PRIMARY_DNS = "PRIMARY_DNS";

        public const string SECONDARY_DNS = "SECONDARY_DNS";

        public const string HAPPINESS_IND_CLIENT_ID = "HAPPINESS_IND_CLIENT_ID";

        public const string HAPPINESS_IND_POST_URL = "HAPPINESS_IND_POST_URL";

        public const string HAPPINESS_IND_SECRET_KEY = "HAPPINESS_IND_SECRET_KEY";

        public const string HAPPINESS_IND_SERVICE_PROVIDER = "HAPPINESS_IND_SERVICE_PROVIDER";

        public const string HAPPINESS_IND_CHANNEL = "HAPPINESS_IND_CHANNEL";

        public const string HAPPINESS_IND_SOURCE = "HAPPINESS_IND_SOURCE";

        public const string HAPPINESS_IND_SERVICE_DESC = "HAPPINESS_IND_SERVICE_DESC";

        public const string HAPPINESS_IND_SERVICE_CODE = "HAPPINESS_IND_SERVICE_CODE";

        public const string HAPPINESS_IND_APP_ID = "HAPPINESS_IND_APP_ID";

        public const string HAPPINESS_IND_GESS_ENABLED = "HAPPINESS_IND_GESS_ENABLED";

        public const string HAPPINESS_IND_APP_TYPE = "HAPPINESS_IND_APP_TYPE";

        public const string HAPPINESS_IND_PLATFORM = "HAPPINESS_IND_PLATFORM";

        public const string HAPPINESS_IND_APP_URL = "HAPPINESS_IND_APP_URL";

        public const string HAPPINESS_IND_VER = "HAPPINESS_IND_VER";

        public const string HAPPINESS_IND_THEME_COLOR = "HAPPINESS_IND_THEME_COLOR";

        public const string MYID_PARTNER_ID = "MYID_PARTNER_ID";

        public const string MYID_SSO_URL = "MYID_SSO_URL";

        public const string MYID_LOGOUT_URL = "MYID_LOGOUT_URL";

        public const string MYID_ISSUER_URL = "MYID_ISSUER_URL";

        public const string MYID_SERVICE_INDEX = "MYID_SERVICE_INDEX";

        public const string MYID_ASSERTION_URL = "MYID_ASSERTION_URL";

        public const string DMS_CONNECTUSERNAME = "DMS_CONNECTUSERNAME";
        public const string DMS_CONNECTPASSWORD = "DMS_CONNECTPASSWORD";
        public const string DMS_DOCUMENTTYPE = "DMS_DOCUMENTTYPE";
        public const string DMS_DOCBASE = "DMS_DOCBASE";
        public const string PDMS_DOCUMENTTYPE = "PDMS_DOCUMENTTYPE";

        public const string BES_BASE_URL = "BES_BASE_URL";

        public const string EFORM_MIDDLEWARE_API_USR = "EFORM_MIDDLEWARE_API_USR";
        public const string EFORM_MIDDLEWARE_API_USRPASS = "EFORM_MIDDLEWARE_API_USRPASS";

        

        public const string SMART_RESPONSE_JSON_PATH = "SMART_RESPONSE_JSON_PATH";
        public const string SMART_RESPONSE_UPLOADPATH = "SMART_RESPONSE_UPLOADPATH";
        public const string SM_DISBABLELOGIC = "SM_DISBABLELOGIC";
        public const string SM_PREDICT_API = "SM_PREDICT_API";
        public const string SM_PREDICT_APIKEY = "SM_PREDICT_APIKEY";
        


        #region [Consumption Comaplaint Response Config key]
        public const string CCR_JSON_PATH = "CCR_JSON_PATH";
        public const string CCR_UPLOADPATH = "CCR_UPLOADPATH";
        public const string CCR_DISBABLELOGIC = "CCR_DISBABLELOGIC";
        public const string CCR_PREDICT_API = "CCR_PREDICT_API";
        public const string CCR_PREDICT_APIKEY = "CCR_PREDICT_APIKEY";
        #endregion

        #region [epass]
        public const string CMorCD = "CMorCD";
        public const string CPPORTAL_REPOSITORY = "CPPORTAL_REPOSITORY";
        public const string CPPORTAL_USERID = "CPPORTAL_USERID";
        public const string CPPORTAL_PWD = "CPPORTAL_PWD";
        #endregion
        public const string ALEXA_URL = "ALEXA_URL";

    }

    public class RecaptchaKeys
    {
        public static string Proxyuser => ConfigurationManager.AppSettings["PROXYUSER"];

        public static string Proxypassword => ConfigurationManager.AppSettings["PROXYPASSWORD"];

        public static string Proxydomain => ConfigurationManager.AppSettings["PROXYDOMAIN"];

        public static string Proxyurl => ConfigurationManager.AppSettings["PROXYURL"];
    }


    public class UtilAppConfig
    {
        public static string DestMapMakerImage => ConfigurationManager.AppSettings["DestMapMakerImage"];
    }
}