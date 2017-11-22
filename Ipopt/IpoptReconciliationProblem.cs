using System;
using System.Linq;
using Cureos.Numerics;
using Ipopt;

namespace WideTech.WiDE.Business.Reconciliation
{
    public class IpoptReconciliationProblem : IpoptProblem
    {
        private Double[] _standardDeviations;
        private Double[] _averageValues;
        private readonly JacobianHelper _jacobianHelper;
        private readonly Func<Double[], Double[]> _constraints;
        private Jacobian _jacobian;
        private readonly Action<IterationResult> _callback;
        private Boolean _refreshJac;
        private Int32 _jacobianEvaluationFrequency;

        public SolverResult Solve(Double[] averageValues, Double[] standardDeviations)
        {
            return this.Solve(averageValues, standardDeviations, averageValues.Clone() as Double[]);
        }

        public SolverResult Solve(Double[] averageValues, Double[] standardDeviations, Double[] initialValues)
        {
            Double obj;
            this._averageValues = averageValues;
            this._standardDeviations = standardDeviations;
            var code = this.SolveProblem(initialValues, out obj);
            var result = new SolverResult(initialValues, (Int32)code);
            return result;
        }

        #region IpoptProblem methods override

        public override IpoptBoolType eval_f(Int32 n, Double[] x, IpoptBoolType new_x, out Double obj_value,
            IntPtr p_user_data)
        {
            //Objective function
            obj_value = x.Select((d, i) => Math.Pow((d - this._averageValues[i]) / this._standardDeviations[i], 2)).Sum();
            return IpoptBoolType.True;
        }

        public override IpoptBoolType eval_g(Int32 n, Double[] x, IpoptBoolType new_x, Int32 m, Double[] g,
            IntPtr p_user_data)
        {
            //Contstraints
            var updatedConstraints = this._constraints(x);
            for (var i = 0; i < updatedConstraints.Length; i++)
            {
                g[i] = updatedConstraints[i];
            }
            return IpoptBoolType.True;
        }

        public override IpoptBoolType eval_grad_f(Int32 n, Double[] x, IpoptBoolType new_x, Double[] grad_f,
            IntPtr p_user_data)
        {
            for (var i = 0; i < x.Length; i++)
            {
                grad_f[i] = 2 * ((x[i] - this._averageValues[i]) / this._standardDeviations[i]) *
                            (1 / this._standardDeviations[i]);
            }
            return IpoptBoolType.True;
        }

        public override IpoptBoolType eval_jac_g(Int32 n, Double[] x, IpoptBoolType new_x, Int32 m, Int32 nele_jac,
            Int32[] iRow, Int32[] jCol, Double[] values, IntPtr p_user_data)
        {
            if (values == null)
            {
                for (var i = 0; i < iRow.Length; i++)
                {
                    iRow[i] = this._jacobian.iRow[i];
                }
                for (var i = 0; i < jCol.Length; i++)
                {
                    jCol[i] = this._jacobian.jCol[i];
                }
            }
            else
            {
                if (this._refreshJac)
                {
                    this._jacobian = this._jacobianHelper.UpdateJacobian(x, this._constraints, this._jacobian);
                }
                //else
                //{
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = this._jacobian.values[i];
                }
                //}
            }
            return IpoptBoolType.True;
        }

        public override IpoptBoolType eval_h(Int32 n, Double[] x, IpoptBoolType new_x, Double obj_factor, Int32 m,
            Double[] lambda, IpoptBoolType new_lambda, Int32 nele_hess, Int32[] iRow, Int32[] jCol, Double[] values,
            IntPtr p_user_data)
        {
            return IpoptBoolType.True;
        }

