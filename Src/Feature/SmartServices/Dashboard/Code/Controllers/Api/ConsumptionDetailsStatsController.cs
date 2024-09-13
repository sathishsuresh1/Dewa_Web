using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DEWAXP.Feature.Dashboard.Models.ConsumptionStatistics;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Models.ConsumptionStatistics;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.meterreading;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.meterreading;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Feature.Dashboard.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class ConsumptionDetailsStatsController : BaseApiController
    {
        public async Task<HttpResponseMessage> ConsumptionDetailsStats(string id)
        {
            string ErrorMessage = "";
            char _seperator = 'X';
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    string consumptionaccount;
                    
                    string premiseNumber = string.IsNullOrEmpty(id) ? "" : id.Split(_seperator)?[1];
                    id = string.IsNullOrEmpty(id) ? "" : id.Split(_seperator)[0];
                    ServiceResponse<YearlyConsumptionDataResponse> consumptionResponse = null;
                    if (CacheProvider.TryGet(CacheKeys.SELECTED_CONSUMPTIONACCOUNT, out consumptionaccount))
                    {
                        if (!string.IsNullOrEmpty(consumptionaccount) && consumptionaccount.Equals(id))
                        {
                            if (CacheProvider.TryGet(CacheKeys.SELECTED_CONSUMPTION, out consumptionResponse))
                            {
                            }
                        }
                    }
                    if(consumptionResponse == null)
                    {
                        consumptionResponse = DewaApiClient.GetConsumptionHistory(CurrentPrincipal.SessionToken, id, RequestLanguage, Request.Segment());
                        CacheProvider.Store(CacheKeys.SELECTED_CONSUMPTION, new CacheItem<ServiceResponse<YearlyConsumptionDataResponse>>(consumptionResponse, TimeSpan.FromMinutes(20)));
                        CacheProvider.Store(CacheKeys.SELECTED_CONSUMPTIONACCOUNT, new CacheItem<string>(id, TimeSpan.FromMinutes(20)));
                    }
                    if (consumptionResponse != null && consumptionResponse.Succeeded)
                    {
                        if (consumptionResponse.Payload != null)
                        {
                            ServiceResponse<slabTarrifOut> SlabResponse = null;
                            if (CacheProvider.TryGet(CacheKeys.SELECTED_CONSUMPTIONACCOUNT, out consumptionaccount))
                            {
                                if (!string.IsNullOrEmpty(consumptionaccount) && consumptionaccount.Equals(id))
                                {
                                    if (CacheProvider.TryGet(CacheKeys.SELECTED_CONSUMPTION_SLAB, out SlabResponse))
                                    {
                                    }
                                }
                            }
                            if (SlabResponse == null)
                            {
                                SlabResponse = DewaApiClient.GetSlabCaps(new GetSlabCaps
                                {
                                    slabTarrifIn = new slabTarrifIn
                                    {
                                        contractAccount = id,
                                        credential = CurrentPrincipal.SessionToken,
                                        
                                    }
                                }, RequestLanguage, Request.Segment());
                                CacheProvider.Store(CacheKeys.SELECTED_CONSUMPTION_SLAB, new CacheItem<ServiceResponse<slabTarrifOut>>(SlabResponse, TimeSpan.FromMinutes(20)));
                            }
                            ServiceResponse<MeterreadingResponse> MeterReading = DTMCInsightsReportClient.GetMeterReading(new MeterreadingRequest
                            {
                                userid = CurrentPrincipal.UserId,
                                usagetypeE=true,
                                usagetypeW= true,
                                sessionid = CurrentPrincipal.SessionToken,
                                vendorid = "EPAY",
                                lang = RequestLanguage.Code(),
                                contractaccountnumber=id,
                                premisenumber= premiseNumber
                            });
                            ConsumptionDetail consumptionDetail = new ConsumptionDetail();
                            ConsumtionResponseData responsedata = new ConsumtionResponseData();

                            var @return = DataSeries.Create(consumptionResponse.Payload);
                            if (!@return.Any(series => series.Utility.Equals(MunicipalService.Electricity)))
                            {
                                @return.Add(DataSeries.Null(MunicipalService.Electricity));
                            }

                            if (!@return.Any(series => series.Utility.Equals(MunicipalService.Water)))
                            {
                                @return.Add(DataSeries.Null(MunicipalService.Water));
                            }

                            responsedata.dataSeries = new TypeDataSeries();
                            List<DataPointwithutility> lstDatapointswithutility = new List<DataPointwithutility>();
                            responsedata.dataSeries.electricity = new List<DataPointV1>();
                            responsedata.dataSeries.water = new List<DataPointV1>();
                            foreach (var dataSeries in @return?.Where(x => x != null))
                            {
                                if (dataSeries != null && dataSeries.Utility == MunicipalService.Electricity)
                                {
                                    foreach (var item in dataSeries.DataPoints)
                                    {
                                        responsedata.dataSeries.electricity.Add(new DataPointV1(item.Year, item.Month, item.Value));
                                    }
                                    
                                }

                                if (dataSeries != null && dataSeries.Utility == MunicipalService.Water)
                                {
                                    foreach (var item in dataSeries.DataPoints)
                                    {
                                        responsedata.dataSeries.water.Add(new DataPointV1(item.Year, item.Month, item.Value));
                                    }
                                    
                                }
                            }

                            //descending Item for Top Latest.
                            if (responsedata.dataSeries.electricity != null)
                            {
                                responsedata.dataSeries.electricity = responsedata.dataSeries.electricity.OrderByDescending(x => (x.year.ToString("0000") + "" + x.month.ToString("00")))?.ToList().Take(13).ToList();
                            }

                            if (responsedata.dataSeries.water != null)
                            {
                                responsedata.dataSeries.water = responsedata.dataSeries.water.OrderByDescending(x => (x.year.ToString("0000") + "" + x.month.ToString("00")))?.ToList().Take(13).ToList();
                            }

                            responsedata.dataSeries.electricity.ForEach(x => lstDatapointswithutility.Add(new DataPointwithutility(x, MunicipalService.Electricity)));
                            responsedata.dataSeries.water.ForEach(x => lstDatapointswithutility.Add(new DataPointwithutility(x, MunicipalService.Water)));
                            consumptionDetail = ConsumptionDetail.From(lstDatapointswithutility, SlabResponse.Payload,MeterReading.Payload);
                            return Request.CreateResponse(HttpStatusCode.OK, new { dataseries = consumptionDetail.dataSeries });
                        }
                        ErrorMessage = "No consumption data available";
                    }
                    ErrorMessage = consumptionResponse.Message;
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
