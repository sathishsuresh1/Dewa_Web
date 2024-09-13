using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class JobSearchResponse
    {
        public string Contracttype { get; set; }
        public string Errorcode { get; set; }
        public string Errormessage { get; set; }
        public string Functionalarea { get; set; }
        public List<Job> Joblist { get; set; }
        public string Keyword { get; set; }
        public string Success { get; set; }
        public string Userid { get; set; }
    }
    public class Job
    {
        public string Enddate { get; set; }
        public string Jobdescription { get; set; }
        public string Jobid { get; set; }
        public string Publishdate { get; set; }
        public string Referencecode { get; set; }
    }
}
