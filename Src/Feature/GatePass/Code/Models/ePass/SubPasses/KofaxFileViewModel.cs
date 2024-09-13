
namespace DEWAXP.Feature.GatePass.Models.ePass.SubPasses
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    public partial class KofaxFileViewModel
    {
        [JsonProperty("FileContent")]
        public string FileContent { get; set; }
        [JsonProperty("FileName")]
        public string FileName { get; set; }
        [JsonProperty("FileType")]
        public string FileType { get; set; }
        [JsonProperty("FileExtension")]
        public string FileExtension { get; set; }
        [JsonProperty("FileContentType")]
        public string FileContentType { get; set; }
        [JsonProperty("FileCategory")]
        public string FileCategory { get; set; }
    }
}