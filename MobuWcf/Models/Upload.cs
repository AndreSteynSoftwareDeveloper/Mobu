using MobuWcf.Controls;
using System;
using System.Collections.Generic;

namespace MobuWcf.Models
{
    /// <summary>
    /// Model used to contain all revevant details of an active upload. 
    /// Objects of this class sit in a concurent dictionary on the web service to manage active uploads.
    /// </summary>
    public class Upload
    {
        /// <summary>
        /// Unique key of the APK being uploaded.
        /// This key contains the name, version and critical state of the APK.
        /// </summary>
        public String Key;


        /// <summary>
        /// The list of fragments being uploaded that will bee recombined into the APK when all fragments have been uploaded.
        /// </summary>
        public List<byte[]> Fragments;

        /// <summary>
        /// The total size of the APK file in bytes.
        /// </summary>
        public long PackageSize;

        /// <summary>
        /// The number of bytes being uploaded at a time.
        /// </summary>
        public int FragmentSize;

        /// <summary>
        /// The last time this upload was interacted with. 
        /// If a upload process is orphaned or forgotten for any reason, it will be cleaned up by the service.
        /// </summary>
        public DateTime LastTimeStamp;

        /// <summary>
        /// Default constructor for uploads
        /// </summary>
        /// <param name="key">Unique key for this upload in the dictionary.</param>
        /// <param name="packageSize">The total size of the APK file to be uploaded in bytes.</param>
        /// <param name="fragmentSize">The number of bytes being uploaded at a time.</param>
        public Upload(String key, int packageSize, int fragmentSize)
        {
            Key = key;
            PackageSize = packageSize;
            FragmentSize = fragmentSize;
            Fragments = new List<byte[]>();
            LastTimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Recombines and returns the complete byte array of an apk after its upload has completed.
        /// </summary>
        public byte[] CombineFragmentsIntoPackage()
        {
            try
            {
                var package = new byte[PackageSize];

                var index = 0;
                var fileSize = package.Length;

                foreach (var fragment in Fragments)
                {
                    Array.Copy(fragment, 0, package, index * FragmentSize, fragment.Length);
                    index++;
                }

                return package;
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while recombining fragments into package upload for " + Key + ".", e);
                return new byte[0];
            }
        }
    }
}