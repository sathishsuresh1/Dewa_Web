using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Extensions;

namespace DEWAXP.Foundation.Integration.Responses
{
    [XmlRoot(ElementName = "DMS_SearchResult")]
    public class PartnerDocumentSearch
    {
        [XmlElement(ElementName = "DMS_Row")]
        public List<DMS_Row> Documents { get; set; }

	    
	}

    [Serializable]
    public class DMS_Row
    {
        [XmlElement(ElementName = "object_name")]
        public string object_name { get; set; }

        [XmlElement(ElementName = "r_content_size")]
        public long r_content_size { get; set; }

        [XmlElement(ElementName = "r_creation_date")]
        public string r_creation_date { get; set; }

        [XmlElement(ElementName = "r_object_id")]
        public string r_object_id { get; set; }

        public string formattedcreationdate
        {
            get
            {
                if (string.IsNullOrEmpty(this.r_creation_date)) return string.Empty;
                if (this.r_creation_date[1] == '/')
                {
                    return "0" + this.r_creation_date;
                }
                return this.r_creation_date;
            }
        }

        public DateTime? CreatedDate
        {
            get { return this.formattedcreationdate.FormatStringIntoDate(); }
        }
        
    }

}
