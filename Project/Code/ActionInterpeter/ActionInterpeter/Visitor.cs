using System;
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
        
        internal object visitAction(Action action, object arg)
        {
            action.single_action.visit(this, arg);
            return null;
        }

        internal object visitSingle_Action(Single_Action single_Action, object arg)
        {
            object selection = single_Action.selection.visit(this, arg);
            object moveOption = single_Action.move_option.visit(this, arg);

            // If the selection is an agent, set the type of the selection.
            if (object.ReferenceEquals(single_Action.selection.GetType(), new AgentID().GetType()))
            {
                single_Action.type = (int)Token.keywords.AGENT;
            }
                // try with Team.
            else if (object.ReferenceEquals(single_Action.selection.GetType(), new TeamID().GetType()))
            {
                single_Action.type = (int)Token.keywords.TEAM;
            }
                // and squad.
            else if (object.ReferenceEquals(single_Action.selection.GetType(), new SquadID().GetType()))
            {
                single_Action.type = (int)Token.keywords.SQUAD;
            }
            else if (object.ReferenceEquals(single_Action.selection.GetType(), new Agent().GetType()))
            {
                single_Action.type = (int)Token.keywords.AGENT;
            }
            else if (object.ReferenceEquals(single_Action.selection.GetType(), new Squad().GetType()))
            {
                single_Action.type = (int)Token.keywords.SQUAD;
            }
            else if (object.ReferenceEquals(single_Action.selection.GetType(), new Team().GetType()))
            {
                single_Action.type = (int)Token.keywords.TEAM;
            }
            else
            {
                throw new GrammarException("Couldn't match the selected unit.");
            }

            #region old
            //// If the identifier is an agent.
            //if (Type.ReferenceEquals(selection.GetType(), new Agent().GetType()))
            //{
            //    Agent agent = (Agent)selection;

            //    int num1;
            //    int num2;

            //    // If the option is a coordinate.
            //    Coordinate coord = new Coordinate();
            //    if (Type.ReferenceEquals(moveOption.GetType(), coord.GetType()))
            //    {
            //        coord = (Coordinate)moveOption;

            //        num1 = Convert.ToInt16(coord.num1.spelling);
            //        num2 = Convert.ToInt16(coord.num2.spelling);
            //    }
            //    else
            //    {
            //        num1 = agent.posX;
            //        num2 = agent.posY;

            //        Token token = (Token)moveOption;
            //        switch (token.spelling.ToLower())
            //        { 
            //            case "up":
            //                num2--;
            //                break;
            //            case "down":
            //                num2++;
            //                break;
            //            case "left":
            //                num1--;
            //                break;
            //            case "right":
            //                num1++;
            //                break;
            //            case "hold":
            //                break;
            //        }
            //    }

            //    Functions.moveAgent((Agent)ident, num1, num2);
            //}
            #endregion
            return null;
        }

        internal object visitMove_Option(Move_Option move_Option, object arg)
        {
            object dir_coord = move_Option.dir_coord.visit(this, arg);

            if (dir_coord != null)
            {
                // If the direction or coordinate is a direction.
                if (object.ReferenceEquals(dir_coord.GetType(), new Direction().GetType()))
                {
                    move_Option.type = (int)Token.keywords.DIR;
                }
                else if (object.ReferenceEquals(dir_coord.GetType(), new Coordinate().GetType()))
                {
                    move_Option.type = (int)Token.keywords.COORD;
                }
                else if (object.ReferenceEquals(dir_coord.GetType(), new ActionPattern().GetType()))
                {
                    move_Option.type = (int)Token.keywords.ACTIONPATTERN;
                }
            }
            else
            {
                throw new GrammarException("An invalid move options was chosen.");
            }
            return null;
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            //Check if this identifier exists
            Token token = identifier.name;
            if( token.kind == (int)Token.keywords.IDENTIFIER)
            {
                object obj = null;
                obj = Lists.RetrieveAgent(token.spelling);
                if (obj != null)
                    return obj;
                obj = Lists.RetrieveSquad(token.spelling);
                if (obj != null)
                    return obj;
                obj = Lists.RetrieveTeam(token.spelling);
                if (obj != null)
                    return obj;
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

        internal object visitMASNumber(MASNumber mASNumber, object arg)
        {
            return mASNumber.ID;
        }

        #region Agent/Team/Squad ID
        internal object visitAgentID(AgentID agentID, object arg)
        {
            return agentID;
        }

        internal object visitTeamID(TeamID teamID, object arg)
        {
            return teamID;
        }

        internal object visitSquadID(SquadID squadID, object arg)
        {
            return squadID;
        }
        #endregion

        internal object visitDirection(Direction direction, object arg)
        {
            return direction.dir;
        }
    }
}
