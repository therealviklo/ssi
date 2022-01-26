using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class Loop : ICommand
    {
        ICommand condition;
        ICommand command;

        public Loop(ICommand condition, ICommand command)
        {
            this.condition = condition;
            this.command = command;
        }

        public void Run(Dictionary<string, StonePile> env, bool print)
        {
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine("Loopkrav:");
            }
            condition.Run(env, print);
            while (env[" "].V > 0)
            {
                if (print)
                {
                    ICommand.PrintIndent();
                    Console.WriteLine("Loopkod:");
                }
                command.Run(env, print);
                if (print)
                {
                    ICommand.PrintIndent();
                    Console.WriteLine("Loopkrav:");
                }
                condition.Run(env, print);
            }
        }
    }
}
