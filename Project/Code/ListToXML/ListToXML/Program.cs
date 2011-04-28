using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using XMLawesome;
using MAS;


namespace ListToXML
{
    class Program
    {
        public static List<Agent> agents = new List<Agent>();
        public static List<Team> teams = new List<Team>();
        public static List<Squad> squads = new List<Squad>();
        public static List<ActionPattern> actionPatterns = new List<ActionPattern>();

        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Title = "@AWESOME XML !";
            Console.WriteLine("\t   ____   __  ____  __ _    " +
"\n\t  / __ \\  \\ \\/ /  \\/  | |    " +
"\n\t / / _` |  \\  /| |\\/| | |    " +
"\n\t| | (_| |  /  \\| |  | | |___ " +
"\n\t \\ \\__,_| /_/\\_\\_|  |_|_____|" +
"\n\t  \\____/ AWESOME                     \n");
            Console.ForegroundColor = ConsoleColor.White;
            //Initializing the Teams, agents, squards and actionpattern
            Team team1 = new Team("Team one", "Red");
            Team team2 = new Team("Team Two", "Blue");
            Agent agent = new Agent("Olsen", 2, team1);
            Squad squad = new Squad("first squad", agent);
            ActionPattern aP = new ActionPattern("first action pattern");
            //Initializing the lists
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
            XML.GenerateThisShizzle("MAS", null);

            //Create instance of the XmlReader with a path to the xml file
            XmlReader Reader = new XmlReader(@"C:\Users\Kristian\Desktop\XML\WarGame.xml");


            Reader.Mount();

            //Add null filter
            //Search virker ikke mere, aner ikke hvorfor :(
            foreach (XmlType item in Reader.XmlSearch("MAS>Teams>Team"))
            {
                Console.WriteLine(item.Tag + " @ " + item.Value + " @ " + item.Order);
            }
            Console.ReadKey();
            for (int i = 0; i < Reader.XmlSearch("MAS>Teams>Team").Count; i++)
            {

            }

            for (int i = 0; i < Reader.XmlSearch("MAS>Teams>Team").Count; i++)
            {

            }

            for (int i = 0; i < Reader.XmlSearch("MAS>Teams>Team").Count; i++)
            {

            }

            for (int i = 0; i < Reader.XmlSearch("MAS>Teams>Team").Count; i++)
            {

            }
        }

        //Generate XML for each list

        public static void mAgents(List<Agent> Agents)
        {
            XMLhelp.Root("Agents", null);
            foreach (var value in Agents)
            {
                XMLhelp.Child("Agent", null);
                XMLhelp.Attribute("Attr", "AttrValue");
                XMLhelp.Attribute("Attr2", "AttrValue2");
                XMLhelp.Attribute("Attr3", "AttrValue3");
                XMLhelp.Attribute("Attr4", "AttrValue4");
                XMLhelp.Node("Id", value.ID.ToString());
                XMLhelp.Node("posX", value.posX.ToString());
                XMLhelp.Node("posY", value.posY.ToString());
                XMLhelp.Node("Name", value.name);
                XMLhelp.Node("Rank", value.rank.ToString());
                XMLhelp.Attribute("Attr", "AttrValue");
                XMLhelp.Attribute("Attr2", "AttrValue2");
                XMLhelp.Attribute("Attr3", "AttrValue3");
                XMLhelp.Attribute("Attr4", "AttrValue4");
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
                XMLhelp.Child("Team", Teams.ToString());
                XMLhelp.Node("Id", value.ID.ToString());
                XMLhelp.Node("Name", value.name);
                XMLhelp.LastNode("Color", value.color);
            }
        }

        public static void mSquads(List<Squad> Squads)
        {
            XMLhelp.Root("Squads", null);
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