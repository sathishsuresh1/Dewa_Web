namespace DEWAXP.Feature.HappinessIndicator.Models.HappinessIndicator
{
    public class HappinessIndicatorModel
    {
        public HappinessIndicatorModel(string postUrl, string jsonPayload, string clientId, string signature, string lang, string timestamp, string random, string nonce)
        {
            Nonce = nonce;
            Random = random;
            Timestamp = timestamp;
            Lang = lang;
            Signature = signature;
            ClientId = clientId;
            JsonPayload = jsonPayload;
            PostUrl = postUrl;
        }

        public string PostUrl { get; private set; }
        public string JsonPayload { get; private set; }
        public string ClientId { get; private set; }
        public string Signature { get; private set; }
        public string Lang { get; private set; }

        public string Timestamp { get; private set; }
        public string Random { get; private set; }
        public string Nonce { get; private set; }
        public string ThemeColor { get; set; }
    }
}