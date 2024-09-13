using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.CareerPortal
{
    public class JobDetails
    {
        public string JobId { get; set; }
        public string PostingGUID { get; set; }
        public string JobTitle { get; set; }
        public string EndDate { get; set; }
        public string ReferenceCode { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Project { get; set; }
        public string Task { get; set; }
        public string Requirement { get; set; }

    }
}