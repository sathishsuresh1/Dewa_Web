using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    public class StatusNotificationDetailModel
    {
        public string Steps { get; set; }
        public int TotalSteps { get; set; }

        public string ResponseCode { get; set; }
        public int CurrentStepIndex { get; set; }

        public string StatusDescription { get; set; }        

        public static StatusNotificationDetailModel MapToModel(DEWAXP.Foundation.Integration.Responses.EVGreenCard.Trd.Return response)
        {
            StatusNotificationDetailModel model = new StatusNotificationDetailModel();
            var steps = response.evstatuslist.OrderBy(x => x.statuscode).ToArray();
            //var current_step=response.evstatuslist.Where(x=>x.)
            model.Steps = string.Join(",", steps.Select(x => x.description));

            model.TotalSteps = response.evstatuslist.Count;
            model.ResponseCode = response.responsecode;
            model.CurrentStepIndex = Array.IndexOf(steps, steps.Where(x => x.statuscode == response.statuscode).FirstOrDefault())+1;
            model.StatusDescription = response.statusdescription;            

            return model;
        }

    }
}