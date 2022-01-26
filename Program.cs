using System;
using System.Collections.Generic;

namespace ssi
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser p = null;
            try
            {
                List<string> infiles = new();
                bool print = false;
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-p":
                            {
                                print = true;
                            }
                            break;
                        default:
                            {
                                infiles.Add(args[i]);
                            }
                            break;
                    }
                }
                FuncCollection fc = new(new List<Tuple<Tuple<string, bool>[], ICommand>>());
                fc.InjectDefaultFunctions();
                foreach (string file in infiles)
                {
                    p = new Parser(new FileContents(file).Contents);
                    SSParser ssp = new(p, fc);
                    ssp.Parse();
                }
                Dictionary<string, StonePile> env = new();
                fc.RunFunc(new string[] { "huvud", "i", "r" }, env, print);
                if (env.ContainsKey("r"))
                {
                    Console.WriteLine(env["r"].V);
                }
                else
                {
                    throw new PError("Huvud returnerade inget");
                }
            }
            catch (PError e)
            {
                if (p != null)
                    Console.WriteLine("Fel: " + e.Message + " (" + p.GetPosString() + ")");
            }
        }
    }
}
