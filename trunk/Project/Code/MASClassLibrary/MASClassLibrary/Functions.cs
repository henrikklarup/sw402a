using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MASClassLibrary
{
    public class Functions
    {
        /// <summary>
        /// Moves the selected agent to the x and y choords selected
        /// </summary>
        /// <param name="agent">Agent to move</param>
        /// <param name="xchord">X chord</param>
        /// <param name="ychord">Y chord</param>
        #region MoveAgent
        public void moveAgent(Agent agent, int xchord, int ychord)
        {
            foreach (Agent a in Lists.agents)
            {
                if (a.ID == agent.ID)
                {
                    #region ifValid
                    //Set Figure to x,y
                    Point newPoint = new Point(xchord - 1, ychord - 1);

                    Agent moveagent = a;
                    moveagent.posX = newPoint.X;
                    moveagent.posY = newPoint.Y;

                    Lists.moveAgents.Add(moveagent);

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
