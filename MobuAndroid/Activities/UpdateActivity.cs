using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Graphics;
using Android.Views.Animations;
using Android.Animation;
using Android.Content.PM;
using MobuAndroid.Models;
using MobuAndroid.Controls;
using MobuAndroid.MobuWebReference;
using Newtonsoft.Json;

namespace MobuAndroid.Activities
{
    /// <summary>
    /// Activity used to download new apks and is also called from external applications to update themselves.
    /// When checking for updates, this activity first checks for new versions of the specified app on the service.
    /// </summary>
    [Activity(Label = "MOBU Update", Name = "com.ces.Mobu.UpdateActivity", Exported = true, MainLauncher = false, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class UpdateActivity : Activity
    {
        /// <summary>
        /// Estimated time to complete download, based on the tme it took to download the previous fragment.
        /// </summary>
        private TimeSpan timeEstimate;

        /// <summary>
        /// True a download is currently in progress.
        /// </summary>
        private bool downloading;

        /// <summary>
        /// True if a user should be prompted to allow an update or whether is should start automatically.
        /// </summary>
        private bool displayPrompts;

        /// <summary>
        /// True is this activity should be removed from the stack when complete or cancelled. Typically true when called from an exterior app.
        /// </summary>
        private bool finishAndRemove;

        /// <summary>
        /// Details of the hosted APK that is being downloaded or updated.
        /// </summary>
        private PackageDetails packageDetails;

        /// <summary>
        /// Current settings.
        /// </summary>
        private Settings settings;

        /// <summary>
        /// Settings control object used to interact with settings.
        /// </summary>
        private SettingsControl settingsControl;

        /// <summary>
        /// Web service control object used to manage APK adownload.
        /// </summary>
        private WebServiceControl webServiceControl;

        /// <summary>
        /// FIle control object used to save and execute files on the device.
        /// </summary>
        private FileControl fileControl;

        /// <summary>
        /// References to UI controls
        /// </summary>
        private TextView txtPackageDescription;
        private TextView txtPercentage;
        private TextView txtCompletedFragments;
        private TextView txtTime;
        private TextView txtUpdateMessage;
        private ProgressBar barProgressInner;
        private ProgressBar barProgressOuter;

        /// <summary>
        /// Android method called first when an activity is launched. 
        /// Sets up the activity based on wether it was called from an external app or the Mobu main activity.
        /// </summary>
        /// <param name="savedInstanceState">Bundle containing data passed from calling activity</param>
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetupActivity();

            var serializedPackageDetails = Intent.GetStringExtra("PackageDetails");

            if (serializedPackageDetails != null)
            {
                finishAndRemove = false;
                packageDetails = JsonConvert.DeserializeObject<PackageDetails>(serializedPackageDetails);
                Download();
            }
            else
            {
                var appName = Intent.GetStringExtra("AppName") ?? "";
                var currentAppVersion = Intent.GetStringExtra("CurrentAppVersion") ?? "";
                displayPrompts = Intent.GetBooleanExtra("DisplayPrompts", false);
                finishAndRemove = true;

                var newPackage = await CheckForNewVersion(appName, currentAppVersion);

                if (newPackage == null)
                {
                    EndActivity();
                    return;
                }
                else
                {
                    packageDetails = newPackage;
                }

                if (displayPrompts)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Update");
                    alert.SetMessage("An update is available. Would you like to install it?");
                    alert.SetPositiveButton("YES", (senderAlert, args) => { Download(); });
                    alert.SetNegativeButton("NO", (senderAlert, args) => { EndActivity(); });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                else
                {
                    Download();
                }
            }
        }


        /// <summary>
        /// Sets up references to UI components, controls and settings.
        /// </summary>
        private void SetupActivity()
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);
                SetContentView(Resource.Layout.Update);

                barProgressInner = FindViewById<ProgressBar>(Resource.Id.barProgressInner);
                barProgressOuter = FindViewById<ProgressBar>(Resource.Id.barProgressOuter);
                txtCompletedFragments = FindViewById<TextView>(Resource.Id.txtFragments);
                txtPercentage = FindViewById<TextView>(Resource.Id.txtPercentage);
                txtTime = FindViewById<TextView>(Resource.Id.txtTime);
                txtPackageDescription = FindViewById<TextView>(Resource.Id.txtPackageDescription);
                txtUpdateMessage = FindViewById<TextView>(Resource.Id.txtUpdateMessage);


                barProgressInner.Visibility = ViewStates.Invisible;
                barProgressOuter.Visibility = ViewStates.Invisible;
                txtCompletedFragments.Visibility = ViewStates.Invisible;
                txtPercentage.Visibility = ViewStates.Invisible;
                txtTime.Visibility = ViewStates.Invisible;

                barProgressInner.Progress = 0;
                barProgressOuter.Progress = 0;
                txtCompletedFragments.Text = "0 / 0";
                txtPercentage.Text = "0 %";
                txtTime.Text = "0 ms";

                settingsControl = new SettingsControl();

                if (settingsControl.CurrentSettingsExist())
                {
                    settings = settingsControl.GetCurrentSettings();
                }
                else
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("No Settings");
                    alert.SetMessage("No settings profile has been setup in Mobu. Please select a settings profile in Mobu to allow updates.");
                    alert.SetNeutralButton("OK", (senderAlert, args) => { EndActivity(); });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                    return;
                }

