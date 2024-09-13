using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace DEWAXP.Feature.Account.Models.UAEPass
{
    public partial class UAEPassInfo
    {
        [JsonProperty("homeAddressAreaCode", NullValueHandling = NullValueHandling.Ignore)]
        public string HomeAddressAreaCode { get; set; }

        [JsonProperty("homeAddressStreetAR", NullValueHandling = NullValueHandling.Ignore)]
        public string HomeAddressStreetAr { get; set; }

        [JsonProperty("sub", NullValueHandling = NullValueHandling.Ignore)]
        public string Sub { get; set; }

        [JsonProperty("passportNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string PassportNumber { get; set; }

        [JsonProperty("homeAddressTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        public string HomeAddressTypeCode { get; set; }

        [JsonProperty("maritalStatus", NullValueHandling = NullValueHandling.Ignore)]
        public string MaritalStatus { get; set; }

        [JsonProperty("idCardIssueDate", NullValueHandling = NullValueHandling.Ignore)]
        public string IdCardIssueDate { get; set; }

        [JsonProperty("userType", NullValueHandling = NullValueHandling.Ignore)]
        public string UserType { get; set; }

        [JsonProperty("homeAddressStreetEN", NullValueHandling = NullValueHandling.Ignore)]
        public string HomeAddressStreetEn { get; set; }

        [JsonProperty("fullnameAR", NullValueHandling = NullValueHandling.Ignore)]
        public string FullnameAr { get; set; }

        [JsonProperty("fullnameEN", NullValueHandling = NullValueHandling.Ignore)]
        public string FullnameEn { get; set; }

        [JsonProperty("homeAddressEmirateCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? HomeAddressEmirateCode { get; set; }

        [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
        public string Domain { get; set; }

        [JsonProperty("gender", NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; }

        [JsonProperty("homeAddressBuildingNameEN", NullValueHandling = NullValueHandling.Ignore)]
        public string HomeAddressBuildingNameEn { get; set; }

        [JsonProperty("homeAddressBuildingNameAR", NullValueHandling = NullValueHandling.Ignore)]
        public string HomeAddressBuildingNameAr { get; set; }

        [JsonProperty("lastnameEN", NullValueHandling = NullValueHandling.Ignore)]
        public string LastnameEn { get; set; }

        [JsonProperty("firstnameEN", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstnameEn { get; set; }

        [JsonProperty("nationalityAR", NullValueHandling = NullValueHandling.Ignore)]
        public string NationalityAr { get; set; }

        [JsonProperty("idn", NullValueHandling = NullValueHandling.Ignore)]
        public string Idn { get; set; }

        [JsonProperty("photo", NullValueHandling = NullValueHandling.Ignore)]
        public string Photo { get; set; }

        [JsonProperty("homeAddressCityCode", NullValueHandling = NullValueHandling.Ignore)]
        public string HomeAddressCityCode { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty("dob", NullValueHandling = NullValueHandling.Ignore)]
        public string Dob { get; set; }

        [JsonProperty("firstnameAR", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstnameAr { get; set; }

        [JsonProperty("nationalityEN", NullValueHandling = NullValueHandling.Ignore)]
        public string NationalityEn { get; set; }

        [JsonProperty("lastnameAR", NullValueHandling = NullValueHandling.Ignore)]
        public string LastnameAr { get; set; }

        [JsonProperty("uuid", NullValueHandling = NullValueHandling.Ignore)]
        public string Uuid { get; set; }

        [JsonProperty("idCardExpiryDate", NullValueHandling = NullValueHandling.Ignore)]
        public string IdCardExpiryDate { get; set; }

        [JsonProperty("residencyNumber", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? ResidencyNumber { get; set; }

        [JsonProperty("acr", NullValueHandling = NullValueHandling.Ignore)]
        public string Acr { get; set; }

        [JsonProperty("mobile", NullValueHandling = NullValueHandling.Ignore)]
        public string Mobile { get; set; }

        [JsonProperty("amr", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Amr { get; set; }
    }

    public partial class UAEPassInfo
    {
        public static UAEPassInfo FromJson(string json) => JsonConvert.DeserializeObject<UAEPassInfo>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this UAEPassInfo self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}