using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication6
{
    public static class Lists
    {
        public static List<Agent> agents;                 //List of agents
        public static List<ActionPattern> actionPatterns; //List of actionPatterns
        public static List<Squad> squads;                 //List of squads
        public static List<Team> teams;                   //List of teams
        public static List<Agent> moveAgents;             //List of agents to move

        public static Agent RetrieveAgent(string ident)
        {
            List<Agent> results;
            Agent agent;

            results = agents.FindAll(delegate(Agent a) { return a.name == ident; });
            if (results.Count == 0)
            {
                throw new Exception("Agent " + ident + " does not exists.");
            }
            else if (results.Count != 1)
            { 
                
            }

            return agent;
        }

        public static Team RetrieveTeam(string ident)
        {
            throw new NotImplementedException();
        }
    }
}
