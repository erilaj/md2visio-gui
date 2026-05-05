using md2visio.mermaid.cmn;
using md2visio.Api;
using md2visio.vsdx.@base;
using System.Reflection;

namespace md2visio.struc.figure
{
    internal class FigureBuilderFactory
    {
        string outputFile;
        string? dir = string.Empty, name = string.Empty;
        Dictionary<string, Type> builderDict = TypeMap.BuilderMap;
        SttIterator iter;
        int count = 1;
        bool isFileMode = false;
        int figuresBuilt = 0;
        List<string> unsupportedTypes = new();

        // Injected dependencies
        private readonly ConversionContext _context;
        private readonly IVisioSession _session;

        /// <summary>
        /// Number of diagrams built
        /// </summary>
        public int FiguresBuilt => figuresBuilt;

        /// <summary>
        /// Unsupported types encountered
        /// </summary>
        public IReadOnlyList<string> UnsupportedTypes => unsupportedTypes;

        public FigureBuilderFactory(SttIterator iter, ConversionContext context, IVisioSession session)
        {
            this.iter = iter;
            this._context = context;
            this._session = session;
            outputFile = iter.Context.InputFile;
        }

        public void Build(string outputFile)
        {
            this.outputFile = outputFile;
            // Reset diagnostics for fresh build
            figuresBuilt = 0;
            unsupportedTypes.Clear();

            InitOutputPath();
            BuildFigures();
        }

        /// <summary>
        /// Get supported types string from BuilderMap
        /// </summary>
        static string GetSupportedTypesString()
        {
            return string.Join(", ", TypeMap.BuilderMap.Keys.Distinct().OrderBy(k => k));
        }

        public void BuildFigures()
        {
            if (_context.Debug)
            {
                _context.Log($"[DEBUG] BuildFigures: Starting to build diagrams");
                _context.Log($"[DEBUG] BuildFigures: iter.HasNext() = {iter.HasNext()}");
                if (iter.Context?.StateList != null)
                {
                    _context.Log($"[DEBUG] BuildFigures: StateList.Count = {iter.Context.StateList.Count}");
                    _context.Log($"[DEBUG] BuildFigures: iter.Pos = {iter.Pos}");

                    for (int i = 0; i < iter.Context.StateList.Count; i++)
                    {
                        var state = iter.Context.StateList[i];
                        _context.Log($"[DEBUG] StateList[{i}]: Type={state.GetType().Name}, Fragment='{state.Fragment}'");
                    }
                }
            }

            // Check whether any mermaid blocks were found
            if (iter.Context?.StateList == null || iter.Context.StateList.Count == 0)
            {
                _context.Log("Warning: No mermaid code blocks found in the file");
                _context.Log("Hint: Please ensure diagrams are enclosed in ```mermaid ... ``` blocks");
                return;
            }

            while (iter.HasNext())
            {
                List<SynState> list = iter.Context.StateList;
                bool foundFigure = false;

                for (int pos = iter.Pos + 1; pos < list.Count; ++pos)
                {
                    string word = list[pos].Fragment;

                    if (_context.Debug)
                    {
                        _context.Log($"[DEBUG] BuildFigures: Checking position {pos}, Fragment = '{word}'");
                        _context.Log($"[DEBUG] BuildFigures: SttFigureType.IsFigure('{word}') = {SttFigureType.IsFigure(word)}");
                    }

                    if (SttFigureType.IsFigure(word))
                    {
                        foundFigure = true;

                        // Check if implemented
                        if (!builderDict.ContainsKey(word))
                        {
                            _context.Log($"Warning: Diagram type '{word}' is not yet supported, skipping");
                            if (!unsupportedTypes.Contains(word))
                                unsupportedTypes.Add(word);

                            // Skip unsupported diagram: advance iterator to the next SttMermaidStart or end
                            SkipUnsupportedFigure(pos);
                            break;
                        }

                        if (_context.Debug)
                        {
                            _context.Log($"[DEBUG] BuildFigures: Found diagram type '{word}', starting build");
                        }
                        BuildFigure(word);
                        figuresBuilt++;
                        break;  // BuildFigure advances the iterator
                    }
                }

            // If we traversed the entire list without finding a diagram type, exit the loop
                if (!foundFigure)
                    break;
            }

            // Summary
            if (figuresBuilt == 0)
            {
                _context.Log("Warning: No diagrams were built");
                if (unsupportedTypes.Count > 0)
                {
                    _context.Log($"Found {unsupportedTypes.Count} unsupported diagram type(s): {string.Join(", ", unsupportedTypes)}");
                    _context.Log($"Currently supported types: {GetSupportedTypesString()}");
                }
            }
            else
            {
                _context.Log($"Successfully built {figuresBuilt} diagram(s)");
            }
        }

