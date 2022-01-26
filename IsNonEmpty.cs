using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class IsNonEmpty : ICommand
    {
        public void Run(Dictionary<string, StonePile> env, bool print)
        {
            if (env["x"].V > 0)
                env["s"].V = 1;
            else
                env["s"].V = 0;
        }
    }
}
