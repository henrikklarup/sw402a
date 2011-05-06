using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace XMLawesome
{
    public static class Generate
    {
        public static void XML(String pathName, List<actionpattern> ActionPatterns, List<team> Teams,  List<agent> Agents, List<squad> Squads)
        {
            mActionPatterns(ActionPatterns);
            mTeams(Teams);
            mAgents(Agents);
            mSquads(Squads);
            aXML.GenerateThisShizzle("MAS", null, pathName);
        }

        private static void mAgents(List<agent> Agents)
        {
            if (Agents.Count != 0)
            {
                XMLhelp.Root("Agents", null);
                foreach (agent value in Agents)
                {
                    XMLhelp.Child("Agent", null);
                    XMLhelp.Node("Name", value.name);
                    XMLhelp.Node("Rank", value.rank.ToString());
                    //Mangler at add team
                    //public Team team;
                    if (value.team != null)
                    {
                        XMLhelp.Child("Team", null);
                        XMLhelp.Node("Name", value.team.name);
                        XMLhelp.LastNode("Color", value.team.colorStr);
                    }

                }
            }
        }

        private static void mTeams(List<team> Teams)
        {
            XMLhelp.Root("Teams", null);
            foreach (team value in Teams)
            {
                XMLhelp.Child("Team", Teams.ToString());
                XMLhelp.Node("Name", value.name);
                XMLhelp.LastNode("Color", value.colorStr);
            }
        }

        private static void mSquads(List<squad> Squads)
        {
            XMLhelp.Root("Squads", null);
            foreach (squad value in Squads)
            {
                XMLhelp.Child("Squad", null);
                foreach (agent agent in value.Agents)
                {
                    XMLhelp.Node("AgentName", agent.name);
                }
                XMLhelp.LastNode("Name", value.name);
            }
        }

        private static void mActionPatterns(List<actionpattern> ActionPatterns)
        {
            XMLhelp.Root("ActionPatterns", null);
            foreach (var value in ActionPatterns)
            {
                XMLhelp.Child("ActionPattern", null);
                foreach (String action in value.actions)
                {
                    XMLhelp.Node("Action", action);
                }
                XMLhelp.LastNode("Name", value.name);
            }
        }
    }
}
