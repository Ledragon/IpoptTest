using System;

namespace Ipopt
{
    public class ConstraintResult
    {
        public String Name { get; set; }
        public Double Target { get; set; }
        public Double ReconciledValue { get; set; }
        public Guid IdentifierGuid { get; set; }
        public Double Min { get; set; }
        public Double Max { get; set; }
    }
}