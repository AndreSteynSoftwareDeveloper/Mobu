using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Util;

namespace MobuAndroid.Controls
{
    /// <summary>
    /// Class used to save and execute APK files on the device.
    /// </summary>
    public class FileControl
    {
        /// <summary>
        /// Async implementation of SaveFile
        /// Saves a file to app storage on the device
        /// </summary>
        /// <param name="appName">The name of the app APK to be saved. Different versions of the same app APKs will overwrite eachother.</param>
        /// <param name="file">The byte array of the APK to be saved.</param>
        public async Task<bool> SaveFileAsync(String appName, byte[] file)
        {
            var result = false;

            await Task.Run(() =>
            {
                result = SaveFile(appName, file);
            });

            return result;
        }

        /// <summary>
        /// Saves a file to app storage on the device
        /// </summary>
        /// <param name="appName">The name of the app APK to be saved. Different versions of the same app APKs will overwrite eachother.</param>
        /// <param name="file">The byte array of the APK to be saved.</param>
        public bool SaveFile(String appName, byte[] file)
        {
            bool result = false;

            try
            {
                var folder = Application.Context.FilesDir.AbsolutePath;

                var fileName = Path.Combine(folder, appName + ".apk");

                File.WriteAllBytes((fileName), file);

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
                Log.Error("Mobu", "Error while saving file.", e);
                throw new Exception("Error while saving file. " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Executes a saved apk on the device to install it.
        /// More recent versions of android will use the new file provider to execute files saved in app storage.
        /// </summary>
        /// <param name="appName"></param>
        public void InstallFile(String appName)
        {
            try
            {
                var folder = Application.Context.FilesDir.AbsolutePath;
                var fileName = Path.Combine(folder, appName + ".apk");

                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    var file = new Java.IO.File(fileName);
                    var downloadUri = FileProvider.GetUriForFile(Application.Context, Application.Context.PackageName + ".fileprovider", file);
                    Intent install = new Intent(Intent.ActionInstallPackage);
                    install.AddFlags(ActivityFlags.GrantReadUriPermission);
                    install.AddFlags(ActivityFlags.GrantWriteUriPermission);
                    install.AddFlags(ActivityFlags.GrantPersistableUriPermission);
                    install.SetDataAndType(downloadUri, "application/vnd.android.package-archive");
                    Application.Context.StartActivity(install);
                }
                else
                {
                    Intent promptInstall = new Intent(Intent.ActionView).SetDataAndType(Android.Net.Uri.Parse("file://" + fileName), "application/vnd.android.package-archive");
                    Application.Context.StartActivity(promptInstall);
                }
            }
            catch (Exception e)
            {
                Log.Error("Mobu", "Error while installing file .", e);
                throw new Exception("Error while installing file. " + e.Message);
            }
        }
    }
}