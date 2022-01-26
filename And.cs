using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class And : ICommand
    {
        ICommand firstcommand;
        ICommand secondcommand;

        public And(ICommand firstcommand, ICommand secondcommand)
        {
            this.firstcommand = firstcommand;
            this.secondcommand = secondcommand;
        }

        public void Run(Dictionary<string, StonePile> env, bool print)
        {
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine("Första ledet i och:");
            }
            firstcommand.Run(env, print);
            bool fcres = env[" "].V > 0;
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine("Andra ledet i och:");
            }
            secondcommand.Run(env, print);
            bool scres = env[" "].V > 0;
            env[" "].V = fcres && scres ? 1u : 0u;
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine($"Och blev {fcres && scres}.");
            }
        }
    }
}
