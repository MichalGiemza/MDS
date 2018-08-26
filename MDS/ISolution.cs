using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS_
{
    public interface ISolution
    {
        double GetValue();
        List<double[]> GetArgument();
    }
}
