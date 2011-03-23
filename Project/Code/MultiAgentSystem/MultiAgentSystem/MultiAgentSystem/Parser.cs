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

        // Holds all the tokens produced by the scanner.
        private List<Token> tokenList;

        // Checks what token is being worked on currently.
        private int listCount = 0;

        /// <summary>
        /// Creates a new parser.
        /// </summary>
        /// <param name="list">A list of tokens produced by the scanner.</param>
        public Parser(List<Token> list)
        {
            tokenList = list;
            currentToken = tokenList.ElementAt(listCount);
        }

        /// <summary>
        /// Parse the tokens into an abstract syntax tree.
        /// </summary>
        /// <returns>Abstract Syntax Tree (AST)</returns>
        private Mainblock parse()
        {
            return parseMainblock();
        }

        /// <summary>
        /// Parses a mainblock token, and is executed as the first parse.
        /// </summary>
        /// <returns>A Mainblock instance.</returns>
        private Mainblock parseMainblock()
        {
            switch(currentToken.kind)
            {
                case (int)Token.keywords.MAIN:
                    acceptIt();
                    accept((int)Token.keywords.LPAREN);
                    accept((int)Token.keywords.RPAREN);
                    return new Mainblock(parseBlock());
                default:
                    accept(-1);
                    return null;
            }
        }

        private Block parseBlock()
        {
            return new Block();
        }

        /// <summary>
        /// Checks if the kind of the current token matches the expected value, 
        /// it prints an error message is that is not the case.
        /// </summary>
        /// <param name="kind">The token kind to check against</param>
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

        /// <summary>
        /// Accepts the current token and updates it to take the next token.
        /// </summary>
        private void acceptIt() 
        {
            UpdateToken();
        }

        /// <summary>
        /// Updates the current token to the next in the list.
        /// </summary>
        private void UpdateToken()
        {
            listCount++;
            currentToken = tokenList.ElementAt(listCount);
        }
    }
}
