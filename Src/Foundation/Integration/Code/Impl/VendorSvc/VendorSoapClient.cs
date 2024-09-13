using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.VendorSvc;
using DEWAXP.Foundation.Integration.SmartVendorSvc;
using DEWAXP.Foundation.Integration.Extensions;
using System.Web.Configuration;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.Impl.VendorSvc
{
    [Service(typeof(IVendorServiceClient),Lifetime =Lifetime.Transient)]
    public class VendorSoapClient : BaseDewaGateway, IVendorServiceClient
    {
        public ServiceResponse<byte[]> GetExportRfx(string rfxId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetExportRFX()
                {
                    rfxid = rfxId,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };


                try
                {
                    var rfxListResponse = client.GetExportRFX(request);

                    var typedRfxListResponse = rfxListResponse.@return;

                    if (typedRfxListResponse != null && typedRfxListResponse.Length > 0)
                    {
                        return new ServiceResponse<byte[]>(typedRfxListResponse);
                    }
                    return new ServiceResponse<byte[]>(null, false, "Empty");

                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<byte[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<SRM_OpenInquiries> GetOpenRFXInquiries(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetOpenRFXInquiries()
                {
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };


                try
                {
                    var rfxListResponse = client.GetOpenRFXInquiries(request);

                    var typedRfxListResponse = rfxListResponse.@return.DeserializeAs<SRM_OpenInquiries>();

                    if (typedRfxListResponse.ResponseCode != "000")
                    {
                        return new ServiceResponse<SRM_OpenInquiries>(null, false, typedRfxListResponse.Description);
                    }
                    return new ServiceResponse<SRM_OpenInquiries>(typedRfxListResponse);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SRM_OpenInquiries>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetTenderResultListDataResponse> GetTenderResultList(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var _request = new GetTenderResultList
                {
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()

                };

                try
                {
                    var _response = client.GetTenderResultList(_request);

                    var typedResponse = _response.@return.DeserializeAs<GetTenderResultListDataResponse>();
                    if (typedResponse.ResponseCode != 0 && typedResponse.ResponseCode != 346)
                    {
                        return new ServiceResponse<GetTenderResultListDataResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<GetTenderResultListDataResponse>(typedResponse);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetTenderResultListDataResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetTenderResultDisplayDataResponse> GetTenderResultDisplay(string tenderNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var _request = new GetTenderResultDisplay
                {
                    tendernumber = tenderNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()

                };

                try
                {
                    var _response = client.GetTenderResultDisplay(_request);

                    var typedResponse = _response.@return.DeserializeAs<GetTenderResultDisplayDataResponse>();
                    if (typedResponse.ResponseCode != 0 && typedResponse.ResponseCode != 346)
                    {
                        return new ServiceResponse<GetTenderResultDisplayDataResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<GetTenderResultDisplayDataResponse>(typedResponse);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetTenderResultDisplayDataResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetOpenTenderListDataResponse> GetOpenTenderList(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var _request = new GetOpenTenderList
                {
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()

                };

                try
                {
                    var _response = client.GetOpenTenderList(_request);

                    var typedResponse = _response.@return.DeserializeAs<GetOpenTenderListDataResponse>();
                    if (typedResponse.ResponseCode != 0 && typedResponse.ResponseCode != 346)
                    {
                        return new ServiceResponse<GetOpenTenderListDataResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<GetOpenTenderListDataResponse>(typedResponse);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetOpenTenderListDataResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetFileResponse> GetTenderAdvertisment(string tenderNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetTenderAdvertisement
                {
                    tendernumber = tenderNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };


                try
                {
                    var _response = client.GetTenderAdvertisement(request);

                    //var typedListResponse = _response.@return;

                    if (_response != null)
                    {
                        GetFileResponse myGetFileResponse = new GetFileResponse
                        {
                            filebytes = _response.@return,
                            FileName = tenderNumber+".pdf"
                        };
                        return new ServiceResponse<GetFileResponse>(myGetFileResponse);
                    }

                    return new ServiceResponse<GetFileResponse>(null, false, "Empty");

                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetFileResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<workPermitResponseDetails> GetWorkPermitPass(GetWorkPermitPass input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.Session.lang = language.Code();
                request.Session.appidentifier = segment.Identifier();
                request.Session.appversion = AppVersion;
                request.Session.mobileosversion = AppVersion;
                request.Session.vendorid = GetVendorId(segment);
                try
                {
                    var response = client.GetWorkPermitPass(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<workPermitResponseDetails>(response.@return, true, response.@return.description);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<workPermitResponseDetails>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new System.Exception("Exception in GetWorkPermitPass"), this);
                        return new ServiceResponse<workPermitResponseDetails>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<workPermitResponseDetails>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<workPermitResponseDetails>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<countryListResponse> GetCountryList(GetCountryList input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.lang = language.Code();
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);

                try
                {
                    var response = client.GetCountryList(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<countryListResponse>(response.@return, true, response.@return.description);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<countryListResponse>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new System.Exception("Exception in GetCountryList"), this);
                        return new ServiceResponse<countryListResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<countryListResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<countryListResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<POListResponse> GetPOList(GetPOList input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.lang = language.Code();
                request.appidentifier = segment.Identifier();
                request.appver = AppVersion;
                request.mobileosver = AppVersion;
                request.vendorid = GetVendorId(segment);

                try
                {
                    var _response = client.GetPOList(request);
                    var response = _response.@return.DeserializeAs<POListResponse>();
                    if (response != null && response.ResponseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<POListResponse>(response, true, response.Description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<POListResponse>(null, false, response.Description);
                        return typedresponse;
                    }
                   
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<POListResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<POListResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<subContractorResponse> GetWorkPermitSubContract(GetWorkPermitSubContract input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.Session.lang = language.Code();
                request.Session.appidentifier = segment.Identifier();
                request.Session.appversion = AppVersion;
                request.Session.mobileosversion = AppVersion;
                request.Session.vendorid = GetVendorId(segment);
                try
                {
                    var response = client.GetWorkPermitSubContract(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<subContractorResponse>(response.@return, true, response.@return.description);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<subContractorResponse>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new System.Exception("Exception in GetWorkPermitSubContract"), this);
                        return new ServiceResponse<subContractorResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<subContractorResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<subContractorResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }


        #region Custom binding code

        private CustomBinding CreateBinding()
        {
            var binding = new CustomBinding()
            {
                ReceiveTimeout = TimeSpan.FromMinutes(2),
                SendTimeout = TimeSpan.FromMinutes(1)
            };

            var security = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            security.IncludeTimestamp = true;
            security.LocalClientSettings.MaxClockSkew = new TimeSpan(0, 0, 10, 0);
            security.LocalServiceSettings.MaxClockSkew = new TimeSpan(0, 0, 10, 0);
            security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            security.SecurityHeaderLayout = SecurityHeaderLayout.Lax;
            security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
            security.EnableUnsecuredResponse = true;
            security.AllowInsecureTransport = true;

            var encoding = new TextMessageEncodingBindingElement();
            encoding.MessageVersion = MessageVersion.Soap11;

            var transport = new HttpsTransportBindingElement();
            transport.MaxReceivedMessageSize = 20000000; // 20 megs

            binding.Elements.Add(security);
            binding.Elements.Add(new CustomTextMessageBindingElement());
            binding.Elements.Add(transport);

            return binding;
        }
        #endregion


        #region Service Proxy methods
        private SmartVendorSvc.SmartVendorClient CreateProxy()
        {
            var client = new SmartVendorSvc.SmartVendorClient(CreateBinding(), GetEndpointAddress("SmartVendorServicesPort"));
            client.ChannelFactory.Endpoint.Behaviors.Remove<System.ServiceModel.Description.ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new Helpers.DewaApiCredentials());
            client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["RestAPI_Client_Id"], "Bearer " + OAuthToken.GetAccessToken()));
            client.ClientCredentials.UserName.UserName = BbUsername;
            client.ClientCredentials.UserName.Password = BbPassword;

            return client;
        }
        #endregion
    }
}
