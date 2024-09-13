using DEWAXP.Foundation.Integration.SmartConsultantSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GeneralServices.Models.OwnerTracking
{
    public class TrackOwnerOrderModel
    {
        public string ApplicationNumber { get; set; }
        public string NOCNumber { get; set; }

        public string ApplicationType { get; set; }
        public buildingPermitElectricityDetails BuildingPermitElectricity_YBPE { get; set; }

        public string ErrorDescription { get; set; }

        public electricityDetails ElectricityNoc_YBNE { get; set; }

        public gettingElectricityDetails GettingElectricity_YDA5 { get; set; }

        public gettingWaterDetails GettingWater_YAPW { get; set; }

        public oneStepElectricityDetails OneStepElectricity_YLVI { get; set; }

        public string ResponseCode { get; set; }

        public waterDetails WaterNoc_YBNW { get; set; }
    }
}