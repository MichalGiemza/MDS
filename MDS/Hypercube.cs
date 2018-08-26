using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS_
{
    public static class Hypercube
    {
        public static int GetVerticesCount(int dimension)
        {
            return (int)Math.Pow(2, dimension);
        }

        public static int GetEdgesCount(int dimension)
        {
            return dimension * (int)Math.Pow(2, dimension - 1);
        }

        public static double[,] GenerateDistanceMatrix(int dimension)
        {
            int n = GetVerticesCount(dimension);

            double[,] m = new double[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    // h=0 => same point, distance = 0
                    // h=1 => edge, distance = 1
                    // h>1 => diagonal, distance = 1 * sqrt(h)
                    m[i, j] = 1.0 * Math.Sqrt(HammingDistance(i, j));
                }

            return m;
        }

        public static int HammingDistance(int n1, int n2)
        {
            int x = n1 ^ n2;

            int h = 0;
            while (x != 0)
            {
                x = (x & (x - 1));
                h++;
            }

            return h;
        }
    }
}
