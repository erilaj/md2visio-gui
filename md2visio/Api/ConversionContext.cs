namespace md2visio.Api
{
    /// <summary>
    /// Conversion context (per-conversion runtime context)
    /// Replaces AppConfig.Instance global state
    /// </summary>
    public sealed class ConversionContext
    {
        /// <summary>
        /// Conversion request parameters
        /// </summary>
        public ConversionRequest Options { get; }

        /// <summary>
        /// Log sink
        /// </summary>
        public ILogSink Logger { get; }

        #region Shortcut properties (reduces callsite modifications)

        /// <summary>
        /// Whether debug mode is enabled
        /// </summary>
        public bool Debug => Options.Debug;

        /// <summary>
        /// Whether to show the Visio window
        /// </summary>
        public bool Visible => Options.ShowVisio;

        /// <summary>
        /// Input file path
        /// </summary>
        public string InputFile => Options.InputPath;

        /// <summary>
        /// Output path
        /// </summary>
        public string OutputPath => Options.OutputPath;

        /// <summary>
        /// Whether to silently overwrite
        /// </summary>
        public bool Quiet => Options.SilentOverwrite;

        #endregion

        /// <summary>
        /// Records the most recent error message (for cross-layer feedback)
        /// </summary>
        public string? LastError { get; private set; }

        public ConversionContext(ConversionRequest options, ILogSink? logger = null)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Logger = logger ?? NullLogSink.Instance;
        }

        /// <summary>
        /// Output debug log (only in debug mode)
        /// </summary>
        public void Log(string message)
        {
            if (Debug)
            {
                Logger.Debug(message);
            }
        }

        /// <summary>
        /// Output info log
        /// </summary>
        public void LogInfo(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// Output warning log
        /// </summary>
        public void LogWarning(string message)
        {
            Logger.Warning(message);
        }

        /// <summary>
        /// Output error log
        /// </summary>
        public void LogError(string message)
        {
            Logger.Error(message);
        }

        /// <summary>
        /// Record an error and save it to the context so that upper layers can terminate the conversion
        /// </summary>
        public void SetError(string message)
        {
            LastError = message;
            Logger.Error(message);
        }
    }
}
