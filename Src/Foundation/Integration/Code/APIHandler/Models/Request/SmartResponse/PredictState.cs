using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartResponseModel
{
    public class PredictState
    {
        //
        public byte[] photo { get; set; }
        public string prev { get; set; }
        public string end { get; set; }
        [JsonProperty("boxes")]
        public List<List<double>> boxes { get; set; }
        public string image { get; set; }
        public string result { get; set; }
        public string imgHeight { get; set; }
        public string imgWidth { get; set; }
        public string height { get; set; }
        public string width { get; set; }
        public string length { get; set; }
        public string sol { get; set; }
        /// <summary>
        /// Fuse box Detection Flag for uplaoded Image
        /// </summary>
        public bool fuseboxDetectionFlag { get; set; }


    }
    public class Boxes {
        public string[] d { get; set; } 
        //string x { get; set; }
        //string y { get; set; }

        //string w { get; set; }

        //string h { get; set; }

    }
}