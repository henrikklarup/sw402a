using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Token
    {
        public byte kind;
        public string spelling;

        //The number identifying the token, as a byte to make sure it isn't misinterpreted
        public static byte
            IDENTIFIER = 0, NUMBER = 1, OPERATOR = 2, SEMICOLON = 3, COLON = 4, LPAREN = 5, RPAREN = 6, BECOMES = 7, LBRACKET = 8, RBRACKET = 9, IF_LOOP = 10, FOR_LOOP = 11, EOL = 12, EOT = 13;

        //The equivilint spellings of the tokens, used by the Token method to change the identifier from identifier to a keyword
        public static string[] spellings = 
        {
            "<identifier>", "<number>", "<operator>", ";", ":", "(", ")", "=", "{", "}", "if", "for", "<EOL>", "<EOT>"                           
        };

        //Converting the string of the identifier to a token if any keyword matches the string
        public Token(byte kind, string spelling)
        {
            this.kind = kind; 
            this.spelling = spelling.ToLower();

            if (kind == IDENTIFIER)
            {
                for (int i = LPAREN; i <= BECOMES; i++)
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
