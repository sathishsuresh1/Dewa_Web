using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Exceptions;
using System.Xml.Schema;

namespace DEWAXP.Foundation.Integration.Extensions
{
    public static class StringExtensions
    {
        internal static T DeserializeAs<T>(this string xml)
            where T : class, new()
        {
            var encodedResponse = Encoding.UTF8.GetBytes(xml);

            using (var input = new MemoryStream(encodedResponse))
            {
                var serializer = new XmlSerializer(typeof(T));

                return ((T) serializer.Deserialize(input));
            }
        }

        internal static XmlSchema GetSchemaFromType(Type type)
        {
            var oReflectionImporter = new XmlReflectionImporter();
            var oXmlTypeMapping = oReflectionImporter.ImportTypeMapping(type);
            var oXmlSchemas = new XmlSchemas();
            var oXmlSchema = new XmlSchema();
            oXmlSchemas.Add(oXmlSchema);
            var oXMLSchemaExporter = new XmlSchemaExporter(oXmlSchemas);
            oXMLSchemaExporter.ExportTypeMapping(oXmlTypeMapping);
            return oXmlSchema;
        }

        internal static string AssertAccountNumberPrefix(this string accountNumber)
		{
			if (!string.IsNullOrWhiteSpace(accountNumber) && !accountNumber.StartsWith("00"))
			{
				return string.Concat("00", accountNumber);
			}
			return accountNumber;
		}
        public static DateTime? FormatStringIntoDate(this string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            List<string> formats = new List<string>
            {
                "MM/dd/yyyy hh:mm:ss tt",
                "M/dd/yyyy hh:mm:ss tt",
                "M/dd/yyyy h:mm:ss tt",
                "MM/dd/yyyy h:mm:ss tt",
            };
            DateTime @return;
            foreach (var myformat in formats)
            {
                if (DateTime.TryParseExact(value.Trim(), myformat, CultureInfo.InvariantCulture, DateTimeStyles.None, out @return))
                {
                    return @return;
                }
            }


            return null;
        }
	}
}
