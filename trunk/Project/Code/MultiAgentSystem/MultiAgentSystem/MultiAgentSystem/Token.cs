using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Token
    {
        public int kind;
        public string spelling;

        //Stores the row and column the token was found at
        public int row;
        public int col;

        //The number identifying the token
        public static int
            IDENTIFIER = 0, NUMBER = 1, OPERATOR = 2, STRING = 3, SEMICOLON = 4, COLON = 5, LPAREN = 6,
            RPAREN = 7, BECOMES = 8, LBRACKET = 9, RBRACKET = 10, IF_LOOP = 11, 
            FOR_LOOP = 12, BOOL = 13, NEW = 14, MAIN = 15,
            TEAM = 16, AGENT = 17, SQUAD = 18,  VOID = 19, ACTION_PATTERN = 20, 
            NUM = 21, TRUE = 22, FALSE = 23, COMMA = 24, PUNCTUATION = 25,
            EOL = 26, EOT = 27, ERROR = 28;

        //The equivilint spellings of the tokens, used by the Token method to change the identifier from identifier to a keyword
        public static string[] spellings = 
        {
            "<identifier>", "<number>", "<operator>", "<string>", ";", ":", "(", ")", "=", "{", "}", 
            "if", "for", "bool", "new", "main", "team", "squad", "void", 
            "actionpattern", "num", "true", "false", ",", ".", "<EOL>", "<EOT>", "<ERROR>"                         
        };

        //Converting the string of the identifier to a token if any keyword matches the string
        public Token(int kind, string spelling, int row, int col)
        {
            this.kind = kind;
            this.spelling = spelling;
            this.row = row;
            this.col = col;

            if (kind == IDENTIFIER)
            {
                for (int i = IF_LOOP; i <= FALSE; i++)
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
