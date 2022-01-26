using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class FuncCall : ICommand
    {
        string[] call;
        FuncCollection fc;

        public FuncCall(string[] call, FuncCollection fc, List<Tuple<string, bool>> outerfuncname)
        {
            this.call = call;
            this.fc = fc;
            
            Tuple<string, bool>[] propername = fc.GetProperName(call);
            for (int i = 0; i < propername.Length; i++)
            {
                if (propername[i].Item2 && !call[i].All(char.IsDigit))
                {
                    for (int j = 0; j < outerfuncname.Count; j++)
                    {
                        if (call[i] == outerfuncname[j].Item1)
                            outerfuncname[j] = new Tuple<string, bool>(outerfuncname[j].Item1, true);
                    }
                }
            }
        }

        public void Run(Dictionary<string, StonePile> env, bool print)
        {
            string s = "";
            foreach (string i in call)
            {
                if (s.Length > 0)
                    s += ' ';
                s += i;
            }
            if (print)
            {
                ICommand.PrintIndent();
                Console.WriteLine(s);
            }
            fc.RunFunc(call, env, print);
        }
    }
}
