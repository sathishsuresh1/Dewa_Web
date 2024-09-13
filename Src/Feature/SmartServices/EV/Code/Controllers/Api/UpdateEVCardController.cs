// <copyright file="UpdateEVCardController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Controllers.Api
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Filters.Http;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using DEWAXP.Foundation.Logger;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Defines the <see cref="UpdateEVCardController" />.
    /// </summary>
    [TwoPhaseAuthorize]
    public class UpdateEVCardController : EVDashboardBaseController
    {
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="accountnumber">The accountnumber<see cref="string"/>.</param>
        /// <param name="cardid">The cardid<see cref="string"/>.</param>
        /// <param name="name">The name<see cref="string"/>.</param>
        /// <param name="bookmark">The bookmark<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/>.</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Card(string accountnumber, string cardid, string name, bool bookmark = false)
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    if (CacheProvider.TryGet(CacheKeys.LISTOFCARDSERVICE_RESPONSE, out List<Evcarddetail> evcarddetails) && !string.IsNullOrWhiteSpace(accountnumber) && !string.IsNullOrWhiteSpace(cardid))
                    {
                        IEnumerable<Evcarddetail> carddetail = evcarddetails.Where(x => !string.IsNullOrWhiteSpace(x.cardnumber) && x.cardnumber.ToLower().Equals(cardid.ToLower()));
                        if (carddetail.HasAny())
                        {
                            Evcarddetail updatedcarddetail = carddetail.FirstOrDefault();
                            if (bookmark)
                            {
                                if (!string.IsNullOrWhiteSpace(updatedcarddetail.bookmarkflag) && updatedcarddetail.bookmarkflag.Equals("X"))
                                {
                                    updatedcarddetail.bookmarkflag = string.Empty;
                                }
                                else
                                {
                                    updatedcarddetail.bookmarkflag = "X";
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                updatedcarddetail.nickname = name;
                            }
                            DEWAXP.Foundation.Integration.Responses.ServiceResponse<List<Evcarddetail>> Response = EVDashboardClient.UpdateEVCard(
                           new UpdateEVCardRequest
                           {
                               updateevcard = new Updateevcard
                               {
                                   contractaccount = accountnumber,
                                   sessionid = CurrentPrincipal.SessionToken,
                                   evcarddetails = updatedcarddetail
                               }
                           }, RequestLanguage, Request.Segment());
                            if (Response.Succeeded)
                            {
                                if (Response.Payload != null)
                                {
                                    CacheProvider.Store(CacheKeys.LISTOFCARDSERVICE_RESPONSE, new CacheItem<List<Evcarddetail>>(Response.Payload, TimeSpan.FromMinutes(20)));
                                    CacheProvider.Store(CacheKeys.LISTOFCARDSERVICE_SELECTEDACCOUNT, new CacheItem<string>(accountnumber, TimeSpan.FromMinutes(20)));
                                    //List<Evcarddetail> listofCardsResponse = ListofActiveEVCards(accountnumber, out ErrorMessage);
                                    return Request.CreateResponse(HttpStatusCode.OK, new { response = true });
                                }
                                ErrorMessage = "No available cards";
                            }
                            ErrorMessage = Response.Message;
                            LogService.Info(new System.Exception(ErrorMessage));
                            return Request.CreateResponse(HttpStatusCode.OK, new { response = true });
                        }
                    }
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