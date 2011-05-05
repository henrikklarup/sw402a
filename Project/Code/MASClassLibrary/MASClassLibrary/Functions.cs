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
            foreach (agent a in Lists.agents)
            {
                if (agent.team.id != Lists.currentteam.id)
                    throw new WrongTeamException("Wrong team");

                if (a.id == agent.id)
                {
                    #region ifValid
                    //Set Figure to x,y
                    Point newPoint = new Point(xchord, ychord);

                    //Just moved agent
                    agent moveagent = new agent(a.id, a.name, a.rank, a.team);
                    moveagent.posx = newPoint.X;
                    moveagent.posy = newPoint.Y;

                    Lists.moveagents.Add(moveagent);

                    //OLD SHIT
                    //a.posX = newPoint.X;
                    //a.posY = newPoint.Y;

                    #endregion
                }
            }
        }
        #endregion
    }
}
