﻿using System;
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
        static void Main(string[] args)
        {
            Console.WriteLine("(R)ead or (S)ave lists: ");

            //Initializing the lists
            Lists.agents = new List<agent>();
            Lists.teams = new List<team>();
            Lists.squads = new List<squad>();
            Lists.actionPatterns = new List<actionpattern>();

            team _team = new team(1, "Team one", "Red");
            agent _agent = new agent(1, "Olsen", 2, _team);
            squad _squad = new squad("first squad");
            _squad.Agents.Add(_agent);
            actionpattern aP = new actionpattern(1, "first action pattern");

            Lists.teams.Add(_team);
            Lists.agents.Add(_agent);
            Lists.actionPatterns.Add(aP);
            Lists.squads.Add(_squad);

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
                    Console.WriteLine("Read");
                }
                cki = Console.ReadKey();
            }
        }

        /// <summary>
        /// Generates the XML documents from the lists.
        /// </summary>
        public static void generateXML()
        {            
            List<oldSquad> oldSquads = new List<oldSquad>();
            List<oldActionPattern> oldActionPatterns = new List<oldActionPattern>();

            //Tests if there is anything in the lists before saving them
            if (!Lists.agents.Any() && !Lists.teams.Any())
            {
                Lists.agents.Add(new agent());
                Console.WriteLine("Missing Agents or Teams.");
                return;
            }
            using (var sw = new StreamWriter(@"C:\agents.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<agent>));
                serializer.Serialize(sw, Lists.agents);
            }

            if (!Lists.teams.Any())
            {
                Lists.teams.Add(new team());
                Console.WriteLine("Missing Teams.");
            }
            using (var sw = new StreamWriter(@"C:\teams.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<team>));
                serializer.Serialize(sw, Lists.teams);
            }

            if (!Lists.squads.Any())
            {
                Lists.squads.Add(new squad());
                Console.WriteLine("No Squads added.");
            }
            foreach (squad s in Lists.squads)
            {
                oldSquad os = new oldSquad();
                int i = 0;
                os.agents = new int[s.Agents.Count];
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

            if (!Lists.actionPatterns.Any())
            {
                Lists.actionPatterns.Add(new actionpattern());
                Console.WriteLine("No Action Patterns added.");
                
            }
            foreach (actionpattern ap in Lists.actionPatterns)
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

        /// <summary>
        /// Loads the XML documents to the lists.
        /// </summary>
        public static void returnLists()
        {
            List<oldSquad> oldSquads = new List<oldSquad>();
            List<oldActionPattern> oldActionPatterns = new List<oldActionPattern>();
            using (var sr = new StreamReader(@"C:\agents.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<agent>));
                Lists.agents = (List<agent>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\teams.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<team>));
                Lists.teams = (List<team>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\squads.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<oldSquad>));
                oldSquads = (List<oldSquad>)deserializer.Deserialize(sr);
                foreach (oldSquad os in oldSquads)
                {
                    squad s = new squad(os.name);
                    foreach (int i in os.agents)
                    {
                        s.Agents.Add(Lists.Retrieveagent(i));
                    }
                }
            }

            using (var sr = new StreamReader(@"C:\actionPatterns.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<oldActionPattern>));
                oldActionPatterns = (List<oldActionPattern>)deserializer.Deserialize(sr);
                foreach (oldActionPattern oap in oldActionPatterns)
                {
                    actionpattern ap = new actionpattern();
                    ap.ID = oap.ID;
                    ap.name = oap.name;
                    ap.actions = oap.actions.ToList();
                }
            }
        }

    }
}
