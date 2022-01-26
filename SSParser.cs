using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class SSParser
    {
        Parser p;
        FuncCollection fc;

        public SSParser(Parser p, FuncCollection fc)
        {
            this.p = p;
            this.fc = fc;
        }

        /// <summary>
        /// Läser tokens tills den når en token i listan eller slutet på raden.
        /// </summary>
        /// <param name="tokens">En variabel som tar emot tokenarna</param>
        /// <param name="finishingTokens">En lista som på tokens somavbryter inläsningen</param>
        /// <returns>Tokenen, eller "" om radslut</returns>
        string ReadTokensUntilLineEndOr(List<string> tokens, string[] finishingTokens)
        {
            do
            {
                string token = p.GetToken();
                for (int i = 0; i < finishingTokens.Length; i++)
                {
                    if (token == finishingTokens[i])
                    {
                        return finishingTokens[i];
                    }
                }
                tokens.Add(token);
            } while (!p.AtEndOfRow());
            return "";
        }

        List<string> ReadTokensUntil(string expectedToken)
        {
            List<string> funccallparts = new();
            while (!p.AtEnd())
            {
                string token = p.GetToken();
                if (token == expectedToken)
                    return funccallparts;
                else
                    funccallparts.Add(token);
            }
            throw new PError("'" + expectedToken + "' förväntades");
        }

        List<Tuple<string, bool>> ReadFuncName()
        {
            List<Tuple<string, bool>> name = new();
            do
            {
                name.Add(new Tuple<string, bool>(p.GetToken(), false));
            }
            while (!p.AtEndOfRow());
            name.RemoveAt(name.Count - 1);
            return name;
        }

        ICommand ParseSecondBool(string endToken, List<Tuple<string, bool>> name)
        {
            ICommand secondcommand;
            string firstToken = p.GetToken();
            if (firstToken == "(")
            {
                secondcommand = ParseBool(")", name, false);
                if (!p.TryRead(endToken))
                    throw new PError("'" + endToken + "' förväntades");
            }
            else if (firstToken == "inte")
            {
                if (!p.TryRead("("))
                    throw new PError("'(' förväntades");
                secondcommand = ParseBool(")", name, true);
                if (!p.TryRead(endToken))
                    throw new PError("'" + endToken + "' förväntades");
            }
            else
            {
                List<string> tokens = new();
                tokens.Add(firstToken);
                ReadTokensUntilLineEndOr(
                    tokens,
                    new string[] { endToken }
                );
                tokens.Add("?");
                tokens.Add("i");
                tokens.Add(" ");
                secondcommand = new FuncCall(tokens.ToArray(), fc, name);
            }
            return secondcommand;
        }

        ICommand ParseBool(string endToken, List<Tuple<string, bool>> name, bool inverted)
        {
            ICommand firstcommand;
            string firstToken = p.GetToken();
            string finishingToken;
            if (firstToken == "(")
            {
                firstcommand = ParseBool(")", name, false);
                finishingToken = p.GetToken();
            }
            else if (firstToken == "inte")
            {
                if (!p.TryRead("("))
                    throw new PError("'(' förväntades");
                firstcommand = ParseBool(")", name, true);
                finishingToken = p.GetToken();
            }
            else
            {
                List<string> tokens = new();
                tokens.Add(firstToken);
                finishingToken = ReadTokensUntilLineEndOr(
                    tokens,
                    new string[] { endToken, "och", "eller" }
                );
                tokens.Add("?");
                tokens.Add("i");
                tokens.Add(" ");
                firstcommand = new FuncCall(tokens.ToArray(), fc, name);
            }

            if (finishingToken == "och")
            {
                ICommand secondcommand = ParseSecondBool(endToken, name);
                if (inverted)
                    return new Not(new And(firstcommand, secondcommand));
                else
                    return new And(firstcommand, secondcommand);
            }
            else if (finishingToken == "eller")
            {
                ICommand secondcommand = ParseSecondBool(endToken, name);
                if (inverted)
                    return new Not(new Or(firstcommand, secondcommand));
                else
                    return new Or(firstcommand, secondcommand);
            }
            if (inverted)
                return new Not(firstcommand);
            else
                return firstcommand;
        }

        /// <summary>
        /// Läser en loop.
        /// </summary>
        /// <param name="commands">Listan som ska ta emot statementet</param>
        /// <param name="name">Namnet på funktionen som statementet är i</param>
        void ParseLoop(List<ICommand> commands, List<Tuple<string, bool>> name)
        {
            ICommand loopcond = ParseBool("{", name, false);
            List<ICommand> loopcommands = new();
            while (ParseStatement(loopcommands, name) != "}") ;
            commands.Add(
                new Loop(
                    loopcond,
                    new Compound(loopcommands)
                )
            );
        }

        /// <summary>
        /// Läser en om-sats.
        /// </summary>
        /// <param name="commands">Listan som ska ta emot statementet</param>
        /// <param name="name">Namnet på funktionen som statementet är i</param>
        /// <returns>Vilken token som avslutade om-satsen</returns>
        string ParseIf(List<ICommand> commands, List<Tuple<string, bool>> name)
        {
            ICommand ifcond = ParseBool("så", name, false);
            List<ICommand> ifcommand = new();
            string s1term = ParseStatement(ifcommand, name);
            if (s1term == "annars" || p.TryRead("annars"))
            {
                List<ICommand> elsecommand = new();
                string s2term = ParseStatement(elsecommand, name);
                commands.Add(
                    new IfElse(
                        ifcond,
                        ifcommand[0],
                        elsecommand[0]
                    )
                );
                return s2term;
            }
            commands.Add(
                new If(
                    ifcond,
                    ifcommand[0]
                )
            );
            return s1term;
        }

        /// <summary>
        /// Läser ett funktionsanrop.
        /// </summary>
        /// <param name="commands">Listan som ska ta emot statementet</param>
        /// <param name="name">Namnet på funktionen som statementet är i</param>
        /// <returns>Vilken token som avslutade funktionsanropet</returns>
        string ParseFuncCall(List<ICommand> commands, List<Tuple<string, bool>> name)
        {
            List<string> funccallparts = new();
            string ret = ReadTokensUntilLineEndOr(
                funccallparts,
                new string[] { "}", ";", "annars" }
            );
            if (funccallparts.Count > 0)
                commands.Add(new FuncCall(funccallparts.ToArray(), fc, name));
            return ret;
        }

        void ParseBoolCall(List<ICommand> commands, List<Tuple<string, bool>> name, bool inverted)
        {
            ICommand boolcommand = ParseBool(")", name, inverted);
            if (!p.TryRead("?"))
                throw new PError("'?' förväntades");
            if (!p.TryRead("i"))
                throw new PError("'?' förväntades");
            string outvar = p.GetToken();
            for (int i = 0; i < name.Count; i++)
            {
                if (outvar == name[i].Item1)
                {
                    name[i] = new Tuple<string, bool>(name[i].Item1, true);
                }
            }
            commands.Add(new RunAndMove(boolcommand, " ", outvar));
        }

        /// <summary>
        /// Läser ett statement, dvs. ett anrop eller en loop. Läser även
        /// ev. ";" eller "}" i slutet.
        /// </summary>
        /// <param name="commands">Listan som ska ta emot statementet</param>
        /// <param name="name">Namnet på funktionen som statementet är i</param>
        /// <returns>Vilken token som avslutade statementet</returns>
        string ParseStatement(List<ICommand> commands, List<Tuple<string, bool>> name)
        {
            if (p.TryRead("sålänge"))
            {
                ParseLoop(commands, name);
                return "";
            }
            if (p.TryRead("om"))
            {
                return ParseIf(commands, name);
            }
            if (p.TryRead("("))
            {
                ParseBoolCall(commands, name, false);
                return "";
            }
            if (p.TryRead("inte"))
            {
                if (!p.TryRead("("))
                    throw new PError("'(' förväntades");
                ParseBoolCall(commands, name, true);
                return "";
            }
            else
            {
                return ParseFuncCall(commands, name);
            }
        }

        /// <summary>
        /// Parsea en funktion.
        /// </summary>
        void ParseFunc()
        {
            if (p.LastTokenInRow() != "=")
                throw new PError("Funktionsdefinition förväntades.");

            List<Tuple<string, bool>> name = ReadFuncName();
            List<ICommand> commands = new();
            while (!p.AtEnd() && p.LastTokenInRow() != "=")
            {
                ParseStatement(commands, name);
            }
            fc.AddFunc(name.ToArray(), new Compound(commands));
        }

        /// <summary>
        /// Parsea funktionerna i en fil.
        /// </summary>
        public void Parse()
        {
            while (!p.AtEnd())
            {
                ParseFunc();
            }
        }
    }
}
