using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS_
{
    public class Solution : ISolution
    {
        public delegate double Function(double[,] d0, double[,] d, int count);

        private double? value = null;
        private Function function;
        private double[,] d0;
        private double[,] d;
        private List<double[]> argument;
        private int count;

        public Solution(Function function, double[,] d0, double[,] d, int count, List<double[]> arg)
        {
            this.function = function;
            this.d0 = d0;
            this.d = d;
            this.count = count;
            argument = arg;
        }

        public double GetValue()
        {
            if (value == null)
            {
                value = function(d0, d, count);
            }

            return value.Value;
        }

        public List<double[]> GetArgument()
        {
            return argument;
        }
    }
}
