using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DEWAXP.Foundation.Helpers
{
    public class QueryString : IEnumerable
    {
	    private readonly bool _allowEmptyValues;
	    private readonly IDictionary<string, string> _values;

        public QueryString(bool allowEmptyValues = true)
        {
	        _allowEmptyValues = allowEmptyValues;
	        _values = new Dictionary<string, string>();
        }

	    public IEnumerable<string> Keys
        {
            get { return _values.Keys; }
        }

        public string this[string key]
        {
            get
            {
                if (_values.ContainsKey(key))
                {
                    return _values[key];
                }
                return null;
            }
        }

        public bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }

	    public string CopyTo(string url)
	    {
		    if (!this.Keys.Any()) return url;

		    var final = new StringBuilder(url);
		    if (url.IndexOf("?") < 0)
		    {
			    final.Append('?');
		    }

		    var pairs = _values.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToArray();
		    var qsPairs = string.Join("&", pairs);

			return final.Append(qsPairs).ToString();
	    }

        public QueryString With(string key, object value, bool urlEncode = false)
        {
	        var s = value != null ? value.ToString() : string.Empty;
	        if (!string.IsNullOrWhiteSpace(s) || _allowEmptyValues)
	        {
				var safeKey = urlEncode ? HttpUtility.UrlEncode(key) : key;
				var safeValue = urlEncode ? HttpUtility.UrlEncode(s) : s;

				_values[safeKey] = safeValue;
			}
            return this;
        }

        public static QueryString Parse(string url)
        {
            if (url.IndexOf("?") > -1)
            {
                var qs = new QueryString();
                var qsPortion = HttpUtility.UrlDecode(url.Substring(url.IndexOf("?") + 1));
                var kvSplit = qsPortion.Split('&');

                foreach (var kv in kvSplit)
                {
                    if (kv.Contains('='))
                    {
                        var key = kv.Substring(0, kv.IndexOf("="));
                        var value = kv.Substring(kv.IndexOf("=") + 1);

                        qs = qs.With(key, value);
                    }
                }
                return qs;
            }
            return new QueryString();
        }

	    public string CombineWith(string url)
	    {
		    if (!string.IsNullOrWhiteSpace(url))
		    {
			    if (url.IndexOf('?') > -1)
			    {
				    return string.Concat(url, "&", this.ToString());
			    }
			    return string.Concat(url, "?", this.ToString());
		    }
		    return url;
	    }

        public override string ToString()
        {
            return string.Join("&", _values.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToArray());
        }

	    public IEnumerator GetEnumerator()
	    {
		    return _values.GetEnumerator();
	    }
    }
}