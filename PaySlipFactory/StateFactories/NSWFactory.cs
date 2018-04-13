using PaySlipEngine.BaseEngine;
using PaySlipEngine.StateEngines;

namespace PaySlipFactory.StateFactories
{
    public class NSWFactory : PaySlipEngineFactory
    {
        public override BasePaySlipEngine GetPaySlipEngine()
        {
            return new NSWPaySlipEngine();
        }
    }
}
