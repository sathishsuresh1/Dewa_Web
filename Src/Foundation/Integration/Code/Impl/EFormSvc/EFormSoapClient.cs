using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using System;

namespace DEWAXP.Foundation.Integration.Impl.EFormSvc
{
    [Service(typeof(IEFormServiceClient), Lifetime = Lifetime.Transient)]
    public class EFormSoapClient : BaseDewaGateway, IEFormServiceClient
    {
        public ServiceResponse<string> SubmitNewForm(string formXml)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    string mapName = "Energy_Audit_Request";
                    string mapAction = "Submit";

                    var response = client.SubmitNewForm(formXml, mapName, mapAction);

                    return new ServiceResponse<string>(response);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<string> SubmitNewForm(string formXml, string mapName, string mapAction)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    //string mapName = "Energy_Audit_Request";
                    //string mapAction = "Submit";

                    var response = client.SubmitNewForm(formXml, mapName, mapAction);

                    return new ServiceResponse<string>(response);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<string> UpdateForm(string eFolderID, string formXml, string mapName, string mapAction)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    //string mapName = "Energy_Audit_Request";
                    //string mapAction = "Submit";

                    var response = client.UpdateForm(eFolderID, formXml, mapName, mapAction);

                    return new ServiceResponse<string>(response);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<System.Data.DataSet> Query_Ework_DB(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                using (var client = CreateProxy())
                {
                    try
                    {
                        var response = client.Query_Ework_DB(sql, CredDbUser, CredDbPassword);
                        if (response != null)
                            return new ServiceResponse<System.Data.DataSet>(response);
                        else
                            return new ServiceResponse<System.Data.DataSet>(null, false, $"response value: '{response}'");
                    }
                    catch (Exception ex)
                    {
                        LogService.Fatal(ex, this);
                        return new ServiceResponse<System.Data.DataSet>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                    }
                }
            }
            return new ServiceResponse<System.Data.DataSet>(null, false, $"sql value: '{sql}'");
        }

        #region Service Proxy methods
        private Integration.EFormSvc.eFormSoapClient CreateProxy()
        {
            var client = new Integration.EFormSvc.eFormSoapClient("eFormSoap", GetEndpointAddress("eFormSoap"));
            if (!string.IsNullOrEmpty(EserviceUserName) && !string.IsNullOrEmpty(EServicePassword))
            {
                client.ClientCredentials.UserName.UserName = EserviceUserName;
                client.ClientCredentials.UserName.Password = EServicePassword;

            }

            return client;
        }

        public ServiceResponse<System.Data.DataSet> Query_Ework_DB_BPM(string sql)
        {
            using (var client = CreateProxy(true))
            {
                try
                {

                    var response = client.Query_Ework_DB(sql, CredDbUserBPM, CredDbPasswordBPM);

                    return new ServiceResponse<System.Data.DataSet>(response);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<System.Data.DataSet>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse<string> SubmitNewFormBPM(string formXml, string mapName, string actionName, string formName, int mapVersion)
        {
            using (var client = CreateProxy(true))
            {
                try
                {

                    string SessionID = client.LoginInEForm("tp_usernoc", "tpyuio");

                    var response = client.SubmitNewForm(formXml, SessionID, mapName, actionName, formName, mapVersion);

                    //Update for Arabic. Check response contains 31 digit FolderID.
                    if (response.Length == 31)
                    {
                        client.Query_Ework_DB_Update_V2(formXml, mapName, response, CredDbUserBPM, CredDbPasswordBPM);
                    }

                    return new ServiceResponse<string>(response);

                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        //Proxy for OLD BPM System
        private EFormBPMSVC.EworkSoapClient CreateProxy(bool isLegacy)
        {
            var client = new EFormBPMSVC.EworkSoapClient("EworkSoap", GetEndpointAddress("EworkSoap"));
            if (!string.IsNullOrEmpty(EserviceUserName) && !string.IsNullOrEmpty(EServicePassword))
            {
                client.ClientCredentials.UserName.UserName = EserviceUserName;
                client.ClientCredentials.UserName.Password = EServicePassword;

            }

            return client;
        }
        #endregion
    }
}
