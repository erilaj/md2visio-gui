using md2visio.mermaid.cmn;

namespace md2visio.mermaid.er
{
    /// <summary>
    /// ER diagram word state class
    /// Handles identifiers such as entity names
    /// </summary>
    internal class ErSttWord : SynState
    {
        public override SynState NextState()
        {
            // Already saved in ErSttChar via Create<ErSttWord>().Save()
            // Forward directly here
            return Forward<ErSttChar>();
        }
    }
}
