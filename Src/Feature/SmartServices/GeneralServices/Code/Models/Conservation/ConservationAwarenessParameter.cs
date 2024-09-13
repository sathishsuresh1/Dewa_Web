using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GeneralServices.Models.Conservation
{
    public class ConservationAwarenessParameter
    {
        public string SocialMediaChannel { get; set; }
        public string ParentTarget { get; set; }
        public string ParentAchievement { get; set; }


        // Environmental Activities
        public List<string> AwarEnvironmentalSelectedValue { get; set; }
        public List<string> AwarEnvironmentalSelectedDetail { get; set; }
        public List<string> AwarEnvironmentalSelectedStdno { get; set; }

        // Lectures & Workshops
        public List<string> AwarLecturesSelectedValue { get; set; }
        public List<string> AwarLecturesSelectedDetail { get; set; }
        public List<string> AwarLecturesSelectedStdno { get; set; }
    }
}