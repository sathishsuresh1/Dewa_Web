using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName= "MoveIn",Namespace = "", IsNullable = false)]
    public partial class MoveInLite
    {

        private ushort responseCodeField;

        private string descriptionField;

        private byte sUBRCField;

        private byte sUBIDField;

        /// <remarks/>
        public ushort ResponseCode
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
    }



    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "MoveIn", Namespace = "", IsNullable = false)]
    public partial class MoveInResponse
    {

        private ushort responseCodeField;

        private string descriptionField;

        private decimal sDAMTField;

        private decimal rCAMTField;

        private decimal aCAMTField;

        private ushort sUBRCField;

        private string tRANIDField;

        private ushort sUBIDField;

        private string pARTNERField;

        private decimal oUTSTNDField;

        private string eTCUSTYPField;

        private string eTACCTCLASSField;

        private uint eTPREMISEField;

        private string eTIDTYPField;

        private string eTIDNUMBERField;

        private string eTIDEXPField;

        private string eTFNAMEField;

        private string eTLNAMEField;

        private string eTPOBOXField;

        private string eTNATIOField;

        private string eTMOBILEField;

        private string eTEMAILField;

        private string eTTSDATEField;

        private string eTTEDATEField;

        private string eTTEVALUField;

        private string eTPARTNERField;

        private string eTLANGField;

        private string eTTRAMTField;

        private string eTDSGREFField;

        private string eTCUSCTGRYField;

        private ushort eTNOROOMSField;

        private string eTREGIOField;

        private decimal knowledgeFee;

        private decimal innovationFee;

        /// <remarks/>
        public ushort ResponseCode
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
        public ushort SUBRC
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
        public string TRANID
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
        public ushort SUBID
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
        public string PARTNER
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
        public string ETCUSTYP
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
        public string ETACCTCLASS
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
        public uint ETPREMISE
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
        public string ETIDTYP
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
        public string ETIDNUMBER
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
        public string ETFNAME
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
        public string ETLNAME
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
        public string ETPOBOX
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
        public string ETNATIO
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
        public string ETMOBILE
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
        public string ETEMAIL
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
        public string ETTEVALU
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
        public string ETPARTNER
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
        public string ETTRAMT
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
        public string ETDSGREF
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
        public string ETCUSCTGRY
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
        public ushort ETNOROOMS
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

        public decimal KNOWLEDGEFEE
        {
            get
            {
                return this.knowledgeFee;
            }
            set
            {
                this.knowledgeFee = value;
            }
        }

        public decimal INNOVATIONFEE
        {
            get
            {
                return this.innovationFee;
            }
            set
            {
                this.innovationFee = value;
            }
        }

        //[XmlElement("KNOWLEDGEFEE")]
        //public string KnowledgeFee { get; set; }

        //[XmlElement("INNOVATIONFEE")]
        //public string InnovationFee { get; set; }

        /// <remarks/>
        public string ETREGIO
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

        [XmlElement("BPLIST")]
        public List<BPLIST> BusinessPartners { get; set; }        

        [XmlElement("OutstandingList")]
        public OutstandingList OutstandingList { get; set; }        

    }

    [Serializable]
    [XmlRoot(ElementName = "OutstandingList")]
    public class OutstandingList
    {
        [XmlElement(ElementName = "ContractAccount")]
        public List<ContractAccount> ContractAccount
        {
            get;
            set;
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "ContractAccount")]
    public class ContractAccount
    {
        [XmlElement(ElementName = "AccountNumber")]
        public string AccountNumber
        {
            get;
            set;
        }

        [XmlElement(ElementName = "TotalDue")]
        public string TotalDue
        {
            get;
            set;
        }
    }

    [Serializable]
    [XmlRoot("BPLIST")]
    public class BPLIST
    {
        [XmlElement(ElementName = "BPNUM")]
        public string BPNUM
        {
            get;
            set;
        }

        [XmlElement(ElementName = "FNAME")]
        public string FNAME { get; set; }

        [XmlElement(ElementName = "LNAME")]
        public string LNAME { get; set; }

        [XmlElement(ElementName = "POBOX")]
        public string POBOX { get; set; }

        [XmlElement(ElementName = "NATION")]
        public string NATION { get; set; }
        [XmlElement(ElementName = "MOBILE")]
        public string MOBILE { get; set; }
        [XmlElement(ElementName = "EMAIL")]
        public string EMAIL { get; set; }
        [XmlElement(ElementName = "TITLE")]
        public string TITLE { get; set; }

        public string BPNUMTrimmed
        {
            get
            {
                return !string.IsNullOrEmpty(this.BPNUM) && this.BPNUM.StartsWith("00")
                    ? this.BPNUM.Substring(2)
                    : this.BPNUM;
            }
            
        }
       
    }

    ///// <remarks/>
    //[XmlType(AnonymousType = true)]
    //[XmlRoot(ElementName = "MoveInEjari", Namespace = "", IsNullable = false)]
    //public partial class MoveInEjariResponse
    //{

    //    private ushort responseCodeField;

    //    private string descriptionField;

    //    private decimal sDAMTField;

    //    private decimal rCAMTField;

    //    private decimal aCAMTField;

    //    private string tRANIDField;

    //    private string nameField;

    //    private string emailField;

    //    private string mobileField;

    //    private string useridField;

    //    private string partnerField;

    //    private decimal oUTSTNDField;

    //    private decimal knowledgeFee;

    //    private decimal innovationFee;

    //    /// <remarks/>
    //    public ushort ResponseCode
    //    {
    //        get
    //        {
    //            return this.responseCodeField;
    //        }
    //        set
    //        {
    //            this.responseCodeField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string Description
    //    {
    //        get
    //        {
    //            return this.descriptionField;
    //        }
    //        set
    //        {
    //            this.descriptionField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public decimal SDAMT
    //    {
    //        get
    //        {
    //            return this.sDAMTField;
    //        }
    //        set
    //        {
    //            this.sDAMTField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public decimal RCAMT
    //    {
    //        get
    //        {
    //            return this.rCAMTField;
    //        }
    //        set
    //        {
    //            this.rCAMTField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public decimal ACAMT
    //    {
    //        get
    //        {
    //            return this.aCAMTField;
    //        }
    //        set
    //        {
    //            this.aCAMTField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string TRANID
    //    {
    //        get
    //        {
    //            return this.tRANIDField;
    //        }
    //        set
    //        {
    //            this.tRANIDField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public decimal OUTSTND
    //    {
    //        get
    //        {
    //            return this.oUTSTNDField;
    //        }
    //        set
    //        {
    //            this.oUTSTNDField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string NAME
    //    {
    //        get
    //        {
    //            return this.nameField;
    //        }
    //        set
    //        {
    //            this.nameField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string EMAIL
    //    {
    //        get
    //        {
    //            return this.emailField;
    //        }
    //        set
    //        {
    //            this.emailField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string MOBILE
    //    {
    //        get
    //        {
    //            return this.mobileField;
    //        }
    //        set
    //        {
    //            this.mobileField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string USERID
    //    {
    //        get
    //        {
    //            return this.useridField;
    //        }
    //        set
    //        {
    //            this.useridField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string PARTNER
    //    {
    //        get
    //        {
    //            return this.partnerField;
    //        }
    //        set
    //        {
    //            this.partnerField = value;
    //        }
    //    }

    //    public decimal KNOWLEDGEFEE
    //    {
    //        get
    //        {
    //            return this.knowledgeFee;
    //        }
    //        set
    //        {
    //            this.knowledgeFee = value;
    //        }
    //    }

    //    public decimal INNOVATIONFEE
    //    {
    //        get
    //        {
    //            return this.innovationFee;
    //        }
    //        set
    //        {
    //            this.innovationFee = value;
    //        }
    //    }

    //    [XmlElement("PremiseList")]
    //    public List<PremiseList> LstPremise { get; set; }
    //    [XmlElement("PremiseAmountList")]
    //    public List<PremiseAmountList> LstPremiseAmount { get; set; }
    //    [XmlElement("AMOUNTLIST")]
    //    public List<AMOUNTLIST> LstAMOUNTLIST { get; set; }

    //}

    ///// <remarks/>
    //[XmlType(AnonymousType = true)]
    //[XmlRoot(ElementName = "MoveInEjari", Namespace = "", IsNullable = false)]
    //public partial class MoveInEjariResponse
    //{

    //    //private ushort responseCodeField;

    //    //private string descriptionField;

    //    //private decimal sDAMTField;

    //    //private decimal rCAMTField;

    //    //private decimal aCAMTField;

    //    //private string tRANIDField;

    //    //private string nameField;

    //    //private string emailField;

    //    //private string mobileField;

    //    //private string useridField;

    //    //private string partnerField;

    //    //private decimal oUTSTNDField;

    //    //private decimal knowledgeFee;

    //    //private decimal innovationFee;

    //    [XmlElement(ElementName = "ResponseCode")]
    //    public string ResponseCode { get; set; }

    //    [XmlElement(ElementName = "Description")]
    //    public string Description { get; set; }



    //    [XmlElement(ElementName = "SDAMT")]
    //    public string SDAMT { get; set; }

    //    [XmlElement(ElementName = "RCAMT")]
    //    public string RCAMT { get; set; }

    //    [XmlElement(ElementName = "ACAMT")]
    //    public string ACAMT { get; set; }

    //    [XmlElement(ElementName = "KNOWLEDGEFEE")]
    //    public string KNOWLEDGEFEE { get; set; }

    //    [XmlElement(ElementName = "INNOVATIONFEE")]
    //    public string INNOVATIONFEE { get; set; }

    //    [XmlElement(ElementName = "TRANID")]
    //    public string TRANID { get; set; }

    //    [XmlElement(ElementName = "NAME")]
    //    public string NAME { get; set; }

    //    [XmlElement(ElementName = "EMAIL")]
    //    public string EMAIL { get; set; }

    //    [XmlElement(ElementName = "MOBILE")]
    //    public string MOBILE { get; set; }

    //    [XmlElement(ElementName = "USERID")]
    //    public string USERID { get; set; }

    //    [XmlElement(ElementName = "PARTNER")]
    //    public string PARTNER { get; set; }

    //    [XmlElement(ElementName = "OUTSTND")]
    //    public string OUTSTND { get; set; }

    //    [XmlElement("PREMISELIST")]
    //    public List<PremiseList> PREMISELIST { get; set; }
    //    [XmlElement("PREMISEAMOUNTLIST")]
    //    public List<PremiseAmountList> PREMISEAMOUNTLIST { get; set; }
    //    [XmlElement("AMOUNTLIST")]
    //    public List<AMOUNTLIST> AMOUNTLIST { get; set; }

    //}

    //[Serializable]
    //[XmlRoot(ElementName = "PREMISELIST")]
    //public class PremiseList
    //{
    //    [XmlElement(ElementName = "PremiseNumber")]
    //    public string PremiseNumber { get; set; }
    //    [XmlElement(ElementName = "PremiseError")]
    //    public string PremiseError { get; set; }
    //}

    //[Serializable]
    //[XmlRoot(ElementName = "AMOUNTLIST")]
    //public class AMOUNTLIST
    //{
    //    [XmlElement(ElementName = "FEE")]
    //    public string FEE { get; set; }
    //    [XmlElement(ElementName = "LABEL")]
    //    public string LABEL { get; set; }
    //}

    //[Serializable]
    //[XmlRoot(ElementName = "PREMISEAMOUNTLIST")]
    //public class PremiseAmountList
    //{
    //    [XmlElement(ElementName = "PremiseNumber")]
    //    public string PremiseNumber { get; set; }
    //    [XmlElement(ElementName = "ContractAccount")]
    //    public string ContractAccount { get; set; }
    //    [XmlElement(ElementName = "BusinessPartner")]
    //    public string BusinessPartner { get; set; }
    //    [XmlElement(ElementName = "Amount")]
    //    public string Amount { get; set; }
    //  }


}
