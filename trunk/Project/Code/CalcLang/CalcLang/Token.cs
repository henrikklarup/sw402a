using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcLang
{
    class Token
    {
        public byte kind;
        public string spelling;

        public static byte
            IDENTIFIER = 0, NUMBER = 1, OPERATOR = 2, SEMICOLON = 3, COLON = 4, LPAREN = 5, RPAREN = 6, BECOMES = 7, EOL = 8, EOT = 9;

        public static string[] spellings = 
        {
            "<identifier>", "<number>", "<operator>", ";", ":", "(", ")", "=", "<EOL>", "<EOT>"                           
        };

        public Token(byte kind, string spelling)
        {
            this.kind = kind; 
            this.spelling = spelling;

            if (kind == IDENTIFIER)
            {
                for (int i = SEMICOLON; i <= RPAREN; i++)
                {
                    if (spelling.Equals(spellings[i]))
                    {
                        this.kind = (byte)i;
                        break;
                    }
                }
            }
        }
    }
}
