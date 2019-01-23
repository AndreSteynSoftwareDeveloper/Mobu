using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Util;
using MobuAndroid.Models;

namespace MobuAndroid.Controls
{
    /// <summary>
    /// Class used to get, save and generate  settings  profiles used to point to different instances of the Mobu web service.
    /// </summary>
    public class SettingsControl
    {
        /// <summary>
        /// The default filename of the current settings file on the device.
        /// </summary>
        private const String SettingsFileName = "MobuSettings.config";

        /// <summary>
        /// The default filename of the list of settings profiles on the device.
        /// </summary>
        private const String SettingsProfilesFileName = "MobuSettingsProfiles.xml";

        /// <summary>
        /// Checks if any settings have been saved for this app. 
        /// </summary>
        public bool CurrentSettingsExist()
        {
            var folder = Application.Context.FilesDir.AbsolutePath;
            var fileName = Path.Combine(folder, SettingsFileName);

            return File.Exists(fileName);
        }

        /// <summary>
        /// Checks if any settings profiles have been saved for this app. 
        /// </summary>
        public bool SettingsProfilesExist()
        {
            var folder = Application.Context.FilesDir.AbsolutePath;
            var fileName = Path.Combine(folder, SettingsProfilesFileName);

            return File.Exists(fileName);
        }

        /// <summary>
        /// Saves a settings profile as the current settings to  be  used by the app.
        /// </summary>
        /// <param name="newSettings">The settings profile to be used by the app.</param>
        public bool SaveCurrentSettings(Settings newSettings)
        {
            bool result = false;

            try
            {
                var folder = Application.Context.FilesDir.AbsolutePath;
                var fileName = Path.Combine(folder, SettingsFileName);

                var writer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                var file = new StreamWriter(fileName);
                writer.Serialize(file, newSettings);
                file.Close();

                if (File.Exists(fileName))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                result = false;
                Log.Error("Mobu", "Error while saving settings.", e);
                throw new Exception("Error while saving settings. " + e.Message);
            }

            return result;
        }


        /// <summary>
        /// Saves a settings profile to the list  of available profiles.
        /// </summary>
        /// <param name="newSettings">The settings profile to be updated or added to the list.</param>
        public bool SaveSettingsProfile(Settings newSettings)
        {
            var savedProfiles = GetSettingsProfiles();

            if (savedProfiles.Any(x => x.ProfileName == newSettings.ProfileName))
            {
                var index = savedProfiles.FindIndex(x => x.ProfileName == newSettings.ProfileName);
                savedProfiles[index] = newSettings;
            }
            else
            {
                savedProfiles.Add(newSettings);
            }

           return SaveSettingsProfiles(savedProfiles);
        }

        /// <summary>
        /// Saves the current list of settings profiles.
        /// </summary>
        /// <param name="settings">The current list of available settings profiles</param>
        public bool SaveSettingsProfiles(List<Settings> settings)
        {
            bool result = false;

            try
            {
                var folder = Application.Context.FilesDir.AbsolutePath;
                var fileName = Path.Combine(folder, SettingsProfilesFileName);

                var writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Settings>));
                var file = new StreamWriter(fileName);
                writer.Serialize(file, settings);
                file.Close();

                if (File.Exists(fileName))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                result = false;
                Log.Error("Mobu", "Error while saving settings profiles.", e);
            }
            return result;
        }

        /// <summary>
        /// Returns the current settings to be used by the app, returning null if none are found.
        /// </summary>
        public Settings GetCurrentSettings()
        {
            Settings settings = null;

            var folder = Application.Context.FilesDir.AbsolutePath;
            var fileName = Path.Combine(folder, SettingsFileName);

            try
            {
                if (File.Exists(fileName))
                    {

                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                    StreamReader file = new StreamReader(fileName);
                    settings = (Settings)reader.Deserialize(file);
                    file.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error("Mobu", "Error while reading settings.", e);
            }

            return settings;
        }

        /// <summary>
        /// Returns a list of available settings profiles that can be used  by the app. Returns null if none are found.
        /// </summary>
        public List<Settings> GetSettingsProfiles()
        {
            List<Settings> profiles = null;

            var folder = Application.Context.FilesDir.AbsolutePath;
            var fileName = Path.Combine(folder, SettingsProfilesFileName);

            try
            {
                if (File.Exists(fileName))
                {

                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<Settings>));
                    StreamReader file = new StreamReader(fileName);
                    profiles = (List<Settings>)reader.Deserialize(file);
                    file.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error("Mobu", "Error while reading settings profiles.", e);
            }

            return profiles;
        }

        /// <summary>
        /// Populates and returns a list of default settings profiles that should always be available when using the app. 
        /// These profiles cannot be edited or removed.
        /// Be sure to create a valid profile with actual credentials for testing
        /// </summary>
        public List<Settings> GenerateDefaultSettings()
        {
            var profiles = new List<Settings>();

            var defaultTest = new Settings()
            {
                ProfileName = "Default Test Settings",
                Url = "http://localhost/MobuWcf/MobuWs.svc",
                Timeout = 60000,
                Domain = "Domain",
                Username = "Username",
                Password = EncryptionControl.EncryptWithDeviceID("Password"),
                FragmentSize = 500000,
                IsTestInstance = true,
                IsDefault = true
            };

            profiles.Add(defaultTest);

            SaveSettingsProfiles(profiles);

            return profiles;
        }
    }
}