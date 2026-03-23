namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents the result of a parsing operation.
    /// </summary>
    /// <typeparam name="T">The type of value returned on success.</typeparam>
    public class ParseResult<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the parsing was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the parsed value (only valid when IsSuccess is true).
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the error message (only valid when IsSuccess is false).
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the line number where the error occurred (optional).
        /// </summary>
        public int? ErrorLine { get; set; }

        /// <summary>
        /// Creates a successful parse result.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <returns>A successful ParseResult.</returns>
        public static ParseResult<T> Success(T value)
        {
            return new ParseResult<T>
            {
                IsSuccess = true,
                Value = value
            };
        }

        /// <summary>
        /// Creates a failed parse result.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <param name="line">The line number where the error occurred (optional).</param>
        /// <returns>A failed ParseResult.</returns>
        public static ParseResult<T> Failure(string error, int? line = null)
        {
            return new ParseResult<T>
            {
                IsSuccess = false,
                ErrorMessage = error,
                ErrorLine = line
            };
        }
    }
}
