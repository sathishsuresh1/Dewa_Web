// <copyright file="IEFormRESTServiceClient.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration
{
    using DEWAXP.Foundation.Integration.Responses;

    /// <summary>
    /// Defines the <see cref="IEFormRESTServiceClient" />
    /// </summary>
    public interface IEFormRESTServiceClient
    {
        /// <summary>
        /// The Query_Ework_DB
        /// </summary>
        /// <param name="SQL">The SQL<see cref="string"/></param>
        /// <returns>The <see cref="ServiceResponse{RestServiceResponse}"/></returns>
        ServiceResponse<WebApiRestResponseEpass> Query_Ework_DB(string SQL);

        /// <summary>
        /// The SubmitNewForm
        /// </summary>
        /// <param name="formXml">The formXml<see cref="string"/></param>
        /// <param name="mapName">The mapName<see cref="string"/></param>
        /// <param name="mapAction">The mapAction<see cref="string"/></param>
        /// <returns>The <see cref="ServiceResponse{RestServiceResponse}"/></returns>
        ServiceResponse<WebApiRestResponseEpass> SubmitNewForm(string formXml, string mapName, string mapAction);

        /// <summary>
        /// The UpdateForm
        /// </summary>
        /// <param name="eFolderID">The eFolderID<see cref="string"/></param>
        /// <param name="formXml">The formXml<see cref="string"/></param>
        /// <param name="mapName">The mapName<see cref="string"/></param>
        /// <param name="mapAction">The mapAction<see cref="string"/></param>
        /// <returns>The <see cref="ServiceResponse{RestServiceResponse}"/></returns>
        ServiceResponse<WebApiRestResponseEpass> UpdateForm(string eFolderID, string formXml, string mapName, string mapAction);

        ServiceResponse<WebApiRestResponseEpass> Query_Ework_DB(Requests.RestServiceRequest apiRequest);
    }
}
