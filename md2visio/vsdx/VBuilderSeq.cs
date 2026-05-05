using md2visio.Api;
using md2visio.struc.sequence;
using md2visio.vsdx.@base;

namespace md2visio.vsdx
{
    internal class VBuilderSeq : VFigureBuilder<Sequence>
    {
        public VBuilderSeq(Sequence figure, ConversionContext context, IVisioSession session)
            : base(figure, context, session) { }

        protected override void ExecuteBuild()
        {
            if (_context.Debug)
            {
                _context.Log($"[DEBUG] VBuilderSeq: Starting build, VisioApp state: {(_session.Application != null ? "created" : "not created")}");
            }

            try
            {
                using var drawer = new VDrawerSeq(figure, _session.Application, _context);
                drawer.Draw();

                if (_context.Debug)
                {
                    _context.Log($"[DEBUG] VBuilderSeq: VDrawerSeq.Draw() complete");
                }
            }
            catch (Exception ex)
            {
                if (_context.Debug)
                {
                    _context.Log($"[DEBUG] VBuilderSeq: VDrawerSeq.Draw() failed: {ex.Message}");
                    _context.Log($"[DEBUG] VBuilderSeq: Exception type: {ex.GetType().Name}");
                    if (ex.InnerException != null)
                    {
                        _context.Log($"[DEBUG] VBuilderSeq: Inner exception: {ex.InnerException.Message}");
                    }
                }
                throw;
            }
        }
    }
}
