using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DEWAXP.Foundation.Integration.Extensions
{
    internal static class XmlDocumentExtensions
    {
        internal static bool TryGetSingleNodeValue(this XmlDocument doc, string path, out string value)
        {
            value = null;
            if(doc != null)
            {
                var node = doc.SelectSingleNode(path);
                if (node != null)
                {
                    value = node.InnerText;
                }
            }
            return !string.IsNullOrEmpty(value);
        }
    }
}
