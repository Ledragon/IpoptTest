using System;
using System.Collections.Generic;

namespace Ipopt
{
    public class ReconciliationResults
    {
        private Int32 _returnCode;
        public List<IterationResult> IterationResults { get; }
        public List<ReconciliationResult> Results { get; }
        public List<ConstraintResult> ConstraintResults { get; }
        public IpoptParams SolverParams { get; }
        public TimeSpan ElapsedTime { get; set; }
        public DateTime ProcessedDate { get; set; }

        public Int32 ReturnCode
        {
            get => this._returnCode;
            set
            {
                this._returnCode = value;
                this.Status = ((ReturnCode)value).ToString();
            }
        }

        public String Status { get; private set; }

        public ReconciliationResults(IpoptParams solverParams)
        {
            this.SolverParams = solverParams;
            this.IterationResults = new List<IterationResult>();
            this.Results = new List<ReconciliationResult>();
            this.ConstraintResults = new List<ConstraintResult>();
        }
    }
}