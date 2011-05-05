using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using XMLawesome;
using MASClassLibrary;


namespace ListToXML
{
    class Program
    {
        public static List<agent> agents = new List<agent>();
        public static List<team> teams = new List<team>();
        public static List<squad> squads = new List<squad>();
        public static List<actionpattern> actionPatterns = new List<actionpattern>();

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
            team team1 = new team("Team one", "Red");
            team team2 = new team("Team Two", "Blue");
            agent agent = new agent(1,"Olsen", 2, team1);
            List<agent> Agent = new List<agent>();
            Agent.Add(agent);
            squad squad = new squad("first squad", Agent);
            actionpattern aP = new actionpattern("first action pattern");
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
            aXML.GenerateThisShizzle("MAS", null);

            //Create instance of the XmlReader with a path to the xml file
            XmlReader Reader = new XmlReader(@"C:\Users\Kristian\Desktop\XML\WarGame.xml");


            Reader.Mount();

            //Add null filter
            //Search virker ikke mere, aner ikke hvorfor :(
            foreach (XmlType item in Reader.XmlSearch("MAS>Squads>Squad"))
            {
                Console.WriteLine(item.Tag + " @ " + item.Value + " @ " + item.Order);
            }

        }

        //Generate XML for each list

        public static void mAgents(List<agent> Agents)
        {
            XMLhelp.Root("Agents", null);
            foreach (var value in Agents)
            {
                XMLhelp.Child("Agent", null);
                XMLhelp.Node("Name", value.name);
                XMLhelp.Node("Rank", value.rank.ToString());
                //Mangler at add team
                //public Team team;
                if (value.team == null)
                {
                    XMLhelp.Child("Teams", null);
                    XMLhelp.Child("Team", null);
                    XMLhelp.Node("Name", value.team.name);
                    XMLhelp.Node("Color", value.team.colorStr);
                }


            }
        }

        public static void mTeams(List<team> Teams)
        {
            XMLhelp.Root("Teams", null);
            foreach (var value in Teams)
            {
                XMLhelp.Child("Team", Teams.ToString());
                XMLhelp.Node("Name", value.name);
                XMLhelp.LastNode("Color", value.colorStr);
            }
        }

        public static void mSquads(List<squad> Squads)
        {
            XMLhelp.Root("Squads", null);
            foreach (var value in Squads)
            {
                XMLhelp.Child("Squad", null);
                XMLhelp.Node("Name", value.name);
                XMLhelp.Child("Agents", null);
                foreach (agent agent in value.Agents)
                {
                    XMLhelp.Node("Id", agent.id.ToString());
                }
            }
        }

        public static void mActionPatterns(List<actionpattern> ActionPatterns)
        {
            XMLhelp.Root("ActionPatterns", null);
            foreach (var value in ActionPatterns)
            {
                XMLhelp.Child("ActionPattern", null);
                XMLhelp.Node("Name", value.name);
                XMLhelp.Child("Actions", null);
                foreach (String action in value.actions)
                {
                    XMLhelp.Node("Action", action);
                }
            }
        }
    }
}