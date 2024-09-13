// <copyright file="DewaStoreClient.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.DewaStoreSvc
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.DewaStoreSvc;
    using RestSharp;
    using System.Configuration;
    using System.Net.Http;
    using System.Web.Configuration;
    using DEWAXP.Foundation.DI;

    /// <summary>
    /// Defines the <see cref="DewaStoreClient" />.
    /// </summary>
    [Service(typeof(IDewaStoreClient),Lifetime =Lifetime.Transient)]
    public class DewaStoreClient : BaseDewaGateway, IDewaStoreClient
    {
        internal string DEWASTOREURL { get; set; } = WebConfigurationManager.AppSettings["DEWASTORE_URL"];

        public ServiceResponse<OfferBaseResponse> GetAllWebOffers(int IsHotOffer,int IsFeaturedOffer,int IsNewOffer,string CompanyUno,string CategoryUno,int PageSize,int PageNumber,int Condition,string ContractAccNumber,SupportedLanguage language)
        {
            try
            {
                RestRequest request = null;

                RestClient client = CreateClient();

                request = new RestRequest("offers", Method.POST);

                request = CreateHeader(request);

                //SQL
                var resquestBody = new
                {
                    LanguageCode = language.Code(),
                    IsHotOffer,
                    IsFeaturedOffer,
                    IsNewOffer,
                    CompanyUno,
                    CategoryUno,
                    PageSize,
                    PageNumber,
                    Condition,
                    ContractAccNumber
                };
                request.AddBody(resquestBody);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    OfferBaseResponse _offerResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OfferBaseResponse>(response.Content);

                    return new ServiceResponse<OfferBaseResponse>(_offerResponse);
                }
                else
                {
                    return new ServiceResponse<OfferBaseResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {

                LogService.Fatal(ex, this);
                return new ServiceResponse<OfferBaseResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<OfferBaseResponse> GetAllGuestOffers(int IsHotOffer, int IsFeaturedOffer, int IsNewOffer, string CompanyUno, string CategoryUno, int PageSize, int PageNumber, int Condition, SupportedLanguage language)
        {
            try
            {
                RestRequest request = null;

                RestClient client = CreateClient();

                request = new RestRequest("guestoffers", Method.POST);

                request = CreateHeader(request);

                //SQL
                var resquestBody = new
                {
                    LanguageCode = language.Code(),
                    IsHotOffer,
                    IsFeaturedOffer,
                    IsNewOffer,
                    CompanyUno,
                    CategoryUno,
                    PageSize,
                    PageNumber,
                    Condition,
                    //ContractAccNumber
                };
                request.AddBody(resquestBody);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    OfferBaseResponse _offerResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OfferBaseResponse>(response.Content);

                    return new ServiceResponse<OfferBaseResponse>(_offerResponse);
                }
                else
                {
                    return new ServiceResponse<OfferBaseResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {

                LogService.Fatal(ex, this);
                return new ServiceResponse<OfferBaseResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        private RestClient CreateClient()
        {
            return new RestClient(DEWASTOREURL);
        }

        private RestRequest CreateHeader(RestRequest request)
        {
            request.AddHeader("Authorization",  "Bearer " + OAuthToken.GetAccessToken());
            request.RequestFormat = DataFormat.Json;
            return request;
        }
        
    }
}
