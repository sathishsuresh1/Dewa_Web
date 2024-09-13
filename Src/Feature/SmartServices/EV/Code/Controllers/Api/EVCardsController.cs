// <copyright file="ListofEVCardsController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Controllers.Api
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Filters.Http;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Defines the <see cref="ListofEVCardsController" />.
    /// </summary>
    [TwoPhaseAuthorize]
    public class EVCardsController : EVDashboardBaseController
    {
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/>.</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Account(string id)
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    if (CacheProvider.TryGet(CacheKeys.LISTOFCARDSERVICE_SELECTEDACCOUNT, out string accountnumber))
                    {
                        if (!string.IsNullOrWhiteSpace(accountnumber) && accountnumber.Equals(id))
                        {
                            if (CacheProvider.TryGet(CacheKeys.LISTOFCARDSERVICE_RESPONSE, out List<Evcarddetail> evcarddetails))
                            {
                                CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(id.TrimStart(new char[] { '0' }), Times.Once));
                                return Request.CreateResponse(HttpStatusCode.OK, evcarddetails);
                            }
                        }
                    }
                    CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(id.TrimStart(new char[] { '0' }), Times.Once));
                    List<Evcarddetail> listofCardsResponse = ListofActiveEVCards(id, out ErrorMessage);
                    if (listofCardsResponse != null && listofCardsResponse.Count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, listofCardsResponse);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { Error = ErrorMessage });
                }
                catch (System.Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage);
            }))());
        }
    }
}