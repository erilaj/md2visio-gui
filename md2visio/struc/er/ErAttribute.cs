namespace md2visio.struc.er
{
    /// <summary>
    /// ER diagram attribute class
    /// Represents an attribute of an entity
    /// </summary>
    internal class ErAttribute
    {
        /// <summary>
        /// Attribute type (e.g. string, int, date)
        /// </summary>
        public string Type { get; set; } = "";

        /// <summary>
        /// Attribute name
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Key type (PK, FK, UK, or combination such as "PK, FK")
        /// </summary>
        public string Keys { get; set; } = "";

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; } = "";

        /// <summary>
        /// Whether this is a primary key
        /// </summary>
        public bool IsPrimaryKey => Keys.Contains("PK");

        /// <summary>
        /// Whether this is a foreign key
        /// </summary>
        public bool IsForeignKey => Keys.Contains("FK");

        /// <summary>
        /// Whether this is a unique key
        /// </summary>
        public bool IsUniqueKey => Keys.Contains("UK");

        /// <summary>
        /// Generate display string
        /// </summary>
        public string ToDisplayString()
        {
            var parts = new List<string> { Type, Name };

            if (!string.IsNullOrEmpty(Keys))
            {
                parts.Add(Keys);
            }

            if (!string.IsNullOrEmpty(Comment))
            {
                parts.Add($"\"{Comment}\"");
            }

            return string.Join(" ", parts);
        }
    }
}
