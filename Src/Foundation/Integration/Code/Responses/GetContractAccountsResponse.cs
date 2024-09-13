using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Extensions;

namespace DEWAXP.Foundation.Integration.Responses
{

    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [XmlRoot(ElementName = "GetContractAccounts")]
    public class ContractAccountsRespponse
    {

        private ulong dateTimeStampField;

        private byte responseCodeField;

        private string descriptionField;

        private GetContractAccountsAccount[] contractAccountsField;

        /// <remarks/>
        public ulong DateTimeStamp
        {
            get
            {
                return this.dateTimeStampField;
            }
            set
            {
                this.dateTimeStampField = value;
            }
        }

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
        [System.Xml.Serialization.XmlArrayItemAttribute("Account", IsNullable = false)]
        public GetContractAccountsAccount[] ContractAccounts
        {
            get
            {
                return this.contractAccountsField;
            }
            set
            {
                this.contractAccountsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class GetContractAccountsAccount
    {

        //private uint contractAccountField;
        private string contractAccountField;

        private string contractAccountNameField;

        //private uint businessPartnerNoField;
        private string businessPartnerNoField;

        private string accountCategoryTextField;

        //private uint legacyAccountField;
        private string legacyAccountField;

        //private uint premiseField;
        private string premiseField;

        private byte billingClassField;

        private string nickNameField;

        private object consStatusField;

        private string cAPhotoField;

        /// <remarks/>
        //public uint ContractAccount
        public string ContractAccount
        {
            get
            {
                return this.contractAccountField;
            }
            set
            {
                this.contractAccountField = value;
            }
        }

        /// <remarks/>
        public string ContractAccountName
        {
            get
            {
                return this.contractAccountNameField;
            }
            set
            {
                this.contractAccountNameField = value;
            }
        }

        /// <remarks/>
        //public uint BusinessPartnerNo
        public string BusinessPartnerNo
        {
            get
            {
                return this.businessPartnerNoField.TrimStart('0');
            }
            set
            {
                this.businessPartnerNoField = value ?? string.Empty;
            }
        }

        /// <remarks/>
        public string AccountCategoryText
        {
            get
            {
                return this.accountCategoryTextField;
            }
            set
            {
                this.accountCategoryTextField = value;
            }
        }

        /// <remarks/>
        //public uint LegacyAccount
        public string LegacyAccount
        {
            get
            {
                return this.legacyAccountField;
            }
            set
            {
                this.legacyAccountField = value;
            }
        }

        /// <remarks/>
        //public uint Premise
        public string Premise
        {
            get
            {
                return this.premiseField.TrimStart('0');
            }
            set
            {
                this.premiseField = value ?? string.Empty;
            }
        }

        /// <remarks/>
        public byte BillingClass
        {
            get
            {
                return this.billingClassField;
            }
            set
            {
                this.billingClassField = value;
            }
        }

        /// <remarks/>
        public string NickName
        {
            get
            {
                return this.nickNameField;
            }
            set
            {
                this.nickNameField = value;
            }
        }

        /// <remarks/>
        public object ConsStatus
        {
            get
            {
                return this.consStatusField;
            }
            set
            {
                this.consStatusField = value;
            }
        }

        /// <remarks/>
        public string CAPhoto
        {
            get
            {
                return this.cAPhotoField;
            }
            set
            {
                this.cAPhotoField = value;
            }
        }
    }


}
