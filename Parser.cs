using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    /// <summary>
    /// En klass för att parsea filer.
    /// </summary>
    class Parser
    {
        string s;
        int pos;
        bool hasCrossedNewline;
        int row;
        int col;
        int disprow;
        int dispcol;

        /// <summary>
        /// Skapa en Parser för en fil.
        /// </summary>
        /// <param name="s">Innehållet</param>
        public Parser(string s)
        {
            this.s = s;
            pos = 0;
            row = 1;
            col = 1;
            disprow = row;
            dispcol = col;
            SkipWhitespace();
        }

        /// <summary>
        /// Skapa en kopia av en Parser. Kopians position är självständig från originialet.
        /// </summary>
        /// <returns>Kopian</returns>
        public Parser Clone()
        {
            return (Parser)MemberwiseClone();
        }

        /// <summary>
        /// Öka positionen, och håll reda på rad och kolumn.
        /// </summary>
        void IncPos()
        {
            if (!AtEnd() && s[pos] == '\n')
            {
                row++;
                col = 1;
            }
            else
            {
                col++;
            }
            pos++;
        }

        /// <summary>
        /// Flytta fram positionen så länge den pekar på whitespace.
        /// </summary>
        void SkipWhitespace()
        {
            hasCrossedNewline = false;
            while (!AtEnd() && char.IsWhiteSpace(s[pos]))
            {
                if (s[pos] == '\n')
                    hasCrossedNewline = true;
                IncPos();
            }
        }

        /// <summary>
        /// Kontrollerar om Parsern har nått filens slut.
        /// </summary>
        /// <returns>Om Parsern har nått slutet</returns>
        public bool AtEnd()
        {
            return pos >= s.Length;
        }

        /// <summary>
        /// Undersöker om ett tecken är ett tecken som anses kunna
        /// vara bredvid ett annat (icke-whitespace) tecken utan att
        /// hänga samman med det.
        /// </summary>
        /// <param name="c">Tecknet</param>
        /// <returns>Om det är det eller ej</returns>
        static bool IsAloneCharacter(char c)
        {
            return
                c == '?' ||
                c == '!' ||
                c == ',' ||
                c == '.' ||
                c == ';' ||
                c == ':' ||
                c == '(' ||
                c == ')' ||
                c == '[' ||
                c == ']' ||
                c == '{' ||
                c == '}';
        }

        /// <summary>
        /// Hämtar nästa sammanhängande sträng av antingen enbart alfanumeriska
        /// tecken eller enbart icke-alfanumeriska tecken. Whitespace försummas.
        /// Texten konverteras till små bokstäver.
        /// </summary>
        /// <returns>Strängen</returns>
        public string GetToken()
        {
            if (AtEnd()) throw new PError("Oväntat filslut");
            
            string ret = "";
            if (IsAloneCharacter(s[pos]))
            {
                ret += char.ToLower(s[pos]);
                IncPos();
            }
            else
            {
                do
                {
                    ret += char.ToLower(s[pos]);
                    IncPos();
                } while (
                    !AtEnd() &&
                    !char.IsWhiteSpace(s[pos]) &&
                    !IsAloneCharacter(s[pos])
                );
            }

            disprow = row;
            dispcol = col;
            SkipWhitespace();
            
            return ret;
        }

        /// <summary>
        /// Returnerar sant om positionen är vid slutet av en rad eller om
        /// den är vid slutet av filen.
        /// </summary>
        /// <returns>Om Parsern är vid slutet av en rad eller ej</returns>
        public bool AtEndOfRow()
        {
            return AtEnd() || hasCrossedNewline;
        }

        /// <summary>
        /// Returnerar den sista tokenen i raden, utan att flytta fram
        /// positionen.
        /// </summary>
        /// <returns>Radens sista token</returns>
        public string LastTokenInRow()
        {
            Parser p = Clone();
            string token;
            do
            {
                token = p.GetToken();
            } while (!p.AtEndOfRow());
            return token;
        }

        public string GetPosString()
        {
            return "rad " + disprow + ", kolumn " + dispcol;
        }

        /// <summary>
        /// Hoppar över nästa token om nästa token är en viss token,
        /// annars händer inget.
        /// </summary>
        /// <param name="token">Tokenen som begärs</param>
        /// <returns>Om det var den tokenen eller ej</returns>
        public bool TryRead(string token)
        {
            if (AtEnd())
                return false;

            Parser peeker = Clone();
            if (peeker.GetToken() == token)
            {
                GetToken();
                return true;
            }
            return false;
        }
    }
}
