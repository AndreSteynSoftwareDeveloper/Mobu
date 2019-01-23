using System;
using System.Runtime.Serialization;

namespace MobuWcf
{
    /// Model used to contain all details of a hosted APK.
    [DataContract]
    public class PackageDetails
    {
        /// <summary>
        /// Name of the app installed by the APK.
        /// </summary>
        [DataMember]
        public string Name;

        /// <summary>
        /// Version of this specific APK
        /// </summary>
        [DataMember]
        public string Version;

        /// <summary>
        /// Critical state of this APK.
        /// This flag specifies whether an app will automatically update itself with this APK if it is newer.
        /// Settings this to false will allow mtesters to manually test new releases before they are pushed to all devices by setting this to true.
        /// </summary>
        [DataMember]
        public bool Critical;

        /// <summary>
        /// Default constructor for package details.
        /// </summary>
        public PackageDetails(String name, String version, long size, bool autoUpdate)
        {
            Name = name;
            Version = version;
            Critical = autoUpdate;
        }

        /// <summary>
        /// Constructs package details from the filepath.
        /// </summary>
        /// <param name="filePath">Path of the APK file on the server.</param>
        public PackageDetails(String filePath)
        {
            GenerateFromPath(filePath);
        }

        /// <summary>
        /// Generates a string from this object.
        /// This is used as the filename which also contains all relevant information of the APK.
        /// Previously this was stored in a seperate catalog file, but that was just another item that could become outdated.
        /// </summary>
        public String GetPackageKey()
        {
            Name = Name.Replace(' ', '_');
            Name = Name.Replace('.', '_');
            Name = Name.Replace("~","");
            Version = Version.Replace('.', '_');

            return Name + "~" + Version + "~" + (Critical ? "1" : "0");
        }

        /// <summary>
        /// Parses out the APK file path to populate this detail object.
        /// </summary>
        /// <param name="filePath">Path of the APK file on the server.</param>
        private void GenerateFromPath(String path)
        {
            path = path.Remove(0, path.LastIndexOf('\\') + 1);
            Name = path.Substring(0, path.IndexOf('~'));

            Version = path.Substring(path.IndexOf('~') + 1, path.LastIndexOf('~') - path.IndexOf('~') - 1);
            Critical = path.Substring(path.LastIndexOf('~') + 1, 1) == "1";

            Name = Name.Replace('_', ' ');
            Version = Version.Replace('_', '.');
        }
    }
}