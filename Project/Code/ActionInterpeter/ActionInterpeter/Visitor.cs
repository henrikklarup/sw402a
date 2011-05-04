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

            // While decorating arg is null.
            if (arg == null)
            {
                #region Compare selection type
                // If the selection is an agent, set the type of the selection.
                if (object.ReferenceEquals(
                    single_Action.selection.GetType(), new AgentID().GetType()))
                {
                    single_Action.type = (int)Type.Types.AGENTID;
                }
                // try with Team.
                else if (object.ReferenceEquals(
                    single_Action.selection.GetType(), new TeamID().GetType()))
                {
                    single_Action.type = (int)Type.Types.TEAMID;
                }
                // and squad.
                else if (object.ReferenceEquals(
                    single_Action.selection.GetType(), new SquadID().GetType()))
                {
                    single_Action.type = (int)Type.Types.SQUADID;
                }
                else if (object.ReferenceEquals(
                    selection.GetType(), new Agent().GetType()))
                {
                    single_Action.type = (int)Type.Types.AGENT;
                }
                else if (object.ReferenceEquals(
                    selection.GetType(), new Squad().GetType()))
                {
                    single_Action.type = (int)Type.Types.SQUAD;
                }
                else if (object.ReferenceEquals(
                    selection.GetType(), new Team().GetType()))
                {
                    single_Action.type = (int)Type.Types.TEAM;
                }
                else
                {
                    throw new GrammarException("Couldn't match the selected unit.");
                }
                #endregion
            }
            else
            {
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
            }
            return null;
        }

        private void visitCodeGen_MoveAgent(Single_Action single_Action, object arg)
        {
            Agent agent;
            if (single_Action.type == (int)Type.Types.AGENTID)
            {
                AgentID select = (AgentID)single_Action.selection;
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
            List<Agent> agents;
            Team team;
            if (single_Action.type == (int)Type.Types.TEAMID)
            {
                TeamID select = (TeamID)single_Action.selection;
                Token selectToken = select.num;
                team = Lists.RetrieveTeam(Convert.ToInt32(selectToken.spelling));
            }
            else
            {
                Identifier ident = (Identifier)single_Action.selection;
                team = Lists.RetrieveTeam(ident.name.spelling);
            }

            agents = Lists.RetrieveAgentsByTeam(team);
            foreach (Agent a in agents)
            {
                visitCodeGen_MoveOption(a, single_Action.move_option);
            }
        }

        private void visitCodeGen_MoveSquad(Single_Action single_Action, object arg)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Execute the moveAgent method.
        /// </summary>
        /// <param name="agent">The agent which is being moved.</param>
        /// <param name="move_Option">Move option from the single_action</param>
        private void visitCodeGen_MoveOption(Agent agent, Move_Option move_Option)
        {
            int num1;
            int num2;

            switch (move_Option.type)
            {
                case (int)Type.Types.DIR:
                    Direction dir = (Direction)move_Option.dir_coord.visit(this, 1);
                    num1 = agent.posX;
                    num2 = agent.posY;

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
            Functions.moveAgent(agent, num1, num2);
        }


        internal object visitMove_Option(Move_Option move_Option, object arg)
        {
            object dir_coord = move_Option.dir_coord.visit(this, arg);

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
                else if (object.ReferenceEquals(dir_coord.GetType(), new ActionPattern().GetType()))
                {
                    move_Option.type = (int)Type.Types.ACTIONPATTERN;
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
            if (token.kind == (int)Type.Types.IDENTIFIER)
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
