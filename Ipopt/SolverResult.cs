using System;

namespace Ipopt
{
    public class SolverResult
    {
        public Double[] Values { get; }
        public Int32 ReturnCode { get; }

        public SolverResult(Double[] values, Int32 returnCode)
        {
            this.Values = values;
            this.ReturnCode = returnCode;
        }
    }
}