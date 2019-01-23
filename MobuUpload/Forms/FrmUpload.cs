using MobuUpload.Controls;
using MobuUpload.MobuWs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobuUpload
{
    /// <summary>
    /// Form used to upload new APKs to the web service.
    /// </summary>
    public partial class FrmUploadPackage : Form
    {
        private bool Uploading;
        private MobuWsClient webService;
        private PackageDetails packageDetails;
        private byte[] package;
        private List<byte[]> fragments;
        private int fragmentSize;
        
        /// <summary>
        /// Default constructor for the upload form.
        /// </summary>
        /// <param name="webService">Reference to the web service instance used by the main form</param>
        public FrmUploadPackage(MobuWsClient webService)
        {
            InitializeComponent();
            MaximumSize = new System.Drawing.Size(Width, 210);
            MinimumSize = new System.Drawing.Size(Width, 210);
            Size = new System.Drawing.Size(Width, 210);
            barUploadProgress.Value = 0;
            comboSize.SelectedIndex = 7;
            this.webService = webService;
            barUploadProgress.Refresh();
            lblUploadMsg.Refresh();
        }

        /// <summary>
        /// Validates the information entered on the form and returns a list of errors if there are any issues.
        /// </summary>
        private String ValidatePackage()
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(txtBxName.Text.Trim()))
            {
                errors.Append("App name cannot be empty." + Environment.NewLine);
            }
            else
            {
                packageDetails.Name = txtBxName.Text.Trim();
            }

            if (string.IsNullOrEmpty(txtBxVersion.Text.Trim()))
            {
                errors.Append("Version name cannot be empty." + Environment.NewLine);
            }
            else
            {
                try
                {
                    packageDetails.Version = new Version(txtBxVersion.Text.Trim()).ToString();
                }
                catch
                {
                    errors.Append("Please enter a valid version like 0.0 ." + Environment.NewLine);
                }
            }

            if (string.IsNullOrEmpty(txtBxPath.Text.Trim()))
            {
                errors.Append("Path cannot be empty." + Environment.NewLine);
            }
            else
            {
                if (File.Exists(txtBxPath.Text))
                {
                    package = File.ReadAllBytes(txtBxPath.Text);
                }
                else
                {
                    errors.Append(txtBxPath.Text + " does not exist." + Environment.NewLine);
                }
            }

            try
            {
                if (!webService.TestWebService())
                {
                    errors.Append("Cannot reach Mobu web service." + Environment.NewLine);
                }
                else
                {
                    var packageList = webService.GetPackageList();

                    if (packageList.Any(x => x.Name == packageDetails.Name && x.Version == packageDetails.Version))
                    {
                        errors.Append(packageDetails.Name + " " + packageDetails.Version + " is already being hosted." + Environment.NewLine);
                    }
                }
            }
            catch (Exception e)
            {
                errors.Append("Error occured while interacting with Mobu web service. " + e.Message + Environment.NewLine);
            }

            packageDetails.Critical = chkBxCriticalUpdate.Checked;

            return errors.ToString();
        }

        /// <summary>
        /// Uploads the APK and its details as entered on the form to the web service.
        /// A progress bar is shown duting this progress.
        /// </summary>
        private void UploadPackage()
        {
            try
            {
                if (!webService.TestWebService())
                {
                    MessageBox.Show("Cannot reach web service at " + webService.Endpoint + ".");
                    return;
                }

                fragmentSize = Convert.ToInt32(comboSize.SelectedItem);
                fragments = SplitPackageIntoFragments(package, fragmentSize);

                if (webService.CheckForActiveUploads(packageDetails))
                {
                    var dialogResult = MessageBox.Show("Package is already being uploaded. Do you want to cancel that upload and proceed with this one?", "Duplicate Upload", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        var t = webService.CancelUpload(packageDetails);
                    }
                    else
                    {
                        return;
                    }
                }

                var result = webService.StartUpload(packageDetails, package.Length, Convert.ToInt32(comboSize.SelectedItem));
                Uploading = true;

                if (!result)
                {
                    MessageBox.Show("Error while initiating upload. ");
                }
                else
                {
                    MaximumSize = new System.Drawing.Size(Width, 270);
                    MinimumSize = new System.Drawing.Size(Width, 270);
                    Size = new System.Drawing.Size(Width, 270);
                    barUploadProgress.Value = 0;
                    barUploadProgress.Refresh();
                    lblUploadMsg.Refresh();

                    int retryMax = 3;
                    int retrycounter = 0;
                    int currentIndex = 0;

                    while (currentIndex < fragments.Count)
                    {
                        barUploadProgress.Value = (int)(((currentIndex + 1) / (float)fragments.Count) * 100);
                        barUploadProgress.Refresh();
                        lblUploadMsg.Refresh();

                        var packageFragment = new Fragment();
                        packageFragment.PackageDetails = packageDetails;
                        packageFragment.Data = fragments[currentIndex];

                        var fragmentResult = webService.UploadNextFragment(packageFragment);

                        while (!fragmentResult && retrycounter < retryMax)
                        {
                            retrycounter++;
                            fragmentResult = webService.UploadNextFragment(packageFragment);
                        }

                        if (retrycounter == retryMax && !fragmentResult)
                        {
                            break;
                        }
                        else
                        {
                            currentIndex++;
                        }
                    }

                    if (currentIndex == fragments.Count)
                    {

                        result = webService.FinishUpload(packageDetails);

                        if (result)
                        {
                            Uploading = false;
                            MessageBox.Show("Succesfully uploaded " + this.packageDetails.Name + " " + this.packageDetails.Version + " to server");
                            Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error occured while uploading fragment " + (currentIndex + 1) + " of " + fragments.Count + ". ");
                        webService.CancelUpload(packageDetails);
                    }
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error occured while uploading package.",e);
                MessageBox.Show("Error occured while uploading package.");
                webService.CancelUpload(packageDetails);
                Close();
            }
        }

        /// <summary>
        /// OPens a file dialog to get the path to the APK file which is to be uploaded.
        /// </summary>
        private String GetFilePath()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    String filePath = ofd.FileName;
                    return filePath;
                }
                catch
                {
                    MessageBox.Show("Error reading file.", "IO Error");
                    return String.Empty;
                }
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Click event for the ok button, starting the APK upload if valid.
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            packageDetails = new PackageDetails();

            var validateResult = ValidatePackage();

            if (!String.IsNullOrEmpty(validateResult))
            {
                MessageBox.Show(validateResult, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                btnOk.Enabled = false;
                btnCancel.Enabled = false;
                btnBrowse.Enabled = false;

                UploadPackage();

                btnOk.Enabled = true;
                btnCancel.Enabled = true;
                btnBrowse.Enabled = true;
            }
        }

        /// <summary>
        /// Click event for the cancel button.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Click event for the browse buton
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            txtBxPath.Text = GetFilePath();
        }

        /// <summary>
        /// Splits the selected APK file into fragments as indicated on the form
        /// </summary>
        /// <param name="package">The byte array representing the APK file to be uploaded.</param>
        /// <param name="fragmentSize">THe size of each fragment in bytes.</param>
        private List<byte[]> SplitPackageIntoFragments(Byte[] package, int fragmentSize)
        {
            List<byte[]> fragments = new List<byte[]>();

            var index = 0;
            var fileSize = package.Length;

            while ((index + 1) * fragmentSize < fileSize)
            {
                var fragment = new byte[fragmentSize];
                Array.Copy(package, index * fragmentSize, fragment, 0, fragmentSize);
                fragments.Add(fragment);
                index++;
            }

            if (index * fragmentSize < fileSize)
            {
                var lastFragmennt = new byte[(fileSize - (index * fragmentSize))];
                Array.Copy(package, index * fragmentSize, lastFragmennt, 0, fileSize - (index * fragmentSize));
                fragments.Add(lastFragmennt);
            }

            return fragments;
        }

        /// <summary>
        /// Form close event to cancel any active upload if the form is closed for any reason.
        /// </summary>
        private void FrmUploadPackage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Uploading)
            {
                webService.CancelUpload(packageDetails);
            }
        }
    }
}
