using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Parser
    {
        // Holds the current token being checked.
        private Token currentToken;

        private List<Token> tokenList;
        private int listCount = 0;
        private AST abstractSyntaxTree = new AST();

        public static const int
            IDENTIFIER = 0, NUMBER = 1, OPERATOR = 2, STRING = 3, SEMICOLON = 4, COLON = 5, LPAREN = 6,
            RPAREN = 7, BECOMES = 8, LBRACKET = 9, RBRACKET = 10, IF_LOOP = 11,
            FOR_LOOP = 12, BOOL = 13, NEW = 14, MAIN = 15,
            TEAM = 16, AGENT = 17, SQUAD = 18, VOID = 19, ACTION_PATTERN = 20,
            NUM = 21, TRUE = 22, FALSE = 23, COMMA = 24, PUNCTUATION = 25,
            EOL = 26, EOT = 27, ERROR = 28;

        public Parser(List<Token> list)
        {
            tokenList = list;
            currentToken = tokenList.ElementAt(listCount);
        }

        private AST parse()
        {
            abstractSyntaxTree.main = parseMainblock();

            return abstractSyntaxTree;
        }

        private Mainblock parseMainblock()
        {
            Mainblock mainBlock = new Mainblock();
            switch(currentToken.kind)
            {
                case MAIN:
                    acceptIt();
                    accept(Token.LPAREN);
                    accept(Token.RPAREN);
                    parseBlock();
                    return mainBlock;
                default:
                    accept(-1);
                    return null;
            }
        }

        private void accept(int kind)
        {
            if (kind == currentToken.kind)
            {
                UpdateToken();
            }
            else
            {
                Console.WriteLine("ERROR at line " + currentToken.row + " col " + currentToken.col);
            }
        }

        private void acceptIt() 
        {
            UpdateToken();
        }

        private void UpdateToken()
        {
            listCount++;
            currentToken = tokenList.ElementAt(listCount);
        }
    }
}
