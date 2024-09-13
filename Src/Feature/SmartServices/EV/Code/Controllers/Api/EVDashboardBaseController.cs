// <copyright file="EVDashboardBaseController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Controllers.Api
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers.Api;
    using DEWAXP.Foundation.Content.Filters.Http;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using DEWAXP.Foundation.Logger;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="EVDashboardBaseController" />.
    /// </summary>
    [TwoPhaseAuthorize]
    public class EVDashboardBaseController : BaseApiController
    {
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <param name="ErrorMessage">The ErrorMessage<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/>.</returns>
        public List<Evcarddetail> ListofActiveEVCards(string id, out string ErrorMessage)
        {
            ErrorMessage = string.Empty;
            try
            {
                CacheProvider.Remove(CacheKeys.LISTOFCARDSERVICE_RESPONSE);
                DEWAXP.Foundation.Integration.Responses.ServiceResponse<List<Evcarddetail>> Response = EVDashboardClient.FetchActiveEVCards(
                        new FetchEVCardsRequest
                        {
                            contractaccount = id,
                            sessionid = CurrentPrincipal.SessionToken
                        }, RequestLanguage, Request.Segment());

                if (Response.Succeeded)
                {
                    if (Response.Payload != null)
                    {
                        CacheProvider.Store(CacheKeys.LISTOFCARDSERVICE_RESPONSE, new CacheItem<List<Evcarddetail>>(Response.Payload, TimeSpan.FromMinutes(20)));
                        CacheProvider.Store(CacheKeys.LISTOFCARDSERVICE_SELECTEDACCOUNT, new CacheItem<string>(id, TimeSpan.FromMinutes(20)));
                        return Response.Payload;
                    }
                    ErrorMessage = "No available cards";
                }
                else
                {
                    ErrorMessage = Response.Message;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return null;
        }
    }
}