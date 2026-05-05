namespace md2visio.Api
{
    /// <summary>
    /// Conversion request parameters (immutable value object)
    /// Supports fluent builder pattern construction
    /// </summary>
    public sealed class ConversionRequest
    {
        /// <summary>
        /// Input Markdown file path
        /// </summary>
        public string InputPath { get; }

        /// <summary>
        /// Output path (can be a .vsdx file path or a directory)
        /// </summary>
        public string OutputPath { get; }

        /// <summary>
        /// Whether to show the Visio window (default: hidden)
        /// </summary>
        public bool ShowVisio { get; }

        /// <summary>
        /// Whether to silently overwrite existing files (default: yes)
        /// </summary>
        public bool SilentOverwrite { get; }

        /// <summary>
        /// Whether to enable debug logging (default: no)
        /// </summary>
        public bool Debug { get; }

        public ConversionRequest(
            string inputPath,
            string outputPath,
            bool showVisio = false,
            bool silentOverwrite = true,
            bool debug = false)
        {
            InputPath = inputPath ?? throw new ArgumentNullException(nameof(inputPath));
            OutputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));
            ShowVisio = showVisio;
            SilentOverwrite = silentOverwrite;
            Debug = debug;
        }

        #region Static Factory Methods

        /// <summary>
        /// Create a conversion request
        /// </summary>
        public static ConversionRequest Create(string inputPath, string outputPath)
        {
            return new ConversionRequest(inputPath, outputPath);
        }

        #endregion

        #region Fluent Builder Methods

        /// <summary>
        /// Set whether to show the Visio window
        /// </summary>
        public ConversionRequest WithShowVisio(bool showVisio = true)
        {
            return new ConversionRequest(InputPath, OutputPath, showVisio, SilentOverwrite, Debug);
        }

        /// <summary>
        /// Set silent overwrite
        /// </summary>
        public ConversionRequest WithSilentOverwrite(bool silentOverwrite = true)
        {
            return new ConversionRequest(InputPath, OutputPath, ShowVisio, silentOverwrite, Debug);
        }

        /// <summary>
        /// Set debug mode
        /// </summary>
        public ConversionRequest WithDebug(bool debug = true)
        {
            return new ConversionRequest(InputPath, OutputPath, ShowVisio, SilentOverwrite, debug);
        }

        #endregion
    }
}
