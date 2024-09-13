using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.Conservation
{
    [Bind(Include = "SavingObjective,SavingBehaviour,SavingInnovation,TechAirconditioningSelectedValue,TechAirconditioningSelectedDetail,TechLightingSelectedValue,TechLightingSelectedDetail, TechOfficeEquipmentSelectedValue,TechOfficeEquipmentSelectedDetail,TechWaterSelectedValue,TechWaterSelectedDetail,TechOtherSelectedValue,TechOtherSelectedDetail,TechNoneSelectedValue,TechNoneSelectedDetail")]
    public class ConservationTechnicalParameter
    {
        public string SavingObjective { get; set; }
        public string SavingBehaviour { get; set; }
        public string SavingInnovation { get; set; }

        // AirConditioning
        public List<string> TechAirconditioningSelectedValue { get; set; }
        public List<string> TechAirconditioningSelectedDetail { get; set; }

        // Lighting
        public List<string> TechLightingSelectedValue { get; set; }
        public List<string> TechLightingSelectedDetail { get; set; }

        // Office Equipment
        public List<string> TechOfficeEquipmentSelectedValue { get; set; }
        public List<string> TechOfficeEquipmentSelectedDetail { get; set; }

        // Water
        public List<string> TechWaterSelectedValue { get; set; }
        public List<string> TechWaterSelectedDetail { get; set; }

        // Other
        public List<string> TechOtherSelectedValue { get; set; }
        public List<string> TechOtherSelectedDetail { get; set; }

        // None
        public List<string> TechNoneSelectedValue { get; set; }
        public List<string> TechNoneSelectedDetail { get; set; }
    }
}