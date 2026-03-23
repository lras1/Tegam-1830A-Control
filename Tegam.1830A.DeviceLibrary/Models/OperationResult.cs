namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents the result of an operation performed on the Tegam 1830A device.
    /// </summary>
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public OperationResult()
        {
        }

        public OperationResult(bool isSuccess, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Creates a successful operation result.
        /// </summary>
        public static OperationResult Success()
        {
            return new OperationResult(true);
        }

        /// <summary>
        /// Creates a failed operation result with an error message.
        /// </summary>
        public static OperationResult Failure(string errorMessage)
        {
            return new OperationResult(false, errorMessage);
        }
    }
}
