
using MobuWcf.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace MobuWcf.Models
{
    /// <summary>
    /// Model used to contain all revevant details of an active download. 
    /// Objects of this class sit in a concurent dictionary on the web service to manage active downloads.
    /// </summary>
    public class Download
    {
        /// <summary>
        /// Unique key of the APK being downloaded.
        /// This key contains the name, version and critical state of the APK.
        /// </summary>
        public string Key;

        /// <summary>
        /// Name of the app installed by the APK.
        /// </summary>
        public string Name;

        /// <summary>
        /// Version of this specific APK
        /// </summary>
        public string Version;

        /// <summary>
        /// THe number of devices currently downloading this version of the apk using a the same fragment size.
        /// </summary>
        public int ActiveDownloads;

        /// <summary>
        /// The number of bytes being downloaded at a time.
        /// </summary>
        public int FragmentSize;

        /// <summary>
        /// The list of fragments that the APK was split into when the download was initiated.
        /// </summary>
        public List<byte[]> Fragments;

        /// <summary>
        /// The last time this download was interacted with. 
        /// If a download process is orphaned or forgotten for any reason, it will be cleaned up by the service.
        /// </summary>
        public DateTime LastTimeStamp;

        /// <summary>
        /// Default constructor for downloads
        /// </summary>
        /// <param name="name">Name of the app installed by the APK</param>
        /// <param name="version">Version of this specific APK</param>
        /// <param name="package">Data of the APK</param>
        /// <param name="fragmentSize">Size of the fragments that the APK will be split into for downloads</param>
        public Download(String name, String version, Byte[] package, int fragmentSize)
        {
            Key = GetKey(name, version, fragmentSize);
            ActiveDownloads = 1;
            FragmentSize = fragmentSize;
            Fragments = SplitPackageIntoFragments(package, fragmentSize);
            LastTimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Generates a key for the dictionary of active downloads hosted by the web service.
        /// </summary>
        /// <param name="name">Name of the app installed by the APK</param>
        /// <param name="version">Version of this specific APK</param>
        /// <param name="fragmentSize">Size of the fragments that the APK will be split into for downloads</param>
        public String GetKey(String name, String version, int fragmentSize)
        {
            return name + "~" + version + "~" + fragmentSize;
        }

        /// <summary>
        /// Splits an apk into a list of fragments to be downloaded.
        /// </summary>
        /// <param name="package">Data of the APK</param>
        /// <param name="fragmentSize">Size of the fragments that the APK will be split into for downloads</param>
        /// <returns></returns>
        private List<byte[]> SplitPackageIntoFragments(Byte[] package, int fragmentSize)
        {
            try
            {
                List<byte[]> fragments = new List<byte[]>();

                var index = 0;
                var fileSize = package.Length;

                while ((index + 1) * fragmentSize < fileSize)
                {
                    var fragment = new byte[fragmentSize];
                    Array.Copy(package, index * fragmentSize, fragment, 0, fragmentSize);
                    fragments.Add(fragment);
                    index++;
                }

                if (index * fragmentSize < fileSize)
                {
                    var lastFragmennt = new byte[(fileSize - (index * fragmentSize))];
                    Array.Copy(package, index * fragmentSize, lastFragmennt, 0, fileSize - (index * fragmentSize));
                    fragments.Add(lastFragmennt);
                }

                return fragments;
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while recombining fragments into package upload for " + Key + ".", e);
                return new List<byte[]>();
            }
        }
    }
}