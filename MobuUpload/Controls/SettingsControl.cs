using System;
using System.IO;
using System.Collections.Generic;
using MobuUpload.Models;

namespace MobuUpload.Controls
{
    /// <summary>
    /// Class used to get, save and generate settings profiles used to point to different instances of the Mobu web service.
    /// </summary>
    public class SettingsControl
    {
        /// <summary>
        /// The default filename of the current settings file being used by the application.
        /// </summary>
        private string DefaultSettingsFileLocation
        {
            get
            {
                var executingDir = AppDomain.CurrentDomain.BaseDirectory;
                var executingDirInfo = new DirectoryInfo(executingDir);
                var defaultSettingsFileLocation = executingDirInfo.FullName + "\\Settings";

                return defaultSettingsFileLocation;
            }
        }

        /// <summary>
        /// The default filename of the list of settings profiles on the device.
        /// </summary>
        private string DefaultSettingsProfilesLocation
        {
            get
            {
                var executingDir = AppDomain.CurrentDomain.BaseDirectory;
                var executingDirInfo = new DirectoryInfo(executingDir);
                var defaultSettingsProfilesLocation = executingDirInfo.FullName + "\\SettingsProfiles";

                return defaultSettingsProfilesLocation;
            }
        }

        /// <summary>
        /// Checks if any settings have been saved for this application. 
        /// </summary>
        public bool SettingsExist()
        {
            return File.Exists(DefaultSettingsFileLocation);
        }

        /// <summary>
        /// Checks if any settings profiles have been saved for this application. 
        /// </summary>
        public bool SettingsProfilesExist()
        {
            return File.Exists(DefaultSettingsProfilesLocation);
        }

        /// <summary>
        /// Saves a settings profile as the current settings to  be  used by the application.
        /// </summary>
        /// <param name="settings">The settings profile to be used by the application.</param>
        public bool SaveSettings(Settings settings)
        {
            try
            {
                var writer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                var file = new StreamWriter(DefaultSettingsFileLocation);
                writer.Serialize(file, settings);
                file.Close();

                return true;
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while saving settings", e);
                return false;
            }
        }

        /// <summary>
        /// Saves the list of settings profiles.
        /// </summary>
        /// <param name="settingsProfiles">The list of available settings profiles</param>
        public bool SaveSettingsProfiles(List<Settings> settingsProfiles)
        {
            try
            {
                var writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Settings>));
                var file = new StreamWriter(DefaultSettingsProfilesLocation);
                writer.Serialize(file, settingsProfiles);
                file.Close();

                return true;
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while saving settings", e);
                return false;
            }
        }


        /// <summary>
        /// Returns the settings to be used by the application, returning null if none are found.
        /// </summary>
        public Settings GetSettings()
        {
            try
            {
                if (!File.Exists(DefaultSettingsFileLocation))
                {
                    return null;
                }

                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                StreamReader file = new StreamReader(DefaultSettingsFileLocation);
                Settings settings = (Settings)reader.Deserialize(file);
                file.Close();

                return settings;
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while retrieving settings", e);
                return null;
            }
        }

        /// <summary>
        /// Returns a list of available settings profiles that can be used  by the app. Returns null if none are found.
        /// </summary>
        public List<Settings> GetSettingsProfiles()
        {
            try
            {
                if (!File.Exists(DefaultSettingsProfilesLocation))
                {
                    return null;
                }

                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<Settings>));
                StreamReader file = new StreamReader(DefaultSettingsProfilesLocation);
                var settingsProfiles = (List<Settings>)reader.Deserialize(file);
                file.Close();

                return settingsProfiles;
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while retrieving settings profiles", e);
                return null;
            }
        }

        /// <summary>
        /// Populates and returns a list of default settings profiles that should always be available when using the application. 
        /// These profiles cannot be edited or removed.
        /// Be sure to create a valid profile with actual credentials for testing
        /// </summary>
        public List<Settings> GenerateDefaultSettingsProfiles()
        {
            var profiles = new List<Settings>();

            var defaultTest = new Settings()
            {
                ProfileName = "Default Test Settings",
                WebServiceUrl = "http://localhost/MobuWcf/MobuWs.svc",
                WebServiceTimeout = 60000,
                IsDefault = true
            };

            profiles.Add(defaultTest);

            SaveSettingsProfiles(profiles);

            return profiles;
        }
    }
}
