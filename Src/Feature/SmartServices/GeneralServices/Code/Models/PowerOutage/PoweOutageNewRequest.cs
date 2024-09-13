using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.PowerOutage
{
    public class PoweOutageNewRequest
    {
        public string AccountNo { get; set; }
        public string TypeOfWork {get;set;}
        public string PowerInterruption {get;set;}
        public string PurposeOfWork {get;set;}
        public string IsolationPoint {get;set;}
        public string DEWASubStationNumber {get;set;}
        public string CustomerAuthorizedPersonName {get;set;}
        public string CompanyName {get;set;}
        public string StartDate {get;set;}
        public string StartTime {get;set;}
        public string EndDate {get;set;}
        public string EndTime {get;set;}
        public string EmailID {get;set;}
        public string MobileNumber {get;set;}
        [MaxFileSize(3 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase CompanyLetterAttachement {get;set; }
        [MaxFileSize(3 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase MethodofStatementAttachment { get;set; }
        [MaxFileSize(3 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase RiskAssessmentAttachment {get;set; }
        [MaxFileSize(3 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase CustomerOutageRequestForm { get;set;}
        public List<SelectListItem> TypeOfWorkList { get; set; }
        public List<SelectListItem> PowerInterruptionList { get; set; }
        public List<DEWAXP.Foundation.Integration.Responses.PowerOutage.GetOutageDropDetails_WorkItem> PurposeOfWorkList { get; set; }
        public string status { get; set; }

        public bool IsCheckStart { get; set; }

    }
}