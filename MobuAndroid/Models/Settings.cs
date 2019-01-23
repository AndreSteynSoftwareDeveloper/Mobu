using System;

namespace MobuAndroid.Models
{
    /// <summary>
    /// Settings model to be used by Mobu. Contains webservice details user preferences for this profile.
    /// Several profiles can be saved to easily switch between instances during testing.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Default settings constructor.
        /// </summary>
        public Settings()
        {
            ProfileName = "New Profile";
            Url = String.Empty;
            Timeout = 0;
            Domain = String.Empty;
            Username = String.Empty;
            Password = String.Empty;
            IsTestInstance = false;
            FragmentSize = 0;
            IsDefault = false;
        }

        /// <summary>
        /// Basic validation to ensure all settings are populated.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !String.IsNullOrEmpty(Url) && !String.IsNullOrEmpty(Domain) && !String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(Password) && !String.IsNullOrEmpty(ProfileName) && Timeout > 0 && FragmentSize > 0;
        }

        /// <summary>
        /// Unique name for this settings profile
        /// </summary>
        public String ProfileName;

        /// <summary>
        /// Address of the mobu web service instance.
        /// </summary>
        public String Url;

        /// <summary>
        /// Amount of time to wait for web service response before timing out.
        /// </summary>
        public int Timeout;

        /// <summary>
        /// The domain of the secure user.
        /// </summary>
        public String Domain;

        /// <summary>
        /// The username of the secure user.
        /// </summary>
        public String Username;

        /// <summary>
        /// The password of the secure user. This is encrypted before being saved to file.
        /// </summary>
        public String Password;

        /// <summary>
        /// The number of bytes that will be downloaded at a time while getting an APK from the web service.
        /// </summary>
        public int FragmentSize;

        /// <summary>
        /// Indicates whether these settings point to a test server. If this is the case, it is visually indicated to the user.
        /// </summary>
        public bool IsTestInstance;

        /// <summary>
        /// Indicates whether this profile can be deleted or edited by the user.
        /// </summary>
        public bool IsDefault;
    }
}