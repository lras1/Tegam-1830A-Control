using System;
using System.Drawing;
using System.Windows.Forms;
using CalibrationTuning.Controllers;
using CalibrationTuning.UserControls;

namespace CalibrationTuning
{
    /// <summary>
    /// Main application form with tab-based interface for device connection, tuning, and visualization.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly ITuningController _tuningController;
        private readonly IDataLoggingController _dataLoggingController;
        private readonly IConfigurationController _configurationController;

        // User controls
        private ConnectionPanel _connectionPanel;
        private TuningPanel _tuningPanel;

        public MainForm(
            ITuningController tuningController,
            IDataLoggingController dataLoggingController,
            IConfigurationController configurationController)
        {
            _tuningController = tuningController ?? throw new ArgumentNullException(nameof(tuningController));
            _dataLoggingController = dataLoggingController ?? throw new ArgumentNullException(nameof(dataLoggingController));
            _configurationController = configurationController ?? throw new ArgumentNullException(nameof(configurationController));

            InitializeComponent();
            InitializeControls();
            InitializeUserControls();
            InitializeEventHandlers();
        }

        private void InitializeControls()
        {
            // Create menu bar
            CreateMenuBar();

            // Create tab control
            CreateTabControl();

            // Create status bar
            CreateStatusBar();
        }

        private MenuStrip _menuStrip;
        private ToolStripMenuItem _fileMenu;
        private ToolStripMenuItem _exitMenuItem;
        private ToolStripMenuItem _helpMenu;
        private ToolStripMenuItem _aboutMenuItem;

        private void CreateMenuBar()
        {
            _menuStrip = new MenuStrip
            {
                Dock = DockStyle.Top
            };

            // File menu
            _fileMenu = new ToolStripMenuItem("&File");
            
            _exitMenuItem = new ToolStripMenuItem("E&xit");
            _exitMenuItem.Click += ExitMenuItem_Click;
            _exitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            
            _fileMenu.DropDownItems.Add(_exitMenuItem);

            // Help menu
            _helpMenu = new ToolStripMenuItem("&Help");
            
            _aboutMenuItem = new ToolStripMenuItem("&About");
            _aboutMenuItem.Click += AboutMenuItem_Click;
            
            _helpMenu.DropDownItems.Add(_aboutMenuItem);

            _menuStrip.Items.Add(_fileMenu);
            _menuStrip.Items.Add(_helpMenu);

            this.Controls.Add(_menuStrip);
            this.MainMenuStrip = _menuStrip;
        }

        private TabControl _tabControl;
        private TabPage _connectionTab;
        private TabPage _tuningTab;
        private TabPage _chartTab;

        private void CreateTabControl()
        {
            _tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // Connection tab
            _connectionTab = new TabPage("Connection")
            {
                Padding = new Padding(10)
            };
            _tabControl.TabPages.Add(_connectionTab);

            // Tuning tab
            _tuningTab = new TabPage("Tuning")
            {
                Padding = new Padding(10)
            };
            _tabControl.TabPages.Add(_tuningTab);

            // Chart tab
            _chartTab = new TabPage("Chart")
            {
                Padding = new Padding(10)
            };
            _tabControl.TabPages.Add(_chartTab);

            this.Controls.Add(_tabControl);
        }

        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _connectionStatusLabel;
        private ToolStripStatusLabel _tuningStatusLabel;

