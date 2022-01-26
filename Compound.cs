using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class Compound : ICommand
    {
        List<ICommand> commands;

        public Compound(List<ICommand> commands)
        {
            this.commands = commands;
        }

        public void Run(Dictionary<string, StonePile> env, bool print)
        {
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine("{");
                ICommand.indent++;
            }
            foreach (ICommand i in commands)
            {
                i.Run(env, print);
            }
            if (print)
            {
                ICommand.indent--;
                ICommand.PrintIndent();
                Console.WriteLine("}");
            }
        }
    }
}
