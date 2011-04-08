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

            XmlReader readit = new XmlReader(@"C:\WarGame.txt");

            for(int i = 0; i <readit.getXML().Count; i++)
            {
           
                Console.WriteLine("whaaaat" + readit.getXML()[i]);
                
            }
            
            Console.WriteLine("(R)ead or (S)ave lists: ");

            DefaultXMLFileNames();


            //Initializing the lists
            Team team = new Team(1, "Team one", "Red");
            Agent agent = new Agent(1, "Olsen", 2, team);
            Squad squad = new Squad(1, "first squad", agent);
            ActionPattern aP = new ActionPattern(1, "first action pattern");

            teams.Add(team);
            agents.Add(agent);
            squads.Add(squad);
            actionPatterns.Add(aP);

            //XMLhelp.Generate(agents, teams, squads, actionPatterns);
            XML.GenerateThisShizzle(agents, teams, squads, actionPatterns, "MAS");

            //Interface to test saving and loading the xml files
            ConsoleKeyInfo cki = Console.ReadKey();

            while (cki.Key != ConsoleKey.Escape)
            {
                if (cki.Key == ConsoleKey.S)
                {
                    Console.WriteLine("");
                    generateXML();
                }
                if (cki.Key == ConsoleKey.R)
                {
                    Console.WriteLine("");
                    returnLists();
                }
                cki = Console.ReadKey();

            }

            
        }

        /// <summary>
        /// Resets the XML file names to their default names.
        /// </summary>
        public static void DefaultXMLFileNames()
        {
            _agentXML = "agents";
            _teamXML = "teams";
            _squadXML = "squads";
            _actionPatternXML = "actionPatterns";
        }

        /// <summary>
        /// Generates the XML documents from the lists
        /// </summary>
        public static void generateXML()
        {

            CheckExistingFilesWrite();

            //Tests if there is anything in the lists before saving them
            if (!agents.Any() && !teams.Any())
            {
                agents.Add(new Agent());
                Console.WriteLine("Missing Agents or Teams.");
                return;
            }
            using (var sw = new StreamWriter(AgentXML))
            {
                var serializer = new XmlSerializer(typeof(List<Agent>));
                serializer.Serialize(sw, agents);
            }

            if (!teams.Any())
            {
                teams.Add(new Team());
                Console.WriteLine("Missing Teams.");
            }
            using (var sw = new StreamWriter(TeamXML))
            {
                var serializer = new XmlSerializer(typeof(List<Team>));
                serializer.Serialize(sw, teams);
            }

            if (!squads.Any())
            {
                squads.Add(new Squad());
                Console.WriteLine("No Squads added.");
            }
            using (var sw = new StreamWriter(SquadXML))
            {
                var serializer = new XmlSerializer(typeof(List<Squad>));
                serializer.Serialize(sw, squads);
            }

            if (!actionPatterns.Any())
            {
                actionPatterns.Add(new ActionPattern());
                Console.WriteLine("No Action Patterns added.");
            }
            using (var sw = new StreamWriter(ActionPatternXML))
            {
                var serializer = new XmlSerializer(typeof(List<ActionPattern>));
                serializer.Serialize(sw, actionPatterns);
            }

            DefaultXMLFileNames();

            Console.WriteLine("XML generated.");
        }

        public static void returnLists()
        {
            CheckExistingFilesRead();

            using (var sr = new StreamReader(AgentXML))
            {
                var deserializer = new XmlSerializer(typeof(List<Agent>));
                agents = (List<Agent>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(TeamXML))
            {
                var deserializer = new XmlSerializer(typeof(List<Team>));
                teams = (List<Team>)deserializer.Deserialize(sr);
            }

            if (File.Exists(SquadXML))
            {
                using (var sr = new StreamReader(SquadXML))
                {
                    var deserializer = new XmlSerializer(typeof(List<Squad>));
                    squads = (List<Squad>)deserializer.Deserialize(sr);
                }
            }

            if (File.Exists(ActionPatternXML))
            {
                using (var sr = new StreamReader(ActionPatternXML))
                {
                    var deserializer = new XmlSerializer(typeof(List<ActionPattern>));
                    actionPatterns = (List<ActionPattern>)deserializer.Deserialize(sr);
                }
            }

            Console.WriteLine("XML read.");

            DefaultXMLFileNames();
        }

        /// <summary>
        /// Adds a suffix to the XML file names.
        /// </summary>
        /// <param name="suffix">The suffix to add.</param>
        public static void AddSuffix(string suffix)
        {
            _agentXML += suffix;
            _teamXML += suffix;
            _squadXML += suffix;
            _actionPatternXML += suffix;
        }

        /// <summary>
        /// Checks if a series of files with the current name exists, 
        /// and allows you change the name or overwrite the old ones.
        /// </summary>
        public static void CheckExistingFilesWrite()
        {
            if (File.Exists(AgentXML) || File.Exists(TeamXML) 
                || File.Exists(SquadXML) || File.Exists(ActionPatternXML))
            {
                Console.WriteLine("An older series of files already exist with this name. \n " +
                "Would you like to (C)hange the suffix of your series or (O)verwrite the default one?");

                ConsoleKeyInfo cki = Console.ReadKey();

                while (true)
                {
                    if (cki.Key == ConsoleKey.C)
                    {
                        Console.WriteLine("\n Please write the suffix you wish to use:");
                        DefaultXMLFileNames();
                        AddSuffix(Console.ReadLine());
                        CheckExistingFilesWrite();
                        break;
                    }
                    if (cki.Key == ConsoleKey.O)
                    {
                        Console.WriteLine("");
                        break;
                    }
                    cki = Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Asks you what suffix the files you want have.
        /// </summary>
        public static void CheckExistingFilesRead()
        {
            Console.WriteLine("Type the suffix of file series you would like to read from: \n " +
                "(leave blank for default file names)");
            AddSuffix(Console.ReadLine());
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
