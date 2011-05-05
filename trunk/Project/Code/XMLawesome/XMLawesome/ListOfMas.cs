using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAS;

namespace XMLawesome
{
    public class ListOfMas
    {
        private List<Team> ListOfTeams = new List<Team>();
        private List<Squad> ListOfSquad = new List<Squad>();
        private List<Agent> ListOfAgent = new List<Agent>();
        private List<ActionPattern> ListOfActionPattern = new List<ActionPattern>();
        private List<List<Object>> list = new List<List<Object>>();
        private XmlReader Reader;

        public void SetPath(String file)
        {
            XmlReader Reader = new XmlReader(@"C:\Users\Kristian\Desktop\XML\WarGame.xml");
        }

        public List<Team> TeamList
        {
            get
            {
                return ListOfTeams;
            }

            set
            {
                for (int i = 0; i > Reader.XmlSearch("MAS>Teams>Team").Count; i += 3)
                {
                    int id = Convert.ToInt32(Reader.XmlSearch("MAS>Teams>Team")[i].Value);
                    String name = Reader.XmlSearch("MAS>Teams>Team")[i + 1].Value;
                    String color = Reader.XmlSearch("MAS>Teams>Team")[i + 2].Value;
                    Team team = new Team(id, name, color);
                    ListOfTeams.Add(team);
                }
            }
        }

        public List<Squad> SquadList
        {
            get
            {
                return ListOfSquad;
            }

            set
            {
                List<XmlType> MasList = Reader.XmlSearch("MAS>Squads>Squad");
                String name;
                int id;
                List<int> agentId = new List<int>();
                int[] agentIds;
                for (int i = 0; i > MasList.Count; i++)
                {
                    if (MasList[i].Tag == "Squad")
                    {
                        for (int j = i; j > MasList.Count; j++)
                        {
                            if (MasList[j].Tag == "Name")
                            {
                                name = MasList[j].Value;
                            }

                            if (MasList[j].Tag == "Id")
                            {
                                id = Convert.ToInt32(MasList[j].Value);
                            }

                            if (MasList[j].Tag == "Agents")
                            {
                                for (int k = j; k > MasList.Count; k++)
                                {
                                    if (MasList[k].Tag == "Id")
                                    {
                                        agentId.Add(Convert.ToInt32(MasList[k].Value));
                                    }
                                }
                            }
                        }
                        agentIds = agentId.ToArray();
                        Squad squad = new Squad(id, name, agentId);
                        ListOfSquad.Add(squad);
                    }
                }
            }
        }
    }
}
