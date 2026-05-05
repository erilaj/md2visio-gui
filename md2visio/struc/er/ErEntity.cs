using Microsoft.Office.Interop.Visio;

namespace md2visio.struc.er
{
    /// <summary>
    /// ER diagram entity class
    /// Represents a database table or entity
    /// </summary>
    internal class ErEntity
    {
        /// <summary>
        /// Entity ID (used for internal references)
        /// </summary>
        public string ID { get; set; } = "";

        /// <summary>
        /// Display name (alias, if any)
        /// </summary>
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// Entity attribute list
        /// </summary>
        public List<ErAttribute> Attributes { get; } = new();

        /// <summary>
        /// Corresponding Visio shape
        /// </summary>
        public Shape? VisioShape { get; set; }

        /// <summary>
        /// Get the name for display
        /// </summary>
        public string GetDisplayName()
        {
            return string.IsNullOrEmpty(DisplayName) ? ID : DisplayName;
        }

        /// <summary>
        /// Add an attribute
        /// </summary>
        public void AddAttribute(ErAttribute attribute)
        {
            Attributes.Add(attribute);
        }

        /// <summary>
        /// Add an attribute
        /// </summary>
        public void AddAttribute(string type, string name, string keys = "", string comment = "")
        {
            Attributes.Add(new ErAttribute
            {
                Type = type,
                Name = name,
                Keys = keys,
                Comment = comment
            });
        }
    }
}
