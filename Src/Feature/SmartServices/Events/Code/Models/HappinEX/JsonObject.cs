using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Events.Models.HappinEX
{
    public class JsonObject
    {
        public string mood { get; set; }
        public List<Reason> reasons { get; set; }
        public string input { get; set; }
        public bool technical_discussion { get; set; }
        public string discussion_mode { get; set; }
        /// <summary>
        /// Tracking id, SAP generated for each customer noc request
        /// </summary>
        public string tid { get; set; }
        /// <summary>
        /// Survey id, Sitecore generated unique id for each survey.
        /// </summary>
        public string sid { get; set; }
        /// <summary>
        /// applicable to Pre survey only, how customer come to know about DEWA Noc online application, values like webiste, email, sms etc. 
        /// </summary>
        public string source { get; set; }

        public string DetailComment { get; set; }
        public List<ServiceFeedback> ServiceFeedbacks { get; set; }
    }
    public class Reason
    {
        public string label { get; set; }
        public List<string> reasons { get; set; }
        public string suggestion { get; set; }
    }


    public class ServiceFeedback
    {

        public string SurveyService { get; set; }
        public string QusScId { get; set; }
        public string Rating { get; set; }
        public string Comment { get; set; }
        public string Descision { get; set; }

    }



}