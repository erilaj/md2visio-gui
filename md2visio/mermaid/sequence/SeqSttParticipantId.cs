using md2visio.mermaid.cmn;

namespace md2visio.mermaid.sequence
{
    internal class SeqSttParticipantId : SynState
    {
        public override SynState NextState()
        {
            // Simple participant ID parse (used for activate/deactivate)
            string participantId = Buffer.ToString().Trim();
            
            if (string.IsNullOrEmpty(participantId))
            {
                throw new SynException("Expected participant ID", Ctx);
            }

            Save(participantId).ClearBuffer();
            return Forward<SeqSttChar>();
        }
    }
}