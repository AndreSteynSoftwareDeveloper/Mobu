using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using MobuUpload.MobuWs;
using MobuUpload.Controls;
using MobuUpload.Models;

namespace MobuUpload
{
    /// <summary>
    /// Main form of the application. Primarily used to show a list of hosted APKs and interact with them.
    /// </summary>
    public partial class FrmMain : Form
    {
        /// <summary>
        /// Monitor control object used to retrieve information from the Mobu web service.
        /// </summary>
        public MonitorControl MonitorControl;

        /// <summary>
        /// Settings control object used to retrieve the current application settings.
        /// </summary>
        public SettingsControl SettingsControl;

        /// <summary>
        /// Current application settings
        /// </summary>
        public Settings Settings;

        /// <summary>
        /// Default constructor for the main form.
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();
            SettingsControl = new SettingsControl();
            Settings = SettingsControl.GetSettings();

            if (!SettingsControl.SettingsProfilesExist() || !SettingsControl.SettingsExist())
            {
                SettingsControl.GenerateDefaultSettingsProfiles();
            }

            if (!SettingsControl.SettingsExist())
            {
                var profiles = SettingsControl.GetSettingsProfiles();
                Settings = profiles.First();
                SettingsControl.SaveSettings(Settings);
            }
            else
            {
                Settings = SettingsControl.GetSettings();
            }

            RestartMonitor();
            mainTimer.Start();

            Text = "Mobu Upload " + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
        }

        /// <summary>
        /// Recreates the monitor from the latest settings.
        /// </summary>
        public void RestartMonitor()
        {
            try
            {
                Settings = SettingsControl.GetSettings();

                if (MonitorControl != null)
                {
                    if (MonitorControl.WebMonitor != null) MonitorControl.WebMonitor.CancelAsync();
                    if (MonitorControl.PackageMonitor != null) MonitorControl.PackageMonitor.CancelAsync();
                }

                MonitorControl = new MonitorControl(Settings);

                statusIndicator.Value = 0;
                statusLabel.Text = "Mobu Web Service Status: Waiting...";

                flowHostedPackages.Controls.Clear();
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while refreshing/recreating manager.", e);
            }
        }

        /// <summary>
        /// Gets the latest package list from the monitor control (which is constantly updated in the background), and updates the package display based on that list.
        /// </summary>
        public void RefreshMonitor()
        {
            try
            {
                if (MonitorControl.WebServiceStatus)
                {
                    statusIndicator.Value = 100;
                    statusLabel.Text = "Mobu Web Service Status: Connected " + Settings.WebServiceUrl + ".";
                }
                else
                {
                    statusIndicator.Value = 0;
                    statusLabel.Text = "Mobu Web Service Status: Cannot connect to web service on " + Settings.WebServiceUrl + ".";
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while refreshing component monitor.", e);
            }

            try
            {
                var webPackages = MonitorControl.CopyPackageList();

                var currentPackages = new List<PackageDetails>();

                foreach (var c in flowHostedPackages.Controls)
                {
                    var control = (UcPackage)c;

                    currentPackages.Add(control.PackageDetails);
                }

                var remove = new List<Control>();

                foreach (var c in flowHostedPackages.Controls)
                {
                    var control = (UcPackage)c;

                    if (!webPackages.Any(x => x.Name == control.PackageDetails.Name && x.Version == control.PackageDetails.Version && x.Critical == control.PackageDetails.Critical))
                    {
                        remove.Add((Control)c);
                    }
                }

                foreach (var c in remove)
                {
                    flowHostedPackages.Controls.Remove(c);
                }

                foreach (var i in webPackages)
                {
                    if (!currentPackages.Any(x => x.Name == i.Name && x.Version == i.Version))
                    {
                        flowHostedPackages.Controls.Add(new UcPackage(this, i));
                    }
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while refreshing package monitor.", e);
            }
        }

        /// <summary>
        /// Displays the settings form
        /// </summary>
        private void ShowSettings()
        {
            Cursor.Current = Cursors.WaitCursor;

            FrmSettings frmSettings = new FrmSettings(this);
            frmSettings.ShowDialog();

            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Displays the form to host a new package
        /// </summary>
        private void HostNewPackage()
        {
            var frmNewPackage = new FrmUploadPackage(MonitorControl.WebService);

            frmNewPackage.Show();
        }

        /// <summary>
        /// Click event for the settings menu item
        /// </summary>
        private void menuSettings_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        /// <summary>
        /// Click event for the add package menu item
        /// </summary>
        private void menuAddPackage_Click(object sender, EventArgs e)
        {
            HostNewPackage();
        }

        /// <summary>
        /// Click event for the refresh menu item
        /// </summary>
        private void menuRefresh_Click(object sender, EventArgs e)
        {
            RestartMonitor();
        }

        /// <summary>
        /// Click event for the exit menu item
        /// </summary>
        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Tick event periodically refreshing the UI with an updated package list.
        /// </summary>
        private void mainTimer_Tick(object sender, EventArgs e)
        {
            RefreshMonitor();
        }
    }
}
