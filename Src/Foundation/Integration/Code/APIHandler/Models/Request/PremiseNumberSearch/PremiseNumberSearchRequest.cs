namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.PremiseNumberSearch
{
    using Newtonsoft.Json;
    public class PremiseNumberSearchRequest
    {
        [JsonProperty("logintype")]
        public string logintype;

        [JsonProperty("searchflag")]
        public string searchflag;

        [JsonProperty("sessionid")]
        public string sessionid;

        [JsonProperty("lang")]
        public string lang;

        [JsonProperty("vendor")]
        public string vendor;

        [JsonProperty("searchinput")]
        public SearchInput searchinput;

        [JsonProperty("downloadFlag")]
        public string downloadFlag;

    }
    public class SearchInput
    {
        [JsonProperty("key")]
        public string key;

        [JsonProperty("value")]
        public string value;
    }
}
