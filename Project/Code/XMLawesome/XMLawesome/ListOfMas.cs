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
 

        public List<Team> TeamList(String file)
        {
            XmlReader Reader = new XmlReader(file);
            Reader.Mount();
                for (int i = 0; i < Reader.XmlSearch("MAS>Teams>Team").Count; i += 3)
                {
                    int id = Convert.ToInt32(Reader.XmlSearch("MAS>Teams>Team")[i].Value);
                    String name = Reader.XmlSearch("MAS>Teams>Team")[i + 1].Value;
                    String color = Reader.XmlSearch("MAS>Teams>Team")[i + 2].Value;
                    Team team = new Team(id, name, color);
                    ListOfTeams.Add(team);
                }
            return ListOfTeams;
        }

        public List<Squad> SquadList(String file)
        {
                XmlReader Reader = new XmlReader(file);
                Reader.Mount();
                List<XmlType> MasList = Reader.XmlSearch("MAS>Squads");
                String name = "";
                int id = 0;
                List<int> agentId = new List<int>();
                for (int i = 0; i < MasList.Count; i++)
                {
                    name = "";
                    id = 0;
                    agentId.Clear();
                    if (MasList[i].Tag == "Squad")
                    {
                        for (int j = i+1; j < MasList.Count; j++)
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
                                for (int k = j+1; k < MasList.Count; k++)
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
                return ListOfSquad;
            }

        public List<Agent> AgentList(String file)
        {
            XmlReader Reader = new XmlReader(file);
            Reader.Mount();
            List<XmlType> MasList = Reader.XmlSearch("MAS>Agents");
            for (int i = 0; i < MasList.Count; i++)
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
                    for (int j = i+1; i < MasList.Count; j++ )
                    {
                        if (MasList[j].Tag == "Id")
                        {
                            id = Convert.ToInt32(MasList[j].Value);
                        }

                        if (MasList[j].Tag == "posX")
                        {
                            posX = Convert.ToInt32(MasList[j].Value);
                        }

                        if (MasList[j].Tag == "posY")
                        {
                            posY = Convert.ToInt32(MasList[j].Value);
                        }

                        if (MasList[j].Tag == "Name")
                        {
                            name = MasList[j].Value;
                        }

                        if (MasList[j].Tag == "Rank")
                        {
                            rank = Convert.ToInt32(MasList[j].Value);
                        }

                        if (MasList[j].Tag == "Team")
                        {
                            for (int g = j; g < MasList.Count; g++)
                            {
                                if (MasList[g].Tag == "Id")
                                {
                                    teamId = Convert.ToInt32(MasList[g].Value);
                                }
                                else if (MasList[g].Tag == "Name")
                                {
                                    teamName = MasList[g].Value;
                                }
                                else if (MasList[g].Tag == "Color")
                                {
                                    teamColor = MasList[g].Value;
                                    j = MasList.Count;
                                    g = MasList.Count;
                                    i = MasList.Count;
                                }
                            }
                        }
                    }
                }
                
                Team team = new Team(teamId, teamName, teamColor);
                Agent agent = new Agent(id, name, rank, posX, posY, team);
                ListOfAgent.Add(agent);
            }
            return ListOfAgent;
        }

        public List<ActionPattern> ApList(String file)
        {
                XmlReader Reader = new XmlReader(file);
                Reader.Mount();
                List<XmlType> MasList = Reader.XmlSearch("MAS>ActionPatterns");
                for (int i = 0; i < MasList.Count; i++)
                {
                    int id = 0;
                    List<String> actions = new List<String>();
                    if (MasList[i].Tag == "ActionPattern")
                    {
                        for (int j = i+1; j < MasList.Count; j++)
                        {
                            if (MasList[j].Tag == "Id")
                            {
                                id = Convert.ToInt32(MasList[j].Value);
                            }

                            if (MasList[j].Tag == "Actions")
                            {
                                for (int k = j+1; k < MasList.Count; k++)
                                {
                                    if (MasList[k].Tag == "Action")
                                    {
                                        actions.Add(MasList[k].Value);
                                    }
                                    else
                                    {
                                        k = MasList.Count;
                                        j = MasList.Count;
                                    }
                                }
                            }
                        }
                        String[] action = actions.ToArray();
                        ActionPattern AP = new ActionPattern(id, action);
                        ListOfActionPattern.Add(AP);
                    }
                }
            return ListOfActionPattern;
            }
        }
    }
