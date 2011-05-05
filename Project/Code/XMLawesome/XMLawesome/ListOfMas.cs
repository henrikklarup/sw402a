using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace XMLawesome
{
    public class ListOfMas
    {
        private List<team> ListOfTeams = new List<team>();
        private List<team> ListOfSquad = new List<team>();
        private List<agent> ListOfAgent = new List<agent>();
        private List<actionpattern> ListOfActionPattern = new List<actionpattern>();
        private List<List<Object>> list = new List<List<Object>>();
 

        public List<team> TeamList(String file)
        {
            XmlReader Reader = new XmlReader(file);
            Reader.Mount();
                for (int i = 0; i < Reader.XmlSearch("MAS>Teams>Team").Count; i += 3)
                {
                    int id = Convert.ToInt32(Reader.XmlSearch("MAS>Teams>Team")[i].Value);
                    String name = Reader.XmlSearch("MAS>Teams>Team")[i + 1].Value;
                    String color = Reader.XmlSearch("MAS>Teams>Team")[i + 2].Value;
                    team team = new team(id, name, color);
                    ListOfTeams.Add(team);
                }
            return ListOfTeams;
        }

        public List<squad> SquadList(String file)
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
                        squad squad = new squad(id, name, agentId);
                        ListOfSquad.Add(squad);
                    }
                }
                return ListOfSquad;
            }

        public List<agent> AgentList(String file)
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
                
                team team = new team(teamId, teamName, teamColor);
                agent agent = new agent(id, name, rank, posX, posY, team);
                ListOfAgent.Add(agent);
            }
            return ListOfAgent;
        }

        public List<actionpattern> ApList(String file)
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
                        actionpattern AP = new actionpattern(id, action);
                        ListOfActionPattern.Add(AP);
                    }
                }
            return ListOfActionPattern;
            }
        }
    }
