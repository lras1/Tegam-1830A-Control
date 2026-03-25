using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CalibrationTuning.Controllers;
using CalibrationTuning.Events;
using CalibrationTuning.Models;

namespace CalibrationTuning.UserControls
{
    /// <summary>
    /// User control for displaying real-time power measurement chart during tuning.
    /// </summary>
    public partial class ChartPanel : UserControl
    {
        private readonly ITuningController _tuningController;
        private Chart _chart;
        private Series _measurementSeries;
        private Series _targetLineSeries;
        private Series _upperToleranceSeries;
        private Series _lowerToleranceSeries;

        public ChartPanel(ITuningController tuningController)
        {
            _tuningController = tuningController ?? throw new ArgumentNullException(nameof(tuningController));

            InitializeComponent();
            InitializeChart();
            SubscribeToEvents();
        }

        private void InitializeChart()
        {
            this.SuspendLayout();

            // Create chart control
            _chart = new Chart
            {
                Location = new Point(10, 10),
                Size = new Size(760, 540),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Create chart area
            var chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "Iteration";
            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.Title = "Power (dBm)";
            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            _chart.ChartAreas.Add(chartArea);

            // Create measurement series
            _measurementSeries = new Series("Measurements")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6,
                MarkerColor = Color.Blue
            };
            _chart.Series.Add(_measurementSeries);

            // Create target line series
            _targetLineSeries = new Series("Target")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash
            };
            _chart.Series.Add(_targetLineSeries);

            // Create upper tolerance series
            _upperToleranceSeries = new Series("Upper Tolerance")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            _chart.Series.Add(_upperToleranceSeries);

            // Create lower tolerance series
            _lowerToleranceSeries = new Series("Lower Tolerance")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            _chart.Series.Add(_lowerToleranceSeries);

            // Configure legend
            var legend = new Legend("MainLegend")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Far
            };
            _chart.Legends.Add(legend);

            this.Controls.Add(_chart);
            this.ResumeLayout(false);
        }

        private void SubscribeToEvents()
        {
            _tuningController.ProgressUpdated += TuningController_ProgressUpdated;
            _tuningController.StateChanged += TuningController_StateChanged;
        }

        private void TuningController_StateChanged(object sender, TuningStateChangedEventArgs e)
        {
            // Clear chart and set target/tolerance when starting a new tuning session
            if (e.NewState == TuningState.Tuning && e.PreviousState == TuningState.Idle)
            {
                ClearChart();
                
                if (_tuningController.Parameters != null)
                {
                    SetTargetAndTolerance(
                        _tuningController.Parameters.TargetPowerDbm,
                        _tuningController.Parameters.ToleranceDb
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

        /// <summary>
        /// Adds a data point to the measurement series.
        /// </summary>
        /// <param name="iteration">Iteration number.</param>
        /// <param name="powerDbm">Measured power in dBm.</param>
        public void AddDataPoint(int iteration, double powerDbm)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AddDataPoint(iteration, powerDbm)));
                return;
            }

            try
            {
                _measurementSeries.Points.AddXY(iteration, powerDbm);

                // Auto-scale Y-axis to fit all data points
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

        /// <summary>
        /// Sets the target power and tolerance lines on the chart.
        /// </summary>
        /// <param name="targetDbm">Target power in dBm.</param>
        /// <param name="toleranceDb">Tolerance in dB.</param>
        public void SetTargetAndTolerance(double targetDbm, double toleranceDb)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SetTargetAndTolerance(targetDbm, toleranceDb)));
                return;
            }

            try
            {
                // Clear existing target and tolerance lines
                _targetLineSeries.Points.Clear();
                _upperToleranceSeries.Points.Clear();
                _lowerToleranceSeries.Points.Clear();

                // Add initial points (will be extended as measurements are added)
                _targetLineSeries.Points.AddXY(0, targetDbm);
                _upperToleranceSeries.Points.AddXY(0, targetDbm + toleranceDb);
                _lowerToleranceSeries.Points.AddXY(0, targetDbm - toleranceDb);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting target and tolerance: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all data points from the chart.
        /// </summary>
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
                _targetLineSeries.Points.Clear();
                _upperToleranceSeries.Points.Clear();
                _lowerToleranceSeries.Points.Clear();
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
                // Unsubscribe from events
                _tuningController.ProgressUpdated -= TuningController_ProgressUpdated;
                _tuningController.StateChanged -= TuningController_StateChanged;
            }
            base.Dispose(disposing);
        }
    }
}
