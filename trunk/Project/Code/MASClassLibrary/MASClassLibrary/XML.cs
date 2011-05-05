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
            // Initialize the lists.
            List<oldTeam> oldTeams = new List<oldTeam>();
            List<oldSquad> oldSquads = new List<oldSquad>();
            List<oldActionPattern> oldActionPatterns = new List<oldActionPattern>();

            //Tests if there is anything in the lists before saving them
            if (Lists.agents.Any() && Lists.teams.Any())
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
                foreach (team t in Lists.teams)
                { 
                    oldTeam ot = new oldTeam();
                    ot.id = t.id;
                    ot.name = t.name;
                    ot.color = "#" + t.color.Name.Remove(0,2).ToUpper();

                    oldTeams.Add(ot);
                }

                using (var fs = new FileStream(path + @"\teams.xml", FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(List<oldTeam>));
                    serializer.Serialize(fs, oldTeams);
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

            if (Lists.actionPatterns.Any())
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
            List<oldTeam> oldTeams = new List<oldTeam>();
            List<oldSquad> oldSquads = new List<oldSquad>();
            List<oldActionPattern> oldActionPatterns = new List<oldActionPattern>();

            if (File.Exists(path + @"\teams.xml"))
            {
                using (var fs = new FileStream(path + @"\teams.xml", FileMode.Open))
                {
                    var deserializer = new XmlSerializer(typeof(List<oldTeam>));
                    oldTeams = (List<oldTeam>)deserializer.Deserialize(fs);
                    foreach (oldTeam ot in oldTeams)
                    {
                        new team(ot.name, ot.color);
                    }
                }
            }

            if (File.Exists(path + @"\agents.xml"))
            {
                using (var fs = new FileStream(path + @"\agents.xml", FileMode.Open))
                {
                    var deserializer = new XmlSerializer(typeof(List<agent>));
                    Lists.agents = (List<agent>)deserializer.Deserialize(fs);
                }
                foreach (agent a in Lists.agents)
                {
                    a.team = Lists.Retrieveteam(a.team.name);
                }
            }

            if (File.Exists(path + @"\squads.xml"))
            {
                using (var fs = new FileStream(path + @"\squads.xml", FileMode.Open))
                {
                    var deserializer = new XmlSerializer(typeof(List<oldSquad>));
                    oldSquads = (List<oldSquad>)deserializer.Deserialize(fs);
                }
                foreach (oldSquad os in oldSquads)
                {
                    squad s = new squad(os.name);
                    foreach (int i in os.agents)
                    {
                        s.Agents.Add(Lists.Retrieveagent(i));
                    }
                }
            }

            if (File.Exists(path + @"\actionPatterns.xml"))
            {
                using (var fs = new FileStream(path + @"\actionPatterns.xml", FileMode.Open))
                {
                    var deserializer = new XmlSerializer(typeof(List<oldActionPattern>));
                    oldActionPatterns = (List<oldActionPattern>)deserializer.Deserialize(fs);
                }
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
    }

    public class oldSquad
    {
        public int ID;
        public string name;
        public int[] agents;

        public oldSquad()
        { }
    }

    public class oldTeam
    {
        public int id;
        public string name;
        public string color;

        public oldTeam()
        { }
    }
}
