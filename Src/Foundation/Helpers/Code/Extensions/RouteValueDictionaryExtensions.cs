using System.Collections;
using System.Web.Mvc;
using System.Web.Routing;

namespace DEWAXP.Foundation.Helpers.Extensions
{
    public static class RouteValueDictionaryExtensions
    {
        public static RouteValueDictionary WithComplexValues(this RouteValueDictionary routeValues)
        {
            var @return = new RouteValueDictionary();
            foreach (var kv in routeValues)
            {
                if (kv.Value is IEnumerable && !(kv.Value is string))
                {
                    var collection = kv.Value as IEnumerable;
                    var @enum = collection.GetEnumerator();
                    var i = 0;

                    while (@enum.MoveNext())
                    {
                        if (@enum.Current is string || @enum.Current.GetType().IsPrimitive || @enum.Current.GetType().IsValueType)
                        {
                            @return.Add(string.Format("{0}[{1}]", kv.Key, i), @enum.Current);
                        }
                        else
                        {
                            var props = @enum.Current.GetType().GetProperties();
                            foreach (var propInfo in props)
                            {
                                @return.Add(string.Format("{0}[{1}].{2}", kv.Key, i, propInfo.Name), propInfo.GetValue(@enum.Current));
                            }
                        }
                        i++;
                    }
                }
                else
                {
                    @return.Add(kv.Key, kv.Value);
                }
            }
            return @return;
        }

        public static RouteValueDictionary CustomerTypeIndicatesRequired(
                                    string  customerType,
                                    object htmlAttributes = null)
        {
            var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            if (customerType!="O")
                dictionary.Add("required", "");

            return dictionary;
        }
        public static RouteValueDictionary ConditionalEmiratesId(
                                    string customercategory,
                                    string customertype,
                                    object htmlAttributes = null)
        {
            var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            if (customercategory == "P" && (customertype == "T" || customertype == "O"))
                dictionary.Add("data-parsley-emiratesid", "");

            return dictionary;
        }
        public static RouteValueDictionary ConditionalDisabled(
                                    bool DetailsReadonly,
                                    object htmlAttributes = null)
        {
            var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            if (DetailsReadonly)
                dictionary.Add("readonly", "readonly");

            return dictionary;
        }
    }
}