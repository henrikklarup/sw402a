using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Parser
    {
        private Token currentToken;
        private List<Token> tokenList;
        private int listCount = 0;

        public Parser(List<Token> list)
        {
            tokenList = list;
            currentToken = tokenList.ElementAt(listCount);
        }

        private AST parse()
        {
            AST abstractSyntaxTree = new AST();
            parseMainblock();

            return abstractSyntaxTree;
        }

        private void parseMainblock()
        {
            switch(currentToken.kind)
            {
                case Token.MAIN:
                    acceptIt();
                    
                    break;
            }
        }

        private void accept(int kind)
        {

        }

        private void acceptIt() 
        { 
            listCount++;
            currentToken = tokenList.ElementAt(listCount);
        }
    }
}
