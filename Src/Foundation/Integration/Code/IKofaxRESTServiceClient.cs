// <copyright file="IKofaxRESTServiceClient.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\altaf.ahmed</author>

namespace DEWAXP.Foundation.Integration
{
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.KofaxRest;

    /// <summary>
    /// Defines the <see cref="IKofaxRESTServiceClient" />
    /// </summary>
    public interface IKofaxRESTServiceClient
    {

        ServiceResponse<KofaxRestResponse> SubmitKofax(string methodname, string modelJson);
        //ServiceResponse<KofaxRestResponse> UpdateApproval(string reqId, string status, string approverType, string currentUserEmail, string remarks = "");
        //ServiceResponse<KofaxRestResponse> GetMyPasses(string currentUserEmail);
        //ServiceResponse<KofaxRestResponse> GetDepartmentPendingPasses();
        //ServiceResponse<KofaxRestResponse> GetSecurityPendingPasses();
        //ServiceResponse<KofaxRestResponse> CreateSubPass(string modelJson);
    }
}
