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


        public void Init(String file, bool print)
        {
            Lists.teams = new List<team>();
            Lists.agents = new List<agent>();
            Lists.squads = new List<squad>();
            Lists.actionPatterns = new List<actionpattern>();
            ApList(file);
            TeamList(file);
            AgentList(file);
            SquadList(file);

            if (print == true)
            {
                Console.WriteLine("\n\nPrint All lists\n");
                foreach (actionpattern ap in Lists.actionPatterns)
                {
                    Console.WriteLine("ActionPattern name: " + ap.name);
                    foreach (String ac in ap.actions)
                    {
                        Console.WriteLine("ActionPattern Actions: " + ac);
                    }
                }

                foreach (team ap in Lists.teams)
                {
                    Console.WriteLine("Team Name: " + ap.name);
                    Console.WriteLine("Team Color: " + ap.colorStr);
                }

                foreach (agent ap in Lists.agents)
                {
                    Console.WriteLine("Agent Name: " + ap.name);
                    Console.WriteLine("Agent Rank: " + ap.rank);
                    Console.WriteLine("Agent Team Name: " + ap.team.name);
                    Console.WriteLine("Agent Team Color: " + ap.team.colorStr);
                }

                foreach (squad ap in Lists.squads)
                {
                    Console.WriteLine("Squad name: " + ap.name);
                    foreach (agent ac in ap.Agents)
                    {
                        Console.WriteLine("Squad agent name: " + ac.name);
                        Console.WriteLine("Squad agent rank: " + ac.rank);
                        Console.WriteLine("Squad agent team name: " + ac.team.name);
                        Console.WriteLine("Squad agent team color: " + ac.team.colorStr);
                    }
                }
            }

        }


        public void TeamList(String file)
        {
            XmlReader Reader = new XmlReader(file);
            Reader.Mount();
            List<XmlType> MasList = Reader.XmlSearch("MAS>Teams>Team");
            if (MasList.Count > 0)
            {
                for (int i = 0; i < MasList.Count; i += 2)
                {
                    String name = MasList[i].Value;
                    String color = MasList[i + 1].Value;
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
            List<agent> agentId = new List<agent>();
            if (MasList.Count > 0)
            {
                for (int i = 0; i < MasList.Count; i++)
                {
                    
                    if (MasList[i].Tag == "Squad")
                    {
                        String sname = "";
                        List<agent> agentList = new List<agent>();
                        for(int k = i+1; k < MasList.Count; k++)
                        {
                            if(MasList[k].Tag == "AgentName")
                            {
                                agentList.Add(Lists.agents.First(x => x.name == MasList[k].Value));
                            } else if (MasList[k].Tag == "Name")
                            {
                                sname = MasList[k].Value;
                                i = i + k;
                                k = MasList.Count;
                            }
                        }
                        
                        squad squad = new squad(sname, agentList);
                    }
            }
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
                        i++;


                            if (MasList[i].Tag == "Name")
                            {
                                name = MasList[i].Value;
                                i++;
                            }

                            if (MasList[i].Tag == "Rank")
                            {
                                rank = Convert.ToInt32(MasList[i].Value);
                                i++;
                            }

                            if (MasList[i].Tag == "Team")
                            {
                                i++;
                                    if (MasList[i].Tag == "Name")
                                    {
                                        teamName = MasList[i].Value;
                                        i++;
                                    }
                                    if (MasList[i].Tag == "Color")
                                    {
                                        teamColor = MasList[i].Value;
                                    }
                            }
                            //team team = new team(teamName, teamColor);
                            agent agent = new agent(name, rank, Lists.teams.First(x => x.name == teamName && x.colorStr == teamColor));
                            //Lists.agents.Add(agent);
                        }
                        
                    }

                }
                //return Lists.agents;
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
                            if (MasList[j].Tag == "Action")
                            {
                                actions.Add(MasList[j].Value);
                            }

                            if (MasList[j].Tag == "Name")
                            {
                                name = MasList[j].Value;
                                i = i + j;
                                j = MasList.Count;
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
