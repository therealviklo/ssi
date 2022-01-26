using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    interface ICommand
    {
        void Run(Dictionary<string, StonePile> env, bool print);


        public static int indent = 0;

        public static void PrintIndent()
        {
            for (int i = 0; i < indent; i++)
                Console.Write(' ');
        }
    }
}
