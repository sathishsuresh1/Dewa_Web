using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Security;
using DEWAXP.Foundation.Integration.Enums;

namespace DEWAXP.Foundation.Integration.Impl
{
    public abstract class BaseDewaGateway
	{
		protected string AppVersion
		{
			get { return ConfigurationManager.AppSettings[ConfigKeys.DEWA_APP_VER]; }
		}
        protected string ServiceVersion
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.DEWA_SERVICE_VER]; }
        }
        protected string DeviceType => "Website";
        protected string BbUsername
		{
			get { return ConfigurationManager.AppSettings[ConfigKeys.BB_UN]; }
		}

        protected string BbPassword
		{
			get { return ConfigurationManager.AppSettings[ConfigKeys.BB_PWD]; }
		}

        protected string EHMUserName
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.HM_UID]; }
        }

        protected string EHMPassword
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.HM_PWD]; }
        }

        protected string SSLSettings
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.SSL_SETTING]; }
        }
        protected string Rammas_SSL_SETTING
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.Rammas_SSL_SETTING]; }
        }
        protected string DubaiModelSSLSettings
        {
            get { return ConfigurationManager.AppSettings["DUBAIMODEL_SSL_SETTING"]; }
        }

        protected string GetMerchantId(RequestSegment segment)
	    {
		    if (segment == RequestSegment.Desktop)
		    {
				return ConfigurationManager.AppSettings[ConfigKeys.DEWA_MERCHANT_ID_DESKTOP];
			}
			return ConfigurationManager.AppSettings[ConfigKeys.DEWA_MERCHANT_ID_MOBILE];
		}

		protected string GetMerchantPassword(RequestSegment segment)
		{
			if (segment == RequestSegment.Desktop)
			{
				return ConfigurationManager.AppSettings[ConfigKeys.DEWA_MERCHANT_PWD_DESKTOP];
			}
			return ConfigurationManager.AppSettings[ConfigKeys.DEWA_MERCHANT_PWD_MOBILE];
		}
        protected string GetUSCMerchantId(RequestSegment segment)
        {
            if (segment == RequestSegment.Desktop)
            {
                return ConfigurationManager.AppSettings[ConfigKeys.DEWA_USC_MERCHANT_ID_DESKTOP];
            }
            return ConfigurationManager.AppSettings[ConfigKeys.DEWA_USC_MERCHANT_ID_MOBILE];
        }

        protected string GetUSCMerchantPassword(RequestSegment segment)
        {
            if (segment == RequestSegment.Desktop)
            {
                return ConfigurationManager.AppSettings[ConfigKeys.DEWA_USC_MERCHANT_PWD_DESKTOP];
            }
            return ConfigurationManager.AppSettings[ConfigKeys.DEWA_USC_MERCHANT_PWD_MOBILE];
        }
        protected string GetVendorId(RequestSegment segment)
		{
			if (segment == RequestSegment.Desktop)
			{
				return ConfigurationManager.AppSettings[ConfigKeys.DEWA_VENDOR_ID_DESKTOP];
			}
			return ConfigurationManager.AppSettings[ConfigKeys.DEWA_VENDOR_ID_MOBILE];
		}
        protected string GetRammasVendorId(RequestSegment segment)
        {
            if (segment == RequestSegment.Desktop)
            {
                return ConfigurationManager.AppSettings[ConfigKeys.Jobseeker_VENDOR_ID_DESKTOP];
            }
            return ConfigurationManager.AppSettings[ConfigKeys.Jobseeker_VENDOR_ID_MOBILE];
        }

        protected string GetJobSeekerVendorId(RequestSegment segment)
        {
            if (segment == RequestSegment.Desktop)
            {
                return ConfigurationManager.AppSettings[ConfigKeys.Jobseeker_NEW_VENDOR_ID_DESKTOP];
            }
            return ConfigurationManager.AppSettings[ConfigKeys.Jobseeker_NEW_VENDOR_ID_MOBILE];
        }

        protected EndpointAddress GetEndpointAddress(string endPointName) 
        {
            var clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;
            string address = string.Empty;
            for (int i = 0; i < clientSection.Endpoints.Count; i++)
            {
                if (clientSection.Endpoints[i].Name == endPointName)
                    address = clientSection.Endpoints[i].Address.ToString();
            }
            return new EndpointAddress(address);
        }

        protected string GetDTMCVendorId(RequestSegment segment)
        {
            if (segment == RequestSegment.Desktop)
            {
                return ConfigurationManager.AppSettings[ConfigKeys.dtmc_desktop_vendor];
            }
            return ConfigurationManager.AppSettings[ConfigKeys.dtmc_mobile_vendor];
        }
        protected string EserviceUserName
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.ESERVICE_UN]; }
        }

        protected string EServicePassword
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.ESERVICE_PWD]; }
        }
        protected string DMS_Username
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.DMS_USERNAME]; }
        }

        protected string DMS_Password
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.DMS_PASSWORD]; }
        }

        protected string CredDbUser
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.CREDDB_UN]; }
        }

        protected string CredDbPassword
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.CREDDB_PWD]; }
        }

		protected string CredDbUserBPM
		{
			get { return ConfigurationManager.AppSettings[ConfigKeys.CREDDBBPM_UN]; }
		}

		protected string CredDbPasswordBPM
		{
			get { return ConfigurationManager.AppSettings[ConfigKeys.CREDDBBPM_PWD]; }
		}

        protected string EMAIL_USER
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.EMAIL_UID]; }
        }

        protected string EMAIL_PASSWORD
        {
            get { return ConfigurationManager.AppSettings[ConfigKeys.EMAIL_PWD]; }
        }
        protected string DCTM_SmartOffice_USERNAME => ConfigurationManager.AppSettings[ConfigKeys.DCTM_SmartOffice_UN];

        protected string DCTM_SmartOffice_PASSWORD => ConfigurationManager.AppSettings[ConfigKeys.DCTM_SmartOffice_PWD];

        protected string DCTM_SmartOffice_SSL_SETTING => ConfigurationManager.AppSettings[ConfigKeys.DCTM_SmartOffice_SSL_SETTING];

        protected string CPPORTAL_REPOSITORY => ConfigurationManager.AppSettings[ConfigKeys.CPPORTAL_REPOSITORY];

        protected string CPPORTAL_USERID => ConfigurationManager.AppSettings[ConfigKeys.CPPORTAL_USERID];
        protected string CPPORTAL_PWD => ConfigurationManager.AppSettings[ConfigKeys.CPPORTAL_PWD];
        protected string Kadamtech_DEWA_Username => ConfigurationManager.AppSettings[ConfigKeys.Kadamtech_DEWA_Username];

        protected string Kadamtech_DEWA_Password => ConfigurationManager.AppSettings[ConfigKeys.Kadamtech_DEWA_Password];
        protected string DEWASMARTVENDORURL => ConfigurationManager.AppSettings[ConfigKeys.DEWASMARTVENDOR_URL];
    }
}
