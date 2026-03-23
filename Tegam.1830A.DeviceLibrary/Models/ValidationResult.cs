namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents the result of input validation.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationResult()
        {
        }

        public ValidationResult(bool isValid, string errorMessage = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }
}
