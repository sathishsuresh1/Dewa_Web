using DEWAXP.Foundation.Integration.Helpers;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using DEWAXP.Foundation.Integration.Responses;
using Exception = System.Exception;
using System.Collections.Generic;
using DEWAXP.Foundation.Integration.JobSeekerSvc;
using System.ServiceModel.Dispatcher;
using Sitecore.Diagnostics;
using System.Web.Configuration;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.CorporatePortal;

namespace DEWAXP.Foundation.Integration.Impl.JobSeekerSvc
{
    [Service(typeof(IJobSeekerServiceClient), Lifetime = Lifetime.Transient)]
    public class JobSeekerSoapClient : BaseDewaGateway, IJobSeekerServiceClient
    {
        #region Methods

        public ServiceResponse<userLoginValidation> GetValidateCandidateLogin(string userId, string password,bool rammas=false, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetValidateCandidateLogin
                {
                    userid = userId,
                    password = password,
                    merchantid = WebConfigurationManager.AppSettings["Jobseeker_New_UserName"],
                    merchantpassword = WebConfigurationManager.AppSettings["Jobseeker_New_Password"],
                    vendorid = GetJobSeekerVendorId(segment),
                    lang = "EN",
                };
                if (rammas)
                {
                    request = new GetValidateCandidateLogin
                    {
                        userid = userId,
                        password = password,
                        merchantid = WebConfigurationManager.AppSettings["RammasMerchantId"],
                        merchantpassword = WebConfigurationManager.AppSettings["RammasMerchantPassword"],
                        vendorid = GetRammasVendorId(segment),
                        lang = "EN",
                    };
                }

                try
                {
                    var response = client.GetValidateCandidateLogin(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new userLoginValidation() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<userLoginValidation>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<userLoginValidation>(typedResponse);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<userLoginValidation>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<academyRegistration> SetAcademyRegistration(
            string ApplicantName,
            string ApplicantLastName,
            string ApplicantMiddleName,
            string Nationality,
            string PassportNumber,
            string EmiratesID,
            DateTime DateOfBirth,
            string Birthplace,
            byte[] PhotoAttachment,
            string PhotoAttachmentfilename,
            string PhotoAttachmentfiletype,
            byte[] PassportAttachment,
            string PassportAttachmentfilename,
            string PassportAttachmentfiletype,
            byte[] EmiratedIDAttachment,
            string EmiratedIDAttachmentfilename,
            string EmiratedIDAttachmentfiletype,
            byte[] FamilyBookAttachment,
            string FamilyBookAttachmentfilename,
            string FamilyBookAttachmentfiletype,
            byte[] BirthcertificateAttachment,
            string BirthcertificateAttachmentfilename,
            string BirthcertificateAttachmentfiletype,
            string emailuserid,
            string MobileNumber,
            string Address,
            string Landmark,
            string City,
            string numberofsiblings,
            string SchoolName,
            string FinalPercentage,
            string AcademicYear,
            byte[] AcademicCertificate,
            string AcademicCertificatefilename,
            string AcademicCertificatefiletype,
            byte[] CoursesandTrainingCertificates,
            string CoursesandTrainingCertificatesfilename,
            string CoursesandTrainingCertificatesfiletype,
            byte[] CertificateIssuedBoard,
            string CertificateIssuedBoardfilename,
            string CertificateIssuedBoardfiletype,
            string Password,
            DateTime Familybookissueddate,
            string Familybooknumber,
            string Familynumber,
            string Fatherfirstname,
            string Fatherlastname,
            string Fathermiddlename,
            string Fathermobilenumber,
            string Motherfirstname,
            string Motherlastname,
            string Mothermiddlename,
            string Mothermobilenumber,
            string Postalcode,
            DateTime EmiratesIDexpiredate,
            DateTime EmiratesIDissueddate,
            string Idbaranumber,
            DateTime Passportexpirydate,
            DateTime Passportissueddate,
            string Placeofidentificationissued,
            string Secondarymobilenumber,
            string Telephonenumber,
            string Unifiednumber,
            SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var academyRegistrationDetails = new academyRegistrationDetails
                {
                    //--Personal
                    applicantFirstName = ApplicantName,
                    applicantLastName = ApplicantLastName,
                    applicantMiddleName = ApplicantMiddleName,
                    nationality = Nationality,
                    passportNumber = PassportNumber,
                    emiratesID = EmiratesID,
                    dateOfBirth = DateOfBirth.ToString("yyyyMMdd"),
                    birthplace = Birthplace,
                    //--Contact
                    userid = emailuserid,
                    mobileNumber = MobileNumber,
                    //address
                    houseNumber = Address,
                    landmark = Landmark,
                    city = City,
                    siblings = numberofsiblings,
                    //--Academic
                    schoolName = SchoolName,
                    finalPercentage = FinalPercentage,
                    password = HexadecimalEncoding.ToHexString(Password),
                    channel = "web",
                    emiratesIDexpiredate = EmiratesIDexpiredate.ToString("yyyyMMdd"),
                    emiratesIDissueddate = EmiratesIDissueddate.ToString("yyyyMMdd"),
                    idbaranumber = Idbaranumber,
                    passportexpirydate = Passportexpirydate.ToString("yyyyMMdd"),
                    passportissueddate = Passportissueddate.ToString("yyyyMMdd"),
                    placeofidentificationissued = Placeofidentificationissued,
                    secondarymobilenumber = Secondarymobilenumber,
                    telephonenumber = Telephonenumber,
                    unifiednumber = Unifiednumber,
                    lang = language.Code()

                };
                var academyCeritifcates = new academyCeritifcates
                {
                    academicYear = AcademicYear,
                    academicCertificate = AcademicCertificate,
                    academicCertificatefilename = AcademicCertificatefilename,
                    academicCertificatefiletype = AcademicCertificatefiletype,
                    coursesandTrainingCertificates = CoursesandTrainingCertificates,
                    coursesandTrainingCertificatesfilename = CoursesandTrainingCertificatesfilename,
                    coursesandTrainingCertificatesfiletype = CoursesandTrainingCertificatesfiletype,
                    certificateIssuedBoard = CertificateIssuedBoard,
                    certificateIssuedBoardfilename = CertificateIssuedBoardfilename,
                    certificateIssuedBoardfiletype = CertificateIssuedBoardfiletype
                };
                var academyPersonalAttachments = new academyPersonalAttachments
                {
                    photoAttachment = PhotoAttachment,
                    photoAttachmentfilename = PhotoAttachmentfilename,
                    photoAttachmentfiletype = PhotoAttachmentfiletype,
                    passportAttachment = PassportAttachment,
                    passportAttachmentfilename = PassportAttachmentfilename,
                    passportAttachmentfiletype = PassportAttachmentfiletype,
                    emiratedIDAttachment = EmiratedIDAttachment,
                    emiratedIDAttachmentfilename = EmiratedIDAttachmentfilename,
                    emiratedIDAttachmentfiletype = EmiratedIDAttachmentfiletype,
                    familyBookAttachment = FamilyBookAttachment,
                    familyBookAttachmentfilename = FamilyBookAttachmentfilename,
                    familyBookAttachmentfiletype = FamilyBookAttachmentfiletype,
                    birthcertificateAttachment = BirthcertificateAttachment,
                    birthcertificateAttachmentfilename = BirthcertificateAttachmentfilename,
                    birthcertificateAttachmentfiletype = BirthcertificateAttachmentfiletype,
                };
                var familyDetails = new familyDetails
                {
                    familybookissueddate = Familybookissueddate.ToString("yyyyMMdd"),
                    familybooknumber = Familybooknumber,
                    familynumber = Familynumber,
                    fatherfirstname = Fatherfirstname,
                    fatherlastname = Fatherlastname,
                    fathermiddlename = Fathermiddlename,
                    fathermobilenumber = Fathermobilenumber,
                    motherfirstname = Motherfirstname,
                    motherlastname = Motherlastname,
                    mothermiddlename = Mothermiddlename,
                    mothermobilenumber = Mothermobilenumber,
                    postalcode = Postalcode
                };

                var request = new SetAcademyRegistration
                {
                    AcademyRegistrationDetails = academyRegistrationDetails,
                    AcademyCeritifcates = academyCeritifcates,
                    AcademyPersonalAttachments = academyPersonalAttachments,
                    FamilyDetails = familyDetails
                };

                try
                {
                    var response = client.SetAcademyRegistration(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new academyRegistration() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<academyRegistration>(failedresponse, false, typedResponse.errormessage);
                    }
                    return new ServiceResponse<academyRegistration>(typedResponse);
                    //return new ServiceResponse<academyRegistration>(typedResponse);
                }
                catch (System.Exception ex)
                {
                    Log.Info(ex.Message + "---" + ex.InnerException.ToString(), this);
                    return new ServiceResponse<academyRegistration>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<academyLogin> GetValidateAcademyLogin(string userId, string password, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetValidateAcademyLogin
                {
                    //--Personal

                    userid = userId,

                    password = HexadecimalEncoding.ToHexString(password),
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetValidateAcademyLogin(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new academyLogin() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<academyLogin>(failedresponse, false, typedResponse.errormessage);
                    }
                    return new ServiceResponse<academyLogin>(typedResponse);

                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<academyLogin>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<SetCareerFairRegistrationResponse> SetCareerFairRegistration(careerFairRegistrationDetails input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            ServiceResponse<SetCareerFairRegistrationResponse> returnResponseData = null;
            using (var client = CreateProxy())
            {
                //default value
                input.lang = language.Code();

                // api request mapping
                var request = new SetCareerFairRegistration
                {
                    Caree = input,
                };
                try
                {
                    SetCareerFairRegistrationResponse response = client.SetCareerFairRegistration(request);
                    returnResponseData = new ServiceResponse<SetCareerFairRegistrationResponse>(response);
                    if (response.@return.errorcode != "0")
                    {
                        LogService.Debug(response.@return.errormessage);
                        returnResponseData = new ServiceResponse<SetCareerFairRegistrationResponse>(response, false, response.@return.errormessage);
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    returnResponseData = new ServiceResponse<SetCareerFairRegistrationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
            return returnResponseData;

        }
        public ServiceResponse ResetPassword(string userId, string email, string sessionid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new SetForgotPassword()
                {
                    userid = userId,
                    emailid = email,
                    vendorid = GetJobSeekerVendorId(segment),
                    lang = language.Code()
                };

                try
                {
                    var response = client.SetForgotPassword(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new userLoginValidation() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<userLoginValidation>(failedresponse, false, typedResponse.errormessage);
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
        public ServiceResponse<profileUpdate> GetCandidateProfile(profileUpdateRequest updateRequest, string userId, string sessionid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                updateRequest.userid = userId;
                updateRequest.vendorid = GetJobSeekerVendorId(segment);
                updateRequest.sessionid = sessionid;
                updateRequest.lang = language.Code();

                var request = new PutCandidateProfileUpdate
                {
                    Personalupdate = updateRequest
                };

                try
                {
                    var response = client.PutCandidateProfileUpdate(request);
                    var typedResponse = response.@return;
                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new profileUpdate() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<profileUpdate>(failedresponse, false, typedResponse.errormessage);
                    }
                    return new ServiceResponse<profileUpdate>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<profileUpdate>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse CandidateRegistration(string userid, string password, string pod, string experience, string natureOfwork, string firstName, string lastName, string emailAddress, string nationality, string privacy, string passport, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {

            using (var client = CreateProxy())
            {
                var request = new PutCandidateRegistration
                {
                    userid = userid,
                    password = HexadecimalEncoding.ToHexString(password),
                    vendorid = GetJobSeekerVendorId(segment),
                    firstname = firstName,
                    lastname = lastName,
                    emailaddress = emailAddress,
                    nationality = nationality,
                    privacy = (privacy != null && privacy.ToString().ToLower() == "true") ? "X" : "",
                    passportnumber = passport,
                    experience = experience,
                    natureofwork = natureOfwork,
                    peopleofdetermination = (pod != null && pod.ToString().ToLower() == "true") ? "X" : "",
                    //regiontype = regionType,
                    lang = language.Code()
                };

                try
                {
                    var response = client.PutCandidateRegistration(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0" && typedResponse.errorcode != "000")
                    {
                        var failedresponse = new userRegistration() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<userRegistration>(failedresponse, false, typedResponse.errormessage);
                    }
                    return new ServiceResponse<userRegistration>(typedResponse);
                }
                catch (Exception ex)
                {

                    LogService.Fatal(ex, this);
                    return new ServiceResponse<userRegistration>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }


            }
        }
        public ServiceResponse<searchJob> GetHotJobs(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetHotJobs
                {
                    vendorid = GetJobSeekerVendorId(segment),
                    lang = language.Code()
                };
                try
                {
                    var response = client.GetHotJobs(request);
                    var typedResponse = response.@return;
                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new searchJob() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<searchJob>(failedresponse, false, typedResponse.errormessage);
                    }

                    //return new ServiceResponse<List<jobList>>(typedResponse.joblist.ToList());
                    return new ServiceResponse<searchJob>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<searchJob>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }

        }
        public ServiceResponse<jobFilter> GetJobFilter(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetJobFilter
                {
                    vendorid = GetJobSeekerVendorId(segment),
                    lang = language.Code()
                };
                try
                {
                    var response = client.GetJobFilter(request);
                    var typedResponse = response.@return;
                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new jobFilter() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<jobFilter>(failedresponse, false, typedResponse.errormessage);
                    }

                    //return new ServiceResponse<List<jobList>>(typedResponse.joblist.ToList());
                    return new ServiceResponse<jobFilter>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<jobFilter>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }

        }
        public ServiceResponse<searchJobDetails> GetSearchJobs(string keyword, string hotjobs, string hierarchylevel, string contracttype, string logicalkeyword, string logicalkeywordbetween, string functionalarea, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new PutCandidateJobSearch
                {
                    freetext = keyword,
                    hotjobs = "",
                    hierarchylevel = hierarchylevel,
                    contracttype = "",
                    logicalkeyword = "",
                    logicalkeywordbetween = "",
                    //jobid = jobId,
                    //keyword = keyword,
                    functionalarea = functionalarea,
                    vendorid = GetJobSeekerVendorId(segment),
                    lang = language.Code()
                };
                try
                {
                    var response = client.PutCandidateJobSearch(request);
                    var typedResponse = response.@return;
                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new searchJobDetails() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<searchJobDetails>(failedresponse, false, typedResponse.errormessage);
                    }

                    //return new ServiceResponse<List<jobList>>(typedResponse.joblist.ToList());
                    return new ServiceResponse<searchJobDetails>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<searchJobDetails>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<emailVerification> verifyRegistrationEmail(string param, string resendFlag, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    var request = new GetEmailVerification
                    {
                        param = param,
                        resendflag = resendFlag,
                        vendorid = GetJobSeekerVendorId(segment),
                        lang = language.Code()
                    };
                    var response = client.GetEmailVerification(request);
                    var typedResponse = response.@return;
                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new emailVerification() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<emailVerification>(failedresponse, true, typedResponse.errormessage);
                    }

                    //return new ServiceResponse<List<jobList>>(typedResponse.joblist.ToList());
                    return new ServiceResponse<emailVerification>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<emailVerification>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<profileHelpValues> GetProfileHelpValues(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetProfileHelpValues
                {
                    vendorid = GetJobSeekerVendorId(segment),
                    lang = language.Code()
                };
                try
                {
                    var response = client.GetProfileHelpValues(request);
                    var typedResponse = response.@return;
                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new profileHelpValues() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<profileHelpValues>(failedresponse, false, typedResponse.errormessage);
                    }

                    //return new ServiceResponse<List<jobList>>(typedResponse.joblist.ToList());
                    return new ServiceResponse<profileHelpValues>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<profileHelpValues>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<eidDetails> GetEmiratesIdDetails(string userId, string emirateId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetEmiratesIDDetails()
                {
                    userid = userId,
                    idnumber = emirateId,
                    sessionid = sessionId,
                    idtype = "01",
                    vendorid = GetJobSeekerVendorId(segment),
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetEmiratesIDDetails(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new eidDetails() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<eidDetails>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<eidDetails>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<eidDetails>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<workEXPUpdate> GetCandidateWorkExp(workEXPRequest workExpRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                workExpRequest.userid = userId;
                workExpRequest.vendorid = GetJobSeekerVendorId(segment);
                workExpRequest.sessionid = sessionId;
                workExpRequest.lang = language.Code();

                var request = new PutCandidateWorkExpUpdate()
                {
                    Workexperience = workExpRequest
                };

                try
                {
                    var response = client.PutCandidateWorkExpUpdate(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new workEXPUpdate() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<workEXPUpdate>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<workEXPUpdate>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<workEXPUpdate>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<educationResponse> GetCandidateEducation(educationRequest educationRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                educationRequest.userid = userId;
                educationRequest.vendorid = GetJobSeekerVendorId(segment);
                educationRequest.sessionid = sessionId;
                educationRequest.lang = language.Code();

                var request = new PutCandidateEducationUpdate()
                {
                    educationdetails = educationRequest
                };

                try
                {
                    var response = client.PutCandidateEducationUpdate(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new educationResponse() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<educationResponse>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<educationResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<educationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<attachmentsResponse> GetCandidateAttachements(attachmentsRequest attachmentsRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                attachmentsRequest.userid = userId;
                attachmentsRequest.vendorid = GetJobSeekerVendorId(segment);
                attachmentsRequest.sessionid = sessionId;
                attachmentsRequest.lang = language.Code();

                var request = new PutCandidateAttachmentsUpdate()
                {
                    attachmentdetails = attachmentsRequest
                };

                try
                {
                    var response = client.PutCandidateAttachmentsUpdate(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new attachmentsResponse() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<attachmentsResponse>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<attachmentsResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<attachmentsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<workEXPUpdate> GetCandidateWorkExperience(workEXPRequest workExpRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                workExpRequest.userid = userId;
                workExpRequest.vendorid = GetJobSeekerVendorId(segment);
                workExpRequest.sessionid = sessionId;
                workExpRequest.lang = language.Code();

                var request = new PutCandidateWorkExpUpdate()
                {
                    Workexperience = workExpRequest
                };

                try
                {
                    var response = client.PutCandidateWorkExpUpdate(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new workEXPUpdate() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<workEXPUpdate>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<workEXPUpdate>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<workEXPUpdate>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<qualificationResponse> GetCandidateQualification(qualificationRequest qualificationRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                qualificationRequest.userid = userId;
                qualificationRequest.vendorid = GetJobSeekerVendorId(segment);
                qualificationRequest.sessionid = sessionId;
                qualificationRequest.lang = language.Code();

                var request = new PutCandidateQualificationUpdate()
                {
                    qualificationdetails = qualificationRequest
                };

                try
                {
                    var response = client.PutCandidateQualificationUpdate(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new qualificationResponse() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<qualificationResponse>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<qualificationResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<qualificationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<coverLetterUpdateResponse> GetCandidateCoverLetterUpdate(coverLetterUpdateRequest coverLetterRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                coverLetterRequest.userid = userId;
                coverLetterRequest.vendorid = GetJobSeekerVendorId(segment);
                coverLetterRequest.sessionid = sessionId;
                coverLetterRequest.lang = language.Code();

                var request = new PutCandidateCoverLetterUpdate()
                {
                    CoverLetterRequest = coverLetterRequest
                };

                try
                {
                    var response = client.PutCandidateCoverLetterUpdate(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new coverLetterUpdateResponse() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<coverLetterUpdateResponse>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<coverLetterUpdateResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<coverLetterUpdateResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<preferenceResponse> GetCandidatePreferenceUpdate(preferenceRequest preferenceRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                preferenceRequest.userid = userId;
                preferenceRequest.vendorid = GetJobSeekerVendorId(segment);
                preferenceRequest.sessionid = sessionId;
                preferenceRequest.lang = language.Code();

                var request = new PutCandidatePreferenceUpdate()
                {
                    preferenceupdate = preferenceRequest,
                };

                try
                {
                    var response = client.PutCandidatePreferenceUpdate(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new preferenceResponse() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<preferenceResponse>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<preferenceResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<preferenceResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<podRegistration> PODCandidateRegistration(PutPODCandidateRegistration PODRegistrationRequest, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {

                using (var client = CreateProxy())
                {

                    PODRegistrationRequest.vendorid = GetJobSeekerVendorId(segment);
                    PODRegistrationRequest.lang = language.Code();
                    var response = client.PutPODCandidateRegistration(PODRegistrationRequest);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {

                        var failedresponse = new podRegistration() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<podRegistration>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<podRegistration>(typedResponse);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<podRegistration>(new podRegistration(), false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        public ServiceResponse<jobApplicationStatus> GetCandidateJobApplicationStatus(string hrobjectId, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCandidateJobApplicationStatus
                {
                    userid = userId,
                    sessionid = sessionId,
                    vendorid = GetJobSeekerVendorId(segment),
                    hrobjectid = hrobjectId
                };

                try
                {
                    var response = client.GetCandidateJobApplicationStatus(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new jobApplicationStatus() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<jobApplicationStatus>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<jobApplicationStatus>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<jobApplicationStatus>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<profileRelaseResponse> GetCandidateProfileRelease(string updateMode, string profilereleased, string termsandconditions, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {

                var request = new GetCandidateProfileRelease
                {
                    userid = userId,
                    vendorid = GetJobSeekerVendorId(segment),
                    sessionid = sessionId,
                    lang = language.Code(),
                    updatemode = updateMode,
                    profilereleased = profilereleased,
                    termsandconditions = termsandconditions
                };

                try
                {
                    var response = client.GetCandidateProfileRelease(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new profileRelaseResponse() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<profileRelaseResponse>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<profileRelaseResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<profileRelaseResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<tellAFriend> PutTellAFriend(PutTellAFriend request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.userid = userId;
                    request.vendorid = GetJobSeekerVendorId(segment);
                    request.sessionid = sessionId;
                    request.lang = language.Code();

                    var response = client.PutTellAFriend(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {

                        var failedresponse = new tellAFriend() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<tellAFriend>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<tellAFriend>(typedResponse);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<tellAFriend>(new tellAFriend(), false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        public ServiceResponse<applyJob> PutCandidateJobApply(string jobId, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new PutCandidateJobApply()
                {
                    userid = userId,
                    jobid = jobId,
                    sessionid = sessionId,
                    vendorid = GetJobSeekerVendorId(segment),
                    lang = language.Code()
                };

                try
                {
                    var response = client.PutCandidateJobApply(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new applyJob() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<applyJob>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<applyJob>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<applyJob>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<myAppDashboardResponse> PutCandidateMyAppdashboard(myAppDashboardRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.userid = userId;
                    request.vendorid = GetJobSeekerVendorId(segment);
                    request.sessionid = sessionId;
                    request.lang = language.Code();
                    var putRequest = new PutCandidateMyAppdashboard
                    {
                        applicationdetails = request,
                    };
                    var response = client.PutCandidateMyAppdashboard(putRequest);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new myAppDashboardResponse() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<myAppDashboardResponse>(failedresponse, false, typedResponse.errormessage);
                    }
                    return new ServiceResponse<myAppDashboardResponse>(typedResponse);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<myAppDashboardResponse>(new myAppDashboardResponse(), false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        public ServiceResponse<postingDisplayResponse> GetCandidatePostingdisplay(string jobId, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCandidatePostingdisplay()
                {
                    userid = userId,
                    hrobjectid = jobId,
                    sessionid = sessionId,
                    vendorid = GetJobSeekerVendorId(segment),
                };

                try
                {
                    var response = client.GetCandidatePostingdisplay(request);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new postingDisplayResponse() { errorcode = typedResponse.errorcode };
                        return new ServiceResponse<postingDisplayResponse>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<postingDisplayResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<postingDisplayResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetCandidateProfileDownloadResponse> GetCandidateProfileDownload(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var profiledownrequest = new GetCandidateProfileDownload()
                    {
                        lang = language.Code(),
                        sessionid = sessionId,
                        userid = userId,
                        vendorid = GetJobSeekerVendorId(segment)//GetVendorId(segment)
                    };

                    var response = client.GetCandidateProfileDownload(profiledownrequest);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new GetCandidateProfileDownloadResponse() {
                            @return=new profileDownload() {
                                errorcode = typedResponse.errorcode,
                                errormessage = typedResponse.errormessage
                            }
                        };

                        return new ServiceResponse<GetCandidateProfileDownloadResponse>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<GetCandidateProfileDownloadResponse>(response);
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetCandidateProfileDownloadResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetCandidateProfileDownloadResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                //return new ServiceResponse<GetCandidateProfileDownloadResponse>(null, false, ex.Message);
            }
        }
        public ServiceResponse<GetProfileCompletionStatusResponse> GetCandidateProfileStatus(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    var profilestatusequest = new GetProfileCompletionStatus()
                    {
                        lang = language.Code(),
                        sessionid = sessionId,
                        userid = userId,
                        vendorid = GetJobSeekerVendorId(segment)
                    };

                    var response = client.GetProfileCompletionStatus(profilestatusequest);
                    var typedResponse = response.@return;

                    if (typedResponse.errorcode != "0")
                    {
                        var failedresponse = new GetProfileCompletionStatusResponse()
                        {
                            @return = new profileStatusCompletion()
                            {
                                errorcode = typedResponse.errorcode,
                                errormessage = typedResponse.errormessage
                            }
                        };

                        return new ServiceResponse<GetProfileCompletionStatusResponse>(failedresponse, false, typedResponse.errormessage);
                    }

                    return new ServiceResponse<GetProfileCompletionStatusResponse>(response);
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetProfileCompletionStatusResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetProfileCompletionStatusResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                //return new ServiceResponse<GetCandidateProfileDownloadResponse>(null, false, ex.Message);
            }
        }
        #endregion

        #region Proxy configuration methods

        private ErecV1Client CreateProxy()
        {
            var client = new ErecV1Client(CreateBinding(), GetEndpointAddress("ErecServices"));
            client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new DewaApiCredentials());
            client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["RestAPI_Client_Id"], "Bearer " + OAuthToken.GetAccessToken()));
            //client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["Jobseeker_apiKey"], ""));
            client.ClientCredentials.UserName.UserName = WebConfigurationManager.AppSettings["Jobseeker_UserName"];
            client.ClientCredentials.UserName.Password = WebConfigurationManager.AppSettings["Jobseeker_Password"];

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
            var transport = new HttpsTransportBindingElement();
            transport.MaxReceivedMessageSize = 20000000; // 20 megs

            binding.Elements.Add(security);
            binding.Elements.Add(new CustomTextMessageBindingElement());
            binding.Elements.Add(transport);


            return binding;
        }




        //private EndpointAddress GetEndpointAddress()
        //{
        //    var clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;
        //    string address = string.Empty;
        //    for (int i = 0; i < clientSection.Endpoints.Count; i++)
        //    {
        //        if (clientSection.Endpoints[i].Name == "ErecServices")
        //            address = clientSection.Endpoints[i].Address.ToString();
        //    }
        //    return new EndpointAddress(address);
        //}

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
                clientRuntime.ClientMessageInspectors.Add(new CustomMessageInspector(this.sapApiKey, this.token));
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
