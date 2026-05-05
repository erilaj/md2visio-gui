namespace md2visio.struc.er
{
    /// <summary>
    /// ER diagram cardinality enumeration
    /// Represents the cardinality of the relationship between entities
    /// </summary>
    internal enum ErCardinality
    {
        /// <summary>
        /// Exactly one (||)
        /// </summary>
        ExactlyOne,

        /// <summary>
        /// Zero or one (|o, o|)
        /// </summary>
        ZeroOrOne,

        /// <summary>
        /// One or more (}|, |{)
        /// </summary>
        OneOrMore,

        /// <summary>
        /// Zero or more (}o, o{)
        /// </summary>
        ZeroOrMore
    }
}
