// <copyright file="EVDashboardClient.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.APIHandler.Clients;
    using DEWAXP.Foundation.Integration.APIHandler.Config;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Helpers;
    using DEWAXP.Foundation.Integration.Impl.SmartCustomerV3Svc;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
    using DEWAXP.Foundation.Integration.Responses;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using DEWAXP.Foundation.DI;

    [Service(typeof(IEVDashboardClient), Lifetime = Lifetime.Transient)]
    /// <summary>
    /// Defines the <see cref="EVDashboardClient" />.
    /// </summary>
    public class EVDashboardClient : BaseApiDewaGateway, IEVDashboardClient
    {
        /// <summary>
        /// Gets the BaseApiUrl.
        /// </summary>
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";
        /// <summary>
        /// The FetchActiveEVCards.
        /// </summary>
        /// <param name="request">The request<see cref="FetchEVCardsRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{List{Evcarddetail}}"/>.</returns>
        public ServiceResponse<List<Evcarddetail>> FetchActiveEVCards(FetchEVCardsRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, SmartCustomerConstant.FETCH_ACTIVEEVCARDS, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FetchEVCardsResponse _Response = CustomJsonConvertor.DeserializeObject<FetchEVCardsResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000") && _Response.Evcarddetails != null && _Response.Evcarddetails.Count > 0)
                    {
                        return new ServiceResponse<List<Evcarddetail>>(_Response.Evcarddetails);
                    }
                    else
                    {
                        return new ServiceResponse<List<Evcarddetail>>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<List<Evcarddetail>>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<List<Evcarddetail>>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        /// <summary>
        /// The UpdateEVCard.
        /// </summary>
        /// <param name="request">The request<see cref="UpdateEVCardRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{List{Evcarddetail}}"/>.</returns>
        public ServiceResponse<List<Evcarddetail>> UpdateEVCard(UpdateEVCardRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.updateevcard.lang = language.Code();
                request.updateevcard.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, SmartCustomerConstant.UPDATEEVCARD, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FetchEVCardsResponse _Response = CustomJsonConvertor.DeserializeObject<FetchEVCardsResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000") && _Response.Evcarddetails != null && _Response.Evcarddetails.Count > 0)
                    {
                        return new ServiceResponse<List<Evcarddetail>>(_Response.Evcarddetails);
                    }
                    else
                    {
                        return new ServiceResponse<List<Evcarddetail>>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<List<Evcarddetail>>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<List<Evcarddetail>>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        /// <summary>
        /// The GetEVConsumptionDetails.
        /// </summary>
        /// <param name="request">The request<see cref="EVConsumptionRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{EVConsumptionResponse}"/>.</returns>
        public ServiceResponse<EVConsumptionResponse> GetEVConsumptionDetails(EVConsumptionRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, SmartCustomerConstant.EVCONSUMPTION, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EVConsumptionResponse _Response = CustomJsonConvertor.DeserializeObject<EVConsumptionResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<EVConsumptionResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<EVConsumptionResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<EVConsumptionResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<EVConsumptionResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        /// <summary>
        /// The GetEVTransactionalDetails.
        /// </summary>
        /// <param name="request">The request<see cref="EVConsumptionRequest"/>.</param>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <param name="segment">The segment<see cref="RequestSegment"/>.</param>
        /// <returns>The <see cref="ServiceResponse{EVTransactionalResponse}"/>.</returns>
        public ServiceResponse<EVTransactionalResponse> GetEVTransactionalDetails(EVConsumptionRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, SmartCustomerConstant.EVTRANSCATIONS, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EVTransactionalResponse _Response = CustomJsonConvertor.DeserializeObject<EVTransactionalResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<EVTransactionalResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<EVTransactionalResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<EVTransactionalResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<EVTransactionalResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        public ServiceResponse<List<Outstandingbreakdown>> GetOutstandingBreakDown(EVBillsRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.outstandingbreakdownIN.lang = language.Code();
                request.outstandingbreakdownIN.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, SmartCustomerConstant.OUTSTANDINGBREAKDOWN, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EVBillsResponse _Response = CustomJsonConvertor.DeserializeObject<EVBillsResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<List<Outstandingbreakdown>>(_Response.Outstandingbreakdown);
                    }
                    else
                    {
                        return new ServiceResponse<List<Outstandingbreakdown>>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<List<Outstandingbreakdown>>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<List<Outstandingbreakdown>>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        public ServiceResponse<EVDeepLinkResponse> GetEVSDPayment(EVDeeplinkRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, SmartCustomerConstant.EVSDPAYMENT, request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EVDeepLinkResponse _Response = CustomJsonConvertor.DeserializeObject<EVDeepLinkResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<EVDeepLinkResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<EVDeepLinkResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<EVDeepLinkResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<EVDeepLinkResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

    }
}
