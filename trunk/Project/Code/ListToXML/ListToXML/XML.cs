using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ListToXML
{
    public class XML
    {
        public int _depth;
        public String _input;
        public String _tag;
        public bool _standalone;
        public XML(int depth, String tag, String input, bool standalone)
        {
            this._input = input;
            this._depth = depth;
            this._tag = tag;
            this._standalone = standalone;
        }

        public static void GenerateThisShizzle(List<Agent> Agents, List<Team> Teams, List<Squad> Squads, List<ActionPattern> ActionPatterns, String XmlName)
        {
            //agents
            mAgents(Agents);
            //teams
            mTeams(Teams);
            //squads
            mSquads(Squads);
            //actionPatterns
            mActionPatterns(ActionPatterns);
            XMLhelp.End();
            

            String a = "<";
            String b = ">";
            String c = "</";
            String XML = a + XmlName + b;
            //Generate the xml string
            List<XML> stack = new List<XML>();

            for(int i = 0; i < XMLhelp.XmlList.Count; i++)
            {
                try
                {
                    //If i is the last element or _depth equals 0 and i is not 0 pop stack
                    if (i + 1 == XMLhelp.XmlList.Count || (XMLhelp.XmlList[i]._depth == 0 && i != 0))
                    {
                        for (int j = stack.Count - 1; j >= 0; j--)
                        {
                            if (stack[j]._tag != null)
                            {
                                XML += c + stack[j]._tag + b;
                            }
                        }
                        stack.Clear();
                        if (XMLhelp.XmlList[i]._tag != null)
                        {
                            XML += a + XMLhelp.XmlList[i]._tag + b;
                            stack.Add(XMLhelp.XmlList[i]);
                        }
                    } 
                        //If the depth is 0 we print first tag and insert second tag into our stack
                    else
                        if (XMLhelp.XmlList[i]._depth == 0)
                        {
                            XML += a + XMLhelp.XmlList[i]._tag + b;

                            stack.Add(XMLhelp.XmlList[i]);
                        }
                            //If i is standalone it will print first tag, value and second tag
                        else
                            if (XMLhelp.XmlList[i]._standalone == true)
                            {
                                XML += a + XMLhelp.XmlList[i]._tag + b;
                                XML += XMLhelp.XmlList[i]._input;
                                XML += c + XMLhelp.XmlList[i]._tag + b;
                            }
                            else
                            {
                                XML += a + XMLhelp.XmlList[i]._tag + b;
                                stack.Add(XMLhelp.XmlList[i]);
                            }


                    }
                catch{}
            }
            XML += c + XmlName + b;
            Console.WriteLine(XML);

            using (StreamWriter outfile = new StreamWriter(@"\WarGame.xml"))
            {
                outfile.Write(XML);
            }
        }

        public static void mAgents(List<Agent> Agents)
        {
            XMLhelp.Root("Agents", null);
            foreach (var value in Agents)
            {
                XMLhelp.Child("Agent", null);
                XMLhelp.Node("Id",value.ID.ToString());
                XMLhelp.Node("posX", value.posX.ToString());
                XMLhelp.Node("posY", value.posY.ToString());
                XMLhelp.Node("Name",value.name);
                XMLhelp.Node("Rank",value.rank.ToString());
                //Mangler at add team
                //public Team team;
                XMLhelp.Child("Teams", null);      
                XMLhelp.Child("Team",null);
                XMLhelp.Node("Id",value.team.ID.ToString());
                XMLhelp.Node("Name", value.team.name);
                XMLhelp.Node("Color", value.team.color);
               
                
            }
        }

        public static void mTeams(List<Team> Teams)
        {
            XMLhelp.Root("Teams", null);
            foreach (var value in Teams)
            {
                XMLhelp.Child("Team", null);
                XMLhelp.Node("Id",value.ID.ToString());
                XMLhelp.Node("Name",value.name);
                XMLhelp.Node("Color",value.color);
            }
        }

        public static void mSquads(List<Squad> Squads)
        {
            XMLhelp.Root("Squards", null);
            foreach (var value in Squads)
            {
                XMLhelp.Child("Squad", null);
                XMLhelp.Node("Name", value.name);
                XMLhelp.Node("Id", value.ID.ToString());
                XMLhelp.Child("Agents",null);
                foreach (int agent in value.agents)
                {
                    XMLhelp.Node("Agent", null);
                    XMLhelp.Node("Id",agent.ToString());
                }
            }
        }

        public static void mActionPatterns(List<ActionPattern> ActionPatterns)
        {
            XMLhelp.Root("ActionPatterns", null);
            foreach (var value in ActionPatterns)
            {
                XMLhelp.Child("ActionPattern", null);
                XMLhelp.Node("Id", value.ID.ToString());
                XMLhelp.Child("Actions", null);
                foreach (String action in value.actions)
                {
                    XMLhelp.Node("Action",action);
                }
            }
        }
    }
}
