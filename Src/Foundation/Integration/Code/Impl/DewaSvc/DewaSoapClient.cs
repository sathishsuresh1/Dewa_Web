using DEWAXP.Foundation.Integration.Helpers;
using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Integration.Responses;
using Exception = System.Exception;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using System.ServiceModel.Dispatcher;
using System.Web.Configuration;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;
using System.Text;
using DEWAXP.Foundation.Integration.Responses.PowerOutage;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.Impl.DewaSvc
{
    [Service(typeof(IDewaServiceClient), Lifetime = Lifetime.Transient)]
    public class DewaSoapClient : BaseDewaGateway, IDewaServiceClient
    {
        public ServiceResponse<UserIdentifierAvailabilityResponse> VerifyUserIdentifierAvailable(string userId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetUserIDCheck
                {
                    userid = userId,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetUserIDCheck(request);
                    var typedResponse = response.@return.DeserializeAs<UserIdentifierAvailabilityResponse>();

					return new ServiceResponse<UserIdentifierAvailabilityResponse>(typedResponse, true, typedResponse.Description);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<UserIdentifierAvailabilityResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<byte[]> GetAccountImage(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetImageContractAccount
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetImageContractAccount(request);

					return new ServiceResponse<byte[]>(response.@return);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<byte[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public async Task<ServiceResponse<byte[]>> GetAccountImageAsync(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetImageContractAccount
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = await client.GetImageContractAccountAsync(request);

					return new ServiceResponse<byte[]>(response.GetImageContractAccountResponse.@return);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<byte[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<string> Authenticate(string userId, string password, bool governmental = false, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            if (governmental)
            {
                return SignInAsGovernmentalUser(userId, password, language, segment);
            }
            return new ServiceResponse<string>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
            //return SignInAsCustomer(userId, password, language, segment);
        }

        //public ServiceResponse<LoginResponse> AuthenticateNew(string userId, string password, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string center = null)
        //{
        //    return SignInAsCustomerNew(userId, password, language, segment, center);
        //}

        public ServiceResponse<MyIdAuthenticationResponse> Authenticate(string emiratesIdentifier, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new getMyID
                {
                    myid = emiratesIdentifier,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    merchantid = GetMerchantId(segment),
                    merchantpassword = GetMerchantPassword(segment),
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    mobileosversion = "dsa"
                };

                try
                {
                    var response = client.getMyID(request);
                    var typedResponse = UnwrapMyIdSoapResponse(response);
                    //var typedResponse = unwrapped.DeserializeAs<MyIdAuthenticationResponse>();
                    if (typedResponse != null && (typedResponse.ResponseCode.Equals(391) || typedResponse.ResponseCode.Equals(392)))
                    {
                        return new ServiceResponse<MyIdAuthenticationResponse>(typedResponse);
                    }
                    return new ServiceResponse<MyIdAuthenticationResponse>(null, false, typedResponse.Description);
                }
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<MyIdAuthenticationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse Logout(string userId, string sessionId, UserType userType = UserType.Customer, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            {
                using (var client = CreateProxy())
                {
                    var request = new GetUserLogout
                    {
                        userid = userId,
                        sessionid = sessionId,
                        usertype = userType.Code(),
                        merchantid = GetMerchantId(segment),
                        merchantpassword = GetMerchantPassword(segment),
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        lang = language.Code()
                    };

                    try
                    {
                        var response = client.GetUserLogout(request);

						var typedResponse = response.@return.DeserializeAs<LogoutResponse>();
						if (typedResponse.ResponseCode != 0)
						{
							return new ServiceResponse(false, typedResponse.Description);
						}
						return new ServiceResponse();
					}
					catch (Exception ex)
					{
                        LogService.Fatal(ex, this);
                        return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
					}
				}
			}
		}

        private MyIdAuthenticationResponse UnwrapMyIdSoapResponse(getMyIDResponse response)
        {
            const string xmlDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            MyIdAuthenticationResponse model = null;
            if (!string.IsNullOrWhiteSpace(response.@return))
            {
                var trimmed = response.@return.Replace(xmlDeclaration, string.Empty);
                var doc = XDocument.Parse(trimmed);
                try
                {
                    var encodedResponse = Encoding.UTF8.GetBytes(doc.ToString());
                    using (var input = new MemoryStream(encodedResponse))
                    {
                        XmlReaderSettings oXmlReaderSettings = new XmlReaderSettings();
                        oXmlReaderSettings.ValidationType = ValidationType.Schema;
                        var oSchema = StringExtensions.GetSchemaFromType(typeof(Envelope));
                        oXmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                        oXmlReaderSettings.ValidationEventHandler += (sender, args) =>
                        {
                            throw args.Exception;
                        };
                        oXmlReaderSettings.Schemas.Add(oSchema);
                        XmlReader oXmlReader = null;
                        if (oSchema != null)
                        {
                            oXmlReader = XmlReader.Create(input, oXmlReaderSettings);
                        }
                        try
                        {
                            XmlSerializer oXmlSerializer = new XmlSerializer(typeof(Envelope));
                            var Enveloperesponse = (Envelope)oXmlSerializer.Deserialize(oXmlReader);
                            return Enveloperesponse?.Body?.GetMyIDResponse?.Response?.GetMyID;
                        }
                        catch (Exception ex)
                        {
                            LogService.Fatal(ex, new object());
                        }
                        finally
                        {
                            if (oXmlReader != null)
                                oXmlReader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, new object());
                }

                /*
                <SOAP-ENV:Envelope
                    xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/"
                    xmlns:xs="http://www.w3.org/2001/XMLSchema"
                    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" >
                    <SOAP-ENV:Body>
                        <rpl:getMyIDResponse
                            xmlns:rpl='urn:MyIDWS'>
                            <rpl:Response>
                                <?xml version="1.0" encoding="UTF-8"?>
                                <GetMyID>
                                    <ResponseCode>391</ResponseCode>
                                    <Description>Valid Login to DEWA</Description>
                                    <Credential>BBMO0000000000787793</Credential>
                                </GetMyID>
                            </rpl:Response>
                        </rpl:getMyIDResponse>
                    </SOAP-ENV:Body>
                </SOAP-ENV:Envelope>
                */
            }
            return model;
        }

        public ServiceResponse ResetPassword(string userId, string email, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetPasswordReset()
                {
                    userid = userId,
                    emailid = email,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.SetPasswordReset(request);

                    var typedResponse = response.@return.DeserializeAs<PasswordResetResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse(false, typedResponse.Description);
                    }
                    return new ServiceResponse();
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse SetNewPassword(string userId, string sessionToken, string flag, string password, string confirmPassword, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetCustomerPasswordValidate()
                {
                    userid = userId,
                    token = sessionToken,
                    flag = flag,
                    password = password,
                    confirmpassword = confirmPassword,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.SetCustomerPasswordValidate(request);

                    var typedResponse = response.@return.DeserializeAs<SetNewPasswordResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse(false, typedResponse.Description);
                    }
                    return new ServiceResponse();
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse ChangePassword(string userId, string sessionId, string oldPassword, string newPassword, string confirmPassword, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetPasswordChange
                {
                    sessionid = sessionId,
                    userid = userId,
                    oldpassword = oldPassword,
                    newpassword = newPassword,
                    confirmpassword = confirmPassword,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    vendorid = GetVendorId(segment)
                };

                try
                {
                    var response = client.SetPasswordChange(request);

                    var typedResponse = response.@return.DeserializeAs<ChangePasswordResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse(false, typedResponse.Description);
                    }
                    return new ServiceResponse();
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse RequestUserIdentifierReminder(string bpNumber, string email, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetForgotUserID()
                {
                    businesspartnernumber = bpNumber,
                    emailid = email,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetForgotUserID(request);

                    var typedResponse = response.@return.DeserializeAs<UserIdentifierReminderResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse(false, typedResponse.Description);
                    }
                    return new ServiceResponse();
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<SetRateCategoryResponse> ChangeCustomerCategory(string userId, string sessionId, string premiseNumber, string contractAccountNumber, string bpNumber, string mobile, string remarks, byte[] attachment, string attachmentType, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetRateCategoryRequest()
                {
                    userid = userId,
                    sessionid = sessionId,
                    businesspartnernumber = bpNumber.AssertAccountNumberPrefix(),
                    contractaccountnumber = contractAccountNumber,
                    premisenumber = premiseNumber.AssertAccountNumberPrefix(),
                    remarks = remarks ?? string.Empty,
                    mobilenumber = mobile,
                    attachmenttype = attachmentType,
                    attachment = attachment,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    requesttype = string.Empty
                };

				try
				{
					var response = client.SetRateCategoryRequest(request);
					var typedResponse = response.@return.DeserializeAs<SetRateCategoryResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<SetRateCategoryResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<SetRateCategoryResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetRateCategoryResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

		public ServiceResponse<MarketingEmailDetails[]> GetEmailListForMobileNumber(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
		{
			using (var client = CreateProxy())
			{
				var request = new GetEmailListForMobileNumber()
				{
					userid = userId,
					sessionid = sessionId,
					lang = language.Code(),
					vendorid = GetVendorId(segment)
				};
				try
				{
					var response = client.GetEmailListForMobileNumber(request);
					var typedResponse = response.@return.DeserializeAs<MarketingEmailListResponse>();
					if (typedResponse != null && typedResponse.ListOfEmails != null && typedResponse.ListOfEmails.Account != null)
					{
						return new ServiceResponse<MarketingEmailDetails[]>(typedResponse.ListOfEmails.Account.ToArray());
					}
					else
					{
						return new ServiceResponse<MarketingEmailDetails[]>(null, false, typedResponse.Description);
					}
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<MarketingEmailDetails[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}

            }
        }

        public ServiceResponse SetMarketingPreferenceEmail(string userId, string sessionId, string email, string subscriptionFlag, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetMarketingPreferenceSMS()
                {
                    userid = userId,
                    sessionid = sessionId,
                    emailid = email,
                    subscriptionflag = subscriptionFlag,
                    lang = language.Code()
                };

                try
                {
                    var response = client.SetMarketingPreferenceSMS(request);

                    var typedResponse = response.@return.DeserializeAs<MarketingPreferenceEmailResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse(false, typedResponse.Description);
                    }
                    return new ServiceResponse();
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<AccountDetails[]> GetAccountList(string userId, string sessionId, bool includeBillingDetails = false, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetContractAccounts
                {
                    userid = userId,
                    sessionid = sessionId,
                    lang = language.Code(),
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion
                };

                try
                {
                    var response = client.GetContractAccounts(request);
                    var typedResponse = response.@return.DeserializeAs<ContractAccountListResponse>();
                    if (typedResponse.responseCode != "0")
                    {
                        return new ServiceResponse<AccountDetails[]>(null, false, typedResponse.responseMessage);
                    }
                    if (typedResponse.ContractAccounts == null && !includeBillingDetails)
                    {
                        return new ServiceResponse<AccountDetails[]>(new AccountDetails[0], false, "NoneActive");
                    }
                    if (!includeBillingDetails)
                    {
                        var payload = typedResponse.ContractAccounts.Select(acc => new AccountDetails
                        {
                            AccountNumber = acc.AccountNumber,
                            PremiseNumber = acc.PremiseNumber,
                            CustomerPremiseNumber = acc.LegacyAccountNumber,
                            BusinessPartnerNumber = acc.BusinessPartnerNumber,
                            BillingClassCode = acc.BillingClassCode,
                            Category = acc.Category,
                            AccountName = acc.Name,
                            NickName = acc.Nickname,
                            IsActive = acc.IsActive,
                            PhotoIndicator = acc.PhotoIndicator,
                        }).ToArray();

                        return new ServiceResponse<AccountDetails[]>(payload);
                    }

                    var detailedListRequest = new GetContractAccountsList
                    {
                        userid = userId,
                        sessionid = sessionId,
                        vendorid = GetVendorId(segment),
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        lang = language.Code()
                    };

                    var detailedListResponse = client.GetContractAccountsList(detailedListRequest);

                    var typedDetailResponse = detailedListResponse.@return.DeserializeAs<DetailedContractAccountListResponse>();
                    if (typedDetailResponse.responseCode != "0")
                    {
                        return new ServiceResponse<AccountDetails[]>(null, false, typedDetailResponse.responseMessage);
                    }

                    foreach (var account in typedDetailResponse.ContractAccounts)
                    {
                        if (typedResponse.ContractAccounts != null)
                        {
                            var matchingDetails = typedResponse.ContractAccounts.SingleOrDefault(acc => acc.AccountNumber.Equals(account.AccountNumber));
                            if (matchingDetails != null)
                            {
                                account.PremiseNumber = matchingDetails.PremiseNumber;
                                account.CustomerPremiseNumber = matchingDetails.LegacyAccountNumber;
                                account.Category = matchingDetails.Category;
                                account.IsActive = true;
                            }
                        }

                    }
                    return new ServiceResponse<AccountDetails[]>(typedDetailResponse.ContractAccounts.ToArray());
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<AccountDetails[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<AccountDetails[]> GetCAList(string userId, string sessionId, string checkMoveOut, string ServiceFlag, bool includeInactive = false, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    if (!includeInactive)
                    {
                        #region Getting Active Accounts
                        var request = new GetContractAccounts
                        {
                            userid = userId,
                            sessionid = sessionId,
                            lang = language.Code(),
                            vendorid = GetVendorId(segment),
                            appidentifier = segment.Identifier(),
                            appversion = AppVersion,

                            checknotification = checkMoveOut
                        };

                        var response = client.GetContractAccounts(request);
                        var typedResponse = response.@return.DeserializeAs<ContractAccountListResponse>();
                        if (typedResponse.responseCode == "0")
                        {
                            string activeCode = Convert.ToInt32(AccountClassification.Active).ToString("00");
                            var payload = typedResponse.ContractAccounts.Select(acc => new AccountDetails
                            {
                                AccountNumber = acc.AccountNumber,
                                PremiseNumber = acc.PremiseNumber,
                                CustomerPremiseNumber = acc.LegacyAccountNumber,
                                BusinessPartnerNumber = acc.BusinessPartnerNumber,
                                BillingClassCode = acc.BillingClassCode,
                                Category = acc.Category,
                                AccountName = acc.Name,
                                NickName = acc.Nickname,
                                BPName = acc.BPName,
                                IsActive = acc.IsActive,
                                PhotoIndicator = acc.PhotoIndicator,
                                CustomerType = acc.CustomerType,
                                NotificationNumber = acc.NotificationNumber,
                                AccountCategory = acc.AccountCategory,
                                PremiseType = acc.PremiseType,
                                Street = acc.Street,
                                Location = acc.Location,
                                AccountStatusCode = activeCode,
                                POD = acc.POD,
                                Medical = acc.Medical,
                                Senior = acc.Senior,
                                XCordinate = acc.XCordinate,
                                YCordinate = acc.YCordinate
                            }).ToArray();

                            return new ServiceResponse<AccountDetails[]>(payload);
                        }
                        else
                        {
                            return new ServiceResponse<AccountDetails[]>(null, false, typedResponse.responseMessage);
                        }
                        //if (typedResponse.ContractAccounts == null && !includeInactive)
                        //{
                        //    return new ServiceResponse<AccountDetails[]>(new AccountDetails[0], false, "NoneActive");
                        //}
                        #endregion
                    }
                    else
                    {
                        #region Getting Active and Inactive Accounts
                        var request = new GetContractAccountsList
                        {
                            userid = userId,
                            sessionid = sessionId,
                            vendorid = GetVendorId(segment),
                            appidentifier = segment.Identifier(),
                            serviceflag = ServiceFlag,
                            appversion = AppVersion,
                            lang = language.Code()
                        };

                        var response = client.GetContractAccountsList(request);
                        var typedDetailResponse = response.@return.DeserializeAs<DetailedContractAccountListResponse>();
                        if (typedDetailResponse.responseCode == "0")
                        {
                            typedDetailResponse.ContractAccounts.Where(c => c.AccountStatusCode == "00").ToList().ForEach(cc => cc.IsActive = true);
                            return new ServiceResponse<AccountDetails[]>(typedDetailResponse.ContractAccounts.ToArray());
                        }
                        else
                        {
                            return new ServiceResponse<AccountDetails[]>(null, false, typedDetailResponse.responseMessage);
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<AccountDetails[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }


        public ServiceResponse<ContractAccountPaymentListResponse> GetTransactionHistory(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCAPayments
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    others = userId,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetCAPayments(request);

                    var typedResponse = response.@return.DeserializeAs<ContractAccountPaymentListResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<ContractAccountPaymentListResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<ContractAccountPaymentListResponse>(typedResponse);
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ContractAccountPaymentListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<Receipt[]> GetReceipts(string userId, string sessionId, string degTransactionId, string dewaTransactionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetBillPaymentReceipt
                {
                    userid = userId,
                    sessionid = sessionId,
                    degtransactionid = degTransactionId,
                    dewatransactionid = dewaTransactionId,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };


                try
                {
                    var response = client.GetBillPaymentReceipt(request);

					var typedResponse = response.@return.DeserializeAs<BillReceiptResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<Receipt[]>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<Receipt[]>(typedResponse.Receipts.ToArray());
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<Receipt[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<CustomerEnquiry[]> GetCustomerEnquiries(string userId, string sessionId, string bpNumber, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string others = null)
        {
            using (var client = CreateProxy())
            {
                var request = new GetComplaintsList
                {
                    userid = userId,
                    sessionid = sessionId,
                    businesspartnernumber = bpNumber,
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    others = others
                };

                try
                {
                    var response = client.GetComplaintsList(request);

					var typedResponse = response.@return.DeserializeAs<CustomerEnquiryListResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<CustomerEnquiry[]>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<CustomerEnquiry[]>(typedResponse.EnquiryList.Enquiries.ToArray());
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<CustomerEnquiry[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<CustomerEnquiry> GetCustomerEnquiry(string enquiryNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetComplaintsDetails
                {
                    complaintnumber = enquiryNumber,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetComplaintsDetails(request);

					var typedResponse = response.@return.DeserializeAs<CustomerEnquiryDetailsResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<CustomerEnquiry>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<CustomerEnquiry>(typedResponse.CustomerEnquiryDetails.Enquiry);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<CustomerEnquiry>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<BillEnquiryResponse> GetBill(string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string center = null, string sessionId = null)
        {
            using (var client = CreateProxy())
            {
                var request = new GetBillEnquiry
                {
                    contractaccountnumber = contractAccountNumber,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    center = center,
                    sesionid = sessionId,
                    vendorid = GetVendorId(segment)
                };

                try
                {
                    var response = client.GetBillEnquiry(request);

					var typedResponse = response.@return.DeserializeAs<BillEnquiryResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<BillEnquiryResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<BillEnquiryResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<BillEnquiryResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<byte[]> GetBill(string userId, string sessionId, string invoiceNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new getBillDetailsData
                {
                    userid = userId,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    invoicenumber = invoiceNumber,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.getBillDetailsData(request);

					return new ServiceResponse<byte[]>(response.@return);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<byte[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<OpenRfxInquiriesResponse> GetOpenRfxInquiries(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            //TODO: FINISH WHEN METHOD AVAILABLE
            using (var client = CreateProxy())
            {
                var request = new GetSlabTariffColorCodes();
            }

            return new ServiceResponse<OpenRfxInquiriesResponse>(null, false);
        }

        public ServiceResponse<PrimaryAccountResponse> GetPrimaryAccount(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetPrimaryContractAccountUX()
                {
                    userid = userId,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetPrimaryContractAccountUX(request);

					var typedResponse = response.@return.DeserializeAs<PrimaryAccountResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<PrimaryAccountResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<PrimaryAccountResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<PrimaryAccountResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse SetPrimaryAccount(string userId, string sessionId, string contractAccountNumber, string terms, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetPrimaryContractAccountUX
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    termsandcondition = terms
                };

                try
                {
                    var response = client.SetPrimaryContractAccountUX(request);

					var typedResponse = response.@return.DeserializeAs<SetPrimaryAccountResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse(false, typedResponse.Description);
					}
					return new ServiceResponse();
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<AccountContactDetails> GetContactDetails(string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCAContactDetails
                {
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    sessionid = sessionId,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetCAContactDetails(request);

					var typedResponse = response.@return.DeserializeAs<ContactDetailsResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<AccountContactDetails>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<AccountContactDetails>(typedResponse.ContactDetails);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<AccountContactDetails>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse ChangeContactDetails(string userId, string sessionId, ChangeContactDetails @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetUpdateContactDetails
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccountnumber = @params.ContractAccountNumber,
                    vendorid = GetVendorId(segment),
                    businesspartnernumber = @params.BusinessPartnerNumber,
                    emailid = @params.EmailAddress,
                    faxnumber = @params.FaxNumber,
                    poboxnumber = @params.PoBox,
                    mobilenumber = @params.MobileNumber,
                    region = @params.Emirate,
                    nickname = @params.NickName,
                    telephonenumber = @params.TelephoneNumber,
                    preferredlang = @params.PreferredLanguage.Code(),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.SetUpdateContactDetails(request);

					var typedResponse = response.@return.DeserializeAs<ChangeContactDetailsResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse(false, typedResponse.Description);
					}
					return new ServiceResponse();
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<string> RequestFinalBill(string userId, string sessionId, RequestFinalBill @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetFinalBillDetails
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccountnumber = @params.ContractAccountNumber,
                    businesspartnernumber = @params.BusinessPartnerNumber,
                    premisenumber = @params.PremiseNumber,
                    attachment = @params.Attachment,
                    attachmenttype = @params.AttachmentExtension,
                    mobilenumber = @params.MobileNumber,
                    ibannumber = @params.IbanNumber,
                    ibanrefund = @params.IbanRefund,
                    refundmode = @params.RefundMode,
                    disconnectiondate = @params.DisconnectionDate.HasValue ? @params.DisconnectionDate.Value.ToString("yyyyMMdd") : string.Empty,
                    remarks = @params.Remarks,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetFinalBillDetails(request);

					var typedResponse = response.@return.DeserializeAs<FinalBillResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<string>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<string>(typedResponse.Reference);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<string> LodgeBillingComplaint(string userId, string sessionId, LodgeBillingComplaint @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new getHighLowComplaints
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccountnumber = @params.ContractAccountNumber,
                    businesspartnernumber = @params.BusinessPartnerNumber,
                    premisenumber = @params.PremiseNumber.AssertAccountNumberPrefix(),
                    attachment = @params.Attachment,
                    attachmenttype = @params.AttachmentExtension,
                    mobilenumber = @params.MobileNumber,
                    waterelectricity = @params.AffectedService.Code(),
                    highlow = @params.Priority.Code(),
                    remarks = @params.Remarks,
                    lang = language.Code(),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    vendorid = GetVendorId(segment)
                };

                try
                {
                    var response = client.getHighLowComplaints(request);

					var typedResponse = response.@return.DeserializeAs<BillingComplaintResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<string>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<string>(typedResponse.Reference);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<ClearanceCertificateDetails> GetClearanceCertificate(string userid, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string center = null)
        {
            using (var client = CreateProxy())
            {
                var request = new GetClearanceCertificateDetails
                {
                    contractaccountnumber = contractAccountNumber,
                    sessionid = sessionId,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    userid = userid,
                    vendorid = GetVendorId(segment),
                    center = center
                };

                try
                {

                    var response = client.GetClearanceCertificateDetails(request);

					var typedResponse = response.@return.DeserializeAs<ClearanceCertificateDetails>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<ClearanceCertificateDetails>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<ClearanceCertificateDetails>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ClearanceCertificateDetails>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<ClearanceCertificateApplicationResponse> RequestClearanceCertificate(string userId, string sessionId, RequestClearanceCertificate @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string center = null)
        {
            using (var client = CreateProxy())
            {
                var request = new SetClearanceCertificate
                {
                    userid = userId,
                    sessionid = sessionId,
                    firstname = @params.FirstName,
                    middlename = string.Empty,
                    lastname = @params.LastName,
                    emailid = @params.EmailAddress,
                    mobilenumber = @params.MobileNumber,
                    emiratesidnumber = @params.IdentityNumber ?? string.Empty,
                    tradelicensenumber = @params.TradeLicenseNumber ?? string.Empty,
                    pobox = @params.PoBox,
                    city = @params.City,
                    purpose = @params.Purpose,
                    remarks = @params.Remarks,
                    contractaccountnumber = @params.ContractAccountNumber,
                    attachment1 = @params.Attachment1,
                    attachmentname1 = @params.Attachment1Extension,
                    attachment2 = @params.Attachment2,
                    attachmentname2 = @params.Attachment2Extension,
                    attachment3 = @params.Attachment3,
                    attachmentname3 = @params.Attachment3Extension,
                    attachment4 = @params.Attachment4 ?? new byte[0],
                    attachmentname4 = @params.Attachment4Extension ?? string.Empty,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    center = center
                };

                try
                {
                    var response = client.SetClearanceCertificate(request);

					var typedResponse = response.@return.DeserializeAs<ClearanceCertificateApplicationResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<ClearanceCertificateApplicationResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<ClearanceCertificateApplicationResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ClearanceCertificateApplicationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        // TODO [AvdM]: This service operation is currently returning an error. Have contacted BB for resolution.
        public ServiceResponse GetCityList(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCityList
                {
                    userid = userId,
                    sessionid = sessionId,
                    lang = language.Code(),
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    city = "X"
                };

                var response = client.GetCityList(request);

                //var typedResponse = response.@return.DeserializeAs<SetPrimaryAccountResponse>();
                //if (typedResponse.ResponseCode != 0)
                //{
                //    return new ServiceResponse(false, typedResponse.Description);
                //}
                return new ServiceResponse();
            }
        }

        public ServiceResponse<TemporaryConnectionApplicationResponse> RequestTemporaryConnection(string userId, string sessionId, RequestTemporaryConnection @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetTempConnectionRequest
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccountnumber = @params.ContractAccountNumber,
                    mobilenumber = @params.MobileNumber,
                    city = @params.City,
                    text = @params.Remarks,
                    startdate = @params.Start.ToString("yyyyMMdd"),
                    starttime = @params.Start.ToString("hhmmss"),
                    enddate = @params.End.ToString("yyyyMMdd"),
                    endtime = @params.End.ToString("hhmmss"),
                    eventtype = @params.EventType.Code(),
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.SetTempConnectionRequest(request);

					var typedResponse = response.@return.DeserializeAs<TemporaryConnectionApplicationResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<TemporaryConnectionApplicationResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<TemporaryConnectionApplicationResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<TemporaryConnectionApplicationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<TemporaryConnectionDetails> GetTemporaryConnectionDetails(string userId, string sessionId, string reference, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetTemporaryConnectionDetails
                {
                    userid = userId,
                    sessionid = sessionId,
                    notification = reference,
                    lang = language.Code(),
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                };

                try
                {
                    var response = client.GetTemporaryConnectionDetails(request);

					var typedResponse = response.@return.DeserializeAs<TemporaryConnectionDetails>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<TemporaryConnectionDetails>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<TemporaryConnectionDetails>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<TemporaryConnectionDetails>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<ComplaintCriteriaResponse> GetServiceComplaintCriteria(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetComplaintsMaster
                {
                    city = "X",
                    codegroup = "X",
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetComplaintsMaster(request);

                    var typedResponse = response.@return.DeserializeAs<ComplaintCriteriaResponse>();

					return new ServiceResponse<ComplaintCriteriaResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ComplaintCriteriaResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<string> LodgeServiceComplaint(string userId, string sessionId, LodgeServiceComplaint @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            const string filename = "complaints.jpg";

            using (var client = CreateProxy())
            {
                var request = new SetLogComplaints
                {
                    userId = userId,
                    sessionid = sessionId,
                    mobile = @params.MobileNumber,
                    name = @params.Name,
                    contractaccountnumber = @params.ContractAccountNumber,
                    city = @params.City,
                    code = @params.Code,
                    codegroup = @params.CodeGroup,
                    text = @params.Remarks,
                    xgps = @params.GpsXCoordinates,
                    ygps = @params.GpsYCoordinates,
                    lang = language.Code(),
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                };

                if (@params.DocumentData.Length > 0)
                {
                    request.docstream = @params.DocumentData;
                    request.filename = filename;
                }

                try
                {
                    var response = client.SetLogComplaints(request);

					var typedResponse = response.@return.DeserializeAs<LodgeServiceComplaintResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<string>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<string>(typedResponse.Reference);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<YearlyConsumptionDataResponse> GetConsumptionHistory(string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCAConsumptionEWYearly
                {
                    sessionid = sessionId,
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetCAConsumptionEWYearly(request);

					var typedResponse = response.@return.DeserializeAs<YearlyConsumptionDataResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<YearlyConsumptionDataResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<YearlyConsumptionDataResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<YearlyConsumptionDataResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<ComparativeConsumptionResponse> GetComparativeConsumption(string sessionId, string[] contractAccountNumbers, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCAConsumptionEW
                {
                    sessionid = sessionId,
                    contractaccountnumber = string.Join(",", contractAccountNumbers),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetCAConsumptionEW(request);
                    var trimmedXml = Regex.Replace(response.@return, "(_[0-9]+)", string.Empty);

					var typedResponse = trimmedXml.DeserializeAs<ComparativeConsumptionResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<ComparativeConsumptionResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<ComparativeConsumptionResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ComparativeConsumptionResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<YearlyAverageConsumptionDataResponse> GetComparativeConsumption(string behaviourIndicator, string sessionId, string contractAccountNumber, string premiseNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCAConsumptionEWAverage
                {
                    sessionid = sessionId,
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    legacyaccountnumber = premiseNumber,//.AssertAccountNumberPrefix(),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    behaviourindicator = behaviourIndicator

                };

                try
                {
                    var response = client.GetCAConsumptionEWAverage(request);

					var typedResponse = response.@return.DeserializeAs<YearlyAverageConsumptionDataResponse>();
					if (typedResponse.ResponseCode != 0 && typedResponse.ResponseCode != 346)
					{
						return new ServiceResponse<YearlyAverageConsumptionDataResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<YearlyAverageConsumptionDataResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<YearlyAverageConsumptionDataResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<decimal> GetCarbonFootprintReading(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCO2Footprint
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetCO2Footprint(request);

					var typedResponse = response.@return.DeserializeAs<CarbonFootprintResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<decimal>(0.00m, false, typedResponse.Description);
					}
					return new ServiceResponse<decimal>(typedResponse.Footprint);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<decimal>(0.00m, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<GovermentUserDetailsResponse> GetGovernmentalUserDetails(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetGovtUserDetails
                {
                    Userid = userId,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    appver = AppVersion,
                    appidentifier = segment.Identifier()
                };

                try
                {
                    var response = client.GetGovtUserDetails(request);

					var typedResponse = response.@return.DeserializeAs<GovermentUserDetailsResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<GovermentUserDetailsResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<GovermentUserDetailsResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GovermentUserDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<GetGovernmentObservationDetailsResponse> SubmitGovernmentObservation(string userId, string sessionId, SubmitGovernmentObservation @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetGovtObservationDetails
                {
                    userid = userId,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    mobileosversion = string.Empty,
                    appversion = AppVersion,
                    appidentifier = segment.Identifier(),
                    area = @params.Area,
                    community = @params.Community ?? string.Empty,
                    contractaccount = @params.ContactAccountNumber,
                    date = @params.Date.ToString("yyyyMMdd"),
                    defect = @params.Defect,
                    email = @params.Email,
                    lightpole = @params.Structure ?? string.Empty,
                    mobilenumber = @params.MobileNumber,
                    road = @params.Road,
                    typeElectricityWater = @params.ElectricityOrWater,
                    xGPS = @params.xGPS,
                    yGPS = @params.yGPS,
                    build = string.Empty,
                    substationpole = string.Empty
                };

                try
                {
                    var response = client.GetGovtObservationDetails(request);

					var typedResponse = response.@return.DeserializeAs<GetGovernmentObservationDetailsResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<GetGovernmentObservationDetailsResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<GetGovernmentObservationDetailsResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetGovernmentObservationDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<SetLandlordDetailsResponse> ChangeLandlordInformation(LandlordDetailsParameters @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetLandlordRequest
                {
                    userid = @params.UserId,
                    sessionid = @params.SessionId,
                    businesspartnernumber = @params.BusinessPartnerNumber,
                    mobilenumber = @params.Mobile,
                    premisenumber = @params.PremiseNumber,
                    remarks = @params.Remarks,
                    Attach1 = @params.Attachment1,
                    Attach2 = @params.Attachment2,
                    FileName1 = @params.Filename1,
                    FileName2 = @params.Filename2,
                    FileType1 = @params.FileType1,
                    FileType2 = @params.FileType2,
                    vendorid = GetVendorId(segment),
                    appversion = AppVersion,
                    appidentifier = segment.Identifier(),
                    lang = language.Code()
                };

				try
				{
					var response = client.SetLandlordRequest(request);
					var typedResponse = response.@return.DeserializeAs<SetLandlordDetailsResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<SetLandlordDetailsResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<SetLandlordDetailsResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetLandlordDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<BusinessPartnerDetailsResponse> GetBusinessPartnerDetailsForRegistration(string bpNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetRegistrationBPDetails()
                {
                    businesspartnernumber = bpNumber,
                    merchantid = GetMerchantId(segment),
                    merchantpassword = GetMerchantPassword(segment),
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetRegistrationBPDetails(request);

					var typedResponse = response.@return.DeserializeAs<BusinessPartnerDetailsResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<BusinessPartnerDetailsResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<BusinessPartnerDetailsResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<BusinessPartnerDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<SendVerificationResponse> SendVerificationCodeForRegistration(string bpNumber, bool sendToMobile, bool sendToEmail, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SendVerifyRegistrationCode()
                {
                    businesspartnernumber = bpNumber,
                    mobileflag = sendToMobile == false ? null : "X",
                    emailflag = sendToEmail == false ? null : "X",
                    verifyflag = null,
                    verificationcode = null,
                    merchantid = GetMerchantId(segment),
                    merchantpassword = GetMerchantPassword(segment),
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.SendVerifyRegistrationCode(request);

                    var typedResponse = response.@return.DeserializeAs<SendVerificationResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<SendVerificationResponse>(new SendVerificationResponse()
                        {
                            Description = typedResponse.Description,
                            ResponseCode = typedResponse.ResponseCode,
                            Maxattempt = typedResponse.Maxattempt
                        },
                        false,
                        typedResponse.Description);
                    }
                    return new ServiceResponse<SendVerificationResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SendVerificationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<SendVerificationResponse> ConfirmVerificationCodeForRegistration(string bpNumber, bool sendToMobile, bool sendToEmail, string verificationCode, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SendVerifyRegistrationCode()
                {
                    businesspartnernumber = bpNumber,
                    mobileflag = sendToMobile == false ? null : "X",
                    emailflag = sendToEmail == false ? null : "X",
                    verifyflag = "X",
                    verificationcode = verificationCode,
                    merchantid = GetMerchantId(segment),
                    merchantpassword = GetMerchantPassword(segment),
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.SendVerifyRegistrationCode(request);

                    var typedResponse = response.@return.DeserializeAs<SendVerificationResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<SendVerificationResponse>(new SendVerificationResponse()
                        {
                            Description = typedResponse.Description,
                            ResponseCode = typedResponse.ResponseCode,
                            Maxattempt = typedResponse.Maxattempt
                        },
                        false,
                        typedResponse.Description);
                    }
                    return new ServiceResponse<SendVerificationResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SendVerificationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<RegisterCustomerResponse> RegisterCustomer(string bpNumber, string updatedMobile, string updatedEmail, string username, string password, string confirmPassword, string verificationCode, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetCustomerAccountRegistration
                {
                    businesspartnernumber = bpNumber,
                    updatemobile = updatedMobile,
                    updateemail = updatedEmail,
                    newuserid = username,
                    password = password,
                    confirmpassword = confirmPassword,
                    verificationcode = verificationCode,
                    merchantid = GetMerchantId(segment),
                    merchantpassword = GetMerchantPassword(segment),
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.SetCustomerAccountRegistration(request);
                    var typedResponse = response.@return.DeserializeAs<RegisterCustomerResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<RegisterCustomerResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<RegisterCustomerResponse>(typedResponse);
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<RegisterCustomerResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse UploadProfilePhoto(byte[] image, string imageType, string userName, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetUIDPhoto()
                {
                    photo = image,
                    userid = userName,
                    sessionid = sessionId,
                    photoattachmenttype = imageType,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

				try
				{
					var response = client.SetUIDPhoto(request);
					var typedResponse = response.@return.DeserializeAs<UploadPhotoResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<UploadPhotoResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<UploadPhotoResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<UploadPhotoResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<SetRequestCollectiveAccountResponse> RequestCollectiveAccount(RequestCollectiveAccountParameters @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetCollectiveAccountRequest()
                {
                    userid = @params.UserId,
                    sessionid = @params.SessionId,
                    businesspartnernumber = @params.BusinessPartnerNumber,
                    type = "D003",
                    name = @params.Name,
                    mobilenumber = @params.Mobile,
                    email = @params.Email,
                    category = @params.Category,
                    residential = @params.Residential,
                    attach1 = @params.Attachment1,
                    attach2 = @params.Attachment2,
                    fileName1 = @params.Filename1,
                    fileName2 = @params.Filename2,
                    fileType1 = @params.FileType1,
                    fileType2 = @params.FileType2,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

				try
				{
					var response = client.SetCollectiveAccountRequest(request);
					var typedResponse = response.@return.DeserializeAs<SetRequestCollectiveAccountResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<SetRequestCollectiveAccountResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<SetRequestCollectiveAccountResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetRequestCollectiveAccountResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<SetRequestCollectiveAccountResponse> AddToCollectiveBilling(RequestCollectiveAccountParameters @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetCollectiveAccountRequest()
                {
                    userid = @params.UserId,
                    sessionid = @params.SessionId,
                    businesspartnernumber = @params.BusinessPartnerNumber,
                    type = "D004", //hardcoded from Existing Mobile WebSite
                    name = @params.Name,
                    mobilenumber = @params.Mobile,
                    email = @params.Email,
                    category = "Residential", //hardcoded from Existing Mobile WebSite
                    residential = @params.Residential,
                    attach1 = @params.Attachment1,
                    attach2 = null, //hardcoded from Existing Mobile WebSite
                    fileName1 = @params.Filename1,
                    fileName2 = string.Empty, //hardcoded from Existing Mobile WebSite
                    fileType1 = @params.FileType1,
                    fileType2 = string.Empty, //hardcoded from Existing Mobile WebSite
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

				try
				{
					var response = client.SetCollectiveAccountRequest(request);
					var typedResponse = response.@return.DeserializeAs<SetRequestCollectiveAccountResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<SetRequestCollectiveAccountResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<SetRequestCollectiveAccountResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetRequestCollectiveAccountResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<byte[]> GetProfilePhoto(string userName, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetUIDPhoto()
                {
                    userid = userName,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetUIDPhoto(request);

					return new ServiceResponse<byte[]>(response.@return);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<byte[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public async Task<ServiceResponse<byte[]>> GetProfilePhotoAsync(string userName, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetUIDPhoto()
                {
                    userid = userName,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = await client.GetUIDPhotoAsync(request);

					return new ServiceResponse<byte[]>(response.GetUIDPhotoResponse.@return);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<byte[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<EstimateNumDetailsResponse> GetEstimateNumDetails(string userName, string sessionId, string estimateNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetEstimateNumDetails()
                {
                    userid = userName,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    estimatenumber = estimateNumber
                };

				try
				{
					var response = client.GetEstimateNumDetails(request);
					var typedResponse = response.@return.DeserializeAs<EstimateNumDetailsResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<EstimateNumDetailsResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<EstimateNumDetailsResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<EstimateNumDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<byte[]> GetEstimatePdf(string userName, string sessionId, string estimateNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetEstimatePDF()
                {
                    userid = userName,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    estimatenumber = estimateNumber
                };

				try
				{
					var response = client.GetEstimatePDF(request);
					return new ServiceResponse<byte[]>(response.@return);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<byte[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<EstimateHistoryResponse> GetEstimateHistory(string userName, string sessionId, bool payForFriend, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetEstimateHistory()
                {
                    userid = userName,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    payforfriend = payForFriend ? "X" : string.Empty,
                    mobileosversion = AppVersion
                };

				try
				{
					var response = client.GetEstimateHistory(request);
					var typedResponse = response.@return.DeserializeAs<EstimateHistoryResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<EstimateHistoryResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<EstimateHistoryResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<EstimateHistoryResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<EstimateLineItems> GetEstimates(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetEstimateListCustomer()
                {
                    userid = userId,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetEstimateListCustomer(request);
                    var typedResponse = response.@return.DeserializeAs<EstimateLineItems>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<EstimateLineItems>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<EstimateLineItems>(typedResponse);
                }
                catch (TimeoutException)
                {
                    return new ServiceResponse<EstimateLineItems>(null, false, "timeout error message");
                }
                catch (Exception)
                {
                    return new ServiceResponse<EstimateLineItems>(null, false, "Unexpected error");
                }
            }
        }

        public ServiceResponse<ReraGetSecurityDepositDetailsResponse> GetReraSecurityDepositDetails(string caNumber, string bpNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetRERASecurityDepositDetails()
                {
                    businesspartnernumber = bpNumber,
                    contractaccountnumber = caNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetRERASecurityDepositDetails(request);
                    var typedResponse = response.@return.DeserializeAs<ReraGetSecurityDepositDetailsResponse>();
                    /*if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<ReraGetSecurityDepositDetailsResponse>(null, false, typedResponse.Description);
                    }*/
					return new ServiceResponse<ReraGetSecurityDepositDetailsResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ReraGetSecurityDepositDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<ServiceInterruptionResponse> GetServiceInterruptionList(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetServiceInterruptionList()
                {
                    userid = userId,
                    vendorid = GetVendorId(segment),
                    sessionid = sessionId,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    others = string.Empty,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetServiceInterruptionList(request);
                    var typedResponse = response.@return.DeserializeAs<ServiceInterruptionResponseLite>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<ServiceInterruptionResponse>(new ServiceInterruptionResponse()
                        {
                            Description = typedResponse.Description,
                            ResponseCode = typedResponse.ResponseCode
                        },
                        true,
                        typedResponse.Description);
                    }

					return new ServiceResponse<ServiceInterruptionResponse>(response.@return.DeserializeAs<ServiceInterruptionResponse>());
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ServiceInterruptionResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}
		public ServiceResponse<PremiseTypeChangeResponse> ChangePremiseType(string userId, string sessionId, PremiseTypeChangeRequest model, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
		{
			using (var client = CreateProxy())
			{
				var request = new SetPremiseTypeChangeRequest
				{
					userid = userId,
					sessionid = sessionId,
					contractaccountnumber = model.ContractAccountNumber,
					mobilenumber = model.MobileNumber,
					premisenumber = model.PremiseNumber,
					date = DateTime.UtcNow.ToString("yyyyMMdd"),
					vendorid = GetVendorId(segment),
					mobileosversion = string.Empty,
					appversion = AppVersion,
					appidentifier = segment.Identifier(),
					lang = language.Code(),
					attachment = model.Attachment ?? new byte[0],
					text = model.Remarks ?? string.Empty,
					type = model.AttachmentType
				};

                try
                {
                    var response = client.SetPremiseTypeChangeRequest(request);
                    var typedResponse = response.@return.DeserializeAs<PremiseTypeChangeResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<PremiseTypeChangeResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<PremiseTypeChangeResponse>(typedResponse);
                }

				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<PremiseTypeChangeResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<RoadWorksListResponse> GetRoadWorksList(string sessionId, string userName, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetRoadWorksList()
                {
                    sessionid = sessionId,
                    businesspartnernumber = userName,
                    vendorid = GetVendorId(segment),
                    mobileosversion = string.Empty,
                    appversion = string.Empty,
                    appidentifier = string.Empty,
                    lang = language.Code()
                };

				try
				{
					var response = client.GetRoadWorksList(request);
					var typedResponse = response.@return.DeserializeAs<RoadWorksListResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<RoadWorksListResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<RoadWorksListResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<RoadWorksListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}


            }
        }

        public ServiceResponse<MoveInAttachmentResponse> SendMoveInAttachment(string userId, string sessionId, string transactionRef, string[] transactionList, string fileName, string attachmentType, byte[] file, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var attachRequest = new SetMoveInAttachment
                {
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    sessionid = sessionId,
                    transactionid = transactionRef,
                    transactionidlist = transactionList,
                    userid = userId,
                    filename = fileName,
                    attachmenttype = attachmentType,
                    attachment = file
                };

                try
                {
                    var moveInAttachResponse = client.SetMoveInAttachment(attachRequest);

                    var typedResponse = moveInAttachResponse.@return.DeserializeAs<MoveInAttachmentResponse>();

					return new ServiceResponse<MoveInAttachmentResponse>(typedResponse, true);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<MoveInAttachmentResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}

            };
        }

        public ServiceResponse<MoveInLite> SendMoveInCredentials(string userId, string password, string confirmPassword, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetMoveInRequest
                {
                    vendorid = GetVendorId(segment),
                    userid = userId,
                    password = password,
                    confrimpassword = confirmPassword,
                    sessionid = string.Empty,
                    customertype = string.Empty,
                    accounttype = string.Empty,
                    premise = string.Empty,
                    idtype = string.Empty,
                    emiratesid = string.Empty,
                    firstname = string.Empty,
                    lastname = string.Empty,
                    pobox = string.Empty,
                    nation = string.Empty,
                    mobile = string.Empty,
                    email = string.Empty,
                    startdate = string.Empty,
                    enddate = string.Empty,
                    screenno = string.Empty,
                    businesspartnerno = string.Empty,
                    transactionid = string.Empty,
                    subid = string.Empty,
                    idexpiry = string.Empty,
                    contractvalue = string.Empty,
                    customercategory = string.Empty,
                    noofrooms = string.Empty,
                    region = string.Empty,
                    loginmode = string.Empty,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = string.Empty
                };

                try
                {
                    var moveInResponse = client.SetMoveInRequest(request);

                    var liteMoveIn = moveInResponse.@return.DeserializeAs<MoveInLite>();

					return new ServiceResponse<MoveInLite>(liteMoveIn, true);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<MoveInLite>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<MoveInResponse> MoveIn(string userId, string password, string confirmPassword, string sessionId, string customerType, string accountType, string premiseNumber, string idType, string emiratesId, string firstName, string lastName, string pobox, string nation, string mobile, string email, DateTime? startDate, DateTime? endDate, string screenNo, string businessPartnerNo, string transactionId, string subId, DateTime? idExpiry, string contractValue, string customerCategory, int? numberOfRooms, string region, string loginMode, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetMoveInRequest
                {
                    userid = userId,
                    password = password,
                    confrimpassword = confirmPassword,
                    sessionid = sessionId,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    customertype = customerType,
                    accounttype = accountType,
                    premise = premiseNumber,
                    idtype = idType,
                    emiratesid = emiratesId,
                    firstname = firstName,
                    lastname = lastName,
                    pobox = pobox,
                    nation = nation,
                    mobile = mobile,
                    email = email,
                    startdate = startDate.HasValue ? startDate.Value.ToString("yyyyMMdd") : string.Empty,
                    enddate = endDate.HasValue ? endDate.Value.ToString("yyyyMMdd") : string.Empty,
                    screenno = screenNo,
                    businesspartnerno = businessPartnerNo,
                    transactionid = transactionId,
                    subid = subId,
                    idexpiry = idExpiry.HasValue ? idExpiry.Value.ToString("yyyyMMdd") : string.Empty,
                    contractvalue = contractValue,
                    customercategory = customerCategory,
                    noofrooms = numberOfRooms.HasValue ? numberOfRooms.ToString() : string.Empty,
                    region = region,
                    loginmode = loginMode,
                    vendorid = GetVendorId(segment),
                    lang = language.Code()
                };

                try
                {
                    var moveInResponse = client.SetMoveInRequest(request);


                    var typedResponse = moveInResponse.@return.DeserializeAs<MoveInResponse>();

					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<MoveInResponse>(typedResponse, false, typedResponse.Description);
					}
					return new ServiceResponse<MoveInResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<MoveInResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<MoveInEjariResponse> MoveInEjariRequest(string userId, string createuseraccount, string password, string confirmPassword, string sessionId, string customerType, string idType, string emiratesId, string firstName, string lastName, string pobox, string nation, string mobile, string email, DateTime? moveinDate, DateTime? moveoutDate, string moveoutAccount, DateTime? startDate, DateTime? endDate, string businessPartnerNo, DateTime? idExpiry, string contractValue, string customerCategory, int? numberOfRooms, string region, string loginMode, string ejariNumber, string[] premiselistField, string processmovein, string vatnumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string center = null)
        {
            using (var client = CreateProxy())
            {
                var request = new SetMoveInEjariRequest
                {
                    userid = userId,
                    createuseraccount = createuseraccount,
                    password = password,
                    confrimpassword = confirmPassword,
                    sessionid = sessionId,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    customertype = customerType,
                    idtype = idType,
                    emiratesid = emiratesId,
                    firstname = firstName,
                    lastname = lastName,
                    pobox = pobox,
                    nation = nation,
                    mobile = mobile,
                    email = email,
                    moveindate = moveinDate.HasValue ? moveinDate.Value.ToString("yyyyMMdd") : string.Empty,
                    moveoutdate = moveoutDate.HasValue ? moveoutDate.Value.ToString("yyyyMMdd") : string.Empty,
                    moveoutaccountnumber = moveoutAccount,
                    startdate = startDate.HasValue ? startDate.Value.ToString("yyyyMMdd") : string.Empty,
                    enddate = endDate.HasValue ? endDate.Value.ToString("yyyyMMdd") : string.Empty,
                    businesspartnerno = businessPartnerNo,
                    idexpiry = idExpiry.HasValue ? idExpiry.Value.ToString("yyyyMMdd") : string.Empty,
                    contractvalue = contractValue,
                    customercategory = customerCategory,
                    noofrooms = numberOfRooms.HasValue ? numberOfRooms.ToString() : string.Empty,
                    region = region,
                    loginmode = loginMode,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    ejari = ejariNumber,
                    vatnumber = vatnumber,
                    premiselist = premiselistField,
                    processmovein = processmovein,
                    mobileosversion = string.Empty,
                    center = center



                };

                try
                {
                    var moveInResponse = client.SetMoveInEjariRequest(request);

                    moveInEjari moveinejarimodel = moveInResponse.@return;

                    if (string.Compare(moveinejarimodel.responseCode, "000", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        return new ServiceResponse<MoveInEjariResponse>(MoveInEjariResponse.From(moveinejarimodel), false, moveinejarimodel.description);
                    }
                    return new ServiceResponse<MoveInEjariResponse>(MoveInEjariResponse.From(moveinejarimodel));

				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<MoveInEjariResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<MoveinActivationResponse> SetUserActivation(string user, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetUserActivation()
                {
                    usercode = user,
                    vendorid = GetVendorId(segment),
                    mobileosversion = AppVersion,
                    appversion = AppVersion,
                    appidentifier = segment.Identifier(),
                    lang = language.Code()
                };

				try
				{
					var response = client.SetUserActivation(request);
					var typedResponse = response.@return.DeserializeAs<MoveinActivationResponse>();
					if (string.Compare(typedResponse.ResponseCode, "000", StringComparison.OrdinalIgnoreCase) != 0)
					{
						return new ServiceResponse<MoveinActivationResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<MoveinActivationResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<MoveinActivationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        #region MoveIn
        public ServiceResponse<moveInPostOutput> SetMoveInPostRequest(moveInPostInput input, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetMoveInPostRequest
                {
                    moveinpostparams = input
                };

                request.moveinpostparams.vendorid = GetVendorId(segment);
                request.moveinpostparams.appidentifier = segment.Identifier();
                request.moveinpostparams.appversion = AppVersion;
                request.moveinpostparams.mobileosversion = AppVersion;

                try
                {
                    var moveInResponse = client.SetMoveInPostRequest(request);

                    moveInPostOutput response = moveInResponse.@return;

                    if (string.Compare(response.responsecode, "000", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        return new ServiceResponse<moveInPostOutput>(response, false, response.description);
                    }
                    return new ServiceResponse<moveInPostOutput>(response, true, response.description);

                }
                catch (TimeoutException)
                {
                    return new ServiceResponse<moveInPostOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<moveInPostOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<moveInReadOutput> SetMoveInReadRequest(moveInReadInput input, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetMoveInReadRequest
                {
                    moveinreadparams = input
                };

                request.moveinreadparams.vendorid = GetVendorId(segment);
                request.moveinreadparams.appidentifier = segment.Identifier();
                request.moveinreadparams.appversion = AppVersion;
                request.moveinreadparams.mobileosversion = AppVersion;

                try
                {
                    var moveInResponse = client.SetMoveInReadRequest(request);

                    moveInReadOutput response = moveInResponse.@return;

                    if (string.Compare(response.responsecode, "000", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        return new ServiceResponse<moveInReadOutput>(response, false, response.description);
                    }
                    return new ServiceResponse<moveInReadOutput>(response, true, response.description);

                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<moveInReadOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }

        }
        #endregion

        public ServiceResponse<GetCustomerDetailsServiceResponse> GetCustomerDetails(string userid, string sessionid, SupportedLanguage language, RequestSegment segment)
		{
			using (var client = CreateProxy())
			{
				var request = new GetCustomerDetails()
				{
					appidentifier = segment.Identifier(),
					appversion = AppVersion,
					lang = language.Code(),
					mobileosversion = AppVersion,
					userid = userid,
					vendorid = GetVendorId(segment),
					sessionid = sessionid
				};
				try
				{
					var response = client.GetCustomerDetails(request);
					var typedResponse = response.@return.DeserializeAs<GetCustomerDetailsServiceResponse>();
					if (string.Compare(typedResponse.ResponseCode, "000", StringComparison.OrdinalIgnoreCase) != 0)
					{
						return new ServiceResponse<GetCustomerDetailsServiceResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<GetCustomerDetailsServiceResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetCustomerDetailsServiceResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}

            }
        }

		public ServiceResponse<ConnectionEnquiryResponse> SubmitConnectionEnquiry(RequestConnectionEnquiry @params, SupportedLanguage language, RequestSegment segment)
		{
			using (var client = CreateProxy())
			{
				var request = new SetRateCategoryRequest()
				{
					appidentifier = segment.Identifier(),
					appversion = AppVersion,
					mobileosversion = AppVersion,
					attachment = @params.Attachment,
					attachmenttype = @params.AttachmentType,
					businesspartnernumber = @params.BusinessPartnerNo,
					contractaccountnumber = @params.ContractAccountNo,
					lang = language.Code(),
					mobilenumber = @params.MobileNo,
					premisenumber = @params.PremiseNo,
					remarks = @params.Remarks,
					sessionid = @params.SessionNo,
					userid = @params.UserId,
					requesttype = @params.QueryType,
					vendorid = GetVendorId(segment)
				};
				try
				{
					var response = client.SetRateCategoryRequest(request);
					var typedResponse = response.@return.DeserializeAs<ConnectionEnquiryResponse>();
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<ConnectionEnquiryResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<ConnectionEnquiryResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ConnectionEnquiryResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<ComplaintsSurveyQuestionnaireResponse> GetComplaintsSurveyQuestionnaire(string notificationkey,
        SupportedLanguage language, RequestSegment segment)
        {

            using (var client = CreateProxy())
            {
                var request = new GetComplaintsSurveyQuestionnaireRequest()
                {
                    GetComplaintsSurveyQuestionnaire = new GetComplaintsSurveyQuestionnaire
                    {
                        notificationkey = notificationkey,
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        lang = language.Code(),
                        mobileosversion = AppVersion,

                    }

                };

                try
                {
                    ///Fetching the Object from Proxy
                    var response = client.GetComplaintsSurveyQuestionnaire(request.GetComplaintsSurveyQuestionnaire);
                    var typedResponse = response.@return;

                    //Filling the Object with Our Class
                    var ObjComplaintSurveyQuestionniareResponse = new ComplaintsSurveyQuestionnaireResponse
                    {

                        QuestionEnglish = typedResponse.questionenglish,
                        QuestionArabic = typedResponse.questionarabic,
                        QuestionNo = typedResponse.questionno

                    };


                    if (string.Compare(typedResponse.responsecode, "000", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        return new ServiceResponse<ComplaintsSurveyQuestionnaireResponse>(null, false, typedResponse.desciption);
                    }

					return new ServiceResponse<ComplaintsSurveyQuestionnaireResponse>(ObjComplaintSurveyQuestionniareResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ComplaintsSurveyQuestionnaireResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}

            }
        }

        public ServiceResponse<ComplaintsSurveyFeedbackResponse> SetComplaintsSurveyFeedback(ComplaintsSurveyFeedback @params,
            SupportedLanguage language, RequestSegment segment)
        {


            using (var client = CreateProxy())
            {
                var request = new SetComplaintsSurveyFeedback()
                {
                    answerchoice = @params.AnswerChoice,
                    questionnumber = @params.QuestionNumber,
                    comment = @params.Comment,
                    notificationkey = @params.notificationkey,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    mobileosversion = AppVersion,
                };

                try
                {
                    ///Fetching the Object from Proxy
                    var response = client.SetComplaintsSurveyFeedback(request);
                    var typedResponse = response.@return;

                    //Mapping Response with the Response of the SAP Service and Returning to Controller
                    ComplaintsSurveyFeedbackResponse feedbackResponse = new ComplaintsSurveyFeedbackResponse();
                    feedbackResponse.Description = typedResponse.desciption;
                    feedbackResponse.ResponseCode = typedResponse.responsecode;

                    if (string.Compare(typedResponse.responsecode, "000", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        return new ServiceResponse<ComplaintsSurveyFeedbackResponse>(null, false, typedResponse.desciption);
                    }

					return new ServiceResponse<ComplaintsSurveyFeedbackResponse>(feedbackResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ComplaintsSurveyFeedbackResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}

            }
        }

        public ServiceResponse<CollectiveStatementResponse> GetCollectiveAccounts(string userId, string sessionId, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCollectiveAccountList
                {
                    userid = userId,
                    sessionid = sessionId,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetCollectiveAccountList(request);
                    var typedResponse = response.@return.DeserializeAs<CollectiveStatementResponse>();

					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<CollectiveStatementResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<CollectiveStatementResponse>(typedResponse);
				}
				catch (System.Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<CollectiveStatementResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public byte[] GetCollectiveStatementPDF(string userId, string sessionId, string contractAccount, string year, string month, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCollectiveStatementPDF
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccount = contractAccount,
                    month = month,
                    year = year,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetCollectiveStatementPDF(request);
                    return response.@return;
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return null;
                }
            }
        }

        public ServiceResponse<IBANListResponse> IBANList(string userId, string sessionId, string contractAccount, string bpnumber, SupportedLanguage language, RequestSegment segment)
        {

            using (var client = CreateProxy())
            {
                var request = new GetIBANListRequest()
                {
                    GetIBANList = new GetIBANList()
                    {

                        bpnumber = bpnumber,
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        lang = language.Code(),
                        mobileosversion = AppVersion,
                        vendorid = GetVendorId(segment),
                        sessionid = sessionId,
                        userid = userId
                    }

                };

                try
                {
                    ///Fetching the Object from Proxy
                    var response = client.GetIBANList(request.GetIBANList);
                    var typedResponse = response.@return;

                    //Filling the Object with Our Class
                    var ObjIBANListResponse = new IBANListResponse
                    {
                        IBAN = typedResponse.ET_IBAN

                    };


                    if (string.Compare(typedResponse.responseCode, "000", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        return new ServiceResponse<IBANListResponse>(null, false, typedResponse.description);
                    }

					return new ServiceResponse<IBANListResponse>(ObjIBANListResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<IBANListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}

            }
        }

        public ServiceResponse<UpdateIBANResponse> UpdateIBAN(string userId, string sessionId, string contractAccount, string ibanNumber, string bpnumber, string chequeiban, string address, SupportedLanguage language, RequestSegment segment)
        {

            using (var client = CreateProxy())
            {
                var request = new SetIBANNumber()
                {
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code(),
                    vendorid = GetVendorId(segment),
                    ibannum = ibanNumber,
                    address = address,
                    userid = userId,
                    sessionid = sessionId,
                    contractaccount = contractAccount,
                    gpart = bpnumber,
                    chequeiban = chequeiban
                };

                try
                {
                    ///Fetching the Object from Proxy
                    var response = client.SetIBANNumber(request);
                    var typedResponse = response.@return;

                    //Mapping Response with the Response of the SAP Service and Returning to Controller
                    UpdateIBANResponse ibanResponse = new UpdateIBANResponse();
                    ibanResponse.ResponseCode = typedResponse.responseCode;
                    ibanResponse.Description = typedResponse.description;


                    if (string.Compare(typedResponse.responseCode, "000", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        return new ServiceResponse<UpdateIBANResponse>(null, false, typedResponse.description);
                    }

					return new ServiceResponse<UpdateIBANResponse>(ibanResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<UpdateIBANResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}

            }
        }

        public ServiceResponse<GetBehaviourinsightCustomerDataResponse> GetBehaviourinsightCustomer(string uniqueID, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetBehaviourinsightCustomer
                {
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code(),
                    unique = uniqueID

                };

                try
                {
                    var response = client.GetBehaviourinsightCustomer(request);

					var typedResponse = response.@return.DeserializeAs<GetBehaviourinsightCustomerDataResponse>();
					if (typedResponse.ResponseCode != 0 && typedResponse.ResponseCode != 346)
					{
						return new ServiceResponse<GetBehaviourinsightCustomerDataResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<GetBehaviourinsightCustomerDataResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetBehaviourinsightCustomerDataResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse UpdatePrimaryContactDetails(string userId, string sessionId, string emailAddress, string mobileNumber, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var request = new SetBusinessPartnerContactDetails()
                {
                    emailid = emailAddress,
                    mobilenumber = mobileNumber,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code(),
                    vendorid = GetVendorId(segment),
                    userid = userId,
                    sessionid = sessionId,
                };

                try
                {
                    var response = client.SetBusinessPartnerContactDetails(request);
                    var typedResponse = response.@return.DeserializeAs<UpdatePrimaryContactResponse>();

					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse(false, typedResponse.Description);
					}
					return new ServiceResponse();
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<BaseResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<SmartWalletAdvanceResponse> GetSmartWalletAdvance(string userID, string sessionID, subscribeDetails[] subscribeDetails, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetSmartWalletCheckAdv
                {
                    userid = userID,
                    sessionid = sessionID,
                    subscribeDetails = subscribeDetails,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion
                };
                var response = client.GetSmartWalletCheckAdv(request);
                var typedresponse = new ServiceResponse<SmartWalletAdvanceResponse>(SmartWalletAdvanceResponse.From(response.@return));
                return typedresponse;
            }
        }

        public ServiceResponse<SmartWalletSubscribeResponse> SaveSmartWalletSubscription(string userID, string sessionID, string subscribe, subscribeDetails[] subscribeDetails, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetSubunsubSmartWallet
                {
                    userid = userID,
                    sessionid = sessionID,
                    subscribe = subscribe,
                    subscribeDetails = subscribeDetails,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion
                };
                var response = client.SetSubunsubSmartWallet(request);
                var typedresponse = new ServiceResponse<SmartWalletSubscribeResponse>(SmartWalletSubscribeResponse.From(response.@return));
                return typedresponse;
            }
        }

        public ServiceResponse<MoveOutAccountResponse> GetMoveOutDetails(string[] contractAccounts, string userID, string sessionID, string checkmoveout, string offlinemode, string checkamount, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetValidateRefund
                {
                    contractaccountlist = contractAccounts,
                    userid = userID,
                    sessionid = sessionID,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    checkmoveout = checkmoveout,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    offlinemode = offlinemode,
                    checkamount = checkamount,
                    mobileosversion = AppVersion
                };
                var response = client.GetValidateRefund(request).@return;
                var typedresponse = new ServiceResponse<MoveOutAccountResponse>(MoveOutAccountResponse.From(response));
                if (typedresponse.Payload.ResponseCode != 0)
                {
                    return new ServiceResponse<MoveOutAccountResponse>(null, false, typedresponse.Payload.Description);
                }
                return new ServiceResponse<MoveOutAccountResponse>(typedresponse.Payload, true, typedresponse.Payload.Description);
            }
        }

        /// <summary>
        /// To check if the contract account exsist for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contractaccount"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public ServiceResponse GetContractAccountUserIDCheck(string userId, string contractaccount, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetContractAccountUserIDCheck
                {
                    contractaccountnumber = contractaccount,
                    userid = userId,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };
                var response = client.GetContractAccountUserIDCheck(request);
                var typedResponse = response.@return.DeserializeAs<Responses.GetContractAccountUserIDCheckResponse>();

                if (typedResponse.ResponseCode != 0)
                {
                    return new ServiceResponse(false, typedResponse.Description);
                }
                return new ServiceResponse();


            }
        }
        public ServiceResponse<SetSubsciptionPreferencesResponse> SetSubscription(string contractaccount, string readwritemode, string email, string emailflag, string mobile, string mobileflag, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetSubsciptionPreferences
                {
                    contractaccountnumber = contractaccount,
                    readWriteMode = readwritemode,
                    email = email,
                    emailSubscribeFlag = emailflag,
                    mobile = mobile,
                    mobileSubscribeFlag = mobileflag,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion
                };
                var response = client.SetSubsciptionPreferences(request);
                if (response != null && response.@return != null && response.@return.responseCode == "000")
                {
                    var typedresponse = new ServiceResponse<SetSubsciptionPreferencesResponse>(response);
                    return typedresponse;
                }
                else
                {
                    var typedresponse = new ServiceResponse<SetSubsciptionPreferencesResponse>(response, false, response.@return.description);
                    return typedresponse;
                }

            }
        }

		public ServiceResponse<GetVatDetailsForBPResponse> GetVatNumber(string userid, string sessionid, SupportedLanguage language, RequestSegment segment)
		{
			using (var client = CreateProxy())
			{
				var request = new GetVatDetailsForBP()
				{
					appidentifier = segment.Identifier(),
					appversion = AppVersion,
					lang = language.Code(),
					mobileosversion = AppVersion,
					userid = userid,
					vendorid = GetVendorId(segment),
					sessionid = sessionid
				};
				try
				{
					var response = client.GetVatDetailsForBP(request);
					if (response != null && response.@return != null && response.@return.responseCode == "000")
					{
						var typedresponse = new ServiceResponse<GetVatDetailsForBPResponse>(response);
						return typedresponse;
					}
					else
					{
						var typedresponse = new ServiceResponse<GetVatDetailsForBPResponse>(response, false, response.@return.description);
						return typedresponse;
					}
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetVatDetailsForBPResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}

            }
        }

		public ServiceResponse<SetVatDetailsForBPResponse> SetVatNumber(string contractaccountnumber, string businesspartnernumber, string region, string vatnumber, byte[] vatdocument, string userid, string sessionid, SupportedLanguage language, RequestSegment segment)
		{
			using (var client = CreateProxy())
			{
				var request = new SetVatDetailsForBP()
				{
					contractaccountnumber = contractaccountnumber,
					businesspartnernumber = businesspartnernumber,
					region = region,
					vatnumber = vatnumber,
					vatdocument = vatdocument,
					appidentifier = segment.Identifier(),
					appversion = AppVersion,
					lang = language.Code(),
					mobileosversion = AppVersion,
					userid = userid,
					vendorid = GetVendorId(segment),
					sessionid = sessionid
				};
				try
				{
					var response = client.SetVatDetailsForBP(request);
					if (response != null && response.@return != null && response.@return.responseCode == "000")
					{
						var typedresponse = new ServiceResponse<SetVatDetailsForBPResponse>(response);
						return typedresponse;
					}
					else
					{
						var typedresponse = new ServiceResponse<SetVatDetailsForBPResponse>(response, false, response.@return.description);
						return typedresponse;
					}
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetVatDetailsForBPResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}

            }
        }

        /// <summary>
        /// Move out Request
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contractaccount"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public ServiceResponse<SetMoveOutRequestResponse> SetMoveOutRequest(moveOutParams moveoutparams, string userid, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                moveoutparams.appidentifier = segment.Identifier();
                moveoutparams.appversion = AppVersion;
                moveoutparams.lang = language.Code();
                moveoutparams.mobileosversion = AppVersion;
                moveoutparams.moveoutversion = "03";
                moveoutparams.userid = userid;
                moveoutparams.vendorid = GetVendorId(segment);
                moveoutparams.sessionid = sessionId;
                var request = new SetMoveOutRequest()
                {
                    moveoutparams = moveoutparams
                };
                try
                {
                    var response = client.SetMoveOutRequest(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<SetMoveOutRequestResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<SetMoveOutRequestResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetMoveOutRequestResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        //public ServiceResponse<paymentHistoryDetails> GetPaymentHistory(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        //{
        //    using (var client = CreateProxy())
        //    {
        //        var request = new GetPaymentHistory
        //        {
        //            userid = userId,
        //            sessionid = sessionId,
        //            contractaccountnumber = contractAccountNumber,
        //            vendorid = GetVendorId(segment),
        //            appidentifier = segment.Identifier(),
        //            appversion = AppVersion,
        //            mobileosversion = AppVersion,
        //            estimatenumber = string.Empty,
        //            lang = language.Code()
        //        };

        //        try
        //        {
        //            var response = client.GetPaymentHistory(request);


        //            if (response != null && response.@return != null && response.@return.responsecode == "000")
        //            {
        //                var typedresponse = new ServiceResponse<paymentHistoryDetails>(response.@return);
        //                return typedresponse;
        //            }
        //            else if (response != null && response.@return != null && response.@return.responsecode == "105")
        //            {
        //                var typedresponse = new ServiceResponse<paymentHistoryDetails>(response.@return, true, response.@return.description);
        //                return typedresponse;
        //            }
        //            else
        //            {
        //                var typedresponse = new ServiceResponse<paymentHistoryDetails>(response.@return, false, response.@return.description);
        //                return typedresponse;
        //            }

        //        }
        //        catch (TimeoutException)
        //        {
        //            return new ServiceResponse<paymentHistoryDetails>(null, false, "timeout error message");
        //        }
        //        catch (Exception ex)
        //        {
        //            return new ServiceResponse<paymentHistoryDetails>(null, false, ex.Message);
        //        }
        //    }
        //}

        public ServiceResponse<byte[]> GetStatementofAccountsPDF(string userId, string sessionId, string contractAccountNumber, string numberofmonths, string frommonth, string tomonth, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetStatementofAccountsPDF
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccountnumber = contractAccountNumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code(),
                    numberofmonths = numberofmonths,
                    frommonth = frommonth,
                    tomonth = tomonth
                };

                try
                {
                    var response = client.GetStatementofAccountsPDF(request);


                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<byte[]>(response.@return.statementPDF);
                        return typedresponse;
                    }
                    else
                    {

                        var typedresponse = new ServiceResponse<byte[]>(null, false, response.@return.description);
                        return typedresponse;
                    }

				}
				
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<byte[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<paymentReceiptDetails> GetOnlinePaymentReceipt(string userId, string sessionId, string transactionid, string date, string type, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetOnlinePaymentReceipt
                {
                    userid = userId,
                    sessionid = sessionId,
                    transactionid = transactionid,
                    paymenttypetext = type,
                    datetimestamp = date,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetOnlinePaymentReceipt(request);

                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<paymentReceiptDetails>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<paymentReceiptDetails>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }

				}
				
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<paymentReceiptDetails>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        public ServiceResponse<refundHistory> GetRefundHistory(string userId, string sessionId, string contractAccountNumber, string businesspartner, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetRefundHistory
                {
                    userid = userId,
                    sessionid = sessionId,
                    contractaccount = contractAccountNumber,
                    businesspartner = businesspartner,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetRefundHistory(request);


                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<refundHistory>(response.@return);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responsecode == "105")
                    {
                        var typedresponse = new ServiceResponse<refundHistory>(response.@return, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<refundHistory>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }

				}
				
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<refundHistory>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}
			}
		}

        #region Helper methods

        //private ServiceResponse<string> SignInAsCustomer(string userId, string password, SupportedLanguage language, RequestSegment segment)
        //{
        //    using (var client = CreateProxy())
        //    {
        //        var request = new GetLoginSessionCustomer()
        //        {
        //            userid = userId,
        //            password = password,
        //            merchantid = GetMerchantId(segment),
        //            merchantpassword = GetMerchantPassword(segment),
        //            appidentifier = segment.Identifier(),
        //            appversion = AppVersion,
        //            lang = language.Code()
        //        };

        //        try
        //        {
        //            var response = client.GetLoginSessionCustomer(request);

        //            var typedResponse = response.@return.DeserializeAs<LoginResponse>();
        //            if (typedResponse.ResponseCode != 0)
        //            {
        //                return new ServiceResponse<string>(null, false, typedResponse.Description);
        //            }
        //            return new ServiceResponse<string>(typedResponse.SessionNumber);
        //        }
        //        catch (Exception ex)
        //        {
        //            var id = LogService.Fatal(ex, this);
        //            return new ServiceResponse<string>(null, false, Translate.Text("Webservice Error"));// + id

        //        }
        //    }
        //}

        //private ServiceResponse<LoginResponse> SignInAsCustomerNew(string userId, string password, SupportedLanguage language, RequestSegment segment, string center = null)
        //{
        //    using (var client = CreateProxy())
        //    {
        //        var request = new GetLoginSessionCustomer()
        //        {
        //            userid = userId,
        //            password = password,
        //            merchantid = GetMerchantId(segment),
        //            merchantpassword = GetMerchantPassword(segment),
        //            appidentifier = segment.Identifier(),
        //            appversion = AppVersion,
        //            lang = language.Code(),
        //            center = center
        //        };

        //        try
        //        {
        //            var response = client.GetLoginSessionCustomer(request);

        //            var typedResponse = response.@return.DeserializeAs<LoginResponse>();
        //            if (typedResponse.ResponseCode != 0)
        //            {
        //                var failedresponse = new LoginResponse() { ResponseCode = typedResponse.ResponseCode };
        //                return new ServiceResponse<LoginResponse>(failedresponse, false, typedResponse.Description);
        //            }
        //            return new ServiceResponse<LoginResponse>(typedResponse);
        //        }
        //        catch (Exception ex)
        //        {
        //            var id = LogService.Fatal(ex, this);
        //            return new ServiceResponse<LoginResponse>(null, false, Translate.Text("Webservice Error"));// +id
        //        }
        //    }
        //}


        private ServiceResponse<string> SignInAsGovernmentalUser(string userId, string password, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var govtRequest = new GetLoginSessionGov
                {
                    userid = userId,
                    password = password,
                    merchantid = GetMerchantId(segment),
                    merchantpassword = GetMerchantPassword(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var govtResponse = client.GetLoginSessionGov(govtRequest);

                    var typedGovtResponse = govtResponse.@return.DeserializeAs<GovernmentalLoginResponse>();

                    if (typedGovtResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<string>(null, false, typedGovtResponse.Description);
                    }
                    return new ServiceResponse<string>(typedGovtResponse.SessionNumber);
                }
                catch (Exception ex)
                {
                    var id = LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE); //+ id
                }
            }
        }

        #endregion

        #region Proxy configuration methods

        private Config1Client CreateProxy()
        {
            var client = new Config1Client(CreateBinding(), GetEndpointAddress("SmartGovtPort"));

            client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new DewaApiCredentials());
            //client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["RestAPI_Client_Id"]));
            client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["RestAPI_Client_Id"], "Bearer " + OAuthToken.GetAccessToken()));
            client.ClientCredentials.UserName.UserName = BbUsername;
            client.ClientCredentials.UserName.Password = BbPassword;
            //using (var scope = new OperationContextScope(client.InnerChannel))
            //{
            //    // Do this if you want to use http header instead
            //    var httpRequestProperty = new HttpRequestMessageProperty();
            //    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = OAuthToken.GetAccessToken();
            //    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
            //}

            return client;
        }

        private Config1Client CreateApplicationProxy()
        {
            var client = new Config1Client(CreateBinding(), GetEndpointAddress("SmartGovtPort"));

            client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new DewaApiCredentials());
            client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["RestAPI_Client_Id"], "Bearer " + OAuthToken.GetApplicationAccessToken()));
            client.ClientCredentials.UserName.UserName = BbUsername;
            client.ClientCredentials.UserName.Password = BbPassword;
            return client;
        }
        private CustomBinding CreateBinding()
        {
            var binding = new CustomBinding()
            {
                ReceiveTimeout = TimeSpan.FromMinutes(2),
                SendTimeout = TimeSpan.FromMinutes(2)
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
            //var transport = null;
            if (!string.IsNullOrWhiteSpace(SSLSettings) && SSLSettings.Equals("1"))
            {
                var transport = new HttpTransportBindingElement();
                transport.MaxReceivedMessageSize = 20000000; // 20 megs

                binding.Elements.Add(security);
                binding.Elements.Add(new CustomTextMessageBindingElement());
                binding.Elements.Add(transport);
            }
            else
            {
                var transport = new HttpsTransportBindingElement();
                transport.MaxReceivedMessageSize = 20000000; // 20 megs
                binding.Elements.Add(security);
                binding.Elements.Add(new CustomTextMessageBindingElement());
                binding.Elements.Add(transport);
            }

            return binding;
        }

        //private EndpointAddress GetEndpointAddress()
        //{
        //	var clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;
        //	string address = string.Empty;
        //	for (int i = 0; i < clientSection.Endpoints.Count; i++)
        //	{
        //		if (clientSection.Endpoints[i].Name == "SmartGovtPort")
        //			address = clientSection.Endpoints[i].Address.ToString();
        //	}
        //	return new EndpointAddress(address);
        //}




        #endregion

        #region ENBD Reference Number Generation

        public string GenerateRefNo(string userId, string sessionId, string[] contractAccounts, string[] amounts, string[] finalBillList, string email, string mobileNo, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var request = new SetReferenceNumberCreate()
                {
                    contractaccountlist = contractAccounts,
                    amountlist = amounts,
                    finalbilllist = finalBillList,
                    userid = userId,
                    vendorid = GetVendorId(segment),
                    sessionid = sessionId,
                    emailid = email,
                    mobilenumber = mobileNo,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code()
                };

                var response = client.SetReferenceNumberCreate(request);
                return response.@return;
            }
        }
        #endregion
        public string GetOutstandingAmount(string[] contractAccounts, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var request = new GetOutstandingBalance()
                {
                    contractaccountnumber = contractAccounts,
                    appidentifier = segment.Identifier(),
                    lang = language.Code(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion
                };

                var response = client.GetOutstandingBalance(request);
                return response.@return;
            }
        }

        public string ValidateAccounts(string userId, string sessionId, string[] contractAccounts, string[] amounts, string[] finalbills, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var request = new GetReferenceAccountNumberValidate()
                {
                    contractaccountlist = contractAccounts,
                    amountlist = amounts,
                    finalbilllist = finalbills,
                    userid = userId,
                    vendorid = GetVendorId(segment),
                    sessionid = sessionId,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code()
                };
                try
                {
                    var response = client.GetReferenceAccountNumberValidate(request);
                    return response.@return;
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return string.Empty;
                }
            }
        }
        public string GetRefNoList(string userId, string sessionId, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var request = new GetReferenceNumberList()
                {
                    userid = userId,
                    vendorid = GetVendorId(segment),
                    sessionid = sessionId,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code()
                };

                var response = client.GetReferenceNumberList(request);
                return response.@return;
            }
        }
        public string GetRefNoDetails(string userId, string sessionId, string refNo, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var request = new GetReferenceNumberDetails()
                {
                    userid = userId,
                    dewareferencenumber = refNo,
                    vendorid = GetVendorId(segment),
                    sessionid = sessionId,
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    lang = language.Code()
                };

                var response = client.GetReferenceNumberDetails(request);
                return response.@return;
            }
        }

        #region New Clearance Certficate V2
        public ServiceResponse<VerifyClearanceCertificateResponse> GetVerifyClearanceCertificate(string referenceNumber, string pinNumber, string urlParameter, SupportedLanguage requestLanguage, RequestSegment requestSegment, string pinFlag = null)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var request = new VerifyClearanceCertificate
                    {
                        referenceNumber = referenceNumber,
                        pinNumber = pinNumber,
                        urlParameter = urlParameter,
                        appversion = AppVersion,
                        appidentifier = requestSegment.Identifier(),
                        vendorid = GetVendorId(requestSegment),
                        mobileosversion = AppVersion,
                        lang = requestLanguage.ToString(),
                        sendPinFlag = pinFlag

                    };

                    var response = client.VerifyClearanceCertificate(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<VerifyClearanceCertificateResponse>(response);
                        return typedresponse;
                    }
                    else
                    {

						var typedresponse = new ServiceResponse<VerifyClearanceCertificateResponse>(response, false, response.@return.description);
						return typedresponse;
					}
				}
			}
			catch (Exception ex)
			{
                LogService.Fatal(ex, this);
                return new ServiceResponse<VerifyClearanceCertificateResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
			}
		}

        public ServiceResponse<GetContractAccountClearanceDetailsResponse> GetContractAccountClearanceDetails(string userid, string sessionId, string contractAccountNumber, SupportedLanguage requestLanguage, RequestSegment requestSegment, string purpose = null)
        {

            try
            {

                using (var client = CreateProxy())
                {

                    var request = new GetContractAccountClearanceDetails
                    {

                        contractAccount = contractAccountNumber,
                        sessionId = sessionId,
                        appidentifier = requestSegment.Identifier(),
                        appversion = AppVersion,
                        lang = requestLanguage.ToString(),
                        userId = userid,
                        vendorid = GetVendorId(requestSegment),
                        purpose = purpose

                        //center = center
                    };

                    var response = client.GetContractAccountClearanceDetails(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetContractAccountClearanceDetailsResponse>(response);
                        return typedresponse;
                    }
                    else
                    {

						var typedresponse = new ServiceResponse<GetContractAccountClearanceDetailsResponse>(response, false, response.@return.description);
						return typedresponse;
					}
				}
			}
			catch (Exception ex)
			{
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetContractAccountClearanceDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
			}
		}

        public ServiceResponse<GetContractAccountClearanceDetailsResponse> GetContractAccountIdentityDetails(GetContractAccountClearanceDetails contractAccountClearanceDetails, string userid, string sessionId, string contractAccountNumber, SupportedLanguage requestLanguage, RequestSegment requestSegment, string purpose = null)
        {

            try
            {

                using (var client = CreateProxy())
                {
                    contractAccountClearanceDetails.sessionId = sessionId;
                    contractAccountClearanceDetails.appidentifier = requestSegment.Identifier();
                    contractAccountClearanceDetails.appversion = AppVersion;
                    contractAccountClearanceDetails.lang = requestLanguage.ToString();
                    contractAccountClearanceDetails.userId = userid;
                    contractAccountClearanceDetails.vendorid = GetVendorId(requestSegment);

                    var response = client.GetContractAccountClearanceDetails(contractAccountClearanceDetails);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetContractAccountClearanceDetailsResponse>(response);
                        return typedresponse;
                    }
                    else
                    {

						var typedresponse = new ServiceResponse<GetContractAccountClearanceDetailsResponse>(response, false, response.@return.description);
						return typedresponse;
					}
				}
			}
			catch (Exception ex)
			{
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetContractAccountClearanceDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
			}
		}

        public ServiceResponse<ApplyClearanceCertificateResponse> ApplyClearanceCertificateDEWACustomer(string userid, string sessionId,
            string branch,
            string contractAccountNumber,
            string remarks, string emailAddress,
            string firstName, string lastName, string mobileNumber, SupportedLanguage requestLanguage, RequestSegment requestSegment, string langCode = "EN")
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var _ccrequest = new clearanceCertificateRequest
                    {
                        branch = branch,
                        firstName = firstName,
                        lastName = lastName,
                        contractAccount = contractAccountNumber,
                        email = emailAddress,
                        mobile = mobileNumber,
                        userId = userid,
                        sessionId = sessionId,
                        remarks = remarks,
                        cclang = langCode

                    };
                    var request = new ApplyClearanceCertificate
                    {

                        appversion = AppVersion,
                        appidentifier = requestSegment.Identifier(),
                        vendorid = GetVendorId(requestSegment),
                        mobileosversion = AppVersion,
                        lang = requestLanguage.ToString(),
                        clearanceCertificateRequest = _ccrequest

                    };

                    var response = client.ApplyClearanceCertificate(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<ApplyClearanceCertificateResponse>(response);
                        return typedresponse;
                    }
                    else
                    {

						var typedresponse = new ServiceResponse<ApplyClearanceCertificateResponse>(response, false, response.@return.description);
						return typedresponse;
					}
				}
			}
			catch (Exception ex)
			{
                LogService.Fatal(ex, this);
                return new ServiceResponse<ApplyClearanceCertificateResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
			}
		}

        public ServiceResponse<ApplyClearanceCertificateResponse> ApplyClearanceCertificate(RequestClearanceCertificate @params, string userid, string sessionId, SupportedLanguage requestLanguage, RequestSegment requestSegment)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var clearanceCertificateRequest = new clearanceCertificateRequest
                    {
                        firstName = @params.FirstName,
                        lastName = @params.LastName,
                        email = @params.EmailAddress,
                        mobile = @params.MobileNumber,
                        purpose = @params.Purpose,
                        poBox = @params.PoBox,
                        contractAccount = @params.ContractAccountNumber,
                        emiratesId = @params.IdentityNumber,
                        tradeLicenseNumber = @params.TradeLicenseNumber,
                        passportNumber = @params.PassportNumber,
                        attachment1 = @params.Attachment1,
                        fileName1 = @params.Attachment1Extension,
                        attachment2 = @params.Attachment2,
                        fileName2 = @params.Attachment2Extension,
                        attachment3 = @params.Attachment3,
                        fileName3 = @params.Attachment3Extension,
                        branch = @params.Branch,
                        courtNumber = @params.CourtNumber,
                        remarks = @params.Remarks,
                        region = @params.Emirates,
                        tradeLicenseAuthority = @params.TradeLicenseAuthority,
                        userId = userid,
                        sessionId = sessionId,
                        cclang = @params.cclang

                    };
                    var request = new ApplyClearanceCertificate
                    {
                        appidentifier = requestSegment.Identifier(),
                        appversion = AppVersion,
                        lang = requestLanguage.ToString(),
                        vendorid = GetVendorId(requestSegment),
                        mobileosversion = AppVersion,
                        clearanceCertificateRequest = clearanceCertificateRequest
                    };

                    var response = client.ApplyClearanceCertificate(request);

					if (response != null && response.@return != null && response.@return.responseCode == "000")
					{
						var typedresponse = new ServiceResponse<ApplyClearanceCertificateResponse>(response);
						return typedresponse;
					}
					else
					{
						var typedresponse = new ServiceResponse<ApplyClearanceCertificateResponse>(response, false, response.@return.description);
						return typedresponse;
					}
				}
			}
			catch (Exception ex)
			{
                LogService.Fatal(ex, this);
                return new ServiceResponse<ApplyClearanceCertificateResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
			}
		}

        public ServiceResponse<clearanceMasterOutput> GetClearanceFieldMaster(string Fieldname, string Service, string Scenerio, SupportedLanguage requestLanguage, RequestSegment requestSegment)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var request = new GetClearanceFieldMaster
                    {
                        clearancemasterinput = new clearanceMasterInput
                        {
                            appidentifier = requestSegment.Identifier(),
                            appversion = AppVersion,
                            lang = requestLanguage.ToString(),
                            vendorid = GetVendorId(requestSegment),
                            mobileosversion = AppVersion,
                            fieldname = Fieldname,
                            service = Service,
                            scenario = Scenerio,
                            type = "1",
                            mode = "R",
                            selectall = ""
                        }

                    };

                    var response = client.GetClearanceFieldMaster(request);

                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<clearanceMasterOutput>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<clearanceMasterOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex,this);
                return new ServiceResponse<clearanceMasterOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<clearanceMasterOutput> GetClearanceFieldMaster(clearanceMasterInput input, SupportedLanguage requestLanguage, RequestSegment requestSegment)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var request = new GetClearanceFieldMaster
                    {
                        clearancemasterinput = new clearanceMasterInput
                        {
                            appidentifier = requestSegment.Identifier(),
                            appversion = AppVersion,
                            lang = requestLanguage.ToString(),
                            vendorid = GetVendorId(requestSegment),
                            mobileosversion = AppVersion,
                            fieldname = input.fieldname ?? "",
                            service = input.service ?? "",
                            scenario = input.scenario,
                            type = input.type ?? "1",
                            mode = input.mode ?? "R",
                            selectall = input.selectall ?? ""
                        }

                    };

                    var response = client.GetClearanceFieldMaster(request);

                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<clearanceMasterOutput>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<clearanceMasterOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<clearanceMasterOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        #endregion

        #region GetEasyPayEnquiry
        public ServiceResponse<GetEasyPayEnquiryResponse> GetEasyPayEnquiry(string userid, string sessionid, string easypaynumber, SupportedLanguage language, RequestSegment segment, string center = null)
        {
            using (var client = CreateProxy())
            {
                var request = new GetEasyPayEnquiry()
                {
                    userid = userid,
                    vendorid = GetVendorId(segment),
                    sessionid = sessionid,
                    mobileosversion = AppVersion,
                    appversion = AppVersion,
                    appidentifier = segment.Identifier(),
                    easypaynumber = easypaynumber,
                    center = center,
                    lang = language.Code()
                };
                try
                {
                    var response = client.GetEasyPayEnquiry(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetEasyPayEnquiryResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetEasyPayEnquiryResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetEasyPayEnquiryResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region Anonymous Bill Download
        public ServiceResponse<greenBillOutput> GetBillDetailsPDF(string billviewkey, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var request = new GetBillDetailsPDF
                {
                    greenbillinput = new greenBillInput
                    {
                        billviewkeycode = billviewkey,
                        vendorid = GetVendorId(segment),
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        mobileosversion = AppVersion,
                        lang = language.Code()
                    }
                };

                try
                {
                    var response = client.GetBillDetailsPDF(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<greenBillOutput>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<greenBillOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }

                }
                
                catch (Exception ex)
                {
                    var id = LogService.Fatal(ex, this);
                    return new ServiceResponse<greenBillOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion


        #region MoveoutAnonymous

        public ServiceResponse<SetMoveOutwithotpResponse> SetMoveOutwithOTP(string _contractaccountNumber, string _premiseNumber, string _appflag, string _branch, SupportedLanguage requestLanguage, RequestSegment requestSegment, string _mobile = "", string _email = "", string _otp = "", string _refundmethod = "", string _disconnectiondate = "", string _disconnectiontime = "", byte[] _attachment1 = null, string _attachmentname1 = "", byte[] _attachment2 = null, string _attachmentname2 = "", string ibannumber = "")
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var request = new SetMoveOutwithotp
                    {
                        moveoutOTPparams = new moveOutOTPParams
                        {
                            contractaccountnumber = _contractaccountNumber,
                            premisenumber = _premiseNumber,
                            appversion = AppVersion,
                            appidentifier = requestSegment.Identifier(),
                            vendorid = GetVendorId(requestSegment),
                            mobileosversion = AppVersion,
                            moveoutversion = "03",
                            lang = requestLanguage.ToString(),
                            branch = _branch,
                            applicationflag = _appflag,
                            mobile = _mobile,
                            email = _email,
                            otp = _otp,
                            refundmethod = _refundmethod,
                            disconnectiondate = _disconnectiondate,
                            disconnectiontime = _disconnectiontime,
                            attachment1 = _attachment1,
                            attachmentname1 = _attachmentname1,
                            attachment2 = _attachment2,
                            attachmentname2 = _attachmentname2,
                            ibannumber = ibannumber
                        }
                    };

                    var response = client.SetMoveOutwithotp(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<SetMoveOutwithotpResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<SetMoveOutwithotpResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
            }
            catch (Exception ex)
            {
                var id = LogService.Fatal(ex, this);
                return new ServiceResponse<SetMoveOutwithotpResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
			}
		}

        #endregion

        #region Miscellaneous Project
        public ServiceResponse<miscellaneousOutput> SetMiscellaneousRequest(miscellaneousInput input, SupportedLanguage language, RequestSegment segment)
        {
            input.lang = language.Code();
            input.vendorid = GetVendorId(segment);
            input.mobileosversion = AppVersion;
            input.appversion = AppVersion;
            input.appidentifier = segment.Identifier();
            var request = new SetMiscellaneousRequest
            {
                miscellaneousinput = input
            };
            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.SetMiscellaneousRequest(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<miscellaneousOutput>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<miscellaneousOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }

                }
                
                catch (Exception ex)
                {
                    var id = LogService.Fatal(ex, this);
                    return new ServiceResponse<miscellaneousOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion

        #region Dubai ID / UAE PASS / MyID
        public ServiceResponse<loginHelper> SetDubaiIDDetails(loginHelper input, SupportedLanguage language, RequestSegment segment)
        {
            input.mob_lang = language.Code();
            //input.vendorid = GetVendorId(segment);
            input.mob_mobileosver = AppVersion;
            input.mob_appver = AppVersion;
            input.mob_appidentifier = segment.Identifier();
            var request = new SetDubaiIDDetails
            {
                loginhelper = input
            };
            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.SetDubaiIDDetails(request);
                    if (response != null && response.@return != null && response.@return.errorCode == "000")
                    {
                        var typedresponse = new ServiceResponse<loginHelper>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<loginHelper>(response.@return, false, response.@return.errorReason);
                        return typedresponse;
                    }

                }
                
                catch (Exception ex)
                {
                    var id = LogService.Fatal(ex, this);
                    return new ServiceResponse<loginHelper>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region GetMasterDataWUlist
        public ServiceResponse<GetMasterDataWUResponse> GetMasterDataWUlist(string _countrycode, string _statecode, string _fetchflag, SupportedLanguage language, RequestSegment segment)
        {
            using (var client = CreateProxy())
            {
                var request = new GetMasterDataWU()
                {
                    countrycode = _countrycode,
                    statecode = _statecode,
                    fetchflag = _fetchflag,
                    lang = language.Code()
                };
                try
                {
                    var response = client.GetMasterDataWU(request);
                    if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<GetMasterDataWUResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetMasterDataWUResponse>(response, false, string.Empty);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetMasterDataWUResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region Estimation
        public ServiceResponse<estimateResponseParams> GetEstimateAmountDisplay(estimateRequestParams input, SupportedLanguage language, RequestSegment segment)
        {
            input.appidentifier = segment.Identifier();
            input.appversion = AppVersion;
            //input.contractaccountnumber
            //input.enddate
            //input.indicator
            input.lang = language.Code();
            //input.mobileosversion = "dsa";
            //input.projectdefination
            //input.sdnumber
            //input.sessionid
            //input.startdate
            input.vendorid = GetVendorId(segment);



            //-----testdata-----
            //input.appidentifier = "1";
            //input.appversion = "23";
            //input.indicator = "01";

            var request = new GetEstimateAmountDisplay
            {
                EstimateDetailsRetrieve = input
            };

            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.GetEstimateAmountDisplay(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<estimateResponseParams>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<estimateResponseParams>(response.@return, false, response.@return.responsecode);
                        return typedresponse;
                    }

                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<estimateResponseParams>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<newConnectionTaxDetails> GetNewConnectionTaxInvoicePDF(GetNewConnectionTaxInvoicePDF input, SupportedLanguage language, RequestSegment segment)
        {

            //input.userid =
            input.vendorid = GetVendorId(segment);
            //input.sessionid
            //input.sddocumentnumber
            input.mobileosversion = "dsa";
            input.appversion = AppVersion;
            input.appidentifier = segment.Identifier();

            input.lang = language.Code();



            //-----testdata-----
            input.appidentifier = "1";
            input.appversion = "23";


            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.GetNewConnectionTaxInvoicePDF(input);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<newConnectionTaxDetails>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<newConnectionTaxDetails>(response.@return, false, response.@return.responsecode);
                        return typedresponse;
                    }

                }
               
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<newConnectionTaxDetails>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }

        }
        public ServiceResponse<paymentHistoryDetails> GetPaymentHistoryV1(GetPaymentHistory input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetPaymentHistory
                {
                    userid = input.userid,
                    sessionid = input.sessionid,
                    contractaccountnumber = input.contractaccountnumber,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    mobileosversion = AppVersion,
                    estimatenumber = input.estimatenumber,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetPaymentHistory(request);


                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<paymentHistoryDetails>(response.@return);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responsecode == "105")
                    {
                        var typedresponse = new ServiceResponse<paymentHistoryDetails>(response.@return, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<paymentHistoryDetails>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }

                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<paymentHistoryDetails>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        #endregion

        #region Smart Response
        #region Get Premise Details - No Power - Violation + Meter Status + Outage + Meter status
        public ServiceResponse<GetPremiseDetailsResponse> GetPremiseDetails(GetPremiseDetails input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.UserCredentialIN.vendorid = GetVendorId(segment);
                request.UserCredentialIN.appidentifier = segment.Identifier();
                request.UserCredentialIN.appver = AppVersion;
                //request.UserCredentialIN. = language.Code();
                request.UserCredentialIN.mobileosver = AppVersion;
                request.UserCredentialIN.vendorid = GetVendorId(segment);

                try
                {
                    var response = client.GetPremiseDetails(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetPremiseDetailsResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetPremiseDetailsResponse>(response, false, response.@return.responseMessage);
                        return typedresponse;
                    }
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetPremiseDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region SetGuestComplaints
        public ServiceResponse<SetGuestComplaintsResponse> SetGuestComplaints(SetGuestComplaints input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                //request.UserCredentialIN.appidentifier = segment.Identifier();
                //request.UserCredentialIN.appver = AppVersion;
                ////request.UserCredentialIN. = language.Code();
                //request.UserCredentialIN.mobileosver = AppVersion;
                request.guestcomplaints.vendorid = GetVendorId(segment);
                request.guestcomplaints.lang = language.Code();
                try
                {
                    var response = client.SetGuestComplaints(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<SetGuestComplaintsResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<SetGuestComplaintsResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetGuestComplaintsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region SetLoginComplaints
        public ServiceResponse<SetLoginComplaintsResponse> SetLoginComplaints(SetLoginComplaints input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                if (request.logincomplaints != null)
                {
                    request.logincomplaints.vendorid = GetVendorId(segment);
                    request.logincomplaints.appidentifier = segment.Identifier();
                    request.logincomplaints.appversion = AppVersion;
                    request.logincomplaints.lang = language.Code();
                    request.logincomplaints.mobileosversion = AppVersion;
                    //request.logincomplaints.
                }

                try
                {
                    var response = client.SetLoginComplaints(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<SetLoginComplaintsResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<SetLoginComplaintsResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetLoginComplaintsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region GetCustomerSurveyQuestions
        public ServiceResponse<GetCustomerSurveyQuestionsResponse> GetCustomerSurveyQuestions(GetCustomerSurveyQuestions input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                //request.surveyquestioninput.ap = segment.Identifier();
                //request.surveyquestioninput.appver = AppVersion;
                //request.surveyquestioninput = AppVersion;
                request.surveyquestioninput.lang = language.Code();
                try
                {
                    var response = client.GetCustomerSurveyQuestions(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetCustomerSurveyQuestionsResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetCustomerSurveyQuestionsResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetCustomerSurveyQuestionsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region SetCustomerSurveyAnswers
        public ServiceResponse<SetCustomerSurveyAnswersResponse> SetCustomerSurveyAnswers(SetCustomerSurveyAnswers input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                //request.surveyquestioninput.ap = segment.Identifier();
                //request.surveyquestioninput.appver = AppVersion;
                //request.surveyquestioninput = AppVersion;
                request.answersinput.lang = language.Code();

                try
                {
                    var response = client.SetCustomerSurveyAnswers(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<SetCustomerSurveyAnswersResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<SetCustomerSurveyAnswersResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetCustomerSurveyAnswersResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region Send NotificationOTP
        public ServiceResponse<SendNotificationOTPResponse> SendNotificationOTP(SendNotificationOTP input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                //request.otpinput.vendorid = GetVendorId(segment);
                //request.otpinput.appidentifier = segment.Identifier();
                //request.otpinput.appversion = AppVersion;
                request.otpinput.lang = language.Code();
                //request.otpinput.mobileosversion = AppVersion;
                //request.logincomplaints.
                try
                {
                    var response = client.SendNotificationOTP(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<SendNotificationOTPResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<SendNotificationOTPResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SendNotificationOTPResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region Verify Notification OTP
        public ServiceResponse<VerifyNotificationOTPResponse> VerifyNotificationOTP(VerifyNotificationOTP input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                //request.otpinput.vendorid = GetVendorId(segment);
                //request.otpinput.appidentifier = segment.Identifier();
                //request.otpinput.appversion = AppVersion;
                request.otpinput.lang = language.Code();
                //request.otpinput.mobileosversion = AppVersion;
                //request.logincomplaints.
                try
                {
                    var response = client.VerifyNotificationOTP(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<VerifyNotificationOTPResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<VerifyNotificationOTPResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<VerifyNotificationOTPResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion

        #region Get Valid SmartMeter
        public ServiceResponse<GetValidSmartMeterResponse> GetValidSmartMeter(GetValidSmartMeter input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.UserCredentialIN.vendorid = GetVendorId(segment);
                request.UserCredentialIN.appidentifier = segment.Identifier();
                request.UserCredentialIN.appver = AppVersion;
                //request.UserCredentialIN. = language.Code();
                request.UserCredentialIN.mobileosver = AppVersion;
                request.UserCredentialIN.vendorid = GetVendorId(segment);
                try
                {
                    var response = client.GetValidSmartMeter(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetValidSmartMeterResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetValidSmartMeterResponse>(response, false, response.@return.responseMessage);
                        return typedresponse;
                    }
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetValidSmartMeterResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion

        #region Get Guest Track Complaints
        public ServiceResponse<GetGuestTrackComplaintsResponse> GetGuestTrackComplaints(GetGuestTrackComplaints input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.guesttrackinput.vendorid = GetVendorId(segment);
                request.guesttrackinput.appidentifier = segment.Identifier();
                request.guesttrackinput.appversion = AppVersion;
                request.guesttrackinput.lang = language.Code();
                request.guesttrackinput.mobileosversion = AppVersion;
                request.guesttrackinput.vendorid = GetVendorId(segment);

                try
                {
                    var response = client.GetGuestTrackComplaints(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetGuestTrackComplaintsResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetGuestTrackComplaintsResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetGuestTrackComplaintsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion

        #region Get Login Track Complaints
        public ServiceResponse<GetLoginTrackComplaintsResponse> GetLoginTrackComplaints(GetLoginTrackComplaints input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                //request.logintrackinput.vendorid = GetVendorId(segment);
                request.logintrackinput.appidentifier = segment.Identifier();
                request.logintrackinput.appversion = AppVersion;
                request.logintrackinput.lang = language.Code();
                request.logintrackinput.mobileosversion = AppVersion;
                request.logintrackinput.vendorid = GetVendorId(segment);
                try
                {
                    var response = client.GetLoginTrackComplaints(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetLoginTrackComplaintsResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetLoginTrackComplaintsResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetLoginTrackComplaintsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region GetContractAccountStatus
        public ServiceResponse<GetContractAccountStatusResponse> GetContractAccountStatus(GetContractAccountStatus input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.contractaccountinput.vendorid = GetVendorId(segment);
                request.contractaccountinput.appidentifier = segment.Identifier();
                request.contractaccountinput.appversion = AppVersion;
                //request.UserCredentialIN. = language.Code();
                request.contractaccountinput.mobileosversion = AppVersion;
                request.contractaccountinput.vendorid = GetVendorId(segment);
                try
                {
                    var response = client.GetContractAccountStatus(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetContractAccountStatusResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetContractAccountStatusResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetContractAccountStatusResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region SetCRMLoginComplaints
        public ServiceResponse<SetCRMInteractionResponse> SetCRMInteraction(SetCRMInteraction input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.logincomplaints.vendorid = GetVendorId(segment);
                request.logincomplaints.appidentifier = segment.Identifier();
                request.logincomplaints.appversion = AppVersion;
                //request.UserCredentialIN. = language.Code();
                request.logincomplaints.mobileosversion = AppVersion;
                request.logincomplaints.vendorid = GetVendorId(segment);
                try
                {
                    var response = client.SetCRMInteraction(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<SetCRMInteractionResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<SetCRMInteractionResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetCRMInteractionResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #region GetSRCompanyDetails
        public ServiceResponse<GetSRCompanyDetailsResponse> GetSRCompanyDetails(GetSRCompanyDetails input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                if (request != null)
                {
                    request.CompaniesInput.vendorid = GetVendorId(segment);
                    request.CompaniesInput.appidentifier = segment.Identifier();
                    request.CompaniesInput.appversion = AppVersion;
                    request.CompaniesInput.lang = language.Code();
                    request.CompaniesInput.mobileosversion = AppVersion;
                    //request.logincomplaints.
                }

                try
                {
                    // TODO: change implementation response code & description

                    var response = client.GetSRCompanyDetails(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")//&& response.@return.responsecode == "000"
                    {
                        var typedresponse = new ServiceResponse<GetSRCompanyDetailsResponse>(response);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetSRCompanyDetailsResponse>(response, false, response.@return.description); //response.@return.description
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetSRCompanyDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        #endregion

        public ServiceResponse<manageBeneficiaryResponse> ManageBeneficiary(ManageBeneficiary input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new ManageBeneficiary()
                {
                    sessionid = input.sessionid,
                    managemode = input.managemode,
                    beneficiarylist = input.beneficiarylist,
                    appversion = AppVersion,
                    appidentifier = segment.Identifier(),
                    lang = language.Code()
                };
                try
                {
                    var response = client.ManageBeneficiary(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<manageBeneficiaryResponse>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<manageBeneficiaryResponse>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<manageBeneficiaryResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        #region Customer Care Survey

        public ServiceResponse<surveyOutput> SetSurveyV3(SetSurveyV3 input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.surveyLinkInput.vendorid = GetVendorId(segment);
                request.surveyLinkInput.appidentifier = segment.Identifier();
                request.surveyLinkInput.appversion = AppVersion;
                request.surveyLinkInput.mobileosversion = AppVersion;
                request.surveyLinkInput.lang = language.Code();
                try
                {
                    var response = client.SetSurveyV3(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<surveyOutput>(response.@return, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<surveyOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<surveyOutput>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<surveyLinkOutput> GetSurveyV3Validation(string dynlink, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetSurveyV3Validation()
                {
                    dynlink = dynlink,
                    surveyLinkInput = new surveyLinkInput
                    {
                        vendorid = GetVendorId(segment),
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        mobileosversion = AppVersion,
                        lang = language.Code()
                    }
                };
                try
                {
                    var response = client.GetSurveyV3Validation(request);
                    if (response != null && response.@return != null)
                    {
                        if (response.@return.responsecode.Equals("000"))
                        {
                            var typedresponse = new ServiceResponse<surveyLinkOutput>(response.@return);
                            return typedresponse;
                        }
                        else
                        {
                            var typedresponse = new ServiceResponse<surveyLinkOutput>(null, false, response.@return.description);
                            return typedresponse;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                }
                return new ServiceResponse<surveyLinkOutput>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
            }
        }
        #endregion

        #region Smart Response live water saving
        public ServiceResponse<GetWaterSaveNotificationResponse> GetWaterSaveNotification(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateApplicationProxy())
            {
                var request = new GetWaterSaveNotification
                {
                    vendorid = GetVendorId(segment),
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetWaterSaveNotification(request);

                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetWaterSaveNotificationResponse>(response);
                        return typedresponse;
                    }
                    //else if (response != null && response.@return != null && response.@return.responsecode == "105")
                    //{
                    //    var typedresponse = new ServiceResponse<GetWaterSaveNotificationResponse>(response, true, response.@return.description);
                    //    return typedresponse;
                    //}
                    else
                    {
                        var typedresponse = new ServiceResponse<GetWaterSaveNotificationResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }

                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetWaterSaveNotificationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #endregion

        #region Conservation Awards
        public ServiceResponse<clearanceMasterOutput> GetConservationFieldMaster(string Fieldname, string Service, string Scenerio, SupportedLanguage requestLanguage, RequestSegment requestSegment)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var request = new GetClearanceFieldMaster
                    {
                        clearancemasterinput = new clearanceMasterInput
                        {
                            appidentifier = requestSegment.Identifier(),
                            appversion = AppVersion,
                            lang = requestLanguage.ToString(),
                            vendorid = GetVendorId(requestSegment),
                            mobileosversion = AppVersion,
                            fieldname = Fieldname,
                            service = Service,
                            scenario = Scenerio,
                            type = "1",
                            mode = "R",
                            selectall = "X"
                        }

                    };

                    var response = client.GetClearanceFieldMaster(request);

                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<clearanceMasterOutput>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<clearanceMasterOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<clearanceMasterOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion
        #region Customer Profile
        //GetProfileDetails
        //profileDetailsOutput
        public ServiceResponse<profileDetailsOutput> GetProfileDetails(string businessPartner, string credential, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var profileDetailsInput = new profileDetailsInput
                {
                    businessPartner = businessPartner,
                    sessionid = credential,
                    vendorid = GetRammasVendorId(RequestSegment.Desktop),
                    lang = language.Code(),

                };
                var request = new GetProfileDetails()
                {
                    profiledetailsinput = profileDetailsInput,
                };
                try
                {
                    var response = client.GetProfileDetails(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<profileDetailsOutput>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<profileDetailsOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<profileDetailsOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<profileSaveOutput> SetCustomerProfileSave(profileSaveInput profileSaveInputs, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                //profileSaveInput = new profileSaveInput
                //{
                //    userid = userId,
                //    vendorid = GetRammasVendorId(segment),
                //    sessionid = sessionId,
                //    lang = language.Code(),
                //    profileheader = profileHeaderDetails,
                //    podlist = profilePODDetails,
                //    bpCommunicationlist = bpCommunications,
                //};
                profileSaveInputs.userid = userId;
                profileSaveInputs.vendorid = GetRammasVendorId(segment);
                profileSaveInputs.sessionid = sessionId;
                profileSaveInputs.lang = language.Code();
                profileSaveInputs.serviceversion = ServiceVersion;
                var request = new SetCustomerProfileSave()
                {
                    profilesaveinput = profileSaveInputs,
                };
                try
                {
                    var response = client.SetCustomerProfileSave(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<profileSaveOutput>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<profileSaveOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<profileSaveOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<profileCustomerValidateOutput> GetCustomerProfileValidate(profileCustomerValidateInput customerdetailsinput, string iv_type, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                customerdetailsinput.userid = userId;
                customerdetailsinput.vendorid = GetRammasVendorId(segment);
                customerdetailsinput.sessionid = sessionId;
                customerdetailsinput.lang = language.Code();
                customerdetailsinput.type = iv_type;
                var request = new GetCustomerProfileValidate
                {
                    customerdetailsinput = customerdetailsinput,
                };
                try
                {
                    var response = client.GetCustomerProfileValidate(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<profileCustomerValidateOutput>(response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<profileCustomerValidateOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<profileCustomerValidateOutput>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        #endregion

        #region  Manage Account Information
        public ServiceResponse<profileContractAccountOutput> GetProfileContractAccountDetails(profileContractAccountInput input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetProfileContractAccountDetails
                {
                    profilecontractaccountinput = input
                };
                request.profilecontractaccountinput.lang = language.Code();
                request.profilecontractaccountinput.appidentifier = segment.Identifier();
                request.profilecontractaccountinput.appversion = AppVersion;
                request.profilecontractaccountinput.mobileosversion = AppVersion;
                request.profilecontractaccountinput.vendorid = GetVendorId(segment);

                try
                {
                    var response = client.GetProfileContractAccountDetails(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<profileContractAccountOutput>(response.@return, true, response.@return.description);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<profileContractAccountOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new Exception("Exception in GetProfileContractAccountDetails"), this);
                        return new ServiceResponse<profileContractAccountOutput>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<profileContractAccountOutput>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<string> SetContractAccountSave(contractAccountSaveInput input, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetContractAccountSave
                {
                    contractaccountsaveinput = input
                };
                request.contractaccountsaveinput.lang = language.Code();
                request.contractaccountsaveinput.appidentifier = segment.Identifier();
                request.contractaccountsaveinput.appversion = AppVersion;
                request.contractaccountsaveinput.mobileosversion = AppVersion;
                request.contractaccountsaveinput.vendorid = GetVendorId(segment);
                request.contractaccountsaveinput.userid = userId;
                request.contractaccountsaveinput.sessionid = sessionId;
                request.contractaccountsaveinput.serviceversion = ServiceVersion;

                try
                {
                    var response = client.SetContractAccountSave(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<string>(response.@return.requestnumber, true, response.@return.description);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<string>(string.Empty, false, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new Exception("Exception in SetContractAccountSave"), this);
                        return new ServiceResponse<string>(string.Empty, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(string.Empty, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<string> SetProfileEmailConfirmation(profileEmailInput input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetProfileEmailConfirmation
                {
                    profileemailinput = input
                };
                request.profileemailinput.lang = language.Code();
                request.profileemailinput.appidentifier = segment.Identifier();
                request.profileemailinput.appversion = AppVersion;
                request.profileemailinput.mobileosversion = AppVersion;
                request.profileemailinput.vendorid = GetVendorId(segment);


                try
                {
                    var response = client.SetProfileEmailConfirmation(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<string>("Success", true, response.@return.message);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<string>(string.Empty, false, response.@return.message);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new Exception("Exception in SetProfileEmailConfirmation"), this);
                        return new ServiceResponse<string>(string.Empty, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(string.Empty, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<string> SetProfileMobileConfirmation(profileMobileInput input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetProfileMobileConfirmation
                {
                    profilemobileinput = input
                };
                request.profilemobileinput.lang = language.Code();
                request.profilemobileinput.appidentifier = segment.Identifier();
                request.profilemobileinput.appversion = AppVersion;
                request.profilemobileinput.mobileosversion = AppVersion;
                request.profilemobileinput.vendorid = GetVendorId(segment);


                try
                {
                    var response = client.SetProfileMobileConfirmation(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<string>("Success", true, response.@return.description);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<string>(string.Empty, false, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new Exception("Exception in SetProfileMobileConfirmation"), this);
                        return new ServiceResponse<string>(string.Empty, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(string.Empty, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<otpRequestOutput> SetResendOTP(otpRequestInput otpRequest, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                otpRequest.lang = language.Code();
                var request = new SetResendOTP
                {
                    otprequestinput = otpRequest
                };

                try
                {
                    var response = client.SetResendOTP(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<otpRequestOutput>(response.@return, true, response.@return.description);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<otpRequestOutput>(response.@return, false, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new Exception("Exception in GetProfileContractAccountDetails"), this);
                        return new ServiceResponse<otpRequestOutput>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<otpRequestOutput>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<ContractAccountImage> SetImageContractAccount(SetImageContractAccount input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
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
                    var response = client.SetImageContractAccount(request);
                    var typedResponse = response.@return.DeserializeAs<ContractAccountImage>();
                    if (typedResponse != null && typedResponse.ResponseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<ContractAccountImage>(typedResponse, true, typedResponse.Description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<ContractAccountImage>(typedResponse, false, typedResponse.Description);
                        return typedresponse;
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                }
                return new ServiceResponse<ContractAccountImage>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
            }
        }

        #endregion

        #region Smart Alert
        public ServiceResponse<slabTarrifOut> GetSlabCaps(GetSlabCaps input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.slabTarrifIn.lang = language.Code();
                try
                {
                    var response = client.GetSlabCaps(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<slabTarrifOut>(response.@return, true, response.@return.responseMessage);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<slabTarrifOut>(response.@return, false, response.@return.responseMessage);
                        return typedresponse;
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<slabTarrifOut>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<slabTarrifOut>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<smartAlertSubrOut> GetSmartAlert(GetSmartAlert input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                try
                {
                    var response = client.GetSmartAlert(request);
                    if (response != null && response.@return != null && response.@return.errorCode == "000")
                    {
                        var typedresponse = new ServiceResponse<smartAlertSubrOut>(response.@return, true, response.@return.errorMessage);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<smartAlertSubrOut>(response.@return, false, response.@return.errorMessage);
                        return typedresponse;
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<smartAlertSubrOut>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<smartAlertSubrOut>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<smartAlertReadingOut> GetSmartConsumptionAlert(GetSmartConsumptionAlert input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                try
                {
                    var response = client.GetSmartConsumptionAlert(request);
                    if (response != null && response.@return != null && response.@return.errorCode == "000")
                    {
                        var typedresponse = new ServiceResponse<smartAlertReadingOut>(response.@return, true, response.@return.errorMessage);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<smartAlertReadingOut>(response.@return, false, response.@return.errorMessage);
                        return typedresponse;
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<smartAlertReadingOut>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<smartAlertReadingOut>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<smartAlertSubwOut> SetSubscribeSmartAlert(SetSubscribeSmartAlert input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                try
                {
                    var response = client.SetSubscribeSmartAlert(request);
                    if (response != null && response.@return != null && response.@return.errorCode == "000")
                    {
                        var typedresponse = new ServiceResponse<smartAlertSubwOut>(response.@return, true, response.@return.errorMessage);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<smartAlertSubwOut>(response.@return, false, response.@return.errorMessage);
                        return typedresponse;
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<smartAlertSubwOut>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<smartAlertSubwOut>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<slabPercentOut> GetSlabWisePercentage(GetSlabWisePercentage input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = input;
                request.slabPercentIn.lang = language.Code();
                try
                {
                    var response = client.GetSlabWisePercentage(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<slabPercentOut>(response.@return, true, response.@return.responseMessage);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<slabPercentOut>(response.@return, false, response.@return.responseMessage);
                        return typedresponse;
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<slabPercentOut>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<slabPercentOut>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }
        #endregion
        public ServiceResponse<SetNotificationSubmitResponse> SetNotificationSubmit(SetNotificationSubmit request, string _appflag, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {

                try
                {
                    request.NotificationInfoIn.userid = userId;
                    request.NotificationInfoIn.sessionid = sessionId;
                    request.NotificationInfoIn.vendorid = GetDTMCVendorId(segment);
                    request.NotificationInfoIn.appidentifier = segment.Identifier();
                    request.NotificationInfoIn.applicationflag = _appflag;
                    request.NotificationInfoIn.appversion = AppVersion;
                    request.NotificationInfoIn.lang = language.Code();


                    var response = client.SetNotificationSubmit(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<SetNotificationSubmitResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<SetNotificationSubmitResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new Exception("Exception in GetProfileContractAccountDetails"), this);
                        return new ServiceResponse<SetNotificationSubmitResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetNotificationSubmitResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetNotificationSubmitResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetShiftHolidayListResponse> GetShiftHolidayList(GetShiftHolidayList request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                request.ShiftHolidayList.credential = userId;
                //request.ShiftHolidayList.sessionid = sessionId;
                request.ShiftHolidayList.vendorid = GetDTMCVendorId(segment);
                //request.ShiftHolidayList.additionalinput = segment.Identifier();
                request.ShiftHolidayList.userid = userId;
                //request.ShiftHolidayList.appversion = AppVersion;
                request.ShiftHolidayList.language = language.Code();


                try
                {
                    var response = client.GetShiftHolidayList(request);
                    if (response != null && response.@return != null && response.@return.responsecode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetShiftHolidayListResponse>(response, true, response.@return.responsemessage);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<GetShiftHolidayListResponse>(response, false, response.@return.responsemessage);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new Exception("Exception in GetProfileContractAccountDetails"), this);
                        return new ServiceResponse<GetShiftHolidayListResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetShiftHolidayListResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetShiftHolidayListResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }
        #region [Power Outage]
        public ServiceResponse<SetOutageRequestResponse> SetOutageRequest(SetOutageRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {

                try
                {
                    if (request != null)
                    {
                        request.userid = userId;
                        request.sessionid = sessionId;
                        request.vendorid = GetVendorId(segment);
                        request.appidentifier = segment.Identifier();
                        request.appversion = AppVersion;
                        request.lang = language.Code();
                        request.mobileosversion = AppVersion;
                    }

                    var response = client.SetOutageRequest(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<SetOutageRequestResponse>(response, true, response.@return.responseCode);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<SetOutageRequestResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new Exception("Exception in GetProfileContractAccountDetails"), this);
                        return new ServiceResponse<SetOutageRequestResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetOutageRequestResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<SetOutageRequestResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetOutageDropDetailsXMLResponse> GetOutageDropDetails(GetOutageDropDetails request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {

                try
                {
                    if (request != null)
                    {
                        request.userid = userId;
                        request.sessionid = sessionId;
                        request.vendorid = GetVendorId(segment);
                        request.appidentifier = segment.Identifier();
                        request.appversion = AppVersion;
                        request.lang = language.Code();
                        request.mobileosversion = AppVersion;
                    }

                    var response = client.GetOutageDropDetails(request)?.@return.DeserializeAs<GetOutageDropDetailsXMLResponse>();
                    if (response != null && response.ResponseCode != null && response.ResponseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetOutageDropDetailsXMLResponse>(response, true, response.Description);
                        return typedresponse;
                    }
                    else if (response != null && response.ResponseCode != null)
                    {
                        var typedresponse = new ServiceResponse<GetOutageDropDetailsXMLResponse>(response, false, response.Description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new Exception("Exception in GetProfileContractAccountDetails"), this);
                        return new ServiceResponse<GetOutageDropDetailsXMLResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetOutageDropDetailsXMLResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetOutageDropDetailsXMLResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetOutageTrackerResponse> GetOutageTracker(GetOutageTracker request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    if (request != null)
                    {
                        request.userid = userId;
                        request.sessionid = sessionId;
                        request.vendorid = GetVendorId(segment);
                        request.appidentifier = segment.Identifier();
                        request.appversion = AppVersion;
                        request.lang = language.Code();
                        request.mobileosversion = AppVersion;
                    }


                    var response = client.GetOutageTracker(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<GetOutageTrackerResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null)
                    {
                        var typedresponse = new ServiceResponse<GetOutageTrackerResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        LogService.Fatal(new Exception("Exception in GetProfileContractAccountDetails"), this);
                        return new ServiceResponse<GetOutageTrackerResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                    }
                }
                catch (TimeoutException ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetOutageTrackerResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetOutageTrackerResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
        }
        #endregion
    }

    public class CustomAuthenticationBehavior : IEndpointBehavior
    {
        /// <summary>
        ///     The sap API key.
        /// </summary>
        private string sapApiKey;
        private string token;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAuthenticationBehavior"/> class. 
        /// Initializes a new instance of the
        ///     <see cref="CustomAuthenticationBehavior"/> class.
        /// </summary>
        /// <param name="sapKey">
        /// The authentication token.
        /// </param>
        public CustomAuthenticationBehavior(string sapKey, string token)
        {
            this.sapApiKey = sapKey;
            this.token = token;
        }

        /// <summary>
        /// The add binding parameters.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="bindingParameters">
        /// The binding parameters.
        /// </param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// The apply client behavior.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="clientRuntime">
        /// The client runtime.
        /// </param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            if (clientRuntime != null)
            {
                clientRuntime.ClientMessageInspectors.Add(new CustomMessageInspector(sapApiKey, token));
            }
        }

        /// <summary>
        /// The apply dispatch behavior.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="endpointDispatcher">
        /// The <paramref name="endpoint"/> dispatcher.
        /// </param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class CustomMessageInspector : IClientMessageInspector
    {
        /// <summary>
        ///     The sap API key.
        /// </summary>
        private readonly string sapApiKey;
        private readonly string token;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMessageInspector"/> class. 
        /// Initializes a new instance of the
        ///     <see cref="CustomMessageInspector"/> class.
        /// </summary>
        /// <param name="sapKey">
        /// The authentication token.
        /// </param>
        public CustomMessageInspector(string sapKey, string token)
        {
            this.sapApiKey = sapKey;
            this.token = token;
        }

        /// <summary>
        /// The after receive reply.
        /// </summary>
        /// <param name="reply">
        /// The reply.
        /// </param>
        /// <param name="correlationState">
        /// The correlation state.
        /// </param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        /// <summary>
        /// The before send request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// The <see cref="System.Object"/> .
        /// </returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var reqMsgProperty = new HttpRequestMessageProperty();
            reqMsgProperty.Headers.Add("apikey", this.sapApiKey);
            reqMsgProperty.Headers.Add("Authorization", this.token);
            if (request != null)
            {
                request.Properties[HttpRequestMessageProperty.Name] = reqMsgProperty;
            }

            return null;
        }

    }
}
