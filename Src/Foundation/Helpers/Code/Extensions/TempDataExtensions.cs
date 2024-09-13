using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Helpers.Extensions
{
    public static class TempDataExtensions
    {
        public static T GetValueAs<T>(this TempDataDictionary dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                var value = dictionary[key];
                if (value is T)
                {
                    return (T) value;
                }
            }
            return default(T);
        }
    }
}