using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPFitLib;

namespace PollMethodUsingThreads
{
    public class LeastSquareFitTest
    {
        /* Main function which drives the whole thing */
        public static void Main2()
        {
            double[] x = { 1.4, 1.6, 2.5 };
            double[] y = { 1.2, 5.2, -4.2 };
            int n = x.Length;
            double[] ey = new double[n];
            for (int i = 0; i < n; i++)
                ey[i] = 0.07;
            CustomUserVariable v = new CustomUserVariable();
            v.X = x;
            v.Y = y;
            v.Ey = ey;

            double[] p = { 1.0, 1.0 };
            int status;

            mp_result result = new mp_result(2);
            status = MPFit.Solve(ForwardModels.LinFunc, n, 2, p, null, null, v, ref result);
            p[1] *= -1.0;
            Console.WriteLine(p[0] + " + " + p[1] + "x");
            Console.ReadKey();
        }

        
    }

}
