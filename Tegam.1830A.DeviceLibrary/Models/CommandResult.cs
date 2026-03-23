namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents the result of a command sent to the Tegam 1830A device.
    /// </summary>
    public class CommandResult
    {
        public bool IsSuccess { get; set; }
        public string Response { get; set; }
        public string ErrorMessage { get; set; }

        public CommandResult()
        {
        }

        public CommandResult(bool isSuccess, string response = null, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            Response = response;
            ErrorMessage = errorMessage;
        }
    }
}
