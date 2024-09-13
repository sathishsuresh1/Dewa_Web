// <copyright file="BaseKofaxViewModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.ePass.SubPasses
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Defines the <see cref="BaseKofaxViewModel" />.
    /// </summary>
    public partial class BaseKofaxViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseKofaxViewModel"/> class.
        /// </summary>
        public BaseKofaxViewModel()
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
        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="variableName">The variableName<see cref="string"/>.</param>
        public Parameter(string variableName)
        {
            VariableName = variableName;
            Attribute = new List<Attribute>();
        }

        /// <summary>
        /// Gets or sets the VariableName
        /// Gets the VariableName..
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
        public EpassTypeEnum Type { get; set; }

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
    /// Defines the EpassTypeEnum.
    /// </summary>
    public enum EpassTypeEnum
    { 
        ///<summary>
      /// Defines the Binary.
      /// </summary>
        Binary,
        /// <summary>
        /// Defines the Text.
        /// </summary>
        Text,

        /// <summary>
        /// Defines the integer.
        /// </summary>
        integer
    };

    /// <summary>
    /// Defines the <see cref="Converter" />.
    /// </summary>
    internal static class Converter
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
                EpassTypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    /// <summary>
    /// Defines the <see cref="EpassTypeEnumConverter" />.
    /// </summary>
    internal class EpassTypeEnumConverter : JsonConverter
    {
        /// <summary>
        /// The CanConvert.
        /// </summary>
        /// <param name="t">The t<see cref="Type"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool CanConvert(Type t)
        {
            return t == typeof(EpassTypeEnum) || t == typeof(EpassTypeEnum?);
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
                    return EpassTypeEnum.Binary;
                case "text":
                    return EpassTypeEnum.Text;
            }
            throw new Exception("Cannot unmarshal type EpassTypeEnum");
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
            EpassTypeEnum value = (EpassTypeEnum)untypedValue;
            switch (value)
            {
                case EpassTypeEnum.Binary:
                    serializer.Serialize(writer, "binary");
                    return;
                case EpassTypeEnum.Text:
                    serializer.Serialize(writer, "text");
                    return;
            }
            throw new Exception("Cannot marshal type EpassTypeEnum");
        }

        /// <summary>
        /// Defines the Singleton.
        /// </summary>
        public static readonly EpassTypeEnumConverter Singleton = new EpassTypeEnumConverter();
    }
}
