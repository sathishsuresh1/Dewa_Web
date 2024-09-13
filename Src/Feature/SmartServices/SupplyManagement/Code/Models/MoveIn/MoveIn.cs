using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveIn
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class MoveIn
    {

        private byte responseCodeField;

        private string descriptionField;

        private decimal sDAMTField;

        private decimal rCAMTField;

        private decimal aCAMTField;

        private byte sUBRCField;

        private ushort tRANIDField;

        private byte sUBIDField;

        private object pARTNERField;

        private decimal oUTSTNDField;

        private object eTCUSTYPField;

        private object eTACCTCLASSField;

        private object eTPREMISEField;

        private object eTIDTYPField;

        private object eTIDNUMBERField;

        private string eTIDEXPField;

        private object eTFNAMEField;

        private object eTLNAMEField;

        private object eTPOBOXField;

        private object eTNATIOField;

        private object eTMOBILEField;

        private object eTEMAILField;

        private string eTTSDATEField;

        private string eTTEDATEField;

        private object eTTEVALUField;

        private object eTPARTNERField;

        private string eTLANGField;

        private object eTTRAMTField;

        private object eTDSGREFField;

        private object eTCUSCTGRYField;

        private byte eTNOROOMSField;

        private object eTREGIOField;

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
        public decimal SDAMT
        {
            get
            {
                return this.sDAMTField;
            }
            set
            {
                this.sDAMTField = value;
            }
        }

        /// <remarks/>
        public decimal RCAMT
        {
            get
            {
                return this.rCAMTField;
            }
            set
            {
                this.rCAMTField = value;
            }
        }

        /// <remarks/>
        public decimal ACAMT
        {
            get
            {
                return this.aCAMTField;
            }
            set
            {
                this.aCAMTField = value;
            }
        }

        /// <remarks/>
        public byte SUBRC
        {
            get
            {
                return this.sUBRCField;
            }
            set
            {
                this.sUBRCField = value;
            }
        }

        /// <remarks/>
        public ushort TRANID
        {
            get
            {
                return this.tRANIDField;
            }
            set
            {
                this.tRANIDField = value;
            }
        }

        /// <remarks/>
        public byte SUBID
        {
            get
            {
                return this.sUBIDField;
            }
            set
            {
                this.sUBIDField = value;
            }
        }

        /// <remarks/>
        public object PARTNER
        {
            get
            {
                return this.pARTNERField;
            }
            set
            {
                this.pARTNERField = value;
            }
        }

        /// <remarks/>
        public decimal OUTSTND
        {
            get
            {
                return this.oUTSTNDField;
            }
            set
            {
                this.oUTSTNDField = value;
            }
        }

        /// <remarks/>
        public object ETCUSTYP
        {
            get
            {
                return this.eTCUSTYPField;
            }
            set
            {
                this.eTCUSTYPField = value;
            }
        }

        /// <remarks/>
        public object ETACCTCLASS
        {
            get
            {
                return this.eTACCTCLASSField;
            }
            set
            {
                this.eTACCTCLASSField = value;
            }
        }

        /// <remarks/>
        public object ETPREMISE
        {
            get
            {
                return this.eTPREMISEField;
            }
            set
            {
                this.eTPREMISEField = value;
            }
        }

        /// <remarks/>
        public object ETIDTYP
        {
            get
            {
                return this.eTIDTYPField;
            }
            set
            {
                this.eTIDTYPField = value;
            }
        }

        /// <remarks/>
        public object ETIDNUMBER
        {
            get
            {
                return this.eTIDNUMBERField;
            }
            set
            {
                this.eTIDNUMBERField = value;
            }
        }

        /// <remarks/>
        public string ETIDEXP
        {
            get
            {
                return this.eTIDEXPField;
            }
            set
            {
                this.eTIDEXPField = value;
            }
        }

        /// <remarks/>
        public object ETFNAME
        {
            get
            {
                return this.eTFNAMEField;
            }
            set
            {
                this.eTFNAMEField = value;
            }
        }

        /// <remarks/>
        public object ETLNAME
        {
            get
            {
                return this.eTLNAMEField;
            }
            set
            {
                this.eTLNAMEField = value;
            }
        }

        /// <remarks/>
        public object ETPOBOX
        {
            get
            {
                return this.eTPOBOXField;
            }
            set
            {
                this.eTPOBOXField = value;
            }
        }

        /// <remarks/>
        public object ETNATIO
        {
            get
            {
                return this.eTNATIOField;
            }
            set
            {
                this.eTNATIOField = value;
            }
        }

        /// <remarks/>
        public object ETMOBILE
        {
            get
            {
                return this.eTMOBILEField;
            }
            set
            {
                this.eTMOBILEField = value;
            }
        }

        /// <remarks/>
        public object ETEMAIL
        {
            get
            {
                return this.eTEMAILField;
            }
            set
            {
                this.eTEMAILField = value;
            }
        }

        /// <remarks/>
        public string ETTSDATE
        {
            get
            {
                return this.eTTSDATEField;
            }
            set
            {
                this.eTTSDATEField = value;
            }
        }

        /// <remarks/>
        public string ETTEDATE
        {
            get
            {
                return this.eTTEDATEField;
            }
            set
            {
                this.eTTEDATEField = value;
            }
        }

        /// <remarks/>
        public object ETTEVALU
        {
            get
            {
                return this.eTTEVALUField;
            }
            set
            {
                this.eTTEVALUField = value;
            }
        }

        /// <remarks/>
        public object ETPARTNER
        {
            get
            {
                return this.eTPARTNERField;
            }
            set
            {
                this.eTPARTNERField = value;
            }
        }

        /// <remarks/>
        public string ETLANG
        {
            get
            {
                return this.eTLANGField;
            }
            set
            {
                this.eTLANGField = value;
            }
        }

        /// <remarks/>
        public object ETTRAMT
        {
            get
            {
                return this.eTTRAMTField;
            }
            set
            {
                this.eTTRAMTField = value;
            }
        }

        /// <remarks/>
        public object ETDSGREF
        {
            get
            {
                return this.eTDSGREFField;
            }
            set
            {
                this.eTDSGREFField = value;
            }
        }

        /// <remarks/>
        public object ETCUSCTGRY
        {
            get
            {
                return this.eTCUSCTGRYField;
            }
            set
            {
                this.eTCUSCTGRYField = value;
            }
        }

        /// <remarks/>
        public byte ETNOROOMS
        {
            get
            {
                return this.eTNOROOMSField;
            }
            set
            {
                this.eTNOROOMSField = value;
            }
        }

        /// <remarks/>
        public object ETREGIO
        {
            get
            {
                return this.eTREGIOField;
            }
            set
            {
                this.eTREGIOField = value;
            }
        }
    }


}