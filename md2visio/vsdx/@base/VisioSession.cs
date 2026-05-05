using System.Runtime.InteropServices;
using Microsoft.Win32;
using Visio = Microsoft.Office.Interop.Visio;

namespace md2visio.vsdx.@base
{
    /// <summary>
    /// Visio COM session implementation
    /// Manages the complete lifecycle of a single Visio Application instance
    /// </summary>
    public sealed class VisioSession : IVisioSession
    {
        private Visio.Application? _app;
        private bool _disposed;
        private readonly object _lock = new();

        /// <summary>
        /// Whether to show the Visio window
        /// </summary>
        public bool Visible { get; }

        /// <summary>
        /// Visio application instance
        /// </summary>
        public Visio.Application Application
        {
            get
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                return _app ?? throw new InvalidOperationException("Visio Application not initialized");
            }
        }

        /// <summary>
        /// Create a Visio session
        /// </summary>
        /// <param name="visible">Whether to show the Visio window</param>
        public VisioSession(bool visible = false)
        {
            Visible = visible;
            EnsureVisioApp();
        }

        /// <summary>
        /// Ensure the Visio application is available
        /// </summary>
        private void EnsureVisioApp()
        {
            lock (_lock)
            {
                try
                {
                    if (_app != null)
                    {
                        // Test whether the COM object is still valid
                        _ = _app.Version;
                        return;
                    }
                }
                catch (COMException ex)
                {
                    Console.WriteLine($"COM exception, re-creating Visio application: {ex.Message}");
                    _app = null;
                }
                catch (InvalidComObjectException ex)
                {
                    Console.WriteLine($"COM object has been released, re-creating: {ex.Message}");
                    _app = null;
                }

                try
                {
                    Console.WriteLine("Creating Visio application...");
                    _app = new Visio.Application();
                    _app.Visible = Visible;
                    Console.WriteLine($"Visio application created successfully, version: {_app.Version}");
                }
                catch (COMException ex)
                {
                    throw new ApplicationException(
                        $"Unable to create Visio application. Please verify:\n" +
                        $"1. Microsoft Visio is correctly installed\n" +
                        $"2. The current user has permission to access Visio\n" +
                        $"3. Visio is not locked by another process\n" +
                        $"Error details: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(
                        $"An unknown error occurred while creating Visio application: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Create a new blank document
        /// </summary>
        public Visio.Document CreateDocument()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return Application.Documents.Add("");
        }

        /// <summary>
        /// Open a template document
        /// </summary>
        public Visio.Document OpenStencil(string path)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return Application.Documents.OpenEx(path, (short)Visio.VisOpenSaveArgs.visOpenDocked);
        }

        /// <summary>
        /// Save document to the specified path
        /// </summary>
        public void SaveDocument(Visio.Document doc, string path, bool overwrite = true)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (!overwrite && File.Exists(path))
            {
                doc.Saved = true;
                return;
            }

            doc.SaveAsEx(path, 0);
        }

        /// <summary>
        /// Close document
        /// </summary>
        public void CloseDocument(Visio.Document doc)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (!Visible)
            {
                doc.Close();
            }
            else
            {
                // In show mode keep the document open, only mark as saved
                doc.Saved = true;
            }
        }

        /// <summary>
        /// Release Visio COM resources
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            lock (_lock)
            {
                if (_disposed) return;

                try
                {
                    if (_app != null && !Visible)
                    {
                        // Non-show mode: quit Visio application
                        _app.Quit();
                    }
                }
                catch (COMException)
                {
                    // Visio may have been manually closed by the user; ignore exception
                }
                finally
                {
                    _app = null;
                    _disposed = true;
                }
            }
        }

        /// <summary>
        /// Get the Visio content directory path
        /// </summary>
        public static string? GetVisioContentDirectory()
        {
            int[] officeVersions = Enumerable.Range(11, 16).ToArray();

            foreach (int version in officeVersions)
            {
                string subKey = $@"Software\Microsoft\Office\{version}.0\Visio\InstallRoot";
#pragma warning disable CA1416, CS8604
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(subKey);
                object? value = key?.GetValue("Path");
                if (value != null)
                {
                    string contentDir = Path.Combine(value.ToString(), "Visio Content");
#pragma warning restore CA1416, CS8604
                    if (Directory.Exists(contentDir))
                    {
                        return contentDir;
                    }
                }
            }

            return null;
        }
    }
}
