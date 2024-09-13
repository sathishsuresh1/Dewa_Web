using Newtonsoft.Json;
using System.IO;

namespace DEWAXP.Foundation.Helpers
{
    public static class CustomJsonConvertor
    {
        public static TResult Deserialize<TResult>(StreamReader responseStream)
        {
            using (var reader = new JsonTextReader(responseStream))
            {
                var serializer = new JsonSerializer
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include
                };

                return serializer.Deserialize<TResult>(reader);
            }
        }

        public static TResult DeserializeObject<TResult>(string strresponse)
        {
            using (var reader = new JsonTextReader(new StringReader(strresponse)))
            {
                var serializer = new JsonSerializer
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include
                };
                return serializer.Deserialize<TResult>(reader);
            }
        }
    }
}