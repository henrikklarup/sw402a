using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MASClassLibrary
{
    public static class Functions
    {
        /// <summary>
        /// Moves the selected agent to the x and y choords selected
        /// </summary>
        /// <param name="agent">agent to move</param>
        /// <param name="xchord">X chord</param>
        /// <param name="ychord">Y chord</param>
        #region Moveagent
        public static void moveagent(agent agent, int xchord, int ychord)
        {
            if (agent.team.id != Lists.currentteam.id)
                throw new WrongTeamException("Wrong team");

            foreach (agent a in Lists.agents)
            {
                if (a.id == agent.id)
                {
                    #region ifValid
                    //Set Figure to x,y
                    Point newPoint = new Point(xchord, ychord);

                    //Just moved agent
                    agent moveagent = new agent(a.id, a.name, a.rank, a.team);
                    moveagent.posx = newPoint.X;
                    moveagent.posy = newPoint.Y;

                    /*
                    foreach (agent doubleAgent in Lists.moveagents)
                    {
                        if (moveagent.id == doubleAgent.id)
                        {
                            Lists.moveagents.Remove(doubleAgent);
                            break;
                        }
                    }
                    */

                    Lists.moveagents.Add(moveagent);

                    //OLD SHIT
                    //a.posX = newPoint.X;
                    //a.posY = newPoint.Y;

                    #endregion
                }
            }
        }
        #endregion

        /// <summary>
        /// Check if any other agents are within next move
        /// </summary>
        /// <param name="checkAgent">Agent to check against</param>
        /// <returns>First agent encountered</returns>
        #region Encounter
        public static agent encounter(agent checkAgent)
        {
            foreach (agent a in Lists.agents)
            {
                if (a.team.id != checkAgent.team.id)
                {
                    if (a.posy == checkAgent.posy - 3 && (a.posx == checkAgent.posx))
                        return a;
                    else if (a.posy == checkAgent.posy - 2 && (a.posx == checkAgent.posx - 1 || a.posx == checkAgent.posx + 1))
                        return a;
                    else if (a.posy == checkAgent.posy - 1 && (a.posx == checkAgent.posx - 2 || a.posx == checkAgent.posx - 1 || a.posx == checkAgent.posx + 1 || a.posx == checkAgent.posx + 2))
                        return a;
                    else if (a.posy == checkAgent.posy && (a.posx == checkAgent.posy - 3 || a.posx == checkAgent.posx - 2 || a.posx == checkAgent.posx - 1 || a.posx == checkAgent.posx + 1 || a.posx == checkAgent.posx + 2 || a.posx == checkAgent.posx + 3))
                        return a;
                    else if (a.posy == checkAgent.posy + 1 && (a.posx == checkAgent.posx - 2 || a.posx == checkAgent.posx - 1 || a.posx == checkAgent.posx + 1 || a.posx == checkAgent.posx + 2))
                        return a;
                    else if (a.posy == checkAgent.posy + 2 && (a.posx == checkAgent.posx - 1 || a.posx == checkAgent.posx + 1))
                        return a;
                    else if (a.posy == checkAgent.posy + 3 && (a.posx == checkAgent.posx))
                        return a;
                }
            }
            return null;
        }
        #endregion

        /// <summary>
        /// Adds encounter to list
        /// </summary>
        /// <param name="id">Agent id</param>
        /// <param name="command">Action command</param>
        #region addEncounter
        public static void addEncounter(agent agent, string command)
        {
            if (agent.team.id != Lists.currentteam.id)
                throw new WrongTeamException("Wrong team");

            encounter en = new encounter();
            en.agentId = agent.id;
            en.action = command;
            Lists.encounters.Add(en);
        }
        #endregion
    }
}
