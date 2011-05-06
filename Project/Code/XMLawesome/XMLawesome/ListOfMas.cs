using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace XMLawesome
{
    public class ListOfMas
    {

        private List<List<Object>> list = new List<List<Object>>();


        public void Init(String file)
        {
            Lists.agents = new List<agent>();
            Lists.teams = new List<team>();
            Lists.squads = new List<squad>();
            Lists.actionPatterns = new List<actionpattern>();
            ApList(file);
            TeamList(file);
            AgentList(file);
            SquadList(file);
        }


        public void TeamList(String file)
        {
            XmlReader Reader = new XmlReader(file);
            Reader.Mount();
            if (Reader.XmlSearch("MAS>Teams>Team").Count > 0)
            {
                for (int i = 0; i < Reader.XmlSearch("MAS>Teams>Team").Count; i += 2)
                {
                    String name = Reader.XmlSearch("MAS>Teams>Team")[i].Value;
                    String color = Reader.XmlSearch("MAS>Teams>Team")[i + 1].Value;
                    team team = new team(name, color);
                    //Lists.teams.Add(team);
                }
            }
            //return Lists.teams;
        }

        public void SquadList(String file)
        {
            XmlReader Reader = new XmlReader(file);
            Reader.Mount();
            List<XmlType> MasList = Reader.XmlSearch("MAS>Squads");
            String name = "";
            List<agent> agentId = new List<agent>();
            if (MasList.Count > 0)
            {
                for (int i = 0; i < MasList.Count; i++)
                {
                    name = "";
                    agentId.Clear();
                    if (MasList[i].Tag == "Squad")
                    {
                        for (int j = i + 1; j < MasList.Count; j++)
                        {
                            if (MasList[j].Tag == "Name")
                            {
                                name = MasList[j].Value;
                            }

                            if (MasList[j].Tag == "Agents")
                            {
                                for (int k = j + 1; k < MasList.Count; k++)
                                {
                                    String aname = "";
                                    int arank = 0;
                                    String tcolor = "";
                                    String tname = "";
                                    if (MasList[k].Tag == "Name")
                                    {
                                        aname = MasList[k].Value;
                                    }
                                    else if (MasList[k].Tag == "Rank")
                                    {
                                        arank = Convert.ToInt32(MasList[k].Value);
                                    }
                                    else if (MasList[k].Tag == "Team")
                                    {
                                        if (MasList[k + 1].Tag == "Color")
                                        {
                                            tcolor = MasList[k].Value;
                                        }
                                        else if (MasList[k + 2].Tag == "Name")
                                        {
                                            tname = MasList[k].Value;
                                        }
                                    }
                                    else
                                    {
                                        k = MasList.Count;
                                        j = MasList.Count;
                                    }
                                    agentId.Add(Lists.agents.First(x => x.name == aname && x.rank == arank));
                                }
                            }
                        }
                        squad squad = new squad(name, agentId);
                        //Lists.squads.Add(squad);
                    }
                }
                //return Lists.squads;
            }
        }

        public void AgentList(String file)
        {
            XmlReader Reader = new XmlReader(file);
            Reader.Mount();
            List<XmlType> MasList = Reader.XmlSearch("MAS>Agents");
            if (MasList.Count > 0)
            {
                for (int i = 0; i < MasList.Count; i++)
                {
                    String name = "";
                    int rank = 0;
                    String teamName = "";
                    String teamColor = "";

                    if (MasList[i].Tag == "Agent")
                    {
                        for (int j = i + 1; i < MasList.Count; j++)
                        {

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
                                    if (MasList[g].Tag == "Name")
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

                    //team team = new team(teamName, teamColor);
                    agent agent = new agent(name, rank, Lists.teams.First(x => x.name == teamName && x.colorStr == teamColor));
                    //Lists.agents.Add(agent);
                }
                //return Lists.agents;
            }
        }

        public void ApList(String file)
        {
            XmlReader Reader = new XmlReader(file);
            Reader.Mount();
            List<XmlType> MasList = Reader.XmlSearch("MAS>ActionPatterns");
            if (MasList.Count > 0)
            {
                for (int i = 0; i < MasList.Count; i++)
                {
                    String name = "";
                    List<String> actions = new List<String>();
                    if (MasList[i].Tag == "ActionPattern")
                    {
                        for (int j = i + 1; j < MasList.Count; j++)
                        {
                            if (MasList[j].Tag == "Name")
                            {
                                name = MasList[j].Value;
                            }

                            if (MasList[j].Tag == "Actions")
                            {
                                for (int k = j + 1; k < MasList.Count; k++)
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
                        actionpattern AP = new actionpattern(name, actions);
                        //Lists.actionPatterns.Add(AP);
                    }
                }
                //return Lists.actionPatterns;
            }
        }
    }
}
