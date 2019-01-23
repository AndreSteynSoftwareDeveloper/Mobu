using MobuWcf.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace MobuWcf
{
    [ServiceContract]
    public interface IMobuWs
    {
        [OperationContract]
        bool TestWebService();

        [OperationContract]
        bool StartUpload(PackageDetails packageDetails, int packageSize, int fragmentSize);

        [OperationContract]
        bool UploadNextFragment(Fragment fragment);

        [OperationContract]
        bool CheckForActiveUploads(PackageDetails packageDetails);

        [OperationContract]
        bool CancelUpload(PackageDetails packageDetails);

        [OperationContract]
        bool FinishUpload(PackageDetails packageDetails);

        [OperationContract]
        bool TogglePackageCriticalState(PackageDetails packageDetails);

        [OperationContract]
        int CheckForActiveDownloads(PackageDetails packageDetails);

        [OperationContract]
        bool CancelActiveDownloads(PackageDetails packageDetails);

        [OperationContract]
        int StartDownload(PackageDetails packageDetails, int fragmentSize);

        [OperationContract]
        Fragment GetNextFragment(PackageDetails packageDetails, int index, int fragmentSize);

        [OperationContract]
        bool FinishDownload(PackageDetails packageDetails, int fragmentSize);

        [OperationContract]
        List<PackageDetails> GetPackageList();

        [OperationContract]
        bool RemovePackage(PackageDetails packageDetails);
    }
}