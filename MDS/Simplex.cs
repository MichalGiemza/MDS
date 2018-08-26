using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS_
{
    public static class Simplex
    {
        public static int GetVerticesCount(int dimension)
        {
            return dimension + 1;
        }

        public static int GetEdgesCount(int dimension)
        {
            if (dimension < 1)
                return int.MinValue;
            if (dimension == 1)
                return 1;

            return GetEdgesCount(dimension - 1) + GetVerticesCount(dimension - 1);
        }

        public static double[,] GenerateDistanceMatrix(int dimension)
        {
            int n = GetVerticesCount(dimension);

            double[,] m = new double[n, n];

            // All vertices in same distance
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    m[i, j] = i == j ? 0 : 1;

            return m;
        }
    }
}
