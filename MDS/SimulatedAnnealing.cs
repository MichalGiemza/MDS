using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS_
{
    public class SimulatedAnnealing : IMinimalizationMethod
    {
        double[,] d0;
        int i = 0;
        int dimY;
        int count;
        ISolution s;
        Random r = new Random();
        double temperature;

        public SimulatedAnnealing(double initial_temperature, int count, double[,] d0, int dimY)
        {
            temperature = initial_temperature;
            this.count = count;
            this.dimY = dimY;
            this.d0 = d0;
        }
        
        public ISolution NextPos()
        {
            if (s == null)
                InitPos();

            temperature = Temperature();
            ISolution s_new = NewSolution();

            double acceptance = AcceptanceProbability(s, s_new, temperature);
            if (acceptance > r.NextDouble())
                s = s_new;

            i++;

            return s;
        }

        double Temperature()
        {
            return temperature / 2;
        }

        static double AcceptanceProbability(ISolution s, ISolution s_new, double temperature)
        {
            double s_val = s.GetValue();
            double s_new_val = s_new.GetValue();

            if (s_new_val < s_val)
                return 1.0;
            else
            {
                return Math.Exp((s_val - s_new_val) / temperature * 100);
            }
        }

        public ISolution InitPos()
        {
            s = NewSolution();
            return s;
        }

        private ISolution NewSolution()
        {
            const double min = -1;
            const double max = 1;

            Random r = new Random();
            var y = new List<double[]>();

            for (int j = 0; j < count; j++)
            {
                double[] _y = new double[dimY];

                for (int i = 0; i < dimY; i++)
                {
                    _y[i] = NextGaussian(r,1,1) * (Math.Abs(min) + Math.Abs(max)) + min; // r.NextDouble()
                }

                y.Add(_y);
            }

            return new Solution(MDS.KruskalStress, d0, MDS.CalcDistances(y, MDS.EuclideanDistance), count, y);
        }

        double NextGaussian(Random r, double mu, double sigma)
        {
            var u1 = r.NextDouble();
            var u2 = r.NextDouble();

            var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            var rand_normal = mu + sigma * rand_std_normal;

            return rand_normal;
        }
    }
}
