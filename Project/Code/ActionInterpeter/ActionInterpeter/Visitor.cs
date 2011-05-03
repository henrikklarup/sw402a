﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace ActionInterpeter
{
    class Visitor
    {
        // Exception for catching errors.
        private GrammarException gException =
            new GrammarException("These errors were found during decoration:");

        public object visitAST(AST ast, object arg)
        {
            ast.visit(this, arg);
            return null;
        }

        internal object visitMainProgram(MainProgram mainProgram, object arg)
        {
            mainProgram.action.visit(this, null);
            return null;
        }

        internal object visitAction(Action action, object arg)
        {
            action.single_action.visit(this, null);
            return null;
        }

        internal object visitSingle_Action(Single_Action single_Action, object arg)
        {
            object ident = single_Action.identifier.visit(this, arg);
            object moveOption = single_Action.move_option.visit(this, arg);

            // If the identifier is an agent.
            if (Type.ReferenceEquals(ident.GetType(), new Agent().GetType()))
            {
                Agent agent = (Agent)ident;

                int num1;
                int num2;

                // If the option is a coordinate.
                Token token = new Token(0,"");
                Coordinate coord = new Coordinate(token, token);
                if (Type.ReferenceEquals(moveOption.GetType(), coord.GetType()))
                {
                    coord = (Coordinate)moveOption;

                    num1 = Convert.ToInt16(coord.num1.spelling);
                    num2 = Convert.ToInt16(coord.num2.spelling);
                }
                else
                {
                    num1 = agent.posX;
                    num2 = agent.posY;

                    token = (Token)moveOption;
                    switch (token.spelling.ToLower())
                    { 
                        case "up":
                            num2--;
                            break;
                        case "down":
                            num2++;
                            break;
                        case "left":
                            num1--;
                            break;
                        case "right":
                            num1++;
                            break;
                        case "hold":
                            break;
                    }
                }

                Functions.moveAgent((Agent)ident, num1, num2);
            }

            return null;
        }

        internal object visitMove_Option(Move_Option move_Option, object arg)
        {
            //If no direction is recorded, this must be a coordinate
            if (move_Option.direction == null)
            {
                return move_Option.coordinate.visit(this, arg);
            }
            else
            {
                return move_Option.direction;
            }
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            //Check if this identifier exists
            Token token = identifier.agent_Name_or_ID;
            if (token.kind == (int)Token.keywords.NUMBER)
            {
                Agent agent = null;
                try
                {
                    agent = Lists.RetrieveAgent(Convert.ToInt16(token.spelling));
                }
                catch (Exception e)
                { }
                if (agent != null)
                    return agent;
            }
            else if( token.kind == (int)Token.keywords.IDENTIFIER)
            {
                Agent agent = null;
                try
                {
                    agent = Lists.RetrieveAgent(token.spelling);
                }
                catch (Exception e)
                { }
                if(agent != null)
                    return agent;
            }
            return null;
        }

        internal object visitCoordinate(Coordinate coordinate, object arg)
        {
            Token firstNum = coordinate.num1;
            Token secondNum = coordinate.num2;

            if (firstNum.kind != (int)Token.keywords.NUMBER )
            {
                gException.containedExceptions.Add(
                    new GrammarException(
                        firstNum.spelling +
                        " is not valid input for coordinates.", firstNum));
            }
            if(secondNum.kind != (int)Token.keywords.NUMBER)
            {
                gException.containedExceptions.Add(
                    new GrammarException(
                        secondNum.spelling +
                        " is not valid input for coordinates.", secondNum));
            }
            return coordinate;
        }
    }
}