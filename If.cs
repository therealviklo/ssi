using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class If : ICommand
    {
        ICommand condition;
        ICommand command;

        public If(ICommand condition, ICommand command)
        {
            this.condition = condition;
            this.command = command;
        }

        public void Run(Dictionary<string, StonePile> env, bool print)
        {
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine("Omkrav:");
            }
            condition.Run(env, print);
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
    }
}
