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
            
            Lists.agents = new List<agent>();
            Lists.teams = new List<team>();
            Lists.squads = new List<squad>();
            Lists.actionPatterns = new List<actionpattern>();
            List<agent> newListagent = new List<agent>();


            team team1 = new team("Team one", "Red");
            team team2 = new team("Team Two", "Blue");
            agent agent = new agent(0,"Olsen", 2, team1);

            List<agent> Agent = new List<agent>();
            Agent.Add(agent);

            squad squad = new squad("first squad", Agent);
            actionpattern aP = new actionpattern("first action pattern","action!");

            //XMLhelp.Generate(agents, teams, squads, actionPatterns);
            mTeams(Lists.teams);
            newListagent.Add(agent);
            mAgents(newListagent);
            mSquads(Lists.squads);
            mActionPatterns(Lists.actionPatterns);

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
            Console.ReadKey();
        }

        //Generate XML for each list

        public static void mAgents(List<agent> Agents)
        {
            if(Agents.Count != 0)
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
                    XMLhelp.Node("Color", value.team.colorStr);
                }

            }
            }
        }

        public static void mTeams(List<team> Teams)
        {
            XMLhelp.Root("Teams", null);
            foreach (team value in Teams)
            {
                XMLhelp.Child("Team", Teams.ToString());
                XMLhelp.Node("Name", value.name);
                XMLhelp.LastNode("Color", value.colorStr);
            }
        }

        public static void mSquads(List<squad> Squads)
        {
            XMLhelp.Root("Squads", null);
            foreach (squad value in Squads)
            {
                XMLhelp.Child("Squad", null);
                XMLhelp.Node("Name", value.name);
                XMLhelp.Child("Agents", null);
                foreach (agent agent in value.Agents)
                {
                    XMLhelp.Node("Name", agent.name);
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