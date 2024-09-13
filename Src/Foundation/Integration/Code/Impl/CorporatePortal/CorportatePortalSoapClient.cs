// <copyright file="CorportatePortalSoapClient.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.ConsultantSvc
{
    using CorporatePortal;
    using DEWAXP.Foundation.DI;
    using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
    using Enums;
    using Foundation.Logger;
    using Responses;
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    [Service(typeof(ICorportatePortalClient), Lifetime = Lifetime.Transient)]
    /// <summary>
    /// Defines the <see cref="CorportatePortalSoapClient" />
    /// </summary>
    public class CorportatePortalSoapClient : BaseDewaGateway, ICorportatePortalClient
    {
        /// <summary>
        /// The CreateProxy
        /// </summary>
        /// <returns>The <see cref="DCTM_SAPWS_ServiceClient"/></returns>
        private DCTM_SAPWS_ServiceClient CreateProxy()
        {
            DCTM_SAPWS_ServiceClient client = new DCTM_SAPWS_ServiceClient();
            var customBinding = new CustomBinding(client.Endpoint.Binding);
            var textEncodingElement = customBinding.Elements.OfType<TextMessageEncodingBindingElement>().Single();
            var customTextEncodingElement = new CustomASCIITextMessageEncodingBindingElement("ASCII", client.Endpoint.Binding.MessageVersion);
            customBinding.Elements.Remove(textEncodingElement);
            customBinding.Elements.Insert(customBinding.Elements.Count - 1, customTextEncodingElement);
            client.Endpoint.Binding = customBinding;
            return client;
        }

        /// <summary>
        /// The GetDashboardDetails
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="recordLimit">The recordLimit<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{taskDetailsExternalResponse}"/></returns>
        public ServiceResponse<taskDetailsExternalResponse> GetDashboardDetails(string coorName, string partnerName, string recordLimit, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                externalUserRequestEntity request = new externalUserRequestEntity()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = CPPORTAL_USERID,
                    password = CPPORTAL_PWD,
                    domain = string.Empty,
                    coordinatorName = coorName,
                    partnerName = partnerName,
                    parameter = recordLimit
                };

                try
                {
                    taskDetailsExternalResponse response = proxy.getAllTasks_externalUsers(request);
                    if (response != null)
                    {
                        return new ServiceResponse<taskDetailsExternalResponse>(response);
                    }
                    return new ServiceResponse<taskDetailsExternalResponse>(null, false, "");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<taskDetailsExternalResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }
        }

        /// <summary>
        /// The GetDocumentListByType
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="parameter">The parameter<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{getDocumentListEntity}"/></returns>
        public ServiceResponse<getDocumentListEntity> GetDocumentListByType(string coorName, string partnerName, string parameter, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                externalUserRequestEntity request = new externalUserRequestEntity()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = CPPORTAL_USERID,
                    password = CPPORTAL_PWD,
                    domain = string.Empty,
                    partnerName = partnerName,
                    coordinatorName = coorName,
                    parameter = parameter
                };

                try
                {
                    getDocumentListEntity response = proxy.GetDocumentListByType(request);
                    if (response != null)
                    {
                        return new ServiceResponse<getDocumentListEntity>(response);
                    }
                    return new ServiceResponse<getDocumentListEntity>(null, false, "");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<getDocumentListEntity>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The DownloadContent
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="objectid">The objectid<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{contentEntity}"/></returns>
        public ServiceResponse<contentEntity> DownloadContent(string userid, string pwd, string objectid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                downloadContentRequest request = new downloadContentRequest()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = CPPORTAL_USERID,
                    password = CPPORTAL_PWD,
                    Id = objectid
                };

                try
                {
                    contentEntity response = proxy.DownloadContent_v4(request);
                    if (response != null)
                    {
                        return new ServiceResponse<contentEntity>(response);
                    }
                    return new ServiceResponse<contentEntity>(null, false, "");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<contentEntity>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The GetOriginalContent
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="objectid">The objectid<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{contentEntity}"/></returns>
        public ServiceResponse<contentEntity> GetOriginalContent(string userid, string pwd, string objectid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                downloadContentRequest request = new downloadContentRequest()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = userid,
                    password = pwd,
                    Id = objectid
                };

                try
                {
                    contentEntity response = proxy.getLatestOriginalContent_v3_1(request);
                    if (response != null)
                    {
                        return new ServiceResponse<contentEntity>(response);
                    }
                    return new ServiceResponse<contentEntity>(null, false, "");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<contentEntity>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The getTaskDetails
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="objid">The objid<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{taskDetailsResponseV2}"/></returns>
        public ServiceResponse<taskDetailsResponseV2> getTaskDetails(string userid, string pwd, string objid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                taskDetailsRequest request = new taskDetailsRequest()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = userid,
                    password = pwd,
                    Id = objid
                };

                try
                {
                    taskDetailsResponseV2 response = proxy.getTaskDetailsV2(request);
                    if (response != null && response.NextTaskDetails != null)
                    {
                        return new ServiceResponse<taskDetailsResponseV2>(response);
                    }
                    //LogService.Error(ex, this);
                    return new ServiceResponse<taskDetailsResponseV2>(null, false, "Invalid number");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<taskDetailsResponseV2>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

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
        public ServiceResponse<bool> ForwardTaskWithTransitionSelect(string userid, string pwd, string workid, string comment, string transitionname, string performername, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                forwardTaskRequest request = new forwardTaskRequest()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = userid,
                    password = pwd,
                    workItemID = workid,
                    comment = comment,
                    nextActivity = new wiActivityEntity[]
                    {
                        new wiActivityEntity {
                        activityName = transitionname,
                        activityPerformerEntity = new activityPerformerEntity[]
                        {
                            new activityPerformerEntity
                            {
                                performer =new string [] { performername }
                            }
                        }
                    }
                    }
                };

                try
                {
                    bool response = proxy.ForwardTaskWithTransitionSelect_v3_1(request);
                    if (response)
                    {
                        return new ServiceResponse<bool>(response);
                    }
                    //LogService.Error(ex, this);
                    return new ServiceResponse<bool>(false, false, "Invalid number");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<bool>(false, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

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
        public ServiceResponse<returnTaskResult> ReturnTask(string userid, string pwd, string workid, string comment, string transitionname, string performername, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                rejectTaskRequest request = new rejectTaskRequest()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = userid,
                    password = pwd,
                    Id = workid,
                    comment = comment,
                    rejectTaskName = new string[] { transitionname },
                    performers = new string[] { performername }

                };

                try
                {
                    returnTaskResult response = proxy.ReturnTask_v3_1(request);
                    if (response != null && response.status.Equals("true"))
                    {
                        return new ServiceResponse<returnTaskResult>(response);
                    }
                    //LogService.Error(ex, this);
                    return new ServiceResponse<returnTaskResult>(null, false, "Invalid number");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<returnTaskResult>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The GetSentandPipelinetask
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="limitPipilineTask">The limitPipilineTask<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{sentTaskResponseCp}"/></returns>
        public ServiceResponse<sentTaskResponseCp> GetSentandPipelinetask(string userid, string pwd, string limitPipilineTask, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                loginInfoCommon request = new loginInfoCommon()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = userid,
                    password = pwd,
                    parameter = limitPipilineTask
                };

                try
                {
                    //var response = proxy.getSentAndPipeLineTasks(request);
                    sentTaskResponseCp response = proxy.getSentItems_cp(request);
                    if (response != null)
                    {
                        return new ServiceResponse<sentTaskResponseCp>(response);
                    }
                    return new ServiceResponse<sentTaskResponseCp>(null, false, "");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<sentTaskResponseCp>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }
        }

        /// <summary>
        /// The GetRelatedDocuments
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="objid">The objid<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{attachmentEntity[]}"/></returns>
        public ServiceResponse<attachmentEntity[]> GetRelatedDocuments(string userid, string pwd, string objid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                getRelatedDocumentsRequest request = new getRelatedDocumentsRequest()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = userid,
                    password = pwd,
                    RObjectId = objid
                };

                try
                {
                    attachmentEntity[] response = proxy.getRelatedDocuments(request);
                    if (response != null)
                    {
                        return new ServiceResponse<attachmentEntity[]>(response);
                    }
                    return new ServiceResponse<attachmentEntity[]>(null, false, "");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<attachmentEntity[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }
        }

        /// <summary>
        /// The GetUserDetailsexternal
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{externalUserEntity[]}"/></returns>
        public ServiceResponse<externalUserEntity[]> GetUserDetailsexternal(string coorName, string partnerName, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                externalUserRequestEntity request = new externalUserRequestEntity()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = CPPORTAL_USERID,
                    password = CPPORTAL_PWD,
                    domain = string.Empty,
                    coordinatorName = coorName,
                    //userLoginName = userid,
                    partnerName = partnerName
                };

                try
                {
                    externalUserEntity[] response = proxy.GetUserDetails_external(request);
                    if (response != null)
                    {
                        return new ServiceResponse<externalUserEntity[]>(response);
                    }
                    return new ServiceResponse<externalUserEntity[]>(null, false, "");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<externalUserEntity[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }
        }

        /// <summary>
        /// The LoginService
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/></param>
        /// <param name="pwd">The pwd<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{bool}"/></returns>
        public ServiceResponse<bool> LoginService(string userid, string pwd, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                loginInfoCommon request = new loginInfoCommon()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = userid,
                    password = pwd
                };

                try
                {
                    string response = proxy.loginService(request);
                    if (!string.IsNullOrWhiteSpace(response) && response.Equals("Login Success"))
                    {
                        return new ServiceResponse<bool>(true);
                    }
                    return new ServiceResponse<bool>(false, false, "");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<bool>(false, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }
        }

        /// <summary>
        /// The createFolder
        /// </summary>
        /// <param name="path">The path<see cref="string"/></param>
        /// <param name="language">The language<see cref="SupportedLanguage"/></param>
        /// <param name="segment">The segment<see cref="RequestSegment"/></param>
        /// <returns>The <see cref="ServiceResponse{bool}"/></returns>
        public ServiceResponse<bool> createFolder(string path, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                createFolderRequest request = new createFolderRequest()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = CPPORTAL_USERID,
                    password = CPPORTAL_PWD,
                    folderPath = path
                };

                try
                {
                    objectIDResponse response = proxy.createFolder(request);
                    if (response != null && response.status)
                    {
                        return new ServiceResponse<bool>(response.status);
                    }
                    return new ServiceResponse<bool>(false, false, "");
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<bool>(false, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        /// <summary>
        /// The createDocument
        /// </summary>
        /// <param name="documents">The documents<see cref="createDocumentRequest[]"/></param>
        /// <param name="folderpath">The folderpath<see cref="string"/></param>
        /// <returns>The <see cref="ServiceResponse{createDocumentsEntity}"/></returns>
        public ServiceResponse<createDocumentEntity[]> createDocument(createDocumentRequestEpass[] documents, string folderpath, string userID, string Password, string repository)
        {
            using (DCTM_SAPWS_ServiceClient proxy = CreateProxy())
            {
                documentsCreationRequestEpass request = new documentsCreationRequestEpass()
                {
                    repository = CPPORTAL_REPOSITORY,
                    userName = CPPORTAL_USERID,
                    password = CPPORTAL_PWD,
                    documents = documents,
                    folderPath = folderpath
                };

                try
                {
                    createDocumentEntity[] response = proxy.createDocuments_ePass(request);
                    if (response != null && response != null)
                    {
                        return new ServiceResponse<createDocumentEntity[]>(response);
                    }
                    return new ServiceResponse<createDocumentEntity[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<createDocumentEntity[]>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
    }
}
