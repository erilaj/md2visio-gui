using md2visio.mermaid.cmn;

namespace md2visio.mermaid.sequence
{
    internal class SeqSttChar : SttCtxChar
    {
        public override SynState NextState()
        {
            string? next = Ctx.Peek();
            if (next == null) return EndOfFile;

            char ch = next[0];

            if (char.IsWhiteSpace(ch) && ch != '\n' && ch != '\r')
            {
                // If Buffer contains a message-arrow format (e.g., a->>b:), keep reading to end of line
                // Don't split at spaces; preserve the integrity of the message line
                if (IsMessageArrowLine())
                {
                    return Take().Forward<SeqSttChar>();
                }

                if (!string.IsNullOrEmpty(Buffer)) return Forward<SeqSttWord>();
                else return SlideSpaces().Forward<SeqSttChar>();
            }
            else if (ch == '\n' || ch == '\r')
            {
                if (!string.IsNullOrEmpty(Buffer)) return Forward<SeqSttWord>();
                else return Forward<SttFinishFlag>();
            }
            else if (ch == '%')
            {
                if (!string.IsNullOrEmpty(Buffer)) return Forward<SeqSttWord>();
                else return Forward<SttComment>();
            }
            else if (ch == '`')
            {
                if (!string.IsNullOrEmpty(Buffer)) return Forward<SeqSttWord>();
                else return Forward<SttMermaidClose>();
            }
            else
            {
                return Take().Forward<SeqSttChar>();
            }
        }

        /// <summary>
        /// Check whether Buffer contains a message-arrow format.
        /// Message format: from->>to: message  or  from-->>to: message  etc.
        /// </summary>
        private bool IsMessageArrowLine()
        {
            string buf = Buffer.ToString();
            // Check whether this contains a message arrow followed by a colon (indicating a message line)
            return (buf.Contains("->>") || buf.Contains("-->>") ||
                    buf.Contains("->") || buf.Contains("-->")) &&
                   buf.Contains(":");
        }
    }
}