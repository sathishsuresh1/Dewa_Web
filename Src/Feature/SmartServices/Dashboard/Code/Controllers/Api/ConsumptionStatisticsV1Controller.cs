using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Feature.Dashboard.Models.ConsumptionStatistics;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Content.Models.ConsumptionStatistics;

namespace DEWAXP.Feature.Dashboard.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class ConsumptionStatisticsV1Controller : BaseApiController
    {
        public async Task<HttpResponseMessage> Get(string id)
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    var consumptionResponse = DewaApiClient.GetConsumptionHistory(CurrentPrincipal.SessionToken, id, RequestLanguage, Request.Segment());
                    if (consumptionResponse.Succeeded)
                    {
                        if (consumptionResponse.Payload != null)
                        {
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
                                responsedata.dataSeries.electricity = responsedata.dataSeries.electricity.OrderByDescending(x => (x.year.ToString("0000") + "" + x.month.ToString("00")))?.ToList().Take(6).ToList();
                            }

                            if (responsedata.dataSeries.water != null)
                            {
                                responsedata.dataSeries.water = responsedata.dataSeries.water.OrderByDescending(x => (x.year.ToString("0000") + "" + x.month.ToString("00")))?.ToList().Take(6).ToList();
                            }

                            var carbonResponse = DewaApiClient.GetCarbonFootprintReading(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id, RequestLanguage, Request.Segment());
                            if (carbonResponse != null && carbonResponse.Succeeded)
                            {
                                var d = responsedata.dataSeries.electricity?.FirstOrDefault();
                                responsedata.dataSeries.carbonFootprint = (d != null) ? new DataPointV1(d.year, d.month, carbonResponse.Payload) : new DataPointV1(0, 0, carbonResponse.Payload);
                            }

                            //Ascending Item for Top Latest.
                            if (responsedata.dataSeries.electricity != null)
                            {
                                responsedata.dataSeries.electricity = responsedata.dataSeries.electricity.OrderBy(x => (x.year.ToString("0000") + "" + x.month.ToString("00")))?.ToList().Take(6).ToList();
                            }

                            if (responsedata.dataSeries.water != null)
                            {
                                responsedata.dataSeries.water = responsedata.dataSeries.water.OrderBy(x => (x.year.ToString("0000") + "" + x.month.ToString("00")))?.ToList().Take(6).ToList();
                            }

                            return Request.CreateResponse(HttpStatusCode.OK, new { series = @return, carbonFootprint = carbonResponse.Payload, formattedData = responsedata });
                        }
                        ErrorMessage = "No consumption data available";
                    }
                    ErrorMessage = consumptionResponse.Message;
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