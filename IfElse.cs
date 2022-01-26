using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class IfElse : ICommand
    {
        ICommand condition;
        ICommand command;
        ICommand elsecommand;

        public IfElse(ICommand condition, ICommand command, ICommand elsecommand)
        {
            this.condition = condition;
            this.command = command;
            this.elsecommand = elsecommand;
        }

        public void Run(Dictionary<string, StonePile> env, bool print)
        {
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine("Om-eller-krav:");
            }
            condition.Run(env, print);
            if (env[" "].V > 0)
            {
                // Loopen härmar det som händer enligt bokens definition
                // om s > 1.
                while (env[" "].V-- > 0)
                {
                    if (print)
                    {
                        ICommand.PrintIndent();
                        Console.WriteLine("Omkod:");
                    }
                    command.Run(env, print);
                }
            }
            else
            {
                if (print)
                {
                    ICommand.PrintIndent();
                    Console.WriteLine("Ellerkod:");
                }
                elsecommand.Run(env, print);
            }
        }
    }
}
