using PaySlipEngine.BaseEngine;
using PaySlipEngine.StateEngines;

namespace PaySlipFactory.StateFactories
{
    public class VictoriaFactory: PaySlipEngineFactory
    {
        public override BasePaySlipEngine GetPaySlipEngine()
        {
            return new VictoriaPaySlipEngine();
        }
    }
}
