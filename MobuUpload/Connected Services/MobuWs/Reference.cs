﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MobuUpload.MobuWs {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PackageDetails", Namespace="http://schemas.datacontract.org/2004/07/MobuWcf")]
    [System.SerializableAttribute()]
    public partial class PackageDetails : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool CriticalField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VersionField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Critical {
            get {
                return this.CriticalField;
            }
            set {
                if ((this.CriticalField.Equals(value) != true)) {
                    this.CriticalField = value;
                    this.RaisePropertyChanged("Critical");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Version {
            get {
                return this.VersionField;
            }
            set {
                if ((object.ReferenceEquals(this.VersionField, value) != true)) {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
                }
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Fragment", Namespace="http://schemas.datacontract.org/2004/07/MobuWcf.Models")]
    [System.SerializableAttribute()]
    public partial class Fragment : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] DataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IndexField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private MobuUpload.MobuWs.PackageDetails PackageDetailsField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Data {
            get {
                return this.DataField;
            }
            set {
                if ((object.ReferenceEquals(this.DataField, value) != true)) {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Index {
            get {
                return this.IndexField;
            }
            set {
                if ((this.IndexField.Equals(value) != true)) {
                    this.IndexField = value;
                    this.RaisePropertyChanged("Index");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public MobuUpload.MobuWs.PackageDetails PackageDetails {
            get {
                return this.PackageDetailsField;
            }
            set {
                if ((object.ReferenceEquals(this.PackageDetailsField, value) != true)) {
                    this.PackageDetailsField = value;
                    this.RaisePropertyChanged("PackageDetails");
                }
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="MobuWs.IMobuWs")]
    public interface IMobuWs {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/TestWebService", ReplyAction="http://tempuri.org/IMobuWs/TestWebServiceResponse")]
        bool TestWebService();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/TestWebService", ReplyAction="http://tempuri.org/IMobuWs/TestWebServiceResponse")]
        System.Threading.Tasks.Task<bool> TestWebServiceAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/StartUpload", ReplyAction="http://tempuri.org/IMobuWs/StartUploadResponse")]
        bool StartUpload(MobuUpload.MobuWs.PackageDetails packageDetails, int packageSize, int fragmentSize);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/StartUpload", ReplyAction="http://tempuri.org/IMobuWs/StartUploadResponse")]
        System.Threading.Tasks.Task<bool> StartUploadAsync(MobuUpload.MobuWs.PackageDetails packageDetails, int packageSize, int fragmentSize);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/UploadNextFragment", ReplyAction="http://tempuri.org/IMobuWs/UploadNextFragmentResponse")]
        bool UploadNextFragment(MobuUpload.MobuWs.Fragment fragment);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/UploadNextFragment", ReplyAction="http://tempuri.org/IMobuWs/UploadNextFragmentResponse")]
        System.Threading.Tasks.Task<bool> UploadNextFragmentAsync(MobuUpload.MobuWs.Fragment fragment);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/CheckForActiveUploads", ReplyAction="http://tempuri.org/IMobuWs/CheckForActiveUploadsResponse")]
        bool CheckForActiveUploads(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/CheckForActiveUploads", ReplyAction="http://tempuri.org/IMobuWs/CheckForActiveUploadsResponse")]
        System.Threading.Tasks.Task<bool> CheckForActiveUploadsAsync(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/CancelUpload", ReplyAction="http://tempuri.org/IMobuWs/CancelUploadResponse")]
        bool CancelUpload(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/CancelUpload", ReplyAction="http://tempuri.org/IMobuWs/CancelUploadResponse")]
        System.Threading.Tasks.Task<bool> CancelUploadAsync(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/FinishUpload", ReplyAction="http://tempuri.org/IMobuWs/FinishUploadResponse")]
        bool FinishUpload(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/FinishUpload", ReplyAction="http://tempuri.org/IMobuWs/FinishUploadResponse")]
        System.Threading.Tasks.Task<bool> FinishUploadAsync(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/TogglePackageCriticalState", ReplyAction="http://tempuri.org/IMobuWs/TogglePackageCriticalStateResponse")]
        bool TogglePackageCriticalState(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/TogglePackageCriticalState", ReplyAction="http://tempuri.org/IMobuWs/TogglePackageCriticalStateResponse")]
        System.Threading.Tasks.Task<bool> TogglePackageCriticalStateAsync(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/CheckForActiveDownloads", ReplyAction="http://tempuri.org/IMobuWs/CheckForActiveDownloadsResponse")]
        int CheckForActiveDownloads(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/CheckForActiveDownloads", ReplyAction="http://tempuri.org/IMobuWs/CheckForActiveDownloadsResponse")]
        System.Threading.Tasks.Task<int> CheckForActiveDownloadsAsync(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/CancelActiveDownloads", ReplyAction="http://tempuri.org/IMobuWs/CancelActiveDownloadsResponse")]
        bool CancelActiveDownloads(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/CancelActiveDownloads", ReplyAction="http://tempuri.org/IMobuWs/CancelActiveDownloadsResponse")]
        System.Threading.Tasks.Task<bool> CancelActiveDownloadsAsync(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/StartDownload", ReplyAction="http://tempuri.org/IMobuWs/StartDownloadResponse")]
        int StartDownload(MobuUpload.MobuWs.PackageDetails packageDetails, int fragmentSize);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/StartDownload", ReplyAction="http://tempuri.org/IMobuWs/StartDownloadResponse")]
        System.Threading.Tasks.Task<int> StartDownloadAsync(MobuUpload.MobuWs.PackageDetails packageDetails, int fragmentSize);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/GetNextFragment", ReplyAction="http://tempuri.org/IMobuWs/GetNextFragmentResponse")]
        MobuUpload.MobuWs.Fragment GetNextFragment(MobuUpload.MobuWs.PackageDetails packageDetails, int index, int fragmentSize);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/GetNextFragment", ReplyAction="http://tempuri.org/IMobuWs/GetNextFragmentResponse")]
        System.Threading.Tasks.Task<MobuUpload.MobuWs.Fragment> GetNextFragmentAsync(MobuUpload.MobuWs.PackageDetails packageDetails, int index, int fragmentSize);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/FinishDownload", ReplyAction="http://tempuri.org/IMobuWs/FinishDownloadResponse")]
        bool FinishDownload(MobuUpload.MobuWs.PackageDetails packageDetails, int fragmentSize);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/FinishDownload", ReplyAction="http://tempuri.org/IMobuWs/FinishDownloadResponse")]
        System.Threading.Tasks.Task<bool> FinishDownloadAsync(MobuUpload.MobuWs.PackageDetails packageDetails, int fragmentSize);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/GetPackageList", ReplyAction="http://tempuri.org/IMobuWs/GetPackageListResponse")]
        System.Collections.Generic.List<MobuUpload.MobuWs.PackageDetails> GetPackageList();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/GetPackageList", ReplyAction="http://tempuri.org/IMobuWs/GetPackageListResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<MobuUpload.MobuWs.PackageDetails>> GetPackageListAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/RemovePackage", ReplyAction="http://tempuri.org/IMobuWs/RemovePackageResponse")]
        bool RemovePackage(MobuUpload.MobuWs.PackageDetails packageDetails);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMobuWs/RemovePackage", ReplyAction="http://tempuri.org/IMobuWs/RemovePackageResponse")]
        System.Threading.Tasks.Task<bool> RemovePackageAsync(MobuUpload.MobuWs.PackageDetails packageDetails);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMobuWsChannel : MobuUpload.MobuWs.IMobuWs, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MobuWsClient : System.ServiceModel.ClientBase<MobuUpload.MobuWs.IMobuWs>, MobuUpload.MobuWs.IMobuWs {
        
        public MobuWsClient() {
        }
        
        public MobuWsClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MobuWsClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MobuWsClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MobuWsClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool TestWebService() {
            return base.Channel.TestWebService();
        }
        
        public System.Threading.Tasks.Task<bool> TestWebServiceAsync() {
            return base.Channel.TestWebServiceAsync();
        }
        
        public bool StartUpload(MobuUpload.MobuWs.PackageDetails packageDetails, int packageSize, int fragmentSize) {
            return base.Channel.StartUpload(packageDetails, packageSize, fragmentSize);
        }
        
        public System.Threading.Tasks.Task<bool> StartUploadAsync(MobuUpload.MobuWs.PackageDetails packageDetails, int packageSize, int fragmentSize) {
            return base.Channel.StartUploadAsync(packageDetails, packageSize, fragmentSize);
        }
        
        public bool UploadNextFragment(MobuUpload.MobuWs.Fragment fragment) {
            return base.Channel.UploadNextFragment(fragment);
        }
        
        public System.Threading.Tasks.Task<bool> UploadNextFragmentAsync(MobuUpload.MobuWs.Fragment fragment) {
            return base.Channel.UploadNextFragmentAsync(fragment);
        }
        
        public bool CheckForActiveUploads(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.CheckForActiveUploads(packageDetails);
        }
        
        public System.Threading.Tasks.Task<bool> CheckForActiveUploadsAsync(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.CheckForActiveUploadsAsync(packageDetails);
        }
        
        public bool CancelUpload(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.CancelUpload(packageDetails);
        }
        
        public System.Threading.Tasks.Task<bool> CancelUploadAsync(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.CancelUploadAsync(packageDetails);
        }
        
        public bool FinishUpload(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.FinishUpload(packageDetails);
        }
        
        public System.Threading.Tasks.Task<bool> FinishUploadAsync(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.FinishUploadAsync(packageDetails);
        }
        
        public bool TogglePackageCriticalState(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.TogglePackageCriticalState(packageDetails);
        }
        
        public System.Threading.Tasks.Task<bool> TogglePackageCriticalStateAsync(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.TogglePackageCriticalStateAsync(packageDetails);
        }
        
        public int CheckForActiveDownloads(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.CheckForActiveDownloads(packageDetails);
        }
        
        public System.Threading.Tasks.Task<int> CheckForActiveDownloadsAsync(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.CheckForActiveDownloadsAsync(packageDetails);
        }
        
        public bool CancelActiveDownloads(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.CancelActiveDownloads(packageDetails);
        }
        
        public System.Threading.Tasks.Task<bool> CancelActiveDownloadsAsync(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.CancelActiveDownloadsAsync(packageDetails);
        }
        
        public int StartDownload(MobuUpload.MobuWs.PackageDetails packageDetails, int fragmentSize) {
            return base.Channel.StartDownload(packageDetails, fragmentSize);
        }
        
        public System.Threading.Tasks.Task<int> StartDownloadAsync(MobuUpload.MobuWs.PackageDetails packageDetails, int fragmentSize) {
            return base.Channel.StartDownloadAsync(packageDetails, fragmentSize);
        }
        
        public MobuUpload.MobuWs.Fragment GetNextFragment(MobuUpload.MobuWs.PackageDetails packageDetails, int index, int fragmentSize) {
            return base.Channel.GetNextFragment(packageDetails, index, fragmentSize);
        }
        
        public System.Threading.Tasks.Task<MobuUpload.MobuWs.Fragment> GetNextFragmentAsync(MobuUpload.MobuWs.PackageDetails packageDetails, int index, int fragmentSize) {
            return base.Channel.GetNextFragmentAsync(packageDetails, index, fragmentSize);
        }
        
        public bool FinishDownload(MobuUpload.MobuWs.PackageDetails packageDetails, int fragmentSize) {
            return base.Channel.FinishDownload(packageDetails, fragmentSize);
        }
        
        public System.Threading.Tasks.Task<bool> FinishDownloadAsync(MobuUpload.MobuWs.PackageDetails packageDetails, int fragmentSize) {
            return base.Channel.FinishDownloadAsync(packageDetails, fragmentSize);
        }
        
        public System.Collections.Generic.List<MobuUpload.MobuWs.PackageDetails> GetPackageList() {
            return base.Channel.GetPackageList();
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<MobuUpload.MobuWs.PackageDetails>> GetPackageListAsync() {
            return base.Channel.GetPackageListAsync();
        }
        
        public bool RemovePackage(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.RemovePackage(packageDetails);
        }
        
        public System.Threading.Tasks.Task<bool> RemovePackageAsync(MobuUpload.MobuWs.PackageDetails packageDetails) {
            return base.Channel.RemovePackageAsync(packageDetails);
        }
    }
}
