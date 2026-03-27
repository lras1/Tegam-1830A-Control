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
    public partial class ChartPanel : UserControl
    {
        private readonly ITuningController _tuningController;

        // Tuning chart
        private Chart _tuningChart;
        private Series _tuningSeries;
        private Series _runningAvgSeries;
        private Series _targetSeries;
        private Series _upperTolSeries;
        private Series _lowerTolSeries;
        private readonly List<double> _tuningValues = new List<double>();
        private int _tuningPointIndex = 0;
        private int _runningAvgWindow = 10;

        // Tuning stats labels
        private Label _tCountVal, _tAvgVal, _tStdDevVal, _tMinVal, _tMaxVal, _tStabilityVal;

        // Manual chart
        private Chart _manualChart;
        private Series _manualSeries;
        private Series _manualAvgSeries;
        private readonly List<double> _manualValues = new List<double>();
        private int _manualPointIndex = 0;

        // Manual stats labels
        private Label _mCountVal, _mAvgVal, _mStdDevVal, _mMinVal, _mMaxVal;

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

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 350,
                Panel1MinSize = 150,
                Panel2MinSize = 150
            };

            // === Tuning panel (top) ===
            var tuningGroup = new GroupBox { Text = "Tuning Measurements", Dock = DockStyle.Fill };
            var tuningStatsPanel = CreateStatsRow(out _tCountVal, out _tAvgVal, out _tStdDevVal, out _tMinVal, out _tMaxVal, out _tStabilityVal);
            tuningStatsPanel.Dock = DockStyle.Top;
            tuningGroup.Controls.Add(tuningStatsPanel);

            _tuningChart = new Chart { Dock = DockStyle.Fill };
            InitializeTuningChart();
            tuningGroup.Controls.Add(_tuningChart);
            _tuningChart.BringToFront();

            splitContainer.Panel1.Controls.Add(tuningGroup);

            // === Manual panel (bottom) ===
            var manualGroup = new GroupBox { Text = "Manual Measurements", Dock = DockStyle.Fill };
            Label dummyStability;
            var manualStatsPanel = CreateStatsRow(out _mCountVal, out _mAvgVal, out _mStdDevVal, out _mMinVal, out _mMaxVal, out dummyStability);
            manualStatsPanel.Dock = DockStyle.Top;
            manualGroup.Controls.Add(manualStatsPanel);

            _manualChart = new Chart { Dock = DockStyle.Fill };
            InitializeManualChart();
            manualGroup.Controls.Add(_manualChart);
            _manualChart.BringToFront();

            splitContainer.Panel2.Controls.Add(manualGroup);

            this.Controls.Add(splitContainer);
            this.ResumeLayout(false);
        }

        private Panel CreateStatsRow(out Label countVal, out Label avgVal, out Label stdDevVal, out Label minVal, out Label maxVal, out Label stabilityVal)
        {
            var panel = new Panel { Height = 30 };
            int x = 10;
            AddLabel(panel, "N:", ref x); countVal = AddValue(panel, "0", ref x);
            AddLabel(panel, "Avg:", ref x); avgVal = AddValue(panel, "--", ref x);
            AddLabel(panel, "σ:", ref x); stdDevVal = AddValue(panel, "--", ref x);
            AddLabel(panel, "Min:", ref x); minVal = AddValue(panel, "--", ref x);
            AddLabel(panel, "Max:", ref x); maxVal = AddValue(panel, "--", ref x);
            AddLabel(panel, "Stab:", ref x); stabilityVal = AddValue(panel, "--", ref x);
            stabilityVal.Font = new Font(stabilityVal.Font, FontStyle.Bold);
            return panel;
        }

        private void AddLabel(Control p, string text, ref int x)
        {
            p.Controls.Add(new Label { Text = text, Location = new Point(x, 5), Size = new Size(30, 20), TextAlign = ContentAlignment.MiddleRight });
            x += 32;
        }

        private Label AddValue(Control p, string text, ref int x)
        {
            var lbl = new Label { Text = text, Location = new Point(x, 5), Size = new Size(80, 20), TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.DarkBlue };
            p.Controls.Add(lbl);
            x += 82;
            return lbl;
        }

        private void ConfigureChartArea(ChartArea area)
        {
            area.AxisX.Title = "Sample #";
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.Title = "Power (dBm)";
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;
            area.CursorY.IsUserEnabled = true;
            area.CursorY.IsUserSelectionEnabled = true;
            area.AxisX.ScaleView.Zoomable = true;
            area.AxisY.ScaleView.Zoomable = true;
            area.AxisX.ScrollBar.IsPositionedInside = true;
            area.AxisY.ScrollBar.IsPositionedInside = true;
        }

        private void InitializeTuningChart()
        {
            var area = new ChartArea("TuningArea");
            ConfigureChartArea(area);
            _tuningChart.ChartAreas.Add(area);

            _tuningSeries = new Series("Tuning") { ChartType = SeriesChartType.Line, Color = Color.Blue, BorderWidth = 2, MarkerStyle = MarkerStyle.Circle, MarkerSize = 4 };
            _tuningChart.Series.Add(_tuningSeries);

            _runningAvgSeries = new Series("Running Avg") { ChartType = SeriesChartType.Line, Color = Color.DarkOrange, BorderWidth = 2 };
            _tuningChart.Series.Add(_runningAvgSeries);

            _targetSeries = new Series("Target") { ChartType = SeriesChartType.Line, Color = Color.Green, BorderWidth = 2, BorderDashStyle = ChartDashStyle.Dash };
            _tuningChart.Series.Add(_targetSeries);

            _upperTolSeries = new Series("Upper Tol") { ChartType = SeriesChartType.Line, Color = Color.Red, BorderWidth = 1, BorderDashStyle = ChartDashStyle.Dash };
            _tuningChart.Series.Add(_upperTolSeries);

            _lowerTolSeries = new Series("Lower Tol") { ChartType = SeriesChartType.Line, Color = Color.Red, BorderWidth = 1, BorderDashStyle = ChartDashStyle.Dash };
            _tuningChart.Series.Add(_lowerTolSeries);

            _tuningChart.Legends.Add(new Legend { Docking = Docking.Top, Alignment = StringAlignment.Far });
            _tuningChart.MouseWheel += (s, e) => HandleZoom(_tuningChart, e);
        }

        private void InitializeManualChart()
        {
            var area = new ChartArea("ManualArea");
            ConfigureChartArea(area);
            _manualChart.ChartAreas.Add(area);

            _manualSeries = new Series("Manual") { ChartType = SeriesChartType.Point, Color = Color.Purple, MarkerStyle = MarkerStyle.Diamond, MarkerSize = 8 };
            _manualChart.Series.Add(_manualSeries);

            _manualAvgSeries = new Series("Running Avg") { ChartType = SeriesChartType.Line, Color = Color.DarkOrange, BorderWidth = 2 };
            _manualChart.Series.Add(_manualAvgSeries);

            _manualChart.Legends.Add(new Legend { Docking = Docking.Top, Alignment = StringAlignment.Far });
            _manualChart.MouseWheel += (s, e) => HandleZoom(_manualChart, e);
        }

        private void HandleZoom(Chart chart, MouseEventArgs e)
        {
            try
            {
                var area = chart.ChartAreas[0];
                if (e.Delta > 0)
                {
                    double xMin = area.AxisX.ScaleView.ViewMinimum;
                    double xMax = area.AxisX.ScaleView.ViewMaximum;
                    double c = (xMin + xMax) / 2, r = (xMax - xMin) / 2;
                    area.AxisX.ScaleView.Zoom(c - r * 0.5, c + r * 0.5);
                }
                else { area.AxisX.ScaleView.ZoomReset(); area.AxisY.ScaleView.ZoomReset(); }
            }
            catch { }
        }

        private void SubscribeToEvents()
        {
            _tuningController.ProgressUpdated += (s, e) => { if (e.Statistics != null) AddDataPoint(e.Statistics.CurrentIteration, e.Statistics.CurrentPowerDbm); };
            _tuningController.StateChanged += (s, e) =>
            {
                if (e.NewState == TuningState.Tuning && e.PreviousState == TuningState.Idle && _tuningController.Parameters != null)
                    SetTargetAndTolerance(_tuningController.Parameters.TargetPowerDbm, _tuningController.Parameters.MaxStdDevDb * _tuningController.Parameters.ConfidenceK);
            };
        }

        public void AddDataPoint(int iteration, double powerDbm)
        {
            if (this.InvokeRequired) { this.Invoke(new Action(() => AddDataPoint(iteration, powerDbm))); return; }
            try
            {
                _tuningPointIndex++;
                _tuningValues.Add(powerDbm);
                _tuningSeries.Points.AddXY(_tuningPointIndex, powerDbm);

                if (_tuningValues.Count >= _runningAvgWindow)
                {
                    double avg = _tuningValues.Skip(_tuningValues.Count - _runningAvgWindow).Take(_runningAvgWindow).Average();
                    _runningAvgSeries.Points.AddXY(_tuningPointIndex, avg);
                }

                ExtendHorizontalLines(_tuningPointIndex);
                UpdateTuningStats();
                _tuningChart.ChartAreas[0].RecalculateAxesScale();
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error adding tuning point: {ex.Message}"); }
        }

        public void AddManualDataPoint(int manualIteration, double powerDbm)
        {
            if (this.InvokeRequired) { this.Invoke(new Action(() => AddManualDataPoint(manualIteration, powerDbm))); return; }
            try
            {
                _manualPointIndex++;
                _manualValues.Add(powerDbm);
                _manualSeries.Points.AddXY(_manualPointIndex, powerDbm);

                if (_manualValues.Count >= _runningAvgWindow)
                {
                    double avg = _manualValues.Skip(_manualValues.Count - _runningAvgWindow).Take(_runningAvgWindow).Average();
                    _manualAvgSeries.Points.AddXY(_manualPointIndex, avg);
                }

                UpdateManualStats();
                _manualChart.ChartAreas[0].RecalculateAxesScale();
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error adding manual point: {ex.Message}"); }
        }

        private void ExtendHorizontalLines(int maxX)
        {
            ExtendLine(_targetSeries, maxX);
            ExtendLine(_upperTolSeries, maxX);
            ExtendLine(_lowerTolSeries, maxX);
        }

        private void ExtendLine(Series s, int maxX)
        {
            if (s.Points.Count > 0)
            {
                double val = s.Points[0].YValues[0];
                s.Points.Clear();
                s.Points.AddXY(0, val);
                s.Points.AddXY(maxX, val);
            }
        }

        private void UpdateTuningStats()
        {
            if (_tuningValues.Count == 0) return;
            double avg = _tuningValues.Average(), min = _tuningValues.Min(), max = _tuningValues.Max();
            double variance = _tuningValues.Sum(v => (v - avg) * (v - avg)) / _tuningValues.Count;
            double stdDev = Math.Sqrt(variance);

            _tCountVal.Text = _tuningValues.Count.ToString();
            _tAvgVal.Text = $"{avg:F3}";
            _tStdDevVal.Text = $"{stdDev:F3}";
            _tMinVal.Text = $"{min:F3}";
            _tMaxVal.Text = $"{max:F3}";

            double maxSD = _tuningController.Parameters?.MaxStdDevDb ?? 0.5;
            double k = _tuningController.Parameters?.ConfidenceK ?? 2.0;
            double target = _tuningController.Parameters?.TargetPowerDbm ?? 0;
            int win = _tuningController.Parameters?.StabilityWindow ?? _runningAvgWindow;

            if (_tuningValues.Count >= win)
            {
                var recent = _tuningValues.Skip(_tuningValues.Count - win).ToList();
                double rAvg = recent.Average();
                double rVar = recent.Sum(v => (v - rAvg) * (v - rAvg)) / recent.Count;
                double rSD = Math.Sqrt(rVar);
                double err = Math.Abs(rAvg - target);

                if (rSD <= maxSD && err <= k * rSD) { _tStabilityVal.Text = $"STABLE (σ={rSD:F2})"; _tStabilityVal.ForeColor = Color.Green; }
                else if (rSD <= maxSD) { _tStabilityVal.Text = $"SETTLING (σ={rSD:F2})"; _tStabilityVal.ForeColor = Color.Orange; }
                else { _tStabilityVal.Text = $"UNSTABLE (σ={rSD:F2})"; _tStabilityVal.ForeColor = Color.Red; }
            }
            else { _tStabilityVal.Text = $"Need {win} pts"; _tStabilityVal.ForeColor = Color.Gray; }
        }

        private void UpdateManualStats()
        {
            if (_manualValues.Count == 0) return;
            double avg = _manualValues.Average(), min = _manualValues.Min(), max = _manualValues.Max();
            double variance = _manualValues.Sum(v => (v - avg) * (v - avg)) / _manualValues.Count;
            double stdDev = Math.Sqrt(variance);

            _mCountVal.Text = _manualValues.Count.ToString();
            _mAvgVal.Text = $"{avg:F3}";
            _mStdDevVal.Text = $"{stdDev:F3}";
            _mMinVal.Text = $"{min:F3}";
            _mMaxVal.Text = $"{max:F3}";
        }

        public void SetTargetAndTolerance(double targetDbm, double toleranceDb)
        {
            if (this.InvokeRequired) { this.Invoke(new Action(() => SetTargetAndTolerance(targetDbm, toleranceDb))); return; }
            try
            {
                int maxX = Math.Max(_tuningPointIndex, 1);
                _targetSeries.Points.Clear(); _targetSeries.Points.AddXY(0, targetDbm); _targetSeries.Points.AddXY(maxX, targetDbm);
                _upperTolSeries.Points.Clear(); _upperTolSeries.Points.AddXY(0, targetDbm + toleranceDb); _upperTolSeries.Points.AddXY(maxX, targetDbm + toleranceDb);
                _lowerTolSeries.Points.Clear(); _lowerTolSeries.Points.AddXY(0, targetDbm - toleranceDb); _lowerTolSeries.Points.AddXY(maxX, targetDbm - toleranceDb);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error setting target: {ex.Message}"); }
        }

        public void ClearChart()
        {
            if (this.InvokeRequired) { this.Invoke(new Action(ClearChart)); return; }
            _tuningSeries.Points.Clear(); _runningAvgSeries.Points.Clear();
            _targetSeries.Points.Clear(); _upperTolSeries.Points.Clear(); _lowerTolSeries.Points.Clear();
            _manualSeries.Points.Clear(); _manualAvgSeries.Points.Clear();
            _tuningValues.Clear(); _manualValues.Clear();
            _tuningPointIndex = 0; _manualPointIndex = 0;
            ResetLabels(_tCountVal, _tAvgVal, _tStdDevVal, _tMinVal, _tMaxVal, _tStabilityVal);
            ResetLabels(_mCountVal, _mAvgVal, _mStdDevVal, _mMinVal, _mMaxVal, null);
        }

        private void ResetLabels(Label c, Label a, Label s, Label mn, Label mx, Label st)
        {
            c.Text = "0"; a.Text = "--"; s.Text = "--"; mn.Text = "--"; mx.Text = "--";
            if (st != null) { st.Text = "--"; st.ForeColor = Color.Gray; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tuningController.ProgressUpdated -= null;
                _tuningController.StateChanged -= null;
            }
            base.Dispose(disposing);
        }
    }
}
