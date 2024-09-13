using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Mvc.Presentation;
using System.ComponentModel.DataAnnotations;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
	[Serializable]
    public class MyEstimates
    {
	    public MyEstimates()
	    {
		    Estimates = new List<EstimateCustomerItemResponse>();
	    }

        public List<EstimateCustomerItemResponse> Estimates { get; set; }
    }
}