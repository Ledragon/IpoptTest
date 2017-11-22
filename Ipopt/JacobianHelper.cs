using System;
using System.Collections.Generic;
using System.Linq;

namespace Ipopt
{
    public class JacobianHelper
    {
        public Jacobian ComputeJacobian(Double[] averageValues, Func<Double[], Double[]> constraints)
        {
            var iRow = new List<Int32>();
            var jCol = new List<Int32>();
            var values = new List<Double>();
            var originalConstraintValues = constraints(averageValues);
            for (var j = 0; j < averageValues.Length; j++)
            {
                var averageValue = averageValues[j];
                var originalValue = averageValue;
                averageValues[j] = averageValue * (1 + 1e-9);
                if (Math.Abs(averageValues[j]) < Double.Epsilon)
                {
                    averageValues[j] = 1e-9;
                }
                var updatedConstraintValues = constraints(averageValues);
                for (var i = 0; i < updatedConstraintValues.Length; i++)
                {
                    var diff = updatedConstraintValues[i] - originalConstraintValues[i];
                    if (Math.Abs(diff) >= Double.Epsilon)
                    {
                        iRow.Add(i);
                        jCol.Add(j);
                        values.Add(diff / (averageValues[j] - originalValue));
                    }
                }
                averageValues[j] = originalValue;
            }
            var computeJacobian = new Jacobian
            {
                iRow = iRow.ToArray(),
                jCol = jCol.ToArray(),
                values = values.ToArray()
            };
            return computeJacobian;
        }

        public Jacobian UpdateJacobian(Double[] averageValues, Func<Double[], Double[]> constraints,
            Jacobian originalJacobian)
        {
            //i: constraints
            //j: variables

            var iRow = originalJacobian.iRow;
            var jCol = originalJacobian.jCol;
            var values = new Double[originalJacobian.values.Length];
            var originalConstraintValues = constraints(averageValues);
            const Double disturbance = 1e-6;
            var globalIndex = 0;
            for (var j = 0; j < averageValues.Length; j++)
            {
                var averageValue = averageValues[j];
                var originalValue = averageValue;
                if (Math.Abs(averageValue) < disturbance)
                {
                    averageValues[j] = disturbance;
                }
                else
                {
                    averageValues[j] = averageValue * (1 + disturbance);
                }
                var updatedConstraintValues = constraints(averageValues);
                var toUpdate = jCol.Select((d, i) => new { index = i, jj = d })
                    .Where(d => d.jj == j)
                    .Select(d => d.index)
                    .ToList();
                foreach (var index in toUpdate)
                {
                    var constraintNumber = iRow[index];
                    var diff = updatedConstraintValues[constraintNumber] - originalConstraintValues[constraintNumber];
                    var gradient = diff / (averageValues[j] - originalValue);
                    values[globalIndex] = gradient;
                    var minGradient = 1e-12;
                    if (Math.Abs(gradient) < minGradient)
                    {
                        values[globalIndex] = minGradient;
                        if (gradient < 0)
                        {
                            values[globalIndex] = -values[globalIndex];
                        }
                    }
                    globalIndex++;
                }
                averageValues[j] = originalValue;
            }
            var computeJacobian = new Jacobian
            {
                iRow = iRow,
                jCol = jCol,
                values = values
            };
            return computeJacobian;
        }
    }
}