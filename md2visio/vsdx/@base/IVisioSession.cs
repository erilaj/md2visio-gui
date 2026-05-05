using Visio = Microsoft.Office.Interop.Visio;

namespace md2visio.vsdx.@base
{
    /// <summary>
    /// Visio COM session interface
    /// Used to manage the lifecycle of a Visio Application
    /// </summary>
    public interface IVisioSession : IDisposable
    {
        /// <summary>
        /// Visio application instance
        /// </summary>
        Visio.Application Application { get; }

        /// <summary>
        /// Whether to show the Visio window
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Create a new blank document
        /// </summary>
        Visio.Document CreateDocument();

        /// <summary>
        /// Open a template document
        /// </summary>
        Visio.Document OpenStencil(string path);

        /// <summary>
        /// Save document to the specified path
        /// </summary>
        void SaveDocument(Visio.Document doc, string path, bool overwrite = true);

        /// <summary>
        /// Close document
        /// </summary>
        void CloseDocument(Visio.Document doc);
    }
}
