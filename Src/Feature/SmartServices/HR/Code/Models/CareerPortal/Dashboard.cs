using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.CareerPortal
{
    public class Dashboard
    {
        public List<ApplicationDetail> applicationDetails { get; set; }
        public string postedApplicationDetails { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string errorCode { get; set; }
    }
    public class ApplicationDetail
    {
        public string ApplicationDate { get; set; }
        public string ApplicationStausID { get; set; }
        public string ApplicationStausText { get; set; }
        public string PostingGuid { get; set; }
        public string PostingGuidStatus { get; set; }
        public JobDetail JobDetails { get; set; }

    }
    public class JobDetail
    {
        public string JobId { get; set; }
        public string JobTitle { get; set; }
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string PlanVersion { get; set; }
    }

}