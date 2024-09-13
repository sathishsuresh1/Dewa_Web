using DEWAXP.Foundation.Content.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Configuration;

namespace DEWAXP.Feature.ChatBot.Models.HayakChat
{
    public class HayakChatModel
    {
        public static string chatHost = WebConfigurationManager.AppSettings["chatHost"]; // "ws://10.15.132.39/services/customer/chat";
        public static string chatStore = WebConfigurationManager.AppSettings["chatStore"]; //"ws://10.15.132.32/services/OceanaCoreDataService/oceana/data/";
        public static string chatInitUrl = WebConfigurationManager.AppSettings["chatInitUrl"]; // "http://10.15.132.32/services/OceanaCoreDataService/oceana/data/context/schema";
        public static string estimatedWaitTimeUrl = WebConfigurationManager.AppSettings["estimatedWaitTimeUrl"]; //"http://10.15.132.39/services/CustomerControllerService/gila/ewt/request";
        public static string fileDownloadUrl = WebConfigurationManager.AppSettings["fileDownloadUrl"];
        public static string coBrowseHost = WebConfigurationManager.AppSettings["coBrowseHost"];

        //public string SocketUrl { get { return  } }
        public string RequestId { get; set; }

        public string RequestLang { get; set; }
        public DewaProfile UserProfile { get; set; }
        public bool IsServerError = false;

        public string GetFirstName()
        {
            if (this.UserProfile == null || this.UserProfile.FullName.Length > 2) return string.Empty;
            var fn = UserProfile.FullName.Split(' ');
            return fn[0];
        }

        public string configdata()
        {
            return JsonConvert.SerializeObject(HayakConstant.config);
        }

        public string ToJson()
        { return Newtonsoft.Json.JsonConvert.SerializeObject(this); }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BaseContactDetailsModel
    {
        [JsonProperty("FullName")]
        public string FullName { get; set; }

        /*[JsonProperty("LastName")]
        public string LastName { get; set; }*/

        [JsonProperty("MobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("EmailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("AccountNumber")]
        public string AccountNumber { get; set; }

        [JsonIgnore]
        public bool ChatBotRequired { get; set; }
    }

    public class Data : BaseContactDetailsModel
    {
        [JsonProperty("ServiceType")]
        public string ServiceType { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("OriginType")]
        public string OriginType { get; set; }

        [JsonProperty("Authenticated")]
        public string Authenticated { get; set; }

        [JsonProperty("ChatbotRequired")]
        public string ChatbotRequired { get; set; }
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
        public Data data { get; set; }
        public string groupId { get; set; }
        public bool persistToEDM { get; set; }
        public Schema schema { get; set; }
    }
}