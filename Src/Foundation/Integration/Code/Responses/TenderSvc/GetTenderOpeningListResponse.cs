using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.TenderSvc
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "GetTenderOpeningList", Namespace = "", IsNullable = false)]
    public partial class GetTenderOpeningListResponse
    {

        private byte responseCodeField;

        private string descriptionField;

        private GetTenderOpeningListTable[] newDataSetField;

        /// <remarks/>
        public byte ResponseCode
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
        [System.Xml.Serialization.XmlArrayItemAttribute("Table", IsNullable = false)]
        public GetTenderOpeningListTable[] NewDataSet
        {
            get
            {
                return this.newDataSetField;
            }
            set
            {
                this.newDataSetField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class GetTenderOpeningListTable
    {

        private string fIDField;

        private byte sINOField;

        private string tENDERNOField;

        private string dESCRIPTIONField;

        private decimal tENDERFEEField;

        private string fLOATINGDATEField;

        private string cLOSINGDATEField;

        private string tENDERSTATUSField;

        private string tENDERTYPEField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
        public string FID
        {
            get
            {
                return this.fIDField;
            }
            set
            {
                this.fIDField = value;
            }
        }

        /// <remarks/>
        public byte SINO
        {
            get
            {
                return this.sINOField;
            }
            set
            {
                this.sINOField = value;
            }
        }

        /// <remarks/>
        public string TENDERNO
        {
            get
            {
                return this.tENDERNOField;
            }
            set
            {
                this.tENDERNOField = value;
            }
        }

        /// <remarks/>
        public string DESCRIPTION
        {
            get
            {
                return this.dESCRIPTIONField;
            }
            set
            {
                this.dESCRIPTIONField = value;
            }
        }

        /// <remarks/>
        public decimal TENDERFEE
        {
            get
            {
                return this.tENDERFEEField;
            }
            set
            {
                this.tENDERFEEField = value;
            }
        }

        /// <remarks/>
        public string FLOATINGDATE
        {
            get
            {
                return this.fLOATINGDATEField;
            }
            set
            {
                this.fLOATINGDATEField = value;
            }
        }

        /// <remarks/>
        public string CLOSINGDATE
        {
            get
            {
                return this.cLOSINGDATEField;
            }
            set
            {
                this.cLOSINGDATEField = value;
            }
        }

        /// <remarks/>
        public string TENDERSTATUS
        {
            get
            {
                return this.tENDERSTATUSField;
            }
            set
            {
                this.tENDERSTATUSField = value;
            }
        }

        /// <remarks/>
        public string TENDERTYPE
        {
            get
            {
                return this.tENDERTYPEField;
            }
            set
            {
                this.tENDERTYPEField = value;
            }
        }
    }


}
