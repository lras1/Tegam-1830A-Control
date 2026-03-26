using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CalibrationTuning.Controllers;
using CalibrationTuning.Events;
using CalibrationTuning.Models;

namespace CalibrationTuning.UserControls
{
    /// <summary>
    /// User control for displaying real-time power measurement chart with
    /// scroll/zoom, running average, and stability statistics.
    /// </summary>
    public partial class ChartPanel : UserControl
    {
        private readonly ITuningController _tuningController;
        private Chart _chart;
        private Series _measurementSeries;
        private Series _runningAvgSeries;
        private Series _targetLineSeries;
        private Series _upperToleranceSeries;
        private Series _lowerToleranceSeries;

        // Statistics panel
        private GroupBox _statsGroup;
        private Label _countValue;
        private Label _avgValue;
        private Label _stdDevValue;
        private Label _minValue;
        private Label _maxValue;
        private Label _stabilityValue;
        private NumericUpDown _windowSizeNumeric;

        // Running statistics data
        private readonly List<double> _powerValues = new List<double>();
        private int _runningAvgWindow = 10;
        private int _dataPointIndex = 0;

        public ChartPanel(ITuningController tuningController)
        {
            _tuningController = tuningController ?? throw new ArgumentNullException(nameof(tuningController));

            InitializeComponent();
            InitializeUI();
            SubscribeToEvents();
        }

        private void InitializeUI()
        {
            this.SuspendLayout();

            // Statistics panel at top
            _statsGroup = new GroupBox
            {
                Text = "Power Statistics",
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(10, 5, 10, 5)
            };

            int x = 15, y = 20, colWidth = 130;

            AddStatLabel(_statsGroup, "Samples:", ref x, y);
            _countValue = AddStatValue(_statsGroup, "0", ref x, y, colWidth);

            AddStatLabel(_statsGroup, "Avg:", ref x, y);
            _avgValue = AddStatValue(_statsGroup, "-- dBm", ref x, y, colWidth);

            AddStatLabel(_statsGroup, "StdDev:", ref x, y);
            _stdDevValue = AddStatValue(_statsGroup, "-- dB", ref x, y, colWidth);

            x = 15; y = 45;
            AddStatLabel(_statsGroup, "Min:", ref x, y);
            _minValue = AddStatValue(_statsGroup, "-- dBm", ref x, y, colWidth);

            AddStatLabel(_statsGroup, "Max:", ref x, y);
            _maxValue = AddStatValue(_statsGroup, "-- dBm", ref x, y, colWidth);

            AddStatLabel(_statsGroup, "Stability:", ref x, y);
            _stabilityValue = AddStatValue(_statsGroup, "--", ref x, y, colWidth);
            _stabilityValue.Font = new Font(_stabilityValue.Font, FontStyle.Bold);

            // Window size control
            var windowLabel = new Label
            {
                Text = "Avg Window:",
                Location = new Point(x + 10, y),
                Size = new Size(70, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            _statsGroup.Controls.Add(windowLabel);

            _windowSizeNumeric = new NumericUpDown
            {
                Location = new Point(x + 82, y),
                Size = new Size(55, 20),
                Minimum = 2,
                Maximum = 100,
                Value = 10
            };
            _windowSizeNumeric.ValueChanged += (s, e) => { _runningAvgWindow = (int)_windowSizeNumeric.Value; RecalculateRunningAverage(); };
            _statsGroup.Controls.Add(_windowSizeNumeric);

            this.Controls.Add(_statsGroup);

            // Chart fills remaining space
            _chart = new Chart { Dock = DockStyle.Fill };
            InitializeChart();
            this.Controls.Add(_chart);

            // Chart must be added before statsGroup so Dock.Fill works under Dock.Top
            _chart.BringToFront();

            this.ResumeLayout(false);
        }

        private Label AddStatLabel(Control parent, string text, ref int x, int y)
        {
            var lbl = new Label { Text = text, Location = new Point(x, y), Size = new Size(50, 20), TextAlign = ContentAlignment.MiddleRight };
            parent.Controls.Add(lbl);
            x += 52;
            return lbl;
        }

        private Label AddStatValue(Control parent, string text, ref int x, int y, int colWidth)
        {
            var lbl = new Label { Text = text, Location = new Point(x, y), Size = new Size(75, 20), TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.DarkBlue };
            parent.Controls.Add(lbl);
            x += colWidth - 52;
            return lbl;
        }

        private void InitializeChart()
        {
            var chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "Sample #";
            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.Title = "Power (dBm)";
            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

            // Enable scroll and zoom
            chartArea.CursorX.IsUserEnabled = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.CursorY.IsUserEnabled = true;
            chartArea.CursorY.IsUserSelectionEnabled = true;
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisY.ScaleView.Zoomable = true;
            chartArea.AxisX.ScrollBar.IsPositionedInside = true;
            chartArea.AxisY.ScrollBar.IsPositionedInside = true;

            _chart.ChartAreas.Add(chartArea);

            _measurementSeries = new Series("Measurements")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 5,
                MarkerColor = Color.Blue
            };
            _chart.Series.Add(_measurementSeries);

            _runningAvgSeries = new Series("Running Avg")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkOrange,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Solid
            };
            _chart.Series.Add(_runningAvgSeries);

            _targetLineSeries = new Series("Target")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash
            };
            _chart.Series.Add(_targetLineSeries);

