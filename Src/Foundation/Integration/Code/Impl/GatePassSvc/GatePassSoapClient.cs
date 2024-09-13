// <copyright file="GatePassSoapClient.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.DewaSvc
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.GatePassSvc;
    using DEWAXP.Foundation.Integration.Helpers;
    using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
    using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
    using DEWAXP.Foundation.Integration.Responses;
    using Sitecore.Globalization;
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using DEWAXP.Foundation.DI;

    [Service(typeof(IGatePassServiceClient), Lifetime = Lifetime.Transient)]
    /// <summary>
    /// Defines the <see cref="GatePassSoapClient" />
    /// </summary>
    public class GatePassSoapClient : BaseDewaGateway, IGatePassServiceClient
    {
        /// <summary>
        /// The GetGPSupplierDetails
        /// </summary>
        /// <param name="userinput">The userinput<see cref="userDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{supplierDetailsOutput}"/></returns>
        public ServiceResponse<supplierDetailsOutput> GetGPSupplierDetails(userDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                GetGPSupplierDetails request = new GetGPSupplierDetails()
                {
                    userdatainput = userinput
                };
                request.userdatainput.lang = language.Code();
                try
                {
                    GetGPSupplierDetailsResponse response = proxy.GetGPSupplierDetails(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.supplierdetails != null && response.@return.responsecode.Equals("000"))
                        {
                            if (!string.IsNullOrWhiteSpace(response.@return.supplierdetails.validemail))
                            {
                                return new ServiceResponse<supplierDetailsOutput>(response.@return);
                            }
                            return new ServiceResponse<supplierDetailsOutput>(null, false, Translate.Text("epass.entervalidemailiderror"));
                        }
                        return new ServiceResponse<supplierDetailsOutput>(null, false, response.@return.description);
                    }
                    return new ServiceResponse<supplierDetailsOutput>(null, false, "");
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<supplierDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }
        }

        /// <summary>
        /// The CreateGPUser
        /// </summary>
        /// <param name="userinput">The userinput<see cref="createDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        public ServiceResponse<userDetailsOutput> CreateGPUser(createDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                SetCreateGPUser request = new SetCreateGPUser
                {
                    createdatainput = userinput
                };
                request.createdatainput.lang = language.Code();
                try
                {
                    SetCreateGPUserResponse response = proxy.SetCreateGPUser(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<userDetailsOutput>(response.@return);
                        }
                        return new ServiceResponse<userDetailsOutput>(null, false, response.@return.description);
                    }
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The UserLogin
        /// </summary>
        /// <param name="logininput">The logininput<see cref="logInOutDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{loginDetailsOutput}"/></returns>
        public ServiceResponse<loginDetailsOutput> UserLogin(logInOutDataInput logininput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                GetGPUserLogin request = new GetGPUserLogin
                {
                    userdatainput = logininput
                };
                request.userdatainput.merchantid = GetMerchantId(segment);
                request.userdatainput.lang = language.Code();
                request.userdatainput.merchantpassword = GetMerchantPassword(segment);
                try
                {
                    GetGPUserLoginResponse response = proxy.GetGPUserLogin(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<loginDetailsOutput>(response.@return);
                        }
                        return new ServiceResponse<loginDetailsOutput>(null, false, response.@return.description);
                    }
                    return new ServiceResponse<loginDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<loginDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The UserDetails
        /// </summary>
        /// <param name="logininput">The logininput<see cref="userDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        public ServiceResponse<userDetailsOutput> UserDetails(userDataInput logininput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                GetSetGPUserDetails request = new GetSetGPUserDetails
                {
                    userdatainput = logininput
                };
                request.userdatainput.vendorid = GetVendorId(segment);
                request.userdatainput.lang = language.Code();
                try
                {
                    GetSetGPUserDetailsResponse response = proxy.GetSetGPUserDetails(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<userDetailsOutput>(response.@return);
                        }
                        return new ServiceResponse<userDetailsOutput>(null, false, response.@return.description);
                    }
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The UserLogout
        /// </summary>
        /// <param name="logininput">The logininput<see cref="logInOutDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{logoutDetailsOutput}"/></returns>
        public ServiceResponse<logoutDetailsOutput> UserLogout(logInOutDataInput logininput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                GetGPUserLogout request = new GetGPUserLogout
                {
                    userdatainput = logininput
                };
                request.userdatainput.lang = language.Code();
                try
                {
                    GetGPUserLogoutResponse response = proxy.GetGPUserLogout(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<logoutDetailsOutput>(response.@return);
                        }
                        return new ServiceResponse<logoutDetailsOutput>(null, false, response.@return.description);
                    }
                    return new ServiceResponse<logoutDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<logoutDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The UserActivation
        /// </summary>
        /// <param name="userinput">The userinput<see cref="userDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        public ServiceResponse<userDetailsOutput> UserActivation(userDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                SetGPUserActivation request = new SetGPUserActivation
                {
                    userdatainput = userinput
                };
                request.userdatainput.lang = language.Code();
                try
                {
                    SetGPUserActivationResponse response = proxy.SetGPUserActivation(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<userDetailsOutput>(response.@return);
                        }
                        return new ServiceResponse<userDetailsOutput>(null, false, response.@return.description);
                    }
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The ForgotUserid
        /// </summary>
        /// <param name="userinput">The userinput<see cref="credentialDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        public ServiceResponse<userDetailsOutput> ForgotUserid(credentialDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                GetGPForgotUserID request = new GetGPForgotUserID
                {
                    createdatainput = userinput
                };
                request.createdatainput.lang = language.Code();
                try
                {
                    GetGPForgotUserIDResponse response = proxy.GetGPForgotUserID(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<userDetailsOutput>(response.@return);
                        }
                    }
                    return new ServiceResponse<userDetailsOutput>(new userDetailsOutput(), true, Translate.Text("You will receive if it is valid user"));
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The ForgotPassword
        /// </summary>
        /// <param name="userinput">The userinput<see cref="credentialDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        public ServiceResponse<userDetailsOutput> ForgotPassword(credentialDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                SetGPPasswordReset request = new SetGPPasswordReset
                {
                    createdatainput = userinput
                };
                request.createdatainput.lang = language.Code();
                try
                {
                    SetGPPasswordResetResponse response = proxy.SetGPPasswordReset(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<userDetailsOutput>(response.@return);
                        }
                    }
                    return new ServiceResponse<userDetailsOutput>(new userDetailsOutput(), true, Translate.Text("You will receive if it is valid user"));
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The SetNewPassword
        /// </summary>
        /// <param name="userinput">The userinput<see cref="credentialDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        public ServiceResponse<userDetailsOutput> SetNewPassword(credentialDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                SetGPPasswordValidate request = new SetGPPasswordValidate
                {
                    createdatainput = userinput
                };
                request.createdatainput.lang = language.Code();
                try
                {
                    SetGPPasswordValidateResponse response = proxy.SetGPPasswordValidate(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<userDetailsOutput>(response.@return);
                        }
                        return new ServiceResponse<userDetailsOutput>(null, false, response.@return.description);
                    }
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The ChangePassword
        /// </summary>
        /// <param name="userinput">The userinput<see cref="credentialDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{userDetailsOutput}"/></returns>
        public ServiceResponse<userDetailsOutput> ChangePassword(credentialDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                SetGPPasswordChange request = new SetGPPasswordChange
                {
                    createdatainput = userinput
                };
                request.createdatainput.vendorid = GetVendorId(segment);
                request.createdatainput.lang = language.Code();
                try
                {
                    SetGPPasswordChangeResponse response = proxy.SetGPPasswordChange(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<userDetailsOutput>(response.@return);
                        }
                        return new ServiceResponse<userDetailsOutput>(null, false, response.@return.description);
                    }
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<userDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The ProjectList
        /// </summary>
        /// <param name="userinput">The userinput<see cref="userDataInput"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{poDetailsOutput}"/></returns>
        public ServiceResponse<poDetailsOutput> ProjectList(userDataInput userinput, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (GatePassClient proxy = CreateProxy())
            {
                GetGPPODetails request = new GetGPPODetails
                {
                    userdatainput = userinput
                };
                request.userdatainput.vendorid = GetVendorId(segment);
                request.userdatainput.lang = language.Code();
                try
                {
                    GetGPPODetailsResponse response = proxy.GetGPPODetails(request);
                    if (response != null && response.@return != null)
                    {
                        if (!string.IsNullOrWhiteSpace(response.@return.responsecode) && response.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<poDetailsOutput>(response.@return);
                        }
                        return new ServiceResponse<poDetailsOutput>(null, false, response.@return.description);
                    }
                    return new ServiceResponse<poDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<poDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The CreateProxy
        /// </summary>
        /// <returns>The <see cref="GatePassClient"/></returns>
        private GatePassClient CreateProxy()
        {
            GatePassClient client = new GatePassClient(CreateBinding(), GetEndpointAddress("GatePassPort"));
            client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new DubaiModelServiceCredentials());
            client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Client_Id_Epass], "Bearer " + OAuthToken.GetAccessToken(ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Oauth_URL_Epass], ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Client_Id_Epass], ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Client_Secret_Epass])));
            client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings[ConfigKeys.Gatepass_UN];
            client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings[ConfigKeys.Gatepass_PWD];
            return client;
        }

        /// <summary>
        /// The CreateBinding
        /// </summary>
        /// <returns>The <see cref="CustomBinding"/></returns>
        private CustomBinding CreateBinding()
        {
            CustomBinding binding = new CustomBinding()
            {
                ReceiveTimeout = TimeSpan.FromMinutes(2),
                SendTimeout = TimeSpan.FromMinutes(2)
            };
            TransportSecurityBindingElement security = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            security.IncludeTimestamp = true;
            security.LocalClientSettings.MaxClockSkew = new TimeSpan(0, 0, 10, 0);
            security.LocalServiceSettings.MaxClockSkew = new TimeSpan(0, 0, 10, 0);
            security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            security.SecurityHeaderLayout = SecurityHeaderLayout.Lax;
            security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
            security.EnableUnsecuredResponse = true;
            security.AllowInsecureTransport = true;
            TextMessageEncodingBindingElement encoding = new TextMessageEncodingBindingElement
            {
                MessageVersion = MessageVersion.Soap11
            };
            HttpsTransportBindingElement transport = new HttpsTransportBindingElement
            {
                MaxReceivedMessageSize = 20000000 // 20 megs
            };
            binding.Elements.Add(security);
            binding.Elements.Add(new CustomTextMessageBindingElement());
            binding.Elements.Add(transport);
            return binding;
        }
    }
}
