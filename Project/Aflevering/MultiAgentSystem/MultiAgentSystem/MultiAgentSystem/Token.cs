using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASSIVE
{
    public class Token
    {
        public int kind;        // The kind of the token.
        public string spelling; // The spelling of the token.
        public int row, col;    // Stores the row and column the token was found at.

        /// <summary>
        /// All possible kinds of tokens.
        /// </summary>
        public enum keywords
        { 
            IDENTIFIER,
            NUMBER,
            OPERATOR,
            ACTUAL_STRING,
            SEMICOLON, 
            COLON, 
            LPAREN,
            RPAREN,
            BECOMES,
            LBRACKET,
            RBRACKET,
            IF_LOOP,
            ELSE_LOOP,
            FOR_LOOP,
            WHILE_LOOP,
            BOOL,
            NEW,
            MAIN,
            TEAM,
            AGENT,
            SQUAD,
            COORDINATES,
            VOID,
            ACTION_PATTERN,
            NUM,
            STRING,
            TRUE,
            FALSE,
            COMMA,
            PUNCTUATION,
            EOL,
            EOT,
            ERROR,
        }

        /// <summary>
        /// The equivilint spellings of the tokens, used by the Token method to change the identifier from identifier to a keyword
        /// </summary>
        public static string[] spellings = 
        {
            "<identifier>", "<number>", "<operator>", "<string>", "; ", ":", "(", ")", "=", "{", "}", 
            "if", "else", "for", "while", "bool", "new", "main", "team", "agent", "squad", "coord", "void", 
            "actionpattern", "num", "string", "true", "false", ",", ".", "<EOL>", "<EOT>", "<ERROR>"                         
        };

        /// <summary>
        /// Converting the string of the identifier to a token if any keyword matches the string
        /// </summary>
        /// <param name="kind">Kind of the current word</param>
        /// <param name="spelling">The string</param>
        /// <param name="row">Line, where the token was found.</param>
        /// <param name="col">Column where the token began.</param>
        public Token(int kind, string spelling, int row, int col)
        {
            this.kind = kind;
            this.spelling = spelling;
            this.row = row;
            this.col = col;

            if (kind == (int)keywords.IDENTIFIER)
            {
                for (int i = (int)keywords.IF_LOOP; i <= (int)keywords.FALSE; i++)
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
