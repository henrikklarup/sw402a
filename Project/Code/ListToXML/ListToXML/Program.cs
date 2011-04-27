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

        public static void Main(string[] args)
        {
            //Initializing the Teams, agents, squards and actionpattern
            Team team = new Team(1, "Team one", "Red");
            Agent agent = new Agent(1, "Olsen", 2, team);
            Squad squad = new Squad(1, "first squad", agent);
            ActionPattern aP = new ActionPattern(1, "first action pattern");
            //Initializing the lists
            teams.Add(team);
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
            XmlReader Reader = new XmlReader(@"C:\Users\Kristian\Desktop\XML\TestXml.xml");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "@AWESOME XML !";
            Console.WriteLine("\tAWESOME XML !");
            Console.WriteLine("@-@-@-@-@-@-@-@-@-@-@-@-@-@-@");
            Reader.Mount();

            //Add null filter
            //Search virker ikke mere, aner ikke hvorfor :(
            foreach (XmlType item in Reader.XmlSearch("note>item"))
            {
                Console.WriteLine(item.Tag + " @ " + item.Order);
            }
            Console.ReadKey();
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

            //Sæt dette til en liste i stedet med methode
            //brug delegates til at sende en liste af funtioner med'

            //lav root med fixed størrelse

            List<ChildNotes> Children = new List<ChildNotes>();

            foreach (var value in Teams)
            {
                XMLhelp.Child("Team", Teams.ToString());
                XMLhelp.Node("Id", value.ID.ToString());
                XMLhelp.Node("Name", value.name);
                XMLhelp.Node("Color", value.color);
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
