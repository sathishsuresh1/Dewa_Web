// <copyright file="ListofEVCardsController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Controllers.Api
{
    using DEWAXP.Feature.EV.Models.EVDashboard;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Filters.Http;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Defines the <see cref="ListofEVCardsController" />.
    /// </summary>
    [TwoPhaseAuthorize]
    public class ListofEVCardsController : EVDashboardBaseController
    {
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/>.</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Card(string id, int page = 1, string keyword = "")
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(id.TrimStart(new char[] { '0' }), Times.Once));
                    CacheProvider.Store(CacheKeys.EV_DEACTIVATE_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(id.TrimStart(new char[] { '0' }), Times.Once));
                    if (CacheProvider.TryGet(CacheKeys.LISTOFCARDSERVICE_SELECTEDACCOUNT, out string accountnumber))
                    {
                        if (!string.IsNullOrWhiteSpace(accountnumber) && accountnumber.Equals(id))
                        {
                            if (CacheProvider.TryGet(CacheKeys.LISTOFCARDSERVICE_RESPONSE, out List<Evcarddetail> evcarddetails))
                            {
                                CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(id.TrimStart(new char[] { '0' }), Times.Once));
                                return Request.CreateResponse(HttpStatusCode.OK, EVCardDetailsViewModel.From(evcarddetails.Where(x => !string.IsNullOrWhiteSpace(x.activeflag) && x.activeflag.Equals("X")).ToList(), page, keyword));
                            }
                        }
                    }

                    List<Evcarddetail> listofCardsResponse = ListofActiveEVCards(id, out ErrorMessage);
                    if (listofCardsResponse != null && listofCardsResponse.Count > 0
                    && listofCardsResponse.Where(x => !string.IsNullOrWhiteSpace(x.activeflag) && x.activeflag.Equals("X")).Any())
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, EVCardDetailsViewModel.From(listofCardsResponse.Where(x => !string.IsNullOrWhiteSpace(x.activeflag) && x.activeflag.Equals("X")).ToList(), 1, keyword));
                    }
                    else
                    {
                        ErrorMessage = "No available cards";
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