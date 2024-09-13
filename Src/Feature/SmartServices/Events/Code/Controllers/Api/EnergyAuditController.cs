using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Enums;
using Sitecore.Globalization;
using System.Net;
using System.Net.Http;

namespace DEWAXP.Feature.Events.Controllers.Api
{
    public class EnergyAuditController : BaseApiController
    {
        //private IEFormServiceClient EFormsServiceClient
        //{
        //    get { return DependencyResolver.Current.GetService<IEFormServiceClient>(); }
        //}

        //private IDubaiModelServiceClient DubaiModelServiceClient
        //{
        //    get { return DependencyResolver.Current.GetService<IDubaiModelServiceClient>(); }
        //}

        public HttpResponseMessage Get(string contractaccountnumber)
        {
            var response = DubaiModelServiceClient.GetAccountClassification(contractaccountnumber,
                RequestLanguage, Request.Segment());
            if (!response.Succeeded || response.Payload == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, Translate.Text(DictionaryKeys.BuildingAudit.ContractAccountNotEligible));
            }

            if (response.Payload != null && (response.Payload.BillingClass == BillingClassification.Residential && response.Payload.PremiseType != GenericConstants.LABOURCAMPPREMISETYPE))
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, Translate.Text(DictionaryKeys.BuildingAudit.ContractAccountNotEligible));
            }

            return Request.CreateResponse(HttpStatusCode.OK, "1");
        }
    }
}