using md2visio.mermaid.cmn;

namespace md2visio.mermaid.er
{
    /// <summary>
    /// ER diagram keyword parameter state class
    /// Handles parameters such as direction TB, direction LR
    /// </summary>
    internal class ErSttKeywordParam : SttKeywordParam
    {
        public override SynState NextState()
        {
            return Save(ExpectedGroups["param"].Value).Forward<ErSttChar>();
        }
    }
}
