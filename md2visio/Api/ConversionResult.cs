namespace md2visio.Api
{
    /// <summary>
    /// Conversion result
    /// </summary>
    public sealed class ConversionResult
    {
        /// <summary>
        /// Whether the conversion succeeded
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Array of paths to generated output files
        /// </summary>
        public string[] OutputFiles { get; }

        /// <summary>
        /// Error message (on failure)
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// Exception details (for debugging)
        /// </summary>
        public Exception? Exception { get; }

        private ConversionResult(bool success, string[] outputFiles, string? errorMessage, Exception? exception)
        {
            Success = success;
            OutputFiles = outputFiles;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        /// <summary>
        /// Create a successful result
        /// </summary>
        public static ConversionResult Succeeded(params string[] outputFiles)
        {
            return new ConversionResult(true, outputFiles ?? Array.Empty<string>(), null, null);
        }

        /// <summary>
        /// Create a failed result
        /// </summary>
        public static ConversionResult Failed(string errorMessage, Exception? exception = null)
        {
            return new ConversionResult(false, Array.Empty<string>(), errorMessage, exception);
        }
    }
}
