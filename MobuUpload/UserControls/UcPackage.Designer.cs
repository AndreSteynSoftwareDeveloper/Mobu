namespace MobuUpload
{
    partial class UcPackage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtBxAppName = new System.Windows.Forms.TextBox();
            this.txtBxAppVersion = new System.Windows.Forms.TextBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lblAppName = new System.Windows.Forms.Label();
            this.lblAppVersion = new System.Windows.Forms.Label();
            this.lblCritical = new System.Windows.Forms.Label();
            this.chkBxCritical = new System.Windows.Forms.CheckBox();
            this.btnToggleCritical = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtBxAppName
            // 
            this.txtBxAppName.Location = new System.Drawing.Point(49, 5);
            this.txtBxAppName.Margin = new System.Windows.Forms.Padding(5);
            this.txtBxAppName.Name = "txtBxAppName";
            this.txtBxAppName.ReadOnly = true;
            this.txtBxAppName.Size = new System.Drawing.Size(201, 20);
            this.txtBxAppName.TabIndex = 0;
            // 
            // txtBxAppVersion
            // 
            this.txtBxAppVersion.Location = new System.Drawing.Point(304, 5);
            this.txtBxAppVersion.Margin = new System.Windows.Forms.Padding(5);
            this.txtBxAppVersion.Name = "txtBxAppVersion";
            this.txtBxAppVersion.ReadOnly = true;
            this.txtBxAppVersion.Size = new System.Drawing.Size(94, 20);
            this.txtBxAppVersion.TabIndex = 1;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(581, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(98, 24);
            this.btnRemove.TabIndex = 12;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lblAppName
            // 
            this.lblAppName.AutoSize = true;
            this.lblAppName.Location = new System.Drawing.Point(3, 8);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(38, 13);
            this.lblAppName.TabIndex = 16;
            this.lblAppName.Text = "Name:";
            // 
            // lblAppVersion
            // 
            this.lblAppVersion.AutoSize = true;
            this.lblAppVersion.Location = new System.Drawing.Point(258, 8);
            this.lblAppVersion.Name = "lblAppVersion";
            this.lblAppVersion.Size = new System.Drawing.Size(45, 13);
            this.lblAppVersion.TabIndex = 17;
            this.lblAppVersion.Text = "Version:";
            // 
            // lblCritical
            // 
            this.lblCritical.AutoSize = true;
            this.lblCritical.Location = new System.Drawing.Point(406, 8);
            this.lblCritical.Name = "lblCritical";
            this.lblCritical.Size = new System.Drawing.Size(41, 13);
            this.lblCritical.TabIndex = 19;
            this.lblCritical.Text = "Critical:";
            // 
            // chkBxCritical
            // 
            this.chkBxCritical.AutoSize = true;
            this.chkBxCritical.Enabled = false;
            this.chkBxCritical.Location = new System.Drawing.Point(453, 7);
            this.chkBxCritical.Name = "chkBxCritical";
            this.chkBxCritical.Size = new System.Drawing.Size(15, 14);
            this.chkBxCritical.TabIndex = 20;
            this.chkBxCritical.UseVisualStyleBackColor = true;
            // 
            // btnToggleCritical
            // 
            this.btnToggleCritical.Location = new System.Drawing.Point(477, 3);
            this.btnToggleCritical.Name = "btnToggleCritical";
            this.btnToggleCritical.Size = new System.Drawing.Size(98, 24);
            this.btnToggleCritical.TabIndex = 21;
            this.btnToggleCritical.Text = "Toggle Critical";
            this.btnToggleCritical.UseVisualStyleBackColor = true;
            this.btnToggleCritical.Click += new System.EventHandler(this.btnToggleCritical_Click);
            // 
            // UcPackage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnToggleCritical);
            this.Controls.Add(this.chkBxCritical);
            this.Controls.Add(this.lblCritical);
            this.Controls.Add(this.lblAppVersion);
            this.Controls.Add(this.lblAppName);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.txtBxAppVersion);
            this.Controls.Add(this.txtBxAppName);
            this.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.MaximumSize = new System.Drawing.Size(3000, 31);
            this.MinimumSize = new System.Drawing.Size(500, 31);
            this.Name = "UcPackage";
            this.Size = new System.Drawing.Size(682, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBxAppName;
        private System.Windows.Forms.TextBox txtBxAppVersion;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblAppVersion;
        private System.Windows.Forms.Label lblCritical;
        private System.Windows.Forms.CheckBox chkBxCritical;
        private System.Windows.Forms.Button btnToggleCritical;
    }
}
