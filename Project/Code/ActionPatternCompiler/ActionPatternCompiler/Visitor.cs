﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;
using ActionPatternCompiler;

namespace ActionPatternCompiler
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
        
        /// <summary>
        /// Vists a single action
        /// </summary>
        /// <param name="single_Action">Any Single Action</param>
        /// <param name="arg">Any object</param>
        /// <returns>null</returns>
        internal object visitSingle_Action(Single_Action single_Action, object arg)
        {
            Token stance = single_Action.stance.stance;

            #region move_option behaviour
            switch (stance.kind)
            {
                case (int)Token.keywords.MOVE:
                    single_Action.move_option.stance = (int)Stance.Stances.MOVE;
                    break;
                case (int)Token.keywords.ENCOUNTER:
                    single_Action.move_option.stance = (int)Stance.Stances.ENCOUNTER;
                    break;
            }
            #endregion

            // Gets the move option (e.g. coordinate) and stores it in the move_option variable.
            single_Action.move_option = (Move_Option)single_Action.move_option.visit(this, arg);

            return null;
        }
        
        /// <summary>
        /// Execute the moveagent method.
        /// </summary>
        /// <param name="agent">The agent which is being moved.</param>
        /// <param name="move_Option">Move option from the single_action</param>
        private void visitCodeGen_MoveOption(Move_Option move_Option)
        {
            int num1;
            int num2;

            switch (move_Option.type)
            {
                case (int)Type.Types.DIR:
                    Direction dir = (Direction)move_Option.dir_coord;
                    num1 = ActionPattern.thisAgent.posx;
                    num2 = ActionPattern.thisAgent.posy;

                    Token token = dir.dir;
                    switch (token.spelling.ToLower())
                    {
                        case "up":
                        case "down":
                        case "left":
                        case "right":
                            foreach (agent a in Lists.moveagents)
                            {
                                if (a.id == ActionPattern.thisAgent.id)
                                {
                                    num1 = a.posx;
                                    num2 = a.posy;
                                }
                            }
                            break;
                    }

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
                            Lists.moveagents.RemoveAll(s => s.id == ActionPattern.thisAgent.id);
                            break;
                    }
                    break;
                case (int)Type.Types.COORD:
                    Coordinate coord = (Coordinate)move_Option.dir_coord;

                    num1 = Convert.ToInt16(coord.num1.spelling);
                    num2 = Convert.ToInt16(coord.num2.spelling);
                    break;
                case (int)Type.Types.ACTIONPATTERN:
                    object moveOption = move_Option.dir_coord.visit(this, null);
                    // If there was no actionpattern with this name, Exception.
                    if (moveOption == null || !object.ReferenceEquals(moveOption.GetType(), new actionpattern().GetType()))
                    {
                        throw new InvalidMoveOptionException("The actionpattern was invalid!");
                    }
                    actionpattern ap = (actionpattern)moveOption;
                    foreach (string s in ap.actions)
                    {
                        ActionPattern.Compile(s, ActionPattern.thisAgent);
                    }
                    return;
                default:
                    throw new InvalidMoveOptionException("The move Option was invalid!");
            }

            switch (move_Option.stance)
            {
                case (int)Stance.Stances.MOVE:
                    Functions.moveagent(ActionPattern.thisAgent, num1, num2);
                    break;
                case (int)Stance.Stances.ENCOUNTER:
                    throw new NotImplementedException();
                    break;
            }
        }


        internal object visitMove_Option(Move_Option move_Option, object arg)
        {
            object dir_coord = move_Option.dir_coord.visit(this, arg);

            if (dir_coord != null)
            {
                // If the direction or coordinate is a direction.
                if (object.ReferenceEquals(move_Option.dir_coord.GetType(), new Direction().GetType()))
                {
                    move_Option.type = (int)Type.Types.DIR;
                }
                else if (object.ReferenceEquals(move_Option.dir_coord.GetType(), new Coordinate().GetType()))
                {
                    move_Option.type = (int)Type.Types.COORD;
                }
                else if (object.ReferenceEquals(move_Option.dir_coord.GetType(), new Identifier().GetType()))
                {
                    move_Option.type = (int)Type.Types.ACTIONPATTERN;
                }
            }
            else
            {
                throw new InvalidMoveOptionException("An invalid move options was chosen.");
            }

            visitCodeGen_MoveOption(move_Option);
            return null;
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            //Check if this identifier exists
            Token token = identifier.name;
            if (token.kind == (int)Token.keywords.IDENTIFIER)
            {
                object obj = null;
                obj = Lists.RetrieveActionPattern(token.spelling);
                if (obj != null)
                    return obj;
            }
            return null;
        }

        internal object visitCoordinate(Coordinate coordinate, object arg)
        {
            Token firstNum = coordinate.num1;
            Token secondNum = coordinate.num2;

            if (firstNum.kind != (int)Token.keywords.NUMBER)
            {
                gException.containedExceptions.Add(
                    new GrammarException(
                        firstNum.spelling +
                        " is not valid input for coordinates.", firstNum));
            }
            if (secondNum.kind != (int)Token.keywords.NUMBER)
            {
                gException.containedExceptions.Add(
                    new GrammarException(
                        secondNum.spelling +
                        " is not valid input for coordinates.", secondNum));
            }
            return coordinate;
        }

        internal object visitMASNumber(MASNumber mASNumber, object arg)
        {
            return mASNumber.ID;
        }

        internal object visitDirection(Direction direction, object arg)
        {
            return direction.dir;
        }
    }
}