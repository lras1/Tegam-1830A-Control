namespace Tegam.WinFormsUI.Forms
{
    partial class SensorManagementPanel
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
            this.grpSelectSensor = new System.Windows.Forms.GroupBox();
            this.lblSensor = new System.Windows.Forms.Label();
            this.cmbSensor = new System.Windows.Forms.ComboBox();
            this.btnSelectSensor = new System.Windows.Forms.Button();
            this.grpCurrentSensor = new System.Windows.Forms.GroupBox();
            this.lblCurrentSensorValue = new System.Windows.Forms.Label();
            this.btnQueryCurrentSensor = new System.Windows.Forms.Button();
            this.grpAvailableSensors = new System.Windows.Forms.GroupBox();
            this.lstAvailableSensors = new System.Windows.Forms.ListBox();
            this.btnQueryAvailableSensors = new System.Windows.Forms.Button();

            this.grpSelectSensor.SuspendLayout();
            this.grpCurrentSensor.SuspendLayout();
            this.grpAvailableSensors.SuspendLayout();
            this.SuspendLayout();

            // grpSelectSensor
            this.grpSelectSensor.Controls.Add(this.lblSensor);
            this.grpSelectSensor.Controls.Add(this.cmbSensor);
            this.grpSelectSensor.Controls.Add(this.btnSelectSensor);
            this.grpSelectSensor.Location = new System.Drawing.Point(10, 10);
            this.grpSelectSensor.Name = "grpSelectSensor";
            this.grpSelectSensor.Size = new System.Drawing.Size(720, 80);
            this.grpSelectSensor.TabIndex = 0;
            this.grpSelectSensor.TabStop = false;
            this.grpSelectSensor.Text = "Select Sensor";

            // lblSensor
            this.lblSensor.AutoSize = true;
            this.lblSensor.Location = new System.Drawing.Point(10, 22);
            this.lblSensor.Name = "lblSensor";
            this.lblSensor.Size = new System.Drawing.Size(41, 13);
            this.lblSensor.TabIndex = 0;
            this.lblSensor.Text = "Sensor:";

            // cmbSensor
            this.cmbSensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSensor.FormattingEnabled = true;
            this.cmbSensor.Items.AddRange(new object[] { "1", "2", "3", "4" });
            this.cmbSensor.Location = new System.Drawing.Point(76, 19);
            this.cmbSensor.Name = "cmbSensor";
            this.cmbSensor.Size = new System.Drawing.Size(60, 21);
            this.cmbSensor.TabIndex = 1;
            this.cmbSensor.Text = "1";

            // btnSelectSensor
            this.btnSelectSensor.Location = new System.Drawing.Point(248, 19);
            this.btnSelectSensor.Name = "btnSelectSensor";
            this.btnSelectSensor.Size = new System.Drawing.Size(100, 47);
            this.btnSelectSensor.TabIndex = 2;
            this.btnSelectSensor.Text = "Select Sensor";
            this.btnSelectSensor.UseVisualStyleBackColor = true;
            this.btnSelectSensor.Click += new System.EventHandler(this.btnSelectSensor_Click);

            // grpCurrentSensor
            this.grpCurrentSensor.Controls.Add(this.lblCurrentSensorValue);
            this.grpCurrentSensor.Controls.Add(this.btnQueryCurrentSensor);
            this.grpCurrentSensor.Location = new System.Drawing.Point(10, 96);
            this.grpCurrentSensor.Name = "grpCurrentSensor";
            this.grpCurrentSensor.Size = new System.Drawing.Size(720, 80);
            this.grpCurrentSensor.TabIndex = 1;
            this.grpCurrentSensor.TabStop = false;
            this.grpCurrentSensor.Text = "Current Sensor";

            // lblCurrentSensorValue
            this.lblCurrentSensorValue.AutoSize = true;
            this.lblCurrentSensorValue.Location = new System.Drawing.Point(10, 22);
            this.lblCurrentSensorValue.Name = "lblCurrentSensorValue";
            this.lblCurrentSensorValue.Size = new System.Drawing.Size(27, 13);
            this.lblCurrentSensorValue.TabIndex = 0;
            this.lblCurrentSensorValue.Text = "N/A";

            // btnQueryCurrentSensor
            this.btnQueryCurrentSensor.Location = new System.Drawing.Point(248, 19);
            this.btnQueryCurrentSensor.Name = "btnQueryCurrentSensor";
            this.btnQueryCurrentSensor.Size = new System.Drawing.Size(100, 47);
            this.btnQueryCurrentSensor.TabIndex = 1;
            this.btnQueryCurrentSensor.Text = "Query Current Sensor";
            this.btnQueryCurrentSensor.UseVisualStyleBackColor = true;
            this.btnQueryCurrentSensor.Click += new System.EventHandler(this.btnQueryCurrentSensor_Click);

            // grpAvailableSensors
            this.grpAvailableSensors.Controls.Add(this.lstAvailableSensors);
            this.grpAvailableSensors.Controls.Add(this.btnQueryAvailableSensors);
            this.grpAvailableSensors.Location = new System.Drawing.Point(10, 182);
            this.grpAvailableSensors.Name = "grpAvailableSensors";
            this.grpAvailableSensors.Size = new System.Drawing.Size(720, 120);
            this.grpAvailableSensors.TabIndex = 2;
            this.grpAvailableSensors.TabStop = false;
            this.grpAvailableSensors.Text = "Available Sensors";

            // lstAvailableSensors
            this.lstAvailableSensors.FormattingEnabled = true;
            this.lstAvailableSensors.Location = new System.Drawing.Point(10, 22);
            this.lstAvailableSensors.Name = "lstAvailableSensors";
            this.lstAvailableSensors.Size = new System.Drawing.Size(700, 69);
            this.lstAvailableSensors.TabIndex = 0;

            // btnQueryAvailableSensors
            this.btnQueryAvailableSensors.Location = new System.Drawing.Point(248, 97);
            this.btnQueryAvailableSensors.Name = "btnQueryAvailableSensors";
            this.btnQueryAvailableSensors.Size = new System.Drawing.Size(100, 23);
            this.btnQueryAvailableSensors.TabIndex = 1;
            this.btnQueryAvailableSensors.Text = "Query Sensors";
            this.btnQueryAvailableSensors.UseVisualStyleBackColor = true;
            this.btnQueryAvailableSensors.Click += new System.EventHandler(this.btnQueryAvailableSensors_Click);

            // SensorManagementPanel
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpAvailableSensors);
            this.Controls.Add(this.grpCurrentSensor);
            this.Controls.Add(this.grpSelectSensor);
            this.Name = "SensorManagementPanel";
            this.Size = new System.Drawing.Size(740, 310);

            this.grpSelectSensor.ResumeLayout(false);
            this.grpSelectSensor.PerformLayout();
            this.grpCurrentSensor.ResumeLayout(false);
            this.grpCurrentSensor.PerformLayout();
            this.grpAvailableSensors.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpSelectSensor;
        private System.Windows.Forms.Label lblSensor;
        private System.Windows.Forms.ComboBox cmbSensor;
        private System.Windows.Forms.Button btnSelectSensor;
        private System.Windows.Forms.GroupBox grpCurrentSensor;
        private System.Windows.Forms.Label lblCurrentSensorValue;
        private System.Windows.Forms.Button btnQueryCurrentSensor;
        private System.Windows.Forms.GroupBox grpAvailableSensors;
        private System.Windows.Forms.ListBox lstAvailableSensors;
        private System.Windows.Forms.Button btnQueryAvailableSensors;
    }
}
