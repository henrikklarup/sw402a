using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class oldSquad
    {
        public int ID;
        public string name;
        public int[] agents;

        public oldSquad()
        { }

        public oldSquad(int ID, string name, List<agent> agents)
        {
            this.ID = ID;
            this.name = name;

            int i = 0;
            foreach (agent a in agents)
            {
                this.agents[i] = a.id;
                i++;
            }
        }

        public oldSquad(int ID, string name, agent agent)
        {
            int[] agentArray = new int[1];
            agentArray[0] = agent.id;

            this.ID = ID;
            this.name = name;
            this.agents = agentArray;
        }
    }
}
