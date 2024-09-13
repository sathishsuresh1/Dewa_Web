using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.JobSeekerSvc;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Integration
{
    public interface IJobSeekerServiceClient
    {
        ServiceResponse<userLoginValidation> GetValidateCandidateLogin(string userId, string password,bool rammas= false, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<academyRegistration> SetAcademyRegistration(string ApplicantName,
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
             byte[] FamilyBookAttachement,
             string FamilyBookAttachementfilename,
             string FamilyBookAttachementfiletype,
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
             string Unifiednumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);


        ServiceResponse<academyLogin> GetValidateAcademyLogin(string userId, string password, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);


        ServiceResponse<SetCareerFairRegistrationResponse> SetCareerFairRegistration(careerFairRegistrationDetails input, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        /// <summary>
        /// Requests that a password reset notification be sent to the specified user.
        /// Relates to SAP WS2
        /// </summary>
        /// <param name="userId">The requesting user's unique identifier</param>
        /// <param name="email">The email address to which the notification should be sent</param>
        /// <param name="language">Desired language of the response message</param>
        /// <param name="segment">The client segment issuing the request</param>
        ServiceResponse ResetPassword(string userId, string email, string sessionid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<profileUpdate> GetCandidateProfile(profileUpdateRequest updateRequest, string userId, string sessionid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse CandidateRegistration(string userid, string password, string pod, string experience, string natureOfwork, string firstName, string lastName, string emailAddress, string nationality, string privacy, string passport, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<searchJob> GetHotJobs(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<jobFilter> GetJobFilter(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<searchJobDetails> GetSearchJobs(string keyword, string hotjobs, string hierarchylevel, string contracttype, string logicalkeyword, string logicalkeywordbetween, string functionalarea, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<emailVerification> verifyRegistrationEmail(string param, string resendFlag, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<profileHelpValues> GetProfileHelpValues(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<eidDetails> GetEmiratesIdDetails(string userId, string emirateId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<workEXPUpdate> GetCandidateWorkExp(workEXPRequest workExpRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<educationResponse> GetCandidateEducation(educationRequest educationRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<attachmentsResponse> GetCandidateAttachements(attachmentsRequest attachmentsRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<workEXPUpdate> GetCandidateWorkExperience(workEXPRequest workExpRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<qualificationResponse> GetCandidateQualification(qualificationRequest qualificationRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<coverLetterUpdateResponse> GetCandidateCoverLetterUpdate(coverLetterUpdateRequest coverLetterRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<preferenceResponse> GetCandidatePreferenceUpdate(preferenceRequest preferenceRequest, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<podRegistration> PODCandidateRegistration(PutPODCandidateRegistration PODRegistrationRequest, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<jobApplicationStatus> GetCandidateJobApplicationStatus(string hrobjectId, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<profileRelaseResponse> GetCandidateProfileRelease(string updateMode, string profilereleased, string termsandconditions, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<tellAFriend> PutTellAFriend(PutTellAFriend request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<applyJob> PutCandidateJobApply(string jobId, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<myAppDashboardResponse> PutCandidateMyAppdashboard(myAppDashboardRequest request, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<postingDisplayResponse> GetCandidatePostingdisplay(string jobId, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<GetCandidateProfileDownloadResponse> GetCandidateProfileDownload(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<GetProfileCompletionStatusResponse> GetCandidateProfileStatus(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
    }
}
