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
                using (var fs = new FileStream(path + @"\agents.xml", FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(List<agent>));
                    serializer.Serialize(fs, Lists.agents);
                    fs.Flush();
                    fs.Close();
                }
            }

            if (Lists.teams.Any())
            {
                using (var fs = new FileStream(path + @"\teams.xml", FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(List<team>));
                    serializer.Serialize(fs, Lists.teams);
                    fs.Flush();
                    fs.Close();
                }
            }

            if (Lists.squads.Any())
            {
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

                using (var fs = new FileStream(path + @"\squads.xml", FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(List<oldSquad>));
                    serializer.Serialize(fs, oldSquads);
                    fs.Flush();
                    fs.Close();
                }
            }

            if (!Lists.actionPatterns.Any())
            {
                foreach (actionpattern ap in Lists.actionPatterns)
                {
                    oldActionPattern oap = new oldActionPattern();
                    oap.actions = ap.actions.ToArray();
                    oap.name = ap.name;
                    oldActionPatterns.Add(oap);
                }
                using (var fs = new FileStream(path + @"\actionPatterns.xml", FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(List<oldActionPattern>));
                    serializer.Serialize(fs, oldActionPatterns);
                    fs.Flush();
                    fs.Close();
                }
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

            if (Directory.Exists(path + @"\agents.xml"))
            {
                using (var fs = new FileStream(path + @"\agents.xml", FileMode.Open))
                {
                    var deserializer = new XmlSerializer(typeof(List<agent>));
                    Lists.agents = (List<agent>)deserializer.Deserialize(fs);
                }
            }

            if (Directory.Exists(path + @"\teams.xml"))
            {
                using (var fs = new FileStream(path + @"\teams.xml", FileMode.Open))
                {
                    var deserializer = new XmlSerializer(typeof(List<team>));
                    Lists.teams = (List<team>)deserializer.Deserialize(fs);
                }
            }

            if (Directory.Exists(path + @"\squads.xml"))
            {
                using (var fs = new FileStream(path + @"\squads.xml", FileMode.Open))
                {
                    var deserializer = new XmlSerializer(typeof(List<oldSquad>));
                    oldSquads = (List<oldSquad>)deserializer.Deserialize(fs);
                    foreach (oldSquad os in oldSquads)
                    {
                        squad s = new squad(os.name);
                        foreach (int i in os.agents)
                        {
                            s.Agents.Add(Lists.Retrieveagent(i));
                        }
                    }
                }
            }

            if (Directory.Exists(path + @"\actionPatterns.xml"))
            {
                using (var fs = new FileStream(path + @"\actionPatterns.xml", FileMode.Open))
                {
                    var deserializer = new XmlSerializer(typeof(List<oldActionPattern>));
                    oldActionPatterns = (List<oldActionPattern>)deserializer.Deserialize(fs);
                    foreach (oldActionPattern oap in oldActionPatterns)
                    {
                        actionpattern ap = new actionpattern(oap.name, oap.actions.ToList());
                    }
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
