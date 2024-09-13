using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration
{
    public interface IProjectDocumentGenerationServiceClient
    {
        ServiceResponse<string> ConnectToDocbase(string username, string password,string docbase);
        ServiceResponse<string> DisconnectDocumentum(string DMS_SessionId, string username);
        ServiceResponse<string> CreateFolder(string DMS_SessionId, string username, string folderpath);

        ServiceResponse<string> UploadFileToDMS(string DMS_SessionId, string username,
            string DMS_Attributes, string folderpath, string Document_Type, string fileType, byte[] filebytesarray);

        ServiceResponse<PartnerDocumentSearch> SearchInDoc(string DMS_SessionId, string username, string documenttype, string where,
            string select);

        ServiceResponse<GetFileResponse> GetFileFromDMS(string DMS_SessionId, string username, string objectid);

        ServiceResponse<string> DeleteFile(string DMS_SessionId, string username,
            string objectid, string recyclebinpath);
    }
}
