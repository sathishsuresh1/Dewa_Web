using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Events.Models.HappinEX
{
    public class SurveyPageModel
    {
        public BasicSurvey Survey { get; set; }
        public string Key { get; set; }

        public string TrackingID { get; set; }
        public string SurveyID { get; set; }
        private bool _error = false;

        public bool ShowError
        {
            get { return this._error; }
            set { this._error = value; }
        }

        //public string SurveyJson { get; set; }
    }
}