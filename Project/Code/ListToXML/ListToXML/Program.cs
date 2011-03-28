using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace ListToXML
{
    class Program
    {
        public static List<Agent> agents = new List<Agent>();
        public static List<Team> teams = new List<Team>();
        public static List<Squad> squads = new List<Squad>();
        public static List<ActionPattern> actionPatterns = new List<ActionPattern>();

        static void Main(string[] args)
        {
            Console.WriteLine("(R)ead or (S)ave lists: ");


            //Initializing the lists
            Team team = new Team(1, "Team one", "Red");
            Agent agent = new Agent(1, "Olsen", 2, team);
            //Squad squad = new Squad(1, "first squad", agent);
            ActionPattern aP = new ActionPattern(1, "first action pattern");

            teams.Add(team);
            agents.Add(agent);
            actionPatterns.Add(aP);

            //Interface to test saving and loading the xml files
            ConsoleKeyInfo cki = Console.ReadKey();

            while (cki.Key != ConsoleKey.Escape)
            {
                if (cki.Key == ConsoleKey.S)
                {
                    generateXML();
                }
                if (cki.Key == ConsoleKey.R)
                {
                    returnLists();
                }
                cki = Console.ReadKey();
            }
        }

        /// <summary>
        /// Generates the XML documents from the lists
        /// </summary>
        public static void generateXML()
        {
            //Tests if there is anything in the lists before saving them
            if (!agents.Any() && !teams.Any())
            {
                agents.Add(new Agent());
                Console.WriteLine("Missing Agents or Teams.");
                return;
            }
            using (var sw = new StreamWriter(@"C:\agents.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<Agent>));
                serializer.Serialize(sw, agents);
            }

            if (!teams.Any())
            {
                teams.Add(new Team());
                Console.WriteLine("Missing Teams.");
            }
            using (var sw = new StreamWriter(@"C:\teams.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<Team>));
                serializer.Serialize(sw, teams);
            }

            if (!squads.Any())
            {
                squads.Add(new Squad());
                Console.WriteLine("No Squads added.");
            }
            using (var sw = new StreamWriter(@"C:\squads.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<Squad>));
                serializer.Serialize(sw, squads);
            }

            if (!actionPatterns.Any())
            {
                actionPatterns.Add(new ActionPattern());
                Console.WriteLine("No Action Patterns added.");
            }
            using (var sw = new StreamWriter(@"C:\actionPatterns.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<ActionPattern>));
                serializer.Serialize(sw, actionPatterns);
            }
        }

        public static void returnLists()
        {
            using (var sr = new StreamReader(@"C:\agents.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<Agent>));
                agents = (List<Agent>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\teams.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<Team>));
                teams = (List<Team>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\squads.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<Squad>));
                squads = (List<Squad>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\actionPatterns.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<ActionPattern>));
                actionPatterns = (List<ActionPattern>)deserializer.Deserialize(sr);
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
