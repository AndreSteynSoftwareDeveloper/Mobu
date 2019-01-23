namespace MobuUpload
{
    partial class FrmUploadPackage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUploadPackage));
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtBxName = new System.Windows.Forms.TextBox();
            this.txtBxVersion = new System.Windows.Forms.TextBox();
            this.txtBxPath = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblCritical = new System.Windows.Forms.Label();
            this.chkBxCriticalUpdate = new System.Windows.Forms.CheckBox();
            this.barUploadProgress = new System.Windows.Forms.ProgressBar();
            this.lblUploadMsg = new System.Windows.Forms.Label();
            this.lblFragmentSize = new System.Windows.Forms.Label();
            this.comboSize = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(189, 139);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(82, 23);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(13, 139);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(82, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtBxName
            // 
            this.txtBxName.Location = new System.Drawing.Point(65, 12);
            this.txtBxName.Name = "txtBxName";
            this.txtBxName.Size = new System.Drawing.Size(206, 20);
            this.txtBxName.TabIndex = 2;
            // 
            // txtBxVersion
            // 
            this.txtBxVersion.Location = new System.Drawing.Point(65, 38);
            this.txtBxVersion.Name = "txtBxVersion";
            this.txtBxVersion.Size = new System.Drawing.Size(206, 20);
            this.txtBxVersion.TabIndex = 3;
            // 
            // txtBxPath
            // 
            this.txtBxPath.Location = new System.Drawing.Point(65, 64);
            this.txtBxPath.Name = "txtBxPath";
            this.txtBxPath.Size = new System.Drawing.Size(206, 20);
            this.txtBxPath.TabIndex = 4;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 15);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 5;
            this.lblName.Text = "Name:";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(12, 41);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(45, 13);
            this.lblVersion.TabIndex = 6;
            this.lblVersion.Text = "Version:";
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(12, 67);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(32, 13);
            this.lblPath.TabIndex = 7;
            this.lblPath.Text = "Path:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(101, 139);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblCritical
            // 
            this.lblCritical.AutoSize = true;
            this.lblCritical.Location = new System.Drawing.Point(12, 119);
            this.lblCritical.Name = "lblCritical";
            this.lblCritical.Size = new System.Drawing.Size(41, 13);
            this.lblCritical.TabIndex = 12;
            this.lblCritical.Text = "Critical:";
            // 
            // chkBxCriticalUpdate
            // 
            this.chkBxCriticalUpdate.AutoSize = true;
            this.chkBxCriticalUpdate.Location = new System.Drawing.Point(65, 119);
            this.chkBxCriticalUpdate.Name = "chkBxCriticalUpdate";
            this.chkBxCriticalUpdate.Size = new System.Drawing.Size(15, 14);
            this.chkBxCriticalUpdate.TabIndex = 13;
            this.chkBxCriticalUpdate.UseVisualStyleBackColor = true;
            // 
            // barUploadProgress
            // 
            this.barUploadProgress.Location = new System.Drawing.Point(13, 200);
            this.barUploadProgress.Name = "barUploadProgress";
            this.barUploadProgress.Size = new System.Drawing.Size(258, 23);
            this.barUploadProgress.TabIndex = 16;
            this.barUploadProgress.Value = 50;
            // 
            // lblUploadMsg
            // 
            this.lblUploadMsg.AutoSize = true;
            this.lblUploadMsg.Location = new System.Drawing.Point(25, 178);
            this.lblUploadMsg.Name = "lblUploadMsg";
            this.lblUploadMsg.Size = new System.Drawing.Size(227, 13);
            this.lblUploadMsg.TabIndex = 17;
            this.lblUploadMsg.Text = "Uploading package. Do not close this window.";
            // 
            // lblFragmentSize
            // 
            this.lblFragmentSize.AutoSize = true;
            this.lblFragmentSize.Location = new System.Drawing.Point(12, 93);
            this.lblFragmentSize.Name = "lblFragmentSize";
            this.lblFragmentSize.Size = new System.Drawing.Size(30, 13);
            this.lblFragmentSize.TabIndex = 19;
            this.lblFragmentSize.Text = "Size:";
            // 
            // comboSize
            // 
            this.comboSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSize.FormattingEnabled = true;
            this.comboSize.Items.AddRange(new object[] {
            "1000",
            "5000",
            "10000",
            "25000",
            "50000",
            "100000",
            "250000",
            "500000",
            "1000000",
            "2000000",
            "5000000"});
            this.comboSize.Location = new System.Drawing.Point(65, 90);
            this.comboSize.Name = "comboSize";
            this.comboSize.Size = new System.Drawing.Size(207, 21);
            this.comboSize.TabIndex = 20;
            // 
            // FrmUploadPackage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(284, 231);
            this.ControlBox = false;
            this.Controls.Add(this.comboSize);
            this.Controls.Add(this.lblFragmentSize);
            this.Controls.Add(this.lblUploadMsg);
            this.Controls.Add(this.barUploadProgress);
            this.Controls.Add(this.chkBxCriticalUpdate);
            this.Controls.Add(this.lblCritical);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtBxPath);
            this.Controls.Add(this.txtBxVersion);
            this.Controls.Add(this.txtBxName);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnBrowse);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(300, 270);
            this.MinimumSize = new System.Drawing.Size(300, 270);
            this.Name = "FrmUploadPackage";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Upload Package";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmUploadPackage_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox txtBxName;
        private System.Windows.Forms.TextBox txtBxVersion;
        private System.Windows.Forms.TextBox txtBxPath;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblCritical;
        private System.Windows.Forms.CheckBox chkBxCriticalUpdate;
        private System.Windows.Forms.ProgressBar barUploadProgress;
        private System.Windows.Forms.Label lblUploadMsg;
        private System.Windows.Forms.Label lblFragmentSize;
        private System.Windows.Forms.ComboBox comboSize;
    }
}