// <copyright file="CustomJsonConvertor.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Helpers
{
    using Newtonsoft.Json;
    using System.IO;

    /// <summary>
    /// Defines the <see cref="CustomJsonConvertor" />.
    /// </summary>
    public class CustomJsonConvertor
    {
        /// <summary>
        /// The Deserialize.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="responseStream">The responseStream<see cref="StreamReader"/>.</param>
        /// <returns>The <see cref="TResult"/>.</returns>
        public static TResult Deserialize<TResult>(StreamReader responseStream)
        {
            using (JsonTextReader reader = new JsonTextReader(responseStream))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include
                };

                return serializer.Deserialize<TResult>(reader);
            }
        }

        /// <summary>
        /// The DeserializeObject.
        /// </summary>
        /// <typeparam name="TResult">.</typeparam>
        /// <param name="strresponse">The strresponse<see cref="string"/>.</param>
        /// <returns>The <see cref="TResult"/>.</returns>
        public static TResult DeserializeObject<TResult>(string strresponse)
        {
            using (JsonTextReader reader = new JsonTextReader(new StringReader(strresponse)))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include
                };
                return serializer.Deserialize<TResult>(reader);
            }
        }
    }
}
