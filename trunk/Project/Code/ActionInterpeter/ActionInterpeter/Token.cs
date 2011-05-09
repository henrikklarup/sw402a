using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    class Token
    {
        public int kind;
        public string spelling;

        public enum keywords
        {
            IDENTIFIER,
            NUMBER,
            PUNCTUATION,
            COMMA,
            UP,
            DOWN,
            LEFT,
            RIGHT,
            HOLD,
            AGENT,
            A,
            TEAM,
            T,
            SQUAD,
            S,
            ENCOUNTER,
            MOVE,
            EOL,
            EOT,
            ERROR,
        }

        //The equivilint spellings of the tokens, used by the Token method to change the identifier from identifier to a keyword
        public static string[] spellings = 
        {
            "<identifier>", "<number>", ".", ",", "up", "down", "left", "right", "hold", 
            "agent", "a", "team", "t", "squad", "s", "encounter", "move", "<EOL>", "<EOT>", "<ERROR>"
        };

        /// <summary>
        /// Converting the string of the identifier to a token if any keyword matches the string
        /// </summary>
        /// <param name="kind">The kind of the word (identifier for all words)</param>
        /// <param name="spelling">The spelling of the word</param>
        public Token(int kind, string spelling)
        {
            this.kind = kind;
            this.spelling = spelling;

            if (kind == (int)keywords.IDENTIFIER)
            {
                for (int i = (int)keywords.UP; i <= (int)keywords.MOVE; i++)
                {
                    if (spelling.ToLower().Equals(spellings[i]))
                    {
                        this.kind = i;
                        break;
                    }
                }
            }
        }
    }
}
