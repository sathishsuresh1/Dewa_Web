using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.DRRG.Models
{
    public class ReportModel
    {
        public IDictionary<string, int> keyValuePairs { get; set; }
        public List<ProcessedApplications> processedApplications { get; set; }
        public List<ProcessedApplications> processedApplicationsMonYear { get; set; }
        public List<EquipmentType> processedEquipmentManufacturer { get; set; }

        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string selectedPeriod { get; set; }
        public long Submitted { get; set; }
        public long Approved { get; set; }
        public long Rejected { get; set; }
        public long SchemeMgrRejected { get; set; }
        public long ReviewerApproved { get; set; } 
        public long Updated { get; set; }
        public long PVModuleApproved { get; set; }
        public long PVModuleRejected { get; set; }
        public long IVModuleApproved { get; set; }
        public long IVModuleRejected { get; set; }
        public long IPModuleApproved { get; set; }
        public long IPModuleRejected { get; set; }

    }
    public class ProcessedApplications
    {
        public string Model { get; set; }
        public string MonYear { get; set; }
        public string Status { get; set; }
        public Int32 Applications { get; set; }
    }
    public class EquipmentType
    {
        public string ApplicationName { get; set; }
        public Int32 Applications { get; set; }
    }
}