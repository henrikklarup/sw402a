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

        //Stores the row and column the token was found at
        public int row;
        public int col;

        public enum keywords
        {
            IDENTIFIER,
            NUMBER,
            LPAREN,
            RPAREN,
            PUNCTUATION,
            UP,
            DOWN,
            LEFT,
            RIGHT,
            HOLD,
            EOL,
            EOT,
            ERROR,
        }

        //The equivilint spellings of the tokens, used by the Token method to change the identifier from identifier to a keyword
        public static string[] spellings = 
        {
            "<identifier>", "<number>", "(", ")", ".", "up", "down", "left", "right", "hold", "<EOL>", "<EOT>", "<ERROR>"                         
        };

        //Converting the string of the identifier to a token if any keyword matches the string
        public Token(int kind, string spelling, int row, int col)
        {
            this.kind = kind;
            this.spelling = spelling;
            this.row = row;
            this.col = col;

            if (kind == (int)keywords.IDENTIFIER)
            {
                for (int i = (int)keywords.UP; i <= (int)keywords.HOLD; i++)
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
