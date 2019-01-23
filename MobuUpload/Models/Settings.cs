using System;

namespace MobuUpload.Models
{
    /// <summary>
    /// Settings model used to save connection details to different instances of the Mobu web service.
    /// Currently it is assumed that this application will be run on a machine on the same domain as the web service and that default credentials can be used.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Unique name of a settings profile.
        /// </summary>
        public String ProfileName;

        /// <summary>
        /// Url of the Mobu web service.
        /// </summary>
        public String WebServiceUrl;

        /// <summary>
        /// Amount of time in millseconds to wait for a response before failing a web service call.
        /// </summary>
        public int WebServiceTimeout;

        /// <summary>
        /// Indicates whether this settings profile can be edited and deleted.
        /// </summary>
        public bool IsDefault;

        /// <summary>
        /// Default constructor for settings.
        /// </summary>
        public Settings()
        {
            ProfileName = "Default";
            WebServiceUrl = "http://127.0.0.1/MobuWcf/MobuWs.svc";
            WebServiceTimeout = 300000;
            IsDefault = false;
        }
    }
}
