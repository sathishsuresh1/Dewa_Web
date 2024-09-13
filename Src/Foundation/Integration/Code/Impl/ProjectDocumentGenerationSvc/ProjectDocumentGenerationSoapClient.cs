using System;
using System.Net;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Foundation.Integration.Impl.ProjectDocumentGenerationSvc
{
    [Service(typeof(IProjectDocumentGenerationServiceClient), Lifetime = Lifetime.Transient)]
    public class ProjectDocumentGenerationSoapClient : BaseDewaGateway, IProjectDocumentGenerationServiceClient
    {
        public ServiceResponse<string> ConnectToDocbase(string username, string password, string docbase)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.ConnectToDocbase(username, password, docbase);

                    return new ServiceResponse<string>(response);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<string> DisconnectDocumentum(string DMS_SessionId, string username)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.DissconnectDocumentum(DMS_SessionId,username);

                    return new ServiceResponse<string>(response);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<string> CreateFolder(string DMS_SessionId, string username,string folderpath)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.Create_Folder(DMS_SessionId, username, folderpath);

                    return new ServiceResponse<string>(response);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<string> UploadFileToDMS(string DMS_SessionId, string username, 
            string DMS_Attributes,string folderpath,string Document_Type,string fileType,byte[] filebytesarray)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.UploadFileToDMS_V1(DMS_SessionId, username, 
                        DMS_Attributes,folderpath,Document_Type,fileType,filebytesarray);

                    return new ServiceResponse<string>(response);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<PartnerDocumentSearch> SearchInDoc(string DMS_SessionId, string username, string documenttype, string where, string select)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.SearchInDoc(DMS_SessionId, username, documenttype,where,select);
                    var typedResponse = response.DeserializeAs<PartnerDocumentSearch>();
                    return new ServiceResponse<PartnerDocumentSearch>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<PartnerDocumentSearch>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<GetFileResponse> GetFileFromDMS(string DMS_SessionId, string username, string objectid)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.GetFileFromDMS(DMS_SessionId, username, objectid);
                    if (response != null)
                    {
                        GetFileResponse myGetFileResponse = new GetFileResponse
                        {
                            filebytes = response.File_Bytes,
                            FileName = response.File_Name
                        };
                        return new ServiceResponse<GetFileResponse>(myGetFileResponse);
                    }
                    return new ServiceResponse<GetFileResponse>(null, false, string.Empty);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetFileResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<string> DeleteFile(string DMS_SessionId, string username, 
            string objectid, string recyclebinpath)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.Move_File(DMS_SessionId, username, objectid, recyclebinpath);

                    return new ServiceResponse<string>(response, false, string.Empty);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        #region Service Proxy methods
        private Integration.ProjectDocumentGenerationSvc.DMS_LiveSoapClient CreateProxy()
        {
            var client = new Integration.ProjectDocumentGenerationSvc.DMS_LiveSoapClient("DMS_Live", GetEndpointAddress("DMS_Live"));
            if (client.ClientCredentials != null)
            {
                client.ClientCredentials.UserName.UserName = DMS_Username;
                client.ClientCredentials.UserName.Password = DMS_Password;
            }
            
            return client;
        }
        #endregion
    }
}
