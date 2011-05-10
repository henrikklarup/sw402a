using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionPatternCompiler
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
            UNIT,
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
            "unit", "encounter", "move", "<EOL>", "<EOT>", "<ERROR>"
        };

        //Converting the string of the identifier to a token if any keyword matches the string
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
