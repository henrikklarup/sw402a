using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace MASClassLibrary
{
    public static class XML
    {
        /// <summary>
        /// Generates the XML documents from the lists.
        /// </summary>
        /// <param name="path">The path the files should be stored to.</param>
        public static void generateXML(string path)
        {
            List<oldSquad> oldSquads = new List<oldSquad>();
            List<oldActionPattern> oldActionPatterns = new List<oldActionPattern>();

            //Tests if there is anything in the lists before saving them
            if (!Lists.agents.Any() && !Lists.teams.Any())
            {
                Lists.agents.Add(new agent());
                return;
            }
            using (var sw = new StreamWriter(path + @"\agents.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<agent>));
                serializer.Serialize(sw, Lists.agents);
            }

            if (!Lists.teams.Any())
            {
                Lists.teams.Add(new team());
            }
            using (var sw = new StreamWriter(path + @"\teams.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<team>));
                serializer.Serialize(sw, Lists.teams);
            }

            if (!Lists.squads.Any())
            {
                Lists.squads.Add(new squad());
            }
            foreach (squad s in Lists.squads)
            {
                oldSquad os = new oldSquad();
                int i = 0;
                os.agents = new int[s.Agents.Count];
                foreach (agent a in s.Agents)
                {
                    os.agents[i] = a.id;
                    i++;
                }
                os.ID = s.id;
                os.name = s.name;
                oldSquads.Add(os);

            }
            using (var sw = new StreamWriter(path + @"\squads.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<oldSquad>));
                serializer.Serialize(sw, oldSquads);
            }

            if (!Lists.actionPatterns.Any())
            {
                Lists.actionPatterns.Add(new actionpattern());

            }
            foreach (actionpattern ap in Lists.actionPatterns)
            {
                oldActionPattern oap = new oldActionPattern();
                oap.actions = ap.actions.ToArray();
                oap.name = ap.name;
                oldActionPatterns.Add(oap);
            }
            using (var sw = new StreamWriter(path + @"\actionPatterns.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<oldActionPattern>));
                serializer.Serialize(sw, oldActionPatterns);
            }
        }

        /// <summary>
        /// Loads the XML documents to the lists.
        /// </summary>
        /// /// <param name="path">The path the files should be loaded from.</param>
        public static void returnLists(string path)
        {
            List<oldSquad> oldSquads = new List<oldSquad>();
            List<oldActionPattern> oldActionPatterns = new List<oldActionPattern>();
            
            using (var sr = new StreamReader(path + @"\agents.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<agent>));
                Lists.agents = (List<agent>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(path + @"\teams.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<team>));
                Lists.teams = (List<team>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(path + @"\squads.xml"))
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

            using (var sr = new StreamReader(path + @"\actionPatterns.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<oldActionPattern>));
                oldActionPatterns = (List<oldActionPattern>)deserializer.Deserialize(sr);
                foreach (oldActionPattern oap in oldActionPatterns)
                {
                    actionpattern ap = new actionpattern(oap.name, oap.actions.ToList());
                }
            }
        }
    }

    public class oldActionPattern
    {
        public int ID;
        public string[] actions;
        public string name;

        public oldActionPattern()
        { }

        public oldActionPattern(int ID, string[] actions)
        {
            this.ID = ID;
            this.actions = actions;
        }

        public oldActionPattern(int ID, string action)
        {
            string[] actionString = new string[1];
            actionString[0] = action;

            this.ID = ID;
            this.actions = actionString;
        }
    }

    public class oldSquad
    {
        public int ID;
        public string name;
        public int[] agents;

        public oldSquad()
        { }

        public oldSquad(int ID, string name, List<agent> agents)
        {
            this.ID = ID;
            this.name = name;

            int i = 0;
            foreach (agent a in agents)
            {
                this.agents[i] = a.id;
                i++;
            }
        }

        public oldSquad(int ID, string name, agent agent)
        {
            int[] agentArray = new int[1];
            agentArray[0] = agent.id;

            this.ID = ID;
            this.name = name;
            this.agents = agentArray;
        }
    }
}
