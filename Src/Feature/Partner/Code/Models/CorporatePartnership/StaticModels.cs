using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Partner.Models.CorporatePartnership.StaticModels
{
    public static class Base
    {
        public const string UserID = "UserID";
        public const string UserName = "UserName";
        public const string ProcessingType = "ProcessingType";
        public const string Partnerid = "Partnerid";
        public const string Msg_subject = "Msg_subject";
        public const string FromMail = "FromMail";
        public const string ToMail = "ToMail";
        public const string Partnername = "Partnername";
        public const string CreationDate = "CreationDate";
        public const string CreatedBy = "CreatedBy";
        public const string ModifiedDate = "ModifiedDate";
        public const string ModifiedBy = "ModifiedBy";
        public const string Status = "Status";
        public const string Message = "Message";
        public const string RequestorName = "RequestorName";
    }
    public static class Idea
    {
        public const string IdeaID = "IdeaID";

    }
    public static class Meeting
    {
        public const string LOCATION = "LOCATION";
        public const string FromTime = "Fromtime";
        public const string ToTime = "Totime";
        public const string EventName = "EVENTNAME";
        public const string FromDate = "Fromdate";
        public const string MeetingID = "MeetingID";
        public const string Attachment = "Attachment";
        public const string ToDate = "Todate";
        public const string EventRequestType = "EVENTREQUESTTYPE";
        //public const string RequestTyype = "RequestType";

    }

    public static class Issue
    {
        public const string IssueID = "IssueID";
    }
    public static class MeetingRequestType
    {
        public const string Meeting = "M";
        public const string Visit = "V";
        public const string Benchmark = "B";

    }
    public static class MeetingRequestTypeLong
    {
        public const string Meeting = "Meeting Request";
        public const string Visit = "Visit Request";
        public const string Benchmark = "Benchmark Request";
    }
    public static class Message
    {
        public const string MessageID = "MessageID";
    }

}
