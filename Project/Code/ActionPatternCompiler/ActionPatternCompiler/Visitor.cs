using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

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

        internal object visitAction(Action action, object arg)
        {
            action.single_action.visit(this, arg);
            return null;
        }

        internal object visitSingle_Action(Single_Action single_Action, object arg)
        {
            object selection = single_Action.selection.visit(this, arg);
            single_Action.move_option = (Move_Option)single_Action.move_option.visit(this, arg);

            #region Compare selection type

            // If there is nothing stored in the selection, trow an exception.
            if (selection == null)
            {
                throw new GrammarException("Couldn't match the selected unit.");
            }
            // If the selection is an agent, set the type of the selection.
            else if (object.ReferenceEquals(
                single_Action.selection.GetType(), new agentID().GetType()))
            {
                single_Action.type = (int)Type.Types.AGENTID;
            }
            // try with team.
            else if (object.ReferenceEquals(
                single_Action.selection.GetType(), new teamID().GetType()))
            {
                single_Action.type = (int)Type.Types.TEAMID;
            }
            // and squad.
            else if (object.ReferenceEquals(
                single_Action.selection.GetType(), new squadID().GetType()))
            {
                single_Action.type = (int)Type.Types.SQUADID;
            }
            else if (object.ReferenceEquals(
                selection.GetType(), new agent().GetType()))
            {
                single_Action.type = (int)Type.Types.AGENT;
            }
            else if (object.ReferenceEquals(
                selection.GetType(), new squad().GetType()))
            {
                single_Action.type = (int)Type.Types.SQUAD;
            }
            else if (object.ReferenceEquals(
                selection.GetType(), new team().GetType()))
            {
                single_Action.type = (int)Type.Types.TEAM;
            }
            #endregion

            switch (single_Action.type)
            {
                case (int)Type.Types.AGENTID:
                case (int)Type.Types.AGENT:
                    visitCodeGen_MoveAgent(single_Action, arg);
                    break;
                case (int)Type.Types.TEAMID:
                case (int)Type.Types.TEAM:
                    visitCodeGen_MoveTeam(single_Action, arg);
                    break;
                case (int)Type.Types.SQUADID:
                case (int)Type.Types.SQUAD:
                    visitCodeGen_MoveSquad(single_Action, arg);
                    break;
            }
            return null;
        }

        private void visitCodeGen_MoveAgent(Single_Action single_Action, object arg)
        {
            agent agent;
            if (single_Action.type == (int)Type.Types.AGENTID)
            {
                agentID select = (agentID)single_Action.selection;
                Token selectToken = select.num;
                agent = Lists.RetrieveAgent(Convert.ToInt32(selectToken.spelling));
            }
            else
            {
                Identifier ident = (Identifier)single_Action.selection;
                agent = Lists.RetrieveAgent(ident.name.spelling);
            }

            visitCodeGen_MoveOption(agent, single_Action.move_option);
        }

        private void visitCodeGen_MoveTeam(Single_Action single_Action, object arg)
        {
            List<agent> agents;
            team team;
            if (single_Action.type == (int)Type.Types.TEAMID)
            {
                teamID select = (teamID)single_Action.selection;
                Token selectToken = select.num;
                team = Lists.RetrieveTeam(Convert.ToInt32(selectToken.spelling));
            }
            else
            {
                Identifier ident = (Identifier)single_Action.selection;
                team = Lists.RetrieveTeam(ident.name.spelling);
            }

            agents = Lists.RetrieveAgentsByteam(team);
            foreach (agent a in agents)
            {
                visitCodeGen_MoveOption(a, single_Action.move_option);
            }
        }

        private void visitCodeGen_MoveSquad(Single_Action single_Action, object arg)
        {
            squad squad;
            if (single_Action.type == (int)Type.Types.SQUADID)
            {
                squadID select = (squadID)single_Action.selection;
                Token selectToken = select.num;
                squad = Lists.RetrieveSquad(Convert.ToInt32(selectToken.spelling));
            }
            else
            {
                Identifier ident = (Identifier)single_Action.selection;
                squad = Lists.RetrieveSquad(ident.name.spelling);
            }

            foreach (agent a in squad.Agents)
            {
                visitCodeGen_MoveOption(a, single_Action.move_option);
            }
        }


        /// <summary>
        /// Execute the moveagent method.
        /// </summary>
        /// <param name="agent">The agent which is being moved.</param>
        /// <param name="move_Option">Move option from the single_action</param>
        private void visitCodeGen_MoveOption(agent agent, Move_Option move_Option)
        {
            int num1;
            int num2;

            switch (move_Option.type)
            {
                case (int)Type.Types.DIR:
                    Direction dir = (Direction)move_Option.dir_coord;
                    num1 = agent.posx;
                    num2 = agent.posy;

                    Token token = dir.dir;
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
                    break;
                case (int)Type.Types.COORD:
                    Coordinate coord = (Coordinate)move_Option.dir_coord;

                    num1 = Convert.ToInt16(coord.num1.spelling);
                    num2 = Convert.ToInt16(coord.num2.spelling);
                    break;
                case (int)Type.Types.ACTIONPATTERN:
                    throw new NotImplementedException();
                default:
                    throw new Exception("The move Option was invalid!");
            }
            Functions.moveagent(agent, num1, num2);
        }


        internal object visitMove_Option(Move_Option move_Option, object arg)
        {
            object dir_coord = move_Option.dir_coord;

            if (dir_coord != null)
            {
                // If the direction or coordinate is a direction.
                if (object.ReferenceEquals(dir_coord.GetType(), new Direction().GetType()))
                {
                    move_Option.type = (int)Type.Types.DIR;
                }
                else if (object.ReferenceEquals(dir_coord.GetType(), new Coordinate().GetType()))
                {
                    move_Option.type = (int)Type.Types.COORD;
                }
                else if (object.ReferenceEquals(dir_coord.GetType(), new actionpattern().GetType()))
                {
                    move_Option.type = (int)Type.Types.ACTIONPATTERN;
                }
            }
            else
            {
                throw new GrammarException("An invalid move options was chosen.");
            }
            return move_Option;
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            //Check if this identifier exists
            Token token = identifier.name;
            if (token.kind == (int)Token.keywords.IDENTIFIER)
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

        #region agent/team/Squad ID
        internal object visitagentID(agentID agentID, object arg)
        {
            return agentID;
        }

        internal object visitteamID(teamID teamID, object arg)
        {
            return teamID;
        }

        internal object visitSquadID(squadID squadID, object arg)
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
