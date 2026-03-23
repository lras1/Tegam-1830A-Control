namespace Tegam.WinFormsUI.Forms
{
    partial class PowerMeasurementPanel
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
            this.grpMeasurement = new System.Windows.Forms.GroupBox();
            this.lblFrequency = new System.Windows.Forms.Label();
            this.numFrequency = new System.Windows.Forms.NumericUpDown();
            this.cmbFrequencyUnit = new System.Windows.Forms.ComboBox();
            this.lblSensor = new System.Windows.Forms.Label();
            this.cmbSensor = new System.Windows.Forms.ComboBox();
            this.btnMeasure = new System.Windows.Forms.Button();
            this.grpResults = new System.Windows.Forms.GroupBox();
            this.lblPowerValue = new System.Windows.Forms.Label();
            this.lblPowerValueDisplay = new System.Windows.Forms.Label();
            this.lblTimestamp = new System.Windows.Forms.Label();
            this.lblTimestampDisplay = new System.Windows.Forms.Label();
            this.grpMultipleMeasurements = new System.Windows.Forms.GroupBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.lblDelay = new System.Windows.Forms.Label();
            this.numDelay = new System.Windows.Forms.NumericUpDown();
            this.btnMeasureMultiple = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();

            this.grpMeasurement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFrequency)).BeginInit();
            this.grpResults.SuspendLayout();
            this.grpMultipleMeasurements.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).BeginInit();
            this.SuspendLayout();

            // grpMeasurement
            this.grpMeasurement.Controls.Add(this.lblFrequency);
            this.grpMeasurement.Controls.Add(this.numFrequency);
            this.grpMeasurement.Controls.Add(this.cmbFrequencyUnit);
            this.grpMeasurement.Controls.Add(this.lblSensor);
            this.grpMeasurement.Controls.Add(this.cmbSensor);
            this.grpMeasurement.Controls.Add(this.btnMeasure);
            this.grpMeasurement.Location = new System.Drawing.Point(10, 10);
            this.grpMeasurement.Name = "grpMeasurement";
            this.grpMeasurement.Size = new System.Drawing.Size(720, 100);
            this.grpMeasurement.TabIndex = 0;
            this.grpMeasurement.TabStop = false;
            this.grpMeasurement.Text = "Single Measurement";

            // lblFrequency
            this.lblFrequency.AutoSize = true;
            this.lblFrequency.Location = new System.Drawing.Point(10, 22);
            this.lblFrequency.Name = "lblFrequency";
            this.lblFrequency.Size = new System.Drawing.Size(60, 13);
            this.lblFrequency.TabIndex = 0;
            this.lblFrequency.Text = "Frequency:";

            // numFrequency
            this.numFrequency.DecimalPlaces = 2;
            this.numFrequency.Location = new System.Drawing.Point(76, 19);
            this.numFrequency.Maximum = new decimal(new int[] { 40000, 0, 0, 0 });
            this.numFrequency.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numFrequency.Name = "numFrequency";
            this.numFrequency.Size = new System.Drawing.Size(100, 20);
            this.numFrequency.TabIndex = 1;
            this.numFrequency.Value = new decimal(new int[] { 2400, 0, 0, 0 });

            // cmbFrequencyUnit
            this.cmbFrequencyUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFrequencyUnit.FormattingEnabled = true;
            this.cmbFrequencyUnit.Items.AddRange(new object[] { "Hz", "kHz", "MHz", "GHz" });
            this.cmbFrequencyUnit.Location = new System.Drawing.Point(182, 19);
            this.cmbFrequencyUnit.Name = "cmbFrequencyUnit";
            this.cmbFrequencyUnit.Size = new System.Drawing.Size(60, 21);
            this.cmbFrequencyUnit.TabIndex = 2;
            this.cmbFrequencyUnit.Text = "MHz";

            // lblSensor
            this.lblSensor.AutoSize = true;
            this.lblSensor.Location = new System.Drawing.Point(10, 48);
            this.lblSensor.Name = "lblSensor";
            this.lblSensor.Size = new System.Drawing.Size(41, 13);
            this.lblSensor.TabIndex = 3;
            this.lblSensor.Text = "Sensor:";

            // cmbSensor
            this.cmbSensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSensor.FormattingEnabled = true;
            this.cmbSensor.Items.AddRange(new object[] { "1", "2", "3", "4" });
            this.cmbSensor.Location = new System.Drawing.Point(76, 45);
            this.cmbSensor.Name = "cmbSensor";
            this.cmbSensor.Size = new System.Drawing.Size(60, 21);
            this.cmbSensor.TabIndex = 4;
            this.cmbSensor.Text = "1";

            // btnMeasure
            this.btnMeasure.Location = new System.Drawing.Point(248, 19);
            this.btnMeasure.Name = "btnMeasure";
            this.btnMeasure.Size = new System.Drawing.Size(75, 47);
            this.btnMeasure.TabIndex = 5;
            this.btnMeasure.Text = "Measure";
            this.btnMeasure.UseVisualStyleBackColor = true;
            this.btnMeasure.Click += new System.EventHandler(this.btnMeasure_Click);

            // grpResults
            this.grpResults.Controls.Add(this.lblPowerValue);
            this.grpResults.Controls.Add(this.lblPowerValueDisplay);
            this.grpResults.Controls.Add(this.lblTimestamp);
            this.grpResults.Controls.Add(this.lblTimestampDisplay);
            this.grpResults.Location = new System.Drawing.Point(10, 116);
            this.grpResults.Name = "grpResults";
            this.grpResults.Size = new System.Drawing.Size(720, 80);
            this.grpResults.TabIndex = 1;
            this.grpResults.TabStop = false;
            this.grpResults.Text = "Measurement Results";

            // lblPowerValue
            this.lblPowerValue.AutoSize = true;
            this.lblPowerValue.Location = new System.Drawing.Point(10, 22);
            this.lblPowerValue.Name = "lblPowerValue";
            this.lblPowerValue.Size = new System.Drawing.Size(70, 13);
            this.lblPowerValue.TabIndex = 0;
            this.lblPowerValue.Text = "Power Value:";

            // lblPowerValueDisplay
            this.lblPowerValueDisplay.AutoSize = true;
            this.lblPowerValueDisplay.Location = new System.Drawing.Point(86, 22);
            this.lblPowerValueDisplay.Name = "lblPowerValueDisplay";
            this.lblPowerValueDisplay.Size = new System.Drawing.Size(27, 13);
            this.lblPowerValueDisplay.TabIndex = 1;
            this.lblPowerValueDisplay.Text = "N/A";

            // lblTimestamp
            this.lblTimestamp.AutoSize = true;
            this.lblTimestamp.Location = new System.Drawing.Point(10, 48);
            this.lblTimestamp.Name = "lblTimestamp";
            this.lblTimestamp.Size = new System.Drawing.Size(61, 13);
            this.lblTimestamp.TabIndex = 2;
            this.lblTimestamp.Text = "Timestamp:";

            // lblTimestampDisplay
            this.lblTimestampDisplay.AutoSize = true;
            this.lblTimestampDisplay.Location = new System.Drawing.Point(86, 48);
            this.lblTimestampDisplay.Name = "lblTimestampDisplay";
            this.lblTimestampDisplay.Size = new System.Drawing.Size(27, 13);
            this.lblTimestampDisplay.TabIndex = 3;
            this.lblTimestampDisplay.Text = "N/A";

            // grpMultipleMeasurements
            this.grpMultipleMeasurements.Controls.Add(this.lblCount);
            this.grpMultipleMeasurements.Controls.Add(this.numCount);
            this.grpMultipleMeasurements.Controls.Add(this.lblDelay);
            this.grpMultipleMeasurements.Controls.Add(this.numDelay);
            this.grpMultipleMeasurements.Controls.Add(this.btnMeasureMultiple);
            this.grpMultipleMeasurements.Controls.Add(this.progressBar);
            this.grpMultipleMeasurements.Location = new System.Drawing.Point(10, 202);
            this.grpMultipleMeasurements.Name = "grpMultipleMeasurements";
            this.grpMultipleMeasurements.Size = new System.Drawing.Size(720, 100);
            this.grpMultipleMeasurements.TabIndex = 2;
            this.grpMultipleMeasurements.TabStop = false;
            this.grpMultipleMeasurements.Text = "Multiple Measurements";

            // lblCount
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(10, 22);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(38, 13);
            this.lblCount.TabIndex = 0;
            this.lblCount.Text = "Count:";

            // numCount
            this.numCount.Location = new System.Drawing.Point(76, 19);
            this.numCount.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numCount.Name = "numCount";
            this.numCount.Size = new System.Drawing.Size(100, 20);
            this.numCount.TabIndex = 1;
            this.numCount.Value = new decimal(new int[] { 10, 0, 0, 0 });

            // lblDelay
            this.lblDelay.AutoSize = true;
            this.lblDelay.Location = new System.Drawing.Point(10, 48);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(67, 13);
            this.lblDelay.TabIndex = 2;
            this.lblDelay.Text = "Delay (ms):";

            // numDelay
            this.numDelay.Location = new System.Drawing.Point(76, 45);
            this.numDelay.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numDelay.Name = "numDelay";
            this.numDelay.Size = new System.Drawing.Size(100, 20);
            this.numDelay.TabIndex = 3;
            this.numDelay.Value = new decimal(new int[] { 100, 0, 0, 0 });

            // btnMeasureMultiple
            this.btnMeasureMultiple.Location = new System.Drawing.Point(248, 19);
            this.btnMeasureMultiple.Name = "btnMeasureMultiple";
            this.btnMeasureMultiple.Size = new System.Drawing.Size(75, 47);
            this.btnMeasureMultiple.TabIndex = 4;
            this.btnMeasureMultiple.Text = "Measure Multiple";
            this.btnMeasureMultiple.UseVisualStyleBackColor = true;
            this.btnMeasureMultiple.Click += new System.EventHandler(this.btnMeasureMultiple_Click);

            // progressBar
            this.progressBar.Location = new System.Drawing.Point(10, 72);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(700, 20);
            this.progressBar.TabIndex = 5;

            // PowerMeasurementPanel
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpMultipleMeasurements);
            this.Controls.Add(this.grpResults);
            this.Controls.Add(this.grpMeasurement);
            this.Name = "PowerMeasurementPanel";
            this.Size = new System.Drawing.Size(740, 310);

            this.grpMeasurement.ResumeLayout(false);
            this.grpMeasurement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFrequency)).EndInit();
            this.grpResults.ResumeLayout(false);
            this.grpResults.PerformLayout();
            this.grpMultipleMeasurements.ResumeLayout(false);
            this.grpMultipleMeasurements.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpMeasurement;
        private System.Windows.Forms.Label lblFrequency;
        private System.Windows.Forms.NumericUpDown numFrequency;
        private System.Windows.Forms.ComboBox cmbFrequencyUnit;
        private System.Windows.Forms.Label lblSensor;
        private System.Windows.Forms.ComboBox cmbSensor;
        private System.Windows.Forms.Button btnMeasure;
        private System.Windows.Forms.GroupBox grpResults;
        private System.Windows.Forms.Label lblPowerValue;
        private System.Windows.Forms.Label lblPowerValueDisplay;
        private System.Windows.Forms.Label lblTimestamp;
        private System.Windows.Forms.Label lblTimestampDisplay;
        private System.Windows.Forms.GroupBox grpMultipleMeasurements;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.NumericUpDown numDelay;
        private System.Windows.Forms.Button btnMeasureMultiple;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}
