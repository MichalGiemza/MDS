using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS_
{
    public class GradientDescent : IMinimalizationMethod
    {
        readonly double min = -0.5;
        readonly double max = 0.5;

        readonly double[,] d0;
        readonly Solution.Function fun;
        readonly int n;

        double[,] d;
        ISolution old;
        double alpha;

        public double Alpha { get { return alpha; } set { alpha = value; } }
        
        public GradientDescent(double[,] d0, double alpha, int count)
        {
            this.d0 = d0;
            Alpha = alpha;
            fun = MDS.KruskalStress;
            n = count;
        }

        public ISolution InitPos()
        {
            // random solution for start
            Random r = new Random();
            List<double[]> pts = new List<double[]>();

            for (int k = 0; k < n; k++)
                pts.Add(new double[2] { RndRange(r, min, max), RndRange(r, min, max) });

            d = MDS.CalcDistances(pts, MDS.EuclideanDistance);

            old = new Solution(fun, d0, d, n, pts);
            return old;
        }

        public ISolution NextPos()
        {
            // calculated using Euclidean Distance

            if (old == null)
                InitPos();
            
            // Calc gradient
            var grad = Grad(d0, d, old);
            // - grad * alpha
            foreach (var pt in grad)
            {
                pt[0] = -pt[0] * Alpha; // X
                pt[1] = -pt[1] * Alpha; // Y
            }
            // next_step = old - alpha * grad
            for (int k = 0; k < n; k++)
            {
                grad[k][0] += old.GetArgument()[k][0]; // X
                grad[k][1] += old.GetArgument()[k][1]; // Y
            }

            // save changes and return
            d = MDS.CalcDistances(grad, MDS.EuclideanDistance);
            old = new Solution(fun, d0, d, n, grad);
            return old;
        }

        List<double[]> Grad(double[,] d0, double[,] d, ISolution s)
        {
            int n = s.GetArgument().Count;

            double _B = B(d0, n);
            double _A = A(s);

            List<double[]> grad = new List<double[]>();
            for (int k = 0; k < n; k++)
                grad.Add(CalcGradPt(k, d0, d, s, _A, _B));

            return grad;
        }
        double[] CalcGradPt(int k, double[,] d0, double[,] d, ISolution s, double _A, double _B)
        {
            var pt = new double[2];

            for (int i = 0; i < n; i++)
            {
                if (i != k)
                {
                    pt[0] += C(i, k, 0, s, d0, _B); // X
                    pt[1] += C(i, k, 1, s, d0, _B); // Y
                }
            }

            pt[0] *= _A; // X
            pt[1] *= _A; // Y

            return pt;
        }
        double A(ISolution s)
        {
            return 1 / (2 * s.GetValue());
        }
        double B(double[,] d0, int n)
        {
            double sum = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    sum += Math.Pow(d0[i, j], 2);
                }
            }

            return sum;
        }
        double C(int i, int j, int pt_i, ISolution s, double[,] d0, double B)
        {
            return 2.0 / B * (-s.GetArgument()[i][pt_i] + s.GetArgument()[j][pt_i]) * (1 - d0[i, j] / d[i, j]);
        }

        static double RndRange(Random r, double min, double max)
        {
            return r.NextDouble() * (Math.Abs(min) + Math.Abs(max)) + min;
        }
    }
}
