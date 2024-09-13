using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using DEWAXP.Foundation.Integration.CustomerSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;

using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using DEWAXP.Foundation.Logger;
using System.Net;
using DEWAXP.Foundation.Integration.Responses.QmsSvc;
using DEWAXP.Foundation.Integration.QmsCustSvc;
using DEWAXP.Foundation.Integration.Requests.QmsSvc;
using Exception = System.Exception;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.QmsStatusSvc;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.Impl.QmsSvc
{
    [Service(typeof(IQmsServiceClient), Lifetime = Lifetime.Transient)]
    public class QmsSoapClient : BaseDewaGateway, IQmsServiceClient
    {
        private string Qms_WsAuth_user => $"{ApiBaseConfig.Qms_WsAuth_User}";
        private string Qms_WsAuth_Pwd => $"{ApiBaseConfig.Qms_WsAuth_Pwd}";
        public ServiceResponse<IssueTicketResponse> IssueTicket(IssueTicketReq request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            wsIssueTicketResponse response = new wsIssueTicketResponse(new wsIssueTicketResponseBody { });
            try
            {
                request.WsAuth = new DEWAXP.Foundation.Integration.Requests.QmsSvc.WsAuth
                {
                    WsClientName = Qms_WsAuth_user,
                    Password = Qms_WsAuth_Pwd
                };
                QmsCustClient client = new QmsCustClient();
                string wsIssueTicketReq = CustomXmlConvertor.Serialize(request);
                wsIssueTicketReq = wsIssueTicketReq.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("</xml>", "");
                //LogService.Info(new Exception("QMS webservice - Request\t" + wsIssueTicketReq));
                var returnData = client.wsIssueTicket(wsIssueTicketReq);
                //LogService.Info(new Exception("QMS webservice - IssueTicket Response\t" + returnData));
                response.Body.wsIssueTicketReturn = returnData;
                IssueTicketResponse _Response = CustomXmlConvertor.DeserializeString<IssueTicketResponse>(response.Body.wsIssueTicketReturn);
                if (_Response != null && _Response.Status.Equals("0"))
                {
                    return new ServiceResponse<IssueTicketResponse>(_Response);
                }
                else
                {
                    if (_Response != null && _Response.Status.Equals("-1"))
                    {
                        return new ServiceResponse<IssueTicketResponse>(_Response, false, _Response.Error.Reason);
                    }
                    else
                        return new ServiceResponse<IssueTicketResponse>(null, false, _Response.Error.Reason);
                }
            }
            catch (TimeoutException)
            {
                return new ServiceResponse<IssueTicketResponse>(null, false, "timeout error message");
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            return new ServiceResponse<IssueTicketResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }
        public ServiceResponse<ServiceStatusListResponse> GetServiceStatusList(ServiceStatusListReq request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            wsGetServiceStatusListResponse response = new wsGetServiceStatusListResponse(new wsGetServiceStatusListResponseBody { });
            try
            {
                request.WsAuth = new DEWAXP.Foundation.Integration.Requests.QmsSvc.WsAuth
                {
                    WsClientName = Qms_WsAuth_user,
                    Password = Qms_WsAuth_Pwd
                };

                QmsStatusClient client = new QmsStatusClient();
                String wsGetServiceStatusListReq = CustomXmlConvertor.Serialize(request);
                wsGetServiceStatusListReq = wsGetServiceStatusListReq.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("</xml>", "");
                var returnData = client.wsGetServiceStatusList(wsGetServiceStatusListReq);
                response.Body.wsGetServiceStatusListReturn = returnData;
                ServiceStatusListResponse _Response = CustomXmlConvertor.DeserializeString<ServiceStatusListResponse>(response.Body.wsGetServiceStatusListReturn);
                if (_Response != null && _Response.Status.Equals("0"))
                {
                    return new ServiceResponse<ServiceStatusListResponse>(_Response);
                }
                else
                {
                    return new ServiceResponse<ServiceStatusListResponse>(null, false, _Response.Error.Reason);
                }
            }
            catch (TimeoutException)
            {
                return new ServiceResponse<ServiceStatusListResponse>(null, false, "timeout error message");
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ServiceStatusListResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }
        public ServiceResponse<BranchServiceStatusResponse> GetBranchServiceStatusList(BranchServiceStatusReq request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            wsGetBranchServiceStatusResponse response = new wsGetBranchServiceStatusResponse(new wsGetBranchServiceStatusResponseBody { });
            try
            {
                request.WsAuth = new DEWAXP.Foundation.Integration.Requests.QmsSvc.WsAuth
                {
                    WsClientName = Qms_WsAuth_user,
                    Password = Qms_WsAuth_Pwd
                };

                QmsStatusClient client = new QmsStatusClient();
                String wsGetBranchServiceStatusReq = CustomXmlConvertor.Serialize(request);
                wsGetBranchServiceStatusReq = wsGetBranchServiceStatusReq.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("</xml>", "");
                var returnData = client.wsGetBranchServiceStatus(wsGetBranchServiceStatusReq);
                response.Body.wsGetBranchServiceStatusReturn = returnData;
                BranchServiceStatusResponse _Response = CustomXmlConvertor.DeserializeString<BranchServiceStatusResponse>(response.Body.wsGetBranchServiceStatusReturn);
                if (_Response != null && _Response.BranchStatusList != null && _Response.BranchStatusList.BranchStatus != null && _Response.BranchStatusList.BranchStatus.Count > 0)
                {
                    return new ServiceResponse<BranchServiceStatusResponse>(_Response);
                }
                else
                {
                    return new ServiceResponse<BranchServiceStatusResponse>(null, false, _Response.Error.Reason);
                }
            }
            catch (TimeoutException)
            {
                return new ServiceResponse<BranchServiceStatusResponse>(null, false, "timeout error message");
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<BranchServiceStatusResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

    }
}
