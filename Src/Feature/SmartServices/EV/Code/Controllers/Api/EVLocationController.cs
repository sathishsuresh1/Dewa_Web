// <copyright file="EVLocationController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Controllers.Api
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.Responses.EVLocationSvc;
    using DEWAXP.Foundation.Content.Controllers.Api;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Response.LocationDataPoints;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using System.Web.Services;
    using DEWAXP.Foundation.Integration.Responses;

    /// <summary>
    /// Defines the <see cref="EVLocationController" />.
    /// </summary>
    public class EVLocationController : BaseApiController
    {
        ///// <summary>
        ///// The GetLocationdata.
        ///// </summary>
        ///// <returns>The <see cref="List{EVLocation}"/>.</returns>
        //[HttpGet]
        //public HttpResponseMessage GetEVLocations()
        //{
        //    List<EVLocation> eVLocations = new List<EVLocation>();
        //    try
        //    {
        //        DEWAXP.Foundation.Integration.Responses.ServiceResponse<Devicelist> locations = EVCSClient.GetLocations();
        //        if (locations != null && locations.Succeeded && locations.Payload != null)
        //        {
        //            eVLocations = locations.Payload.Devices.Select(x => new EVLocation
        //            {
        //                Code = x.DeviceDBID,
        //                Status = x.DeviceStatus,
        //                HubeleonId = x.HubeleonID
        //            }).ToList();
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, eVLocations);
        //    }
        //    catch (System.Web.Mvc.HttpAntiForgeryException ex)
        //    {
        //        LogService.Fatal(ex, this);
        //        return Request.CreateResponse(HttpStatusCode.Unauthorized);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogService.Fatal(ex, this);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        //}

        [HttpPost, System.Web.Mvc.ValidateAntiForgeryToken]
        public async Task<HttpResponseMessage> GetMapDataPoints()
        {
            MapDataPoints resp = new MapDataPoints();
            try
            {
                var rawMessage = await Request.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(rawMessage))
                {
                    throw new Exception("parameter empty");
                }

                var serilizeData = JsonConvert.DeserializeObject<DEWAXP.Foundation.Integration.APIHandler.Models.Request.LocationDataPoints.DataPointsRequest.Root>(rawMessage);

                ServiceResponse<DataPointsResponse> locations = LocationDataPointsClient.GetMapDataPoints(serilizeData);

                if (locations != null && locations.Succeeded && locations.Payload != null)
                {
                    resp.CustomerService = locations.Payload.happinesscenterlist;
                    resp.FastEVCharge = locations.Payload.chargerlist?.Where(x => x.chargetype == "F").ToList();
                    resp.NormalEVCharge = locations.Payload.chargerlist?.Where(x => x.chargetype == "P").ToList();
                    resp.PaymentLocation = locations.Payload.paymentlocationlist;
                    resp.WaterSupply = locations.Payload.watersupplylist;
                }

                return Request.CreateResponse(HttpStatusCode.OK, resp);
            }
            catch (System.Web.Mvc.HttpAntiForgeryException ex)
            {
                LogService.Fatal(ex, this);
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
