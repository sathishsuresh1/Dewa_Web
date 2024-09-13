using DEWAXP.Foundation.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.IdealHome.Models.IdealHome
{
    public class Survey:GlassBase
    {
        public IEnumerable<SurveyQuestions> Questions { get; set; }

        public string Entity { get; set; }
        public string ParticipantName { get; set; }
        public string Address { get; set; }
        public string DateTime { get; set; }
        public string Telephone { get; set; }
        public string AssessorPR { get; set; } 

                        
    }
}