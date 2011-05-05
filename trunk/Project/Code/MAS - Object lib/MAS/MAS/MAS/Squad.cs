using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAS
{
    public class Squad
    {
        private static int IDcount = 0;
        public int ID;
        public string name;
        public int[] agents;

        public Squad()
        { }

        public Squad(string name, List<Agent> agents)
        {
            IDcount++;
            this.ID = IDcount;
            this.name = name;

            int i = 0;
            foreach (Agent a in agents)
            {
                this.agents[i] = a.ID;
                i++;
            }
        }

        public Squad(string name, Agent agent)
        {
            int[] agentArray = new int[1];
            agentArray[0] = agent.ID;

            IDcount++;
            this.ID = IDcount;
            this.name = name;
            this.agents = agentArray;
        }

        public Squad(int Id, string name, List<Agent> agents)
        {
            this.ID = Id;
            this.name = name;

            int i = 0;
            foreach (Agent a in agents)
            {
                this.agents[i] = a.ID;
                i++;
            }
        }

        public Squad(int Id, string name, List<int> agents)
        {
            this.ID = Id;
            this.name = name;

            int i = 0;
            foreach (int a in agents)
            {
                this.agents[i] = a;
                i++;
            }
        }

        public Squad(int Id, string name, Agent agent)
        {
            int[] agentArray = new int[1];
            agentArray[0] = agent.ID;

            this.ID = Id;
            this.name = name;
            this.agents = agentArray;
        }
    }
}
