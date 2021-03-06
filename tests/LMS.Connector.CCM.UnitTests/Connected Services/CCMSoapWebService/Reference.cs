﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LMS.Connector.CCM.UnitTests.CCMSoapWebService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", ConfigurationName="CCMSoapWebService.CcmWebServiceSoap")]
    public interface CcmWebServiceSoap {
        
        // CODEGEN: Generating message contract since message GetPersonNameRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://www.opensolutions.com/CCM/CcmWebService/GetPersonName", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameResponse GetPersonName(LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.opensolutions.com/CCM/CcmWebService/GetPersonName", ReplyAction="*")]
        System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameResponse> GetPersonNameAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameRequest request);
        
        // CODEGEN: Generating message contract since message ProcessRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://www.opensolutions.com/CCM/CcmWebService/Process", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessResponse Process(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.opensolutions.com/CCM/CcmWebService/Process", ReplyAction="*")]
        System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessResponse> ProcessAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessRequest request);
        
        // CODEGEN: Generating message contract since message ProcessMessageStringRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://www.opensolutions.com/CCM/CcmWebService/ProcessMessageString", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringResponse ProcessMessageString(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.opensolutions.com/CCM/CcmWebService/ProcessMessageString", ReplyAction="*")]
        System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringResponse> ProcessMessageStringAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringRequest request);
        
        // CODEGEN: Generating message contract since message ProcessMessageNodeRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://www.opensolutions.com/CCM/CcmWebService/ProcessNativeMessage", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeResponse ProcessMessageNode(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.opensolutions.com/CCM/CcmWebService/ProcessNativeMessage", ReplyAction="*")]
        System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeResponse> ProcessMessageNodeAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeRequest request);
        
        // CODEGEN: Generating message contract since message ProcessMultiMessageNodeRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://www.opensolutions.com/CCM/CcmWebService/ProcessNativeMultiMessage", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeResponse ProcessMultiMessageNode(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.opensolutions.com/CCM/CcmWebService/ProcessNativeMultiMessage", ReplyAction="*")]
        System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeResponse> ProcessMultiMessageNodeAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService")]
    public partial class CredentialsHeader : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string usernameField;
        
        private string passwordField;
        
        private string facilityField;
        
        private string cultureIdField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string Username {
            get {
                return this.usernameField;
            }
            set {
                this.usernameField = value;
                this.RaisePropertyChanged("Username");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
                this.RaisePropertyChanged("Password");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Facility {
            get {
                return this.facilityField;
            }
            set {
                this.facilityField = value;
                this.RaisePropertyChanged("Facility");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string CultureId {
            get {
                return this.cultureIdField;
            }
            set {
                this.cultureIdField = value;
                this.RaisePropertyChanged("CultureId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
                this.RaisePropertyChanged("AnyAttr");
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
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetPersonName", WrapperNamespace="http://www.opensolutions.com/CCM/CcmWebService", IsWrapped=true)]
    public partial class GetPersonNameRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService")]
        public LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", Order=0)]
        public string partyId;
        
        public GetPersonNameRequest() {
        }
        
        public GetPersonNameRequest(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, string partyId) {
            this.CredentialsHeader = CredentialsHeader;
            this.partyId = partyId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetPersonNameResponse", WrapperNamespace="http://www.opensolutions.com/CCM/CcmWebService", IsWrapped=true)]
    public partial class GetPersonNameResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", Order=0)]
        public string GetPersonNameResult;
        
        public GetPersonNameResponse() {
        }
        
        public GetPersonNameResponse(string GetPersonNameResult) {
            this.GetPersonNameResult = GetPersonNameResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Process", WrapperNamespace="http://www.opensolutions.com/CCM/CcmWebService", IsWrapped=true)]
    public partial class ProcessRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService")]
        public LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] request;
        
        public ProcessRequest() {
        }
        
        public ProcessRequest(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, byte[] request) {
            this.CredentialsHeader = CredentialsHeader;
            this.request = request;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ProcessResponse", WrapperNamespace="http://www.opensolutions.com/CCM/CcmWebService", IsWrapped=true)]
    public partial class ProcessResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] ProcessResult;
        
        public ProcessResponse() {
        }
        
        public ProcessResponse(byte[] ProcessResult) {
            this.ProcessResult = ProcessResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ProcessMessageString", WrapperNamespace="http://www.opensolutions.com/CCM/CcmWebService", IsWrapped=true)]
    public partial class ProcessMessageStringRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService")]
        public LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", Order=0)]
        public string request;
        
        public ProcessMessageStringRequest() {
        }
        
        public ProcessMessageStringRequest(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, string request) {
            this.CredentialsHeader = CredentialsHeader;
            this.request = request;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ProcessMessageStringResponse", WrapperNamespace="http://www.opensolutions.com/CCM/CcmWebService", IsWrapped=true)]
    public partial class ProcessMessageStringResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", Order=0)]
        public string ProcessMessageStringResult;
        
        public ProcessMessageStringResponse() {
        }
        
        public ProcessMessageStringResponse(string ProcessMessageStringResult) {
            this.ProcessMessageStringResult = ProcessMessageStringResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ProcessMessageNode", WrapperNamespace="http://www.opensolutions.com/CCM/CcmWebService", IsWrapped=true)]
    public partial class ProcessMessageNodeRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService")]
        public LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", Order=0)]
        public System.Xml.XmlNode request;
        
        public ProcessMessageNodeRequest() {
        }
        
        public ProcessMessageNodeRequest(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, System.Xml.XmlNode request) {
            this.CredentialsHeader = CredentialsHeader;
            this.request = request;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ProcessMessageNodeResponse", WrapperNamespace="http://www.opensolutions.com/CCM/CcmWebService", IsWrapped=true)]
    public partial class ProcessMessageNodeResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", Order=0)]
        public System.Xml.XmlNode ProcessMessageNodeResult;
        
        public ProcessMessageNodeResponse() {
        }
        
        public ProcessMessageNodeResponse(System.Xml.XmlNode ProcessMessageNodeResult) {
            this.ProcessMessageNodeResult = ProcessMessageNodeResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ProcessMultiMessageNode", WrapperNamespace="http://www.opensolutions.com/CCM/CcmWebService", IsWrapped=true)]
    public partial class ProcessMultiMessageNodeRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService")]
        public LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", Order=0)]
        public System.Xml.XmlNode request;
        
        public ProcessMultiMessageNodeRequest() {
        }
        
        public ProcessMultiMessageNodeRequest(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, System.Xml.XmlNode request) {
            this.CredentialsHeader = CredentialsHeader;
            this.request = request;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ProcessMultiMessageNodeResponse", WrapperNamespace="http://www.opensolutions.com/CCM/CcmWebService", IsWrapped=true)]
    public partial class ProcessMultiMessageNodeResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.opensolutions.com/CCM/CcmWebService", Order=0)]
        public System.Xml.XmlNode ProcessMultiMessageNodeResult;
        
        public ProcessMultiMessageNodeResponse() {
        }
        
        public ProcessMultiMessageNodeResponse(System.Xml.XmlNode ProcessMultiMessageNodeResult) {
            this.ProcessMultiMessageNodeResult = ProcessMultiMessageNodeResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface CcmWebServiceSoapChannel : LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CcmWebServiceSoapClient : System.ServiceModel.ClientBase<LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap>, LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap {
        
        public CcmWebServiceSoapClient() {
        }
        
        public CcmWebServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CcmWebServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CcmWebServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CcmWebServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameResponse LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap.GetPersonName(LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameRequest request) {
            return base.Channel.GetPersonName(request);
        }
        
        public string GetPersonName(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, string partyId) {
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameRequest inValue = new LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameRequest();
            inValue.CredentialsHeader = CredentialsHeader;
            inValue.partyId = partyId;
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameResponse retVal = ((LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap)(this)).GetPersonName(inValue);
            return retVal.GetPersonNameResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameResponse> LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap.GetPersonNameAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameRequest request) {
            return base.Channel.GetPersonNameAsync(request);
        }
        
        public System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameResponse> GetPersonNameAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, string partyId) {
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameRequest inValue = new LMS.Connector.CCM.UnitTests.CCMSoapWebService.GetPersonNameRequest();
            inValue.CredentialsHeader = CredentialsHeader;
            inValue.partyId = partyId;
            return ((LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap)(this)).GetPersonNameAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessResponse LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap.Process(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessRequest request) {
            return base.Channel.Process(request);
        }
        
        public byte[] Process(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, byte[] request) {
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessRequest inValue = new LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessRequest();
            inValue.CredentialsHeader = CredentialsHeader;
            inValue.request = request;
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessResponse retVal = ((LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap)(this)).Process(inValue);
            return retVal.ProcessResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessResponse> LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap.ProcessAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessRequest request) {
            return base.Channel.ProcessAsync(request);
        }
        
        public System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessResponse> ProcessAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, byte[] request) {
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessRequest inValue = new LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessRequest();
            inValue.CredentialsHeader = CredentialsHeader;
            inValue.request = request;
            return ((LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap)(this)).ProcessAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringResponse LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap.ProcessMessageString(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringRequest request) {
            return base.Channel.ProcessMessageString(request);
        }
        
        public string ProcessMessageString(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, string request) {
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringRequest inValue = new LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringRequest();
            inValue.CredentialsHeader = CredentialsHeader;
            inValue.request = request;
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringResponse retVal = ((LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap)(this)).ProcessMessageString(inValue);
            return retVal.ProcessMessageStringResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringResponse> LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap.ProcessMessageStringAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringRequest request) {
            return base.Channel.ProcessMessageStringAsync(request);
        }
        
        public System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringResponse> ProcessMessageStringAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, string request) {
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringRequest inValue = new LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageStringRequest();
            inValue.CredentialsHeader = CredentialsHeader;
            inValue.request = request;
            return ((LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap)(this)).ProcessMessageStringAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeResponse LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap.ProcessMessageNode(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeRequest request) {
            return base.Channel.ProcessMessageNode(request);
        }
        
        public System.Xml.XmlNode ProcessMessageNode(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, System.Xml.XmlNode request) {
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeRequest inValue = new LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeRequest();
            inValue.CredentialsHeader = CredentialsHeader;
            inValue.request = request;
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeResponse retVal = ((LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap)(this)).ProcessMessageNode(inValue);
            return retVal.ProcessMessageNodeResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeResponse> LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap.ProcessMessageNodeAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeRequest request) {
            return base.Channel.ProcessMessageNodeAsync(request);
        }
        
        public System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeResponse> ProcessMessageNodeAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, System.Xml.XmlNode request) {
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeRequest inValue = new LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMessageNodeRequest();
            inValue.CredentialsHeader = CredentialsHeader;
            inValue.request = request;
            return ((LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap)(this)).ProcessMessageNodeAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeResponse LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap.ProcessMultiMessageNode(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeRequest request) {
            return base.Channel.ProcessMultiMessageNode(request);
        }
        
        public System.Xml.XmlNode ProcessMultiMessageNode(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, System.Xml.XmlNode request) {
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeRequest inValue = new LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeRequest();
            inValue.CredentialsHeader = CredentialsHeader;
            inValue.request = request;
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeResponse retVal = ((LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap)(this)).ProcessMultiMessageNode(inValue);
            return retVal.ProcessMultiMessageNodeResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeResponse> LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap.ProcessMultiMessageNodeAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeRequest request) {
            return base.Channel.ProcessMultiMessageNodeAsync(request);
        }
        
        public System.Threading.Tasks.Task<LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeResponse> ProcessMultiMessageNodeAsync(LMS.Connector.CCM.UnitTests.CCMSoapWebService.CredentialsHeader CredentialsHeader, System.Xml.XmlNode request) {
            LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeRequest inValue = new LMS.Connector.CCM.UnitTests.CCMSoapWebService.ProcessMultiMessageNodeRequest();
            inValue.CredentialsHeader = CredentialsHeader;
            inValue.request = request;
            return ((LMS.Connector.CCM.UnitTests.CCMSoapWebService.CcmWebServiceSoap)(this)).ProcessMultiMessageNodeAsync(inValue);
        }
    }
}
