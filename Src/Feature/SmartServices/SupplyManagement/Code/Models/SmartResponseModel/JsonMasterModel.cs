using DEWAXP.Foundation.Content.Models.SmartResponseModel;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Models.SmartResponseModel
{
    public partial class JsonMasterModel
    {
        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }

        //[JsonProperty("questions_track")]
        //public List<Question> QuestionsTrack { get; set; }
    }

    public partial class Answer : BaseInfo
    {
        [JsonProperty("type")]
        public TypeEnum Type { get; set; }

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        [JsonProperty("btntitle")]
        public string Btntitle { get; set; }

        [JsonProperty("btnvalue")]
        public string BtnValue { get; set; }

        [JsonProperty("btnvalue2")]
        public string BtnValue2 { get; set; }

        [JsonProperty("id")]
        public SM_Id Id { get; set; }

        //[JsonProperty("id")]
        public string SubmittedValue { get; set; }

        [JsonProperty("actiondata")]
        public string Actiondata { get; set; }

        [JsonProperty("action")]
        public SM_Action Action { get; set; }

        public bool? disabled { get; set; }

        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }

        public bool IsSelected { get; set; }
    }

    public partial class Question : BaseInfo
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("infotype")]
        public Infotype Infotype { get; set; }

        [JsonProperty("infotext")]
        public string Infotext { get; set; }

        [JsonProperty("code")]
        public SM_Code Code { get; set; }

        [JsonProperty("answers")]
        public List<Answer> Answers { get; set; }

        [JsonProperty("infoanswer")]
        public Answer InfoAnswer { get; set; }

        [JsonProperty("id")]
        public SM_Id Id { get; set; }
    }

    public enum SM_Action
    {
        Empty, Call, Checklogin, Getotpmobile, Resendotpmobile,
        Submit, Track, UploadMedia, Verifyotp, SubmitRequestFire,
        BillPayment, SubmitRequestSpark, SubmitRequestSmoke, SubmitRequestEfluctuation, NoPower,
        CheckMeterStatus, SubMeterCheck, Paybill, Accountcountcheck, Customerlogin,
        Checkpod, NopowerPod, SubmetercheckPod, GetLocation, GetNotificationList, GuestGetNotificationList,
        ReportTechnicalIncident, TrackAnotherIncident, VerifyAccount, NoWater, NoWaterPOD, ShowInterruption, UploadMedia_ssd, GETMAINTENANCEPROVIDERS
    };

    public enum SM_Actiondata
    { Camera, Empty, Gallery };

    public enum SM_Id
    {
        Empty, Location, Mobile, Account, Success, Media, Error, ContactPersonName,
        More, Otp,

        //---------------------------------------------------
        //operation Field
        /// <summary>
        /// login = 1, Guest = 2
        /// </summary>
        SessionType,

        /// <summary>
        /// Complaint Id
        /// </summary>
        ComplaintId,

        /// <summary>
        /// Customer Type to save "Myself, Someone, Public " -
        /// </summary>
        customertype,

        customercategory,

        /// <summary>
        ///  Parent Incident Type
        /// </summary>
        codegroup,

        /// <summary>
        ///
        /// </summary>
        code,

        /// <summary>
        ///
        /// </summary>
        trcodegroup,

        /// <summary>
        ///
        /// </summary>
        trcode,

        /// <summary>
        /// gps
        /// </summary>
        Latitude,

        /// <summary>
        /// gps
        /// </summary>
        Longitude,

        /// <summary>
        /// 13131
        /// </summary>
        OtpCount,

        ElectricityMeterType,
        ElectricityIsSmartMeter,
        WaterMeterType,
        WaterIsSmartMeter,

        Street,
        CA_Location,
        SearchTxt,
        NotificationNumber,
        WaterMeterNo,
        CheckSmartMeter,
        ElectricityMeterNo,
        Image1,
        Image2,
        Notification,
        LangType,
        UsernName,
        IsUserLoggedIn,
        UploadSSD,
        Image1Preview,
        LANG,
        PartnerId,
        SmartMeterLoader
    };

    public enum SM_Placeholder
    { ContactPersonMobile, ContactnumberLocation, ContactpersonContactnumberLocation, Empty, EnterLocation, EnterTextOrRecord, PlaceholderContactpersonContactnumberLocation };

    public enum TypeEnum
    { Empty, Accountselection, Button, Confirmation, Textinput, Notes, Loading, Showlist, Notification, maintenanceproviders, PercentageLoader };

    public enum SM_Code
    { Empty, Success, The000, The001, The002, The003, The004, The005, Getotpmobile, Pod, CRM };

    public enum SM_SessionType
    { IsLoggedIn, IsGuest }

    public enum Infotype
    { Empty, WarningR, WarningO, WarningAction };

    public enum ErrorType
    {
        AccountError,
    }

    

    public enum SmScreenCode
    {
        /// <summary>
        /// by Default
        /// </summary>
        d = 0,

        /// <summary>
        /// anonymous notification tracking
        /// </summary>
        ant = 1
    }

    public partial class JsonMasterModel
    {
        public static JsonMasterModel FromJson(string json) => JsonConvert.DeserializeObject<JsonMasterModel>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this JsonMasterModel self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ActionConverter.Singleton,
                ActiondataConverter.Singleton,
                IdConverter.Singleton,
                PlaceholderConverter.Singleton,
                TypeEnumConverter.Singleton,
                CodeConverter.Singleton,
                InfotypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ActionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SM_Action) || t == typeof(SM_Action?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "CALL":
                        return SM_Action.Call;

                    case "CHECKLOGIN":
                        return SM_Action.Checklogin;

                    case "GETOTPMOBILE":
                        return SM_Action.Getotpmobile;

                    case "RESENDOTPMOBILE":
                        return SM_Action.Resendotpmobile;

                    case "SUBMIT":
                        return SM_Action.Submit;

                    case "TRACK":
                        return SM_Action.Track;

                    case "UPLOAD_MEDIA":
                        return SM_Action.UploadMedia;

                    case "VERIFYOTP":
                        return SM_Action.Verifyotp;

                    case "SUBMITREQUEST_FIRE":
                        return SM_Action.SubmitRequestFire;

                    case "BILLPAYMENT":
                        return SM_Action.BillPayment;

                    case "SUBMITREQUEST_SPARK":
                        return SM_Action.SubmitRequestSpark;

                    case "SUBMITREQUEST_SMOKE":
                        return SM_Action.SubmitRequestSmoke;

                    case "SUBMITREQUEST_EFLUCTUATION":
                        return SM_Action.SubmitRequestEfluctuation;

                    case "NOPOWER":
                        return SM_Action.NoPower;

                    case "CHECKMETERSTATUS":
                        return SM_Action.CheckMeterStatus;

                    case "SUBMETERCHECK":
                        return SM_Action.SubMeterCheck;

                    case "PAYBILL":
                        return SM_Action.Paybill;

                    case "ACCOUNTCOUNTCHECK":
                        return SM_Action.Accountcountcheck;

                    case "CUSTOMERLOGIN":
                        return SM_Action.Customerlogin;

                    case "SUBMETERCHECK+POD":
                        return SM_Action.SubmetercheckPod;

                    case "NOPOWER+POD":
                        return SM_Action.NopowerPod;

                    case "CHECKPOD":
                        return SM_Action.Checkpod;

                    case "GETLOCATION":
                        return SM_Action.GetLocation;

                    case "GETNOTIFICATIONLIST":
                        return SM_Action.GetNotificationList;

                    case "GUESTGETNOTIFICATIONLIST":
                        return SM_Action.GuestGetNotificationList;

                    case "REPORTTECHNICALINCIDENT":
                        return SM_Action.ReportTechnicalIncident;

                    case "TRACKANOTHERINCIDENT":
                        return SM_Action.TrackAnotherIncident;

                    case "VERIFYACCOUNT":
                        return SM_Action.VerifyAccount;
                    //"NOWATER+POD"
                    case "NOWATER+POD":
                        return SM_Action.NoWaterPOD;

                    case "NOWATER":
                        return SM_Action.NoWater;

                    case "SHOWINTERRUPTION":
                        return SM_Action.ShowInterruption;

                    case "UPLOAD_MEDIA_SSD":
                        return SM_Action.UploadMedia_ssd;

                    case "GETMAINTENANCEPROVIDERS":
                        return SM_Action.GETMAINTENANCEPROVIDERS;

                    case "":
                    default:
                        return SM_Action.Empty;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return SM_Action.Empty;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            try
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (SM_Action)untypedValue;
                switch (value)
                {
                    case SM_Action.Call:
                        serializer.Serialize(writer, "CALL");
                        return;

                    case SM_Action.Checklogin:
                        serializer.Serialize(writer, "CHECKLOGIN");
                        return;

                    case SM_Action.Getotpmobile:
                        serializer.Serialize(writer, "GETOTPMOBILE");
                        return;

                    case SM_Action.Resendotpmobile:
                        serializer.Serialize(writer, "RESENDOTPMOBILE");
                        return;

                    case SM_Action.Submit:
                        serializer.Serialize(writer, "SUBMIT");
                        return;

                    case SM_Action.Track:
                        serializer.Serialize(writer, "TRACK");
                        return;

                    case SM_Action.UploadMedia:
                        serializer.Serialize(writer, "UPLOAD_MEDIA");
                        return;

                    case SM_Action.Verifyotp:
                        serializer.Serialize(writer, "VERIFYOTP");
                        return;

                    case SM_Action.SubmitRequestFire:
                        serializer.Serialize(writer, "SUBMITREQUEST_FIRE");
                        return;

                    case SM_Action.BillPayment:
                        serializer.Serialize(writer, "BILLPAYMENT");
                        return;

                    case SM_Action.SubmitRequestSpark:
                        serializer.Serialize(writer, "SUBMITREQUEST_SPARK");
                        return;

                    case SM_Action.SubmitRequestSmoke:
                        serializer.Serialize(writer, "SUBMITREQUEST_SMOKE");
                        return;

                    case SM_Action.SubmitRequestEfluctuation:
                        serializer.Serialize(writer, "SUBMITREQUEST_EFLUCTUATION");
                        return;

                    case SM_Action.NoPower:
                        serializer.Serialize(writer, "NOPOWER");
                        return;

                    case SM_Action.CheckMeterStatus:
                        serializer.Serialize(writer, "CHECKMETERSTATUS");
                        return;

                    case SM_Action.SubMeterCheck:
                        serializer.Serialize(writer, "SUBMETERCHECK");
                        return;

                    case SM_Action.Paybill:
                        serializer.Serialize(writer, "PAYBILL");
                        return;

                    case SM_Action.Accountcountcheck:
                        serializer.Serialize(writer, "ACCOUNTCOUNTCHECK");
                        return;

                    case SM_Action.Customerlogin:
                        serializer.Serialize(writer, "CUSTOMERLOGIN");
                        return;

                    case SM_Action.SubmetercheckPod:
                        serializer.Serialize(writer, "SUBMETERCHECK+POD");
                        return;

                    case SM_Action.NopowerPod:
                        serializer.Serialize(writer, "NOPOWER+POD");
                        return;

                    case SM_Action.Checkpod:
                        serializer.Serialize(writer, "CHECKPOD");
                        return;

                    case SM_Action.GetLocation:
                        serializer.Serialize(writer, "GETLOCATION");
                        return;

                    case SM_Action.GetNotificationList:
                        serializer.Serialize(writer, "GETNOTIFICATIONLIST");
                        return;

                    case SM_Action.GuestGetNotificationList:
                        serializer.Serialize(writer, "GUESTGETNOTIFICATIONLIST");
                        return;

                    case SM_Action.ReportTechnicalIncident:
                        serializer.Serialize(writer, "REPORTTECHNICALINCIDENT");
                        return;

                    case SM_Action.TrackAnotherIncident:
                        serializer.Serialize(writer, "TRACKANOTHERINCIDENT");
                        return;

                    case SM_Action.VerifyAccount:
                        serializer.Serialize(writer, "VERIFYACCOUNT");
                        return;

                    case SM_Action.NoWaterPOD:
                        serializer.Serialize(writer, "NOWATER+POD");
                        return;

                    case SM_Action.NoWater:
                        serializer.Serialize(writer, "NOWATER");
                        return;

                    case SM_Action.ShowInterruption:
                        serializer.Serialize(writer, "SHOWINTERRUPTION");
                        return;

                    case SM_Action.UploadMedia_ssd:
                        serializer.Serialize(writer, "UPLOAD_MEDIA_SSD");
                        return;

                    case SM_Action.GETMAINTENANCEPROVIDERS:
                        serializer.Serialize(writer, "GETMAINTENANCEPROVIDERS");
                        return;

                    case SM_Action.Empty:
                    default:
                        serializer.Serialize(writer, "");
                        return;
                        //
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            serializer.Serialize(writer, "");
        }

        public static readonly ActionConverter Singleton = new ActionConverter();
    }

    internal class ActiondataConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SM_Actiondata) || t == typeof(SM_Actiondata?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "CAMERA":
                        return SM_Actiondata.Camera;

                    case "GALLERY":
                        return SM_Actiondata.Gallery;

                    case "":
                    default:
                        return SM_Actiondata.Empty;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return SM_Actiondata.Empty;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            try
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (SM_Actiondata)untypedValue;
                switch (value)
                {
                    case SM_Actiondata.Camera:
                        serializer.Serialize(writer, "CAMERA");
                        return;

                    case SM_Actiondata.Gallery:
                        serializer.Serialize(writer, "GALLERY");
                        return;

                    case SM_Actiondata.Empty:
                    default:
                        serializer.Serialize(writer, "");
                        return;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            serializer.Serialize(writer, "");
        }

        public static readonly ActiondataConverter Singleton = new ActiondataConverter();
    }

    internal class IdConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SM_Id) || t == typeof(SM_Id?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "location":
                        return SM_Id.Location;

                    case "mobile":
                        return SM_Id.Mobile;

                    case "account":
                        return SM_Id.Account;

                    case "SUCCESS":
                        return SM_Id.Success;

                    case "MEDIA":
                        return SM_Id.Media;

                    case "customertype":
                        return SM_Id.customertype;

                    case "customercategory":
                        return SM_Id.customercategory;

                    case "codegroup":
                        return SM_Id.codegroup;

                    case "code":
                        return SM_Id.code;

                    case "trcode":
                        return SM_Id.trcode;

                    case "more":
                        return SM_Id.More;

                    case "otp":
                        return SM_Id.Otp;

                    case "searchtxt":
                        return SM_Id.SearchTxt;

                    case "notificationnumber":
                        return SM_Id.NotificationNumber;

                    case "trcodegroup":
                        return SM_Id.trcodegroup;

                    case "image1":
                        return SM_Id.Image1;

                    case "image2":
                        return SM_Id.Image2;

                    case "NOTIFICATION":
                        return SM_Id.Notification;

                    case "LANG":
                        return SM_Id.LANG;

                    case "":
                    default:
                        return SM_Id.Empty;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return SM_Id.Empty;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            try
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (SM_Id)untypedValue;
                switch (value)
                {
                    case SM_Id.Empty:
                        serializer.Serialize(writer, "");
                        return;

                    case SM_Id.Location:
                        serializer.Serialize(writer, "location");
                        return;

                    case SM_Id.Mobile:
                        serializer.Serialize(writer, "mobile");
                        return;

                    case SM_Id.Account:
                        serializer.Serialize(writer, "");
                        return;

                    case SM_Id.Success:
                        serializer.Serialize(writer, "SUCCESS");
                        return;

                    case SM_Id.Media:
                        serializer.Serialize(writer, "MEDIA");
                        return;

                    case SM_Id.customertype:
                        serializer.Serialize(writer, "customertype");
                        return;

                    case SM_Id.customercategory:
                        serializer.Serialize(writer, "customercategory");
                        return;

                    case SM_Id.codegroup:
                        serializer.Serialize(writer, "codegroup");
                        return;

                    case SM_Id.code:
                        serializer.Serialize(writer, "code");
                        return;

                    case SM_Id.trcode:
                        serializer.Serialize(writer, "ACTION_TYPE");
                        return;

                    case SM_Id.trcodegroup:
                        serializer.Serialize(writer, "PARENT_ACTION_TYPE");
                        return;

                    case SM_Id.More:
                        serializer.Serialize(writer, "more");
                        return;

                    case SM_Id.Otp:
                        serializer.Serialize(writer, "otp");
                        return;

                    case SM_Id.SearchTxt:
                        serializer.Serialize(writer, "searchtxt");
                        return;

                    case SM_Id.NotificationNumber:
                        serializer.Serialize(writer, "notificationnumber");
                        return;

                    case SM_Id.Image1:
                        serializer.Serialize(writer, "image1");
                        return;

                    case SM_Id.Image2:
                        serializer.Serialize(writer, "image2");
                        return;

                    case SM_Id.Notification:
                        serializer.Serialize(writer, "NOTIFICATION");
                        return;

                    case SM_Id.LANG:
                        serializer.Serialize(writer, "LANG");
                        return;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            serializer.Serialize(writer, "");
        }

        public static readonly IdConverter Singleton = new IdConverter();
    }

    internal class PlaceholderConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SM_Placeholder) || t == typeof(SM_Placeholder?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "Contact person mobile":
                        return SM_Placeholder.ContactPersonMobile;

                    case "Enter location":
                        return SM_Placeholder.EnterLocation;

                    case "Enter text or record":
                        return SM_Placeholder.EnterTextOrRecord;

                    case "contactnumber,location":
                        return SM_Placeholder.ContactnumberLocation;

                    case "contactperson, contactnumber,location":
                        return SM_Placeholder.PlaceholderContactpersonContactnumberLocation;

                    case "contactperson,contactnumber,location":
                        return SM_Placeholder.ContactpersonContactnumberLocation;

                    case "":
                    default:
                        return SM_Placeholder.Empty;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return SM_Placeholder.Empty;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            try
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (SM_Placeholder)untypedValue;
                switch (value)
                {
                    case SM_Placeholder.ContactPersonMobile:
                        serializer.Serialize(writer, "Contact person mobile");
                        return;

                    case SM_Placeholder.EnterLocation:
                        serializer.Serialize(writer, "Enter location");
                        return;

                    case SM_Placeholder.EnterTextOrRecord:
                        serializer.Serialize(writer, "Enter text or record");
                        return;

                    case SM_Placeholder.ContactnumberLocation:
                        serializer.Serialize(writer, "contactnumber,location");
                        return;

                    case SM_Placeholder.PlaceholderContactpersonContactnumberLocation:
                        serializer.Serialize(writer, "contactperson, contactnumber,location");
                        return;

                    case SM_Placeholder.ContactpersonContactnumberLocation:
                        serializer.Serialize(writer, "contactperson,contactnumber,location");
                        return;

                    case SM_Placeholder.Empty:
                    default:
                        serializer.Serialize(writer, "");
                        return;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            serializer.Serialize(writer, "");
        }

        public static readonly PlaceholderConverter Singleton = new PlaceholderConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "accountselection":
                        return TypeEnum.Accountselection;

                    case "button":
                        return TypeEnum.Button;

                    case "confirmation":
                        return TypeEnum.Confirmation;

                    case "textinput":
                        return TypeEnum.Textinput;

                    case "notes":
                        return TypeEnum.Notes;

                    case "showlist":
                        return TypeEnum.Showlist;

                    case "loading":
                        return TypeEnum.Loading;

                    case "percentloader":
                        return TypeEnum.PercentageLoader;

                    case "notification":
                        return TypeEnum.Notification;

                    case "maintenanceproviders":
                        return TypeEnum.maintenanceproviders;

                    case "":
                    default:
                        return TypeEnum.Empty;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return TypeEnum.Empty;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            try
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (TypeEnum)untypedValue;
                switch (value)
                {
                    case TypeEnum.Accountselection:
                        serializer.Serialize(writer, "accountselection");
                        return;

                    case TypeEnum.Button:
                        serializer.Serialize(writer, "button");
                        return;

                    case TypeEnum.Confirmation:
                        serializer.Serialize(writer, "confirmation");
                        return;

                    case TypeEnum.Textinput:
                        serializer.Serialize(writer, "textinput");
                        return;

                    case TypeEnum.Notes:
                        serializer.Serialize(writer, "notes");
                        return;

                    case TypeEnum.Showlist:
                        serializer.Serialize(writer, "showlist");
                        return;

                    case TypeEnum.Loading:
                        serializer.Serialize(writer, "loading");
                        return;

                    case TypeEnum.PercentageLoader:
                        serializer.Serialize(writer, "percentloader");
                        return;

                    case TypeEnum.Notification:
                        serializer.Serialize(writer, "notification");
                        return;

                    case TypeEnum.maintenanceproviders:
                        serializer.Serialize(writer, "maintenanceproviders");
                        return;

                    case TypeEnum.Empty:
                    default:
                        serializer.Serialize(writer, "");
                        return;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            serializer.Serialize(writer, "");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }

    internal class CodeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SM_Code) || t == typeof(SM_Code?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "000":
                        return SM_Code.The000;

                    case "001":
                        return SM_Code.The001;

                    case "002":
                        return SM_Code.The002;

                    case "003":
                        return SM_Code.The003;

                    case "004":
                        return SM_Code.The004;

                    case "005":
                        return SM_Code.The005;

                    case "SUCCESS":
                        return SM_Code.Success;

                    case "GETOTPMOBILE":
                        return SM_Code.Getotpmobile;

                    case "POD":
                        return SM_Code.Pod;

                    case "CRM":
                        return SM_Code.CRM;

                    case "":
                    default:
                        return SM_Code.Empty;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return SM_Code.Empty;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            try
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (SM_Code)untypedValue;
                switch (value)
                {
                    case SM_Code.Empty:
                        serializer.Serialize(writer, "");
                        return;

                    case SM_Code.The000:
                        serializer.Serialize(writer, "000");
                        return;

                    case SM_Code.The001:
                        serializer.Serialize(writer, "001");
                        return;

                    case SM_Code.The002:
                        serializer.Serialize(writer, "002");
                        return;

                    case SM_Code.The003:
                        serializer.Serialize(writer, "003");
                        return;

                    case SM_Code.The004:
                        serializer.Serialize(writer, "004");
                        return;

                    case SM_Code.The005:
                        serializer.Serialize(writer, "005");
                        return;

                    case SM_Code.Success:
                        serializer.Serialize(writer, "SUCCESS");
                        return;

                    case SM_Code.Getotpmobile:
                        serializer.Serialize(writer, "GETOTPMOBILE");
                        return;

                    case SM_Code.Pod:
                        serializer.Serialize(writer, "POD");
                        return;

                    case SM_Code.CRM:
                        serializer.Serialize(writer, "CRM");
                        return;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            serializer.Serialize(writer, "");
        }

        public static readonly CodeConverter Singleton = new CodeConverter();
    }

    internal class InfotypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Infotype) || t == typeof(Infotype?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "warning_r":
                        return Infotype.WarningR;

                    case "warning_o":
                        return Infotype.WarningO;

                    case "warning_action":
                        return Infotype.WarningAction;

                    case "":
                    default:
                        return Infotype.Empty;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return Infotype.Empty;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            try
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (Infotype)untypedValue;
                switch (value)
                {
                    case Infotype.Empty:
                        serializer.Serialize(writer, "");
                        return;

                    case Infotype.WarningR:
                        serializer.Serialize(writer, "warning_r");
                        return;

                    case Infotype.WarningO:
                        serializer.Serialize(writer, "warning_o");
                        return;

                    case Infotype.WarningAction:
                        serializer.Serialize(writer, "warning_action");
                        return;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            serializer.Serialize(writer, "");
        }

        public static readonly InfotypeConverter Singleton = new InfotypeConverter();
    }

    #region MyRegion

    public partial class BaseInfo
    {
        public int TrackingId { get; set; }

        public int ParentTrackingId { get; set; }
    }

    public partial class CommonRender : BaseInfo
    {
        public bool IsQuestion { get; set; }
        public Question Question { get; set; }

        public Answer Answer { get; set; }

        public int PreviousAnsTrackingId { get; set; }

        public CommonRenderRequest CurrentRequest { get; set; }

        public string RedirectUrl { get; set; }
        public int RedirectCount { get; set; }

        public RequestConfirmationDetail RequestConfirmationDetail { get; set; }
        public bool IsError { get; set; }
        public List<SM_ErrorDetail> ErrorDetails { get; set; }

        public DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetailsResponse SRCompanyDetails { get; set; }

        public SM_LoaderDetails LoaderDetails { get; set; }

        public int f_bckid { get; set; }
    }

    public class SM_ErrorDetail
    {
        public SM_Id ControlId { get; set; }
        public string ErorrMessage { get; set; }
    }

    public partial class CommonRenderRequest
    {
        /// <summary>
        /// Tacking ID of Node Or Question
        /// </summary>
        public int TckId { get; set; }

        /// <summary>
        /// To check Is Loaded from Start & without step session.
        /// </summary>
        public bool IsStart { get; set; }

        public int QusId { get; set; }
        public int AnsId { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public string AnswerValue { get; set; }

        /// <summary>
        /// to capture all the selected value by user
        /// </summary>
        public List<SmartDataValue> SelectedValues { get; set; }

        /// <summary>
        /// if back button is click
        /// </summary>
        public bool IsPageBack { get; set; }

        /// <summary>
        /// TO add City List
        /// </summary>
        public IEnumerable<SelectListItem> DdlList { get; set; }

        /// <summary>
        /// if page is refresh
        /// </summary>
        public bool IsPageRefesh { get; set; }

        /// <summary>
        /// to filter data on Code
        /// </summary>
        public SM_Code FilterCode { get; set; }

        public SmlangCode? LangType { get; set; }
        public bool IsAnsAltered { get; set; }

        public string SubmittedIds { get; set; }
    }

    public partial class TrackingRequest
    {
        public string SearchText { get; set; }
        public SM_Id Id { get; set; }
        public SM_Action Action { get; set; }
    }

    public partial class TrackingResponse
    {
        public TrackingRequest TrackingRequest { get; set; }
        public List<DEWAXP.Foundation.Integration.DewaSvc.trackNotificationDetails> TrackNotificationDetails { get; set; }
    }

    public class RequestConfirmationDetail
    {
        public string ContactAccountNo { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string ContactLocation { get; set; }
    }

    public class SmartDataValue
    {
        public SM_Id Type { get; set; }
        public string Value { get; set; }
    }

    public class SM_FileUplploadRequest
    {
        public HttpPostedFileBase Uploadfile { get; set; }
        public SM_Id ImageType { get; set; }
    }

    public class CommonConst
    {
        public const string NOOUTAGE = "NOOUTAGE";
        public const string UNPLANNED = "UNPLANNED";
        public const string PLANNED = "PLANNED";

        #region [userType]

        public const string SmUserTypeMyself = "CO01";
        public const string SmUserTypeSomeone = "CO02";
        public const string SmUserTypePublic = "CO03";

        #endregion [userType]

        #region [Code]

        #region [parentIncidentType]

        public const string PARENT_INCIDENT_ELECTRICITY = "Y0000002";
        public const string PARENT_INCIDENT_WATER = "Y0000003";
        public const string PARENT_INCIDENT_EV_GREEN_CHARGER = "EGC"; //TODO:SM- : NEEDED EGC CODE

        #endregion [parentIncidentType]

        #region [IncidentType]

        public const string INCIDENT_NOWATER = "D001";
        public const string INCIDENT_LOW_PRESSURE_OF_WATER = "D002";
        public const string INCIDENT_SMELLY_WATER_MUDDY_WATER = "D006";
        public const string INCIDENT_WATER_LEAKAGE = "D015";

        public const string INCIDENT_PUBLIC_ELECTRICAL_HAZARD = "D024";
        public const string INCIDENT_FLOODED_ROAD_BROKEN_WATER_PIPE = "D016";
        public const string INCIDENT_TILE_COLLAPSE = "D017";

        public const string INCIDENT_FIRE = "D007";
        public const string INCIDENT_NOPOWER_POD = "D001";
        public const string INCIDENT_SPARK_SHOCK_SMOKE_SMELL = "D023";
        public const string INCIDENT_ELECTRICITY_FLUCTUATION = "D003";

        #endregion [IncidentType]

        #endregion [Code]

        #region [ElementAttributeData]

        public const string ErrorMessage = "ErrorMessage";
        public const string ElementErrorContainerId = "ElementErrorContainerId";
        public const string ElementId = "ElementId";
        public const string ValidationRule = "ValidationRule";

        #endregion [ElementAttributeData]

        #region [Water n Electricity]

        public const string IsStandAloneMeter = "00";
        public const string IsMainMeter = "03";
        public const string IsSubMeter = "04";

        #endregion [Water n Electricity]

        #region [CustomerCategories]

        public const string CusSubTyp_POD = "CO01";
        public const string CusSubTyp_SpecialNeeds = "CO02";
        public const string CusSubTyp_ElderPeople = "CO03";
        public const string CusSubTyp_Medicalsituations = "CO04";
        public const string CusSubTyp_VIP = "CO05";
        public const string CusSubTyp_Normal = "CO06";

        #endregion [CustomerCategories]

        //public const string TRCODE_ElectricityFluctuation_ElectricityBox = "DP01";

        #region [TRCodeGroup]

        /// <summary>
        /// Part of Premise has no Power[DTMCDP02]
        /// </summary>
        public const string TRCODE_GP_AllPrm_NP_SbMtr = "DTMCDP01";

        /// <summary>
        ///  Part of Premise has no Power[DTMCDP02]
        /// </summary>
        public const string TRCODE_GP_POP_NP = "DTMCDP02";

        /// <summary>
        /// Part of Premise has no Power-Self Diagns [DTMCDP03]
        /// </summary>
        public const string TRCODE_GP_POP_NP_SD = "DTMCDP03";

        /// <summary>
        /// Spark/Shock/Smoke/Smell[DTMCDP04]
        /// </summary>
        public const string TRCODE_GP_SparkShockSmokeSmell = "DTMCDP04";

        /// <summary>
        ///  Electricity Fluctuation[DTMCDP05]
        /// </summary>
        public const string TRCODE_GP_Electricityluctuation = "DTMCDP05";

        #endregion [TRCodeGroup]

        #region [TR CODE]

        #region All Premises has no power-Sub meter No[DTMCDP01]

        public const string TRCODE_AllPrm_NP_SbMtr_Yes = "DP01";
        public const string TRCODE_AllPrm_NP_SbMtr__No = "DP02";

        #endregion All Premises has no power-Sub meter No[DTMCDP01]

        #region Part of Premise has no Power[DTMCDP02]

        public const string TRCODE_POP_NP_DEWATeamOnsite = "DP01";

        #endregion Part of Premise has no Power[DTMCDP02]

        #region Part of Premise has no Power-Self Diagns [DTMCDP03]

        public const string TRCODE_POP_NP_SD_Yes = "DP01";
        public const string TRCODE_POP_NP_SD__No = "DP02";

        #endregion Part of Premise has no Power-Self Diagns [DTMCDP03]

        #region Spark/Shock/Smoke/Smell[DTMCDP04]

        public const string TRCODE_SparkShockSmokeSmell_ElectricityBox = "DP01";
        public const string TRCODE_SparkShockSmokeSmell_ElectricityMeter = "DP02";
        public const string TRCODE_SparkShockSmokeSmell_Others = "DP03";

        #endregion Spark/Shock/Smoke/Smell[DTMCDP04]

        #region Electricity Fluctuation[DTMCDP05]

        public const string TRCODE_Electricityluctuation_Yes = "DP04";
        public const string TRCODE_Electricityluctuation_No = "DP05";

        #endregion Electricity Fluctuation[DTMCDP05]

        #endregion [TR CODE]
    }

    public class CustomerSubmittedData
    {
        public string Location { get; set; }
        public string Mobile { get; set; }
        public string ContractAccountNo { get; set; }
        public string Success { get; set; }
        public string Media { get; set; }
        public string ContactPersonName { get; set; }
        public string MoreDescription { get; set; }
        public string MobileOtp { get; set; }
        public string SessionType { get; set; }
        public string ComplaintId { get; set; }
        public string CustomerType { get; set; }
        public string CustomerCategory { get; set; }

        /// <summary>
        /// codegroup
        /// </summary>
        public string CodeGroup { get; set; }

        /// <summary>
        /// code trcode
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// trcodegroup
        /// </summary>
        public string TrCodeGroup { get; set; }

        /// <summary>
        /// trcode
        /// </summary>
        public string TrCode { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string OtpCount { get; set; }
        public string ElectricityMeterType { get; set; }
        public string ElectricityIsSmartMeter { get; set; }
        public string ElectricityMeterNo { get; internal set; }
        public string WaterMeterType { get; set; }
        public string WaterIsSmartMeter { get; set; }
        public string WaterMeterNo { get; internal set; }
        public string Street { get; set; }
        public string CA_Location { get; set; }
        public bool CheckSmartMeter { get; internal set; }
        public string NotificationNumber { get; internal set; }
        public string Image1 { get; set; }
        public string Image1filePath { get; set; }
        public string Image2 { get; set; }
        public string Image2filePath { get; set; }
        public string More { get; set; }
        public SmlangCode LangType { get; internal set; }
        public bool IsUserLogined { get; set; }
        public string Image1Preview { get; set; }
        public string PartnerNo { get; set; }
        public string SmartMeterLoader { get; set; }
    }

    public class SM_LoaderDetails
    {
        public string Voltagerequest { get; set; }
        public string strretry { get; set; }
        public string Status { get; set; }

        public int TotalCount
        {
            get
            {
                int i;
                if (!string.IsNullOrWhiteSpace(strretry) && Int32.TryParse(strretry, out i))
                {
                    return i + 1;
                }
                return 0;
            }
        }

        public int RetryCount { get; set; }
    }


    public class SubmitComplaintReponse
    {
        public string complaintId { get; set; }
        public string notif { get; set; }
        public string description { get; set; }
        public string code { get; set; }
    }
    #endregion MyRegion
}