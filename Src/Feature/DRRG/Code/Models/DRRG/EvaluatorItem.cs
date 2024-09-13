using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.DRRG.Models
{
    public class EvaluatorItem
    {
        public string Id { get; set; }
        public string serialNumber { get; set; }
        public string loginid { get; set; }
        public string ReferenceID { get; set; }
        public string Name { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string Status { get; set; }
        public string ReviewedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string PublishedBy { get; set; }
    }
}