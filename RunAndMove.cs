using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class RunAndMove : ICommand
    {
        ICommand command;
        string from;
        string to;

        public RunAndMove(ICommand command, string from, string to)
        {
            this.command = command;
            this.from = from;
            this.to = to;
        }

        public void Run(Dictionary<string, StonePile> env, bool print)
        {
            command.Run(env, print);
            env[to].V = env[from].V;
        }
    }
}
