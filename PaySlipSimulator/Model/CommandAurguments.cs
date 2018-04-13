using PaySlipEngine.Common;

namespace PaySlipSimulator.Model
{
    public class CommandArguments
    {
        public int X { get; set; }

        public int Y{ get; set; }

        public Direction Face { get; set; }

        public Command Instruction { get; set; }
    }
}
