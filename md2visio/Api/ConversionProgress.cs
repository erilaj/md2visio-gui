namespace md2visio.Api
{
    /// <summary>
    /// Conversion progress information
    /// </summary>
    public sealed class ConversionProgress
    {
        /// <summary>
        /// Progress percentage (0-100)
        /// </summary>
        public int Percentage { get; }

        /// <summary>
        /// Status message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Current phase
        /// </summary>
        public ConversionPhase Phase { get; }

        public ConversionProgress(int percentage, string message, ConversionPhase phase)
        {
            Percentage = Math.Clamp(percentage, 0, 100);
            Message = message ?? string.Empty;
            Phase = phase;
        }
    }

    /// <summary>
    /// Conversion phase enumeration
    /// </summary>
    public enum ConversionPhase
    {
        /// <summary>Starting up</summary>
        Starting,
        /// <summary>Parsing Mermaid syntax</summary>
        Parsing,
        /// <summary>Building diagram data structures</summary>
        Building,
        /// <summary>Rendering to Visio</summary>
        Rendering,
        /// <summary>Saving files</summary>
        Saving,
        /// <summary>Completed</summary>
        Completed
    }
}
