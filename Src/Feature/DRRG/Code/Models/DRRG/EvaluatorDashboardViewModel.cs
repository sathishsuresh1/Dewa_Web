using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.DRRG.Models
{
    public class EvaluatorDashboardViewModel
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public int Submittedcount { get; set; }
        public int Updatedcount { get; set; }
        public int Rejectedcount { get; set; }
        public int Equipmentcount { get; set; }
    }
}