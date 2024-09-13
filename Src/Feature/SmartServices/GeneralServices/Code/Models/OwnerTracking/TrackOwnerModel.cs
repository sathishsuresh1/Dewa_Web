using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.OwnerTracking
{
    public class TrackOwnerModel
    {
        public List<SelectListItem> SearchOptions { get; set; }
        public string selectedSearchType { get; set; }
        public string SearchText { get; set; }
        public List<SelectListItem> ApplicationTypeList { get; set; }
        public string selectedApplicationType { get; set; }
        public List<SelectListItem> ProjectAreaList { get; set; }
        public string selectedProjectArea { get; set; }
        public List<TrackResult> TrackResultList { get; set; }
        public bool isViewDetail { get; set; }
        public string processType { get; set; }
        public string nocNumer { get; set; }
        public string applicationNumer { get; set; }
    }
    public class TrackResult
    {
        public string ApplicationNumber { get; set; }
        public string NOCNumber { get; set; }
        public string ApplicationType { get; set; }
        public string ApplicationTypeCode { get; set; }
        public string ProjectArea { get; set; }
        public string PlotNo { get; set; }
        public string Status { get; set; }
        public string PowerRequirementDate { get; set; }
        public string DateApplied { get; set; }
        public string ColorCode { get; set; }

        public string CommunityCode { get; set; }
    }
}