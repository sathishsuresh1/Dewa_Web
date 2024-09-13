using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.Kofax
{
    /// <summary>
    /// Defines the <see cref="KofaxBaseViewModel" />.
    /// </summary>
    public partial class KofaxBaseViewModel
    {
        public KofaxBaseViewModel()
        {
            Parameters = new List<Parameter>();
        }
        /// <summary>
        /// Gets or sets the Parameters.
        /// </summary>
        [JsonProperty("parameters")]
        public List<Parameter> Parameters { get; set; }
    }
    /// <summary>
    /// Defines the <see cref="Parameter" />.
    /// </summary>
    public partial class Parameter
    {
        public Parameter(string variableName)
        {
            VariableName = variableName;
            Attribute = new List<Attribute>();
        }
        /// <summary>
        /// Gets the VariableName.
        /// </summary>
        [JsonProperty("variableName")]
        public string VariableName { get; set; }

        /// <summary>
        /// Gets or sets the Attribute.
        /// </summary>
        [JsonProperty("attribute")]
        public List<Attribute> Attribute { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Attribute" />.
    /// </summary>
    public partial class Attribute
    {
        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        [JsonProperty("type")]
        public kofaxTypeEnum Type { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
    /// <summary>
    /// Defines the SponsorTypeEnum.
    /// </summary>
    public enum kofaxTypeEnum
    { /// <summary>
      /// Defines the Binary.
      /// </summary>
        Binary,
        /// <summary>
        /// Defines the Text.
        /// </summary>
        Text,
        /// <summary>
        /// Defines number
        /// </summary>
        Number,
        /// <summary>
        /// Defines number
        /// </summary>
        Integer
    };
    /// <summary>
    /// Defines the <see cref="Converter" />.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Defines the Settings.
        /// </summary>
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                KofaxTypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    /// <summary>
    /// Defines the <see cref="KofaxTypeEnumConverter" />.
    /// </summary>
    internal class KofaxTypeEnumConverter : JsonConverter
    {
        /// <summary>
        /// The CanConvert.
        /// </summary>
        /// <param name="t">The t<see cref="Type"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool CanConvert(Type t)
        {
            return t == typeof(kofaxTypeEnum) || t == typeof(kofaxTypeEnum?);
        }

        /// <summary>
        /// The ReadJson.
        /// </summary>
        /// <param name="reader">The reader<see cref="JsonReader"/>.</param>
        /// <param name="t">The t<see cref="Type"/>.</param>
        /// <param name="existingValue">The existingValue<see cref="object"/>.</param>
        /// <param name="serializer">The serializer<see cref="JsonSerializer"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            string value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "binary":
                    return kofaxTypeEnum.Binary;
                case "text":
                    return kofaxTypeEnum.Text;
                case "number":
                    return kofaxTypeEnum.Number;
                case "integer":
                    return kofaxTypeEnum.Integer;
            }
            throw new Exception("Cannot unmarshal type SponsorTypeEnum");
        }

        /// <summary>
        /// The WriteJson.
        /// </summary>
        /// <param name="writer">The writer<see cref="JsonWriter"/>.</param>
        /// <param name="untypedValue">The untypedValue<see cref="object"/>.</param>
        /// <param name="serializer">The serializer<see cref="JsonSerializer"/>.</param>
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            kofaxTypeEnum value = (kofaxTypeEnum)untypedValue;
            switch (value)
            {
                case kofaxTypeEnum.Binary:
                    serializer.Serialize(writer, "binary");
                    return;
                case kofaxTypeEnum.Text:
                    serializer.Serialize(writer, "text");
                    return;
                case kofaxTypeEnum.Number:
                    serializer.Serialize(writer, "number");
                    return;
                case kofaxTypeEnum.Integer:
                    serializer.Serialize(writer, "integer");
                    return;
            }
            throw new Exception("Cannot marshal type SponsorTypeEnum");
        }

        /// <summary>
        /// Defines the Singleton.
        /// </summary>
        public static readonly KofaxTypeEnumConverter Singleton = new KofaxTypeEnumConverter();
    }
}