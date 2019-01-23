using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.Util;
using MobuAndroid.Models;
using MobuAndroid.MobuWebReference;

namespace MobuAndroid.Controls
{
    /// <summary>
    /// Class used to call web service methods to check currently hosted APKs and and download these apks from the service.
    /// </summary>
    public class WebServiceControl
    {
        /// <summary>
        /// Mobu web service reference used to download APKs.
        /// </summary>
        private MobuWs WebService;

        /// <summary>
        /// Constructor for the web service control.
        /// </summary>
        /// <param name="settings">Current app settings used to setup the web service.</param>
        public WebServiceControl(Settings settings)
        {
            try
            {
                if (settings != null)
                {
                    WebService = new MobuWs
                    {
                        Url = settings.Url,
                        Timeout = settings.Timeout,
                        Credentials = new NetworkCredential(settings.Username, EncryptionControl.DecryptWithDeviceID(settings.Password), settings.Domain),
                        PreAuthenticate = false,
                        EnableDecompression = true
                    };
                }
            }
            catch (Exception e)
            {
                Log.Error("Mobu", "Error while creating web service.", e);
                throw new Exception("Error while creating web service. " + e.Message);
            }
        }

        /// <summary>
        /// Return true if web service is reachable and can be interacted with using current settings.
        /// </summary>
        public bool IsWebServiceReachable()
        {
            bool t;
            try
            {
                WebService.TestWebService(out t, out bool tSpecified);
            }
            catch (Exception e)
            {
                t = false;
            }

            return t;
        }

        /// <summary>
        /// Async implementation of IsWebServiceReachable. Return true if web service is reachable and can be interacted with using current settings.
        /// </summary>
        public async Task<bool> IsWebServiceReachableAsync()
        {
            bool result = false;

            await Task.Run(() =>
            {
                result = IsWebServiceReachable();
            });

            return result;
        }

        /// <summary>
        /// Retrieves a list of package details from the service describing currently hosted APKs.
        /// </summary>
        public List<PackageDetails> GetPackageList()
        {
            try
            {
                return  WebService.GetPackageList().ToList(); ;
            }
            catch (Exception e)
            {
                Log.Error("Mobu", "Error while getting package information set from web service.", e);
                throw new Exception("Error while getting package information set from web service. " + e.Message);
            }
        }



        /// <summary>
        /// Async implementation of GetPackageList. Retrieves a list of package details from the service describing currently hosted APKs.
        /// </summary>
        public async Task<List<PackageDetails>> GetPackageListAsync()
        {
            var result = new List<PackageDetails>();

            await Task.Run(() =>
            {
                result = GetPackageList();
            });

            return result;
        }

        /// <summary>
        /// Async implementation of StartPackageDownload.
        /// Initiates a download of this apk on the web service. This loads a list of fragments into memory to be retrieved one by one.
        /// Returns the number of fragments to be downloaded.
        /// </summary>
        /// <param name="packageDetails">The details of the APK that will be downloaded.</param>
        /// <param name="fragmentSize">The size of the fragments that the APK will be split into on the service.</param>
        public async Task<int> StartPackageDownloadAsync(PackageDetails packageDetails, int fragmentSize)
        {
            var fragmentCount = -1;

            await Task.Run(() =>
            {
                fragmentCount = StartPackageDownload(packageDetails, fragmentSize);
            });

            return fragmentCount;
        }

        /// <summary>
        /// Initiates a download of this apk on the web service. This loads a list of fragments into memory to be retrieved one by one.
        /// Returns the number of fragments to be downloaded.
        /// </summary>
        /// <param name="packageDetails">The details of the APK that will be downloaded.</param>
        /// <param name="fragmentSize">The size of the fragments that the APK will be split into on the service.</param>
        public int StartPackageDownload(PackageDetails packageDetails, int fragmentSize)
        {
            var result = -1;

            try
            {
               WebService.StartDownload(packageDetails, fragmentSize, true, out result, out bool resultSpecified);
            }
            catch (Exception e)
            {
                Log.Error("Mobu", "Error while getting latest installer from web service.", e);
                throw new Exception("Error while getting latest installer from web service. " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Async implementation of GetNextFragment.
        /// Retrieves the next fragment to be downloaded from the web service.
        /// </summary>
        /// <param name="packageDetails">The details of the APK that is being downloaded.</param>
        /// <param name="fragmentSize">The fragment size used to download this APK.</param>
        /// <param name="index">The index of fragment of the specified size that is being retrieved.</param>
        public async Task<Fragment> GetNextFragmentAsync(PackageDetails packageDetails, int fragmentSize, int index)
        {
            var result = new Fragment();

            await Task.Run(() =>
            {
                result = GetNextFragment(packageDetails, fragmentSize, index);
            });

            return result;
        }

        /// <summary>
        /// Retrieves the next fragment to be downloaded from the web service.
        /// </summary>
        /// <param name="packageDetails">The details of the APK that is being downloaded.</param>
        /// <param name="fragmentSize">The fragment size used to download this APK.</param>
        /// <param name="index">The index of fragment of the specified size that is being retrieved.</param>
        public Fragment GetNextFragment(PackageDetails packageDetails, int fragmentSize, int index)
        {
            try
            {
                return WebService.GetNextFragment(packageDetails, index, true, fragmentSize, true);
            }
            catch (Exception e)
            {
                Log.Error("Mobu", "Error while getting fragment " + index + " for " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                throw new Exception("Error while getting fragment " + index + " for " + packageDetails.Name + " " + packageDetails.Version + "." + e.Message);
            }
        }

        /// <summary>
        /// Async implementation of FinishDownload. 
        /// Completes the download on the web service side, cleaning up the fragment from memory if they are not being downloaded by another device.
        /// </summary>
        /// <param name="packageDetails">The details of the APK that is being downloaded.</param>
        /// <param name="fragmentSize">The fragment size used to download this APK. Indicates  the specific fragment set that needs to be removed from memory on the web service</param>
        public async Task<bool> FinishDownloadAsync(PackageDetails packageDetails, int fragmentSize)
        {
            var result = false;

            await Task.Run(() =>
            {
                result = FinishDownload(packageDetails, fragmentSize);
            });

            return result;
        }

        /// <summary>
        /// Completes the download on the web service side, cleaning up the fragment from memory if they are not being downloaded by another device.
        /// </summary>
        /// <param name="packageDetails">The details of the APK that is being downloaded.</param>
        /// <param name="fragmentSize">The fragment size used to download this APK. Indicates  the specific fragment set that needs to be removed from memory on the web service</param>
        public bool FinishDownload(PackageDetails packageDetails, int fragmentSize)
        {
            var result = false;

            try
            {
               WebService.FinishDownload(packageDetails, fragmentSize, true, out result, out bool resultSpecified);
            }
            catch (Exception e)
            {
                Log.Error("Mobu", "Error while finishing download of " + packageDetails.Name + " " + packageDetails.Version + ".", e);
            }

            return result;
        }
    }
}