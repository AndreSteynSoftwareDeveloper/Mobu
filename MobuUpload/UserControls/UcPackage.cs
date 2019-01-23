using MobuUpload.Controls;
using MobuUpload.MobuWs;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MobuUpload
{
    /// <summary>
    /// User control used to represent a hosted APK on the main UI
    /// </summary>
    public partial class UcPackage : UserControl
    {
        /// <summary>
        /// Details of the APK represented by this user control.
        /// </summary>
        public PackageDetails PackageDetails;

        /// <summary>
        /// Reference to the parent form.
        /// </summary>
        public FrmMain FrmMain;

        /// <summary>
        /// Default constructor for this user control
        /// </summary>
        /// <param name="parent">Reference to the parent form.</param>
        /// <param name="packageDetails">Details of the APK represented by this user control.</param>
        public UcPackage(FrmMain parent, PackageDetails packageDetails)
        {
            Parent = parent;
            FrmMain = parent;

            InitializeComponent();
            BackColor = SystemColors.Control;
            PackageDetails = packageDetails;
            PopulateControl();
        }

        /// <summary>
        /// Loads package details onto the user control
        /// </summary>
        private void PopulateControl()
        {
            txtBxAppName.Text = PackageDetails.Name;
            txtBxAppVersion.Text = PackageDetails.Version.ToString();
            chkBxCritical.Checked = PackageDetails.Critical;
        }

        /// <summary>
        /// Toggles the critical state of the APK represented by this control.
        /// </summary>
        private void btnToggleCritical_Click(object sender, EventArgs e)
        {
            if (CancelActiveDownloads())
            {
                if (FrmMain.MonitorControl.ToggleCritical(PackageDetails))
                {
                    FrmMain.RestartMonitor();
                }
            }
        }

        /// <summary>
        /// Prompts the user and checks for active downloads before removing this APK from the service and UI.
        /// </summary>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to remove this package?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (CancelActiveDownloads())
                {
                    FrmMain.MonitorControl.RemovePackage(PackageDetails);
                    Dispose();
                }
            }
        }

        /// <summary>
        /// Cancels any active downloads to devices.
        /// The downloads will fail on the devices, giving the user the option to retry.
        /// </summary>
        private bool CancelActiveDownloads()
        {
            var activeDownloads = FrmMain.MonitorControl.WebService.CheckForActiveDownloads(PackageDetails);

            if (activeDownloads > 0)
            {
                var result = MessageBox.Show("Package " + PackageDetails.Name + " " + PackageDetails.Version + " is currently being downloaded by " + activeDownloads + ""
                    + " devices. Do you want to abort these downloads? ", "Downloads in progress", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    return FrmMain.MonitorControl.WebService.CancelActiveDownloads(PackageDetails);
                }
                else
                {
                    LoggingControl.Log("Error while cancelling active downloads for " + PackageDetails.Name + " " + PackageDetails.Version + ".");
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
