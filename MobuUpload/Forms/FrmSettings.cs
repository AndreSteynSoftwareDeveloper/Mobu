using MobuUpload.Controls;
using MobuUpload.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MobuUpload
{
    /// <summary>
    /// Settings form allowing users to select, create, update and delete settings profiles.
    /// </summary>
    public partial class FrmSettings : Form
    {   
        /// <summary>
        /// Reference to the parent form.
        /// </summary>
        private FrmMain frmMain;

        /// <summary>
        /// Settings control used to interact with settings profiles.
        /// </summary>
        public SettingsControl SettingsControl;

        /// <summary>
        /// Current settings shown on the form.
        /// </summary>
        public Settings Settings;

        /// <summary>
        /// List of available settings profiles.
        /// </summary>
        public List<Settings> SettingsProfiles;

        /// <summary>
        /// Default constructor for the settings form.
        /// </summary>
        /// <param name="parent"></param>
        public FrmSettings(FrmMain parent)
        {
            frmMain = parent;
            InitializeComponent();
            SettingsControl = new SettingsControl();
            LoadSettings();
        }

        /// <summary>
        /// Refreshes the form and shows the current settings.
        /// </summary>
        public void LoadSettings()
        {
            SettingsProfiles = SettingsControl.GetSettingsProfiles();
            Settings = SettingsControl.GetSettings();
            comProfiles.Items.Clear();
            comProfiles.Items.AddRange(SettingsProfiles.Select(x => x.ProfileName).ToArray());
            comProfiles.SelectedIndex = SettingsProfiles.FindIndex(x => x.ProfileName == Settings.ProfileName);
            PopulateForm();
        }

        /// <summary>
        /// Populates the form wwith the current settings
        /// </summary>
        public void PopulateForm()
        {
            txtBxSettingsWebServiceBaseAddress.Text = Settings.WebServiceUrl;
            numSettingsWebServiceTimeout.Text = Settings.WebServiceTimeout.ToString();
            btnDeleteProfile.Visible = !Settings.IsDefault;
        }

        /// <summary>
        /// Saves the current settings on the form either updating an existing profile or creating a new one.
        /// </summary>
        public void SaveSettings()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (String.IsNullOrEmpty(comProfiles.Text.Trim()))
                {
                    MessageBox.Show("Profile name cannot be empty. Please choose an existing profile to update or enter a new profile name.", "Invalid Profile Name");
                    return;
                }

                if (String.IsNullOrEmpty(txtBxSettingsWebServiceBaseAddress.Text.Trim()) || (int)numSettingsWebServiceTimeout.Value <= 0)
                {
                    MessageBox.Show("Please enter a URL and a timeout greater than 0.", "Invalid Settings");
                    return;
                }

                if (Settings.IsDefault &&
                    Settings.ProfileName.ToLower() == comProfiles.Text.Trim().ToLower() &&
                    (Settings.WebServiceUrl != txtBxSettingsWebServiceBaseAddress.Text.Trim() ||
                    Settings.WebServiceTimeout != (int)numSettingsWebServiceTimeout.Value))
                {
                    MessageBox.Show("Cannot overwrite default settings. Please enter a different name to save a new profile.", "Default Settings");
                    return;
                }

                if (!SettingsProfiles.Any(x => x.ProfileName.Trim().ToLower() == comProfiles.Text.Trim().ToLower()))
                {
                    Settings = new Settings();
                    Settings.WebServiceUrl = txtBxSettingsWebServiceBaseAddress.Text.Trim();
                    Settings.WebServiceTimeout = (int)numSettingsWebServiceTimeout.Value;
                    Settings.ProfileName = comProfiles.Text.Trim();
                    Settings.IsDefault = false;
                    SettingsProfiles.Add(Settings);
                }
                else
                {
                    Settings.WebServiceUrl = txtBxSettingsWebServiceBaseAddress.Text.Trim();
                    Settings.WebServiceTimeout = (int)numSettingsWebServiceTimeout.Value;
                }

                if (!SettingsControl.SaveSettingsProfiles(SettingsProfiles))
                {
                    MessageBox.Show("Failed to save new settings profile.", "Error");
                    return;
                }


                if (SettingsControl.SaveSettings(Settings))
                {
                    MessageBox.Show("Settings Saved. Restarting manager.", "Settings");
                    frmMain.RestartMonitor();
                    Close();
                }
                else
                {
                    MessageBox.Show("Error while saving settings. ", "Setting Error");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while saving settings. " + Environment.NewLine + e.Message, "Setting Error");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Deletes the currently selected settings profile.
        /// </summary>
        private void DeleteProfile()
        {
            var result = MessageBox.Show("Are you sure you want to delete settings profile " + comProfiles.Text + "?", "Delete Profile", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                SettingsProfiles.RemoveAt(comProfiles.SelectedIndex);
                SettingsControl.SaveSettingsProfiles(SettingsProfiles);
                Settings = SettingsProfiles.First();
                SettingsControl.SaveSettings(Settings);
                comProfiles.Items.Clear();
                comProfiles.Items.AddRange(SettingsProfiles.Select(x => x.ProfileName).ToArray());
                comProfiles.SelectedIndex = SettingsProfiles.FindIndex(x => x.ProfileName == Settings.ProfileName);
                PopulateForm();
                frmMain.RestartMonitor();
            }
        }

        /// <summary>
        /// Click event for the cancel button, discarding any unsaved changes.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Click event for the save button.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        /// <summary>
        /// Click event for the delete button.
        /// </summary>
        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            DeleteProfile();
        }

        /// <summary>
        /// Index changed event for the profiles list.
        /// </summary>
        private void comProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings = SettingsProfiles[comProfiles.SelectedIndex];
            PopulateForm();
        }
    }
}
