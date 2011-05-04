using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using MASClassLibrary;

namespace ListToXML
{
    class Program
    {
        public static List<agent> agents = new List<agent>();
        public static List<team> teams = new List<team>();
        public static List<squad> squads = new List<squad>();
        public static List<actionpattern> actionPatterns = new List<actionpattern>();
        public static List<oldSquad> oldSquads = new List<oldSquad>();
        public static List<oldActionPattern> oldActionPatterns = new List<oldActionPattern>();

        static void Main(string[] args)
        {
            Console.WriteLine("(R)ead or (S)ave lists: ");


            //Initializing the lists
            team _team = new team(1, "Team one", "Red");
            agent _agent = new agent(1, "Olsen", 2, _team);
            squad _squad = new squad("first squad");
            _squad.Agents.Add(_agent);
            actionpattern aP = new actionpattern(1, "first action pattern");

            teams.Add(_team);
            agents.Add(_agent);
            actionPatterns.Add(aP);
            squads.Add(_squad);

            //Interface to test saving and loading the xml files
            ConsoleKeyInfo cki = Console.ReadKey();

            while (cki.Key != ConsoleKey.Escape)
            {
                if (cki.Key == ConsoleKey.S)
                {
                    generateXML();
                    Console.WriteLine("Saved");
                }
                if (cki.Key == ConsoleKey.R)
                {
                    returnLists();
                    Console.WriteLine("Loaded");
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
                agents.Add(new agent());
                Console.WriteLine("Missing Agents or Teams.");
                return;
            }
            using (var sw = new StreamWriter(@"C:\agents.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<agent>));
                serializer.Serialize(sw, agents);
            }

            if (!teams.Any())
            {
                teams.Add(new team());
                Console.WriteLine("Missing Teams.");
            }
            using (var sw = new StreamWriter(@"C:\teams.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<team>));
                serializer.Serialize(sw, teams);
            }

            if (!squads.Any())
            {
                squads.Add(new squad());
                Console.WriteLine("No Squads added.");
            }
            foreach (squad s in squads)
            {
                oldSquad os = new oldSquad();
                int i = 0;
                foreach (agent a in s.Agents)
                {
                    os.agents[i] = a.ID;
                    i++;
                }
                os.ID = s.id;
                os.name = s.name;
                oldSquads.Add(os);

            }
            using (var sw = new StreamWriter(@"C:\squads.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<oldSquad>));
                serializer.Serialize(sw, oldSquads);
            }

            if (!actionPatterns.Any())
            {
                actionPatterns.Add(new actionpattern());
                Console.WriteLine("No Action Patterns added.");
                
            }
            foreach (actionpattern ap in actionPatterns)
            { 
                oldActionPattern oap = new oldActionPattern();
                oap.actions = ap.actions.ToArray();
                oap.ID = ap.ID;
                oap.name = ap.name;
                oldActionPatterns.Add(oap);
            }
            using (var sw = new StreamWriter(@"C:\actionPatterns.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<oldActionPattern>));
                serializer.Serialize(sw, oldActionPatterns);
            }
        }

        public static void returnLists()
        {
            using (var sr = new StreamReader(@"C:\agents.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<agent>));
                agents = (List<agent>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\teams.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<team>));
                teams = (List<team>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\squads.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<squad>));
                squads = (List<squad>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\actionPatterns.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<actionpattern>));
                actionPatterns = (List<actionpattern>)deserializer.Deserialize(sr);
            }
        }

    }
}
