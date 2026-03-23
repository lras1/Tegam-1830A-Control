namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents the calibration status of the Tegam 1830A device.
    /// </summary>
    public class CalibrationStatus
    {
        public bool IsCalibrating { get; set; }
        public bool IsComplete { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }

        public CalibrationStatus()
        {
        }

        public CalibrationStatus(bool isCalibrating, bool isComplete, bool isSuccessful, string errorMessage = null)
        {
            IsCalibrating = isCalibrating;
            IsComplete = isComplete;
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage;
        }
    }
}
