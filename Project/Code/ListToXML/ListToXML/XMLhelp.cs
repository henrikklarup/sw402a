using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ListToXML
{
    static class XMLhelp
    {
        public static List<XML> XmlList = new List<XML>();
        public static int Depth = 0;

        public static void Root(String Tag, String Value)
        {
            //Return to root
            Depth = 0;
            XML temp = new XML(Depth, Tag, Value, false);
            XmlList.Add(temp);
        }

        public static void End()
        {
            //Return to root
            Depth = 0;
            XML temp = new XML(-1, null, null, false);
            XmlList.Add(temp);
        }

        public static void Child(String Tag, String Value)
        {
            //Make child
            Depth++;
            XML temp = new XML(Depth, Tag, Value, false);
            XmlList.Add(temp);
            Depth++;
        }

        public static void Parent(String Tag, String Value)
        {
            //Make parent
            Depth--;
            XML temp = new XML(Depth, Tag, Value, false);
            XmlList.Add(temp);
        }

        public static void Node(String Tag, String Value)
        {
            XML temp = new XML(Depth, Tag, Value, true);
            XmlList.Add(temp);
        }

        static String mAgents(List<Agent> agents)
        {
            String _agents = "<Agents>";
            for (int i = 0; i < agents.Count; i++)
            {
                _agents += "<Agent>";
                _agents += "<Id>" + agents[i].ID + "</Id>";
                _agents += "<posX>" + agents[i].posX + "</posX>";
                _agents += "<posY>" + agents[i].posY + "</posY>";
                _agents += "<name>" + agents[i].name + "</name>";
                _agents += "<rank>" + agents[i].rank + "</rank>";
                _agents += "<Team>";
                _agents += "<Id>" + agents[i].team.ID + "</Id>";
                _agents += "<Name>" + agents[i].team.name + "</Name>";
                _agents += "<Color>" + agents[i].team.color + "</Color>";
                _agents += "</Team>";
                _agents += "</Agent>";
            }
            _agents += "</Agents>";
            return _agents;
        }


        static String mTeams(List<Team> teams)
        {
            String _teams = "<Teams>\r\n";
            for (int i = 0; i < teams.Count; i++)
            {
                _teams += "<Team>\r\n";
                _teams += "<Id>" + teams[i].ID + "</Id>\r\n";
                _teams += "<Name>" + teams[i].name + "</Name>\r\n";
                _teams += "<Color>" + teams[i].color + "</Color>\r\n";
                _teams += "</Team>\r\n";

            }
            _teams += "</Teams>\r\n";
            return _teams;
        }

        static String mSquads(List<Squad> squads)
        {
            String _squards = "<Squards>\r\n";
            for (int i = 0; i < squads.Count; i++)
            {
                _squards += "<Squard>\r\n";
                _squards += "<Id>" + squads[i].ID + "</Id>\r\n";
                _squards += "<Name>" + squads[i].name + "</Name>\r\n";
                _squards += "<Agents>\r\n";
                foreach (int a in squads[i].agents)
                {
                    _squards += "<Agent>\r\n";
                    _squards += "<int>" + a + "</int>\r\n";
                    _squards += "</Agent>\r\n";
                }
                _squards += "</Agents>\r\n";
                _squards += "</Squard>\r\n";
            }
            _squards += "</Squards>\r\n";
            return _squards; 
        }

        static string mActionPatterns(List<ActionPattern> actionPatterns)
        {
            String _actions = "<ActionPatterns>";
            for (int i = 0; i < actionPatterns.Count; i++)
            {
                _actions += "<ActionPattern>";
                _actions += "<Id>" + actionPatterns[i].ID + "</Id>";
                _actions += "<Actions>";
                foreach (String a in actionPatterns[i].actions)
                {
                    _actions += "<Action>" + a +"</Action>";
                }
                _actions += "</Actions>";
                _actions += "</ActionPattern>";
            }

            _actions += "</ActionPatterns>";
            return _actions;
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
