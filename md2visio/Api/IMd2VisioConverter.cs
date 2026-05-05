namespace md2visio.Api
{
    /// <summary>
    /// Mermaid to Visio converter interface
    /// </summary>
    public interface IMd2VisioConverter : IDisposable
    {
        /// <summary>
        /// Execute the conversion
        /// </summary>
        /// <param name="request">Conversion request parameters</param>
        /// <param name="progress">Progress reporter (optional)</param>
        /// <param name="logger">Log sink (optional)</param>
        /// <returns>Conversion result</returns>
        ConversionResult Convert(
            ConversionRequest request,
            IProgress<ConversionProgress>? progress = null,
            ILogSink? logger = null);
    }
}
