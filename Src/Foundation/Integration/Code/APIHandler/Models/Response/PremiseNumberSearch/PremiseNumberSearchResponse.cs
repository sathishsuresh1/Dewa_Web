namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.PremiseNumberSearch
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class PremiseNumberSearchResponse : ApiBaseResponse
    {
        [JsonProperty("mastervaluelist")]
        public List<MasterValueList> masterValueList;

        [JsonProperty("searchresultlist")]
        public List<SearchResultList> searchResultList;

        [JsonProperty("content")]
        public string content;

    }
    public class MasterValueList
    {
        [JsonProperty("key")]
        public string key;

        [JsonProperty("value")]
        public string value;

        [JsonProperty("dummy")]
        public string dummy;
    }
    public class SearchResultList
    {
        [JsonProperty("businesspartner")]
        public string businessPartner;

        [JsonProperty("communityname")]
        public string communityName;

        [JsonProperty("contractaccountnumber")]
        public string contractAccount;

        [JsonProperty("electricityapplication")]
        public string electricityApplication;

        [JsonProperty("gisx")]
        public string gisX;

        [JsonProperty("gisy")]
        public string gisY;

        [JsonProperty("legacynumber")]
        public string legacyNumber;

        [JsonProperty("load")]
        public string load;

        [JsonProperty("makaninumber")]
        public string makaniNumber;

        [JsonProperty("name")]
        public string name;

        [JsonProperty("premisetype")]
        public string premiseType;

        [JsonProperty("ptypetext")]
        public string ptypeText;

        [JsonProperty("roomnumber")]
        public string roomNumber;

        [JsonProperty("unitnumber")]
        public string unitNumber;

        [JsonProperty("waterapplication")]
        public string waterApplication;
    }
}
