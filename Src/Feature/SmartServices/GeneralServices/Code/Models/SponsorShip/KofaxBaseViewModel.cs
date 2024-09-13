﻿// <copyright file="KofaxBaseViewModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\mayur.prajapati</author>

namespace DEWAXP.Feature.GeneralServices.Models.Sponsorship
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

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
        public List<Attribute> Attribute {get ; set ; }
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
        public SponsorTypeEnum Type { get; set; }

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
    public enum SponsorTypeEnum
    { /// <summary>
      /// Defines the Binary.
      /// </summary>
        Binary,
        /// <summary>
        /// Defines the Text.
        /// </summary>
        Text
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
                SponsorTypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    /// <summary>
    /// Defines the <see cref="SponsorTypeEnumConverter" />.
    /// </summary>
    internal class SponsorTypeEnumConverter : JsonConverter
    {
        /// <summary>
        /// The CanConvert.
        /// </summary>
        /// <param name="t">The t<see cref="Type"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool CanConvert(Type t)
        {
            return t == typeof(SponsorTypeEnum) || t == typeof(SponsorTypeEnum?);
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
                    return SponsorTypeEnum.Binary;
                case "text":
                    return SponsorTypeEnum.Text;
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
            SponsorTypeEnum value = (SponsorTypeEnum)untypedValue;
            switch (value)
            {
                case SponsorTypeEnum.Binary:
                    serializer.Serialize(writer, "binary");
                    return;
                case SponsorTypeEnum.Text:
                    serializer.Serialize(writer, "text");
                    return;
            }
            throw new Exception("Cannot marshal type SponsorTypeEnum");
        }

        /// <summary>
        /// Defines the Singleton.
        /// </summary>
        public static readonly SponsorTypeEnumConverter Singleton = new SponsorTypeEnumConverter();
    }
}
