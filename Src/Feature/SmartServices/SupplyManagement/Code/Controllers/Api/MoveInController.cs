using DEWAXP.Feature.SupplyManagement.Models.MoveIn;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DEWAXP.Feature.SupplyManagement.Controllers.Api
{
    public class MoveInController : BaseApiController
    {
        public HttpResponseMessage Get(string idtype, string businesspartner, string moveoutaccount, string moveoutdate, string idnumber, string customerCategory, string customerType, string accounttype, string occupiedby, string ejarinumber, string contractnumber, string startdate, string enddate, string contractvalue, string numberofrooms, [FromUri] string[] premiselist)
        {
            moveinpassedmodel model = new moveinpassedmodel
            {
                idtype = idtype,
                accounttype = accounttype,
                businesspartner = businesspartner,
                contractnumber = contractnumber,
                contractvalue = contractvalue,
                customerCategory = customerCategory,
                customerType = customerType,
                ejarinumber = ejarinumber,
                enddate = enddate,
                idnumber = idnumber,
                moveoutaccount = moveoutaccount,
                moveoutdate = moveoutdate,
                numberofrooms = numberofrooms,
                occupiedby = occupiedby,
                premiselist = premiselist,
                startdate = startdate
            };
            bool skipservicecall = false;
            moveinpassedmodel model1;
            ServiceResponse<moveInPostOutput> response = null;
            if (CacheProvider.TryGet(CacheKeys.MOVEIN_PASSEDMODEL, out model1) && CacheProvider.TryGet(CacheKeys.MOVEIN_PASSEDRESPONSE, out response))
            {
                if (CompareObject.Compare(model, model1))
                {
                    skipservicecall = true;
                }
            }
            if (!skipservicecall)
            {
                //Added for Future Center
                var _fc = FetchFutureCenterValues();
                string moveoutaccountnumber = !string.IsNullOrWhiteSpace(model.moveoutaccount) ? model.moveoutaccount : string.Empty;
                string startdatepost = string.Empty;
                string enddatepost = string.Empty;
                string contractvaluepost = string.Empty;
                string numberofroomspost = string.Empty;
                if (string.IsNullOrWhiteSpace(model.ejarinumber))
                {
                    DateTime startdt;
                    if (DateTime.TryParse(model.startdate, out startdt))
                    {
                        startdatepost = startdt.ToString("yyyyMMdd");
                    }
                    DateTime enddt;
                    if (DateTime.TryParse(model.enddate, out enddt))
                    {
                        enddatepost = enddt.ToString("yyyyMMdd");
                    }
                    contractvaluepost = model.contractvalue;
                    numberofroomspost = model.numberofrooms;
                }

                response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                {
                    premiseDetailsList = model.premiselist.Where(x => x != "").Select(x => new premiseDetails { premise = x }).ToArray(),
                    moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           customercategory=model.customerCategory,
                           customertype=model.customerType,
                           accounttype =model.accounttype,
                           occupiedby = model.occupiedby,
                           businesspartnernumber=model.businesspartner,
                           idtype=model.idtype,
                           idnumber=model.idnumber,
                           moveoutdate=model.moveoutdate,
                           moveoutcontractaccount=model.moveoutaccount,
                           ejarinumber=model.ejarinumber,
                           contractnumber =model.contractnumber,
                           tenancystartdate=startdatepost,
                           tenancyenddate=enddatepost,
                           tenancycontractvalue=contractvaluepost,
                           noofrooms=numberofroomspost,
                           center =_fc.Branch
                       }
                    },
                    userid = CurrentPrincipal.UserId ?? string.Empty,
                    sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                    lang = RequestLanguage.Code(),
                    executionflag = "R",
                    channel = "A",
                    applicationflag = "M",
                }, Request.Segment());
                CacheProvider.Store(CacheKeys.MOVEIN_PASSEDMODEL, new CacheItem<moveinpassedmodel>(model, TimeSpan.FromMinutes(20)));
                CacheProvider.Store(CacheKeys.MOVEIN_PASSEDRESPONSE, new CacheItem<ServiceResponse<moveInPostOutput>>(response, TimeSpan.FromMinutes(20)));
            }
            if (response != null && response.Payload != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response.Payload);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}