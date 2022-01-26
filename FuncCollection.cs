using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    using NameElem = Tuple<string, bool>; // (Ord, Är ett variabelnamn)

    /// <summary>
    /// En samling av funktioner med namn.
    /// </summary>
    class FuncCollection
    {
        List<Tuple<NameElem[], ICommand>> funcs;

        /// <summary>
        /// Kontrollerar om ett namn passar på ett namn. Om en namnsdel
        /// är ett variabelnamn så passar det alltid. Om namnen inte
        /// har samma längd så passar de aldrig.
        /// </summary>
        /// <param name="fitter">Namnet som kommer in</param>
        /// <param name="fittee">Det redan kända namnet, från listan</param>
        /// <returns></returns>
        static bool NameFits(string[] fitter, NameElem[] fittee)
        {
            if (fitter.Length != fittee.Length)
                return false;

            for (int i = 0; i < fittee.Length; i++)
            {
                if (!fittee[i].Item2)
                {
                    if (fitter[i] != fittee[i].Item1)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Kontrollerar om ett namn passar på ett namn. Om en namnsdel
        /// är ett variabelnamn så passar det alltid. Om namnen inte
        /// har samma längd så passar de aldrig.
        /// </summary>
        /// <param name="fitter">Namnet som kommer in</param>
        /// <param name="fittee">Det redan kända namnet, från listan</param>
        /// <returns></returns>
        static bool NameFits(NameElem[] fitter, NameElem[] fittee)
        {
            if (fitter.Length != fittee.Length)
                return false;

            for (int i = 0; i < fittee.Length; i++)
            {
                if (!fittee[i].Item2)
                {
                    if (fitter[i].Item1 != fittee[i].Item1)
                        return false;
                }
            }
            return true;
        }

        public FuncCollection(List<Tuple<NameElem[], ICommand>> funcs)
        {
            this.funcs = funcs;
        }

        /// <summary>
        /// Kör en funktion.
        /// </summary>
        /// <param name="name">Funktionsanropet</param>
        /// <param name="env">Variabelmiljön som funktionsanropet sker i</param>
        public void RunFunc(string[] name, Dictionary<string, StonePile> env, bool print)
        {
            // Gå igenom alla kända funktioner.
            for (int i = 0; i < funcs.Count; i++)
            {
                // Om funktionsanropet passar på funktionsnamnet
                if (NameFits(name, funcs[i].Item1))
                {
                    // Skapa en ny variabelmiljö där variablerna i anropen har namnen som
                    // funktionen förväntar sig.
                    Dictionary<string, StonePile> newenv = new();
                    for (int j = 0; j < name.Length; j++)
                    {
                        if (funcs[i].Item1[j].Item2)
                        {
                            // Om en variabel inte finns, lägg till den.
                            if (!env.ContainsKey(name[j]))
                            {
                                // Om namnet endast är siffror så skapas en hög med
                                // det antalet stenar.
                                if (name[j].All(char.IsDigit))
                                {
                                    newenv.Add(
                                        funcs[i].Item1[j].Item1,
                                        new StonePile(Convert.ToUInt64(name[j]))
                                    );
                                    // Skippar resten av loopen så att den inte
                                    // försöker lägga till högen igen utifrån namnet.
                                    continue;
                                }
                                else
                                {
                                    env.Add(name[j], new StonePile(0));
                                }
                            }
                            newenv.Add(funcs[i].Item1[j].Item1, env[name[j]]);
                        }
                    }
                    // Kör funktionen och returnera.
                    funcs[i].Item2.Run(newenv, print);
                    return;
                }
            }
        }

        static string StringifyFuncName(NameElem[] name)
        {
            string ret = "";
            foreach (NameElem i in name)
            {
                if (ret.Length > 0)
                    ret += ' ';
                ret += i.Item1;
            }
            return ret;
        }

        static string StringifyFuncName(string[] name)
        {
            string ret = "";
            foreach (string i in name)
            {
                if (ret.Length > 0)
                    ret += ' ';
                ret += i;
            }
            return ret;
        }

        /// <summary>
        /// Lägger till en funktion.
        /// </summary>
        /// <param name="name">Namnet</param>
        /// <param name="command">Kommandot som ska köras</param>
        public void AddFunc(NameElem[] name, ICommand command)
        {
            foreach (var i in funcs)
            {
                if (NameFits(name, i.Item1))
                    throw new PError("Funktionsnamnet \"" + StringifyFuncName(name) + "\" krockar med ett redan existerande namn.");
            }
            funcs.Add(new Tuple<NameElem[], ICommand>(name, command));
        }

        /// <summary>
        /// Hämtar namnet som är registrerat i FuncCollectionen utifrån en
        /// string[].
        /// </summary>
        /// <param name="name">En string[]</param>
        /// <returns>Motsvarande NameElem[]</returns>
        public NameElem[] GetProperName(string[] name)
        {
            foreach (var i in funcs)
            {
                if (NameFits(name, i.Item1))
                    return i.Item1;
            }
            throw new PError("Funktionen \"" + StringifyFuncName(name) + "\" finns inte");
        }

        public void InjectDefaultFunctions()
        {
            AddFunc(
                new Tuple<string, bool>[] {
                        new Tuple<string, bool>("öka", false),
                        new Tuple<string, bool>("x", true)
                },
                new Inc()
            );
            AddFunc(
                new Tuple<string, bool>[] {
                        new Tuple<string, bool>("minska", false),
                        new Tuple<string, bool>("x", true)
                },
                new Dec()
            );
            AddFunc(
                new Tuple<string, bool>[] {
                        new Tuple<string, bool>("x", true),
                        new Tuple<string, bool>("är", false),
                        new Tuple<string, bool>("icketom", false),
                        new Tuple<string, bool>("?", false),
                        new Tuple<string, bool>("i", false),
                        new Tuple<string, bool>("s", true)
                },
                new IsNonEmpty()
            );
        }
    }
}
