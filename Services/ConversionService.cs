using md2visio.main;
using md2visio.struc.figure;
using System.Diagnostics;

namespace md2visio.GUI.Services
{
    /// <summary>
    /// Mermaid to Visio conversion service
    /// </summary>
    public class ConversionService
    {
        public event EventHandler<ConversionProgressEventArgs>? ProgressChanged;
        public event EventHandler<ConversionLogEventArgs>? LogMessage;

        /// <summary>
        /// Convert MD file to Visio
        /// </summary>
        /// <param name="inputFile">Path to the input MD file</param>
        /// <param name="outputDir">Output directory</param>
        /// <param name="showVisio">Whether to show the Visio window</param>
        /// <param name="silentOverwrite">Whether to silently overwrite existing files</param>
        /// <returns>Conversion result</returns>
        public async Task<ConversionResult> ConvertAsync(string inputFile, string outputDir, bool showVisio = false, bool silentOverwrite = false)
        {
            return await Task.Run(() => Convert(inputFile, outputDir, showVisio, silentOverwrite));
        }

        /// <summary>
        /// Synchronous conversion method
        /// </summary>
        private ConversionResult Convert(string inputFile, string outputDir, bool showVisio, bool silentOverwrite)
        {
            try
            {
                ReportProgress(0, "Starting conversion...");
                ReportLog($"Input file: {inputFile}");
                ReportLog($"Output directory: {outputDir}");

                // Validate input file
                if (!File.Exists(inputFile))
                    return ConversionResult.Error($"Input file does not exist: {inputFile}");

                if (!Path.GetExtension(inputFile).Equals(".md", StringComparison.OrdinalIgnoreCase))
                    return ConversionResult.Error("Input file must be in .md format");

                // Create output directory
                Directory.CreateDirectory(outputDir);
                ReportProgress(20, "Preparing conversion environment...");

                // Build arguments
                var args = new List<string>
                {
                    "/I", $"\"{inputFile}\"",
                    "/O", $"\"{outputDir}\""
                };

                if (showVisio) args.Add("/V");
                if (silentOverwrite) args.Add("/Y");

                ReportProgress(40, "Executing conversion...");
                ReportLog($"Conversion arguments: {string.Join(" ", args)}");

                // Call AppConfig to perform conversion
                var config = new AppConfig();
                if (!config.LoadArguments(args.ToArray()))
                {
                    return ConversionResult.Error("Failed to parse arguments");
                }

                ReportProgress(60, "Parsing Mermaid content...");

                // Execute conversion
                config.Main();

                ReportProgress(80, "Generating Visio file...");

                // Find generated files
                var outputFiles = Directory.GetFiles(outputDir, "*.vsdx");
                
                ReportProgress(100, "Conversion complete!");

                if (outputFiles.Length > 0)
                {
                    ReportLog($"Successfully generated {outputFiles.Length} file(s):");
                    foreach (var file in outputFiles)
                    {
                        ReportLog($"  - {Path.GetFileName(file)}");
                    }
                    return ConversionResult.Success(outputFiles);
                }
                else
                {
                    return ConversionResult.Error("Conversion completed but no output files were found");
                }
            }
            catch (NotImplementedException ex)
            {
                ReportLog($"Feature not implemented: {ex.Message}");
                return ConversionResult.Error($"This diagram type is not yet supported: {ex.Message}");
            }
            catch (Exception ex)
            {
                ReportLog($"Conversion error: {ex.Message}");
                return ConversionResult.Error($"Conversion failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Detect Mermaid diagram types in a MD file
        /// </summary>
        /// <param name="filePath">Path to the MD file</param>
        /// <returns>List of detected diagram types</returns>
        public List<string> DetectMermaidTypes(string filePath)
        {
            var types = new List<string>();
            
            try
            {
                var content = File.ReadAllText(filePath);
                var lines = content.Split('\n');
                
                bool inMermaidBlock = false;
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    
                    if (trimmed.StartsWith("```mermaid"))
                    {
                        inMermaidBlock = true;
                        continue;
                    }
                    
                    if (trimmed.StartsWith("```") && inMermaidBlock)
                    {
                        inMermaidBlock = false;
                        continue;
                    }
                    
                    if (inMermaidBlock && !string.IsNullOrWhiteSpace(trimmed))
                    {
                        // Detect diagram type
                        var words = trimmed.Split(' ');
                        if (words.Length > 0)
                        {
                            var type = words[0].ToLower();
                            if (!types.Contains(type))
                            {
                                types.Add(type);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ReportLog($"Error detecting file types: {ex.Message}");
            }
            
            return types;
        }

        private void ReportProgress(int percentage, string message)
        {
            ProgressChanged?.Invoke(this, new ConversionProgressEventArgs(percentage, message));
        }

        private void ReportLog(string message)
        {
            LogMessage?.Invoke(this, new ConversionLogEventArgs(DateTime.Now, message));
        }
    }

    /// <summary>
    /// Conversion result
    /// </summary>
    public class ConversionResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string[]? OutputFiles { get; set; }

        public static ConversionResult Success(string[] outputFiles)
        {
            return new ConversionResult { IsSuccess = true, OutputFiles = outputFiles };
        }

        public static ConversionResult Error(string message)
        {
            return new ConversionResult { IsSuccess = false, ErrorMessage = message };
        }
    }

    /// <summary>
    /// Conversion progress event arguments
    /// </summary>
    public class ConversionProgressEventArgs : EventArgs
    {
        public int Percentage { get; }
        public string Message { get; }

        public ConversionProgressEventArgs(int percentage, string message)
        {
            Percentage = percentage;
            Message = message;
        }
    }

    /// <summary>
    /// Conversion log event arguments
    /// </summary>
    public class ConversionLogEventArgs : EventArgs
    {
        public DateTime Timestamp { get; }
        public string Message { get; }

        public ConversionLogEventArgs(DateTime timestamp, string message)
        {
            Timestamp = timestamp;
            Message = message;
        }
    }
} 