        /// <summary>
        /// Intermediate callback for each iteration.
        /// </summary>
        /// <param name="alg_mod">The algorithm mode.</param>
        /// <param name="iter_count">The current iteration count.</param>
        /// <param name="obj_value">The unscaled objective value at the current point.</param>
        /// <param name="inf_pr">The unscaled constraint violation at the current point.</param>
        /// <param name="inf_du">The scaled dual infeasibility at the current point.</param>
        /// <param name="mu">log_10 of the value of the barrier parameter mu</param>
        /// <param name="d_norm">The infinity norm (max) of the primal step.</param>
        /// <param name="regularization_size">log_10 of the value of the regularization term for the Hessian of the Lagrangian in the augmented system. A dash (-) indicates that no regularization was done.</param>
        /// <param name="alpha_du">The step size for the dual variables for the box constraints in the equality constrained formulation.</param>
        /// <param name="alpha_pr">The step size for the primal variables  x and  lambda in the equality constrained formulation.</param>
        /// <param name="ls_trials">The number of backtracking line search steps (does not include second-order correction steps).</param>
        /// <param name="p_user_data">User data.</param>
        /// <returns></returns>
        public override IpoptBoolType intermediate(IpoptAlgorithmMode alg_mod, Int32 iter_count, Double obj_value,
            Double inf_pr, Double inf_du,
            Double mu, Double d_norm, Double regularization_size, Double alpha_du, Double alpha_pr, Int32 ls_trials,
            IntPtr p_user_data)
        {
            this._callback(new IterationResult
            {
                ObjectiveFunction = obj_value,
                IterationNumber = iter_count,
                BacktrackingLines = ls_trials,
                ConstraintViolation = inf_pr,
                DualInfeasibility = inf_du,
                DualVariablesStepSize = alpha_du,
                Mu = mu,
                PrimalStepNorm = d_norm,
                PrimalVariablesStepSize = alpha_pr,
                RegularizationTerm = regularization_size
            });
            this._refreshJac = iter_count > 0 && iter_count % this._jacobianEvaluationFrequency == 0;
            return IpoptBoolType.True;
        }

        #endregion

        #region Constructors

        public IpoptReconciliationProblem(JacobianHelper jacobianHelper, Double[] lowerBounds,
            Double[] upperBounds, Double[] constraintLowerBounds, Double[] constraintUpperBounds,
            Func<Double[], Double[]> constraints, Jacobian jacobian, IpoptParams ipoptParams,
            Action<IterationResult> callback)
            : base(
                lowerBounds.Length, lowerBounds, upperBounds, constraintLowerBounds.Length, constraintLowerBounds,
                constraintUpperBounds, jacobian.iRow.Length, 0, true, true, true)
        {
            this._jacobianHelper = jacobianHelper;
            this._constraints = constraints;
            this._jacobian = jacobian;
            this._callback = callback;
            this.SetOptions(ipoptParams);
        }

        #endregion

        private void SetOptions(IpoptParams ipoptParams)
        {
            this.AddOption("tol", ipoptParams.Tolerance);
            this.AddOption("max_iter", ipoptParams.MaximumIterations);
            this.AddOption("dual_inf_tol", ipoptParams.DualInfeasibilityTolerance);
            this.AddOption("constr_viol_tol", ipoptParams.ConstraintViolationTolerance);
            this.AddOption("compl_inf_tol", ipoptParams.ComplementaryInfTolerance);

            // Acceptable solution
            this.AddOption("acceptable_iter", ipoptParams.AcceptableIterationNumber);
            this.AddOption("acceptable_tol", ipoptParams.AcceptableTolerance);
            this.AddOption("acceptable_obj_change_tol", ipoptParams.AcceptableObjFctChange);
            this.AddOption("acceptable_dual_inf_tol", ipoptParams.AcceptableDualInfeasibilityTolerance);
            this.AddOption("acceptable_constr_viol_tol", ipoptParams.AcceptableConstraintViolationTolerance);

            // Initialization
            this.AddOption("bound_mult_init_val", ipoptParams.BoundMultiplier);
            this.AddOption("bound_mult_init_method", ipoptParams.BoundMultiplierInitializationMethod);
            this.AddOption("mu_init", ipoptParams.MuInit);
            this.AddOption("mu_strategy", ipoptParams.MuStrategy);
            //this.AddOption("nlp_upper_bound_inf", 1e-8);
            //this.AddOption("bound_relax_factor", 1e-4);
            //this.AddOption("nlp_scaling_min_value", 0.1);
            //this.AddOption("bound_push", 1e-16);
            //this.AddOption("bound_frac", 1e-16);

            this.AddOption("alpha_for_y", ipoptParams.AlphaForY);
            //this.AddOption("quality_function_max_section_steps",3);
            this.AddOption("recalc_y_feas_tol", ipoptParams.RecalcYFeasTol);
            this.AddOption("recalc_y", ipoptParams.RecalcY);
            if (!String.IsNullOrEmpty(ipoptParams.LogFilePath))
            {
                this.OpenOutputFile(ipoptParams.LogFilePath, ipoptParams.LogLevel);
            }
            //JsonFile.Save(ipoptParams, @"d:\tmp\ipoptParams.json");

            this.AddOption("nlp_scaling_max_gradient", ipoptParams.MaxGradient);
            this.AddOption("mumps_scaling", ipoptParams.MumpsScaling);
            this._jacobianEvaluationFrequency = ipoptParams.JacFreq;
        }
    }
}