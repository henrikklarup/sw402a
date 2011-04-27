using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLawesome
{
    public class Squad
    {
        public int ID;
        public string name;
        public int[] agents;

        public Squad()
        { }

        public Squad(int ID, string name, List<Agent> agents)
        {
            this.ID = ID;
            this.name = name;

            int i = 0;
            foreach (Agent a in agents)
            {
                this.agents[i] = a.ID;
                i++;
            }
        }

        public Squad(int ID, string name, Agent agent)
        {
            int[] agentArray = new int[1];
            agentArray[0] = agent.ID;

            this.ID = ID;
            this.name = name;
            this.agents = agentArray;
        }
    }
}
