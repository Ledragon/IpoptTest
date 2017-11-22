using System;
using System.IO;
using System.Linq;

namespace Ipopt
{
    internal class Program
    {
        private static void Main(String[] args)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var data = Path.Combine(baseDirectory,
                $@"..\..\data\data.json");
            var recResults = JsonFile.Load<ReconciliationResults>(data);
            var constraints = 2;
            var results = recResults.Results
                //.OrderBy(d => d.TagName)
                .ToList();
            var dic = results.Select((d, i) => new {Name = d.TagName, Index = i})
                .ToDictionary(d => d.Name, d => d.Index);
            var avg = results.Select(d => d.AverageValue).ToArray();
            var stdDev = results.Select(d => d.StdDev).ToArray();
            var lowerBounds = results.Select(d => d.Min).ToArray();
            var upperBounds = results.Select(d => d.Max).ToArray();
            var cLowerBounds = Enumerable.Range(0, constraints).Select(d => -Double.Epsilon).ToArray();
            var cUpperBoundsBounds = cLowerBounds.Select(d => Double.Epsilon).ToArray();

            var jacobianHelper = new JacobianHelper();

            Func<Double[], Double[]> constraintCallback = values =>
            {
                var gasHpToComp = values[dic["Gas Outlet HP to Comp (kSm3/d)"]];
                var gasLpToComp = values[dic["Gas Outlet LP to Comp (kSm3/d)"]];
                var flareRecovery = values[dic["Flares Recovery"]];
                var flareLP2 = dic.ContainsKey("Gas Comp LP2 to HP Flare (kSm3/d)")
                    ? values[dic["Gas Comp LP2 to HP Flare (kSm3/d)"]]
                    : 0;
                var gasBeforeTeg = gasHpToComp + gasLpToComp + flareRecovery + flareLP2;
                var teg = values[dic["Gas TEG"]];
                var r1 = gasBeforeTeg - teg;

                var gasExport = values[dic["Gas Export"]];
                var hp1Flare = values[dic["Gas Comp HP1 to HP Flare"]];
                var fuelgas = values[dic["Fuel Gas"]];
                var gasLift = values[dic["Gas Lift"]];

                var gasAfterTeg = gasExport + hp1Flare + fuelgas + gasLift;

                var r2 = teg - gasAfterTeg;
                return new[] {r2, r1};
            };
            var jac = jacobianHelper.ComputeJacobian(avg, constraintCallback);

            const String fileName = "ipoptLog";
            var ipoptParams = new IpoptParams
            {
                LogLevel = 12
            };
            for (var i = 0; i < 5; i++)
            {
                ipoptParams.LogFilePath = Path.Combine(baseDirectory, $"{fileName}.{i}.txt");
                var res = Solve(jacobianHelper, lowerBounds, upperBounds, cLowerBounds, cUpperBoundsBounds,
                    constraintCallback, jac, ipoptParams, avg, stdDev);
                Console.WriteLine("Resolution " + i + " - result: " + res.ReturnCode);
            }
            Console.WriteLine("Complete. Press any key to continue");
            Console.ReadLine();
        }

        private static SolverResult Solve(JacobianHelper jacobianHelper, Double[] lowerBounds, Double[] upperBounds,
            Double[] cLowerBounds, Double[] cUpperBoundsBounds, Func<Double[], Double[]> constraintCallback,
            Jacobian jac, IpoptParams ipoptParams,
            Double[] avg, Double[] stdDev)
        {
            var res = new SolverResult(new Double[0], -1);
            using (var sut = new IpoptReconciliationProblem(jacobianHelper, lowerBounds, upperBounds, cLowerBounds,
                cUpperBoundsBounds, constraintCallback, jac, ipoptParams, ir => { }))
            {
                res = sut.Solve(avg, stdDev);
            }
            return res;
        }
    }
}