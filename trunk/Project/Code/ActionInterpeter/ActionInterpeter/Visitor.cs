using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;
using ActionPatternCompiler;

namespace ActionInterpeter
{
    class Visitor
    {
        // Exception for catching errors.
        private GrammarException gException =
            new GrammarException("These errors were found during decoration:");

        /// <summary>
        /// Visits the AST
        /// </summary>
        /// <param name="ast">Any AST</param>
        /// <param name="arg">Any object</param>
        /// <returns>null</returns>
        public object visitAST(AST ast, object arg)
        {
            ast.visit(this, arg);
            return null;
        }

        /// <summary>
        /// Visits an Action
        /// </summary>
        /// <param name="action">Any Action</param>
        /// <param name="arg">Any object</param>
        /// <returns>null</returns>
        internal object visitAction(Action action, object arg)
        {
            action.single_action.visit(this, arg);
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
            object selection = single_Action.selection.visit(this, arg);
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
                // set arg to null if its an id.
                visitCodeGen_MoveAgent(single_Action, null);
            }
            // try with team.
            else if (object.ReferenceEquals(
                single_Action.selection.GetType(), new teamID().GetType()))
            {
                // set arg to null if its an id.
                visitCodeGen_MoveTeam(single_Action, null);
            }
            // and squad.
            else if (object.ReferenceEquals(
                single_Action.selection.GetType(), new squadID().GetType()))
            {
                // set arg to null if its an id.
                visitCodeGen_MoveSquad(single_Action, null);
            }
            else if (object.ReferenceEquals(
                selection.GetType(), new agent().GetType()))
            {
                // set arg to not null if its an identifier.
                visitCodeGen_MoveAgent(single_Action, 1);
            }
            else if (object.ReferenceEquals(
                selection.GetType(), new team().GetType()))
            {
                // set arg to not null if its an identifier.
                visitCodeGen_MoveTeam(single_Action, 1);
            }
            else if (object.ReferenceEquals(
                selection.GetType(), new squad().GetType()))
            {
                // set arg to not null if its an identifier.
                visitCodeGen_MoveSquad(single_Action, 1);
            }
            #endregion
            return null;
        }

        /// <summary>
        /// Calls the moveoption method.
        /// </summary>
        /// <param name="single_Action">Any SingleAction containing a defined type.</param>
        /// <param name="arg">Any object</param>
        private void visitCodeGen_MoveAgent(Single_Action single_Action, object arg)
        {
            agent agent;
            // If arg is null, the selection is an ID.
            if (arg == null)
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
            // If arg is null, the selection is an ID.
            if (arg == null)
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
            // If arg is null, the selection is an ID.
            if (arg == null)
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
        private void visitCodeGen_MoveOption(agent _agent, Move_Option move_Option)
        {
            int num1;
            int num2;

            switch (move_Option.type)
            {
                case (int)Type.Types.DIR:
                    Direction dir = (Direction)move_Option.dir_coord;
                    num1 = _agent.posx;
                    num2 = _agent.posy;

                    Token token = dir.dir;

                    switch (token.spelling.ToLower())
                    {
                        case "up":
                        case "down":
                        case "left":
                        case "right":
                            foreach (agent a in Lists.moveagents)
                            {
                                if (a.id == _agent.id)
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
                            Lists.moveagents.RemoveAll(s => s.id == _agent.id);
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
                        ActionPattern.Compile(s, _agent);
                    }
                    return;
                default:
                    // No move has been matched!
                    throw new InvalidMoveOptionException("The move option was invalid!");
            }

            switch (move_Option.stance)
            { 
                case (int)Stance.Stances.MOVE:
                    Functions.moveagent(_agent, num1, num2);
                    break;
                case (int)Stance.Stances.ENCOUNTER:
                    throw new NotImplementedException();
                    break;
            }
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
                else if (object.ReferenceEquals(dir_coord.GetType(), new Identifier().GetType()))
                {
                    move_Option.type = (int)Type.Types.ACTIONPATTERN;
                }
            }
            else
            {
                throw new GrammarException("An invalid move option was chosen.");
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
