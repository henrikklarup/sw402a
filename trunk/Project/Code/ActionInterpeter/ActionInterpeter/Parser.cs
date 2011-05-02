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

        // Exception for catching errors.
        private GrammarException gException = new GrammarException("These errors were found by the parser:");
        private bool throwException = false;

        /// <summary>
        /// Checks if the kind of the current token matches the expected value, 
        /// it prints an error message is that is not the case.
        /// </summary>
        /// <param name="kind">The token kind to check against</param>
        private void accept(Token.keywords kind)
        {
            if ((int)kind != currentToken.kind)
            {
                Printer.ErrorMarker();
                throwException = true;
                gException.containedExceptions.Add(new GrammarException(
                    "Token " + (Token.keywords)currentToken.kind +
                    " was not legal. \n  A token of kind " + kind + " was expected.", currentToken));
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
            
            if (throwException)
            { throw gException; }
            
            return ast;
        }

        private ActionAST parseAction()
        {
            ActionAST actionAST;
            actionAST = parseSingle_Action();

            return actionAST;
        }

        private Identifier parseIdentifier()
        {
            Identifier ident;
            if (currentToken.kind == (int)Token.keywords.IDENTIFIER)
            {
                ident = new Identifier(currentToken);
                acceptIt();
                return ident;
            }
            else
            {
                throwException = true;
                gException.containedExceptions.Add(new GrammarException(
                    "Token " +
                    (Token.keywords)currentToken.kind + " is not a valid identifier.", currentToken));
            }
            return null;
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
                    throwException = true;
                    gException.containedExceptions.Add(new GrammarException(
                        "Token " + 
                        currentToken.spelling + " is not valid for a command.", currentToken));
                    return null;
            }
        }

        private Move_Action parseMove_Action()
        {
            Move_Action move_action;           
            move_action = new Move_Action(parseMove_Option());
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
                    acceptIt();
                    break;
                case (int)Token.keywords.NUMBER:
                    move_option = new Move_Option(parseCoordinate());
                    break;
                default:
                    throwException = true;
                    gException.containedExceptions.Add(new GrammarException(
                        "Token " + 
                        (Token.keywords)currentToken.kind + " is not valid for an option.", currentToken));
                    return null;
            }
            return move_option;
        }

        private Coordinate parseCoordinate()
        {
            switch(currentToken.kind)
            {
                case (int)Token.keywords.NUMBER:
                    // Accept the first token, since its a number.
                    Token num1 = currentToken;
                    acceptIt();

                    // Accept the token if its a comma.
                    accept(Token.keywords.COMMA);

                    // Accept the next token, if its a number.
                    Token num2 = currentToken;
                    accept(Token.keywords.NUMBER);

                    return new Coordinate(num1, num2);
                default:
                   throwException = true;
                    gException.containedExceptions.Add(new GrammarException(
                        "Token " + 
                        (Token.keywords)currentToken.kind + " is not a valid coordinate.", currentToken));
                    return null;
            }

            
        }
    }
}
