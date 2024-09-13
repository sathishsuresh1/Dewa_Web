using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DEWAXP.Foundation.Integration.Impl.JobSeekerSvc;

namespace DEWAXP.Feature.HR.Models.CareerPortal
{
    public class SearchJobModel
    {
        public string Contracttype { get; set; }
        public string Errorcode { get; set; }
        public string Errormessage { get; set; }
        public string Functionalareacode { get; set; }
        public string Hierarcy { get; set; }
        public List<Jobs> Joblist { get; set; }
        public string Keyword { get; set; }
        public string Success { get; set; }
        public string Userid { get; set; }
        public List<JobFilters> Functionalarea { get; set; }
        public List<JobFilters> Hierarcylevel { get; set; }
        /// <summary>
        /// Gets or sets the totalpage
        /// </summary>
        public int totalpage { get; set; }
        /// <summary>
        /// Gets or sets the page
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether pagination
        /// </summary>
        public bool pagination { get; set; }
        /// <summary>
        /// Gets or sets the pagenumbers
        /// </summary>
        public IEnumerable<int> pagenumbers { get; set; }
        public int totalRecords { get; set; }
    }
    public class Jobs
    {
        public string functionalArea { get; set; }
        public string Jobdescription { get; set; }
        public string Jobid { get; set; }
        public string Publishdate { get; set; }
        public string Referencecode { get; set; }
        public string EmploymentEndDate { get; set; }
        public string JobPostingKey { get; set; }
    }
    public class JobFilters
    {
        public string Code { get; set; }
        public string Description { get; set; }

    }
}