            _upperToleranceSeries = new Series("Upper Tol")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            _chart.Series.Add(_upperToleranceSeries);

            _lowerToleranceSeries = new Series("Lower Tol")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            _chart.Series.Add(_lowerToleranceSeries);

            var legend = new Legend("MainLegend") { Docking = Docking.Top, Alignment = StringAlignment.Far };
            _chart.Legends.Add(legend);

            // Mouse wheel zoom
            _chart.MouseWheel += Chart_MouseWheel;
        }

        private void Chart_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                var area = _chart.ChartAreas[0];
                if (e.Delta > 0)
                {
                    double xMin = area.AxisX.ScaleView.ViewMinimum;
                    double xMax = area.AxisX.ScaleView.ViewMaximum;
                    double xCenter = (xMin + xMax) / 2;
                    double xRange = (xMax - xMin) / 2;
                    area.AxisX.ScaleView.Zoom(xCenter - xRange * 0.5, xCenter + xRange * 0.5);
                }
                else
                {
                    area.AxisX.ScaleView.ZoomReset();
                    area.AxisY.ScaleView.ZoomReset();
                }
            }
            catch { }
        }

        private void SubscribeToEvents()
        {
            _tuningController.ProgressUpdated += TuningController_ProgressUpdated;
            _tuningController.StateChanged += TuningController_StateChanged;
        }

        private void TuningController_StateChanged(object sender, TuningStateChangedEventArgs e)
        {
            if (e.NewState == TuningState.Tuning && e.PreviousState == TuningState.Idle)
            {
                if (_tuningController.Parameters != null)
                {
                    SetTargetAndTolerance(
                        _tuningController.Parameters.TargetPowerDbm,
                        _tuningController.Parameters.MaxStdDevDb * _tuningController.Parameters.ConfidenceK
                    );
                }
            }
        }

        private void TuningController_ProgressUpdated(object sender, TuningProgressEventArgs e)
        {
            if (e.Statistics != null)
            {
                AddDataPoint(e.Statistics.CurrentIteration, e.Statistics.CurrentPowerDbm);
            }
        }

        public void AddDataPoint(int iteration, double powerDbm)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AddDataPoint(iteration, powerDbm)));
                return;
            }

            try
            {
                _dataPointIndex++;
                _powerValues.Add(powerDbm);
                _measurementSeries.Points.AddXY(_dataPointIndex, powerDbm);

                // Calculate and add running average point
                if (_powerValues.Count >= _runningAvgWindow)
                {
                    double avg = _powerValues.Skip(_powerValues.Count - _runningAvgWindow).Take(_runningAvgWindow).Average();
                    _runningAvgSeries.Points.AddXY(_dataPointIndex, avg);
                }

                // Extend target/tolerance lines to current X
                ExtendHorizontalLines(_dataPointIndex);

                UpdateStatistics();

                if (_measurementSeries.Points.Count > 0)
                {
                    _chart.ChartAreas[0].RecalculateAxesScale();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding data point to chart: {ex.Message}");
            }
        }

        private void ExtendHorizontalLines(int maxX)
        {
            // Keep target/tolerance lines spanning the full X range
            if (_targetLineSeries.Points.Count > 0)
            {
                double target = _targetLineSeries.Points[0].YValues[0];
                _targetLineSeries.Points.Clear();
                _targetLineSeries.Points.AddXY(0, target);
                _targetLineSeries.Points.AddXY(maxX, target);
            }
            if (_upperToleranceSeries.Points.Count > 0)
            {
                double upper = _upperToleranceSeries.Points[0].YValues[0];
                _upperToleranceSeries.Points.Clear();
                _upperToleranceSeries.Points.AddXY(0, upper);
                _upperToleranceSeries.Points.AddXY(maxX, upper);
            }
            if (_lowerToleranceSeries.Points.Count > 0)
            {
                double lower = _lowerToleranceSeries.Points[0].YValues[0];
                _lowerToleranceSeries.Points.Clear();
                _lowerToleranceSeries.Points.AddXY(0, lower);
                _lowerToleranceSeries.Points.AddXY(maxX, lower);
            }
        }

        private void UpdateStatistics()
        {
            if (_powerValues.Count == 0) return;

            int n = _powerValues.Count;
            double avg = _powerValues.Average();
            double min = _powerValues.Min();
            double max = _powerValues.Max();
            double variance = _powerValues.Sum(v => (v - avg) * (v - avg)) / n;
            double stdDev = Math.Sqrt(variance);

            _countValue.Text = n.ToString();
            _avgValue.Text = $"{avg:F3} dBm";
            _stdDevValue.Text = $"{stdDev:F3} dB";
            _minValue.Text = $"{min:F3} dBm";
            _maxValue.Text = $"{max:F3} dBm";

            // Stability: check using statistical criteria from parameters
            double maxStdDev = _tuningController.Parameters?.MaxStdDevDb ?? 0.5;
            double confidenceK = _tuningController.Parameters?.ConfidenceK ?? 2.0;
            double target = _tuningController.Parameters?.TargetPowerDbm ?? 0;
            int stabWindow = _tuningController.Parameters?.StabilityWindow ?? _runningAvgWindow;
            
            if (_powerValues.Count >= stabWindow)
            {
                var recent = _powerValues.Skip(_powerValues.Count - stabWindow).ToList();
                double recentAvg = recent.Average();
                double recentVariance = recent.Sum(v => (v - recentAvg) * (v - recentAvg)) / recent.Count;
                double recentStdDev = Math.Sqrt(recentVariance);
                double meanError = Math.Abs(recentAvg - target);

                // Stable: stdDev within limit AND mean within k*stdDev of target
                bool stdDevOk = recentStdDev <= maxStdDev;
                bool meanOk = meanError <= confidenceK * recentStdDev;

                if (stdDevOk && meanOk)
                {
                    _stabilityValue.Text = $"STABLE (σ={recentStdDev:F2})";
                    _stabilityValue.ForeColor = Color.Green;
                }
                else if (stdDevOk)
                {
                    _stabilityValue.Text = $"SETTLING (σ={recentStdDev:F2}, Δ={meanError:F2})";
                    _stabilityValue.ForeColor = Color.Orange;
                }
                else
                {
                    _stabilityValue.Text = $"UNSTABLE (σ={recentStdDev:F2})";
                    _stabilityValue.ForeColor = Color.Red;
                }
            }
            else
            {
                _stabilityValue.Text = $"Need {stabWindow} pts";
                _stabilityValue.ForeColor = Color.Gray;
            }
        }

        private void RecalculateRunningAverage()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RecalculateRunningAverage));
                return;
            }

            _runningAvgSeries.Points.Clear();
            for (int i = _runningAvgWindow - 1; i < _powerValues.Count; i++)
            {
                double avg = 0;
                for (int j = i - _runningAvgWindow + 1; j <= i; j++)
                    avg += _powerValues[j];
                avg /= _runningAvgWindow;
                _runningAvgSeries.Points.AddXY(i + 1, avg);
            }
            UpdateStatistics();
        }

        public void SetTargetAndTolerance(double targetDbm, double toleranceDb)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SetTargetAndTolerance(targetDbm, toleranceDb)));
                return;
            }

            try
            {
                _targetLineSeries.Points.Clear();
                _upperToleranceSeries.Points.Clear();
                _lowerToleranceSeries.Points.Clear();

                int maxX = Math.Max(_dataPointIndex, 1);
                _targetLineSeries.Points.AddXY(0, targetDbm);
                _targetLineSeries.Points.AddXY(maxX, targetDbm);
                _upperToleranceSeries.Points.AddXY(0, targetDbm + toleranceDb);
                _upperToleranceSeries.Points.AddXY(maxX, targetDbm + toleranceDb);
                _lowerToleranceSeries.Points.AddXY(0, targetDbm - toleranceDb);
                _lowerToleranceSeries.Points.AddXY(maxX, targetDbm - toleranceDb);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting target and tolerance: {ex.Message}");
            }
        }

        public void ClearChart()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ClearChart));
                return;
            }

            try
            {
                _measurementSeries.Points.Clear();
                _runningAvgSeries.Points.Clear();
                _targetLineSeries.Points.Clear();
                _upperToleranceSeries.Points.Clear();
                _lowerToleranceSeries.Points.Clear();
                _powerValues.Clear();
                _dataPointIndex = 0;
                UpdateStatistics();
                _countValue.Text = "0";
                _avgValue.Text = "-- dBm";
                _stdDevValue.Text = "-- dB";
                _minValue.Text = "-- dBm";
                _maxValue.Text = "-- dBm";
                _stabilityValue.Text = "--";
                _stabilityValue.ForeColor = Color.Gray;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing chart: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tuningController.ProgressUpdated -= TuningController_ProgressUpdated;
                _tuningController.StateChanged -= TuningController_StateChanged;
            }
            base.Dispose(disposing);
        }
    }
}
