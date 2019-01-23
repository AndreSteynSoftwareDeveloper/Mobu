using MobuWcf.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace MobuWcf.Controls
{
    /// <summary>
    /// Class used to manage all APK downloads and uploads.
    /// </summary>
    public class TransferControl
    {
        /// <summary>
        /// Concurrent dictionary containing all active downloads.
        /// All devices downloading a specific APK using the same fragment size interact with the same entry in this dictionary.
        /// If downloading devices are using different fragment sizes, eash fragment size will load a differently fragmented version of the requested APK into this dictionary.
        /// </summary>
        private static ConcurrentDictionary<String, Download> activeDownloads = new ConcurrentDictionary<string, Download>();

        /// <summary>
        /// Concurrent dictionary containing all active uploads.
        /// A specific version of an APK can only be uploaded from one device at a time.
        /// </summary>
        private static ConcurrentDictionary<String, Upload> activeUploads = new ConcurrentDictionary<string, Upload>();

        #region Upload Methods

        /// <summary>
        /// Generates a key for the upload dictionary from the package details.
        /// </summary>
        /// <param name="packageDetails">Details of the package being uploaded.</param>
        /// <returns></returns>
        private static String GetUploadKey(PackageDetails packageDetails)
        {
            return packageDetails.Name + "~" + packageDetails.Version;
        }

        /// <summary>
        /// Initiates an opload of a specified APK.
        /// Inserts an entyry into the upload dictionary where uploaded fragments are kept in memory until they are recompiled into the complete APK and saved.
        /// </summary>
        /// <param name="packageDetails">Details of the package being uploaded.</param>
        /// <param name="packageSize">The full size of the APK being uploaded in bytes.</param>
        /// <param name="fragmentSize">The size of the fragments being uploaded.</param>
        /// <returns></returns>
        public static bool StartUpload(PackageDetails packageDetails, int packageSize, int fragmentSize)
        {
            try
            {
                CleanUpUploads();

                var packageUpload = new Upload(GetUploadKey(packageDetails), packageSize, fragmentSize);

                if (activeUploads.ContainsKey(packageUpload.Key))
                {
                    LoggingControl.Log("Error while starting upload for " + packageDetails.Name + " " + packageDetails.Version + ". FIle is already being uploaded.");
                    return false;
                }
                else
                {
                    return activeUploads.TryAdd(packageUpload.Key, packageUpload);
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while starting upload for " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return false;
            }
        }

        /// <summary>
        /// Adds a upload fragment to the list of fragments being uploaded for this APK.
        /// </summary>
        /// <param name="fragment">Contains apk details as well as the data content of the fragment itself.</param>
        public static bool UploadNextFragment(Fragment fragment)
        {
            try
            {
                var uploadKey = GetUploadKey(fragment.PackageDetails);

                if (activeUploads.ContainsKey(uploadKey))
                {
                    activeUploads[uploadKey].LastTimeStamp = DateTime.Now;
                    activeUploads[uploadKey].Fragments.Add(fragment.Data);
                    return true;
                }
                else
                {
                    return false; ;
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while uploading next fragment for " + fragment.PackageDetails.Name + " " + fragment.PackageDetails.Version + ".", e);
                return false;
            }
        }

        /// <summary>
        /// Checks the service for any existing uploads of an APK.
        /// This helps to prevent the same APK from being uploaded at the same time.
        /// </summary>
        /// <param name="packageDetails">Details of the package being checked for active uploads.</param>
        public static bool CheckForActiveUpload(PackageDetails packageDetails)
        {
            try
            {
                CleanUpUploads();
                var uploadKey = GetUploadKey(packageDetails);
                return activeUploads.Any(x => x.Key.Equals(uploadKey, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while checking for uploads for " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return false;
            }
        }

        /// <summary>
        /// Cancels a specific upload and removes its entry from the upload dictionary.
        /// </summary>
        /// <param name="packageDetails">Details of the package upload that is being cancelled</param>
        /// <returns></returns>
        public static bool CancelNewUpload(PackageDetails packageDetails)
        {
            try
            {
                Upload packageUpload = null;

                return activeUploads.TryRemove(GetUploadKey(packageDetails), out packageUpload);
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while cancelling upload for " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return false;
            }

        }

        /// <summary>
        /// Recombines uploaded fragments into an APK file which is saved on the server before removing this entry from the upload dictionary.
        /// </summary>
        /// <param name="packageDetails">Details of the package upload that is being completed.</param>
        public static bool FinishUpload(PackageDetails packageDetails)
        {
            try
            {
                Upload upload = null;
                var uploadKey = GetUploadKey(packageDetails);

                if (activeUploads.TryGetValue(uploadKey, out upload))
                {
                    var package = upload.CombineFragmentsIntoPackage();

                    if (package.Length != upload.PackageSize)
                    {
                        LoggingControl.Log("Error while finishing upload for " + packageDetails.Name + " " + packageDetails.Version + ". Unexpected file size.");
                        return false;
                    }

                    var path = FileControl.SavePackage(packageDetails.GetPackageKey(), package);

                    if (String.IsNullOrEmpty(path))
                    {
                        LoggingControl.Log("Error while finishing upload for " + packageDetails.Name + " " + packageDetails.Version + ". Could not find key save file.");
                        return false;
                    }

                    return activeUploads.TryRemove(uploadKey, out upload);
                }
                else
                {
                    LoggingControl.Log("Error while finishing upload for " + packageDetails.Name + " " + packageDetails.Version + ". Could not find key " + uploadKey + ".");
                    return false;
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while finishing upload for " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return false;
            }
        }

        /// <summary>
        /// Removes any entries in the upload dictionary that have not been interacted with in the past 3 minutes.
        /// This ensures old entries that are incomplete for whatever reasons cannot lock out other attempts to upload an APK. 
        /// </summary>
        public static void CleanUpUploads()
        {
            try
            {
                foreach (var upload in activeUploads)
                {
                    if ((DateTime.Now - upload.Value.LastTimeStamp).TotalMinutes > 3)
                    {
                        Download forgottenPackage;
                        activeDownloads.TryRemove(upload.Key, out forgottenPackage);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while cleaning up uploads.", e);
            }
        }

        #endregion

        #region Download Methods

        /// <summary>
        /// Generates a key for the download dictionary from the package details.
        /// </summary>
        /// <param name="packageDetails">Details of the package being downloaded.</param>
        /// <param name="fragmentSize">The size, in bytes, of the fragments that the APK file on the server will be split into before being downloaded.</param>
        /// <returns></returns>
        private static String GetDownloadKey(PackageDetails packageDetails, int fragmentSize)
        {
            return packageDetails.Name + "~" + packageDetails.Version + "~" + fragmentSize;
        }

        /// <summary>
        /// Checks for active downloads of a specific APK. This allows the users altering or removing hosted APKs to be warned if an APK is currently in use.
        /// </summary>
        /// <param name="packageDetails">Details of the package being checked for active downloads.</param>
        public static int GetActiveDownloadCountForPackage(PackageDetails packageDetails)
        {
            try
            {
                CleanUpDownloads();

                var downloads = activeDownloads.Where(x => x.Value.Name == packageDetails.Name && x.Value.Version == packageDetails.Version).Sum(x => x.Value.ActiveDownloads);
                return downloads;
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while checking for downloads for " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return -1;
            }
        }

        /// <summary>
        /// Cancels all downloads of a specific APK and removes its entry from the download dictionary.
        /// This cancels all downloads of this version of an app, regardless of the requested fragment size.
        /// </summary>
        /// <param name="packageDetails">Details of the package being cancelled.</param>
        public static bool CancelActiveDownloadsForApp(PackageDetails packageDetails)
        {
            try
            {
                var downloads = activeDownloads.Where(x => x.Value.Name == packageDetails.Name && x.Value.Version == packageDetails.Version);

                foreach (var x in downloads)
                {
                    if (!activeDownloads.TryRemove(x.Key, out Download cancelledApp))
                    {
                        LoggingControl.Log("Error while cancelling download for " + x + ".");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while cancelling downloads for " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return false;
            }
        }

        /// <summary>
        /// Initiates an download of a specified APK.
        /// Inserts an entry into the upload dictionary where download fragments are kept in memory while they are being sent to devices.
        /// Returns a count of fragments to be expected for this APK when using this fragment size.
        /// </summary>
        /// <param name="packageDetails">Details of the package being downloaded.</param>
        /// <param name="fragmentSize">The size of the fragments being downloaded as requested by the device.</param>
        public static int StartOrIncrementDownload(PackageDetails packageDetails, int fragmentSize)
        {
            try
            {
                CleanUpDownloads();

                Download packageDownload = null;

                var downloadKey = GetDownloadKey(packageDetails, fragmentSize);
                var packageKey = packageDetails.GetPackageKey();
                var package = new byte[0];

                if (FileControl.GetPackageList().Any(x => x.GetPackageKey().Equals(packageKey, StringComparison.OrdinalIgnoreCase)))
                {
                    var path = FileControl.GetPackagePath(packageKey);

                    if (File.Exists(path))
                    {
                        package = FileControl.GetPackage(path);

                        if (activeDownloads.TryGetValue(downloadKey, out packageDownload))
                        {
                            packageDownload.ActiveDownloads++;
                            packageDownload.LastTimeStamp = DateTime.Now;
                            return packageDownload.Fragments.Count;
                        }
                        else
                        {
                            packageDownload = new Download(packageDetails.Name, packageDetails.Version, package, fragmentSize);

                            if (activeDownloads.TryAdd(downloadKey, packageDownload))
                            {
                                return packageDownload.Fragments.Count;
                            }
                            else
                            {
                                LoggingControl.Log("Error while starting download for " + packageDetails.Name + " " + packageDetails.Version + ". Could not add instance to download list.");
                            }
                        }
                    }
                    else
                    {
                        LoggingControl.Log("Error while starting download for " + packageDetails.Name + " " + packageDetails.Version + ". Could not find file.");
                    }
                }
                else
                {
                    LoggingControl.Log("Error while cancelling downloads for " + packageDetails.Name + " " + packageDetails.Version + ". Could not find package in list.");
                }

                return -1;
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while cancelling downloads for " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return -1;
            }
        }

        /// <summary>
        /// Retrieves a specific fragment to be sent to the device.
        /// </summary>
        /// <param name="packageDetails">Details of the package being downloaded.</param>
        /// <param name="index">Index of the specific fragment </param>
        /// <param name="fragmentSize">Fragment size requested by the device</param>
        public static Fragment GetFragment(PackageDetails packageDetails, int index, int fragmentSize)
        {
            try
            {
                Download package = null;
                var downloadKey = GetDownloadKey(packageDetails, fragmentSize);

                if (activeDownloads.TryGetValue(downloadKey, out package))
                {
                    package.LastTimeStamp = DateTime.Now;
                    return new Fragment(packageDetails, index, package.Fragments[index]);
                }
                else
                {
                    LoggingControl.Log("Error while getting next download fragment for " + packageDetails.Name + " " + packageDetails.Version + ". Could not find entry in download list.");
                    return null;
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while getting next download fragment for " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return null;
            }
        }

        /// <summary>
        /// Removes this download entry from the download dictionary if only one device is busy downloading or simply decremtents the count otherwise..
        /// </summary>
        /// <param name="packageDetails">Details of the package download that is being completed.</param>
        public static bool EndOrDecrementDownload(PackageDetails packageDetails, int fragmentSize)
        {
            try
            {
                var packageKey = packageDetails.GetPackageKey();
                var downloadKey = GetDownloadKey(packageDetails, fragmentSize);

                Download download;

                if (activeDownloads.TryGetValue(downloadKey, out download))
                {
                    download.LastTimeStamp = DateTime.Now;
                    download.ActiveDownloads--;

                    if (download.ActiveDownloads <= 0)
                    {
                        return activeDownloads.TryRemove(packageKey, out download);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    LoggingControl.Log("Error while ending download for " + packageDetails.Name + " " + packageDetails.Version + ". Entry no longer exists in download list.");
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while ending download for  " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return false;
            }
        }

        /// <summary>
        /// Removes any entries in the download dictionary that have not been interacted with in the past 3 minutes.
        /// This ensures old entries that are incomplete for whatever reason will not remain on the server indefinately.
        /// </summary>
        public static void CleanUpDownloads()
        {
            try
            {
                foreach (var download in activeDownloads)
                {
                    if ((DateTime.Now - download.Value.LastTimeStamp).TotalMinutes > 3)
                    {
                        Download forgottenPackage;
                        activeDownloads.TryRemove(download.Key, out forgottenPackage);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while cleaning up downloads.", e);
            }
        }

        #endregion
    }
}