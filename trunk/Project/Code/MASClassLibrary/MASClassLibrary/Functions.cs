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
                if (agent.team.ID != Lists.currentteam.ID)
                    throw new Exception("Wrong team");

                if (a.ID == agent.ID)
                {
                    #region ifValid
                    //Set Figure to x,y
                    Point newPoint = new Point(xchord, ychord);

                    //Just moved agent
                    agent moveagent = new agent(a.ID, a.name, a.rank, a.team);
                    moveagent.posX = newPoint.X;
                    moveagent.posY = newPoint.Y;

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
