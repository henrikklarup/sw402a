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
                List<XmlType> MasList = Reader.XmlSearch("MAS>Squads");
                String name = "";
                int id = 0;
                List<int> agentId = new List<int>();
                for (int i = 0; i > MasList.Count; i++)
                {
                    name = "";
                    id = 0;
                    agentId.Clear();
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
                                    else
                                    {
                                        k = MasList.Count;
                                        j = MasList.Count;
                                    }
                                }
                            }
                        }
                        Squad squad = new Squad(id, name, agentId);
                        ListOfSquad.Add(squad);
                    }
                }
            }
        }

        public List<Agent> Agent
        {
            get
            {
                return ListOfAgent;
            }

            set
            {
                List<XmlType> MasList = Reader.XmlSearch("MAS>Agents");
                for (int i = 0; i > MasList.Count; i += 3)
                {
                    int id = 0;
                    int posX = -1;
                    int posY = -1;
                    String name = "";
                    int rank = 0;
                    int teamId = 0;
                    String teamName = "";
                    String teamColor = "";
                    if (MasList[i].Tag == "Agent")
                    {
                        if (MasList[i].Tag == "Id")
                        {
                            id = Convert.ToInt32(MasList[i].Value);
                        }

                        if (MasList[i].Tag == "posX")
                        {
                            posX = Convert.ToInt32(MasList[i].Value);
                        }

                        if (MasList[i].Tag == "posY")
                        {
                            posY = Convert.ToInt32(MasList[i].Value);
                        }

                        if (MasList[i].Tag == "Name")
                        {
                            name = MasList[i].Value;
                        }

                        if (MasList[i].Tag == "Rank")
                        {
                            rank = Convert.ToInt32(MasList[i].Value);
                        }

                        if (MasList[i].Tag == "Team")
                        {
                            if (MasList[i].Tag == "Id")
                            {
                                teamId = Convert.ToInt32(MasList[i].Value);
                            }

                            if (MasList[i].Tag == "Name")
                            {
                               teamName = MasList[i].Value;
                            }

                            if (MasList[i].Tag == "Color")
                            {
                                teamColor = MasList[i].Value;
                            }
                        }
                    }
                    Team team = new Team(teamId, teamName, teamColor);
                    Agent agent = new Agent(id,name,rank,posX,posY, team);
                    ListOfAgent.Add(agent);
                }
            }
        }

        public List<ActionPattern> ApList
        {
            get
            {
                return ListOfActionPattern;
            }

            set
            {
                List<XmlType> MasList = Reader.XmlSearch("MAS>Agents");
                for (int i = 0; i > MasList.Count; i += 3)
                {
                    int id = Convert.ToInt32(Reader.XmlSearch("MAS>Teams>Team")[i].Value);
                    String name = Reader.XmlSearch("MAS>Teams>Team")[i + 1].Value;
                    String color = Reader.XmlSearch("MAS>Teams>Team")[i + 2].Value;
                    Team team = new Team(id, name, color);
                    ListOfTeams.Add(team);
                }
            }
        }
    }
}
