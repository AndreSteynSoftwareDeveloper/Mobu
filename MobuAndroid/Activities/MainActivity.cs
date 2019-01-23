using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Content;
using Android.Graphics;
using Android;
using Android.Content.PM;
using MobuAndroid.MobuWebReference;
using MobuAndroid.Controls;
using MobuAndroid.Models;
using Newtonsoft.Json;

namespace MobuAndroid.Activities
{
    /// <summary>
    /// Main activity first displayed when the app is launched.
    /// Used to view ad select from a list of hosted APKs on the web service.
    /// When checking for updates, this activity first checks for new versions of the specified app on the service.
    /// </summary>
    [Activity(Name = "com.ces.Mobu.MainActivity", MainLauncher = true,ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        /// <summary>
        /// True if activity is currently busy updating etc. UI indicators like connectivity will not be updated while this is true.
        /// </summary>
        bool busy;

        /// <summary>
        /// True if web service can be reached.
        /// </summary>
        bool serviceIsReachable;

        /// <summary>
        /// String indicating the current status of this screen and web service.
        /// </summary>
        String serviceStatus;

        /// <summary>
        /// String ticker indicating that the service screen status is frequently updated. Prevents the look of a "frozen" screen.
        /// </summary>
        String statusIndicator;

        /// <summary>
        /// List of details describing APKs currently hosted on the web service.
        /// </summary>
        List<PackageDetails> packages;

        /// <summary>
        /// Current settings used to connect to the web service.
        /// </summary>
        Settings settings;

        /// <summary>
        /// Object used to interact with settings files on the device.
        /// </summary>
        SettingsControl settingsControl;

        /// <summary>
        /// Object used to interact with the web service.
        /// </summary>
        WebServiceControl webServiceControl;

        /// <summary>
        /// UI Component References
        /// </summary>
        TextView txtInstance;
        TextView txtServiceStatus;
        LinearLayout linAppList;

        /// <summary>
        /// Android method called first when an activity is launched. 
        /// </summary>
        /// <param name="savedInstanceState">Bundle containing data passed from calling activity</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Title = "Mobu " + PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName; ;
            SetContentView(Resource.Layout.Main);
            SetupActivity();
        }

        /// <summary>
        /// Android method called first when an activity is reloaded after being paused and resumed by the Android OS. 
        /// </summary>
        protected override async void OnResume()
        {
            base.OnResume();

            if (await webServiceControl.IsWebServiceReachableAsync())
            {
                RefreshAppList();
            }
        }

        /// <summary>
        /// Prompts the user, if needed, to allow any additional permissions required by the a
        /// </summary>
        private void AccuirePermissions()
        {
            try
            {
                var activity = (Activity)this;
                activity.RequestPermissions(new String[] 
                {
                Manifest.Permission.Internet,
                Manifest.Permission.InstallPackages
                }, 1);
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while accuiring permissions.", e.Message);
            }
        }

        /// <summary>
        /// Sets up references to UI components, controls and settings.
        /// </summary>
        void SetupActivity()
        {
            try
            {
                txtInstance = FindViewById<TextView>(Resource.Id.txtMobuInstance);
                txtServiceStatus = FindViewById<TextView>(Resource.Id.txtMobuWebStatus);
                linAppList = FindViewById<LinearLayout>(Resource.Id.linAppList);
                linAppList.RemoveAllViewsInLayout();

                settingsControl = new SettingsControl();


                if (!settingsControl.SettingsProfilesExist() || !settingsControl.CurrentSettingsExist())
                {
                    settingsControl.GenerateDefaultSettings();
                }

                ProcessOldSettings();

                if (!settingsControl.CurrentSettingsExist())
                {
                    var profiles = settingsControl.GetSettingsProfiles();
                    settings = profiles.First();
                    settingsControl.SaveCurrentSettings(settings);
                }
                else
                {
                    settings = settingsControl.GetCurrentSettings();
                }

                webServiceControl = new WebServiceControl(settings);

                if (settings.IsTestInstance)
                {
                    txtInstance.SetBackgroundResource(Resource.Drawable.ContainerBackgroundRed);
                    txtInstance.Text = "Instance: Test";
                }
                else
                {
                    txtInstance.SetBackgroundResource(Resource.Drawable.ContainerBackground);
                    txtInstance.Text = "Instance: Production";
                }

                busy = false;
                serviceIsReachable = false;
                serviceStatus = "Status: Waiting";
                statusIndicator = ".";
                packages = new List<PackageDetails>();

                RunUiIndicator();
                RunUpdateServiceStatus();
                RunUpdateStatus();
                RunAutomaticRefresh();

                AccuirePermissions();
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while setting up main activity.", e.Message);
                FinishAndRemoveTask();
            }
        }


        public void ProcessOldSettings()
        {
            var folder = Application.Context.FilesDir.AbsolutePath;
            var fileName = System.IO.Path.Combine(folder, "MobuSettings.config");

            try
            {
                if (File.Exists(fileName))
                {

                    var settingsXml = File.ReadAllText(fileName);
                    var profiles = settingsControl.GetSettingsProfiles();

                    if (settingsXml.Contains("<Profile>Test</Profile>"))
                    {
                        File.Delete(fileName);

                        settings = profiles.First(x => x.ProfileName.ToLower().Contains("test"));
                        settingsControl.SaveCurrentSettings(settings);
                        Toast.MakeText(this, "Test profile carried over " + settings.Url, ToastLength.Long).Show();
                    }

                    if (settingsXml.Contains("<Profile>Prod</Profile>"))
                    {
                        File.Delete(fileName);

                        settings = profiles.First(x => x.ProfileName.ToLower().Contains("prod"));
                        settingsControl.SaveCurrentSettings(settings);
                        Toast.MakeText(this, "Prod profile carried over " + settings.Url, ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Settings error.");
                alert.SetMessage("Could not transfer old settings. Please be sure to go to settings and specify whether this device should be running in production or test more. If this is not possible, contact your admin." + System.Environment.NewLine + System.Environment.NewLine + e.Message);
                alert.SetNeutralButton("Ok", (senderAlert, args) => {});
               
                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }

        /// <summary>
        /// Automatically refreshes the app list every 10 seconds.
        /// </summary>
        async void RunAutomaticRefresh()
        {
            while (true)
            {
                await Task.Delay(10000);

                RefreshAppList();
            }
        }

        /// <summary>
        /// Update the status display based on the current behaviour of the activity and web service.
        /// </summary>
        async void RunUiIndicator()
        {
            while (!busy)
            {
                await Task.Delay(500);

                if (statusIndicator.Length > 4)
                {
                    statusIndicator = ".";
                }
                else
                {
                    statusIndicator += ".";
                }

                    RunOnUiThread(() => { txtServiceStatus.Text = serviceStatus + statusIndicator; });
            }
        }

        /// <summary>
        /// Checks if the web service is currently reachable and sets the status.
        /// </summary>
        async void RunUpdateServiceStatus()
        {
            while (!busy)
            {
                await Task.Delay(3000);

                if (!busy)
                {
                    serviceIsReachable = await webServiceControl.IsWebServiceReachableAsync();
                }
            }
        }

        /// <summary>
        /// Sets the current status string based on the behaviour of the activity and web service.
        /// </summary>
        async void RunUpdateStatus()
        {
            while (true)
            {
                await Task.Delay(1000);

                if (!busy)
                {
                    if (serviceIsReachable)
                    {
                        serviceStatus = "Status: Connected";
                    }
                    else
                    {
                        serviceStatus = "Status: Disconnected";
                    }
                }
                else if (busy)
                {
                    serviceStatus = "Status: Busy";
                }
                else
                {
                    serviceStatus = "Status: Waiting";
                }
            }
        }

        /// <summary>
        /// Retrieves and displays the list of APKs hosted on the web service.
        /// Each APK is represented by a button.
        /// </summary>
        void RefreshAppList()
        {
            try
            {
                if (settings != null)
                {
                    if (!busy)
                    {
                        busy = true;

                        serviceIsReachable = false;
                        serviceStatus = "Status: Refreshing";
                        linAppList.RemoveAllViewsInLayout();
                        packages = new List<PackageDetails>();
                        packages = webServiceControl.GetPackageList();

                        foreach (var a in webServiceControl.GetPackageList())
                        {
                            Button btn = new Button(ApplicationContext)
                            {
                                Tag = packages.FindIndex(x => x.Name == a.Name && x.Version == a.Version),
                                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 1f)
                                {
                                    TopMargin = 10,
                                    BottomMargin = 10,
                                    LeftMargin = 0,
                                    RightMargin = 0
                                }
                            };

                            btn.SetBackgroundResource(Resource.Drawable.Button);
                            btn.SetTextColor(Color.ParseColor(Resources.GetString(Resource.Color.ButtonTextColor)));
                            btn.SetShadowLayer(10, 0, 0, Color.Black);
                            btn.Text = a.Name + " " + a.Version;
                            btn.SetAllCaps(true);
                            btn.Click += delegate { DownloadApp(btn); };

                            linAppList.AddView(btn);
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "No hosted applications found.", ToastLength.Long).Show();
                    }

                    busy = false;
                    CheckForNewerMobuVersion();
                }
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while refreshing application list.", e.Message);
                busy = false;
            }
        }

        /// <summary>
        /// Checks the web service for any newer versions of Mobu and promps the user to update.
        /// </summary>
        private void CheckForNewerMobuVersion()
        {
            var mobuPackages = packages.Where(x => x.Name.Equals("Mobu", StringComparison.OrdinalIgnoreCase) &&
            new Version(x.Version) > new Version(PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName) && x.Critical).ToList();

            if (mobuPackages.Any())
            {
                busy = true;

                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Mobu Update");
                alert.SetMessage("A newer version of Mobu is available. Do you want to install it now?");
                alert.SetPositiveButton("YES", (senderAlert, args) =>
                {
                    busy = false;
                    LaunchUpdateActivity(mobuPackages.OrderByDescending(x => x.Version).First());
                });
                alert.SetNegativeButton("NO", (senderAlert, args) =>
                {
                    busy = false;
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }

        /// <summary>
        /// Launches the Update activity to download the APK accociated with the button pressed in the apk list.
        /// The user is prompted to confirm the choice.
        /// </summary>
        /// <param name="button">The button in the APK list that was pressed. The details of the APK it represents are contained in it's tag. </param>
        void DownloadApp(Button button)
        {
            try
            {
                busy = true;
                for (int i = 0; i < linAppList.ChildCount; i++)
                {
                    View v = linAppList.GetChildAt(i);
                    v.Enabled = false;
                }

                var package = packages[(int)button.Tag];

                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Install");
                alert.SetMessage("Do you want to download and install " + button.Text + " ?");
                alert.SetPositiveButton("YES", (senderAlert, args) =>
                {
                    LaunchUpdateActivity(package);
                });
                alert.SetNegativeButton("NO", (senderAlert, args) =>
                {
                });

                Dialog dialog = alert.Create();
                dialog.Show();

                busy = false;
                for (int i = 0; i < linAppList.ChildCount; i++)
                {
                    View v = linAppList.GetChildAt(i);
                    v.Enabled = true;
                }
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while downloading package.", e.Message);
            }
        }

        /// <summary>
        /// Launches the Mobu update activity to manage downlod and installation of an update.
        /// </summary>
        /// <param name="appName">The name of the app being updated.</param>
        /// <param name="version">The new version of the app.</param>
        public void LaunchUpdateActivity(PackageDetails packageDetails)
        {
            Intent intent = new Intent();
            intent.SetComponent(new ComponentName("com.ces.Mobu", "com.ces.Mobu.UpdateActivity"));
            intent.PutExtra("PackageDetails", JsonConvert.SerializeObject(packageDetails));
            intent.PutExtra("FinishAndRemove", false);
            Application.Context.StartActivity(intent);
        }

        /// <summary>
        /// Launches the settings activity.
        /// </summary>
        public void LaunchSettingsActivity()
        {
            var intent = new Intent(this, typeof(SettingsActivity));
            StartActivity(intent);
            Finish();
        }

        /// <summary>
        /// ANdroid method used to polulate the menu/title bar.
        /// </summary>
        /// <param name="menu">Default instance of the android menu/title bar</param>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.MainMenu, menu);
            return true;
        }

        /// <summary>
        /// Android method used to react to clicked meny/title bar items.
        /// On this screen the title bar only launches the settings activity and refreshes the APK list.
        /// </summary>
        /// <param name="item">Selected menu item</param>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.menuRefresh)
            {
                RefreshAppList();
                return true;
            }
            else if (id == Resource.Id.menuSettings)
            {
                LaunchSettingsActivity();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
   
}


