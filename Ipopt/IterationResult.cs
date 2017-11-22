using System;

namespace Ipopt
{
    public class IterationResult
    {
        public Int32 IterationNumber { get; set; }
        public Double ObjectiveFunction { get; set; }
        public Double ConstraintViolation { get; set; }
        public Double DualInfeasibility { get; set; }
        public Double Mu { get; set; }
        public Double PrimalStepNorm { get; set; }
        public Double RegularizationTerm { get; set; }
        public Double DualVariablesStepSize { get; set; }
        public Double PrimalVariablesStepSize { get; set; }
        public Int32 BacktrackingLines { get; set; }

        public override String ToString()
        {
            return String.Format("IterationNumber: {0}, ObjectiveFunction: {1}", this.IterationNumber,
                this.ObjectiveFunction);
        }
    }
}