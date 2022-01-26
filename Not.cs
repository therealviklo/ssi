using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class Not : ICommand
    {
        ICommand invertedcommand;

        public Not(ICommand invertedcommand)
        {
            this.invertedcommand = invertedcommand;
        }

        public void Run(Dictionary<string, StonePile> env, bool print)
        {
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine("Inte:");
            }
            invertedcommand.Run(env, print);
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine("Interesultatet är " + env[" "].V + ".");
            }
            env[" "].V = env[" "].V > 0u ? 0u : 1u;
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine("Så resultatet är " + env[" "].V + ".");
            }
        }
    }
}
