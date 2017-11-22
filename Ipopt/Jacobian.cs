using System;
using System.Linq;

namespace Ipopt
{
    public class Jacobian
    {
        public Int32[] iRow { get; set; }
        public Int32[] jCol { get; set; }
        public Double[] values { get; set; }

        public override String ToString()
        {
            var rows = iRow.Select(d => d.ToString());
            var cols = jCol.Select(d => d.ToString());
            var val = this.values.Select(d => d.ToString());
            return
                $"Rows: {String.Join(",", rows)}\r\nCols: {String.Join(",", cols)}\r\nvalues: {String.Join(",", val)}";
        }
    }
}