                webServiceControl = new WebServiceControl(settings);
                fileControl = new FileControl();
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while setting up update activity.", e.Message);
                EndActivity();
            }
        }

        /// <summary>
        /// Checks for newer versions of the sp[ecified app on the server and returns it's details if any are found.
        /// </summary>
        /// <param name="appName">Name of the application</param>
        /// <param name="currentAppVersion">Current version of the app installed on the device.</param>
        private async Task<PackageDetails> CheckForNewVersion(String appName, String currentAppVersion)
        {
            try
            {
                if (String.IsNullOrEmpty(appName))
                {
                    return null;
                }

                if (String.IsNullOrEmpty(currentAppVersion))
                {
                    return null;
                }

                if (!await webServiceControl.IsWebServiceReachableAsync())
                {
                    Toast.MakeText(this, "Cannot reach the MOBU web service.", ToastLength.Long);
                    return null;
                }
                else
                {
                    var packages = await webServiceControl.GetPackageListAsync();

                    packages = packages.Where(x => x.Name.Equals(appName,StringComparison.OrdinalIgnoreCase) && new Version(x.Version) > new Version(currentAppVersion) && x.Critical).ToList();

                    if (packages.Any())
                    {
                        return packages.OrderByDescending(x => x.Version).First();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while checking for updates.", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Handles all aspects of an APK download. Starting it on th service, retrieving all fragments one by one, completing the download, saving the recombined file to app storage and executing it.
        /// Any fragment retrieval can be tried 3 times before the download fails.
        /// After the downloaded file is execued, the android OS will handle the rest of the installation.
        /// </summary>
        private async void Download()
        {
            try
            {
                txtPackageDescription.Text = packageDetails.Name + " " + packageDetails.Version;
                barProgressInner.Visibility = ViewStates.Visible;
                barProgressOuter.Visibility = ViewStates.Visible;
                txtCompletedFragments.Visibility = ViewStates.Visible;
                txtPercentage.Visibility = ViewStates.Visible;
                txtTime.Visibility = ViewStates.Visible;
                txtPackageDescription.Visibility = ViewStates.Visible;


                if (settings.IsTestInstance)
                {
                    txtUpdateMessage.SetBackgroundResource(Resource.Drawable.ContainerBackgroundRed);
                    txtUpdateMessage.Text = "DOWNLOADING FROM TEST INSTANCE";
                    txtUpdateMessage.Visibility = ViewStates.Visible;
                    txtUpdateMessage.Invalidate();
                }

                var fragments = new List<byte[]>();
                var timeStamp = DateTime.Now;

                if (!await webServiceControl.IsWebServiceReachableAsync())
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Error");
                    alert.SetMessage("Cannot reach the MOBU web service.");
                    alert.SetNeutralButton("OK", (senderAlert, args) => { EndActivity(); });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                    return;
                }
                else
                {
                    timeStamp = DateTime.Now;
                    downloading = true;
                    var fragmentCount = await webServiceControl.StartPackageDownloadAsync(packageDetails, settings.FragmentSize);
                    if (fragmentCount < 0)
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle("Error");
                        alert.SetMessage("Falied to initialise package download.");
                        alert.SetNeutralButton("OK", (senderAlert, args) => { EndActivity(); });

                        Dialog dialog = alert.Create();
                        dialog.Show();
                        return;
                    }

                    barProgressInner.ProgressDrawable.ClearColorFilter();
                    barProgressOuter.ProgressDrawable.ClearColorFilter();

                    int retryMax = 3;
                    int retrycounter = 0;
                    int currentIndex = 0;

                    barProgressInner.Progress = 0;
                    barProgressOuter.Progress = 0;
                    txtCompletedFragments.Text = "0 / " + fragmentCount;
                    txtPercentage.Text = "0 %";
                    txtTime.Text = "";
                    RunUpdateTimeEstimate();

                    while (currentIndex < fragmentCount && downloading)
                    {
                        timeEstimate = new TimeSpan((DateTime.Now - timeStamp).Ticks * (fragmentCount - currentIndex));
                        timeStamp = DateTime.Now;

                        var fragment = await webServiceControl.GetNextFragmentAsync(packageDetails, settings.FragmentSize, currentIndex);

                        while (fragment == null && retrycounter < retryMax)
                        {
                            retrycounter++;
                            timeEstimate = new TimeSpan((DateTime.Now - timeStamp).Ticks * (fragmentCount - currentIndex));
                            timeStamp = DateTime.Now;

                            fragment = await webServiceControl.GetNextFragmentAsync(packageDetails, settings.FragmentSize, currentIndex);
                        }

                        if (retrycounter == retryMax && fragment == null)
                        {
                            downloading = false;
                            txtTime.Text = "";
                            break;
                        }
                        else
                        {
                            var percentage = (int)((((float)currentIndex + 1) / (float)fragmentCount) * 100);

                            ObjectAnimator animationO = ObjectAnimator.OfInt(barProgressOuter, "progress", barProgressOuter.Progress, percentage);
                            animationO.SetDuration(100);
                            animationO.SetInterpolator(new LinearInterpolator());
                            animationO.Start();

                            ObjectAnimator animationI = ObjectAnimator.OfInt(barProgressInner, "progress", barProgressInner.Progress, percentage);
                            animationI.SetDuration(100);
                            animationI.SetInterpolator(new LinearInterpolator());
                            animationI.Start();

                            txtCompletedFragments.Text = (currentIndex + 1) + " / " + fragmentCount;
                            txtPercentage.Text = percentage.ToString() + " %";

                            fragments.Add(fragment.Data);
                            currentIndex++;
                        }
                    }

                    if (fragments.Count == fragmentCount)
                    {
                        downloading = false;
                        txtTime.Text = "";
                        await webServiceControl.FinishDownloadAsync(packageDetails, settings.FragmentSize);

                        var file = ReassemblePackage(packageDetails, settings.FragmentSize, fragments);

                        if (file != null)
                        {
                            var saved = await fileControl.SaveFileAsync(packageDetails.Name, file);
                            if (saved)
                            {
                                try
                                {
                                    fileControl.InstallFile(packageDetails.Name);

                                    Log.Info("MOBU", "Ending activity after install");
                                    EndActivity();
                                }
                                catch (Exception e)
                                {
                                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                                    alert.SetTitle("Error");
                                    alert.SetMessage("Package failed to install.");
                                    alert.SetPositiveButton("OK", (senderAlert, args) => { EndActivity(); });

                                    Dialog dialog = alert.Create();
                                    dialog.Show();
                                }
                            }
                            else
                            {
                                Log.Info("MOBU", "Saving failed");
                                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                                alert.SetTitle("Error");
                                alert.SetMessage("Package failed to save.");
                                alert.SetPositiveButton("OK", (senderAlert, args) => { EndActivity(); });

                                Dialog dialog = alert.Create();
                                dialog.Show();
                            }
                        }
                    }
                    else
                    {
                        barProgressInner.ProgressDrawable.SetColorFilter(Color.Red, PorterDuff.Mode.SrcIn);
                        barProgressOuter.ProgressDrawable.SetColorFilter(Color.Red, PorterDuff.Mode.SrcIn);

                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle("Retry?");
                        alert.SetMessage("Package download failed. Would you like to try again?");
                        alert.SetPositiveButton("YES", (senderAlert, args) => { Download(); });
                        alert.SetNegativeButton("NO", (senderAlert, args) => { EndActivity(); });

                        Dialog dialog = alert.Create();
                        dialog.Show();

                        await webServiceControl.FinishDownloadAsync(packageDetails, settings.FragmentSize);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while downloading package.", e.Message);
                await webServiceControl.FinishDownloadAsync(packageDetails, settings.FragmentSize);
            }
        }

        /// <summary>
        /// Displays the time to completion estimate of the current download.
        /// Updating the UI too frequently while downloading many small fragments make it unreadable, so it is only updated every second.
        /// </summary>
        private async void RunUpdateTimeEstimate()
        {
            while (downloading)
            {
                await Task.Delay(1000);

                if (timeEstimate.TotalSeconds <= 30)
                {
                    txtTime.Text = "A few seconds";
                }
                else if (timeEstimate.Seconds <= 90)
                {
                    txtTime.Text = "About a minute";
                }
                else
                {
                    txtTime.Text = (timeEstimate.Minutes + 1) + " minutes";
                }
            }
        }

        /// <summary>
        /// Ends the activity. It is completely removed from the stack if called from an external app.
        /// </summary>
        private void EndActivity()
        {
            if (finishAndRemove)
            {
                FinishAndRemoveTask();
            }
            else
            {
                Finish();
            }
        }

        /// <summary>
        /// Cancels the current download. 
        /// Stops retrieving fragments and cancels the download on the service.
        /// If no other devices are currently downloading this APK using the same fragment size, it is removed from memory on the server.
        /// </summary>
        private async void Cancel()
        {
            downloading = false;
            await webServiceControl.FinishDownloadAsync(packageDetails, settings.FragmentSize);
            EndActivity();
        }

        /// <summary>
        /// Recombines downloaded fragments into a byte array to be saved as an APK file.
        /// </summary>
        /// <param name="details">Details of the APK.</param>
        /// <param name="fragmentSize">Size of each individual fragment.</param>
        /// <param name="fragments">THe number of fragments this APK was split into.</param>
        /// <returns></returns>
        private byte[] ReassemblePackage(PackageDetails details, int fragmentSize, List<byte[]> fragments)
        {
            try
            {
                var index = 0;
                var fileSize = fragments.Sum(x => x.Length);
                var package = new byte[fileSize];

                foreach (var fragment in fragments)
                {
                    Array.Copy(fragment, 0, package, index * fragmentSize, fragment.Length);
                    index++;
                }

                return package;
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while downloading package.", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Android method fired when the back hardware key is pressed.
        /// Prompts the user to cancel the download in progress or simply exits the activity.
        /// </summary>
        public override void OnBackPressed()
        {
            if (downloading)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Cancel?");
                alert.SetMessage("Are you sure you want to cancel this update?");
                alert.SetPositiveButton("YES", (senderAlert, args) => { Cancel(); });
                alert.SetNegativeButton("NO", (senderAlert, args) => { });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else
            {
                EndActivity();
            }
        }
    }
}

