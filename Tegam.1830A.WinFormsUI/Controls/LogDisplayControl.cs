using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam.WinFormsUI.Controls
{
    /// <summary>
    /// UserControl for displaying log entries in a ListView with color coding and auto-scroll.
    /// Maintains a circular buffer of recent entries (default 100).
    /// </summary>
    public partial class LogDisplayControl : UserControl
    {
        private ListView _listView;
        private int _maxEntries = 100;
        private bool _autoScroll = true;
        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the LogDisplayControl class.
        /// </summary>
        public LogDisplayControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Create ListView
            _listView = new ListView();
            _listView.Dock = DockStyle.Fill;
            _listView.View = View.Details;
            _listView.FullRowSelect = true;
            _listView.GridLines = true;
            _listView.MultiSelect = false;

            // Add columns
            _listView.Columns.Add("Type", 80);
            _listView.Columns.Add("Timestamp", 120);
            _listView.Columns.Add("Details", 400);

            this.Controls.Add(_listView);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Adds a log entry to the display with thread-safe invocation.
        /// Applies color coding and maintains circular buffer size.
        /// </summary>
        /// <param name="entry">The log entry to add.</param>
        public void AddEntry(LogEntry entry)
        {
            if (entry == null)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action<LogEntry>(AddEntry), entry);
                return;
            }

            lock (_lock)
            {
                // Create list view item
                var item = new ListViewItem(entry.Type);
                item.SubItems.Add(entry.Timestamp.ToString("HH:mm:ss.fff"));
                item.SubItems.Add(entry.ToDisplayString());

                // Apply color coding
                if (entry.Type == "Data")
                {
                    item.ForeColor = Color.Blue;
                }
                else if (entry.Type == "Setting")
                {
                    item.ForeColor = Color.Green;
                }

                // Add to ListView
                _listView.Items.Add(item);

                // Maintain circular buffer - remove oldest if exceeds max
                while (_listView.Items.Count > _maxEntries)
                {
                    _listView.Items.RemoveAt(0);
                }

                // Auto-scroll to bottom if enabled
                if (_autoScroll && _listView.Items.Count > 0)
                {
                    _listView.EnsureVisible(_listView.Items.Count - 1);
                }
            }
        }

        /// <summary>
        /// Clears all entries from the display.
        /// </summary>
        public void Clear()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Clear));
                return;
            }

            lock (_lock)
            {
                _listView.Items.Clear();
            }
        }

        /// <summary>
        /// Sets the maximum number of entries to display (circular buffer size).
        /// </summary>
        /// <param name="maxEntries">Maximum number of entries to keep in the display.</param>
        public void SetMaxEntries(int maxEntries)
        {
            if (maxEntries < 1)
                throw new ArgumentOutOfRangeException(nameof(maxEntries), "Max entries must be at least 1");

            lock (_lock)
            {
                _maxEntries = maxEntries;

                // Trim existing entries if needed
                if (InvokeRequired)
                {
                    Invoke(new Action(() => TrimToMaxEntries()));
                }
                else
                {
                    TrimToMaxEntries();
                }
            }
        }

        /// <summary>
        /// Sets whether the control should auto-scroll to the latest entry.
        /// </summary>
        /// <param name="autoScroll">True to enable auto-scroll, false to disable.</param>
        public void SetAutoScroll(bool autoScroll)
        {
            lock (_lock)
            {
                _autoScroll = autoScroll;
            }
        }

        private void TrimToMaxEntries()
        {
            while (_listView.Items.Count > _maxEntries)
            {
                _listView.Items.RemoveAt(0);
            }
        }
    }
}
