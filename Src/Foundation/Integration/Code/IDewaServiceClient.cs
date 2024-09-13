using System;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Integration.Responses.PowerOutage;

namespace DEWAXP.Foundation.Integration
{
    public interface IDewaServiceClient
    {
        /// <summary>
        /// Verifies whether a user identifier is available for use.
        /// </summary>
        /// <param name="userId">The user identifier to verify</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <param name="language">Desired language of the response message</param>
        /// <returns></returns>
        ServiceResponse<UserIdentifierAvailabilityResponse> VerifyUserIdentifierAvailable(string userId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves the image (if available) for a contract account
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="contractAccountNumber">The account number to be queried.</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A session identifier for further requests</returns>
        ServiceResponse<byte[]> GetAccountImage(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Sets the Premise Type for an account
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="caNumber">The account to be queried</param>
        /// <param name="premiseNumber">The legacy premise number of the account</param>
        /// <param name="mobileNumber">The user's mobile number</param>
        /// <param name="remarks">User's remarks</param>
        /// <param name="file">The attachment file</param>
        /// <param name="language">Desired language of the response message</param>
        /// <returns></returns>
        ServiceResponse<PremiseTypeChangeResponse> ChangePremiseType(string userId, string sessionId, PremiseTypeChangeRequest model, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);


        /// <summary>
        /// Retrieves the image (if available) for a contract account
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="contractAccountNumber">The account number to be queried.</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A session identifier for further requests</returns>
        Task<ServiceResponse<byte[]>> GetAccountImageAsync(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Authenticates the user using the supplied credentials
        /// Relates to SAP WS4 & WS5
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="password">The requesting user's password</param>
        /// <param name="governmental">When true, an alternative authentication mechanism will be used.</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A session identifier for further requests</returns>
        ServiceResponse<string> Authenticate(string userId, string password, bool governmental = false, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Authenticates the user using the supplied credentials
        /// Relates to SAP WS4 & WS5
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="password">The requesting user's password</param>
        /// <param name="governmental">When true, an alternative authentication mechanism will be used.</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A session identifier for further requests</returns>
        //ServiceResponse<LoginResponse> AuthenticateNew(string userId, string password, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string center = null);

        /// <summary>
        /// Authenticates a MyID user given a known Emirates ID
        /// Relates to SAP WS17
        /// </summary>
        /// <param name="emiratesIdentifier">The requesting user's unique identifier</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A session identifier for further requests</returns>
        ServiceResponse<MyIdAuthenticationResponse> Authenticate(string emiratesIdentifier, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Closes the session for an authenticated user
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="userType">The type of user account</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse Logout(string userId, string sessionId, UserType userType = UserType.Customer, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Requests that a password reset notification be sent to the specified user.
        /// Relates to SAP WS2
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="email">The email address to which the notification should be sent</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        ServiceResponse ResetPassword(string userId, string email, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Sets a password for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionToken"></param>
        /// <param name="flag"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse SetNewPassword(string userId, string sessionToken, string flag, string password, string confirmPassword,
            SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Changes the given customer's password
        /// Relates to SAP WS6
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="oldPassword">The current user password</param>
        /// <param name="newPassword">The desired password</param>
        /// <param name="confirmPassword">The confirmed desired password</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        ServiceResponse ChangePassword(string userId, string sessionId, string oldPassword, string newPassword, string confirmPassword, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Requests that the user identifier be emailed to the requesting user
        /// Relates to SAP WS3
        /// </summary>
        /// <param name="bpNumber">The requesting user's BP number</param>
        /// <param name="email">The email address to which the notification should be sent</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        ServiceResponse RequestUserIdentifierReminder(string bpNumber, string email, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Requests a list of contract accounts for the given user
        /// Relates to SAP WS8
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="includeBillingDetails">When true, billing details will be retrieved in addition to standard account information</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A list of contract accounts and balances</returns>
        ServiceResponse<AccountDetails[]> GetAccountList(string userId, string sessionId, bool includeBillingDetails = false, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Requests a list of contract accounts for the given user
        /// Relates to SAP WS8
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="includeBillingDetails">When true, billing details will be retrieved in addition to standard account information</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A list of contract accounts and balances</returns>
        ServiceResponse<AccountDetails[]> GetCAList(string userId, string sessionId, string checkMoveOut = "", string ServiceFlag = "", bool includeInactive = false, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Requests a list of invoices and payments
        /// Relates to SAP WS23
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="contractAccountNumber">The contract account for which to request payment history</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A list of invoices and payments</returns>
        ServiceResponse<ContractAccountPaymentListResponse> GetTransactionHistory(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Requests a list of invoices and payments
        /// Relates to SAP WS23
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="degTransactionId">The DEG transaction ID to query</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A list of invoices and payments</returns>
        ServiceResponse<Receipt[]> GetReceipts(string userId, string sessionId, string degTransactionId, string dewaTransactionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves a list of complaints or enquiries issued by a customer
        /// Relates to SAP WS15
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="bpNumber">The requesting user's BP number</param>
        /// <param name="contractAccountNumber">The contract account to filter complaints/enquiries by</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A list of complaints/enquiries</returns>
        ServiceResponse<CustomerEnquiry[]> GetCustomerEnquiries(string userId, string sessionId, string bpNumber, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string others = null);

        /// <summary>
        /// Retrieves the details of a specific customer complaint/enquiry
        /// Relates to SAP WS15
        /// </summary>
        /// <param name="enquiryNumber">Unique enquiry/complaint reference number</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<CustomerEnquiry> GetCustomerEnquiry(string enquiryNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves the bill information for the given contract account
        /// Relates to SAP WS10
        /// </summary>
        /// <param name="contractAccountNumber">The contract account number to query</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<BillEnquiryResponse> GetBill(string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string center = null, string sessionId = null);

        /// <summary>
        /// Retrieves the billing document
        /// Relates to SAP WS23
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="invoiceNumber">The invoice number to query</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>A binary file that can be downloaded by the customer</returns>
        ServiceResponse<byte[]> GetBill(string userId, string sessionId, string invoiceNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves the user's nominated primary account
        /// Relates to SAP WS7
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<PrimaryAccountResponse> GetPrimaryAccount(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Nominates the user's primary account
        /// Relates to SAP WS7
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="contractAccountNumber">The account to be set as primary</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse SetPrimaryAccount(string userId, string sessionId, string contractAccountNumber, string terms, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves the contact details related to a contract account
        /// Relates to SAP WS14
        /// </summary>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="contractAccountNumber">The account to be set as primary</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<AccountContactDetails> GetContactDetails(string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Changes the contact details related to a contract accountprim
        /// Relates to SAP WS14
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="params">Updated contact details</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse ChangeContactDetails(string userId, string sessionId, ChangeContactDetails @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Requests that a final bill be issued to the requesting party
        /// Relates to SAP WS13
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="params">Request details and attachments</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<string> RequestFinalBill(string userId, string sessionId, RequestFinalBill @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Lodges a billing complaint
        /// Relates to SAP WS16
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="params">Request details and attachments</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<string> LodgeBillingComplaint(string userId, string sessionId, LodgeBillingComplaint @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// <param name="segment">The client segment issuing the request</param>
        /// Submits a request for the issuing of a clearance certificate
        /// Relates to SAP WS18
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="params">Request details and attachments</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<ClearanceCertificateApplicationResponse> RequestClearanceCertificate(string userId, string sessionId, RequestClearanceCertificate @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string center = null);

        /// <summary>
        /// Retrieves a list of cities
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse GetCityList(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Submits a request for the a temporary electric connection
        /// Relates to SAP WS24
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="params">Application details</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<TemporaryConnectionApplicationResponse> RequestTemporaryConnection(string userId, string sessionId, RequestTemporaryConnection @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves the status information of a previously lodged temporary connection request
        /// Relates to SAP WS24
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="reference">Application reference</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<TemporaryConnectionDetails> GetTemporaryConnectionDetails(string userId, string sessionId, string reference, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves metadata required for a customer to lodge a complaint
        /// Relates to SAP WS25
        /// </summary>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<ComplaintCriteriaResponse> GetServiceComplaintCriteria(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Gets a list of service interruptions for a user
        /// J77
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<ServiceInterruptionResponse> GetServiceInterruptionList(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Lodges a service complaint
        /// Relates to SAP WS25
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="params">Complaint details</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<string> LodgeServiceComplaint(string userId, string sessionId, LodgeServiceComplaint @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves electricity and water usage metrics for the specified contract account
        /// Relates to SAP WS27
        /// </summary>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="contractAccountNumber">The account to be queried</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<YearlyConsumptionDataResponse> GetConsumptionHistory(string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves electricity and water usage metrics for the specified contract accounts
        /// Relates to SAP WS28
        /// </summary>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="contractAccountNumbers">The accounts to be queried (maximum of two allowed)</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<ComparativeConsumptionResponse> GetComparativeConsumption(string sessionId, string[] contractAccountNumbers, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves electricity and water usage metrics for the specified contract account, relative to the surrounding neighbourhood
        /// Relates to SAP WS29
        /// </summary>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="contractAccountNumber">The account to be queried</param>
        /// <param name="premiseNumber">The legacy premise number of the account</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<YearlyAverageConsumptionDataResponse> GetComparativeConsumption(string behaviourIndicator, string sessionId, string contractAccountNumber, string premiseNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieves the estimated carbon footprint for the specified contract account
        /// Relates to SAP WS45
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="sessionId">The requesting user's active session identifier</param>
        /// <param name="contractAccountNumber">The account to be queried</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<decimal> GetCarbonFootprintReading(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieve business partner contact details (party hidden) by sending the BP number.
        /// Relates to SAP WS1
        /// </summary>
        /// <param name="bpNumber"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns>Will return email and mobile associated with the account </returns>
        ServiceResponse<BusinessPartnerDetailsResponse> GetBusinessPartnerDetailsForRegistration(string bpNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Sends verification code to selected messaging service.
        /// Relates to SAP WS1
        /// </summary>
        /// <param name="bpNumber"></param>
        /// <param name="sendToMobile"></param>
        /// <param name="sendToEmail"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<SendVerificationResponse> SendVerificationCodeForRegistration(string bpNumber, bool sendToMobile, bool sendToEmail, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Validates if the code entered is correct.
        /// Relates to SAP WS1
        /// </summary>
        /// <param name="bpNumber"></param>
        /// <param name="sendToMobile"></param>
        /// <param name="sendToEmail"></param>
        /// <param name="verificationCode"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<SendVerificationResponse> ConfirmVerificationCodeForRegistration(string bpNumber, bool sendToMobile, bool sendToEmail, string verificationCode, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Registers customer
        /// Relates to SAP WS1
        /// </summary>
        /// <param name="bpNumber"></param>
        /// <param name="updatedMobile"></param>
        /// <param name="updatedEmail"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <param name="verificationCode"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<RegisterCustomerResponse> RegisterCustomer(string bpNumber, string updatedMobile, string updatedEmail, string username,
             string password, string confirmPassword, string verificationCode, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Retrieve the profile image associated with the requesting user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="sessionId"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<byte[]> GetProfilePhoto(string userName, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Asynchronously retrieve the profile image associated with the requesting user
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageType"></param>
        /// <param name="userName"></param>
        /// <param name="sessionId"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        Task<ServiceResponse<byte[]>> GetProfilePhotoAsync(string userName, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Upload an image to user account
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageType"></param>
        /// <param name="userName"></param>
        /// <param name="sessionId"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse UploadProfilePhoto(byte[] image, string imageType, string userName, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Gets the estimate based on an estimate number
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="sessionId"></param>
        /// <param name="estimateNumber"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<EstimateNumDetailsResponse> GetEstimateNumDetails(string userName, string sessionId, string estimateNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Gets the estimate pdf
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="sessionId"></param>
        /// <param name="estimateNumber"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<byte[]> GetEstimatePdf(string userName, string sessionId, string estimateNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<EstimateHistoryResponse> GetEstimateHistory(string userName, string sessionId, bool payForFriend, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<EstimateLineItems> GetEstimates(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Fetch a list of the local purchase RFX
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<OpenRfxInquiriesResponse> GetOpenRfxInquiries(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Requests collective account
        /// Relates to SAP WS42
        /// </summary>
        /// <param name="params"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<SetRequestCollectiveAccountResponse> RequestCollectiveAccount(RequestCollectiveAccountParameters @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Adds to collective account
        /// Relates to SAP WS43
        /// </summary>
        /// <param name="@params"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        ServiceResponse<SetRequestCollectiveAccountResponse> AddToCollectiveBilling(RequestCollectiveAccountParameters @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Gets user details and amount to pay for rera move-in
        /// j73 move-in-RERA
        /// </summary>
        /// <param name="caNumber"></param>
        /// <param name="bpNumber"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<ReraGetSecurityDepositDetailsResponse> GetReraSecurityDepositDetails(string caNumber, string bpNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);


        /// <summary>
        /// Light call to send new user details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<MoveInLite> SendMoveInCredentials(string userId, string password, string confirmPassword, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Send an attachment into SAP for moving in
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="transactionRef"></param>
        /// <param name="fileName"></param>
        /// <param name="attachmentType"></param>
        /// <param name="file"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<MoveInAttachmentResponse> SendMoveInAttachment(string userId, string sessionId, string transactionRef, string[] transactionList, string fileName, string attachmentType, byte[] file, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Sets a users move in details
        /// Also used to get the initial move-in cost for a premise
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <param name="sessionId"></param>
        /// <param name="customerType"></param>
        /// <param name="accountType"></param>
        /// <param name="premiseNumber"></param>
        /// <param name="idType"></param>
        /// <param name="emiratesId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="pobox"></param>
        /// <param name="nation"></param>
        /// <param name="mobile"></param>
        /// <param name="email"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="screenNo"></param>
        /// <param name="businessPartnerNo"></param>
        /// <param name="transactionId"></param>
        /// <param name="subId"></param>
        /// <param name="idExpiry"></param>
        /// <param name="contractValue"></param>
        /// <param name="customerCategory"></param>
        /// <param name="numberOfRooms"></param>
        /// <param name="region"></param>
        /// <param name="loginMode"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<MoveInResponse> MoveIn(string userId, string password, string confirmPassword, string sessionId, string customerType, string accountType, string premiseNumber, string idType, string emiratesId, string firstName, string lastName, string pobox, string nation, string mobile, string email, DateTime? startDate, DateTime? endDate, string screenNo, string businessPartnerNo, string transactionId, string subId, DateTime? idExpiry, string contractValue, string customerCategory, int? numberOfRooms, string region, string loginMode, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Sets a users move in details
        /// Also used to get the initial move-in cost for a premise
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <param name="sessionId"></param>
        /// <param name="customerType"></param>
        /// <param name="idType"></param>
        /// <param name="emiratesId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="pobox"></param>
        /// <param name="nation"></param>
        /// <param name="mobile"></param>
        /// <param name="email"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="businessPartnerNo"></param>
        /// <param name="idExpiry"></param>
        /// <param name="contractValue"></param>
        /// <param name="customerCategory"></param>
        /// <param name="numberOfRooms"></param>
        /// <param name="ejariNumber"></param>
        /// <param name="premiselistField"></param>
        /// <param name="region"></param>
        /// <param name="loginMode"></param>
        /// <param name="language"></param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<MoveInEjariResponse> MoveInEjariRequest(string userId, string createuseraccount, string password, string confirmPassword, string sessionId, string customerType, string idType, string emiratesId, string firstName, string lastName, string pobox, string nation, string mobile, string email, DateTime? moveinDate, DateTime? moveoutDate, string moveoutAccountnumber, DateTime? startDate, DateTime? endDate, string businessPartnerNo, DateTime? idExpiry, string contractValue, string customerCategory, int? numberOfRooms, string region, string loginMode, string ejariNumber, string[] premiselistField, string processmovein, string vatnumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string center = null);

        /// <summary>
        /// Sets a users move in details
        /// Also used to get the initial move-in cost for a premise
        /// </summary>		
        /// <param name="input">request object moveInPostInput</param>
        /// <returns></returns>
        ServiceResponse<moveInPostOutput> SetMoveInPostRequest(moveInPostInput input, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
		/// Sets a users move in details
		/// Also used to get the initial move-in cost for a premise
		/// </summary>		
		/// <param name="input">request object moveInPostInput</param>
		/// <returns></returns>
        ServiceResponse<moveInReadOutput> SetMoveInReadRequest(moveInReadInput input, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Requests change customer category
        /// Relates to SAP WS39
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="premiseNumber"></param>
        /// <param name="contractAccountNumber"></param>
        /// <param name="bpNumber"></param>
        /// <param name="mobile"></param>
        /// <param name="remarks"></param>
        /// <param name="attachment"></param>
        /// <param name="attachmentType"></param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        /// <returns></returns>
        ServiceResponse<SetRateCategoryResponse> ChangeCustomerCategory(string userId, string sessionId, string premiseNumber, string contractAccountNumber, string bpNumber, string mobile, string remarks, byte[] attachment, string attachmentType, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        ///
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        ServiceResponse<GetGovernmentObservationDetailsResponse> SubmitGovernmentObservation(string userId, string sessionId,
            SubmitGovernmentObservation @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <param name="language"></param>
        /// <returns></returns>

        ServiceResponse<GovermentUserDetailsResponse> GetGovernmentalUserDetails(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<RoadWorksListResponse> GetRoadWorksList(string sessionId, string bpNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Requests change landlord information
        /// Relates to SAP WS44
        /// </summary>
        /// <param name="params"></param>
        /// <param name="language"></param>
        /// <returns>SetLandlordDetailsResponse</returns>
        ServiceResponse<SetLandlordDetailsResponse> ChangeLandlordInformation(LandlordDetailsParameters @params,
            SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Gets Email List For Mobile Number
        /// Relates to SAP WS51
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="language"></param>
        ServiceResponse<MarketingEmailDetails[]> GetEmailListForMobileNumber(string userId, string sessionId,
            SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Set Email Marketing Email Prefernce
        /// Relates to SAP WS51
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="email"></param>
        /// <param name="subscriptionFlag"></param>
        /// <param name="language"></param>
        ServiceResponse SetMarketingPreferenceEmail(string userId, string sessionId, string email, string subscriptionFlag, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Activate move in account
        /// </summary>
        /// <param name="user"></param>
        /// <param name="language"></param>
        ServiceResponse<MoveinActivationResponse> SetUserActivation(string user, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<ClearanceCertificateDetails> GetClearanceCertificate(string userid, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string center = null);

        ServiceResponse<GetCustomerDetailsServiceResponse> GetCustomerDetails(string userid, string sessionid, SupportedLanguage language, RequestSegment segment);

        ServiceResponse<ConnectionEnquiryResponse> SubmitConnectionEnquiry(RequestConnectionEnquiry @params, SupportedLanguage language, RequestSegment segment);

        /// <summary>
        /// Get the Complaint Survey Questionare from SAP
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="sessionid"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<ComplaintsSurveyQuestionnaireResponse> GetComplaintsSurveyQuestionnaire(string notificationkey,
            SupportedLanguage language, RequestSegment segment);

        /// <summary>
        /// /// Submit Survey Questionare to SAP
        /// </summary>
        /// <param name="notificationkey"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<ComplaintsSurveyFeedbackResponse> SetComplaintsSurveyFeedback(ComplaintsSurveyFeedback @params,
            SupportedLanguage language, RequestSegment segment);

        ServiceResponse<CollectiveStatementResponse> GetCollectiveAccounts(string userId, string sessionId,
                                                                           SupportedLanguage language, RequestSegment segment);

        byte[] GetCollectiveStatementPDF(string userId, string sessionId, string contractAccount, string year, string month, SupportedLanguage language, RequestSegment segment);


        /// <summary>
        /// SAP WS
        /// Get the Complaint Survey Questionare from SAP
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="sessionid"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>

        ServiceResponse<IBANListResponse> IBANList(string userId, string sessionId, string contractAccount, string bpnumber, SupportedLanguage language, RequestSegment segment);

        ServiceResponse<UpdateIBANResponse> UpdateIBAN(string userId, string sessionId, string contractAccount, string ibanNumber, string bpnumber, string chequeiban, string address, SupportedLanguage language, RequestSegment segment);

        /// <summary>
        //  GetBehaviourinsightCustomer
        /// Relates to SAP WS 
        /// </summary>
        /// <param name="contractAccountNumber"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <param name="uniqueID"></param>
        /// <returns>GetBehaviourinsightCustomerDataResponse</returns>
        ServiceResponse<GetBehaviourinsightCustomerDataResponse> GetBehaviourinsightCustomer(
           string uniqueID, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse UpdatePrimaryContactDetails(string userId, string sessionId, string emailAddress, string mobileNumber, SupportedLanguage language, RequestSegment segment);
        ServiceResponse<SmartWalletAdvanceResponse> GetSmartWalletAdvance(string userId, string sessionToken, subscribeDetails[] subscribeDetails, SupportedLanguage requestLanguage, RequestSegment requestSegment);
        ServiceResponse<SmartWalletSubscribeResponse> SaveSmartWalletSubscription(string userId, string sessionToken, string v, subscribeDetails[] subscribeDetails, SupportedLanguage requestLanguage, RequestSegment requestSegment);
        ServiceResponse<MoveOutAccountResponse> GetMoveOutDetails(string[] contractAccounts, string userID, string sessionID, string checkmoveout, string offlinemode, string checkamount, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        string GetOutstandingAmount(string[] contractAccounts, SupportedLanguage language, RequestSegment segment);
        string ValidateAccounts(string userId, string sessionId, string[] contractAccounts, string[] amounts, string[] finalbills, SupportedLanguage language, RequestSegment segment);
        string GenerateRefNo(string userId, string sessionId, string[] contractAccounts, string[] amounts, string[] finalBillList, string email, string mobileNo, SupportedLanguage language, RequestSegment segment);
        string GetRefNoDetails(string userId, string sessionId, string refNo, SupportedLanguage language, RequestSegment segment);
        string GetRefNoList(string userId, string sessionId, SupportedLanguage language, RequestSegment segment);
        ServiceResponse<SetSubsciptionPreferencesResponse> SetSubscription(string contractaccount, string readwritemode, string email, string emailflag, string mobile, string mobileflag, SupportedLanguage requestLanguage, RequestSegment requestSegment);
        //ServiceResponse<SmartWalletAdvanceResponse> GetSmartWalletAdvance(string userID, string sessionID, subscribeDetails[] subscribeDetails, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        //void SaveSmartWalletSubscription(string userID, string sessionID, string subscribe, subscribeDetails[] subscribeDetails, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// To Check the contract account is present for the user Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contractaccount"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse GetContractAccountUserIDCheck(string userId, string contractaccount, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<GetVatDetailsForBPResponse> GetVatNumber(string userid, string sessionId, SupportedLanguage requestLanguage, RequestSegment requestSegment);
        ServiceResponse<SetVatDetailsForBPResponse> SetVatNumber(string contractaccountnumber, string businesspartnernumber, string region, string vatnumber, byte[] vatdocument, string userid, string sessionId, SupportedLanguage requestLanguage, RequestSegment requestSegment);
        ServiceResponse<SetMoveOutRequestResponse> SetMoveOutRequest(moveOutParams moveoutparams, string userid, string sessionId, SupportedLanguage requestLanguage, RequestSegment requestSegment);

        /// <summary>
		/// Requests a list of invoices and payments V2
		/// Relates to SAP WS23
		/// </summary>
		/// <param name="userId">The requesting user's unique identifier</param>
		/// <param name="sessionId">The requesting user's active session identifier</param>
		/// <param name="contractAccountNumber">The contract account for which to request payment history</param>
		/// <param name="language">Desired language of the response message</param>
		/// <param name="segment">The client segment issuing the request</param>
		/// <returns>A list of invoices and payments</returns>
		//ServiceResponse<paymentHistoryDetails> GetPaymentHistory(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<byte[]> GetStatementofAccountsPDF(string userId, string sessionId, string contractAccountNumber, string numberofmonths, string frommonth, string tomonth, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<paymentReceiptDetails> GetOnlinePaymentReceipt(string userId, string sessionId, string transactionid, string date, string type, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        #region New Clearance Certficate V2
        ServiceResponse<VerifyClearanceCertificateResponse> GetVerifyClearanceCertificate(string referenceNumber, string pinNumber, string urlParameter, SupportedLanguage requestLanguage, RequestSegment requestSegment, string pinFlag = null);

        ServiceResponse<GetContractAccountClearanceDetailsResponse> GetContractAccountClearanceDetails(string userid, string sessionId, string contractAccountNumber, SupportedLanguage requestLanguage, RequestSegment requestSegment, string purpose = "");

        ServiceResponse<GetContractAccountClearanceDetailsResponse> GetContractAccountIdentityDetails(GetContractAccountClearanceDetails contractAccountClearanceDetails, string userid, string sessionId, string contractAccountNumber, SupportedLanguage requestLanguage, RequestSegment requestSegment, string purpose = null);

        ServiceResponse<ApplyClearanceCertificateResponse> ApplyClearanceCertificateDEWACustomer(string userid, string sessionId,
            string branch,
            string contractAccountNumber,
            string remarks, string emailAddress,
            string firstName, string lastName, string mobileNumber, SupportedLanguage requestLanguage, RequestSegment requestSegment, string langCode = "EN");

        ServiceResponse<ApplyClearanceCertificateResponse> ApplyClearanceCertificate(RequestClearanceCertificate @params, string userid, string sessionId, SupportedLanguage requestLanguage, RequestSegment requestSegment);

        ServiceResponse<clearanceMasterOutput> GetClearanceFieldMaster(string Fieldname, string Service, string Scenerio, SupportedLanguage requestLanguage, RequestSegment requestSegment);
        ServiceResponse<clearanceMasterOutput> GetClearanceFieldMaster(clearanceMasterInput input, SupportedLanguage requestLanguage, RequestSegment requestSegment);
        #endregion

        ServiceResponse<GetEasyPayEnquiryResponse> GetEasyPayEnquiry(string userid, string sessionid, string easypaynumber, SupportedLanguage language, RequestSegment segment, string center = null);
        #region MoveoutAnonymouswithOTP

        ServiceResponse<SetMoveOutwithotpResponse> SetMoveOutwithOTP(string _contractaccountNumber, string _premiseNumber, string _appflag, string _branch, SupportedLanguage requestLanguage, RequestSegment requestSegment, string _mobile = "", string _email = "", string _otp = "", string _refundmethod = "", string _disconnectiondate = "", string _disconnectiontime = "", byte[] _attachment1 = null, string _attachmentname1 = "", byte[] _attachment2 = null, string _attachmentname2 = "", string ibannumber = "");

        #endregion

        ServiceResponse<greenBillOutput> GetBillDetailsPDF(string billviewkey, SupportedLanguage language, RequestSegment segment);

        ServiceResponse<miscellaneousOutput> SetMiscellaneousRequest(miscellaneousInput input, SupportedLanguage language, RequestSegment segment);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">login helper class</param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<loginHelper> SetDubaiIDDetails(loginHelper input, SupportedLanguage language, RequestSegment segment);
        ServiceResponse<GetMasterDataWUResponse> GetMasterDataWUlist(string _countrycode, string _statecode, string _fetchflag, SupportedLanguage language, RequestSegment segment);

        ServiceResponse<refundHistory> GetRefundHistory(string userId, string sessionId, string contractAccountNumber,string businesspartner, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// Get Premise Details - No Power - Violation + Meter Status + Outage
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<GetPremiseDetailsResponse> GetPremiseDetails(GetPremiseDetails input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// SetLoginComplaints - Create Notification
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<SetLoginComplaintsResponse> SetLoginComplaints(SetLoginComplaints input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// SetGuestComplaints - Create Notification
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<SetGuestComplaintsResponse> SetGuestComplaints(SetGuestComplaints input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Get Customer Survey Questions
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<GetCustomerSurveyQuestionsResponse> GetCustomerSurveyQuestions(GetCustomerSurveyQuestions input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Set Customer Survey Answers
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<SetCustomerSurveyAnswersResponse> SetCustomerSurveyAnswers(SetCustomerSurveyAnswers input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Send Notification OTP
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<SendNotificationOTPResponse> SendNotificationOTP(SendNotificationOTP input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Set Verify Notification OTP
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<VerifyNotificationOTPResponse> VerifyNotificationOTP(VerifyNotificationOTP input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<GetValidSmartMeterResponse> GetValidSmartMeter(GetValidSmartMeter input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Get Guest Track Complaints
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<GetGuestTrackComplaintsResponse> GetGuestTrackComplaints(GetGuestTrackComplaints input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Get Login Track Complaints
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<GetLoginTrackComplaintsResponse> GetLoginTrackComplaints(GetLoginTrackComplaints input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Get Contract Account Status
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<GetContractAccountStatusResponse> GetContractAccountStatus(GetContractAccountStatus input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// SetCRMLoginComplaints
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<SetCRMInteractionResponse> SetCRMInteraction(SetCRMInteraction input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Get SR Company Details
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<GetSRCompanyDetailsResponse> GetSRCompanyDetails(GetSRCompanyDetails input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// ManageBeneficiary
        /// Mangemode:        
        /// A-> Add(List to be passed)
        /// G->Get
        /// E->Edit(List to be passed)
        /// D->Delete(List to be passed) 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<manageBeneficiaryResponse> ManageBeneficiary(ManageBeneficiary input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// SetSurveyV3
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<surveyOutput> SetSurveyV3(SetSurveyV3 input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// GetSurveyV3Validation
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<surveyLinkOutput> GetSurveyV3Validation(string dynlink, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<estimateResponseParams> GetEstimateAmountDisplay(estimateRequestParams input, SupportedLanguage language, RequestSegment segment);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<newConnectionTaxDetails> GetNewConnectionTaxInvoicePDF(GetNewConnectionTaxInvoicePDF input, SupportedLanguage language, RequestSegment segment);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<paymentHistoryDetails> GetPaymentHistoryV1(GetPaymentHistory input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<GetWaterSaveNotificationResponse> GetWaterSaveNotification(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<clearanceMasterOutput> GetConservationFieldMaster(string Fieldname, string Service, string Scenerio, SupportedLanguage requestLanguage, RequestSegment requestSegment);
        /// <summary>
        ///  Method to get Profile Contract Account details
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>

        ServiceResponse<profileContractAccountOutput> GetProfileContractAccountDetails(profileContractAccountInput input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        ///  Method to Set or save the contract account details
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<string> SetContractAccountSave(contractAccountSaveInput input, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        ///  Method for Email Verification
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<string> SetProfileEmailConfirmation(profileEmailInput input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        ///  Method for Mobile number Verification
        /// </summary>
        /// <param name="input"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<string> SetProfileMobileConfirmation(profileMobileInput input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<otpRequestOutput> SetResendOTP(otpRequestInput otpRequest, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<ContractAccountImage> SetImageContractAccount(SetImageContractAccount input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<profileDetailsOutput> GetProfileDetails(string businessPartner, string credential, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<profileSaveOutput> SetCustomerProfileSave(profileSaveInput profileSaveInput, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<profileCustomerValidateOutput> GetCustomerProfileValidate(profileCustomerValidateInput profileCustomerDetails, string iv_type, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<slabTarrifOut> GetSlabCaps(GetSlabCaps input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<smartAlertSubrOut> GetSmartAlert(GetSmartAlert input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<smartAlertReadingOut> GetSmartConsumptionAlert(GetSmartConsumptionAlert input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<smartAlertSubwOut> SetSubscribeSmartAlert(SetSubscribeSmartAlert input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<slabPercentOut> GetSlabWisePercentage(GetSlabWisePercentage input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<SetNotificationSubmitResponse> SetNotificationSubmit(SetNotificationSubmit request, string _appflag, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<GetShiftHolidayListResponse> GetShiftHolidayList(GetShiftHolidayList request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        #region [power outage]
        /// <summary>
        /// SetOutageRequest
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<SetOutageRequestResponse> SetOutageRequest(SetOutageRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// GetOutageDropDetails
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<GetOutageDropDetailsXMLResponse> GetOutageDropDetails(GetOutageDropDetails request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// GetOutageTracker
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<GetOutageTrackerResponse> GetOutageTracker(GetOutageTracker request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        #endregion
    }
}