        private void CreateStatusBar()
        {
            _statusStrip = new StatusStrip
            {
                Dock = DockStyle.Bottom
            };

            _connectionStatusLabel = new ToolStripStatusLabel
            {
                Text = "Devices: Disconnected",
                BorderSides = ToolStripStatusLabelBorderSides.Right,
                BorderStyle = Border3DStyle.Etched,
                Spring = false,
                AutoSize = true
            };

            _tuningStatusLabel = new ToolStripStatusLabel
            {
                Text = "Status: Idle",
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _statusStrip.Items.Add(_connectionStatusLabel);
            _statusStrip.Items.Add(_tuningStatusLabel);

            this.Controls.Add(_statusStrip);
        }

        private void InitializeUserControls()
        {
            // Create and add ConnectionPanel to Connection tab
            _connectionPanel = new ConnectionPanel(_tuningController, this)
            {
                Dock = DockStyle.Fill
            };
            _connectionTab.Controls.Add(_connectionPanel);

            // Create and add TuningPanel to Tuning tab
            _tuningPanel = new TuningPanel(_tuningController, this)
            {
                Dock = DockStyle.Fill
            };
            _tuningTab.Controls.Add(_tuningPanel);
        }

        private void InitializeEventHandlers()
        {
            // Subscribe to tuning controller events for status updates
            _tuningController.StateChanged += TuningController_StateChanged;
            
            // Form lifecycle events
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Load configuration from storage
            LoadConfiguration();
            
            // TODO: Initialize remaining user controls in Tasks 6.3, 7.1, 8.1
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save configuration before closing
            SaveConfiguration();
            
            // Disconnect devices if connected
            _tuningController.DisconnectDevices();
        }

        private void TuningController_StateChanged(object sender, Events.TuningStateChangedEventArgs e)
        {
            // Update status bar with current tuning state
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateTuningStatus(e.NewState)));
            }
            else
            {
                UpdateTuningStatus(e.NewState);
            }
        }

        private void UpdateTuningStatus(Models.TuningState state)
        {
            _tuningStatusLabel.Text = $"Status: {state}";
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Calibration Tuning Application\n\n" +
                "Integrates Tegam 1830A Power Meter and Siglent SDG6052X Signal Generator\n" +
                "for automated RF power calibration.\n\n" +
                "Version 1.0",
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // Public properties to access tabs for adding user controls
        public TabPage ConnectionTab => _connectionTab;
        public TabPage TuningTab => _tuningTab;
        public TabPage ChartTab => _chartTab;

        // Public property to access ConnectionPanel for configuration
        public ConnectionPanel ConnectionPanel => _connectionPanel;

        // Public property to access TuningPanel for configuration
        public TuningPanel TuningPanel => _tuningPanel;

        /// <summary>
        /// Loads configuration from storage and populates UI controls.
        /// </summary>
        private void LoadConfiguration()
        {
            try
            {
                // Load device IP addresses
                var deviceConfig = _configurationController.LoadDeviceConfiguration();
                if (deviceConfig != null)
                {
                    _connectionPanel.PowerMeterIpAddress = deviceConfig.PowerMeterIpAddress;
                    _connectionPanel.SignalGeneratorIpAddress = deviceConfig.SignalGeneratorIpAddress;
                }

                // Load tuning parameters
                var tuningParams = _configurationController.LoadLastParameters();
                if (tuningParams != null)
                {
                    _tuningPanel.SetTuningParameters(tuningParams);
                }

                // Load last log file path
                // Note: Log file path control will be added in Task 9.1
                // For now, we just load it to ensure it's available when needed
                var lastLogPath = _configurationController.LoadLastLogPath();
                // Store for future use when log file path control is implemented
            }
            catch (Exception ex)
            {
                // Log error but don't prevent application from starting
                MessageBox.Show(
                    $"Warning: Failed to load configuration.\n\n{ex.Message}\n\nDefault values will be used.",
                    "Configuration Load Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Saves current configuration to storage.
        /// </summary>
        private void SaveConfiguration()
        {
            try
            {
                // Save device IP addresses
                var deviceConfig = new Models.DeviceConfiguration
                {
                    PowerMeterIpAddress = _connectionPanel.PowerMeterIpAddress,
                    SignalGeneratorIpAddress = _connectionPanel.SignalGeneratorIpAddress
                };
                _configurationController.SaveDeviceConfiguration(deviceConfig);

                // Save tuning parameters
                var tuningParams = _tuningPanel.GetTuningParameters();
                _configurationController.SaveParameters(tuningParams);

                // Save last log file path
                // Note: Log file path control will be added in Task 9.1
                // For now, we save the current log file from DataLoggingController if available
                if (!string.IsNullOrEmpty(_dataLoggingController.CurrentLogFile))
                {
                    _configurationController.SaveLastLogPath(_dataLoggingController.CurrentLogFile);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't prevent application from closing
                MessageBox.Show(
                    $"Warning: Failed to save configuration.\n\n{ex.Message}",
                    "Configuration Save Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // Public method to update connection status from external controls
        public void UpdateConnectionStatus(bool powerMeterConnected, bool signalGenConnected)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateConnectionStatusInternal(powerMeterConnected, signalGenConnected)));
            }
            else
            {
                UpdateConnectionStatusInternal(powerMeterConnected, signalGenConnected);
            }
        }

        private void UpdateConnectionStatusInternal(bool powerMeterConnected, bool signalGenConnected)
        {
            bool bothConnected = powerMeterConnected && signalGenConnected;

            if (bothConnected)
            {
                _connectionStatusLabel.Text = "Devices: Connected";
            }
            else if (powerMeterConnected || signalGenConnected)
            {
                _connectionStatusLabel.Text = "Devices: Partially Connected";
            }
            else
            {
                _connectionStatusLabel.Text = "Devices: Disconnected";
            }

            // Update TuningPanel connection state
            _tuningPanel?.SetDevicesConnected(bothConnected);
        }
    }
}
