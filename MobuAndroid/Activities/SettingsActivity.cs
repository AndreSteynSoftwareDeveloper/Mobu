using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using MobuAndroid.Controls;
using MobuAndroid.Models;

namespace MobuAndroid.Activities
{
    /// <summary>
    /// Activity used to create, edit, select and delete settigns profiles.
    /// Actually editing any settings is locked by default and is unlocked by rapidly tapping the "Profiles" label.
    /// This s similar to the way developer options are unlocked on android devices.
    /// </summary>
    [Activity(Label = "MOBU Settings", Name = "com.ces.Mobu.SettingsActivity", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : Activity
    {
        /// <summary>
        /// The number of taps on the "Profile" label required to unlock the activity for editing.
        /// </summary>
        private const int touchRequired = 20;

        /// <summary>
        /// The current number of taps on the "Profile" label.
        /// </summary>
        private int touchCount;

        /// <summary>
        /// Popup text view used throughout the activity and to indicate the number of taps remaining to unlock the creen.
        /// </summary>
        private Toast SettingToast;

        /// <summary>
        /// True if the activity has been unlocked for editing.
        /// </summary>
        private bool customEnabled;

        /// <summary>
        /// The selected settings profile.
        /// </summary>
        private Settings currentProfile;

        /// <summary>
        /// A list of previously saved settings profiles.
        /// </summary>
        private List<Settings> profiles;

        /// <summary>
        /// Object used to interact with settings files on the device.
        /// </summary>
        private SettingsControl settingsControl;

        /// <summary>
        /// UI component references.
        /// </summary>
        private TextView txtProfiles;
        private Spinner spinProfiles;
        private EditText edtBackendUrl;
        private EditText edtBackendTimeout;
        private EditText edtDomain;
        private EditText edtUsername;
        private EditText edtPassword;
        private EditText edtFragmentSize;
        private CheckBox chkIsTestInstance;
        private TextView txtProfileName;
        private EditText edtProfileName;
        private LinearLayout linButtons;
        private Button btnSave;
        private Button btnCancel;
        private Button btnDelete;

        /// <summary>
        /// Android method called first when an activity is launched. 
        /// Sets up the activity with current settings.
        /// </summary>
        /// <param name="savedInstanceState">Bundle containing data passed from calling activity</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Set our view from the "main" layout resource
                SetContentView(Resource.Layout.Settings);

                settingsControl = new SettingsControl();
                profiles = settingsControl.GetSettingsProfiles();

                txtProfiles = FindViewById<TextView>(Resource.Id.txtProfiles);
                txtProfiles.Click += delegate { EnableCustomProfiles(); };
                spinProfiles = FindViewById<Spinner>(Resource.Id.spinProfiles);
                spinProfiles.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, profiles.Select(x => x.ProfileName).ToList());
                spinProfiles.ItemSelected += delegate { PopulateForm(profiles[spinProfiles.SelectedItemPosition]); };
                edtBackendUrl = FindViewById<EditText>(Resource.Id.edtBackendUrl);
                edtBackendTimeout = FindViewById<EditText>(Resource.Id.edtBackendTimeout);
                edtDomain = FindViewById<EditText>(Resource.Id.edtDomain);
                edtUsername = FindViewById<EditText>(Resource.Id.edtUsername);
                edtPassword = FindViewById<EditText>(Resource.Id.edtPassword);
                edtFragmentSize = FindViewById<EditText>(Resource.Id.edtFragmentSize);
                chkIsTestInstance = FindViewById<CheckBox>(Resource.Id.chkTestInstance);
                txtProfileName = FindViewById<TextView>(Resource.Id.txtProfileName);
                edtProfileName = FindViewById<EditText>(Resource.Id.edtProfileName);
                linButtons = FindViewById<LinearLayout>(Resource.Id.linButtons);
                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                btnSave.Click += delegate { SaveProfile(); };
                btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
                btnCancel.Click += delegate { PopulateForm(currentProfile); };
                btnDelete = FindViewById<Button>(Resource.Id.btnDelete);
                btnDelete.Click += delegate { DeleteCustomProfile(); };

                SettingToast = new Toast(this.ApplicationContext);
                Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);

                currentProfile = settingsControl.GetCurrentSettings();

                var settingsIndex = profiles.FindIndex(x => x.ProfileName == currentProfile.ProfileName);

                if (settingsIndex >= 0)
                {
                    spinProfiles.SetSelection(profiles.FindIndex(x => x.ProfileName == currentProfile.ProfileName));
                }
                else
                {
                    spinProfiles.SetSelection(0);
                }
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while setting up settings activity.", e.Message);
                Finish();
            }
        }

        /// <summary>
        /// Populates form fields with a settings profile.
        /// </summary>
        /// <param name="settings">Settings profile that should be displayed on the form./</param>
        private void PopulateForm(Settings settings)
        {
            try
            {
                currentProfile = settings;
                edtBackendUrl.Text = settings.Url;
                edtBackendTimeout.Text = settings.Timeout.ToString();
                edtDomain.Text = settings.Domain;
                edtUsername.Text = settings.Username;
                edtPassword.Text = EncryptionControl.DecryptWithDeviceID(settings.Password);
                edtFragmentSize.Text = settings.FragmentSize.ToString();
                chkIsTestInstance.Checked = settings.IsTestInstance;
                edtProfileName.Text = settings.ProfileName;

                if (!customEnabled)
                {
                    edtBackendUrl.Enabled = false;
                    edtBackendTimeout.Enabled = false;
                    edtDomain.Enabled = false;
                    edtUsername.Enabled = false;
                    edtPassword.Enabled = false;
                    edtFragmentSize.Enabled = false;
                    chkIsTestInstance.Enabled = false;
                    txtProfileName.Visibility = ViewStates.Invisible;
                    edtProfileName.Visibility = ViewStates.Invisible;
                    linButtons.Visibility = ViewStates.Invisible;
                }
                else
                {
                    edtBackendUrl.Enabled = true;
                    edtBackendTimeout.Enabled = true;
                    edtDomain.Enabled = true;
                    edtUsername.Enabled = true;
                    edtPassword.Enabled = true;
                    edtFragmentSize.Enabled = true;
                    chkIsTestInstance.Enabled = true;
                    txtProfileName.Visibility = ViewStates.Visible;
                    edtProfileName.Visibility = ViewStates.Visible;
                    linButtons.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while populating settings form.", e.Message);
            }
        }

        /// <summary>
        /// Applies the selected profile as the settings to be used by the app and notifies the user.
        /// </summary>
        private void ApplyCurrentSettings()
        {
            if (settingsControl.SaveCurrentSettings(currentProfile))
            {
                Toast.MakeText(Application.Context, "Settings profile " + currentProfile.ProfileName + " has been applied.", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(Application.Context, "Settings profile " + currentProfile.ProfileName + " has could not be applied.", ToastLength.Long).Show();
            }
        }

        /// <summary>
        /// Saves the details on screen to a new or existing profile to file after validating it.
        /// Default profiles cannot be edited.
        /// </summary>
        private void SaveProfile()
        {
            var newSettings = new Settings();

            try
            {
                newSettings.ProfileName = spinProfiles.SelectedItem.ToString().Trim();
                newSettings.Url = edtBackendUrl.Text.Trim();
                newSettings.Timeout = Convert.ToInt32(edtBackendTimeout.Text);
                newSettings.Domain = edtDomain.Text;
                newSettings.Username = edtUsername.Text;
                newSettings.Password = EncryptionControl.EncryptWithDeviceID(edtPassword.Text);
                newSettings.FragmentSize = Convert.ToInt32(edtFragmentSize.Text);
                newSettings.IsTestInstance = chkIsTestInstance.Checked;
            }
            catch (Exception e)
            {
                Log.Info("MobuAndroid", "Error while saving invalid settings.", e.Message);
            }

            if (!String.IsNullOrEmpty(edtProfileName.Text) && edtProfileName.Text != spinProfiles.SelectedItem.ToString())
            {
                newSettings.ProfileName = edtProfileName.Text;
            }

            if (profiles.Any((x => x.IsDefault && x.ProfileName == newSettings.ProfileName)))
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Cannot Change Default Settings");
                alert.SetMessage("You cannot change a default settings profile. Please enter a different name for this new settings profile.");
                alert.SetNeutralButton("OK", (senderAlert, args) => { });
                Dialog dialog = alert.Create();
                dialog.Show();
                return;
            }

            if (!newSettings.IsValid())
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Invalid Settings");
                alert.SetMessage("Please complete all fields before saving settings.");
                alert.SetNeutralButton("OK", (senderAlert, args) => { });
                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else
            {
                if (settingsControl.SaveSettingsProfile(newSettings))
                {
                    currentProfile = newSettings;

                    profiles = settingsControl.GetSettingsProfiles();
                    spinProfiles.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, profiles.Select(x => x.ProfileName).ToList());
                    spinProfiles.SetSelection(profiles.FindIndex(x => x.ProfileName == currentProfile.ProfileName));

                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Profile saved");
                    alert.SetMessage("Settings profile " + currentProfile.ProfileName + " saved.");
                    alert.SetNeutralButton("OK", (senderAlert, args) => { });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                else
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Save Failed");
                    alert.SetMessage("Settings profile " + currentProfile.ProfileName + " could not be saved.");
                    alert.SetNeutralButton("OK", (senderAlert, args) => { });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
            }
        }

        /// <summary>
        /// Unlocks the screen for editing or creation of new profiles.
        /// </summary>
        private void EnableCustomProfiles()
        {
            if (touchCount < touchRequired)
            {
                touchCount++;

                if (touchCount > (touchRequired / 2))
                {
                    SettingToast = Toast.MakeText(this.ApplicationContext, "Tap " + (touchRequired - touchCount + 1) + " more times to enable custom settings", ToastLength.Short);
                    SettingToast.Show();
                }
            }
            else if (touchCount == touchRequired)
            {
                customEnabled = true;
                SettingToast.Cancel();
                PopulateForm(currentProfile);
            }
        }

        /// <summary>
        /// Deletes the current custom settings profile.
        /// Default profiles cannot be deleted.
        /// </summary>
        private void DeleteCustomProfile()
        {
            if (!currentProfile.IsDefault)
            {
                profiles.RemoveAt(spinProfiles.SelectedItemPosition);

                if (settingsControl.SaveSettingsProfiles(profiles))
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Deleted");
                    alert.SetMessage("Settings profile " + currentProfile.ProfileName + " deleted. Applying default settings.");
                    alert.SetNeutralButton("OK", (senderAlert, args) => { });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                else
                {
                    profiles = settingsControl.GetSettingsProfiles();
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Delete Failed");
                    alert.SetMessage("Error occured while deleting profile.");
                    alert.SetNeutralButton("OK", (senderAlert, args) => { });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }

                spinProfiles.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, profiles.Select(x => x.ProfileName).ToList());
                spinProfiles.SetSelection(0);
                currentProfile = profiles.First();
                settingsControl.SaveCurrentSettings(currentProfile);
            }
            else
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Delete Failed");
                alert.SetMessage("Cannot delete default settings.");
                alert.SetNeutralButton("OK", (senderAlert, args) => { });
                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }

        /// <summary>
        /// Android method fired when the back hardware key is pressed.
        /// Saves the selected settings profile as the settings to be used by the app.
        /// </summary>
        public override void OnBackPressed()
        {
            ApplyCurrentSettings();
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            Finish();
        }
    }
}
