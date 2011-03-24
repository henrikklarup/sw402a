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
        public void parse()
        {
            parseMainblock();
        }

        /// <summary>
        /// Parses a mainblock token, and is executed as the first parse.
        /// </summary>
        private void parseMainblock()
        {
            switch(currentToken.kind)
            {
                case (int)Token.keywords.MAIN:
                    acceptIt();
                    accept(Token.keywords.LPAREN);
                    accept(Token.keywords.RPAREN);
                    parseBlock();
                    break;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    break;
            }
        }

        /// <summary>
        /// Method for parsing a block.
        /// </summary>
        private void parseBlock()
        {
            // parseCommand is run until the end of the block is reached.
            switch (currentToken.kind)
            {
                case (int)Token.keywords.LBRACKET:
                    acceptIt();
                    while (currentToken.kind != (int)Token.keywords.RBRACKET)
                    {
                        parseCommand();
                    }
                    acceptIt();
                    break;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    break;
            }
        }

        /// <summary>
        /// Method for parsing a command.
        /// </summary>
        private void parseCommand()
        {
            switch (currentToken.kind)
            {
                    // The command can be a block...
                case (int)Token.keywords.LBRACKET:
                    parseBlock();
                    break;
                    // or an object declaration...
                case (int)Token.keywords.NEW:
                    parseObjectDeclaration();
                    accept(Token.keywords.SEMICOLON);
                    break;
                    // or an if-sentence...
                case (int)Token.keywords.IF_LOOP:
                    parseIfCommand();
                    break;
                    // for loop...
                case (int)Token.keywords.FOR_LOOP:
                    parseForCommand();
                    break;
                    // while loop...
                case (int)Token.keywords.WHILE_LOOP:
                    parseWhileCommand();
                    break;
                    // type declaration...
                case (int)Token.keywords.NUM:
                case (int)Token.keywords.STRING:
                case (int)Token.keywords.BOOL:
                    parseTypeDeclaration();
                    accept(Token.keywords.SEMICOLON);
                    break;
                    // or expression or method call.
                case (int)Token.keywords.IDENTIFIER:
                    /* If the next token is an operator, this is an expression. 
                     * Else it's a method call. */
                    if (tokenList.ElementAt(listCount + 1).kind ==
                        (int)Token.keywords.OPERATOR)
                    {
                        parseExpression();
                    }
                    else
                    {
                        parseMethodCall();
                    }
                    accept(Token.keywords.SEMICOLON);
                    break;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    break;
            }
        }

        /// <summary>
        /// Method for parsing an object declaration.
        /// </summary>
        private void parseObjectDeclaration()
        {
            /* As the current token will already have been checked,
             * we can just accept it. */
            accept(Token.keywords.NEW);
            parseObject();
            parseIdentifier();
            accept(Token.keywords.LPAREN);
            parseInput();
            accept(Token.keywords.RPAREN);
        }

        /// <summary>
        /// Method for parsing an object.
        /// </summary>
        private void parseObject()
        {
            switch (currentToken.kind)
            {
                    // If the token represents an object, accept.
                case (int)Token.keywords.TEAM:
                case (int)Token.keywords.AGENT:
                case (int)Token.keywords.SQUAD:
                case (int)Token.keywords.COORDINATES:
                    acceptIt();
                    break;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    break;
            }
        }

        /// <summary>
        /// Method for parsing a type.
        /// </summary>
        private void parseType()
        {
            switch (currentToken.kind)
            {
                case (int)Token.keywords.BOOL:
                case (int)Token.keywords.NUM:
                case (int)Token.keywords.STRING:
                    acceptIt();
                    break;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    break;
            }
        }

        /// <summary>
        /// Method for parsing an if command.
        /// </summary>
        private void parseIfCommand()
        {
            accept(Token.keywords.IF_LOOP);
            accept(Token.keywords.LPAREN);
            parseExpression();
            accept(Token.keywords.RPAREN);
            parseBlock();
            // Check for an else-statement and react accordingly.
            switch (currentToken.kind)
            {
                case (int)Token.keywords.ELSE_LOOP:
                    acceptIt();
                    parseBlock();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method for parsing a for-loop.
        /// </summary>
        private void parseForCommand()
        {
            accept(Token.keywords.FOR_LOOP);
            accept(Token.keywords.LPAREN);
            parseTypeDeclaration();
            accept(Token.keywords.SEMICOLON);
            parseExpression();
            accept(Token.keywords.SEMICOLON);
            parseExpression();
            accept(Token.keywords.RPAREN);
            parseBlock();
        }

        /// <summary>
        /// Method for parsing a while loop.
        /// </summary>
        private void parseWhileCommand()
        {
            accept(Token.keywords.WHILE_LOOP);
            accept(Token.keywords.LPAREN);
            parseExpression();
            accept(Token.keywords.RPAREN);
            parseBlock();
        }

        /// <summary>
        /// Method for parsing a type declaration.
        /// </summary>
        private void parseTypeDeclaration()
        {
            parseType();
            parseIdentifier();
            accept(Token.keywords.BECOMES);
            switch (currentToken.kind)
            {
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                case (int)Token.keywords.NUMBER:
                case (int)Token.keywords.ACTUAL_STRING:
                    acceptIt();
                    break;
                case (int)Token.keywords.IDENTIFIER:
                    parseIdentifier();
                    break;
                default:
                    accept(Token.keywords.ERROR);
                    break;
            }
        }

        /// <summary>
        /// Method for parsing a method call.
        /// </summary>
        private void parseMethodCall()
        {
            parseIdentifier();
            // If there is a punctuation, dot your way through.
            while (currentToken.kind == (int)Token.keywords.PUNCTUATION)
            {
                acceptIt();
                parseIdentifier();
            }
            accept(Token.keywords.LPAREN);
            parseInput();
            accept(Token.keywords.RPAREN);
        }

        /// <summary>
        /// Method for parsing an expression (unfinished).
        /// </summary>
        private void parseExpression()
        {
            
        }

        /// <summary>
        /// Method for parsing an identifier (unfinished).
        /// </summary>
        private void parseIdentifier()
        {
            // spelling
        }

        /// <summary>
        /// Method for parsing an operator.
        /// </summary>
        private void parseOperator()
        {
            switch (currentToken.kind)
            {
                case (int)Token.keywords.OPERATOR:
                    acceptIt();
                    break;
                default:
                    accept(Token.keywords.ERROR);
                    break;
            }
        }

        /// <summary>
        /// Method for parsing method input.
        /// </summary>
        private void parseInput()
        {
            switch (currentToken.kind)
            {
                case (int)Token.keywords.IDENTIFIER:
                case (int)Token.keywords.NUMBER:
                case (int)Token.keywords.ACTUAL_STRING:
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                    acceptIt();
                    break;
                case (int)Token.keywords.NEW:
                    parseObjectDeclaration();
                    break;
            }
            // Input variables are seperated by comma.
            while (currentToken.kind == (int)Token.keywords.COMMA)
            {
                acceptIt();
                switch (currentToken.kind)
                {
                    case (int)Token.keywords.IDENTIFIER:
                    case (int)Token.keywords.NUMBER:
                    case (int)Token.keywords.ACTUAL_STRING:
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                        acceptIt();
                        break;
                    case (int)Token.keywords.NEW:
                        parseObjectDeclaration();
                        break;
                }
            }
        }

        /// <summary>
        /// Checks if the kind of the current token matches the expected value, 
        /// it prints an error message is that is not the case.
        /// </summary>
        /// <param name="kind">The token kind to check against</param>
        private void accept(Token.keywords kind)
        {
            if (kind == Token.keywords.ERROR)
            {
                Console.WriteLine("ERROR at line " + currentToken.row + " col " + currentToken.col);
            }
            else if ((int)kind == currentToken.kind)
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
