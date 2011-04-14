using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using XMLawesome;


namespace ListToXML
{
    class Program
    {
        public static List<Agent> agents = new List<Agent>();
        public static List<Team> teams = new List<Team>();
        public static List<Squad> squads = new List<Squad>();
        public static List<ActionPattern> actionPatterns = new List<ActionPattern>();

        private static string _agentXML, _teamXML, _squadXML, _actionPatternXML;
        public const string xmlTag = ".xml";
        public const string filePath = @"C:\";

        public static string AgentXML
        { get { return filePath + _agentXML + xmlTag; } }

        public static string TeamXML
        { get { return filePath + _teamXML + xmlTag; } }

        public static string SquadXML
        { get { return filePath + _squadXML + xmlTag; } }

        public static string ActionPatternXML
        { get { return filePath + _actionPatternXML + xmlTag; } }

        public static void Main(string[] args)
        {
            //Initializing the Teams, agents, squards and actionpattern
            Team team = new Team(1, "Team one", "Red");
            Team team1 = new Team(2, "Team two", "Blue");
            Team team2 = new Team(3, "Team three", "Green");
            Agent agent = new Agent(1, "Olsen", 2, team);
            Squad squad = new Squad(1, "first squad", agent);
            ActionPattern aP = new ActionPattern(1, "first action pattern");
            //Initializing the lists
            teams.Add(team);
            teams.Add(team1);
            teams.Add(team2);
            agents.Add(agent);
            squads.Add(squad);
            actionPatterns.Add(aP);

            //XMLhelp.Generate(agents, teams, squads, actionPatterns);
            mTeams(teams);
            mAgents(agents);
            mSquads(squads);
            mActionPatterns(actionPatterns);

            //Generate the xml file with <MAS> as root and default encoding
            XML.GenerateThisShizzle("MAS",null);

            //Create instance of the XmlReader with a path to the xml file
            XmlReader Reader = new XmlReader(@"C:\WarGame.xml");
            Reader.Mount();
            foreach (XmlType item in Reader.XmlSearch(""))
            {
                Console.WriteLine(item.Tag + item.Value);
            }
        }

        //Generate XML for each list

        public static void mAgents(List<Agent> Agents)
        {
            XMLhelp.Root("Agents", null);
            foreach (var value in Agents)
            {
                XMLhelp.Child("Agent", null);
                XMLhelp.Attribute("Attr","AttrValue");
                XMLhelp.Node("Id", value.ID.ToString());
                XMLhelp.Node("posX", value.posX.ToString());
                XMLhelp.Node("posY", value.posY.ToString());
                XMLhelp.Node("Name", value.name);
                XMLhelp.Node("Rank", value.rank.ToString());
                //Mangler at add team
                //public Team team;
                XMLhelp.Child("Teams", null);
                XMLhelp.Child("Team", null);
                XMLhelp.Node("Id", value.team.ID.ToString());
                XMLhelp.Node("Name", value.team.name);
                XMLhelp.Node("Color", value.team.color);


            }
        }

        public static void mTeams(List<Team> Teams)
        {
            XMLhelp.Root("Teams", null);
            foreach (var value in Teams)
            {
                XMLhelp.Child("Team", null);
                XMLhelp.Node("Id", value.ID.ToString());
                XMLhelp.Node("Name", value.name);
                XMLhelp.Node("Color", value.color);
            }
        }

        public static void mSquads(List<Squad> Squads)
        {
            XMLhelp.Root("Squards", null);
            foreach (var value in Squads)
            {
                XMLhelp.Child("Squad", null);
                XMLhelp.Node("Name", value.name);
                XMLhelp.Node("Id", value.ID.ToString());
                XMLhelp.Child("Agents", null);
                foreach (int agent in value.agents)
                {
                    XMLhelp.Child("Agent", null);
                    XMLhelp.Node("Id", agent.ToString());
                }
            }
        }

        public static void mActionPatterns(List<ActionPattern> ActionPatterns)
        {
            XMLhelp.Root("ActionPatterns", null);
            foreach (var value in ActionPatterns)
            {
                XMLhelp.Child("ActionPattern", null);
                XMLhelp.Node("Id", value.ID.ToString());
                XMLhelp.Child("Actions", null);
                foreach (String action in value.actions)
                {
                    XMLhelp.Node("Action", action);
                }
            }
        }

       }
    }

    public class Agent
    {
        public int ID;
        public int posX;
        public int posY;
        public string name;
        public int rank;
        public Team team;

        public Agent()
        { }

        public Agent(int ID, string name, int rank, Team team)
        {
            this.ID = ID;
            this.name = name;
            this.rank = rank;
            this.posX = 0;
            this.posY = 0;
            this.team = team;

        }
    }

    public class Team
    {
        public int ID;
        public string name;
        public string color;

        public Team()
        { }

        public Team(int ID, string name, string color)
        {
            this.ID = ID;
            this.name = name;
            this.color = color;
        }
    }

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

    public class ActionPattern
    {
        public int ID;
        public string[] actions;

        public ActionPattern()
        { }

        public ActionPattern(int ID, string[] actions)
        {
            this.ID = ID;
            this.actions = actions;
        }

        public ActionPattern(int ID, string action)
        {
            string[] actionString = new string[1];
            actionString[0] = action;

            this.ID = ID;
            this.actions = actionString;
        }
    }


}
