using md2visio.mermaid.cmn;
using System.Text.RegularExpressions;

namespace md2visio.mermaid.er
{
    /// <summary>
    /// ER diagram character state class
    /// Core state dispatcher; decides next state based on the current character
    /// </summary>
    internal class ErSttChar : SynState
    {
        // Starting character pattern for relationship symbols
        static readonly Regex regRelationStart = new(
            @"^(\|\||[|o}\|])",
            RegexOptions.Compiled);

        public override SynState NextState()
        {
            string? next = Ctx.Peek();
            if (next == null) return EndOfFile;

            // Comment handling
            if (next == "%") return Forward<SttPercent>();

            // End of line
            if (next == "\n")
            {
                if (Buffer.Length > 0)
                {
                    Create<ErSttWord>().Save(Buffer);
                    ClearBuffer();
                }
                return Forward<SttFinishFlag>();
            }

            // Mermaid code block end
            if (next == "`") return Forward<SttMermaidClose>();

            // Whitespace — possible end of word
            if (next == " " || next == "\t")
            {
                if (Buffer.Length > 0)
                {
                    // Check whether this is a keyword
                    if (ErSttKeyword.IsKeyword(Buffer))
                    {
                        return Forward<ErSttKeyword>();
                    }
                    Create<ErSttWord>().Save(Buffer);
                    ClearBuffer();
                }
                return Take().Forward<ErSttChar>();
            }

            // Entity attribute block start
            if (next == "{")
            {
                if (Buffer.Length > 0)
                {
                    Create<ErSttWord>().Save(Buffer);
                    ClearBuffer();
                }
                return Forward<ErSttEntityBody>();
            }

            // Relationship label
            if (next == ":")
            {
                if (Buffer.Length > 0)
                {
                    Create<ErSttWord>().Save(Buffer);
                    ClearBuffer();
                }
                return Forward<ErSttLabel>();
            }

            // Check whether this is the start of a relationship symbol
            if (IsRelationStart())
            {
                if (Buffer.Length > 0)
                {
                    Create<ErSttWord>().Save(Buffer);
                    ClearBuffer();
                }
                return Forward<ErSttRelation>();
            }

            // Regular character — accumulate to Buffer
            return Take().Forward<ErSttChar>();
        }

        bool IsRelationStart()
        {
            string incoming = Ctx.Incoming.ToString();
            // Relationship symbol must start with a complete cardinality pattern:
            // ||, |o, |{, }o, }|, }{ (followed by -- or ..)
            if (incoming.StartsWith("||") || incoming.StartsWith("|o") ||
                incoming.StartsWith("|{") || incoming.StartsWith("}|") || 
                incoming.StartsWith("}o") || incoming.StartsWith("}{"))
            {
                return true;
            }
            // o| and o{ cases
            if (incoming.StartsWith("o|") || incoming.StartsWith("o{"))
            {
                return true;
            }
            return false;
        }
    }
}
