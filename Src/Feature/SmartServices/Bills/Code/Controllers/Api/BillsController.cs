// <copyright file="BillsController.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Bills.Controllers.Api
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers.Api;
    using DEWAXP.Foundation.Content.Filters.Http;
    using DEWAXP.Foundation.Content.Models;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Logger;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Helpers;
    using System.Web.Http;

    /// <summary>
    /// Defines the <see cref="BillsController" />.
    /// </summary>
    [TwoPhaseAuthorize]
    public class BillsController : BaseApiController
    {
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/>.</returns>
        [HttpPost, System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<HttpResponseMessage> Totalamount(string id)
        {
            string ErrorMessage = "";

            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    AntiForgery.Validate();
                    FutureCenterValues _fc = FetchFutureCenterValues();
                    CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(id.TrimStart(new char[] { '0' }), Times.Once));
                    DEWAXP.Foundation.Integration.Responses.ServiceResponse<DEWAXP.Foundation.Integration.Responses.BillEnquiryResponse> bill = DewaApiClient.GetBill(id, RequestLanguage, Request.Segment(), _fc.Branch, CurrentPrincipal.SessionToken);
                    if (bill.Succeeded)
                    {
                        Account model = Account.From(bill.Payload);
                        return Request.CreateResponse(HttpStatusCode.OK, model);
                    }
                    ErrorMessage = bill.Message;
                }
                catch (System.Web.Mvc.HttpAntiForgeryException ex)
                {
                    LogService.Fatal(ex, this);
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ErrorMessage);
                }
                catch (Exception ex)
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