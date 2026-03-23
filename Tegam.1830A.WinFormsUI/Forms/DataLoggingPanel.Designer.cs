namespace Tegam.WinFormsUI.Forms
{
    partial class DataLoggingPanel
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
            this.grpLoggingControl = new System.Windows.Forms.GroupBox();
            this.lblFilename = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnStartLogging = new System.Windows.Forms.Button();
            this.btnStopLogging = new System.Windows.Forms.Button();
            this.grpLoggingStatus = new System.Windows.Forms.GroupBox();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.lblFileLocation = new System.Windows.Forms.Label();
            this.lblFileLocationValue = new System.Windows.Forms.Label();
            this.lblMeasurementCount = new System.Windows.Forms.Label();
            this.lblMeasurementCountValue = new System.Windows.Forms.Label();
            this.btnOpenLogFile = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();

            this.grpLoggingControl.SuspendLayout();
            this.grpLoggingStatus.SuspendLayout();
            this.SuspendLayout();

            // grpLoggingControl
            this.grpLoggingControl.Controls.Add(this.lblFilename);
            this.grpLoggingControl.Controls.Add(this.txtFilename);
            this.grpLoggingControl.Controls.Add(this.btnBrowse);
            this.grpLoggingControl.Controls.Add(this.btnStartLogging);
            this.grpLoggingControl.Controls.Add(this.btnStopLogging);
            this.grpLoggingControl.Location = new System.Drawing.Point(10, 10);
            this.grpLoggingControl.Name = "grpLoggingControl";
            this.grpLoggingControl.Size = new System.Drawing.Size(720, 100);
            this.grpLoggingControl.TabIndex = 0;
            this.grpLoggingControl.TabStop = false;
            this.grpLoggingControl.Text = "Logging Control";

            // lblFilename
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(10, 22);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(52, 13);
            this.lblFilename.TabIndex = 0;
            this.lblFilename.Text = "Filename:";

            // txtFilename
            this.txtFilename.Location = new System.Drawing.Point(76, 19);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(300, 20);
            this.txtFilename.TabIndex = 1;
            this.txtFilename.Text = "measurements.csv";

            // btnBrowse
            this.btnBrowse.Location = new System.Drawing.Point(382, 19);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            // btnStartLogging
            this.btnStartLogging.Location = new System.Drawing.Point(463, 19);
            this.btnStartLogging.Name = "btnStartLogging";
            this.btnStartLogging.Size = new System.Drawing.Size(100, 23);
            this.btnStartLogging.TabIndex = 3;
            this.btnStartLogging.Text = "Start Logging";
            this.btnStartLogging.UseVisualStyleBackColor = true;
            this.btnStartLogging.Click += new System.EventHandler(this.btnStartLogging_Click);

            // btnStopLogging
            this.btnStopLogging.Enabled = false;
            this.btnStopLogging.Location = new System.Drawing.Point(569, 19);
            this.btnStopLogging.Name = "btnStopLogging";
            this.btnStopLogging.Size = new System.Drawing.Size(100, 23);
            this.btnStopLogging.TabIndex = 4;
            this.btnStopLogging.Text = "Stop Logging";
            this.btnStopLogging.UseVisualStyleBackColor = true;
            this.btnStopLogging.Click += new System.EventHandler(this.btnStopLogging_Click);

            // grpLoggingStatus
            this.grpLoggingStatus.Controls.Add(this.lblStatusValue);
            this.grpLoggingStatus.Controls.Add(this.lblFileLocation);
            this.grpLoggingStatus.Controls.Add(this.lblFileLocationValue);
            this.grpLoggingStatus.Controls.Add(this.lblMeasurementCount);
            this.grpLoggingStatus.Controls.Add(this.lblMeasurementCountValue);
            this.grpLoggingStatus.Controls.Add(this.btnOpenLogFile);
            this.grpLoggingStatus.Location = new System.Drawing.Point(10, 116);
            this.grpLoggingStatus.Name = "grpLoggingStatus";
            this.grpLoggingStatus.Size = new System.Drawing.Size(720, 150);
            this.grpLoggingStatus.TabIndex = 1;
            this.grpLoggingStatus.TabStop = false;
            this.grpLoggingStatus.Text = "Logging Status";

            // lblStatusValue
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.Location = new System.Drawing.Point(10, 22);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(56, 13);
            this.lblStatusValue.TabIndex = 0;
            this.lblStatusValue.Text = "Not logging";

            // lblFileLocation
            this.lblFileLocation.AutoSize = true;
            this.lblFileLocation.Location = new System.Drawing.Point(10, 48);
            this.lblFileLocation.Name = "lblFileLocation";
            this.lblFileLocation.Size = new System.Drawing.Size(68, 13);
            this.lblFileLocation.TabIndex = 1;
            this.lblFileLocation.Text = "File Location:";

            // lblFileLocationValue
            this.lblFileLocationValue.AutoSize = true;
            this.lblFileLocationValue.Location = new System.Drawing.Point(84, 48);
            this.lblFileLocationValue.Name = "lblFileLocationValue";
            this.lblFileLocationValue.Size = new System.Drawing.Size(27, 13);
            this.lblFileLocationValue.TabIndex = 2;
            this.lblFileLocationValue.Text = "N/A";

            // lblMeasurementCount
            this.lblMeasurementCount.AutoSize = true;
            this.lblMeasurementCount.Location = new System.Drawing.Point(10, 74);
            this.lblMeasurementCount.Name = "lblMeasurementCount";
            this.lblMeasurementCount.Size = new System.Drawing.Size(100, 13);
            this.lblMeasurementCount.TabIndex = 3;
            this.lblMeasurementCount.Text = "Measurements Logged:";

            // lblMeasurementCountValue
            this.lblMeasurementCountValue.AutoSize = true;
            this.lblMeasurementCountValue.Location = new System.Drawing.Point(116, 74);
            this.lblMeasurementCountValue.Name = "lblMeasurementCountValue";
            this.lblMeasurementCountValue.Size = new System.Drawing.Size(13, 13);
            this.lblMeasurementCountValue.TabIndex = 4;
            this.lblMeasurementCountValue.Text = "0";

            // btnOpenLogFile
            this.btnOpenLogFile.Location = new System.Drawing.Point(248, 100);
            this.btnOpenLogFile.Name = "btnOpenLogFile";
            this.btnOpenLogFile.Size = new System.Drawing.Size(100, 23);
            this.btnOpenLogFile.TabIndex = 5;
            this.btnOpenLogFile.Text = "Open Log File";
            this.btnOpenLogFile.UseVisualStyleBackColor = true;
            this.btnOpenLogFile.Click += new System.EventHandler(this.btnOpenLogFile_Click);

            // saveFileDialog
            this.saveFileDialog.DefaultExt = "csv";
            this.saveFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // DataLoggingPanel
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpLoggingStatus);
            this.Controls.Add(this.grpLoggingControl);
            this.Name = "DataLoggingPanel";
            this.Size = new System.Drawing.Size(740, 310);

            this.grpLoggingControl.ResumeLayout(false);
            this.grpLoggingControl.PerformLayout();
            this.grpLoggingStatus.ResumeLayout(false);
            this.grpLoggingStatus.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpLoggingControl;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnStartLogging;
        private System.Windows.Forms.Button btnStopLogging;
        private System.Windows.Forms.GroupBox grpLoggingStatus;
        private System.Windows.Forms.Label lblStatusValue;
        private System.Windows.Forms.Label lblFileLocation;
        private System.Windows.Forms.Label lblFileLocationValue;
        private System.Windows.Forms.Label lblMeasurementCount;
        private System.Windows.Forms.Label lblMeasurementCountValue;
        private System.Windows.Forms.Button btnOpenLogFile;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}
