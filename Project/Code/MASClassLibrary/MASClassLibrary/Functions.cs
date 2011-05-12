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
            //Check if team match else throw exeption
            if (agent.team.id != Lists.currentteam.id)
                throw new WrongTeamException("Wrong team");

            //Loop all agents
            foreach (agent a in Lists.agents)
            {
                //Check id
                if (a.id == agent.id)
                {
                    #region ifValid
                    //Set Figure to x,y
                    Point newPoint = new Point(xchord, ychord);

                    //Just moved agent
                    agent moveagent = new agent(a.id, a.name, a.rank, a.team);
                    moveagent.posx = newPoint.X;
                    moveagent.posy = newPoint.Y;

                    //Add to moveagents
                    Lists.moveagents.Add(moveagent);

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
            //Loop all agents
            foreach (agent a in Lists.agents)
            {
                //Check team id doesn't match
                if (a.team.id != checkAgent.team.id)
                {
                    //Check logic
                    #region Some Logic
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
                    #endregion
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
            //Check same team, else throw exeption
            if (agent.team.id != Lists.currentteam.id)
                throw new WrongTeamException("Wrong team");

            //New encounter
            encounter en = new encounter();
            //Set encounter agent id
            en.agentId = agent.id;
            //Set encounter action
            en.action = command;
            //Add to encounters
            Lists.encounters.Add(en);
        }
        #endregion
    }
}
