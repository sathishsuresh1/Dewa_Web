using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.VendorSvc
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class SRM_OpenInquiries
    {

        private string responseCodeField;
            
        private string descriptionField;

        private SRM_OpenInquiriesItem[] itemField;

        /// <remarks/>
        public string ResponseCode
        {
            get
            {
                return this.responseCodeField;
            }
            set
            {
                this.responseCodeField = value;
            }
        }

        /// <remarks/>
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Item")]
        public SRM_OpenInquiriesItem[] Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SRM_OpenInquiriesItem
    {

        private string sNOField;

        private uint objectIDField;

        private string objectDescField;

        private string floatDateField;

        private string clsDateField;

        /// <remarks/>
        public string SNO
        {
            get
            {
                return this.sNOField;
            }
            set
            {
                this.sNOField = value;
            }
        }

        /// <remarks/>
        public uint ObjectID
        {
            get
            {
                return this.objectIDField;
            }
            set
            {
                this.objectIDField = value;
            }
        }

        /// <remarks/>
        public string ObjectDesc
        {
            get
            {
                return this.objectDescField;
            }
            set
            {
                this.objectDescField = value;
            }
        }

        /// <remarks/>
        public string FloatDate
        {
            get
            {
                return this.floatDateField;
            }
            set
            {
                this.floatDateField = value;
            }
        }

        /// <remarks/>
        public string ClsDate
        {
            get
            {
                return this.clsDateField;
            }
            set
            {
                this.clsDateField = value;
            }
        }
    }


}
