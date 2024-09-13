// <copyright file="ICorportatePortalClient.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration
{
    using DEWAXP.Foundation.Integration.CorporatePortal;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Responses;

    /// <summary>
    /// Defines the <see cref="ICorportatePortalClient" />
    /// </summary>
    public interface ICorportatePortalClient
    {
        /// <summary>
        /// The GetDashboardDetails
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="recordLimit">The recordLimit<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{taskDetailsExternalResponse}"/></returns>
        ServiceResponse<taskDetailsExternalResponse> GetDashboardDetails(string coorName, string partnerName, string recordLimit, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetDocumentListByType
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="parameter">The parameter<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{getDocumentListEntity}"/></returns>
        ServiceResponse<getDocumentListEntity> GetDocumentListByType(string coorName, string partnerName, string parameter, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The DownloadContent
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="objectid">The objectid<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{contentEntity}"/></returns>
        ServiceResponse<contentEntity> DownloadContent(string userid, string pwd, string objectid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetOriginalContent
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="objectid">The objectid<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{contentEntity}"/></returns>
        ServiceResponse<contentEntity> GetOriginalContent(string userid, string pwd, string objectid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The getTaskDetails
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="objid">The objid<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{taskDetailsResponseV2}"/></returns>
        ServiceResponse<taskDetailsResponseV2> getTaskDetails(string userid, string pwd, string objid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The ForwardTaskWithTransitionSelect
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="workid">The workid<see cref="string"/></param>
        /// <param name="comment">The comment<see cref="string"/></param>
        /// <param name="transitionname">The transitionname<see cref="string"/></param>
        /// <param name="performername">The performername<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{bool}"/></returns>
        ServiceResponse<bool> ForwardTaskWithTransitionSelect(string userid, string pwd, string workid, string comment, string transitionname, string performername, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The ReturnTask
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="workid">The workid<see cref="string"/></param>
        /// <param name="comment">The comment<see cref="string"/></param>
        /// <param name="transitionname">The transitionname<see cref="string"/></param>
        /// <param name="performername">The performername<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{returnTaskResult}"/></returns>
        ServiceResponse<returnTaskResult> ReturnTask(string userid, string pwd, string workid, string comment, string transitionname, string performername, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetSentandPipelinetask
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="limitPipilineTask">The limitPipilineTask<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{sentTaskResponseCp}"/></returns>
        ServiceResponse<sentTaskResponseCp> GetSentandPipelinetask(string userid, string pwd, string limitPipilineTask, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetUserDetailsexternal
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{externalUserEntity[]}"/></returns>
        ServiceResponse<externalUserEntity[]> GetUserDetailsexternal(string coorName, string partnerName, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The GetRelatedDocuments
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="objid">The objid<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{attachmentEntity[]}"/></returns>
        ServiceResponse<attachmentEntity[]> GetRelatedDocuments(string userid, string pwd, string objid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The LoginService
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{bool}"/></returns>
        ServiceResponse<bool> LoginService(string userid, string pwd, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The createFolder
        /// </summary>
        /// <param name="path">The path<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{bool}"/></returns>
        ServiceResponse<bool> createFolder(string path, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// The createDocument
        /// </summary>
        /// <param name="documents">The documents<see cref="createDocumentRequest[]"/></param>
        /// <param name="folderpath">The folderpath<see cref="string"/></param>
        /// <returns>The <see cref="ServiceResponse{createDocumentsEntity}"/></returns>
        ServiceResponse<createDocumentEntity[]> createDocument(createDocumentRequestEpass[] documents, string folderpath, string userID, string Password, string repository);
    }
}
