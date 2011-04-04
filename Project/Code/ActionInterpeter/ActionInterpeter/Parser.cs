using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    class Parser
    {
        //Initialize the scanner.
        Scanzor scanner = new Scanzor();

        // Holds the current token being checked.
        private Token currentToken;

        /// <summary>
        /// Checks if the kind of the current token matches the expected value, 
        /// it prints an error message is that is not the case.
        /// </summary>
        /// <param name="kind">The token kind to check against</param>
        private void accept(Token.keywords kind)
        {
            if (kind == Token.keywords.ERROR || (int)kind != currentToken.kind)
            {
                Console.WriteLine("ERROR at line " + currentToken.row + " col " + currentToken.col +
                    ". The recieved token of kind " + (Token.keywords)currentToken.kind + " was not legal.");
                Console.ReadKey();
            }

            if (currentToken.kind != (int)Token.keywords.EOT)
            {
                acceptIt();
            }
        }

        /// <summary>
        /// Accepts the current token and updates it to take the next token.
        /// </summary>
        private void acceptIt()
        {
            currentToken = scanner.scan();
        }

        /// <summary>
        /// Parse the tokens into an abstract syntax tree.
        /// </summary>
        public AST parse()
        {
            currentToken = scanner.scan();
            AST ast = parseAction();
            if (currentToken.kind != (int)Token.keywords.EOT)
            {
                Console.WriteLine("There has been an error!");
            }
            return ast;
        }

        private Action parseAction()
        {
            Action actionAST;
            actionAST = parseSingle_Action();

            return actionAST;
        }

        private Identifier parseIdentifier()
        {
            Identifier identifierAST;
            if (currentToken.kind == (int)Token.keywords.IDENTIFIER)
            {
                identifierAST = new Identifier(currentToken);
                currentToken = scanner.scan();
            }
            else
            {
                Console.WriteLine("Something went wrong when trying to parse an Identifier");
                identifierAST = new Identifier(new Token((int)Token.keywords.EOT, "FAIL", 0, 0));
            }
            return identifierAST;
        }

        private Single_Action parseSingle_Action()
        {
            Identifier identifier = parseIdentifier();
            
            switch (currentToken.kind)
            {
                case (int)Token.keywords.MOVE:
                    acceptIt();
                    Move_Action move_action = parseMove_Action();
                    return new Single_Action(identifier, move_action);
                default:
                    Console.WriteLine("Something went wrong when trying to parse a single action.");
                    return null;
            }
        }

        private Move_Action parseMove_Action()
        {
            Move_Action move_action;
            accept(Token.keywords.LPAREN);                
            move_action = new Move_Action(parseMove_Option());
            accept(Token.keywords.RPAREN);
            return move_action;
        }

        private Move_Option parseMove_Option()
        {
            Move_Option move_option;
            switch(currentToken.kind)
            {
                case (int)Token.keywords.DOWN:
                case (int)Token.keywords.UP:
                case (int)Token.keywords.LEFT:
                case (int)Token.keywords.RIGHT:
                case (int)Token.keywords.HOLD:
                    move_option = new Move_Option(currentToken);
                    currentToken = scanner.scan();
                    break;
                case (int)Token.keywords.NUMBER:
                    move_option = new Move_Option(parseCoordinate());
                    break;
                default:
                    Console.WriteLine("Something is wrong with the move option.");
                    return null;
            }
            return move_option;
        }

        private Coordinate parseCoordinate()
        {
            switch(currentToken.kind)
            {
                case (int)Token.keywords.NUMBER:
                    Token num1 = currentToken;
                    scanner.scan();
                    Token num2 = currentToken;
                    scanner.scan();
                    return new Coordinate(num1, num2);
                default:
                    Console.WriteLine("Something went wrong when trying to parse a coordinate.");
                    return null;
            }

            
        }
    }
}
