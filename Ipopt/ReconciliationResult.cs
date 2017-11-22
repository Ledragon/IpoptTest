using System;

namespace Ipopt
{
    public class ReconciliationResult
    {
        public String TagName { get; set; }
        public Guid Guid { get; set; }
        public Double AverageValue { get; set; }
        public Double ReconciledValue { get; set; }
        public Double Resolution { get; set; }
        public Double Penalty { get; set; }
        public Double Min { get; set; }
        public Double Max { get; set; }
        public Double StdDev { get; set; }
        public Double InitialValue { get; set; }

        public override String ToString()
        {
            return
                String.Format(
                    "TagName: {0}, Guid: {1}, AverageValue: {2}, ReconciledValue: {3}, Resolution: {4}, Penalty: {5}",
                    this.TagName, this.Guid, this.AverageValue, this.ReconciledValue, this.Resolution, this.Penalty);
        }
    }
}