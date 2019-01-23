namespace MobuUpload
{
    partial class FrmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSettings));
            this.lblSettingsWebServiceTimeout = new System.Windows.Forms.Label();
            this.lblSettingsWebServiceBaseAddress = new System.Windows.Forms.Label();
            this.numSettingsWebServiceTimeout = new System.Windows.Forms.NumericUpDown();
            this.txtBxSettingsWebServiceBaseAddress = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comProfiles = new System.Windows.Forms.ComboBox();
            this.btnDeleteProfile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numSettingsWebServiceTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSettingsWebServiceTimeout
            // 
            this.lblSettingsWebServiceTimeout.AutoSize = true;
            this.lblSettingsWebServiceTimeout.Location = new System.Drawing.Point(8, 66);
            this.lblSettingsWebServiceTimeout.Name = "lblSettingsWebServiceTimeout";
            this.lblSettingsWebServiceTimeout.Size = new System.Drawing.Size(48, 13);
            this.lblSettingsWebServiceTimeout.TabIndex = 7;
            this.lblSettingsWebServiceTimeout.Text = "Timeout:";
            // 
            // lblSettingsWebServiceBaseAddress
            // 
            this.lblSettingsWebServiceBaseAddress.AutoSize = true;
            this.lblSettingsWebServiceBaseAddress.Location = new System.Drawing.Point(8, 41);
            this.lblSettingsWebServiceBaseAddress.Name = "lblSettingsWebServiceBaseAddress";
            this.lblSettingsWebServiceBaseAddress.Size = new System.Drawing.Size(75, 13);
            this.lblSettingsWebServiceBaseAddress.TabIndex = 6;
            this.lblSettingsWebServiceBaseAddress.Text = "Base Address:";
            // 
            // numSettingsWebServiceTimeout
            // 
            this.numSettingsWebServiceTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numSettingsWebServiceTimeout.Location = new System.Drawing.Point(96, 64);
            this.numSettingsWebServiceTimeout.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numSettingsWebServiceTimeout.Name = "numSettingsWebServiceTimeout";
            this.numSettingsWebServiceTimeout.Size = new System.Drawing.Size(268, 20);
            this.numSettingsWebServiceTimeout.TabIndex = 5;
            // 
            // txtBxSettingsWebServiceBaseAddress
            // 
            this.txtBxSettingsWebServiceBaseAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBxSettingsWebServiceBaseAddress.Location = new System.Drawing.Point(96, 38);
            this.txtBxSettingsWebServiceBaseAddress.Name = "txtBxSettingsWebServiceBaseAddress";
            this.txtBxSettingsWebServiceBaseAddress.Size = new System.Drawing.Size(268, 20);
            this.txtBxSettingsWebServiceBaseAddress.TabIndex = 4;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(289, 90);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(208, 90);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Profile:";
            // 
            // comProfiles
            // 
            this.comProfiles.FormattingEnabled = true;
            this.comProfiles.Items.AddRange(new object[] {
            "Production",
            "Test",
            "Custom"});
            this.comProfiles.Location = new System.Drawing.Point(96, 11);
            this.comProfiles.Name = "comProfiles";
            this.comProfiles.Size = new System.Drawing.Size(268, 21);
            this.comProfiles.TabIndex = 12;
            this.comProfiles.SelectedIndexChanged += new System.EventHandler(this.comProfiles_SelectedIndexChanged);
            // 
            // btnDeleteProfile
            // 
            this.btnDeleteProfile.Location = new System.Drawing.Point(127, 90);
            this.btnDeleteProfile.Name = "btnDeleteProfile";
            this.btnDeleteProfile.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteProfile.TabIndex = 13;
            this.btnDeleteProfile.Text = "Delete";
            this.btnDeleteProfile.UseVisualStyleBackColor = true;
            this.btnDeleteProfile.Click += new System.EventHandler(this.btnDeleteProfile_Click);
            // 
            // FrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 125);
            this.ControlBox = false;
            this.Controls.Add(this.btnDeleteProfile);
            this.Controls.Add(this.comProfiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblSettingsWebServiceTimeout);
            this.Controls.Add(this.lblSettingsWebServiceBaseAddress);
            this.Controls.Add(this.numSettingsWebServiceTimeout);
            this.Controls.Add(this.txtBxSettingsWebServiceBaseAddress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmSettings";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numSettingsWebServiceTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSettingsWebServiceTimeout;
        private System.Windows.Forms.Label lblSettingsWebServiceBaseAddress;
        private System.Windows.Forms.NumericUpDown numSettingsWebServiceTimeout;
        private System.Windows.Forms.TextBox txtBxSettingsWebServiceBaseAddress;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comProfiles;
        private System.Windows.Forms.Button btnDeleteProfile;
    }
}