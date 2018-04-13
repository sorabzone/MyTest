using PaySlipEngine.BaseEngine;

namespace PaySlipFactory
{
    public abstract class PaySlipEngineFactory
    {
        public abstract BasePaySlipEngine GetPaySlipEngine();
    }
}
