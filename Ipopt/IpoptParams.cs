using System;

namespace Ipopt
{
    //IPOPT parameters - http://www.coin-or.org/Ipopt/documentation/node42.html
    /// <summary>
    ///     Class containing IPOPT paramaeters.
    /// </summary>
    public class IpoptParams
    {
        /// <summary>
        /// Desired convergence tolerance (relative).
        /// </summary>
        public Double Tolerance { get; set; }

        /// <summary>
        /// Maximum number of iterations.
        /// </summary>
        public Int32 MaximumIterations { get; set; }

        /// <summary>
        /// Desired threshold for the dual infeasibility.
        /// </summary>
        public Double DualInfeasibilityTolerance { get; set; }

        /// <summary>
        /// Desired threshold for the constraint violation.
        /// </summary>
        public Double ConstraintViolationTolerance { get; set; }

        /// <summary>
        /// Desired threshold for the complementarity conditions.
        /// </summary>
        public Double ComplementaryInfTolerance { get; set; }


        public Double AcceptableTolerance { get; set; }
        public Int32 AcceptableIterationNumber { get; set; }
        public Double AcceptableConstraintViolationTolerance { get; set; }
        public Double AcceptableDualInfeasibilityTolerance { get; set; }
        public Double AcceptableComplementaryInfTolerance { get; set; }
        public Double AcceptableObjFctChange { get; set; }


        // Initialization
        /// <summary>
        /// Initial value for the bound multipliers.
        /// </summary>
        public Double BoundMultiplier { get; set; }

        public String BoundMultiplierInitializationMethod { get; set; }
        public Double MuInit { get; set; }
        public String MuStrategy { get; set; }
        public Double RecalcYFeasTol { get; set; }
        public String RecalcY { get; set; }
        public String AlphaForY { get; set; }
        public Double MaxGradient { get; set; }
        public Int32 MumpsScaling { get; set; }
        public Int32 JacFreq { get; set; }
        public String LogFilePath { get; set; }
        public Int32 LogLevel { get; set; }
        public IpoptParams()
        {
            this.Tolerance = 1e-8;
            this.MaximumIterations = 50;
            this.DualInfeasibilityTolerance = 1;
            this.ConstraintViolationTolerance = 0.0001;
            this.ComplementaryInfTolerance = 0.0001;

            this.AcceptableTolerance = 1e-6d;
            this.AcceptableIterationNumber = 15;
            this.AcceptableConstraintViolationTolerance = .01;
            this.AcceptableDualInfeasibilityTolerance = 1e10d;
            this.AcceptableComplementaryInfTolerance = 0.01;
            this.AcceptableObjFctChange = 1e+20d;

            this.BoundMultiplier = 1;
            this.BoundMultiplierInitializationMethod = "constant";
            this.MuInit = 1;
            this.MuStrategy = "monotone";
            this.AlphaForY = "primal";
            this.RecalcY = "no";
            this.RecalcYFeasTol = 1e-6;
            this.MaxGradient = 100;
            this.MumpsScaling = 77;
            this.JacFreq = 3;

            this.LogLevel = 5;
        }

        public override String ToString()
        {
            return $"{nameof(this.Tolerance)}: {this.Tolerance}, {nameof(this.MaximumIterations)}: {this.MaximumIterations}, {nameof(this.DualInfeasibilityTolerance)}: {this.DualInfeasibilityTolerance}, {nameof(this.ConstraintViolationTolerance)}: {this.ConstraintViolationTolerance}, {nameof(this.ComplementaryInfTolerance)}: {this.ComplementaryInfTolerance}, {nameof(this.AcceptableTolerance)}: {this.AcceptableTolerance}, {nameof(this.AcceptableIterationNumber)}: {this.AcceptableIterationNumber}, {nameof(this.AcceptableConstraintViolationTolerance)}: {this.AcceptableConstraintViolationTolerance}, {nameof(this.AcceptableDualInfeasibilityTolerance)}: {this.AcceptableDualInfeasibilityTolerance}, {nameof(this.AcceptableComplementaryInfTolerance)}: {this.AcceptableComplementaryInfTolerance}, {nameof(this.AcceptableObjFctChange)}: {this.AcceptableObjFctChange}, {nameof(this.BoundMultiplier)}: {this.BoundMultiplier}, {nameof(this.BoundMultiplierInitializationMethod)}: {this.BoundMultiplierInitializationMethod}, {nameof(this.MuInit)}: {this.MuInit}, {nameof(this.MuStrategy)}: {this.MuStrategy}, {nameof(this.RecalcYFeasTol)}: {this.RecalcYFeasTol}, {nameof(this.RecalcY)}: {this.RecalcY}, {nameof(this.AlphaForY)}: {this.AlphaForY}, {nameof(this.MaxGradient)}: {this.MaxGradient}, {nameof(this.MumpsScaling)}: {this.MumpsScaling}, {nameof(this.JacFreq)}: {this.JacFreq}";
        }
    }
}