        /// <summary>
        /// Skip unsupported diagram types; advance iterator to the next diagram boundary
        /// </summary>
        void SkipUnsupportedFigure(int startPos)
        {
            List<SynState> list = iter.Context.StateList;

            // Advance the iterator from the current position until we hit SttMermaidClose or run out
            while (iter.HasNext())
            {
                var state = iter.Next();
                if (state.GetType().Name == "SttMermaidClose")
                    break;
            }

            if (_context.Debug)
            {
                _context.Log($"[DEBUG] SkipUnsupportedFigure: Skip complete, iter.Pos = {iter.Pos}");
            }
        }

        public void Quit()
        {
            // Quit logic has moved to VisioSession.Dispose()
            // This method is retained empty for backward compatibility
        }

        void BuildFigure(string figureType)
        {
            if (_context.Debug)
            {
                _context.Log($"[DEBUG] BuildFigure: Starting build for diagram type '{figureType}'");
                _context.Log($"[DEBUG] BuildFigure: builderDict.ContainsKey('{figureType}') = {builderDict.ContainsKey(figureType)}");
            }

            if (!builderDict.ContainsKey(figureType))
                throw new NotImplementedException($"'{figureType}' builder not implemented");

            Type type = builderDict[figureType];

            if (_context.Debug)
            {
                _context.Log($"[DEBUG] BuildFigure: Builder type = {type.Name}");
            }

            // Create Builder using injected session and context
            object? obj = Activator.CreateInstance(type, iter, _context, _session);
            MethodInfo? method = type.GetMethod("Build", BindingFlags.Public | BindingFlags.Instance, null,
                new Type[] { typeof(string) }, null);

            if (_context.Debug)
            {
                _context.Log($"[DEBUG] BuildFigure: Builder instance created = {obj != null}");
                _context.Log($"[DEBUG] BuildFigure: Build method found = {method != null}");
            }

            string outputFilePath;
            if (isFileMode && count == 1)
            {
                // First figure in file mode: use exact filename
                outputFilePath = $"{dir}\\{name}.vsdx";
                count++;
            }
            else
            {
                // Multiple figures or directory mode: use numbered filenames
                outputFilePath = $"{dir}\\{name}{count++}.vsdx";
            }

            if (_context.Debug)
            {
                _context.Log($"[DEBUG] Building diagram: {figureType}");
                _context.Log($"[DEBUG] Output mode: {(isFileMode ? "file mode" : "directory mode")}");
                _context.Log($"[DEBUG] Output path: {outputFilePath}");
                _context.Log($"[DEBUG] Output directory: {dir}");
                _context.Log($"[DEBUG] File name: {name}");
            }

            if (_context.Debug)
            {
                _context.Log($"[DEBUG] BuildFigure: About to call {type.Name}.Build('{outputFilePath}')");
            }

            try
            {
                method?.Invoke(obj, new object[] { outputFilePath });

                if (_context.Debug)
                {
                    _context.Log($"[DEBUG] BuildFigure: {type.Name}.Build() call complete");
                }
            }
            catch (Exception ex)
            {
                if (_context.Debug)
                {
                    _context.Log($"[DEBUG] BuildFigure: {type.Name}.Build() call failed: {ex.Message}");
                    _context.Log($"[DEBUG] BuildFigure: Exception type: {ex.GetType().Name}");
                    if (ex.InnerException != null)
                    {
                        _context.Log($"[DEBUG] BuildFigure: Inner exception: {ex.InnerException.Message}");
                    }
                }
                throw;
            }

            if (_context.Debug)
            {
                if (File.Exists(outputFilePath))
                {
                    _context.Log($"[DEBUG] ✅ File generated successfully: {outputFilePath}");
                }
                else
                {
                    _context.Log($"[DEBUG] ❌ File generation failed: {outputFilePath}");
                }
            }
        }

        void InitOutputPath()
        {
            if (outputFile.ToLower().EndsWith(".vsdx"))
            {
                isFileMode = true;
                name = Path.GetFileNameWithoutExtension(outputFile);
                dir = Path.GetDirectoryName(outputFile);

                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            else if (Directory.Exists(outputFile))
            {
                isFileMode = false;
                name = Path.GetFileNameWithoutExtension(iter.Context.InputFile);
                dir = Path.GetFullPath(outputFile).TrimEnd(new char[] { '/', '\\' });
            }
            else
            {
                throw new ArgumentException($"Invalid output path: '{outputFile}'. Please specify a .vsdx file path or an existing directory.");
            }
        }
    }
}
