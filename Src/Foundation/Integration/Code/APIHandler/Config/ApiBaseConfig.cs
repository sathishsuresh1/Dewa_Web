using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Config
{
    public static class ApiBaseConfig
    {
        public static string RESTAPI_SMART_CUSTOMER => ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Smart_Customer];

        internal static string SmartCustomerV3_ApiUrl => ConfigurationManager.AppSettings[ConfigKeys.SmartCustomerV3_ApiUrl];
        internal static string SmartMeterV2_ApiUrl => ConfigurationManager.AppSettings[ConfigKeys.SmartMeterV2_ApiUrl];
        internal static string Qms_WsAuth_User => ConfigurationManager.AppSettings[ConfigKeys.Qms_WsAuth_Username];
        internal static string Qms_WsAuth_Pwd => ConfigurationManager.AppSettings[ConfigKeys.Qms_WsAuth_Pwd];
        internal static string RestAPIUrl_SmartDubai => ConfigurationManager.AppSettings[ConfigKeys.RestAPIUrl_SmartDubai];
        internal static string DEWAScholarship_ApiUrl => ConfigurationManager.AppSettings[ConfigKeys.DEWAScholarship_ApiUrl];
        internal static string SmartBooking_ApiUrl => ConfigurationManager.AppSettings[ConfigKeys.SmartBooking_ApiUrl];
        internal static string JobseekerV2_ApiUrl => ConfigurationManager.AppSettings[ConfigKeys.JobseekerV2_ApiUrl];
        internal static string UsersV3_ApiUrl => ConfigurationManager.AppSettings[ConfigKeys.UsersV3_ApiUrl];
        internal static string NBDPay_ApiUrl => ConfigurationManager.AppSettings[ConfigKeys.NBDPay_ApiUrl];
        internal static string SmartSurvey_ApiUrl => ConfigurationManager.AppSettings[ConfigKeys.SmartSurvey_ApiUrl];

        internal static string SmartCustomerV4_ApiUrl => ConfigurationManager.AppSettings[ConfigKeys.SmartCustomerV4_ApiUrl];

        internal static string SmartCustomerV3_LoginApiUrl => ConfigurationManager.AppSettings[ConfigKeys.SmartCustomerV3_LoginApiUrl];

        internal static string SmartCustomerV4_MerchantId => ConfigurationManager.AppSettings[ConfigKeys.SmartCustomerV4_MerchantId];

        internal static string SmartCustomerV4_MerchantPass => ConfigurationManager.AppSettings[ConfigKeys.SmartCustomerV4_MerchantPass];

        internal static string CVI_Vendor_ID_DESKTOP => ConfigurationManager.AppSettings[ConfigKeys.CVI_Vendor_ID_DESKTOP];
    }
    public static class UAEPassConfig
    {
        public static string UAEPASS_RETURNOAUTHURL => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_RETURNOAUTHURL];
        public static string UAEPASS_CLIENT_ID => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_CLIENT_ID];
        public static string UAEPASS_CLIENT_SECRETID => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_CLIENT_SECRETID];
        public static string UAEPASS_OPENID_ACCESSCODE => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_OPENID_ACCESSCODE];
        public static string UAEPASS_OAUTH_URL => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_OAUTH_URL];
        public static string UAEPASS_STATE => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_STATE];
        public static string UAEPASS_ACCESSRETURNURL => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_ACCESSRETURNURL];
        public static string UAEPASS_USER_INFO => ConfigurationManager.AppSettings[ConfigKeys.UAEPASS_USER_INFO];
    }
}
