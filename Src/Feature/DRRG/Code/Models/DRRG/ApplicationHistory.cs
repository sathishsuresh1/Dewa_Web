using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.DRRG.Models
{
    public class ApplicationHistory
    {
        public long id { get; set; }
        public string Reference { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string User { get; set; }
        public string processingTime { get; set; }
    }
}