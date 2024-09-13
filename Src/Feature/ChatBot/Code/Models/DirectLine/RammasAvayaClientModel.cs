using DEWAXP.Foundation.Content.Services;
using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;


namespace DEWAXP.Feature.ChatBot.Models.DirectLine
{
    public class RammasAvayaClientModel : ChatModel
    {
        public static string chatHost = WebConfigurationManager.AppSettings["chatHost"]; // "ws://10.15.132.39/services/customer/chat";
        public static string chatStore = WebConfigurationManager.AppSettings["chatStore"]; //"ws://10.15.132.32/services/OceanaCoreDataService/oceana/data/";
        public static string chatInitUrl = WebConfigurationManager.AppSettings["chatInitUrl"]; // "http://10.15.132.32/services/OceanaCoreDataService/oceana/data/context/schema";
        public static string estimatedWaitTimeUrl = WebConfigurationManager.AppSettings["estimatedWaitTimeUrl"]; //"http://10.15.132.39/services/CustomerControllerService/gila/ewt/request";
        //public string SocketUrl { get { return  } }
        public string RequestId { get; set; }

        public DewaProfile UserProfile { get; set; }
        public bool IsServerError = false;
        public string ToJson()
        { return Newtonsoft.Json.JsonConvert.SerializeObject(this); }
    }

    #region InitReqstObject
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Data1
    {
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string EmailAddress { get; set; }
    }

    public class Attributes
    {
        public List<string> Channel { get; set; }
        public List<string> Language { get; set; }
        public List<string> ServiceType { get; set; }
    }

    public class SM
    {
        public Attributes attributes { get; set; }
        public int priority { get; set; }
    }

    public class ServiceMap
    {
        [Newtonsoft.Json.JsonProperty("1")]
        public SM SM1 { get; set; }
    }

    public class Schema
    {
        public string CustomerId { get; set; }
        public string Locale { get; set; }
        public ServiceMap ServiceMap { get; set; }
        public string Strategy { get; set; }
    }

    public class InitReqRoot
    {
        public Data1 data { get; set; }
        public string groupId { get; set; }
        public bool persistToEDM { get; set; }
        public Schema schema { get; set; }
    }

    #endregion

    #region InitResponseObject

    public class RetObj
    {
        public string contextId { get; set; }
    }
    public class InitResponseRoot
    {
        public RetObj data { get; set; }
    }

    #endregion
}