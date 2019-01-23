using MobuWcf.Controls;
using MobuWcf.Models;
using System.Collections.Generic;

namespace MobuWcf
{
    public class MobuWs : IMobuWs
    {
        public bool TestWebService()
        {
            return true;
        }

        #region Upload

        public bool StartUpload(PackageDetails packageDetails, int packageSize, int fragmentSize)
        {
            return TransferControl.StartUpload(packageDetails, packageSize, fragmentSize);
        }

        public bool UploadNextFragment(Fragment fragment)
        {
            return TransferControl.UploadNextFragment(fragment);
        }

        public bool CheckForActiveUploads(PackageDetails packageDetails)
        {
            return TransferControl.CheckForActiveUpload(packageDetails);
        }

        public bool CancelUpload(PackageDetails packageDetails)
        {
            return TransferControl.CancelNewUpload(packageDetails);
        }

        public bool FinishUpload(PackageDetails packageDetails)
        {
            return TransferControl.FinishUpload(packageDetails);
        }

        #endregion

        #region Download

        public bool TogglePackageCriticalState(PackageDetails packageDetails)
        {
            return FileControl.ToggleCriticalState(packageDetails);
        }

        public int CheckForActiveDownloads(PackageDetails packageDetails)
        {
            return TransferControl.GetActiveDownloadCountForPackage(packageDetails);
        }
        public bool CancelActiveDownloads(PackageDetails packageDetails)
        {
            return TransferControl.CancelActiveDownloadsForApp(packageDetails);
        }

        public int StartDownload(PackageDetails packageDetails, int fragmentSize)
        {
            return TransferControl.StartOrIncrementDownload(packageDetails, fragmentSize);
        }

        public Fragment GetNextFragment(PackageDetails packageDetails, int index, int fragmentSize)
        {
            return TransferControl.GetFragment(packageDetails, index, fragmentSize);
        }

        public bool FinishDownload(PackageDetails packageDetails, int fragmentSize)
        {
            return TransferControl.EndOrDecrementDownload(packageDetails, fragmentSize);
        }

        #endregion

        #region General

        public List<PackageDetails> GetPackageList()
        {
            return FileControl.GetPackageList();
        }

        public bool RemovePackage(PackageDetails packageDetails)
        {
            return FileControl.DeletePackage(packageDetails);
        }

        #endregion
    }
}
