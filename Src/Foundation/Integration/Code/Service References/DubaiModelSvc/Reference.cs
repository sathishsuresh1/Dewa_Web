﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DEWAXP.Foundation.Integration.DubaiModelSvc {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://ws.dmodel.customer.dewa.gov.ae/", ConfigurationName="DubaiModelSvc.Config")]
    public interface Config {
        
        // CODEGEN: Generating message contract since the operation getAccountTypeStatus is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusResponse1 getAccountTypeStatus(DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        System.Threading.Tasks.Task<DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusResponse1> getAccountTypeStatusAsync(DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ws.dmodel.customer.dewa.gov.ae/")]
    public partial class getAccountTypeStatus : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string contractaccountnumberField;
        
        private string merchantidField;
        
        private string merchantpasswordField;
        
        private string vendoridField;
        
        private string appversionField;
        
        private string mobileosversionField;
        
        private string appidentifierField;
        
        private string langField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string contractaccountnumber {
            get {
                return this.contractaccountnumberField;
            }
            set {
                this.contractaccountnumberField = value;
                this.RaisePropertyChanged("contractaccountnumber");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string merchantid {
            get {
                return this.merchantidField;
            }
            set {
                this.merchantidField = value;
                this.RaisePropertyChanged("merchantid");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string merchantpassword {
            get {
                return this.merchantpasswordField;
            }
            set {
                this.merchantpasswordField = value;
                this.RaisePropertyChanged("merchantpassword");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string vendorid {
            get {
                return this.vendoridField;
            }
            set {
                this.vendoridField = value;
                this.RaisePropertyChanged("vendorid");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string appversion {
            get {
                return this.appversionField;
            }
            set {
                this.appversionField = value;
                this.RaisePropertyChanged("appversion");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string mobileosversion {
            get {
                return this.mobileosversionField;
            }
            set {
                this.mobileosversionField = value;
                this.RaisePropertyChanged("mobileosversion");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string appidentifier {
            get {
                return this.appidentifierField;
            }
            set {
                this.appidentifierField = value;
                this.RaisePropertyChanged("appidentifier");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string lang {
            get {
                return this.langField;
            }
            set {
                this.langField = value;
                this.RaisePropertyChanged("lang");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ws.dmodel.customer.dewa.gov.ae/")]
    public partial class accountTypeStatusRes : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string accountClassField;
        
        private string billingClassField;
        
        private string connectionTypeField;
        
        private string descriptionField;
        
        private string premiseTypeField;
        
        private string responseCodeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string accountClass {
            get {
                return this.accountClassField;
            }
            set {
                this.accountClassField = value;
                this.RaisePropertyChanged("accountClass");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string billingClass {
            get {
                return this.billingClassField;
            }
            set {
                this.billingClassField = value;
                this.RaisePropertyChanged("billingClass");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string connectionType {
            get {
                return this.connectionTypeField;
            }
            set {
                this.connectionTypeField = value;
                this.RaisePropertyChanged("connectionType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
                this.RaisePropertyChanged("description");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string premiseType {
            get {
                return this.premiseTypeField;
            }
            set {
                this.premiseTypeField = value;
                this.RaisePropertyChanged("premiseType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string responseCode {
            get {
                return this.responseCodeField;
            }
            set {
                this.responseCodeField = value;
                this.RaisePropertyChanged("responseCode");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ws.dmodel.customer.dewa.gov.ae/")]
    public partial class getAccountTypeStatusResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private accountTypeStatusRes returnField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public accountTypeStatusRes @return {
            get {
                return this.returnField;
            }
            set {
                this.returnField = value;
                this.RaisePropertyChanged("return");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getAccountTypeStatusRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.dmodel.customer.dewa.gov.ae/", Order=0)]
        public DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatus getAccountTypeStatus;
        
        public getAccountTypeStatusRequest() {
        }
        
        public getAccountTypeStatusRequest(DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatus getAccountTypeStatus) {
            this.getAccountTypeStatus = getAccountTypeStatus;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getAccountTypeStatusResponse1 {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.dmodel.customer.dewa.gov.ae/", Order=0)]
        public DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusResponse getAccountTypeStatusResponse;
        
        public getAccountTypeStatusResponse1() {
        }
        
        public getAccountTypeStatusResponse1(DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusResponse getAccountTypeStatusResponse) {
            this.getAccountTypeStatusResponse = getAccountTypeStatusResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ConfigChannel : DEWAXP.Foundation.Integration.DubaiModelSvc.Config, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ConfigClient : System.ServiceModel.ClientBase<DEWAXP.Foundation.Integration.DubaiModelSvc.Config>, DEWAXP.Foundation.Integration.DubaiModelSvc.Config {
        
        public ConfigClient() {
        }
        
        public ConfigClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ConfigClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConfigClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConfigClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusResponse1 DEWAXP.Foundation.Integration.DubaiModelSvc.Config.getAccountTypeStatus(DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusRequest request) {
            return base.Channel.getAccountTypeStatus(request);
        }
        
        public DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusResponse getAccountTypeStatus(DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatus getAccountTypeStatus1) {
            DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusRequest inValue = new DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusRequest();
            inValue.getAccountTypeStatus = getAccountTypeStatus1;
            DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusResponse1 retVal = ((DEWAXP.Foundation.Integration.DubaiModelSvc.Config)(this)).getAccountTypeStatus(inValue);
            return retVal.getAccountTypeStatusResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusResponse1> DEWAXP.Foundation.Integration.DubaiModelSvc.Config.getAccountTypeStatusAsync(DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusRequest request) {
            return base.Channel.getAccountTypeStatusAsync(request);
        }
        
        public System.Threading.Tasks.Task<DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusResponse1> getAccountTypeStatusAsync(DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatus getAccountTypeStatus) {
            DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusRequest inValue = new DEWAXP.Foundation.Integration.DubaiModelSvc.getAccountTypeStatusRequest();
            inValue.getAccountTypeStatus = getAccountTypeStatus;
            return ((DEWAXP.Foundation.Integration.DubaiModelSvc.Config)(this)).getAccountTypeStatusAsync(inValue);
        }
    }
}
