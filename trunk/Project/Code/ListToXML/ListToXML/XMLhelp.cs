using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ListToXML
{
    static class XMLhelp
    {

        public static String space = "";
        static void Child(String tag, String value)
        {

        }

        static void Node(String tag, String value)
        {
             
        }

        static String mAgents(List<Agent> agents)
        {
            String _agents = "<Agents>\r\n";
            for (int i = 0; i < agents.Count; i++)
            {
                _agents += space+"<Agent>\r\n";
                _agents += space+space+"<Id>" + agents[i].ID + "</Id>\r\n";
                _agents += space+space+"<posX>" + agents[i].posX + "</posX>\r\n";
                _agents += space+space+"<posY>" + agents[i].posY + "</posY>\r\n";
                _agents += space+space+"<name>" + agents[i].name + "</name>\r\n";
                _agents += space+space+"<rank>" + agents[i].rank + "</rank>\r\n";
                _agents += space+space+"<Team>\r\n";
                _agents += space+space+space+"<Id>" + agents[i].team.ID + "</Id>\r\n";
                _agents += space+space+space+"<Name>" + agents[i].team.name + "</Name>\r\n";
                _agents += space+space+space+"<Color>" + agents[i].team.color + "</Color>\r\n";
                _agents += space+space+"</Team>\r\n";
                _agents += space+"</Agent>\r\n";
            }
            _agents += "</Agents>\r\n";
            return _agents;
        }


        static String mTeams(List<Team> teams)
        {
            String _teams = "<Teams>\r\n";
            for (int i = 0; i < teams.Count; i++)
            {
                _teams += space+"<Team>\r\n";
                _teams += space+space+"<Id>" + teams[i].ID + "</Id>\r\n";
                _teams += space+space+"<Name>" + teams[i].name + "</Name>\r\n";
                _teams += space+space+"<Color>" + teams[i].color + "</Color>\r\n";
                _teams += space+"</Team>\r\n";

            }
            _teams += "</Teams>\r\n";
            return _teams;
        }

        static String mSquads(List<Squad> squads)
        {
            String _squards = "<Squards>\r\n";
            for (int i = 0; i < squads.Count; i++)
            {
                _squards += space+"<Squard>\r\n";
                _squards += space+space+"<Id>" + squads[i].ID + "</Id>\r\n";
                _squards += space+space+"<Name>" + squads[i].name + "</Name>\r\n";
                _squards += space+space+"<Agents>\r\n";
                foreach (int a in squads[i].agents)
                {
                    _squards += space+space+space+"<Agent>\r\n";
                    _squards += space+space+space+"<int>" + a + "</int>\r\n";
                    _squards += space+space+space+"</Agent>\r\n";
                }
                _squards += space+space+"</Agents>\r\n";
                _squards += space+"</Squard>\r\n";
            }
            _squards += "</Squards>\r\n";
            return _squards; 
        }

        static string mActionPatterns(List<ActionPattern> actionPatterns)
        {
            return "";
        }

        public static void Generate(List<Agent> agents, List<Team> teams, List<Squad> squads, List<ActionPattern> actionPatterns)
        {
            //Generate xml for each list
            Console.WriteLine(mAgents(agents));
            Console.WriteLine(mTeams(teams));
            Console.WriteLine(mSquads(squads));
            //Console.WriteLine(mActionPatterns(actionPatterns));

            using (StreamWriter outfile = new StreamWriter(@"\AllTxtFiles.xml"))
            {
                outfile.Write("<MAS>\r\n" + mAgents(agents) + mTeams(teams) + mSquads(squads) + mActionPatterns(actionPatterns) + "</MAS>");
            }

        }
    }
}
