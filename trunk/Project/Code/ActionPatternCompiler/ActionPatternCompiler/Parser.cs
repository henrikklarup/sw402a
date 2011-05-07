﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionPatternCompiler
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

        #region Accept
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
        #endregion

        /// <summary>
        /// Parse the tokens into an abstract syntax tree.
        /// </summary>
        public AST parse()
        {
            currentToken = scanner.scan();
            
            // the first token in an actionpattern should always be "unit".
            accept(Token.keywords.UNIT);

            AST ast = parseMove_Option();
            
            if (throwException)
            { throw gException; }
            
            return ast;
        }
        
        private MASNumber parseMASNumber()
        { 
            MASNumber num;
            if (currentToken.kind == (int)Token.keywords.NUMBER)
            { 
                num = new MASNumber();
                num.ID = currentToken;
                acceptIt();
                return num;
            }
            else
            {
                throwException = true;
                gException.containedExceptions.Add(new GrammarException(
                    "Token " +
                    (Token.keywords)currentToken.kind + " is not a valid number.", currentToken));
            }
            return null;
        }

        private Move_Option parseMove_Option()
        {
            Move_Option move_option;
            accept(Token.keywords.MOVE);

            switch(currentToken.kind)
            {
                case (int)Token.keywords.DOWN:
                case (int)Token.keywords.UP:
                case (int)Token.keywords.LEFT:
                case (int)Token.keywords.RIGHT:
                case (int)Token.keywords.HOLD:
                    move_option = new Move_Option();
                    move_option.dir_coord = parseDirection();
                    break;
                case (int)Token.keywords.NUMBER:
                    move_option = new Move_Option();
                    move_option.dir_coord = parseCoordinate();
                    break;
                case (int)Token.keywords.IDENTIFIER:
                    move_option = new Move_Option();
                    move_option.dir_coord = parseIdentifier();
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

        private Identifier parseIdentifier()
        {
            Identifier ident;
            if (currentToken.kind == (int)Token.keywords.IDENTIFIER)
            {
                ident = new Identifier();
                ident.name = currentToken;
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

        private Direction parseDirection()
        {
            Direction dir = new Direction();
            dir.dir = currentToken;
            acceptIt();
            return dir;
        }
        
        /// <summary>
        /// Accepts the coordinate definition num,num
        /// </summary>
        /// <returns>Coordinate</returns>
        private Coordinate parseCoordinate()
        {
            Coordinate coord;
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

                    coord = new Coordinate();
                    coord.num1 = num1;
                    coord.num2 = num2;
                    return coord;
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