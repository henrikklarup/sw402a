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
            team team3 = new team("Team Three", "Pink");
            team team4 = new team("Team Four", "Black");
            agent agent1 = new agent("Olsen", 2, team1);
            agent agent2 = new agent("Kasper", 2, team1);
            agent agent3 = new agent("Rasmus", 2, team2);
            agent agent4 = new agent("Simon", 2, team2);
            agent agent5 = new agent("Henrik", 2, team3);
            agent agent6 = new agent("Lasse", 2, team3);
            agent agent7 = new agent("Kristian", 2, team4);
            agent agent8 = new agent("Jørgen", 2, team4);

            List<agent> Agent1 = new List<agent>();
            Agent1.Add(agent1);
            Agent1.Add(agent2);
            List<agent> Agent2 = new List<agent>();
            Agent2.Add(agent3);
            Agent2.Add(agent4);
            List<agent> Agent3 = new List<agent>();
            Agent3.Add(agent5);
            Agent3.Add(agent6);
            List<agent> Agent4 = new List<agent>();
            Agent4.Add(agent7);
            Agent4.Add(agent8);

            squad squad1 = new squad("first squad", Agent1);
            squad squad2 = new squad("second squad", Agent2);
            squad squad3 = new squad("third squad", Agent3);
            squad squad4 = new squad("forth squad", Agent4);

            List<String> newString = new List<String>();
            newString.Add("action1");
            newString.Add("action2");
            newString.Add("action3");
            newString.Add("action4");
            List<String> newString1 = new List<String>();
            newString1.Add("action5");
            newString1.Add("action6");
            newString1.Add("action7");
            newString1.Add("action8");
            actionpattern aP = new actionpattern("first action pattern", newString);
            actionpattern aP1 = new actionpattern("second action pattern", newString1);

            //XMLhelp.Generate(agents, teams, squads, actionPatterns);
            mTeams(Lists.teams);
            mAgents(Lists.agents);
            mSquads(Lists.squads);
            mActionPatterns(Lists.actionPatterns);

            //Generate the xml file with <MAS> as root and default encoding
            //aXML.GenerateThisShizzle("MAS", null, @"C:\Users\Kristian\Desktop\XML\WarGame.xml");

            Generate.XML(@"C:\Users\Kristian\Desktop\XML\WarGame.xml", Lists.actionPatterns, Lists.teams, Lists.agents, Lists.squads);
            //Create instance of the XmlReader with a path to the xml file
            XmlReader Reader = new XmlReader(@"C:\Users\Kristian\Desktop\XML\WarGame.xml");


            Reader.Mount();

            //Add null filter
            //Search virker ikke mere, aner ikke hvorfor :(

            foreach (XmlType item in Reader.XmlSearch("MAS>Squads>Squad"))
            {
                Console.WriteLine(item.Tag + " @ " + item.Value + " @ " + item.Order);
            }

            ListOfMas newLists = new ListOfMas();
            newLists.Init(@"C:\Users\Kristian\Desktop\XML\WarGame.xml",false);
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
                    XMLhelp.LastNode("Color", value.team.colorStr);
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
                foreach (agent agent in value.Agents)
                {
                    XMLhelp.Node("AgentName", agent.name);
                }
                XMLhelp.LastNode("Name", value.name);
            }
        }

        public static void mActionPatterns(List<actionpattern> ActionPatterns)
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