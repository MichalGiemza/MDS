using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS_
{
    public class MDS
    {
        public delegate double Distance(double[] x1, double[] x2);
        public delegate double Stress(double[,] d0, double[,] d, int dimX);

        private int iteration = 0;
        Distance distance;
        Stress stress;
        IMinimalizationMethod minimalization;

        public int Iteration { get => iteration; }

        public MDS(Distance distanceFunction, Stress stressFunction, IMinimalizationMethod minimalizationMethod)
        {
            distance = distanceFunction;
            stress = stressFunction;
            minimalization = minimalizationMethod;
        }

        public ISolution InitPos()
        {
            return minimalization.InitPos();
        }

        public ISolution NextIteration()
        {
            iteration++;
            return minimalization.NextPos();
        }

        public static double[,] CalcDistances(List<double[]> x, Distance distance)
        {
            var distanceMatrix = new double[x.Count, x.Count];

            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < x.Count; j++)
                {
                    distanceMatrix[i, j] = distance(x[i], x[j]);
                }
            }

            return distanceMatrix;
        }

        public static double EuclideanDistance(double[] x1, double[] x2)
        {
            double sum = 0;

            for (int i = 0; i < x1.Length; i++)
            {
                sum += Math.Pow(x1[i] - x2[i], 2);
            }

            return Math.Sqrt(sum);
        }

        public static double KruskalStress(double[,] d0, double[,] d, int count)
        {
            double sum1 = 0;
            double sum2 = 0;

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    sum1 += Math.Pow(d0[i, j] - d[i, j], 2);
                    sum2 += Math.Pow(d0[i, j], 2);
                }
            }

            return Math.Sqrt(sum1 / sum2);
        }
    }
}
