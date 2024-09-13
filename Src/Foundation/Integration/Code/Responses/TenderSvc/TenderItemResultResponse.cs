using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.TenderSvc
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName= "GetTenderOpeningResult", Namespace = "", IsNullable = false)]
    public partial class TenderItemResultResponse
    {

        private byte responseCodeField;

        private string descriptionField;

        private GetTenderOpeningResultTable[] newDataSetField;

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
        public GetTenderOpeningResultTable[] NewDataSet
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
    public partial class GetTenderOpeningResultTable
    {

        private string oFFER_NUMBERField;

        private string tENDERER_NAMEField;

        private string tOTAL_AMOUNTField;

        private string dELIVERY_COMPLETIONField;

        private string bANK_GUARANTEEField;

        private string rEMARKSField;

        /// <remarks/>
        public string OFFER_NUMBER
        {
            get
            {
                return this.oFFER_NUMBERField;
            }
            set
            {
                this.oFFER_NUMBERField = value;
            }
        }

        /// <remarks/>
        public string TENDERER_NAME
        {
            get
            {
                return this.tENDERER_NAMEField;
            }
            set
            {
                this.tENDERER_NAMEField = value;
            }
        }

        /// <remarks/>
        public string TOTAL_AMOUNT
        {
            get
            {
                return this.tOTAL_AMOUNTField;
            }
            set
            {
                this.tOTAL_AMOUNTField = value;
            }
        }

        /// <remarks/>
        public string DELIVERY_COMPLETION
        {
            get
            {
                return this.dELIVERY_COMPLETIONField;
            }
            set
            {
                this.dELIVERY_COMPLETIONField = value;
            }
        }

        /// <remarks/>
        public string BANK_GUARANTEE
        {
            get
            {
                return this.bANK_GUARANTEEField;
            }
            set
            {
                this.bANK_GUARANTEEField = value;
            }
        }

        /// <remarks/>
        public string REMARKS
        {
            get
            {
                return this.rEMARKSField;
            }
            set
            {
                this.rEMARKSField = value;
            }
        }
    }


}
