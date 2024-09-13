namespace DEWAXP.Foundation.Integration.Responses
{


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "ServiceInterruption", Namespace = "", IsNullable = false)]
    public partial class ServiceInterruptionResponseLite
    {

        private int responseCodeField;

        private string descriptionField;

        /// <remarks/>
        public int ResponseCode
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
    }



    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "ServiceInterruption", Namespace = "", IsNullable = false)]
    public partial class ServiceInterruptionResponse
    {

        private int responseCodeField;

        private string descriptionField;

        private ServiceInterruptionServiceDetails[] interruptionDetailsField;

        /// <remarks/>
        public int ResponseCode
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
        [System.Xml.Serialization.XmlArrayItemAttribute("ServiceDetails", IsNullable = false)]
        public ServiceInterruptionServiceDetails[] InterruptionDetails
        {
            get
            {
                return this.interruptionDetailsField;
            }
            set
            {
                this.interruptionDetailsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ServiceInterruptionServiceDetails
    {

        private string cityNameField;

        private string sTREETField;

        private string iNTRUPTIONField;

        private string sT_DATEField;

        private System.DateTime sT_TIMEField;

        private string eND_DATEField;

        private System.DateTime eND_TIMEField;

        private string dURATIONField;

        private uint cONTRACTACCOUNTField;

        private uint bUSINESSPARTNERField;

        /// <remarks/>
        public string CityName
        {
            get
            {
                return this.cityNameField;
            }
            set
            {
                this.cityNameField = value;
            }
        }

        /// <remarks/>
        public string STREET
        {
            get
            {
                return this.sTREETField;
            }
            set
            {
                this.sTREETField = value;
            }
        }

        /// <remarks/>
        public string INTRUPTION
        {
            get
            {
                return this.iNTRUPTIONField;
            }
            set
            {
                this.iNTRUPTIONField = value;
            }
        }

        /// <remarks/>
        public string ST_DATE
        {
            get
            {
                return this.sT_DATEField;
            }
            set
            {
                this.sT_DATEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "time")]
        public System.DateTime ST_TIME
        {
            get
            {
                return this.sT_TIMEField;
            }
            set
            {
                this.sT_TIMEField = value;
            }
        }

        /// <remarks/>
        public string END_DATE
        {
            get
            {
                return this.eND_DATEField;
            }
            set
            {
                this.eND_DATEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "time")]
        public System.DateTime END_TIME
        {
            get
            {
                return this.eND_TIMEField;
            }
            set
            {
                this.eND_TIMEField = value;
            }
        }

        /// <remarks/>
        public string DURATION
        {
            get
            {
                return this.dURATIONField;
            }
            set
            {
                this.dURATIONField = value;
            }
        }

        /// <remarks/>
        public uint CONTRACTACCOUNT
        {
            get
            {
                return this.cONTRACTACCOUNTField;
            }
            set
            {
                this.cONTRACTACCOUNTField = value;
            }
        }

        /// <remarks/>
        public uint BUSINESSPARTNER
        {
            get
            {
                return this.bUSINESSPARTNERField;
            }
            set
            {
                this.bUSINESSPARTNERField = value;
            }
        }
    }
}
