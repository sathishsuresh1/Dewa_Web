using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ChatBot.Models.DirectLine
{
    [Serializable]
    public class DirectLineValidationModel
    {
        public string ConversationId { get; set; }
        public string SessionId { get; set; }
        public string LoginType { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string LanguageCode { get; set; }
        public int MagicNumber { get; set; }

        public string ToJson(bool addConversationId)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);

            using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();

                writer.WritePropertyName("sessionId");
                writer.WriteValue(this.SessionId);
                writer.WritePropertyName("userName");
                writer.WriteValue(this.UserName);
                writer.WritePropertyName("loginType");
                writer.WriteValue(this.LoginType);
                writer.WritePropertyName("displayName");
                writer.WriteValue(this.DisplayName);
                writer.WritePropertyName("magicNumber");
                writer.WriteValue(this.MagicNumber);
                writer.WritePropertyName("languageCode");
                writer.WriteValue(this.LanguageCode);
                if(addConversationId)
                {
                    writer.WritePropertyName("conversationId");
                    writer.WriteValue(this.ConversationId);
                }
                //@"{'sessionId':'" + model.SessionId + "','userName':'" + model.UserName + "','loginType':'" +
                //model.LoginType + "','displayName':'" + model.DisplayName + "','magicNumber':'" + model.MagicNumber + "','languageCode':'" + model.LanguageCode + "'}");

                //@"{'sessionId':'" + model.SessionId + "','userName':'" + model.UserName + "','loginType':'" + 
                //model.LoginType + "','displayName':'" + model.DisplayName + "','magicNumber':'" + model.MagicNumber + "','languageCode':'" + model.LanguageCode + "','conversationId':'" + con.Id + "'}");
                writer.WriteEndObject();
            }
            return sb.ToString();
        }
    }
}