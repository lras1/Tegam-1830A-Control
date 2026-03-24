namespace CalibrationTuning
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this._fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._tabControl = new System.Windows.Forms.TabControl();
            this._connectionTab = new System.Windows.Forms.TabPage();
            this._tuningTab = new System.Windows.Forms.TabPage();
            this._chartTab = new System.Windows.Forms.TabPage();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._connectionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._tuningStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._menuStrip.SuspendLayout();
            this._tabControl.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _menuStrip
            // 
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._fileMenu,
            this._helpMenu});
            this._menuStrip.Location = new System.Drawing.Point(0, 0);
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.Size = new System.Drawing.Size(1024, 24);
            this._menuStrip.TabIndex = 0;
            this._menuStrip.Text = "menuStrip1";
            // 
            // _fileMenu
            // 
            this._fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._exitMenuItem});
            this._fileMenu.Name = "_fileMenu";
            this._fileMenu.Size = new System.Drawing.Size(37, 20);
            this._fileMenu.Text = "&File";
            // 
            // _exitMenuItem
            // 
            this._exitMenuItem.Name = "_exitMenuItem";
            this._exitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this._exitMenuItem.Size = new System.Drawing.Size(135, 22);
            this._exitMenuItem.Text = "E&xit";
            // 
            // _helpMenu
            // 
            this._helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._aboutMenuItem});
            this._helpMenu.Name = "_helpMenu";
            this._helpMenu.Size = new System.Drawing.Size(44, 20);
            this._helpMenu.Text = "&Help";
            // 
            // _aboutMenuItem
            // 
            this._aboutMenuItem.Name = "_aboutMenuItem";
            this._aboutMenuItem.Size = new System.Drawing.Size(107, 22);
            this._aboutMenuItem.Text = "&About";
            // 
            // _tabControl
            // 
            this._tabControl.Controls.Add(this._connectionTab);
            this._tabControl.Controls.Add(this._tuningTab);
            this._tabControl.Controls.Add(this._chartTab);
            this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabControl.Location = new System.Drawing.Point(0, 24);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(1024, 722);
            this._tabControl.TabIndex = 1;
            // 
            // _connectionTab
            // 
            this._connectionTab.Location = new System.Drawing.Point(4, 22);
            this._connectionTab.Name = "_connectionTab";
            this._connectionTab.Padding = new System.Windows.Forms.Padding(10);
            this._connectionTab.Size = new System.Drawing.Size(1016, 696);
            this._connectionTab.TabIndex = 0;
            this._connectionTab.Text = "Connection";
            this._connectionTab.UseVisualStyleBackColor = true;
            // 
            // _tuningTab
            // 
            this._tuningTab.Location = new System.Drawing.Point(4, 22);
            this._tuningTab.Name = "_tuningTab";
            this._tuningTab.Padding = new System.Windows.Forms.Padding(10);
            this._tuningTab.Size = new System.Drawing.Size(1016, 696);
            this._tuningTab.TabIndex = 1;
            this._tuningTab.Text = "Tuning";
            this._tuningTab.UseVisualStyleBackColor = true;
            // 
            // _chartTab
            // 
            this._chartTab.Location = new System.Drawing.Point(4, 22);
            this._chartTab.Name = "_chartTab";
            this._chartTab.Padding = new System.Windows.Forms.Padding(10);
            this._chartTab.Size = new System.Drawing.Size(1016, 696);
            this._chartTab.TabIndex = 2;
            this._chartTab.Text = "Chart";
            this._chartTab.UseVisualStyleBackColor = true;
            // 
            // _statusStrip
            // 
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._connectionStatusLabel,
            this._tuningStatusLabel});
            this._statusStrip.Location = new System.Drawing.Point(0, 746);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.Size = new System.Drawing.Size(1024, 22);
            this._statusStrip.TabIndex = 2;
            this._statusStrip.Text = "statusStrip1";
            // 
            // _connectionStatusLabel
            // 
            this._connectionStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this._connectionStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this._connectionStatusLabel.Name = "_connectionStatusLabel";
            this._connectionStatusLabel.Size = new System.Drawing.Size(127, 17);
            this._connectionStatusLabel.Text = "Devices: Disconnected";
            // 
            // _tuningStatusLabel
            // 
            this._tuningStatusLabel.Name = "_tuningStatusLabel";
            this._tuningStatusLabel.Size = new System.Drawing.Size(882, 17);
            this._tuningStatusLabel.Spring = true;
            this._tuningStatusLabel.Text = "Status: Idle";
            this._tuningStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this._tabControl);
            this.Controls.Add(this._statusStrip);
            this.Controls.Add(this._menuStrip);
            this.MainMenuStrip = this._menuStrip;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calibration Tuning Application";
            this._menuStrip.ResumeLayout(false);
            this._menuStrip.PerformLayout();
            this._tabControl.ResumeLayout(false);
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip _menuStrip;
        private System.Windows.Forms.ToolStripMenuItem _fileMenu;
        private System.Windows.Forms.ToolStripMenuItem _exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _helpMenu;
        private System.Windows.Forms.ToolStripMenuItem _aboutMenuItem;
        private System.Windows.Forms.TabControl _tabControl;
        private System.Windows.Forms.TabPage _connectionTab;
        private System.Windows.Forms.TabPage _tuningTab;
        private System.Windows.Forms.TabPage _chartTab;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _connectionStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel _tuningStatusLabel;
    }
}
