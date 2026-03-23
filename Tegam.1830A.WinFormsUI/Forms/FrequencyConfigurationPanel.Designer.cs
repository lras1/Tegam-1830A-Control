namespace Tegam.WinFormsUI.Forms
{
    partial class FrequencyConfigurationPanel
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
            this.grpSetFrequency = new System.Windows.Forms.GroupBox();
            this.lblFrequency = new System.Windows.Forms.Label();
            this.numFrequency = new System.Windows.Forms.NumericUpDown();
            this.cmbUnit = new System.Windows.Forms.ComboBox();
            this.btnSetFrequency = new System.Windows.Forms.Button();
            this.grpCurrentFrequency = new System.Windows.Forms.GroupBox();
            this.lblCurrentFrequencyValue = new System.Windows.Forms.Label();
            this.btnQueryFrequency = new System.Windows.Forms.Button();
            this.grpFrequencyRange = new System.Windows.Forms.GroupBox();
            this.lblRangeInfo = new System.Windows.Forms.Label();

            this.grpSetFrequency.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFrequency)).BeginInit();
            this.grpCurrentFrequency.SuspendLayout();
            this.grpFrequencyRange.SuspendLayout();
            this.SuspendLayout();

            // grpSetFrequency
            this.grpSetFrequency.Controls.Add(this.lblFrequency);
            this.grpSetFrequency.Controls.Add(this.numFrequency);
            this.grpSetFrequency.Controls.Add(this.cmbUnit);
            this.grpSetFrequency.Controls.Add(this.btnSetFrequency);
            this.grpSetFrequency.Location = new System.Drawing.Point(10, 10);
            this.grpSetFrequency.Name = "grpSetFrequency";
            this.grpSetFrequency.Size = new System.Drawing.Size(720, 80);
            this.grpSetFrequency.TabIndex = 0;
            this.grpSetFrequency.TabStop = false;
            this.grpSetFrequency.Text = "Set Frequency";

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

            // cmbUnit
            this.cmbUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUnit.FormattingEnabled = true;
            this.cmbUnit.Items.AddRange(new object[] { "Hz", "kHz", "MHz", "GHz" });
            this.cmbUnit.Location = new System.Drawing.Point(182, 19);
            this.cmbUnit.Name = "cmbUnit";
            this.cmbUnit.Size = new System.Drawing.Size(60, 21);
            this.cmbUnit.TabIndex = 2;
            this.cmbUnit.Text = "MHz";

            // btnSetFrequency
            this.btnSetFrequency.Location = new System.Drawing.Point(248, 19);
            this.btnSetFrequency.Name = "btnSetFrequency";
            this.btnSetFrequency.Size = new System.Drawing.Size(100, 47);
            this.btnSetFrequency.TabIndex = 3;
            this.btnSetFrequency.Text = "Set Frequency";
            this.btnSetFrequency.UseVisualStyleBackColor = true;
            this.btnSetFrequency.Click += new System.EventHandler(this.btnSetFrequency_Click);

            // grpCurrentFrequency
            this.grpCurrentFrequency.Controls.Add(this.lblCurrentFrequencyValue);
            this.grpCurrentFrequency.Controls.Add(this.btnQueryFrequency);
            this.grpCurrentFrequency.Location = new System.Drawing.Point(10, 96);
            this.grpCurrentFrequency.Name = "grpCurrentFrequency";
            this.grpCurrentFrequency.Size = new System.Drawing.Size(720, 80);
            this.grpCurrentFrequency.TabIndex = 1;
            this.grpCurrentFrequency.TabStop = false;
            this.grpCurrentFrequency.Text = "Current Frequency";

            // lblCurrentFrequencyValue
            this.lblCurrentFrequencyValue.AutoSize = true;
            this.lblCurrentFrequencyValue.Location = new System.Drawing.Point(10, 22);
            this.lblCurrentFrequencyValue.Name = "lblCurrentFrequencyValue";
            this.lblCurrentFrequencyValue.Size = new System.Drawing.Size(27, 13);
            this.lblCurrentFrequencyValue.TabIndex = 0;
            this.lblCurrentFrequencyValue.Text = "N/A";

            // btnQueryFrequency
            this.btnQueryFrequency.Location = new System.Drawing.Point(248, 19);
            this.btnQueryFrequency.Name = "btnQueryFrequency";
            this.btnQueryFrequency.Size = new System.Drawing.Size(100, 47);
            this.btnQueryFrequency.TabIndex = 1;
            this.btnQueryFrequency.Text = "Query Frequency";
            this.btnQueryFrequency.UseVisualStyleBackColor = true;
            this.btnQueryFrequency.Click += new System.EventHandler(this.btnQueryFrequency_Click);

            // grpFrequencyRange
            this.grpFrequencyRange.Controls.Add(this.lblRangeInfo);
            this.grpFrequencyRange.Location = new System.Drawing.Point(10, 182);
            this.grpFrequencyRange.Name = "grpFrequencyRange";
            this.grpFrequencyRange.Size = new System.Drawing.Size(720, 80);
            this.grpFrequencyRange.TabIndex = 2;
            this.grpFrequencyRange.TabStop = false;
            this.grpFrequencyRange.Text = "Frequency Range";

            // lblRangeInfo
            this.lblRangeInfo.AutoSize = true;
            this.lblRangeInfo.Location = new System.Drawing.Point(10, 22);
            this.lblRangeInfo.Name = "lblRangeInfo";
            this.lblRangeInfo.Size = new System.Drawing.Size(200, 13);
            this.lblRangeInfo.TabIndex = 0;
            this.lblRangeInfo.Text = "Valid frequency range: 100 kHz to 40 GHz";

            // FrequencyConfigurationPanel
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpFrequencyRange);
            this.Controls.Add(this.grpCurrentFrequency);
            this.Controls.Add(this.grpSetFrequency);
            this.Name = "FrequencyConfigurationPanel";
            this.Size = new System.Drawing.Size(740, 310);

            this.grpSetFrequency.ResumeLayout(false);
            this.grpSetFrequency.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFrequency)).EndInit();
            this.grpCurrentFrequency.ResumeLayout(false);
            this.grpCurrentFrequency.PerformLayout();
            this.grpFrequencyRange.ResumeLayout(false);
            this.grpFrequencyRange.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpSetFrequency;
        private System.Windows.Forms.Label lblFrequency;
        private System.Windows.Forms.NumericUpDown numFrequency;
        private System.Windows.Forms.ComboBox cmbUnit;
        private System.Windows.Forms.Button btnSetFrequency;
        private System.Windows.Forms.GroupBox grpCurrentFrequency;
        private System.Windows.Forms.Label lblCurrentFrequencyValue;
        private System.Windows.Forms.Button btnQueryFrequency;
        private System.Windows.Forms.GroupBox grpFrequencyRange;
        private System.Windows.Forms.Label lblRangeInfo;
    }
}
