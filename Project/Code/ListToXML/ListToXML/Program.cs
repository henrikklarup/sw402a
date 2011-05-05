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
            Team team3 = new Team("Team Three", "Pink");
            Team team4 = new Team("Team Four", "Purple");
            Agent agent = new Agent("Olsen", 2, team1);
            Squad squad = new Squad("first squad", agent);
            ActionPattern aP = new ActionPattern("first action pattern");
            //Initializing the lists
            teams.Add(team1);
            teams.Add(team2);
            teams.Add(team3);
            teams.Add(team4);
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
          //  foreach (XmlType item in Reader.XmlSearch("MAS>Agents"))
            //{
              //  Console.WriteLine(item.Tag + " @ " + item.Value + " @ " + item.Order);
            //}

            ListOfMas derp = new ListOfMas();


            foreach (Team a in derp.TeamList(@"C:\Users\Kristian\Desktop\XML\WarGame.xml"))
            {
                Console.WriteLine("test");
                Console.WriteLine(a.ID + a.color + a.name);
                //Console.WriteLine(a.ID + "@" + a.name + "@" + a.posX + "@" + a.posY + "@" + a.rank + "@" + a.team.color + "@" + a.team.name + "@" + a.team.ID);
            }

            foreach (Agent a in derp.AgentList(@"C:\Users\Kristian\Desktop\XML\WarGame.xml"))
            {
                Console.WriteLine("test");
                Console.WriteLine(a.ID + "@" + a.name + "@" + a.posX + "@" + a.posY + "@" + a.rank + "@" + a.team.color + "@" + a.team.name + "@" + a.team.ID);
            }

            foreach (Squad a in derp.SquadList(@"C:\Users\Kristian\Desktop\XML\WarGame.xml"))
            {
                Console.WriteLine("test");
                Console.WriteLine(a.ID + "@" + a.name);
                foreach (int b in a.agents)
                {
                    Console.WriteLine(b);
                }
                Console.WriteLine("testend");
            }

            foreach (ActionPattern a in derp.ApList(@"C:\Users\Kristian\Desktop\XML\WarGame.xml"))
            {
                Console.WriteLine("test");
                Console.WriteLine("id: " + a.ID);
                foreach (String b in a.actions)
                {
                    Console.WriteLine("action: " + b);
                }
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
                XMLhelp.Node("Id", value.ID.ToString());
                XMLhelp.Node("posX", value.posX.ToString());
                XMLhelp.Node("posY", value.posY.ToString());
                XMLhelp.Node("Name", value.name);
                XMLhelp.Node("Rank", value.rank.ToString());
                //Mangler at add team
                //public Team team;
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