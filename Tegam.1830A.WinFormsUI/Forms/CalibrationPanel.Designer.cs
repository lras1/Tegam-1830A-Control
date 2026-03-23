namespace Tegam.WinFormsUI.Forms
{
    partial class CalibrationPanel
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpStartCalibration = new System.Windows.Forms.GroupBox();
            this.lblMode = new System.Windows.Forms.Label();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.btnStartCalibration = new System.Windows.Forms.Button();
            this.grpCalibrationStatus = new System.Windows.Forms.GroupBox();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.btnQueryStatus = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();

            this.grpStartCalibration.SuspendLayout();
            this.grpCalibrationStatus.SuspendLayout();
            this.SuspendLayout();

            // grpStartCalibration
            this.grpStartCalibration.Controls.Add(this.lblMode);
            this.grpStartCalibration.Controls.Add(this.cmbMode);
            this.grpStartCalibration.Controls.Add(this.btnStartCalibration);
            this.grpStartCalibration.Location = new System.Drawing.Point(10, 10);
            this.grpStartCalibration.Name = "grpStartCalibration";
            this.grpStartCalibration.Size = new System.Drawing.Size(720, 80);
            this.grpStartCalibration.TabIndex = 0;
            this.grpStartCalibration.TabStop = false;
            this.grpStartCalibration.Text = "Start Calibration";

            // lblMode
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(10, 22);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(37, 13);
            this.lblMode.TabIndex = 0;
            this.lblMode.Text = "Mode:";

            // cmbMode
            this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Items.AddRange(new object[] { "Internal", "External" });
            this.cmbMode.Location = new System.Drawing.Point(76, 19);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(100, 21);
            this.cmbMode.TabIndex = 1;
            this.cmbMode.Text = "Internal";

            // btnStartCalibration
            this.btnStartCalibration.Location = new System.Drawing.Point(248, 19);
            this.btnStartCalibration.Name = "btnStartCalibration";
            this.btnStartCalibration.Size = new System.Drawing.Size(100, 47);
            this.btnStartCalibration.TabIndex = 2;
            this.btnStartCalibration.Text = "Start Calibration";
            this.btnStartCalibration.UseVisualStyleBackColor = true;
            this.btnStartCalibration.Click += new System.EventHandler(this.btnStartCalibration_Click);

            // grpCalibrationStatus
            this.grpCalibrationStatus.Controls.Add(this.lblStatusValue);
            this.grpCalibrationStatus.Controls.Add(this.btnQueryStatus);
            this.grpCalibrationStatus.Controls.Add(this.progressBar);
            this.grpCalibrationStatus.Location = new System.Drawing.Point(10, 96);
            this.grpCalibrationStatus.Name = "grpCalibrationStatus";
            this.grpCalibrationStatus.Size = new System.Drawing.Size(720, 120);
            this.grpCalibrationStatus.TabIndex = 1;
            this.grpCalibrationStatus.TabStop = false;
            this.grpCalibrationStatus.Text = "Calibration Status";

            // lblStatusValue
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.Location = new System.Drawing.Point(10, 22);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(27, 13);
            this.lblStatusValue.TabIndex = 0;
            this.lblStatusValue.Text = "N/A";

            // btnQueryStatus
            this.btnQueryStatus.Location = new System.Drawing.Point(248, 19);
            this.btnQueryStatus.Name = "btnQueryStatus";
            this.btnQueryStatus.Size = new System.Drawing.Size(100, 47);
            this.btnQueryStatus.TabIndex = 1;
            this.btnQueryStatus.Text = "Query Status";
            this.btnQueryStatus.UseVisualStyleBackColor = true;
            this.btnQueryStatus.Click += new System.EventHandler(this.btnQueryStatus_Click);

            // progressBar
            this.progressBar.Location = new System.Drawing.Point(10, 72);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(700, 40);
            this.progressBar.TabIndex = 2;

            // CalibrationPanel
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpCalibrationStatus);
            this.Controls.Add(this.grpStartCalibration);
            this.Name = "CalibrationPanel";
            this.Size = new System.Drawing.Size(740, 310);

            this.grpStartCalibration.ResumeLayout(false);
            this.grpStartCalibration.PerformLayout();
            this.grpCalibrationStatus.ResumeLayout(false);
            this.grpCalibrationStatus.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpStartCalibration;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.Button btnStartCalibration;
        private System.Windows.Forms.GroupBox grpCalibrationStatus;
        private System.Windows.Forms.Label lblStatusValue;
        private System.Windows.Forms.Button btnQueryStatus;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}
