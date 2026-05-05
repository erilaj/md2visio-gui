using Microsoft.Office.Interop.Visio;

namespace md2visio.struc.er
{
    /// <summary>
    /// ER diagram relationship class
    /// Represents the relationship between two entities
    /// </summary>
    internal class ErRelation
    {
        /// <summary>
        /// Source entity ID
        /// </summary>
        public string FromEntity { get; set; } = "";

        /// <summary>
        /// Target entity ID
        /// </summary>
        public string ToEntity { get; set; } = "";

        /// <summary>
        /// Source cardinality
        /// </summary>
        public ErCardinality LeftCardinality { get; set; } = ErCardinality.ExactlyOne;

        /// <summary>
        /// Target cardinality
        /// </summary>
        public ErCardinality RightCardinality { get; set; } = ErCardinality.ExactlyOne;

        /// <summary>
        /// Whether this is an identifying relationship (solid line); if not, it is non-identifying (dashed line)
        /// </summary>
        public bool IsIdentifying { get; set; } = true;

        /// <summary>
        /// Relationship label
        /// </summary>
        public string Label { get; set; } = "";

        /// <summary>
        /// Corresponding Visio shape
        /// </summary>
        public Shape? VisioShape { get; set; }

        /// <summary>
        /// Parse cardinality symbol
        /// </summary>
        public static ErCardinality ParseCardinality(string symbol)
        {
            // Normalize symbol
            symbol = symbol.Trim();

            return symbol switch
            {
                "||" => ErCardinality.ExactlyOne,
                "|o" or "o|" => ErCardinality.ZeroOrOne,
                "}|" or "|{" => ErCardinality.OneOrMore,
                "}o" or "o{" => ErCardinality.ZeroOrMore,
                _ => ErCardinality.ExactlyOne
            };
        }

        /// <summary>
        /// Parse the complete relationship symbol
        /// </summary>
        public static (ErCardinality left, ErCardinality right, bool isIdentifying) ParseRelationSymbol(string symbol)
        {
            // Find the middle line style (-- or ..)
            int dashPos = symbol.IndexOf("--");
            int dotPos = symbol.IndexOf("..");

            bool isIdentifying = dashPos >= 0;
            int splitPos = isIdentifying ? dashPos : dotPos;

            if (splitPos < 0)
            {
                return (ErCardinality.ExactlyOne, ErCardinality.ExactlyOne, true);
            }

            string leftPart = symbol.Substring(0, splitPos);
            string rightPart = symbol.Substring(splitPos + 2);

            return (
                ParseCardinality(leftPart),
                ParseCardinality(rightPart),
                isIdentifying
            );
        }
    }
}
